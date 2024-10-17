using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.Text;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Interfaces;
using WeTalk.Models;
using WeTalk.Web.Extensions;
using WeTalk.Web.Services;

namespace WeTalk.Web.Areas.Admin_WeTalk.Controllers
{
	[Area("Admin_WeTalk")]
	public class ConfigController : Base.BaseController
	{
		private SqlSugarClient _context=null;
		private IHttpContextAccessor _accessor;
		private readonly TokenManager _tokenManager;
		protected readonly IStringLocalizer<LangResource> _localizer;//语言包
		public ConfigController(IConfiguration config, IHttpContextAccessor accessor, IWebHostEnvironment env, TokenManager tokenManager, IStringLocalizer<LangResource> localizer)
			: base(tokenManager)
		{
			_accessor = accessor;
			_tokenManager = tokenManager;
			_localizer = localizer;
		}

        #region "系统设置" 
        [Authorization(Power = "Main")]
        public async Task<IActionResult> Index(string key)
		{
			ViewBag.ScriptStr = _tokenManager.ScriptStr;
			Web.ViewModels.Config.Index vm_config = new Web.ViewModels.Config.Index();

			var model_Config = _context.Queryable<ViewPubConfig>().First(u => u.Key.ToLower() == key.ToLower() && u.Lang== Lang);
			if (model_Config != null) {
				vm_config.Val = model_Config.Val;
				vm_config.PubConfigid = model_Config.PubConfigid;
			}
			return View(vm_config);
		}

		[HttpPost]
        [Authorization(Power = "Main")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(Web.ViewModels.Config.Index vm_config)
		{
			if (ModelState.IsValid)
			{
				int isrefresh = GSRequestHelper.GetInt(_accessor.HttpContext.Request, "chk_refresh", 0);
				string json = "";
				try
				{
					var val = JArray.Parse(vm_config.Val);
					json = JsonConvert.SerializeObject(val);
				}
				catch
				{
					return Content("<script>alert('"+ _localizer["提交的值解析失败"]+"');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
				}
				int count = 0;
				var model_ConfigLang = _context.Queryable<PubConfigLang>().First(u => u.PubConfigid == vm_config.PubConfigid && u.Lang == vm_config.Lang);
				if (model_ConfigLang != null)
				{
					model_ConfigLang.Val = json;
					count = _context.Updateable(model_ConfigLang).ExecuteCommand();
				}
				else {
					model_ConfigLang = new PubConfigLang();
					model_ConfigLang.PubConfigid = model_ConfigLang.PubConfigid;
					model_ConfigLang.Lang = vm_config.Lang;
					model_ConfigLang.Val = json;
					count = _context.Insertable(model_ConfigLang).ExecuteCommand();
				}
				if(count==0) 
					return Content("<script>alert('" + _localizer["修改配置失败"] + "');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
				
				return Content("<script>alert('" + _localizer["修改配置成功"] + "');history.go(-1);</script>", "text/html", Encoding.GetEncoding("utf-8"));
			}
			return View(vm_config);
		}
		#endregion

	}
}