using Autofac;
using Microsoft.AspNetCore.Mvc;

namespace WeTalk.Api
{
	public class AutofacPropertityModuleReg : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//重写，所有继承ControllerBase的类的注入
			var controllerBaseType = typeof(ControllerBase);
			builder.RegisterAssemblyTypes(typeof(Program).Assembly)
				.Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
				.PropertiesAutowired();

		}
	}
}
