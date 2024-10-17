using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Models;
using WeTalk.Web.Extensions;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class LessonController : Base.BaseController
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly TokenManager _tokenManager;

		private readonly ILogger<CourseController> _logger;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		public LessonController(IConfiguration config, IHttpContextAccessor accessor,SqlSugarScope dbcontext, TokenManager tokenManager, ILogger<CourseController> logger,
			IStringLocalizer<LangResource> localizer)
			: base(tokenManager)
		{
			_context = dbcontext;
			_accessor = accessor;
			_logger = logger;
			_localizer = localizer;
			_tokenManager = tokenManager;
		}

        #region "申请表"    
        [Authorization(Power = "Main")]
        public IActionResult CancelApply()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CancelApplyData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string UserLessonCancelids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "UserLessonCancelids");
			long UserLessonCancelid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "UserLessonCancelid", 0);
			var list_UserLessonCancel = new List<WebUserLessonCancel>();
			if (UserLessonCancelids.Trim() == "") UserLessonCancelids = "0";
			string[] arr = UserLessonCancelids.Split(',');
			int count = 0;
			string message = ""; 
			
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					message = $"[{DateTime.Now}]{AdminMasterName}删除记录";
                    count = _context.Updateable<WebUserLessonCancel>()
						.SetColumns(u => u.Status == -1)
						.SetColumns(u=>u.Remarks == SqlFunc.MergeString(message, u.Remarks))
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.UserLessonCancelid) || u.UserLessonCancelid == UserLessonCancelid)
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "refuse":
					count = _context.Updateable<WebUserLessonCancel>()
						.SetColumns(u => u.Status == 2)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.UserLessonCancelid) || u.UserLessonCancelid == UserLessonCancelid)
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["拒绝成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["拒绝失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enableu"://启用
					count = _context.Updateable<WebUserLessonCancel>()
						.SetColumns(u => u.Status == 1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.UserLessonCancelid) || u.UserLessonCancelid == UserLessonCancelid)
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["启用成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["启用失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enablef"://禁用
					count = _context.Updateable<WebUserLessonCancel>()
						.SetColumns(u => u.Status == 0)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.UserLessonCancelid) || u.UserLessonCancelid == UserLessonCancelid)
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["禁用成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["禁用失败,不存在此记录"] + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();

					int total = 0;
					var exp = new Expressionable<WebUserLessonCancel>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u =>SqlFunc.Subqueryable<WebUserLesson>().Where(s=>s.UserLessonid == u.UserLessonid && (s.MenkeStudentName.Contains(keys) || SqlFunc.MergeString(s.MenkeStudentCode, s.MenkeStudentMobile).Contains(keys) || s.MenkeTeacherName.Contains(keys) || SqlFunc.MergeString(s.MenkeTeacherCode, s.MenkeTeacherMobile).Contains(keys))).Any() || SqlFunc.MergeString(u.Message, "").Contains(keys.Trim()));
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebUserLessonCancel>().Where(exp.ToExpression());
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							query.OrderBy(sort + " desc");
						}
						else
						{
							query.OrderBy(sort);
						}
					}
					query.OrderBy("dtime desc");
					list_UserLessonCancel = query.ToPageList(page, pagesize, ref total);
                    var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => list_UserLessonCancel.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();
                    var list_MenkCourse = _context.Queryable<MenkeCourse>().Where(u => list_UserLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_UserLessonCancel.Count; i++)
					{
						var model_UserLesson = list_UserLesson.FirstOrDefault(u => u.UserLessonid == list_UserLessonCancel[i].UserLessonid);
                        var model_MenkeCourse = list_MenkCourse.FirstOrDefault(u => u.MenkeCourseId == model_UserLesson?.MenkeCourseId);
						

						str.Append("{");
						str.Append("\"UserLessonCancelid\":\"" + list_UserLessonCancel[i].UserLessonCancelid + "\",");
						str.Append("\"MenkeCourseName\":\"" + model_MenkeCourse?.MenkeName + "\",");
						str.Append("\"MenkeLessonName\":\"" + model_UserLesson?.MenkeLessonName + "\",");
						str.Append("\"MenkeLiveSerial\":\"" + model_UserLesson?.MenkeLiveSerial + "\",");
                        str.Append("\"StudentName\":\"" + model_UserLesson?.MenkeStudentName + "\",");
                        str.Append("\"StudentMobile\":\"" + model_UserLesson?.MenkeStudentCode + model_UserLesson?.MenkeStudentMobile + "\",");
                        str.Append("\"TeacherName\":\"" + model_UserLesson?.MenkeTeacherName + "\",");
                        str.Append("\"TeacherMobile\":\"" + model_UserLesson?.MenkeTeacherCode + model_UserLesson?.MenkeTeacherMobile + "\",");
						str.Append("\"Dtime\":\"" + list_UserLessonCancel[i].Dtime.ToString() + "\",");
						str.Append("\"Status\":" + list_UserLessonCancel[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_UserLessonCancel.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					_context.Updateable(list_UserLessonCancel).UpdateColumns(u => u.Userid).ExecuteCommand();
					break;
			}
			return Content(str.ToString());
		}

        [Authorization(Power = "Main")]
        public async Task<IActionResult> CancelApplyAdd(long UserLessonCancelid = 0)
		{
			var vm_ListAdd = new ViewModels.Lesson.CancelApplyAdd();

			var model_CancelApply = _context.Queryable<WebUserLessonCancel>().InSingle(UserLessonCancelid);
			if (model_CancelApply != null)
			{
				var model_UserLesson = _context.Queryable<WebUserLesson>().InSingle(model_CancelApply.UserLessonid);
				if (model_UserLesson != null)
				{
					var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == model_UserLesson.MenkeCourseId);
					vm_ListAdd.MenkeStudentName = model_UserLesson.MenkeStudentName;
					vm_ListAdd.MenkeStudentMobile = model_UserLesson.MenkeStudentCode+ model_UserLesson.MenkeStudentMobile;
					vm_ListAdd.MenkeTeacherName = model_UserLesson.MenkeTeacherName;
					vm_ListAdd.MenkeTeacherMobile = model_UserLesson.MenkeTeacherCode + model_UserLesson.MenkeTeacherMobile;
					vm_ListAdd.MenkeCourseName = model_MenkeCourse?.MenkeName;
					vm_ListAdd.MenkeLessonName = model_UserLesson.MenkeLessonName;
					vm_ListAdd.MenkeLiveSerial = model_UserLesson.MenkeLiveSerial;
				}
				vm_ListAdd.Message = model_CancelApply.Message;
				vm_ListAdd.Dtime = model_CancelApply.Dtime;
				vm_ListAdd.Status = model_CancelApply.Status;
			}

			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}
		#endregion

		#region "申请表(只读)"    
		[Authorization(Power = "Main")]
		public IActionResult CancelApplyRead()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> CancelApplyReadData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string UserLessonCancelids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "UserLessonCancelids");
			long UserLessonCancelid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "UserLessonCancelid", 0);
			var list_UserLessonCancel = new List<WebUserLessonCancel>();
			if (UserLessonCancelids.Trim() == "") UserLessonCancelids = "0";
			string[] arr = UserLessonCancelids.Split(',');
	
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();

					int total = 0;
					var exp = new Expressionable<WebUserLessonCancel>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebUserLesson>().Where(s => s.UserLessonid == u.UserLessonid && (s.MenkeStudentName.Contains(keys) || SqlFunc.MergeString(s.MenkeStudentCode, s.MenkeStudentMobile).Contains(keys) || s.MenkeTeacherName.Contains(keys) || SqlFunc.MergeString(s.MenkeTeacherCode, s.MenkeTeacherMobile).Contains(keys))).Any() || SqlFunc.MergeString(u.Message, "").Contains(keys.Trim()));
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebUserLessonCancel>().Where(exp.ToExpression());
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							query.OrderBy(sort + " desc");
						}
						else
						{
							query.OrderBy(sort);
						}
					}
					query.OrderBy("dtime desc");
					list_UserLessonCancel = query.ToPageList(page, pagesize, ref total);
					var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => list_UserLessonCancel.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();
					var list_MenkCourse = _context.Queryable<MenkeCourse>().Where(u => list_UserLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_UserLessonCancel.Count; i++)
					{
						var model_UserLesson = list_UserLesson.FirstOrDefault(u => u.UserLessonid == list_UserLessonCancel[i].UserLessonid);
						var model_MenkeCourse = list_MenkCourse.FirstOrDefault(u => u.MenkeCourseId == model_UserLesson?.MenkeCourseId);


						str.Append("{");
						str.Append("\"UserLessonCancelid\":\"" + list_UserLessonCancel[i].UserLessonCancelid + "\",");
						str.Append("\"MenkeCourseName\":\"" + model_MenkeCourse?.MenkeName + "\",");
						str.Append("\"MenkeLessonName\":\"" + model_UserLesson?.MenkeLessonName + "\",");
						str.Append("\"MenkeLiveSerial\":\"" + model_UserLesson?.MenkeLiveSerial + "\",");
						str.Append("\"StudentName\":\"" + model_UserLesson?.MenkeStudentName + "\",");
						str.Append("\"StudentMobile\":\"" + model_UserLesson?.MenkeStudentCode + model_UserLesson?.MenkeStudentMobile + "\",");
						str.Append("\"TeacherName\":\"" + model_UserLesson?.MenkeTeacherName + "\",");
						str.Append("\"TeacherMobile\":\"" + model_UserLesson?.MenkeTeacherCode + model_UserLesson?.MenkeTeacherMobile + "\",");
						str.Append("\"Dtime\":\"" + list_UserLessonCancel[i].Dtime.ToString() + "\",");
						str.Append("\"Status\":" + list_UserLessonCancel[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_UserLessonCancel.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					_context.Updateable(list_UserLessonCancel).UpdateColumns(u => u.Userid).ExecuteCommand();
					break;
			}
			return Content(str.ToString());
		}

		[Authorization(Power = "Main")]
		public async Task<IActionResult> CancelApplyReadAdd(long UserLessonCancelid = 0)
		{
			var vm_ListAdd = new ViewModels.Lesson.CancelApplyAdd();

			var model_CancelApply = _context.Queryable<WebUserLessonCancel>().InSingle(UserLessonCancelid);
			if (model_CancelApply != null)
			{
				var model_UserLesson = _context.Queryable<WebUserLesson>().InSingle(model_CancelApply.UserLessonid);
				if (model_UserLesson != null)
				{
					var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == model_UserLesson.MenkeCourseId);
					vm_ListAdd.MenkeStudentName = model_UserLesson.MenkeStudentName;
					vm_ListAdd.MenkeStudentMobile = model_UserLesson.MenkeStudentCode + model_UserLesson.MenkeStudentMobile;
					vm_ListAdd.MenkeTeacherName = model_UserLesson.MenkeTeacherName;
					vm_ListAdd.MenkeTeacherMobile = model_UserLesson.MenkeTeacherCode + model_UserLesson.MenkeTeacherMobile;
					vm_ListAdd.MenkeCourseName = model_MenkeCourse?.MenkeName;
					vm_ListAdd.MenkeLessonName = model_UserLesson.MenkeLessonName;
					vm_ListAdd.MenkeLiveSerial = model_UserLesson.MenkeLiveSerial;
				}
				vm_ListAdd.Message = model_CancelApply.Message;
				vm_ListAdd.Dtime = model_CancelApply.Dtime;
				vm_ListAdd.Status = model_CancelApply.Status;
			}

			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}
		#endregion

	}
}
