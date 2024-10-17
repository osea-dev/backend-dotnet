using Autofac;
using Autofac.Extensions.DependencyInjection;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using WeTalk.Common;
using WeTalk.Console.CMD;
using WeTalk.Console.Filter;
using WeTalk.Extensions;
using WeTalk.Interfaces;

namespace WeTalk.Console
{
    class Program
	{
		public static ILoggerRepository repository { get; set; }//log4net日志
		public static ICacheService myCache;
		static void Main(string[] args)
		{

			System.Console.WriteLine(MiniProfiler.Current.RenderPlainText());

			var builder = new HostBuilder()
			 .UseServiceProviderFactory(new AutofacServiceProviderFactory())
			 .ConfigureContainer<ContainerBuilder>(builders =>
			 {
				 builders.RegisterModule(new AutofacModuleRegister());
				 builders.RegisterModule<AutofacPropertityModuleReg>();
			 })
			 .ConfigureLogging(loggingBuilder => {
                 loggingBuilder.AddFilter("System", LogLevel.Warning);
                 loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                 loggingBuilder.AddLog4Net(Directory.GetCurrentDirectory() + "/Config/log4netcore.config");
             })
             .ConfigureServices((context, services) =>
			 {
				 // 支持中文编码
				 Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

				 //string Path = "C:/website/StrangeAuction_Test/Console/123456.txt";
				 //log.Info(AppDomain.CurrentDomain.BaseDirectory + "//appsettings.json");

				 //return;

				 //配置文件
				 var builder1 = new ConfigurationBuilder()
				 .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				 .AddJsonFile("//appsettings.json", optional: true, reloadOnChange: true)
				 .AddJsonFile($"//appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", true, true)
				 .AddEnvironmentVariables();
				 IConfiguration config = builder1.Build();
				 IConfigurationRoot configuration = builder1.Build();

                 //config["Web:AppSty"]
                 //依赖注入
                 //IServiceCollection services = new ServiceCollection();
                 //全局静态调用_config
                 services.AddSingleton(new Appsettings(config));

				 services.AddOptions();

				 //日志
				 repository = LogManager.CreateRepository("AprilLog");
				 XmlConfigurator.Configure(repository, new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Config/log4net.config"));//配置文件路径可以自定义
				 BasicConfigurator.Configure(repository);

				 services.AddSingleton<IConfiguration>(configuration);

				 //mysql
				 //var connectionString = configuration.GetConnectionString("StrangeAuctionConnection");
				 //ServerVersion serverVersion = ServerVersion.AutoDetect(connectionString);
				 //services.AddDbContextPool<uecardcontext>(options =>
				 //	options.UseMySql(connectionString, serverVersion));

				 var log = LogManager.GetLogger(repository.Name, typeof(Program));
				 services.AddSqlsugarSetup(log);

                 //设置语言包文件夹名称
                 services.AddLocalization(options => options.ResourcesPath = "Resources");
                 services.Configure<RequestLocalizationOptions>(options =>
                 {
					 var supportedCultures = new[]
                     {
                        new CultureInfo("zh-cn"),
                        new CultureInfo("en")
					 };
                     options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures[0], uiCulture: supportedCultures[0]);
                     options.SupportedCultures = supportedCultures;
                     options.SupportedUICultures = supportedCultures;
                 });
				 services.AddSingleton<IStringLocalizer>((sp) =>
                 {
                     var sharedLocalizer = sp.GetRequiredService<IStringLocalizer<LangResource>>();
                     return sharedLocalizer;
                 });

                 ////注册Redis
                 //services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")));

                 //注册REDIS 服务
                 RedisServer.Initalize();

				 services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
				 services.AddSingleton<CmdHostService>();
				 services.AddAutofac();
			 })
			 .Build();
            var cmdHostService = builder.Services.GetService<CmdHostService>();
            cmdHostService.Run().Wait();
			//builder.Run();

		}
	}
}
