using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System;
using System.Collections.Generic;
using WeTalk.Common.Helper;
using WeTalk.Web.Services;

namespace WeTalk.Web.Base
{
	public class ApiController : Controller
	{
		public ApiController()
		{
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
		public JsonResult OkJson(Object value, long userid = 0)
		{
			Dictionary<string, object> result = new Dictionary<string, object>();
			result.Add("ErrorCode", 0);
			result.Add("IsSuccess", true);
			if (value != null) result.Add("Data", value);
			if (userid > 0) result.Add("Userid", userid);
			JsonResult o = new JsonResult(result);
			return o;
		}

		[NonAction]
		public JsonResult OkJson(Object value, bool IsSuccess = true, int ErrorCode = 0, string msg = "", long userid = 0)
		{
			if (IsSuccess) ErrorCode = 0;
			var result = new Dictionary<string, object>();
			result.Add("ErrorCode", ErrorCode);
			result.Add("IsSuccess", IsSuccess);
			if (value != null) result.Add("Data", value);
			if (userid > 0) result.Add("Userid", userid);
			if (msg != null) result.Add("ErrorMessage", msg);

			JsonResult o = new JsonResult(result);
			return o;
		}


	}

}
