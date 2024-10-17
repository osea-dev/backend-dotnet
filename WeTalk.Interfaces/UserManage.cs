using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Extensions;
using WeTalk.Models;
using WeTalk.Common.Helper;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace WeTalk.Interfaces
{
	public class UserManage:BaseService, IUserManage
	{
		private readonly IHttpContextAccessor _accessor;

		private readonly SqlSugarScope _context;
		private readonly ILogger<UserManage> _logger;

		/// <summary>
		/// 用户ID
		/// </summary>
		public long Userid { get; set; }
		/// <summary>
		/// 用户Token
		/// </summary>
		public string UserToken { get; set; }
		/// <summary>
		/// 展示币种
		/// </summary>
		public string CurrencyCode { get; set; }
		/// <summary>
		/// 结算币种
		/// </summary>
		public string SettlementCurrencyCode { get; set; }
		/// <summary>
		/// 语言
		/// </summary>
		public string Lang { get; set; }
		/// <summary>
		/// UTC名称
		/// </summary>
		public string Utc { get; set; }
		/// <summary>
		/// 时区差(分)
		/// </summary>
		public int UtcSec { get; set; } = -480;//默认中国

		public UserManage(SqlSugarScope dbcontext, ILogger<UserManage> logger, IHttpContextAccessor accessor)
		{
			_accessor = accessor;
			_context = dbcontext;
			_logger = logger;

			if(string.IsNullOrEmpty(UserToken)) UserToken = GSRequestHelper.GetServerString(_accessor.HttpContext, "X-UserToken");
			if (string.IsNullOrEmpty(CurrencyCode)) CurrencyCode = GSRequestHelper.GetServerString(_accessor.HttpContext, "X-CurrencyCode");
			//语言不能切换前写死中文
			if (string.IsNullOrEmpty(Lang)) Lang = GSRequestHelper.GetServerString(_accessor.HttpContext, "Accept-Language");
			if (string.IsNullOrEmpty(Utc)) Utc = GSRequestHelper.GetServerString(_accessor.HttpContext, "X-Utc");
			string utcSec = GSRequestHelper.GetServerString(_accessor.HttpContext, "X-UtcSec");
			if (!string.IsNullOrEmpty(utcSec)) UtcSec = int.Parse(utcSec);
			if (string.IsNullOrEmpty(SettlementCurrencyCode)) SettlementCurrencyCode = GetCurrencyCode(GetCountryCode(""));			
			if (string.IsNullOrEmpty(CurrencyCode)) CurrencyCode = SettlementCurrencyCode;// "USD";//默认美元
			Lang += "";
			if (string.IsNullOrEmpty(Lang) || !_context.Queryable<PubLanguage>().Any(u=>u.Lang==Lang.ToLower() && u.Status==1)) Lang = "zh-cn";//默认中文
		}

		/// <summary>
		/// 存放httpcontext.request.body
		/// </summary>
		public string RequestBody { get; set; }

		/// <summary>
		/// 取指定IP国家代码
		/// </summary>
		/// <param name="ip"></param>
		/// <returns></returns>
		public string GetCountryCode(string ip="")
		{
			//_logger.LogInformation(request.Scheme + "://" + request.Host + request.Path + request.QueryString);

			// 更新IP定位
			ip = string.IsNullOrEmpty(ip) ? IpHelper.GetCurrentIp(_accessor.HttpContext) : ip;
			if (string.IsNullOrEmpty(ip) || ip == "127.0.0.1" ||ip== "0.0.0.1" || ip.StartsWith("192") || ip.StartsWith("172")) return "";
			string code = "";
			string key = ip;
			if (RedisServer.Cache.Exists(key))
			{
				code = RedisServer.Cache.Get(key);
				RedisServer.Cache.Set(key, code, 60 * 24 * 3600);
			}
			else
			{
				var url = Appsettings.app("APIS:Aliyun.Ip:Url");
				var dic_header = new Dictionary<string, string>();
				dic_header.Add("Authorization", "APPCODE " + Appsettings.app("APIS:Aliyun.Ip:AppCode"));
				try
				{
					_logger.LogInformation(ip + ":yes");
					var json = GetRemoteHelper.HttpWebRequestUrl(url, "ip=" + ip, "utf-8", "post", dic_header);
					var o = JObject.Parse(json);
					if (o != null && o["code"] != null && o["code"].ToString() == "200" && o["data"] != null && o["data"]["country_id"] != null)
					{
						code = o["data"]["country_id"].ToString();
						RedisServer.Cache.Set(key, code, 60 * 24 * 3600);
					}
					else
					{
						code = "CN";
					}
				}
				catch
				{
					code = "";
				}
			}
			return code;
		}

		/// <summary>
		/// 取指定币种
		/// </summary>
		/// <param name="country_code"></param>
		/// <returns></returns>
		public string GetCurrencyCode(string country_code = "")
		{
			var model_Currency = _context.Queryable<PubCurrency>().First(u => u.CountryCode == country_code);
			if (model_Currency != null)
				return model_Currency.CurrencyCode;
			else
				return "USD";
		}
		/// <summary>
		/// 设置用户Token
		/// </summary>
		/// <param name="userid"></param>
		/// <returns></returns>
		public Models.Redis.UserToken SetUserToken(long userid, int expiresin = 7200)
		{
			string token = RandomHelper.GetMd5Code(userid.ToString());
			

			var model_User = _context.Queryable<WebUser>().InSingle(userid);
			if (model_User != null)
			{
				var ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
				var model_token = new Models.Redis.UserToken();
				model_token.Userid = model_User.Userid;
				model_token.FirstName = model_User.FirstName;
				model_token.LastName = model_User.LastName;
				model_token.MobileCode = model_User.MobileCode;
				model_token.Mobile = model_User.Mobile;
				model_token.Email = model_User.Email;
				model_token.Birthdate = model_User.Birthdate;
				model_token.HeadImg = model_User.HeadImg;
				model_token.Lang = Lang;
				model_token.Updatetime = DateTime.Now;
				model_token.Expiresin = expiresin;
				model_token.Token = token;
				model_token.SettlementCurrencyCode = string.IsNullOrEmpty(SettlementCurrencyCode) ? GetCurrencyCode(GetCountryCode(ip)) : SettlementCurrencyCode;//结算币种
				RedisServer.Token.HMSet(token, ToHashEntries(model_token));
				RedisServer.Token.Expire(token, expiresin);
                RedisServer.Token.Del(model_User.Token);//旧的要删掉

                model_User.Utc = Utc;
				model_User.UtcSec= UtcSec;
				model_User.Lang = Lang;
				model_User.Token = token;
				model_User.Updatetime = DateTime.Now;
				model_User.Expiresin = expiresin;
				model_User.Lastip = ip;
				model_User.Lasttime = DateTime.Now;
				model_User.CurrencyCode = model_token.SettlementCurrencyCode;
				_context.Updateable(model_User).UpdateColumns(u => new { u.Lang ,u.Lastip, u.Lasttime, u.CurrencyCode,u.Token,u.Updatetime,u.Expiresin,u.Utc,u.UtcSec}).ExecuteCommand();

				UserToken = token;
				return model_token;
			}
			else {
				return null;
			}

		}

		/// <summary>
		/// 通过Token获取用户相关信息
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public Models.Redis.UserToken GetUserToken()
		{
			string token = UserToken;
			if (string.IsNullOrEmpty(token)) return null;
			// 会员客户端览权逻辑
			// 1，判断是否过期:判断REDIS，如不存在，则判断DB，再不存在则返回NULL
			// 2，判断Updatetime是否超过24小时，超过则加更新当前CountryCode，IP定位API刷
			// 3，刷新Token时间：Updatetime
			var hashEntries = RedisServer.Token.HGetAll(token);
			try
			{
				string ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
				var model_token = (hashEntries.Count > 0) ? ConvertFromRedis<Models.Redis.UserToken>(hashEntries) : null;
				if (model_token != null)
				{
					if (model_token.Updatetime.AddHours(24) < DateTime.Now) {
						// 更新IP定位
						model_token.SettlementCurrencyCode = string.IsNullOrEmpty(SettlementCurrencyCode) ? GetCurrencyCode(GetCountryCode(ip)) : SettlementCurrencyCode;//结算币种
						_context.Updateable<WebUser>()
							.SetColumns(u => u.Utc == Utc)
							.SetColumns(u => u.UtcSec == UtcSec)
							.SetColumns(u => u.Lang == Lang)
                            .SetColumns(u => u.Lastip == ip)
							.SetColumns(u => u.Lasttime == DateTime.Now)
							.Where(u => u.Userid == model_token.Userid)
							.ExecuteCommand();
					}
					model_token.Updatetime = DateTime.Now;
					model_token.Lang = Lang;
					this.Userid = model_token.Userid;
					RedisServer.Token.HMSet(token, ToHashEntries(model_token));
					RedisServer.Token.Expire(token, model_token.Expiresin);
				}
				else
				{
					//Redis异常，从DB中检查
					//从DB取出有效TOKEN并写入REDIS
					var model_User = _context.Queryable<WebUser>().First(u=>u.Token==token);
					if (model_User != null && model_User.Status != -1 && model_User.Updatetime.AddSeconds(model_User.Expiresin - 100) >= DateTime.Now)
					{
						this.Userid = model_User.Userid;
						model_token = new Models.Redis.UserToken();
						model_token.Userid = model_User.Userid;
						model_token.FirstName = model_User.FirstName;
						model_token.LastName = model_User.LastName;
						model_token.MobileCode = model_User.MobileCode;
						model_token.Mobile = model_User.Mobile;
						model_token.Email = model_User.Email;
						model_token.Birthdate = model_User.Birthdate;
						model_token.HeadImg = model_User.HeadImg;
						model_token.Lang = Lang;

						model_token.Updatetime = DateTime.Now;
						model_token.Expiresin = model_User.Expiresin;
						model_token.Token = model_User.Token;
						model_token.SettlementCurrencyCode = string.IsNullOrEmpty(SettlementCurrencyCode) ? GetCurrencyCode(GetCountryCode(ip)) : SettlementCurrencyCode;//结算币种
						RedisServer.Token.HMSet(token, ToHashEntries(model_token));
						RedisServer.Token.Expire(token, model_token.Expiresin);

                        model_User.Utc = Utc;
						model_User.UtcSec = UtcSec;
						model_User.Lang = Lang;
						model_User.Updatetime = DateTime.Now;
						model_User.Lastip = ip;
						model_User.Lasttime = DateTime.Now;
						model_User.CurrencyCode = model_token.SettlementCurrencyCode;
						_context.Updateable(model_User).UpdateColumns(u => new {u.Lang, u.Lastip, u.Lasttime, u.CurrencyCode, u.Updatetime, u.Utc, u.UtcSec }).ExecuteCommand();
						UserToken = model_User.Token;
					}
					else
					{
						//过期了，需要重新AUTH
					}
				}
				return model_token;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "UserManage刷用户token异常");
				RedisServer.Token.Del(token);
				return null;
			}
		}
	}
}
