using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WeTalk.Common.Helper;
using WeTalk.Extensions;

namespace WeTalk.Api.Base
{
	public class WebApiController: Controller
	{
		IHttpContextAccessor _accessor;


		///// <summary>
		///// 展示货币
		///// </summary>
		//public string CurrencyCode = "";

		///// <summary>
		///// 时区差(分)
		///// </summary>
		//public int UtcSec = -480;//默认中国

		public WebApiController(IHttpContextAccessor accessor)
		{
			_accessor = accessor;
			//Lang = GSRequestHelper.GetServerString(_accessor.HttpContext, "X-Lang");
			//CurrencyCode = GSRequestHelper.GetServerString(_accessor.HttpContext, "X-CurrencyCode");
			//var utc = GSRequestHelper.GetServerString(_accessor.HttpContext, "X-Utc");
			//var utcSec = GSRequestHelper.GetServerString(_accessor.HttpContext, "X-UtcSec");
			//if (string.IsNullOrEmpty(Lang)) Lang = "en";
			//if (string.IsNullOrEmpty(CurrencyCode)) CurrencyCode = "USD";
			//if (!string.IsNullOrEmpty(utcSec)) UtcSec = int.Parse(utcSec);
		
		}

		[NonAction]
		public override OkObjectResult Ok(Object value)
		{
			Dictionary<string, object> result = new Dictionary<string, object>();
			result.Add("ErrorCode", 0);
			result.Add("IsSuccess", true);
			if (value != null) result.Add("Data", value);
			OkObjectResult o = new OkObjectResult(result);
			return o;
		}
		[NonAction]
		public OkObjectResult Ok(Object value, bool IsSuccess = true, int ErrorCode = 0, string msg = "")
		{
			if (IsSuccess) ErrorCode = 0;
			var result = new Dictionary<string, object>();
			result.Add("ErrorCode", ErrorCode);
			result.Add("IsSuccess", IsSuccess);
			if (value != null) result.Add("Data", value);
			if (msg != null) result.Add("ErrorMessage", msg);
			
			OkObjectResult o = new OkObjectResult(result);
			return o;
		}

		[NonAction]
		public JsonResult OkJson(Object value, bool IsSuccess = true, int ErrorCode = 0, string msg = "")
		{
			if (IsSuccess) ErrorCode = 0;
			var result = new Dictionary<string, object>();
			result.Add("ErrorCode", ErrorCode);
			result.Add("IsSuccess", IsSuccess);
			if (value != null) result.Add("Data", value);
			if (msg != null) result.Add("ErrorMessage", msg);

			JsonResult o = new JsonResult(result);
			return o;
		}
	}

}
