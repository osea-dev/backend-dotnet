using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
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
using WeTalk.Interfaces.Base;
using WeTalk.Models;
using WeTalk.Web.Extensions;
using WeTalk.Web.ViewModels.Course;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class CourseController : Base.BaseController
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly TokenManager _tokenManager;

		private readonly ILogger<CourseController> _logger;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		private readonly ICommonBaseService _commonBaseService;
		public CourseController(IConfiguration config, IHttpContextAccessor accessor,SqlSugarScope dbcontext, TokenManager tokenManager, ILogger<CourseController> logger,
			 IStringLocalizer<LangResource> localizer, ICommonBaseService commonBaseService)
			: base(tokenManager)
		{
			_context = dbcontext;
			_accessor = accessor;
			_logger = logger;
			_localizer = localizer;
			_tokenManager = tokenManager;
			_commonBaseService = commonBaseService;
		}

        #region "课程列表"
        [Authorization(Power = "Main")]
        public IActionResult List()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListData(string keys = "", long categoryid = 0, string status = "", string recommend = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string Courseids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Courseids");
			long Courseid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Courseid", 0);
			var list_Course = new List<WebCourse>();
			if (Courseids.Trim() == "") Courseids = "0";
			string[] arr = Courseids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebCourse>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Courseid) || u.Courseid == Courseid)
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
					count = _context.Updateable<WebCourse>()
						.SetColumns(u => u.Status == 1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Courseid) || u.Courseid == Courseid)
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
					count = _context.Updateable<WebCourse>()
						.SetColumns(u => u.Status == 0)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Courseid) || u.Courseid == Courseid)
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
					var exp = new Expressionable<WebCourse>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebCourseLang>().Where(s=>s.Courseid==u.Courseid && SqlFunc.MergeString(s.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.Message, "").Contains(keys.Trim())).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Sendtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Sendtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebCourse>().Where(exp.ToExpression());
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
					query.OrderBy("sort,sendtime desc,Courseid desc");
					list_Course = query.ToPageList(page, pagesize, ref total);
					var list_CourseLang = _context.Queryable<WebCourseLang>().Where(u => list_Course.Select(s => s.Courseid).Contains(u.Courseid) && u.Lang == Lang).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Course.Count; i++)
					{
						var model_CourseLang = list_CourseLang.FirstOrDefault(u => u.Courseid == list_Course[i].Courseid);
						str.Append("{");
						str.Append("\"Courseid\":\"" + list_Course[i].Courseid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_CourseLang?.Title + "") + "\",");
						str.Append("\"Img\":\"" + list_Course[i].Img + "\",");
						str.Append("\"Dtime\":\"" + list_Course[i].Dtime.ToString() + "\",");
						str.Append("\"Sort\":\"" + list_Course[i].Sort + "\",");
						str.Append("\"Sendtime\":\"" + list_Course[i].Sendtime + "\",");
						str.Append("\"Status\":" + list_Course[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Course.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "课程修改"
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListAdd(long Courseid = 0)
		{
			var vm_ListAdd = new ViewModels.Course.ListAdd();
			
			var model_Course = _context.Queryable<WebCourse>().InSingle(Courseid);
			if (model_Course != null)
			{
				var model_CourseLang = _context.Queryable<WebCourseLang>().First(u => u.Courseid == Courseid && u.Lang == Lang);
				if (model_CourseLang != null) {
					vm_ListAdd.Title = model_CourseLang.Title;
					vm_ListAdd.Message = model_CourseLang.Message;
					vm_ListAdd.Curricula = model_CourseLang.Curricula;
					vm_ListAdd.Keys = model_CourseLang.Keys;
				}

				vm_ListAdd.Courseid = Courseid;
				vm_ListAdd.Img = model_Course.Img;
				vm_ListAdd.Banner = model_Course.Banner;
				vm_ListAdd.BannerH5 = model_Course.BannerH5;
				vm_ListAdd.Keysids = model_Course.Keysids;
				vm_ListAdd.Hits = model_Course.Hits;

				vm_ListAdd.Sort = model_Course.Sort;
				vm_ListAdd.Dtime = model_Course.Dtime;
				vm_ListAdd.Status = model_Course.Status;
				vm_ListAdd.Sendtime = model_Course.Sendtime;
				vm_ListAdd.Remark = model_Course.Remark;
			}
			else
			{
				vm_ListAdd.Status = 1;
				vm_ListAdd.Sort = 100;
				vm_ListAdd.Sendtime = DateTime.Now;
			}
			vm_ListAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListAdd(ListAdd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/Course/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			int chk_thumb = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_thumb", 0);
			int chk_banner = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner", 0);
			int chk_banner_h5 = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner_h5", 0);
			var old_imgs = GSRequestHelper.GetString(_accessor.HttpContext.Request, "input_old_imgs").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
			var files = _accessor.HttpContext.Request.Form.Files;
			var list_imgs = new List<string>();
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext = "";//扩展名
			var model_master = _tokenManager.GetAdminInfo();
			string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改课程信息";
			var model_Course = _context.Queryable<WebCourse>().InSingle(vm_ListAdd.Courseid);
			if (model_Course != null)
			{
				var model_CourseLang = _context.Queryable<WebCourseLang>().First(u => u.Lang== vm_ListAdd.Lang && u.Courseid==vm_ListAdd.Courseid);
				if (model_CourseLang?.Title != vm_ListAdd.Title)
				{
					message += ",将课程名称由[" + model_CourseLang?.Title + "]改为[" + vm_ListAdd.Title + "]";
				}
				if (model_CourseLang?.Message != vm_ListAdd.Message)
				{
					message += ",将简介由[" + model_CourseLang?.Message + "]改为[" + vm_ListAdd.Message + "]";
				}
				if (model_Course.Sort != vm_ListAdd.Sort)
				{
					message += ",将排序字段由[" + model_Course.Sort + "]改为[" + vm_ListAdd.Sort + "]";
				}
				if (model_Course.Sendtime != vm_ListAdd.Sendtime)
				{
					message += ",将发布时间由[" + model_Course.Sendtime + "]改为[" + vm_ListAdd.Sendtime + "]";
				}
				if (model_Course.Status != vm_ListAdd.Status)
				{
					message += ",将状态由[" + model_Course.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (model_CourseLang != null)
				{
					model_CourseLang.Title = vm_ListAdd.Title;
					model_CourseLang.Keys = vm_ListAdd.Keys;
					model_CourseLang.Message = vm_ListAdd.Message;
					model_CourseLang.Curricula = vm_ListAdd.Curricula;
					_context.Updateable(model_CourseLang).ExecuteCommand();
				}
				else {
					model_CourseLang = new WebCourseLang();
					model_CourseLang.Courseid = vm_ListAdd.Courseid;
					model_CourseLang.Title = vm_ListAdd.Title;
					model_CourseLang.Keys = vm_ListAdd.Keys;
					model_CourseLang.Message = vm_ListAdd.Message;
					model_CourseLang.Curricula = vm_ListAdd.Curricula;
					model_CourseLang.Lang = vm_ListAdd.Lang;
					_context.Insertable(model_CourseLang).ExecuteCommand();
				}
				model_Course.Keysids = vm_ListAdd.Keysids;
				model_Course.Sort = vm_ListAdd.Sort;
				model_Course.Sendtime = vm_ListAdd.Sendtime;
				model_Course.Status = vm_ListAdd.Status;
				model_Course.Masterid = model_master.AdminMasterid;
				model_Course.Remark = message + "<hr>" + vm_ListAdd.Remark;
				#region "上传"
				if (chk_thumb == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Course.Img)) FileHelper.FileDel(ImgRoot + model_Course.Img);
					}
					catch { }
					model_Course.Img = "";
				}
				if (chk_banner == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Course.Banner)) FileHelper.FileDel(ImgRoot + model_Course.Banner);
					}
					catch { }
					model_Course.Banner = "";
				}
				if (chk_banner_h5 == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Course.BannerH5)) FileHelper.FileDel(ImgRoot + model_Course.BannerH5);
					}
					catch { }
					model_Course.BannerH5 = "";
				}
				if (files.Count > 0)
				{
					//缩略图
					var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Course.Img)) FileHelper.FileDel(ImgRoot + model_Course.Img);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.Img = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Course.Banner)) FileHelper.FileDel(ImgRoot + model_Course.Banner);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.Banner = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Course.BannerH5)) FileHelper.FileDel(ImgRoot + model_Course.BannerH5);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.BannerH5 = fileurl + filename;
						}
					}
				}
				#endregion
				_context.Updateable(model_Course).ExecuteCommand();

				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?Courseid=" + vm_ListAdd.Courseid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_Course = new WebCourse();
				model_Course.Keysids = vm_ListAdd.Keysids;
				model_Course.Sort = vm_ListAdd.Sort;
				model_Course.Sendtime = vm_ListAdd.Sendtime;
				model_Course.Status = vm_ListAdd.Status;
				model_Course.Masterid = model_master.AdminMasterid;
				model_Course.Remark = message + "<hr>" + vm_ListAdd.Remark;
				#region "上传"
				if (files.Count > 0)
				{
					var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.Img = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.Banner = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.BannerH5 = fileurl + filename;
						}
					}
				}
				#endregion
				vm_ListAdd.Courseid = _context.Insertable(model_Course).ExecuteReturnBigIdentity();

				var model_CourseLang = new WebCourseLang();
				model_CourseLang.Courseid = vm_ListAdd.Courseid;
				model_CourseLang.Title = vm_ListAdd.Title;
				model_CourseLang.Keys = vm_ListAdd.Keys;
				model_CourseLang.Message = vm_ListAdd.Message;
				model_CourseLang.Curricula = vm_ListAdd.Curricula;
				_context.Insertable(model_CourseLang).ExecuteCommand();

				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?Courseid=" + vm_ListAdd.Courseid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion

		#region "课程Sku子列表"
		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListSubData()
		{
			StringBuilder str = new StringBuilder();
			long Courseid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Courseid", 0);
			long CourseSkuid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "CourseSkuid", 0);
			var list_CourseSku = new List<WebCourseSku>();
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebCourseSku>()
						.SetColumns(u => u.Status == -1)
						.Where(u => u.CourseSkuid == CourseSkuid)
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
					count = _context.Updateable<WebCourseSku>()
						.SetColumns(u => u.Status == 1)
						.Where(u => u.CourseSkuid == CourseSkuid)
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
					count = _context.Updateable<WebCourseSku>()
						.SetColumns(u => u.Status == 0)
						.Where(u => u.CourseSkuid == CourseSkuid)
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
					var exp = new Expressionable<WebCourseSku>();
					exp.And(u => u.Status != -1 && u.Courseid == Courseid);
					list_CourseSku = _context.Queryable<WebCourseSku>().Where(exp.ToExpression()).OrderBy("sort,sku_typeid,class_hour").ToList();
					var list_CoursePrice = _context.Queryable<WebCourseSkuPrice>().LeftJoin<PubCurrency>((l,r)=>l.PubCurrencyid==r.PubCurrencyid).Where((l,r) => list_CourseSku.Select(s => s.CourseSkuid).Contains(l.CourseSkuid)).OrderBy((l,r)=>r.Sort).ToList();
					var list_SkuType = _context.Queryable<WebSkuTypeLang>().Where(u =>u.Lang==Lang && list_CourseSku.Select(s => s.SkuTypeid).Contains(u.SkuTypeid)).ToList();
					//var list_MenkeCourse = _context.Queryable<MenkeCourse>().Where(u => list_CourseSku.Select(s => s.MenkeCourseId).Contains(u.MenkeCourseId)).ToList();
					str.Append("[");
					string PriceList = "";
					for (int i = 0; i < list_CourseSku.Count; i++)
					{
						PriceList = "";
                        PriceList += "<table style=\"width:100%\">";
						list_CoursePrice.Where(u => u.CourseSkuid == list_CourseSku[i].CourseSkuid).ToList().ForEach(model =>
						{
							var discount = (model.MarketPrice==0?"0":(model.Price / model.MarketPrice).ToString("0.00"));
                            PriceList += $"<tr><td>{_localizer["原价"]}{model.CurrencyCode} : {model.MarketPrice}</td><td>{discount}</td><td>{_localizer["售价"]}{model.CurrencyCode} : {model.Price}</td></tr>";
						});
						var discount= (list_CourseSku[i].MarketPrice == 0 ? "0" : (list_CourseSku[i].Price / list_CourseSku[i].MarketPrice).ToString("0.00"));
						PriceList += $"<tr><td>{_localizer["原价"]}USD : {list_CourseSku[i].MarketPrice}</td><td>{discount}</td><td>{_localizer["售价"]}USD : {list_CourseSku[i].Price}</td></tr>";
						PriceList += "</table>";
						var model_SkuType = list_SkuType.FirstOrDefault(u => u.SkuTypeid == list_CourseSku[i].SkuTypeid);
						//var model_MenkeCourse = list_MenkeCourse.FirstOrDefault(u => u.MenkeCourseId == list_CourseSku[i].MenkeCourseId);

                        str.Append("{");
						str.Append("\"CourseSkuid\":\"" + list_CourseSku[i].CourseSkuid + "\",");
						//str.Append("\"MenkeCourseId\":\"" + list_CourseSku[i].MenkeCourseId + "\",");
						//str.Append("\"MenkeCourseName\":\"" + model_MenkeCourse?.MenkeName + "\",");
						str.Append("\"Type\":\"" + model_SkuType?.Title + "\",");
						str.Append("\"ClassHour\":\"" + list_CourseSku[i].ClassHour + "\",");
						//str.Append("\"Price\":\"" + list_CourseSku[i].Price + "\",");
						//str.Append("\"MarketPrice\":\"" + list_CourseSku[i].MarketPrice + "\",");
						str.Append("\"PriceList\":\"" + JsonHelper.JsonCharFilter(PriceList) + "\",");
						str.Append("\"Dtime\":\"" + list_CourseSku[i].Dtime.ToString() + "\",");
						str.Append("\"Sort\":\"" + list_CourseSku[i].Sort + "\",");
						str.Append("\"Status\":" + list_CourseSku[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_CourseSku.Count - 1)) str.Append(",");
					}
					str.Append("]");
					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "课程Sku修改"
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListSubAdd(long Courseid = 0, long CourseSkuid = 0)
		{
			var vm_ListSubAdd = new ViewModels.Course.ListSubAdd();
			var url = Appsettings.app("Web:menke_course");

			vm_ListSubAdd.ListPubCurrency = new List<PubCurrencyPrice>();
			var list_ViewPubCurrency = _context.Queryable<ViewPubCurrency>().Where(u =>u.Status==1 && u.Isuse == 1 && u.Lang == Lang).OrderBy(u => u.Sort).OrderBy(u=>u.PubCurrencyid,OrderByType.Desc).ToList();
			var list_SkuPrice = _context.Queryable<WebCourseSkuPrice>().Where(u => u.CourseSkuid == CourseSkuid && list_ViewPubCurrency.Select(s => s.PubCurrencyid).Contains(u.PubCurrencyid)).ToList();
			list_ViewPubCurrency.ForEach(model =>
			{
				var model_SkuPrice = list_SkuPrice.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
				vm_ListSubAdd.ListPubCurrency.Add(new PubCurrencyPrice()
				{
					PubCurrencyid = model.PubCurrencyid,
					Country = model.Country,
					CountryCode = model.CountryCode,
					CurrencyCode = model.CurrencyCode,
					Price = (model_SkuPrice!=null)?model_SkuPrice.Price:0,
					MarketPrice = (model_SkuPrice != null) ? model_SkuPrice.MarketPrice : 0
				});
			});

			vm_ListSubAdd.MenkeCourseItem = _context.Queryable<MenkeCourse>().Where(u=>u.Istrial==0 && u.Status==1 && u.MenkeDeleteTime==0).OrderBy(u=>u.MenkeUpdateTime,OrderByType.Desc).ToList().Select(u=>new SelectListItem(u.MenkeName, u.MenkeCourseId.ToString())).ToList();
			vm_ListSubAdd.SkuTypeItem = _context.Queryable<WebSkuType, WebSkuTypeLang>((l, r) => l.SkuTypeid == r.SkuTypeid).Where((l, r) => r.Lang == Lang).Select((l, r) => new { l.SkuTypeid, r.Title })
				.ToList().Select(u => new SelectListItem(u.Title, u.SkuTypeid.ToString())).ToList();
			var model_CourseSku = _context.Queryable<WebCourseSku>().InSingle(CourseSkuid);
			if (model_CourseSku != null)
			{
				vm_ListSubAdd.CourseSkuid = model_CourseSku.CourseSkuid;
				//vm_ListSubAdd.MenkeCourseId = model_CourseSku.MenkeCourseId;
				vm_ListSubAdd.SkuTypeid = model_CourseSku.SkuTypeid;
				vm_ListSubAdd.ClassHour = model_CourseSku.ClassHour;
				vm_ListSubAdd.Price = model_CourseSku.Price;
				vm_ListSubAdd.MarketPrice = model_CourseSku.MarketPrice;
				vm_ListSubAdd.Sort = model_CourseSku.Sort;
				vm_ListSubAdd.Dtime = model_CourseSku.Dtime;
				vm_ListSubAdd.Status = model_CourseSku.Status;
				vm_ListSubAdd.Remark = model_CourseSku.Remark;
				vm_ListSubAdd.Courseid = model_CourseSku.Courseid;
			}
			else
			{
				vm_ListSubAdd.Status = 1;
				vm_ListSubAdd.Sort = 100;
			}
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListSubAdd);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListSubAdd(ListSubAdd vm_ListSubAdd)
		{
			//if (vm_ListSubAdd.MenkeCourseId <= 0) {
			//	return Content("<script>alert('"+ _localizer["请选择[拓课云]关联课程"] +"')</script>", "text/html", Encoding.GetEncoding("utf-8"));
			//}
			if (_context.Queryable<WebCourseSku>().Any(u=>u.Status==1 && u.SkuTypeid==vm_ListSubAdd.SkuTypeid && u.ClassHour==vm_ListSubAdd.ClassHour && u.Courseid==vm_ListSubAdd.Courseid && u.CourseSkuid!=vm_ListSubAdd.CourseSkuid))
			{
				return Content("<script>alert('" + _localizer["不允许重复发布相同参数的SKU"] + "')</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			var model_master = _tokenManager.GetAdminInfo();
			string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改课程信息";
			var list_Currency = _context.Queryable<PubCurrency>().Where(u=>u.Status==1).ToList();
			var list_SkuPrice = new List<WebCourseSkuPrice>();
			var model_CourseSku = _context.Queryable<WebCourseSku>().InSingle(vm_ListSubAdd.CourseSkuid);
			if (model_CourseSku != null)
			{
				//if (model_CourseSku.MenkeCourseId != vm_ListSubAdd.MenkeCourseId)
				//{
				//	message += ",将关联课程ID由[" + model_CourseSku.MenkeCourseId + "]改为[" + vm_ListSubAdd.MenkeCourseId + "]";
				//}
				if (model_CourseSku.SkuTypeid != vm_ListSubAdd.SkuTypeid)
				{
					message += ",将上课方式由[" + model_CourseSku.SkuTypeid + "]改为[" + vm_ListSubAdd.SkuTypeid + "]";
				}
				if (model_CourseSku.ClassHour != vm_ListSubAdd.ClassHour)
				{
					message += ",将课时由[" + model_CourseSku.ClassHour + "]改为[" + vm_ListSubAdd.ClassHour + "]";
				}
				//if (model_CourseSku.Price != vm_ListSubAdd.Price)
				//{
				//	message += ",将价格由[" + model_CourseSku.Price + "]改为[" + vm_ListSubAdd.Price + "]";
				//}
				//if (model_CourseSku.Discount != vm_ListSubAdd.Discount)
				//{
				//	message += ",将折扣由[" + model_CourseSku.Discount + "]改为[" + vm_ListSubAdd.Discount + "]";
				//}
				if (model_CourseSku.Sort != vm_ListSubAdd.Sort)
				{
					message += ",将排序字段由[" + model_CourseSku.Sort + "]改为[" + vm_ListSubAdd.Sort + "]";
				}
				if (model_CourseSku.Status != vm_ListSubAdd.Status)
				{
					message += ",将状态由[" + model_CourseSku.Status + "]改为[" + vm_ListSubAdd.Status + "]";
				}
				list_SkuPrice = _context.Queryable<WebCourseSkuPrice>().Where(u => u.CourseSkuid == vm_ListSubAdd.CourseSkuid).ToList();
				foreach (var model in list_Currency) {
					var model_SkuPrice = list_SkuPrice.FirstOrDefault(u=>u.PubCurrencyid == model.PubCurrencyid);
					if (model_SkuPrice != null)
					{
						model_SkuPrice.Courseid= vm_ListSubAdd.Courseid;
						model_SkuPrice.Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_"+model.PubCurrencyid, 0.00M);
						model_SkuPrice.MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M);
						model_SkuPrice.CountryCode = model.CountryCode;
						model_SkuPrice.CurrencyCode = model.CurrencyCode;
					}
					else {
						list_SkuPrice.Add(new WebCourseSkuPrice()
						{
							Courseid = vm_ListSubAdd.Courseid,
							CourseSkuid = vm_ListSubAdd.CourseSkuid,
							PubCurrencyid = model.PubCurrencyid,
							Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M),
							MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M),
							CountryCode = model.CountryCode,
							CurrencyCode = model.CurrencyCode
						});
					}
				}
				var x = _context.Storageable(list_SkuPrice).ToStorage();
				x.AsInsertable.ExecuteCommand();
				x.AsUpdateable.ExecuteCommand();

				model_CourseSku.SkuTypeid = vm_ListSubAdd.SkuTypeid;
				model_CourseSku.ClassHour = vm_ListSubAdd.ClassHour;
				//model_CourseSku.MenkeCourseId = vm_ListSubAdd.MenkeCourseId;
				model_CourseSku.ClassHour = vm_ListSubAdd.ClassHour;
				model_CourseSku.Price = vm_ListSubAdd.Price;
				model_CourseSku.MarketPrice = vm_ListSubAdd.MarketPrice;
				model_CourseSku.Sort = vm_ListSubAdd.Sort;
				model_CourseSku.Status = vm_ListSubAdd.Status;
				model_CourseSku.Remark = message + "<hr>" + vm_ListSubAdd.Remark;
			
				_context.Updateable(model_CourseSku).ExecuteCommand();

				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改课程SKU"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_CourseSku = new WebCourseSku();

				model_CourseSku.Courseid = vm_ListSubAdd.Courseid;
				model_CourseSku.SkuTypeid = vm_ListSubAdd.SkuTypeid;
				model_CourseSku.ClassHour = vm_ListSubAdd.ClassHour;
				//model_CourseSku.MenkeCourseId = vm_ListSubAdd.MenkeCourseId;
				model_CourseSku.ClassHour = vm_ListSubAdd.ClassHour;
				model_CourseSku.Price = vm_ListSubAdd.Price;
				model_CourseSku.MarketPrice = vm_ListSubAdd.MarketPrice;
				model_CourseSku.Sort = vm_ListSubAdd.Sort;
				model_CourseSku.Status = vm_ListSubAdd.Status;
				model_CourseSku.Remark = message;
				vm_ListSubAdd.CourseSkuid = _context.Insertable(model_CourseSku).ExecuteReturnBigIdentity();

				list_SkuPrice = _context.Queryable<WebCourseSkuPrice>().Where(u => u.CourseSkuid == vm_ListSubAdd.CourseSkuid).ToList();
				foreach (var model in list_Currency)
				{
					var model_SkuPrice = list_SkuPrice.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
					if (model_SkuPrice != null)
					{
						model_SkuPrice.Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M);
						model_SkuPrice.MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M);
						model_SkuPrice.CountryCode = model.CountryCode;
						model_SkuPrice.CurrencyCode = model.CurrencyCode;
					}
					else
					{
						list_SkuPrice.Add(new WebCourseSkuPrice()
						{
							CourseSkuid = vm_ListSubAdd.CourseSkuid,
							PubCurrencyid = model.PubCurrencyid,
							Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M),
							MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M),
							CountryCode = model.CountryCode,
							CurrencyCode = model.CurrencyCode
						});
					}
				}
				var x = _context.Storageable(list_SkuPrice).ToStorage();
				x.AsInsertable.ExecuteCommand();
				x.AsUpdateable.ExecuteCommand();

				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加课程SKU"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion

        #region "课程组列表"
        [Authorization(Power = "Main")]
        public IActionResult CourseGroup(long Courseid)
		{
			ViewBag.Courseid = Courseid;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CourseGroupData(long Courseid, string keys = "", string type = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string CourseGroupids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "CourseGroupids");
			long CourseGroupid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "CourseGroupid", 0);
			List<WebCourseGroup> list_CourseGroup = new List<WebCourseGroup>();
			if (CourseGroupids.Trim() == "") CourseGroupids = "0";
			string[] arr = CourseGroupids.Split(',');

			int total = 0, count = 0;
			var exp = new Expressionable<WebCourseGroup>();
			string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
			string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebCourseGroup>()
						.SetColumns(u => u.Status == -1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.CourseGroupid) || u.CourseGroupid == CourseGroupid))
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
					count = _context.Updateable<WebCourseGroup>()
						.SetColumns(u => u.Status == 1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.CourseGroupid) || u.CourseGroupid == CourseGroupid))
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
					count = _context.Updateable<WebCourseGroup>()
						.SetColumns(u => u.Status == 0)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.CourseGroupid) || u.CourseGroupid == CourseGroupid))
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
					exp.And(u => u.Status != -1 && u.Courseid == Courseid);
					if (!string.IsNullOrEmpty(keys))
					{
						exp.And(u => SqlFunc.Subqueryable<WebCourseGroupLang>().Where(s=>s.CourseGroupid==u.CourseGroupid && SqlFunc.MergeString(s.GroupName, "").Contains(keys.Trim())).Any());
					}
					if (!string.IsNullOrEmpty(status))
					{
						exp.And(u => u.Status.ToString() == status.Trim());
					}
					if (CourseGroupid > 0)
					{
						exp.And(u => u.CourseGroupid == CourseGroupid);
					}
					if (!string.IsNullOrEmpty(begintime))
					{
						exp.And(u => u.Dtime >= DateTime.Parse(begintime));
					}
					if (!string.IsNullOrEmpty(endtime))
					{
						exp.And(u => u.Dtime <= DateTime.Parse(endtime));
					}
					var query = _context.Queryable<WebCourseGroup>().Where(exp.ToExpression());
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
					query.OrderBy("sort,course_groupid desc");
					list_CourseGroup = query.Where(exp.ToExpression()).ToPageList(page, pagesize, ref total);
					var list_CourseGroupLang = _context.Queryable<WebCourseGroupLang>().Where(u => list_CourseGroup.Select(s => s.CourseGroupid).Contains(u.CourseGroupid) && u.Lang == Lang).ToList();
					var list_Course = _context.Queryable<WebCourse,WebCourseLang>((l,r)=>l.Courseid==r.Courseid).Where((l,r) => list_CourseGroup.Select(s => s.Courseid).Contains(l.Courseid))
						.Select((l,r)=>new { l.Courseid, r.Title,r.Lang}).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_CourseGroup.Count; i++)
					{
						var model_Course = list_Course.FirstOrDefault(u => u.Courseid == list_CourseGroup[i].Courseid);
						var model_CourseGroupLang = list_CourseGroupLang.FirstOrDefault(u => u.CourseGroupid == list_CourseGroup[i].CourseGroupid);
						str.Append("{");
						str.Append("\"CourseGroupid\":\"" + list_CourseGroup[i].CourseGroupid + "\",");
						str.Append("\"CourseName\":\"" + JsonHelper.JsonCharFilter(model_Course?.Title + "") + "\",");
						str.Append("\"GroupName\":\"" + JsonHelper.JsonCharFilter(model_CourseGroupLang?.GroupName) + "\",");
						str.Append("\"Sendtime\":\"" + list_CourseGroup[i].Sendtime.ToString() + "\",");
						str.Append("\"Dtime\":\"" + list_CourseGroup[i].Dtime.ToString() + "\",");
						str.Append("\"Sort\":\"" + list_CourseGroup[i].Sort + "\",");
						str.Append("\"Status\":\"" + list_CourseGroup[i].Status + "\",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_CourseGroup.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}

        #endregion

        #region "课程组修改"
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CourseGroupAdd(long Courseid, long CourseGroupid = 0)
		{
			var vm_Add = new CourseGroupAdd();
			var model_CourseGroup = _context.Queryable<WebCourseGroup>().InSingle(CourseGroupid);
			if (model_CourseGroup != null)
			{
				var model_Course = _context.Queryable<WebCourse,WebCourseLang>((l,r)=>l.Courseid==r.Courseid)
					.Where((l,r)=>l.Courseid == model_CourseGroup.Courseid && r.Lang==Lang)
					.Select((l,r)=>new { r.Title,l.Courseid})
					.First();
				var model_CourseGroupLang = _context.Queryable<WebCourseGroupLang>().First(u => u.Lang == Lang && u.CourseGroupid == CourseGroupid);

				vm_Add.CourseGroupid = model_CourseGroup.CourseGroupid;
				vm_Add.CourseName = model_Course?.Title;
				vm_Add.GroupName = model_CourseGroupLang?.GroupName;
				vm_Add.Status = model_CourseGroup.Status;
				vm_Add.Dtime = model_CourseGroup.Dtime;
				vm_Add.Sort = model_CourseGroup.Sort;
				vm_Add.Sendtime = model_CourseGroup.Sendtime;
			}
			else
			{
                var model_Course = _context.Queryable<WebCourse, WebCourseLang>((l, r) => l.Courseid == r.Courseid)
                    .Where((l, r) => l.Courseid == Courseid && r.Lang == Lang)
                    .Select((l, r) => new { r.Title, l.Courseid })
                    .First();
                vm_Add.CourseName = model_Course?.Title;
                vm_Add.Sort = 100;
				vm_Add.Status = 1;
				vm_Add.Dtime = DateTime.Now;
				vm_Add.Sendtime = DateTime.Now;
			}
			vm_Add.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_Add);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CourseGroupAdd(ViewModels.Course.CourseGroupAdd vm_Add)
		{
			string message = "[" + DateTime.Now + "]" + _tokenManager.GetAdminInfo()?.Username + "修改课程组";
			var model_CourseGroup = _context.Queryable<WebCourseGroup>().InSingle(vm_Add.CourseGroupid);
			if (model_CourseGroup != null)
			{
				model_CourseGroup.Courseid = vm_Add.Courseid;
				model_CourseGroup.Status = vm_Add.Status;
				model_CourseGroup.Sort = vm_Add.Sort;
				model_CourseGroup.Sendtime = vm_Add.Sendtime;

				var model_Lang = _context.Queryable<WebCourseGroupLang>().First(u => u.Lang == vm_Add.Lang && u.CourseGroupid == vm_Add.CourseGroupid);
				if (model_Lang != null)
				{
					model_Lang.GroupName = vm_Add.GroupName;
					_context.Updateable(model_Lang).ExecuteCommand();
				}
				else {
					model_Lang = new WebCourseGroupLang();
					model_Lang.CourseGroupid = vm_Add.CourseGroupid;
					model_Lang.Lang = vm_Add.Lang;
					model_Lang.GroupName = vm_Add.GroupName;
					_context.Insertable(model_Lang).ExecuteCommand();
				}

				_context.Updateable(model_CourseGroup).ExecuteCommand();
				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改分类"] + "',msg:'" + _localizer["保存分类成功"] + "',timeout:3000,showType:'slide'});location.href='?Courseid=" + vm_Add.Courseid + "&CourseGroupid=" + vm_Add.CourseGroupid + "&lang=" + vm_Add.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_CourseGroup = new WebCourseGroup();
				model_CourseGroup.Courseid = vm_Add.Courseid;
				model_CourseGroup.Status = vm_Add.Status;
				model_CourseGroup.Sort = vm_Add.Sort;
				model_CourseGroup.Sendtime = vm_Add.Sendtime;
				model_CourseGroup.Dtime = DateTime.Now;
				vm_Add.CourseGroupid = _context.Insertable(model_CourseGroup).ExecuteReturnBigIdentity();

				var model_Lang = new WebCourseGroupLang();
				model_Lang.CourseGroupid = vm_Add.CourseGroupid;
				model_Lang.Lang = vm_Add.Lang;
				model_Lang.GroupName = vm_Add.GroupName;
				_context.Insertable(model_Lang).ExecuteCommand();

				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加分类"] + "',msg:'" + _localizer["创建分类成功"] + "',timeout:3000,showType:'slide'});location.href='?Courseid=" + vm_Add.Courseid + "&CourseGroupid=" + vm_Add.CourseGroupid + "&lang=" + vm_Add.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion

        #region "子课程列表"
        [Authorization(Power = "Main")]
        public IActionResult CourseGroupInfo(long Courseid)
		{
			ViewBag.Courseid = Courseid;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CourseGroupInfoData(long Courseid, string keys = "", string type = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string CourseGroupInfoids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "CourseGroupInfoids");
			long CourseGroupInfoid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "CourseGroupInfoid", 0);
			List<WebCourseGroupInfo> list_CourseGroupInfo = new List<WebCourseGroupInfo>();
			if (CourseGroupInfoids.Trim() == "") CourseGroupInfoids = "0";
			string[] arr = CourseGroupInfoids.Split(',');

			int total = 0, count = 0;
			var exp = new Expressionable<WebCourseGroupInfo>();
			string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
			string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebCourseGroupInfo>()
						.SetColumns(u => u.Status == -1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.CourseGroupInfoid) || u.CourseGroupInfoid == CourseGroupInfoid))
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
					count = _context.Updateable<WebCourseGroupInfo>()
						.SetColumns(u => u.Status == 1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.CourseGroupInfoid) || u.CourseGroupInfoid == CourseGroupInfoid))
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
					count = _context.Updateable<WebCourseGroupInfo>()
						.SetColumns(u => u.Status == 0)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.CourseGroupInfoid) || u.CourseGroupInfoid == CourseGroupInfoid))
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
					exp.And(u => u.Status != -1 && u.Courseid == Courseid);
					if (!string.IsNullOrEmpty(keys))
					{
						exp.And(u => SqlFunc.Subqueryable<WebCourseGroupInfoLang>().Where(s=>s.CourseGroupInfoid ==u.CourseGroupInfoid && SqlFunc.MergeString(s.Title, "").Contains(keys.Trim())).Any() || SqlFunc.Subqueryable<WebCourseGroupLang>().Where(s=>s.GroupName.Contains(keys.Trim()) && s.CourseGroupid==u.CourseGroupid).Any());
					}
					if (!string.IsNullOrEmpty(status))
					{
						exp.And(u => u.Status.ToString() == status.Trim());
					}
					if (CourseGroupInfoid > 0)
					{
						exp.And(u => u.CourseGroupInfoid == CourseGroupInfoid);
					}
					if (!string.IsNullOrEmpty(begintime))
					{
						exp.And(u => u.Dtime >= DateTime.Parse(begintime));
					}
					if (!string.IsNullOrEmpty(endtime))
					{
						exp.And(u => u.Dtime <= DateTime.Parse(endtime));
					}
					var query = _context.Queryable<WebCourseGroupInfo>().Where(exp.ToExpression());
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
					query.OrderBy("sort,course_group_infoid desc");
					list_CourseGroupInfo = query.Where(exp.ToExpression()).ToPageList(page, pagesize, ref total);
					var list_CourseGroupInfoLang = _context.Queryable<WebCourseGroupInfoLang>().Where(u => u.Lang == Lang && list_CourseGroupInfo.Select(s => s.CourseGroupInfoid).Contains(u.CourseGroupInfoid)).ToList();
					var list_CourseGroup = _context.Queryable<WebCourseGroupLang>().Where(u =>u.Lang==Lang && list_CourseGroupInfo.Select(s => s.CourseGroupid).Contains(u.CourseGroupid)).ToList();
					var list_Course = _context.Queryable<WebCourseLang>().Where(u => u.Lang == Lang && list_CourseGroupInfo.Select(s => s.Courseid).Contains(u.Courseid)).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_CourseGroupInfo.Count; i++)
					{
						var model_Course = list_Course.FirstOrDefault(u => u.Courseid == list_CourseGroupInfo[i].Courseid);
						var model_CourseGroup = list_CourseGroup.FirstOrDefault(u => u.CourseGroupid == list_CourseGroupInfo[i].CourseGroupid);
						var model_CourseGroupInfo = list_CourseGroupInfoLang.FirstOrDefault(u => u.CourseGroupInfoid == list_CourseGroupInfo[i].CourseGroupInfoid);
						str.Append("{");
						str.Append("\"CourseGroupInfoid\":\"" + list_CourseGroupInfo[i].CourseGroupInfoid + "\",");
						str.Append("\"CourseName\":\"" + JsonHelper.JsonCharFilter(model_Course?.Title + "") + "\",");
						str.Append("\"GroupName\":\"" + JsonHelper.JsonCharFilter(model_CourseGroup?.GroupName+"") + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_CourseGroupInfo?.Title + "") + "\",");
						str.Append("\"Img\":\"" + list_CourseGroupInfo[i].Img + "\",");
						str.Append("\"Sendtime\":\"" + list_CourseGroupInfo[i].Sendtime.ToString() + "\",");
						str.Append("\"Dtime\":\"" + list_CourseGroupInfo[i].Dtime.ToString() + "\",");
						str.Append("\"Sort\":\"" + list_CourseGroupInfo[i].Sort + "\",");
						str.Append("\"Status\":\"" + list_CourseGroupInfo[i].Status + "\",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_CourseGroupInfo.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "子课程修改"  
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CourseGroupInfoAdd(long Courseid, long CourseGroupInfoid = 0)
		{
			var vm_Add = new CourseGroupInfoAdd();
			var list_CourseGroup = _context.Queryable<WebCourseGroup>().Where(u => u.Status == 1 && u.Courseid==Courseid).ToList();
			var list_CourseGroupLang = _context.Queryable<WebCourseGroupLang>().Where(u => list_CourseGroup.Select(s=>s.CourseGroupid).Contains(u.CourseGroupid) && u.Lang == Lang).ToList();
			vm_Add.CourseGroupItem = new List<SelectListItem>();
			list_CourseGroup.ForEach(model => {
				var model_Lang = list_CourseGroupLang.FirstOrDefault(u => u.CourseGroupid == model.CourseGroupid);
				vm_Add.CourseGroupItem.Add(new SelectListItem(model_Lang?.GroupName, model.CourseGroupid.ToString()));
			});
			var model_CourseGroupInfo = _context.Queryable<WebCourseGroupInfo>().InSingle(CourseGroupInfoid);
			if (model_CourseGroupInfo != null)
			{
				var model_Lang = _context.Queryable<WebCourseGroupInfoLang>().First(u => u.CourseGroupInfoid == CourseGroupInfoid && u.Lang == Lang);

				vm_Add.Title = model_Lang?.Title;
				vm_Add.Keys = model_Lang?.Keys;
				vm_Add.Message = model_Lang?.Message;
				vm_Add.CourseGroupInfoid = model_CourseGroupInfo.CourseGroupInfoid;
				vm_Add.CourseGroupid = model_CourseGroupInfo.CourseGroupid;
				vm_Add.Status = model_CourseGroupInfo.Status;
				vm_Add.Dtime = model_CourseGroupInfo.Dtime;
				vm_Add.Sort = model_CourseGroupInfo.Sort;
				vm_Add.Sendtime = model_CourseGroupInfo.Sendtime;
				vm_Add.Img = model_CourseGroupInfo.Img;
				vm_Add.Level = model_CourseGroupInfo.Level;
			}
			else
			{
				vm_Add.Courseid = Courseid;
				vm_Add.Sort = 100;
				vm_Add.Status = 1;
				vm_Add.Dtime = DateTime.Now;
				vm_Add.Sendtime = DateTime.Now;
				vm_Add.Level = "0";
			}
			vm_Add.Lang = Lang;
			var model_Course = _context.Queryable<WebCourseLang>().First(u => u.Lang == Lang && u.Courseid == vm_Add.Courseid );
			vm_Add.CourseName = model_Course?.Title;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_Add);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CourseGroupInfoAdd(CourseGroupInfoAdd vm_Add)
		{
			string message = "[" + DateTime.Now + "]" + _tokenManager.GetAdminInfo()?.Username + "修改子课程";
			string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/Course/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			int chk_img = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_img", 0);
			var files = _accessor.HttpContext.Request.Form.Files;
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext = "";//扩展名
			var model_CourseGroupInfo = _context.Queryable<WebCourseGroupInfo>().InSingle(vm_Add.CourseGroupInfoid);
			if (model_CourseGroupInfo != null)
			{
				var model_Lang = _context.Queryable<WebCourseGroupInfoLang>().First(u => u.CourseGroupInfoid == vm_Add.CourseGroupInfoid && u.Lang == vm_Add.Lang);
				if (model_Lang != null)
				{
					model_Lang.Title = vm_Add.Title;
					model_Lang.Keys = vm_Add.Keys;
					model_Lang.Message = vm_Add.Message;
					_context.Updateable(model_Lang).ExecuteCommand();
				}
				else {
					model_Lang = new WebCourseGroupInfoLang();
					model_Lang.CourseGroupInfoid = vm_Add.CourseGroupInfoid;
					model_Lang.Title = vm_Add.Title;
					model_Lang.Keys = vm_Add.Keys;
					model_Lang.Message = vm_Add.Message;
					model_Lang.Lang = vm_Add.Lang;
					_context.Insertable(model_Lang).ExecuteCommand();
				}
				model_CourseGroupInfo.Courseid = vm_Add.Courseid;
				model_CourseGroupInfo.CourseGroupid = vm_Add.CourseGroupid;
				model_CourseGroupInfo.Status = vm_Add.Status;
				model_CourseGroupInfo.Sort = vm_Add.Sort;
				model_CourseGroupInfo.Sendtime = vm_Add.Sendtime;
				model_CourseGroupInfo.Level = vm_Add.Level;
				#region "上传"
				if (chk_img == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_CourseGroupInfo.Img)) FileHelper.FileDel(ImgRoot + model_CourseGroupInfo.Img);
					}
					catch { }
					model_CourseGroupInfo.Img = "";
				}
				if (files.Count > 0)
				{
					//缩略图
					var formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_CourseGroupInfo.Img)) FileHelper.FileDel(ImgRoot + model_CourseGroupInfo.Img);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_CourseGroupInfo.Img = fileurl + filename;
						}
					}
				}
				#endregion
				_context.Updateable(model_CourseGroupInfo).ExecuteCommand();
				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改子课程"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'});location.href='?Courseid="+ vm_Add.Courseid + "&CourseGroupInfoid=" + vm_Add.CourseGroupInfoid + "&lang=" + vm_Add.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_CourseGroupInfo = new WebCourseGroupInfo();
				model_CourseGroupInfo.Courseid = vm_Add.Courseid;
				model_CourseGroupInfo.CourseGroupid = vm_Add.CourseGroupid;
				model_CourseGroupInfo.Status = vm_Add.Status;
				model_CourseGroupInfo.Level = vm_Add.Level;
				model_CourseGroupInfo.Sort = vm_Add.Sort;
				model_CourseGroupInfo.Sendtime = vm_Add.Sendtime;
				model_CourseGroupInfo.Dtime = DateTime.Now;
				#region "上传"
				if (files.Count > 0)
				{
					var formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_CourseGroupInfo.Img = fileurl + filename;
						}
					}
				}
				#endregion
				vm_Add.CourseGroupInfoid = _context.Insertable(model_CourseGroupInfo).ExecuteReturnBigIdentity();
				var model_Lang = new WebCourseGroupInfoLang();
				model_Lang.CourseGroupInfoid = vm_Add.CourseGroupInfoid;
				model_Lang.Title = vm_Add.Title;
				model_Lang.Keys = vm_Add.Keys;
				model_Lang.Message = vm_Add.Message;
				model_Lang.Lang = vm_Add.Lang;
				_context.Insertable(model_Lang).ExecuteCommand();

				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加子课程"] + "',msg:'" + _localizer["创建信息成功"] + "',timeout:3000,showType:'slide'});location.href='?Courseid=" + vm_Add.Courseid + "&CourseGroupInfoid=" + vm_Add.CourseGroupInfoid + "&lang=" + vm_Add.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion

        #region "设置介绍与特色等子课程编辑器"     
        [Authorization(Power = "Main")]
        public async Task<IActionResult> SetPage(long CourseGroupInfoid = 0)
		{
			var vm_SetPage = new ViewModels.Course.SetPage();
			vm_SetPage.Sty = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sty");
			var model_CourseGroupInfo = _context.Queryable<WebCourseGroupInfoLang>().First(u=>u.Lang==Lang && u.CourseGroupInfoid == CourseGroupInfoid);
			if (model_CourseGroupInfo != null)
			{
				switch (vm_SetPage.Sty)
				{
					case "Intro"://课程介绍
						vm_SetPage.Content = model_CourseGroupInfo.Intro;
						break;
					case "Objectives"://课程目标
						vm_SetPage.Content = model_CourseGroupInfo.Objectives;
						break;
					case "Crowd"://适合人群
						vm_SetPage.Content = model_CourseGroupInfo.Crowd;
						break;
					case "Merit"://课程特色
						vm_SetPage.Content = model_CourseGroupInfo.Merit;
						break;
					case "Begintime"://开始时间
						vm_SetPage.Content = model_CourseGroupInfo.Begintime;
						break;
					case "Catalog"://课程目录
						vm_SetPage.Content = model_CourseGroupInfo.Catalog;
						break;
				}
				try
				{
					var o = JObject.Parse(vm_SetPage.Content);
					vm_SetPage.EncryptData = o["encrypt_data"].ToString();
				}
				catch { }
			}
			vm_SetPage.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_SetPage);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> SetPage(ViewModels.Course.SetPage vm_SetPage)
		{
			if (string.IsNullOrEmpty(vm_SetPage.Lang) || vm_SetPage.CourseGroupInfoid == 0) {
				return Content("<script>alert('" + _localizer["见此信息，请通知北京技术人员"] + "'); history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			var model_CourseGroupInfoLang = _context.Queryable<WebCourseGroupInfoLang>().First(u => u.Lang == vm_SetPage.Lang && u.CourseGroupInfoid == vm_SetPage.CourseGroupInfoid);
			if (model_CourseGroupInfoLang != null)
			{
				switch (vm_SetPage.Sty)
				{
					case "Intro"://课程介绍
						model_CourseGroupInfoLang.Intro = vm_SetPage.Content;
						_context.Updateable(model_CourseGroupInfoLang).UpdateColumns(u => new { u.Intro}).ExecuteCommand();
						break;
					case "Objectives"://课程目标
						model_CourseGroupInfoLang.Objectives= vm_SetPage.Content;
						_context.Updateable(model_CourseGroupInfoLang).UpdateColumns(u => new { u.Objectives }).ExecuteCommand();
						break;
					case "Crowd"://适合人群
						model_CourseGroupInfoLang.Crowd = vm_SetPage.Content;
						_context.Updateable(model_CourseGroupInfoLang).UpdateColumns(u => new { u.Crowd }).ExecuteCommand();
						break;
					case "Merit"://课程特色
						model_CourseGroupInfoLang.Merit = vm_SetPage.Content;
						_context.Updateable(model_CourseGroupInfoLang).UpdateColumns(u => new { u.Merit }).ExecuteCommand();
						break;
					case "Begintime"://开始时间
						model_CourseGroupInfoLang.Begintime = vm_SetPage.Content;
						_context.Updateable(model_CourseGroupInfoLang).UpdateColumns(u => new { u.Begintime }).ExecuteCommand();
						break;
					case "Catalog"://课程目录
						model_CourseGroupInfoLang.Catalog = vm_SetPage.Content;
						_context.Updateable(model_CourseGroupInfoLang).UpdateColumns(u => new { u.Catalog }).ExecuteCommand();
						break;
				}
			}
			else
			{
				model_CourseGroupInfoLang = new WebCourseGroupInfoLang();
				model_CourseGroupInfoLang.CourseGroupInfoid = vm_SetPage.CourseGroupInfoid;
				model_CourseGroupInfoLang.Lang = vm_SetPage.Lang;
				switch (vm_SetPage.Sty)
				{
					case "Intro"://课程介绍
						model_CourseGroupInfoLang.Intro = vm_SetPage.Content;
						break;
					case "Objectives"://课程目标
						model_CourseGroupInfoLang.Objectives = vm_SetPage.Content;
						break;
					case "Crowd"://适合人群
						model_CourseGroupInfoLang.Crowd = vm_SetPage.Content;
						break;
					case "Merit"://课程特色
						model_CourseGroupInfoLang.Merit = vm_SetPage.Content;
						break;
					case "Begintime"://开始时间
						model_CourseGroupInfoLang.Begintime = vm_SetPage.Content;
						break;
					case "Catalog"://课程目录
						model_CourseGroupInfoLang.Catalog = vm_SetPage.Content;
						break;
				}
				_context.Insertable(model_CourseGroupInfoLang).ExecuteCommand();
			}
			return Content("<script>alert('" + _localizer["保存信息成功"] + "'); location.href='?sty="+ vm_SetPage.Sty + "&CourseGroupInfoid="+ vm_SetPage.CourseGroupInfoid + "&lang="+ vm_SetPage.Lang + "';</script>", "text/html", Encoding.GetEncoding("utf-8"));
		}
        #endregion

        #region "申请表"    
        [Authorization(Power = "Main")]
        public IActionResult Apply()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ApplyData(string keys = "", string status = "",string sty="",string lecture_lang = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string CourseApplyids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "CourseApplyids");
			long CourseApplyid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "CourseApplyid", 0);
			var list_CourseApply = new List<WebCourseApply>();
			if (CourseApplyids.Trim() == "") CourseApplyids = "0";
			string[] arr = CourseApplyids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebCourseApply>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.CourseApplyid) || u.CourseApplyid == CourseApplyid)
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
					string msg = $"[{DateTime.Now}]{AdminMasterName}拒绝了申请";
					count = _context.Updateable<WebCourseApply>()
						.SetColumns(u => u.Status == 3)
                        .SetColumns(u => u.Remark == SqlFunc.MergeString(msg,u.Remark))
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.CourseApplyid) || u.CourseApplyid == CourseApplyid)
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
					count = _context.Updateable<WebCourseApply>()
						.SetColumns(u => u.Status == 1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.CourseApplyid) || u.CourseApplyid == CourseApplyid)
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
					count = _context.Updateable<WebCourseApply>()
						.SetColumns(u => u.Status == 0)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.CourseApplyid) || u.CourseApplyid == CourseApplyid)
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
					var exp = new Expressionable<WebCourseApply>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u =>SqlFunc.Subqueryable<WebCourseLang>().Where(s=>s.Courseid==u.Courseid && s.Lang== Lang && s.Title.Contains(keys.Trim())).Any() || SqlFunc.MergeString(u.ContactMobile, "").Contains(keys.Trim()) || SqlFunc.MergeString(u.ContactEmail, "").Contains(keys.Trim()));
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(sty), u => u.Sty.ToString() == sty.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(lecture_lang), u => u.LectureLang.ToString() == lecture_lang.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebCourseApply>().Where(exp.ToExpression());
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
					list_CourseApply = query.ToPageList(page, pagesize, ref total);
					var list_Course = _context.Queryable<WebCourseLang>().Where(u => list_CourseApply.Select(s => s.Courseid).Contains(u.Courseid) && u.Lang==Lang).ToList();
					var list_User = _context.Queryable<WebUser>().Where(u => list_CourseApply.Select(s => s.ContactMobileCode + s.ContactMobile).Contains(SqlFunc.MergeString(u.MobileCode, u.Mobile)) && u.Status != -1).ToList();
					var list_Skutype = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == Lang).ToList();
					var list_Info = _context.Queryable<WebCourseGroupInfoLang>().Where(u => u.Lang == Lang && list_CourseApply.Select(s => s.CourseGroupInfoid).Contains(u.CourseGroupInfoid)).ToList();


					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_CourseApply.Count; i++)
					{
						var model_Course = list_Course.FirstOrDefault(u => u.Courseid == list_CourseApply[i].Courseid);
						var model_User = list_User.FirstOrDefault(u => u.MobileCode == list_CourseApply[i].ContactMobileCode && u.Mobile== list_CourseApply[i].ContactMobile);

						if (list_CourseApply[i].Userid == 0 && model_User!=null) list_CourseApply[i].Userid = model_User.Userid;
						string sty_str = "";
						string course_name = model_Course?.Title;

						switch (list_CourseApply[i].Sty)
                        {
                            case 0:
                                sty_str = _localizer["试听申请"];
                                break;
                            case 1:
                                sty_str = _localizer["课程咨询"];
								var model_Info = list_Info.FirstOrDefault(u => u.CourseGroupInfoid == list_CourseApply[i].CourseGroupInfoid);
								if (model_Info != null) course_name += ("" + model_Info.Title);
								break;
                            case 2:
                                sty_str = _localizer["排课申请"];
								var model_Skutype = list_Skutype.FirstOrDefault(u => u.SkuTypeid == list_CourseApply[i].SkuTypeid );
								if (model_Skutype != null) course_name += ("" + model_Skutype.Title);
								if (list_CourseApply[i].ClassHour > 0) course_name += (" " + list_CourseApply[i].ClassHour + "课时");
								break;
                        }

						str.Append("{");
						str.Append("\"CourseApplyid\":\"" + list_CourseApply[i].CourseApplyid + "\",");
						str.Append("\"Sty\":" + list_CourseApply[i].Sty + ",");
						str.Append("\"StyStr\":\"" + sty_str + "\",");
						str.Append("\"IsReg\":" + (list_CourseApply[i].Userid>0 ? 1:0) + ",");
						str.Append("\"CourseName\":\"" + list_CourseApply[i].CourseName + "\",");
						//str.Append("\"LectureLang\":\"" + list_CourseApply[i].LectureLang + "\",");
						str.Append("\"CourseName\":\"" + course_name + "\",");
						str.Append("\"MenkeLessonName\":\"" + list_CourseApply[i].MenkeLessonName + "\",");
						str.Append("\"TeacherName\":\"" + JsonHelper.JsonCharFilter(list_CourseApply[i].Teacher + "") + "\",");
						str.Append("\"TeacherMobile\":\"" + JsonHelper.JsonCharFilter(list_CourseApply[i].TeacherMobileCode + list_CourseApply[i].TeacherMobile + "") + "\",");
						str.Append("\"ContactMobile\":\"" + JsonHelper.JsonCharFilter(list_CourseApply[i].ContactMobile) + "\",");
						str.Append("\"ContactEmail\":\"" + JsonHelper.JsonCharFilter(list_CourseApply[i].ContactEmail) + "\",");
						str.Append("\"Dtime\":\"" + list_CourseApply[i].Dtime.ToString() + "\",");
						str.Append("\"Status\":" + list_CourseApply[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_CourseApply.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					_context.Updateable(list_CourseApply).UpdateColumns(u => u.Userid).ExecuteCommand();
					break;
			}
			return Content(str.ToString());
		}

        [Authorization(Power = "Main")]
        public async Task<IActionResult> ApplyAdd(long CourseApplyid = 0)
		{
			var vm_ListAdd = new ViewModels.Course.ApplyAdd();

			var model_CourseApply = _context.Queryable<WebCourseApply>().InSingle(CourseApplyid);
			if (model_CourseApply != null)
			{
				vm_ListAdd.Sty = model_CourseApply.Sty;
                vm_ListAdd.LectureLang = model_CourseApply.LectureLang;
				vm_ListAdd.MenkeCourseName = model_CourseApply.MenkeCourseName;
				vm_ListAdd.MenkeLessonName = model_CourseApply.MenkeLessonName;
				vm_ListAdd.Teacher = model_CourseApply.Teacher;
				vm_ListAdd.TeacherMobileCode = model_CourseApply.TeacherMobileCode;
				vm_ListAdd.TeacherMobile = model_CourseApply.TeacherMobile;
				vm_ListAdd.ContactEmail = model_CourseApply.ContactEmail;
				vm_ListAdd.ContactMobile = model_CourseApply.ContactMobile;
				vm_ListAdd.Dtime = model_CourseApply.Dtime;
				vm_ListAdd.Status = model_CourseApply.Status;
				

				var model_MenkeLesson = _context.Queryable<MenkeLesson>().First(u => u.MenkeLessonId == model_CourseApply.MenkeLessonId);
				vm_ListAdd.MenkeLiveSerial = model_MenkeLesson?.MenkeLiveSerial + "";

				var model_User = _context.Queryable<WebUser>().InSingle(model_CourseApply.Userid);
				if (model_User != null)
				{
					vm_ListAdd.StudentName = model_User.FirstName +" "+ model_User.LastName;
					if (model_CourseApply.Age == 0) { 
						vm_ListAdd.Age= (int)Math.Ceiling((DateTime.Now - model_User.Birthdate).TotalDays / 365);
					}
				}
				if(model_CourseApply.Age>0) vm_ListAdd.Age = model_CourseApply.Age;
				vm_ListAdd.Ischinese = model_CourseApply.Ischinese;

				var model_Course = _context.Queryable<WebCourseLang>().First(u => u.Courseid == model_CourseApply.Courseid && u.Lang == Lang);
				if (model_Course != null) {
					vm_ListAdd.CourseName = model_Course.Title;
				}
				if (model_CourseApply.Sty == 2)
				{
					var model_Skutype = _context.Queryable<WebSkuTypeLang>().First(u => u.SkuTypeid == model_CourseApply.SkuTypeid && u.Lang == Lang);
					if (model_Skutype != null) vm_ListAdd.SkuType = model_Skutype.Title;
					vm_ListAdd.ClassHour = model_CourseApply.ClassHour;
				}
			}

			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}
		#endregion

		#region "申请表(只读)"    
		[Authorization(Power = "Main")]
		public IActionResult ApplyRead()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> ApplyReadData(string keys = "", string status = "", string sty = "", string lecture_lang = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string CourseApplyids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "CourseApplyids");
			long CourseApplyid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "CourseApplyid", 0);
			var list_CourseApply = new List<WebCourseApply>();
			if (CourseApplyids.Trim() == "") CourseApplyids = "0";
			string[] arr = CourseApplyids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();

					int total = 0;
					var exp = new Expressionable<WebCourseApply>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebCourseLang>().Where(s => s.Courseid == u.Courseid && s.Lang == Lang && s.Title.Contains(keys.Trim())).Any() || SqlFunc.MergeString(u.ContactMobile, "").Contains(keys.Trim()) || SqlFunc.MergeString(u.ContactEmail, "").Contains(keys.Trim()));
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(sty), u => u.Sty.ToString() == sty.Trim());
					exp.AndIF(!string.IsNullOrEmpty(lecture_lang), u => u.LectureLang.ToString() == lecture_lang.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebCourseApply>().Where(exp.ToExpression());
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
					list_CourseApply = query.ToPageList(page, pagesize, ref total);
					var list_Course = _context.Queryable<WebCourseLang>().Where(u => list_CourseApply.Select(s => s.Courseid).Contains(u.Courseid) && u.Lang == Lang).ToList();
					var list_User = _context.Queryable<WebUser>().Where(u => list_CourseApply.Select(s => s.ContactMobileCode + s.ContactMobile).Contains(SqlFunc.MergeString(u.MobileCode, u.Mobile)) && u.Status != -1).ToList();
					var list_Skutype = _context.Queryable<WebSkuTypeLang>().Where(u => u.Lang == Lang).ToList();
					var list_Info = _context.Queryable<WebCourseGroupInfoLang>().Where(u => u.Lang == Lang && list_CourseApply.Select(s => s.CourseGroupInfoid).Contains(u.CourseGroupInfoid)).ToList();


					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_CourseApply.Count; i++)
					{
						var model_Course = list_Course.FirstOrDefault(u => u.Courseid == list_CourseApply[i].Courseid);
						var model_User = list_User.FirstOrDefault(u => u.MobileCode == list_CourseApply[i].ContactMobileCode && u.Mobile == list_CourseApply[i].ContactMobile);

						if (list_CourseApply[i].Userid == 0 && model_User != null) list_CourseApply[i].Userid = model_User.Userid;
						string sty_str = "";
						string course_name = model_Course?.Title;

						switch (list_CourseApply[i].Sty)
						{
							case 0:
								sty_str = _localizer["试听申请"];
								break;
							case 1:
								sty_str = _localizer["课程咨询"];
								var model_Info = list_Info.FirstOrDefault(u => u.CourseGroupInfoid == list_CourseApply[i].CourseGroupInfoid);
								if (model_Info != null) course_name += ("" + model_Info.Title);
								break;
							case 2:
								sty_str = _localizer["排课申请"];
								var model_Skutype = list_Skutype.FirstOrDefault(u => u.SkuTypeid == list_CourseApply[i].SkuTypeid);
								if (model_Skutype != null) course_name += ("" + model_Skutype.Title);
								if (list_CourseApply[i].ClassHour > 0) course_name += (" " + list_CourseApply[i].ClassHour + "课时");
								break;
						}

						str.Append("{");
						str.Append("\"CourseApplyid\":\"" + list_CourseApply[i].CourseApplyid + "\",");
						str.Append("\"Sty\":" + list_CourseApply[i].Sty + ",");
						str.Append("\"StyStr\":\"" + sty_str + "\",");
						str.Append("\"IsReg\":" + (list_CourseApply[i].Userid > 0 ? 1 : 0) + ",");
						str.Append("\"LectureLang\":\"" + list_CourseApply[i].LectureLang + "\",");
						str.Append("\"CourseName\":\"" + course_name + "\",");
						str.Append("\"MenkeLessonName\":\"" + list_CourseApply[i].MenkeLessonName + "\",");
						str.Append("\"TeacherName\":\"" + JsonHelper.JsonCharFilter(list_CourseApply[i].Teacher + "") + "\",");
						str.Append("\"TeacherMobile\":\"" + JsonHelper.JsonCharFilter(list_CourseApply[i].TeacherMobileCode + list_CourseApply[i].TeacherMobile + "") + "\",");
						str.Append("\"ContactMobile\":\"" + JsonHelper.JsonCharFilter(list_CourseApply[i].ContactMobile) + "\",");
						str.Append("\"ContactEmail\":\"" + JsonHelper.JsonCharFilter(list_CourseApply[i].ContactEmail) + "\",");
						str.Append("\"Dtime\":\"" + list_CourseApply[i].Dtime.ToString() + "\",");
						str.Append("\"Status\":" + list_CourseApply[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_CourseApply.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					_context.Updateable(list_CourseApply).UpdateColumns(u => u.Userid).ExecuteCommand();
					break;
			}
			return Content(str.ToString());
		}

		[Authorization(Power = "Main")]
		public async Task<IActionResult> ApplyReadAdd(long CourseApplyid = 0)
		{
			var vm_ListAdd = new ViewModels.Course.ApplyAdd();

			var model_CourseApply = _context.Queryable<WebCourseApply>().InSingle(CourseApplyid);
			if (model_CourseApply != null)
			{
				vm_ListAdd.Sty = model_CourseApply.Sty;
				vm_ListAdd.LectureLang = model_CourseApply.LectureLang;
				vm_ListAdd.MenkeCourseName = model_CourseApply.MenkeCourseName;
				vm_ListAdd.MenkeLessonName = model_CourseApply.MenkeLessonName;
				vm_ListAdd.Teacher = model_CourseApply.Teacher;
				vm_ListAdd.TeacherMobileCode = model_CourseApply.TeacherMobileCode;
				vm_ListAdd.TeacherMobile = model_CourseApply.TeacherMobile;
				vm_ListAdd.ContactEmail = model_CourseApply.ContactEmail;
				vm_ListAdd.ContactMobile = model_CourseApply.ContactMobile;
				vm_ListAdd.Dtime = model_CourseApply.Dtime;
				vm_ListAdd.Status = model_CourseApply.Status;


				var model_MenkeLesson = _context.Queryable<MenkeLesson>().First(u => u.MenkeLessonId == model_CourseApply.MenkeLessonId);
				vm_ListAdd.MenkeLiveSerial = model_MenkeLesson?.MenkeLiveSerial + "";

				var model_User = _context.Queryable<WebUser>().InSingle(model_CourseApply.Userid);
				if (model_User != null)
				{
					vm_ListAdd.StudentName = model_User.FirstName + " " + model_User.LastName;
					if (model_CourseApply.Age == 0)
					{
						vm_ListAdd.Age = (int)Math.Ceiling((DateTime.Now - model_User.Birthdate).TotalDays / 365);
					}
				}
				if (model_CourseApply.Age > 0) vm_ListAdd.Age = model_CourseApply.Age;
				vm_ListAdd.Ischinese = model_CourseApply.Ischinese;

				var model_Course = _context.Queryable<WebCourseLang>().First(u => u.Courseid == model_CourseApply.Courseid && u.Lang == Lang);
				if (model_Course != null)
				{
					vm_ListAdd.CourseName = model_Course.Title;
				}
				if (model_CourseApply.Sty == 2)
				{
					var model_Skutype = _context.Queryable<WebSkuTypeLang>().First(u => u.SkuTypeid == model_CourseApply.SkuTypeid && u.Lang == Lang);
					if (model_Skutype != null) vm_ListAdd.SkuType = model_Skutype.Title;
					vm_ListAdd.ClassHour = model_CourseApply.ClassHour;
				}
			}

			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}
		#endregion

		#region "选择标签" 
		[HttpGet]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ChooseKeys(string keysids = "", string ControlID = "")
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_ChooseKeys = new ViewModels.Course.ChooseKeys();
			vm_ChooseKeys.Keysids = (keysids + "");
			vm_ChooseKeys.ControlID = ControlID + "";
			StringBuilder str = new StringBuilder();
			long num = 0;
			var list_keysid = (keysids + "").Split(",").Where(u=> long.TryParse(u, out num)).Select(u=>long.Parse(u)).ToList();
			var list_Keys = _context.Queryable<PubKeysLang>().Where(u => u.Lang == Lang && list_keysid.Contains(u.Keysid)).ToList();
			foreach (var model in list_Keys)
			{
				int i = list_Keys.IndexOf(model);
				str.Append("<div class=\"col-lg-2\">");
				str.Append("<div class=\"custom-control custom-checkbox\">");
				str.Append("<input type=\"hidden\" name=\"input_keysid\" value=\""+ model.Keysid + "\" />");
				str.Append("<input type=\"checkbox\" checked class=\"custom-control-input\" name=\"chk_keys\" id=\"chk_keys" + i + "\" value=\"" + model.Title + "\" data-parsley-multiple=\"groups\" data-parsley-mincheck=\"2\">");
				str.Append("<label class=\"custom-control-label\" for=\"chk_keys" + i + "\">" + WebUtility.UrlDecode(model.Title) + "</label>");
				str.Append("</div>");
				str.Append("</div>");
			}
			vm_ChooseKeys.List_Keys = str.ToString();
			return View(vm_ChooseKeys);
		}

		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ChooseKeys(string keysids = "", string keys = "",string abc="")
		{
			StringBuilder str = new StringBuilder();
			long num = 0;
			var list_keysid = (keysids+"").Split(",").Where(u => long.TryParse(u, out num)).Select(u => long.Parse(u)).ToList();
			var exp = new Expressionable<PubKeys>();
			exp.And(u => u.Status == 1 && u.Sty==1);
			exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<PubKeysLang>().Where(s => s.Title.Contains(keys) && s.Keysid == u.Keysid).Any());

			var list_PubKeys = _context.Queryable<PubKeys>().Where(exp.ToExpression()).OrderBy("sort,keysid desc").ToList();
			var list_KeysLang = _context.Queryable<PubKeysLang>().Where(u => list_PubKeys.Select(s => s.Keysid).Contains(u.Keysid) && u.Lang == Lang).ToList();
			for (int i = 0; i < list_PubKeys.Count; i++)
			{
				var model_Lang = list_KeysLang.FirstOrDefault(u=>u.Keysid == list_PubKeys[i].Keysid);
				str.Append("<div class=\"col-lg-2\">");
				str.Append("<div class=\"custom-control custom-checkbox\">");
				str.Append("<input type=\"checkbox\" class=\"custom-control-input\" "+ (list_keysid.Contains(list_PubKeys[i].Keysid)?"checked":"") +" name=\"chk_keys\" id=\"chk_keys_" + list_PubKeys[i].Keysid + "\" value=\"" + model_Lang?.Title + "|"+ list_PubKeys[i].Keysid + "\" data-parsley-multiple=\"groups\" data-parsley-mincheck=\"2\">");
				str.Append("<label class=\"custom-control-label\" for=\"chk_keys_" + list_PubKeys[i].Keysid + "\">" + model_Lang?.Title + "</label>");
				str.Append("</div>");
				str.Append("</div>");
			}
			return Content(str.ToString());
		}
		#endregion

		#region "短视频课程列表"
		[Authorization(Power = "Main")]
		public IActionResult VideoList()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> VideoListData(string keys = "", string status = "", string recommend = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string ShortVideoids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "ShortVideoids");
			long ShortVideoid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "ShortVideoid", 0);
			var list_Video = new List<WebShortVideo>();
			if (ShortVideoids.Trim() == "") ShortVideoids = "0";
			string[] arr = ShortVideoids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebShortVideo>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.ShortVideoid) || u.ShortVideoid == ShortVideoid)
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
					count = _context.Updateable<WebShortVideo>()
						.SetColumns(u => u.Status == 1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.ShortVideoid) || u.ShortVideoid == ShortVideoid)
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
					count = _context.Updateable<WebShortVideo>()
						.SetColumns(u => u.Status == 0)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.ShortVideoid) || u.ShortVideoid == ShortVideoid)
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
					string price_list = "";
					int total = 0;
					var exp = new Expressionable<WebShortVideo>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebShortVideoLang>().Where(s => s.ShortVideoid == u.ShortVideoid && SqlFunc.MergeString(s.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.Message, "").Contains(keys.Trim())).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(recommend), u => u.Recommend.ToString() == recommend.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Sendtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Sendtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebShortVideo>().Where(exp.ToExpression());
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
					query.OrderBy("sort,sendtime desc,short_videoid desc");
					list_Video = query.ToPageList(page, pagesize, ref total);
					var list_CourseLang = _context.Queryable<WebShortVideoLang>().Where(u => list_Video.Select(s => s.ShortVideoid).Contains(u.ShortVideoid) && u.Lang == Lang).ToList();
					var list_Teacher = _context.Queryable<WebTeacherLang>().Where(u => list_Video.Select(s => s.Teacherid).Contains(u.Teacherid) && u.Lang == Lang).ToList();
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Video.Count; i++)
					{
						price_list = "";
						var model_CourseLang = list_CourseLang.FirstOrDefault(u => u.ShortVideoid == list_Video[i].ShortVideoid);
						var Teacher = list_Teacher.FirstOrDefault(u=>u.Teacherid==list_Video[i].Teacherid)?.Name + "";
						str.Append("{");
						str.Append("\"ShortVideoid\":\"" + list_Video[i].ShortVideoid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_CourseLang?.Title + "") + "\",");
						str.Append("\"Img\":\"" + list_Video[i].Img + "\",");
						str.Append("\"Dtime\":\"" + list_Video[i].Dtime.ToString() + "\",");
                        str.Append("\"Sort\":\"" + list_Video[i].Sort + "\",");
                        str.Append("\"Hits\":" + list_Video[i].Hits + ",");
                        str.Append("\"Recommend\":" + list_Video[i].Recommend + ",");
                        str.Append("\"Keys\":\"" + JsonHelper.JsonCharFilter(model_CourseLang?.Keys + "") + "\",");
						str.Append("\"Teacher\":\"" + JsonHelper.JsonCharFilter(Teacher) + "\",");
						str.Append("\"Sendtime\":\"" + list_Video[i].Sendtime + "\",");
						str.Append("\"Status\":" + list_Video[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Video.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
		#endregion

		#region "短视频课程修改"
		[Authorization(Power = "Main")]
		public async Task<IActionResult> VideoListAdd(long ShortVideoid = 0)
		{
			var vm_ListAdd = new ViewModels.Course.VideoListAdd();
			vm_ListAdd.TeacherItem = _context.Queryable<WebTeacher>().LeftJoin<WebTeacherLang>((l, r) => l.Teacherid == r.Teacherid && r.Lang == Lang)
				.Where((l, r) => l.Status == 1).Select((l, r) => new { l.Teacherid, r.Name }).ToList().Select(u => new SelectListItem(u.Name, u.Teacherid.ToString())).ToList();
			vm_ListAdd.TeacherItem.Insert(0, new SelectListItem(_localizer["请关联老师"], "0"));

			var model_Video = _context.Queryable<WebShortVideo>().InSingle(ShortVideoid);
			if (model_Video != null)
			{
				var model_CourseLang = _context.Queryable<WebShortVideoLang>().First(u => u.ShortVideoid == ShortVideoid && u.Lang == Lang);
				if (model_CourseLang != null)
				{
					vm_ListAdd.Title = model_CourseLang.Title;
					vm_ListAdd.Message = model_CourseLang.Message;
					vm_ListAdd.Keys = model_CourseLang.Keys;
					vm_ListAdd.Intro = model_CourseLang.Intro;
					if (JsonHelper.IsJson(model_CourseLang.Intro))
					{
						try
						{
							var o = JObject.Parse(model_CourseLang.Intro);
							vm_ListAdd.EncryptData = o["encrypt_data"].ToString();
						}
						catch { }
					}
				}

				vm_ListAdd.ShortVideoid = ShortVideoid;
				vm_ListAdd.Img = model_Video.Img;
				vm_ListAdd.Banner = model_Video.Banner;
				vm_ListAdd.BannerH5 = model_Video.BannerH5;
				vm_ListAdd.Hits = model_Video.Hits;
				vm_ListAdd.Teacherid = model_Video.Teacherid;
				vm_ListAdd.Video = _commonBaseService.ResourceDomain(model_Video.Video);

				vm_ListAdd.Recommend = model_Video.Recommend;
				vm_ListAdd.Sort = model_Video.Sort;
				vm_ListAdd.Dtime = model_Video.Dtime;
				vm_ListAdd.Status = model_Video.Status;
				vm_ListAdd.Sendtime = model_Video.Sendtime;
				vm_ListAdd.Remark = model_Video.Remark;
			}
			else
			{
				vm_ListAdd.Recommend = 0;
				vm_ListAdd.Status = 1;
				vm_ListAdd.Sort = 100;
				vm_ListAdd.Sendtime = DateTime.Now;
			}
			vm_ListAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> VideoListAdd(VideoListAdd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/ShortVideo/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			int chk_thumb = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_thumb", 0);
			int chk_banner = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner", 0);
			int chk_banner_h5 = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner_h5", 0);
			var old_imgs = GSRequestHelper.GetString(_accessor.HttpContext.Request, "input_old_imgs").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
			var files = _accessor.HttpContext.Request.Form.Files;
			var list_imgs = new List<string>();
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext = "";//扩展名
			var model_master = _tokenManager.GetAdminInfo();
			string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改课程信息";
			
			var list_Currency = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList();
			var model_Video = _context.Queryable<WebShortVideo>().InSingle(vm_ListAdd.ShortVideoid);
			if (model_Video != null)
			{
				var model_CourseLang = _context.Queryable<WebShortVideoLang>().First(u => u.Lang == vm_ListAdd.Lang && u.ShortVideoid == vm_ListAdd.ShortVideoid);
				if (model_CourseLang?.Title != vm_ListAdd.Title)
				{
					message += ",将课程名称由[" + model_CourseLang?.Title + "]改为[" + vm_ListAdd.Title + "]";
				}
				if (model_CourseLang?.Message != vm_ListAdd.Message)
				{
					message += ",将简介由[" + model_CourseLang?.Message + "]改为[" + vm_ListAdd.Message + "]";
				}
				if (model_Video.Sort != vm_ListAdd.Sort)
				{
					message += ",将排序字段由[" + model_Video.Sort + "]改为[" + vm_ListAdd.Sort + "]";
				}
				if (model_Video.Sendtime != vm_ListAdd.Sendtime)
				{
					message += ",将发布时间由[" + model_Video.Sendtime + "]改为[" + vm_ListAdd.Sendtime + "]";
				}
				if (model_Video.Status != vm_ListAdd.Status)
				{
					message += ",将状态由[" + model_Video.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (model_Video.Teacherid != vm_ListAdd.Teacherid)
				{
					message += ",将关联老师ID由[" + model_Video.Teacherid + "]改为[" + vm_ListAdd.Teacherid + "]";
                }
                if (model_Video.Recommend != vm_ListAdd.Recommend)
                {
                    message += ",将推荐状态由[" + model_Video.Recommend + "]改为[" + vm_ListAdd.Recommend + "]";
                }
                if (model_CourseLang != null)
				{
					model_CourseLang.Title = vm_ListAdd.Title;
					model_CourseLang.Keys = vm_ListAdd.Keys;
					model_CourseLang.Message = vm_ListAdd.Message;
					model_CourseLang.Intro = vm_ListAdd.Intro;
					_context.Updateable(model_CourseLang).ExecuteCommand();
				}
				else
				{
					model_CourseLang = new WebShortVideoLang();
					model_CourseLang.ShortVideoid = vm_ListAdd.ShortVideoid;
					model_CourseLang.Title = vm_ListAdd.Title;
					model_CourseLang.Keys = vm_ListAdd.Keys;
					model_CourseLang.Message = vm_ListAdd.Message;
					model_CourseLang.Intro = vm_ListAdd.Intro;
					model_CourseLang.Lang = vm_ListAdd.Lang;
					_context.Insertable(model_CourseLang).ExecuteCommand();
				}
				model_Video.Video = _commonBaseService.ResourceVar(vm_ListAdd.Video);
				model_Video.Recommend = vm_ListAdd.Recommend;
				model_Video.Hits = vm_ListAdd.Hits;
				model_Video.Sort = vm_ListAdd.Sort;
				model_Video.Sendtime = vm_ListAdd.Sendtime;
				model_Video.Status = vm_ListAdd.Status;
				model_Video.Masterid = model_master.AdminMasterid;
				model_Video.Remark = message + "<hr>" + vm_ListAdd.Remark;
				model_Video.Teacherid = vm_ListAdd.Teacherid;
				#region "上传"
				if (chk_thumb == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Video.Img)) FileHelper.FileDel(ImgRoot + model_Video.Img);
					}
					catch { }
					model_Video.Img = "";
				}
				if (chk_banner == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Video.Banner)) FileHelper.FileDel(ImgRoot + model_Video.Banner);
					}
					catch { }
					model_Video.Banner = "";
				}
				if (chk_banner_h5 == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Video.BannerH5)) FileHelper.FileDel(ImgRoot + model_Video.BannerH5);
					}
					catch { }
					model_Video.BannerH5 = "";
				}
				if (files.Count > 0)
				{
					//缩略图
					var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Video.Img)) FileHelper.FileDel(ImgRoot + model_Video.Img);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Video.Img = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Video.Banner)) FileHelper.FileDel(ImgRoot + model_Video.Banner);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Video.Banner = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Video.BannerH5)) FileHelper.FileDel(ImgRoot + model_Video.BannerH5);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Video.BannerH5 = fileurl + filename;
						}
					}
				}
				#endregion

				_context.Updateable(model_Video).ExecuteCommand();

				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?ShortVideoid=" + vm_ListAdd.ShortVideoid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_Video = new WebShortVideo();
				model_Video.Video = vm_ListAdd.Video;
				model_Video.Recommend = vm_ListAdd.Recommend;
				model_Video.Hits = vm_ListAdd.Hits;
				model_Video.Sort = vm_ListAdd.Sort;
				model_Video.Sendtime = vm_ListAdd.Sendtime;
				model_Video.Status = vm_ListAdd.Status;
				model_Video.Masterid = model_master.AdminMasterid;
				model_Video.Remark = message + "<hr>" + vm_ListAdd.Remark;
				model_Video.Teacherid = vm_ListAdd.Teacherid;
				#region "上传"
				if (files.Count > 0)
				{
					var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Video.Img = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Video.Banner = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Video.BannerH5 = fileurl + filename;
						}
					}
				}
				#endregion
				vm_ListAdd.ShortVideoid = _context.Insertable(model_Video).ExecuteReturnBigIdentity();

				var model_CourseLang = new WebShortVideoLang();
				model_CourseLang.Lang = vm_ListAdd.Lang;
				model_CourseLang.ShortVideoid = vm_ListAdd.ShortVideoid;
				model_CourseLang.Title = vm_ListAdd.Title;
				model_CourseLang.Keys = vm_ListAdd.Keys;
				model_CourseLang.Message = vm_ListAdd.Message;
				model_CourseLang.Intro = vm_ListAdd.Intro;
				_context.Insertable(model_CourseLang).ExecuteCommand();


				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?ShortVideoid=" + vm_ListAdd.ShortVideoid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion

		#region "直播课分类列表"
		[Authorization(Power = "Main")]
		public IActionResult OnlineCategoryList()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> OnlineCategoryListData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string OnlineCategoryids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "OnlineCategoryids");
			long OnlineCategoryid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "OnlineCategoryid", 0);
			var list_Category = new List<WebOnlineCategory>();
			if (OnlineCategoryids.Trim() == "") OnlineCategoryids = "0";
			string[] arr = OnlineCategoryids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebOnlineCategory>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.OnlineCategoryid) || u.OnlineCategoryid == OnlineCategoryid)
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
					count = _context.Updateable<WebOnlineCategory>()
						.SetColumns(u => u.Status == 1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.OnlineCategoryid) || u.OnlineCategoryid == OnlineCategoryid)
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
					count = _context.Updateable<WebOnlineCategory>()
						.SetColumns(u => u.Status == 0)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.OnlineCategoryid) || u.OnlineCategoryid == OnlineCategoryid)
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
					string price_list = "";
					int total = 0;
					var exp = new Expressionable<WebOnlineCategory>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebOnlineCategoryLang>().Where(s => s.OnlineCategoryid == u.OnlineCategoryid && SqlFunc.MergeString(s.Title, "").Contains(keys.Trim()) ).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Sendtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Sendtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebOnlineCategory>().Where(exp.ToExpression());
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
					query.OrderBy("sort,sendtime desc,online_categoryid desc");
					list_Category = query.ToPageList(page, pagesize, ref total);
					var list_CategoryLang = _context.Queryable<WebOnlineCategoryLang>().Where(u => list_Category.Select(s => s.OnlineCategoryid).Contains(u.OnlineCategoryid) && u.Lang == Lang).ToList();
					
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Category.Count; i++)
					{
						price_list = "";
						var model_CategoryLang = list_CategoryLang.FirstOrDefault(u => u.OnlineCategoryid == list_Category[i].OnlineCategoryid);
						
						str.Append("{");
						str.Append("\"OnlineCategoryid\":\"" + list_Category[i].OnlineCategoryid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_CategoryLang?.Title + "") + "\",");
						str.Append("\"Img\":\"" + list_Category[i].Img + "\",");
						str.Append("\"Dtime\":\"" + list_Category[i].Dtime.ToString() + "\",");
						str.Append("\"Sort\":\"" + list_Category[i].Sort + "\",");
						str.Append("\"Sendtime\":\"" + list_Category[i].Sendtime + "\",");
						str.Append("\"Status\":" + list_Category[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Category.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
		#endregion

		#region "直播课分类修改"
		[Authorization(Power = "Main")]
		public async Task<IActionResult> OnlineCategoryListAdd(long OnlineCategoryid = 0)
		{
			var vm_ListAdd = new ViewModels.Course.OnlineCategoryListAdd();
			var list_ViewPubCurrency = _context.Queryable<ViewPubCurrency>().Where(u => u.Status == 1 && u.Isuse == 1 && u.Lang == Lang).OrderBy(u => u.Sort).OrderBy(u => u.PubCurrencyid, OrderByType.Desc).ToList();
			
			var model_Category = _context.Queryable<WebOnlineCategory>().InSingle(OnlineCategoryid);
			if (model_Category != null)
			{
				var model_CourseLang = _context.Queryable<WebOnlineCategoryLang>().First(u => u.OnlineCategoryid == OnlineCategoryid && u.Lang == Lang);
				if (model_CourseLang != null)
				{
					vm_ListAdd.Title = model_CourseLang.Title;
				}

				vm_ListAdd.OnlineCategoryid = OnlineCategoryid;
				vm_ListAdd.Img = model_Category.Img;

				vm_ListAdd.Sort = model_Category.Sort;
				vm_ListAdd.Dtime = model_Category.Dtime;
				vm_ListAdd.Status = model_Category.Status;
				vm_ListAdd.Sendtime = model_Category.Sendtime;
			}
			else
			{
				vm_ListAdd.Status = 1;
				vm_ListAdd.Sort = 100;
				vm_ListAdd.Sendtime = DateTime.Now;
			}
			vm_ListAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> OnlineCategoryListAdd(OnlineCategoryListAdd vm_ListAdd)
		{
			var model_master = _tokenManager.GetAdminInfo();
			string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改直播课分类信息";

			var model_Category = _context.Queryable<WebOnlineCategory>().InSingle(vm_ListAdd.OnlineCategoryid);
			if (model_Category != null)
			{
				var model_CourseLang = _context.Queryable<WebOnlineCategoryLang>().First(u => u.Lang == vm_ListAdd.Lang && u.OnlineCategoryid == vm_ListAdd.OnlineCategoryid);
				if (model_CourseLang?.Title != vm_ListAdd.Title)
				{
					message += ",将课程分类由[" + model_CourseLang?.Title + "]改为[" + vm_ListAdd.Title + "]";
				}
				if (model_Category.Sort != vm_ListAdd.Sort)
				{
					message += ",将排序字段由[" + model_Category.Sort + "]改为[" + vm_ListAdd.Sort + "]";
				}
				if (model_Category.Sendtime != vm_ListAdd.Sendtime)
				{
					message += ",将发布时间由[" + model_Category.Sendtime + "]改为[" + vm_ListAdd.Sendtime + "]";
				}
				if (model_Category.Status != vm_ListAdd.Status)
				{
					message += ",将状态由[" + model_Category.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (model_CourseLang != null)
				{
					model_CourseLang.Title = vm_ListAdd.Title;
					_context.Updateable(model_CourseLang).ExecuteCommand();
				}
				else
				{
					model_CourseLang = new WebOnlineCategoryLang();
					model_CourseLang.OnlineCategoryid = vm_ListAdd.OnlineCategoryid;
					model_CourseLang.Title = vm_ListAdd.Title;
					model_CourseLang.Lang = vm_ListAdd.Lang;
					_context.Insertable(model_CourseLang).ExecuteCommand();
				}
				model_Category.Sort = vm_ListAdd.Sort;
				model_Category.Sendtime = vm_ListAdd.Sendtime;
				model_Category.Status = vm_ListAdd.Status;
				
				_context.Updateable(model_Category).ExecuteCommand();

				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?OnlineCategoryid=" + vm_ListAdd.OnlineCategoryid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_Category = new WebOnlineCategory();
				model_Category.Sort = vm_ListAdd.Sort;
				model_Category.Sendtime = vm_ListAdd.Sendtime;
				model_Category.Status = vm_ListAdd.Status;
				model_Category.Depth = 1;
				vm_ListAdd.OnlineCategoryid = _context.Insertable(model_Category).ExecuteReturnBigIdentity();
				_context.Updateable<WebOnlineCategory>().SetColumns(u => u.Path == vm_ListAdd.OnlineCategoryid.ToString()).Where(u=>u.OnlineCategoryid==vm_ListAdd.OnlineCategoryid).ExecuteCommand();

				var model_CourseLang = new WebOnlineCategoryLang();
				model_CourseLang.Lang = vm_ListAdd.Lang;
				model_CourseLang.OnlineCategoryid = vm_ListAdd.OnlineCategoryid;
				model_CourseLang.Title = vm_ListAdd.Title;
				_context.Insertable(model_CourseLang).ExecuteCommand();


				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?OnlineCategoryid=" + vm_ListAdd.OnlineCategoryid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion

		#region "直播课程列表"
		[Authorization(Power = "Main")]
        public IActionResult OnlineList()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> OnlineListData(string keys = "", string status = "", string recommend = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string OnlineCourseids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "OnlineCourseids");
			long OnlineCourseid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "OnlineCourseid", 0);
			var list_Course = new List<WebOnlineCourse>();
			if (OnlineCourseids.Trim() == "") OnlineCourseids = "0";
			string[] arr = OnlineCourseids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebOnlineCourse>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.OnlineCourseid) || u.OnlineCourseid == OnlineCourseid)
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
					count = _context.Updateable<WebOnlineCourse>()
						.SetColumns(u => u.Status == 1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.OnlineCourseid) || u.OnlineCourseid == OnlineCourseid)
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
					count = _context.Updateable<WebOnlineCourse>()
						.SetColumns(u => u.Status == 0)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.OnlineCourseid) || u.OnlineCourseid == OnlineCourseid)
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
					string price_list = "",category="";
					int total = 0;
					var exp = new Expressionable<WebOnlineCourse>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebOnlineCourseLang>().Where(s=>s.OnlineCourseid==u.OnlineCourseid && SqlFunc.MergeString(s.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.Message, "").Contains(keys.Trim())).Any());
					exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(recommend), u => u.Recommend.ToString() == recommend.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Sendtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Sendtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebOnlineCourse>().Where(exp.ToExpression());
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
					query.OrderBy("sort,sendtime desc,online_courseid desc");
					list_Course = query.ToPageList(page, pagesize, ref total);
					var list_Category = _context.Queryable<WebOnlineCategory>().LeftJoin<WebOnlineCategoryLang>((l, r) => l.OnlineCategoryid == r.OnlineCategoryid && r.Lang == Lang)
				.Where((l, r) => l.Status == 1).Select((l, r) => new { l.OnlineCategoryid, r.Title }).ToList();

                    var list_CourseLang = _context.Queryable<WebOnlineCourseLang>().Where(u => list_Course.Select(s => s.OnlineCourseid).Contains(u.OnlineCourseid) && u.Lang == Lang).ToList();
					var list_Teacher = _context.Queryable<WebTeacherLang>().Where(u => list_Course.Select(s => s.Teacherid).Contains(u.Teacherid) && u.Lang == Lang).ToList();

					var list_OnlineCoursePrice = _context.Queryable<WebOnlineCoursePrice>()
						.LeftJoin<PubCurrency>((l, r)=>l.PubCurrencyid==r.PubCurrencyid)
						.Where((l, r) => list_Course.Select(s => s.OnlineCourseid).Contains(l.OnlineCourseid)).OrderBy((l, r) => r.Sort).ToList();
                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Course.Count; i++)
					{
						price_list = "";
						var model_Category = list_Category.FirstOrDefault(u => u.OnlineCategoryid == list_Course[i].OnlineCategoryid);
						category = model_Category != null ? model_Category.Title : "";
                        var model_CourseLang = list_CourseLang.FirstOrDefault(u => u.OnlineCourseid == list_Course[i].OnlineCourseid);
						var list_price = list_OnlineCoursePrice.Where(u => u.OnlineCourseid == list_Course[i].OnlineCourseid).ToList();
                        price_list += "<table style=\"width:100%\">";
                        foreach (var model in list_price) {
                            var dis = (model.MarketPrice == 0 ? "0" : (model.Price / model.MarketPrice).ToString("0.00"));
                            price_list += $"<tr><td>{_localizer["原价"]}{model.CurrencyCode} : {model.MarketPrice}</td><td>{dis}</td><td>{_localizer["售价"]}{model.CurrencyCode} : {model.Price}</td></tr>";
                        }
                        var discount = (list_Course[i].MarketPrice == 0 ? "0" : (list_Course[i].Price / list_Course[i].MarketPrice).ToString("0.00"));
                        price_list += $"<tr><td>{_localizer["原价"]}USD : {list_Course[i].MarketPrice}</td><td>{discount}</td><td>{_localizer["售价"]}USD : {list_Course[i].Price}</td></tr>";
                        price_list += "</table>";
						var teacher = list_Teacher.FirstOrDefault(u => u.Teacherid == list_Course[i].Teacherid)?.Name + "";


						str.Append("{");
						str.Append("\"OnlineCourseid\":\"" + list_Course[i].OnlineCourseid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_CourseLang?.Title + "") + "\",");
						str.Append("\"Category\":\"" + JsonHelper.JsonCharFilter(category) + "\",");
						str.Append("\"Img\":\"" + list_Course[i].Img + "\",");
						str.Append("\"Dtime\":\"" + list_Course[i].Dtime.ToString() + "\",");
						str.Append("\"Sort\":\"" + list_Course[i].Sort + "\",");
						str.Append("\"Recommend\":" + list_Course[i].Recommend + ",");
                        str.Append("\"LessonCount\":\"" + list_Course[i].LessonCount + "\",");
                        str.Append("\"LessonStart\":\"" + list_Course[i].LessonStart + "\",");
                        str.Append("\"PriceList\":\"" + JsonHelper.JsonCharFilter(price_list) + "\",");
                        str.Append("\"Teacher\":\"" + JsonHelper.JsonCharFilter(teacher) + "\",");
						str.Append("\"Sendtime\":\"" + list_Course[i].Sendtime + "\",");
						str.Append("\"Status\":" + list_Course[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Course.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
		#endregion

		#region "直播课程修改"
		[Authorization(Power = "Main")]
        public async Task<IActionResult> OnlineListAdd(long OnlineCourseid = 0)
		{
			var vm_ListAdd = new ViewModels.Course.OnlineListAdd();
			vm_ListAdd.ListPubCurrency = new List<PubCurrencyPrice>();
			var list_ViewPubCurrency = _context.Queryable<ViewPubCurrency>().Where(u => u.Status == 1 && u.Isuse == 1 && u.Lang == Lang).OrderBy(u => u.Sort).OrderBy(u => u.PubCurrencyid, OrderByType.Desc).ToList();
			var list_SkuPrice = _context.Queryable<WebOnlineCoursePrice>().Where(u => u.OnlineCourseid == OnlineCourseid && list_ViewPubCurrency.Select(s => s.PubCurrencyid).Contains(u.PubCurrencyid)).ToList();
			list_ViewPubCurrency.ForEach(model =>
			{
				var model_SkuPrice = list_SkuPrice.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
				vm_ListAdd.ListPubCurrency.Add(new PubCurrencyPrice()
				{
					PubCurrencyid = model.PubCurrencyid,
					Country = model.Country,
					CountryCode = model.CountryCode,
					CurrencyCode = model.CurrencyCode,
					Price = (model_SkuPrice != null) ? model_SkuPrice.Price : 0,
					MarketPrice = (model_SkuPrice != null) ? model_SkuPrice.MarketPrice : 0
				});
			});
			vm_ListAdd.TeacherItem = _context.Queryable<WebTeacher>().LeftJoin<WebTeacherLang>((l, r) => l.Teacherid == r.Teacherid && r.Lang == Lang)
				.Where((l, r) => l.Status == 1).Select((l, r) => new { l.Teacherid, r.Name }).ToList().Select(u => new SelectListItem(u.Name, u.Teacherid.ToString())).ToList();
			vm_ListAdd.TeacherItem.Insert(0, new SelectListItem(_localizer["请关联老师"], "0"));

			vm_ListAdd.OnlineCategoryItem = _context.Queryable<WebOnlineCategory>().LeftJoin<WebOnlineCategoryLang>((l, r) => l.OnlineCategoryid == r.OnlineCategoryid && r.Lang == Lang)
				.Where((l, r) => l.Status == 1).Select((l, r) => new { l.OnlineCategoryid, r.Title }).ToList().Select(u => new SelectListItem(u.Title, u.OnlineCategoryid.ToString())).ToList();

			var model_Course = _context.Queryable<WebOnlineCourse>().InSingle(OnlineCourseid);
			if (model_Course != null)
			{
				var model_CourseLang = _context.Queryable<WebOnlineCourseLang>().First(u => u.OnlineCourseid == OnlineCourseid && u.Lang == Lang);
				if (model_CourseLang != null) {
					vm_ListAdd.Title = model_CourseLang.Title;
					vm_ListAdd.Message = model_CourseLang.Message;
					vm_ListAdd.Keys = model_CourseLang.Keys;
					vm_ListAdd.Intro = model_CourseLang.Intro;
					if (JsonHelper.IsJson(model_CourseLang.Intro))
					{
						try
						{
							var o = JObject.Parse(model_CourseLang.Intro);
							vm_ListAdd.EncryptData = o["encrypt_data"].ToString();
						}
						catch { }
					}
				}
				vm_ListAdd.OnlineCategoryid = model_Course.OnlineCategoryid;
				vm_ListAdd.OnlineCourseid = OnlineCourseid;
				vm_ListAdd.Img = model_Course.Img;
				vm_ListAdd.Banner = model_Course.Banner;
				vm_ListAdd.BannerH5 = model_Course.BannerH5;
				vm_ListAdd.Hits = model_Course.Hits;
				vm_ListAdd.LessonCount = model_Course.LessonCount;
				vm_ListAdd.LessonStart = model_Course.LessonStart;
				vm_ListAdd.Price = model_Course.Price;
				vm_ListAdd.MarketPrice = model_Course.MarketPrice;
				vm_ListAdd.Teacherid= model_Course.Teacherid;
				vm_ListAdd.Recommend = model_Course.Recommend;

				vm_ListAdd.Sort = model_Course.Sort;
				vm_ListAdd.Dtime = model_Course.Dtime;
				vm_ListAdd.Status = model_Course.Status;
				vm_ListAdd.Sendtime = model_Course.Sendtime;
				vm_ListAdd.Remark = model_Course.Remark;
			}
			else
			{
				vm_ListAdd.LessonStart = DateTime.Now.ToString();
				vm_ListAdd.Status = 1;
				vm_ListAdd.Sort = 100;
				vm_ListAdd.Sendtime = DateTime.Now;
			}
			vm_ListAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> OnlineListAdd(OnlineListAdd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/OnlineCourse/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			int chk_thumb = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_thumb", 0);
			int chk_banner = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner", 0);
			int chk_banner_h5 = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner_h5", 0);
			var old_imgs = GSRequestHelper.GetString(_accessor.HttpContext.Request, "input_old_imgs").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
			var files = _accessor.HttpContext.Request.Form.Files;
			var list_imgs = new List<string>();
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext = "";//扩展名
			var model_master = _tokenManager.GetAdminInfo();
			string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改课程信息";
			var list_Price = new List<WebOnlineCoursePrice>();

			var list_Currency = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList();
			var model_Course = _context.Queryable<WebOnlineCourse>().InSingle(vm_ListAdd.OnlineCourseid);
			if (model_Course != null)
			{
				var model_CourseLang = _context.Queryable<WebOnlineCourseLang>().First(u => u.Lang== vm_ListAdd.Lang && u.OnlineCourseid==vm_ListAdd.OnlineCourseid);
				if (model_CourseLang?.Title != vm_ListAdd.Title)
				{
					message += ",将课程名称由[" + model_CourseLang?.Title + "]改为[" + vm_ListAdd.Title + "]";
				}
				if (model_CourseLang?.Message != vm_ListAdd.Message)
				{
					message += ",将简介由[" + model_CourseLang?.Message + "]改为[" + vm_ListAdd.Message + "]";
				}
				if (model_Course.LessonCount != vm_ListAdd.LessonCount)
				{
					message += ",将课节数由[" + model_Course.LessonCount + "]改为[" + vm_ListAdd.LessonCount + "]";
				}
				if (model_Course.LessonStart != vm_ListAdd.LessonStart)
				{
					message += ",将上课时间由[" + model_Course.LessonStart + "]改为[" + vm_ListAdd.LessonStart + "]";
				}
				if (model_Course.Sort != vm_ListAdd.Sort)
				{
					message += ",将排序字段由[" + model_Course.Sort + "]改为[" + vm_ListAdd.Sort + "]";
				}
				if (model_Course.OnlineCategoryid != vm_ListAdd.OnlineCategoryid)
				{
					message += ",将课程分类由[" + model_Course.OnlineCategoryid + "]改为[" + vm_ListAdd.OnlineCategoryid + "]";
				}
				if (model_Course.Sendtime != vm_ListAdd.Sendtime)
				{
					message += ",将发布时间由[" + model_Course.Sendtime + "]改为[" + vm_ListAdd.Sendtime + "]";
				}
				if (model_Course.Status != vm_ListAdd.Status)
				{
					message += ",将状态由[" + model_Course.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (model_Course.Teacherid != vm_ListAdd.Teacherid)
				{
					message += ",将老师ID由[" + model_Course.Teacherid + "]改为[" + vm_ListAdd.Teacherid + "]";
                }
                if (model_Course.Recommend != vm_ListAdd.Recommend)
                {
                    message += ",将推荐状态由[" + model_Course.Recommend + "]改为[" + vm_ListAdd.Recommend + "]";
                }
                if (model_CourseLang != null)
				{
					model_CourseLang.Title = vm_ListAdd.Title;
					model_CourseLang.Keys = vm_ListAdd.Keys;
					model_CourseLang.Message = vm_ListAdd.Message;
					model_CourseLang.Intro = vm_ListAdd.Intro;
					_context.Updateable(model_CourseLang).ExecuteCommand();
				}
				else {
					model_CourseLang = new WebOnlineCourseLang();
					model_CourseLang.OnlineCourseid = vm_ListAdd.OnlineCourseid;
					model_CourseLang.Title = vm_ListAdd.Title;
					model_CourseLang.Keys = vm_ListAdd.Keys;
					model_CourseLang.Message = vm_ListAdd.Message;
					model_CourseLang.Intro = vm_ListAdd.Intro;
					model_CourseLang.Lang = vm_ListAdd.Lang;
					_context.Insertable(model_CourseLang).ExecuteCommand();
				}
				model_Course.OnlineCategoryid = vm_ListAdd.OnlineCategoryid;
				model_Course.Recommend = vm_ListAdd.Recommend;
				model_Course.Sort = vm_ListAdd.Sort;
				model_Course.Sendtime = vm_ListAdd.Sendtime;
				model_Course.Status = vm_ListAdd.Status;
				model_Course.Masterid = model_master.AdminMasterid;
				model_Course.Remark = message + "<hr>" + vm_ListAdd.Remark;
				model_Course.LessonCount = vm_ListAdd.LessonCount;
				model_Course.LessonStart = vm_ListAdd.LessonStart;
				model_Course.Teacherid = vm_ListAdd.Teacherid;
				#region "上传"
				if (chk_thumb == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Course.Img)) FileHelper.FileDel(ImgRoot + model_Course.Img);
					}
					catch { }
					model_Course.Img = "";
				}
				if (chk_banner == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Course.Banner)) FileHelper.FileDel(ImgRoot + model_Course.Banner);
					}
					catch { }
					model_Course.Banner = "";
				}
				if (chk_banner_h5 == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Course.BannerH5)) FileHelper.FileDel(ImgRoot + model_Course.BannerH5);
					}
					catch { }
					model_Course.BannerH5 = "";
				}
				if (files.Count > 0)
				{
					//缩略图
					var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Course.Img)) FileHelper.FileDel(ImgRoot + model_Course.Img);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.Img = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Course.Banner)) FileHelper.FileDel(ImgRoot + model_Course.Banner);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.Banner = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Course.BannerH5)) FileHelper.FileDel(ImgRoot + model_Course.BannerH5);
						}
						catch
						{
						}
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名
						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.BannerH5 = fileurl + filename;
						}
					}
				}
				#endregion

				list_Price = _context.Queryable<WebOnlineCoursePrice>().Where(u => u.OnlineCourseid == vm_ListAdd.OnlineCourseid).ToList();
				foreach (var model in list_Currency)
				{
					var model_Price = list_Price.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
					if (model_Price != null)
					{
						model_Price.Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M);
						model_Price.MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M);
						model_Price.CountryCode = model.CountryCode;
						model_Price.CurrencyCode = model.CurrencyCode;
					}
					else
					{
						list_Price.Add(new WebOnlineCoursePrice()
						{
							OnlineCourseid = vm_ListAdd.OnlineCourseid,
							PubCurrencyid = model.PubCurrencyid,
							Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M),
							MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M),
							CountryCode = model.CountryCode,
							CurrencyCode = model.CurrencyCode
						});
					}
				}
				var x = _context.Storageable(list_Price).ToStorage();
				x.AsInsertable.ExecuteCommand();
				x.AsUpdateable.ExecuteCommand();
				model_Course.Price = vm_ListAdd.Price;
				model_Course.MarketPrice = vm_ListAdd.MarketPrice;
				_context.Updateable(model_Course).ExecuteCommand();

				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?OnlineCourseid=" + vm_ListAdd.OnlineCourseid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_Course = new WebOnlineCourse();
				model_Course.OnlineCategoryid = vm_ListAdd.OnlineCategoryid;
				model_Course.Recommend = vm_ListAdd.Recommend;
				model_Course.Sort = vm_ListAdd.Sort;
				model_Course.Sendtime = vm_ListAdd.Sendtime;
				model_Course.Status = vm_ListAdd.Status;
				model_Course.Masterid = model_master.AdminMasterid;
				model_Course.Remark = message + "<hr>" + vm_ListAdd.Remark;
				model_Course.LessonCount = vm_ListAdd.LessonCount;
				model_Course.LessonStart = vm_ListAdd.LessonStart;
				model_Course.Teacherid = vm_ListAdd.Teacherid;
				model_Course.Price = vm_ListAdd.Price;
				model_Course.MarketPrice = vm_ListAdd.MarketPrice;
				#region "上传"
				if (files.Count > 0)
				{
					var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.Img = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.Banner = fileurl + filename;
						}
					}
					formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = System.IO.Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_Course.BannerH5 = fileurl + filename;
						}
					}
				}
				#endregion
				vm_ListAdd.OnlineCourseid = _context.Insertable(model_Course).ExecuteReturnBigIdentity();

				var model_CourseLang = new WebOnlineCourseLang();
				model_CourseLang.Lang = vm_ListAdd.Lang;
				model_CourseLang.OnlineCourseid = vm_ListAdd.OnlineCourseid;
				model_CourseLang.Title = vm_ListAdd.Title;
				model_CourseLang.Keys = vm_ListAdd.Keys;
				model_CourseLang.Message = vm_ListAdd.Message;
				model_CourseLang.Intro = vm_ListAdd.Intro;
				_context.Insertable(model_CourseLang).ExecuteCommand();

				list_Price = _context.Queryable<WebOnlineCoursePrice>().Where(u => u.OnlineCourseid == vm_ListAdd.OnlineCourseid).ToList();
				foreach (var model in list_Currency)
				{
					var model_Price = list_Price.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
					if (model_Price != null)
					{
						model_Price.Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M);
						model_Price.MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M);
						model_Price.CountryCode = model.CountryCode;
						model_Price.CurrencyCode = model.CurrencyCode;
					}
					else
					{
						list_Price.Add(new WebOnlineCoursePrice()
						{
							OnlineCourseid = vm_ListAdd.OnlineCourseid,
							PubCurrencyid = model.PubCurrencyid,
							Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M),
							MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M),
							CountryCode = model.CountryCode,
							CurrencyCode = model.CurrencyCode
						});
					}
				}
				var x = _context.Storageable(list_Price).ToStorage();
				x.AsInsertable.ExecuteCommand();
				x.AsUpdateable.ExecuteCommand();

				return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?OnlineCourseid=" + vm_ListAdd.OnlineCourseid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion

        #region "录播课程列表"
        [Authorization(Power = "Main")]
        public IActionResult RecordList()
        {
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View();
        }

        [HttpGet]
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RecordListData(string keys = "", string status = "", string recommend = "", string begintime = "", string endtime = "")
        {
            StringBuilder str = new StringBuilder();
            int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
            int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
            string RecordCourseids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "RecordCourseids");
            long RecordCourseid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "RecordCourseid", 0);
            var list_Course = new List<WebRecordCourse>();
            if (RecordCourseids.Trim() == "") RecordCourseids = "0";
            string[] arr = RecordCourseids.Split(',');
            int count = 0;
            switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
            {
                case "del":
                    count = _context.Updateable<WebRecordCourse>()
                        .SetColumns(u => u.Status == -1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.RecordCourseid) || u.RecordCourseid == RecordCourseid)
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
                    count = _context.Updateable<WebRecordCourse>()
                        .SetColumns(u => u.Status == 1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.RecordCourseid) || u.RecordCourseid == RecordCourseid)
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
                    count = _context.Updateable<WebRecordCourse>()
                        .SetColumns(u => u.Status == 0)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.RecordCourseid) || u.RecordCourseid == RecordCourseid)
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
                    string price_list = "";
                    int total = 0;
                    var exp = new Expressionable<WebRecordCourse>();
                    exp.And(u => u.Status != -1);
                    exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebRecordCourseLang>().Where(s => s.RecordCourseid == u.RecordCourseid && SqlFunc.MergeString(s.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.Message, "").Contains(keys.Trim())).Any());
                    exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(recommend), u => u.Recommend.ToString() == recommend.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Sendtime >= DateTime.Parse(begintime));
                    exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Sendtime <= DateTime.Parse(endtime));
                    var query = _context.Queryable<WebRecordCourse>().Where(exp.ToExpression());
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
                    query.OrderBy("sort,sendtime desc,record_courseid desc");
                    list_Course = query.ToPageList(page, pagesize, ref total);
					var list_Teacher = _context.Queryable<WebTeacherLang>().Where(u => list_Course.Select(s => s.Teacherid).Contains(u.Teacherid) && u.Lang == Lang).ToList();

					var list_CourseLang = _context.Queryable<WebRecordCourseLang>().Where(u => list_Course.Select(s => s.RecordCourseid).Contains(u.RecordCourseid) && u.Lang == Lang).ToList();

                    var list_OnlineCoursePrice = _context.Queryable<WebRecordCoursePrice>()
                        .LeftJoin<PubCurrency>((l, r) => l.PubCurrencyid == r.PubCurrencyid)
                        .Where((l, r) => list_Course.Select(s => s.RecordCourseid).Contains(l.RecordCourseid)).OrderBy((l, r) => r.Sort).ToList();
                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
                    for (int i = 0; i < list_Course.Count; i++)
                    {
                        price_list = "";
                        var model_CourseLang = list_CourseLang.FirstOrDefault(u => u.RecordCourseid == list_Course[i].RecordCourseid);
                        var list_price = list_OnlineCoursePrice.Where(u => u.RecordCourseid == list_Course[i].RecordCourseid).ToList();
                        price_list += "<table style=\"width:100%\">";
                        foreach (var model in list_price)
                        {
                            var dis = (model.MarketPrice == 0 ? "0" : (model.Price / model.MarketPrice).ToString("0.00"));
                            price_list += $"<tr><td>{_localizer["原价"]}{model.CurrencyCode} : {model.MarketPrice}</td><td>{dis}</td><td>{_localizer["售价"]}{model.CurrencyCode} : {model.Price}</td></tr>";
                        }
                        var discount = (list_Course[i].MarketPrice == 0 ? "0" : (list_Course[i].Price / list_Course[i].MarketPrice).ToString("0.00"));
                        price_list += $"<tr><td>{_localizer["原价"]}USD : {list_Course[i].MarketPrice}</td><td>{discount}</td><td>{_localizer["售价"]}USD : {list_Course[i].Price}</td></tr>";
                        price_list += "</table>";
						var teacher = list_Teacher.FirstOrDefault(u => u.Teacherid == list_Course[i].Teacherid)?.Name + "";

						str.Append("{");
                        str.Append("\"RecordCourseid\":\"" + list_Course[i].RecordCourseid + "\",");
                        str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_CourseLang?.Title + "") + "\",");
                        str.Append("\"Img\":\"" + list_Course[i].Img + "\",");
                        str.Append("\"Dtime\":\"" + list_Course[i].Dtime.ToString() + "\",");
                        str.Append("\"Sort\":\"" + list_Course[i].Sort + "\",");
                        str.Append("\"Recommend\":" + list_Course[i].Recommend + ",");
                        str.Append("\"LessonCount\":\"" + list_Course[i].LessonCount + "\",");
                        str.Append("\"StudentCount\":\"" + list_Course[i].StudentCount + "\",");
						str.Append("\"Teacher\":\"" + JsonHelper.JsonCharFilter(teacher) + "\",");
						str.Append("\"PriceList\":\"" + JsonHelper.JsonCharFilter(price_list) + "\",");
                        str.Append("\"Sendtime\":\"" + list_Course[i].Sendtime + "\",");
                        str.Append("\"Status\":" + list_Course[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
                        if (i < (list_Course.Count - 1)) str.Append(",");
                    }
                    str.Append("]}");

                    break;
            }
            return Content(str.ToString());
        }
        #endregion

        #region "录播课程修改"
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RecordListAdd(long RecordCourseid = 0)
        {
            var vm_ListAdd = new ViewModels.Course.RecordListAdd();
            vm_ListAdd.ListPubCurrency = new List<PubCurrencyPrice>();
            var list_ViewPubCurrency = _context.Queryable<ViewPubCurrency>().Where(u => u.Status == 1 && u.Isuse == 1 && u.Lang == Lang).OrderBy(u => u.Sort).OrderBy(u => u.PubCurrencyid, OrderByType.Desc).ToList();
            var list_SkuPrice = _context.Queryable<WebRecordCoursePrice>().Where(u => u.RecordCourseid == RecordCourseid && list_ViewPubCurrency.Select(s => s.PubCurrencyid).Contains(u.PubCurrencyid)).ToList();
            list_ViewPubCurrency.ForEach(model =>
            {
                var model_SkuPrice = list_SkuPrice.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
                vm_ListAdd.ListPubCurrency.Add(new PubCurrencyPrice()
                {
                    PubCurrencyid = model.PubCurrencyid,
                    Country = model.Country,
                    CountryCode = model.CountryCode,
                    CurrencyCode = model.CurrencyCode,
                    Price = (model_SkuPrice != null) ? model_SkuPrice.Price : 0,
                    MarketPrice = (model_SkuPrice != null) ? model_SkuPrice.MarketPrice : 0
                });
            });
			vm_ListAdd.TeacherItem = _context.Queryable<WebTeacher>().LeftJoin<WebTeacherLang>((l, r) => l.Teacherid == r.Teacherid && r.Lang == Lang)
				.Where((l, r) => l.Status == 1).Select((l, r) => new { l.Teacherid, r.Name }).ToList().Select(u => new SelectListItem(u.Name, u.Teacherid.ToString())).ToList();
			vm_ListAdd.TeacherItem.Insert(0, new SelectListItem(_localizer["请关联老师"], "0"));

			var model_Course = _context.Queryable<WebRecordCourse>().InSingle(RecordCourseid);
            if (model_Course != null)
            {
                var model_CourseLang = _context.Queryable<WebRecordCourseLang>().First(u => u.RecordCourseid == RecordCourseid && u.Lang == Lang);
                if (model_CourseLang != null)
                {
                    vm_ListAdd.Title = model_CourseLang.Title;
                    vm_ListAdd.Message = model_CourseLang.Message;
                    vm_ListAdd.Keys = model_CourseLang.Keys;
                    vm_ListAdd.IntroUp = model_CourseLang.IntroUp;
                    if (JsonHelper.IsJson(model_CourseLang.IntroUp))
                    {
                        try
                        {
                            var o = JObject.Parse(model_CourseLang.IntroUp);
                            vm_ListAdd.EncryptDataUp = o["encrypt_data"].ToString();
                        }
                        catch { }
					}
					vm_ListAdd.IntroLow = model_CourseLang.IntroLow;
					if (JsonHelper.IsJson(model_CourseLang.IntroLow))
					{
						try
						{
							var o = JObject.Parse(model_CourseLang.IntroLow);
							vm_ListAdd.EncryptDataLow = o["encrypt_data"].ToString();
						}
						catch { }
					}
				}

                vm_ListAdd.RecordCourseid = RecordCourseid;
                vm_ListAdd.Img = model_Course.Img;
                vm_ListAdd.Banner = model_Course.Banner;
                vm_ListAdd.BannerH5 = model_Course.BannerH5;
                vm_ListAdd.Hits = model_Course.Hits;
                vm_ListAdd.LessonCount = model_Course.LessonCount;
                vm_ListAdd.StudentCount = model_Course.StudentCount;
                vm_ListAdd.Price = model_Course.Price;
                vm_ListAdd.MarketPrice = model_Course.MarketPrice;
				vm_ListAdd.Teacherid = model_Course.Teacherid;
				vm_ListAdd.Recommend = model_Course.Recommend;

				vm_ListAdd.Sort = model_Course.Sort;
                vm_ListAdd.Dtime = model_Course.Dtime;
                vm_ListAdd.Status = model_Course.Status;
                vm_ListAdd.Sendtime = model_Course.Sendtime;
                vm_ListAdd.Remark = model_Course.Remark;
            }
            else
            {
                vm_ListAdd.StudentCount = 0;
                vm_ListAdd.Status = 1;
                vm_ListAdd.Sort = 100;
                vm_ListAdd.Sendtime = DateTime.Now;
            }
            vm_ListAdd.Lang = Lang;
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View(vm_ListAdd);
        }

        //保存添改
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RecordListAdd(RecordListAdd vm_ListAdd)
        {
            string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/RecordCourse/" + DateTime.Now.ToString("yyyyMMdd") + "/";
            int chk_thumb = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_thumb", 0);
            int chk_banner = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner", 0);
            int chk_banner_h5 = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner_h5", 0);
            var old_imgs = GSRequestHelper.GetString(_accessor.HttpContext.Request, "input_old_imgs").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
            var files = _accessor.HttpContext.Request.Form.Files;
            var list_imgs = new List<string>();
            string ImgRoot = Appsettings.app("Web:ImgRoot");
            string ext = "";//扩展名
            var model_master = _tokenManager.GetAdminInfo();
            string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改课程信息";
            var list_Price = new List<WebRecordCoursePrice>();

            var list_Currency = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList();
            var model_Course = _context.Queryable<WebRecordCourse>().InSingle(vm_ListAdd.RecordCourseid);
            if (model_Course != null)
            {
                var model_CourseLang = _context.Queryable<WebRecordCourseLang>().First(u => u.Lang == vm_ListAdd.Lang && u.RecordCourseid == vm_ListAdd.RecordCourseid);
                if (model_CourseLang?.Title != vm_ListAdd.Title)
                {
                    message += ",将课程名称由[" + model_CourseLang?.Title + "]改为[" + vm_ListAdd.Title + "]";
                }
                if (model_CourseLang?.Message != vm_ListAdd.Message)
                {
                    message += ",将简介由[" + model_CourseLang?.Message + "]改为[" + vm_ListAdd.Message + "]";
                }
                if (model_Course.LessonCount != vm_ListAdd.LessonCount)
                {
                    message += ",将课节数由[" + model_Course.LessonCount + "]改为[" + vm_ListAdd.LessonCount + "]";
                }
                if (model_Course.StudentCount != vm_ListAdd.StudentCount)
                {
                    message += ",将学习人数由[" + model_Course.StudentCount + "]改为[" + vm_ListAdd.StudentCount + "]";
                }
                if (model_Course.Sort != vm_ListAdd.Sort)
                {
                    message += ",将排序字段由[" + model_Course.Sort + "]改为[" + vm_ListAdd.Sort + "]";
                }
                if (model_Course.Sendtime != vm_ListAdd.Sendtime)
                {
                    message += ",将发布时间由[" + model_Course.Sendtime + "]改为[" + vm_ListAdd.Sendtime + "]";
				}
				if (model_Course.Status != vm_ListAdd.Status)
				{
					message += ",将状态由[" + model_Course.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (model_Course.Teacherid != vm_ListAdd.Teacherid)
				{
					message += ",将关联老师ID由[" + model_Course.Teacherid + "]改为[" + vm_ListAdd.Teacherid + "]";
                }
                if (model_Course.Recommend != vm_ListAdd.Recommend)
                {
                    message += ",将推荐状态由[" + model_Course.Recommend + "]改为[" + vm_ListAdd.Recommend + "]";
                }
                if (model_CourseLang != null)
                {
                    model_CourseLang.Title = vm_ListAdd.Title;
                    model_CourseLang.Keys = vm_ListAdd.Keys;
                    model_CourseLang.Message = vm_ListAdd.Message;
					model_CourseLang.IntroUp = vm_ListAdd.IntroUp;
					model_CourseLang.IntroLow = vm_ListAdd.IntroLow;
					_context.Updateable(model_CourseLang).ExecuteCommand();
                }
                else
                {
                    model_CourseLang = new WebRecordCourseLang();
                    model_CourseLang.RecordCourseid = vm_ListAdd.RecordCourseid;
                    model_CourseLang.Title = vm_ListAdd.Title;
                    model_CourseLang.Keys = vm_ListAdd.Keys;
                    model_CourseLang.Message = vm_ListAdd.Message;
					model_CourseLang.IntroUp = vm_ListAdd.IntroUp;
					model_CourseLang.IntroLow = vm_ListAdd.IntroLow;
					model_CourseLang.Lang = vm_ListAdd.Lang;
                    _context.Insertable(model_CourseLang).ExecuteCommand();
				}
				model_Course.Recommend = vm_ListAdd.Recommend;
				model_Course.Sort = vm_ListAdd.Sort;
				model_Course.Sendtime = vm_ListAdd.Sendtime;
                model_Course.Status = vm_ListAdd.Status;
                model_Course.Masterid = model_master.AdminMasterid;
                model_Course.Remark = message + "<hr>" + vm_ListAdd.Remark;
                model_Course.LessonCount = vm_ListAdd.LessonCount;
				model_Course.StudentCount = vm_ListAdd.StudentCount;
				model_Course.Teacherid = vm_ListAdd.Teacherid;
				#region "上传"
				if (chk_thumb == 1)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(model_Course.Img)) FileHelper.FileDel(ImgRoot + model_Course.Img);
                    }
                    catch { }
                    model_Course.Img = "";
                }
                if (chk_banner == 1)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(model_Course.Banner)) FileHelper.FileDel(ImgRoot + model_Course.Banner);
                    }
                    catch { }
                    model_Course.Banner = "";
                }
                if (chk_banner_h5 == 1)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(model_Course.BannerH5)) FileHelper.FileDel(ImgRoot + model_Course.BannerH5);
                    }
                    catch { }
                    model_Course.BannerH5 = "";
                }
                if (files.Count > 0)
                {
                    //缩略图
                    var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
                    if (formFile != null)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(model_Course.Img)) FileHelper.FileDel(ImgRoot + model_Course.Img);
                        }
                        catch
                        {
                        }
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名
                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.Img = fileurl + filename;
                        }
                    }
                    formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
                    if (formFile != null)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(model_Course.Banner)) FileHelper.FileDel(ImgRoot + model_Course.Banner);
                        }
                        catch
                        {
                        }
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名
                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.Banner = fileurl + filename;
                        }
                    }
                    formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
                    if (formFile != null)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(model_Course.BannerH5)) FileHelper.FileDel(ImgRoot + model_Course.BannerH5);
                        }
                        catch
                        {
                        }
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名
                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.BannerH5 = fileurl + filename;
                        }
                    }
                }
                #endregion

                list_Price = _context.Queryable<WebRecordCoursePrice>().Where(u => u.RecordCourseid == vm_ListAdd.RecordCourseid).ToList();
                foreach (var model in list_Currency)
                {
                    var model_Price = list_Price.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
                    if (model_Price != null)
                    {
                        model_Price.Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M);
                        model_Price.MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M);
                        model_Price.CountryCode = model.CountryCode;
                        model_Price.CurrencyCode = model.CurrencyCode;
                    }
                    else
                    {
                        list_Price.Add(new WebRecordCoursePrice()
                        {
                            RecordCourseid = vm_ListAdd.RecordCourseid,
                            PubCurrencyid = model.PubCurrencyid,
                            Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M),
                            MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M),
                            CountryCode = model.CountryCode,
                            CurrencyCode = model.CurrencyCode
                        });
                    }
                }
                var x = _context.Storageable(list_Price).ToStorage();
                x.AsInsertable.ExecuteCommand();
                x.AsUpdateable.ExecuteCommand();
                model_Course.Price = vm_ListAdd.Price;
                model_Course.MarketPrice = vm_ListAdd.MarketPrice;
                _context.Updateable(model_Course).ExecuteCommand();

                return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?RecordCourseid=" + vm_ListAdd.RecordCourseid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
            else
            {
                model_Course = new WebRecordCourse();
				model_Course.Recommend = vm_ListAdd.Recommend;
				model_Course.Sort = vm_ListAdd.Sort;
                model_Course.Sendtime = vm_ListAdd.Sendtime;
                model_Course.Status = vm_ListAdd.Status;
                model_Course.Masterid = model_master.AdminMasterid;
                model_Course.Remark = message + "<hr>" + vm_ListAdd.Remark;
                model_Course.LessonCount = vm_ListAdd.LessonCount;
                model_Course.StudentCount = vm_ListAdd.StudentCount;
				model_Course.Price = vm_ListAdd.Price;
				model_Course.MarketPrice = vm_ListAdd.MarketPrice;
				model_Course.Teacherid = vm_ListAdd.Teacherid;
				#region "上传"
				if (files.Count > 0)
                {
                    var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
                    if (formFile != null)
                    {
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名

                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.Img = fileurl + filename;
                        }
                    }
                    formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
                    if (formFile != null)
                    {
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名

                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.Banner = fileurl + filename;
                        }
                    }
                    formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
                    if (formFile != null)
                    {
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名

                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.BannerH5 = fileurl + filename;
                        }
                    }
                }
                #endregion
                vm_ListAdd.RecordCourseid = _context.Insertable(model_Course).ExecuteReturnBigIdentity();

                var model_CourseLang = new WebRecordCourseLang();
                model_CourseLang.Lang = vm_ListAdd.Lang;
                model_CourseLang.RecordCourseid = vm_ListAdd.RecordCourseid;
                model_CourseLang.Title = vm_ListAdd.Title;
                model_CourseLang.Keys = vm_ListAdd.Keys;
                model_CourseLang.Message = vm_ListAdd.Message;
				model_CourseLang.IntroUp = vm_ListAdd.IntroUp;
				model_CourseLang.IntroLow = vm_ListAdd.IntroLow;
				_context.Insertable(model_CourseLang).ExecuteCommand();

                list_Price = _context.Queryable<WebRecordCoursePrice>().Where(u => u.RecordCourseid == vm_ListAdd.RecordCourseid).ToList();
                foreach (var model in list_Currency)
                {
                    var model_Price = list_Price.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
                    if (model_Price != null)
                    {
                        model_Price.Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M);
                        model_Price.MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M);
                        model_Price.CountryCode = model.CountryCode;
                        model_Price.CurrencyCode = model.CurrencyCode;
                    }
                    else
                    {
                        list_Price.Add(new WebRecordCoursePrice()
                        {
                            RecordCourseid = vm_ListAdd.RecordCourseid,
                            PubCurrencyid = model.PubCurrencyid,
                            Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M),
                            MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M),
                            CountryCode = model.CountryCode,
                            CurrencyCode = model.CurrencyCode
                        });
                    }
                }
                var x = _context.Storageable(list_Price).ToStorage();
                x.AsInsertable.ExecuteCommand();
                x.AsUpdateable.ExecuteCommand();

                return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?RecordCourseid=" + vm_ListAdd.RecordCourseid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
        }
        #endregion

        #region "录播课程视频列表"
        [Authorization(Power = "Main")]
        public IActionResult RecordInfoList(long RecordCourseid)
        {
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
			ViewBag.RecordCourseid = RecordCourseid;
            return View();
        }

        [HttpGet]
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RecordInfoListData(long RecordCourseid, string keys = "", string status = "", string recommend = "", string begintime = "", string endtime = "")
        {
            StringBuilder str = new StringBuilder();
            int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
            int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
            string RecordCourseids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "RecordCourseInfoids");
            long RecordCourseInfoid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "RecordCourseInfoid", 0);
            var list_RecordInfo = new List<WebRecordCourseInfo>();
            if (RecordCourseids.Trim() == "") RecordCourseids = "0";
            string[] arr = RecordCourseids.Split(',');
            int count = 0;
            switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
            {
                case "del":
                    count = _context.Updateable<WebRecordCourseInfo>()
                        .SetColumns(u => u.Status == -1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.RecordCourseid) || u.RecordCourseid == RecordCourseInfoid)
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
                    count = _context.Updateable<WebRecordCourseInfo>()
                        .SetColumns(u => u.Status == 1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.RecordCourseid) || u.RecordCourseid == RecordCourseInfoid)
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
                    count = _context.Updateable<WebRecordCourseInfo>()
                        .SetColumns(u => u.Status == 0)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.RecordCourseid) || u.RecordCourseid == RecordCourseInfoid)
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
                    var exp = new Expressionable<WebRecordCourseInfo>();
                    exp.And(u => u.Status != -1 && u.RecordCourseid==RecordCourseid);
                    exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebRecordCourseInfoLang>().Where(s => s.RecordCourseid == u.RecordCourseid && SqlFunc.MergeString(s.Title, "").Contains(keys.Trim())).Any());
                    exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= DateTime.Parse(begintime));
                    exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Dtime <= DateTime.Parse(endtime));
                    var query = _context.Queryable<WebRecordCourseInfo>().Where(exp.ToExpression());
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
                    query.OrderBy("sort,record_course_infoid desc");
                    list_RecordInfo = query.ToPageList(page, pagesize, ref total);
                    var list_RecordInfoLang = _context.Queryable<WebRecordCourseInfoLang>().Where(u => list_RecordInfo.Select(s => s.RecordCourseid).Contains(u.RecordCourseid) && u.Lang == Lang).ToList();

                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
                    for (int i = 0; i < list_RecordInfo.Count; i++)
                    {
                        var model_RecordInfoLang = list_RecordInfoLang.FirstOrDefault(u => u.RecordCourseInfoid == list_RecordInfo[i].RecordCourseInfoid);
                
                        str.Append("{");
                        str.Append("\"RecordCourseInfoid\":\"" + list_RecordInfo[i].RecordCourseInfoid + "\",");
                        str.Append("\"RecordCourseid\":\"" + list_RecordInfo[i].RecordCourseid + "\",");
                        str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_RecordInfoLang?.Title + "") + "\",");
                        str.Append("\"Duration\":\"" + JsonHelper.JsonCharFilter(list_RecordInfo[i].Duration + "") + "\",");
                        str.Append("\"ViewCount\":\"" + list_RecordInfo[i].ViewCount + "\",");
                        str.Append("\"Video\":\"" + _commonBaseService.ResourceDomain(list_RecordInfo[i].Video) + "\",");
                        str.Append("\"Dtime\":\"" + list_RecordInfo[i].Dtime.ToString() + "\",");
                        str.Append("\"Sort\":\"" + list_RecordInfo[i].Sort + "\",");
                        str.Append("\"Status\":" + list_RecordInfo[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
                        if (i < (list_RecordInfo.Count - 1)) str.Append(",");
                    }
                    str.Append("]}");

                    break;
            }
            return Content(str.ToString());
        }
        #endregion

        #region "录播课程视频修改"
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RecordInfoListAdd(long RecordCourseInfoid = 0,long RecordCourseid=0 )
        {
            var vm_ListAdd = new RecordInfoListAdd();
            var model_RecordInfo = _context.Queryable<WebRecordCourseInfo>().InSingle(RecordCourseInfoid);
            if (model_RecordInfo != null)
            {
                var model_CourseLang = _context.Queryable<WebRecordCourseInfoLang>().First(u => u.RecordCourseInfoid == RecordCourseInfoid && u.Lang == Lang);
                if (model_CourseLang != null)
                {
                    vm_ListAdd.Title = model_CourseLang.Title;
                }

                vm_ListAdd.RecordCourseInfoid = model_RecordInfo.RecordCourseInfoid;
                vm_ListAdd.RecordCourseid = model_RecordInfo.RecordCourseid;
                vm_ListAdd.Duration = model_RecordInfo.Duration;
				vm_ListAdd.ViewCount = model_RecordInfo.ViewCount;
				vm_ListAdd.Video = _commonBaseService.ResourceDomain(model_RecordInfo.Video);
				vm_ListAdd.Sort = model_RecordInfo.Sort;
				vm_ListAdd.Hits = model_RecordInfo.Hits;
				vm_ListAdd.Dtime = model_RecordInfo.Dtime;
                vm_ListAdd.Status = model_RecordInfo.Status;
            }
            else
			{
				vm_ListAdd.RecordCourseid = RecordCourseInfoid;
				vm_ListAdd.Status = 1;
                vm_ListAdd.Sort = 100;
            }
            vm_ListAdd.Lang = Lang;
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View(vm_ListAdd);
        }

        //保存添改
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RecordInfoListAdd(RecordInfoListAdd vm_ListAdd)
        {
            string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/RecordCourse/" + DateTime.Now.ToString("yyyyMMdd") + "/";
            int chk_thumb = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_thumb", 0);
            int chk_banner = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner", 0);
            int chk_banner_h5 = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner_h5", 0);
            var old_imgs = GSRequestHelper.GetString(_accessor.HttpContext.Request, "input_old_imgs").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
            var files = _accessor.HttpContext.Request.Form.Files;
            var list_imgs = new List<string>();
            string ImgRoot = Appsettings.app("Web:ImgRoot");
            string ext = "";//扩展名
            var model_master = _tokenManager.GetAdminInfo();
            string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改课程信息";
            var list_Price = new List<WebRecordCoursePrice>();

            var list_Currency = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList();
            var model_RecordInfo = _context.Queryable<WebRecordCourseInfo>().InSingle(vm_ListAdd.RecordCourseInfoid);
            if (model_RecordInfo != null)
            {
                var model_RecordInfoLang = _context.Queryable<WebRecordCourseInfoLang>().First(u => u.Lang == vm_ListAdd.Lang && u.RecordCourseid == vm_ListAdd.RecordCourseid);
                if (model_RecordInfoLang?.Title != vm_ListAdd.Title)
                {
                    message += ",将课程名称由[" + model_RecordInfoLang?.Title + "]改为[" + vm_ListAdd.Title + "]";
                }
                if (model_RecordInfo.ViewCount != vm_ListAdd.ViewCount)
                {
                    message += ",将学习人数由[" + model_RecordInfo.ViewCount + "]改为[" + vm_ListAdd.ViewCount + "]";
                }
                if (model_RecordInfo.Duration != vm_ListAdd.Duration)
                {
                    message += ",将视频时长由[" + model_RecordInfo.Duration + "]改为[" + vm_ListAdd.Duration + "]";
                }
                if (model_RecordInfo.Sort != vm_ListAdd.Sort)
                {
                    message += ",将排序字段由[" + model_RecordInfo.Sort + "]改为[" + vm_ListAdd.Sort + "]";
                }
                if (model_RecordInfo.Status != vm_ListAdd.Status)
                {
                    message += ",将状态由[" + model_RecordInfo.Status + "]改为[" + vm_ListAdd.Status + "]";
                }
                if (model_RecordInfoLang != null)
                {
                    model_RecordInfoLang.Title = vm_ListAdd.Title;
                    _context.Updateable(model_RecordInfoLang).ExecuteCommand();
                }
                else
                {
                    model_RecordInfoLang = new WebRecordCourseInfoLang();
                    model_RecordInfoLang.RecordCourseid = vm_ListAdd.RecordCourseid;
                    model_RecordInfoLang.RecordCourseInfoid = vm_ListAdd.RecordCourseInfoid;
                    model_RecordInfoLang.Title = vm_ListAdd.Title;
                    model_RecordInfoLang.Lang = vm_ListAdd.Lang;
                    _context.Insertable(model_RecordInfoLang).ExecuteCommand();
                }
				model_RecordInfo.Duration = vm_ListAdd.Duration;
				model_RecordInfo.ViewCount = vm_ListAdd.ViewCount;
				model_RecordInfo.Sort = vm_ListAdd.Sort;
				model_RecordInfo.Status = vm_ListAdd.Status;
				model_RecordInfo.Video = _commonBaseService.ResourceVar(vm_ListAdd.Video);
				_context.Updateable(model_RecordInfo).ExecuteCommand();

                return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?RecordCourseid=" + vm_ListAdd.RecordCourseid + "&RecordCourseInfoid=" + vm_ListAdd.RecordCourseInfoid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
            else
            {
                model_RecordInfo = new WebRecordCourseInfo();
                model_RecordInfo.RecordCourseid = vm_ListAdd.RecordCourseid;
                model_RecordInfo.Duration = vm_ListAdd.Duration;
				model_RecordInfo.ViewCount = vm_ListAdd.ViewCount;
				model_RecordInfo.Sort = vm_ListAdd.Sort;
				model_RecordInfo.Status = vm_ListAdd.Status;
				model_RecordInfo.Video = _commonBaseService.ResourceVar(vm_ListAdd.Video);
				vm_ListAdd.RecordCourseInfoid = _context.Insertable(model_RecordInfo).ExecuteReturnBigIdentity();

                var model_RecordInfoLang = new WebRecordCourseInfoLang();
                model_RecordInfoLang.Lang = vm_ListAdd.Lang;
                model_RecordInfoLang.RecordCourseid = vm_ListAdd.RecordCourseid;
                model_RecordInfoLang.RecordCourseInfoid = vm_ListAdd.RecordCourseInfoid;
                model_RecordInfoLang.Title = vm_ListAdd.Title;
                _context.Insertable(model_RecordInfoLang).ExecuteCommand();


                return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?RecordCourseid=" + vm_ListAdd.RecordCourseid + "&RecordCourseInfoid=" + vm_ListAdd.RecordCourseInfoid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
        }
        #endregion

        #region "线下课程列表"
        [Authorization(Power = "Main")]
        public IActionResult OfflineList()
        {
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View();
        }

        [HttpGet]
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> OfflineListData(string keys = "", string status = "", string recommend = "", string begintime = "", string endtime = "")
        {
            StringBuilder str = new StringBuilder();
            int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
            int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
            string OfflineCourseids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "OfflineCourseids");
            long OfflineCourseid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "OfflineCourseid", 0);
            var list_Course = new List<WebOfflineCourse>();
            if (OfflineCourseids.Trim() == "") OfflineCourseids = "0";
            string[] arr = OfflineCourseids.Split(',');
            int count = 0;
            switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
            {
                case "del":
                    count = _context.Updateable<WebOfflineCourse>()
                        .SetColumns(u => u.Status == -1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.OfflineCourseid) || u.OfflineCourseid == OfflineCourseid)
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
                    count = _context.Updateable<WebOfflineCourse>()
                        .SetColumns(u => u.Status == 1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.OfflineCourseid) || u.OfflineCourseid == OfflineCourseid)
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
                    count = _context.Updateable<WebOfflineCourse>()
                        .SetColumns(u => u.Status == 0)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.OfflineCourseid) || u.OfflineCourseid == OfflineCourseid)
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
                    string price_list = "";
                    int total = 0;
                    var exp = new Expressionable<WebOfflineCourse>();
                    exp.And(u => u.Status != -1);
                    exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebOfflineCourseLang>().Where(s => s.OfflineCourseid == u.OfflineCourseid && SqlFunc.MergeString(s.Title, "").Contains(keys.Trim()) || SqlFunc.MergeString(s.Message, "").Contains(keys.Trim())).Any());
                    exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(recommend), u => u.Recommend.ToString() == recommend.Trim());
                    exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Sendtime >= DateTime.Parse(begintime));
                    exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Sendtime <= DateTime.Parse(endtime));
                    var query = _context.Queryable<WebOfflineCourse>().Where(exp.ToExpression());
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
                    query.OrderBy("sort,sendtime desc,Offline_courseid desc");
                    list_Course = query.ToPageList(page, pagesize, ref total);
                    var list_CourseLang = _context.Queryable<WebOfflineCourseLang>().Where(u => list_Course.Select(s => s.OfflineCourseid).Contains(u.OfflineCourseid) && u.Lang == Lang).ToList();
                    var list_Teacher = _context.Queryable<WebTeacherLang>().Where(u => list_Course.Select(s => s.Teacherid).Contains(u.Teacherid) && u.Lang == Lang).ToList();

                    var list_OfflineCoursePrice = _context.Queryable<WebOfflineCoursePrice>()
                        .LeftJoin<PubCurrency>((l, r) => l.PubCurrencyid == r.PubCurrencyid)
                        .Where((l, r) => list_Course.Select(s => s.OfflineCourseid).Contains(l.OfflineCourseid)).OrderBy((l, r) => r.Sort).ToList();
                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
                    for (int i = 0; i < list_Course.Count; i++)
                    {
                        price_list = "";
                        var model_CourseLang = list_CourseLang.FirstOrDefault(u => u.OfflineCourseid == list_Course[i].OfflineCourseid);
                        var list_price = list_OfflineCoursePrice.Where(u => u.OfflineCourseid == list_Course[i].OfflineCourseid).ToList();
                        price_list += "<table style=\"width:100%\">";
                        foreach (var model in list_price)
                        {
                            var dis = (model.MarketPrice == 0 ? "0" : (model.Price / model.MarketPrice).ToString("0.00"));
                            price_list += $"<tr><td>{_localizer["原价"]}{model.CurrencyCode} : {model.MarketPrice}</td><td>{dis}</td><td>{_localizer["售价"]}{model.CurrencyCode} : {model.Price}</td></tr>";
                        }
                        var discount = (list_Course[i].MarketPrice == 0 ? "0" : (list_Course[i].Price / list_Course[i].MarketPrice).ToString("0.00"));
                        price_list += $"<tr><td>{_localizer["原价"]}USD : {list_Course[i].MarketPrice}</td><td>{discount}</td><td>{_localizer["售价"]}USD : {list_Course[i].Price}</td></tr>";
                        price_list += "</table>";
                        var teacher = list_Teacher.FirstOrDefault(u => u.Teacherid == list_Course[i].Teacherid)?.Name + "";


                        str.Append("{");
                        str.Append("\"OfflineCourseid\":\"" + list_Course[i].OfflineCourseid + "\",");
                        str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_CourseLang?.Title + "") + "\",");
                        str.Append("\"Img\":\"" + list_Course[i].Img + "\",");
                        str.Append("\"Dtime\":\"" + list_Course[i].Dtime.ToString() + "\",");
                        str.Append("\"Sort\":\"" + list_Course[i].Sort + "\",");
                        str.Append("\"Recommend\":" + list_Course[i].Recommend + ",");
                        str.Append("\"LessonCount\":\"" + list_Course[i].LessonCount + "\",");
                        str.Append("\"StudentCount\":\"" + list_Course[i].StudentCount + "\",");
                        str.Append("\"LessonStart\":\"" + list_Course[i].LessonStart + "\",");
                        str.Append("\"PriceList\":\"" + JsonHelper.JsonCharFilter(price_list) + "\",");
                        str.Append("\"Teacher\":\"" + JsonHelper.JsonCharFilter(teacher) + "\",");
                        str.Append("\"Sendtime\":\"" + list_Course[i].Sendtime + "\",");
                        str.Append("\"Status\":" + list_Course[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
                        if (i < (list_Course.Count - 1)) str.Append(",");
                    }
                    str.Append("]}");

                    break;
            }
            return Content(str.ToString());
        }
        #endregion

        #region "线下课程修改"
        [Authorization(Power = "Main")]
        public async Task<IActionResult> OfflineListAdd(long OfflineCourseid = 0)
        {
            var vm_ListAdd = new OfflineListAdd();
            vm_ListAdd.ListPubCurrency = new List<PubCurrencyPrice>();
            var list_ViewPubCurrency = _context.Queryable<ViewPubCurrency>().Where(u => u.Status == 1 && u.Isuse == 1 && u.Lang == Lang).OrderBy(u => u.Sort).OrderBy(u => u.PubCurrencyid, OrderByType.Desc).ToList();
            var list_SkuPrice = _context.Queryable<WebOfflineCoursePrice>().Where(u => u.OfflineCourseid == OfflineCourseid && list_ViewPubCurrency.Select(s => s.PubCurrencyid).Contains(u.PubCurrencyid)).ToList();
            list_ViewPubCurrency.ForEach(model =>
            {
                var model_SkuPrice = list_SkuPrice.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
                vm_ListAdd.ListPubCurrency.Add(new PubCurrencyPrice()
                {
                    PubCurrencyid = model.PubCurrencyid,
                    Country = model.Country,
                    CountryCode = model.CountryCode,
                    CurrencyCode = model.CurrencyCode,
                    Price = (model_SkuPrice != null) ? model_SkuPrice.Price : 0,
                    MarketPrice = (model_SkuPrice != null) ? model_SkuPrice.MarketPrice : 0
                });
            });
            vm_ListAdd.TeacherItem = _context.Queryable<WebTeacher>().LeftJoin<WebTeacherLang>((l, r) => l.Teacherid == r.Teacherid && r.Lang == Lang)
                .Where((l, r) => l.Status == 1).Select((l, r) => new { l.Teacherid, r.Name }).ToList().Select(u => new SelectListItem(u.Name, u.Teacherid.ToString())).ToList();
            vm_ListAdd.TeacherItem.Insert(0, new SelectListItem(_localizer["请关联老师"], "0"));

            var model_Course = _context.Queryable<WebOfflineCourse>().InSingle(OfflineCourseid);
            if (model_Course != null)
            {
                var model_CourseLang = _context.Queryable<WebOfflineCourseLang>().First(u => u.OfflineCourseid == OfflineCourseid && u.Lang == Lang);
                if (model_CourseLang != null)
                {
                    vm_ListAdd.Title = model_CourseLang.Title;
                    vm_ListAdd.Message = model_CourseLang.Message;
                    vm_ListAdd.Keys = model_CourseLang.Keys;
                    vm_ListAdd.Intro = model_CourseLang.Intro;
					vm_ListAdd.Address = model_CourseLang.Address;
					if (JsonHelper.IsJson(model_CourseLang.Intro))
                    {
                        try
                        {
                            var o = JObject.Parse(model_CourseLang.Intro);
                            vm_ListAdd.EncryptData = o["encrypt_data"].ToString();
                        }
                        catch { }
                    }
                }

                vm_ListAdd.OfflineCourseid = OfflineCourseid;
                vm_ListAdd.Img = model_Course.Img;
                vm_ListAdd.Banner = model_Course.Banner;
                vm_ListAdd.BannerH5 = model_Course.BannerH5;
                vm_ListAdd.Hits = model_Course.Hits;
                vm_ListAdd.LessonCount = model_Course.LessonCount;
                vm_ListAdd.LessonStart = model_Course.LessonStart;
                vm_ListAdd.Price = model_Course.Price;
                vm_ListAdd.MarketPrice = model_Course.MarketPrice;
                vm_ListAdd.Teacherid = model_Course.Teacherid;
                vm_ListAdd.Recommend = model_Course.Recommend;
                vm_ListAdd.StudentCount = model_Course.StudentCount;

                vm_ListAdd.Sort = model_Course.Sort;
                vm_ListAdd.Dtime = model_Course.Dtime;
                vm_ListAdd.Status = model_Course.Status;
                vm_ListAdd.Sendtime = model_Course.Sendtime;
                vm_ListAdd.Remark = model_Course.Remark;
            }
            else
            {
                vm_ListAdd.LessonStart = DateTime.Now.ToString();
                vm_ListAdd.Status = 1;
                vm_ListAdd.Sort = 100;
                vm_ListAdd.Sendtime = DateTime.Now;
            }
            vm_ListAdd.Lang = Lang;
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            return View(vm_ListAdd);
        }

        //保存添改
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> OfflineListAdd(OfflineListAdd vm_ListAdd)
        {
            string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/OfflineCourse/" + DateTime.Now.ToString("yyyyMMdd") + "/";
            int chk_thumb = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_thumb", 0);
            int chk_banner = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner", 0);
            int chk_banner_h5 = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_banner_h5", 0);
            var old_imgs = GSRequestHelper.GetString(_accessor.HttpContext.Request, "input_old_imgs").Split(",").Where(u => !string.IsNullOrEmpty(u)).ToList();
            var files = _accessor.HttpContext.Request.Form.Files;
            var list_imgs = new List<string>();
            string ImgRoot = Appsettings.app("Web:ImgRoot");
            string ext = "";//扩展名
            var model_master = _tokenManager.GetAdminInfo();
            string message = "[" + DateTime.Now + "]" + model_master?.Username + "修改课程信息";
            var list_Price = new List<WebOfflineCoursePrice>();

            var list_Currency = _context.Queryable<PubCurrency>().Where(u => u.Status == 1).ToList();
            var model_Course = _context.Queryable<WebOfflineCourse>().InSingle(vm_ListAdd.OfflineCourseid);
            if (model_Course != null)
            {
                var model_CourseLang = _context.Queryable<WebOfflineCourseLang>().First(u => u.Lang == vm_ListAdd.Lang && u.OfflineCourseid == vm_ListAdd.OfflineCourseid);
                if (model_CourseLang?.Title != vm_ListAdd.Title)
                {
                    message += ",将课程名称由[" + model_CourseLang?.Title + "]改为[" + vm_ListAdd.Title + "]";
                }
                if (model_CourseLang?.Message != vm_ListAdd.Message)
                {
                    message += ",将简介由[" + model_CourseLang?.Message + "]改为[" + vm_ListAdd.Message + "]";
                }
                if (model_Course.LessonCount != vm_ListAdd.LessonCount)
                {
                    message += ",将课节数由[" + model_Course.LessonCount + "]改为[" + vm_ListAdd.LessonCount + "]";
                }
                if (model_Course.StudentCount != vm_ListAdd.StudentCount)
                {
                    message += ",将学习人数由[" + model_Course.StudentCount + "]改为[" + vm_ListAdd.StudentCount + "]";
                }
                if (model_Course.LessonStart != vm_ListAdd.LessonStart)
                {
                    message += ",将上课时间由[" + model_Course.LessonStart + "]改为[" + vm_ListAdd.LessonStart + "]";
                }
                if (model_Course.Sort != vm_ListAdd.Sort)
                {
                    message += ",将排序字段由[" + model_Course.Sort + "]改为[" + vm_ListAdd.Sort + "]";
                }
                if (model_Course.Sendtime != vm_ListAdd.Sendtime)
                {
                    message += ",将发布时间由[" + model_Course.Sendtime + "]改为[" + vm_ListAdd.Sendtime + "]";
                }
                if (model_Course.Status != vm_ListAdd.Status)
                {
                    message += ",将状态由[" + model_Course.Status + "]改为[" + vm_ListAdd.Status + "]";
                }
                if (model_Course.Recommend != vm_ListAdd.Recommend)
                {
                    message += ",将推荐状态由[" + model_Course.Recommend + "]改为[" + vm_ListAdd.Recommend + "]";
                }
                if (model_Course.Teacherid != vm_ListAdd.Teacherid)
                {
                    message += ",将老师ID由[" + model_Course.Teacherid + "]改为[" + vm_ListAdd.Teacherid + "]";
                }
                if (model_CourseLang != null)
                {
                    model_CourseLang.Title = vm_ListAdd.Title;
                    model_CourseLang.Keys = vm_ListAdd.Keys;
                    model_CourseLang.Message = vm_ListAdd.Message;
                    model_CourseLang.Intro = vm_ListAdd.Intro;
                    model_CourseLang.Address = vm_ListAdd.Address;
                    _context.Updateable(model_CourseLang).ExecuteCommand();
                }
                else
                {
                    model_CourseLang = new WebOfflineCourseLang();
                    model_CourseLang.OfflineCourseid = vm_ListAdd.OfflineCourseid;
                    model_CourseLang.Title = vm_ListAdd.Title;
                    model_CourseLang.Keys = vm_ListAdd.Keys;
                    model_CourseLang.Message = vm_ListAdd.Message;
                    model_CourseLang.Intro = vm_ListAdd.Intro;
                    model_CourseLang.Lang = vm_ListAdd.Lang;
					model_CourseLang.Address = vm_ListAdd.Address;
					_context.Insertable(model_CourseLang).ExecuteCommand();
                }
                model_Course.Recommend = vm_ListAdd.Recommend;
                model_Course.Sort = vm_ListAdd.Sort;
                model_Course.Sendtime = vm_ListAdd.Sendtime;
                model_Course.Status = vm_ListAdd.Status;
                model_Course.Masterid = model_master.AdminMasterid;
                model_Course.Remark = message + "<hr>" + vm_ListAdd.Remark;
                model_Course.LessonCount = vm_ListAdd.LessonCount;
                model_Course.LessonStart = vm_ListAdd.LessonStart;
                model_Course.Teacherid = vm_ListAdd.Teacherid;
                model_Course.StudentCount = vm_ListAdd.StudentCount;
                #region "上传"
                if (chk_thumb == 1)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(model_Course.Img)) FileHelper.FileDel(ImgRoot + model_Course.Img);
                    }
                    catch { }
                    model_Course.Img = "";
                }
                if (chk_banner == 1)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(model_Course.Banner)) FileHelper.FileDel(ImgRoot + model_Course.Banner);
                    }
                    catch { }
                    model_Course.Banner = "";
                }
                if (chk_banner_h5 == 1)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(model_Course.BannerH5)) FileHelper.FileDel(ImgRoot + model_Course.BannerH5);
                    }
                    catch { }
                    model_Course.BannerH5 = "";
                }
                if (files.Count > 0)
                {
                    //缩略图
                    var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
                    if (formFile != null)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(model_Course.Img)) FileHelper.FileDel(ImgRoot + model_Course.Img);
                        }
                        catch
                        {
                        }
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名
                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.Img = fileurl + filename;
                        }
                    }
                    formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
                    if (formFile != null)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(model_Course.Banner)) FileHelper.FileDel(ImgRoot + model_Course.Banner);
                        }
                        catch
                        {
                        }
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名
                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.Banner = fileurl + filename;
                        }
                    }
                    formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
                    if (formFile != null)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(model_Course.BannerH5)) FileHelper.FileDel(ImgRoot + model_Course.BannerH5);
                        }
                        catch
                        {
                        }
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名
                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.BannerH5 = fileurl + filename;
                        }
                    }
                }
                #endregion

                list_Price = _context.Queryable<WebOfflineCoursePrice>().Where(u => u.OfflineCourseid == vm_ListAdd.OfflineCourseid).ToList();
                foreach (var model in list_Currency)
                {
                    var model_Price = list_Price.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
                    if (model_Price != null)
                    {
                        model_Price.Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M);
                        model_Price.MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M);
                        model_Price.CountryCode = model.CountryCode;
                        model_Price.CurrencyCode = model.CurrencyCode;
                    }
                    else
                    {
                        list_Price.Add(new WebOfflineCoursePrice()
                        {
                            OfflineCourseid = vm_ListAdd.OfflineCourseid,
                            PubCurrencyid = model.PubCurrencyid,
                            Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M),
                            MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M),
                            CountryCode = model.CountryCode,
                            CurrencyCode = model.CurrencyCode
                        });
                    }
                }
                var x = _context.Storageable(list_Price).ToStorage();
                x.AsInsertable.ExecuteCommand();
                x.AsUpdateable.ExecuteCommand();
                model_Course.Price = vm_ListAdd.Price;
                model_Course.MarketPrice = vm_ListAdd.MarketPrice;
                _context.Updateable(model_Course).ExecuteCommand();

                return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?OfflineCourseid=" + vm_ListAdd.OfflineCourseid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
            else
            {
                model_Course = new WebOfflineCourse();
                model_Course.Recommend = vm_ListAdd.Recommend;
                model_Course.Sort = vm_ListAdd.Sort;
                model_Course.Sendtime = vm_ListAdd.Sendtime;
                model_Course.Status = vm_ListAdd.Status;
                model_Course.Masterid = model_master.AdminMasterid;
                model_Course.Remark = message + "<hr>" + vm_ListAdd.Remark;
                model_Course.LessonCount = vm_ListAdd.LessonCount;
                model_Course.LessonStart = vm_ListAdd.LessonStart;
                model_Course.Teacherid = vm_ListAdd.Teacherid;
                model_Course.Price = vm_ListAdd.Price;
                model_Course.MarketPrice = vm_ListAdd.MarketPrice;
				model_Course.StudentCount = vm_ListAdd.StudentCount;
				#region "上传"
				if (files.Count > 0)
                {
                    var formFile = files.FirstOrDefault(u => u.Name == "input_thumb" && u.Length > 0);
                    if (formFile != null)
                    {
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名

                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.Img = fileurl + filename;
                        }
                    }
                    formFile = files.FirstOrDefault(u => u.Name == "input_banner" && u.Length > 0);
                    if (formFile != null)
                    {
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名

                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.Banner = fileurl + filename;
                        }
                    }
                    formFile = files.FirstOrDefault(u => u.Name == "input_banner_h5" && u.Length > 0);
                    if (formFile != null)
                    {
                        FileHelper.AddFolder(ImgRoot + fileurl);
                        ext = System.IO.Path.GetExtension(formFile.FileName);
                        filename = Guid.NewGuid().ToString() + ext;//纯文件名

                        using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
                        {
                            await formFile.CopyToAsync(fs);
                            model_Course.BannerH5 = fileurl + filename;
                        }
                    }
                }
                #endregion
                vm_ListAdd.OfflineCourseid = _context.Insertable(model_Course).ExecuteReturnBigIdentity();

                var model_CourseLang = new WebOfflineCourseLang();
                model_CourseLang.Lang = vm_ListAdd.Lang;
                model_CourseLang.OfflineCourseid = vm_ListAdd.OfflineCourseid;
                model_CourseLang.Title = vm_ListAdd.Title;
                model_CourseLang.Keys = vm_ListAdd.Keys;
                model_CourseLang.Message = vm_ListAdd.Message;
                model_CourseLang.Intro = vm_ListAdd.Intro;
				model_CourseLang.Address = vm_ListAdd.Address;
				_context.Insertable(model_CourseLang).ExecuteCommand();

                list_Price = _context.Queryable<WebOfflineCoursePrice>().Where(u => u.OfflineCourseid == vm_ListAdd.OfflineCourseid).ToList();
                foreach (var model in list_Currency)
                {
                    var model_Price = list_Price.FirstOrDefault(u => u.PubCurrencyid == model.PubCurrencyid);
                    if (model_Price != null)
                    {
                        model_Price.Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M);
                        model_Price.MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M);
                        model_Price.CountryCode = model.CountryCode;
                        model_Price.CurrencyCode = model.CurrencyCode;
                    }
                    else
                    {
                        list_Price.Add(new WebOfflineCoursePrice()
                        {
                            OfflineCourseid = vm_ListAdd.OfflineCourseid,
                            PubCurrencyid = model.PubCurrencyid,
                            Price = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_price_" + model.PubCurrencyid, 0.00M),
                            MarketPrice = GSRequestHelper.GetDecimal(_accessor.HttpContext.Request, "input_market_price_" + model.PubCurrencyid, 0.00M),
                            CountryCode = model.CountryCode,
                            CurrencyCode = model.CurrencyCode
                        });
                    }
                }
                var x = _context.Storageable(list_Price).ToStorage();
                x.AsInsertable.ExecuteCommand();
                x.AsUpdateable.ExecuteCommand();

                return Content("<script>alert('保存信息成功');parent.$('#table_view').datagrid('reload');location.href='?OfflineCourseid=" + vm_ListAdd.OfflineCourseid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
        }
        #endregion
    }
}