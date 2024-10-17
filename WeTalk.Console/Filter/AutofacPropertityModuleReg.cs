using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace WeTalk.Console.Filter
{
    public class AutofacPropertityModuleReg : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //WeTalk.Console.CMD.Services
            //重写，所有继承ControllerBase的类的注入
            var controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                //.Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired();


            #region 带有接口层的服务注入
            var cacheType = new List<Type>();

            builder.RegisterAssemblyTypes(Assembly.Load("WeTalk.Console"))
                        .AsImplementedInterfaces()
                        //.InstancePerDependency()//对每一个依赖或每一次调用创建一个新的唯一的实例
                        .InstancePerLifetimeScope()
                        //.SingleInstance()	//所有对父容器或者嵌套容器的请求都会返回同一个实例
                        //.InstancePerRequest()
                        .PropertiesAutowired()
                        .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                        .InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。
            #endregion

        }
    }
}
