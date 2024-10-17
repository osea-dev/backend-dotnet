using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Base;
using WeTalk.Models;
using WeTalk.Models.Dto.Menke;

namespace WeTalk.Interfaces.Services
{
	public partial class MenkeService : BaseService, IMenkeService
	{
		private readonly SqlSugarScope _context;
		private readonly ILogger<MenkeService> _logger;
		private readonly IMenkeBaseService _menkeBaseService;
		private readonly IMessageBaseService _messageBaseService;
		private readonly IPubConfigBaseService _pubConfigBaseService;
		private readonly ICommonBaseService _commonBaseService;

		private readonly IUserManage _userManage;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		private readonly ISobotBaseService _sobotBaseService;
		public MenkeService(SqlSugarScope dbcontext, ILogger<MenkeService> logger, IStringLocalizer<LangResource> localizer, IUserManage userManage,IMenkeBaseService menkeBaseService, IMessageBaseService messageBaseService,
			IPubConfigBaseService pubConfigBaseService, ISobotBaseService sobotBaseService, ICommonBaseService commonBaseService)
		{
			_context = dbcontext;
			_logger = logger;
			_menkeBaseService = menkeBaseService;
			_messageBaseService = messageBaseService;
			_pubConfigBaseService = pubConfigBaseService;
			_commonBaseService = commonBaseService;
			_localizer = localizer;
			_userManage = userManage;
			_sobotBaseService = sobotBaseService;
		}

		/// <summary>
		/// 同步课程信息
		/// </summary>
		/// <param name="starttime">开始时间戳</param>
		/// <returns></returns>
		public async Task<ApiResult> CourseSync(string starttime)
		{
			return await _menkeBaseService.CourseSync(starttime);
		}

		/// <summary>
		/// 同步课节信息(只能同步一天)
		/// </summary>
		/// <param name="starttime">开始时间戳</param>
		/// <returns></returns>
		public async Task<ApiResult<List<long>>> LessonSync(string starttime, int menke_userid = 0, int menke_course_id = 0)
		{
			await SuccessionEmail();//续课提醒
			return await _menkeBaseService.LessonSync(starttime, menke_userid, menke_course_id);
		}
		/// <summary>
		/// 执行拆分课节
		/// </summary>
		/// <param name="list_MenkeLesson">要处理的课节信息,为null时则按count取数</param>
		/// <param name="reset">是否重置isexe=0,0否，1是</param>
		/// <param name="count">数量</param>
		/// <returns></returns>
		public async Task<ApiResult> LessonSplit(List<MenkeLesson> list_MenkeLesson = null, int reset = 0, int count = 100) {
			return await _menkeBaseService.LessonSplit(list_MenkeLesson, reset, count);
		}
		/// <summary>
		/// 删除课节
		/// </summary>
		/// <param name="menke_lessonid"></param>
		/// <returns></returns>
		public async Task<ApiResult> DeleteLesson(int menke_lessonid) {
			return await _menkeBaseService.DeleteLesson(menke_lessonid);
        }

        #region 编辑课节中的上课学生
        /// <summary>
        /// 编辑课节中的上课学生
        /// </summary>
        /// <param name="menke_lessonid"></param>
        /// <param name="mobiles">["86-13145678912"]</param>
        /// <returns></returns>
        public async Task<ApiResult> ModifyLessonStudent(MenkeLesson model_MenkeLesson, List<string> mobiles)
        {
			return await _menkeBaseService.ModifyLessonStudent(model_MenkeLesson, mobiles);
        }
        public async Task<ApiResult> ModifyLessonStudent(int menke_lessonid, List<string> mobiles)
        {
            return await _menkeBaseService.ModifyLessonStudent(menke_lessonid, mobiles);
        }
        #endregion

        /// <summary>
        /// 补充关联排课表中Userid,课程及SKU id
        /// </summary>
        /// <param name="userLessonid"></param>
        /// <returns></returns>
        public async Task UnionLessonUserid(List<int> menkeLessonIds) {
			await _menkeBaseService.UnionLessonUserid(menkeLessonIds);
		}
		/// <summary>
		/// 按手机号关联网站与拓课云用户ID
		/// </summary>
		/// <param name="code"></param>
		/// <param name="mobile"></param>
		/// <param name="sty">0学生，1老师</param>
		/// <returns></returns>
		public async Task<ApiResult<int>> UnionMenkeUserId(string code, string mobile, int sty = 0) {
			return await _menkeBaseService.GetMenkeUserId(code, mobile, sty);
		}
		/// <summary>
		/// 按手机号关联网站与拓课云用户ID
		/// </summary>
		/// <param name="code"></param>
		/// <param name="mobile"></param>
		/// <param name="sty">0学生，1老师</param>
		/// <returns></returns>
		public async Task UnionUserLessonUrl(List<int> menkeLessonIds) {
			await _menkeBaseService.UnionUserLessonUrl(menkeLessonIds);
		}
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
			return await _menkeBaseService.CreateStudents(data);
		}
		/// <summary>
		/// 修改学生信息
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public async Task<ApiResult> ModifyStudents(string mobile_code, string mobile, MenkeStudentDto data) {
			return await _menkeBaseService.ModifyStudent(mobile_code, mobile, data);
		}

		/// <summary>
		/// 创建学生
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
			return await _menkeBaseService.CreateTeachers(data);
		}
		/// <summary>
		/// 修改老师信息
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public async Task<ApiResult> ModifyTeacher(string mobile_code, string mobile, MenkeTeacherDto data) {
			return await _menkeBaseService.ModifyTeacher(mobile_code, mobile, data);
		}
		/// <summary>
		/// 同步课堂回放信息
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> RecordSync(string starttime) {
			return await _menkeBaseService.RecordSync(starttime);
        }
        /// <summary>
        /// 同步课节考勤信息
        /// </summary>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public async Task<ApiResult> AttendanceSync(string searchDay)
        {
            return await _menkeBaseService.AttendanceSync(searchDay);
        }
        /// <summary>
        /// 同步作业列表
        /// </summary>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public async Task<ApiResult> HomeworkSync(string starttime)
        {
            return await _menkeBaseService.HomeworkSync(starttime);
        }
        /// <summary>
        /// 同步作业提交列表
        /// </summary>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public async Task<ApiResult> HomeworkSubmitSync(string starttime)
        {
            return await _menkeBaseService.HomeworkSubmitSync(starttime);
        }
		/// <summary>
		/// 同步作业记录点评列表
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> HomeworkRemarkSync(string starttime)
        {
            return await _menkeBaseService.HomeworkRemarkSync(starttime);
		}

		/// <summary>
		/// 定时发送预约成功邮件
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> LessonEmail()
		{
			var result = new ApiResult();
			var dic_data = new Dictionary<string, string>();
			var dic_config = _pubConfigBaseService.GetConfigs("class_begins_hour,cancel_hour");
			var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.IsTrialEmail == 0 && u.MenkeDeleteTime==0).ToList();
			var list_User = _context.Queryable<WebUser>().Where(u => list_UserLesson.Select(s => s.Userid).Contains(u.Userid)).ToList();
			foreach(var model_UserLesson in list_UserLesson)
			{
				var model_User = list_User.FirstOrDefault(u => u.Userid == model_UserLesson.Userid);
				if (model_User == null) continue;
                string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
				dic_data.Clear();
				var btime = DateHelper.ConvertIntToDateTime(model_UserLesson.MenkeStarttime.ToString());//UTC
				btime = btime.AddMinutes(0 - model_User.UtcSec);//转学生所在地时间
				if (model_UserLesson.Istrial == 1)
				{

					dic_data.Add("name", model_User.FirstName + " " + model_User.LastName);
					dic_data.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
					dic_data.Add("menke_entry_url", model_UserLesson.MenkeEntryurl);
					//知道学生所在时区，上课时间是UTC时间，需要转换 btime.AddMinutes(0-(-480))
					dic_data.Add("menke_starttime", btime.ToString());
					dic_data.Add("class_begins_min", dic_config.ContainsKey("class_begins_hour") ? (double.Parse(dic_config["class_begins_hour"]) * 60).ToString() : "");//课前提醒时间（分钟）
					dic_data.Add("cancel_hour", dic_config.ContainsKey("cancel_hour") ? dic_config["cancel_hour"] : "");//超过此时间以上的才可以取消（小时）
					await _messageBaseService.AddEmailTask("TrialLesson", model_User.Email, dic_data, model_User.Lang);
				}
				else {
                    dic_data.Add("name", model_User.FirstName + " " + model_User.LastName);
                    dic_data.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
                    dic_data.Add("menke_entry_url", model_UserLesson.MenkeEntryurl);
                    dic_data.Add("menke_lesson_name", model_UserLesson.MenkeLessonName);
                    dic_data.Add("menke_starttime", btime.ToString());
                    dic_data.Add("class_begins_min", dic_config.ContainsKey("class_begins_hour") ? (double.Parse(dic_config["class_begins_hour"]) * 60).ToString() : "");//课前提醒时间（分钟）
                    dic_data.Add("cancel_hour", dic_config.ContainsKey("cancel_hour") ? dic_config["cancel_hour"] : "");//超过此时间以上的才可以取消（小时）
                    await _messageBaseService.AddEmailTask("Lesson", model_User.Email, dic_data, model_User.Lang);
                }
				model_UserLesson.IsTrialEmail = 1;
				model_UserLesson.TrialEmailTime = DateTime.Now;
			}
			_context.Updateable(list_UserLesson).UpdateColumns(u=>new { u.IsTrialEmail,u.TrialEmailTime}).ExecuteCommand();
			return result;
		}

		/// <summary>
		/// 定时发送试听完成邮件(未出报告时不发MAIL)
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> CompleteTrialLessonEmail()
		{
			var result = new ApiResult();
			var dic_data = new Dictionary<string, string>();
			var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u =>u.Istrial==1 && u.IsTrialCompleteEmail == 0 && u.MenkeDeleteTime==0).ToList();
			var list_User = _context.Queryable<WebUser>().Where(u => list_UserLesson.Select(s => s.Userid).Contains(u.Userid)).ToList();
			var list_Report = _context.Queryable<WebUserLessonReport>().Where(u => list_UserLesson.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();
			foreach(var model_UserLesson in list_UserLesson)
			{
				var model_User = list_User.FirstOrDefault(u => u.Userid == model_UserLesson.Userid);
				if (model_User == null) continue;
				var model_Report = list_Report.FirstOrDefault(u => u.UserLessonid == model_UserLesson.UserLessonid);
				if (model_Report == null) continue;
                dic_data.Clear();

                dic_data.Add("name", model_User.FirstName + " " + model_User.LastName);
				string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
				dic_data.Add("head_img", _commonBaseService.ResourceDomain(head_img));
				dic_data.Add("report_url", $"{Appsettings.app("Web:Host")}/user/report/{model_Report.UserLessonReportid}");
				dic_data.Add("url", $"{string.Format(Appsettings.app("Web:ServiceUrl"),_userManage.Lang)}");
				var result_email = await _messageBaseService.AddEmailTask("CompleteTrialLesson", model_User.Email, dic_data, model_User.Lang);
				if (result_email.StatusCode == 0)
				{
					model_UserLesson.IsTrialCompleteEmail = 1;
					model_UserLesson.TrialCompleteEmailTime = DateTime.Now;
				}
			}
			_context.Updateable(list_UserLesson).UpdateColumns(u=>new { u.IsTrialCompleteEmail, u.TrialCompleteEmailTime }).ExecuteCommand();
			return result;
		}

        /// <summary>
        /// 定时完成正课邮件(未出报告时不发MAIL)
        /// </summary>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public async Task<ApiResult> CompleteLessonEmail()
		{
			var result = new ApiResult();
			var dic_data = new Dictionary<string, string>();
            //直播课不需要发报告：OnlineCourseid==0
            var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u =>u.OnlineCourseid==0 && u.IsTrialEmail == 0 && u.MenkeDeleteTime == 0 && u.Istrial == 0).ToList();
			var list_User = _context.Queryable<WebUser>().Where(u => list_UserLesson.Select(s => s.Userid).Contains(u.Userid)).ToList();
            var list_Report = _context.Queryable<WebUserLessonReport>().Where(u => list_UserLesson.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();
            foreach (var model_UserLesson in list_UserLesson)
			{
				var model_User = list_User.FirstOrDefault(u => u.Userid == model_UserLesson.Userid);
				if (model_User == null) continue;
                var model_Report = list_Report.FirstOrDefault(u => u.UserLessonid == model_UserLesson.UserLessonid);
                if (model_Report == null) continue;

                dic_data.Clear();
                dic_data.Add("name", model_User.FirstName + " " + model_User.LastName);
                string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
                dic_data.Add("head_img", _commonBaseService.ResourceDomain(head_img));
                dic_data.Add("report_url", $"{Appsettings.app("Web:Host")}/user/report/{model_Report.UserLessonReportid}");
				dic_data.Add("url", $"{string.Format(Appsettings.app("Web:ServiceUrl"), _userManage.Lang)}");
				dic_data.Add("menke_lesson_name", model_UserLesson.MenkeLessonName);
                await _messageBaseService.AddEmailTask("CompleteLesson", model_User.Email, dic_data, model_User.Lang);
				model_UserLesson.IsTrialEmail = 1;
				model_UserLesson.TrialEmailTime = DateTime.Now;
			}
			_context.Updateable(list_UserLesson).UpdateColumns(u => new { u.IsTrialEmail, u.TrialEmailTime }).ExecuteCommand();
			return result;
		}

		/// <summary>
		/// 续课提醒
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> SuccessionEmail()
		{
			var result = new ApiResult();
			var dic_data = new Dictionary<string, string>();
			//_context.DataCache.RemoveDataCache("");//清所有SQL缓存
			int residue_class_hour = _pubConfigBaseService.GetConfigInt("residue_class_hour", 5);
			var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => u.Status == 1 && (u.IsResidueEmail==0 || u.IsTicket==0) && (u.ClassHour - u.Classes) <= residue_class_hour).ToList();
			if (list_UserCourse.Count == 0) return result;
			var list_User = _context.Queryable<WebUser>().Where(u => list_UserCourse.Select(s => s.Userid).Contains(u.Userid)).ToList();
			var list_Skutype = _context.Queryable<WebSkuTypeLang>().Where(u => list_UserCourse.Select(s => s.SkuTypeid).Contains(u.SkuTypeid) && u.Lang == _userManage.Lang).ToList();
			var list_Order = _context.Queryable<WebOrder>().Where(u => list_UserCourse.Select(s => s.Orderid).Contains(u.Orderid)).ToList();
			var list_Paytype = _context.Queryable<WebPaytype>().Where(u=>u.Status==1).ToList();
			foreach (var model_UserCourse in list_UserCourse)
			{
				var model_User = list_User.FirstOrDefault(u => u.Userid == model_UserCourse.Userid);
				if (model_User == null) continue;
				var model_Skutype = list_Skutype.FirstOrDefault(u => u.SkuTypeid == model_UserCourse.SkuTypeid);
				var model_Order = list_Order.FirstOrDefault(u => u.Orderid == model_UserCourse.Orderid);
				if (model_UserCourse.IsResidueEmail == 0)
				{
					dic_data.Clear();
					dic_data.Add("name", model_User.FirstName + " " + model_User.LastName);
					string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
					dic_data.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
					dic_data.Add("count", (model_UserCourse.ClassHour - model_UserCourse.Classes).ToString());
					dic_data.Add("url", $"{string.Format(Appsettings.app("Web:ServiceUrl"), _userManage.Lang)}");

					var model_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.UserCourseid == model_UserCourse.UserCourseid).OrderBy(u => u.MenkeStarttime, OrderByType.Desc).First();
					dic_data.Add("teacher", model_UserLesson?.MenkeTeacherName + "");
					await _messageBaseService.AddEmailTask("Succession", model_User.Email, dic_data, model_User.Lang);
					model_UserCourse.IsResidueEmail = 1;
					model_UserCourse.ResidueEmailTime = DateTime.Now;
				}

				if (model_UserCourse.IsTicket == 0)
				{
					//工单场景：续课购买
					dic_data.Clear();
					dic_data.Add(_localizer["姓名"], model_User.FirstName + " " + model_User.LastName);
					dic_data.Add(_localizer["手机"], model_User.MobileCode + "-" + model_User.Mobile);
					dic_data.Add(_localizer["邮箱"], model_User.Email);
					dic_data.Add(_localizer["所在地时区"], model_User.Utc);
					if (model_Order != null)
					{
						dic_data.Add(_localizer["所选课程"], model_Order.Title + "");
						dic_data.Add(_localizer["上课方式"], model_Skutype?.Title + "");
						dic_data.Add(_localizer["购买课时"], model_Order.ClassHour.ToString());
						dic_data.Add(_localizer["已用课时"], model_UserCourse.Classes.ToString());
						dic_data.Add(_localizer["购买时间"], model_Order.Paytime + "");
						dic_data.Add(_localizer["售价"], model_Order.Payment.ToString());
						dic_data.Add(_localizer["币种"], model_Order.CurrencyCode);
						dic_data.Add(_localizer["付款方式"], list_Paytype.FirstOrDefault(u => u.Paytypeid == model_Order.Paytypeid)?.Title + "");
					}

					var body = string.Join("<br>", dic_data.ToList());
					var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["续课提醒"], body);
					if (result_ticket.StatusCode == 0)
					{
						model_UserCourse.IsTicket = 1;
						model_UserCourse.TicketTime = DateTime.Now;
						model_UserCourse.TicketId = result_ticket.Data;
					}
					else
					{
						_logger.LogError(_localizer["续课提醒"] + "," + result_ticket.Message);
					}
				}
			}
			_context.Updateable(list_UserCourse).UpdateColumns(u => new { u.IsResidueEmail, u.ResidueEmailTime,u.IsTicket,u.TicketTime,u.TicketId }).ExecuteCommand();
			return result;
		}

		/// <summary>
		/// 定时发送课前提醒邮件
		/// </summary>
		/// <param name="starttime"></param>
		/// <returns></returns>
		public async Task<ApiResult> ClassBeginsEmail()
		{
			var result = new ApiResult();
			var dic_data = new Dictionary<string, string>();
			var dic_config = _pubConfigBaseService.GetConfigs("class_begins_hour,cancel_hour");
            var class_begins_hour = dic_config.ContainsKey("class_begins_hour") ? dic_config["class_begins_hour"] : "0";
            var cancel_hour = dic_config.ContainsKey("cancel_hour") ? dic_config["cancel_hour"] : "0";
            var btime =DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddHours(double.Parse(class_begins_hour)));
			var dtime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
			var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.IsBeginEmail == 0 && u.MenkeDeleteTime==0 && u.MenkeStarttime< btime && u.MenkeStarttime> dtime).ToList();
			var list_User = _context.Queryable<WebUser>().Where(u => list_UserLesson.Select(s => s.Userid).Contains(u.Userid)).ToList();
			
			foreach(var model_UserLesson in list_UserLesson)
			{
				var model_User = list_User.FirstOrDefault(u => u.Userid == model_UserLesson.Userid);
				if (model_User == null) continue;
				string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
				string class_hour_str = "";
				var class_hour = (DateHelper.ConvertIntToDateTime(model_UserLesson.MenkeStarttime.ToString())- DateTime.UtcNow).TotalHours;
				if (class_hour >= 1)
				{
					class_hour_str = class_hour.ToString("0.0") + _localizer["小时"];
				}
				else {
					class_hour_str = (int)(class_hour * 60) + _localizer["分钟"];
				}

				dic_data.Clear();
				dic_data.Add("name", model_User.FirstName + " " + model_User.LastName);
				dic_data.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
                dic_data.Add("menke_entry_url", model_UserLesson.MenkeEntryurl);
				dic_data.Add("mylesson_url", $"{Appsettings.app("Web:Host")}/user/course/{model_UserLesson.Courseid}");
                dic_data.Add("cancel_hour", cancel_hour);
                dic_data.Add("class_hour", class_hour_str);
                await _messageBaseService.AddEmailTask("ClassBegins", model_User.Email, dic_data, model_User.Lang);
				model_UserLesson.IsBeginEmail = 1;
				model_UserLesson.BeginEmailTime = DateTime.Now;
			}
			_context.Updateable(list_UserLesson).UpdateColumns(u=>new { u.IsBeginEmail, u.BeginEmailTime }).ExecuteCommand();
			return result;
		}


		#region 教室模板
		/// <summary>
		/// 教室模板
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public async Task<ApiResult<List<MenkeTemplateDto>>> GetTemplate()
		{
			return await _menkeBaseService.GetTemplate();
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
			return await _menkeBaseService.CreateCourse(name, students, room_template_id);
		}
		#endregion

		#region 编辑课程
		/// <summary>
		/// 编辑课程
		/// </summary>
		/// <param name="menke_course_id">门课ID</param>
		/// <param name="name">课程名称</param>
		/// <param name="students">学生</param>
		/// <param name="room_template_id">房间模板ID</param>
		/// <returns></returns>
		public async Task<ApiResult> ModifyCourse(int menke_course_id, string name, List<string> students, int room_template_id)
		{
			return await ModifyCourse(menke_course_id, name, students, room_template_id);
		}
		#endregion

		#region 双向检测课程比对
		/// <summary>
		/// 同步检测:
		/// 课程比对:以网站为准，检测最近一小时已购课程，强写拓课云(编辑课程口)，并取消课程同步
		/// 课节比对:从拓课云=>网站，走增量口无需比对,注意：添加修改课节学生不会触发接口（缺陷）
		/// </summary>
		/// <param name="begintime">比对起始时间戳</param>
		/// <param name="endtime">比对结束时间戳</param>
		/// <returns></returns>
		public async Task<ApiResult> ChkCourseComparison(int begintime,int endtime)
		{
			var result = new ApiResult();
			string msg = "";

			//结束时间在1小时之前才执行下面逻辑
			if (endtime > DateHelper.ConvertDateTimeInt(DateTime.UtcNow.AddHours(-1))) return result;

			#region "1,检索网站已购课程最近一小时，与拓课云比对"
			//找出要比对的网站已购课程(试听课除外)
			var list_MenkeCourseid = _context.Queryable<WebUserCourse>().Where(u =>u.Lastchanged >= begintime && u.Lastchanged <=endtime).GroupBy(u => u.MenkeCourseId).Select(u => u.MenkeCourseId).ToList();
			var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => list_MenkeCourseid.Contains(u.MenkeCourseId)).ToList();
			var list_User = _context.Queryable<WebUser>().Where(u => list_UserCourse.Select(s => s.Userid).Contains(u.Userid)).ToList();
			var list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_MenkeCourseid.Contains(u.MenkeCourseId)).ToList();
			var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => list_MenkeCourseid.Contains(u.MenkeCourseId)).ToList();
			var list_SkuType = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
				   .Where((l, r) => r.Lang == _userManage.Lang)
				   .Select((l, r) => new { l.SkuTypeid, r.Title, l.MenkeType, l.MenkeTemplateId })
				   .ToList();
			foreach (var menke_courseid in list_MenkeCourseid) {
				var model_MenkeCourse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == menke_courseid );
				if (model_MenkeCourse == null || model_MenkeCourse.Istrial == 1) continue;//门课不存在（还没来的及同步）或是试听课则不比对

				var list_UserCourseTmp = list_UserCourse.Where(u => u.Status == 1 && u.MenkeCourseId == menke_courseid).ToList();//临时取同一堂课的学生已购课程
				if (list_UserCourseTmp.Count > 0)
				{
					//存在已购课程,强制写入修改课程接口
					//注意：后继如果课程同步接口中有学生ID，这里可以与menke_course表去比对，没必要每次都强写接口覆盖
					var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == list_UserCourseTmp[0].SkuTypeid);
					if (list_UserCourseTmp.Count <= 0)
					{
						msg += $"[{DateTime.Now}]比对异常,已购课程[menke_courseid={menke_courseid}]的上课方式[sky_typeid={list_UserCourseTmp[0].SkuTypeid}]不存在<hr>";
						continue;
					}
					string course_name = string.Join(",", list_User.Select(u => u.FirstName + u.LastName).ToList()) + "-" + list_UserCourseTmp[0].Title + "-" + model_SkuType.Title;
					var result_course = await _menkeBaseService.ModifyCourse(menke_courseid, course_name, list_User.Select(u => u.FirstName + "" + u.LastName).ToList(), model_SkuType.MenkeTemplateId);
					if (result_course.StatusCode == 0) {
						#region 删掉拓课云的无效课节
						//找出属于这门课，但没有正式已购课节的所有课节信息
						var list_UserLessonTmp = list_UserLesson.Where(u =>u.MenkeDeleteTime==0 && (u.MenkeState==1 ||u.MenkeState==4) && u.MenkeCourseId == menke_courseid && !list_UserCourseTmp.Select(s=>s.UserCourseid).Contains(u.UserCourseid)).ToList();
						foreach (var model in list_UserLessonTmp) {
							//拓课云中要判断是否存在其他学生，只能移除
							var list = list_UserLesson.Where(u => u.UserLessonid != model.UserLessonid && u.MenkeLessonId == model.MenkeLessonId && u.MenkeDeleteTime==0 && u.Status==1).ToList();
							if (list.Count > 0)
							{
								//移除学生,只保留其他学生
								await _menkeBaseService.ModifyLessonStudent(model.MenkeLessonId, list.Select(u => u.MenkeStudentCode + "-" + u.MenkeStudentMobile).ToList());
							}
							else
							{
								//仅此学生一人，可删除课节
								await _menkeBaseService.DeleteLesson(model.MenkeLessonId);
							}
						}
						#endregion
					}
					else
					{
						_logger.LogError($"[{DateTime.Now}]比对异常,课程[menke_courseid={menke_courseid}]的修改出错:" + result_course.Message + "");
						msg += $"[{DateTime.Now}]比对异常,课程[menke_courseid={menke_courseid}]的修改出错:" + result_course.Message + "<hr>";
					}
				}
				else if (list_MenkeCourse.Any(u => u.MenkeCourseId == menke_courseid && u.MenkeDeleteTime > 0))
				{
					//无已购课程，并且已删除拓课云课程，无需覆盖操作
				}
				else {
					//无已购课程，需同步删除拓课去云课程
					var result_course = await _menkeBaseService.DelCourse(menke_courseid);
					if (result_course.StatusCode == 0) {
						#region 删掉拓课云的无效课节
						//找出属于这门课，但没有正式已购课节的所有课节信息
						var list_UserLessonTmp = list_UserLesson.Where(u => u.MenkeDeleteTime == 0 && (u.MenkeState == 1 || u.MenkeState == 4) && u.MenkeCourseId == menke_courseid && !list_UserCourseTmp.Select(s => s.UserCourseid).Contains(u.UserCourseid)).ToList();
						foreach (var model in list_UserLessonTmp)
						{
							//拓课云中要判断是否存在其他学生，只能移除
							var list = list_UserLesson.Where(u => u.UserLessonid != model.UserLessonid && u.MenkeLessonId == model.MenkeLessonId && u.MenkeDeleteTime == 0 && u.Status == 1).ToList();
							if (list.Count > 0)
							{
								//移除学生,只保留其他学生
								await _menkeBaseService.ModifyLessonStudent(model.MenkeLessonId, list.Select(u => u.MenkeStudentCode + "-" + u.MenkeStudentMobile).ToList());
							}
							else
							{
								//仅此学生一人，可删除课节
								await _menkeBaseService.DeleteLesson(model.MenkeLessonId);
							}
						}
						#endregion
					}
					else
					{
						msg += $"[{DateTime.Now}]比对异常,无已购课程,删除课程[menke_courseid={menke_courseid}]时出错:" + result_course.Message + "<hr>";
					}
				}
			}
			if (!string.IsNullOrEmpty(msg))
            {
                _logger.LogError(msg);
            }
			_context.Updateable<UeTask>().SetColumns(u => u.Paramenter == endtime.ToString()).Where(u => u.Code == "ChkCourseComparison").ExecuteCommand();
			#endregion

			#region "2,检索网站已购课程最近一小时，与拓课云比对"
			//课节每次获取时都会重复一段时间获取，无需再次补此逻辑
			#endregion
			return result;
		}
		#endregion

		#region 同步检测忘添学生的课节
		/// <summary>
		/// 同步检测忘添学生的课节
		/// </summary>
		/// <param name="email">要通知的人</param>
		/// <returns></returns>
		public async Task<ApiResult> OmissionStudent(string email)
        {
            var result = new ApiResult();
			if (string.IsNullOrEmpty(email)) return result;
            var onlinetime = 1673835800;//1673835800=>2023-1-1上线时间
            var list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => u.MenkeDeleteTime == 0 && u.Status == 1 && u.MenkeState == 1 && u.MenkeStarttime > onlinetime && u.Istrial == 0 &&
                SqlFunc.Subqueryable<WebUserLesson>().Where(s => s.MenkeLessonId == u.MenkeLessonId && s.MenkeDeleteTime == 0 && s.Status == 1 && s.MenkeState == 1 && s.MenkeStarttime > onlinetime).NotAny())
                .ToList();
            string title = $"共有{list_MenkeLesson.Count()}节课漏添加学生信息";
            string body = "";
            foreach (var model in list_MenkeLesson)
            {
                body += $"课节名称:{model.MenkeName},老师:{model.MenkeTeacherName},房间号:{model.MenkeLiveSerial},开课时间:{DateHelper.ConvertIntToDateTime(model.MenkeStarttime.ToString()).ToLocalTime()}<br>";
            }
            var dic_config = _pubConfigBaseService.GetConfigs("sendname,mailservice,sendmail,sendpwd");
			string toEmail="",toEmailCC = "";
			if (email.Contains(";"))
			{
				toEmail = email.Split(";")[0];
				toEmailCC = email.Replace(email.Split(";")[0] + ";", "");
			}
			else {
				toEmail = email;

            }
            EmailHelper.Sentmail(title, body, dic_config["sendname"], dic_config["sendmail"], toEmail, toEmailCC, dic_config["mailservice"], dic_config["sendmail"], dic_config["sendpwd"]);
            return result;
        }
        #endregion
    }
}