using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto.Menke;
using WeTalk.Web.Extensions;
using WeTalk.Web.ViewModels.User;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class UserController : Base.BaseController
	{
		private readonly TokenManager _tokenManager;
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		private readonly ICommonService _commonService;
		private readonly IMenkeService _menkeService;
		private readonly ICourseService _courseService;
		private readonly ILogger<UserController> _logger;
		private readonly IPubConfigService _pubConfigService;

		private string prefix = "{local}";//图片上传本地服务器的前缀标签
		public UserController(TokenManager tokenManager, IStringLocalizer<LangResource> localizer, SqlSugarScope context, IHttpContextAccessor accessor, ICommonService commonService,
			IMenkeService menkeService, ICourseService courseService, ILogger<UserController> logger,IPubConfigService pubConfigService
			)
			: base(tokenManager)
		{
			_tokenManager = tokenManager;
			_context = context;
			_accessor = accessor;
			_localizer = localizer;
			_commonService = commonService;
			_menkeService = menkeService;
			_courseService = courseService;
			_logger = logger;
			_pubConfigService = pubConfigService;
		}

		#region "学生管理（只读）" 
		#region "会员列表" 
		[Authorization(Power = "Main")]
		public IActionResult ListRead()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> ListReadData(string keys,string status="",string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long Userid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Userid", 0);
			string Userids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Userids");
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
                case "union"://关联用户
                    var model_User = _context.Queryable<WebUser>().InSingle(Userid);
                    if (model_User != null)
                    {
                        var result = await _menkeService.UnionMenkeUserId(model_User.MobileCode, model_User.Mobile, 0);
                        if (result.StatusCode == 0)
                        {
							model_User.MenkeUserId = result.Data;
							_context.Updateable(model_User).UpdateColumns(u=>u.MenkeUserId).ExecuteCommand();
                            str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["关联成功"] + "！\"}");
                        }
                        else
                        {
                            str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + result.Message + "！\"}");
                        }
                    }
                    else
                    {
                        str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["关联失败"] + "！\"}");
                    }
                    break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					string son_str = "";
					var exp = new Expressionable<WebUser>();
					exp.AndIF(!string.IsNullOrEmpty(keys),u => SqlFunc.MergeString(u.Email,"").Contains(keys) || SqlFunc.MergeString(u.FirstName, u.LastName,"").Contains(keys) || SqlFunc.MergeString(u.MobileCode, u.Mobile,"").Contains(keys));
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							sort += " desc,";
						}
						else {
							sort += ",";
						}
					}
					sort += "userid desc";

					var total = 0;
					var list_User = _context.Queryable<WebUser>().Where(exp.ToExpression()).OrderBy(sort).ToPageList(page,pagesize,ref total);
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_User.Count; i++)
					{
						str.Append("{");
						str.Append("\"Userid\":\"" + list_User[i].Userid + "\",");
						str.Append("\"Name\":\"" + JsonHelper.JsonCharFilter(list_User[i].FirstName + ""+ list_User[i].LastName) + "\",");
						str.Append("\"Mobile\":\"" + list_User[i].MobileCode+ " " + list_User[i].Mobile + "\",");
						str.Append("\"Email\":\"" + list_User[i].Email + "\",");
						str.Append("\"MenkeUserId\":\"" + list_User[i].MenkeUserId + "\",");
						str.Append("\"HeadImg\":\"" + _commonService.ResourceDomain(string.IsNullOrEmpty(list_User[i].HeadImg) ? "/Upfile/images/none.png" : list_User[i].HeadImg) + "\",");
						str.Append("\"Gender\":\"" + list_User[i].Gender + "\",");
						str.Append("\"Birthdate\":\"" + list_User[i].Birthdate.ToString("d") + "\",");
						str.Append("\"Regtime\":\"" + list_User[i].Regtime + "\",");
						str.Append("\"Dtime\":\"" + list_User[i].Dtime + "\","); 
						str.Append("\"Status\":\"" + list_User[i].Status + "\",");
                        str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(son_str);
						if (i < (list_User.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}
			return Content(str.ToString());
		}
		#endregion

		#region "查看修改会员"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> ListReadAdd(long Userid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_ListAdd = new ListAdd();
			if (Lang.ToLower() == "zh-cn")
			{
				vm_ListAdd.CountryItem = _context.Queryable<PubCountry>()
					.OrderBy(u => u.Country).ToList()
					.Select(u => new SelectListItem(u.Country, u.Code))
					.ToList();
			}
			else
			{
				vm_ListAdd.CountryItem = _context.Queryable<PubCountry>()
					.OrderBy(u => u.CountryEn).ToList()
					.Select(u => new SelectListItem(u.CountryEn, u.Code))
					.ToList();
			}

			vm_ListAdd.UtcItem = _context.Queryable<PubTimezone>()
				.Where(u => u.Status == 1)
				.OrderBy(u => u.Title).ToList()
				.Select(u => new SelectListItem(u.Title, u.Timezoneid.ToString()))
				.ToList();

			var model_User = _context.Queryable<WebUser>().InSingle(Userid);
			if (model_User != null)
			{
				vm_ListAdd.Userid = model_User.Userid;
				vm_ListAdd.Username = model_User.Username;
				vm_ListAdd.HeadImg = _commonService.ResourceDomain(string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg);
				vm_ListAdd.Email = model_User.Email;
				vm_ListAdd.Gender = model_User.Gender;
				vm_ListAdd.MobileCode = model_User.MobileCode;
				vm_ListAdd.Mobile = model_User.Mobile;
				vm_ListAdd.FirstName = model_User.FirstName;
				vm_ListAdd.LastName = model_User.LastName;
				vm_ListAdd.Birthdate = model_User.Birthdate;
				vm_ListAdd.Native = model_User.Native;
				vm_ListAdd.Timezoneid = model_User.Timezoneid;
				vm_ListAdd.Education = model_User.Education;
				vm_ListAdd.Status = model_User.Status;
				vm_ListAdd.Remark = model_User.Remark;
				vm_ListAdd.MenkeUserId = model_User.MenkeUserId;
				var model_Timezone = _context.Queryable<PubTimezone>().First(u => u.Timezoneid == model_User.Timezoneid);
				if (model_Timezone != null)
				{
					vm_ListAdd.Timezone = model_Timezone.Title + $"(UTC {model_Timezone.UtcSec/3600})";
				}
			}
			else {
				vm_ListAdd.Status = 1;
				vm_ListAdd.Birthdate.AddYears(-2);
			}
			return View(vm_ListAdd);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> ListReadAdd(ListAdd vm_ListAdd)
		{
			string message = ""; 
			var model_Master = _tokenManager.GetAdminInfo();
			var model_User = _context.Queryable<WebUser>().InSingle(vm_ListAdd.Userid);
			if (model_User!=null)
			{
				if (model_User.MenkeUserId == 0)
				{
					var result_menke = await _menkeService.CreateStudents(new MenkeStudentDto
					{
						name = vm_ListAdd.FirstName + " " + vm_ListAdd.LastName,
						nickname = vm_ListAdd.FirstName + " " + vm_ListAdd.LastName,
						sex = model_User.Gender,
						birthday = vm_ListAdd.Birthdate.ToString("d"),
						locale = (vm_ListAdd.CountryCode+"").ToUpper(),
						code = vm_ListAdd.MobileCode,
						mobile = vm_ListAdd.Mobile,
						p_name = ""
					});
					if (result_menke.StatusCode == 0) model_User.MenkeUserId = result_menke.Data;
				}
				else {
					var result_menke = await _menkeService.ModifyStudents(vm_ListAdd.MobileCode,vm_ListAdd.Mobile, new MenkeStudentDto{
						name = vm_ListAdd.FirstName + " " + vm_ListAdd.LastName,
						nickname = vm_ListAdd.FirstName + " " + vm_ListAdd.LastName,
						sex = model_User.Gender,
						birthday = vm_ListAdd.Birthdate.ToString("d"),
						p_name = model_User.GuardianName
                    });
				}
				if (!string.IsNullOrEmpty(vm_ListAdd.Message)) {
					message = $"[{DateTime.Now}]{model_Master.Username}" + _localizer["添加备注"]+":"+ vm_ListAdd.Message;
				}

				if (string.IsNullOrEmpty(model_User.Remark) || model_User.Remark.Length < 4000)
				{
					model_User.Remark = message + "<hr>" + model_User.Remark;
				}
				else {
					model_User.Remark = message + "<hr>" + model_User.Remark.Substring(0,4000);
				}
				_context.Updateable(model_User).UpdateColumns(u=>u.Remark).ExecuteCommand();
				return Content("<script>parent.$('#win').window('close');alert('"+_localizer["保存信息成功"] +"');parent.$('#table_view').datagrid('reload');</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				return Content("<script>alert('"+_localizer["您无权限创建学生信息"] +"');parent.$('#table_view').datagrid('reload');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
		#endregion

		#region "学生已购课程(只读)" 
		#region "学生课程列表" 
		[Authorization(Power = "Main")]
		public IActionResult UserCourseRead()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserCourseReadData(string keys = "", string mobile = "", string type = "", string status = "", string begintime = "", string endtime = "")
		{
			var model_Master = _tokenManager.GetAdminInfo();
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long UserCourseid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "UserCourseid", 0);
			string UserCourseids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "UserCourseids");
			WebUser model_User = null;
			List<WebUserCourse> list_UserCourse;
			List<long> list_UserCourseid = new List<long>();
			string url = "";
			int n = 0;

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					if (UserCourseids.Trim() == "") UserCourseids = "0";
					list_UserCourseid = UserCourseids.Split(',').Select(u => long.Parse(u)).ToList();
					if (UserCourseid > 0) list_UserCourseid.Add(UserCourseid);
					list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => u.Status != -1 && list_UserCourseid.Contains(u.UserCourseid)).ToList();
					foreach (var model in list_UserCourse)
					{
						model.Status = -1;
						model.Remarks = $"[{DateTime.Now}]{model_Master?.Username}删除课程<hr>" + model.Remarks;
					}
					n = _context.Updateable(list_UserCourse).UpdateColumns(u => new { u.Status, u.Remarks }).ExecuteCommand();
					if (n > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enableu"://解锁
					if (UserCourseids.Trim() == "") UserCourseids = "0";
					list_UserCourseid = UserCourseids.Split(',').Select(u => long.Parse(u)).ToList();
					if (UserCourseid > 0) list_UserCourseid.Add(UserCourseid);
					list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => list_UserCourseid.Contains(u.UserCourseid)).ToList();
					foreach (var model in list_UserCourse)
					{
						model.Status = 1;
						model.Remarks = $"[{DateTime.Now}]{model_Master?.Username}解锁课程<hr>" + model.Remarks;
					}
					n = _context.Updateable(list_UserCourse).UpdateColumns(u => new { u.Status, u.Remarks }).ExecuteCommand();
					if (n > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["解锁成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["解锁失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enablef"://锁定,注意清除所有已排课未上课的
					if (UserCourseids.Trim() == "") UserCourseids = "0";
					list_UserCourseid = UserCourseids.Split(',').Select(u => long.Parse(u)).ToList();
					if (UserCourseid > 0) list_UserCourseid.Add(UserCourseid);
					list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => list_UserCourseid.Contains(u.UserCourseid)).ToList();
					foreach (var model in list_UserCourse)
					{
						model.Status = 2;
						model.Remarks = $"[{DateTime.Now}]{model_Master?.Username}锁定课程<hr>" + model.Remarks;
					}
					n = _context.Updateable(list_UserCourse).UpdateColumns(u => new { u.Status, u.Remarks }).ExecuteCommand();
					if (n > 0)
					{
						//清除未开始的课节,注意，1V1可以直接删，多人只能移除

						//所有相关课节，仅要移除的学生
						var list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status == 1 && u.MenkeState == 1 && list_UserCourse.Select(s => s.UserCourseid).Contains(u.UserCourseid)).ToList();

						//所有相关的课节，包括其他学生
						var list_AllUserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status == 1 && u.MenkeState == 1 && list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
						foreach (var model in list_UserLesson)
						{
							//网站中本学生的课节必需要删除
							model.Status = -1;
							model.Remark = $"[{DateTime.Now}]{AdminMasterName}锁定用户课程,清除未上课的排课课节<hr>" + model.Remark;

							//拓课云中要判断是否存在其他学生，只能移除
							var list = list_AllUserLesson.Where(u => u.UserLessonid != model.UserLessonid && u.MenkeLessonId == model.MenkeLessonId).ToList();
							if (list.Count > 0)
							{
								//移除学生,只保留其他学生
								await _menkeService.ModifyLessonStudent(model.MenkeLessonId, list.Select(u => u.MenkeStudentCode + "-" + u.MenkeStudentMobile).ToList());
							}
							else
							{
								//仅此学生一人，可删除课节
								await _menkeService.DeleteLesson(model.MenkeLessonId);
							}
						}
						_context.Updateable(list_UserLesson).UpdateColumns(u => new { u.Status, u.Remark }).ExecuteCommand();

						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["锁定成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["锁定失败,不存在此记录"] + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					string son_str = "";

					int btime = !string.IsNullOrEmpty(begintime) ? DateHelper.ConvertDateTimeInt(DateTime.Parse(begintime)) : 0;
					int etime = !string.IsNullOrEmpty(endtime) ? DateHelper.ConvertDateTimeInt(DateTime.Parse(endtime)) : 0;
					var exp = new Expressionable<WebUserCourse>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u =>
						SqlFunc.Subqueryable<WebUser>().Where(s => s.Userid == u.Userid && (SqlFunc.MergeString(s.Mobile, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.Email, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.FirstName, s.LastName, "").Contains(keys.Trim()))).Any());
					exp.AndIF(!string.IsNullOrEmpty(mobile), u => SqlFunc.Subqueryable<WebUser>().Where(s => s.Userid == u.Userid && s.Mobile.Contains(mobile)).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(type), u => u.Type.ToString() == type.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));

					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							sort += " desc,";
						}
						else
						{
							sort += ",";
						}
					}
					sort += "user_courseid desc";

					var total = 0;
					list_UserCourse = _context.Queryable<WebUserCourse>().Where(exp.ToExpression()).OrderBy(sort).ToPageList(page, pagesize, ref total);
					//var list_Course = _context.Queryable<WebCourseLang>().Where(u => u.Lang == Lang && list_UserCourse.Select(s => s.Courseid).Contains(u.Courseid)).ToList();
					var list_User = _context.Queryable<WebUser>().Where(u => list_UserCourse.Select(s => s.Userid).Contains(u.Userid)).ToList();
					var list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_UserCourse.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
					var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == Lang && list_UserCourse.Select(s => s.SkuTypeid).Contains(u.SkuTypeid)).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_UserCourse.Count; i++)
					{
						model_User = list_User.FirstOrDefault(u => u.Userid == list_UserCourse[i].Userid);
						var model_MenkeCourse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == list_UserCourse[i].MenkeCourseId);
						//var model_Course = list_Course.FirstOrDefault(u => u.Courseid == list_UserCourse[i].Courseid);
						var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == list_UserCourse[i].SkuTypeid);
						str.Append("{");
						str.Append("\"UserCourseid\":\"" + list_UserCourse[i].UserCourseid + "\",");
						str.Append("\"Type\":" + list_UserCourse[i].Type + ",");
						str.Append("\"OrderSn\":\"" + list_UserCourse[i].OrderSn + "\",");
						str.Append("\"Name\":\"" + model_User?.FirstName + " " + model_User?.LastName + "\",");
						str.Append("\"Mobile\":\"" + model_User?.MobileCode + "-" + model_User?.Mobile + "\",");
						str.Append("\"MenkeName\":\"" + (model_MenkeCourse?.MenkeName??"等待同步中") + "\",");
                        str.Append("\"Title\":\"" + list_UserCourse[i].Title + "\",");
						str.Append("\"SkuType\":\"" + model_SkuType?.Title + "\",");
						str.Append("\"ClassHour\":" + list_UserCourse[i].ClassHour + ",");
						str.Append("\"Classes\":" + list_UserCourse[i].Classes + ",");
						str.Append("\"Dtime\":\"" + list_UserCourse[i].Dtime + "\",");
                        str.Append("\"Status\":" + list_UserCourse[i].Status + ",");
                        str.Append("\"Istrial\":" + list_UserCourse[i].Istrial + ",");
                        str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(son_str);
						if (i < (list_UserCourse.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}

		#endregion

		#region "添加修改学生课程"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserCourseReadAdd(long UserCourseid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_Add = new UserCourseAdd();
			vm_Add.MenkeCourseItem = _context.Queryable<MenkeCourse>()
				.Where(u => u.Status == 1 && u.MenkeDeleteTime == 0 && u.Istrial==0)
				.Select(u => new { u.MenkeName, u.MenkeCourseId }).ToList()
				.Select(u => new SelectListItem(u.MenkeName, u.MenkeCourseId.ToString())).ToList();

			vm_Add.MenkeCourseItem.Insert(0, new SelectListItem(_localizer["请选择拓课云课程"], "0"));
			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(UserCourseid);
			if (model_UserCourse != null)
			{

				var model_User = _context.Queryable<WebUser>().InSingle(model_UserCourse.Userid);
				var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId);
				var model_SkuType = _context.Queryable<WebSkuTypeLang>().First(u => u.Lang == Lang && u.SkuTypeid == model_UserCourse.SkuTypeid);


				vm_Add.UserCourseid = model_UserCourse.UserCourseid;
				vm_Add.OrderSn = model_UserCourse.OrderSn;
				if (model_User != null)
				{
					vm_Add.Name = model_User.FirstName + " " + model_User.LastName;
					vm_Add.Email = model_User.Email;
					vm_Add.Mobile = (model_User.Mobile + "").Replace(" ", "").Replace("-", "");
					vm_Add.MobileCode = model_User.MobileCode;
				}
				if (model_MenkeCourse != null)
				{
					vm_Add.MenkeName = model_MenkeCourse.MenkeName;
					vm_Add.MenkeCourseId = model_MenkeCourse.MenkeCourseId;
				}
				vm_Add.Title = model_UserCourse.Title;
				vm_Add.SkuType = model_SkuType?.Title;
				vm_Add.ClassHour = model_UserCourse.ClassHour;
				vm_Add.Classes = model_UserCourse.Classes;
				vm_Add.Status = model_UserCourse.Status;
				vm_Add.Remarks = model_UserCourse.Remarks;
				vm_Add.SkuTypeid = model_UserCourse.SkuTypeid;
			}
			else
			{
				vm_Add.Status = 1;
			}
			return View(vm_Add);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserCourseReadAdd(UserCourseAdd vm_Add)
		{
			string message = "";
			var model_Master = _tokenManager.GetAdminInfo();
			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(vm_Add.UserCourseid);
			if (model_UserCourse != null)
			{
				if (!string.IsNullOrEmpty(vm_Add.Message1))
				{
					message = $"[{DateTime.Now}]{model_Master.Username}增加备注:" + vm_Add.Message1;
				}
				if ((model_UserCourse.Remarks + "").Length < 4000)
				{
					model_UserCourse.Remarks = message + "<hr>" + model_UserCourse.Remarks;
				}
				else
				{
					model_UserCourse.Remarks = message + "<hr>" + model_UserCourse.Remarks.Substring(0, 4000);
				}
				_context.Updateable(model_UserCourse).ExecuteCommand();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改学生课程"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				return Content("<script>alert('" + _localizer["不允许在网站后台添加学生课程操作"] + "!')</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
		#endregion

		#region "已排课节管理(只读)" 
		#region "已排课节列表" 
		[Authorization(Power = "Main")]
		public IActionResult UserLessonRead()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserLessonReadData(string keys = "", string status = "", string menke_state = "", string is_user_courseid="",string istrial="", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long UserLessonid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "UserLessonid", 0);
			string UserLessonids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "UserLessonids");
			WebUser model_User = null;
			List<WebUserLesson> list_UserLesson;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "unionid"://手工关联单条记录的ID
					await _menkeService.UnionLessonUserid(_context.Queryable<WebUserLesson>().Where(u => u.UserLessonid == UserLessonid).Select(u => u.MenkeCourseId).ToList());
					str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["同步关联完成"] + "！\"}");
					break;
				case "union_student"://关联学生
					long userid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Userid", 0);
					model_User = _context.Queryable<WebUser>().First(u => u.Userid == userid && !string.IsNullOrEmpty(u.MobileCode) && !string.IsNullOrEmpty(u.Mobile));
					if (model_User != null)
					{
						var result_student = await _menkeService.UnionMenkeUserId(model_User.MobileCode, model_User.Mobile, 0);
						if (result_student.StatusCode == 0)
						{
							model_User.MenkeUserId = result_student.Data;
							_context.Updateable(model_User).UpdateColumns(u => u.MenkeUserId).ExecuteCommand();
							str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["同步关联完成"] + "！\"}");
						}
						else
						{
							str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["拓课云查询学生失败"] + "！\"}");
						}
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["用户手机号码资料不完整"] + "！\"}");
					}
					break;
				case "union_teacher"://关联老师
					long teacherid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Teacherid", 0);
					var model_Teacher = _context.Queryable<WebTeacher>().First(u => u.Teacherid == teacherid && !string.IsNullOrEmpty(u.MobileCode) && !string.IsNullOrEmpty(u.Mobile));
					if (model_Teacher != null)
					{
						var result_student = await _menkeService.UnionMenkeUserId(model_Teacher.MobileCode, model_Teacher.Mobile, 0);
						if (result_student.StatusCode == 0)
						{
							model_Teacher.MenkeUserId = result_student.Data;
							_context.Updateable(model_Teacher).UpdateColumns(u => u.MenkeUserId).ExecuteCommand();
							str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["同步关联完成"] + "！\"}");
						}
						else
						{
							str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["拓课云查询老师失败"] + "！\"}");
						}
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["老师手机号码资料不完整"] + "！\"}");
					}
					break;
				case "sync"://手工实时拉取最新信息
					var starttime = DateTime.UtcNow.AddDays(-1);
					var result = await _menkeService.LessonSync(DateHelper.ConvertDateTimeInt(starttime).ToString());
					if (result.StatusCode == 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + string.Format(_localizer["同步时间[{0}],拉取成功"], starttime) + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["同步拉取失败"] + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					string son_str = "";
					var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
					int btime = !string.IsNullOrEmpty(begintime) ? DateHelper.ConvertDateTimeInt(DateTime.Parse(begintime)) : 0;
					int etime = !string.IsNullOrEmpty(endtime) ? DateHelper.ConvertDateTimeInt(DateTime.Parse(endtime)) : 0;
					var exp = new Expressionable<WebUserLesson>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u =>
						SqlFunc.Subqueryable<WebUser>().Where(s => s.Userid == u.Userid && (SqlFunc.MergeString(s.Mobile, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.Email, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.FirstName, s.LastName, "").Contains(keys.Trim()))).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(istrial), u => u.Istrial.ToString() == istrial.Trim());
					if (!string.IsNullOrEmpty(is_user_courseid)) {
						switch (is_user_courseid)
						{
							case "0":
								exp.And(u => u.UserCourseid == 0);
								break;
							case "1":
                                exp.And(u => u.UserCourseid > 0);
                                break;
						}
					}
					if (!string.IsNullOrEmpty(menke_state)) {
						switch (menke_state)
						{
							case "0":
								exp.And(u => (u.Status == 0 || u.MenkeDeleteTime > 0 || u.MenkeState == 0));
								break;
							case "1":
								exp.And(u => u.MenkeState == 1 && u.Status == 1 && u.MenkeDeleteTime == 0 && u.MenkeEndtime >= thetime);
								break;
							case "2":
								exp.And(u => u.MenkeState == 2 && u.Status == 1 && u.MenkeDeleteTime == 0);
								break;
							case "3":
								exp.And(u => u.MenkeState == 3 && u.Status == 1 && u.MenkeDeleteTime == 0);
								break;
							case "4":
								exp.And(u => (u.MenkeState == 4 || (u.MenkeState == 1 && u.MenkeEndtime < thetime)) && u.Status == 1 && u.MenkeDeleteTime == 0);
								break;
						}
					}
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.MenkeStarttime >= btime);
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.MenkeStarttime <= etime);
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							sort += " desc,";
						}
						else
						{
							sort += ",";
						}
					}
					sort += "menke_lesson_id desc";

					var total = 0;
					list_UserLesson = _context.Queryable<WebUserLesson>().Where(exp.ToExpression()).OrderBy(sort).ToPageList(page, pagesize, ref total);
					var list_User = _context.Queryable<WebUser>().Where(u => list_UserLesson.Select(s => s.Userid).Contains(u.Userid)).ToList();
					var list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_UserLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
					var list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
					var list_Report = _context.Queryable<WebUserLessonReport>().Where(u => list_UserLesson.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();
					var list_Homework = _context.Queryable<MenkeHomework>().Where(u => list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
					var list_HomeworkSubmit = _context.Queryable<MenkeHomeworkSubmit>().Where(u => list_Homework.Select(s => s.MenkeHomeworkId).Contains(u.MenkeHomeworkId)).ToList();
					var list_HomeworkRemark = _context.Queryable<MenkeHomeworkRemark>().Where(u => list_HomeworkSubmit.Select(s => s.MenkeSubmitId).Contains(u.MenkeSubmitId)).ToList();
					
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_UserLesson.Count; i++)
					{
						model_User = list_User.FirstOrDefault(u => u.Userid == list_UserLesson[i].Userid);
						var model_MenkeCourse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == list_UserLesson[i].MenkeCourseId);
						var model_MenkeLesson = list_MenkeLesson.FirstOrDefault(u => u.MenkeLessonId == list_UserLesson[i].MenkeLessonId);
						var model_Homework = list_Homework.FirstOrDefault(u => u.MenkeLessonId == list_UserLesson[i].MenkeLessonId);
						var model_HomeworkSubmit = list_HomeworkSubmit.Where(u => u.MenkeSubmitDelTime == 0 && u.MenkeStudentCode == list_UserLesson[i].MenkeStudentCode && u.MenkeStudentMobile == list_UserLesson[i].MenkeStudentMobile && u.MenkeHomeworkId == model_Homework?.MenkeHomeworkId).OrderByDescending(u => u.MenkeSubmitTime).FirstOrDefault();
						var model_HomeworkRemark = list_HomeworkRemark.Where(u => u.MenkeSubmitId == model_HomeworkSubmit?.MenkeSubmitId).OrderByDescending(u => u.MenkeIsPass).FirstOrDefault();
						var time1 = DateHelper.ConvertIntToDateTime(list_UserLesson[i].MenkeStarttime.ToString()).ToLocalTime();
						var time2 = DateHelper.ConvertIntToDateTime(list_UserLesson[i].MenkeEndtime.ToString()).ToLocalTime();

						str.Append("{");
						str.Append("\"UserLessonid\":\"" + list_UserLesson[i].UserLessonid + "\",");
						str.Append("\"UserCourseid\":\"" + list_UserLesson[i].UserCourseid + "\",");
						str.Append("\"Type\":" + (list_UserLesson[i].OnlineCourseid > 0 ? 1 : 0) + ",");
						str.Append("\"IsReport\":" + (list_Report.Any(u => u.UserLessonid == list_UserLesson[i].UserLessonid) ? 1 : 0) + ",");
						str.Append("\"IsHomework\":" + (model_Homework != null ? 1 : 0) + ",");
						str.Append("\"IsHomeworkSubmit\":" + (model_HomeworkSubmit != null ? 1 : 0) + ",");
						str.Append("\"Istrial\":\"" + (model_MenkeCourse?.Istrial == 1 ? "是" : "否") + "\",");
						str.Append("\"IsPass\":" + (model_HomeworkRemark != null ? model_HomeworkRemark.MenkeIsPass : -1) + ",");
						str.Append("\"Email\":\"" + model_User?.Email + "\",");
						str.Append("\"Userid\":" + list_UserLesson[i].Userid + ",");
						str.Append("\"MenkeUserId\":\"" + list_UserLesson[i].MenkeUserId + "\",");
						str.Append("\"Teacherid\":" + list_UserLesson[i].Teacherid + ",");
						str.Append("\"CourseSkuid\":" + list_UserLesson[i].CourseSkuid + ",");
						str.Append("\"MenkeLiveSerial\":\"" + list_UserLesson[i].MenkeLiveSerial + "\",");
                        str.Append("\"MenkeCourseId\":\"" + list_UserLesson[i].MenkeCourseId + "\",");
                        str.Append("\"MenkeLessonId\":\"" + list_UserLesson[i].MenkeLessonId + "\",");
                        str.Append("\"MenkeLessonName\":\"" + JsonHelper.JsonCharFilter(list_UserLesson[i].MenkeLessonName) + "\",");
                        str.Append("\"MenkeTeacherName\":\"" + JsonHelper.JsonCharFilter(list_UserLesson[i].MenkeTeacherName) + "\",");
                        str.Append("\"MenkeStudentName\":\"" + JsonHelper.JsonCharFilter(list_UserLesson[i].MenkeStudentName) + "\",");
                        str.Append("\"MenkeStudentMobile\":\"" + list_UserLesson[i].MenkeStudentCode + " " + list_UserLesson[i].MenkeStudentMobile + "\",");
                        str.Append("\"MenkeName\":\"" + JsonHelper.JsonCharFilter(model_MenkeCourse?.MenkeName + "") + "\",");
                        str.Append("\"MenkeTime\":\"" + JsonHelper.JsonCharFilter(time1.ToString("yyyy-MM-dd") + "<br/>" + time1.ToString("HH:mm:ss") + " - " + time2.ToString("HH:mm:ss")) + "\",");
						str.Append("\"MenkeState\":" + ((list_UserLesson[i].MenkeState == 1 && list_UserLesson[i].MenkeEndtime < thetime) ? 4 : list_UserLesson[i].MenkeState) + ",");
						str.Append("\"TeacherCode\":\"" + model_MenkeLesson?.TeacherCode + "\",");
						str.Append("\"Dtime\":\"" + list_UserLesson[i].Dtime + "\",");
						str.Append("\"MenkeDeleteTime\":" + list_UserLesson[i].MenkeDeleteTime + ",");
                        str.Append("\"Status\":" + list_UserLesson[i].Status + ",");
                        str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(son_str);
						if (i < (list_UserLesson.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}

		#endregion

		#region "添加修改已排课节"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserLessonReadAdd(long UserLessonid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_Add = new UserLessonAdd();
			var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
			var model_UserLesson = _context.Queryable<WebUserLesson>().InSingle(UserLessonid);
			if (model_UserLesson != null)
			{
				var model_MenkeLesson = _context.Queryable<MenkeLesson>().First(u => u.MenkeLessonId == model_UserLesson.MenkeLessonId);
				var model_Report = _context.Queryable<WebUserLessonReport>().First(u => model_UserLesson.UserLessonid == u.UserLessonid);

				vm_Add.UserLessonid = model_UserLesson.UserLessonid;
				vm_Add.MenkeLessonName = model_UserLesson.MenkeLessonName;
				vm_Add.MenkeState = ((model_UserLesson.MenkeState == 1 && model_UserLesson.MenkeEndtime < thetime) ? 4 : model_UserLesson.MenkeState);
				vm_Add.MenkeStarttime = model_UserLesson.MenkeStarttime;
				vm_Add.MenkeEndtime = model_UserLesson.MenkeEndtime;
				vm_Add.Teacherid = model_UserLesson.Teacherid;
				vm_Add.MenkeTeacherName = model_UserLesson.MenkeTeacherName;
				vm_Add.MenkeTeacherCode = model_UserLesson.MenkeTeacherCode;
				vm_Add.MenkeTeacherMobile = model_UserLesson.MenkeTeacherMobile;
				vm_Add.Userid = model_UserLesson.Userid;
				vm_Add.MenkeStudentName = model_UserLesson.MenkeStudentName;
				vm_Add.MenkeStudentCode = model_UserLesson.MenkeStudentCode;
				vm_Add.MenkeStudentMobile = model_UserLesson.MenkeStudentMobile;
				vm_Add.Status = model_UserLesson.Status;
				vm_Add.Remark = model_UserLesson.Remark;

				vm_Add.TeacherCode = model_MenkeLesson?.TeacherCode;
				vm_Add.IsReport = model_Report == null ? 0 : 1;
			}
			else
			{
				vm_Add.Status = 1;
			}
			return View(vm_Add);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserLessonReadAdd(UserLessonAdd vm_Add)
		{
			string message = "";
			var model_Master = _tokenManager.GetAdminInfo();
			var model_UserLesson = _context.Queryable<WebUserLesson>().InSingle(vm_Add.UserLessonid);
			if (model_UserLesson != null)
			{
				message = $"[{DateTime.Now}]{model_Master.Username}" + _localizer["修改用户课节信息"];
				if (!string.IsNullOrEmpty(vm_Add.Message))
				{
					message += ",增加备注:" + vm_Add.Message;
				}
				if (model_UserLesson.Remark.Length < 4000)
				{
					model_UserLesson.Remark = message + "<hr>" + model_UserLesson.Remark;
				}
				else
				{
					model_UserLesson.Remark = message + "<hr>" + model_UserLesson.Remark.Substring(0, 4000);
				}
				_context.Updateable(model_UserLesson).ExecuteCommand();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改用户信息"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				return Content("<script>alert('" + _localizer["不允许在网站后台进行排课操作"] + "!')</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
		#endregion

		//*********************************************************************************************************************************************************

		#region "学生管理" 
		#region "会员列表" 
		[Authorization(Power = "Main")]
		public IActionResult List()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> ListData(string keys, string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long Userid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Userid", 0);
			string Userids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Userids");
			int count = 0;

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					if (Userids.Trim() == "") Userids = "0";
					long num = 0;
					var list_userid = Userids.Split(',').Where(u => long.TryParse(u, out num)).Select(u => long.Parse(u)).ToList();
					if (Userid > 0) list_userid.Add(Userid);
					int n = _context.Updateable<WebUser>().SetColumns(u => u.Status == -1).Where(u => list_userid.Contains(u.Userid)).ExecuteCommand();
					if (n > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "union"://关联用户
					var model_User = _context.Queryable<WebUser>().InSingle(Userid);
					if (model_User != null)
					{
						var result = await _menkeService.UnionMenkeUserId(model_User.MobileCode, model_User.Mobile, 0);
						if (result.StatusCode == 0)
						{
							model_User.MenkeUserId = result.Data;
							_context.Updateable(model_User).UpdateColumns(u => u.MenkeUserId).ExecuteCommand();
							str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["关联成功"] + "！\"}");
						}
						else
						{
							str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + result.Message + "！\"}");
						}
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["关联失败"] + "！\"}");
					}
					break;
				case "enableu"://启用
					count = _context.Updateable<WebUser>().SetColumns(u => u.Status == 1).UpdateColumns(u => u.Userid == Userid).ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["解冻成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["解冻失败,不存在此用户"] + "！\"}");
					}
					break;
				case "enablef"://禁用   
					count = _context.Updateable<WebUser>().SetColumns(u => u.Status == 0).UpdateColumns(u => u.Userid == Userid).ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["冻结成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["冻结失败,不存在此用户"] + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					string son_str = "";
					var exp = new Expressionable<WebUser>();
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.MergeString(u.Email, "").Contains(keys) || SqlFunc.MergeString(u.FirstName, u.LastName, "").Contains(keys) || SqlFunc.MergeString(u.MobileCode, u.Mobile, "").Contains(keys));
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							sort += " desc,";
						}
						else
						{
							sort += ",";
						}
					}
					sort += "userid desc";

					var total = 0;
					var list_User = _context.Queryable<WebUser>().Where(exp.ToExpression()).OrderBy(sort).ToPageList(page, pagesize, ref total);
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_User.Count; i++)
					{
						str.Append("{");
						str.Append("\"Userid\":\"" + list_User[i].Userid + "\",");
						str.Append("\"Name\":\"" + JsonHelper.JsonCharFilter(list_User[i].FirstName + "" + list_User[i].LastName) + "\",");
						str.Append("\"Mobile\":\"" + list_User[i].MobileCode + " " + list_User[i].Mobile + "\",");
						str.Append("\"Email\":\"" + list_User[i].Email + "\",");
						str.Append("\"MenkeUserId\":\"" + list_User[i].MenkeUserId + "\",");
						str.Append("\"HeadImg\":\"" + _commonService.ResourceDomain(string.IsNullOrEmpty(list_User[i].HeadImg) ? "/Upfile/images/none.png" : list_User[i].HeadImg) + "\",");
						str.Append("\"Gender\":\"" + list_User[i].Gender + "\",");
						str.Append("\"Birthdate\":\"" + list_User[i].Birthdate.ToString("d") + "\",");
						str.Append("\"Regtime\":\"" + list_User[i].Regtime + "\",");
						str.Append("\"Dtime\":\"" + list_User[i].Dtime + "\",");
						str.Append("\"Status\":\"" + list_User[i].Status + "\",");
						str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(son_str);
						if (i < (list_User.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}

		#endregion

		#region "添加修改会员"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> ListAdd(long Userid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_ListAdd = new ListAdd();
			if (Lang.ToLower() == "zh-cn")
			{
				vm_ListAdd.CountryItem = _context.Queryable<PubCountry>()
					.OrderBy(u => u.Country).ToList()
					.Select(u => new SelectListItem(u.Country, u.Code))
					.ToList();
			}
			else
			{
				vm_ListAdd.CountryItem = _context.Queryable<PubCountry>()
					.OrderBy(u => u.CountryEn).ToList()
					.Select(u => new SelectListItem(u.CountryEn, u.Code))
					.ToList();
			}

			vm_ListAdd.UtcItem = _context.Queryable<PubTimezone>()
				.Where(u => u.Status == 1)
				.OrderBy(u => u.Title).ToList()
				.Select(u => new SelectListItem(u.Title, u.Timezoneid.ToString()))
				.ToList();

			var model_User = _context.Queryable<WebUser>().InSingle(Userid);
			if (model_User != null)
			{
				vm_ListAdd.Userid = model_User.Userid;
				vm_ListAdd.Username = model_User.Username;
				vm_ListAdd.HeadImg = _commonService.ResourceDomain(string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg);
				vm_ListAdd.Email = model_User.Email;
				vm_ListAdd.Gender = model_User.Gender;
				vm_ListAdd.MobileCode = model_User.MobileCode;
				vm_ListAdd.Mobile = model_User.Mobile;
				vm_ListAdd.FirstName = model_User.FirstName;
				vm_ListAdd.LastName = model_User.LastName;
				vm_ListAdd.Birthdate = model_User.Birthdate;
				vm_ListAdd.Native = model_User.Native;
				vm_ListAdd.Timezoneid = model_User.Timezoneid;
				vm_ListAdd.Education = model_User.Education;
				vm_ListAdd.Status = model_User.Status;
				vm_ListAdd.Remark = model_User.Remark;
				vm_ListAdd.MenkeUserId = model_User.MenkeUserId;


				//如果有课程或课节，就不允许修改手机
				if (_context.Queryable<WebUserCourse>().Any(u => u.Status != -1 && u.Userid == model_User.Userid) ||
					_context.Queryable<WebUserLesson>().Any(u => u.Status != -1 && u.MenkeDeleteTime == 0 && u.Userid == model_User.Userid))
				{
					vm_ListAdd.IsModifyMobile = 0;
				}
				else {
					vm_ListAdd.IsModifyMobile = 1;
				}
			}
			else
			{
				vm_ListAdd.Status = 1;
				vm_ListAdd.Birthdate.AddYears(-2);
			}
			return View(vm_ListAdd);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> ListAdd(ListAdd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/User/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			int chk_img = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_img", 0);
			string message = "";
			var files = _accessor.HttpContext.Request.Form.Files;
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext = "";//扩展名
			IFormFile formFile = null;
			PubTimezone model_Zone = null;

			if (DateTime.Now.AddYears(-2) < vm_ListAdd.Birthdate)
			{
				return Content("<script>alert('" + _localizer["学生最小年龄为2岁"] + "!');history.go(-1)</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}

			var model_Master = _tokenManager.GetAdminInfo();
			var model_User = _context.Queryable<WebUser>().InSingle(vm_ListAdd.Userid);
			if (model_User != null)
			{
				message = $"[{DateTime.Now}]{model_Master.Username}" + _localizer["修改用户信息"];
				if (model_User.Email != vm_ListAdd.Email)
				{
					if (_context.Queryable<WebUser>().Any(u => u.Userid != vm_ListAdd.Userid && u.Email == vm_ListAdd.Email))
					{
						return Content("<script>alert('" + _localizer["此电子邮件已存在，不允许重名"] + "!');history.go(-1)</script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
					message += $",{_localizer["修改电子邮件"]}[" + model_User.Email + "]=>[" + vm_ListAdd.Email + "]";
				}
				if (!string.IsNullOrEmpty(vm_ListAdd.Userpwd) && model_User.Userpwd != MD5Helper.MD5Encrypt32(vm_ListAdd.Userpwd))
				{
					message += $",{_localizer["修改过密码"]}";
				}
				if (model_User.Gender != vm_ListAdd.Gender)
				{
					message += $",{_localizer["修改性别"]}[" + model_User.Gender + "]=>[" + vm_ListAdd.Gender + "]";
				}
				if (model_User.Native != vm_ListAdd.Native)
				{
					message += $",{_localizer["修改母语"]}[" + model_User.Native + "]=>[" + vm_ListAdd.Native + "]";
				}
				if (model_User.Timezoneid != vm_ListAdd.Timezoneid)
				{
					message += $",{_localizer["修改时区"]}[" + model_User.Timezoneid + "]=>[" + vm_ListAdd.Timezoneid + "]";
				}
				if (files.Any(u => u.Name == "input_img" && u.Length > 0))
				{
					message += $",{_localizer["修改过头像"]}";
				}

                if (string.IsNullOrEmpty(vm_ListAdd.MobileCode)) vm_ListAdd.MobileCode = model_User.MobileCode;
                if (string.IsNullOrEmpty(vm_ListAdd.Mobile)) vm_ListAdd.Mobile = model_User.Mobile;
                if (model_User.MobileCode != vm_ListAdd.MobileCode || model_User.Mobile != vm_ListAdd.Mobile)
				{
					if (_context.Queryable<WebUser>().Any(u => u.Userid != vm_ListAdd.Userid && u.Status!=-1 && u.MobileCode == vm_ListAdd.MobileCode && u.MobileCode == vm_ListAdd.Mobile))
					{
						return Content("<script>alert('" + _localizer["手机已存在，不允许重名"] + "!');history.go(-1)</script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
					message += $",{_localizer["修改手机"]}[" + model_User.MobileCode + model_User.Mobile + "]=>[" + vm_ListAdd.MobileCode + vm_ListAdd.Mobile + "]";
					model_User.MenkeUserId = 0;//清0后，后边会再次创建绑定
				}
				if (model_User.Status != vm_ListAdd.Status)
				{
					if (vm_ListAdd.Status == 1 && (string.IsNullOrEmpty(vm_ListAdd.MobileCode) || string.IsNullOrEmpty(vm_ListAdd.Mobile)))
					{
						return Content("<script>alert('" + _localizer["未完善手机号码,不能修改状态为已启用"] + "!');history.go(-1)</script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
					message += $",{_localizer["修改状态"]}[" + model_User.Status + "]=>[" + vm_ListAdd.Status + "]";
				}


				if (model_User.MenkeUserId == 0)
				{
					var result_menke = await _menkeService.CreateStudents(new MenkeStudentDto
					{
						name = vm_ListAdd.FirstName + " " + vm_ListAdd.LastName,
						nickname = vm_ListAdd.FirstName + " " + vm_ListAdd.LastName,
						sex = vm_ListAdd.Gender,
						birthday = vm_ListAdd.Birthdate.ToString("d"),
						locale = (vm_ListAdd.CountryCode + "").ToUpper(),
						code = vm_ListAdd.MobileCode,
						mobile = vm_ListAdd.Mobile,
						p_name = ""
					});
					if (result_menke.StatusCode == 0) model_User.MenkeUserId = result_menke.Data;
				}
				else
				{
					var result_menke = await _menkeService.ModifyStudents(vm_ListAdd.MobileCode, vm_ListAdd.Mobile, new MenkeStudentDto
					{
						name = vm_ListAdd.FirstName + " " + vm_ListAdd.LastName,
						nickname = vm_ListAdd.FirstName + " " + vm_ListAdd.LastName,
						sex = vm_ListAdd.Gender,
						birthday = vm_ListAdd.Birthdate.ToString("d"),
						p_name = model_User.GuardianName
					});
				}

				#region "上传"
				if (chk_img == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_User.HeadImg)) FileHelper.FileDel(ImgRoot + model_User.HeadImg.Replace(prefix, ""));
					}
					catch { }
					model_User.HeadImg = "";
				}
				if (files.Count > 0)
				{
					//头像
					formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_User.HeadImg)) FileHelper.FileDel(ImgRoot + model_User.HeadImg.Replace(prefix, ""));
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_User.HeadImg = prefix + fileurl + filename;
						}
					}
				}
				#endregion

				model_User.Username = vm_ListAdd.Username;
				if (!string.IsNullOrEmpty(vm_ListAdd.Userpwd)) model_User.Userpwd = MD5Helper.MD5Encrypt32(vm_ListAdd.Userpwd);
				model_User.Email = vm_ListAdd.Email;
				model_User.Gender = vm_ListAdd.Gender;
				model_User.MobileCode = vm_ListAdd.MobileCode;
				model_User.Mobile = vm_ListAdd.Mobile.Replace(" ", "");
				model_User.FirstName = vm_ListAdd.FirstName;
				model_User.LastName = vm_ListAdd.LastName;
				model_User.Birthdate = vm_ListAdd.Birthdate;
				model_User.Native = vm_ListAdd.Native;
				model_User.Education = vm_ListAdd.Education;
				model_User.Status = vm_ListAdd.Status;
				if (model_User.Timezoneid != vm_ListAdd.Timezoneid)
				{
					model_Zone = _context.Queryable<PubTimezone>().InSingle(vm_ListAdd.Timezoneid);
					if (model_Zone != null)
					{
						model_User.Timezoneid = vm_ListAdd.Timezoneid;
						model_User.Utc = model_Zone.Title;
						model_User.UtcSec = -(model_Zone?.UtcSec ?? 0) / 60;

                    }
				}
				if (string.IsNullOrEmpty(model_User.Remark) || model_User.Remark.Length < 4000)
				{
					model_User.Remark = message + "<hr>" + model_User.Remark;
				}
				else
				{
					model_User.Remark = message + "<hr>" + model_User.Remark.Substring(0, 4000);
				}
				_context.Updateable(model_User).ExecuteCommand();
				return Content("<script>parent.$('#win').window('close');alert('保存信息成功');parent.$('#table_view').datagrid('reload');</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				if (_context.Queryable<WebUser>().Any(u => u.Email == vm_ListAdd.Email))
				{
					return Content("<script>alert('" + _localizer["电子邮件已存在，不允许重名"] + "!');history.go(-1)</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				if (_context.Queryable<WebUser>().Any(u => u.MobileCode == vm_ListAdd.MobileCode && u.MobileCode == vm_ListAdd.Mobile))
				{
					return Content("<script>alert('" + _localizer["手机已存在，不允许重名"] + "!');history.go(-1)</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				if (string.IsNullOrEmpty(vm_ListAdd.Userpwd) || vm_ListAdd.Userpwd.Trim().Length < 6 || vm_ListAdd.Userpwd.Trim().Length > 20)
				{
					return Content("<script>alert('" + _localizer["密码必需大于6位并小于20位"] + "!');history.go(-1)</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				message = $"[{DateTime.Now}]{model_Master.Username}" + _localizer["添加用户信息"];

				model_User = new WebUser();
				model_User.Username = model_User.Username;
				model_User.Userpwd = MD5Helper.MD5Encrypt32(vm_ListAdd.Userpwd);
				model_User.Email = vm_ListAdd.Email;
				model_User.Gender = vm_ListAdd.Gender;
				model_User.MobileCode = vm_ListAdd.MobileCode;
				model_User.Mobile = vm_ListAdd.Mobile.Replace(" ", "");
				model_User.FirstName = vm_ListAdd.FirstName;
				model_User.LastName = vm_ListAdd.LastName;
				model_User.Birthdate = vm_ListAdd.Birthdate;
				model_User.Native = vm_ListAdd.Native;
				model_User.Education = vm_ListAdd.Education;
				model_User.Status = vm_ListAdd.Status;
				model_User.Remark = message;
				model_Zone = _context.Queryable<PubTimezone>().InSingle(vm_ListAdd.Timezoneid);
				if (model_Zone != null)
				{
					model_User.Timezoneid = vm_ListAdd.Timezoneid;
					model_User.Utc = model_Zone.Utc;
					model_User.UtcSec = model_Zone.UtcSec;
				}

				#region "上传"
				if (files.Count > 0)
				{
					//头像
					formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_User.HeadImg)) FileHelper.FileDel(ImgRoot + model_User.HeadImg.Replace(prefix, ""));
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_User.HeadImg = prefix + fileurl + filename;
						}
					}
				}
				#endregion

				var result_menke = await _menkeService.CreateStudents(new MenkeStudentDto
				{
					name = model_User.FirstName + " " + model_User.LastName,
					nickname = model_User.FirstName + " " + model_User.LastName,
					sex = model_User.Gender,
					birthday = vm_ListAdd.Birthdate.ToString("d"),
					locale = (vm_ListAdd.CountryCode + "").ToUpper(),
					code = vm_ListAdd.MobileCode,
					mobile = vm_ListAdd.Mobile,
					p_name = ""
				});
				if (result_menke.StatusCode == 0)
				{
					model_User.MenkeUserId = result_menke.Data;
					vm_ListAdd.Userid = _context.Insertable(model_User).ExecuteReturnBigIdentity();
					return Content("<script>parent.$('#win').window('close');alert('保存信息成功');parent.$('#table_view').datagrid('reload');</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				else
				{
					return Content("<script>alert('同步拓课云学生信息失败');parent.$('#table_view').datagrid('reload');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}

			}
		}
		#endregion
		#endregion

		#region "学生已购课程" 
		#region "学生课程列表" 
		[Authorization(Power = "Main")]
		public IActionResult UserCourse()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserCourseData(string keys = "",string menke_courseid = "",string mobile = "",string type="", string status = "", string begintime = "", string endtime = "")
		{
			var model_Master = _tokenManager.GetAdminInfo();
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long UserCourseid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "UserCourseid", 0);
			string UserCourseids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "UserCourseids");
			WebUser model_User = null;
			List<WebUserCourse> list_UserCourse;
			List<long> list_UserCourseid = new List<long>();
			string msg = "";

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					if (UserCourseids.Trim() == "") UserCourseids = "0";
					list_UserCourseid = UserCourseids.Split(',').Select(u => long.Parse(u)).ToList();
					if (UserCourseid > 0) list_UserCourseid.Add(UserCourseid);
					
					foreach (var user_courseid in list_UserCourseid) {
						var result = await _courseService.DelUserCourse(user_courseid);
						if (result.StatusCode > 0) {
							msg += (",ID="+user_courseid + "|" +result.Message);
						}
					}

					if (string.IsNullOrEmpty(msg))
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["有删除失败信息:"] + msg + "\"}");
					}
					break;
				case "sync"://同步课节
					int menke_course_id = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "MenkeCourseId", 0);
					//无法同步课程，有可能会同步1年前的课程，数据量太大
					//var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == menke_course_id);
					//if(model_MenkeCourse!=null) await _menkeService.CourseSync(model_MenkeCourse.MenkeUpdateTime.ToString());
                    var result_lesson = await _menkeService.LessonSync("", 0, menke_course_id);
                    if (result_lesson.StatusCode == 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["同步课节成功"] + ",共计"+ result_lesson.Data.Count() + "节！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["同步课节失败:"] +msg+ "！\"}");
					}
					break;
				case "enableu"://解锁
					if (UserCourseids.Trim() == "") UserCourseids = "0";
					list_UserCourseid = UserCourseids.Split(',').Select(u => long.Parse(u)).ToList();
					if (UserCourseid > 0) list_UserCourseid.Add(UserCourseid);
					
					foreach (var user_courseid in list_UserCourseid)
					{
						var result = await _courseService.UnLockUserCourse(user_courseid);
						if (result.StatusCode > 0)
						{
							msg += (",ID=" + user_courseid + "|" + result.Message);
						}
					}
					if (string.IsNullOrEmpty(msg))
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["解锁成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["有解锁失败信息:"] +msg+ "！\"}");
					}
					break;
				case "enablef"://锁定,注意清除所有已排课未上课的
					if (UserCourseids.Trim() == "") UserCourseids = "0";
					list_UserCourseid = UserCourseids.Split(',').Select(u => long.Parse(u)).ToList();
					if (UserCourseid > 0) list_UserCourseid.Add(UserCourseid);
					list_UserCourse = _context.Queryable<WebUserCourse>().Where(u => list_UserCourseid.Contains(u.UserCourseid)).ToList();

					foreach (var user_courseid in list_UserCourseid)
					{
						var result = await _courseService.LockUserCourse(user_courseid);
						if (result.StatusCode > 0)
						{
							msg += (",ID=" + user_courseid + "|" + result.Message);
						}
					}
					if (string.IsNullOrEmpty(msg))
					{						
                        str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["锁定成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["有锁定失败信息:"] + msg + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					string son_str = "";

					int btime = !string.IsNullOrEmpty(begintime) ? DateHelper.ConvertDateTimeInt(DateTime.Parse(begintime)) : 0;
					int etime = !string.IsNullOrEmpty(endtime) ? DateHelper.ConvertDateTimeInt(DateTime.Parse(endtime)) : 0;
					var exp = new Expressionable<WebUserCourse>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u =>
						SqlFunc.Subqueryable<WebUser>().Where(s => s.Userid == u.Userid && (SqlFunc.MergeString(s.Mobile, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.Email, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.FirstName, s.LastName, "").Contains(keys.Trim()))).Any());
					exp.AndIF(!string.IsNullOrEmpty(mobile), u =>SqlFunc.Subqueryable<WebUser>().Where(s=>s.Userid==u.Userid && s.Mobile.Contains(mobile)).Any());
					exp.AndIF(!string.IsNullOrEmpty(menke_courseid), u => u.MenkeCourseId.ToString() == menke_courseid.Trim());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(type), u => u.Type.ToString() == type.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					if (sort != "")
					{
						sort = StringHelper.ChgToUnderLine(sort);
						if (order == "desc")
						{
							sort += " desc,";
						}
						else
						{
							sort += ",";
						}
					}
					sort += "user_courseid desc";

					var total = 0;
					list_UserCourse = _context.Queryable<WebUserCourse>().Where(exp.ToExpression()).OrderBy(sort).ToPageList(page, pagesize, ref total);
					//var list_Course = _context.Queryable<WebCourseLang>().Where(u => u.Lang == Lang && list_UserCourse.Select(s => s.Courseid).Contains(u.Courseid)).ToList();
					var list_User = _context.Queryable<WebUser>().Where(u => list_UserCourse.Select(s => s.Userid).Contains(u.Userid)).ToList();
					var list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_UserCourse.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
					var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == Lang && list_UserCourse.Select(s => s.SkuTypeid).Contains(u.SkuTypeid)).ToList();
					var list_Menketype = _context.Queryable<WebSkuType>().Where(u => u.Status == 1).Select(u => new { u.SkuTypeid, u.MenkeType }).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_UserCourse.Count; i++)
					{
						string userCenter = "";
						model_User = list_User.FirstOrDefault(u => u.Userid == list_UserCourse[i].Userid);
						if (model_User != null) {
							if (model_User.Updatetime.AddSeconds(model_User.Expiresin) >= DateTime.Now)
							{
                                userCenter = Appsettings.app("Web:Host")+ $"/Api/V1/UserLogin/Redirect?token={model_User.Token}";
							}
							else {
                                userCenter = "";
							}
						}

						var model_MenkeCourse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == list_UserCourse[i].MenkeCourseId);
						//var model_Course = list_Course.FirstOrDefault(u => u.Courseid == list_UserCourse[i].Courseid);
						var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == list_UserCourse[i].SkuTypeid);
						str.Append("{");
						str.Append("\"UserCourseid\":\"" + list_UserCourse[i].UserCourseid + "\",");
						str.Append("\"Type\":" + list_UserCourse[i].Type + ",");
						str.Append("\"OrderSn\":\"" + list_UserCourse[i].OrderSn + "\",");
						str.Append("\"Name\":\"" + model_User?.FirstName +" "+ model_User?.LastName + "\",");
						str.Append("\"Mobile\":\"" + model_User?.MobileCode + "-" + model_User?.Mobile + "\",");
						str.Append("\"MenkeCourseId\":\"" + list_UserCourse[i].MenkeCourseId + "\",");
						str.Append("\"MenkeName\":\"" + (model_MenkeCourse?.MenkeName ?? "等待同步中") + "\",");
						str.Append("\"Title\":\"" + list_UserCourse[i].Title + "\",");
						str.Append("\"SkuType\":\"" + model_SkuType?.Title + "\",");
						str.Append("\"UserCenter\":\"" + userCenter + "\",");
						str.Append("\"ClassHour\":" + list_UserCourse[i].ClassHour + ",");
						str.Append("\"Classes\":" + list_UserCourse[i].Classes + ",");
						str.Append("\"Dtime\":\"" + list_UserCourse[i].Dtime + "\",");
						str.Append("\"Status\":" + list_UserCourse[i].Status + ",");
                        str.Append("\"Istrial\":" + list_UserCourse[i].Istrial + ",");
                        str.Append("\"MenkeType\":" + list_Menketype.FirstOrDefault(u=>u.SkuTypeid==list_UserCourse[i].SkuTypeid)?.MenkeType + ",");
						str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(son_str);
						if (i < (list_UserCourse.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}

        #endregion

        #region "JS获取用户资料和上课方式"  
        [HttpGet]
        [Authorization(Power = "Main")]
        public async Task<JsonResult> GetUser(string mobile_code, string mobile)
        {
            var result = new Dictionary<string, object>();
            var model_Sku = _context.Queryable<WebUser>()
                .Where(u => u.MobileCode == mobile_code && u.Mobile == mobile)
                .Select(u => new { u.FirstName, u.LastName, u.Email, u.Userid })
                .First();
            if (model_Sku != null)
            {
                result.Add("name", model_Sku.FirstName + " " + model_Sku.LastName);
                result.Add("email", model_Sku.Email);
                result.Add("userid", model_Sku.Userid);
                result.Add("code", 0);
            }
            else
            {
                result.Add("code", -1);
                result.Add("msg", _localizer["此手机用户不存在"] + "");
            }
            return new JsonResult(result);
        }

        [HttpGet]
        [Authorization(Power = "Main")]
        public async Task<string> GetSkuType(long Courseid)
        {
            string str = "<option value=\"0\">" + _localizer["请选择上课方式"] + "</option>";
            if (Courseid == 0)
			{
                var list_SkuType = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid && r.Lang == Lang)
                                    .Where((l, r) => l.Status == 1)
                                    .Select((l, r) => new { l.SkuTypeid,SkuType = r.Title })
                                    .ToList();
                foreach (var item in list_SkuType)
                {
                    str += "<option value=\"" + item.SkuTypeid + "\">" + item.SkuType + "</option>";
                }
            }
			else
			{
				var list_SkuType = _context.Queryable<WebCourseSku>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid && r.Lang == Lang)
					.Where((l, r) => l.Courseid == Courseid)
					.GroupBy((l, r) => new { l.SkuTypeid, l.Courseid, r.Title })
					.Select((l, r) => new { l.SkuTypeid, l.Courseid, SkuType = r.Title })
					.ToList();
                foreach (var item in list_SkuType)
                {
                    str += "<option value=\"" + item.SkuTypeid + "\">" + item.SkuType + "</option>";
                }
            }
            return str;
        }
        #endregion

        #region "添加修改学生课程(改)"  
        [Authorization(Power = "Main")]
		public async Task<IActionResult> UserCourseAdd(long UserCourseid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_Add = new UserCourseAdd();
			vm_Add.CourseItem = _context.Queryable<WebCourse>().LeftJoin<WebCourseLang>((l,r)=>l.Courseid==r.Courseid && r.Lang==Lang)
				.Where((l,r) => l.Status == 1)
				.Select((l,r) => new { r.Title,l.Courseid }).ToList()
				.Select(u => new SelectListItem(u.Title, u.Courseid.ToString())).ToList();
            vm_Add.CourseItem.Insert(0, new SelectListItem(_localizer["免费试听课"], "0"));
            //vm_Add.CourseItem.Insert(0, new SelectListItem(_localizer["请选择课程"], "0"));

			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(UserCourseid);
			if (model_UserCourse != null)
			{
				if (model_UserCourse.Courseid > 0)
				{
					vm_Add.SkuTypeItem = _context.Queryable<WebCourseSku>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid && r.Lang == Lang)
					.Where((l, r) => l.Courseid == model_UserCourse.Courseid)
					.Select((l, r) => new { l.SkuTypeid, l.Courseid, SkuType = r.Title })
					.ToList()
					.Select(u => new SelectListItem(u.SkuType, u.SkuTypeid.ToString()))
					.ToList();
				}
				vm_Add.SkuTypeItem.Insert(0, new SelectListItem(_localizer["请选择上课方式"], "0"));
				var model_User = _context.Queryable<WebUser>().InSingle(model_UserCourse.Userid);
				var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId);
				var model_SkuType = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l,r)=>l.SkuTypeid==r.SkuTypeid)
					.Where((l,r) => r.Lang == Lang && r.SkuTypeid == model_UserCourse.SkuTypeid)
					.Select((l,r)=>new {r.Title,l.SkuTypeid,l.MenkeType })
					.First();

				vm_Add.UserCourseid = model_UserCourse.UserCourseid;
				vm_Add.OrderSn = model_UserCourse.OrderSn;
				if (model_User != null)
				{
					vm_Add.Name = model_User.FirstName + " " + model_User.LastName;
					vm_Add.Email = model_User.Email;
					vm_Add.Mobile = (model_User.Mobile + "").Replace(" ", "").Replace("-", "");
					vm_Add.MobileCode = model_User.MobileCode;
				}
				if (model_MenkeCourse != null)
				{
					vm_Add.MenkeName = model_MenkeCourse.MenkeName;
					vm_Add.MenkeCourseId = model_MenkeCourse.MenkeCourseId;
				}
				vm_Add.Courseid = model_UserCourse.Courseid;
				vm_Add.Title = model_UserCourse.Title;
				vm_Add.SkuType = model_SkuType?.Title;
				vm_Add.ClassHour = model_UserCourse.ClassHour;
				vm_Add.Classes = model_UserCourse.Classes;
				vm_Add.Status = model_UserCourse.Status;
				vm_Add.Remarks = model_UserCourse.Remarks;
				vm_Add.SkuTypeid = model_UserCourse.SkuTypeid;

				vm_Add.MenkeType = model_SkuType?.MenkeType ?? 0;
			}
			else
			{
				vm_Add.Status = 1;
			}
			return View(vm_Add);
		}


		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserCourseAdd(UserCourseAdd vm_Add)
		{
			string message = "";
			var model_Master = _tokenManager.GetAdminInfo();
			if ( vm_Add.ClassHour == 0)
			{
				return Content("<script>alert('" + _localizer["请填写课时数"] + "!');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			
			var students = new List<string>();
			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(vm_Add.UserCourseid);
			if (model_UserCourse != null)
			{
				message = $"[{DateTime.Now}]{model_Master.Username}" + _localizer["修改学生课程信息"];
				if (model_UserCourse.ClassHour != vm_Add.ClassHour)
				{
					if (vm_Add.ClassHour < 0 || vm_Add.ClassHour < model_UserCourse.Classes)
					{
						return Content("<script>alert('" + _localizer["总课时数不能小于已上课时数"] + "!');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
					message += $",{_localizer["修改课时总数"]}[" + model_UserCourse.ClassHour + "]=>[" + vm_Add.ClassHour + "]";
				}
				
				if (model_UserCourse.Status != vm_Add.Status)
				{
					message += $",{_localizer["修改显示状态"]}[" + model_UserCourse.Status + "]=>[" + vm_Add.Status + "]";
				}
				if (!string.IsNullOrEmpty(vm_Add.Message1))
				{
					message += ",增加备注:" + vm_Add.Message1;
				}

				model_UserCourse.Title = vm_Add.Title;
				model_UserCourse.ClassHour = vm_Add.ClassHour;
				if (model_UserCourse.MenkeCourseId == 0 && vm_Add.Status==1) {
					//调创建课程
					var result_menke_course = await _courseService.UserCourseArranging(vm_Add.UserCourseid);
					if (result_menke_course.StatusCode != 0)
					{
						_logger.LogError("后台拓课云安排课程失败，原因:" + result_menke_course.Message);
						return Content("<script>alert('拓课云安排课程失败，请联系技术人员处理!');parent.$('#table_view').datagrid('reload');</script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
				}

				model_UserCourse.Status = vm_Add.Status;
				if ((model_UserCourse.Remarks + "").Length < 4000)
				{
					model_UserCourse.Remarks = message + "<hr>" + model_UserCourse.Remarks;
				}
				else
				{
					model_UserCourse.Remarks = message + "<hr>" + model_UserCourse.Remarks.Substring(0, 4000);
				}

				model_UserCourse.Lastchanged = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
				_context.Updateable(model_UserCourse).ExecuteCommand();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改学生课程"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				var model_User = _context.Queryable<WebUser>().First(u => u.MobileCode == vm_Add.MobileCode && u.Mobile == vm_Add.Mobile);
				if (model_User == null) return Content("<script>alert('" + _localizer["此手机号对应学生不存在"] + "!');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
				if (vm_Add.SkuTypeid==0) {
					return Content("<script>alert('" + _localizer["请选上课方式"] + "!');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				var model_SkuType = _context.Queryable<WebSkuType>().LeftJoin<WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid)
					.Where((l, r) => l.SkuTypeid == vm_Add.SkuTypeid && r.Lang == Lang)
					.Select((l, r) => new { l.SkuTypeid, r.Title, l.MenkeType, l.MenkeTemplateId })
					.First();
				if (model_SkuType == null)
				{
					return Content("<script>alert('上课类型(" + Lang + ")不存在!');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				model_UserCourse = new WebUserCourse();
				model_UserCourse.Type = 0;//众语课
				model_UserCourse.Title = vm_Add.Title;
				model_UserCourse.OrderSn = "";
				model_UserCourse.Orderid = 0;
				model_UserCourse.Userid = model_User.Userid;
				model_UserCourse.MenkeCourseId = vm_Add.MenkeCourseId;
				model_UserCourse.ClassHour = vm_Add.ClassHour;
				model_UserCourse.Courseid = vm_Add.Courseid;
				model_UserCourse.SkuTypeid = vm_Add.SkuTypeid;
				if (vm_Add.Courseid == 0)
				{
					model_UserCourse.Istrial = 1;
					model_UserCourse.Title = "免费试听课";

                }
				else
				{
					var model_Course = _context.Queryable<WebCourseLang>().LeftJoin<WebCourse>((l, r) => l.Courseid == r.Courseid)
						.Where((l, r) => l.Courseid == vm_Add.Courseid && l.Lang == Lang)
							.Select((l, r) => new { l.Title, r.Img, l.Message })
							.First();
					if (model_Course != null)
					{
						model_UserCourse.Title = model_Course.Title;
						model_UserCourse.Img = model_Course.Img;
						model_UserCourse.Message = model_Course.Message;
					}
					else
					{
						return Content("<script>alert('" + _localizer["课程不存在"] + "!');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
				}
				model_UserCourse.Status = vm_Add.Status;
				model_UserCourse.Lastchanged = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
				var user_courseid = _context.Insertable(model_UserCourse).ExecuteReturnBigIdentity();

				if (vm_Add.IsAuto == 1 && vm_Add.Courseid>0)
				{
                    //自动安排课程。vm_Add.Courseid=0试听课一定是全新创建
                    var result_menke_course = await _courseService.UserCourseArranging(user_courseid);
					if (result_menke_course.StatusCode != 0)
					{
						_context.Updateable<WebUserCourse>()
							.SetColumns(u => u.Status==-1)
							.SetColumns(u=>u.Remarks == SqlFunc.MergeString("拓课云安排课程失败<hr>",u.Remarks))
							.Where(u => u.UserCourseid == user_courseid)
							.ExecuteCommand();

						_logger.LogError("后台拓课云安排课程失败，原因:" + result_menke_course.Message);
						return Content("<script>alert('拓课云安排课程失败，请联系技术人员处理!');parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');</script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
					else {
						//自动更新写入web_user_course.menke_course_id
						//model_UserCourse.MenkeCourseId = result_menke_course.Data;
					}
				}
				else {
					students.Add(model_User.MobileCode + "-" + model_User.Mobile);
					string course_name = $"{model_User.FirstName} {model_User.LastName}-{model_UserCourse.Title}-{model_SkuType.Title}";
					var result_menke_course = await _menkeService.CreateCourse(course_name, students, model_SkuType.MenkeTemplateId);
					if (result_menke_course.StatusCode != 0)
					{
						_context.Updateable<WebUserCourse>()
							.SetColumns(u => u.Status == -1)
							.SetColumns(u => u.Remarks == SqlFunc.MergeString("拓课云安排课程失败<hr>", u.Remarks))
							.Where(u => u.UserCourseid == user_courseid)
							.ExecuteCommand();
						_logger.LogError("后台拓课云创建课程失败，原因:" + result_menke_course.Message);
						return Content("<script>alert('拓课云创建课程失败，请联系技术人员处理!');parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');</script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
					else {
						//手动更新写入web_user_course.menke_course_id
						_context.Updateable<WebUserCourse>()
							.SetColumns(u => u.MenkeCourseId == result_menke_course.Data)
							.Where(u => u.UserCourseid == user_courseid)
							.ExecuteCommand();
					}
				}
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["创建学生课程"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion

		#region "调整拓课云课程"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserCourseAdjust(long UserCourseid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_Add = new UserCourseAdject();
			var model_UserCourse = _context.Queryable<WebUserCourse>().InSingle(UserCourseid);

			if (model_UserCourse != null)
			{
				int private_class_max = _pubConfigService.GetConfigInt("private_class_max", 5);
				var list_MenkeCourseId = _context.Queryable<WebUserLesson>().Where(u => u.MenkeCourseId > 0 && u.Courseid== model_UserCourse.Courseid && u.SkuTypeid== model_UserCourse.SkuTypeid && u.Status == 1)
				.GroupBy(u => u.MenkeCourseId)
				.Having(u => SqlFunc.AggregateCount(u.MenkeCourseId) < private_class_max)
				.Select(u => u.MenkeCourseId)
				.ToList();
				var list_NewMenkeCourseId = _context.Queryable<WebUserCourse>()
								.Where(u => u.MenkeCourseId > 0 && u.UserCourseid != model_UserCourse.UserCourseid && u.Courseid == model_UserCourse.Courseid && u.SkuTypeid == model_UserCourse.SkuTypeid && u.Status == 1 &&
								SqlFunc.Subqueryable<WebUserLesson>().Where(s => s.UserCourseid == u.UserCourseid && s.Status == 1 && s.MenkeDeleteTime == 0).NotAny())
								.GroupBy(u => u.MenkeCourseId)
								.Having(u => SqlFunc.AggregateDistinctCount(u.Userid) < private_class_max)
								.Select(u => u.MenkeCourseId).ToList();
				list_MenkeCourseId = (list_MenkeCourseId.Union(list_NewMenkeCourseId)).Distinct().ToList();
				vm_Add.MenkeCourseItem = _context.Queryable<MenkeCourse>()
					.LeftJoin<WebUserCourse>((l,r)=>l.MenkeCourseId==r.MenkeCourseId)
					.Where(l=> list_MenkeCourseId.Contains(l.MenkeCourseId) && l.MenkeCourseId!=model_UserCourse.MenkeCourseId)
					.GroupBy((l,r)=>new { l.MenkeCourseId,l.MenkeName})
					.Select(l => new { l.MenkeName, l.MenkeCourseId, Count = SqlFunc.AggregateCount(l.MenkeCourseId) }).ToList()
					.Select(u => new SelectListItem(u.MenkeName+"(门课ID:"+ u.MenkeCourseId + ")", u.MenkeCourseId.ToString())).ToList();
				vm_Add.MenkeCourseItem.Insert(0, new SelectListItem(_localizer["创建全新小班课课程"], "0"));

				var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u=>u.MenkeCourseId== model_UserCourse.MenkeCourseId);
				vm_Add.UserCourseid = model_UserCourse.UserCourseid;
				vm_Add.MenkeName = model_MenkeCourse?.MenkeName;
				vm_Add.Remarks = model_UserCourse.Remarks;

				var list_Userid = _context.Queryable<WebUserCourse>().Where(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId && u.Status == 1).Select(u => u.Userid).ToList();
				var list_LessonUserid = _context.Queryable<WebUserLesson>().Where(u => u.MenkeCourseId == model_UserCourse.MenkeCourseId && u.Status == 1).Select(u => u.Userid).ToList();
				list_Userid = list_Userid.Union(list_LessonUserid).Distinct().ToList();
				vm_Add.Students = string.Join(",", _context.Queryable<WebUser>().Where(u => list_Userid.Contains(u.Userid))
					.Select(u => SqlFunc.MergeString(u.MobileCode, "-", u.Mobile)).ToList());
			}
			else
			{
				return Content("<script>alert('" + _localizer["课程不存在"] + "!');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			return View(vm_Add);
		}

		[HttpGet]
		[Authorization(Power = "Main")]
		public async Task<JsonResult> GetMenkCourse(long MenkeCourseid)
		{
			var result = new Dictionary<string, object>();
			var list_UserLesson = _context.Queryable<WebUserLesson>()
				.Where(u => u.MenkeCourseId== MenkeCourseid && u.MenkeDeleteTime==0 && u.Status==1)
				.Select(u => new { u.MenkeTeacherName,u.MenkeStudentName })
				.ToList();
			var list_teacher = list_UserLesson.Select(u => u.MenkeTeacherName).Distinct().ToList();
			var list_student = list_UserLesson.Select(u => u.MenkeStudentName).Distinct().ToList();
			result.Add("code", 1);
			result.Add("teacher", string.Join(",", list_teacher));
			result.Add("students", string.Join(",", list_student));
			return new JsonResult(result);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserCourseAdjust(UserCourseAdject vm_Add)
		{

			var result = await _courseService.AdjustUserCourse(vm_Add.UserCourseid, vm_Add.MenkeCourseId);
			return Content("<script>alert('" + _localizer["调整课程成功"] + "!');parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');</script>", "text/html", Encoding.GetEncoding("utf-8"));
		}
		#endregion
		#endregion

		#region "已排课节管理" 
		#region "已排课节列表" 
		[Authorization(Power = "Main")]
		public IActionResult UserLesson()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserLessonData(string keys = "", string status = "", string menke_state = "", string is_user_courseid = "",string istrial = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long UserLessonid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "UserLessonid", 0);
			string UserLessonids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "UserLessonids");
			WebUser model_User = null;
			List<WebUserLesson> list_UserLesson;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					if (UserLessonids.Trim() == "") UserLessonids = "0";
					var list_UserLessonid = UserLessonids.Split(',').Select(u => long.Parse(u)).ToList();
					if (UserLessonid > 0) list_UserLessonid.Add(UserLessonid);
					var url = Appsettings.app("APIS:menke_delete_lesson");
					list_UserLesson = _context.Queryable<WebUserLesson>().Where(u => u.Status == 1 && list_UserLessonid.Contains(u.UserLessonid)).ToList();
					foreach (var model in list_UserLesson) {
						model.Status = -1;
						await _menkeService.DeleteLesson(model.MenkeLessonId);
					}
					int n = _context.Updateable(list_UserLesson).ExecuteCommand();
					if (n > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "unionid"://手工关联单条记录的ID
					await _menkeService.UnionLessonUserid(_context.Queryable<WebUserLesson>().Where(u => u.UserLessonid == UserLessonid).Select(u => u.MenkeCourseId).ToList());
					str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["同步关联完成"] + "！\"}");
					break;
				case "union_student"://关联学生
					long userid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Userid", 0);
					model_User = _context.Queryable<WebUser>().First(u=>u.Userid == userid && !string.IsNullOrEmpty(u.MobileCode) && !string.IsNullOrEmpty(u.Mobile));
					if (model_User != null)
					{
						var result_student = await _menkeService.UnionMenkeUserId(model_User.MobileCode, model_User.Mobile, 0);
						if (result_student.StatusCode == 0)
						{
							model_User.MenkeUserId = result_student.Data;
							_context.Updateable(model_User).UpdateColumns(u => u.MenkeUserId).ExecuteCommand();
							str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["同步关联完成"] + "！\"}");
						}
						else
						{
							str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["拓课云查询学生失败"] + "！\"}");
						}
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["用户手机号码资料不完整"] + "！\"}");
					}
					break;
				case "union_teacher"://关联老师
					long teacherid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Teacherid", 0);
					var model_Teacher = _context.Queryable<WebTeacher>().First(u=>u.Teacherid == teacherid && !string.IsNullOrEmpty(u.MobileCode) && !string.IsNullOrEmpty(u.Mobile));
					if (model_Teacher != null)
					{
						var result_student = await _menkeService.UnionMenkeUserId(model_Teacher.MobileCode, model_Teacher.Mobile, 0);
						if (result_student.StatusCode == 0)
						{
							model_Teacher.MenkeUserId = result_student.Data;
							_context.Updateable(model_Teacher).UpdateColumns(u => u.MenkeUserId).ExecuteCommand();
							str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["同步关联完成"] + "！\"}");
						}
						else
						{
							str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["拓课云查询老师失败"] + "！\"}");
						}
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["老师手机号码资料不完整"] + "！\"}");
					}
					break;
				case "sync"://手工实时拉取最新信息
					var starttime = DateTime.UtcNow.AddDays(-1);
					var result = await _menkeService.LessonSync(DateHelper.ConvertDateTimeInt(starttime).ToString());
					if (result.StatusCode==0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + string.Format(_localizer["同步时间[{0}],拉取成功"], starttime) + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["同步拉取失败"] + "！\"}");
					}
					break;
				default:
					var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					string son_str = "";

					int btime = !string.IsNullOrEmpty(begintime)?DateHelper.ConvertDateTimeInt(DateTime.Parse(begintime)):0;
					int etime = !string.IsNullOrEmpty(endtime) ? DateHelper.ConvertDateTimeInt(DateTime.Parse(endtime)):0;
					var exp = new Expressionable<WebUserLesson>();
					exp.And(u => u.Status != -1 && u.OnlineCourseid == 0);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => 
						SqlFunc.Subqueryable<WebUser>().Where(s => s.Userid == u.Userid && (SqlFunc.MergeString(s.Mobile, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.Email, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.FirstName, s.LastName, "").Contains(keys.Trim())) ).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(istrial), u => u.Istrial.ToString() == istrial.Trim());
                    if (!string.IsNullOrEmpty(is_user_courseid))
                    {
                        switch (is_user_courseid)
                        {
                            case "0":
                                exp.And(u => u.UserCourseid == 0);
                                break;
                            case "1":
                                exp.And(u => u.UserCourseid > 0);
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(menke_state))
                    {
                        switch (menke_state)
                        {
                            case "0":
                                exp.And(u => (u.Status == 0 || u.MenkeDeleteTime > 0 || u.MenkeState == 0));
                                break;
                            case "1":
                                exp.And(u => u.MenkeState == 1 && u.Status == 1 && u.MenkeDeleteTime == 0 && u.MenkeEndtime >= thetime);
                                break;
                            case "2":
                                exp.And(u => u.MenkeState == 2 && u.Status == 1 && u.MenkeDeleteTime == 0);
                                break;
                            case "3":
                                exp.And(u => u.MenkeState == 3 && u.Status == 1 && u.MenkeDeleteTime == 0);
                                break;
                            case "4":
                                exp.And(u => (u.MenkeState == 4 || (u.MenkeState == 1 && u.MenkeEndtime < thetime)) && u.Status == 1 && u.MenkeDeleteTime == 0);
                                break;
                        }
                    }
                    exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.MenkeStarttime >= btime);
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.MenkeStarttime <= etime);
					if (sort != "")
                    {
                        sort = StringHelper.ChgToUnderLine(sort);
                        if (order == "desc")
						{
							sort += " desc,";
						}
						else
						{
							sort += ",";
						}
					}
					sort += "menke_lesson_id desc";

					var total = 0;
					list_UserLesson = _context.Queryable<WebUserLesson>().Where(exp.ToExpression()).OrderBy(sort).ToPageList(page, pagesize, ref total);
					var list_User = _context.Queryable<WebUser>().Where(u => list_UserLesson.Select(s => s.Userid).Contains(u.Userid)).ToList();
					var list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_UserLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
					var list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
					var list_Report = _context.Queryable<WebUserLessonReport>().Where(u => list_UserLesson.Select(s => s.UserLessonid).Contains(u.UserLessonid)).ToList();
					var list_Homework = _context.Queryable<MenkeHomework>().Where(u => list_UserLesson.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
					var list_HomeworkSubmit = _context.Queryable<MenkeHomeworkSubmit>().Where(u => list_Homework.Select(s => s.MenkeHomeworkId).Contains(u.MenkeHomeworkId)).ToList();
					var list_HomeworkRemark = _context.Queryable<MenkeHomeworkRemark>().Where(u => list_HomeworkSubmit.Select(s => s.MenkeSubmitId).Contains(u.MenkeSubmitId)).ToList();
					
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_UserLesson.Count; i++)
					{
						model_User = list_User.FirstOrDefault(u => u.Userid == list_UserLesson[i].Userid);
						var model_MenkeCourse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == list_UserLesson[i].MenkeCourseId);
						var model_MenkeLesson = list_MenkeLesson.FirstOrDefault(u => u.MenkeLessonId == list_UserLesson[i].MenkeLessonId);
						var model_Homework = list_Homework.FirstOrDefault(u => u.MenkeLessonId == list_UserLesson[i].MenkeLessonId);
						var model_HomeworkSubmit = list_HomeworkSubmit.Where(u =>u.MenkeSubmitDelTime==0 && u.MenkeStudentCode == list_UserLesson[i].MenkeStudentCode && u.MenkeStudentMobile == list_UserLesson[i].MenkeStudentMobile && u.MenkeHomeworkId == model_Homework?.MenkeHomeworkId).OrderByDescending(u=>u.MenkeSubmitTime).FirstOrDefault();
						var model_HomeworkRemark = list_HomeworkRemark.Where(u => u.MenkeSubmitId == model_HomeworkSubmit?.MenkeSubmitId).OrderByDescending(u=>u.MenkeIsPass).FirstOrDefault();
						var time1 = DateHelper.ConvertIntToDateTime(list_UserLesson[i].MenkeStarttime.ToString()).ToLocalTime();
                        var time2 = DateHelper.ConvertIntToDateTime(list_UserLesson[i].MenkeEndtime.ToString()).ToLocalTime();

                        str.Append("{");
						str.Append("\"UserLessonid\":\"" + list_UserLesson[i].UserLessonid + "\",");
						str.Append("\"UserCourseid\":\"" + list_UserLesson[i].UserCourseid + "\",");
						str.Append("\"Type\":" + (list_UserLesson[i].OnlineCourseid > 0 ? 1 : 0) + ",");
						str.Append("\"IsReport\":" + (list_Report.Any(u => u.UserLessonid == list_UserLesson[i].UserLessonid) ? 1 : 0) + ",");
						str.Append("\"IsHomework\":" + (model_Homework!=null ? 1 : 0) + ",");
						str.Append("\"IsHomeworkSubmit\":" + (model_HomeworkSubmit!=null ? 1 : 0) + ",");
						str.Append("\"Istrial\":\"" + (model_MenkeCourse?.Istrial == 1?"是":"否") + "\",");
						str.Append("\"IsPass\":" + (model_HomeworkRemark!=null? model_HomeworkRemark.MenkeIsPass: -1) + ",");
						str.Append("\"Email\":\"" + model_User?.Email + "\",");
						str.Append("\"Userid\":" + list_UserLesson[i].Userid + ",");
                        str.Append("\"MenkeUserId\":\"" + list_UserLesson[i].MenkeUserId + "\",");
						str.Append("\"Teacherid\":" + list_UserLesson[i].Teacherid + ",");
						str.Append("\"CourseSkuid\":" + list_UserLesson[i].CourseSkuid + ",");
                        str.Append("\"MenkeLiveSerial\":\"" + list_UserLesson[i].MenkeLiveSerial + "\",");
                        str.Append("\"MenkeCourseId\":\"" + list_UserLesson[i].MenkeCourseId + "\",");
                        str.Append("\"MenkeLessonId\":\"" + list_UserLesson[i].MenkeLessonId + "\",");
						str.Append("\"MenkeLessonName\":\"" + JsonHelper.JsonCharFilter(list_UserLesson[i].MenkeLessonName) + "\",");
						str.Append("\"MenkeTeacherName\":\"" + JsonHelper.JsonCharFilter(list_UserLesson[i].MenkeTeacherName) + "\",");
						str.Append("\"MenkeStudentName\":\"" + JsonHelper.JsonCharFilter(list_UserLesson[i].MenkeStudentName) + "\",");
						str.Append("\"MenkeStudentMobile\":\"" + list_UserLesson[i].MenkeStudentCode +" "+ list_UserLesson[i].MenkeStudentMobile + "\",");
						str.Append("\"MenkeName\":\"" + JsonHelper.JsonCharFilter(model_MenkeCourse?.MenkeName+"") + "\",");
						str.Append("\"MenkeTime\":\"" + JsonHelper.JsonCharFilter(time1.ToString("yyyy-MM-dd") + "<br/>" + time1.ToString("HH:mm:ss") + " - " + time2.ToString("HH:mm:ss")) + "\",");
						str.Append("\"MenkeState\":" + ((list_UserLesson[i].MenkeState==1 && list_UserLesson[i].MenkeEndtime<thetime)?4: list_UserLesson[i].MenkeState) + ",");
						str.Append("\"TeacherCode\":\"" + model_MenkeLesson?.TeacherCode + "\",");
						str.Append("\"Dtime\":\"" + list_UserLesson[i].Dtime + "\",");
                        str.Append("\"MenkeDeleteTime\":" + list_UserLesson[i].MenkeDeleteTime + ",");
                        str.Append("\"Status\":" + list_UserLesson[i].Status + ",");
						str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(son_str);
						if (i < (list_UserLesson.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}

		#endregion

		#region "添加修改已排课节"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserLessonAdd(long UserLessonid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_Add = new UserLessonAdd(); 
			var thetime = DateHelper.ConvertDateTimeInt(DateTime.UtcNow);
			var model_UserLesson = _context.Queryable<WebUserLesson>().InSingle(UserLessonid);
			if (model_UserLesson != null)
			{
				var model_MenkeLesson = _context.Queryable<MenkeLesson>().First(u => u.MenkeLessonId == model_UserLesson.MenkeLessonId);
				var model_Report = _context.Queryable<WebUserLessonReport>().First(u => model_UserLesson.UserLessonid == u.UserLessonid);

				vm_Add.UserLessonid = model_UserLesson.UserLessonid;
				vm_Add.MenkeLessonName = model_UserLesson.MenkeLessonName;
				vm_Add.MenkeState = ((model_UserLesson.MenkeState == 1 && model_UserLesson.MenkeEndtime < thetime) ? 4 : model_UserLesson.MenkeState);
				vm_Add.MenkeStarttime = model_UserLesson.MenkeStarttime;
				vm_Add.MenkeEndtime = model_UserLesson.MenkeEndtime;
				vm_Add.Teacherid = model_UserLesson.Teacherid;
				vm_Add.MenkeTeacherName = model_UserLesson.MenkeTeacherName;
				vm_Add.MenkeTeacherCode = model_UserLesson.MenkeTeacherCode;
				vm_Add.MenkeTeacherMobile = model_UserLesson.MenkeTeacherMobile;
				vm_Add.Userid = model_UserLesson.Userid;
				vm_Add.MenkeStudentName = model_UserLesson.MenkeStudentName;
				vm_Add.MenkeStudentCode = model_UserLesson.MenkeStudentCode;
				vm_Add.MenkeStudentMobile = model_UserLesson.MenkeStudentMobile;
				vm_Add.Status = model_UserLesson.Status;
				vm_Add.Remark = model_UserLesson.Remark;

				vm_Add.TeacherCode = model_MenkeLesson?.TeacherCode;
				vm_Add.IsReport = model_Report == null ? 0 : 1;
			}
			else
			{
				vm_Add.Status = 1;
			}
			return View(vm_Add);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserLessonAdd(UserLessonAdd vm_Add)
		{
			string message="";
			var model_Master = _tokenManager.GetAdminInfo();
			var model_UserLesson = _context.Queryable<WebUserLesson>().InSingle(vm_Add.UserLessonid);
			if (model_UserLesson != null)
			{
				message = $"[{DateTime.Now}]{model_Master.Username}" + _localizer["修改用户课节信息"];
				if (model_UserLesson.Status != vm_Add.Status)
				{
					message += $",{_localizer["修改显示状态"]}[" + model_UserLesson.Status + "]=>[" + vm_Add.Status + "]";
				}
				if (!string.IsNullOrEmpty(vm_Add.Message)) {
					message += ",增加备注:" + vm_Add.Message;
				}
				model_UserLesson.Status = vm_Add.Status;
				if (model_UserLesson.Remark.Length < 4000)
				{
					model_UserLesson.Remark = message + "<hr>" + model_UserLesson.Remark;
				}
				else
				{
					model_UserLesson.Remark = message + "<hr>" + model_UserLesson.Remark.Substring(0, 4000);
				}
				_context.Updateable(model_UserLesson).ExecuteCommand();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改用户信息"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				return Content("<script>alert('"+ _localizer["不允许在网站后台进行排课操作"] +"!')</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion

		#region "课节报告"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserLessonReport(long UserLessonid,long MenkeUserid)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_Add = new UserLessonReport();
			vm_Add.IsReport = false;
			var model_UserLesson = _context.Queryable<WebUserLesson>().InSingle(UserLessonid);
			if (model_UserLesson != null)
			{
				vm_Add.UserLessonid = model_UserLesson.UserLessonid;
				vm_Add.MenkeLessonName = model_UserLesson.MenkeLessonName;
				vm_Add.Begintime = DateHelper.ConvertIntToDateTime(model_UserLesson.MenkeStarttime.ToString()).ToLocalTime();
				vm_Add.Endtime = DateHelper.ConvertIntToDateTime(model_UserLesson.MenkeEndtime.ToString()).ToLocalTime();
				vm_Add.TeacherName = model_UserLesson.MenkeTeacherName;
				vm_Add.TeacherMobileCode = model_UserLesson.MenkeTeacherCode;
				vm_Add.TeacherMobile = model_UserLesson.MenkeTeacherMobile;
				vm_Add.StudentName = model_UserLesson.MenkeStudentName;
				vm_Add.StudentMobileCode = model_UserLesson.MenkeStudentCode;
				vm_Add.StudentMobile = model_UserLesson.MenkeStudentMobile;
				var model_MenkeCourse = _context.Queryable<MenkeCourse>().First(u => u.MenkeCourseId == model_UserLesson.MenkeCourseId);
				vm_Add.MenkeCourseName = model_MenkeCourse?.MenkeName;
			}

			var model_UserLessonReport = _context.Queryable<WebUserLessonReport>().First(u => u.UserLessonid == UserLessonid && u.MenkeUserid == MenkeUserid);
			if (model_UserLessonReport != null)
			{
				vm_Add.IsReport = true;
				var model_Teacher = _context.Queryable<WebTeacher>().LeftJoin<WebTeacherLang>((l,r)=>l.Teacherid==r.Teacherid)
					.Where((l,r) => l.Teacherid == model_UserLessonReport.Teacherid && r.Lang == Lang)
					.Select((l,r)=>new { r.Name, l.Teacherid,l.Mobile,l.MobileCode })
					.First();
				vm_Add.Message = model_UserLessonReport.Message;
				vm_Add.Homework = model_UserLessonReport.Homework;
				vm_Add.Attention = model_UserLessonReport.Attention;
				vm_Add.Enthusiasm = model_UserLessonReport.Enthusiasm;
				vm_Add.Hear = model_UserLessonReport.Hear;
				vm_Add.Say = model_UserLessonReport.Say;
				vm_Add.Read = model_UserLessonReport.Read;
				vm_Add.Write = model_UserLessonReport.Write;
				vm_Add.Thinking = model_UserLessonReport.Thinking;
				vm_Add.EmotionalQuotient = model_UserLessonReport.EmotionalQuotient;
				vm_Add.LoveQuotient = model_UserLessonReport.LoveQuotient;
				vm_Add.InverseQuotient = model_UserLessonReport.InverseQuotient;
				vm_Add.Performance = model_UserLessonReport.Performance;
				vm_Add.Remarks = model_UserLessonReport.Remarks;
			}
			return View(vm_Add);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> UserLessonReport(UserLessonReport vm_Add)
		{
			string message = "";
			var model_Master = _tokenManager.GetAdminInfo();
			var model_UserLesson = _context.Queryable<WebUserLessonReport>().InSingle(vm_Add.UserLessonid);
			if (model_UserLesson != null)
			{
				message = $"[{DateTime.Now}]{model_Master.Username}" + _localizer["修改用户课节信息"];
				
				if (!string.IsNullOrEmpty(vm_Add.Message)) {
					message += ",增加备注:" + vm_Add.Message;
				}
				
				if (model_UserLesson.Remarks.Length < 4000)
				{
					model_UserLesson.Remarks = message + "<hr>" + model_UserLesson.Remarks;
				}
				else
				{
					model_UserLesson.Remarks = message + "<hr>" + model_UserLesson.Remarks.Substring(0, 4000);
				}
				_context.Updateable(model_UserLesson).ExecuteCommand();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改课节报告"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				return Content("<script>alert('"+ _localizer["不允许在网站后台创建课节报告"] +"!')</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion
        #endregion

        #region "学生考勤表" 
        [Authorization(Power = "Main")]
        public IActionResult Attendance()
        {
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View();
        }

        [HttpGet]
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> AttendanceData(string keys = "", string status = "", string begintime = "", string endtime = "")
        {
            StringBuilder str = new StringBuilder();
            int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
            int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
            string MenkeAttendanceids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "MenkeAttendanceids");
            long MenkeAttendanceid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "MenkeAttendanceid", 0);
            var list_Attendance = new List<MenkeAttendance>();
            if (MenkeAttendanceids.Trim() == "") MenkeAttendanceids = "0";
            string[] arr = MenkeAttendanceids.Split(',');
            var exp = new Expressionable<MenkeAttendance>();
            string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
            string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
            List<MenkeLesson> list_MenkeLesson;
            List<MenkeCourse> list_MenkeCourse;
            List<WebUser> list_User;

            switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
            {
                case "export":
                    exp.And(u => u.Status != -1 && u.MenkeAttendanceUserroleid == 8);
                    exp.AndIF(!string.IsNullOrEmpty(keys), u =>
                        SqlFunc.MergeString(u.MenkeAttendanceMobileCode, u.MenkeAttendanceMobile).Contains(keys) ||
                        SqlFunc.Subqueryable<MenkeLesson>().Where(s => s.MenkeLessonId == u.MenkeLessonId && SqlFunc.MergeString(s.MenkeTeacherName, "").Contains(keys)).Any()
                    );
                    exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    if (!string.IsNullOrEmpty(begintime))
                    {
                        var btime = DateHelper.ConvertDateTimeInt(DateTime.Parse(begintime));
                        exp.And(u => u.MenkeStarttime >= btime);
                    }
                    if (!string.IsNullOrEmpty(endtime))
                    {
                        var etime = DateHelper.ConvertDateTimeInt(DateTime.Parse(endtime));
                        exp.And(u => u.MenkeEndtime <= etime);
                    }

                    var query_export = _context.Queryable<MenkeAttendance>().Where(exp.ToExpression()); 
					if (sort != "")
                    {
                        sort = StringHelper.ChgToUnderLine(sort);
                        if (order == "desc")
                        {
                            query_export.OrderBy(sort + " desc");
                        }
                        else
                        {
                            query_export.OrderBy(sort);
                        }
                    }
                    query_export.OrderBy("Menke_Starttime desc,Menke_Attendanceid desc");
                    list_Attendance = query_export.ToList();
                    list_User = _context.Queryable<WebUser>().Where(u => list_Attendance.Select(u => u.MenkeAttendanceMobileCode + u.MenkeAttendanceMobile).Contains(SqlFunc.MergeString(u.MobileCode, u.Mobile))).ToList();
                    var list_Teacher1 = _context.Queryable<MenkeAttendance>().Where(u => list_Attendance.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId) && !string.IsNullOrEmpty(u.MenkeAttendanceAccessRecord) && u.Status != -1 && u.MenkeAttendanceUserroleid == 7)
                        .Select(u => new { u.MenkeLessonId, u.MenkeAttendanceAccessRecord, u.MenkeTimeinfo }).ToList();
                    list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => list_Attendance.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
                    list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_MenkeLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();

                    string fileurl = Appsettings.app("Web:Upfile") + "/Attendance/";
                    FileHelper.AddFolder(Appsettings.app("Web:ImgRoot") + fileurl);
                    string filename = fileurl + "" + Guid.NewGuid() + ".csv";
                    var encoding = Lang == "zh-cn" ? Encoding.GetEncoding("GB2312") : Encoding.Default;
                    CsvWriterHelper csv = new CsvWriterHelper(Appsettings.app("Web:ImgRoot") + filename, encoding);
                    csv[1, 1] = "ID";
                    csv[1, 2] = _localizer["学生名称"];
                    csv[1, 3] = _localizer["学生手机"];
                    csv[1, 4] = _localizer["课程名称"];
                    csv[1, 5] = _localizer["课节名称"];
                    csv[1, 6] = _localizer["排课时间"];
                    csv[1, 7] = _localizer["上下课时间"];
                    csv[1, 8] = _localizer["进出教室时间"];
                    csv[1, 9] = _localizer["学生有效时长"];
                    csv[1, 10] = _localizer["老师有效时长"];
                    csv[1, 11] = _localizer["迟到"];
                    csv[1, 12] = _localizer["早退"];
                    csv[1, 13] = _localizer["缺勤"];
                    for (int i = 0; i < list_Attendance.Count; i++)
                    {
                        var model_MenkeLesson = list_MenkeLesson.FirstOrDefault(u => u.MenkeLessonId == list_Attendance[i].MenkeLessonId);
                        var model_MenkeCoursse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == model_MenkeLesson?.MenkeCourseId);
						var model_User = list_User.FirstOrDefault(u => u.MobileCode == list_Attendance[i].MenkeAttendanceMobileCode && u.Mobile == list_Attendance[i].MenkeAttendanceMobile);

                        var result_student_time = TimeIntersection(list_Attendance[i].MenkeAttendanceAccessRecord, list_Attendance[i].MenkeTimeinfo);
                        string eotime = result_student_time.out_eotime != null ? string.Join("\r\n", result_student_time.out_eotime.Select(u => u.bengintime.ToString("yyyy-MM-dd HH:mm:ss") + " | " + u.endtime.ToString("HH:mm:ss"))) : "";
                        string timeinfo = result_student_time.out_timeinfo != null ? string.Join("\r\n", result_student_time.out_timeinfo.Select(u => u.bengintime.ToString("yyyy-MM-dd HH:mm:ss") + " | " + u.endtime.ToString("HH:mm:ss"))) : "";
                        var student_thetime = result_student_time.timeSpan;

						var model_Teacher = list_Teacher1.FirstOrDefault(u => u.MenkeLessonId == list_Attendance[i].MenkeLessonId);
                        var result_teacher_time = TimeIntersection(model_Teacher.MenkeAttendanceAccessRecord, model_Teacher.MenkeTimeinfo);
                        var teacher_thetime = result_teacher_time.timeSpan;


                        csv[i + 2, 1] = list_Attendance[i].MenkeAttendanceid.ToString();
                        csv[i + 2, 2] = JsonHelper.JsonCharFilter(model_User?.FirstName + " " + model_User?.LastName).Replace("\"", "");
                        csv[i + 2, 3] = list_Attendance[i].MenkeAttendanceMobileCode + "-" + list_Attendance[i].MenkeAttendanceMobile;
                        csv[i + 2, 4] = (model_MenkeCoursse?.MenkeName + "").Replace("\"", "");
                        csv[i + 2, 5] = (model_MenkeLesson?.MenkeName + "").Replace("\"", "");
                        csv[i + 2, 6] = DateHelper.ConvertIntToDateTime(list_Attendance[i].MenkeStarttime.ToString()).ToLocalTime() + "\r\n" + DateHelper.ConvertIntToDateTime(list_Attendance[i].MenkeEndtime.ToString()).ToLocalTime();
                        csv[i + 2, 7] = eotime;
                        csv[i + 2, 8] = timeinfo;
                        csv[i + 2, 9] = student_thetime.ToString();
                        csv[i + 2, 10] = teacher_thetime.ToString();
                        csv[i + 2, 11] = (list_Attendance[i].MenkeAttendanceLate == 1 ? _localizer["是"] : _localizer["否"]);
                        csv[i + 2, 12] = (list_Attendance[i].MenkeAttendanceLeaveEarly == 1 ? _localizer["是"] : _localizer["否"]);
                        csv[i + 2, 13] = (list_Attendance[i].MenkeAttendanceAbsent == 1 ? _localizer["是"] : _localizer["否"]);
                    }
                    csv.Save();
                    if (list_Attendance.Count > 0)
                    {
                        str.Append("{\"errorcode\":\"0\",\"msg\":\"执行成功,导出" + list_Attendance.Count + "条！\",\"filename\":\"" + filename + "\"}");
                    }
                    else
                    {
                        str.Append("{\"errorcode\":\"-1\",\"msg\":\"执行失败！\",\"filename\":\"\"}");
                    }
                    break;
                default:
                    int total = 0;
                    exp.And(u => u.Status != -1 && u.MenkeAttendanceUserroleid == 7);
                    exp.AndIF(!string.IsNullOrEmpty(keys), u =>
                        SqlFunc.MergeString(u.MenkeAttendanceMobileCode, u.MenkeAttendanceMobile).Contains(keys) ||
                        SqlFunc.Subqueryable<MenkeLesson>().Where(s => s.MenkeLessonId == u.MenkeLessonId && SqlFunc.MergeString(s.MenkeTeacherName, "").Contains(keys)).Any()
                    );
                    exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    if (!string.IsNullOrEmpty(begintime))
                    {
                        var btime = DateHelper.ConvertDateTimeInt(DateTime.Parse(begintime));
                        exp.And(u => u.MenkeStarttime >= btime);
                    }
                    if (!string.IsNullOrEmpty(endtime))
                    {
                        var etime = DateHelper.ConvertDateTimeInt(DateTime.Parse(endtime));
                        exp.And(u => u.MenkeEndtime <= etime);
                    }
                    var query = _context.Queryable<MenkeAttendance>().Where(exp.ToExpression());
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
                    query.OrderBy("Menke_Starttime desc,Menke_Attendanceid desc");
                    list_Attendance = query.ToPageList(page, pagesize, ref total);
                    var list_Student = _context.Queryable<MenkeAttendance>().Where(u => list_Attendance.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId) && !string.IsNullOrEmpty(u.MenkeAttendanceAccessRecord) && u.Status != -1 && u.MenkeAttendanceUserroleid == 8)
                        .Select(u => new { u.MenkeLessonId, u.MenkeAttendanceAccessRecord, u.MenkeTimeinfo }).ToList();
                    list_MenkeLesson = _context.Queryable<MenkeLesson>().Where(u => list_Attendance.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId)).ToList();
                    list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_MenkeLesson.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
                    list_User = _context.Queryable<WebUser>().Where(u => list_Attendance.Select(u => u.MenkeAttendanceMobileCode + u.MenkeAttendanceMobile).Contains(SqlFunc.MergeString(u.MobileCode, u.Mobile))).ToList();
                    var list_Teacher = _context.Queryable<MenkeAttendance>().Where(u => list_Attendance.Select(s => s.MenkeLessonId).Contains(u.MenkeLessonId) && !string.IsNullOrEmpty(u.MenkeAttendanceAccessRecord) && u.Status != -1 && u.MenkeAttendanceUserroleid == 7)
                        .Select(u => new { u.MenkeLessonId, u.MenkeAttendanceAccessRecord, u.MenkeTimeinfo }).ToList();

                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
                    for (int i = 0; i < list_Attendance.Count; i++)
                    {
                        var model_MenkeLesson = list_MenkeLesson.FirstOrDefault(u => u.MenkeLessonId == list_Attendance[i].MenkeLessonId);
                        var model_MenkeCoursse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == model_MenkeLesson?.MenkeCourseId);
                        var model_User = list_User.FirstOrDefault(u => u.MobileCode == list_Attendance[i].MenkeAttendanceMobileCode && u.Mobile == list_Attendance[i].MenkeAttendanceMobile);

                        var result_student_time = TimeIntersection(list_Attendance[i].MenkeAttendanceAccessRecord, list_Attendance[i].MenkeTimeinfo);
                        string eotime = result_student_time.out_eotime != null ? string.Join("<br>", result_student_time.out_eotime.Select(u => u.bengintime.ToString("yyyy-MM-dd HH:mm:ss") + " | " + u.endtime.ToString("HH:mm:ss"))) : "";
                        string timeinfo = result_student_time.out_timeinfo != null ? string.Join("<br>", result_student_time.out_timeinfo.Select(u => u.bengintime.ToString("yyyy-MM-dd HH:mm:ss") + " | " + u.endtime.ToString("HH:mm:ss"))) : "";
                        var student_thetime = result_student_time.timeSpan;
						var teacher_thetime = TimeSpan.Zero;
                        var model_Teacher = list_Teacher.FirstOrDefault(u => u.MenkeLessonId == list_Attendance[i].MenkeLessonId);
						if (model_Teacher != null)
						{
							var result_teacher_time = TimeIntersection(model_Teacher.MenkeAttendanceAccessRecord, model_Teacher.MenkeTimeinfo);
							teacher_thetime = result_teacher_time.timeSpan;
						}
                        str.Append("{");
                        str.Append("\"MenkeAttendanceid\":" + list_Attendance[i].MenkeAttendanceid + ",");
                        str.Append("\"MenkeLessonId\":" + list_Attendance[i].MenkeLessonId + ",");
                        str.Append("\"Name\":\"" + JsonHelper.JsonCharFilter(model_User?.FirstName + " " + model_User?.LastName) + "\",");
                        str.Append("\"Mobile\":\"" + list_Attendance[i].MenkeAttendanceMobileCode + "-" + list_Attendance[i].MenkeAttendanceMobile + "\",");
                        str.Append("\"MenkeCourseName\":\"" + model_MenkeCoursse?.MenkeName + "\",");//menke_course
                        str.Append("\"MenkeLessonName\":\"" + model_MenkeLesson?.MenkeName + "\",");//web_user_lesson,menke_lesson
                        str.Append("\"Menketime\":\"" + DateHelper.ConvertIntToDateTime(list_Attendance[i].MenkeStarttime.ToString()).ToLocalTime() + "<br />" + DateHelper.ConvertIntToDateTime(list_Attendance[i].MenkeEndtime.ToString()).ToLocalTime() + "\",");
                        str.Append("\"EOtime\":\"" + eotime + "\",");
                        str.Append("\"Timeinfo\":\"" + JsonHelper.JsonCharFilter(timeinfo) + "\",");
                        str.Append("\"TeacherThetime\":\"" + JsonHelper.JsonCharFilter(teacher_thetime.ToString()) + "\",");
                        str.Append("\"StudentThetime\":\"" + JsonHelper.JsonCharFilter(student_thetime.ToString()) + "\",");
                        str.Append("\"MenkeAttendanceLate\":" + list_Attendance[i].MenkeAttendanceLate + ",");
                        str.Append("\"MenkeAttendanceLeaveEarly\":" + list_Attendance[i].MenkeAttendanceLeaveEarly + ",");
                        str.Append("\"MenkeAttendanceAbsent\":" + list_Attendance[i].MenkeAttendanceAbsent + ",");
                        str.Append("\"Status\":" + list_Attendance[i].Status + "}");
                        if (i < (list_Attendance.Count - 1)) str.Append(",");
                    }
                    str.Append("]}");

                    break;
            }
            return Content(str.ToString());
        }

        /// <summary>
        /// 取两组数据的交集，并反回时分秒
        /// </summary>
        /// <param name="list_eotime"></param>
        /// <param name="list_timeinfo"></param>
        /// <returns></returns>
        private (TimeSpan timeSpan, List<Thetime> out_eotime, List<Thetime> out_timeinfo) TimeIntersection(string str_eotime, string str_timeinfo)
        {
            List<Thetime> list_eotime = new List<Thetime>();
            List<Thetime> list_timeinfo = new List<Thetime>();
            if (!string.IsNullOrEmpty(str_eotime)) list_eotime = JArray.Parse(str_eotime).Select(u => new Thetime() { btime = (int)u["entertime"], etime = (int)u["outtime"], bengintime = DateHelper.ConvertIntToDateTime(u["entertime"].ToString()).ToLocalTime(), endtime = DateHelper.ConvertIntToDateTime(u["outtime"].ToString()).ToLocalTime() }).ToList();
            if (!string.IsNullOrEmpty(str_timeinfo)) list_timeinfo = JArray.Parse(str_timeinfo).Select(u => new Thetime() { btime = (int)u["starttime"], etime = (int)u["endtime"], bengintime = DateHelper.ConvertIntToDateTime(u["starttime"].ToString()).ToLocalTime(), endtime = DateHelper.ConvertIntToDateTime(u["endtime"].ToString()).ToLocalTime() }).ToList();
            if (string.IsNullOrEmpty(str_eotime) || string.IsNullOrEmpty(str_timeinfo)) return (TimeSpan.Zero, list_eotime, list_timeinfo);
            TimeSpan timeSpan = new TimeSpan();
            foreach (var eotime in list_eotime)
            {
                foreach (var timeinfo in list_timeinfo)
                {
                    //拿最小的etime-最大的btime,值大于0表示相交
                    int min_etime = timeinfo.etime > eotime.etime ? eotime.etime : timeinfo.etime;
                    int max_btime = timeinfo.btime < eotime.btime ? eotime.btime : timeinfo.btime;
                    if ((min_etime - max_btime) > 0)
                    {
                        timeSpan += TimeSpan.FromSeconds(min_etime - max_btime);
                    }
                    else
                    {
                        //无并集
                    }
                }
            }
            return (timeSpan, list_eotime, list_timeinfo);
        }
        #endregion
    }
}