using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Models;
using WeTalk.Web.ViewModels.System;
using WeTalk.Web.Extensions;
using WeTalk.Interfaces.Services;
using Microsoft.AspNetCore.Routing;
using WeTalk.Common;
using WeTalk.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class SystemController : Base.BaseController
	{
		//抽像

		private readonly IPubConfigService _pubConfigService;


		private readonly IConfiguration _config;
		private readonly TokenManager _tokenManager;
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包
		public SystemController(TokenManager tokenManager,  IConfiguration config,  IHttpContextAccessor accessor,IStringLocalizer<LangResource> localizer, SqlSugarScope context,
			IPubConfigService pubConfigService
			)
			: base(tokenManager)
		{
			_config = config;
			_tokenManager = tokenManager;
			_accessor = accessor;
			_context = context;

			_pubConfigService = pubConfigService;
			_localizer = localizer;
		}

		#region "系统设置" 
		[Authorization(Power = "Main")]
		public async Task<IActionResult> Config()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			Config vm_config = new Config();
			vm_config.Lang = Lang;
			var dic_config = _pubConfigService.GetConfigs("isclose,closetips,sitename,siteurl,sitetitle,sitekeywords,siteDescription,upfiletype,upfilesize,sendname,mailservice,sendmail,sendpwd,fooder,sitecopyright,icp");
			if (dic_config.ContainsKey("isclose")) vm_config.IsClose = bool.Parse(dic_config["isclose"].ToLower());
			if (dic_config.ContainsKey("closetips")) vm_config.CloseTips = dic_config["closetips"];
			if (dic_config.ContainsKey("sitename")) vm_config.SiteName = dic_config["sitename"];
			if (dic_config.ContainsKey("siteurl")) vm_config.SiteUrl = dic_config["siteurl"];
			if (dic_config.ContainsKey("sitetitle")) vm_config.SiteTitle = dic_config["sitetitle"];
			if (dic_config.ContainsKey("sitekeywords")) vm_config.SiteKeyWords = dic_config["sitekeywords"];
			if (dic_config.ContainsKey("siteDescription")) vm_config.SiteDescription = dic_config["siteDescription"];
			if (dic_config.ContainsKey("upfiletype")) vm_config.UpFileType = dic_config["upfiletype"];
			if (dic_config.ContainsKey("upfilesize")) vm_config.UpFileSize = int.Parse(dic_config["upfilesize"]);
			if (dic_config.ContainsKey("sendname")) vm_config.Sendname = dic_config["sendname"];
			if (dic_config.ContainsKey("mailservice")) vm_config.MailService = dic_config["mailservice"];
			if (dic_config.ContainsKey("sendmail")) vm_config.SendMail = dic_config["sendmail"];
			if (dic_config.ContainsKey("sendpwd")) vm_config.SendPwd = dic_config["sendpwd"];
			if (dic_config.ContainsKey("fooder")) vm_config.Fooder = dic_config["fooder"];
			if (dic_config.ContainsKey("sitecopyright")) vm_config.SiteCopyRight = dic_config["sitecopyright"];
			if (dic_config.ContainsKey("icp")) vm_config.ICP = dic_config["icp"];

			return View(vm_config);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> Config(Config vm_config)
		{
			if (ModelState.IsValid)
			{
				#region "更新系统设置"
				string val = vm_config.IsClose.ToString().ToLower();
				var dic_config = new Dictionary<string, string>();
				dic_config.Add("isclose", vm_config.IsClose.ToString().ToLower());
				dic_config.Add("closetips", vm_config.CloseTips);
				dic_config.Add("sitename", vm_config.SiteName);
				dic_config.Add("siteurl", vm_config.SiteUrl);
				dic_config.Add("sitetitle", vm_config.SiteTitle);
				dic_config.Add("sitekeywords", vm_config.SiteKeyWords);
				dic_config.Add("sitedescription", vm_config.SiteDescription);
				dic_config.Add("upfiletype", vm_config.UpFileType);
				dic_config.Add("upfilesize", vm_config.UpFileSize.ToString());
				dic_config.Add("sendname", vm_config.Sendname);
				dic_config.Add("mailservice", vm_config.MailService);
				dic_config.Add("sendmail", vm_config.SendMail);
				if(!string.IsNullOrEmpty(vm_config.SendPwd))dic_config.Add("sendpwd", vm_config.SendPwd);
				dic_config.Add("fooder", vm_config.Fooder);
				dic_config.Add("sitecopyright", vm_config.SiteCopyRight);
				dic_config.Add("icp", vm_config.ICP);
				_pubConfigService.UpdateConfigs(dic_config);
				#endregion
				return Content("<script>alert('" + _localizer["修改配置成功"] + "');location.href='config?lang=" + vm_config.Lang + "';</script>", "text/html", Encoding.GetEncoding("utf-8"));
				//return RedirectToAction(nameof(Config));
			}
			return View(vm_config);
		}
		#endregion

		#region "总后台菜单栏目" 
		#region "菜单列表" 
		[Authorization(Power = "Main")]
		public IActionResult Menu()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> MenuData()
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long AdminMenuid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "AdminMenuid", 0);
			string AdminMenuids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "AdminMenuids");
			int count = 0;
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					if (AdminMenuids.Trim() == "") AdminMenuids = "0";
					var list_menuid = AdminMenuids.Split(',').Select(u=> long.Parse(u)).ToList();
					if (AdminMenuid > 0) list_menuid.Add(AdminMenuid);

					var menuids = new List<long>();
					var list_admin_menus = _context.Queryable<AdminMenu>().Where(u => u.Status == 1 && list_menuid.Contains(u.AdminMenuid)).ToList();
					if (list_admin_menus.Count > 0)
					{
						for (int i = 0; i < list_admin_menus.Count; i++)
						{
							menuids.Add(list_admin_menus[i].AdminMenuid);
							list_admin_menus[i].Status = -1;
						}
						_context.Updateable(list_admin_menus).UpdateColumns(u => u.Status).ExecuteCommand();

						//删除控件
						_context.Updateable<AdminMenuFunction>().SetColumns(u => u.Status == -1).Where(u => u.Status == 1 && menuids.Contains(u.AdminMenuid)).ExecuteCommand();

						//删除权限与控件关联表
						_context.Updateable<AdminRoleMenuFunction>().SetColumns(u=>u.Status == -1).Where(u => menuids.Contains(u.AdminMenuid)).ExecuteCommand();

						////删除管理用户与控件关联表
						//var list_admin_masterMenuFunction = await _context.admin_masterMenuFunction.Where(u => arr_menuids.Contains(u.AdminMenuid)).ToListAsync();
						//for (int i = 0; i < list_admin_masterMenuFunction.Count; i++)
						//{
						//    _context.admin_masterMenuFunction.Remove(list_admin_masterMenuFunction[i]);
						//}
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "！\"}");
					}
					break;
				case "enableu"://启用
					count = _context.Updateable<AdminMenu>().SetColumns(u=>u.Status == 1).UpdateColumns(u => u.AdminMenuid == AdminMenuid).ExecuteCommand();
					if (count>0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["解冻成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["解冻失败,不存在此用户"] + "！\"}");
					}
					break;
				case "enablef"://禁用   
					count = _context.Updateable<AdminMenu>().SetColumns(u => u.Status == 0).UpdateColumns(u => u.AdminMenuid == AdminMenuid).ExecuteCommand();
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
					string sty_str = "";
					string son_str = "";
					var exp = new Expressionable<AdminMenu>();
					exp.And(u => u.Status == 1);
					
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
					sort += "sty,sort,admin_menuid desc";
					var list_Menus_all = _context.Queryable<AdminMenu>().Where(exp.ToExpression()).OrderBy(sort).ToList();
					var list_MenuLang = _context.Queryable<AdminMenuLang>().Where(u => list_Menus_all.Select(s => s.AdminMenuid).Contains(u.AdminMenuid) && u.Lang == Lang).ToList();
					var list_Menus = list_Menus_all.Where(u => u.Fid == 0).Skip((page - 1) * pagesize).Take(pagesize).ToList();
					str.Append("{\"total\":" + list_Menus_all.Where(u => u.Fid == 0).Count() + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_Menus.Count(); i++)
					{
						switch (list_Menus[i].Sty)
						{
							case 0:
								sty_str = _localizer["开发者菜单"];
								break;
							case 1:
								sty_str = _localizer["普通菜单"];
								break;
						}
						son_str = MenuSon(list_Menus[i].AdminMenuid, list_Menus_all, list_MenuLang, sort);
						var model_Lang = list_MenuLang.FirstOrDefault(u => u.AdminMenuid == list_Menus[i].AdminMenuid);
						str.Append("{");
						str.Append("\"AdminMenuid\":\"" + list_Menus[i].AdminMenuid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_Lang?.Title + "") + "\",");
						str.Append("\"Dtime\":\"" + list_Menus[i].Dtime + "\",");
						str.Append("\"sty_str\":\"" + sty_str + "\",");
						str.Append("\"Sort\":\"" + list_Menus[i].Sort + "\",");
						if (son_str.Trim() == "")
						{
							str.Append("\"MenuFunction\":\"" + JsonHelper.JsonCharFilter("<a href=\"#\" onclick=\"mainGrid.menu_function('" + list_Menus[i].AdminMenuid + "','" + (model_Lang?.Title + "") + "');\">子页/控件管理</a>") + "\",");
						}
						if (list_Menus[i].Depth < 3)
						{
							str.Append("\"add\":\""+ _localizer["添加"] + "\",");
						}
						else
						{
							str.Append("\"add\":\"\",");
						}
						str.Append("\"operation\":\"" + _localizer["查看/修改"]+"\"}");
						str.Append(son_str);
						if (i < (list_Menus.Count() - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}

		private string MenuSon(long f_id, List<AdminMenu> list_Menus_all,List<AdminMenuLang> list_MenuLang,string sort)
		{
			StringBuilder str = new StringBuilder();
			string son_str = "";
			//DataSet ds = bll_menu.GetList("fid=" + f_id + " order by sort,AdminMenuid desc");
			var list_Menus = list_Menus_all.Where(u => u.Status == 1 && u.Fid == f_id).ToList();
			for (int i = 0; i < list_Menus.Count; i++)
			{
				var model_Lang = list_MenuLang.FirstOrDefault(u => u.AdminMenuid == list_Menus[i].AdminMenuid);
				str.Append(",{");
				str.Append("\"AdminMenuid\":\"" + list_Menus[i].AdminMenuid.ToString() + "\",");
				str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_Lang?.Title+"") + "\",");
				if (son_str.Trim() == "")
				{
					str.Append("\"MenuFunction\":\"" + JsonHelper.JsonCharFilter("<a href=\"#\" onclick=\"mainGrid.menu_function('" + list_Menus[i].AdminMenuid.ToString() + "','" + model_Lang?.Title + "');\">子页/控件管理</a>") + "\",");
				}
				str.Append("\"Dtime\":\"" + list_Menus[i].Dtime.ToString() + "\",");
				str.Append("\"Sort\":\"" + list_Menus[i].Sort.ToString() + "\",");
				if (list_Menus[i].Depth < 3)
				{
					str.Append("\"add\":\"" + _localizer["添加"] + "\",");
				}
				else
				{
					str.Append("\"add\":\"\",");
				}
				str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\",");
				str.Append("\"_parentId\":" + f_id + "}");

				str.Append(MenuSon(list_Menus[i].AdminMenuid, list_Menus_all, list_MenuLang,sort));
			}
			return str.ToString();
		}
		#endregion

		#region "菜单列表添加修改菜单"  
		[Authorization(Power = "Main")]
		public async Task<IActionResult> MenuAdd(long AdminMenuid = 0, long Fid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var vm_menuAdd = new MenuAdd();
			var model_Menu = _context.Queryable<AdminMenu>().First(u => u.AdminMenuid == AdminMenuid);
			if (model_Menu != null)
			{
				var model_Lang = _context.Queryable<AdminMenuLang>().First(u => u.AdminMenuid == AdminMenuid && u.Lang == Lang);
				vm_menuAdd.AdminMenuid = AdminMenuid;
				vm_menuAdd.Fid = model_Menu.Fid;
				vm_menuAdd.Title = model_Lang?.Title;
				vm_menuAdd.Sty = model_Menu.Sty;
				vm_menuAdd.Ico = model_Menu.Ico;
				vm_menuAdd.Sort = model_Menu.Sort;
				var list_MenuFunction = _context.Queryable<AdminMenuFunction>().Where(u => u.AdminMenuid == AdminMenuid && u.Fid == 0 && u.Status == 1).ToList();
				if (list_MenuFunction.Count > 0)
				{
					vm_menuAdd.Area = list_MenuFunction[0].Area;
					vm_menuAdd.Controller = list_MenuFunction[0].Controller;
					vm_menuAdd.Action = list_MenuFunction[0].Action;
					vm_menuAdd.Parameter = list_MenuFunction[0].Parameter;
					vm_menuAdd.Method = list_MenuFunction[0].Method;
				}
			}
			else
			{
				vm_menuAdd.AdminMenuid = 0;
				vm_menuAdd.Fid = Fid;
				vm_menuAdd.Sty = 1;
				vm_menuAdd.Area = _accessor.HttpContext.GetRouteData().Values["Area"].ToString();
				vm_menuAdd.Sort = 10;
			}
			vm_menuAdd.Lang = Lang;
			return View(vm_menuAdd);
		}

		//保存添改的菜单
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> MenuAdd(MenuAdd vm_menuAdd)
		{
			StringBuilder str = new StringBuilder();
			long fid = GSRequestHelper.GetFormLong(_accessor.HttpContext.Request, "input_fid", 0);
			var model_Menu = _context.Queryable<AdminMenu>().InSingle(vm_menuAdd.AdminMenuid);
			AdminMenu model_Menu_f = null;
			if (model_Menu != null)
			{
				if (("|" + model_Menu.Path + "|").Contains("|" + fid + "|") && fid != 0 && fid != model_Menu.Fid)
				{
					return Content("<script>alert('不允许选持本菜单的直系菜单，这将会造成死循环，请后退重选!');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				model_Menu.Sty = vm_menuAdd.Sty;
				model_Menu.Ico = vm_menuAdd.Ico;
				model_Menu.Sort = vm_menuAdd.Sort;
				model_Menu.Fid = fid;
				_context.BeginTran();
				#region "更新菜单事务"
				_context.Updateable(model_Menu).UpdateColumns(u => new { u.Sty, u.Ico, u.Sort, u.Fid }).ExecuteCommand();
				var model_MenuLang = _context.Queryable<AdminMenuLang>().First(u => u.AdminMenuid == vm_menuAdd.AdminMenuid && u.Lang == vm_menuAdd.Lang);
				if (model_MenuLang != null)
				{
					model_MenuLang.Title = vm_menuAdd.Title;
					_context.Updateable(model_MenuLang).ExecuteCommand();
				}
				else
				{
					model_MenuLang = new AdminMenuLang();
					model_MenuLang.AdminMenuid = vm_menuAdd.AdminMenuid;
					model_MenuLang.Title = vm_menuAdd.Title;
					model_MenuLang.Lang = vm_menuAdd.Lang;
					_context.Insertable(model_MenuLang).ExecuteCommand();
				}

				AdminMenuFunction model_MenuFunction;
				if (vm_menuAdd.Area != "" && vm_menuAdd.Controller != "")
				{
					//更新功能项
					var list_menu_function = _context.Queryable<AdminMenuFunction>().Where(u => u.AdminMenuid == vm_menuAdd.AdminMenuid && u.Fid == 0 && u.Status == 1).ToList();
					foreach (var model_function in list_menu_function)
					{
						model_function.Area = vm_menuAdd.Area;
						model_function.Controller = vm_menuAdd.Controller;
						model_function.Action = vm_menuAdd.Action;
						model_function.Parameter = vm_menuAdd.Parameter;
						model_function.Method = vm_menuAdd.Method;
						model_function.Ispage = true;
						model_function.Status = string.IsNullOrEmpty(vm_menuAdd.Controller) ? 0 : 1;
					}
					int count = _context.Updateable(list_menu_function).ExecuteCommand();
					if (count == 0)
					{
						model_MenuFunction = new AdminMenuFunction();
						model_MenuFunction.AdminMenuid = vm_menuAdd.AdminMenuid;
						model_MenuFunction.Fid = 0;
						model_MenuFunction.Area = vm_menuAdd.Area;
						model_MenuFunction.Controller = vm_menuAdd.Controller;
						model_MenuFunction.Action = vm_menuAdd.Action;
						model_MenuFunction.Parameter = vm_menuAdd.Parameter;
						model_MenuFunction.Method = vm_menuAdd.Method;
						model_MenuFunction.Status = string.IsNullOrEmpty(vm_menuAdd.Controller) ? 0 : 1;//控制器为空表示为非子节点，置为-1
						long menu_functionid = _context.Insertable(model_MenuFunction).ExecuteReturnBigIdentity();
						_context.Updateable<AdminMenuFunction>(u => new AdminMenuFunction { Path = menu_functionid.ToString(), Rootid = menu_functionid, Ispage = true })
							.Where(u => u.AdminMenuFunctionid == menu_functionid)
							.ExecuteCommand();
						model_MenuFunction.AdminMenuFunctionid = menu_functionid;
						list_menu_function.Add(model_MenuFunction);
					}

					var list_menu_funcitonid = list_menu_function.Select(s => s.AdminMenuFunctionid).ToList();//取所有功能项

					//更新功能项语言（已存在的）
					var list_menu_function_lang = _context.Queryable<AdminMenuFunctionLang>().Where(u =>u.Lang==vm_menuAdd.Lang && list_menu_funcitonid.Contains(u.AdminMenuFunctionid)).ToList();
					list_menu_function_lang.ForEach(model =>
					{
						model.Title = vm_menuAdd.Title + " (" + _localizer["权限"] + ")";
						list_menu_funcitonid.Remove(model.AdminMenuFunctionid);//去掉已更新的
					});

					//剩下的全是要补齐的
					foreach (var AdminMenuFunctionid in list_menu_funcitonid)
					{
						list_menu_function_lang.Add(new AdminMenuFunctionLang()
						{
							Lang = vm_menuAdd.Lang,
							AdminMenuFunctionid = AdminMenuFunctionid,
							Title = vm_menuAdd.Title + " (" + _localizer["权限"] + ")"
						});
					}
					//用AdminMenuFunctionid来判断存在则更新，不存在则添加
					var storage = _context.Storageable(list_menu_function_lang).WhereColumns(u => new { u.AdminMenuFunctionLangid }).ToStorage();
					storage.AsInsertable.ExecuteCommand();
					storage.AsUpdateable.ExecuteCommand();
				}
				#endregion
				_context.CommitTran();
				await updateMenu(new List<long> { vm_menuAdd.AdminMenuid }, fid);
			}
			else
			{
				_context.BeginTran();
				#region "添加菜单事务"
				//添加菜单主表
				model_Menu = new AdminMenu();
				model_Menu.Fid = fid;
				model_Menu.Sty = vm_menuAdd.Sty;
				model_Menu.Ico = vm_menuAdd.Ico;
				model_Menu.Sort = vm_menuAdd.Sort;
				model_Menu.Dtime = DateTime.Now;
				model_Menu.Status = 1;
				vm_menuAdd.AdminMenuid = _context.Insertable(model_Menu).ExecuteReturnBigIdentity();
				var model_MenuLang = new AdminMenuLang();
				model_MenuLang.Title = vm_menuAdd.Title;
				model_MenuLang.Lang = vm_menuAdd.Lang;
				model_MenuLang.AdminMenuid = vm_menuAdd.AdminMenuid;
				_context.Insertable(model_MenuLang).ExecuteCommand(); ;
				model_Menu_f = _context.Queryable<AdminMenu>().InSingle(fid);
				if (model_Menu_f != null)
				{
					model_Menu.Path = model_Menu_f.Path + "|" + vm_menuAdd.AdminMenuid;
					model_Menu.Rootid = model_Menu_f.Rootid;
					model_Menu.Depth = model_Menu_f.Depth + 1;
				}
				else
				{
					model_Menu.Path = vm_menuAdd.AdminMenuid.ToString();
					model_Menu.Rootid = vm_menuAdd.AdminMenuid;
					model_Menu.Depth = 1;
				}
				model_Menu.AdminMenuid = vm_menuAdd.AdminMenuid;
				_context.Updateable(model_Menu).UpdateColumns(u => new { u.Path, u.Rootid, u.Depth }).ExecuteCommand();

				//创建功能项默认
				long menu_functionid = 0;
				var model_MenuFunction = _context.Queryable<AdminMenuFunction>().First(u => u.AdminMenuid == vm_menuAdd.AdminMenuid && u.Fid == 0 && u.Status == 1);
				if (model_MenuFunction != null)
				{
					menu_functionid = model_MenuFunction.AdminMenuFunctionid;
					model_MenuFunction.Area = vm_menuAdd.Area;
					model_MenuFunction.Controller = vm_menuAdd.Controller;
					model_MenuFunction.Action = vm_menuAdd.Action;
					model_MenuFunction.Parameter = vm_menuAdd.Parameter;
					model_MenuFunction.Method = vm_menuAdd.Method;
					_context.Updateable(model_MenuFunction).ExecuteCommand();
				}
				else
				{
					model_MenuFunction = new AdminMenuFunction();
					model_MenuFunction.AdminMenuid = vm_menuAdd.AdminMenuid;
					model_MenuFunction.Fid = 0;
					model_MenuFunction.Area = vm_menuAdd.Area;
					model_MenuFunction.Controller = vm_menuAdd.Controller;
					model_MenuFunction.Action = vm_menuAdd.Action;
					model_MenuFunction.Parameter = vm_menuAdd.Parameter;
					model_MenuFunction.Method = vm_menuAdd.Method;
					model_MenuFunction.AdminMenuFunctionid = _context.Insertable(model_MenuFunction).ExecuteReturnBigIdentity();
					menu_functionid = model_MenuFunction.AdminMenuFunctionid;
					model_MenuFunction.Path = menu_functionid.ToString();
					model_MenuFunction.Rootid = menu_functionid;
				}
				model_MenuFunction.Status = string.IsNullOrEmpty(model_MenuFunction.Controller) ? 0 : 1;//控制器为空表示为非子节点，置为-1
				model_MenuFunction.Ispage = true;
				_context.Updateable(model_MenuFunction).UpdateColumns(u => new { u.Path, u.Rootid, u.Status, u.Ispage }).ExecuteCommand();

				var model_MenuFunctionLang = new AdminMenuFunctionLang();
				model_MenuFunctionLang.AdminMenuFunctionid = menu_functionid;
				model_MenuFunctionLang.Lang = vm_menuAdd.Lang;
				model_MenuFunctionLang.Title = vm_menuAdd.Title.Trim() + "权限";
				_context.Insertable(model_MenuFunctionLang).ExecuteCommand();
				#endregion
				_context.CommitTran();
			}
			return Content("<script>parent.$('#table_view').treegrid('reload');parent.$.messager.show({title:'" + _localizer["修改菜单信息菜单"] + "',msg:'" + _localizer["保存信息成功"] + "',timeout:3000,showType:'slide'});location.href='?AdminMenuid=" + vm_menuAdd.AdminMenuid + "&lang=" + vm_menuAdd.Lang + "' </script>", "text/html", Encoding.GetEncoding("utf-8"));
		}

		/// <summary>
		/// 更新菜单上下级关系，是本节点下属所有子节点一系列的更新
		/// </summary>
		/// <param name="AdminMenuids"></param>
		/// <param name="fid"></param>
		/// <returns></returns>
		private async Task updateMenu(List<long> AdminMenuids, long fid = 0)
		{
			var list_Menu = _context.Queryable<AdminMenu>().Where(u => AdminMenuids.Contains(u.AdminMenuid)).ToList();
			var list_Menu_f = _context.Queryable<AdminMenu>().Where(u => list_Menu.Select(s => s.Fid).Contains(u.AdminMenuid)).ToList();
			for (int i = 0; i < list_Menu.Count; i++)
			{
				var model_Menu_f = list_Menu_f.FirstOrDefault(u => u.AdminMenuid == fid);
				string path = "";
				long rootid = 0;
				if (model_Menu_f != null)
				{
					path = model_Menu_f.Path + "|" + list_Menu[i].AdminMenuid;
					rootid = model_Menu_f.Rootid;
					list_Menu[i].Fid = fid;
					list_Menu[i].Path = path;
					list_Menu[i].Rootid = rootid;
					list_Menu[i].Depth = model_Menu_f.Depth + 1;
				}
				else
				{
					path = list_Menu[i].AdminMenuid.ToString();
					rootid = list_Menu[i].AdminMenuid;
					list_Menu[i].Fid = 0;
					list_Menu[i].Path = path;
					list_Menu[i].Rootid = rootid;
					list_Menu[i].Depth = 1;
				}
				//更新父节点，父节点不需要权限
				_context.Updateable<AdminMenuFunction>()
					.SetColumns(u => u.Status == -1)
					.Where(u => u.AdminMenuid == fid && u.Status != -1)
					.ExecuteCommand();

				var list_AdminMenuids = _context.Queryable<AdminMenu>().Where(u => u.Fid == list_Menu[i].AdminMenuid).Select(u => u.AdminMenuid).ToList();
				if (list_AdminMenuids.Count > 0) updateMenu(list_AdminMenuids, list_Menu[i].AdminMenuid).Wait();
			}
			_context.Updateable(list_Menu).UpdateColumns(u => new { u.Fid, u.Path, u.Rootid, u.Depth }).ExecuteCommand();
		}
		#endregion

		#region "菜单控件列表" 
		[Authorization(Power = "Main")]
		public async Task<IActionResult> MenuFunction(long AdminMenuid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			var view = new MenuFunction();
			view.AdminMenuid = AdminMenuid;
			return View(view);
		}

		[HttpGet]
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> MenuFunctionData(long AdminMenuid = 0)
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long AdminMenuFunctionid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "AdminMenuFunctionid", 0);
			string AdminMenuFunctionids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "AdminMenuFunctionids");
			//List<ViewAdminMenuFunction > list_MenuFunctions;

			var list_menu_functionid = AdminMenuFunctionids.Split(',').Where(u => !string.IsNullOrEmpty(u)).Select(u => long.Parse(u)).ToList();

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					if (IsAdmin)
					{
						var list_MenuFunctions = _context.Queryable<AdminMenuFunction>().Where(u => list_menu_functionid.Contains(u.AdminMenuFunctionid)).ToList();
						if (list_MenuFunctions.Count > 0)
						{
							for (int i = 0; i < list_MenuFunctions.Count(); i++)
							{
								if (list_MenuFunctions[i].Fid == 0)
								{
									str.Append("{\"errorcode\":\"-1\",\"msg\":\"基础查询权限不允许删除！\"}");
								}
								else
								{
									list_MenuFunctions[i].Status = -1;
								}
							}
							_context.Updateable(list_MenuFunctions).UpdateColumns(u => u.Status).ExecuteCommand();

							//删除权限与控件关联表
							_context.Updateable<AdminRoleMenuFunction>().SetColumns(u => u.Status == -1).Where(u => list_menu_functionid.Contains(u.AdminMenuFunctionid)).ExecuteCommand();

							////删除管理用户与控件关联表
							//var list_admin_masterMenuFunction = await _context.admin_masterMenuFunction.Where(u => arr.Contains(u.AdminMenuFunctionid)).ToListAsync();
							//for (int i = 0; i < list_admin_masterMenuFunction.Count; i++)
							//{
							//    _context.admin_masterMenuFunction.Remove(list_admin_masterMenuFunction[i]);
							//}
							str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
						}
						else
						{
							str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,不存在此记录"] + "!\"}");
						}
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除失败,无权删除"] + "!\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					var exp = new Expressionable<ViewAdminMenuFunction>();
					exp.And(u => u.AdminMenuid == AdminMenuid && u.Status != -1 && u.Lang == Lang);
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
					sort += "fid,sort,admin_menu_functionid desc";
					var list_MenuFunctions_all = _context.Queryable<ViewAdminMenuFunction>().Where(exp.ToExpression()).OrderBy(sort).ToList();
					var list_VmMenuFunctions = list_MenuFunctions_all.Where(u => u.Fid == 0).Skip((page - 1) * pagesize).Take(pagesize).ToList();

					str.Append("{\"total\":" + list_MenuFunctions_all.Where(u => u.Fid == 0).Count() + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < list_VmMenuFunctions.Count; i++)
					{
						if (i > 0) str.Append(",");
						str.Append("{");
						str.Append("\"AdminMenuFunctionid\":\"" + list_VmMenuFunctions[i].AdminMenuFunctionid.ToString() + "\",");
						str.Append("\"IsPage\":\"" + (list_VmMenuFunctions[i].Ispage.ToString() == "1" ? _localizer["是"] : _localizer["否"]) + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(list_VmMenuFunctions[i].Title) + "\",");
						str.Append("\"Fid\":\"" + list_VmMenuFunctions[i].Fid + "\",");
						str.Append("\"Dtime\":\"" + list_VmMenuFunctions[i].Dtime.ToString() + "\",");
						str.Append("\"ControlId\":\"" + JsonHelper.JsonCharFilter(list_VmMenuFunctions[i].ControlId+"") + "\",");
						str.Append("\"FunctionName\":\"" + JsonHelper.JsonCharFilter(list_VmMenuFunctions[i].FunctionName+"") + "\",");
						str.Append("\"Area\":\"" + list_VmMenuFunctions[i].Area + "\",");
						str.Append("\"Controller\":\"" + list_VmMenuFunctions[i].Controller + "\",");
						str.Append("\"Action\":\"" + list_VmMenuFunctions[i].Action + "\",");
						str.Append("\"Method\":\"" + list_VmMenuFunctions[i].Method + "\",");
						str.Append("\"Sort\":\"" + list_VmMenuFunctions[i].Sort + "\",");
						if (list_VmMenuFunctions[i].Ispage)
						{
							str.Append("\"add\":\"" + _localizer["添加"] + "\",");
						}
						else
						{
							str.Append("\"add\":\"\",");
						}
						str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						str.Append(MenuFunctionSon(list_VmMenuFunctions[i].AdminMenuFunctionid, list_MenuFunctions_all));
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}

		private string MenuFunctionSon(long f_id, List<ViewAdminMenuFunction > model_MenuFunctions_all)
		{
			StringBuilder str = new StringBuilder();
			//DataSet ds = bll_function.GetList("fid=" + f_id + " and status<>-1 order by fid,sort,AdminMenuFunctionid desc");
			var model_MenuFunctions = model_MenuFunctions_all.Where(u => u.Fid == f_id && u.Status != -1).OrderBy(u=>new { u.Fid,u.Sort}).OrderByDescending(u=>u.AdminMenuFunctionid).ToList();
			for (int i = 0; i < model_MenuFunctions.Count; i++)
			{
				str.Append(",{");
				str.Append("\"AdminMenuFunctionid\":\"" + model_MenuFunctions[i].AdminMenuFunctionid.ToString() + "\",");
				str.Append("\"IsPage\":\"" + (model_MenuFunctions[i].Ispage.ToString() == "1" ? _localizer["是"] : _localizer["否"]) + "\",");
				str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(model_MenuFunctions[i].Title) + "\",");
				str.Append("\"Fid\":\"" + model_MenuFunctions[i].Fid + "\",");
				str.Append("\"Dtime\":\"" + model_MenuFunctions[i].Dtime.ToString() + "\",");
				str.Append("\"ControlId\":\"" + JsonHelper.JsonCharFilter(model_MenuFunctions[i].ControlId.ToString()) + "\",");
				str.Append("\"FunctionName\":\"" + JsonHelper.JsonCharFilter(model_MenuFunctions[i].FunctionName.ToString()) + "\",");
				str.Append("\"Area\":\"" + model_MenuFunctions[i].Area + "\",");
				str.Append("\"Method\":\"" + model_MenuFunctions[i].Method + "\",");
				str.Append("\"Controller\":\"" + model_MenuFunctions[i].Controller + "\",");
				str.Append("\"Action\":\"" + model_MenuFunctions[i].Action + "\",");
				str.Append("\"sort\":\"" + model_MenuFunctions[i].Sort + "\",");
				if (model_MenuFunctions[i].Ispage.ToString() == "1")
				{
					str.Append("\"add\":\"" + _localizer["添加"] + "\",");
				}
				else
				{
					str.Append("\"add\":\"\",");
				}
				str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\",");
				str.Append("\"_parentId\":" + f_id + "}");

				str.Append(MenuFunctionSon(model_MenuFunctions[i].AdminMenuFunctionid, model_MenuFunctions_all));
			}
			return str.ToString();
		}
		#endregion

		#region "菜单控件列表添加修改菜单"      
		[Authorization(Power = "Main")]
		public async Task<IActionResult> MenuFunctionAdd(long AdminMenuid = 0, long AdminMenuFunctionid = 0, long Fid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			MenuFunctionAdd menuFunctionAdd = new MenuFunctionAdd();
			var model_MenuFunction = _context.Queryable<ViewAdminMenuFunction>().First(u => u.AdminMenuFunctionid == AdminMenuFunctionid && u.Lang == Lang);
			if (model_MenuFunction != null)
			{
				menuFunctionAdd.MenuTitle = model_MenuFunction.Title;

				menuFunctionAdd.AdminMenuid = AdminMenuid;
				menuFunctionAdd.Fid = model_MenuFunction.Fid;
				menuFunctionAdd.Title = model_MenuFunction.Title;
				menuFunctionAdd.FunctionName = model_MenuFunction.FunctionName;
				menuFunctionAdd.ControlId = model_MenuFunction.ControlId;
				menuFunctionAdd.Intro = model_MenuFunction.Intro;
				menuFunctionAdd.Ispage = model_MenuFunction.Ispage;
				menuFunctionAdd.Sort = model_MenuFunction.Sort;
				menuFunctionAdd.Area = model_MenuFunction.Area;
				menuFunctionAdd.Controller = model_MenuFunction.Controller;
				menuFunctionAdd.Action = model_MenuFunction.Action;
				menuFunctionAdd.Parameter = model_MenuFunction.Parameter;
				menuFunctionAdd.Method = model_MenuFunction.Method;
			}
			else
			{
				menuFunctionAdd.AdminMenuid = AdminMenuid;
				menuFunctionAdd.Fid = Fid;
				menuFunctionAdd.Area = _accessor.HttpContext.GetRouteValue("Area").ToString();
				menuFunctionAdd.Sort = 10;
				menuFunctionAdd.Method = "get";
			}
			return View(menuFunctionAdd);
		}

		//保存添改的菜单控件
		[HttpPost]
		[Authorization(Power = "Main")]
		public async Task<IActionResult> MenuFunctionAdd(MenuFunctionAdd menuFunctionAdd)
		{
			string path_f = "";
			StringBuilder str = new StringBuilder();
			long fid = GSRequestHelper.GetFormLong(_accessor.HttpContext.Request, "input_fid", 0);
			var model_MenuFunction = _context.Queryable<AdminMenuFunction>().InSingle(menuFunctionAdd.AdminMenuFunctionid);
			AdminMenuFunction model_MenuFunction_f = null;
			if (model_MenuFunction != null)
			{
				model_MenuFunction_f = _context.Queryable<AdminMenuFunction>().InSingle(fid);
				if (model_MenuFunction_f != null)
				{
					path_f = model_MenuFunction_f.Path;
				}
				if (("|" + path_f + "|").Contains("|" + menuFunctionAdd.AdminMenuFunctionid + "|") && fid != 0 && fid != model_MenuFunction.Fid)
				{
					return Content("<script>alert('" + _localizer["不允许选持本功能控件的直系功能控件，这将会造成死循环，请后退重选"] + "');</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				if (!string.IsNullOrEmpty(menuFunctionAdd.FunctionName) && _context.Queryable<AdminMenuFunction>().Any(u => u.AdminMenuFunctionid != menuFunctionAdd.AdminMenuFunctionid && u.AdminMenuid == menuFunctionAdd.AdminMenuid && u.FunctionName == menuFunctionAdd.FunctionName.Trim()))
				{
					return Content("<script>alert('" + _localizer["同菜单下不允许出现相同的方法名"] + "');</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				if (!string.IsNullOrEmpty(menuFunctionAdd.ControlId) && _context.Queryable<AdminMenuFunction>().Any(u => u.AdminMenuFunctionid != menuFunctionAdd.AdminMenuFunctionid && u.Fid == fid && u.ControlId == menuFunctionAdd.ControlId.Trim()))
				{
					return Content("<script>alert('" + _localizer["同级控件不允许出现相同的控件ID"] + "');</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}

				if (model_MenuFunction_f != null)
				{
					model_MenuFunction.Fid = fid;
					model_MenuFunction.Path = model_MenuFunction_f.Path + "|" + menuFunctionAdd.AdminMenuFunctionid;
					model_MenuFunction.Rootid = model_MenuFunction_f.Rootid;
					model_MenuFunction.Depth = model_MenuFunction_f.Depth + 1;
				}
				else
				{
					model_MenuFunction.Fid = 0;
					model_MenuFunction.Path = menuFunctionAdd.AdminMenuFunctionid.ToString();
					model_MenuFunction.Rootid = menuFunctionAdd.AdminMenuFunctionid;
					model_MenuFunction.Depth = 1;
				}
				model_MenuFunction.AdminMenuid = menuFunctionAdd.AdminMenuid;
				model_MenuFunction.FunctionName = menuFunctionAdd.FunctionName.Trim();
				model_MenuFunction.ControlId = menuFunctionAdd.ControlId.Trim();
				model_MenuFunction.Ispage = menuFunctionAdd.Ispage;
				model_MenuFunction.Area = menuFunctionAdd.Area.Trim();
				model_MenuFunction.Controller = menuFunctionAdd.Controller.Trim();
				model_MenuFunction.Action = menuFunctionAdd.Action.Trim();
				model_MenuFunction.Parameter = menuFunctionAdd.Parameter;
				model_MenuFunction.Intro = menuFunctionAdd.Intro;
				model_MenuFunction.Sort = menuFunctionAdd.Sort;
				model_MenuFunction.Method = menuFunctionAdd.Method;

				_context.Updateable<AdminMenuFunctionLang>()
					.SetColumns(u => u.Title == menuFunctionAdd.Title)
					.Where(u => u.AdminMenuFunctionid == menuFunctionAdd.AdminMenuFunctionid && u.Lang == Lang)
					.ExecuteCommand();
				_context.Updateable(model_MenuFunction).ExecuteCommand();

			}
			else
			{
				if (!string.IsNullOrEmpty(menuFunctionAdd.FunctionName) && _context.Queryable<AdminMenuFunction>().Any(u => u.AdminMenuid == menuFunctionAdd.AdminMenuid && u.FunctionName == menuFunctionAdd.FunctionName.Trim()))
				{
					return Content("<script>alert('" + _localizer["同菜单下不允许出现相同的方法名"] + "');</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				if (!string.IsNullOrEmpty(menuFunctionAdd.ControlId) && _context.Queryable<AdminMenuFunction>().Any(u => u.Fid == fid && u.ControlId == menuFunctionAdd.ControlId.Trim()))
				{
					return Content("<script>alert('" + _localizer["同级控件不允许出现相同的控件ID"] + "');</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				model_MenuFunction = new AdminMenuFunction();
				model_MenuFunction.Fid = fid;
				model_MenuFunction.AdminMenuid = menuFunctionAdd.AdminMenuid;
				model_MenuFunction.FunctionName = menuFunctionAdd.FunctionName.Trim();
				model_MenuFunction.ControlId = menuFunctionAdd.ControlId.Trim();
				model_MenuFunction.Ispage = menuFunctionAdd.Ispage;
				model_MenuFunction.Area = menuFunctionAdd.Area.Trim();
				model_MenuFunction.Controller = menuFunctionAdd.Controller.Trim();
				model_MenuFunction.Action = menuFunctionAdd.Action.Trim();
				model_MenuFunction.Parameter = menuFunctionAdd.Parameter.Trim();
				model_MenuFunction.Intro = menuFunctionAdd.Intro.Trim();
				model_MenuFunction.Sort = menuFunctionAdd.Sort;
				model_MenuFunction.Method = menuFunctionAdd.Method;

				model_MenuFunction.Dtime = DateTime.Now;
				model_MenuFunction.AdminMenuFunctionid = _context.Insertable(model_MenuFunction).ExecuteReturnBigIdentity();

				model_MenuFunction_f = _context.Queryable<AdminMenuFunction>().InSingle(fid);
				if (model_MenuFunction_f != null)
				{
					model_MenuFunction.Path = model_MenuFunction_f.Path + "|" + model_MenuFunction.AdminMenuFunctionid;
					model_MenuFunction.Rootid = model_MenuFunction_f.Rootid;
					model_MenuFunction.Depth = model_MenuFunction_f.Depth + 1;
				}
				else
				{
					model_MenuFunction.Path = model_MenuFunction.AdminMenuFunctionid.ToString();
					model_MenuFunction.Rootid = model_MenuFunction.AdminMenuFunctionid;
					model_MenuFunction.Depth = 1;
				}
				var model_MenuFunctionLang = new AdminMenuFunctionLang();
				model_MenuFunctionLang.AdminMenuFunctionid = model_MenuFunction.AdminMenuFunctionid;
				model_MenuFunctionLang.Lang = Lang;
				model_MenuFunctionLang.Title = menuFunctionAdd.Title.Trim();
				_context.Insertable(model_MenuFunctionLang).ExecuteCommand();
				_context.Updateable(model_MenuFunction).UpdateColumns(u => new { u.Path, u.Rootid, u.Depth }).ExecuteCommand();
			}
			return Content("<script>parent.$('#win').window('close');parent.$('#table_view').treegrid('reload');parent.$.messager.show({title:'" + _localizer["保存功能控件信息功能控件成功"] + "',msg:'" + _localizer["保存功能控件信息功能控件成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
		}
		#endregion

		#region "树型菜单的JSON"
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> MenuClass(long AdminMenuid = 0, long Fid = 0)
		{
			StringBuilder str = new StringBuilder();
			string path = "";
			long id = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "id", 0);//每次点击节点打开下一节点时
			var list_menu_all = _context.Queryable<AdminMenu>().Where(u => u.Status == 1).OrderBy(u => new { u.Sty, u.Sort }).OrderBy(u => u.AdminMenuid, OrderByType.Desc).ToList();//((IsDevelopment == 1) ? "" : " and sty=1") 
			var list_Lang = _context.Queryable<AdminMenuLang>().Where(u => list_menu_all.Select(s => s.AdminMenuid).Contains(u.AdminMenuid) && u.Lang==Lang).ToList();
			var model_Menu = list_menu_all.FirstOrDefault(u => u.AdminMenuid == AdminMenuid);
			if (model_Menu != null)
			{
				path = model_Menu.Path;
			}
			else
			{
				var model_MenuF = list_menu_all.FirstOrDefault(u => u.AdminMenuid == Fid);
				path = model_MenuF != null ? model_MenuF.Path : "";
			}

			str.Append("[");
			if (Fid == 0) str.Append("{\"id\":0,\"text\":\"根菜单\"},");

			var list_menus = list_menu_all.Where(u => u.Fid == id && u.Status == 1).ToList();
			for (int i = 0; i < list_menus.Count; i++)
			{
				var model_Lang = list_Lang.FirstOrDefault(u => u.AdminMenuid == list_menus[i].AdminMenuid);
				str.Append("{");
				str.Append("    \"id\":" + list_menus[i].AdminMenuid + ",");
				str.Append("    \"text\":\"" + model_Lang?.Title + "\",");
				//判断是否加载下级子菜单
				if (("|" + path + "|").Contains("|" + list_menus[i].AdminMenuid.ToString() + "|"))
				{
					//加载，则判断是否有子菜单
					if (list_menu_all.Any(u => u.Fid == list_menus[i].AdminMenuid))
					{
						str.Append("    \"iconCls\":\"icon-ok\",");
						str.Append("    \"state\":\"open\",");
						str.Append(MenuClassSon(list_menus[i].AdminMenuid, list_menu_all, list_Lang, path));
					}
				}
				else
				{
					//不加载，则判断是否有子菜单
					if (list_menu_all.Any(u => u.Fid == list_menus[i].AdminMenuid))
					{
						str.Append("    \"state\":\"closed\",");
					}
				}
				str.Remove(str.Length - 1, 1);
				str.Append("    },");
			}
			str.Remove(str.Length - 1, 1);
			str.Append("]");
			return Content(str.ToString());
		}
		private string MenuClassSon(long fid, List<AdminMenu> admin_menus,List<AdminMenuLang> list_Lang,string path)
		{
			StringBuilder str = new StringBuilder();
			var list_menus = admin_menus.Where(u => u.Fid == fid && u.Status == 1).OrderBy(u => u.Sort).OrderByDescending(u => u.AdminMenuid).ToList();
			str.Append("    \"children\":[");
			for (int i = 0; i < list_menus.Count; i++)
			{
				var model_Lang = list_Lang.FirstOrDefault(u => u.AdminMenuid == list_menus[i].AdminMenuid);
				str.Append("{");
				str.Append("    \"id\":" + list_menus[i].AdminMenuid.ToString() + ",");
				str.Append("    \"text\":\"" + model_Lang?.Title + "\",");
				//判断是否加载下级子菜单
				if (("|" + path + "|").Contains("|" + list_menus[i].AdminMenuid.ToString() + "|"))
				{
					//加载，则判断是否有子菜单
					if (admin_menus.Any(u => u.Fid == list_menus[i].AdminMenuid))
					{
						str.Append("    \"iconCls\":\"icon-ok\",");
						str.Append("    \"state\":\"open\",");
						str.Append(MenuClassSon(int.Parse(list_menus[i].AdminMenuid.ToString()), admin_menus, list_Lang, path));
					}
				}
				else
				{
					//不加载，则判断是否有子菜单
					if (admin_menus.Any(u => u.Fid == list_menus[i].AdminMenuid))
					{
						str.Append("    \"state\":\"closed\",");
					}
				}
				str.Remove(str.Length - 1, 1);
				str.Append("},");
			}
			str.Remove(str.Length - 1, 1);
			str.Append("],");
			return str.ToString();
		}
		#endregion

		#region "树型菜单控件的JSON"
		[HttpPost]
		public async Task<IActionResult> MenuFunctionClass(long AdminMenuid = 0, long AdminMenuFunctionid = 0, long Fid = 0)
		{
			StringBuilder str = new StringBuilder();
			string path = "";
			long id = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "id", 0);//每次点击节点打开下一节点时
			var list_menu_functions = _context.Queryable<ViewAdminMenuFunction>().Where(u => u.Ispage && u.AdminMenuid == AdminMenuid && u.Status == 1 && u.Lang == Lang).ToList();//((IsDevelopment == 1) ? "" : " and sty=1") 
			var model_MenuFunction = list_menu_functions.FirstOrDefault(u => u.AdminMenuFunctionid == AdminMenuFunctionid);
			if (model_MenuFunction != null)
			{
				path = model_MenuFunction.Path;
			}
			else
			{
				var model_MenuFunctionsF = list_menu_functions.FirstOrDefault(u => u.AdminMenuFunctionid == Fid);
				path = model_MenuFunctionsF != null ? model_MenuFunctionsF.Path : "";
			}
			str.Append("[");

			var model_admin_menu_functions = list_menu_functions.Where(u => u.Fid == id && u.Status == 1).OrderBy(u => u.Sort).OrderByDescending(u => u.AdminMenuFunctionid).ToList();//((IsDevelopment == 1) ? "" : " and sty=1") 
			for (int i = 0; i < model_admin_menu_functions.Count; i++)
			{
				str.Append("{");
				str.Append("    \"id\":" + model_admin_menu_functions[i].AdminMenuFunctionid + ",");
				str.Append("    \"text\":\"" + JsonHelper.JsonCharFilter(model_admin_menu_functions[i].Title) + "\",");
				//判断是否加载下级子菜单
				if (("|" + path + "|").Contains("|" + model_admin_menu_functions[i].AdminMenuFunctionid.ToString() + "|"))
				{
					//加载，则判断是否有子菜单
					if (list_menu_functions.Any(u => u.Fid == model_admin_menu_functions[i].AdminMenuFunctionid))
					{
						str.Append("    \"iconCls\":\"icon-ok\",");
						str.Append("    \"state\":\"open\",");
						str.Append(MenuFunctionClassSon(model_admin_menu_functions[i].AdminMenuFunctionid, list_menu_functions, path));
					}
				}
				else
				{
					//不加载，则判断是否有子菜单
					if (list_menu_functions.Any(u => u.Fid == model_admin_menu_functions[i].AdminMenuFunctionid))
					{
						str.Append("    \"state\":\"closed\",");
					}
				}
				str.Remove(str.Length - 1, 1);
				str.Append("    },");
			}
			str.Remove(str.Length - 1, 1);
			str.Append("]");
			return Content(str.ToString());
		}
		private string MenuFunctionClassSon(long fid, List<ViewAdminMenuFunction> admin_menu_functions, string path)
		{
			StringBuilder str = new StringBuilder();
			var model_admin_menu_functions = admin_menu_functions.Where(u => u.Fid == fid && u.Status == 1).OrderBy(u => u.Sort).OrderByDescending(u => u.AdminMenuFunctionid).ToList();
			str.Append("    \"children\":[");
			for (int i = 0; i < model_admin_menu_functions.Count; i++)
			{
				str.Append("{");
				str.Append("    \"id\":" + model_admin_menu_functions[i].AdminMenuFunctionid.ToString() + ",");
				str.Append("    \"text\":\"" + JsonHelper.JsonCharFilter(model_admin_menu_functions[i].Title) + "\",");
				//判断是否加载下级子菜单
				if (("|" + path + "|").Contains("|" + model_admin_menu_functions[i].AdminMenuFunctionid.ToString() + "|"))
				{
					//加载，则判断是否有子菜单
					if (admin_menu_functions.Any(u => u.Fid == model_admin_menu_functions[i].AdminMenuFunctionid))
					{
						str.Append("    \"iconCls\":\"icon-ok\",");
						str.Append("    \"state\":\"open\",");
						str.Append(MenuFunctionClassSon(model_admin_menu_functions[i].AdminMenuFunctionid, admin_menu_functions, path));
					}
				}
				else
				{
					//不加载，则判断是否有子菜单
					if (admin_menu_functions.Any(u => u.Fid == model_admin_menu_functions[i].AdminMenuFunctionid && u.Status == 1))
					{
						str.Append("    \"state\":\"closed\",");
					}
				}
				str.Remove(str.Length - 1, 1);
				str.Append("},");
			}
			str.Remove(str.Length - 1, 1);
			str.Append("],");
			return str.ToString();
		}
        #endregion
        #endregion

        #region "字典栏目" 

        #region "字典列表"   
        [Authorization(Power = "Main")]
        public async Task<IActionResult> Dictionary()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> DictionaryData(string keys = "", string status = "")
		{
			StringBuilder str = new StringBuilder();

			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string PubConfigids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "PubConfigids");
			var list_PubConfigid = PubConfigids.Split(',').Where(u=>!string.IsNullOrEmpty(u)).Select(u => long.Parse(u)).ToList();			
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					//_logger.LogInformation("删除数据字典字段pub_logids:[" + String.Join(',', list_PubConfigid) + "]");
					int count = _context.Updateable<PubConfig>().SetColumns(u => u.Status == -1).Where(u => list_PubConfigid.Contains(u.PubConfigid) && !u.Isadmin).ExecuteCommand();
					if (count>0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\""+ _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\""+ _localizer["删除失败,不存在此记录"] + "\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					var exp = new Expressionable<ViewPubConfig>();
					exp = exp.And(u => u.Status == 1 && (u.Islang == 0 || (u.Islang == 1 && u.Lang == Lang)));
					if (!string.IsNullOrEmpty(keys))
					{
						exp.And(u => u.Title.Contains(keys.Trim()) || u.Key.Contains(keys.Trim()) || u.Val.Contains(keys.Trim()) || u.Description.Contains(keys.Trim()));
					}
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
					sort += "pub_configid desc";
					var pageInfo = _context.Queryable<ViewPubConfig>().Where(exp.ToExpression()).OrderBy(sort).ToPageInfo(page, pagesize);

					str.Append("{\"total\":" + pageInfo.TotalCount + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < pageInfo.DataSource.Count; i++)
					{
						str.Append("{");
						str.Append("\"PubConfigid\":\"" + pageInfo.DataSource[i].PubConfigid.ToString() + "\",");
						str.Append("\"Isadmin\":\"" + pageInfo.DataSource[i].Isadmin.ToString() + "\",");
						str.Append("\"Islang\":\"" + (pageInfo.DataSource[i].Islang==0? _localizer["通用"]: _localizer["多语言"]) + "\",");
						str.Append("\"Isadmin_str\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].Isadmin ? _localizer["是"] : _localizer["否"]) + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].Title + "") + "\",");
						str.Append("\"Description\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].Description) + "\",");
						str.Append("\"Name\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].Key) + "\",");
						str.Append("\"Val\":\"" + JsonHelper.JsonCharFilter(string.IsNullOrEmpty(pageInfo.DataSource[i].Val) ? pageInfo.DataSource[i].Longval : pageInfo.DataSource[i].Val) + "\",");
						str.Append("\"Dtime\":\"" + pageInfo.DataSource[i].Dtime.ToString() + "\",");
						str.Append("\"Status\":\"" + pageInfo.DataSource[i].Status + "\",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (pageInfo.DataSource.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}
			return Content(str.ToString());
		}
        #endregion

        #region "数据字典列表添加修改"
        [Authorization(Power = "Main")]
        public async Task<IActionResult> DictionaryAdd(long PubConfigid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			DictionaryAdd vm_DictionaryAdd = new DictionaryAdd();

			var model_PubConfig = _context.Queryable<PubConfig>().InSingle(PubConfigid);
			if (model_PubConfig != null)
			{
				vm_DictionaryAdd.PubConfigid = PubConfigid;
				vm_DictionaryAdd.Title = model_PubConfig.Title;
				vm_DictionaryAdd.Key = model_PubConfig.Key;
				vm_DictionaryAdd.Isadmin = model_PubConfig.Isadmin;
				vm_DictionaryAdd.Description = model_PubConfig.Description;
				vm_DictionaryAdd.Status = model_PubConfig.Status;
				vm_DictionaryAdd.Islang = model_PubConfig.Islang;
				vm_DictionaryAdd.Lang = Lang;
				if (model_PubConfig.Islang == 1)
				{
					var model_Lang = _context.Queryable<PubConfigLang>().First(u => u.Lang == Lang && u.PubConfigid == PubConfigid);
					if (model_Lang != null)
					{
						vm_DictionaryAdd.Val = model_Lang.Val;
						vm_DictionaryAdd.Longval = model_Lang.Longval;
					}
				}
				else
				{
					vm_DictionaryAdd.Val = model_PubConfig.Val;
					vm_DictionaryAdd.Longval = model_PubConfig.Longval;
				}
			}
			else
			{
				vm_DictionaryAdd.Islang = 0;
				vm_DictionaryAdd.Lang = Lang;
				vm_DictionaryAdd.Status = 1;
				vm_DictionaryAdd.Isadmin = false;
			}
			vm_DictionaryAdd.IsCurrency = vm_DictionaryAdd.Islang == 0;
			return View(vm_DictionaryAdd);
		}

		//保存添改的数据字典
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> DictionaryAdd(DictionaryAdd vm_DictionaryAdd)
		{
			var model_ConfigLang = new PubConfigLang();
			var model_PubConfig = _context.Queryable<PubConfig>().InSingle(vm_DictionaryAdd.PubConfigid);
			var api = new ApiResult();
			if (model_PubConfig != null)
			{
				//只有不是管理字段，才可以改标题和名，还有状态
				if (!model_PubConfig.Isadmin)
				{
					model_PubConfig.Title = vm_DictionaryAdd.Title;
					model_PubConfig.Key = vm_DictionaryAdd.Key;
					model_PubConfig.Status = vm_DictionaryAdd.Status;
				}
				if (vm_DictionaryAdd.IsCurrency)
				{
					model_PubConfig.Islang = 0;
					model_PubConfig.Val = vm_DictionaryAdd.Val;
					model_PubConfig.Longval = vm_DictionaryAdd.Longval;
					_context.Deleteable<PubConfigLang>().Where(u => u.PubConfigid == model_PubConfig.PubConfigid).ExecuteCommand();
				}
				else
				{
					model_PubConfig.Islang = 1;
					model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == model_PubConfig.PubConfigid && u.Lang == vm_DictionaryAdd.Lang);
					if (model_ConfigLang != null)
					{
						model_ConfigLang.Val = vm_DictionaryAdd.Val;
						model_ConfigLang.Longval = vm_DictionaryAdd.Longval;
						_context.Updateable(model_ConfigLang).ExecuteCommand();
					}
					else
					{
						model_ConfigLang = new PubConfigLang();
						model_ConfigLang.PubConfigid = model_PubConfig.PubConfigid;
						model_ConfigLang.Val = vm_DictionaryAdd.Val;
						model_ConfigLang.Longval = vm_DictionaryAdd.Longval;
						model_ConfigLang.Lang = vm_DictionaryAdd.Lang;
						_context.Insertable(model_ConfigLang).ExecuteCommand();
					}
				}
				model_PubConfig.Description = vm_DictionaryAdd.Description;
				_context.Updateable(model_PubConfig).ExecuteCommand();
				_context.DataCache.RemoveDataCache("view_pub_config");
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["保存数据字段"] + "',msg:'" + _localizer["修改数据字典信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_PubConfig = new PubConfig();
				model_PubConfig.Title = vm_DictionaryAdd.Title;
				model_PubConfig.Key = vm_DictionaryAdd.Key;
				model_PubConfig.Status = vm_DictionaryAdd.Status;
				model_PubConfig.Description = vm_DictionaryAdd.Description;
				model_PubConfig.Isadmin = false;
				model_PubConfig.Dtime = DateTime.Now;
				if (vm_DictionaryAdd.IsCurrency)
				{
					model_PubConfig.Islang = 0;
					model_PubConfig.Val = vm_DictionaryAdd.Val;
					model_PubConfig.Longval = vm_DictionaryAdd.Longval;
					_context.Insertable(model_PubConfig).ExecuteCommand();
				}
				else
				{
					model_PubConfig.Islang = 1;
					var pubconfigid = _context.Insertable(model_PubConfig).ExecuteReturnBigIdentity();
					model_ConfigLang = new PubConfigLang();
					model_ConfigLang.PubConfigid = pubconfigid;
					model_ConfigLang.Val = vm_DictionaryAdd.Val;
					model_ConfigLang.Longval = vm_DictionaryAdd.Longval;
					model_ConfigLang.Lang = vm_DictionaryAdd.Lang;
					_context.Insertable(model_ConfigLang).ExecuteCommand();
				}
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["保存数据字段"] + "',msg:'" +_localizer["添加数据字典信息成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion

        #endregion

        #region "业务日志"

        #region "日志列表"    
        [Authorization(Power = "Main")]
        public async Task<IActionResult> Log()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> LogData(string keys = "", string status = "")
		{
			StringBuilder str = new StringBuilder();
			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string PubLogids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "PubLogids");
			PubLog model_PubLog = new PubLog();
			if (PubLogids.Trim() == "") PubLogids = "0";

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					var list_logid = PubLogids.Split(',').Select(u => long.Parse(u)).ToList();
					int count = _context.Updateable<PubLog>()
								.SetColumns(u => u.Status == -1)
								.Where(u => list_logid.Contains(u.PubLogid))
								.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\""+ _localizer["删除失败,不存在此记录"] + "\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					var exp = new Expressionable<PubLog>();
					exp.And(u => u.Status != -1);
					if (!string.IsNullOrEmpty(keys))
					{
						exp.And(u => u.Username.Contains(keys.Trim()) || u.Ip.Contains(keys.Trim()) || u.Content.Contains(keys.Trim()));
					}
					if (!string.IsNullOrEmpty(status))
					{
						exp.And(u => u.Status.ToString() == status.Trim());
					}
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
					sort += "pub_logid desc";
					var pageInfo = _context.Queryable<PubLog>().Where(exp.ToExpression()).OrderBy(sort).ToPageInfo(page, pagesize);

					str.Append("{\"total\":" + pageInfo.TotalCount + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < pageInfo.DataSource.Count; i++)
					{
						str.Append("{");
						str.Append("\"PubLogid\":\"" + pageInfo.DataSource[i].PubLogid + "\",");
						str.Append("\"Username\":\"" + pageInfo.DataSource[i].Username + "\",");
						str.Append("\"Content\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].Content) + "\",");
						str.Append("\"Url\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].Url) + "\",");
						str.Append("\"Ip\":\"" + pageInfo.DataSource[i].Ip + "\",");
						str.Append("\"Dtime\":\"" + pageInfo.DataSource[i].Dtime.ToString() + "\",");
						str.Append("\"Status\":\"" + pageInfo.DataSource[i].Status + "\",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (pageInfo.DataSource.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}
        #endregion

        #endregion

        #region "IP设置" 
        [Authorization(Power = "Main")]
        public async Task<IActionResult> IP()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			IP config = new IP();
			var list_config = _context.Queryable<PubConfig>().Where(u => u.Key.ToLower() == "whileip" || u.Key.ToLower() == "blackip").ToList();

			var model_configs = list_config.FirstOrDefault(u => u.Key.ToLower() == "whileip");
			if (model_configs != null) config.WhileIp = model_configs.Val;

			model_configs = list_config.FirstOrDefault(u => u.Key.ToLower() == "blackip");
			if (model_configs != null) config.BlackIp = model_configs.Val;

			return View(config);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> IP(IP vm_IP)
		{
			if (ModelState.IsValid)
			{
				PubConfig model_config;
				int count = 0;
				count = _context.Updateable<PubConfig>().SetColumns(u => u.Val == vm_IP.WhileIp).Where(u => (u.Key + "").ToLower() == "whileip").ExecuteCommand();
				if (count == 0)
				{
					model_config = new PubConfig();
					model_config.Key = "whileip";
					model_config.Val = vm_IP.WhileIp;
					model_config.Dtime = DateTime.Now;
					model_config.Isadmin = true;
					model_config.Status = 1;
					model_config.Islang = 0;
					await _context.Insertable(model_config).ExecuteCommandAsync();
				}

				count = _context.Updateable<PubConfig>().SetColumns(u => u.Val == vm_IP.BlackIp).Where(u => (u.Key + "").ToLower() == "blackip").ExecuteCommand();
				if (count == 0)
				{
					model_config = new PubConfig();
					model_config.Key = "blackip";
					model_config.Val = vm_IP.BlackIp;
					model_config.Dtime = DateTime.Now;
					model_config.Isadmin = true;
					model_config.Status = 1;
					model_config.Islang = 0;
					await _context.Insertable(model_config).ExecuteCommandAsync();
				}
				return Content("<script>alert('" + _localizer["修改IP设置成功"] + "');location.href='IP';</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			return View(vm_IP);
		}
        #endregion

        #region "平台角色权限" 

        #region "角色权限列表"    
        [Authorization(Power = "Main")]
        public IActionResult Role()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RoleData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();

			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			string AdminRoleids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "AdminRoleids");
			if (AdminRoleids.Trim() == "") AdminRoleids = "0";
			int set_status = 0, count = 0;
			string message = "";
			string _loginPrefix = Appsettings.app("Web:LoginPrefix");
			var list_roleid = AdminRoleids.Split(',').Select(u => long.Parse(u)).ToList();

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					set_status = -1;
					switch (set_status)
					{
						case 0:
							message = "禁用管理用户成功";
							break;
						case 1:
							message = "启用管理用户成功";
							break;
						case -1:
							message = "删除管理用户成功";
							break;
					}
					count = _context.Updateable<AdminRole>()
						.SetColumns(u => u.Status == set_status)
						.SetColumns(u => u.Remark == SqlFunc.MergeString($"[{DateTime.Now}]{_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminName")}({_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID")}){message}<hr />", u.Remark))
						.Where(u => u.Isadmin == 0 && list_roleid.Contains(u.AdminRoleid))
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
					set_status = 1;
					switch (set_status)
					{
						case 0:
							message = "禁用管理用户成功";
							break;
						case 1:
							message = "启用管理用户成功";
							break;
						case -1:
							message = "删除管理用户成功";
							break;
					}
					count = _context.Updateable<AdminRole>()
						.SetColumns(u => u.Status == set_status)
						.SetColumns(u => u.Remark == SqlFunc.MergeString($"[{DateTime.Now}]{_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminName")}({_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID")}){message}<hr />", u.Remark))
						.Where(u => u.Isadmin == 0 && list_roleid.Contains(u.AdminRoleid))
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
					set_status = 0;
					switch (set_status)
					{
						case 0:
							message = "禁用管理用户成功";
							break;
						case 1:
							message = "启用管理用户成功";
							break;
						case -1:
							message = "删除管理用户成功";
							break;
					}
					count = _context.Updateable<AdminRole>()
						.SetColumns(u => u.Status == set_status)
						.SetColumns(u => u.Remark == SqlFunc.MergeString($"[{DateTime.Now}]{_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminName")}({_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID")}){message}<hr />", u.Remark))
						.Where(u => u.Isadmin == 0 && list_roleid.Contains(u.AdminRoleid))
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
					var exp = new Expressionable<AdminRole, AdminRoleLang>();
					exp.And((l, r) => l.Status == 1 && r.Lang == Lang);
					if (!string.IsNullOrEmpty(keys))
					{
						exp.And((l, r) => r.Title.Contains(keys.Trim()));
					}
					if (!string.IsNullOrEmpty(status))
					{
						exp.And((l, r) => l.Status.ToString() == status.Trim());
					}
					if (!string.IsNullOrEmpty(begintime))
					{
						exp.And((l, r) => l.Dtime >= DateTime.Parse(begintime));
					}
					if (!string.IsNullOrEmpty(endtime))
					{
						exp.And((l, r) => l.Dtime <= DateTime.Parse(endtime));
					}
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
					sort += "Admin_Roleid desc";
					var pageInfo = _context.Queryable<AdminRole, AdminRoleLang>((l, r) => l.AdminRoleid == r.AdminRoleid)
						.Where(exp.ToExpression())
						.Select((l, r) => new AdminRoleVM()
						{
							AdminRoleid = l.AdminRoleid.SelectAll(),
							Title = r.Title,
							Lang = r.Lang
						}).MergeTable().OrderByIF(!string.IsNullOrEmpty(sort), sort).ToPageInfo(page, pagesize);
					str.Append("{\"total\":" + pageInfo.TotalCount + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < pageInfo.DataSource.Count; i++)
					{
						str.Append("{");
						str.Append("\"AdminRoleid\":\"" + pageInfo.DataSource[i].AdminRoleid + "\",");
						str.Append("\"Title\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].Title) + "\",");
						str.Append("\"Sort\":\"" + pageInfo.DataSource[i].Sort + "\",");
						str.Append("\"Isadmin\":\"" + pageInfo.DataSource[i].Isadmin + "\",");
						str.Append("\"Dtime\":\"" + pageInfo.DataSource[i].Dtime + "\",");
						str.Append("\"Status\":\"" + pageInfo.DataSource[i].Status + "\",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (pageInfo.DataSource.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}
        #endregion

        #region "角色权限添加修改"   
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RoleAdd(long AdminRoleid = 0)
		{
			StringBuilder str0 = new StringBuilder();
			StringBuilder str = new StringBuilder();

			string MenuTab = "", MenuTabPane = "";
			var list_Menus = _context.Queryable<AdminMenu>().Where(u => u.Status == 1).OrderBy(u => new { u.Sty, u.Sort }).OrderBy(u => u.AdminMenuid, OrderByType.Desc).ToList();
			var list_Lang = _context.Queryable<AdminMenuLang>().Where(u => list_Menus.Select(s => s.AdminMenuid).Contains(u.AdminMenuid) && u.Lang == Lang).ToList();
			var list_menu_functions = _context.Queryable<AdminMenuFunction>()
				.Where(l => l.Status == 1)
				.Select(l => new ViewAdminMenuFunction
				{
					AdminMenuFunctionid = l.AdminMenuFunctionid.SelectAll(),
					Title = SqlFunc.Subqueryable<AdminMenuFunctionLang>().Where(u => u.AdminMenuFunctionid == l.AdminMenuFunctionid).Select(u => u.Title),//&& u.Lang == Lang,控件权限没做跟随栏目同步创建多语言名称，所以暂时去掉
					Lang = Lang
				})
				.ToList();

			var list_admin_menus0 = list_Menus.Where(u => u.Fid == 0).ToList();
			for (int i = 0; i < list_admin_menus0.Count; i++)
			{
				var model_Lang = list_Lang.FirstOrDefault(u => u.AdminMenuid == list_admin_menus0[i].AdminMenuid);
				str0.Append("<li class=\"nav-item\">");
				str0.Append("    <a class=\"nav-link " + (i == 0 ? "active" : "") + "\" data-toggle=\"tab\" href=\"#tab_" + list_admin_menus0[i].AdminMenuid + "\" role=\"tab\">" + model_Lang?.Title + "(" + list_admin_menus0[i].AdminMenuid + ")</a>");
				str0.Append("</li>");

				var list_admin_menus1 = list_Menus.Where(u => u.Fid == list_admin_menus0[i].AdminMenuid).ToList();
				str.Append("<div class=\"tab-pane " + (i == 0 ? "active" : "") + " p-3\" id=\"tab_" + list_admin_menus0[i].AdminMenuid + "\" role=\"tabpanel\">");
				str.Append("	<div class=\"row\">");
				str.Append("<ul class=\"nav\">");
				str.Append("<li class=\"nav-item\">");
				str.Append("<a class=\"nav-link active\" href='javascript:void(0)' onclick=\"chkmenu(" + list_admin_menus0[i].AdminMenuid + ",true)\">"+_localizer["全选"] +"</a>");
				str.Append("</li>");
				str.Append("<li class=\"nav-item\">");
				str.Append("<a class=\"nav-link active\" href='javascript:void(0)' onclick=\"chkmenu_toggle(" + list_admin_menus0[i].AdminMenuid + ")\">" + _localizer["反选"] + "</a>");
				str.Append("</li>");
				str.Append("</ul>");
				str.Append("	</div>");
				str.Append("	<div class=\"row\">");
				str.Append("		<div class=\"col-sm-3\">");
				str.Append("			<div class=\"nav flex-column nav-pills text-center\" id=\"v-pills-tab-" + list_admin_menus0[i].AdminMenuid + "\" role=\"tablist\" aria-orientation=\"vertical\">");
				for (int j = 0; j < list_admin_menus1.Count; j++)
				{
					var model_Lang_1 = list_Lang.FirstOrDefault(u => u.AdminMenuid == list_admin_menus1[j].AdminMenuid);
					str.Append("				<a class=\"nav-link waves-effect waves-light " + (j == 0 ? "active" : "") + "\" id=\"v-pills-home-tab-" + list_admin_menus1[j].AdminMenuid + "\" data-toggle=\"pill\" href=\"#v-pills-home-" + list_admin_menus1[j].AdminMenuid + "\" role=\"tab\" aria-controls=\"v-pills-home\" aria-selected=\"true\">" + model_Lang_1?.Title + "(" + list_admin_menus1[j].AdminMenuid + ")</a>");
				}
				str.Append("			</div>");
				str.Append("		</div>");
				str.Append("		<div class=\"col-sm-9\">");
				//str.Append("<input type=\"checkbox\" class=\"\" onclick=\"javascript:chkmenu(" + list_admin_menus0[i].Rootid + ",$(this).prop('checked'))\" />全选");
				str.Append("			<div class=\"tab-content mo-mt-2\" id=\"v-pills-tabContent\">");
				for (int j = 0; j < list_admin_menus1.Count; j++)
				{
					//本二级ID下的所有子菜单ID
					//var menuids = model_admin_menus.Where(u => u.Depth > 1 && ("|" + u.Path + "|").Contains("|" + list_admin_menus1[j].AdminMenuid + "|")).Select(u => u.AdminMenuid).ToList();
					var menuids = list_Menus.Where(u => u.Depth > 1 && ("|"+u.Path+"|").Contains("|"+list_admin_menus1[j].AdminMenuid+"|")).Select(u => u.AdminMenuid).ToList();
					str.Append("				<div class=\"tab-pane fade " + (j == 0 ? "active" : "") + " show\" id=\"v-pills-home-" + list_admin_menus1[j].AdminMenuid + "\" role=\"tabpanel\" aria-labelledby=\"v-pills-home-tab\">");
					str.Append("						" + Role_GetMenuFunction(list_menu_functions, list_Menus, list_Lang, menuids, list_admin_menus1[j].AdminMenuid, list_admin_menus0[i].Rootid) + "");
					str.Append("				</div>");
				}
				str.Append("			</div>");
				str.Append("		</div>");
				str.Append("	</div>");
				str.Append("</div>");
			}
			MenuTab = str0.ToString();
			MenuTabPane = str.ToString();

			RoleAdd vm_RoleAdd = new RoleAdd();
			vm_RoleAdd.Lang = Lang;
			var model_AdminRole = _context.Queryable<AdminRole>().First(u => u.AdminRoleid == AdminRoleid);
			vm_RoleAdd.MenuTabPane = MenuTabPane;
			vm_RoleAdd.MenuTab = MenuTab;
			if (model_AdminRole != null)
			{
				vm_RoleAdd.AdminRoleid = AdminRoleid;
				vm_RoleAdd.Title = _context.Queryable<AdminRoleLang>().Where(u => u.Lang == Lang && u.AdminRoleid == AdminRoleid).First()?.Title;
				vm_RoleAdd.Sort = model_AdminRole.Sort;
				vm_RoleAdd.Status = model_AdminRole.Status;
			}
			else
			{
				vm_RoleAdd.Sort = 100;
				vm_RoleAdd.Status = 1;
			}
			string script_str = "";
			var list_admin_role_menu_function = _context.Queryable<AdminRoleMenuFunction>().Where(u => u.Status != -1 && u.AdminRoleid == AdminRoleid).ToList();
			for (int i = 0; i < list_admin_role_menu_function.Count; i++)
			{
				script_str += "$('#input_AdminMenuFunctionid_" + list_admin_role_menu_function[i].AdminMenuFunctionid + "').prop('checked',true);";
			}
			ViewBag.ScriptStr = _tokenManager.ScriptStr + script_str;
			return View(vm_RoleAdd);
		}
		//保存添改
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> RoleAdd(RoleAdd vm_RoleAdd)
		{
			string _loginPrefix = Appsettings.app("Web:LoginPrefix");
			StringBuilder str = new StringBuilder();
			string AdminMenuFunctionids = GSRequestHelper.GetFormString(_accessor.HttpContext.Request, "input_AdminMenuFunctionid");
			var list_menu_functionid = AdminMenuFunctionids.Split(',').Where(u => !string.IsNullOrEmpty(u)).Select(u => long.Parse(u)).ToList();
			AdminRoleLang model_RoleLang = null;
			var model_AdminRole = _context.Queryable<AdminRole>().First(u => u.AdminRoleid == vm_RoleAdd.AdminRoleid);
			if (model_AdminRole != null)
			{
				var count = _context.Updateable<AdminRoleLang>().SetColumns(u => u.Title == vm_RoleAdd.Title).Where(u => u.Lang == vm_RoleAdd.Lang && u.AdminRoleid == model_AdminRole.AdminRoleid).ExecuteCommand();
				if (count == 0)
				{
					_context.Insertable(new AdminRoleLang()
					{
						AdminRoleid = vm_RoleAdd.AdminRoleid,
						Title = vm_RoleAdd.Title,
						Lang = vm_RoleAdd.Lang
					})
					.ExecuteCommand();

				}
				model_AdminRole.Sort = vm_RoleAdd.Sort;
				model_AdminRole.Status = vm_RoleAdd.Status;

				if (UpdateRoleFunction(model_AdminRole.AdminRoleid, list_menu_functionid))
				{
					model_AdminRole.Remark = $"[{DateTime.Now}]{_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminName")}({_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID")})修改权限内容为[{AdminMenuFunctionids}]<hr/>" + model_AdminRole.Remark;
				}
				_context.Updateable(model_AdminRole).UpdateColumns(u => new { u.Sort, u.Status, u.Remark }).ExecuteCommand();
				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改角色权限"] + "',msg:'" + _localizer["保存成功"] + "',timeout:3000,showType:'slide'});location.href='?AdminRoleid="+vm_RoleAdd.AdminRoleid + "&lang=" + vm_RoleAdd.Lang + "'; </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				model_AdminRole = new AdminRole();
				model_AdminRole.Isagent = 0;
				model_AdminRole.Sort = vm_RoleAdd.Sort;
				model_AdminRole.Status = vm_RoleAdd.Status;
				model_AdminRole.Dtime = DateTime.Now;
				vm_RoleAdd.AdminRoleid = _context.Insertable(model_AdminRole).ExecuteReturnBigIdentity();

				model_RoleLang = new AdminRoleLang();
				model_RoleLang.AdminRoleid = vm_RoleAdd.AdminRoleid;
				model_RoleLang.Title = vm_RoleAdd.Title;
				model_RoleLang.Lang = vm_RoleAdd.Lang;
				_context.Insertable(model_RoleLang).ExecuteCommand();

				UpdateRoleFunction(model_AdminRole.AdminRoleid, list_menu_functionid);
				return Content("<script>parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加角色权限"] + "',msg:'" + _localizer["保存成功"] + "',timeout:3000,showType:'slide'}); location.href='?AdminRoleid=" + vm_RoleAdd.AdminRoleid + "&lang=" + vm_RoleAdd.Lang + "';</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		/// <summary>
		/// 重新更新用户角色关系表
		/// </summary>
		/// <param name="AdminRoleid"></param>
		/// <param name="list_menu_functionid"></param>
		/// <returns>返回是否变化</returns>
		private bool UpdateRoleFunction(long AdminRoleid, List<long> list_menu_functionid)
		{
			var list_RoleMenuFunction = _context.Queryable<AdminRoleMenuFunction>().Where(u => u.Status != -1 && u.AdminRoleid == AdminRoleid).ToList();
			var old_list = list_RoleMenuFunction.Select(u => u.AdminMenuFunctionid).ToList();
			var new_list = list_menu_functionid;
			old_list.Sort();
			new_list.Sort();


			for (int i = 0; i < list_RoleMenuFunction.Count; i++)
			{
				//如果旧角色组中的角色在新的角色中不存在，则删除DB记录；否则跳过，并把新角色ID集中的这个剔除（因为不剔除的一会要ADD）
				if (!list_menu_functionid.Contains(list_RoleMenuFunction[i].AdminMenuFunctionid))
				{
					_context.Updateable<AdminRoleMenuFunction>().SetColumns(u => u.Status == -1)
						.Where(u => u.AdminMenuFunctionid == list_RoleMenuFunction[i].AdminMenuFunctionid && u.AdminRoleid== AdminRoleid).ExecuteCommand();
				}
				else
				{
					//已经存在，剔除不再新增
					list_menu_functionid.Remove(list_RoleMenuFunction[i].AdminMenuFunctionid);
				}
			}

			var list_adminRoleMenuFunction = new List<AdminRoleMenuFunction>();
			AdminRoleMenuFunction adminRoleMenuFunction;
			var list_admin_role_menu_functions = _context.Queryable<AdminRoleMenuFunction>().Where(u => u.Status != -1 && u.AdminRoleid == AdminRoleid).ToList();
			var list_menu_functions = _context.Queryable<AdminMenuFunction>().Where(u => list_menu_functionid.Contains(u.AdminMenuFunctionid)).ToList();
			for (int i = 0; i < list_menu_functions.Count; i++)
			{
				if (!list_admin_role_menu_functions.Exists(u => u.AdminRoleid == AdminRoleid && u.AdminMenuid == list_menu_functions[i].AdminMenuid && u.AdminMenuFunctionid == list_menu_functions[i].AdminMenuFunctionid))
				{
					adminRoleMenuFunction = new AdminRoleMenuFunction();
					adminRoleMenuFunction.AdminMenuid = list_menu_functions[i].AdminMenuid;
					adminRoleMenuFunction.AdminRoleid = AdminRoleid;
					adminRoleMenuFunction.AdminMenuFunctionid = list_menu_functions[i].AdminMenuFunctionid;
					adminRoleMenuFunction.Status = 1;
					list_adminRoleMenuFunction.Add(adminRoleMenuFunction);
				}
			}
			if (list_adminRoleMenuFunction.Count > 0) _context.Insertable(list_adminRoleMenuFunction).ExecuteCommand();

			return !Enumerable.SequenceEqual(new_list, old_list);
		}
		private string Role_GetMenuFunction(List<ViewAdminMenuFunction> model_admin_menu_functions, List<AdminMenu> model_admin_menu,List<AdminMenuLang> list_Lang, List<long> menuids, long AdminMenuid = 0, long Rootid = 0)
		{
			if (model_admin_menu_functions == null) return null;
			StringBuilder str = new StringBuilder();
			var list_admin_menu = model_admin_menu.Where(u => menuids.Contains(u.AdminMenuid)).OrderBy(u => new { u.Depth, u.Rootid, u.Sort }).OrderByDescending(u => u.AdminMenuid).ToList();
			str.Append("<table width=\"100%\">");
			str.Append("<tr><td  colspan=\"2\">");
			str.Append("<input type=\"checkbox\" class=\"\" onclick=\"javascript:chkfid(" + AdminMenuid + ",$(this).prop('checked'))\" />全选");
			str.Append("</td></tr>");
			for (int i = 0; i < list_admin_menu.Count; i++)
			{
				var model_Lang = list_Lang.FirstOrDefault(u => u.AdminMenuid == list_admin_menu[i].AdminMenuid);
				if (AdminMenuid != list_admin_menu[i].AdminMenuid)
				{
					str.Append("<tr><td  colspan=\"2\" style=\"padding:10px 10px 10px 10px;background-color:#eef8ed\">&nbsp;[" + (i + 1).ToString("00") + "]&nbsp;" + model_Lang?.Title + "</td></tr>\r");
					str.Append("<tr><td class=\"firsttd\" valign=\"top\" ></td><td>");
					str.Append("<table width=\"100%\">\r");
				}
				var list_admin_menu_functions = model_admin_menu_functions.Where(u => u.AdminMenuid == list_admin_menu[i].AdminMenuid).OrderBy(u => new { u.Depth, u.Path }).OrderByDescending(u => u.AdminMenuFunctionid).ToList();
				for (int j = 0; j < list_admin_menu_functions.Count; j++)
				{
					str.Append("<tr style=\"border-bottom: 1px solid #D4D4D4;\">\r");
					str.Append("<td valign=\"top\" style=\"padding-top:5px;padding-left:" + (list_admin_menu_functions[j].Depth * 30) + "px;\"><input type=\"checkbox\" class=\"fid_" + AdminMenuid + " function_fid_" + list_admin_menu_functions[j].AdminMenuFunctionid + " menuid_" + Rootid + "\" id=\"input_AdminMenuFunctionid_" + list_admin_menu_functions[j].AdminMenuFunctionid + "\" name=\"input_AdminMenuFunctionid\" value=\"" + list_admin_menu_functions[j].AdminMenuFunctionid + "\" onclick=\"javascript:chk(" + list_admin_menu_functions[j].AdminMenuFunctionid + ",$(this).prop('checked'))\" />&nbsp;[" + list_admin_menu_functions[j].AdminMenuFunctionid + "]&nbsp;" + list_admin_menu_functions[j].Title + (list_admin_menu_functions[j].Ispage ? "(页)：" : "：") + "<br />");
					str.Append("<span>" + list_admin_menu_functions[j].Intro + "</span></td>");
					str.Append("</tr>\r");
				}
				if (AdminMenuid != list_admin_menu[i].AdminMenuid)
				{
					str.Append("</table><br />");

					str.Append("</td></tr>");
				}
			}
			str.Append("</table>");

			return str.ToString();
		}

        #endregion

        #endregion

        #region "平台管理员账号" 
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        [Authorization(Power = "Main")]
        public async Task<IActionResult> Master()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			return View();
		}

		/// <summary>
		/// 列表页数据
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="status"></param>
		/// <param name="begintime"></param>
		/// <param name="endtime"></param>
		/// <returns></returns>
		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> MasterData(string keys = "", string status = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();

			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			long AdminMasterid = GSRequestHelper.GetLong(_accessor.HttpContext.Request, "AdminMasterid", 0);
			string AdminMasterids = GSRequestHelper.GetString(_accessor.HttpContext.Request, "AdminMasterids");
			if (AdminMasterids.Trim() == "") AdminMasterids = "0";
			var list_masterid = AdminMasterids.Split(',').Select(u => long.Parse(u)).ToList();
			list_masterid.Add(AdminMasterid);
			var list_admins = _config["Web:Admins"].ToLower().Split(',').Where(u=>!string.IsNullOrEmpty(u)).ToList();

			string msg = "";
			string _loginPrefix = Appsettings.app("Web:LoginPrefix");
			int set_status = 0, count=0;

			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				case "del":
					set_status = -1;
					switch (set_status)
					{
						case 0:
							msg = "禁用管理用户成功";
							break;
						case 1:
							msg = "启用管理用户成功";
							break;
						case -1:
							msg = "删除管理用户成功";
							break;
					}
					var message = $"[{DateTime.Now}]{_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminName")}({_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID")}){msg}<hr />";

                    count = _context.Updateable<AdminMaster>()
						.SetColumns(u => u.Status == set_status)
						.SetColumns(u => u.Remark == SqlFunc.MergeString(message, u.Remark))
						.Where(u => !list_admins.Contains(u.Username) && list_masterid.Contains(u.AdminMasterid))
						.ExecuteCommand();
					if (count>0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["删除管理员成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["删除管理员失败,无权限或不存在此记录"] + "！\"}");
					}
					break;
				case "enableu"://启用
					set_status = 1;
					switch (set_status)
					{
						case 0:
							msg = "禁用管理用户成功";
							break;
						case 1:
							msg = "启用管理用户成功";
							break;
						case -1:
							msg = "删除管理用户成功";
							break;
					}
					count = _context.Updateable<AdminMaster>()
						.SetColumns(u => u.Status == set_status)
						.SetColumns(u => u.Remark == SqlFunc.MergeString($"[{DateTime.Now}]{_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminName")}({_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID")}){msg}<hr />", u.Remark))
						.Where(u => !list_admins.Contains(u.Username) && list_masterid.Contains(u.AdminMasterid))
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["启用管理员成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["启用管理员失败,无权限或不存在此记录"] + "！\"}");
					}
					break;
				case "enablef"://禁用
					set_status = 0;
					switch (set_status)
					{
						case 0:
							msg = "删除管理用户成功";
							break;
						case 1:
							msg = "启用管理用户成功";
							break;
						case -1:
							msg = "禁用管理用户成功";
							break;
					}
					count = _context.Updateable<AdminMaster>()
						.SetColumns(u => u.Status == set_status)
						.SetColumns(u => u.Remark == SqlFunc.MergeString($"[{DateTime.Now}]{_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminName")}({_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID")}){msg}<hr />", u.Remark))
						.Where(u => !list_admins.Contains(u.Username) && list_masterid.Contains(u.AdminMasterid))
						.ExecuteCommand();
					if (count > 0)
					{
						str.Append("{\"errorcode\":\"0\",\"msg\":\"" + _localizer["禁用管理员成功"] + "！\"}");
					}
					else
					{
						str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["禁用管理员失败,无权限或不存在此记录"] + "！\"}");
					}
					break;
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					string Admins = Appsettings.app("Web:Admins");
					var exp = new Expressionable<AdminMaster>();
					exp.And(u => !list_admins.Contains(u.Username) && u.Status != -1);

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
					sort += "admin_masterid desc";
					var page_Master = _context.Queryable<AdminMaster>().Where(exp.ToExpression()).OrderBy(sort).ToPageInfo(page, pagesize);
					string role_name = "";
					var pageInfo = new PagedInfo<MasterData>();
					if (page_Master.DataSource.Count > 0)
					{
						var list_Masterid = page_Master.DataSource.Select(u => u.AdminMasterid).ToList();
						var list_MasterRole = _context.Queryable<AdminMasterRole>().Where(u => list_Masterid.Contains(u.AdminMasterid)).ToList();
						var list_Role = _context.Queryable<AdminRole, AdminRoleLang>((l, r) => l.AdminRoleid == r.AdminRoleid)
							.Where((l, r) => l.Status == 1 && list_MasterRole.Select(s => s.AdminRoleid).Contains(l.AdminRoleid))
							.Select((l, r) => new AdminRoleVM()
							{
								AdminRoleid = r.AdminRoleid.SelectAll(),
								Title = r.Title,
								Lang = r.Lang
							})
							.ToList();
						if (pageInfo.DataSource == null) pageInfo.DataSource = new List<MasterData>();
						foreach (var model in page_Master.DataSource)
						{
							role_name = "";
							if (("," + Admins + ",").Contains("," + (model.Username.ToLower() + "").Trim() + ","))
							{
								role_name = "超级管理员";
							}
							else
							{
								var model_admin_masterRole = list_MasterRole.Where(u => u.AdminMasterid == model.AdminMasterid);
								var list_AdminRole = list_Role.Where(u => model_admin_masterRole.Select(m => m.AdminRoleid).Contains(u.AdminRoleid) && u.Lang == Lang).ToList();
								for (int j = 0; j < list_AdminRole.Count; j++)
								{
									if (role_name != "") role_name += "<br>";
									role_name += list_AdminRole[j].Title;
								}
							}
							//pageInfo.DataSource.Add(new MasterData()
							//{
							//	AdminMasterid = model.AdminMasterid.SelectAll(),
							//	RoleName = role_name
							//});
							pageInfo.DataSource.Add(new MasterData()
							{
								AdminMasterid = model.AdminMasterid,
								Username = model.Username,
								Userpwd = model.Userpwd,
								Nums = model.Nums,
								Ip = model.Ip,
								Lasttime = model.Lasttime,
								AdminRoleids = model.AdminRoleids,
								AdminMenuids = model.AdminMenuids,
								AdminMenuFunctionids = model.AdminMenuFunctionids,
								Name = model.Name,
								Dtime = model.Dtime,
								Email = model.Email,
								Mobile = model.Mobile,
								Photo = model.Photo,
								SmsCode = model.SmsCode,
								SmsTime = model.SmsTime,
								Loginnum = model.Loginnum,
								Status = model.Status,
								Remark = model.Remark,
								Isadmin = model.Isadmin,
								Token = model.Token,
								Onlyone = model.Onlyone,
								LastLang = model.LastLang,
								RoleName = role_name
							});
						}
					}
					pageInfo.Page = page;
					pageInfo.PageSize = pagesize;
					pageInfo.TotalCount = page_Master.TotalCount;
					str.Append("{\"total\":" + pageInfo.TotalCount + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < pageInfo.TotalCount; i++)
					{
						str.Append("{");
						str.Append("\"AdminMasterid\":\"" + pageInfo.DataSource[i].AdminMasterid + "\",");
						str.Append("\"Username\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].Username) + "\",");
						str.Append("\"RoleName\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].RoleName + "") + "\",");
						str.Append("\"Name\":\"" + JsonHelper.JsonCharFilter(pageInfo.DataSource[i].Name) + "\",");
						str.Append("\"Ip\":\"" + pageInfo.DataSource[i].Ip + "" + "\",");
						str.Append("\"Lasttime\":\"" + pageInfo.DataSource[i].Lasttime + "" + "\",");
						str.Append("\"Loginnum\":\"" + pageInfo.DataSource[i].Loginnum + "\",");
						str.Append("\"Status\":\"" + pageInfo.DataSource[i].Status + "\",\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (pageInfo.DataSource.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}
			return Content(str.ToString());
		}

        /// <summary>
        /// 添改管理员
        /// </summary>
        /// <param name="AdminMasterid"></param>
        /// <returns></returns>
        [Authorization(Power = "Main")]
        public async Task<IActionResult> MasterAdd(long AdminMasterid = 0)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr; MasterAdd vm_MasterAdd = new MasterAdd();

			List<long> list_roleid = new List<long>();
			list_roleid.Add(0);
			var list_admin_masterRole = _context.Queryable<AdminMasterRole>().Where(u => u.AdminMasterid == AdminMasterid).ToList();
			for (int i = 0; i < list_admin_masterRole.Count; i++)
			{
				list_roleid.Add(list_admin_masterRole[i].AdminRoleid);//取该用户的权限ID集
			}
			List<SelectListItem> Roleids = new List<SelectListItem>();
			var list_AdminRole = _context.Queryable<AdminRole>()
				.Where(l => l.Status == 1)
				.OrderBy(l => l.Sort)
				.OrderBy(l => l.AdminRoleid, OrderByType.Desc)
				.Select(l => new AdminRoleVM
				{
					AdminRoleid = l.AdminRoleid,
					Title = SqlFunc.Subqueryable<AdminRoleLang>().Where(u => u.AdminRoleid == l.AdminRoleid && u.Lang == Lang).Select(u => u.Title)
				})
				.ToList();
			var model_admin_master = _context.Queryable<AdminMaster>().InSingle(AdminMasterid);
			if (model_admin_master != null)
			{
				vm_MasterAdd.AdminMasterid = AdminMasterid;
				vm_MasterAdd.Username = model_admin_master.Username;
				vm_MasterAdd.Name = model_admin_master.Name;
				vm_MasterAdd.Mobile = model_admin_master.Mobile;
				vm_MasterAdd.Email = model_admin_master.Email;
				vm_MasterAdd.Status = model_admin_master.Status;
				vm_MasterAdd.IsAdmin = ("," + Appsettings.app("Web:Admins") + ",").Contains("," + model_admin_master.Username.ToLower().Trim() + ",");
				vm_MasterAdd.AdminRoleids = string.Join(",", list_AdminRole.Where(u => list_roleid.Contains(u.AdminRoleid)).ToList().Select(u => u.AdminRoleid));
			}
			for (int i = 0; i < list_AdminRole.Count; i++)
			{
				Roleids.Add(new SelectListItem(list_AdminRole[i].Title, list_AdminRole[i].AdminRoleid + ""));
			}
			vm_MasterAdd.Roleids = Roleids;
			vm_MasterAdd.Lang = Lang;
			return View(vm_MasterAdd);
		}

		//保存添改的管理员信息
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> MasterAdd(MasterAdd vm_MasterAdd)
		{
			//vm_MasterAdd.AdminRoleids; //需测试下，能否通过vm带过来，还是必需request取值，如VM能带过来，则_adminMasterService.MasterAdd中的参数需要调整下
			var api = new ApiResult();
			string AdminRoleids = GSRequestHelper.GetFormString(_accessor.HttpContext.Request, "AdminRoleids");
			string _loginPrefix = Appsettings.app("Web:LoginPrefix");
			var model_admin_master = _context.Queryable<AdminMaster>().InSingle(vm_MasterAdd.AdminMasterid);
			if (model_admin_master != null)
			{
				if (_context.Queryable<AdminMaster>().Any(u => u.Username == vm_MasterAdd.Username && u.AdminMasterid != vm_MasterAdd.AdminMasterid))
				{
					return Content("<script>alert('" + _localizer["不允许添加相同的用户账号"] + "');history.go(-1);</script>", "text/html", UTF8Encoding.UTF8);
				}
				model_admin_master.Username = vm_MasterAdd.Username;
				if (!string.IsNullOrEmpty(vm_MasterAdd.Userpwd)) model_admin_master.Userpwd = MD5Helper.MD5Encrypt32(vm_MasterAdd.Userpwd); ;
				model_admin_master.Name = vm_MasterAdd.Name;
				model_admin_master.Mobile = vm_MasterAdd.Mobile;
				model_admin_master.Email = vm_MasterAdd.Email;
				model_admin_master.Status = vm_MasterAdd.Status;
				if (model_admin_master.AdminRoleids != vm_MasterAdd.AdminRoleids)
				{
					model_admin_master.AdminRoleids = vm_MasterAdd.AdminRoleids;
					model_admin_master.Remark = $"[{DateTime.Now}]{_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminName")}({_accessor.HttpContext.Session.GetString(_loginPrefix + "AdminID")})改角色组由[{model_admin_master.AdminRoleids}]变为[{vm_MasterAdd.AdminRoleids}]" + model_admin_master.Remark;
				}
				_context.Updateable(model_admin_master).UpdateColumns(u => new {
					u.Username,
					u.Userpwd,
					u.Name,
					u.Mobile,
					u.Email,
					u.Status,
					u.Remark,
					u.AdminRoleids
				}).ExecuteCommand();
				update_master_role(model_admin_master.AdminMasterid, AdminRoleids);
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["修改管理员信息成功"] + "',msg:'" + _localizer["保存成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				if (_context.Queryable<AdminMaster>().Any(u => u.Username == vm_MasterAdd.Username))
				{
					return Content("<script>alert('" + _localizer["不允许添加相同的用户账号"] + "');history.go(-1);</script>", "text/html", UTF8Encoding.UTF8);
				}
				model_admin_master = new AdminMaster();
				model_admin_master.Username = vm_MasterAdd.Username;
				model_admin_master.Userpwd = MD5Helper.MD5Encrypt32(vm_MasterAdd.Userpwd); ;
				model_admin_master.Name = vm_MasterAdd.Name;
				model_admin_master.Mobile = vm_MasterAdd.Mobile;
				model_admin_master.Email = vm_MasterAdd.Email;
				model_admin_master.AdminRoleids = vm_MasterAdd.AdminRoleids;
				model_admin_master.Status = vm_MasterAdd.Status;
				model_admin_master.Dtime = DateTime.Now;
				vm_MasterAdd.AdminMasterid = _context.Insertable(model_admin_master).ExecuteReturnIdentity();
				update_master_role(vm_MasterAdd.AdminMasterid, AdminRoleids);
				return Content("<script>parent.$('#win').window('close');parent.$('#table_view').datagrid('reload');parent.$.messager.show({title:'" + _localizer["添加管理员信息成功"] + "',msg:'" + _localizer["保存成功"] + "',timeout:3000,showType:'slide'}); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
		//更新管理角色关系表
		private void update_master_role(long admin_masterid, string admin_roleids)
		{
			var list_roleid = admin_roleids.Split(',').Where(u => !string.IsNullOrEmpty(u)).Select(u => long.Parse(u)).ToList();
			//删除掉已有的，但并且不在admin_roleids中的
			_context.Deleteable<AdminMasterRole>().Where(u => u.AdminMasterid == admin_masterid && !list_roleid.Contains(u.AdminRoleid)).ExecuteCommand();

			var list_sub_roleid = _context.Queryable<AdminMasterRole>().Where(u => u.AdminMasterid == admin_masterid && list_roleid.Contains(u.AdminRoleid)).Select(u => u.AdminRoleid).ToList();//已存在的要过滤掉的ID
			list_roleid = list_roleid.Except(list_sub_roleid).ToList();//取差值，过滤掉已存在的

			var list_Add = new List<AdminMasterRole>();
			for (int i = 0; i < list_roleid.Count(); i++)
			{
				if (list_roleid[i] > 0)
				{
					AdminMasterRole model_admin_masterRole = new AdminMasterRole();
					model_admin_masterRole.AdminMasterid = admin_masterid;
					model_admin_masterRole.AdminRoleid = list_roleid[i];
					list_Add.Add(model_admin_masterRole);
				}
			}
			_context.Insertable(list_Add).ExecuteCommand();
		}
        #endregion

        #region "修改密码" 
        [Authorization(Power = "Main")]
        public async Task<IActionResult> Pwd()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			MasterAdd vm_MasterAdd = new MasterAdd();
			string AdminID = _accessor.HttpContext.Session.GetString(Appsettings.app("Web:LoginPrefix") + "AdminID");
			long MasterID = string.IsNullOrEmpty(AdminID) ? 0 : long.Parse(AdminID);
			var model_admin_master = _context.Queryable<AdminMaster>().InSingle(MasterID);
			if (model_admin_master != null)
			{
				vm_MasterAdd.Username = model_admin_master.Username;
				vm_MasterAdd.Name = model_admin_master.Name;
				vm_MasterAdd.Mobile = model_admin_master.Mobile;
				vm_MasterAdd.Email = model_admin_master.Email;
			}
			vm_MasterAdd.AdminMasterid = MasterID;
			return View(vm_MasterAdd);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> Pwd(MasterAdd vm_MasterAdd)
		{
			var model_admin_master = _context.Queryable<AdminMaster>().InSingle(vm_MasterAdd.AdminMasterid);
			if (model_admin_master != null)
			{
				if (string.IsNullOrEmpty(vm_MasterAdd.Userpwd) || (model_admin_master.Userpwd.ToLower().Trim() != MD5Helper.MD5Encrypt32(vm_MasterAdd.Userpwd).ToLower().Trim() && model_admin_master.Userpwd.ToLower().Trim() != MD5Helper.MD5Encrypt32(vm_MasterAdd.Userpwd).ToLower().Trim().Substring(8, 16)))
				{
					return Content("<script>alert('" + _localizer["您输入的原密码不正确"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				if (!string.IsNullOrEmpty(vm_MasterAdd.Userpwd1) && vm_MasterAdd.Userpwd1 != vm_MasterAdd.Userpwd2)
				{
					return Content("<script>alert('" + _localizer["您两次输入的新密码不相同"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
				}

				if (!string.IsNullOrEmpty(vm_MasterAdd.Userpwd1)) model_admin_master.Userpwd = MD5Helper.MD5Encrypt32(vm_MasterAdd.Userpwd1);
				model_admin_master.Name = vm_MasterAdd.Name;
				model_admin_master.Mobile = vm_MasterAdd.Mobile;
				model_admin_master.Email = vm_MasterAdd.Email;
				model_admin_master.AdminRoleids = vm_MasterAdd.AdminRoleids;

				_context.Updateable(model_admin_master).UpdateColumns(u => new { u.Userpwd, u.Name, u.Mobile, u.Email, u.AdminRoleids }).ExecuteCommand();
				return Content("<script>alert('" + _localizer["修改个人信息成功"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			else
			{
				return Content("<script>alert('" + _localizer["修改个人信息失败"] + "');history.go(-1); </script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
		}
        #endregion

        #region "接口日志"    
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ApiLog()
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			ViewBag.Begintime = DateTime.Now.AddDays(-1).ToString();
			return View();
		}

		[HttpGet]
		[HttpPost]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> ApiLogData(string keys = "", string key_url = "",string ip = "", string limit = "", string key_request = "", string key_response = "", string begintime = "", string endtime = "")
		{
			StringBuilder str = new StringBuilder();

			int page = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "page", 1);
			int pagesize = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "rows", 20);
			
			switch (GSRequestHelper.GetString(_accessor.HttpContext.Request, "action"))
			{
				default:
					string sort = GSRequestHelper.GetString(_accessor.HttpContext.Request, "sort").Trim();
					string order = GSRequestHelper.GetString(_accessor.HttpContext.Request, "order").Trim();
					var exp = new Expressionable<Logs>();
					if (!string.IsNullOrEmpty(begintime))
					{
						exp.And(u => u.Dtime >= DateTime.Parse(begintime));
					}
					else
					{
						begintime = DateTime.Now.AddDays(-2).ToString();
						exp.And(u => u.Dtime >= DateTime.Parse(begintime));
					}
					if (!string.IsNullOrEmpty(endtime))
					{
						exp.And(u => u.Dtime <= DateTime.Parse(endtime));
					}

					if (!string.IsNullOrEmpty(key_url))
					{
						exp.And(u => u.Url.Contains(key_url.Trim()));
					}
					if (!string.IsNullOrEmpty(key_request))
					{
						exp.And(u => u.Requestbody.Contains(key_request.Trim()));
					}
					if (!string.IsNullOrEmpty(key_response))
					{
						exp.And(u => u.Responsebody.Contains(key_response.Trim()));
					}
					if (!string.IsNullOrEmpty(ip))
					{
						exp.And(u => u.Ip.Contains(ip.Trim()));
					}
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
					sort += "excutestarttime desc";
					PageParm parm = new PageParm();
					parm.Sort = sort;
					parm.PageIndex = page;
					parm.PageSize = pagesize;
					var pageInfo = new List<Logs>();
					int total = 0;
                    if (!string.IsNullOrEmpty(limit))
					{
						var count = int.Parse(limit);
                        if (count > 1000)
                        {
                            str.Append("{\"errorcode\":\"-1\",\"msg\":\"" + _localizer["Limit值不得超出1000"] + "\"}");
                            return Content(str.ToString());
                        }
						total = count;
                        pageInfo = _context.Queryable<Logs>().Where(exp.ToExpression()).OrderBy(sort).Take(count).ToList();
                    }
					else {
                        pageInfo = _context.Queryable<Logs>().Where(exp.ToExpression()).OrderBy(sort).ToPageList(page, pagesize, ref total);
                    }
                    
					
					str.Append("{\"total\":" + total + ",\"success\":true,\"rows\":[");
					for (int i = 0; i < pageInfo.Count; i++)
					{
						str.Append("{");
						str.Append("\"Logsid\":\"" + pageInfo[i].Logsid + "\",");
						str.Append("\"Url\":\"" + pageInfo[i].Url + "\",");
						str.Append("\"Method\":\"" + pageInfo[i].Method + "\",");
						str.Append("\"Responsebody\":\""+ JsonHelper.JsonCharFilter(pageInfo[i].Responsebody) + "\",");
						str.Append("\"Requestbody\":\"" + JsonHelper.JsonCharFilter(pageInfo[i].Requestbody) + "\",");
						str.Append("\"Headers\":\"" + JsonHelper.JsonCharFilter(pageInfo[i].Headers) + "\",");
						str.Append("\"Ip\":\"" + pageInfo[i].Ip + "\",");
						str.Append("\"Excutestarttime\":\"" + pageInfo[i].Excutestarttime + "\",");
						str.Append("\"Excuteendtime\":\"" + pageInfo[i].Excuteendtime + "\",");
						str.Append("\"Dtime\":\"" + pageInfo[i].Dtime.ToString() + "\",");
						str.Append("\"operation\":\"" + _localizer["查看/修改"] + "\"}");
						if (i < (pageInfo.Count - 1)) str.Append(",");
					}
					str.Append("]}");
					break;
			}

			return Content(str.ToString());
		}

		[HttpGet]
        [Authorization(Power = "Main")]
        public async Task<IActionResult> GetLogData(long Logsid=0,int type=0)
		{
			string data = "";
			var model_Logs = _context.Queryable<Logs>().InSingle(Logsid);
			if (model_Logs != null)
			{
				if (type == 0)
				{
					if (string.IsNullOrEmpty(model_Logs.Requestbody))
					{
						data = _localizer["提交值过长，暂存至文本日志中"];
					}
					else
					{
						data = model_Logs.Requestbody;
					}
				}
				else
				{
					if (string.IsNullOrEmpty(model_Logs.Responsebody))
					{
						data = _localizer["返回值过长，暂存至文本日志中"];
					}
					else
					{
						data = model_Logs.Responsebody;
					}
				}
			}
			else
			{
				data = "Logsid=" + Logsid + ","+ _localizer["无此日志"];
			}
			return Content(data);
		}
		#endregion

	}
}