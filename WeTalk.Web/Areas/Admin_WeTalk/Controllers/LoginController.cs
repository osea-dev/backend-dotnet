using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Interfaces;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Web.Extensions;
using WeTalk.Web.ViewModels;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class LoginController : Controller
	{
		/// <summary>
		/// 会话管理接口
		/// </summary>
		private readonly TokenManager _tokenManager;
		private readonly IPubConfigService _pubConfigService;
		private readonly ILanguageService _languageService;

		private readonly HttpContext _httpContext;
		private readonly SqlSugarScope _context;
		private readonly IWebHostEnvironment _env;

		private readonly string _isSms = "";
		private readonly string _loginPrefix = "";

		private readonly ILogger<LoginController> _logger;

		protected readonly IStringLocalizer<LangResource> _localizer;//语言包

		public LoginController(IHttpContextAccessor accessor, IStringLocalizer<LangResource> localizer, SqlSugarScope context, IWebHostEnvironment env,
			TokenManager tokenManager, ILogger<LoginController> logger, IPubConfigService pubConfigService, ILanguageService languageService,IUserService userService)
		{
			_isSms = Appsettings.app("Web:IsSMS");
			_loginPrefix = Appsettings.app("Web:LoginPrefix");
			_httpContext = accessor.HttpContext;
			_context = context;
			_env = env;
			_logger = logger;

			_tokenManager = tokenManager;
			_localizer = localizer;
			_pubConfigService = pubConfigService;
			_languageService = languageService;
		}

		// GET: Admin_WeTalk/Login
		public async Task<IActionResult> Index()
		{
			string ReturnUrl = _httpContext.Request.Query["returnurl"].ToString();
			var cookie = _httpContext.Request.Cookies.FirstOrDefault(m => m.Key == _loginPrefix + "Token");
			if (cookie.Key != null && !string.IsNullOrEmpty(ReturnUrl))
			{
				string value = cookie.Value;
				//重新通过cookie中的token登录
				var userinfo = _context.Queryable<AdminMaster>().First(u => u.Token == value && u.Status == 1);
				if (userinfo != null)
				{
					_tokenManager.CreateSession(userinfo, 2, userinfo.LastLang);
					return Redirect(ReturnUrl);
				}
			}

			ReturnUrl = (ReturnUrl == "") ? "/" + RouteData.Values["area"] + "/" + RouteData.Values["Controller"] : ReturnUrl;
			var list_lang = _context.Queryable<PubLanguage>().Where(u => u.Isadmin == 1).ToList();

			var config = new Login
			{
				IsSMS = _isSms,
				ReturnUrl = ReturnUrl,
				LangItem = list_lang.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(u.Title, u.Lang)).ToList()
			};
			return View(config);
		}


		[HttpPost]
		public async Task<IActionResult> Index(Login vm_login)
		{
			//调接口
			var dic_config = _pubConfigService.GetConfigs("", "app_key,blackip,whileip");//取通用版字典
			string userpwd = MD5Helper.MD5Encrypt32(vm_login.Password).ToLower();
			var model_admin_master = await _context.Queryable<AdminMaster>().FirstAsync(u => u.Status == 1 && u.Username == vm_login.UserName && (u.Userpwd.ToLower() == userpwd || u.Userpwd.ToLower() == userpwd.Substring(8, 16)));
			if (model_admin_master != null)
			{
				Console.WriteLine(dic_config.Count);
				model_admin_master.LastLang = vm_login.Lang;
				//判断是否授权IP 
				string ip_str = IpHelper.GetCurrentIp(_httpContext);
				string blackip = dic_config.Keys.Contains("blackip") ? (dic_config["blackip"] + "").Trim() : "";
				string whileip = dic_config.Keys.Contains("whileip") ? (dic_config["whileip"] + "").Trim() : "";
				//失败次数过多，锁定1小时
				if (model_admin_master.Nums >= 5 && (DateTime.Now - model_admin_master.Lasttime).TotalSeconds < 3600)
				{
					return new JsonResult(new { code = -1, msg = string.Format( _localizer["您的账号已被锁定,离解锁时间还需{0}分钟"], (60 - (DateTime.Now - model_admin_master.Lasttime).TotalMinutes).ToString("0")) });
				}

				//判断IP授权
				if (whileip != "" && !("," + whileip.ToString() + ",").Contains("," + ip_str + ","))
				{
				}
				else if (blackip != "" && ("," + blackip.ToString() + ",").Contains("," + ip_str + ",")) //判断是否被加入黑名单IP 
				{
					model_admin_master.Ip = ip_str;
					model_admin_master.Nums++;
					model_admin_master.Lasttime = DateTime.Now;

					_context.Updateable(model_admin_master).UpdateColumns(u => new { u.Ip, u.Nums, u.Lasttime }).ExecuteCommand();
					_logger.LogInformation("失败：IP被加入黑名单");
					return new JsonResult(new { code = -1, msg = _localizer["您已被禁止登录，请联系管理员进行处理"] });
				}

				//短信验证码判断
				if (Appsettings.app("Web:IsSMS") == "1")
				{
					if ((model_admin_master.SmsTime < DateTime.Now.AddMinutes(-3) || model_admin_master.SmsCode.Trim().ToLower() != vm_login.SafeCode.ToLower() || vm_login.SafeCode.Trim() == ""))
					{
						model_admin_master.Ip = ip_str;
						model_admin_master.Nums++;
						model_admin_master.Lasttime = DateTime.Now;
						await _context.Updateable(model_admin_master).UpdateColumns(u => new { u.Ip, u.Nums, u.Lasttime }).ExecuteCommandAsync();
						_logger.LogInformation("失败：手机验证码不正确，或验证超时");
						return new JsonResult(new { code = -1, msg = _localizer["您的手机验证码不正确，或验证超时"] });
					}
				}
				model_admin_master.Lasttime = (DateTime)DateTime.Now;
				model_admin_master.Loginnum = model_admin_master.Loginnum + 1;
				model_admin_master.Ip = ip_str;
				model_admin_master.Nums = 0;


				List<long> list_roleid = new List<long>();
				List<long> list_functionid = new List<long>();
				List<long> list_menuid = new List<long>();
				if (("," + Appsettings.app("Web:Admins") + ",").Contains("," + vm_login.UserName.ToLower().Trim() + ","))
				{

				}
				else
				{
					//取用户角色ID集
					var list_admin_masterRoles = _context.Queryable<AdminMasterRole>().Where(u => u.AdminMasterid == model_admin_master.AdminMasterid).ToList();
					list_roleid = list_admin_masterRoles.Select(u => u.AdminRoleid).Distinct().ToList();


					//取角色对应的控件ID集
					var list_AdminRoleMenuFunctions = _context.Queryable<AdminRoleMenuFunction>().Where(u => u.Status != -1 && list_roleid.Contains(u.AdminRoleid)).ToList();
					list_functionid = list_AdminRoleMenuFunctions.Select(u => u.AdminMenuFunctionid).Distinct().ToList();
					list_menuid = list_AdminRoleMenuFunctions.Select(u => u.AdminMenuid).Distinct().ToList();

					////取用户对应控件ID集
					//var model_admin_masterMenuFunctions = _context.admin_masterMenuFunction.AsNoTracking().Where(u => u.admin_masterid == model_admin_master.admin_masterid).ToList();
					//for (int i = 0; i < model_admin_masterMenuFunctions.Count; i++)
					//{
					//    if (!("," + functionids + ",").Contains("," + model_admin_masterMenuFunctions[i].admin_menu_functionid + ",")) functionids += ("," + model_admin_masterMenuFunctions[i].admin_menu_functionid);
					//    if (!("," + menuids + ",").Contains("," + model_admin_masterMenuFunctions[i].admin_menuid + ",")) menuids += ("," + model_admin_masterMenuFunctions[i].admin_menuid);
					//}
				}
				if (list_menuid.Count > 0)
				{
					var list_admin_menu = _context.Queryable<AdminMenu>().Where(u => list_menuid.Contains(u.AdminMenuid)).ToList();
					for (int i = 0; i < list_admin_menu.Count; i++)
					{
						var list = list_admin_menu[i].Path.Split('|').Where(u => !string.IsNullOrEmpty(u)).Select(u => long.Parse(u)).ToList();
						list_menuid.AddRange(list);
					}
				}
				list_menuid = list_menuid.Distinct().ToList();
				_context.Updateable(model_admin_master).UpdateColumns(u => new { u.Ip, u.Nums, u.Lasttime, u.Loginnum, u.Token });
				_logger.LogInformation("后台成功登录");
			
				string token = _tokenManager.CreateSession(model_admin_master, 2, vm_login.Lang);
				_context.Updateable<AdminMaster>().SetColumns(u => u.Token == token).Where(u => u.AdminMasterid == model_admin_master.AdminMasterid).ExecuteCommand();

				await _languageService.SetLang(vm_login.Lang);//设置语言

				string ReturnUrl = HttpContext.Request.Query["returnurl"].ToString();
				ReturnUrl = (ReturnUrl == "") ? "/" + RouteData.Values["area"] + "/Main" : ReturnUrl;
				return new JsonResult(new { code = 200, returnurl = ReturnUrl });
			}
			else
			{
				return new JsonResult(new { code = -1, msg = _localizer["账号密码输入错误"]+"" });
			}
		}


		[HttpPost]
		public async Task<IActionResult> SMS(Login login)
		{
			string code = "";
			var api = new ApiResult<string>();
			var model_admin_master = _context.Queryable<AdminMaster>().First(u => u.Username == login.UserName);
			if (model_admin_master != null)
			{
				if (!string.IsNullOrEmpty(model_admin_master.Mobile.Trim()))
				{
					//code = _publibService.GetSMS(model_admin_master.mobile); //这里短信网关发送
					api.Data = code;
				}
				else
				{
					api.StatusCode = 4009;
					api.Message = $"error:{_localizer["您的手机号不存在，请联系管理员维护"]}！";
				}
				model_admin_master.SmsCode = code;
				model_admin_master.SmsTime = DateTime.Now;
				_context.Updateable(model_admin_master).UpdateColumns(u => new { u.SmsCode, u.SmsTime }).ExecuteCommand();
				return Content("");
			}
			else
			{
				api.StatusCode = 4009;
				api.Message = $"error:{_localizer["用户不存在"]}！";
				return Content($"error:{_localizer["用户不存在"]}！");
			}
		}

		// GET: Admin_WeTalk/Error
		public async Task<IActionResult> Error()
		{
			string Code = _httpContext.Request.Query["code"].ToString();
			string ReturnUrl = _httpContext.Request.Query["return_url"].ToString();
			string Msg = _httpContext.Request.Query["msg"].ToString();

			var error = new Error
			{
				Code = (Code == "") ? "404" : Code,
				Msg = Msg,
				ReturnUrl = (ReturnUrl == "") ? "/" + RouteData.Values["area"] + "/Login" : System.Net.WebUtility.UrlDecode(ReturnUrl)
			};

			return View(error);
		}

		public async Task<IActionResult> Exit()
		{
			//清除redis
			if (_httpContext.Session.Keys.Contains(_loginPrefix + "AdminID"))
			{
				long admin_masterid = long.Parse(_httpContext.Session.GetString(_loginPrefix + "AdminID"));
				_tokenManager.RemoveAllSession(admin_masterid);
			}
			else {
				var token = _tokenManager.GetSysToken;
				if(!string.IsNullOrEmpty(token))_tokenManager.RemoveSession(token);
			}
			//清COOKIE
			CookieHelper.DeleteCookie(_httpContext, _loginPrefix + "Token");
			//清session
			_httpContext.Session.Clear();
			return RedirectToAction("Index", "Login");
		}

	}
}
