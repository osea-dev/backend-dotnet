using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using SqlSugar;
using System.Threading.Tasks;
using WeTalk.Models;

namespace WeTalk.Interfaces.Services
{
	public class LanguageService : ILanguageService
	{
		private readonly IHttpContextAccessor _accessor;
		public LanguageService(IHttpContextAccessor accessor)
		{
			_accessor = accessor;
		}

		#region ȡ��ǰ���԰�
		public async Task<ApiResult> SetLang(string lang)
		{
			var api = new ApiResult();
			_accessor.HttpContext.Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName, //Ĭ�� Cookie �����ǣ�.AspNetCore.Culture
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang))
				);		
			return api;
		}
		#endregion

	}
}
