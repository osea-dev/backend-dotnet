using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WeTalk.Common.Helper;

namespace WeTalk.Api.Middleware
{
	/// <summary>
	/// IP白名单
	/// </summary>
	public class AdminSafeListMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<AdminSafeListMiddleware> _logger;
		private readonly string _safelist;

		public AdminSafeListMiddleware(
			RequestDelegate next,
			ILogger<AdminSafeListMiddleware> logger,
			string safelist)
		{
			_safelist = safelist;
			_logger = logger;
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			//context.Request.Headers.Add("Access-Control-Allow-Headers", "*");
			if (_safelist != "*")
			{
				var remoteIp = context.Connection.RemoteIpAddress;
				//_logger.LogDebug("来自远程IP地址的请求: {RemoteIp}", remoteIp);
				try
				{
					string[] ip = _safelist.Split(';');

					var bytes = remoteIp.GetAddressBytes();
					var badIp = true;
					foreach (var address in ip)
					{
						if (!string.IsNullOrEmpty(address))
						{
							var testIp = IPAddress.Parse(address);
							if (testIp.GetAddressBytes().SequenceEqual(bytes))
							{
								badIp = false;
								break;
							}
						}
					}

					if (badIp)
					{
						//_logger.LogWarning("来自远程IP地址的禁止请求: {RemoteIp}", remoteIp);
						_logger.LogInformation("来自远程IP地址的禁止请求:" + remoteIp);
						context.Response.StatusCode = StatusCodes.Status403Forbidden;
						return;
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "IP策略异常[" + remoteIp + "]:" );
				}
			}

			await _next.Invoke(context);
		}
	}
}