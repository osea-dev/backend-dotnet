using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WeTalk.Common.Helper
{
	public class IpHelper
	{
		/// <summary>
		/// 获取IP地址
		/// </summary>
		/// <param name="preferredNetworks"></param>
		/// <returns></returns>
		public static string GetCurrentIp(HttpContext httpContext,string preferredNetworks = "")
		{
			var instanceIp = "";
			if (httpContext != null)
			{
				instanceIp = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
				if (!string.IsNullOrEmpty(instanceIp)) return instanceIp;

				instanceIp = httpContext.Request.Headers["X-Real-IP"].ToString();
				if (!string.IsNullOrEmpty(instanceIp)) return instanceIp;
				instanceIp = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
				if (!string.IsNullOrEmpty(instanceIp)) return instanceIp;
			}



			if (string.IsNullOrEmpty(instanceIp))
			{
				try
				{
					// 获取可用网卡
					var nics = NetworkInterface.GetAllNetworkInterfaces()?.Where(network => network.OperationalStatus == OperationalStatus.Up);

					// 获取所有可用网卡IP信息
					var ipCollection = nics?.Select(x => x.GetIPProperties())?.SelectMany(x => x.UnicastAddresses);

					foreach (var ipadd in ipCollection)
					{
						if (!IPAddress.IsLoopback(ipadd.Address) && ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							if (string.IsNullOrEmpty(preferredNetworks))
							{
								instanceIp = ipadd.Address.ToString();
								break;
							}

							if (!ipadd.Address.ToString().StartsWith(preferredNetworks)) continue;
							instanceIp = ipadd.Address.ToString();
							break;
						}
					}
				}
				catch
				{
					// ignored
				}
			}
			return instanceIp;
		}

	}
}
