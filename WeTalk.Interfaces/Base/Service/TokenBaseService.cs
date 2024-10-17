using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SqlSugar;
using WeTalk.Common;
using WeTalk.Common.Helper;

namespace WeTalk.Interfaces.Base
{
	public partial class TokenBaseService : BaseService, ITokenBaseService
	{
		private readonly SqlSugarScope _context;
		private readonly ILogger<TokenBaseService> _logger;
		private readonly IWebHostEnvironment _env;
		private readonly IPubConfigBaseService _pubConfigBaseService;

		public TokenBaseService(SqlSugarScope dbcontext,  ILogger<TokenBaseService> logger, IWebHostEnvironment env, IPubConfigBaseService pubConfigBaseService)
		{
			_context = dbcontext;
			_logger = logger;
			_env = env;
			_pubConfigBaseService = pubConfigBaseService;
		}

		#region "刷新AccessToken"
		/// <summary>
		/// 获取微信服务端AccessToken
		/// </summary>
		/// <returns></returns>
		public string UpdateAccessToken()
		{
			string access_token = "";
			var dic_config = _pubConfigBaseService.GetConfigs("appid,secret");
			string url = Appsettings.app("APIS:auth.getAccessToken");
			var json = GetRemoteHelper.GetRemoteCode(url, "utf-8");
			var o = JObject.Parse(json);
			if (o != null && o["access_token"] != null) {
				access_token = o["access_token"].ToString();
				var expires_in = (o["expires_in"]!=null)?o["expires_in"].ToString():"7200";
				dic_config.Clear();
				dic_config.Add("access_token", access_token);
				dic_config.Add("expires_in", expires_in);
				_pubConfigBaseService.UpdateConfigs(dic_config);
			}
			return access_token;
		}
		#endregion

	}
}
