using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using WeTalk.Extensions;

namespace WeTalk.Web.Services
{
	public class RedisService : IRedisService
	{

		private readonly IConfiguration _config;
		private HttpContext _httpcontent;
		private static bool _isdevelopment = false;
		public readonly IPublibService _publibService;

		public RedisService(IHttpContextAccessor httpContext, IWebHostEnvironment env, IConfiguration config, IPublibService publibService)
		{
			_config = config;
			_httpcontent = httpContext.HttpContext;
			_isdevelopment = env.IsDevelopment();

			_publibService = publibService;
		}


		#region "设置用户与Token"
		/// <summary>
		/// 设置Token,键值：token:946a2260a5595037c4d32e2ee277dbaaa46e3682
		/// </summary>        
		public async Task SetTokenAsync(string token, Models.Redis.UserToken redis_model, int times = 7200)
		{
			await RedisServer.Cache.HMSetAsync("token:" + token, _publibService.ToHashEntries(redis_model));
			if (times > 0) await RedisServer.Cache.ExpireAsync("token:" + token, times);
		}
		public async Task<bool> SetTokenFieldAsync(string token, string field, object val, int times = 7200)
		{
			bool isok = await RedisServer.Cache.HMSetAsync("token:" + token, field, val);
			if (times > 0) await RedisServer.Cache.ExpireAsync("token:" + token, times);
			return isok;
		}
		public void SetToken(string token, Models.Redis.UserToken redis_model, int times = 7200)
		{
			RedisServer.Cache.HMSet("token:" + token, _publibService.ToHashEntries(redis_model));
			if (times > 0) RedisServer.Cache.Expire("token:" + token, times);
		}
		public bool SetTokenField(string token, string field, object val, int times = 7200)
		{
			if (!ExistsToken(token)) return false;//如果KEY都不存在，写入字段就没有意义了
			bool isok = RedisServer.Cache.HSet("token:" + token, field, val);
			if (times > 0) RedisServer.Cache.Expire("token:" + token, times);
			return isok;
		}
		public void DelToken(string token)
		{
			RedisServer.Cache.Del("token:" + token);
		}
		public async Task DelTokenAsync(string token)
		{
			await RedisServer.Cache.DelAsync("token:" + token);
		}
		public Models.Redis.UserToken GetToken(string token)
		{
			var hashEntries = RedisServer.Cache.HGetAll("token:" + token);
			if (hashEntries != null)
			{
				return _publibService.ConvertFromRedis<Models.Redis.UserToken>(hashEntries);
			}
			else
			{
				return null;
			}
		}
		public string GetToken(string token, string key)
		{
			if (!string.IsNullOrEmpty(key))
			{
				return RedisServer.Cache.HGet("token:" + token.Trim(), key);
			}
			else
			{
				return "";
			}
		}
		public bool ExistsToken(string token)
		{
			return RedisServer.Cache.Exists("token:" + token.Trim());
		}
		public bool ExistsToken(string token, string key)
		{
			return RedisServer.Cache.HExists("token:" + token.Trim(), key);
		}
		public void UpdateTokenExpire(string token, int times = 7200)
		{
			if (times > 0) RedisServer.Cache.Expire("token:" + token.Trim(), times);
		}

		/// <summary>
		/// 设置User,键值：user:eric
		/// </summary>        
		public async Task SetUserAsync(string username, Models.Redis.UserToken redis_model, int times = 7200)
		{
			await RedisServer.Cache.SetAsync("user:" + username.Trim(), redis_model.Token);
			await SetTokenAsync(redis_model.Token, redis_model, times);
		}
		public async Task<bool> SetUserFieldAsync(string username, string field, object val, int times = 7200)
		{
			if (RedisServer.Cache.Exists("user:" + username))
			{
				string jstoken = RedisServer.Cache.Get("user:" + username);
				return await SetTokenFieldAsync(jstoken, field, val, times);
			}
			else
			{
				return false;
			}
		}
		public void SetUser(string username, Models.Redis.UserToken redis_model, int times = 7200)
		{
			RedisServer.Cache.Set("user:" + username, redis_model.Token);
			SetToken(redis_model.Token, redis_model, times);
		}
		public bool SetUserField(string username, string field, object val, int times = 7200)
		{
			if (RedisServer.Cache.Exists("user:" + username))
			{
				string jstoken = RedisServer.Cache.Get("user:" + username);
				if (field.ToLower() == "jstoken" && jstoken.ToLower() != val.ToString().ToLower())
				{
					jstoken = val.ToString();
					RedisServer.Cache.Set("user:" + username, val);
				}
				return SetTokenField(jstoken, field, val, times);
			}
			else
			{
				return false;
			}
		}
		public void DelUser(string username)
		{
			RedisServer.Cache.Del("user:" + username);
		}
		public async Task DelUserAsync(string username)
		{
			await RedisServer.Cache.DelAsync("user:" + username);
		}
		public Models.Redis.UserToken GetUser(string username)
		{
			string jstoken = RedisServer.Cache.Get("user:" + username);
			return GetToken(jstoken);
		}
		public string GetUser(string username, string key)
		{
			string jstoken = RedisServer.Cache.Get("user:" + username);
			if (string.IsNullOrEmpty(jstoken)) return null;
			return GetToken(jstoken, key);
		}
		public bool ExistsUser(string username)
		{
			return RedisServer.Cache.Exists("user:" + username.Trim());
		}
		public bool ExistsUser(string username, string key)
		{
			string jstoken = RedisServer.Cache.Get("user:" + username);
			return ExistsUser(jstoken.Trim(), key);
		}
		#endregion


		//===========================================================================


	}
}
