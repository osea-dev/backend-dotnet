using Autofac;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WeTalk.Api.Middleware;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Extensions;
using WeTalk.Interfaces;

namespace WeTalk.Api
{
	public class Startup
	{
		private readonly IWebHostEnvironment _env;
		public static ILoggerRepository repository { get; set; }//log4net日志
		public static ICacheService myCache;
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

		public IConfiguration _configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
				options.Cookie.Expiration = TimeSpan.FromDays(1);
			});

            //关闭参数自动校验,我们需要返回自定义的格式
            services.Configure<ApiBehaviorOptions>((o) =>
            {
                o.SuppressModelStateInvalidFilter = true;
            });

            //全局静态调用_config
            var basePath = AppContext.BaseDirectory;
			services.AddSingleton(new Appsettings(basePath));

			//跨域预检请求
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
			//数据库
			services.AddSqlsugarSetup(log);

			//services.AddSession(options =>
			//{
			//	options.IdleTimeout = TimeSpan.FromMinutes(30);
			//	options.Cookie.HttpOnly = true;
			//});

			//设置语言包文件夹名称
			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services.AddControllersWithViews(setup =>
			{
				setup.Filters.Add<ValidateGlobalAttribute>();//全局参数较验
				setup.ReturnHttpNotAcceptable = true;//Accept填错，不返回值 
													 //setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());//添加XML响应格式，默认是JSON(旧的写法，输出和输入要分开加，新的全包含)
			})
			.AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;// 如字段为null值，该字段不会返回到前端
				//options.SerializerSettings.ContractResolver = new DefaultContractResolver();// 默认驼峰,设置成不使用驼峰				
				options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";// 设置时间格式
			})
			.AddXmlDataContractSerializerFormatters()
			.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)//视图使用的语言包
			.AddDataAnnotationsLocalization(options =>
			{
				options.DataAnnotationLocalizerProvider = (type, factory) =>
					factory.Create(typeof(LangResource));//使用集中的公共语言包，如果要按类一一对应区分开，则把options参数全去掉
			})
			.AddRazorRuntimeCompilation();
			services.AddRazorPages();


            //var connectionString = Configuration.GetConnectionString("WeTalkConnection");
            //ServerVersion serverVersion = ServerVersion.AutoDetect(connectionString);
            //services.AddDbContextPool<uecardcontext>(options =>
            //	options.UseMySql(connectionString, serverVersion));

            services.AddScoped<TokenMiddleware>();
			services.AddControllers();

			services.AddMiniProfiler(options =>
			{
				options.PopupRenderPosition = RenderPosition.BottomLeft;
				options.PopupShowTimeWithChildren = true;
				options.RouteBasePath = "/profiler";
			});

			if (_configuration.GetSection("Web:UseSwagger").Value == "1")
			{
				services.AddSwaggerGen(option =>
				{
					option.SwaggerDoc("V1", new Microsoft.OpenApi.Models.OpenApiInfo
					{
						Title = "WeTalk.Api",//文档标题
						Version = "V1.0",//文档版本
						Description = "",
						Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "Eric", Url = new Uri("https://www.uelike.com") },
						License = new Microsoft.OpenApi.Models.OpenApiLicense { Name = "Uelike", Url = new Uri("https://www.uelike.com") }
					});
                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    //添加自定义header授权参数
                    option.OperationFilter<HttpHeaderFilter>(Array.Empty<object>());
					option.CustomSchemaIds(c => c.FullName.Replace("+","."));

					option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
					option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "WeTalk.Models.xml"), true);
					
				});
			}

			//注册REDIS 服务
			RedisServer.Initalize();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			//图片压缩工具
			services.AddImageSharp();
		}

		// This method gets called by the runtime. Use this metsahod to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseDefaultFiles();
			app.UseStaticFiles();//本项目静态文件不过ImageSharp;
			app.UseMiddleware<ApiLogMiddleware>();//接口日志，包括初始化用户状态，必需放在鉴权前面
			app.UseMiniProfiler();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			//跨域
			app.UseCors("CorsPolicy");
			//app.UseOptionsRequest();

			if (_configuration.GetSection("Web:UseSwagger").Value == "1")
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/Swagger/V1/Swagger.json", "WeTalk.Api V1.0");
					c.DefaultModelsExpandDepth(-1);
				});
			}

			//IP过滤
			app.UseMiddleware<AdminSafeListMiddleware>(_configuration["AdminSafeList"]);

			if (!Directory.Exists(_configuration["Web:ImgRoot"] + "/Upfile"))
			{
				FileHelper.AddFolder(_configuration["Web:ImgRoot"] + "/Upfile");
			}



			app.UseCookiePolicy();
			//app.UseSession();

			////ImageSharp V1.0.0不支持静态物理路径的设置，
			var _webRootFileProvider = _env.WebRootFileProvider;
			app.MapWhen(context =>//不切回主分支  
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
			_env.WebRootFileProvider = _webRootFileProvider;
			//接口鉴权
			app.UseTokenMiddleware();
			//app.UseMiddleware<TokenMiddleware>();
			//app.UseHttpsRedirection();


			app.UseRouting();
            //语言包
            List<string> list_lang = new List<string>();
			_configuration.Bind("Languate", list_lang);
			var supportedCultures = list_lang.ToArray();
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddInitialRequestCultureProvider(new AcceptLanguageHeaderRequestCultureProvider())  //设置判断当前语言的方式,我项目中是使用了Accept-Language 的header值作为判断
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            if (env.IsDevelopment())
			{
				app.UseEndpoints(endpoints =>
				{
					endpoints.MapControllerRoute(
						name: "areas",
						pattern: "{area:exists}/{controller=Home}/{action=Index}/{appid?}");
				});
			}
			else
			{
				app.UseEndpoints(endpoints =>
				{
					endpoints.MapControllerRoute(
						name: "areas",
						pattern: "{area:exists}/{controller=Home}/{action=Index}/{appid?}");
				});
			}
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
