using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeTalk.Common.Helper;
using WeTalk.Web.Extensions;

namespace WeTalk.Web.Base
{
	public class BaseController : Controller
	{
		//public string WebRoot = "";
		//public string AppRoot = "";
		//public string ImgRoot = "";
		//public string LoginPrefix = "";
		public bool IsAdmin = false;    //是否超管
		public long AdminMasterid = 0;
        public string AdminMasterName = "";
        public string Lang = "cn"; //优先取QUERY值，次取Cookie值

		public BaseController(TokenManager tokenManager)
		{
			//if (string.IsNullOrEmpty(config["Web:WebRoot"])) WebRoot = env.WebRootPath;
			//if (string.IsNullOrEmpty(config["Web:AppRoot"])) AppRoot = env.ContentRootPath;
			//ImgRoot = config["Web:ImgRoot"];
			//LoginPrefix = config["Web:LoginPrefix"];
			var model = tokenManager.GetAdminInfo();
			if (model != null)
			{
				AdminMasterid = model.AdminMasterid;
				AdminMasterName = model.Username;
                IsAdmin = model.Isadmin;
			}
			//Lang = model.lang;//这里不再取db和redis中的语言，改成取cookie中的值
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var lang_cookie = CookieHelper.GetCookie(filterContext.HttpContext, CookieRequestCultureProvider.DefaultCookieName);
			string lang = filterContext.HttpContext.Request.Query["lang"];
			if (!string.IsNullOrEmpty(lang))
			{
				Lang = lang;
			}
			else
			{
				Lang = (!string.IsNullOrEmpty(lang_cookie)) ? CookieRequestCultureProvider.ParseCookieValue(lang_cookie).Cultures[0].Value.ToLower() : "";
			}
			ViewBag.Lang = Lang;
			base.OnActionExecuting(filterContext);
		}



	}
}
