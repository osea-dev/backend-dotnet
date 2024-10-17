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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Models;
using WeTalk.Web.Extensions;
using WeTalk.Web.ViewModels.News;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class MessageController : Base.BaseController
	{

		private readonly TokenManager _tokenManager;
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		public MessageController(TokenManager tokenManager, IStringLocalizer<LangResource> localizer, SqlSugarScope context, IHttpContextAccessor accessor
			)
			: base(tokenManager)
		{
			_tokenManager = tokenManager;
			_context = context;
			_accessor = accessor;
			_localizer = localizer;
		}

        #region "短消息列表"    
        [Authorization(Power = "Main")]
        public IActionResult Notice(long categoryid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			ViewBag.Categoryid = categoryid;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> NoticeData(string keys = "", string send_status = "", string acc_status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string Messageids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Messageids");
			long Messageid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Messageid", 0);
			var list_Message = new List<WebMessage>();
			if (Messageids.Trim() == "") Messageids = "0";
			string[] arr = Messageids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebMessage>()
						.SetColumns(u => u.SendStatus == -1)
						.SetColumns(u => u.AccStatus == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Messageid) || u.Messageid == Messageid)
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
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();

					int total = 0;
					var exp = new Expressionable<WebMessage>();
					exp.And(u => u.SendStatus != -1 ||u.AccStatus!=-1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => u.Message.Contains(keys));
					exp.AndIF(!string.IsNullOrEmpty(send_status), u => u.SendStatus.ToString() == send_status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(acc_status), u => u.AccStatus.ToString() == acc_status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Sendtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Sendtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebMessage>().Where(exp.ToExpression());
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
					query.OrderBy("sendtime desc,Messageid desc");
					list_Message = query.ToPageList(page, pagesize, ref total);
					var list_User = _context.Queryable<WebUser>().Where(u => list_Message.Select(s => s.SendUserid).Contains(u.Userid) || list_Message.Select(s => s.AccUserid).Contains(u.Userid))
						.Select(u=>new { u.Userid,u.FirstName,u.LastName,u.MessageTime})
						.ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Message.Count; i++)
					{
						var model_Send = list_User.FirstOrDefault(u => u.Userid == list_Message[i].SendUserid);
						var model_Acc = list_User.FirstOrDefault(u => u.Userid == list_Message[i].AccUserid);
						var sendname = model_Send?.FirstName + "" + model_Send?.LastName;
						var accname = model_Acc?.FirstName + "" + model_Acc?.LastName;
						int isread = (model_Acc != null && DateHelper.ConvertIntToDateTime(model_Acc.MessageTime.ToString(),"Local") >= DateTime.Now) ? 1 : 0;

						if (list_Message[i].SendUserid == 0)
						{
							sendname = "系统消息";
							accname = "系统消息";
						}
						str.Append("{");
						str.Append("\"Messageid\":\"" + list_Message[i].Messageid + "\",");
						str.Append("\"SendUserid\":" + list_Message[i].SendUserid + ",");
						str.Append("\"Message\":\"" + JsonHelper.JsonCharFilter(list_Message[i].Message + "") + "\",");
						str.Append("\"SendName\":\"" + sendname + "\",");
						str.Append("\"Sendtime\":\"" + list_Message[i].Sendtime + "\",");
						str.Append("\"SendStatus\":" + list_Message[i].SendStatus + ",");
						str.Append("\"AccName\":\"" + accname + "\",");
						str.Append("\"Acctime\":\"" + list_Message[i].Acctime.ToString() + "\",");
						str.Append("\"AccStatus\":" + list_Message[i].AccStatus + ",");
						str.Append("\"IsRead\":" + isread + ",");
						str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Message.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "修改"    
        [Authorization(Power = "Main")]
        public async Task<IActionResult> NoticeAdd(long Messageid = 0)
		{
			var vm_NoticeAdd = new NoticeAdd();

			var model_Message = _context.Queryable<WebMessage>().InSingle(Messageid);
			if (model_Message != null)
			{
				var list_User = _context.Queryable<WebUser>().Where(u => model_Message.SendUserid==u.Userid || model_Message.AccUserid == u.Userid)
						   .Select(u => new { u.Userid, u.FirstName, u.LastName })
						   .ToList();
				var model_Send = list_User.FirstOrDefault(u => u.Userid == model_Message.SendUserid);
				var model_Acc = list_User.FirstOrDefault(u => u.Userid == model_Message.AccUserid);
				if (model_Send != null) vm_NoticeAdd.SendName = model_Send.FirstName + " " + model_Send.LastName;
				if (model_Acc != null) vm_NoticeAdd.AccName = model_Acc.FirstName + " " + model_Acc.LastName;
				if (model_Message.SendUserid == 0)
				{
					vm_NoticeAdd.SendName = "系统短消息";
					vm_NoticeAdd.AccName = "系统短消息";
				}
				vm_NoticeAdd.Messageid = Messageid;				
				vm_NoticeAdd.AccUserid = model_Message.AccUserid;
				vm_NoticeAdd.AccStatus = model_Message.AccStatus;
				vm_NoticeAdd.Acctime = model_Message.Acctime;
				vm_NoticeAdd.SendUserid = model_Message.SendUserid;
				vm_NoticeAdd.SendStatus = model_Message.SendStatus;
				vm_NoticeAdd.Sendtime = model_Message.Sendtime;
				vm_NoticeAdd.Message = model_Message.Message;
			}
			else
			{
			}
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_NoticeAdd);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> NoticeAdd(NoticeAdd vm_NoticeAdd)
		{
			var model_Master = _tokenManager.GetAdminInfo();

			string message = "[" + DateTime.Now + "]" + model_Master.Name + _localizer["修改消息"] + "";
			var model_Message = _context.Queryable<WebMessage>().InSingle(vm_NoticeAdd.Messageid);
			if (model_Message != null)
			{
				if (model_Message.Message != vm_NoticeAdd.Message)
				{
					message += "," + _localizer["修改消息内容"];
				}
				model_Message.SendUserid = vm_NoticeAdd.SendUserid;
				model_Message.Sendtime = vm_NoticeAdd.Sendtime;
				model_Message.SendStatus = vm_NoticeAdd.SendStatus;
				model_Message.AccUserid = vm_NoticeAdd.AccUserid;
				model_Message.AccStatus = vm_NoticeAdd.AccStatus;
				model_Message.Remark = message + "<hr>" + vm_NoticeAdd.Remark;
				model_Message.Message=vm_NoticeAdd.Message;

				_context.Updateable(model_Message).ExecuteCommand();
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改短消息"] + "',msg:'" + _localizer["保存成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_Message = new WebMessage();
				model_Message.SendUserid = vm_NoticeAdd.SendUserid;
				model_Message.Sendtime = vm_NoticeAdd.Sendtime;
				model_Message.SendStatus = vm_NoticeAdd.SendStatus;
				model_Message.AccUserid = vm_NoticeAdd.AccUserid;
				model_Message.AccStatus = vm_NoticeAdd.AccStatus;
				model_Message.Remark = message + "<hr>" + vm_NoticeAdd.Remark;
				model_Message.Message = vm_NoticeAdd.Message;
				vm_NoticeAdd.Messageid = _context.Insertable(model_Message).ExecuteReturnBigIdentity();

				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加短消息"] + "',msg:'" + _localizer["保存成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
	}
}