using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SqlSugar;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Interfaces;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Web.ViewModels;

namespace WeTalk.Web.Areas.Admin_WeTalk.ViewComponents
{
	public class LanguageTagViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly SqlSugarScope _context;
        private readonly IStringLocalizer<LangResource> _localizer;//语言包
        public LanguageTagViewComponent(IHttpContextAccessor accessor, SqlSugarScope context, IStringLocalizer<LangResource> localizer)
        {
            _accessor = accessor;
            _context = context;
            _localizer = localizer;
        }

        public async Task<IViewComponentResult> InvokeAsync(string lang = "",int Isadmin = -1,string query = "")
        {
            //if (string.IsNullOrEmpty(FunctionName)) FunctionName = "changeLangTag";
            var lang_cookie = CookieHelper.GetCookie(_accessor.HttpContext, CookieRequestCultureProvider.DefaultCookieName);
            string Lang = (string.IsNullOrEmpty(lang) && !string.IsNullOrEmpty(lang_cookie))?CookieRequestCultureProvider.ParseCookieValue(lang_cookie).Cultures[0].Value: lang;
            StringBuilder str = new StringBuilder();
            var url = _accessor.HttpContext.Request.GetRequestUrl();
            if (!url.Contains("lang="))
            {
                if (url.Contains("?"))
                {
                    url += "&lang=" + Lang;
                }
                else
                {
                    url += "?lang=" + Lang;
                }
            } 
            if (!string.IsNullOrEmpty(query))
            {
                url += "&" + query;
            }
            var vm_LangTag = new LanguageTag();
            var list_Lang = _context.Queryable<PubLanguage>().Where(u=>u.Status==1).WhereIF(Isadmin > -1,u=>u.Isadmin == Isadmin).ToList();
            foreach (var model in list_Lang) {
                str.Append("<li class=\"nav-item\">");
                //str.Append($"<a class=\"nav-link {(model.Lang.ToLower() == Lang.ToLower() ? "active":"")}\" id=\"lang_{model.Languageid}\" data-toggle=\"tab\" href=\"#lang_{model.Languageid}\" onclick=\"{FunctionName}('{model.Lang}')\">{model.Title}</a>");
                url = UrlHelper.ReplaceQueryStringValue(url, "lang", model.Lang);
                str.Append($"<a class=\"nav-link {(model.Lang.ToLower() == Lang.ToLower() ? "active" : "")}\" href=\"{url}\" onclick=\"javascript:return confirm('{ _localizer["切换语言之前请先保存当前页信息, 确认要切换吗"]}?')\" >{model.Title}<i class=\"mdi mdi-google-translate\"></i></a>");
                str.Append("</li>\r\n");
            }
            vm_LangTag.LangList = str.ToString();
            return View(vm_LangTag);
        }
    }
}