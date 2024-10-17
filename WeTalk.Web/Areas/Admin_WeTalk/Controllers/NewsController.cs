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
using Aliyun.OSS;
using WeTalk.Interfaces.Services;
using Newtonsoft.Json;
using System.Text.Json;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class NewsController : Base.BaseController
	{

		private readonly TokenManager _tokenManager;
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		private readonly IPubConfigService _pubConfigService;
		private readonly ICommonService _commonService;
		public NewsController(TokenManager tokenManager, IStringLocalizer<LangResource> localizer, SqlSugarScope context, IHttpContextAccessor accessor,
			IPubConfigService pubConfigService, ICommonService commonService
			)
			: base(tokenManager)
		{
			_tokenManager = tokenManager;
			_context = context;
			_accessor = accessor;
			_localizer = localizer;
			_pubConfigService = pubConfigService;
			_commonService = commonService;
		}


        #region "分类列表"    
        [Authorization(Power = "Main")]
        public IActionResult Category(string type = "")
		{
			var list_Lang = _context.Queryable<PubLanguage>().Where(u => u.Status == 1).ToList();
			string str = "";
			foreach (var item in list_Lang) {
				str += $"<option value=\"{item.Lang}\">{item.Title}</option>";
			}
			ViewBag.Langs = str;
            ViewBag.Type = type;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CategoryData(string keys = "", string langs = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string NewsCategoryids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "NewsCategoryids");
			long NewsCategoryid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "NewsCategoryid", 0);
			List<WebNewsCategory> list_Category = new List<WebNewsCategory>();
			if (NewsCategoryids.Trim() == "") NewsCategoryids = "0";
			string[] arr = NewsCategoryids.Split(',');

			int total = 0, count = 0;
			var exp = new Expressionable<WebNewsCategory>();
			string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
			string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebNewsCategory>()
						.SetColumns(u => u.Status == -1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.NewsCategoryid) || u.NewsCategoryid == NewsCategoryid))
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
					count = _context.Updateable<WebNewsCategory>()
						.SetColumns(u => u.Status == 1)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.NewsCategoryid) || u.NewsCategoryid == NewsCategoryid))
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
					count = _context.Updateable<WebNewsCategory>()
						.SetColumns(u => u.Status == 0)
						.Where(u => (Array.ConvertAll(arr, long.Parse).Contains(u.NewsCategoryid) || u.NewsCategoryid == NewsCategoryid))
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
					exp.And(u => u.Status != -1);
					if (!string.IsNullOrEmpty(keys))
					{
						exp.And(u => SqlFunc.Subqueryable<WebNewsCategoryLang>().Where(s=> SqlFunc.MergeString(s.Title, "").Contains(keys.Trim())).Any());
					}
					if (!string.IsNullOrEmpty(status))
					{
						exp.And(u => u.Status.ToString() == status.Trim());
					}
					if (!string.IsNullOrEmpty(langs))
					{
						exp.And(u => u.Langs.Contains(langs.Trim()));
					}
					if (NewsCategoryid > 0)
					{
						exp.And(u => u.NewsCategoryid == NewsCategoryid);
					}
					if (!string.IsNullOrEmpty(begintime))
					{
						exp.And(u => u.Dtime >= DateTime.Parse(begintime));
					}
					if (!string.IsNullOrEmpty(endtime))
					{
						exp.And(u => u.Dtime <= DateTime.Parse(endtime));
					}
					var query = _context.Queryable<WebNewsCategory>().Where(exp.ToExpression());
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
					query.OrderBy(u => u.Sort).OrderBy(u => u.NewsCategoryid, OrderByType.Desc);
					var list_Category_all = await query.ToListAsync();
					list_Category = query.Where(u => u.Fid == 0).ToPageList(page, pagesize, ref total);
					var list_CategoryLang = _context.Queryable<WebNewsCategoryLang>().Where(u =>u.Lang==Lang && list_Category.Select(s => s.NewsCategoryid).Contains(u.NewsCategoryid)).ToList();
                    var dic_PubLang = _context.Queryable<PubLanguage>().Where(u => u.Status == 1).ToDictionary(u => u.Lang, u => u.Title);
                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Category.Count; i++)
					{
                        List<string> list_lang = new List<string>();
                        if (!string.IsNullOrEmpty(list_Category[i].Langs))
                        {
                            var jarray = JArray.Parse(list_Category[i].Langs);
                            foreach (var item in jarray)
                            {
                                list_lang.Add(dic_PubLang[item.ToString()].ToString());
                            }
                        }
                        var model_Lang = list_CategoryLang.FirstOrDefault(u => u.NewsCategoryid == list_Category[i].NewsCategoryid);
						str.Append("{");
						str.Append("\"NewsCategoryid\":\"" + list_Category[i].NewsCategoryid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_Lang?.Title) + "\",");
						str.Append("\"Depth\":\"" + list_Category[i].Depth + "\",");
						str.Append("\"Dtime\":\"" + list_Category[i].Dtime.ToString() + "\",");
						str.Append("\"Sort\":\"" + list_Category[i].Sort + "\",");
                        str.Append("\"Langs\":\"" + string.Join(",", list_lang) + "\",");
                        str.Append("\"Isadmin\":\"" + list_Category[i].Isadmin + "\",");
						str.Append("\"Status\":\"" + list_Category[i].Status + "\",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_Category.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "分类修改"
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CategoryAdd(long NewsCategoryid = 0, long fid = 0)
		{
			CategoryAdd vm_ListAdd = new CategoryAdd();
			vm_ListAdd.LangItem = _context.Queryable<PubLanguage>().Where(u => u.Status == 1).OrderBy(u => u.Languageid, OrderByType.Desc).Select(u => new { u.Lang, u.Title })
				.ToList()
				.Select(u => new SelectListItem(u.Title, u.Lang))
				.ToList();
				
			var model_Category = _context.Queryable<WebNewsCategory>().InSingle(NewsCategoryid);
			if (model_Category != null)
			{
				vm_ListAdd.NewsCategoryid = model_Category.NewsCategoryid;
				vm_ListAdd.Fid = model_Category.Fid;
				vm_ListAdd.Depth = model_Category.Depth;
				vm_ListAdd.Status = model_Category.Status;
				vm_ListAdd.Dtime = model_Category.Dtime;
				vm_ListAdd.Sort = model_Category.Sort;
				vm_ListAdd.Sendtime = model_Category.Sendtime;
				vm_ListAdd.Img = model_Category.Img;
				var model_Lang = _context.Queryable<WebNewsCategoryLang>().First(u =>u.Lang==Lang && u.NewsCategoryid == model_Category.NewsCategoryid);
				vm_ListAdd.Title = model_Lang?.Title;
				vm_ListAdd.Langs = model_Category.Langs;
			}
			else
			{
                vm_ListAdd.Fid = fid;
				vm_ListAdd.Sort = 100;
				vm_ListAdd.Status = 1;
				vm_ListAdd.Dtime = DateTime.Now;
            }
            if(string.IsNullOrEmpty(vm_ListAdd.Langs))vm_ListAdd.Langs = "[]";
            vm_ListAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CategoryAdd(CategoryAdd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/Category/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			int chk_img = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_img", 0);
			int chk_img_h5 = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_img_h5", 0);
			int chk_load_torque = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_load_torque", 0);
			if (_accessor.HttpContext.Request.Form["Langs"].Count <= 0) {
                return Content("<script>alert('" + _localizer["请选择展示语言"] + "'); history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
            }
			var files = _accessor.HttpContext.Request.Form.Files;
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext = "";//扩展名
			var model_Master = _tokenManager.GetAdminInfo();
			string message = "[" + DateTime.Now + "]" + model_Master.Username + _localizer["修改信息"];

			var model_Category = _context.Queryable<WebNewsCategory>().InSingle(vm_ListAdd.NewsCategoryid);
			if (model_Category != null)
			{
				var model_Lang = _context.Queryable<WebNewsCategoryLang>().First(u => u.NewsCategoryid == model_Category.NewsCategoryid && u.Lang==vm_ListAdd.Lang);
				if (model_Lang != null)
				{
					model_Lang.Title = vm_ListAdd.Title;
					_context.Updateable(model_Lang).UpdateColumns(u => u.Title).ExecuteCommand();
				}
				else
				{
					model_Lang = new WebNewsCategoryLang();
					model_Lang.NewsCategoryid = vm_ListAdd.NewsCategoryid;
					model_Lang.Lang = vm_ListAdd.Lang;
					model_Lang.Title = vm_ListAdd.Title;
					_context.Insertable(model_Lang).ExecuteCommand();
				}
				model_Category.Langs = JsonConvert.SerializeObject(_accessor.HttpContext.Request.Form["Langs"].ToArray());

                model_Category.Status = vm_ListAdd.Status;
				model_Category.Dtime = DateTime.Now;
				model_Category.Sort = vm_ListAdd.Sort;
				model_Category.Sendtime = vm_ListAdd.Sendtime;
				model_Category.Depth = 1;
				model_Category.Path = model_Category.NewsCategoryid.ToString();
				model_Category.Rootid = model_Category.NewsCategoryid;
				#region "上传"
				if (chk_img == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_Category.Img)) FileHelper.FileDel(ImgRoot + model_Category.Img);
					}
					catch { }
					model_Category.Img = "";
				}
				if (files.Count > 0)
				{
					var formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Category.Img)) FileHelper.FileDel(ImgRoot + model_Category.Img);
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
							model_Category.Img = fileurl + filename;
						}
					}
				}
				#endregion
				_context.Updateable(model_Category).ExecuteCommand();
				return Content("<script>parent.$('#table_view').datagrid('reload');alert('"+ _localizer["保存分类成功"] +"'); location.href='?NewsCategoryid=" + vm_ListAdd.NewsCategoryid + "&lang=" + vm_ListAdd.Lang + "';</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_Category = new WebNewsCategory();
                model_Category.Langs = JsonConvert.SerializeObject(_accessor.HttpContext.Request.Form["Langs"].ToArray());
                model_Category.Status = vm_ListAdd.Status;
				model_Category.Dtime = DateTime.Now;
				model_Category.Sort = vm_ListAdd.Sort;
				model_Category.Sendtime = vm_ListAdd.Sendtime;

				#region "上传"
				if (files.Count > 0)
				{
					var formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null && formFile.Length > 0)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_Category.Img)) FileHelper.FileDel(ImgRoot + model_Category.Img);
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
							model_Category.Img = fileurl + filename;
						}
					}
				}
				#endregion

				vm_ListAdd.NewsCategoryid = _context.Insertable(model_Category).ExecuteReturnBigIdentity();
				var model_Lang = new WebNewsCategoryLang();
				model_Lang.NewsCategoryid = vm_ListAdd.NewsCategoryid;
				model_Lang.Lang = Lang;
				model_Lang.Title = vm_ListAdd.Title;
				_context.Insertable(model_Lang).ExecuteCommand();
				return Content("<script>parent.$('#table_view').datagrid('reload');alert('" + _localizer["保存分类成功"] + "');location.href='?NewsCategoryid=" + vm_ListAdd.NewsCategoryid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion

        #region "新闻列表"   
        [Authorization(Power = "Main")]
        public IActionResult List(long categoryid = 0)
        {
            var list_Lang = _context.Queryable<PubLanguage>().Where(u => u.Status == 1).ToList();
            string str = "";
            foreach (var item in list_Lang)
            {
                str += $"<option value=\"{item.Lang}\">{item.Title}</option>";
            }
            ViewBag.Langs = str;

            ViewBag.ScriptStr = _tokenManager.ScriptStr;
			ViewBag.Categoryid = categoryid;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListData(string keys = "", string langs = "", long categoryid = 0, string status = "", string recommend = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string Newsids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "Newsids");
			long Newsid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "Newsid", 0);
			var list_News = new List<WebNews>();
			if (Newsids.Trim() == "") Newsids = "0";
			string[] arr = Newsids.Split(',');
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					count = _context.Updateable<WebNews>()
						.SetColumns(u => u.Status == -1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Newsid) || u.Newsid == Newsid)
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
					count = _context.Updateable<WebNews>()
						.SetColumns(u => u.Status == 1)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Newsid) || u.Newsid == Newsid)
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
					count = _context.Updateable<WebNews>()
						.SetColumns(u => u.Status == 0)
						.Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.Newsid) || u.Newsid == Newsid)
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
					var exp = new Expressionable<WebNews>();
					exp.And(u => u.Status != -1);
					exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<WebNewsLang>().Where(s =>s.Newsid==u.Newsid && SqlFunc.MergeString(s.Title, "").Contains(keys.Trim())).Any() ||
						SqlFunc.Subqueryable<PubKeysLang>().Where(s => SqlFunc.MergeString(s.Title, "").Contains(keys.Trim()) && SqlFunc.MergeString(",", u.Keysids, ",").Contains(SqlFunc.MergeString(",",s.Keysid.ToString(),","))).Any()
					);
                    if (!string.IsNullOrEmpty(langs))
                    {
                        exp.And(u => u.Langs.Contains(langs.Trim()));
                    }
                    exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Sendtime >= DateTime.Parse(begintime));
					exp.AndIF(!string.IsNullOrEmpty(endtime), u => u.Sendtime <= DateTime.Parse(endtime));
					var query = _context.Queryable<WebNews>().Where(exp.ToExpression());
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
					query.OrderBy("sort,sendtime desc,newsid desc");
					list_News = query.ToPageList(page, pagesize, ref total);
					var list_Category = _context.Queryable<WebNewsCategoryLang>().Where(u => u.Lang == Lang && list_News.Select(s => s.NewsCategoryid).Contains(u.NewsCategoryid)).ToList();
					var list_Lang = _context.Queryable<WebNewsLang>().Where(u =>u.Lang==Lang && list_News.Select(s => s.Newsid).Contains(u.Newsid)).ToList();
                    var dic_PubLang = _context.Queryable<PubLanguage>().Where(u => u.Status == 1).ToDictionary(u=>u.Lang, u => u.Title);

                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_News.Count; i++)
					{
						var model_Lang = list_Lang.FirstOrDefault(u => u.Newsid == list_News[i].Newsid);
						var model_Category = list_Category.FirstOrDefault(u => u.NewsCategoryid == list_News[i].NewsCategoryid);
						long num = 0;
						var list_keysid = !string.IsNullOrEmpty(list_News[i].Keysids)?list_News[i].Keysids.Split(",").Where(u=> long.TryParse(u, out num)).Select(u=>long.Parse(u)).ToList():new List<long>();
						var list_keys = _context.Queryable<PubKeysLang>().Where(u =>u.Lang == Lang && list_keysid.Contains(u.Keysid)).Select(u=>u.Title).ToList();
						List<string> list_lang = new List<string>();
                        if (!string.IsNullOrEmpty(list_News[i].Langs)) {
							var jarray = JArray.Parse(list_News[i].Langs);
							foreach (var item in jarray)
							{
								list_lang.Add(dic_PubLang[item.ToString()].ToString());                            }
                        }

						str.Append("{");
						str.Append("\"Newsid\":\"" + list_News[i].Newsid + "\",");
						if (model_Category != null) str.Append("\"NewsCategory\":\"" + JsonHelper.JsonCharFilter(model_Category?.Title) + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_Lang?.Title + "") + "\",");
						str.Append("\"Keys\":\"" + string.Join(",", list_keys) + "\",");
						str.Append("\"Img\":\"" + list_News[i].Img + "\",");
						str.Append("\"Dtime\":\"" + list_News[i].Dtime.ToString() + "\",");
                        str.Append("\"Langs\":\"" + string.Join(",", list_lang) + "\",");
                        str.Append("\"Sort\":\"" + list_News[i].Sort + "\",");
						str.Append("\"Sendtime\":\"" + list_News[i].Sendtime + "\",");
						str.Append("\"Status\":" + list_News[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (list_News.Count - 1)) str.Append(",");
					}
					str.Append("]}");

					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "修改"   
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListAdd(long Newsid = 0)
		{
			var vm_ListAdd = new ListAdd();

			vm_ListAdd.LangItem = _context.Queryable<PubLanguage>().Where(u => u.Status == 1).OrderBy(u => u.Languageid, OrderByType.Desc).Select(u => new { u.Lang, u.Title })
				.ToList()
				.Select(u => new SelectListItem(u.Title, u.Lang))
				.ToList();

			vm_ListAdd.CategoryItems = _context.Queryable<WebNewsCategory>().LeftJoin<WebNewsCategoryLang>((l,r)=>l.NewsCategoryid==r.NewsCategoryid)
				.Where((l,r) => l.Fid == 0 && l.Status == 1).OrderBy((l, r) => l.Sort)
				.Select((l, r)=>new { r.Title,r.NewsCategoryid})
				.ToList().Select(u => new SelectListItem(u.Title, u.NewsCategoryid.ToString())).ToList();

			var model_News = _context.Queryable<WebNews>().InSingle(Newsid);
			if (model_News != null)
			{
				var model_NewsLang = _context.Queryable<WebNewsLang>().First(u => u.Newsid == Newsid && u.Lang == Lang);
				if (model_NewsLang != null)
				{
					vm_ListAdd.Keys = model_NewsLang.Keys;
					vm_ListAdd.Title = model_NewsLang.Title;
					vm_ListAdd.Message = model_NewsLang.Message;
					vm_ListAdd.Intro = model_NewsLang.Intro;
					if (JsonHelper.IsJson(model_NewsLang.Intro))
					{
						try
						{
							var o = JObject.Parse(model_NewsLang.Intro);
							vm_ListAdd.EncryptData = o["encrypt_data"].ToString();
						}
						catch { }
					}
				}
				vm_ListAdd.Newsid = Newsid;
				vm_ListAdd.NewsCategoryid = model_News.NewsCategoryid;
				vm_ListAdd.Img = model_News.Img;
				vm_ListAdd.Sendtime = model_News.Sendtime;
				vm_ListAdd.Keysids = model_News.Keysids;
				
				vm_ListAdd.Source = model_News.Source;
				vm_ListAdd.Sort = model_News.Sort;
				vm_ListAdd.Dtime = model_News.Dtime;
				vm_ListAdd.Status = model_News.Status;
				vm_ListAdd.Remark = model_News.Remark;
				vm_ListAdd.Hits = model_News.Hits;
				vm_ListAdd.Lasttime = model_News.Lasttime;
				vm_ListAdd.Ip = model_News.Ip;
				vm_ListAdd.Url = model_News.Url;
				vm_ListAdd.Video = model_News.Video;
				vm_ListAdd.PreView = _commonService.ResourceDomain(model_News.Video);
				vm_ListAdd.Langs = model_News.Langs;
			}
			else
			{
				vm_ListAdd.Status = 1;
				vm_ListAdd.Sort = 100;
				vm_ListAdd.Sendtime = DateTime.Now;
				vm_ListAdd.EncryptData = "";
			}
			if (string.IsNullOrEmpty(vm_ListAdd.Langs)) vm_ListAdd.Langs = "[]";
			vm_ListAdd.Lang = Lang;
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ListAdd(ListAdd vm_ListAdd)
		{
			string filename = "", fileurl = Appsettings.app("Web:Upfile").ToString() + "/News/" + DateTime.Now.ToString("yyyyMMdd") + "/";
			int chk_img = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_img", 0);
			var files = _accessor.HttpContext.Request.Form.Files;
			string ImgRoot = Appsettings.app("Web:ImgRoot");
			string ext = "";//扩展名
			var model_Master = _tokenManager.GetAdminInfo();
			IFormFile formFile;
			if (_accessor.HttpContext.Request.Form["Langs"].Count <= 0)
			{
				return Content("<script>alert('" + _localizer["请选择展示语言"] + "'); history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}

			string message = "[" + DateTime.Now + "]" + model_Master.Name + _localizer["修改信息"] + "";
			var model_News = _context.Queryable<WebNews>().InSingle(vm_ListAdd.Newsid);
			if (model_News != null)
			{
				var model_NewsLang = _context.Queryable<WebNewsLang>().First(u => u.Lang == vm_ListAdd.Lang && u.Newsid == vm_ListAdd.Newsid);
				if (model_News.NewsCategoryid != vm_ListAdd.NewsCategoryid)
				{
					message += "," + _localizer["修改分类"] + "，[" + model_News.NewsCategoryid + "]=>[" + vm_ListAdd.NewsCategoryid + "]";
				}
				if (model_NewsLang?.Title != vm_ListAdd.Title)
				{
					message += "," + _localizer["修改标题"] + "[" + model_NewsLang?.Title + "]=>[" + vm_ListAdd.Title + "]";
				}
				if (model_News.Keysids != vm_ListAdd.Keysids)
				{
					message += "," + _localizer["修改标题"] + "[" + model_News.Keysids + "]=>[" + vm_ListAdd.Keysids + "]";
				}
				if (model_NewsLang?.Message != vm_ListAdd.Message)
				{
					message += "," + _localizer["修改简介"] + "[" + model_NewsLang?.Message + "]=>[" + vm_ListAdd.Message + "]";
				}
				if (model_NewsLang?.Intro != vm_ListAdd.Intro)
				{
					message += "," + _localizer["修改过内容"] + "";
				}
				if (model_News.Source != vm_ListAdd.Source)
				{
					message += "," + _localizer["修改来源"] + "[" + model_News.Source + "]=>[" + vm_ListAdd.Source + "]";
				}
				if (model_News.Sort != vm_ListAdd.Sort)
				{
					message += ",将排序字段由[" + model_News.Sort + "]改为[" + vm_ListAdd.Sort + "]";
				}
				if (model_News.Sendtime != vm_ListAdd.Sendtime)
				{
					message += ",将发布时间由[" + model_News.Sendtime + "]改为[" + vm_ListAdd.Sendtime + "]";
				}
				if (model_News.Status != vm_ListAdd.Status)
				{
					message += ",将状态由[" + model_News.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (model_NewsLang != null)
				{
					model_NewsLang.Title = vm_ListAdd.Title;
					model_NewsLang.Keys = vm_ListAdd.Keys;
					model_NewsLang.Message = vm_ListAdd.Message;
					model_NewsLang.Intro = vm_ListAdd.Intro;
					_context.Updateable(model_NewsLang).ExecuteCommand();
				}
				else
				{
					model_NewsLang = new WebNewsLang();
					model_NewsLang.Newsid = vm_ListAdd.Newsid;
					model_NewsLang.Title = vm_ListAdd.Title;
					model_NewsLang.Keys = vm_ListAdd.Keys;
					model_NewsLang.Message = vm_ListAdd.Message;
					model_NewsLang.Title = vm_ListAdd.Title;
					model_NewsLang.Intro = vm_ListAdd.Intro;
					model_NewsLang.Lang = vm_ListAdd.Lang;
					_context.Insertable(model_NewsLang).ExecuteCommand();
				}
				model_News.Langs = JsonConvert.SerializeObject(_accessor.HttpContext.Request.Form["Langs"].ToArray());
				model_News.Url = vm_ListAdd.Url;
				model_News.Video = vm_ListAdd.Video;
				model_News.NewsCategoryid = vm_ListAdd.NewsCategoryid;
				model_News.Sendtime = vm_ListAdd.Sendtime;
				model_News.Keysids = vm_ListAdd.Keysids;
				model_News.Source = vm_ListAdd.Source;
				model_News.Status = vm_ListAdd.Status;
				model_News.Sort = vm_ListAdd.Sort;
				model_News.Remark = message + "<hr>" + vm_ListAdd.Remark;
				#region "上传"
				if (chk_img == 1)
				{
					try
					{
						if (!string.IsNullOrEmpty(model_News.Img)) FileHelper.FileDel(ImgRoot + model_News.Img);
					}
					catch { }
					model_News.Img = "";
				}
				if (files.Count > 0)
				{
					//缩略图
					formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						try
						{
							if (!string.IsNullOrEmpty(model_News.Img)) FileHelper.FileDel(ImgRoot + model_News.Img);
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
							model_News.Img = fileurl + filename;
						}
					}
				}
				#endregion
				_context.Updateable(model_News).ExecuteCommand();

				return Content("<script>parent.$('#table_view').datagrid('reload');alert('保存信息成功'); location.href='?Newsid=" + vm_ListAdd.Newsid + "&lang="+ vm_ListAdd.Lang +"';</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_News = new WebNews();
				model_News.NewsCategoryid = vm_ListAdd.NewsCategoryid;
				model_News.Langs = JsonConvert.SerializeObject(_accessor.HttpContext.Request.Form["Langs"].ToArray());
				model_News.Url = vm_ListAdd.Url;
				model_News.Sendtime = vm_ListAdd.Sendtime;
				model_News.Keysids = vm_ListAdd.Keysids;
				model_News.Source = vm_ListAdd.Source;
				model_News.Status = vm_ListAdd.Status;
				model_News.Sort = vm_ListAdd.Sort;
				model_News.Video = vm_ListAdd.Video;
				model_News.Remark = message + "<hr>" + vm_ListAdd.Remark;
				#region "上传"
				if (files.Count > 0)
				{
					formFile = files.FirstOrDefault(u => u.Name == "input_img" && u.Length > 0);
					if (formFile != null)
					{
						FileHelper.AddFolder(ImgRoot + fileurl);
						ext = Path.GetExtension(formFile.FileName);
						filename = Guid.NewGuid().ToString() + ext;//纯文件名

						using (FileStream fs = System.IO.File.Create(ImgRoot + fileurl + filename))
						{
							await formFile.CopyToAsync(fs);
							model_News.Img = fileurl + filename;
						}
					}
				}
				#endregion

				vm_ListAdd.Newsid = _context.Insertable(model_News).ExecuteReturnBigIdentity();
				var model_NewsLang = new WebNewsLang();
				model_NewsLang.Newsid = vm_ListAdd.Newsid;
				model_NewsLang.Title = vm_ListAdd.Title;
				model_NewsLang.Intro = vm_ListAdd.Intro;
				model_NewsLang.Message = vm_ListAdd.Message;
				model_NewsLang.Title = vm_ListAdd.Title;
				model_NewsLang.Lang = vm_ListAdd.Lang;
				_context.Insertable(model_NewsLang).ExecuteCommand();

				return Content("<script>parent.$('#table_view').datagrid('reload');alert('保存信息成功');location.href='?Newsid=" + vm_ListAdd.Newsid + "&lang=" + vm_ListAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
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
			var list_keysid = (keysids + "").Split(",").Where(u => long.TryParse(u, out num)).Select(u => long.Parse(u)).ToList();
			var list_Keys = _context.Queryable<PubKeysLang>().Where(u => u.Lang == Lang && list_keysid.Contains(u.Keysid)).ToList();
			foreach (var model in list_Keys)
			{
				int i = list_Keys.IndexOf(model);
				str.Append("<div class=\"col-lg-2\">");
				str.Append("<div class=\"custom-control custom-checkbox\">");
				str.Append("<input type=\"hidden\" name=\"input_keysid\" value=\"" + model.Keysid + "\" />");
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
        public async Task<IActionResult> ChooseKeys(string keysids = "", string keys = "", string abc = "")
		{
			StringBuilder str = new StringBuilder();
			long num = 0;
			var list_keysid = (keysids + "").Split(",").Where(u => long.TryParse(u, out num)).Select(u => long.Parse(u)).ToList();
			var exp = new Expressionable<PubKeys>();
			exp.And(u => u.Status == 1 && u.Sty == 1);
			exp.AndIF(!string.IsNullOrEmpty(keys), u => SqlFunc.Subqueryable<PubKeysLang>().Where(s => s.Title.Contains(keys) && s.Keysid == u.Keysid).Any());

			var list_PubKeys = _context.Queryable<PubKeys>().Where(exp.ToExpression()).OrderBy("sort,keysid desc").ToList();
			var list_KeysLang = _context.Queryable<PubKeysLang>().Where(u => list_PubKeys.Select(s => s.Keysid).Contains(u.Keysid) && u.Lang == Lang).ToList();
			for (int i = 0; i < list_PubKeys.Count; i++)
			{
				var model_Lang = list_KeysLang.FirstOrDefault(u => u.Keysid == list_PubKeys[i].Keysid);
				str.Append("<div class=\"col-lg-2\">");
				str.Append("<div class=\"custom-control custom-checkbox\">");
				str.Append("<input type=\"checkbox\" class=\"custom-control-input\" " + (list_keysid.Contains(list_PubKeys[i].Keysid) ? "checked" : "") + " name=\"chk_keys\" id=\"chk_keys_" + list_PubKeys[i].Keysid + "\" value=\"" + model_Lang?.Title + "|" + list_PubKeys[i].Keysid + "\" data-parsley-multiple=\"groups\" data-parsley-mincheck=\"2\">");
				str.Append("<label class=\"custom-control-label\" for=\"chk_keys_" + list_PubKeys[i].Keysid + "\">" + model_Lang?.Title + "</label>");
				str.Append("</div>");
				str.Append("</div>");
			}
			return Content(str.ToString());
		}
        #endregion


        #region "新闻评论列表"   
        [Authorization(Power = "Main")]
        public IActionResult Comment(long newsid = 0)
        {
            ViewBag.ScriptStr = _tokenManager.ScriptStr;
            ViewBag.Newsid = newsid;
            return View();
        }

        [HttpGet]
        [HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> CommentData(string keys = "", long newsid = 0, string status = "", string begintime = "", string endtime = "")
        {
            StringBuilder str = new StringBuilder();
            int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
            int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
            string NewsCommentids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "NewsCommentids");
            long NewsCommentid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "NewsCommentid", 0);
            var list_NewsComment = new List<WebNewsComment>();
            if (NewsCommentids.Trim() == "") NewsCommentids = "0";
            string[] arr = NewsCommentids.Split(',');
            int count = 0;
            switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
            {
                case "del":
                    count = _context.Updateable<WebNewsComment>()
                        .SetColumns(u => u.Status == -1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.NewsCommentid) || u.NewsCommentid == NewsCommentid)
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
                    count = _context.Updateable<WebNewsComment>()
                        .SetColumns(u => u.Status == 1)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.NewsCommentid) || u.NewsCommentid == NewsCommentid)
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
                    count = _context.Updateable<WebNewsComment>()
                        .SetColumns(u => u.Status == 0)
                        .Where(u => Array.ConvertAll(arr, long.Parse).Contains(u.NewsCommentid) || u.NewsCommentid == NewsCommentid)
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
                    var exp = new Expressionable<WebNewsComment>();
                    exp.And(u => u.Status != -1);
                    exp.AndIF(!string.IsNullOrEmpty(keys),u=>u.Title.Contains(keys) || u.Message.Contains(keys));
                    exp.AndIF(!string.IsNullOrEmpty(status), u => u.Status.ToString() == status.Trim());
					if (!string.IsNullOrEmpty(begintime))
					{
						var btime = DateHelper.ConvertDateTimeInt(DateTime.Parse(begintime));
						exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime >= btime);
					}
                    if (!string.IsNullOrEmpty(endtime))
                    {
                        var etime = DateHelper.ConvertDateTimeInt(DateTime.Parse(endtime));
                        exp.AndIF(!string.IsNullOrEmpty(begintime), u => u.Dtime <= etime);
                    }
                    var query = _context.Queryable<WebNewsComment>().Where(exp.ToExpression());
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
                    list_NewsComment = query.ToPageList(page, pagesize, ref total);
					var list_User = _context.Queryable<WebUser>().Where(u => list_NewsComment.Select(s => s.Userid).Contains(u.Userid)).ToList();
                    str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
                    for (int i = 0; i < list_NewsComment.Count; i++)
                    {
						string name = "",mobile = "", message = HtmlHelper.ReplaceHtmlTag(list_NewsComment[i].Message,20);
						var model_User = list_User.FirstOrDefault(u => u.Userid == list_NewsComment[i].Userid);
						if (model_User != null) {
							name = model_User.FirstName + " " + model_User.LastName;
							mobile = model_User.MobileCode + "-" + model_User.Mobile;
                        }
                        str.Append("{");
                        str.Append("\"NewsCommentid\":\"" + list_NewsComment[i].NewsCommentid + "\",");
                        str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(list_NewsComment[i].Title + "") + "\",");
                        str.Append("\"Name\":\"" + JsonHelper.JsonCharFilter(name) + "\",");
                        str.Append("\"Message\":\"" + JsonHelper.JsonCharFilter(message) + "\",");
                        str.Append("\"Mobile\":\"" + mobile + "\",");
                        str.Append("\"Dtime\":\"" + DateHelper.ConvertIntToDateTime(list_NewsComment[i].Dtime.ToString()) + "\",");
                        str.Append("\"Status\":" + list_NewsComment[i].Status + ",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
                        if (i < (list_NewsComment.Count - 1)) str.Append(",");
                    }
                    str.Append("]}");

                    break;
            }
            return Content(str.ToString());
        }
		#endregion

		#region "修改"   
		[Authorization(Power = "Main")]
		public async Task<IActionResult> CommentAdd(long NewsCommentid = 0)
		{
			var vm_ListAdd = new CommentAdd();

			var model_NewsComment = _context.Queryable<WebNewsComment>().InSingle(NewsCommentid);
			if (model_NewsComment != null)
			{
				vm_ListAdd.NewsCommentid = NewsCommentid;
				vm_ListAdd.Title = model_NewsComment.Title;
				vm_ListAdd.Message = model_NewsComment.Message;
				vm_ListAdd.Dtime = model_NewsComment.Dtime;
				vm_ListAdd.Status = model_NewsComment.Status;
				vm_ListAdd.Remark = model_NewsComment.Remark;
				vm_ListAdd.Ip = model_NewsComment.Ip;
				vm_ListAdd.Remark=model_NewsComment.Remark;
				var model_User = _context.Queryable<WebUser>().First(u => u.Userid == model_NewsComment.Userid);
				if (model_User != null) {
					vm_ListAdd.Name = model_User.FirstName + " " + model_User.LastName;
					vm_ListAdd.Mobile = model_User.MobileCode + "-" + model_User.Mobile;
				}
			}
			else
			{
				vm_ListAdd.Status = 1;
			}
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View(vm_ListAdd);
		}

		//保存添改
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> CommentAdd(CommentAdd vm_ListAdd)
		{
			var model_Master = _tokenManager.GetAdminInfo();

			string message = "[" + DateTime.Now + "]" + model_Master.Name + _localizer["修改信息"] + "";
			var model_NewsComment = _context.Queryable<WebNewsComment>().InSingle(vm_ListAdd.Newsid);
			if (model_NewsComment != null)
			{
				if (model_NewsComment.Status != vm_ListAdd.Status)
				{
					message += ",将状态由[" + model_NewsComment.Status + "]改为[" + vm_ListAdd.Status + "]";
				}
				if (!string.IsNullOrEmpty(vm_ListAdd.Message1))
				{
					message += ",添加日志:" + vm_ListAdd.Message1;
				}
				model_NewsComment.Status = vm_ListAdd.Status;
				model_NewsComment.Remark = message + "<hr>" + vm_ListAdd.Remark;
				_context.Updateable(model_NewsComment).ExecuteCommand();

				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改评论"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				return Content("<script>alert('" + _localizer["不允许添加评论"] + "'); history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		#endregion
	}
}