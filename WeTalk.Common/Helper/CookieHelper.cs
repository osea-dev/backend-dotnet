using Microsoft.AspNetCore.Http;
using System;

namespace WeTalk.Common.Helper
{
	public class CookieHelper
	{
		public CookieHelper()
		{
		}
		/// <summary>
		/// 添加cookie缓存设置过期时间
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="time">分钟</param>
		public static void SetCookie(HttpContext httpContext, string key, string value, int time = 120)
		{
			httpContext.Response.Cookies.Append(key, value, new CookieOptions
			{
				HttpOnly = true,//后台只读模式，前端 无法通过JS获取COOKIE值 
				Secure = true,//只在https下生效
				SameSite = SameSiteMode.None,//设置为none就必需设置Secure
				Expires = DateTimeOffset.Now.AddMinutes(time),
				IsEssential = true    //强制写COOKIE
			});
			//存COOKIES
			//HttpContext.Response.Cookies.Append("AdminName", _httpContext.Session.GetString("AdminName").Trim(), new CookieOptions()
			//{
			//	HttpOnly = true,//设置为后台只读模式,前端无法通过JS来获取cookie值,可以有效的防止XXS攻击
			//	Secure = true,  //采用安全模式来传递cookie,如果设置为true,就是当你的网站开启了SSL(就是https),的时候,这个cookie值才会被传递
			//	MaxAge = TimeSpan.FromMinutes(1440),  //cookie的有效毫秒数,如果设置为负值的话，则为浏览器进程Cookie(内存中保存)，关闭浏览器就失效；如果设置为0，则立即删除该Cookie。
			//	IsEssential = true  //是否强制存储cookie,注意,这里的强制 是针对于上面所讲的内容的..也就是当用户不同意使用cookie的时候,你也可以通过设置这个属性为true把cookie强制存储.
			//});

		}
		public static void SetCookie(IHttpContextAccessor accessor, string key, string value, int time = 120)
		{
			accessor.HttpContext.Response.Cookies.Append(key, value, new CookieOptions
			{
				HttpOnly = true,//后台只读模式，前端 无法通过JS获取COOKIE值 
				Secure = true,//只在https下生效
				SameSite = SameSiteMode.None,//设置为none就必需设置Secure
				Expires = DateTimeOffset.Now.AddMinutes(time),
				IsEssential = true    //强制写COOKIE
			});
			//存COOKIES
			//HttpContext.Response.Cookies.Append("AdminName", _httpContext.Session.GetString("AdminName").Trim(), new CookieOptions()
			//{
			//	HttpOnly = true,//设置为后台只读模式,前端无法通过JS来获取cookie值,可以有效的防止XXS攻击
			//	Secure = true,  //采用安全模式来传递cookie,如果设置为true,就是当你的网站开启了SSL(就是https),的时候,这个cookie值才会被传递
			//	MaxAge = TimeSpan.FromMinutes(1440),  //cookie的有效毫秒数,如果设置为负值的话，则为浏览器进程Cookie(内存中保存)，关闭浏览器就失效；如果设置为0，则立即删除该Cookie。
			//	IsEssential = true  //是否强制存储cookie,注意,这里的强制 是针对于上面所讲的内容的..也就是当用户不同意使用cookie的时候,你也可以通过设置这个属性为true把cookie强制存储.
			//});

		}
		/// <summary>
		/// 根据键获取对应的cookie
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetCookie(HttpContext httpContext, string key, int time = 0)
		{
			var value = "";
			httpContext.Request.Cookies.TryGetValue(key, out value);
			if (string.IsNullOrWhiteSpace(value))
			{
				value = string.Empty;
				if (time > 0) SetCookie(httpContext, key, value, time);
			}
			return value;
		}
		public static string GetCookie(IHttpContextAccessor accessor, string key, int time = 0)
		{
			var value = "";
			accessor.HttpContext.Request.Cookies.TryGetValue(key, out value);
			if (string.IsNullOrWhiteSpace(value))
			{
				value = string.Empty;
				if (time > 0) SetCookie(accessor, key, value, time);
			}
			return value;
		}

		/// <summary>
		/// 刷新cookie过期时长
		/// </summary>
		/// <param name="key"></param>
		/// <param name="time"></param>
		public static void UpdateCookie(HttpContext httpContext, string key, int time = 120)
		{
			var value = GetCookie(httpContext, key);
			SetCookie(httpContext, key, value, time);
		}


		/// <summary>
		/// 删除cookie缓存
		/// </summary>
		/// <param name="key"></param>
		public static void DeleteCookie(HttpContext httpContext, string key)
		{
			httpContext.Response.Cookies.Delete(key);
		}

	}
}
