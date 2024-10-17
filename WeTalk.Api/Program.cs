using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;

namespace WeTalk.Api
{
	public class Program
	{
		//	public static void Main(string[] args)
		//	{
		//		CreateWebHostBuilder(args).Build().Run();
		//	}

		//	public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
		//		WebHost.CreateDefaultBuilder(args)
		//			.UseStartup<Startup>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			var isok = ThreadPool.SetMinThreads(100, 100);
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
		  Host.CreateDefaultBuilder(args)
			  .UseServiceProviderFactory(new AutofacServiceProviderFactory())
			  .ConfigureLogging(loggingBuilder => {
				  loggingBuilder.AddFilter("System", LogLevel.Warning);
				  loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
				  loggingBuilder.AddLog4Net(Directory.GetCurrentDirectory()+"/Config/log4netcore.config");
			  })
			  .ConfigureWebHostDefaults(webBuilder =>
			  {
				  webBuilder.UseStartup<Startup>();//.UseUrls("http://*:8001")
			  });
	}
}
