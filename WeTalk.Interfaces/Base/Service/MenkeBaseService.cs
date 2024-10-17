using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Models;
using WeTalk.Models.Dto.Menke;

namespace WeTalk.Interfaces.Base
{
	public partial class MenkeBaseService : BaseService, IMenkeBaseService
	{
		private readonly SqlSugarScope _context;
		private readonly ILogger<MenkeBaseService> _logger;
		private readonly IMessageBaseService _messageBaseService;
		private readonly ICommonBaseService _commonBaseService;
        private readonly IUserManage _userManage;

        public MenkeBaseService(SqlSugarScope dbcontext, ILogger<MenkeBaseService> logger, IMessageBaseService messageBaseService, IUserManage userManage, ICommonBaseService commonBaseService)
		{
			_context = dbcontext;
			_logger = logger;
			_messageBaseService = messageBaseService;
			_commonBaseService = commonBaseService;
            _userManage = userManage;
        }
		#region 同步课程信息
		/// <summary>
		/// 同步课程信息
		/// </summary>
		/// <param name="starttime">开始时间戳</param>
		/// <returns></returns>
		public async Task<ApiResult> CourseSync(string starttime)
		{
			var result = new ApiResult();
			var list_MenkeCourse = new List<MenkeCourse>();
			int page = 1, last_page = 1;
            if (!string.IsNullOrEmpty(starttime)) starttime = (int.Parse(starttime) - 3600).ToString();//重叠1小时
            string update_starttime = starttime;
			string update_endtime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow).ToString();

			string url = Appsettings.app("APIS:menke_course");

            var dic_header = new Dictionary<string, string>();
            dic_header.Add("key", Appsettings.app("Web:MenkeKey"));
            dic_header.Add("version", "v1");

            var dic_data = new Dictionary<string, object>();
			do
            {
				dic_data.Clear();
                dic_data.Add("page", page);
                if (!string.IsNullOrEmpty(update_starttime)) dic_data.Add("update_starttime", update_starttime);
                if (!string.IsNullOrEmpty(update_endtime)) dic_data.Add("update_endtime", update_endtime);
                var data = string.Join("&", dic_data.Select(u => u.Key + "=" + u.Value).ToList());
                var json = GetRemoteHelper.HttpWebRequestUrl(url + "?" + data, "", "utf-8", "get", dic_header);
				var o = JObject.Parse(json);
				if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
				{
					last_page = (o["data"]["last_page"] != null) ? (int)o["data"]["last_page"] : 1;
					if (o["data"]["data"] != null)
					{
						var jarray = (JArray)o["data"]["data"];
						foreach (var item in jarray)
						{
							list_MenkeCourse.Add(new MenkeCourse()
							{
								MenkeCourseId = (item["id"] != null) ? (int)item["id"] : 0,
								MenkeName = (item["name"] != null) ? item["name"].ToString() : "",
								MenkeIntro = (item["intro"] != null) ? item["intro"].ToString() : "",
								MenkeUpdateTime = (item["update_time"] != null) ? (int)item["update_time"] : 0,
								MenkeFirstStartTime = (item["first_start_time"] != null) ? (int)item["first_start_time"] : 0,
								MenkeLastestEndTime = (item["latest_end_time"] != null) ? (int)item["latest_end_time"] : 0,
								MenkeDeleteTime = (item["delete_time"] != null) ? (int)item["delete_time"] : 0,
								MenkeMsg = (item["msg"] != null) ? item["msg"].ToString() : "",
								Status = (item["delete_time"] != null && ((int)item["delete_time"]) > 0) ? -1 : 1,
								Istrial = (item["name"] != null && (item["name"] + "").Contains("试听")) ? 1 : 0,
								Json = json
							});
						}
					}
					if (list_MenkeCourse.Count > 0)
					{
						var x = _context.Storageable(list_MenkeCourse).WhereColumns(u => u.MenkeCourseId).ToStorage();
						x.AsInsertable.ExecuteCommand();
						x.AsUpdateable.UpdateColumns(u=>new { u.MenkeCourseId,u.MenkeName,u.MenkeIntro,u.MenkeUpdateTime,u.MenkeFirstStartTime,u.MenkeLastestEndTime,u.MenkeDeleteTime,u.MenkeMsg,u.Status,u.Istrial,u.Json}).ExecuteCommand();
					}
					#region "拓课云删除课程，同步锁定购课信息,删除已排课节"
					var list_DelMenkeCourse = list_MenkeCourse.Where(u => u.MenkeDeleteTime > 0).ToList();
					string message = "";
					var list_UserCourse = _context.Queryable<WebUserCourse>()
						.Where(u => u.Status == 1 && list_DelMenkeCourse.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId))
						.ToList();
					foreach (var model in list_UserCourse)
					{
						var model_MenkeCourse = list_DelMenkeCourse.FirstOrDefault(u => u.MenkeCourseId == u.MenkeCourseId);
						message = $"{DateTime.Now}拓课云课程["+ model_MenkeCourse?.MenkeName + "]被删除导致锁定<hr>";
						model.Status = -1;
						model.Remarks = message + model.Remarks;
					}
					_context.Updateable(list_UserCourse).UpdateColumns(u=>new { u.Status,u.Remarks}).ExecuteCommand();

					//删除已排课节
					_context.Updateable<WebUserLesson>()
						.SetColumns(u=>u.Status==-1)
						.SetColumns(u=>u.Remark==SqlFunc.MergeString($"{DateTime.Now}拓课云课程被删除导致排课无效<hr>",u.Remark))
						.Where(u => u.Status == 1 && u.MenkeState==1 && list_DelMenkeCourse.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId))
						.ExecuteCommand();
					#endregion
				}
				else
				{
					page = last_page;//退出
				}
				page++;
			} while (page <= last_page);//page>=last_page退出，至少执行1次
			_context.Updateable<UeTask>().SetColumns(u => u.Paramenter == update_endtime.ToString()).Where(u => u.Code == "MenkeCourse").ExecuteCommand();
			return result;
		}
		#endregion

		#region 同步课节信息(只能同步一天)
		/// <summary>
		/// 同步课节信息
		/// </summary>
		/// <param name="starttime">开始时间戳(只能同步一天)</param>
		/// <param name="menke_userid">指定学生用户,重置isexe=0</param>
		/// <param name="menke_course_id">指定课程ID,重置isexe=0</param>
		/// <returns></returns>
		public async Task<ApiResult<List<long>>> LessonSync(string starttime, int menke_userid = 0, int menke_course_id = 0)
		{
			var result =new ApiResult<List<long>>();

			if(string.IsNullOrEmpty(starttime) && menke_userid==0 && menke_course_id == 0){
				starttime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddDays(-7)).ToString();
			}

			var list_MenkeLesson = new List<MenkeLesson>();
			var list_UserLesson = new List<WebUserLesson>();
			var list_User = new List<WebUser>();
			int page = 1, last_page = 1;
			string update_starttime = "", update_endtime="";
			var last_time = 0;//最大的更新时间戳
			if (!string.IsNullOrEmpty(starttime))
			{
				var begintime = DateHelper.ConvertIntToDateTime(starttime).AddHours(-6);
				update_starttime = DateHelper.ConvertDateTimeInt(begintime).ToString();//必需重叠6个小时，拓课云添加课节完，才能添加学生，两个动作是分开的，以防万一

				var endtime = begintime.AddDays(1);
				if (endtime > DateTime.UtcNow) endtime = DateTime.UtcNow;
				update_endtime = DateHelper.ConvertDateTimeInt(endtime).ToString();
			}

			var list_DelMenkeLessonId = new List<long>();//要删除掉的课节信息

			string url = Appsettings.app("APIS:menke_lesson");
			var dic_data = new Dictionary<string, object>();
			var dic_header = new Dictionary<string, string>
			{
				{ "key", Appsettings.app("Web:MenkeKey") },
				{ "version", "v1" }
			};
			do
            {
				dic_data.Clear();
                dic_data.Add("page", page);
                if (!string.IsNullOrEmpty(update_starttime)) dic_data.Add("update_starttime", update_starttime);
                if (!string.IsNullOrEmpty(update_endtime)) dic_data.Add("update_endtime", update_endtime);
				if (menke_userid>0) dic_data.Add("user_id", menke_userid);
				if (menke_course_id > 0) dic_data.Add("course_id", menke_course_id);
				var data = string.Join("&", dic_data.Select(u => u.Key + "=" + u.Value).ToList());
                var json = GetRemoteHelper.HttpWebRequestUrl(url + "?" + data, "", "utf-8", "get", dic_header);
				var o = JObject.Parse(json);
				if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
				{
					last_page = (o["data"]["last_page"] != null) ? (int)o["data"]["last_page"] : 1;
					if (o["data"]["data"] != null)
					{
						var jarray = (JArray)o["data"]["data"];
						var list_CourseId = jarray.Select(u => (int)u["course_id"]).Distinct().ToList();
						var list_menke_course = _context.Queryable<MenkeCourse>().Where(u => list_CourseId.Contains(u.MenkeCourseId) && u.Istrial == 1).ToList();
						foreach (var item in jarray)
						{
							int menke_courseid = (item["course_id"] != null) ? (int)item["course_id"] : 0;
							int istrial = list_menke_course.Any(u => u.MenkeCourseId == menke_courseid && u.Istrial == 1) ? 1 : 0;
							string teacher_name = (item["teacher"] != null && item["teacher"].Count()>0 && item["teacher"]["name"] != null) ? item["teacher"]["name"].ToString() : "";
							string teacher_mobile_code = (item["teacher"] != null && item["teacher"].Count() > 0 && item["teacher"]["code"] != null) ? item["teacher"]["code"].ToString() : "";
							string teacher_mobile = (item["teacher"] != null && item["teacher"].Count() > 0 && item["teacher"]["mobile"] != null) ? item["teacher"]["mobile"].ToString() : "";
							string menke_lesson_name = (item["name"] != null) ? item["name"].ToString() : "";
							int update_time = (item["update_time"] != null) ? (int)item["update_time"] : 0;
							string students = (item["student"] != null && item["student"].Count() > 0) ? item["student"].ToString() : "";
							if (update_time > last_time) last_time = update_time;
							//同步排课课节信息
							list_MenkeLesson.Add(new MenkeLesson()
							{
								MenkeLessonId = (item["id"] != null) ? (int)item["id"] : 0,
								MenkeCourseId = menke_courseid,
								MenkeName = (item["name"] != null) ? item["name"].ToString() : "",
								MenkeTeacherName = teacher_name,
								MenkeTeacherMobile = teacher_mobile,
								MenkeTeacherCode = teacher_mobile_code,
								MenkeHelpers = (item["helpers"] != null) ? item["helpers"].ToString() : "",
								MenkeState = (item["state"] != null) ? (int)item["state"] : 0, //1未开始2进行中3已结课4已过期								
								MenkeStarttime = (item["starttime"] != null) ? (int)item["starttime"] : 0,
								MenkeEndtime = (item["endtime"] != null) ? (int)item["endtime"] : 0,
								MenkeUpdateTime = update_time,
								MenkeDeleteTime = (item["delete_time"] != null) ? (int)item["delete_time"] : 0,
								MenkeLiveSerial = (item["live_serial"] != null) ? item["live_serial"].ToString() : "",
								Status = (item["delete_time"] != null && ((int)item["delete_time"]) > 0) ? -1 : 1,
								Istrial = istrial,
								Students = students,
								Json = item.ToString()
							});
						}
					}
				}
				else
				{
					page = last_page;//退出
				}
				page++;
			} while (page <= last_page);//page>=last_page退出，至少执行1次

			if (list_MenkeLesson.Count > 0)
			{
				string message = "";
				//课节删除，网站中已排课课节则需要全删，同步操作
				#region 同步课节删除状态
				message = $"[{DateTime.Now}]拓课云删除排课课节";
				var list_DelMenkeLesson = list_MenkeLesson.Where(u => u.MenkeDeleteTime > 0).ToList();
				var list_DelUserLesson = _context.Queryable<WebUserLesson>().Where(u => u.MenkeDeleteTime == 0 && list_DelMenkeLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
				list_DelUserLesson.ForEach(model =>
				{
					var model_MenkeLesson = list_DelMenkeLesson.FirstOrDefault(u => u.MenkeLessonId == model.MenkeLessonId);
					model.MenkeDeleteTime = model_MenkeLesson.MenkeDeleteTime;
					model.MenkeUpdateTime = model_MenkeLesson.MenkeUpdateTime;
					model.Remark = message + "<br>" + model.Remark;
				});
				_context.Updateable(list_DelUserLesson).UpdateColumns(u => new { u.Remark, u.MenkeDeleteTime, u.MenkeUpdateTime }).ExecuteCommand();
				#endregion

				#region 更新menke_lesson表
				var x = _context.Storageable(list_MenkeLesson).WhereColumns(u => u.MenkeLessonId).ToStorage();
				x.AsInsertable.ExecuteCommand();//新的插入
												//旧的看时间戳更新
				var list_TmpUpdateMenkeLesson = x.UpdateList.Select(u => u.Item).ToList();
				var list_UpdateMenkeLesson = _context.Queryable<MenkeLesson>().Where(u => list_TmpUpdateMenkeLesson.Select(s => s.MenkeLessonId).Distinct().Contains(u.MenkeLessonId)).ToList();
				var list_UpdateMenkeLessonId = new List<int>();
				foreach (var model in list_UpdateMenkeLesson)
				{
					var model_TmpUpdateMenkeLesson = list_TmpUpdateMenkeLesson.FirstOrDefault(u => u.MenkeLessonId == model.MenkeLessonId);
					if (model_TmpUpdateMenkeLesson != null)
					{
						if (menke_userid > 0 || menke_course_id > 0 || model.Json != model_TmpUpdateMenkeLesson.Json || model_TmpUpdateMenkeLesson.MenkeUpdateTime > model.MenkeUpdateTime)
						{
							model.MenkeLessonId = model_TmpUpdateMenkeLesson.MenkeLessonId;
							model.MenkeCourseId = model_TmpUpdateMenkeLesson.MenkeCourseId;
							model.MenkeName = model_TmpUpdateMenkeLesson.MenkeName;
							model.MenkeTeacherName = model_TmpUpdateMenkeLesson.MenkeTeacherName;
							model.MenkeTeacherMobile = model_TmpUpdateMenkeLesson.MenkeTeacherMobile;
							model.MenkeTeacherCode = model_TmpUpdateMenkeLesson.MenkeTeacherCode;
							model.MenkeHelpers = model_TmpUpdateMenkeLesson.MenkeHelpers;
							model.MenkeState = model_TmpUpdateMenkeLesson.MenkeState;//1未开始2进行中3已结课4已过期								
							model.MenkeStarttime = model_TmpUpdateMenkeLesson.MenkeStarttime;
							model.MenkeEndtime = model_TmpUpdateMenkeLesson.MenkeEndtime;
							model.MenkeUpdateTime = model_TmpUpdateMenkeLesson.MenkeUpdateTime;
							model.MenkeDeleteTime = model_TmpUpdateMenkeLesson.MenkeDeleteTime;
							model.MenkeLiveSerial = model_TmpUpdateMenkeLesson.MenkeLiveSerial;
							model.Status = model_TmpUpdateMenkeLesson.Status;
							model.Istrial = model_TmpUpdateMenkeLesson.Istrial;
							model.Json = model_TmpUpdateMenkeLesson.Json;
							model.Students = model_TmpUpdateMenkeLesson.Students;
							model.Isexe = 0;
							model.ExeTime = DateTime.Now;
							list_UpdateMenkeLessonId.Add(model.MenkeLessonId);
						}
					}
				}
				list_UpdateMenkeLesson = list_UpdateMenkeLesson.Where(u => list_UpdateMenkeLessonId.Contains(u.MenkeLessonId)).ToList();
				int n = _context.Updateable(list_UpdateMenkeLesson).WhereColumns(u => u.MenkeLessonId).ExecuteCommand();
				#endregion

				//完结的课程给老师发邮件
				#region 完结的课程给老师发邮件
				var list_MenkeLesson1 = _context.Queryable<MenkeLesson>().Where(u => u.IsTeacherEmail == 0 && u.MenkeState == 3).ToList();
				if (list_MenkeLesson1.Count > 0)
				{
					var list_Teacher = _context.Queryable<WebTeacher>().Where(u => list_MenkeLesson1.Select(s => s.MenkeTeacherCode).Contains(u.MobileCode) && list_MenkeLesson1.Select(s => s.MenkeTeacherMobile).Contains(u.Mobile) && !string.IsNullOrEmpty(u.Email))
						.Select(u => new { u.Email, u.MobileCode, u.Mobile, u.HeadImg, u.TeacherLang }).ToList();
					var list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_MenkeLesson1.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
					foreach (var model in list_MenkeLesson1)
					{
						var model_Teacher = list_Teacher.FirstOrDefault(u => u.MobileCode == model.MenkeTeacherCode && u.Mobile == model.MenkeTeacherMobile);
						if (model_Teacher == null) continue;
						string teacher_code = RandomHelper.RandCode(8, list_MenkeLesson1.IndexOf(model));

						//这里有个问题，不确定老师时区，只能给UTC时间
						var btime = DateHelper.ConvertIntToDateTime(model.MenkeStarttime.ToString());
						var etime = DateHelper.ConvertIntToDateTime(model.MenkeEndtime.ToString());
						var dic_email = new Dictionary<string, string>();
						string head_img = string.IsNullOrEmpty(model_Teacher.HeadImg) ? "/Upfile/images/teacher_none.png" : model_Teacher.HeadImg;
						dic_email.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
						dic_email.Add("teacher_name", model.MenkeTeacherName);
						//dic_email.Add("date", btime.ToString("yyyy-MM-dd"));
						//dic_email.Add("week", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(btime.DayOfWeek));
						//dic_email.Add("btime", btime.ToString("HH:mm"));
						//dic_email.Add("etime", etime.ToString("HH:mm"));
						dic_email.Add("menke_course_name", list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == model.MenkeCourseId)?.MenkeName + "");
						dic_email.Add("menke_lesson_name", model.MenkeName);
						dic_email.Add("url", $"{Appsettings.app("Web:Host")}/user/teacher/{model.MenkeLessonId}");
						_logger.LogInformation("老师评价url=" + $"{Appsettings.app("Web:Host")}/user/teacher/{model.MenkeLessonId}");
						dic_email.Add("teacher_code", teacher_code);
						var result_email = await _messageBaseService.AddEmailTask("TeacherLessonReport", model_Teacher.Email, dic_email, model_Teacher.TeacherLang);
						if (result_email.StatusCode == 0)
						{
							model.TeacherCode = teacher_code;
							model.TeacherCodeTime = DateTime.Now;
							model.IsTeacherEmail = 1;
						}
					}
					_context.Updateable(list_MenkeLesson1).WhereColumns(it => new { it.MenkeLessonId }).ExecuteCommand();
				}
				#endregion

				#region "强制重置完isexe=0后，直接执行分解"
				await LessonSplit(list_MenkeLesson,1);
				#endregion
			}
			//使用last_time的原因是，如果1分钟执行不完，至少不会漏
			if ((starttime == last_time.ToString() || last_time == 0) && menke_userid == 0 && menke_course_id == 0)
			{
				last_time = int.Parse(update_endtime);
			}
			if(last_time>0)
				_context.Updateable<UeTask>().SetColumns(u => u.Paramenter == last_time.ToString()).Where(u => u.Code == "MenkeLesson").ExecuteCommand();
			result.Data = _context.Queryable<WebUserLesson>().Where(u=> list_MenkeLesson.Select(u => u.MenkeLessonId).Contains(u.MenkeLessonId) && u.Status==1).Select(u=>u.UserLessonid).Distinct().ToList();
			return result;
		}
		#endregion

		#region 执行拆分课节
		/// <summary>
		/// 执行拆分课节
		/// </summary>
		/// <param name="list_MenkeLesson">要处理的课节信息,为null时则按count取数</param>
		/// <param name="reset">是否重置isexe=0,0否，1是</param>
		/// <param name="count">数量</param>
		/// <returns></returns>
		public async Task<ApiResult> LessonSplit(List<MenkeLesson> list_MenkeLesson = null, int reset = 0, int count = 100)
		{
			var result = new ApiResult();


			var list_UserLesson = new List<WebUserLesson>();
			var list_User = new List<WebUser>();

			var list_DelMenkeLessonId = new List<long>();//要删除掉的课节信息


			//取要执行的所有门课课节
			if(list_MenkeLesson==null)
				list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => u.Isexe == 0).OrderBy(u => u.MenkeStarttime).Take(count).ToList();
			if (list_MenkeLesson.Count == 0) return result;
			var list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_MenkeLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();

			#region "按学生拆分成已排课节,暂存入list_UserLesson"
			foreach (var model in list_MenkeLesson) {
				if (string.IsNullOrEmpty(model.Json))continue;
				var item = JObject.Parse(model.Json);
				//同步学生排课课节信息
				if (item != null && item["student"] != null)
				{
					int course_id = (item["course_id"] != null) ? (int)item["course_id"] : 0;
					int lesson_id = (item["id"] != null) ? (int)item["id"] : 0;
					int menke_state = (item["state"] != null) ? (int)item["state"] : 0; //1未开始2进行中3已结课4已过期	
					int menke_starttime = (item["starttime"] != null) ? (int)item["starttime"] : 0;
					int menke_endtime = (item["endtime"] != null) ? (int)item["endtime"] : 0;
					int menke_updatetime = (item["update_time"] != null) ? (int)item["update_time"] : 0;
					int menke_deletetime = (item["delete_time"] != null) ? (int)item["delete_time"] : 0;
					string menke_liveserial = (item["live_serial"] != null) ? item["live_serial"].ToString() : "";
					string menke_lesson_name = (item["name"] != null) ? item["name"].ToString() : "";
					string teacher_name = "", teacher_mobile_code = "", teacher_mobile = "";
					if (item["teacher"] != null && item["teacher"].Count() > 0)
					{
						teacher_name = (item["teacher"]["name"] != null) ? item["teacher"]["name"].ToString() : "";
						teacher_mobile_code = (item["teacher"]["code"] != null) ? item["teacher"]["code"].ToString() : "";
						teacher_mobile = (item["teacher"]["mobile"] != null) ? item["teacher"]["mobile"].ToString() : "";
					}
					var jarray_student = (JArray)item["student"];
					foreach (var student in jarray_student)
					{
						//添加试听学生
						string mobile = (student["mobile"] != null) ? student["mobile"].ToString() : "";
						string code = (student["code"] != null) ? student["code"].ToString() : "";
						string menke_student_name = (student["name"] != null) ? student["name"].ToString() : "";

						list_UserLesson.Add(new WebUserLesson()
						{
							MenkeCourseId = course_id,
							MenkeLessonId = lesson_id,
							MenkeLessonName = menke_lesson_name,
							MenkeStudentName = menke_student_name,
							MenkeStudentMobile = mobile,
							MenkeStudentCode = code,
							MenkeTeacherName = teacher_name,
							MenkeTeacherMobile = teacher_mobile,
							MenkeTeacherCode = teacher_mobile_code,
							MenkeState = menke_state,
							MenkeStarttime = menke_starttime,
							MenkeEndtime = menke_endtime,
							MenkeUpdateTime = menke_updatetime,
							MenkeDeleteTime = menke_deletetime,
							MenkeLiveSerial = menke_liveserial,
							Status = 1, //(item["delete_time"] != null && ((int)item["delete_time"]) > 0) ? -1 : 1,
							Istrial = list_MenkeCourse.Any(u => u.MenkeCourseId == model.MenkeCourseId && u.Istrial == 1) ? 1 : 0
						});
					}
				}
				model.Isexe = 1;
				model.ExeTime = DateTime.Now;
			}
			_context.Updateable(list_MenkeLesson).WhereColumns(it => new { it.MenkeLessonId }).UpdateColumns(u => new { u.Isexe, u.ExeTime }).ExecuteCommand();
			#endregion

			#region "判断学生明细，同步移除操作"
			//取所有旧的所有相关排课课节信息
			var list_AllUserLesson = _context.Queryable<WebUserLesson>().Where(u => list_MenkeLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();//取DB中本次需要更新的课节

			//从旧的课节中取出所有有效排课课节
			var list_RemoveUserLesson = list_AllUserLesson.Where(u => u.MenkeDeleteTime == 0 && u.Status == 1).ToList();//取DB中还是正常的学生课节，准备移除
			foreach (var model in list_RemoveUserLesson)
			{
				//判断当前学生课节是否在本次同步中存在，否表示学生被从此课节中移除了
				if (!list_UserLesson.Any(u => u.MenkeLessonId == model.MenkeLessonId && u.MenkeStudentCode == model.MenkeStudentCode && u.MenkeStudentMobile == model.MenkeStudentMobile))
				{
					model.Status = 0;
					model.Remark = $"[{DateTime.Now}]拓课云移除学生排课信息<hr>" + model.Remark;
				}
			}
			_context.Updateable(list_RemoveUserLesson).UpdateColumns(u => new { u.Status, u.Remark }).ExecuteCommand();
			#endregion

			if (list_UserLesson.Count > 0)
			{
				#region "更新当前所有课节web_user_lesson"
				//执行同步课节学生信息
				var x = _context.Storageable(list_UserLesson).WhereColumns(u => new { u.MenkeLessonId, u.MenkeStudentCode, u.MenkeStudentMobile }).ToStorage();
				x.AsInsertable.ExecuteCommand();
				//旧的看时间戳更新
				var list_TmpUpdateUserLesson = x.UpdateList.Select(u => u.Item).ToList();
				var list_UpdateUserLesson = _context.Queryable<WebUserLesson>().Where(u => list_TmpUpdateUserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId) && list_TmpUpdateUserLesson.Select(s => s.MenkeStudentCode + s.MenkeStudentMobile).Contains(SqlFunc.MergeString(u.MenkeStudentCode, u.MenkeStudentMobile))).ToList();
				var list_UpdateUserLessonId = new List<long>();
				foreach (var model in list_UpdateUserLesson)
				{
					var model_TmpUpdateUserLesson = list_TmpUpdateUserLesson.FirstOrDefault(u => u.MenkeLessonId == model.MenkeLessonId && u.MenkeStudentMobile == model.MenkeStudentMobile && u.MenkeStudentCode == model.MenkeStudentCode);
					if (model_TmpUpdateUserLesson != null && (model_TmpUpdateUserLesson.MenkeUpdateTime > model.MenkeUpdateTime || reset == 1))
					{
						model.MenkeCourseId = model_TmpUpdateUserLesson.MenkeCourseId;
						model.MenkeLessonId = model_TmpUpdateUserLesson.MenkeLessonId;
						model.MenkeLessonName = model_TmpUpdateUserLesson.MenkeLessonName;
						model.MenkeStudentName = model_TmpUpdateUserLesson.MenkeStudentName;
						model.MenkeStudentCode = model_TmpUpdateUserLesson.MenkeStudentCode;
						model.MenkeStudentMobile = model_TmpUpdateUserLesson.MenkeStudentMobile;
						model.MenkeTeacherCode = model_TmpUpdateUserLesson.MenkeTeacherCode;
						model.MenkeTeacherMobile = model_TmpUpdateUserLesson.MenkeTeacherMobile;
						model.MenkeTeacherName = model_TmpUpdateUserLesson.MenkeTeacherName;
						model.MenkeState = model_TmpUpdateUserLesson.MenkeState;
						model.MenkeStarttime = model_TmpUpdateUserLesson.MenkeStarttime;
						model.MenkeEndtime = model_TmpUpdateUserLesson.MenkeEndtime;
						model.MenkeUpdateTime = model_TmpUpdateUserLesson.MenkeUpdateTime;
						model.MenkeDeleteTime = model_TmpUpdateUserLesson.MenkeDeleteTime;
						model.MenkeLiveSerial = model_TmpUpdateUserLesson.MenkeLiveSerial;
						model.Istrial = model_TmpUpdateUserLesson.Istrial;
						if (model_TmpUpdateUserLesson.MenkeDeleteTime == 0 && model.Status == 0) model.Status = 1;
						list_UpdateUserLessonId.Add(model.UserLessonid);
					}
				}
				list_UpdateUserLesson = list_UpdateUserLesson.Where(u => list_UpdateUserLessonId.Contains(u.UserLessonid)).ToList();
				int n = _context.Updateable(list_UpdateUserLesson).ExecuteCommand();//有主键
				#endregion

				#region "更新排课缺课所有课节"
				//排课后缺课
				var del_menke_lessonid = list_UserLesson.Where(u => u.MenkeState == 4).Select(u => u.MenkeLessonId).ToList();
				_context.Updateable<WebCourseApply>()
					.SetColumns(u => u.Status == 2)
					.SetColumns(u => u.Remark == SqlFunc.MergeString($"[{DateTime.Now}]计划任务关联课节已过期,自动缺课<hr>", u.Remark))
					.Where(u => del_menke_lessonid.Contains(u.MenkeLessonId))
					.ExecuteCommand();
				#endregion

				#region 试听课,更新试听申请状态
				//取试听课程model_TrialCourse
				//取本次排课属于试听课的排课信息list_TrialUserLesson
				var model_TrialCourse = _context.Queryable<MenkeCourse>().First(u => u.Istrial == 1 && u.MenkeDeleteTime == 0);
				var list_TrialUserLesson = list_UserLesson.Where(u => u.Istrial == 1).ToList();//所有试听课程的学生
				if (model_TrialCourse != null && list_TrialUserLesson.Count > 0)
				{
					//取本次试听课节中的学生信息
					list_User = _context.Queryable<WebUser>().Where(u => list_TrialUserLesson.Select(s => s.MenkeStudentCode).Contains(u.MobileCode) && list_TrialUserLesson.Select(s => s.MenkeStudentMobile).Contains(u.Mobile)).ToList();

					//取所有试听用户有提交过的未审核的申请表单
					var list_Apply = _context.Queryable<WebCourseApply>().Where(u => u.Sty == 0 && ((list_TrialUserLesson.Select(s => s.MenkeStudentCode + s.MenkeStudentMobile).Contains(SqlFunc.MergeString(u.ContactMobileCode, u.ContactMobile))) || list_User.Select(s => s.Userid).Contains(u.Userid)) && u.Status == 0).ToList();
					foreach (var model in list_Apply)
					{
						//找到申请人
						var model_User = list_User.Where(u => (u.Mobile == model.ContactMobile && u.MobileCode == model.ContactMobileCode) || u.Userid == model.Userid).OrderByDescending(u => u.Userid).FirstOrDefault();//找到申请人
						if (model_User != null)
						{
							//找到申请人本次排的课节（手机号相同，并且查找标准备排课时间在申请时间之后）
							var model_TrialLesson = list_TrialUserLesson.FirstOrDefault(u => u.MenkeLessonId == model.MenkeLessonId);
							if (model_TrialLesson != null)
							{
								if (model_TrialLesson.MenkeDeleteTime == 0)
								{

									model.Status = 1;
									model.Remark = $"[{DateTime.Now}]计划任务关联课节并自动审核<hr>" + model.Remark;
								}
								else
								{
									model.Status = 3;
									model.Remark = $"[{DateTime.Now}]计划任务关联课节已删除,自动拒绝<hr>" + model.Remark;
								}
								model.MenkeCourseId = model_TrialLesson.MenkeCourseId;
								model.MenkeLessonId = model_TrialLesson.MenkeLessonId;
								model.MenkeCourseName = model_TrialCourse.MenkeName;
								model.MenkeLessonName = model_TrialLesson.MenkeLessonName;
								model.Teacher = model_TrialLesson.MenkeTeacherName;
								model.TeacherMobileCode = model_TrialLesson.MenkeTeacherCode;
								model.TeacherMobile = model_TrialLesson.MenkeTeacherMobile;
							}
							else
							{
								model_TrialLesson = list_TrialUserLesson.FirstOrDefault(u => u.Dtime > model.Dtime && u.MenkeStudentCode == model_User.MobileCode && u.MenkeStudentMobile == model_User.Mobile);
								if (model_TrialLesson != null)
								{
									if (model_TrialLesson.MenkeDeleteTime == 0)
									{

										model.Status = 1;
										model.Remark = $"[{DateTime.Now}]计划任务关联课节并自动审核<hr>" + model.Remark;
									}
									else
									{
										model.Status = 3;
										model.Remark = $"[{DateTime.Now}]计划任务关联课节已删除,自动拒绝<hr>" + model.Remark;
									}
									model.MenkeCourseId = model_TrialLesson.MenkeCourseId;
									model.MenkeLessonId = model_TrialLesson.MenkeLessonId;
									model.MenkeCourseName = model_TrialCourse.MenkeName;
									model.MenkeLessonName = model_TrialLesson.MenkeLessonName;
									model.Teacher = model_TrialLesson.MenkeTeacherName;
									model.TeacherMobileCode = model_TrialLesson.MenkeTeacherCode;
									model.TeacherMobile = model_TrialLesson.MenkeTeacherMobile;
								}
							}
						}
					}
					_context.Updateable(list_Apply).UpdateColumns(u => new { u.Status, u.MenkeCourseId, u.MenkeLessonId, u.MenkeCourseName, u.MenkeLessonName, u.Teacher, u.TeacherMobile, u.TeacherMobileCode }).ExecuteCommand();

				}
				#endregion

				#region 正课,更新正课排课申请状态
				var list_FormalLesson = list_UserLesson.Where(u => u.Istrial == 0 && u.MenkeDeleteTime == 0).ToList();
				var list_FormalApply = _context.Queryable<WebCourseApply>().Where(u => u.Sty == 2 && list_FormalLesson.Select(s => s.MenkeStudentCode + s.MenkeStudentMobile).Contains(SqlFunc.MergeString(u.ContactMobileCode, u.ContactMobile)) && u.Status == 0).ToList();
				var list_FormalCourse = _context.Queryable<MenkeCourse>().Where(u => list_FormalLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
				list_FormalApply.ForEach(model =>
				{
					//找到申请人本次排的课节（同个用户，并且查找标准备排课时间在申请时间之后）
					var model_FormalLesson = list_FormalLesson.FirstOrDefault(u => u.MenkeCourseId == model.MenkeCourseId && u.Userid == model.Userid && u.Dtime > model.Dtime);
					if (model_FormalLesson != null)
					{
						model.Status = 1;
						model.MenkeCourseId = model_FormalLesson.MenkeCourseId;
						model.MenkeLessonId = model_FormalLesson.MenkeLessonId;
						model.MenkeCourseName = list_FormalCourse.FirstOrDefault(u => u.MenkeCourseId == model.MenkeCourseId)?.MenkeName + "";
						model.MenkeLessonName = model_FormalLesson.MenkeLessonName;
						model.Teacher = model_FormalLesson.MenkeTeacherName;
						model.TeacherMobileCode = model_FormalLesson.MenkeTeacherCode;
						model.TeacherMobile = model_FormalLesson.MenkeTeacherMobile;
					}
				});
				_context.Updateable(list_FormalApply).ExecuteCommand();
				#endregion
			}
			//补齐缺失的userid,courseid等网站关联ID
			await UnionLessonUserid(list_MenkeLesson.Select(u => u.MenkeCourseId).ToList());

			//补充关联排课中缺失的进入教室的URL
			await UnionUserLessonUrl(list_MenkeLesson.Select(u => u.MenkeCourseId).ToList());

			//自动处理取消课节申请
			await UserLessonCancel(list_MenkeLesson.Select(u => u.MenkeCourseId).ToList());

			//重新统计课时
			await UserLessonRecount(list_MenkeLesson.Select(u => u.MenkeCourseId).ToList());
			return result;
		}
		#endregion

		#region 删除课节
		/// <summary>
		/// 删除课节
		/// </summary>
		/// <param name="menke_lessonid"></param>
		/// <returns></returns>
		public async Task<ApiResult> DeleteLesson(int menke_lessonid)
		{
			string url, json;
			JObject o = null;
			var result = new ApiResult<int>();
			var dic_header = new Dictionary<string, string>
			{
				{ "key", Appsettings.app("Web:MenkeKey") },
				{ "version", "v1" }
			};
			
			url = string.Format(Appsettings.app("APIS:menke_delete_lesson"),menke_lessonid.ToString());
			json = GetRemoteHelper.HttpWebRequestUrl(url, "", "utf-8", "delete", dic_header, "application/json");
			o = JObject.Parse(json);
			if (o != null && o["result"] != null && o["result"].ToString() == "0")
			{
				result.StatusCode = 0;
			}
			else
			{
				result.StatusCode = 4002;
				result.Message = "删除拓课云课节接口异常,json=>" + json;
				_logger.LogError("删除拓课云课节["+ menke_lessonid +"]接口异常,json=>" + json);
			}			
			return result;
		}
        #endregion

        #region 编辑课节中的上课学生
        /// <summary>
        /// 编辑课节中的上课学生
        /// </summary>
        /// <param name="menke_lessonid"></param>
        /// <param name="mobiles">["86-13145678912"]</param>
        /// <returns></returns>
        public async Task<ApiResult> ModifyLessonStudent(MenkeLesson model_MenkeLesson,List<string> mobiles)
        {
            var result = new ApiResult<int>();
			if (model_MenkeLesson != null)
            {
				var dic_header = new Dictionary<string, string>
				{
					{ "key", Appsettings.app("Web:MenkeKey") },
					{ "version", "v1" }
				};
                string url = "", json = "";
				JObject o = null;
				url = string.Format(Appsettings.app("APIS:menke_modify_lesson"), model_MenkeLesson.MenkeLessonId.ToString());
				var dic_data = new Dictionary<string, object>();
				dic_data.Add("roomname", model_MenkeLesson.MenkeName);
                dic_data.Add("teacher_mobile", model_MenkeLesson.MenkeTeacherCode+"-"+model_MenkeLesson.MenkeTeacherMobile);
                dic_data.Add("students", mobiles);
				var btime = DateHelper.ConvertIntToDateTime(model_MenkeLesson.MenkeStarttime.ToString()).ToUniversalTime();
                var etime = DateHelper.ConvertIntToDateTime(model_MenkeLesson.MenkeEndtime.ToString()).ToUniversalTime();
                dic_data.Add("start_date", btime.ToString("yyyy-MM-dd"));
				dic_data.Add("start_time", btime.ToString("HH:mm"));
				dic_data.Add("end_time", etime.ToString("HH:mm"));
                json = GetRemoteHelper.HttpWebRequestUrl(url+ "?timezone=Europe/London", JsonConvert.SerializeObject(dic_data), "utf-8", "put", dic_header, "application/json");
				o = JObject.Parse(json);
				if (o != null && o["result"] != null && o["result"].ToString() == "0")
				{
					result.StatusCode = 0;
				}
				else
				{
					result.StatusCode = 4002;
					result.Message = "修改拓课云课节上课学生,接口异常,json=>" + json;
					_logger.LogError("修改拓课云课节["+ model_MenkeLesson.MenkeLessonId + "]上课学生,接口异常,json=>" + json);
				}
			}
			else
            {
                result.StatusCode = 4009;
                result.Message = "拓课云课节不存在";
            }
            return result;
        }
        public async Task<ApiResult> ModifyLessonStudent(int menke_lessonid,List<string> mobiles)
        {
            return await ModifyLessonStudent(_context.Queryable<MenkeLesson>().First(u=>u.MenkeLessonId == menke_lessonid),mobiles);
        }
        #endregion

        #region 补充关联排课表中Userid,MenkeUserid,课程及SKU id
        /// <summary>
        /// 补充关联排课表中Userid,课程及SKU id
        /// 关联学生ID，关联老师ID，关联课程SKU，关联已购课程UserCourseid
        /// </summary>
        /// <param name="user_lessonid"></param>
        /// <returns></returns>
        public async Task UnionLessonUserid(List<int> menkeLessonIds)
		{
			string err_msg = "";
            //处理userid,courseid等网站关联ID
            //取当前所有未关联上的用户课节
            var list_UserLesson = _context.Queryable<WebUserLesson>()
				.Where(u => menkeLessonIds.Contains(u.MenkeCourseId) && ((u.Istrial==0 && ((u.Courseid == 0 && u.OnlineCourseid==0) || u.UserCourseid == 0))|| u.Teacherid == 0 || u.Userid == 0 || u.SkuTypeid==0))
				.ToList();
			if (list_UserLesson.Count == 0) return;
			//找到课节所对应用户
			var list_User = _context.Queryable<WebUser>().Where(u => list_UserLesson.Select(s => s.MenkeStudentCode+s.MenkeStudentMobile).Contains(SqlFunc.MergeString(u.MobileCode,u.Mobile)) ).ToList();
			var list_Techer = _context.Queryable<WebTeacher>().Where(u => list_UserLesson.Select(s => s.MenkeTeacherCode+s.MenkeTeacherMobile).Contains(SqlFunc.MergeString(u.MobileCode, u.Mobile)) ).ToList();
			var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u=> list_User.Select(s=>s.Userid).Contains(u.Userid) && u.Status!=-1).ToList();

			#region 查拓课云用户ID为0的全面更新
			//取当前已找到的并且有关联好的用户
			var dic_MenkeUserid = list_User.Where(u => u.MenkeUserId > 0).ToDictionary(u => (u.MobileCode + u.Mobile), u => u.MenkeUserId);
			//取出课节中所有没查到menke_userid的用户，调接口查menke_userid,并入dic_MenkeUserid
			var list_GetMenKeUser = list_UserLesson.Where(u => u.MenkeUserId == 0 && !dic_MenkeUserid.Select(s => s.Key).Contains(u.MenkeStudentCode + u.MenkeStudentMobile)).ToList();
			foreach (var model in list_GetMenKeUser.GroupBy(u=>new { u.MenkeStudentCode,u.MenkeStudentMobile}).Select(u=>new { u.Key.MenkeStudentCode, u.Key.MenkeStudentMobile }).ToList()) {
				var result_student = await GetMenkeUserId(model.MenkeStudentCode, model.MenkeStudentMobile, 0);
				if (result_student.StatusCode == 0)
				{
					dic_MenkeUserid.Add(model.MenkeStudentCode + model.MenkeStudentMobile, result_student.Data);
				}
			}
            #endregion

            foreach (var model in list_UserLesson)
			{
				err_msg = "";
				#region 关联学生ID
				//关联学生ID
				if (model.MenkeUserId == 0)
				{
					var model_User = list_User.FirstOrDefault(u => u.Mobile == model.MenkeStudentMobile && u.MobileCode == model.MenkeStudentCode);
					if (model_User != null)
					{
						model.Userid = model_User.Userid;
						model.MenkeUserId = model_User.MenkeUserId;
						model.Remark = $"[{DateTime.Now}]关联客户ID成功" + "<hr>" + model.Remark;
						if (model_User.MenkeUserId == 0 && dic_MenkeUserid.ContainsKey(model.MenkeStudentCode + model.MenkeStudentMobile))
						{
							//同步顺便检查下web_user中的用户关联信息
							model_User.MenkeUserId = dic_MenkeUserid[model.MenkeStudentCode + model.MenkeStudentMobile];
							_context.Updateable(model_User).UpdateColumns(u => u.MenkeUserId).ExecuteCommand();
						}
					}
					else
					{
						err_msg += $"拓课云学生:{model.MenkeStudentName}[{model.MenkeStudentCode}-{model.MenkeStudentMobile}]未在网站注册!<hr>";
					}
					if (model.MenkeUserId == 0 && dic_MenkeUserid.ContainsKey(model.MenkeStudentCode + model.MenkeStudentMobile))
					{
						model.MenkeUserId = dic_MenkeUserid[model.MenkeStudentCode + model.MenkeStudentMobile];
					}
				}
				#endregion

				#region 关联老师ID
				//关联老师ID
				if (model.Teacherid == 0)
				{
					var model_Teacher = list_Techer.FirstOrDefault(u => u.Mobile == model.MenkeTeacherMobile && u.MobileCode == model.MenkeTeacherCode);
					if (model_Teacher != null)
					{
						model.Teacherid = model_Teacher.Teacherid;
						model.Remark = $"[{DateTime.Now}]关联老师ID成功" + "<hr>" + model.Remark;
					}
					else
					{
						err_msg += $"拓课云老师:{model.MenkeTeacherName}[{model.MenkeTeacherCode}-{model.MenkeTeacherMobile}]未在网站补齐资料!<hr>";
					}
                }
				#endregion

				#region 关联正课课程ID，上课方式，SKUID。以及已购课程相关
				if (model.Istrial==0)
                {
                    //关联已购课程
                    if (model.UserCourseid == 0 || model.SkuTypeid==0 || (model.Courseid==0 && model.OnlineCourseid==0))
                    {
                        var model_UserCouser = list_UserCourse.Where(u => u.MenkeCourseId == model.MenkeCourseId && u.Userid==model.Userid && u.ScheduleClasses < u.ClassHour).OrderByDescending(u=>u.Status).FirstOrDefault();
                        if (model_UserCouser != null)
						{
							model.Courseid = model_UserCouser.Courseid;
							model.OnlineCourseid = model_UserCouser.OnlineCourseid;
							model.SkuTypeid = model_UserCouser.SkuTypeid;
							model.UserCourseid = model_UserCouser.UserCourseid;
                            model.Remark = $"[{DateTime.Now}]关联已购的课程ID={model_UserCouser.UserCourseid}成功" + "<hr>" + model.Remark;
							model_UserCouser.ScheduleClasses++;
                        }
                        else
                        {
                            model_UserCouser = list_UserCourse.FirstOrDefault(u => u.MenkeCourseId == model.MenkeCourseId && u.Userid == model.Userid );
							if (model_UserCouser != null)
							{
								model.Courseid = model_UserCouser.Courseid;
								model.OnlineCourseid = model_UserCouser.OnlineCourseid;
								model.SkuTypeid = model_UserCouser.SkuTypeid;
								model.UserCourseid = model_UserCouser.UserCourseid;
                                model.Remark = $"[{DateTime.Now}]警告:排课已超出总课时数！关联已购的课程ID={model_UserCouser.UserCourseid}成功" + "<hr>" + model.Remark;
                                model_UserCouser.ScheduleClasses++;
                            }
							else
							{
								err_msg += $"用户已购的课程中无法关联这节课!<hr>";
							}
                        }
                    }
                }
				#endregion

				if (!string.IsNullOrEmpty(err_msg)) model.Remark = "["+DateTime.Now+"]"+ err_msg ;
				if (model.Remark.Length > 4000) {
					model.Remark = model.Remark.Remove(4000);
				}
			}
			_context.Updateable(list_UserLesson).UpdateColumns(u => new { u.Teacherid, u.Remark, u.Userid, u.MenkeUserId, u.Courseid,u.OnlineCourseid,u.UserCourseid,u.SkuTypeid }).ExecuteCommand();

		}
        #endregion

        #region 补充关联排课中缺失的进入教室的URL
        /// <summary>
        /// 补充关联排课中缺失的进入教室的URL
        /// </summary>
        /// <param name="user_lessonid"></param>
        /// <returns></returns>
        public async Task UnionUserLessonUrl(List<int> menkeCourseIds)
		{
			List<WebUserLesson> list_UserLesson = new List<WebUserLesson>();
			list_UserLesson = _context.Queryable<WebUserLesson>().Where(u =>string.IsNullOrEmpty(u.MenkeEntryurl) && u.Status == 1 && menkeCourseIds.Contains(u.MenkeCourseId) && SqlFunc.Subqueryable<MenkeLessonUrl>().Where(s => s.MenkeLessonId == u.MenkeLessonId && s.MenkeUserId == u.MenkeUserId).NotAny()).ToList();
			if (list_UserLesson.Count == 0) return;
			string url = Appsettings.app("APIS:menke_lesson_entry");
			var dic_header = new Dictionary<string, string>
			{
				{ "key", Appsettings.app("Web:MenkeKey") },
				{ "version", "v1" }
			};
			var list_MenkeLessonUrl = new List<MenkeLessonUrl>();
			foreach (var model in list_UserLesson)
			{
				var json = GetRemoteHelper.HttpWebRequestUrl(url + "/" + model.MenkeLessonId + "?user_name=" + model.MenkeStudentName + "&user_id=" + model.MenkeUserId, "", "utf-8", "get", dic_header);
				var o = JObject.Parse(json);
				if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
				{
					var obj = (JObject)o["data"];
					list_MenkeLessonUrl.Add(new MenkeLessonUrl()
					{
						MenkeLessonId = model.MenkeLessonId,
						MenkeUserName = model.MenkeStudentName,
						MenkeTeacherUserpassword = (obj["Teacher"] != null && obj["Teacher"]["userpassword"] != null) ? obj["Teacher"]["userpassword"].ToString() : "",
						MenkeTeacherEnterurl = (obj["Teacher"] != null && obj["Teacher"]["enterUrl"] != null) ? obj["Teacher"]["enterUrl"].ToString() : "",
						MenkeTeacherEntryurl = (obj["Teacher"] != null && obj["Teacher"]["entryUrl"] != null) ? obj["Teacher"]["entryUrl"].ToString() : "",
						MenkeAssistantUserpassword = (obj["Assistant"] != null && obj["Assistant"]["userpassword"] != null) ? obj["Assistant"]["userpassword"].ToString() : "",
						MenkeAssistantEnterurl = (obj["Assistant"] != null && obj["Assistant"]["enterUrl"] != null) ? obj["Assistant"]["enterUrl"].ToString() : "",
						MenkeAssistantEntryurl = (obj["Assistant"] != null && obj["Assistant"]["entryUrl"] != null) ? obj["Assistant"]["entryUrl"].ToString() : "",
						MenkeStudentUserpassword = (obj["Student"] != null && obj["Student"]["userpassword"] != null) ? obj["Student"]["userpassword"].ToString() : "",
						MenkeStudentEnterurl = (obj["Student"] != null && obj["Student"]["enterUrl"] != null) ? obj["Student"]["enterUrl"].ToString() : "",
						MenkeStudentEntryurl = (obj["Student"] != null && obj["Student"]["entryUrl"] != null) ? obj["Student"]["entryUrl"].ToString() : "",
						MenkeTourUserpassword = (obj["Tour"] != null && obj["Tour"]["userpassword"] != null) ? obj["Tour"]["userpassword"].ToString() : "",
						MenkeTourEnterurl = (obj["Tour"] != null && obj["Tour"]["enterUrl"] != null) ? obj["Tour"]["enterUrl"].ToString() : "",
						MenkeTourEntryurl = (obj["Tour"] != null && obj["Tour"]["entryUrl"] != null) ? obj["Tour"]["entryUrl"].ToString() : "",
						MenkeAuditorsUserpassword = (obj["Auditors"] != null && obj["Auditors"]["userpassword"] != null) ? obj["Auditors"]["userpassword"].ToString() : "",
						MenkeAuditorsEnterurl = (obj["Auditors"] != null && obj["Auditors"]["enterUrl"] != null) ? obj["Auditors"]["enterUrl"].ToString() : "",
						MenkeAuditorsEntryurl = (obj["Auditors"] != null && obj["Auditors"]["entryUrl"] != null) ? obj["Auditors"]["entryUrl"].ToString() : "",
						Json = json
					});
					model.MenkeEntryurl = (obj["Student"] != null && obj["Student"]["entryUrl"] != null) ? obj["Student"]["entryUrl"].ToString() : "";
                }
			}
			_context.Updateable(list_UserLesson).UpdateColumns(u => u.MenkeEntryurl).ExecuteCommand();
			if (list_MenkeLessonUrl.Count > 0)
			{
				var x =_context.Storageable(list_MenkeLessonUrl).WhereColumns(u => new { u.MenkeLessonId, u.MenkeUserId }).ToStorage();
				x.AsInsertable.ExecuteCommand();
				x.AsUpdateable.UpdateColumns(u=>new { 
				u.MenkeLessonId,u.MenkeUserName,u.MenkeTeacherUserpassword,u.MenkeTeacherEnterurl,u.MenkeTeacherEntryurl,u.MenkeAssistantUserpassword,u.MenkeAssistantEnterurl,u.MenkeAssistantEntryurl,
				u.MenkeStudentUserpassword,u.MenkeStudentEnterurl,u.MenkeStudentEntryurl,u.MenkeTourEnterurl,u.MenkeTourEntryurl,u.MenkeTourUserpassword,u.MenkeAuditorsEnterurl,u.MenkeAuditorsEntryurl,u.MenkeAuditorsUserpassword,
				u.Json
                }).ExecuteCommand();
			}
		}
		#endregion

		#region 重新统计课时
		/// <summary>
		/// 重新统计课时
		/// </summary>
		/// <param name="menkeLessonIds"></param>
		/// <returns></returns>
		public async Task UserLessonRecount(List<int> menkeCourseIds)
		{
			var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
			var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u =>u.Status != -1 && menkeCourseIds.Contains(u.MenkeCourseId)).ToList();
			var list_UserLesson= _context.Queryable<WebUserLesson>().Where(u =>u.MenkeDeleteTime == 0 && u.Status==1 && list_UserCourse.Select(s=>s.UserCourseid).Contains(u.UserCourseid)).ToList();
			foreach (var model in list_UserCourse) {
				model.Classes = list_UserLesson.Where(u =>(u.MenkeState == 3 || u.MenkeState == 4 ||(u.MenkeState==1 && u.MenkeEndtime< thetime)) && u.UserCourseid == model.UserCourseid && u.Userid==model.Userid).Count();//已完成的课节
				model.ScheduleClasses= list_UserLesson.Where(u => u.UserCourseid == model.UserCourseid && u.Userid == model.Userid).Count();//已排课的课节
            }
			_context.Updateable(list_UserCourse).UpdateColumns(u => u.Classes).ExecuteCommand();
			
		}
		#endregion

		#region 处理取消课节申请
		/// <summary>
		/// 处理取消课节申请
		/// </summary>
		/// <param name="menkeCourseIds">本次更新的课程ID</param>
		/// <returns></returns>
		public async Task UserLessonCancel(List<int> menkeCourseIds)
		{
			var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u =>(u.MenkeDeleteTime>0 || u.Status!=1)&& menkeCourseIds.Contains(u.MenkeCourseId) ).ToList();
			var list_Apply = _context.Queryable<WebUserLessonCancel>().Where(u => list_UserLesson.Select(s => s.UserLessonid).Contains(u.UserLessonid) && u.Status != 1).ToList();
			foreach (var model in list_Apply) {
				model.Status = 1;
			}
			_context.Updateable(list_Apply).UpdateColumns(u => u.Status).ExecuteCommand();
		}
		#endregion

		#region 按手机号查询拓课云用户ID
		/// <summary>
		/// 按手机号查询拓课云用户ID
		/// </summary>
		/// <param name="code"></param>
		/// <param name="mobile"></param>
		/// <param name="sty">0学生，1老师</param>
		/// <returns></returns>
		public async Task<ApiResult<int>> GetMenkeUserId(string code,string mobile,int sty=0) {
			string url = "",json="";
			JObject o = null;
			var result = new ApiResult<int>();
			var dic_header = new Dictionary<string, string>
			{
				{ "key", Appsettings.app("Web:MenkeKey") },
				{ "version", "v1" }
			};
			mobile = (mobile+"").Replace(" ", "").Replace("-", "");
			if (string.IsNullOrEmpty(mobile)){
				result.StatusCode = 4003;
				result.Message = "手机号不能为空";
				return result;
            }
			if (sty == 1)
			{
                //1老师
                url = Appsettings.app("APIS:menke_user_teacher") + "/" + code + "-" + mobile;
                json = GetRemoteHelper.HttpWebRequestUrl(url, "", "utf-8", "get", dic_header, "application/json");
                o = JObject.Parse(json);
                if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
                {
                    result.Data = (o["data"]["userid"] != null ? (int)o["data"]["userid"] : 0);
                }
                else
                {
                    result.StatusCode = 4002;
                    result.Message = "同步查询拓课云老师详情接口异常,json=>" + json;
                }
            }
			else {
                //0学生
                url = Appsettings.app("APIS:menke_user_student") + "/" + code + "-" + mobile;
                json = GetRemoteHelper.HttpWebRequestUrl(url, "", "utf-8", "get", dic_header, "application/json");
                o = JObject.Parse(json);
                if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
                {
                    result.Data = (o["data"]["userid"] != null ? (int)o["data"]["userid"] : 0);
                }
                else
                {
                    result.StatusCode = 4002;
                    result.Message = "同步查询拓课云学生详情接口异常,json=>" + json;
                }
            }
			return result;
		}
		#endregion

		#region 创建学生
		/// <summary>
		/// 创建学生
		/// </summary>
		/// <param name="name">姓名</param>
		/// <param name="nickname">昵称</param>
		/// <param name="sex">性别(0女1男)</param>
		/// <param name="birthday">生日</param>
		/// <param name="code">手机区号</param>
		/// <param name="mobile">手机号</param>
		/// <param name="p_name">家长姓名</param>
		/// <returns>menke_user_id</returns>
		public async Task<ApiResult<int>> CreateStudents(MenkeStudentDto data)
		{
			var result = new ApiResult<int>();
			data.mobile = data.mobile.Replace("-", "").Replace(" ", "");
			string url = Appsettings.app("APIS:menke_students");
			var list_dic_data=new List<MenkeStudentDto>();
			list_dic_data.Add(data);
			var data1 = new
			{
				users = list_dic_data
			};
			var dic_header = new Dictionary<string, string>
			{
				{ "key", Appsettings.app("Web:MenkeKey") },
				{ "version", "v2" }
			};
			var json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(data1, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "utf-8", "post", dic_header, "application/json");
			var o = JObject.Parse(json);
			if (o != null && o["result"] != null)
			{
				if (o["result"].ToString() == "0")
				{
					result.StatusCode = 0;
					var jarry_user_id = o["data"] != null ? (JArray)o["data"] : new JArray();
					if (jarry_user_id.Count > 0) {
						result.Data = (int)jarry_user_id[0];
					}
				}
				else {
					result.StatusCode = (int)o["result"];
					result.Message = o["msg"]!=null? o["msg"].ToString():"未知错误!";
					if (result.Message.Contains("已存在")) {
                        //触发查询menke_user_id
                        return await GetMenkeUserId(data.code ,data.mobile,0);
                    }
				}
			}
			else {
                _logger.LogError("创建拓课云学生接口失败[" + data.mobile + "]上课学生,接口异常,json=>" + json);
                result.StatusCode = 4002;
				result.Message = "创建拓课云学生接口失败";
			}
			return result;
		}
		#endregion

		#region 修改学生信息
		/// <summary>
		/// 修改学生信息
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public async Task<ApiResult> ModifyStudent(string mobile_code,string mobile,MenkeStudentDto data)
		{
			var result = new ApiResult<int>();

			string url = string.Format(Appsettings.app("APIS:menke_modify_student"), mobile_code +"-"+ mobile);

			var data1 = new
			{
				name = data.name,
				nickname = data.nickname,
				sex = data.sex,
				birthday = data.birthday,
				p_name = data.p_name+""
			};
			var dic_header = new Dictionary<string, string>
			{
				{ "key", Appsettings.app("Web:MenkeKey") },
				{ "version", "v1" }
			};
			var json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(data1, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "utf-8", "put", dic_header, "application/json");
			var o = JObject.Parse(json);
			if (o != null && o["result"] != null)
			{
				if (o["result"].ToString() == "0")
				{
					result.StatusCode = 0;
				}
				else
				{
					string msg = o["msg"] != null ? o["msg"].ToString() : "未知错误!";
                    result.StatusCode = (int)o["result"];
                    result.Message = msg;
                }
			}
			else
			{
				result.StatusCode = 4002;
				result.Message = "修改拓课云学生资料失败";
			}
			return result;
		}
		#endregion

		#region 创建老师
		/// <summary>
		/// 创建老师
		/// </summary>
		/// <param name="name">姓名</param>
		/// <param name="sex">性别(0女1男)</param>
		/// <param name="birthday">生日</param>
		/// <param name="email">邮箱</param>
		/// <param name="code">手机区号</param>
		/// <param name="mobile">手机号</param>
		/// <param name="pwd">密码（不填默认手机号后8位）</param>
		/// <returns>menke_user_id</returns>
		public async Task<ApiResult<int>> CreateTeachers(MenkeTeacherDto data)
		{
			var result = new ApiResult<int>();
			string url = Appsettings.app("APIS:menke_teachers");
			var list_dic_data = new List<MenkeTeacherDto>();
			list_dic_data.Add(data);
			var data1 = new
			{
				users = list_dic_data
			};
			var dic_header = new Dictionary<string, string>();
			dic_header.Add("key", Appsettings.app("Web:MenkeKey"));
			dic_header.Add("version", "v2");
			var json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(data1, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "utf-8", "post", dic_header, "application/json");
			var o = JObject.Parse(json);
			if (o != null && o["result"] != null)
			{
				if (o["result"].ToString() == "0")
				{
					result.StatusCode = 0;
					var jarry_user_id = o["data"] != null ? (JArray)o["data"] : new JArray();
					if (jarry_user_id.Count > 0)
					{
						result.Data = (int)jarry_user_id[0];
					}
				}
				else
				{
					result.StatusCode = (int)o["result"];
					result.Message = o["msg"] != null ? o["msg"].ToString() : "未知错误!";
					if (result.Message.Contains("已存在"))
					{
						//触发查询menke_user_id
						return await GetMenkeUserId(data.code, data.mobile, 1);
					}
				}
			}
			else
			{
				result.StatusCode = 4002;
				result.Message = "创建拓课云老师账号失败";
			}
			return result;
		}
		#endregion

		#region 修改老师信息
		/// <summary>
		/// 修改老师信息
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public async Task<ApiResult> ModifyTeacher(string mobile_code, string mobile, MenkeTeacherDto data)
		{
			var result = new ApiResult<int>();

			string url = string.Format(Appsettings.app("APIS:menke_modify_teacher"), mobile_code+"-" + mobile);

			var data1 = new
			{
				name = data.name,
				sex = data.sex,
				birthday = data.birthday,
				email = data.email
			};
			var dic_header = new Dictionary<string, string>
			{
				{ "key", Appsettings.app("Web:MenkeKey") },
				{ "version", "v1" }
			};
			var json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(data1, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "utf-8", "put", dic_header, "application/json");
			var o = JObject.Parse(json);
			if (o != null && o["result"] != null)
			{
				if (o["result"].ToString() == "0")
				{
					result.StatusCode = 0;
				}
				else
				{
					result.StatusCode = (int)o["result"];
					result.Message = o["msg"] != null ? o["msg"].ToString() : "未知错误!";
				}
			}
			else
			{
				result.StatusCode = 4002;
				result.Message = "修改拓课云老师信息失败";
			}
			return result;
		}
		#endregion

		#region 同步课堂回放信息
		/// <summary>
		/// 同步课堂回放信息
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> RecordSync(string starttime)
		{
			var result = new ApiResult();
			var list_MenkeRecord = new List<MenkeRecord>();
			var list_UserLesson = new List<WebUserLesson>();
			var list_User = new List<WebUser>();
			int page = 1, last_page = 1;
			string update_starttime = string.IsNullOrEmpty(starttime) ? DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddDays(-7)).ToString() : starttime;
			var begintime = DateHelper.ConvertIntToDateTime(update_starttime);
			var endtime = begintime.AddDays(1);
			if (endtime > DateTime.UtcNow) endtime = DateTime.UtcNow;
			string update_endtime = DateHelper.ConvertDateTimeInt(endtime).ToString();

			string url = Appsettings.app("APIS:menke_lesson_record");
			var dic_data = new Dictionary<string, object>();
			var dic_header = new Dictionary<string, string>();
			dic_header.Add("key", Appsettings.app("Web:MenkeKey"));
			dic_header.Add("version", "v1");
			do
            {
				dic_data.Clear();
                dic_data.Add("page", page);
                if (!string.IsNullOrEmpty(update_starttime)) dic_data.Add("start_time", update_starttime);
                if (!string.IsNullOrEmpty(update_endtime)) dic_data.Add("end_time", update_endtime);
                var data = string.Join("&", dic_data.Select(u => u.Key + "=" + u.Value).ToList());
                var json = GetRemoteHelper.HttpWebRequestUrl(url + "?" + data, "", "utf-8", "get", dic_header);
				var o = JObject.Parse(json);
				if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
				{
					last_page = (o["data"]["last_page"] != null) ? (int)o["data"]["last_page"] : 1;
					if (o["data"]["data"] != null)
					{
						var jarray = (JArray)o["data"]["data"];
						foreach (var item in jarray)
						{
                            //同步课节回放信息
                            list_MenkeRecord.Add(new MenkeRecord()
							{
								MenkeLessonId = (item["room_id"] != null) ? (int)item["room_id"] : 0,
								MenkeRecordtitle = (item["recordtitle"] != null) ? item["recordtitle"].ToString() : "",
								MenkeDuration = (item["duration"] != null) ? (int)item["duration"] : 0,
                                MenkeSize = (item["size"] != null) ? (long)item["size"] : 0,
                                MenkeType = (item["type"] != null) ? item["type"].ToString() : "",
                                MenkeRecordid = (item["recordid"] != null) ? item["recordid"].ToString() : "",
								MenkeUrl = (item["url"] != null) ? item["url"].ToString() : "", 						
								MenkeStarttime = (item["starttime"] != null) ? (int)item["starttime"] : 0,
								Json = json
							});
						}
					}
				}
				else
				{
					page = last_page;//退出
				}
				page++;
			} while (page <= last_page);//page>=last_page退出，至少执行1次
                                        //执行同步课节信息
            if (list_MenkeRecord.Count > 0)
            {
                var x = _context.Storageable(list_MenkeRecord).WhereColumns(u => u.MenkeLessonId).ToStorage();
                x.AsInsertable.ExecuteCommand();
                x.AsUpdateable.UpdateColumns(u => new {
                    u.MenkeLessonId,
                    u.MenkeRecordid,
                    u.MenkeRecordtitle,
                    u.MenkeDuration,
                    u.MenkeSize,
                    u.MenkeType,
                    u.MenkeUrl,
                    u.MenkeStarttime,
                    u.Json
                }).ExecuteCommand();

            }
            _context.Updateable<UeTask>().SetColumns(u => u.Paramenter == update_endtime.ToString()).Where(u => u.Code == "MenkeRecord").ExecuteCommand();
			return result;
		}
		#endregion

		#region 同步课节考勤信息
		/// <summary>
		/// 同步课节考勤信息
		/// </summary>
		/// <param name="search_day">UTC日期</param>
		/// <returns></returns>
		public async Task<ApiResult> AttendanceSync(string search_day)
		{
			var result = new ApiResult();
			var list_MenkeAttendance = new List<MenkeAttendance>();
			var list_User = new List<WebUser>();
			int page = 1, last_page = 1;
			search_day = string.IsNullOrEmpty(search_day) ? DateTime.UtcNow.AddDays(-7).ToString("d") : search_day;
			var begintime = DateTime.Parse(search_day);
			if (begintime.Date >= DateTime.UtcNow.Date) return result;//只拉今天之前的


			string url = Appsettings.app("APIS:menke_lesson_attendance");
			var dic_data = new Dictionary<string, object>();
			var dic_header = new Dictionary<string, string>();
			dic_header.Add("key", Appsettings.app("Web:MenkeKey"));
			dic_header.Add("version", "v1");
			do
            {
				dic_data.Clear();
                dic_data.Add("page", page);
                dic_data.Add("search_day", search_day);
                var data = string.Join("&", dic_data.Select(u => u.Key + "=" + u.Value).ToList());
                var json = GetRemoteHelper.HttpWebRequestUrl(url + "?" + data, "", "utf-8", "get", dic_header);
				var o = JObject.Parse(json);
				if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
				{
					last_page = (o["data"]["last_page"] != null) ? (int)o["data"]["last_page"] : 1;
					if (o["data"]["data"] != null)
					{
						var jarray = (JArray)o["data"]["data"];
						var list_MenkeLessonid = jarray.Select(u => (int)u["id"]).ToList();
						foreach (var item in jarray)
						{
							if (item["attendance"] != null)
							{
								var jarray_attendance = (JArray)item["attendance"];
								foreach (var item_attendance in jarray_attendance)
								{
									//同步考勤信息
									list_MenkeAttendance.Add(new MenkeAttendance()
                                    {
                                        MenkeLessonId = (item["id"] != null) ? (int)item["id"] : 0,
                                        MenkeStarttime = (item["starttime"] != null) ? (int)item["starttime"] : 0,
                                        MenkeEndtime = (item["endtime"] != null) ? (int)item["endtime"] : 0,
                                        MenkeAttendanceMobileCode = (item_attendance["code"] != null) ? item_attendance["code"].ToString() : "",
                                        MenkeAttendanceMobile = (item_attendance["mobile"] != null) ? item_attendance["mobile"].ToString() : "",
                                        MenkeAttendanceUserroleid = (item_attendance["userroleid"] != null) ? (int)item_attendance["userroleid"] : 0,
                                        MenkeAttendanceAccessRecord = (item_attendance["access_record"] != null) ? item_attendance["access_record"].ToString() : "",
                                        MenkeAttendanceLate = (item_attendance["late"] != null) ? (int)item_attendance["late"] : 0,
                                        MenkeAttendanceLeaveEarly = (item_attendance["leave_early"] != null) ? (int)item_attendance["leave_early"] : 0,
                                        MenkeAttendanceAbsent = (item_attendance["absent"] != null) ? (int)item_attendance["absent"] : 0,
                                        MenkeTimeinfo = (item["timeinfo"] != null) ? item["timeinfo"].ToString() : "",
                                        Status = 0,
										Json = json
									});
								}
							}
						}
					}
				}
				else
				{
					page = last_page;//退出
				}
				page++;
			} while (page <= last_page);//page>=last_page退出，至少执行1次

			//执行同步课节信息
			if (list_MenkeAttendance.Count > 0)
			{
				var x = _context.Storageable(list_MenkeAttendance).WhereColumns(u => new { u.MenkeLessonId, u.MenkeAttendanceMobileCode, u.MenkeAttendanceMobile, u.MenkeAttendanceAccessRecord }).ToStorage();
				x.AsInsertable.ExecuteCommand();
                x.AsUpdateable.UpdateColumns(u => new {
                    u.MenkeLessonId,
                    u.MenkeStarttime,
                    u.MenkeEndtime,
                    u.MenkeAttendanceMobileCode,
                    u.MenkeAttendanceMobile,
                    u.MenkeAttendanceUserroleid,
                    u.MenkeAttendanceAccessRecord,
                    u.MenkeAttendanceLate,
                    u.MenkeAttendanceLeaveEarly,
                    u.MenkeAttendanceAbsent,
                    u.MenkeTimeinfo,
                    u.Json
                }).ExecuteCommand();

            }
			search_day = begintime.AddDays(1).ToString("d");
			_context.Updateable<UeTask>().SetColumns(u => u.Paramenter == search_day).Where(u => u.Code == "MenkeAttendance").ExecuteCommand();
			return result;
		}
		#endregion

		#region 同步作业列表
		/// <summary>
		/// 同步作业列表
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> HomeworkSync(string starttime)
        {
            var result = new ApiResult();
            var list_MenkeHomework = new List<MenkeHomework>();
            var list_User = new List<WebUser>();
            int page = 1, last_page = 1;
            string update_starttime = string.IsNullOrEmpty(starttime) ? DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddDays(-7)).ToString() : starttime;
            var begintime = DateHelper.ConvertIntToDateTime(update_starttime);
            var endtime = begintime.AddDays(1);
            if (endtime > DateTime.UtcNow) endtime = DateTime.UtcNow;
            string update_endtime = DateHelper.ConvertDateTimeInt(endtime).ToString();

            string url = Appsettings.app("APIS:menke_homework");
            var dic_data = new Dictionary<string, object>();
            var dic_header = new Dictionary<string, string>();
            dic_header.Add("key", Appsettings.app("Web:MenkeKey"));
            dic_header.Add("version", "v1");
            do
            {
				dic_data.Clear();
                dic_data.Add("page", page);
                if (!string.IsNullOrEmpty(update_starttime)) dic_data.Add("start_time", update_starttime);
                if (!string.IsNullOrEmpty(update_endtime)) dic_data.Add("end_time", update_endtime);
                var data = string.Join("&", dic_data.Select(u => u.Key + "=" + u.Value).ToList());
                var json = GetRemoteHelper.HttpWebRequestUrl(url + "?" + data, "", "utf-8", "get", dic_header);
                var o = JObject.Parse(json);
                if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
                {
                    last_page = (o["data"]["last_page"] != null) ? (int)o["data"]["last_page"] : 1;
                    if (o["data"]["data"] != null)
                    {
                        var jarray = (JArray)o["data"]["data"];
                        foreach (var item in jarray)
                        {
							list_MenkeHomework.Add(new MenkeHomework()
							{
								MenkeHomeworkId= (item["id"] != null) ? (int)item["id"] : 0,
                                MenkeDay = (item["day"] != null) ? item["day"].ToString() : "",
                                MenkeTitle = (item["title"] != null) ? item["title"].ToString() : "",
                                MenkeContent = (item["content"] != null) ? item["content"].ToString() : "",
                                MenkeResources = (item["resources"] != null) ? item["resources"].ToString() : "",
                                MenkeSubmitWay = (item["submit_way"] != null) ? (int)item["submit_way"] : 0,
                                MenkeIsDraft = (item["is_draft"] != null) ? (int)item["is_draft"] : 0,
                                MenkeLessonId = (item["room_id"] != null) ? (int)item["room_id"] : 0,
                                MenkeDeleteTime = (item["delete_time"] != null) ? (int)item["delete_time"] : 0,
                                MenkeUpdateTime = (item["update_time"] != null) ? (int)item["update_time"] : 0,
                                MenkeCreateTime = (item["create_time"] != null) ? (int)item["create_time"] : 0,
                                MenkeEndDate = (item["end_date"] != null) ? (int)item["end_date"] : 0,
								MenkeAccount = (item["account"] != null) ? item["account"].ToString() : "",
                                Json = json,
                                Status = 1
                            });
                        }
                    }
                }
                else
                {
                    page = last_page;//退出
                }
                page++;
            } while (page <= last_page);//page>=last_page退出，至少执行1次
                                        
            if (list_MenkeHomework.Count > 0)
            {
                var x = _context.Storageable(list_MenkeHomework).WhereColumns(u => new { u.MenkeLessonId, u.MenkeHomeworkId }).ToStorage();
                x.AsInsertable.ExecuteCommand();
                x.AsUpdateable.UpdateColumns(u=>new { 
					u.MenkeHomeworkId,u.MenkeDay,u.MenkeTitle,u.MenkeContent,u.MenkeResources,u.MenkeSubmitWay,u.MenkeIsDraft,u.MenkeLessonId,u.MenkeDeleteTime,u.MenkeUpdateTime,u.MenkeCreateTime,
					u.MenkeEndDate,u.MenkeAccount,u.Json
                }).ExecuteCommand();
            }
            _context.Updateable<UeTask>().SetColumns(u => u.Paramenter == update_endtime.ToString()).Where(u => u.Code == "MenkeHomework").ExecuteCommand();
            return result;
        }
		#endregion

		#region 同步作业提交记录列表
		/// <summary>
		/// 同步作业提交记录列表
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> HomeworkSubmitSync(string starttime)
        {
            var result = new ApiResult();
            var list_MenkeHomework = new List<MenkeHomeworkSubmit>();
            var list_User = new List<WebUser>();
            int page = 1, last_page = 1;
            string update_starttime = string.IsNullOrEmpty(starttime) ? DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddDays(-7)).ToString() : starttime;
            var begintime = DateHelper.ConvertIntToDateTime(update_starttime);
            var endtime = begintime.AddDays(1);
            if (endtime > DateTime.UtcNow) endtime = DateTime.UtcNow;
            string update_endtime = DateHelper.ConvertDateTimeInt(endtime).ToString();

            string url = Appsettings.app("APIS:menke_homework_submit");
            var dic_header = new Dictionary<string, string>();
            dic_header.Add("key", Appsettings.app("Web:MenkeKey"));
            dic_header.Add("version", "v1");
            var dic_data = new Dictionary<string, object>();
            do
            {
				dic_data.Clear();
                dic_data.Add("page", page);
                if (!string.IsNullOrEmpty(update_starttime)) dic_data.Add("start_time", update_starttime);
                if (!string.IsNullOrEmpty(update_endtime)) dic_data.Add("end_time", update_endtime);
                var data = string.Join("&", dic_data.Select(u => u.Key + "=" + u.Value).ToList());
                var json = GetRemoteHelper.HttpWebRequestUrl(url + "?" + data, "", "utf-8", "get", dic_header);
                var o = JObject.Parse(json);
                if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
                {
                    last_page = (o["data"]["last_page"] != null) ? (int)o["data"]["last_page"] : 1;
                    if (o["data"]["data"] != null)
                    {
                        var jarray = (JArray)o["data"]["data"];
                        foreach (var item in jarray)
                        {
							string code = "", mobile = "";
							string student_account = (item["student_account"] != null) ? item["student_account"].ToString() : "";
							if (student_account.Contains("-"))
							{
								var arr = student_account.Split('-');
								if (arr.Length > 0) code = arr[0];
								if (arr.Length > 1) mobile = arr[1];
							}
							else {
								mobile = student_account;
                            }
							list_MenkeHomework.Add(new MenkeHomeworkSubmit()
							{
								MenkeSubmitId = (item["id"] != null) ? (int)item["id"] : 0,
								MenkeHomeworkId = (item["homework_id"] != null) ? (int)item["homework_id"] : 0,
								MenkeSubmitTime = (item["submit_time"] != null) ? (int)item["submit_time"] : 0,
								MenkeSubmitDelTime = (item["submit_del_time"] != null) ? (int)item["submit_del_time"] : 0,
								MenkeSubmitContent = (item["submit_content"] != null) ? item["submit_content"].ToString() : "",
								MenkeSubmitFiles = (item["submit_files"] != null) ? item["submit_files"].ToString() : "",
								MenkeStudentCode = code,
								MenkeStudentMobile = mobile,
                                Json = json,
                                Status = 1
                            });
                        }
                    }

                }
                else
                {
                    page = last_page;//退出
                }
                page++;
            } while (page <= last_page);//page>=last_page退出，至少执行1次
          
            if (list_MenkeHomework.Count > 0)
            {
                var x = _context.Storageable(list_MenkeHomework).WhereColumns(u => new { u.MenkeHomeworkId, u.MenkeSubmitId }).ToStorage();
                x.AsInsertable.ExecuteCommand();
                x.AsUpdateable.UpdateColumns(u=>new { 
					u.MenkeSubmitId,u.MenkeHomeworkId,u.MenkeSubmitTime,u.MenkeSubmitDelTime,u.MenkeSubmitContent,u.MenkeSubmitFiles,u.MenkeStudentCode,u.MenkeStudentMobile,u.Json
				}).ExecuteCommand();
            }
            _context.Updateable<UeTask>().SetColumns(u => u.Paramenter == update_endtime.ToString()).Where(u => u.Code == "MenkeHomeworkSubmit").ExecuteCommand();
            return result;
        }
		#endregion

		#region 同步作业记录点评列表
		/// <summary>
		/// 同步作业记录点评列表
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> HomeworkRemarkSync(string starttime)
        {
            var result = new ApiResult();
            var list_MenkeHomework = new List<MenkeHomeworkRemark>();
            var list_User = new List<WebUser>();
            int page = 1, last_page = 1;
            string update_starttime = string.IsNullOrEmpty(starttime) ? DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddDays(-7)).ToString() : starttime;
            var begintime = DateHelper.ConvertIntToDateTime(update_starttime);
            var endtime = begintime.AddDays(1);
            if (endtime > DateTime.UtcNow) endtime = DateTime.UtcNow;
            string update_endtime = DateHelper.ConvertDateTimeInt(endtime).ToString();

            string url = Appsettings.app("APIS:menke_homework_remark");
            var dic_header = new Dictionary<string, string>();
            dic_header.Add("key", Appsettings.app("Web:MenkeKey"));
            dic_header.Add("version", "v1");
            var dic_data = new Dictionary<string, object>();
            do
            {
                dic_data.Clear();
                dic_data.Add("page", page);
                if (!string.IsNullOrEmpty(update_starttime)) dic_data.Add("start_time", update_starttime);
                if (!string.IsNullOrEmpty(update_endtime)) dic_data.Add("end_time", update_endtime);
                var data = string.Join("&", dic_data.Select(u => u.Key + "=" + u.Value).ToList());
                var json = GetRemoteHelper.HttpWebRequestUrl(url + "?" + data, "", "utf-8", "get", dic_header);
                var o = JObject.Parse(json);
                if (o != null && o["result"] != null && o["result"].ToString() == "0" && o["data"] != null)
                {
                    last_page = (o["data"]["last_page"] != null) ? (int)o["data"]["last_page"] : 1;
                    if (o["data"]["data"] != null)
                    {
                        var jarray = (JArray)o["data"]["data"];
                        foreach (var item in jarray)
                        {
                            string code = "", mobile = "";
                            string teacher_account = (item["teacher_account"] != null) ? item["teacher_account"].ToString() : "";
                            if (teacher_account.Contains("-"))
                            {
                                var arr = teacher_account.Split('-');
                                if (arr.Length > 0) code = arr[0];
                                if (arr.Length > 1) mobile = arr[1];
                            }
                            else
                            {
                                mobile = teacher_account;
                            }
							list_MenkeHomework.Add(new MenkeHomeworkRemark()
							{
								MenkeSubmitId = (item["id"] != null) ? (int)item["id"] : 0,
								MenkeHomeworkId = (item["homework_id"] != null) ? (int)item["homework_id"] : 0,
								MenkeIsPass = (item["is_pass"] != null) ? (int)item["is_pass"] : 0,
								MenkeRank = (item["rank"] != null) ? (int)item["rank"] : 0,
								MenkeRemarkTime = (item["remark_time"] != null) ? (int)item["remark_time"] : 0,
								MenkeRemarkDelTime = (item["remark_del_time"] != null) ? (int)item["remark_del_time"] : 0,
								MenkeRemarkContent = (item["remark_content"] != null) ? item["remark_content"].ToString() : "",
								MenkeRemarkFiles = (item["remark_files"] != null) ? item["remark_files"].ToString() : "",
								MenkeTeacherCode = code,
								MenkeTeacherMobile = mobile,
								Json = json,
                                Status = 1
                            }) ;
                        }
                    }
                }
                else
                {
                    page = last_page;//退出
                }
                page++;
            } while (page <= last_page);//page>=last_page退出，至少执行1次
            if (list_MenkeHomework.Count > 0)
            {
                var x = _context.Storageable(list_MenkeHomework).WhereColumns(u => new { u.MenkeHomeworkId, u.MenkeSubmitId }).ToStorage();
                x.AsInsertable.ExecuteCommand();
                x.AsUpdateable.ExecuteCommand();
            }
            _context.Updateable<UeTask>().SetColumns(u => u.Paramenter == update_endtime.ToString()).Where(u => u.Code == "MenkeHomeworkRemark").ExecuteCommand();
            return result;
		}
        #endregion

        #region 教室模板
        /// <summary>
        /// 教室模板
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ApiResult<List<MenkeTemplateDto>>> GetTemplate()
        {
            var result = new ApiResult<List<MenkeTemplateDto>>();

            string url = Appsettings.app("APIS:menke_template");

            var dic_header = new Dictionary<string, string>
            {
                { "key", Appsettings.app("Web:MenkeKey") },
                { "version", "v1" }
            };
            var json = GetRemoteHelper.HttpWebRequestUrl(url,"", "utf-8", "get", dic_header, "application/json");
            var o = JObject.Parse(json);
            if (o != null && o["result"] != null)
            {
                if (o["result"].ToString() == "0")
                {
                    result.StatusCode = 0;
					var jarray = (JArray)o["data"];
					result.Data = jarray.Select(u => new MenkeTemplateDto()
					{
						menke_template_id = (int)u["id"],
						menke_name = u["name"].ToString(),
						menke_type = (int)u["type"]
					}).ToList();
                }
                else
                {
                    result.StatusCode = (int)o["result"];
                    result.Message = o["msg"] != null ? o["msg"].ToString() : "未知错误!";
					_logger.LogError("获取[拓课云]模板失败,json=>" + json);
				}
            }
            else
            {
                result.StatusCode = 4002;
                result.Message = "获取[拓课云]模板失败";
				_logger.LogError("获取[拓课云]模板失败,json=>" + json);
			}
            return result;
        }
        #endregion

        #region 创建课程
        /// <summary>
        /// 创建课程
        /// </summary>
		/// <param name="name">课程名称</param>
		/// <param name="students">学生</param>
		/// <param name="room_template_id">房间模板ID</param>
        /// <returns></returns>
        public async Task<ApiResult<int>> CreateCourse(string name, List<string> students, int room_template_id)
        {
            var result = new ApiResult<int>();

            string url = Appsettings.app("APIS:menke_course");
			name += "";
            if (name.Trim().Length > 50) name = name.Trim().Substring(0, 50);
            var data = new
            {
                name = name,
                students = students,
                room_template_id = room_template_id
            };
            var dic_header = new Dictionary<string, string>
            {
                { "key", Appsettings.app("Web:MenkeKey") },
                { "version", "v1" }
            };
			string data_str = JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var json = GetRemoteHelper.HttpWebRequestUrl(url, data_str, "utf-8", "post", dic_header, "application/json");
            var o = JObject.Parse(json);
            if (o != null && o["result"] != null)
            {
                if (o["result"].ToString() == "0")
                {
                    result.StatusCode = 0;
					result.Data = (int)(o["data"]["id"]);
					/////创建完成后，主动拉下课程更新
					/////这步无意义，做废。因为拓课云课程的接口取数据需要时间，还没准备好，拉了也是白拉
					//await CourseSync(DateHelper.ConvertDateTimeInt(DateTime.UtcNow).ToString());
                }
                else
                {
                    result.StatusCode = (int)o["result"];
                    result.Message = o["msg"] != null ? o["msg"].ToString() : "未知错误!";
					_logger.LogError("创建课程失败,data=>"+ data_str + ",json=>" + json);
				}
            }
            else
            {
                result.StatusCode = 4002;
                result.Message = "创建课程失败:" + json;
				_logger.LogError("创建课程失败,data=>"+ data_str + ",json=>" + json);
			}
            return result;
        }
        #endregion

        #region 编辑课程
        /// <summary>
		/// 编辑课程
		/// </summary>
		/// <param name="menke_course_id">门课ID</param>
		/// <param name="name">课程名称</param>
		/// <param name="students">学生，可为null</param>
		/// <param name="room_template_id">房间模板ID</param>
		/// <returns></returns>
        public async Task<ApiResult> ModifyCourse(int menke_course_id,string name, List<string> students, int room_template_id)
        {
            var result = new ApiResult();

            string url = string.Format(Appsettings.app("APIS:menke_modify_course"),menke_course_id);
            name += "";
            if (name.Trim().Length > 50) name = name.Trim().Substring(0, 50);
            var data = new
            {
                name = name,
                students = students,
                room_template_id = room_template_id
            };
            var dic_header = new Dictionary<string, string>
            {
                { "key", Appsettings.app("Web:MenkeKey") },
                { "version", "v1" }
            };
			string data_str = JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var json = GetRemoteHelper.HttpWebRequestUrl(url, data_str, "utf-8", "put", dic_header, "application/json");
            var o = JObject.Parse(json);
            if (o != null && o["result"] != null)
            {
                if (o["result"].ToString() == "0")
                {
                    result.StatusCode = 0;
					////修改完成后，主动拉下课程更新
					///这步无意义，做废。因为拓课云课程的接口取数据需要时间，还没准备好，拉了也是白拉
					//await CourseSync(DateHelper.ConvertDateTimeInt(DateTime.UtcNow).ToString());
				}
                else
                {
                    result.StatusCode = (int)o["result"];
                    result.Message = o["msg"] != null ? o["msg"].ToString() : "未知错误!";
					_logger.LogError("编辑课程失败,data=>"+ data_str + ",json=>" + json);
				}
            }
            else
            {
                result.StatusCode = 4002;
                result.Message = "编辑课程失败:" + json;
				_logger.LogError("编辑课程失败,data=>"+ data_str + ",menke_course_id="+ menke_course_id + ",json=>" + json);
			}
            return result;
        }
		#endregion

		#region 删除课程
		/// <summary>
		/// 删除课程
		/// </summary>
		/// <param name="menke_course_id">门课ID</param>
		/// <param name="name">课程名称</param>
		/// <param name="students">学生</param>
		/// <param name="room_template_id">房间模板ID</param>
		/// <returns></returns>
		public async Task<ApiResult> DelCourse(int menke_course_id)
		{
			var result = new ApiResult();

			string url = string.Format(Appsettings.app("APIS:menke_modify_course"), menke_course_id);

			var dic_header = new Dictionary<string, string>
			{
				{ "key", Appsettings.app("Web:MenkeKey") },
				{ "version", "v1" }
			};
			var json = GetRemoteHelper.HttpWebRequestUrl(url,"", "utf-8", "delete", dic_header, "application/json");
			var o = JObject.Parse(json);
			if (o != null && o["result"] != null)
			{
				if (o["result"].ToString() == "0")
				{
					result.StatusCode = 0;
					/////修改完成后，主动拉下课程更新
					/////这步无意义，做废。因为拓课云课程的接口取数据需要时间，还没准备好，拉了也是白拉
					//await CourseSync(DateHelper.ConvertDateTimeInt(DateTime.UtcNow).ToString());
				}
				else
				{
					result.StatusCode = (int)o["result"];
					result.Message = o["msg"] != null ? o["msg"].ToString() : "未知错误!";
				}
			}
			else
			{
				result.StatusCode = 4002;
				result.Message = "删除课程失败:" + json;
				_logger.LogError("删除课程失败,menke_course_id="+ menke_course_id + ",json=>" + json);
			}
			return result;
		}
        #endregion


    }
}