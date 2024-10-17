using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WeTalk.Web.Middleware
{
	public class AccountMiddleware
	{
		private readonly RequestDelegate _next;

		/// <summary>
		/// 管道执行到该中间件时候下一个中间件的RequestDelegate请求委托，如果有其它参数，也同样通过注入的方式获得
		/// </summary>
		/// <param name="next"></param>
		public AccountMiddleware(RequestDelegate next)
		{
			//通过注入方式获得对象
			_next = next;
		}

		/// <summary>
		/// 自定义中间件要执行的逻辑
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task Invoke(HttpContext context)
		{

			await _next(context);//把context传进去执行下一个中间件
		}
	}
}