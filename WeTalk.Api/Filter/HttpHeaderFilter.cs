using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeTalk.Api
{
	public class HttpHeaderFilter : IOperationFilter
    {
        /// <summary>
        /// Header注解（swagger）中是否含币种参数
        /// </summary>
        public class CurrencyAttribute : System.Attribute
        {
        }
        /// <summary>
        /// 给Swagger添加Header头部参数
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            if(operation.Parameters==null)
                operation.Parameters = new List<OpenApiParameter>();
            //var actionAttrs = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            //actionAttrs.Any(u => u.Filter.GetType() == typeof(UserTokenAttribute))
            if (context.MethodInfo.CustomAttributes.Any(u => u.AttributeType == typeof(UserTokenAttribute)))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-UserToken",
                    Description = "用户登录状态Token",
                    In = ParameterLocation.Header,
                    Required = false
                });
            }

            if (context.MethodInfo.CustomAttributes.Any(u => u.AttributeType == typeof(CurrencyAttribute)))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-CurrencyCode",
                    Description = "展示币种",
                    In = ParameterLocation.Header,
                    Required = false
                });
            }
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Utc",
                Description = "UTC名称,如Asia/ShangHai",
                In = ParameterLocation.Header,
                Required = false
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-UtcSec",
                Description = "UTC分差（如北京UTC+8:(0-8)*60=-480)",
                In = ParameterLocation.Header,
                Required = false
            });

            operation.Parameters.Add(
                new OpenApiParameter
                {
                    Name = "Accept-Language",
                    Description = "客户端语言",
                    In = ParameterLocation.Header,
                    Required = false
                }
            );
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Timestamp",
                Description = "时间戳",
                In = ParameterLocation.Query,
                Required = false
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Sign",
                Description = "签名算法=Sha1(key + timestamp + url + data),加密串先转换成小写后再用sha1加密",
                In = ParameterLocation.Query,
                Required = false
            });
        }
    }
}
