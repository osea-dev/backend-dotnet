using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace WeTalk.Extensions
{
	/// <summary>
	/// Autofac服务工厂
	/// </summary>
	public class AutofacModuleRegister : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			var basePath = AppContext.BaseDirectory;
			#region 带有接口层的服务注入
			var cacheType = new List<Type>();

			// 获取 Service.dll 程序集服务，并注册
			//var assemblysServices = Assembly.LoadFrom(servicesDllFile);
			var assemblysServices = Assembly.Load("WeTalk.Interfaces");
			builder.RegisterAssemblyTypes(assemblysServices)
						.AsImplementedInterfaces()
						//.InstancePerDependency()//对每一个依赖或每一次调用创建一个新的唯一的实例
						.InstancePerLifetimeScope()
						//.SingleInstance()	//所有对父容器或者嵌套容器的请求都会返回同一个实例
						//.InstancePerRequest()
						.PropertiesAutowired()
						.EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
						.InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。
			#endregion

			#region 没有接口层的服务层注入

			////因为没有接口层，所以不能实现解耦，只能用 Load 方法。
			////注意如果使用没有接口的服务，并想对其使用 AOP 拦截，就必须设置为虚方法
			//var assemblysServicesNoInterfaces = Assembly.Load("WeTalk.Console");
			//builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);

			#endregion


		}
	}
}
