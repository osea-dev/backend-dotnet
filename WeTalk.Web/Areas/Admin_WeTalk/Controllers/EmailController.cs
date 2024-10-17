using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
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
using WeTalk.Models;
using WeTalk.Web.Extensions;
using WeTalk.Web.ViewModels.Email;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class EmailController : Base.BaseController
	{

		private readonly TokenManager _tokenManager;
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		public EmailController(TokenManager tokenManager, IStringLocalizer<LangResource> localizer, SqlSugarScope context, IHttpContextAccessor accessor
			)
			: base(tokenManager)
		{
			_tokenManager = tokenManager;
			_context = context;
			_accessor = accessor;
			_localizer = localizer;
		}
        #region "模板列表"  
        [Authorization(Power = "Main")]
        public IActionResult List()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string EmailTemplateids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "EmailTemplateids");
			long EmailTemplateid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "EmailTemplateid", 0);
			var list_Template = new List<WebEmailTemplate>();
			if (EmailTemplateids.Trim() == "") EmailTemplateids = "0";
			string[] arr = EmailTemplateids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebEmailTemplate>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.EmailTemplateid) || u.EmailTemplateid == EmailTemplateid)
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
				case "enableu"://启用
					count = _context.Updateable<WebEmailTemplate>()
						.SetColumns(u => u.Status == 1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.EmailTemplateid) || u.EmailTemplateid == EmailTemplateid)
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
					count = _context.Updateable<WebEmailTemplate>()
						.SetColumns(u => u.Status == 0)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.EmailTemplateid) || u.EmailTemplateid == EmailTemplateid)
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
					var exp = new Expressionable<WebEmailTemplate>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.MergeString(u.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(u.Code, "").Contains(keys.Trim()));
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebEmailTemplate>().Where(exp.ToExpression());
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
					list_Template = query.ToPageList(page, pagesize, ref total);
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Template.Count; i++)
					{
						str.Append("{");
						str.Append("\"EmailTemplateid\":\"" + list_Template[i].EmailTemplateid + "\",");
						str.Append("\"TemplateName\":\"" + JsonHelper.JsonCharFilter(list_Template[i].TemplateName + "") + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(list_Template[i].Title + "") + "\",");
						str.Append("\"Code\":\"" + list_Template[i].Code + "\",");
						str.Append("\"Lang\":\"" + list_Template[i].Lang + "\",");
						str.Append("\"Dtime\":\"" + list_Template[i].Dtime.ToString() + "\",");
						str.Append("\"Status\":" + list_Template[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Template.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "模板修改"  
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListAdd(long EmailTemplateid = 0)
		{
			var vm_ListAdd = new ListAdd();
			vm_ListAdd.LangItems = _context.Queryable<PubLanguage>().Where(u => u.Status == 1).Select(u => new { u.Title, u.Lang }).ToList().Select(u => new SelectListItem(u.Title,u.Lang)).ToList();
			var model_Template = _context.Queryable<WebEmailTemplate>().InSingle(EmailTemplateid);
			if (model_Template != null)
			{
				vm_ListAdd.EmailTemplateid = EmailTemplateid;
				vm_ListAdd.TemplateName = model_Template.TemplateName;
				vm_ListAdd.Title = model_Template.Title;
				vm_ListAdd.Message = model_Template.Message;
				vm_ListAdd.Content = model_Template.Content;
				vm_ListAdd.Code = model_Template.Code;
				vm_ListAdd.Lang = model_Template.Lang;
				vm_ListAdd.Dtime = model_Template.Dtime;
				vm_ListAdd.Status = model_Template.Status;
				vm_ListAdd.Sort = model_Template.Sort;
			}
			else
			{
				vm_ListAdd.Sort = 100;
				vm_ListAdd.Status = 1;
			}
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListAdd(ListAdd vm_ListAdd)
		{
			var model_Master = _tokenManager.GetAdminInfo();

			string message = "[" + DateTime.Now + "]" + model_Master.Name + _localizer["修改信息"] + "";
			var model_Template = _context.Queryable<WebEmailTemplate>().InSingle(vm_ListAdd.EmailTemplateid);
			if (model_Template != null)
			{
				if (model_Template.TemplateName != vm_ListAdd.TemplateName)
				{
					message += "," + _localizer["修改模板名称"] + "，[" + model_Template.TemplateName + "]=>[" + vm_ListAdd.TemplateName + "]";
				}
				if (model_Template.Code != vm_ListAdd.Code)
				{
					message += "," + _localizer["修改模板代码"] + "，[" + model_Template.Code + "]=>[" + vm_ListAdd.Code + "]";
				}
				if (model_Template.Title != vm_ListAdd.Title)
				{
					message += "," + _localizer["修改邮件标题"] + "[" + model_Template.Title + "]=>[" + vm_ListAdd.Title + "]";
				}
				if (model_Template.Content != vm_ListAdd.Content)
				{
					message += "," + _localizer["修改过内容"] + "";
				}
				if (model_Template.Lang != vm_ListAdd.Lang)
				{
					message += "," + _localizer["修改所属语言"] + "[" + model_Template.Lang + "]=>[" + vm_ListAdd.Lang + "]";
				}
				if (model_Template.Sort != vm_ListAdd.Sort)
				{
					message += ",将排序字段由[" + model_Template.Sort + "]改为[" + vm_ListAdd.Sort + "]";
				}
				if (model_Template.Status != vm_ListAdd.Status)
				{
					message += ",将状态由[" + model_Template.Status + "]改为[" + vm_ListAdd.Status + "]";
				}

				model_Template.TemplateName = vm_ListAdd.TemplateName;
				model_Template.Title = vm_ListAdd.Title;
				model_Template.Message = vm_ListAdd.Message;
				model_Template.Content = vm_ListAdd.Content;
				model_Template.Code = vm_ListAdd.Code;
				model_Template.Lang = vm_ListAdd.Lang;
				model_Template.Status = vm_ListAdd.Status;
				model_Template.Sort = vm_ListAdd.Sort;
				model_Template.Remark = message + "<hr>" + vm_ListAdd.Remark;
				_context.Updateable(model_Template).ExecuteCommand();

				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改邮件模板"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_Template = new WebEmailTemplate();
				model_Template.TemplateName = vm_ListAdd.TemplateName;
				model_Template.Title = vm_ListAdd.Title;
				model_Template.Message = vm_ListAdd.Message;
				model_Template.Content = vm_ListAdd.Content;
				model_Template.Code = vm_ListAdd.Code;
				model_Template.Lang = vm_ListAdd.Lang;
				model_Template.Status = vm_ListAdd.Status;
				model_Template.Sort = vm_ListAdd.Sort;
				model_Template.Remark = message + "<hr>" + vm_ListAdd.Remark;
				_context.Insertable(model_Template).ExecuteCommand();

				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加邮件模板"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
	}
}