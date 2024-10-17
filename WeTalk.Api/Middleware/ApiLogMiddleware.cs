using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Extensions;
using WeTalk.Interfaces;
using WeTalk.Models;
using WeTalk.Models.Redis;

namespace WeTalk.Api.Middleware
{
	public class ApiLogMiddleware
	{
		private HttpContext _httpcontext;
		private readonly RequestDelegate _next;
		private readonly ILogger<ApiLogMiddleware> _logger;
		private SqlSugarScope _context;

		public const int MaxLength = 40000;

		/// <summary>
		/// 管道执行到该中间件时候下一个中间件的RequestDelegate请求委托，如果有其它参数，也同样通过注入的方式获得
		/// </summary>
		/// <param name="next"></param>
		public ApiLogMiddleware(RequestDelegate next, ILogger<ApiLogMiddleware> logger)
		{
			_logger = logger;

			//通过注入方式获得对象
			_next = next;
		}

		/// <summary>
		/// 自定义中间件要执行的逻辑
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task Invoke(HttpContext httpcontext, SqlSugarScope dbcontext, IUserManage userManage)
		{
			_httpcontext = httpcontext;
			
				_context = dbcontext;
				string Log4netType = Appsettings.app("Logs:ApiLog");
				var list_While = Appsettings.app<string>("Web:LogWhiteList");
				var btime = DateTime.Now;
				if (!string.IsNullOrEmpty(userManage.UserToken)) userManage.GetUserToken();//初始化用户状态

				if (list_While.Any(u => httpcontext.Request.Path.StartsWithSegments(u)))
				{
					Log4netType = "-1";
				}
			if (Log4netType == "-1")
			{
				try
				{
					await _next.Invoke(httpcontext);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "ApiLogMiddleware错误处理,StatusCode=" + _httpcontext.Response.StatusCode);
					var result = new ApiResult();
					result.StatusCode = 4012;
					result.Message = ex.Message;
					var data = JsonConvert.SerializeObject(result);
					_httpcontext.Response.ContentType = "application/json;charset=utf-8";
					await _httpcontext.Response.WriteAsync(data);
				}
				//await _next(context);//把context传进去执行下一个中间件
			}
			else
			{
				//Filter(context);
				httpcontext.Request.EnableBuffering();
				var _logInfo = new RequestResponseLog();

				HttpRequest request = httpcontext.Request;
				if (request.Host.Port != httpcontext.Connection.LocalPort)
				{
					_logInfo.Url = request.Scheme + "://" + request.Host.Host+":"+ httpcontext.Connection.LocalPort + request.Path + request.QueryString;
				}
				else
				{
					_logInfo.Url = request.Scheme + "://" + request.Host + request.Path + request.QueryString;
				}
				_logInfo.Headers = request.Headers.ToDictionary(k => k.Key, v => string.Join(";", v.Value.ToList()).Replace("\"","\'"));
				_logInfo.Method = request.Method;
				_logInfo.ExcuteStartTime = btime;
				_logInfo.IP = IpHelper.GetCurrentIp(httpcontext);

				//获取request.Body内容
				if (request.Method.ToLower().Equals("post"))
				{
					httpcontext.Request.EnableBuffering(); //启用倒带功能，就可以让 Request.Body 可以再次读取

					Stream stream = request.Body;
					byte[] buffer = new byte[request.ContentLength.Value];
					await stream.ReadAsync(buffer, 0, buffer.Length);
					_logInfo.RequestBody = Encoding.UTF8.GetString(buffer);
					request.Body.Position = 0;
				}
				else if (request.Method.ToLower().Equals("get"))
				{
					_logInfo.RequestBody = request.QueryString.Value;
				}

				//获取Response.Body内容
				var originalBodyStream = httpcontext.Response.Body;

				using (var responseBody = new MemoryStream())
				{
					httpcontext.Response.Body = responseBody;

					//_next.Invoke(context).Wait();
					try
					{
						await _next.Invoke(httpcontext);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "ApiLogMiddleware错误处理,StatusCode=" + _httpcontext.Response.StatusCode);
						var result = new ApiResult();
						result.StatusCode = 4012;
						result.Message = ex.Message;
						var data = JsonConvert.SerializeObject(result);
						_httpcontext.Response.ContentType = "application/json;charset=utf-8";
						await _httpcontext.Response.WriteAsync(data);
					}
					_logInfo.ResponseBody = FormatResponse(httpcontext.Response).Result;
					_logInfo.ExcuteEndTime = DateTime.Now;
					_logInfo.Userid = userManage.Userid;

					switch (Log4netType)
					{
						case "0":
							_logger.LogInformation(_logInfo.ToJson());
							break;
						case "1":
							try
							{
								string headers = "[" + string.Join(",", _logInfo.Headers.Select(i => "{" + $"\"{i.Key}\":\"{i.Value}\"" + "}")) + "]";
								if (_logInfo.Url.Length > 500 || headers.Length > 4000 || (_logInfo.ResponseBody != null && _logInfo.ResponseBody.Length > MaxLength) || (_logInfo.RequestBody != null && _logInfo.RequestBody.Length > MaxLength))
								{
									if (!_logInfo.ResponseBody.Contains("{\"ErrorCode\":0,"))
										_logger.LogInformation(_logInfo.ToJson());
								}
								await _context.Insertable(_logInfo.ToModel()).ExecuteCommandAsync();
							}
							catch (Exception ex)
							{
								_logger.LogError(ex, "写入接口日志异常");
								_logger.LogInformation(_logInfo.ToJson());
							}
							break;
					}

					//var text = StackExchange.Profiling.MiniProfiler.Current.RenderPlainText();
					//log.Info(_logInfo.Url + "\r\n" + text);
					await responseBody.CopyToAsync(originalBodyStream);
				}
			}
			//try
			//{
			//    await _next(context);//把context传进去执行下一个中间件
			//}
			//catch (Exception ex) {
			//    Console.WriteLine(ex.Message);
			//}
			
		}


		/// <summary>
		/// IP与Token检验
		/// </summary>
		/// <param name="context"></param>
		private void Filter(HttpContext context)
		{


		}

		private async Task<string> FormatResponse(HttpResponse response)
		{
			response.Body.Seek(0, SeekOrigin.Begin);
			var text = await new StreamReader(response.Body).ReadToEndAsync();
			response.Body.Seek(0, SeekOrigin.Begin);

			return text;
		}

		/// <summary>
		/// 防止并发请求处理
		/// </summary>
		/// <param name="t">间隔毫秒数</param>
		public void NoConcurrency(int t, string url, string query, string post_data)
		{
			string md5 = MD5Helper.MD5Encrypt32(url + query + post_data);
			if (RedisServer.Cache.Exists(md5))
			{
				//直接返回值
				var result = JsonConvert.SerializeObject(RedisServer.Cache.Get(md5));
				_httpcontext.Response.ContentType = "application/json;charset=utf-8";
				_httpcontext.Response.WriteAsync(result);
			}
			else
			{
				//写入redis
				RedisServer.Cache.Set(md5, "");
			}
		}
	}

	public class RequestResponseLog
	{
		public long Userid { get; set; }
		public string Url { get; set; }
		public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
		public string IP { get; set; }
		public string Method { get; set; }
		public string RequestBody { get; set; }
		public string ResponseBody { get; set; }
		public DateTime ExcuteStartTime { get; set; }
		public DateTime ExcuteEndTime { get; set; }
		public RequestResponseLog()
		{
		}

		public string ToJson()
		{
			string headers = "[" + string.Join(",", this.Headers.Select(i => "{" + $"\"{i.Key}\":\"{i.Value}\"" + "}")) + "]";

			if ((this.RequestBody + "").Contains("\"filebase64\""))
			{
				try
				{
					JObject o = JObject.Parse(this.RequestBody);
					if (o["filebase64"] != null) o["filebase64"] = o["filebase64"].ToString().Length;
					this.RequestBody = WebUtility.UrlDecode(JsonConvert.SerializeObject(o));
				}
				catch (Exception ex)
				{
					this.RequestBody = "解析RequestBody异常，" + ex.Message;
				}
			}
			return $"\r\nIP:{this.IP},\r\nUrl: {this.Url},\r\nHeaders: {headers},\r\nMethod: {this.Method},\r\nRequestBody: {WebUtility.UrlDecode(this.RequestBody)},\r\nResponseBody: {this.ResponseBody},\r\nExcuteStartTime: {this.ExcuteStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")},\r\nExcuteEndTime: {this.ExcuteEndTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}";
		}

		public Logs ToModel()
		{
			//改用异步写入数据库
			string headers = "[" + string.Join(",", this.Headers.Select(i => "{" + $"\"{i.Key}\":\"{i.Value}\"" + "}")) + "]";
			var model_Logs = new Logs();
			model_Logs.Method = this.Method;

			if ((this.RequestBody + "").Contains("\"filebase64\""))
			{
				try
				{
					JObject o = JObject.Parse(this.RequestBody);
					if (o["filebase64"] != null) o["filebase64"] = o["filebase64"].ToString().Length;
					model_Logs.Requestbody = WebUtility.UrlDecode(JsonConvert.SerializeObject(o));
				}
				catch (Exception ex)
				{
					model_Logs.Requestbody = "解析RequestBody异常，" + ex.Message;
				}
			}
			else if (this.RequestBody != null && this.RequestBody.Length > ApiLogMiddleware.MaxLength)
			{
				model_Logs.Requestbody = "详情查看文本日志(" + this.RequestBody.Length + ")";
			}
			else
			{
				model_Logs.Requestbody = WebUtility.UrlDecode(this.RequestBody);
			}
			if (this.ResponseBody != null && this.ResponseBody.Length >= ApiLogMiddleware.MaxLength)
			{
				model_Logs.Responsebody = "详情查看文本日志(" + this.RequestBody.Length + ")";
			}
			else
			{
				model_Logs.Responsebody = this.ResponseBody;
			}
			if (this.Url.Length > 500)
			{
				model_Logs.Url = "详情查看文本日志";
			}
			else
			{
				model_Logs.Url = Url;
			}
			if (headers.Length > 4000)
			{
				model_Logs.Headers = "详情查看文本日志";
			}
			else
			{
				model_Logs.Headers = headers;
			}
			model_Logs.Excutestarttime = DateHelper.ToUnixTimestampByMilliseconds(this.ExcuteStartTime);
			model_Logs.Excuteendtime = DateHelper.ToUnixTimestampByMilliseconds(this.ExcuteEndTime);
			model_Logs.Ip = this.IP;
			model_Logs.Userid = this.Userid;
			if (this.Headers.Any(u => u.Key.ToLower().Contains("x-forwarded-for")))
			{
				//如果使用CDN
				model_Logs.Ip = this.Headers.FirstOrDefault(u => u.Key.ToLower() == "x-forwarded-for").Value;
			}
			return model_Logs;
			//return $"Url: {this.Url},\r\nHeaders: {headers},\r\nMethod: {this.Method},\r\nRequestBody: {WebUtility.UrlDecode(this.RequestBody)},\r\nResponseBody: {this.ResponseBody},\r\nExcuteStartTime: {this.ExcuteStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")},\r\nExcuteEndTime: {this.ExcuteEndTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}";
		}

	}
}