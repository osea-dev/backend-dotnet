using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WeTalk.Common.Helper;
using WeTalk.Interfaces;
using WeTalk.Web.Extensions;

namespace WeTalk.Web
{
    //优先级1：权限过滤器：它在Filter Pipleline中首先运行，并用于决定当前用户是否有请求权限。如果没有请求权限直接返回。
    public class AuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// 无值：表示需要经过子控件细化控制项（admin_menu_function）
        /// 有值：仅做登录控制，不做子控件控制
        /// 客户如果不要求细化控制项到按扭级别，全部默认用"有值"
        /// </summary>
        public string Power { get; set; } = "";
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.Filters.Any(it => it is Microsoft.AspNetCore.Mvc.Authorization.IAllowAnonymousFilter))
            {

            }
            else
            {
                var _tokenManager = context.HttpContext.RequestServices.GetService<TokenManager>();
                #region 判断是否拥有权限
                var result_auth = _tokenManager.IsAuthenticated(Power, context.RouteData);
                if (result_auth.isok)
                {
                    var list_controls = result_auth.list_controls;
                    string _script_str = "";
                    _script_str += "$(\".datagrid-toolbar a\").each(function () {\r\n";
                    _script_str += "if($(this).attr(\"group\").indexOf(\"iscontrol\")>=0){$(this).addClass($(this).attr(\"id\"));$(this).hide();}\r\n";
                    _script_str += "});\r\n";
                    _script_str += "$(\".iscontrol\").hide();\r\n";
                    for (int i = 0; i < list_controls.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(list_controls[i]))
                        {
                            _script_str += "$(\"." + list_controls[i].Trim() + "\").show();";
                        }
                    }
                    _tokenManager.ScriptStr = _script_str;
                }
                else
                {
                    RedirectResult result = new RedirectResult($"~/Admin_WeTalk/Login/Error?return_url={WebUtility.UrlEncode("../Login?returnurl=" + GSRequestHelper.GetUrl(context.HttpContext.Request))}");
                    context.Result = result;
                    return;
                }
                #endregion
            }
        }
    }
    //优先级2：资源过滤器： 它在Authorzation后面运行，同时在后面的其它过滤器完成后还会执行。Resource filters 实现缓存或其它性能原因返回。因为它运行在模型绑定前，所以这里的操作都会影响模型绑定。
    public class MyResourceFilterAttribute : IResourceFilter
    {
        //这个ResourceFiltersAttribute是最适合做缓存了,在这里做缓存有什么好处?因为这个OnResourceExecuting是在控制器实例化之前运营，如果能再这里获取ViewReuslt就不必实例化控制器，在走一次视图了，提升性能
        private static readonly Dictionary<string, object> _Cache = new Dictionary<string, object>();
      
        /// <summary>
        /// 这个方法会在控制器实例化之前之前
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Console.WriteLine("OnResourceExecuting");
            //_cacheKey = context.HttpContext.Request.Path.ToString();//这个是请求地址，它肯定是指向的视图
            //if (_Cache.ContainsKey(_cacheKey))
            //{
            //    var cachedValue = _Cache[_cacheKey] as ViewResult;
            //    if (cachedValue != null)
            //    {
            //        context.Result = cachedValue;
            //    }
            //}
        }
        /// <summary>
        /// 这个方法是是Action的OnResultExecuted过滤器执行完之后在执行的（每次执行完Action之后得到就是一个ViewResult）
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //_cacheKey = context.HttpContext.Request.Path.ToString();//这个是请求地址
            //if (!string.IsNullOrEmpty(_cacheKey) && !_Cache.ContainsKey(_cacheKey))
            //{
            //    //因为这个方法是是Action的OnResultExecuted过滤器执行完之后在执行的，所以context.Result必然有值了，这个值就是Action方法执行后得到的ViewResult
            //    var result = context.Result as ViewResult;
            //    if (result != null)
            //    {
            //        _Cache.Add(_cacheKey, result);
            //    }
            //}
        }
    }
    //优先级3：方法过滤器：它会在执行Action方法前后被调用。这个可以在方法中用来处理传递参数和处理方法返回结果。
    public class MyActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("OnActionExecuting");
            //context.HttpContext.Response.WriteAsync("abc");
        }

    }
    //优先级4：异常过滤器：被应用全局策略处理未处理的异常发生前异常被写入响应体
    public class MyExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IModelMetadataProvider _moprovider;
        public MyExceptionFilterAttribute(IModelMetadataProvider moprovider)
        {
            this._moprovider = moprovider;
        }
        public override void OnException(ExceptionContext context)
        {
            Console.WriteLine("MyExceptionFilterAttribute");
            base.OnException(context);
            if (!context.ExceptionHandled)//如果异常没有被处理过
            {
                string controllerName = (string)context.RouteData.Values["controller"];
                string actionName = (string)context.RouteData.Values["action"];
                //string msgTemplate =string.Format( "在执行controller[{0}的{1}]方法时产生异常",controllerName,actionName);//写入日志

                if (this.IsAjaxRequest(context.HttpContext.Request))
                {

                    context.Result = new JsonResult(new
                    {
                        Result = false,
                        PromptMsg = "系统出现异常，请联系管理员",
                        DebugMessage = context.Exception.Message
                    });
                }
                else
                {
                    var result = new ViewResult { ViewName = "~Views/Shared/Error.cshtml" };
                    result.ViewData = new ViewDataDictionary(_moprovider, context.ModelState);
                    result.ViewData.Add("Execption", context.Exception);
                    context.Result = result;
                }

;
            }
        }
        //判断是否为ajax请求
        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }
    //优先级5：结果过滤器：它可以在执行Action结果之前执行，且执行Action成功后执行，使用逻辑必须围绕view或格式化执行结果。
    public class MyResultFilterAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine("OnResultExecuting");
            base.OnResultExecuting(context);
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
        }
    }
}