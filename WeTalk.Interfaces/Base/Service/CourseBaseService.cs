using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Models;

namespace WeTalk.Interfaces.Base
{
	public partial class CourseBaseService : BaseService, ICourseBaseService
    {
		private readonly SqlSugarScope _context;

		private readonly IUserManage _userManage;
		private readonly IMenkeBaseService _menkeBaseService;
		private readonly IPubConfigBaseService _pubConfigBaseService;
		private readonly ILogger<CourseBaseService> _logger;

		public CourseBaseService(SqlSugarScope dbcontext,IUserManage userManage, ILogger<CourseBaseService> logger,
			IMenkeBaseService menkeBaseService, IPubConfigBaseService pubConfigBaseService)
		{
			_context = dbcontext;
			_userManage = userManage;
			_logger = logger;
			_menkeBaseService = menkeBaseService;
			_pubConfigBaseService = pubConfigBaseService;
		}

		#region 已购课程自动安排课程
		/// <summary>
		/// 已购课程自动安排课程
		/// </summary>
		/// <param name="user_courseid">已购课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult<int>> UserCourseArranging(long user_courseid)
		{
			var result = new ApiResult<int>();
			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(user_courseid);
			if (model_UserCourse == null)
			{
				result.StatusCode = 4003;
				result.Message = "我的课程(" + _userManage.Lang + ")不存在";
				return result;
			}
			switch (model_UserCourse.Type) {
				case 0:
					return await UserCourseArranging(model_UserCourse);
				case 1:
					return await OnlineUserCourseArranging(model_UserCourse);
				default:
					result.StatusCode = 4003;
					result.Message = "我的课程-类型异常";
					return result;
			}
			
		}
		public async Task<ApiResult<int>> UserCourseArranging(WebUserCourse model_UserCourse)
		{
			var result = new ApiResult<int>();
			if (model_UserCourse != null)
			{
				if (model_UserCourse.MenkeCourseId > 0)
				{
					result.Data = model_UserCourse.MenkeCourseId;
					return result;
				}
				string title = "";
				var model_Course = _context.Queryable<WebCourse>().LeftJoin<WebCourseLang>((l, r) => l.Courseid == r.Courseid && r.Lang == _userManage.Lang)
					.Where((l, r) => l.Courseid == model_UserCourse.Courseid)
					.Select((l, r) => new { l.Courseid, r.Title })
					.First();
				if (model_Course == null)
				{
					result.StatusCode = 4003;
					result.Message = "课程ID(" + _userManage.Lang + ")不存在";
					return result;
				}
				else {
					title = model_Course.Title;
				}
				var model_User = _context.Queryable<WebUser>().InSingle(model_UserCourse.Userid);
				if (model_User == null)
				{
					result.StatusCode = 4003;
					result.Message = "用户ID不存在";
					return result;
				}
				var model_SkuType = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
					.Where((l, r) => l.SkuTypeid == model_UserCourse.SkuTypeid && r.Lang == _userManage.Lang)
					.Select((l, r) => new { l.SkuTypeid, r.Title, l.MenkeType, l.MenkeTemplateId })
					.First();
				if (model_SkuType == null)
				{
					result.StatusCode = 4003;
					result.Message = "上课类型(" + _userManage.Lang + ")不存在";
					return result;
				}
				string course_name = "";
				var students = new List<string>();
				//if(model_UserCourse.tri)

				//教室类型教室类型（一对一0、一对多3、大直播4）
				switch (model_SkuType.MenkeType)
				{
					case 0:
						course_name = model_User.FirstName + " " + model_User.LastName + "-" + title + "" + model_SkuType.Title;
						students.Add(model_User.MobileCode + "-" + model_User.Mobile);
						var result_course = await _menkeBaseService.CreateCourse(course_name, students, model_SkuType.MenkeTemplateId);
						if (result_course.StatusCode == 0)
						{
							model_UserCourse.MenkeCourseId = result_course.Data;
						}
						break;
					case 3:
						#region "查找相同课程,相同老师，未满的分组存放至list_UserLesson"
						int private_class_max = _pubConfigBaseService.GetConfigInt("private_class_max", 5);
						var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);

						//找出报有相同课程的已排课课节
						var list_UserLesson = _context.Queryable<WebUserLesson>()
										.Where(u => u.MenkeCourseId > 0 && u.Courseid == model_UserCourse.Courseid && u.SkuTypeid == model_UserCourse.SkuTypeid && u.MenkeDeleteTime == 0 && u.MenkeStarttime >= thetime && u.Status == 1)
										.GroupBy(u => new { u.MenkeCourseId, u.MenkeTeacherCode, u.MenkeTeacherMobile, u.Userid })
										.Select(u => new { u.MenkeCourseId, u.MenkeTeacherCode, u.MenkeTeacherMobile, u.Userid })
										.MergeTable()
										.GroupBy(u => new { u.MenkeCourseId, u.MenkeTeacherCode, u.MenkeTeacherMobile })
										.Having(u => SqlFunc.AggregateDistinctCount(u.Userid) < private_class_max)
										.Select(u => new {
											u.MenkeCourseId,
											u.MenkeTeacherCode,
											u.MenkeTeacherMobile,
											Userids = SqlFunc.MappingColumn(default(string), "GROUP_CONCAT(Userid)"),
											Count = SqlFunc.AggregateDistinctCount(u.Userid)
										})
										.ToList();
						var list_Userid = string.Join(",", list_UserLesson.Where(s => !string.IsNullOrEmpty(s.Userids)).Select(s => s.Userids));
						if (!string.IsNullOrEmpty(list_Userid)) list_Userid = "," + list_Userid + ",";
						var list_User = _context.Queryable<WebUser>().Where(u => list_Userid.Contains(SqlFunc.MergeString(",", u.Userid.ToString(), ","))).ToList();
						#endregion

						#region "当前用户是否试听课，有取其老师手机"
						var list_TrialTeacher = _context.Queryable<WebUserLesson>().Where(u => u.Userid == model_UserCourse.Userid && u.MenkeDeleteTime == 0 && u.MenkeState == 3 && u.Status == 1 && u.Istrial == 1)
							.Select(u => new { u.MenkeCourseId, u.MenkeTeacherCode, u.MenkeTeacherMobile })
							.ToList();
						#endregion

						#region "优选试听老师的课，次选学生数最多的课"
						var _menkeCourseid = list_UserLesson.Where(u => list_TrialTeacher.Select(s => s.MenkeTeacherCode + "-" + s.MenkeTeacherMobile).Contains(u.MenkeTeacherCode + "-" + u.MenkeTeacherMobile)).OrderByDescending(u => u.Count).Select(u => u.MenkeCourseId).FirstOrDefault();
						if (_menkeCourseid == 0)
						{
							//没有找到试听老师的课，就从那些从来没排过课的已购课程中先找，不用插班
							var list_NewUserCourse = _context.Queryable<WebUserCourse>()
								.Where(u => u.MenkeCourseId > 0 && u.UserCourseid != model_UserCourse.UserCourseid && u.Courseid == model_UserCourse.Courseid && u.SkuTypeid == model_UserCourse.SkuTypeid && u.Status == 1 &&
								SqlFunc.Subqueryable<WebUserLesson>().Where(s => s.UserCourseid == u.UserCourseid && s.Status == 1 && s.MenkeDeleteTime == 0).NotAny())
								.GroupBy(u => u.MenkeCourseId)
								.Having(u => SqlFunc.AggregateDistinctCount(u.Userid) < private_class_max)
								.Select(u => new {
									u.MenkeCourseId,
									Count = SqlFunc.AggregateDistinctCount(u.MenkeCourseId)
								}).ToList();
							_menkeCourseid = list_NewUserCourse.OrderByDescending(u => u.Count).Select(u => u.MenkeCourseId).FirstOrDefault();
							if (_menkeCourseid == 0)
							{
								//只能从排过课的课节中去找合适的；
								_menkeCourseid = list_UserLesson.OrderByDescending(u => u.Count).Select(u => u.MenkeCourseId).FirstOrDefault();
							}
						}
						#endregion

						if (_menkeCourseid > 0)
						{
							var userids = _context.Queryable<WebUserCourse>().Where(u => u.MenkeCourseId == _menkeCourseid).Select(u => u.Userid).ToList();
							list_User = _context.Queryable<WebUser>().Where(u => userids.Contains(u.Userid)).ToList();

							//取指定课程所有学生ID
							var list_Student = list_User.Select(u => new { mobile = (u.MobileCode + "-" + u.Mobile), name = (u.FirstName + " " + u.LastName) }).ToList();
							students = list_Student.Select(u => u.mobile).ToList();
							course_name = string.Join(",", list_Student.Select(u => u.name).ToList()) + "," + model_User.FirstName + " " + model_User.LastName + "-" + title + "" + model_SkuType.Title;
							var result_course0 = await _menkeBaseService.ModifyCourse(_menkeCourseid, course_name, students, model_SkuType.MenkeTemplateId);
							if (result_course0.StatusCode == 0)
							{
								model_UserCourse.MenkeCourseId = _menkeCourseid;
							}
							else
							{
								result.StatusCode = 4002;
								result.Message = "加入已有课程[" + _menkeCourseid + "]异常:" + result_course0.Message;
								return result;
							}
						}
						else
						{
							//新创建课程
							course_name = model_User.FirstName + " " + model_User.LastName + "-" + title + "-" + model_SkuType.Title;
							students.Add(model_User.MobileCode + "-" + model_User.Mobile);
							var result_course1 = await _menkeBaseService.CreateCourse(course_name, students, model_SkuType.MenkeTemplateId);
							if (result_course1.StatusCode == 0)
							{
								model_UserCourse.MenkeCourseId = result_course1.Data;
							}
						}
						break;
					default:
						result.StatusCode = 4002;
						result.Message = "请检查上课方式[" + model_UserCourse.SkuTypeid + "]是否绑定正确的教室";
						return result;
				}
				_context.Updateable(model_UserCourse).UpdateColumns(u => u.MenkeCourseId).ExecuteCommand();
			}
			return result;
		}


		public async Task<ApiResult<int>> OnlineUserCourseArranging(WebUserCourse model_UserCourse)
		{
			var result = new ApiResult<int>();
			if (model_UserCourse != null)
			{
				if (model_UserCourse.MenkeCourseId > 0)
				{
					result.Data = model_UserCourse.MenkeCourseId;
					return result;
				}
				string title = "";
				var model_Course = _context.Queryable<WebOnlineCourse>().LeftJoin<WebOnlineCourseLang>((l, r) => l.OnlineCourseid == r.OnlineCourseid && r.Lang == _userManage.Lang)
					.Where((l, r) => l.OnlineCourseid == model_UserCourse.OnlineCourseid)
					.Select((l, r) => new { l.OnlineCourseid, r.Title })
					.First();
				if (model_Course == null)
				{
					result.StatusCode = 4003;
					result.Message = "课程ID(" + _userManage.Lang + ")不存在";
					return result;
				}
				else
				{
					title = model_Course.Title;
				}
				var model_User = _context.Queryable<WebUser>().InSingle(model_UserCourse.Userid);
				if (model_User == null)
				{
					result.StatusCode = 4003;
					result.Message = "用户ID不存在";
					return result;
				}
				int menkeType = 3;//（教室类型:一对一0、一对多3、大直播4）
				int menkeTemplateId = 0;//教室ID：335一对一，336一对多，3大直播
				string course_name = "";
				var students = new List<string>();

				
				switch (menkeType)
				{
					case 0:
						menkeTemplateId = 335;
						course_name = model_User.FirstName + " " + model_User.LastName + "-" + title + "-直播课" ;
						students.Add(model_User.MobileCode + "-" + model_User.Mobile);
						var result_course = await _menkeBaseService.CreateCourse(course_name, students, menkeTemplateId);
						if (result_course.StatusCode == 0)
						{
							model_UserCourse.MenkeCourseId = result_course.Data;
						}
						break;
					case 3:
						menkeTemplateId = 336;
						#region "查找相同课程,相同老师，未满的分组存放至list_UserLesson"
						int private_class_max = _pubConfigBaseService.GetConfigInt("private_class_max", 5);
						var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);

						//找出报有相同课程的已排课课节
						var list_UserLesson = _context.Queryable<WebUserLesson>()
										.Where(u => u.MenkeCourseId > 0 && u.OnlineCourseid == model_UserCourse.OnlineCourseid && u.MenkeDeleteTime == 0 && u.MenkeStarttime >= thetime && u.Status == 1)
										.GroupBy(u => new { u.MenkeCourseId, u.MenkeTeacherCode, u.MenkeTeacherMobile, u.Userid })
										.Select(u => new { u.MenkeCourseId, u.MenkeTeacherCode, u.MenkeTeacherMobile, u.Userid })
										.MergeTable()
										.GroupBy(u => new { u.MenkeCourseId, u.MenkeTeacherCode, u.MenkeTeacherMobile })
										.Having(u => SqlFunc.AggregateDistinctCount(u.Userid) < private_class_max)
										.Select(u => new {
											u.MenkeCourseId,
											u.MenkeTeacherCode,
											u.MenkeTeacherMobile,
											Userids = SqlFunc.MappingColumn(default(string), "GROUP_CONCAT(Userid)"),
											Count = SqlFunc.AggregateDistinctCount(u.Userid)
										})
										.ToList();
						var list_Userid = string.Join(",", list_UserLesson.Where(s => !string.IsNullOrEmpty(s.Userids)).Select(s => s.Userids));
						if (!string.IsNullOrEmpty(list_Userid)) list_Userid = "," + list_Userid + ",";
						var list_User = _context.Queryable<WebUser>().Where(u => list_Userid.Contains(SqlFunc.MergeString(",", u.Userid.ToString(), ","))).ToList();
						#endregion


						//#region "优选学生数最多的课"
						////从那些从来没排过课的已购课程中先找，不用插班
						//var list_NewUserCourse = _context.Queryable<WebUserCourse>()
						//	.Where(u => u.MenkeCourseId > 0 && u.UserCourseid != model_UserCourse.UserCourseid && u.OnlineCourseid == model_UserCourse.OnlineCourseid && u.Status == 1 &&
						//	SqlFunc.Subqueryable<WebUserLesson>().Where(s => s.UserCourseid == u.UserCourseid && s.Status == 1 && s.MenkeDeleteTime == 0).NotAny())
						//	.GroupBy(u => u.MenkeCourseId)
						//	.Having(u => SqlFunc.AggregateDistinctCount(u.Userid) < private_class_max)
						//	.Select(u => new {
						//		u.MenkeCourseId,
						//		Count = SqlFunc.AggregateDistinctCount(u.MenkeCourseId)
						//	}).ToList();
						//var _menkeCourseid = list_NewUserCourse.OrderByDescending(u => u.Count).Select(u => u.MenkeCourseId).FirstOrDefault();
						//if (_menkeCourseid == 0)
						//{
						//	//只能从排过课的课节中去找合适的；
						//	_menkeCourseid = list_UserLesson.OrderByDescending(u => u.Count).Select(u => u.MenkeCourseId).FirstOrDefault();
						//}
						var _menkeCourseid = list_UserLesson.OrderByDescending(u => u.Count).Select(u => u.MenkeCourseId).FirstOrDefault();
						//#endregion

						if (_menkeCourseid > 0)
						{
							var userids = _context.Queryable<WebUserCourse>().Where(u => u.MenkeCourseId == _menkeCourseid).Select(u => u.Userid).ToList();
							list_User = _context.Queryable<WebUser>().Where(u => userids.Contains(u.Userid)).ToList();

							//取指定课程所有学生ID
							var list_Student = list_User.Select(u => new { mobile = (u.MobileCode + "-" + u.Mobile), name = (u.FirstName + " " + u.LastName) }).ToList();
							students = list_Student.Select(u => u.mobile).ToList();
							course_name = string.Join(",", list_Student.Select(u => u.name).ToList()) + "," + model_User.FirstName + " " + model_User.LastName + "-" + title + "-直播课";
							var result_course0 = await _menkeBaseService.ModifyCourse(_menkeCourseid, course_name, students, menkeTemplateId);
							if (result_course0.StatusCode == 0)
							{
								model_UserCourse.MenkeCourseId = _menkeCourseid;
							}
							else
							{
								result.StatusCode = 4002;
								result.Message = "加入已有课程[" + _menkeCourseid + "]异常:" + result_course0.Message;
								return result;
							}
						}
						else
						{
							//新创建课程
							course_name = model_User.FirstName + " " + model_User.LastName + "-" + title + "-直播课";
							students.Add(model_User.MobileCode + "-" + model_User.Mobile);
							var result_course1 = await _menkeBaseService.CreateCourse(course_name, students, menkeTemplateId);
							if (result_course1.StatusCode == 0)
							{
								model_UserCourse.MenkeCourseId = result_course1.Data;
							}
						}
						break;
					default:
						result.StatusCode = 4002;
						result.Message = "请检查上课方式[" + model_UserCourse.SkuTypeid + "]是否绑定正确的教室";
						return result;
				}
				_context.Updateable(model_UserCourse).UpdateColumns(u => u.MenkeCourseId).ExecuteCommand();
			}
			return result;
		}
		#endregion

		#region 锁定/解锁已购课程
		/// <summary>
		/// 锁定已购课程（删除未上课节）
		/// 多人：移除学生，删除/移除未上课节
		/// 单人：加前缀[锁定],删除未上课节
		/// </summary>
		/// <param name="user_courseid">已购课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult> LockUserCourse(long user_courseid)
		{
			var result = new ApiResult();
			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(user_courseid);
			if (model_UserCourse != null)
			{
				//清除未开始的课节,注意，1V1可以直接删，多人只能移除
				if (model_UserCourse.MenkeCourseId == 0)
				{
					model_UserCourse.Status = 2;
					model_UserCourse.Remarks = $"[{DateTime.Now}]课程未关联拓课云,直接锁定<hr>" + model_UserCourse.Remarks;
					model_UserCourse.Lastchanged = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
					_context.Updateable(model_UserCourse).UpdateColumns(u => new { u.Status, u.Remarks, u.Lastchanged }).ExecuteCommand();
					return result;
				}
				else
				{
					var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId);
					if (model_MenkeCourse == null)
					{
						result.StatusCode = 4002;
						result.Message = "拓课云课程不存在";
						return result;
					}
					var model_SkuType = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
						.Where((l, r) => l.SkuTypeid == model_UserCourse.SkuTypeid && r.Lang == _userManage.Lang)
						.Select((l, r) => new { l.SkuTypeid, r.Title, l.MenkeType, l.MenkeTemplateId })
						.First();
					if (model_SkuType == null)
					{
						result.StatusCode = 4003;
						result.Message = "上课类型(" + _userManage.Lang + ")不存在";
						return result;
					}
					var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId && u.Status == 1).ToList();
					if (list_UserCourse.Count <= 0)
					{
						result.StatusCode = 4003;
						result.Message = "已购课程不存在";
						return result;
					}
					var list_User = _context.Queryable<WebUser>().Where(u => list_UserCourse.Select(u => u.Userid).Contains(u.Userid)).ToList();
					var model_User = list_User.FirstOrDefault(u => u.Userid == model_UserCourse.Userid);
					if (model_User == null)
					{
						result.StatusCode = 4003;
						result.Message = "用户ID不存在";
						return result;
					}
					string course_name = model_MenkeCourse.MenkeName + "";

					///多人课程，移除学生即可。单人课程改名“锁定”
					#region 锁定课程
					if (list_UserCourse.Count > 1)
					{
						course_name = course_name.Replace(model_User.FirstName + " " + model_User.LastName, "").Replace(",,", ",");
						if (course_name.StartsWith(",")) course_name.Remove(0, 1);
						var students = list_User.Where(u => u.Userid != model_UserCourse.Userid).Select(u => SqlFunc.MergeString(u.MobileCode, "-", u.Mobile)).ToList();
						var result_menke_course = await _menkeBaseService.ModifyCourse(model_UserCourse.MenkeCourseId, course_name, students, model_SkuType.MenkeTemplateId);
						if (result_menke_course.StatusCode == 0)
						{
							model_UserCourse.Status = 2;
							model_UserCourse.Remarks = $"[{DateTime.Now}]锁定课程,将学生从课程中移除<hr>" + model_UserCourse.Remarks;
						}
						else
						{
							result.StatusCode = 4002;
							result.Message = result_menke_course.Message;
							return result;
						}
					}
					else
					{
						if (!course_name.Contains("[锁定]"))
						{
							course_name = "[锁定]" + course_name;
						}
						var result_menke_course = await _menkeBaseService.ModifyCourse(model_UserCourse.MenkeCourseId, course_name, null, model_SkuType.MenkeTemplateId);
						if (result_menke_course.StatusCode == 0)
						{
							model_UserCourse.Status = 2;
							model_UserCourse.Remarks = $"[{DateTime.Now}]锁定课程,打上锁定标记<hr>" + model_UserCourse.Remarks;
						}
						else
						{
							result.StatusCode = 4002;
							result.Message = result_menke_course.Message;
							return result;
						}
					}
					#endregion
				}

				#region 删除课节
				//所有相关课节，仅要移除的学生
				var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status == 1 && u.MenkeState == 1 && u.UserCourseid== user_courseid).ToList();

				//所有相关的课节，包括其他学生
				var list_AllUserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status == 1 && u.MenkeState == 1 && list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
				foreach (var model in list_UserLesson)
				{
					//网站中本学生的课节必需要删除
					model.Status = -1;
					model.Remark = $"[{DateTime.Now}]锁定用户课程,清除未上课的排课课节<hr>" + model.Remark;

					//拓课云中要判断是否存在其他学生，只能移除
					var list = list_AllUserLesson.Where(u => u.UserLessonid != model.UserLessonid && u.MenkeLessonId == model.MenkeLessonId).ToList();
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
				_context.Updateable(list_UserLesson).UpdateColumns(u => new { u.Status, u.Remark }).ExecuteCommand();
				#endregion

				model_UserCourse.Lastchanged = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
				_context.Updateable(model_UserCourse).UpdateColumns(u => new { u.Status,u.Remarks,u.Lastchanged}).ExecuteCommand();
			}
			return result;
		}

		/// <summary>
		/// 解锁已购课程（不恢复课节）
		/// </summary>
		/// <param name="user_courseid"></param>
		/// <returns></returns>
		public async Task<ApiResult> UnLockUserCourse(long user_courseid)
		{
			var result = new ApiResult();
			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(user_courseid);
			if (model_UserCourse != null)
			{
				var model_MenkeCourse = _context.Queryable<MenkeCourse>().InSingle(model_UserCourse.MenkeCourseId);
				if (model_MenkeCourse == null)
				{
					result.StatusCode = 4002;
					result.Message = "拓课云课程不存在";
					return result;
				}
				var model_SkuType = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
					.Where((l, r) => l.SkuTypeid == model_UserCourse.SkuTypeid && r.Lang == _userManage.Lang)
					.Select((l, r) => new { l.SkuTypeid, r.Title, l.MenkeType, l.MenkeTemplateId })
					.First();
				if (model_SkuType == null)
				{
					result.StatusCode = 4003;
					result.Message = "上课类型(" + _userManage.Lang + ")不存在";
					return result;
				}
				//所有同一课程的已购课程(包括当前解锁的已购课程)
				var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => (u.MenkeCourseId == model_UserCourse.MenkeCourseId && u.Status == 1) ||u.UserCourseid==user_courseid).ToList();
				if (list_UserCourse.Count <= 0)
				{
					result.StatusCode = 4003;
					result.Message = "已购课程不存在";
					return result;
				}
				var list_User = _context.Queryable<WebUser>().Where(u => list_UserCourse.Select(u => u.Userid).Contains(u.Userid)).ToList();
				var model_User = list_User.FirstOrDefault(u => u.Userid == model_UserCourse.Userid);
				if (model_User == null)
				{
					result.StatusCode = 4003;
					result.Message = "用户ID不存在";
					return result;
				}
				string course_name = model_MenkeCourse.MenkeName + "";
				///多人课程，加入学生即可。单人课程改名将“锁定”去掉
				#region 解锁课程
				if (list_UserCourse.Count > 1)
				{
					course_name = string.Join(",", list_User.Select(u => u.FirstName + u.LastName).ToList()) + "-" + model_UserCourse.Title + "-" + model_SkuType.Title;
					var students = list_User.Select(u => SqlFunc.MergeString(u.MobileCode, "-", u.Mobile)).ToList();
					var result_menke_course = await _menkeBaseService.ModifyCourse(model_UserCourse.MenkeCourseId, course_name, students, model_SkuType.MenkeTemplateId);
					if (result_menke_course.StatusCode == 0)
					{
						model_UserCourse.Status = 1;
						model_UserCourse.Remarks = $"[{DateTime.Now}]解锁课程,将学生加入课程中<hr>" + model_UserCourse.Remarks;
					}
					else
					{
						result.StatusCode = 4002;
						result.Message = result_menke_course.Message;
						return result;
					}
				}
				else
				{
					if (course_name.Contains("[锁定]"))
					{
						course_name = course_name.Replace("[锁定]","");
					}
					var result_menke_course = await _menkeBaseService.ModifyCourse(model_UserCourse.MenkeCourseId, course_name, null, model_SkuType.MenkeTemplateId);
					if (result_menke_course.StatusCode == 0)
					{
						model_UserCourse.Status = 2;
						model_UserCourse.Remarks = $"[{DateTime.Now}]锁定课程,打上锁定标记<hr>" + model_UserCourse.Remarks;
					}
					else
					{
						result.StatusCode = 4002;
						result.Message = result_menke_course.Message;
						return result;
					}
				}
				#endregion
				model_UserCourse.Lastchanged = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
				_context.Updateable(model_UserCourse).UpdateColumns(u => new { u.Status, u.Remarks, u.Lastchanged }).ExecuteCommand();
			}
			return result;
		}
		#endregion


		#region 删除已购课程
		/// <summary>
		/// 删除已购课程
		/// 先删拓课云课程成功才更新网站已购课程
		/// 先删拓课云课节成功才更新网站课节
		/// 但课节删除成功与否与课程没有必然关系
		/// </summary>
		/// <param name="user_courseid">已购课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult> DelUserCourse(long user_courseid)
		{
			var result = new ApiResult();
			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(user_courseid);
			if (model_UserCourse != null)
			{

				if (model_UserCourse.MenkeCourseId == 0)
				{
					model_UserCourse.Status = -1;
					model_UserCourse.Remarks = $"[{DateTime.Now}]课程未关联拓课云,直接删除<hr>" + model_UserCourse.Remarks;
					model_UserCourse.Lastchanged = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
					_context.Updateable(model_UserCourse).UpdateColumns(u => new { u.Status, u.Remarks, u.Lastchanged }).ExecuteCommand();
					return result;
				}
				else
				{

					//清除未开始的课节,注意，1V1可以直接删，多人只能移除
					var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId);
					if (model_MenkeCourse == null || model_UserCourse.MenkeCourseId == 0)
					{
						result.StatusCode = 4002;
						result.Message = "拓课云课程不存在";
						return result;
					}
					var model_SkuType = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
						.Where((l, r) => l.SkuTypeid == model_UserCourse.SkuTypeid && r.Lang == _userManage.Lang)
						.Select((l, r) => new { l.SkuTypeid, r.Title, l.MenkeType, l.MenkeTemplateId })
						.First();
					if (model_SkuType == null)
					{
						result.StatusCode = 4003;
						result.Message = "上课类型(" + _userManage.Lang + ")不存在";
						return result;
					}

					var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId).ToList();
					var list_User = _context.Queryable<WebUser>().Where(u => list_UserCourse.Select(u => u.Userid).Contains(u.Userid)).ToList();//取所有同一门课学生信息
					var model_User = list_User.FirstOrDefault(u => u.Userid == model_UserCourse.Userid);//取当前学生
					if (model_User == null)
					{
						result.StatusCode = 4003;
						result.Message = "用户不存在";
						return result;
					}
					string course_name = "";
					course_name = string.Join(",", list_User.Where(u => u.Userid != model_UserCourse.Userid).Select(u => u.FirstName + " " + u.LastName).ToList()) + "-" + model_UserCourse.Title + "-" + model_SkuType.Title;
					if (course_name.Length > 50) course_name = course_name.Substring(0, 50);

					///多人课程，移除学生即可。单人课程直接删除
					///若单人，删除拓课云失败，则改为名称前缀加[删除]
					#region 删除课程
					if (list_UserCourse.Count > 1)
					{
						var students = list_User.Where(u => u.Userid != model_UserCourse.Userid).Select(u => u.MobileCode + "-" + u.Mobile).ToList();
						var result_menke_course = await _menkeBaseService.ModifyCourse(model_UserCourse.MenkeCourseId, course_name, students, model_SkuType.MenkeTemplateId);
						if (result_menke_course.StatusCode == 0)
						{
							model_UserCourse.Status = -1;
							model_UserCourse.Remarks = $"[{DateTime.Now}]删除课程,将学生从课程中移除<hr/>" + model_UserCourse.Remarks;
						}
						else
						{
							result.StatusCode = 4002;
							result.Message = result_menke_course.Message;
							return result;
						}
					}
					else
					{
						var result_menke_course = await _menkeBaseService.DelCourse(model_UserCourse.MenkeCourseId);
						if (result_menke_course.StatusCode == 0)
						{
							model_UserCourse.Status = -1;
							model_UserCourse.Remarks = $"[{DateTime.Now}]删除课程<hr/>" + model_UserCourse.Remarks;
						}
						else
						{
							if (!course_name.Contains("[删除]"))
							{
								course_name = "[删除]" + course_name;
							}
							var result_modify = await _menkeBaseService.ModifyCourse(model_UserCourse.MenkeCourseId, course_name, null, model_SkuType.MenkeTemplateId);
							if (result_modify.StatusCode == 0)
							{
								model_UserCourse.Status = -1;
								model_UserCourse.Remarks = $"[{DateTime.Now}]删除课程,无法删除,打上删除标记<hr/>" + model_UserCourse.Remarks;
							}
							else
							{
								if (result_menke_course.Message.Contains("不存在"))
								{
									model_UserCourse.Status = -1;
									model_UserCourse.Remarks = $"[{DateTime.Now}]拓课云已删除，这里直接删除课程<hr/>" + model_UserCourse.Remarks;
								}
								else
								{
									result.StatusCode = 4002;
									result.Message = result_menke_course.Message;
									return result;
								}
							}
						}
					}
					#endregion
				}

				#region 删除课节
				//所有相关课节，仅要移除的学生
				
				var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status == 1 && u.MenkeState == 1 && u.UserCourseid== user_courseid).ToList();

				//所有相关的课节，包括其他学生
				var list_AllUserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status == 1 && u.MenkeState == 1 && list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
				foreach (var model in list_UserLesson)
				{
					//网站中本学生的课节必需要删除
					model.Status = -1;
					model.Remark = $"[{DateTime.Now}]删除用户课程,清除未上课的排课课节<hr>" + model.Remark;

					//拓课云中要判断是否存在其他学生，只能移除
					var list = list_AllUserLesson.Where(u => u.UserLessonid != model.UserLessonid && u.MenkeLessonId == model.MenkeLessonId).ToList();
					if (list.Count > 0)
					{
						//移除学生,只保留其他学生
						await _menkeBaseService.ModifyLessonStudent(model.MenkeLessonId, list.Select(u => u.MenkeStudentCode + "-" + u.MenkeStudentMobile).ToList());
					}
					else
					{
						//仅此学生一人，可删除课节
						if (model.MenkeDeleteTime == 0)
						{
							await _menkeBaseService.DeleteLesson(model.MenkeLessonId);
						}
					}
				}
				_context.Updateable(list_UserLesson).UpdateColumns(u => new { u.Status, u.Remark }).ExecuteCommand();
				#endregion

				model_UserCourse.Lastchanged = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
				_context.Updateable(model_UserCourse).UpdateColumns(u => new { u.Status,u.Remarks,u.Lastchanged}).ExecuteCommand();
			}
			return result;
		}
		#endregion

		#region 调整已购课程
		/// <summary>
		/// 调整已购课程
		/// 1，删除或从旧课程中移除学生
		/// 2，删除旧课程中所有课节，或从旧课程的课节中移除学生
		/// 3，创建新课程或加入到别的课程中
		/// </summary>
		/// <param name="user_courseid">已购课程ID</param>
		/// <param name="menke_courseid">要调整的新课程ID</param>
		/// <returns></returns>
		public async Task<ApiResult> AdjustUserCourse(long user_courseid,int menke_courseid)
		{
			var result = new ApiResult();
			var students = new List<string>();
			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(user_courseid);
			if (model_UserCourse != null)
			{
				if (model_UserCourse.Status != 1) {
					result.StatusCode = 4002;
					result.Message = "已购课程不是正常状态";
					return result;
				}

				var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u=>u.MenkeCourseId == model_UserCourse.MenkeCourseId);
				if (model_MenkeCourse == null)
				{
					result.StatusCode = 4002;
					result.Message = "拓课云课程不存在";
					return result;
				}
				//var model_NewMenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == menke_courseid);
				//if (model_NewMenkeCourse == null)
				//{
				//	result.StatusCode = 4003;
				//	result.Message = "要调整的拓课云课程不存在";
				//	return result;
				//}
				var model_SkuType = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
					.Where((l, r) => l.SkuTypeid == model_UserCourse.SkuTypeid && r.Lang == _userManage.Lang)
					.Select((l, r) => new { l.SkuTypeid, r.Title, l.MenkeType, l.MenkeTemplateId })
					.First();
				if (model_SkuType == null)
				{
					result.StatusCode = 4003;
					result.Message = "上课类型(" + _userManage.Lang + ")不存在";
					return result;
				}
				var list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId && u.Status == 1).ToList();
				if (list_UserCourse.Count <= 0)
				{
					result.StatusCode = 4003;
					result.Message = "已购课程不存在";
					return result;
				}
				var list_User = _context.Queryable<WebUser>().Where(u => list_UserCourse.Select(u => u.Userid).Contains(u.Userid)).ToList();
				var model_User = list_User.FirstOrDefault(u => u.Userid == model_UserCourse.Userid);
				if (model_User == null)
				{
					result.StatusCode = 4003;
					result.Message = "用户ID不存在";
					return result;
				}
				//要移入的课程中，取相关所有已购课程
				var list_NewUserCourse = _context.Queryable<WebUserCourse>().Where(u => u.MenkeCourseId == menke_courseid && u.Status == 1).ToList();
				
				string course_name = "";

				///多人课程，移除学生即可。单人课程直接删除
				///若单人，删除拓课云失败，则改为名称前缀加[删除]
				#region 旧课程移除学生
				if (list_UserCourse.Count > 1)
				{
					course_name = string.Join(",", list_User.Where(u => u.Userid != model_UserCourse.Userid).Select(u => u.FirstName + u.LastName).ToList())+"-"
						+model_UserCourse.Title+ "-" + model_SkuType.Title;
                    students = list_User.Where(u => u.Userid != model_UserCourse.Userid).Select(u => u.MobileCode+ "-"+ u.Mobile).ToList();
					var result_menke_course = await _menkeBaseService.ModifyCourse(model_UserCourse.MenkeCourseId, course_name, students, model_SkuType.MenkeTemplateId);
					if (result_menke_course.StatusCode == 0)
					{
						model_UserCourse.Remarks = $"[{DateTime.Now}]从旧课程[{model_UserCourse.MenkeCourseId}]中,将学生移除<hr/>" + model_UserCourse.Remarks;
					}
					else
					{
						_logger.LogError("调课[user_courseid:" + user_courseid + "=>menke_courseid:" + menke_courseid + "],旧课程移除学生异常,Message=" + result_menke_course.Message);
						result.StatusCode = 4002;
						result.Message = result_menke_course.Message;
						return result;
					}
				}
				else
				{
					var result_menke_course = await _menkeBaseService.DelCourse(model_UserCourse.MenkeCourseId);
					if (result_menke_course.StatusCode == 0)
					{
						model_UserCourse.Remarks = $"[{DateTime.Now}]删除旧课程[{model_UserCourse.MenkeCourseId}]<hr/>" + model_UserCourse.Remarks;
					}
					else
					{
						if (!course_name.Contains("[删除]"))
						{
							course_name = "[删除]" + course_name;
						}
						var result_modify = await _menkeBaseService.ModifyCourse(model_UserCourse.MenkeCourseId, course_name, null, model_SkuType.MenkeTemplateId);
						if (result_modify.StatusCode == 0)
						{
							model_UserCourse.Remarks = $"[{DateTime.Now}]删除旧课程,无法删除,打上删除标记<hr/>" + model_UserCourse.Remarks;
						}
						else
						{
							_logger.LogError("调课[user_courseid:" + user_courseid + "=>menke_courseid:" + menke_courseid + "],旧课程无法删除,打上删除标记时异常,Message=" + result_modify.Message);
							result.StatusCode = 4002;
							result.Message = result_menke_course.Message;
							return result;
						}
					}
				}
				#endregion

				#region 删除旧课程中的所有课节
				//所有相关课节，仅要移除的学生
				var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status == 1 && u.MenkeState == 1 && u.UserCourseid == user_courseid).ToList();

				//所有相关的课节，包括其他学生
				var list_AllUserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status == 1 && u.MenkeState == 1 && list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
				foreach (var model in list_UserLesson)
				{
					//网站中本学生的课节必需要删除
					model.Status = -1;
					model.Remark = $"[{DateTime.Now}]删除用户课程,清除未上课的排课课节<hr>" + model.Remark;

					//拓课云中要判断是否存在其他学生，只能移除
					var list = list_AllUserLesson.Where(u => u.UserLessonid != model.UserLessonid && u.MenkeLessonId == model.MenkeLessonId).ToList();
					if (list.Count > 0)
					{
						//移除学生,只保留其他学生
						var result_lesson = await _menkeBaseService.ModifyLessonStudent(model.MenkeLessonId, list.Select(u => u.MenkeStudentCode + "-" + u.MenkeStudentMobile).ToList());
						if (result_lesson.StatusCode > 0) {
							_logger.LogError("调课[user_courseid:" + user_courseid + "=>menke_courseid:" + menke_courseid +"],修改课节["+ model.MenkeLessonId + "],移除学生时异常,Message=" + result_lesson.Message);
						}
					}
					else
					{
						//仅此学生一人，可删除课节
						var result_lesson = await _menkeBaseService.DeleteLesson(model.MenkeLessonId);
						if (result_lesson.StatusCode > 0)
						{
							_logger.LogError("调课[user_courseid:" + user_courseid + "=>menke_courseid:" + menke_courseid + "],删除课节[" + model.MenkeLessonId + "]时异常,Message=" + result_lesson.Message);
						}
					}
				}
				_context.Updateable(list_UserLesson).UpdateColumns(u => new { u.Status, u.Remark }).ExecuteCommand();
				#endregion

				#region"添加进入新课程"
				if (menke_courseid == 0)
				{
					#region "创建新课程"
					course_name = model_User.FirstName + " " + model_User.LastName + "-" + model_UserCourse.Title + "" + model_SkuType.Title;
					students.Clear();
					students.Add(model_User.MobileCode + "-" + model_User.Mobile);
					var result_course = await _menkeBaseService.CreateCourse(course_name, students, model_SkuType.MenkeTemplateId);
					if (result_course.StatusCode == 0)
					{
						model_UserCourse.MenkeCourseId = result_course.Data;
						model_UserCourse.Remarks = $"[{DateTime.Now}]调整创建新课程<hr/>" + model_UserCourse.Remarks;
					}
					else
					{
						_logger.LogError("调课[user_courseid:" + user_courseid + "=>menke_courseid:" + menke_courseid + "],创建新课程时异常,Message=" + result_course.Message); 
						result.StatusCode = 4002;
						result.Message = result_course.Message;
						return result;
					}
					#endregion
				}
				else
				{
					var list_NewUser = _context.Queryable<WebUser>().Where(u => list_NewUserCourse.Select(s => s.Userid).Contains(u.Userid)).ToList();
					students = list_NewUser.Select(u => u.MobileCode + "-" + u.Mobile).ToList();
					students.Add(model_User.MobileCode + "-" + model_User.Mobile);
					var list_name = list_NewUser.Select(u => u.FirstName + "-" + u.LastName).ToList();
					list_name.Add(model_User.FirstName + " " + model_User.LastName);
					course_name = $"{string.Join(",", list_name)}-{model_UserCourse.Title}-{model_SkuType.Title}";
					var result_new_menke_course = await _menkeBaseService.ModifyCourse(menke_courseid, course_name, students, model_SkuType.MenkeTemplateId);
					if (result_new_menke_course.StatusCode == 0)
					{
						model_UserCourse.Remarks = $"[{DateTime.Now}]加入新课程[{menke_courseid}]中<hr/>" + model_UserCourse.Remarks;
						model_UserCourse.MenkeCourseId = menke_courseid;

					}
					else
					{
						_logger.LogError("调课[user_courseid:" + user_courseid + "=>menke_courseid:" + menke_courseid + "],加入其他课程时异常,Message=" + result_new_menke_course.Message);
						result.StatusCode = 4002;
						result.Message = result_new_menke_course.Message;
						return result;
					}
				}
				#endregion
				model_UserCourse.Lastchanged = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
				_context.Updateable(model_UserCourse).UpdateColumns(u => new {u.MenkeCourseId, u.Remarks,u.Lastchanged }).ExecuteCommand();
			}
			return result;
		}
		#endregion
	}
}