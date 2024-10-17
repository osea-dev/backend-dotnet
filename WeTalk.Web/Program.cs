using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;

namespace WeTalk.Web
{
	public class Program
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
		  Host.CreateDefaultBuilder(args)
			  .UseServiceProviderFactory(new AutofacServiceProviderFactory())
			  .ConfigureLogging(loggingBuilder => {
				  loggingBuilder.AddFilter("System", LogLevel.Warning);
				  loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
				  loggingBuilder.AddLog4Net(Directory.GetCurrentDirectory() + "/Config/log4netcore.config");
			  })
			  .ConfigureWebHostDefaults(webBuilder =>
			  {
				  webBuilder.UseStartup<Startup>().UseUrls("https://localhost:5001");
			  });
	}
}
