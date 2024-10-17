using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Interfaces;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Web.Extensions;
using WeTalk.Web.ViewModels;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class Maincontroller : Base.BaseController
	{

		private readonly ILanguageService _languageService;
		private string WebRoot="";
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IWebHostEnvironment _env;
		protected readonly IStringLocalizer<LangResource> _localizer;//语言包

		public Maincontroller( IConfiguration config, IHttpContextAccessor accessor, IWebHostEnvironment env, TokenManager tokenManager, ILanguageService languageService,
			SqlSugarScope context, IStringLocalizer<LangResource> localizer)
			: base(tokenManager)
		{
			_languageService = languageService;
			_accessor = accessor;
			_context= context;
			_localizer = localizer;
			_env = env;
			WebRoot = (!string.IsNullOrEmpty(config["Web:WebRoot"]))?env.WebRootPath:"";
		}

		// GET: Admin_WeTalk/Main
        [Authorization(Power = "Main")]
		public async Task<IActionResult> Index()
		{
			DateTime dtime1 = DateTime.Now;
			StringBuilder str = new StringBuilder();
			StringBuilder str1 = new StringBuilder();
			var list_Menus = _context.Queryable<AdminMenu>().Where(u => u.Status == 1).ToList();
			var list_Lang = _context.Queryable<AdminMenuLang>().Where(u =>u.Lang==Lang && list_Menus.Select(s => s.AdminMenuid).Contains(u.AdminMenuid)).ToList();
			string TargetFrame = "rightFrame";
			List<AdminMenu> drs, drs1;
			string url = "";
			ViewAdminMenuFunction model_admin_menu_function = null;
			var list_admin_menu_functions = await GetFunctions(Lang, u => u.Status == 1);
			if (IsAdmin)
			{
				drs = list_Menus.Where(u => u.Fid == 0).OrderBy(u => u.Sty).ThenBy(u => u.Sort).ThenByDescending(s => s.AdminMenuid).ToList();
				for (int i = 0; i < drs.Count; i++)
				{
					var model_MenuLang = list_Lang.FirstOrDefault(u => u.Lang == Lang && u.AdminMenuid == drs[i].AdminMenuid);
					drs1 = list_Menus.Where(u => u.Fid == drs[i].AdminMenuid).OrderBy(u => u.Sty).ThenBy(s => s.Sort).ThenByDescending(s => s.AdminMenuid).ToList();
					if (drs1.Count <= 0)
					{
						model_admin_menu_function = list_admin_menu_functions.FirstOrDefault(u => u.Fid == 0 && u.AdminMenuid == drs[i].AdminMenuid);
						if (model_admin_menu_function != null)
						{
							url = "/" + model_admin_menu_function.Area + "/" + model_admin_menu_function.Controller + "/" + model_admin_menu_function.Action;
							url = (!string.IsNullOrEmpty(model_admin_menu_function.Parameter)) ? url + "?" + model_admin_menu_function.Parameter : url;
						}
						else
						{
							url = "#Metrica" + drs[i].AdminMenuid;
						}
						str1.Append("<a href=\"" + url + "\" class=\"nav-link J_menuItem" + ((i == 0) ? " active" : "") + "\" data-toggle=\"tooltip-custom\" data-placement=\"top\" title=\"\">\r");
						str1.Append("<span>" + model_MenuLang?.Title + "</span>\r");
						str1.Append("</a>\r");
					}
					else
					{
						str1.Append("<a href=\"#Metrica" + drs[i].AdminMenuid + "\" class=\"nav-link" + ((i == 0) ? " active" : "") + "\" data-toggle=\"tooltip-custom\" data-placement=\"top\" title=\"\">\r");
						str1.Append("<span>" + model_MenuLang?.Title + "</span>\r");
						str1.Append("</a>\r");
						str.Append(CreateNode(TargetFrame, drs[i].AdminMenuid.ToString(), list_Menus, list_admin_menu_functions, drs1, list_Lang, model_MenuLang?.Title, (i == 0 ? "active" : "")));
					}
				}
			}
			else
			{
				string MenuIDs = _accessor.HttpContext.Session.GetString(Appsettings.app("Web:LoginPrefix") + "MenuIDs");
				if (string.IsNullOrEmpty(MenuIDs)) MenuIDs = "0";
				string[] arr = MenuIDs.Split(',');
				if (_env.IsDevelopment())
				{
					drs = list_Menus.Where(u => u.Fid == 0 && (Array.ConvertAll(arr, long.Parse).Contains(u.AdminMenuid) || u.Sty == 0)).OrderBy(u => u.Sty).ThenBy(u => u.Sort).ThenByDescending(u => u.AdminMenuid).ToList();
				}
				else
				{
					drs = list_Menus.Where(u => u.Fid == 0 && Array.ConvertAll(arr, long.Parse).Contains(u.AdminMenuid)).OrderBy(u => u.Sty).ThenBy(u => u.Sort).ThenByDescending(u => u.AdminMenuid).ToList();
				}
				for (int i = 0; i < drs.Count; i++)
				{
					var model_MenuLang = list_Lang.FirstOrDefault(u => u.Lang == Lang && u.AdminMenuid == drs[i].AdminMenuid);
					if (_env.IsDevelopment())
					{
						drs1 = list_Menus.Where(u => u.Fid == drs[i].AdminMenuid && (Array.ConvertAll(arr, long.Parse).Contains(u.AdminMenuid) || u.Sty == 0)).OrderBy(u => u.Sty).ThenBy(u => u.Sort).ThenByDescending(s => s.AdminMenuid).ToList();
					}
					else
					{
						drs1 = list_Menus.Where(u => u.Fid == drs[i].AdminMenuid && Array.ConvertAll(arr, long.Parse).Contains(u.AdminMenuid)).OrderBy(u => u.Sty).ThenBy(u => u.Sort).ThenByDescending(s => s.AdminMenuid).ToList();
					}
					if (drs1.Count < 1)
					{
						model_admin_menu_function = list_admin_menu_functions.FirstOrDefault(u => u.Fid == 0 && u.AdminMenuid == drs[i].AdminMenuid);
						if (model_admin_menu_function != null)
						{
							url = "/" + model_admin_menu_function.Area + "/" + model_admin_menu_function.Controller + "/" + model_admin_menu_function.Action;
							url = (!string.IsNullOrEmpty(model_admin_menu_function.Parameter)) ? url + "?" + model_admin_menu_function.Parameter : url;
						}
						else
						{
							url = "#Metrica" + drs[i].AdminMenuid;
						}
						str1.Append("<a href=\"" + url + "\" class=\"nav-link J_menuItem\" data-toggle=\"tooltip-custom\" data-placement=\"top\" title=\"\">\r");
						str1.Append("<span>" + model_MenuLang?.Title + "</span>\r");
						str1.Append("</a>\r");
					}
					else
					{
						str1.Append("<a href=\"#Metrica" + drs[i].AdminMenuid + "\" class=\"nav-link\" data-toggle=\"tooltip-custom\" data-placement=\"top\" title=\"\">\r");
						str1.Append("<span>" + model_MenuLang?.Title + "</span>\r");
						str1.Append("</a>\r");
						str.Append(CreateNode(TargetFrame, drs[i].AdminMenuid.ToString(), list_Menus, list_admin_menu_functions, drs1, list_Lang, model_MenuLang?.Title, (i == 0 ? "active" : "")));
					}
				}
			}
			var vm_Main = new Main
			{
				HeadImg = "",
				AdminName = _accessor.HttpContext.Session.GetString(Appsettings.app("Web:LoginPrefix") + "AdminName"),
				LeftHtml_f = str1.ToString(),
				LeftHtml = str.ToString()
			};
			var model_Lang = _context.Queryable<PubLanguage>().First(u => u.Lang == Lang);
			if (model_Lang != null)
			{
				vm_Main.LangTtile = model_Lang.Title;
			}
			str.Clear();
			var list_lang = _context.Queryable<PubLanguage>().Where(u => u.Isadmin == 1 && u.Status == 1).ToList();
			foreach (var item in list_lang)
			{
				str.Append($"<a class=\"dropdown-item\" href=\"javascript: setlang('{item.Lang}');\"><span> {item.Title} </span></a>");
			}
			vm_Main.LangList = str.ToString();
			return View(vm_Main);
		}

        [Authorization(Power = "Main")]
		public async Task<IActionResult> SetLang(string lang)
		{
			var result = await _languageService.SetLang(lang);
			return new JsonResult(result);
		}

		[HttpPost]
        [Authorization(Power = "Main")]
        public JsonResult UploadCKEditorImage()
		{
			var files = Request.Form.Files;
			if (files.Count == 0)
			{
				var rError = new
				{
					uploaded = false,
					url = string.Empty
				};
				return Json(rError);
			}
			var formFile = files[0];
			var upFileName = formFile.FileName;
			//大小，格式校验....
			var fileName = Guid.NewGuid() + Path.GetExtension(upFileName);
			var folder = "/upload/" + DateTime.Now.ToString("yyyy-MM-dd");
			var savePath = WebRoot + folder + "/" + fileName;

			bool result = true;
			try
			{
				if (!Directory.Exists(WebRoot + folder))
				{
					Directory.CreateDirectory(WebRoot + folder);
				}
				using (FileStream fs = System.IO.File.Create(savePath))
				{
					formFile.CopyTo(fs);
					fs.Flush();
				}
			}
			catch (Exception ex)
			{
				result = false;
			}
			var rUpload = new
			{
				uploaded = result,
				url = result ? (folder + "/" + fileName) : string.Empty
			};
			return Json(rUpload);

		}


		#region 私有方法
		//绑定任意节点 
		private string CreateNode(string TargetFrame, string parentid, List<AdminMenu> list_Menus, List<ViewAdminMenuFunction> list_admin_menu_functions, List<AdminMenu> drs,List<AdminMenuLang> list_Lang, string f_name, string isactive = "")
		{
			ViewAdminMenuFunction ds;
			string url = "", ico = "";
			StringBuilder str = new StringBuilder();
			List<AdminMenu> list_menus = null;
			str.Append("<div id=\"Metrica" + parentid + "\" class=\"main-icon-menu-pane " + isactive + "\">\r");
			str.Append("<div class=\"title-box\">\r");
			str.Append("<h6 class=\"menu-title\">" + f_name + "</h6>\r");
			str.Append("</div>\r");
			str.Append("<ul class=\"nav\">\r");
			for (int i = 0; i < drs.Count; i++)
			{
				if (drs[i].Depth > 2) break;
				var model_Lang = list_Lang.FirstOrDefault(u => u.AdminMenuid == drs[i].AdminMenuid);
				ico = drs[i].Ico;
				if (string.IsNullOrEmpty(ico)) ico = "dripicons-document";

				list_menus = list_Menus.Where(u => u.Fid == drs[i].AdminMenuid).ToList();
				if (list_menus != null && list_menus.Count > 0)
				{
					str.Append("<li class=\"nav-item\">");
					str.Append("    <a class=\"nav-link\" href=\"#\"><i class=\"" + ico + "\"></i><span class=\"w-100\">" + model_Lang?.Title + "</span><span class=\"menu-arrow\"><i class=\"mdi mdi-chevron-right\"></i></span></a>");
					str.Append("    <ul class=\"nav-second-level\" aria-expanded=\"false\">");
					for (int j = 0; j < list_menus.Count; j++)
					{
						var model_son_Lang = list_Lang.FirstOrDefault(u => u.AdminMenuid == list_menus[i].AdminMenuid);
						//ds = _context.admin_menu_function.FirstOrDefault(u => u.Fid == 0 && u.AdminMenuid == admin_menus[j].AdminMenuid);
						ds = list_admin_menu_functions.FirstOrDefault(u => u.Fid == 0 && u.AdminMenuid == list_menus[j].AdminMenuid);
						if (ds != null)
						{
							url = "/" + ds.Area + "/" + ds.Controller + "/" + ds.Action;
							url = (!string.IsNullOrEmpty(ds.Parameter)) ? url + "?" + ds.Parameter : url;
						}
						str.Append("        <li><a class=\"J_menuItem\" href=\"" + url + "\">" + model_son_Lang?.Title + "</a></li>");
					}
					str.Append("    </ul>");
					str.Append("</li>");
				}
				else
				{
					ds = list_admin_menu_functions.FirstOrDefault(u => u.Fid == 0 && u.AdminMenuid == drs[i].AdminMenuid);
					if (ds != null)
					{
						url = "/" + ds.Area + "/" + ds.Controller + "/" + ds.Action;
						url = (!string.IsNullOrEmpty(ds.Parameter)) ? url + "?" + ds.Parameter : url;
					}
					str.Append("<li class=\"nav-item\"><a class=\"nav-link J_menuItem\" href=\"" + url + "\"><i class=\"" + ico + "\"></i>" + model_Lang?.Title + "</a></li>\r");
				}
			}
			str.Append("</ul>\r");
			str.Append("</div>\r");
			return str.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lang"></param>
		/// <param name="whereExpression"></param>
		/// <returns></returns>
		private async Task<List<ViewAdminMenuFunction>> GetFunctions(string lang, Expression<Func<ViewAdminMenuFunction, bool>> whereExpression)
		{
			var list = _context.Queryable<AdminMenuFunction>()
				.Where(l => l.Status == 1)
				.Select(l => new ViewAdminMenuFunction()
				{
					AdminMenuFunctionid = l.AdminMenuFunctionid.SelectAll(),
					Lang = lang,
					Title = SqlFunc.Subqueryable<AdminMenuFunctionLang>().Where(r => r.AdminMenuFunctionid == l.AdminMenuFunctionid && r.Lang == lang).Select(r => r.Title)
				}).MergeTable()
				.Where(whereExpression)
				.ToList();
			return list;
		}
		#endregion
	}
}
