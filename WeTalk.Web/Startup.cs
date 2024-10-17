using Autofac;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;
using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.IO;
using UEditorNetCore;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Extensions;
using WeTalk.Web;
using WeTalk.Web.Areas.Admin_WeTalk.Controllers;
using WeTalk.Web.Extensions;
using WeTalk.Web.Services;

namespace WeTalk
{
	public class Startup
	{
		public static ICacheService myCache;
		public readonly IConfiguration _configuration;
		public readonly IWebHostEnvironment _env;
		public static ILoggerRepository repository { get; set; }//log4net日志
		private ILog log;

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			_configuration = configuration;
			_env = env;
			//Log4Net
			repository = LogManager.CreateRepository("AprilLog");
			XmlConfigurator.Configure(repository, new FileInfo("Config/log4net.config"));//配置文件路径可以自定义
			BasicConfigurator.Configure(repository);
			log = LogManager.GetLogger(repository.Name, typeof(Startup));
		}
		public void ConfigureServices(IServiceCollection services)
		{
			//全局静态调用_config
			var basePath = AppContext.BaseDirectory;
			services.AddSingleton(new Appsettings(basePath));

			////服务器缓存
			//services.AddMemoryCacheSetup();


			//跨域设置
			services.AddCors(options =>
			{
				// CorsPolicy 是自訂的 Policy 名稱
				options.AddPolicy("CorsPolicy", policy =>
				{
					policy.SetPreflightMaxAge(TimeSpan.FromSeconds(1800L));//update by jason
					policy.WithOrigins(_configuration.GetSection("Cors:Origins").Get<string[]>())
						  .AllowCredentials()
						  .AllowAnyHeader()
						  .AllowAnyMethod();
				});
			});

			//MiniProfiler性能监测
			services.AddMiniProfiler(options =>
			{
				options.PopupRenderPosition = RenderPosition.BottomLeft;
				options.PopupShowTimeWithChildren = true;
				options.RouteBasePath = "/profiler";
			});

			////Swagger接口文档
			//services.AddSwaggerSetup();

			//HttpContext 相关服务
			//services.AddHttpContextSetup();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


			//注入 TokenManager
			services.AddScoped<TokenManager>();

			////注册全局过滤器
			//Action<MvcOptions> filters = new Action<MvcOptions>(r => {
			//	r.Filters.Add(typeof(MyAuthorization));	//授权
			//	r.Filters.Add(typeof(MyExceptionFilterAttribute));	//排错
			//	r.Filters.Add(typeof(MyResourceFilterAttribute));	//资源
			//	r.Filters.Add(typeof(MyActionFilterAttribute));
			//	r.Filters.Add(typeof(MyResultFilterAttribute));
			//});
			//services.AddMvc(filters) 
			//	.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			//services.Configure<CookiePolicyOptions>(options =>
			//{
			//	// This lambda determines whether user consent for non-essential cookies is needed for a given request.
			//	options.CheckConsentNeeded = context => false;//设置为false.或者删除这一段代码,那么我们的 cookie就能正常的使用了
			//	options.MinimumSameSitePolicy = SameSiteMode.None;
			//});
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(config=>config.LoginPath="/Admin_WeTalk/Login");
			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
			});


			services.AddScoped<IPublibService, PublibService>();

			//设置语言包文件夹名称
			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services.AddControllersWithViews(setup =>
			{
                setup.ReturnHttpNotAcceptable = true;//Accept填错，不返回值 
				//setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());//添加XML响应格式，默认是JSON(旧的写法，输出和输入要分开加，新的全包含)
			}).AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ContractResolver = new DefaultContractResolver();
				options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;// 如字段为null值，该字段不会返回到前端
				//options.SerializerSettings.ContractResolver = new DefaultContractResolver();// 不使用驼峰				
				options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";// 设置时间格式
			}).AddXmlDataContractSerializerFormatters()
			.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)//视图使用的语言包
			.AddDataAnnotationsLocalization(options =>
			{
				options.DataAnnotationLocalizerProvider = (type, factory) =>
					factory.Create(typeof(LangResource));//使用集中的公共语言包，如果要按类一一对应区分开，则把options参数全去掉
			})
			.AddRazorRuntimeCompilation();//NuGet:Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation,可以有条件的设置部份库为“运行时编译”，后继补上
			
			services.AddAntiforgery(options =>
			{
				//options.Cookie = new CookieBuilder {
				//    Name = "abc"
				//};
				options.FormFieldName = "AntiforgeryFieldname";//防伪系统用于在视图中呈现防伪标记的隐藏窗体字段的名称。
				options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";//防伪系统使用的标头的名称。 如果 null，系统只考虑窗体数据。
				options.SuppressXFrameOptionsHeader = false;//指定是否取消生成 X-Frame-Options 标头。 默认情况下，会生成一个值为 "SAMEORIGIN" 的标头。 默认为 false。
			});
			services.Configure<IISOptions>(options =>
			{
				options.ForwardClientCertificate = false;
			});

			//services.AddControllers();

			//注册REDIS 服务
			RedisServer.Initalize();

			//数据库
			services.AddSqlsugarSetup(log);
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //test
			services.AddUEditorService();//百度编辑器


			services.Configure<RequestLocalizationOptions>(options =>
			{
				var supportedCultures = new[] { "zh-cn", "en"};
				options.SetDefaultCulture(supportedCultures[0])
					.AddSupportedCultures(supportedCultures)
					.AddSupportedUICultures(supportedCultures);
			});


			//services.MystiqueSetup(_configuration);
			//图片压缩工具
			services.AddImageSharp(options =>
			{
				options.Configuration = SixLabors.ImageSharp.Configuration.Default;
				options.BrowserMaxAge = System.TimeSpan.FromDays(7);
				options.CacheMaxAge = System.TimeSpan.FromDays(365);
				options.CachedNameLength = 8;
				options.OnParseCommandsAsync = _ => System.Threading.Tasks.Task.CompletedTask;
				options.OnBeforeSaveAsync = _ => System.Threading.Tasks.Task.CompletedTask;
				options.OnProcessedAsync = _ => System.Threading.Tasks.Task.CompletedTask;
				options.OnPrepareResponseAsync = _ => System.Threading.Tasks.Task.CompletedTask;
			})
			  .SetRequestParser<QueryCollectionRequestParser>()
			  .Configure<PhysicalFileSystemCacheOptions>(options =>
			  {
				  options.CacheFolder = "is-cache";
			  })
			  .SetCache(provider =>
			  {
				  return new PhysicalFileSystemCache(
							  provider.GetRequiredService<IOptions<PhysicalFileSystemCacheOptions>>(),
							  provider.GetRequiredService<IWebHostEnvironment>(),
							  provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>(),
							  provider.GetRequiredService<FormatUtilities>());
			  })
			  .SetCacheHash<CacheHash>()
			  .AddProvider<PhysicalFileSystemProvider>()
			  .AddProcessor<ResizeWebProcessor>()
			  .AddProcessor<FormatWebProcessor>()
			  .AddProcessor<BackgroundColorWebProcessor>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			//loggerFactory.AddLog4Net(Directory.GetCurrentDirectory() + "\\Config\\log4net.config");


			// CORS跨域
			app.UseCors("CorsPolicy");

			////强制HTTPS
			//app.UseHttpsRedirection();

			//使用cookies
			app.UseCookiePolicy();


			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}




			if (!Directory.Exists(_configuration["Web:ImgRoot"] + "/Upfile")) {
				FileHelper.AddFolder(_configuration["Web:ImgRoot"] + "/Upfile");
			}

			app.UseImageSharp();//web图片处理

			app.UseStaticFiles();

			////ImageSharp V1.0.0不支持静态物理路径的设置，
			var old_rootfile = _env.WebRootFileProvider;
			app.MapWhen(context =>
				context.Request.Path.StartsWithSegments("/Upfile"),
				builder =>
				{
					_env.WebRootFileProvider = new PhysicalFileProvider(_configuration["Web:ImgRoot"]);
					builder.UseImageSharp();//web图片处理
					builder.UseStaticFiles(new StaticFileOptions()
					{
						FileProvider = new PhysicalFileProvider(_configuration["Web:ImgRoot"] + "/Upfile"),//指定实际物理路径
						RequestPath = new PathString("/Upfile")//对外的访问路径
					});//静态文件
				}
			);

			_env.WebRootFileProvider = old_rootfile;
			//app.UseAuthentication();
			app.UseSession();
			app.UseRouting();

            //语言包
            List<string> list_lang = new List<string>();
            _configuration.Bind("Languate", list_lang);
            var supportedCultures = list_lang.ToArray();
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
				//.AddInitialRequestCultureProvider(new AcceptLanguageHeaderRequestCultureProvider())  //设置判断当前语言的方式,我项目中是使用了Accept-Language 的header值作为判断
				.AddSupportedCultures(supportedCultures)
				.AddSupportedUICultures(supportedCultures);
			app.UseRequestLocalization(localizationOptions);
			
			if (env.IsDevelopment())
			{
				app.UseEndpoints(endpoints =>
				{
					endpoints.MapControllerRoute(
						name: "areas",
						pattern: "{area=Admin_WeTalk}/{controller=Login}/{action=Index}/{id?}");
				});
			}
			else
			{
				app.UseEndpoints(endpoints =>
				{
					endpoints.MapControllerRoute(
						name: "areas",
						pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
				});
			}

			//app.MystiqueRoute(_configuration);

		}

		#region 自动注入服务
		// 注意在Program.CreateHostBuilder，添加Autofac服务工厂
		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new AutofacModuleRegister());
			builder.RegisterModule<AutofacPropertityModuleReg>();
		}
		#endregion
	}
}
