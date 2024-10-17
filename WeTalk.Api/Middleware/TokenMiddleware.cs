using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Interfaces;
using WeTalk.Models;

namespace WeTalk.Api.Middleware
{
	public class TokenMiddleware: IMiddleware
	{
		private SqlSugarScope _context;
		private IWebHostEnvironment _env;
		private IBaseService _baseService;
		private IUserManage _userManage;
		private ILogger<TokenMiddleware> _logger;
		/// <summary>
		/// 管道执行到该中间件时候下一个中间件的RequestDelegate请求委托，如果有其它参数，也同样通过注入的方式获得
		/// </summary>
		/// <param name="next"></param>
		public TokenMiddleware(IWebHostEnvironment env, SqlSugarScope dbcontext, ILogger<TokenMiddleware> logger,
			 IBaseService baseService, IUserManage userManage)
		{
			_baseService = baseService;
			_userManage = userManage;
			_context = dbcontext;
			_env = env;
			_logger = logger;
			//通过注入方式获得对象
		}

		/// <summary>
		/// 自定义中间件要执行的逻辑
		/// </summary>
		/// <param name="httpcontext"></param>
		/// <returns></returns>
		public async Task InvokeAsync(HttpContext httpcontext, RequestDelegate _next )
		{
			var reader = new StreamReader(httpcontext.Request.Body);
			_userManage.RequestBody = (await reader.ReadToEndAsync());
			if(!string.IsNullOrEmpty(_userManage.RequestBody))
				httpcontext.Request.Body.Position = 0;
			//long userid = 0;

			var list_While = Appsettings.app<string>("Web:ApiWhiteList");
			if (!_env.IsDevelopment() && !list_While.Any(u => httpcontext.Request.Path.StartsWithSegments(u)))
			{
				//非白名单才需合法性较验
				var result = CheckSign(httpcontext).Result;
				if (!result.isok)
				{
					HandleExceptionAsync(httpcontext, result.error_code, result.msg, _userManage.Userid);
					return;
				}
			}
			_next.Invoke(httpcontext).Wait();
		}
		///4003：Ticket鉴权失效
		///4002：字段不完整 
		///4001：签名验证失败

		/// <summary>
		/// 签名较验
		/// </summary>
		/// <param name="context"></param>
		private async Task<(bool isok, int error_code, string msg)> CheckSign(HttpContext context)
		{
			var data = _userManage.RequestBody;
			string sign = GSRequestHelper.GetString(context.Request, "sign");
			string timestamp = GSRequestHelper.GetString(context.Request, "timestamp");
			string url = context.Request.Path;// context.Request.Scheme + "://" + context.Request.Host +

			//非开发环境，并且签名较验开启IsSign=1
			if (!_env.IsDevelopment()  && Appsettings.app("Web:IsSign") == "1")
			{
				if (string.IsNullOrEmpty(timestamp))
				{
					return (false, 4003, "url=" + url + ",缺少参数timestamp");
				}
				if (DateHelper.ConvertInt13ToDateTime(timestamp) < DateTime.UtcNow.AddMinutes(-10))
				{
					return (false, 4003, "url=" + url + ",参数timestamp过时");
				}
				if (string.IsNullOrEmpty(sign))
				{
					return (false, 4003, "url=" + url + ",缺少参数sign");
				}

				//较验传输数据合法性
				string sign1 = MD5Helper.Sha1(Appsettings.app("Web:Key").ToLower() + timestamp + url.ToLower() + data.ToLower());
				if (sign.ToLower() != sign1.ToLower())
				{
					if (data.Length > 4000)
					{
						_logger.LogInformation("sign=" + sign + "&sign1=" + sign1 + "("+ Appsettings.app("Web:Key").ToLower() + timestamp + url.ToLower() + ")");
					}
					else
					{
						_logger.LogInformation("sign=" + sign + "&sign1=" + sign1 + "("+ Appsettings.app("Web:Key").ToLower() + timestamp + url.ToLower() + data.ToLower() + ")");
					}
					return (false, 4004, "url=" + url + ",签名验证失败");
				}
			}
			return (true, 0, "");
		}

		private async Task<(bool isok, int error_code, string msg)> CheckAudit(HttpContext context)
		{
			var data = _userManage.RequestBody;
			//检测内容安全(开发环境不检测)
			//*************************************************************************
			if (!string.IsNullOrEmpty(data) && !_env.IsDevelopment()) {
				if (Appsettings.app("ContentAudit:IsAudit") == "1")
				{
					var arr = Appsettings.app<string>("ContentAudit:Api");
					if (arr.Any(u => (context.Request.Path.ToString()+ context.Request.QueryString.ToString()).ToLower().Contains(u.ToString().ToLower()+"?")))
					{
						//url = Appsettings.app("APIS:security.msgSecCheck"); 
						//string AccessToken = _weiXinService.UpdateAccessToken();
						//url = string.Format(url, AccessToken);
						//Dictionary<string, object> dic_data = new Dictionary<string, object>();
						//dic_data.Add("content", data);
						//var json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(dic_data), "utf-8", "POST");//调微信接口
						//JObject result = JObject.Parse(json);
						//if (result["errcode"] != null)
						//{
						//	if (result["errcode"].ToString() == "87014")
						//	{
						//		HandleExceptionAsync(context, (int)result["errcode"], "内容含有违法违规内容", userid);
						//		return "";
						//	}
						//	else if (result["errcode"].ToString() != "0")
						//	{
						//		_logger.LogInformation("检测内容安全,提交：" + data + "\r\n返回：" + json);
						//	}
						//}
						//else
						//{
						//	HandleExceptionAsync(context, 4000, "接口异常", userid);
						//	return "";
						//}
					}
				}
			}
			//*************************************************************************
			return (true, 0, "");
		}

		private void HandleExceptionAsync(HttpContext context, int statusCode, string msg,long userid = 0)
		{
			var result = new ApiResult();
			result.StatusCode = statusCode;
			result.Message = msg;
			var data = JsonConvert.SerializeObject(result);
            context.Response.ContentType = "application/json;charset=utf-8";
			context.Response.WriteAsync(data);
			_logger.LogInformation("TokenMiddleware("+ userid +"):" + data);
		}
	}

	public static class TokenMiddlewareExtensions
	{
		public static IApplicationBuilder UseTokenMiddleware(
			this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<TokenMiddleware>();
		}
	}
}