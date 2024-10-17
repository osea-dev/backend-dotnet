using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace WeTalk.Common.Helper
{
	// <summary>
	// Request操作类
	// </summary>
	public static class GSRequestHelper
	{

		// <summary>
		// 得到主机头
		// www.xxx.com
		// </summary>
		// <returns></returns>        
		public static string GetHost(this HttpRequest req)
		{
			return req.Host.ToString();
		}
		//GetHost

		// <summary>
		// 判断当前页面是否接收到了Post请求
		// </summary>
		// <returns>是否接收到了Post请求</returns>
		public static bool IsPost(this HttpRequest req)
		{
			return req.IsPost();
		}
		//IsPost

		// <summary>
		// 判断当前页面是否接收到了Get请求
		// </summary>
		// <returns>是否接收到了Get请求</returns>
		public static bool IsGet(this HttpRequest req)
		{
			return req.IsGet();
		}
		//IsGet

		/// <summary>
		/// 判断当前页面是否ajax请求
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public static bool IsAjax(this HttpRequest req)
		{
			bool result = false;
			result = req.IsAjax();
			return result;
		}

		// <summary>
		// 返回指定的服务器变量信息
		// </summary>
		// <param name="strName">服务器变量名</param>
		// <returns>服务器变量信息</returns>
		public static string GetServerString(this HttpContext context, string strName)
		{
			if (context == null) return null;
			var heards = context.Request.Headers[strName];
			if (string.IsNullOrEmpty(heards))
			{
				return "";
			}
			else
			{
				return heards.ToString();
			}
		}
		//GetServerString


		// <summary>
		// 返回上一个页面的地址
		// </summary>
		// <returns>上一个页面的地址</returns>
		public static string GetUrlReferrer(this HttpContext context)
		{
			if (context == null) return null;
			//using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;
			//var referer = ((FrameRequestHeaders)Request.Headers).HeaderReferer.FirstOrDefault();
			string retVal = null;
			try
			{
				retVal = context.Request.Headers["Referer"].ToString(); ;
			}
			catch
			{
			}
			if (retVal == null)
			{
				return "";
			}
			return retVal;
		}
		//GetUrlReferrer


		// <summary>
		// 得到当前完整主机头,www.xxx.com:81
		// </summary>
		// <returns></returns>
		public static string GetFullHost(this HttpRequest req)
		{
			if (req.Host.Port != null)
			{
				return string.Format("{0}:{1}", req.Host, req.Host.Port.ToString());
			}
			else
			{
				return req.Host.ToString();
			}
		}
		//GetFullHost




		// <summary>
		// 获取当前请求的原始 URL(/list/unsolved?page=3)
		// </summary>
		// <returns>原始 URL</returns>
		public static string GetRawUrl(this HttpRequest req)
		{
			return $"{req.Path}{req.QueryString}";
		}


		//GetRawUrl



		// <summary>
		// 判断是否来自搜索引擎链接
		// </summary>
		// <returns>是否来自搜索引擎链接</returns>
		public static bool IsSearchEnginesGet(this HttpRequest req)
		{
			string referrer = GetUrlReferrer(req.HttpContext);
			if (string.IsNullOrEmpty(referrer)) return false;
			string[] SearchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou", "bing" };
			string tmpReferrer = referrer.ToLower();
			int i;
			for (i = 0; i <= SearchEngine.Length - 1; i++)
			{
				if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
				{
					return true;
				}
			}
			return false;
		}
		//IsSearchEnginesGet


		// <summary>
		// 获得当前完整Url地址
		// </summary>
		// <returns>当前完整Url地址</returns>
		public static string GetUrl(this HttpRequest request)
		{
			var scheme = request.Scheme ?? string.Empty;
			var host = request.Host.Value ?? string.Empty;
			var pathBase = request.PathBase.Value ?? string.Empty;
			var path = request.Path.Value ?? string.Empty;
			var queryString = request.QueryString.Value ?? string.Empty;

			return scheme + "://" + host + pathBase + path + queryString;
		}

		public static string GetRequestUrl(this HttpRequest source)
		{
			return $"{source.PathBase}{source.Path}{source.QueryString}";
		}
		//GetUrl

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static string GetHostUrl(this HttpRequest request)
		{
			var scheme = request.Scheme ?? string.Empty;
			var host = request.Host.Value ?? string.Empty;
			var pathBase = request.PathBase.Value ?? string.Empty;
			return scheme + "://" + host + pathBase;
		}



		// <summary>
		// 获得指定Url参数的值
		// </summary>
		// <param name="strName">Url参数</param>
		// <returns>Url参数的值</returns>
		public static string GetQueryString(this HttpRequest req, string strName)
		{
			return req.Query[strName].ToString();
		}
		//GetQueryString


		// <summary>
		// 获得当前页面的名称
		// </summary>
		// <returns>当前页面的名称</returns>
		public static string GetPageName(this HttpRequest req)
		{
			string[] urlArr = req.GetUrl().Split(char.Parse("/"));
			return urlArr[(urlArr.Length - 1)].ToLower();
		}
		//GetPageName


		// <summary>
		// 返回表单或Url参数的总个数
		// </summary>
		// <returns></returns>
		public static int GetParamCount(this HttpRequest req)
		{
			int count = 0;
			if (req.HasFormContentType) count += req.Form.Count;
			count += req.Query.Count;
			return count;
		}
		//GetParamCount



		// <summary>
		// 获得指定表单参数的值
		// </summary>
		// <param name="strName">表单参数</param>
		// <returns>表单参数的值</returns>
		public static string GetFormString(this HttpRequest req, string strName)
		{
			if (req.HasFormContentType && !string.IsNullOrEmpty(req.Form[strName]))
			{
				return req.Form[strName];
			}
			else
			{
				return "";
			}
		}
		//GetFormString


		// <summary>
		// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
		// </summary>
		// <param name="strName">参数</param>
		// <returns>Url或表单参数的值</returns>
		public static string GetString(this HttpRequest req, string strName)
		{
			if (GetQueryString(req, strName).ToString().Trim() == "")
			{
				return GetFormString(req, strName);
			}
			else
			{
				return GetQueryString(req, strName);
			}
		}
		//GetString



		// <summary>
		// 获得指定Url参数的int类型值
		// </summary>
		// <param name="strName">Url参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>Url参数的int类型值</returns>
		public static int GetQueryInt(this HttpRequest req, string strName, int defValue)
		{
			//Return Utils.StrToInt(req.QueryString[strName], defValue)
			if (!string.IsNullOrEmpty(req.Query[strName]))
			{
				string str = req.Query[strName].ToString();
				if (str.Length > 0 & str.Length <= 11 & Regex.IsMatch(str, "^[-]?[0-9]*$"))
				{
					if ((str.Length < 10) | (str.Length == 10 & str.Substring(0, 1) == "1"))
					{
						return Convert.ToInt32(str);
					}
					else if (str.Length > 1 & str.Length <= 11 & Regex.IsMatch(str, "^[-]?[0-9]*$"))
					{
						if (str.Length == 11 & str.Substring(0, 1) == "-" & str.Substring(1, 1) == "1") return Convert.ToInt32(str);
					}
				}
			}
			return defValue;
		}
		//GetQueryInt

		// <summary>
		// 获得指定Url参数的long类型值
		// </summary>
		// <param name="strName">Url参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>Url参数的long类型值</returns>
		public static long GetQueryLong(this HttpRequest req, string strName, long defValue)
		{
			//Return Utils.StrToLong(req.QueryString[strName], defValue)
			if (!string.IsNullOrEmpty(req.Query[strName]))
			{
				string str = req.Query[strName].ToString();
				if (str.Length > 0 & str.Length <= 11 & Regex.IsMatch(str, "^[-]?[0-9]*$"))
				{
					if ((str.Length < 10) | (str.Length == 10 & str.Substring(0, 1) == "1"))
					{
						return Convert.ToInt64(str);
					}
					else if (str.Length > 1 & str.Length <= 11 & Regex.IsMatch(str, "^[-]?[0-9]*$"))
					{
						if (str.Length == 11 & str.Substring(0, 1) == "-" & str.Substring(1, 1) == "1") return Convert.ToInt64(str);
					}
				}
			}
			return defValue;
		}
		//GetQueryLong


		// <summary>
		// 获得指定表单参数的int类型值
		// </summary>
		// <param name="strName">表单参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>表单参数的int类型值</returns>
		public static int GetFormInt(this HttpRequest req, string strName, int defValue)
		{
			if (req.HasFormContentType && !string.IsNullOrEmpty(req.Form[strName]))
			{
				string str = req.Form[strName].ToString();
				if (str.Length > 0 & str.Length <= 11 & Regex.IsMatch(str, "^[-]?[0-9]*$"))
				{
					if ((str.Length < 10) | (str.Length == 10 & str.Substring(0, 1) == "1"))
					{
						return Convert.ToInt32(str);
					}
					else if (str.Length > 1 & str.Length <= 11 & Regex.IsMatch(str, "^[-]?[0-9]*$"))
					{
						if ((str.Length == 11 & str.Substring(0, 1) == "-" & str.Substring(1, 1) == "1")) return Convert.ToInt32(str);
					}
				}
			}
			return defValue;
		}
		//GetFormInt

		// <summary>
		// 获得指定表单参数的long类型值
		// </summary>
		// <param name="strName">表单参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>表单参数的long类型值</returns>
		public static long GetFormLong(this HttpRequest req, string strName, long defValue)
		{
			if (req.HasFormContentType && !string.IsNullOrEmpty(req.Form[strName]))
			{
				string str = req.Form[strName].ToString();
				if (str.Length > 0 & str.Length <= 11 & Regex.IsMatch(str, "^[-]?[0-9]*$"))
				{
					if ((str.Length < 10) | (str.Length == 10 & str.Substring(0, 1) == "1"))
					{
						return Convert.ToInt64(str);
					}
					else if (str.Length > 1 & str.Length <= 11 & Regex.IsMatch(str, "^[-]?[0-9]*$"))
					{
						if ((str.Length == 11 & str.Substring(0, 1) == "-" & str.Substring(1, 1) == "1")) return Convert.ToInt64(str);
					}
				}
			}
			return defValue;
		}
		//GetFormInt


		// <summary>
		// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
		// </summary>
		// <param name="strName">Url或表单参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>Url或表单参数的int类型值</returns>
		public static int GetInt(this HttpRequest req, string strName, int defValue)
		{
			if (GetQueryInt(req, strName, defValue) == defValue)
			{
				return GetFormInt(req, strName, defValue);
			}
			else
			{
				return GetQueryInt(req, strName, defValue);
			}
		}
		//GetInt


		// <summary>
		// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
		// </summary>
		// <param name="strName">Url或表单参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>Url或表单参数的int类型值</returns>
		public static long GetLong(this HttpRequest req, string strName, long defValue)
		{
			if (GetQueryLong(req, strName, defValue) == defValue)
			{
				return GetFormLong(req, strName, defValue);
			}
			else
			{
				return GetQueryLong(req, strName, defValue);
			}
		}
		//GetInt


		// <summary>
		// 获得指定Url参数的float类型值
		// </summary>
		// <param name="strName">Url参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>Url参数的int类型值</returns>
		public static float GetQueryFloat(this HttpRequest req, string strName, float defValue)
		{
			//Return Utils.StrToFloat(req.QueryString[strName], defValue)
			if (!string.IsNullOrEmpty(req.Query[strName]))
			{
				if (Regex.IsMatch(req.Query[strName].ToString(), "^([0-9])[0-9]*(\\.\\w*)?$"))
				{
					return float.Parse(req.Query[strName].ToString().Trim());
				}
				else
				{
					return 0;
				}
			}
			return 0;
		}
		//GetQueryFloat


		// <summary>
		// 获得指定Url参数的decimal类型值
		// </summary>
		// <param name="strName">Url参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>Url参数的int类型值</returns>
		public static decimal GetQueryDecimal(this HttpRequest req, string strName, decimal defValue)
		{
			//Return Utils.StrToFloat(req.QueryString[strName], defValue)
			if (!string.IsNullOrEmpty(req.Query[strName]))
			{
				if (Regex.IsMatch(req.Query[strName].ToString(), "^([0-9])[0-9]*(\\.\\w*)?$"))
				{
					return decimal.Parse(req.Query[strName].ToString().Trim());
				}
				else
				{
					return 0;
				}
			}
			return 0;
		}
		//GetQueryFloat



		// <summary>
		// 获得指定表单参数的float类型值
		// </summary>
		// <param name="strName">表单参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>表单参数的float类型值</returns>
		public static float GetFormFloat(this HttpRequest req, string strName, float defValue)
		{
			if (req.HasFormContentType && !string.IsNullOrEmpty(req.Form[strName]))
			{
				if (Regex.IsMatch(req.Form[strName].ToString(), "^([0-9])[0-9]*(\\.\\w*)?$"))
				{
					return float.Parse(req.Form[strName].ToString().Trim());
				}
				else
				{
					return 0;
				}
			}
			return 0;
		}
		//GetFormFloat



		// <summary>
		// 获得指定表单参数的float类型值
		// </summary>
		// <param name="strName">表单参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>表单参数的float类型值</returns>
		public static decimal GetFormDecimal(this HttpRequest req, string strName, decimal defValue)
		{
			if (req.HasFormContentType && !string.IsNullOrEmpty(req.Form[strName]))
			{
				if (Regex.IsMatch(req.Form[strName].ToString(), "^([0-9])[0-9]*(\\.\\w*)?$"))
				{
					return decimal.Parse(req.Form[strName].ToString().Trim());
				}
				else
				{
					return 0;
				}
			}
			return 0;
		}
		//GetFormFloat


		// <summary>
		// 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
		// </summary>
		// <param name="strName">Url或表单参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>Url或表单参数的int类型值</returns>
		public static float GetFloat(this HttpRequest req, string strName, float defValue)
		{
			if (GetQueryFloat(req, strName, defValue) == defValue)
			{
				return GetFormFloat(req, strName, defValue);
			}
			else
			{
				return GetQueryFloat(req, strName, defValue);
			}
		}
		//GetFloat

		// <summary>
		// 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
		// </summary>
		// <param name="strName">Url或表单参数</param>
		// <param name="defValue">缺省值</param>
		// <returns>Url或表单参数的int类型值</returns>
		public static decimal GetDecimal(this HttpRequest req, string strName, decimal defValue)
		{
			if (GetQueryDecimal(req, strName, defValue) == defValue)
			{
				return GetFormDecimal(req, strName, defValue);
			}
			else
			{
				return GetQueryDecimal(req, strName, defValue);
			}
		}
		//GetFloat


		// <summary>
		// 获得当前页面客户端的IP
		// </summary>
		// <returns>当前页面客户端的IP</returns>


		public static string GetIP(this HttpContext context)
		{
			if (context == null) return null;
			var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
			if (string.IsNullOrEmpty(ip))
			{
				ip = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
			}
			return ip;
		}
		//GetIP


		public static bool IsIP4(string ip)
		{
			return Regex.IsMatch(ip, "^((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$");
		}


		public static bool IsIPSect(string ip)
		{
			return Regex.IsMatch(ip, "^((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){2}((2[0-4]\\d|25[0-5]|[01]?\\d\\d?|\\*)\\.)(2[0-4]\\d|25[0-5]|[01]?\\d\\d?|\\*)$");
		}
	}
	//DSRequest 
}
