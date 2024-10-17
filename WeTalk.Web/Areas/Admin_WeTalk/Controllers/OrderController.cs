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
using WeTalk.Web.ViewModels.Order;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class OrderController : Base.BaseController
	{

		private readonly TokenManager _tokenManager;
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		public OrderController(TokenManager tokenManager, IStringLocalizer<LangResource> localizer, SqlSugarScope context, IHttpContextAccessor accessor
			)
			: base(tokenManager)
		{
			_tokenManager = tokenManager;
			_context = context;
			_accessor = accessor;
			_localizer = localizer;
		}
        //0众语课程，1直播课，2录播课，3线下课
        #region "众语课程订单"
        #region "订单列表"    
        [Authorization(Power = "Main")]
        public IActionResult List()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListData(string keys = "", string status = "",string user_deleted="", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string Orderids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Orderids");
			long Orderid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Orderid", 0);
			var list_Order = new List<WebOrder>();
			if (Orderids.Trim() == "") Orderids = "0";
			string[] arr = Orderids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebOrder>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Orderid) || u.Orderid == Orderid)
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
					var exp = new Expressionable<WebOrder>();
					exp.And(u => u.Status != -1 && u.Type == 0);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.MergeString(u.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(u.OrderSn, "").Contains(keys.Trim()) ||
					SqlFunc.Subqueryable<WebUser>().Where(s=>s.Userid==u.Userid && (s.Email.Contains(keys.Trim()) || s.Mobile.Contains(keys.Trim()) || SqlFunc.MergeString(s.FirstName,s.LastName).Contains(keys.Trim()))).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(user_deleted), u => u.UserDeleted.ToString() == user_deleted.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebOrder>().Where(exp.ToExpression());
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
					query.OrderBy("Orderid desc");
					list_Order = query.ToPageList(page, pagesize, ref total);
					var list_Course = _context.Queryable<WebCourse>().Where(u => list_Order.Select(s => s.Courseid).Contains(u.Courseid)).ToList();
					var list_User = _context.Queryable<WebUser>().Where(u => list_Order.Select(s => s.Userid).Contains(u.Userid)).ToList();
					var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == Lang && list_Order.Select(s => s.SkuTypeid).Contains(u.SkuTypeid)).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Order.Count; i++)
					{
						var model_Course = list_Course.FirstOrDefault(u => u.Courseid == list_Order[i].Courseid);
						var model_User = list_User.FirstOrDefault(u => u.Userid == list_Order[i].Userid);
						var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == list_Order[i].SkuTypeid);
						str.Append("{");
						str.Append("\"Orderid\":\"" + list_Order[i].Orderid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(list_Order[i].Title + "") + "\",");
						str.Append("\"Img\":\"" + JsonHelper.JsonCharFilter(model_Course?.Img + "") + "\",");
						str.Append("\"OrderSn\":\"" + list_Order[i].OrderSn + "\",");
						str.Append("\"Type\":\"" + model_SkuType?.Title + "\",");
						str.Append("\"ClassHour\":\"" + list_Order[i].ClassHour + "\",");
						if (model_User != null)
						{
							str.Append("\"Name\":\"" + model_User.FirstName + " "+ model_User.LastName + "\",");
							str.Append("\"Mobile\":\"" + model_User.MobileCode + " " + model_User.Mobile + "\",");
							str.Append("\"Email\":\"" + model_User.Email + "\",");

						}
						str.Append("\"Dtime\":\"" + list_Order[i].Dtime.ToString() + "\",");
						str.Append("\"Amount\":\"" + list_Order[i].Amount + "\",");
						str.Append("\"UserDeleted\":" + list_Order[i].UserDeleted + ",");
						str.Append("\"CurrencyCode\":\"" + list_Order[i].CurrencyCode + "\",");
						str.Append("\"Status\":" + list_Order[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Order.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "订单查阅"      
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListAdd(long Orderid = 0)
		{
			var vm_ListAdd = new ListAdd();
			vm_ListAdd.CurrencyCodeItem = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList().Select(u => new SelectListItem(u.CurrencyCode, u.CurrencyCode)).ToList();
			var model_Order = _context.Queryable<WebOrder>().InSingle(Orderid);
			if (model_Order != null)
			{
				vm_ListAdd.Orderid = Orderid;
				vm_ListAdd.OrderSn = model_Order.OrderSn;
				vm_ListAdd.Title = model_Order.Title;
				var model_User = _context.Queryable<WebUser>().InSingle(model_Order.Userid);
				if (model_User != null)
				{
					vm_ListAdd.Email = model_User.Email;
					vm_ListAdd.Name = model_User.FirstName + " " + model_User.LastName;
					vm_ListAdd.Mobile = model_User.MobileCode + model_User.Mobile;
				}
				var model_Course = _context.Queryable<WebCourse>().InSingle(model_Order.Courseid);
				if (model_Course != null)
				{
					vm_ListAdd.Img = model_Course.Img;
				}
				var model_SkuType = _context.Queryable<WebSkuTypeLang>().First(u => u.Lang == Lang && u.SkuTypeid == model_Order.SkuTypeid);
				vm_ListAdd.SkuType = model_SkuType?.Title;
				vm_ListAdd.ClassHour = model_Order.ClassHour;
				vm_ListAdd.Ip = model_Order.Ip;
				vm_ListAdd.CountryCode = model_Order.CountryCode;
				vm_ListAdd.Amount = model_Order.Amount;
				vm_ListAdd.CurrencyCode = model_Order.CurrencyCode;
				
				vm_ListAdd.Paytime = model_Order.Paytime;
				vm_ListAdd.Payment = model_Order.Payment;
				var model_Paytype = _context.Queryable<WebPaytype>().InSingle(model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					vm_ListAdd.PayTitle = model_Paytype.Title;
				}
				vm_ListAdd.TransactionId = model_Order.TransactionId;
				vm_ListAdd.Dtime = model_Order.Dtime;
				vm_ListAdd.Status = model_Order.Status;
				vm_ListAdd.UserDeleted = model_Order.UserDeleted;
				vm_ListAdd.CancelMessage = model_Order.CancelMessage;
				vm_ListAdd.CancelTime = model_Order.CancelTime;
				vm_ListAdd.Remark = model_Order.Remark;
			}
			else
			{
				vm_ListAdd.Status = 0;
				vm_ListAdd.UserDeleted = 0;
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
			var model_Order = _context.Queryable<WebOrder>().InSingle(vm_ListAdd.Orderid);
			if (model_Order != null)
			{
				if (model_Order.Status != vm_ListAdd.Status)
				{
					if (vm_ListAdd.Status == 1 && string.IsNullOrEmpty(vm_ListAdd.Message)) {
						return Content("<script>alert('"+ _localizer["手工修改订单为已付款，需要在【操作日志】处填写原因"] +"');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
					message += ",将状态由[" + model_Order.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (model_Order.Payment != vm_ListAdd.Payment)
				{
					message += ",将实付款由[" + model_Order.Payment + "]改为[" + vm_ListAdd.Payment + "]";
				}
				if (model_Order.CurrencyCode != vm_ListAdd.CurrencyCode)
				{
					message += ",将支付币种由[" + model_Order.CurrencyCode + "]改为[" + vm_ListAdd.CurrencyCode + "]";
				}
				if (model_Order.TransactionId != vm_ListAdd.TransactionId)
				{
					message += ",将支付流水号由[" + model_Order.TransactionId + "]改为[" + vm_ListAdd.TransactionId + "]";
				}
				if (model_Order.TransactionId != vm_ListAdd.TransactionId)
				{
					message += ",将支付流水号由[" + model_Order.TransactionId + "]改为[" + vm_ListAdd.TransactionId + "]";
				}
				if (!string.IsNullOrEmpty(vm_ListAdd.Message)) {
					message += ",添加日志:" + vm_ListAdd.Message;
				}
				model_Order.CurrencyCode = vm_ListAdd.CurrencyCode;
				model_Order.Payment = vm_ListAdd.Payment;
				model_Order.TransactionId = vm_ListAdd.TransactionId;
				model_Order.Status = vm_ListAdd.Status;
				model_Order.Remark = message + "<hr>" + model_Order.Remark;
				
				_context.Updateable(model_Order).ExecuteCommand();

				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改订单"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				return Content("<script>alert('" + _localizer["不允许后台创建订单!"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion
        #endregion

        #region "直播课程订单"
        #region "订单列表"    
        [Authorization(Power = "Main")]
        public IActionResult OnlineList()
        {
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View();
        }

        [HttpGet]
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> OnlineListData(string keys = "", string status = "", string user_deleted = "", string begintime = "", string endtime = "")
        {
            StringBuilder str = new StringBuilder();
            int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
            int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
            string Orderids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Orderids");
            long Orderid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Orderid", 0);
            var list_Order = new List<WebOrder>();
            if (Orderids.Trim() == "") Orderids = "0";
            string[] arr = Orderids.Split(',');
            int count = 0;
            switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
            {
                case "del":
                    count = _context.Updateable<WebOrder>()
                        .SetColumns(u => u.Status == -1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Orderid) || u.Orderid == Orderid)
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
                    var exp = new Expressionable<WebOrder>();
                    exp.And(u => u.Status != -1 && u.Type == 1);
                    exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.MergeString(u.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(u.OrderSn, "").Contains(keys.Trim()) ||
                    SqlFunc.Subqueryable<WebUser>().Where(s => s.Userid == u.Userid && (s.Email.Contains(keys.Trim()) || s.Mobile.Contains(keys.Trim()) || SqlFunc.MergeString(s.FirstName, s.LastName).Contains(keys.Trim()))).Any());
                    exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(user_deleted), u => u.UserDeleted.ToString() == user_deleted.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
                    exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
                    var query = _context.Queryable<WebOrder>().Where(exp.ToExpression());
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
                    query.OrderBy("Orderid desc");
                    list_Order = query.ToPageList(page, pagesize, ref total);
                    var list_Course = _context.Queryable<WebOnlineCourse>().Where(u => list_Order.Select(s => s.OnlineCourseid).Contains(u.OnlineCourseid)).ToList();
                    var list_User = _context.Queryable<WebUser>().Where(u => list_Order.Select(s => s.Userid).Contains(u.Userid)).ToList();
                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
                    for (int i = 0; i < list_Order.Count; i++)
                    {
                        var model_Course = list_Course.FirstOrDefault(u => u.OnlineCourseid == list_Order[i].OnlineCourseid);
                        var model_User = list_User.FirstOrDefault(u => u.Userid == list_Order[i].Userid);
                        str.Append("{");
                        str.Append("\"Orderid\":\"" + list_Order[i].Orderid + "\",");
                        str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(list_Order[i].Title + "") + "\",");
                        str.Append("\"Img\":\"" + JsonHelper.JsonCharFilter(model_Course?.Img + "") + "\",");
                        str.Append("\"OrderSn\":\"" + list_Order[i].OrderSn + "\",");
                        str.Append("\"LessonCount\":\"" + model_Course?.LessonCount + "\",");
                        str.Append("\"LessonStart\":\"" + model_Course?.LessonStart + "\",");
                        if (model_User != null)
                        {
                            str.Append("\"Name\":\"" + model_User.FirstName + " " + model_User.LastName + "\",");
                            str.Append("\"Mobile\":\"" + model_User.MobileCode + " " + model_User.Mobile + "\",");
                            str.Append("\"Email\":\"" + model_User.Email + "\",");
                        }
                        str.Append("\"Dtime\":\"" + list_Order[i].Dtime.ToString() + "\",");
                        str.Append("\"Amount\":\"" + list_Order[i].Amount + "\",");
                        str.Append("\"UserDeleted\":" + list_Order[i].UserDeleted + ",");
                        str.Append("\"CurrencyCode\":\"" + list_Order[i].CurrencyCode + "\",");
                        str.Append("\"Status\":" + list_Order[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
                        if (i < (list_Order.Count - 1)) str.Append(",");
                    }
                    str.Append("]}");

                    break;
            }
            return Content(str.ToString());
        }
        #endregion

        #region "订单查阅"      
        [Authorization(Power = "Main")]
        public async Task<IActionResult> OnlineListAdd(long Orderid = 0)
        {
            var vm_ListAdd = new ListAdd();
            vm_ListAdd.CurrencyCodeItem = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList().Select(u => new SelectListItem(u.CurrencyCode, u.CurrencyCode)).ToList();
            var model_Order = _context.Queryable<WebOrder>().InSingle(Orderid);
            if (model_Order != null)
            {
                vm_ListAdd.Orderid = Orderid;
                vm_ListAdd.OrderSn = model_Order.OrderSn;
                vm_ListAdd.Title = model_Order.Title;
                var model_User = _context.Queryable<WebUser>().InSingle(model_Order.Userid);
                if (model_User != null)
                {
                    vm_ListAdd.Email = model_User.Email;
                    vm_ListAdd.Name = model_User.FirstName + " " + model_User.LastName;
                    vm_ListAdd.Mobile = model_User.MobileCode + model_User.Mobile;
                }
                var model_Course = _context.Queryable<WebOnlineCourse>().InSingle(model_Order.OnlineCourseid);
                if (model_Course != null)
                {
                    vm_ListAdd.Img = model_Course.Img;
                    vm_ListAdd.LessonCount = model_Course.LessonCount;
                    vm_ListAdd.LessonStart = model_Course.LessonStart;
                }
                vm_ListAdd.Ip = model_Order.Ip;
                vm_ListAdd.CountryCode = model_Order.CountryCode;
                vm_ListAdd.Amount = model_Order.Amount;
                vm_ListAdd.CurrencyCode = model_Order.CurrencyCode;

                vm_ListAdd.Paytime = model_Order.Paytime;
                vm_ListAdd.Payment = model_Order.Payment;
                var model_Paytype = _context.Queryable<WebPaytype>().InSingle(model_Order.Paytypeid);
                if (model_Paytype != null)
                {
                    vm_ListAdd.PayTitle = model_Paytype.Title;
                }
                vm_ListAdd.TransactionId = model_Order.TransactionId;
                vm_ListAdd.Dtime = model_Order.Dtime;
                vm_ListAdd.Status = model_Order.Status;
                vm_ListAdd.UserDeleted = model_Order.UserDeleted;
                vm_ListAdd.CancelMessage = model_Order.CancelMessage;
                vm_ListAdd.CancelTime = model_Order.CancelTime;
                vm_ListAdd.Remark = model_Order.Remark;
            }
            else
            {
                vm_ListAdd.Status = 0;
                vm_ListAdd.UserDeleted = 0;
            }
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View(vm_ListAdd);
        }

        //保存添改
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> OnlineListAdd(ListAdd vm_ListAdd)
        {
            var model_Master = _tokenManager.GetAdminInfo();

            string message = "[" + DateTime.Now + "]" + model_Master.Name + _localizer["修改信息"] + "";
            var model_Order = _context.Queryable<WebOrder>().InSingle(vm_ListAdd.Orderid);
            if (model_Order != null)
            {
                if (model_Order.Status != vm_ListAdd.Status)
                {
                    if (vm_ListAdd.Status == 1 && string.IsNullOrEmpty(vm_ListAdd.Message))
                    {
                        return Content("<script>alert('" + _localizer["手工修改订单为已付款，需要在【操作日志】处填写原因"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
                    }
                    message += ",将状态由[" + model_Order.Status + "]改为[" + vm_ListAdd.Status + "]";
                }
                if (model_Order.Payment != vm_ListAdd.Payment)
                {
                    message += ",将实付款由[" + model_Order.Payment + "]改为[" + vm_ListAdd.Payment + "]";
                }
                if (model_Order.CurrencyCode != vm_ListAdd.CurrencyCode)
                {
                    message += ",将支付币种由[" + model_Order.CurrencyCode + "]改为[" + vm_ListAdd.CurrencyCode + "]";
                }
                if (model_Order.TransactionId != vm_ListAdd.TransactionId)
                {
                    message += ",将支付流水号由[" + model_Order.TransactionId + "]改为[" + vm_ListAdd.TransactionId + "]";
                }
                if (model_Order.TransactionId != vm_ListAdd.TransactionId)
                {
                    message += ",将支付流水号由[" + model_Order.TransactionId + "]改为[" + vm_ListAdd.TransactionId + "]";
                }
                if (!string.IsNullOrEmpty(vm_ListAdd.Message))
                {
                    message += ",添加日志:" + vm_ListAdd.Message;
                }
                model_Order.CurrencyCode = vm_ListAdd.CurrencyCode;
                model_Order.Payment = vm_ListAdd.Payment;
                model_Order.TransactionId = vm_ListAdd.TransactionId;
                model_Order.Status = vm_ListAdd.Status;
                model_Order.Remark = message + "<hr>" + model_Order.Remark;

                _context.Updateable(model_Order).ExecuteCommand();

                return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改订单"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
            else
            {
                return Content("<script>alert('" + _localizer["不允许后台创建订单!"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
        }
        #endregion
        #endregion

        #region "录播课程订单"
        #region "订单列表"    
        [Authorization(Power = "Main")]
        public IActionResult RecordList()
        {
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View();
        }

        [HttpGet]
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RecordListData(string keys = "", string status = "", string user_deleted = "", string begintime = "", string endtime = "")
        {
            StringBuilder str = new StringBuilder();
            int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
            int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
            string Orderids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Orderids");
            long Orderid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Orderid", 0);
            var list_Order = new List<WebOrder>();
            if (Orderids.Trim() == "") Orderids = "0";
            string[] arr = Orderids.Split(',');
            int count = 0;
            switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
            {
                case "del":
                    count = _context.Updateable<WebOrder>()
                        .SetColumns(u => u.Status == -1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Orderid) || u.Orderid == Orderid)
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
                    var exp = new Expressionable<WebOrder>();
                    exp.And(u => u.Status != -1 && u.Type == 2);
                    exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.MergeString(u.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(u.OrderSn, "").Contains(keys.Trim()) ||
                    SqlFunc.Subqueryable<WebUser>().Where(s => s.Userid == u.Userid && (s.Email.Contains(keys.Trim()) || s.Mobile.Contains(keys.Trim()) || SqlFunc.MergeString(s.FirstName, s.LastName).Contains(keys.Trim()))).Any());
                    exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(user_deleted), u => u.UserDeleted.ToString() == user_deleted.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
                    exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
                    var query = _context.Queryable<WebOrder>().Where(exp.ToExpression());
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
                    query.OrderBy("Orderid desc");
                    list_Order = query.ToPageList(page, pagesize, ref total);
                    var list_Course = _context.Queryable<WebRecordCourse>().Where(u => list_Order.Select(s => s.RecordCourseid).Contains(u.RecordCourseid)).ToList();
                    var list_User = _context.Queryable<WebUser>().Where(u => list_Order.Select(s => s.Userid).Contains(u.Userid)).ToList();
                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
                    for (int i = 0; i < list_Order.Count; i++)
                    {
                        var model_Course = list_Course.FirstOrDefault(u => u.RecordCourseid == list_Order[i].RecordCourseid);
                        var model_User = list_User.FirstOrDefault(u => u.Userid == list_Order[i].Userid);
                        str.Append("{");
                        str.Append("\"Orderid\":\"" + list_Order[i].Orderid + "\",");
                        str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(list_Order[i].Title + "") + "\",");
                        str.Append("\"Img\":\"" + JsonHelper.JsonCharFilter(model_Course?.Img + "") + "\",");
                        str.Append("\"OrderSn\":\"" + list_Order[i].OrderSn + "\",");
                        str.Append("\"LessonCount\":\"" + model_Course?.LessonCount + "\",");
                        str.Append("\"StudentCount\":\"" + model_Course?.StudentCount + "\",");
                        if (model_User != null)
                        {
                            str.Append("\"Name\":\"" + model_User.FirstName + " " + model_User.LastName + "\",");
                            str.Append("\"Mobile\":\"" + model_User.MobileCode + " " + model_User.Mobile + "\",");
                            str.Append("\"Email\":\"" + model_User.Email + "\",");
                        }
                        str.Append("\"Dtime\":\"" + list_Order[i].Dtime.ToString() + "\",");
                        str.Append("\"Amount\":\"" + list_Order[i].Amount + "\",");
                        str.Append("\"UserDeleted\":" + list_Order[i].UserDeleted + ",");
                        str.Append("\"CurrencyCode\":\"" + list_Order[i].CurrencyCode + "\",");
                        str.Append("\"Status\":" + list_Order[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
                        if (i < (list_Order.Count - 1)) str.Append(",");
                    }
                    str.Append("]}");

                    break;
            }
            return Content(str.ToString());
        }
        #endregion

        #region "订单查阅"      
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RecordListAdd(long Orderid = 0)
        {
            var vm_ListAdd = new ListAdd();
            vm_ListAdd.CurrencyCodeItem = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList().Select(u => new SelectListItem(u.CurrencyCode, u.CurrencyCode)).ToList();
            var model_Order = _context.Queryable<WebOrder>().InSingle(Orderid);
            if (model_Order != null)
            {
                vm_ListAdd.Orderid = Orderid;
                vm_ListAdd.OrderSn = model_Order.OrderSn;
                vm_ListAdd.Title = model_Order.Title;
                var model_User = _context.Queryable<WebUser>().InSingle(model_Order.Userid);
                if (model_User != null)
                {
                    vm_ListAdd.Email = model_User.Email;
                    vm_ListAdd.Name = model_User.FirstName + " " + model_User.LastName;
                    vm_ListAdd.Mobile = model_User.MobileCode + model_User.Mobile;
                }
                var model_Course = _context.Queryable<WebRecordCourse>().InSingle(model_Order.RecordCourseid);
                if (model_Course != null)
                {
                    vm_ListAdd.Img = model_Course.Img;
                    vm_ListAdd.LessonCount = model_Course.LessonCount;
                    vm_ListAdd.StudentCount = model_Course.StudentCount;
                }
                vm_ListAdd.Ip = model_Order.Ip;
                vm_ListAdd.CountryCode = model_Order.CountryCode;
                vm_ListAdd.Amount = model_Order.Amount;
                vm_ListAdd.CurrencyCode = model_Order.CurrencyCode;

                vm_ListAdd.Paytime = model_Order.Paytime;
                vm_ListAdd.Payment = model_Order.Payment;
                var model_Paytype = _context.Queryable<WebPaytype>().InSingle(model_Order.Paytypeid);
                if (model_Paytype != null)
                {
                    vm_ListAdd.PayTitle = model_Paytype.Title;
                }
                vm_ListAdd.TransactionId = model_Order.TransactionId;
                vm_ListAdd.Dtime = model_Order.Dtime;
                vm_ListAdd.Status = model_Order.Status;
                vm_ListAdd.UserDeleted = model_Order.UserDeleted;
                vm_ListAdd.CancelMessage = model_Order.CancelMessage;
                vm_ListAdd.CancelTime = model_Order.CancelTime;
                vm_ListAdd.Remark = model_Order.Remark;
            }
            else
            {
                vm_ListAdd.Status = 0;
                vm_ListAdd.UserDeleted = 0;
            }
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View(vm_ListAdd);
        }

        //保存添改
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RecordListAdd(ListAdd vm_ListAdd)
        {
            var model_Master = _tokenManager.GetAdminInfo();

            string message = "[" + DateTime.Now + "]" + model_Master.Name + _localizer["修改信息"] + "";
            var model_Order = _context.Queryable<WebOrder>().InSingle(vm_ListAdd.Orderid);
            if (model_Order != null)
            {
                if (model_Order.Status != vm_ListAdd.Status)
                {
                    if (vm_ListAdd.Status == 1 && string.IsNullOrEmpty(vm_ListAdd.Message))
                    {
                        return Content("<script>alert('" + _localizer["手工修改订单为已付款，需要在【操作日志】处填写原因"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
                    }
                    message += ",将状态由[" + model_Order.Status + "]改为[" + vm_ListAdd.Status + "]";
                }
                if (model_Order.Payment != vm_ListAdd.Payment)
                {
                    message += ",将实付款由[" + model_Order.Payment + "]改为[" + vm_ListAdd.Payment + "]";
                }
                if (model_Order.CurrencyCode != vm_ListAdd.CurrencyCode)
                {
                    message += ",将支付币种由[" + model_Order.CurrencyCode + "]改为[" + vm_ListAdd.CurrencyCode + "]";
                }
                if (model_Order.TransactionId != vm_ListAdd.TransactionId)
                {
                    message += ",将支付流水号由[" + model_Order.TransactionId + "]改为[" + vm_ListAdd.TransactionId + "]";
                }
                if (model_Order.TransactionId != vm_ListAdd.TransactionId)
                {
                    message += ",将支付流水号由[" + model_Order.TransactionId + "]改为[" + vm_ListAdd.TransactionId + "]";
                }
                if (!string.IsNullOrEmpty(vm_ListAdd.Message))
                {
                    message += ",添加日志:" + vm_ListAdd.Message;
                }
                model_Order.CurrencyCode = vm_ListAdd.CurrencyCode;
                model_Order.Payment = vm_ListAdd.Payment;
                model_Order.TransactionId = vm_ListAdd.TransactionId;
                model_Order.Status = vm_ListAdd.Status;
                model_Order.Remark = message + "<hr>" + model_Order.Remark;

                _context.Updateable(model_Order).ExecuteCommand();

                return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改订单"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
            else
            {
                return Content("<script>alert('" + _localizer["不允许后台创建订单!"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
        }
		#endregion
		#endregion


		#region "录播课程订单"
		#region "订单列表"    
		[Authorization(Power = "Main")]
		public IActionResult OfflineList()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> OfflineListData(string keys = "", string status = "", string user_deleted = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string Orderids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Orderids");
			long Orderid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Orderid", 0);
			var list_Order = new List<WebOrder>();
			if (Orderids.Trim() == "") Orderids = "0";
			string[] arr = Orderids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebOrder>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Orderid) || u.Orderid == Orderid)
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
					var exp = new Expressionable<WebOrder>();
					exp.And(u => u.Status != -1 && u.Type == 3);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.MergeString(u.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(u.OrderSn, "").Contains(keys.Trim()) ||
					SqlFunc.Subqueryable<WebUser>().Where(s => s.Userid == u.Userid && (s.Email.Contains(keys.Trim()) || s.Mobile.Contains(keys.Trim()) || SqlFunc.MergeString(s.FirstName, s.LastName).Contains(keys.Trim()))).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(user_deleted), u => u.UserDeleted.ToString() == user_deleted.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebOrder>().Where(exp.ToExpression());
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
					query.OrderBy("Orderid desc");
					list_Order = query.ToPageList(page, pagesize, ref total);
					var list_Course = _context.Queryable<WebOfflineCourse>().Where(u => list_Order.Select(s => s.OfflineCourseid).Contains(u.OfflineCourseid)).ToList();
					var list_User = _context.Queryable<WebUser>().Where(u => list_Order.Select(s => s.Userid).Contains(u.Userid)).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Order.Count; i++)
					{
						var model_Course = list_Course.FirstOrDefault(u => u.OfflineCourseid == list_Order[i].OfflineCourseid);
						var model_User = list_User.FirstOrDefault(u => u.Userid == list_Order[i].Userid);
						str.Append("{");
						str.Append("\"Orderid\":\"" + list_Order[i].Orderid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(list_Order[i].Title + "") + "\",");
						str.Append("\"Img\":\"" + JsonHelper.JsonCharFilter(model_Course?.Img + "") + "\",");
						str.Append("\"OrderSn\":\"" + list_Order[i].OrderSn + "\",");
						str.Append("\"LessonCount\":\"" + model_Course?.LessonCount + "\",");
						str.Append("\"LessonStart\":\"" + model_Course?.LessonStart + "\",");
						if (model_User != null)
						{
							str.Append("\"Name\":\"" + model_User.FirstName + " " + model_User.LastName + "\",");
							str.Append("\"Mobile\":\"" + model_User.MobileCode + " " + model_User.Mobile + "\",");
							str.Append("\"Email\":\"" + model_User.Email + "\",");
						}
						str.Append("\"Dtime\":\"" + list_Order[i].Dtime.ToString() + "\",");
						str.Append("\"Amount\":\"" + list_Order[i].Amount + "\",");
						str.Append("\"UserDeleted\":" + list_Order[i].UserDeleted + ",");
						str.Append("\"CurrencyCode\":\"" + list_Order[i].CurrencyCode + "\",");
						str.Append("\"Status\":" + list_Order[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Order.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
		#endregion

		#region "订单查阅"      
		[Authorization(Power = "Main")]
		public async Task<IActionResult> OfflineListAdd(long Orderid = 0)
		{
			var vm_ListAdd = new ListAdd();
			vm_ListAdd.CurrencyCodeItem = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList().Select(u => new SelectListItem(u.CurrencyCode, u.CurrencyCode)).ToList();
			var model_Order = _context.Queryable<WebOrder>().InSingle(Orderid);
			if (model_Order != null)
			{
				vm_ListAdd.Orderid = Orderid;
				vm_ListAdd.OrderSn = model_Order.OrderSn;
				vm_ListAdd.Title = model_Order.Title;
				var model_User = _context.Queryable<WebUser>().InSingle(model_Order.Userid);
				if (model_User != null)
				{
					vm_ListAdd.Email = model_User.Email;
					vm_ListAdd.Name = model_User.FirstName + " " + model_User.LastName;
					vm_ListAdd.Mobile = model_User.MobileCode + model_User.Mobile;
				}
				var model_Course = _context.Queryable<WebOfflineCourse>().InSingle(model_Order.OfflineCourseid);
				if (model_Course != null)
				{
					vm_ListAdd.Img = model_Course.Img;
					vm_ListAdd.LessonCount = model_Course.LessonCount;
					vm_ListAdd.LessonStart = model_Course.LessonStart;
				}
				vm_ListAdd.Ip = model_Order.Ip;
				vm_ListAdd.CountryCode = model_Order.CountryCode;
				vm_ListAdd.Amount = model_Order.Amount;
				vm_ListAdd.CurrencyCode = model_Order.CurrencyCode;

				vm_ListAdd.Paytime = model_Order.Paytime;
				vm_ListAdd.Payment = model_Order.Payment;
				var model_Paytype = _context.Queryable<WebPaytype>().InSingle(model_Order.Paytypeid);
				if (model_Paytype != null)
				{
					vm_ListAdd.PayTitle = model_Paytype.Title;
				}
				vm_ListAdd.TransactionId = model_Order.TransactionId;
				vm_ListAdd.Dtime = model_Order.Dtime;
				vm_ListAdd.Status = model_Order.Status;
				vm_ListAdd.UserDeleted = model_Order.UserDeleted;
				vm_ListAdd.CancelMessage = model_Order.CancelMessage;
				vm_ListAdd.CancelTime = model_Order.CancelTime;
				vm_ListAdd.Remark = model_Order.Remark;
			}
			else
			{
				vm_ListAdd.Status = 0;
				vm_ListAdd.UserDeleted = 0;
			}
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> OfflineListAdd(ListAdd vm_ListAdd)
		{
			var model_Master = _tokenManager.GetAdminInfo();

			string message = "[" + DateTime.Now + "]" + model_Master.Name + _localizer["修改信息"] + "";
			var model_Order = _context.Queryable<WebOrder>().InSingle(vm_ListAdd.Orderid);
			if (model_Order != null)
			{
				if (model_Order.Status != vm_ListAdd.Status)
				{
					if (vm_ListAdd.Status == 1 && string.IsNullOrEmpty(vm_ListAdd.Message))
					{
						return Content("<script>alert('" + _localizer["手工修改订单为已付款，需要在【操作日志】处填写原因"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
					}
					message += ",将状态由[" + model_Order.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (model_Order.Payment != vm_ListAdd.Payment)
				{
					message += ",将实付款由[" + model_Order.Payment + "]改为[" + vm_ListAdd.Payment + "]";
				}
				if (model_Order.CurrencyCode != vm_ListAdd.CurrencyCode)
				{
					message += ",将支付币种由[" + model_Order.CurrencyCode + "]改为[" + vm_ListAdd.CurrencyCode + "]";
				}
				if (model_Order.TransactionId != vm_ListAdd.TransactionId)
				{
					message += ",将支付流水号由[" + model_Order.TransactionId + "]改为[" + vm_ListAdd.TransactionId + "]";
				}
				if (model_Order.TransactionId != vm_ListAdd.TransactionId)
				{
					message += ",将支付流水号由[" + model_Order.TransactionId + "]改为[" + vm_ListAdd.TransactionId + "]";
				}
				if (!string.IsNullOrEmpty(vm_ListAdd.Message))
				{
					message += ",添加日志:" + vm_ListAdd.Message;
				}
				model_Order.CurrencyCode = vm_ListAdd.CurrencyCode;
				model_Order.Payment = vm_ListAdd.Payment;
				model_Order.TransactionId = vm_ListAdd.TransactionId;
				model_Order.Status = vm_ListAdd.Status;
				model_Order.Remark = message + "<hr>" + model_Order.Remark;

				_context.Updateable(model_Order).ExecuteCommand();

				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改订单"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				return Content("<script>alert('" + _localizer["不允许后台创建订单!"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
		#endregion
	}
}