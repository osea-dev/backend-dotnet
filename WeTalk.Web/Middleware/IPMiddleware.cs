using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WeTalk.Common.Helper;

namespace WeTalk.Web.Middleware
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class IPMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;
		public IPMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
		{
			_next = next;
			_logger = loggerFactory.CreateLogger<IPMiddleware>();
		}

		public Task Invoke(HttpContext httpContext)
		{
			Console.WriteLine("Request2");
			_logger.LogInformation($"Client Ip:{IpHelper.GetCurrentIp(httpContext)}");
			return _next(httpContext);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class IPMiddlewareExtensions
	{
		public static IApplicationBuilder UseIP(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<IPMiddleware>();
		}
	}
}