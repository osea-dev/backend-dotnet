using Microsoft.AspNetCore.Http;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Models;

namespace WeTalk.Interfaces.Services
{
	public class PubLogService :  IPubLogService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		public PubLogService(IHttpContextAccessor accessor, SqlSugarScope context) 
		{
			_context = context;
			_accessor = accessor;
		}

		#region 删除日志
		/// <summary>
		/// 删除日志
		/// </summary>
		/// <param name="list_menu_functionid"></param>
		/// <param name="IsAdmin"></param>
		/// <returns></returns>
		public async Task<ApiResult> DelLog(List<long> listLogid)
		{
			var api = new ApiResult();
			int count = _context.Updateable<PubLog>()
				.SetColumns(u => u.Status == -1)
				.Where(u => listLogid.Contains(u.PubLogid))
				.ExecuteCommand();
			if (count > 0)
			{
				return api;
			}
			else
			{
				api.StatusCode = 4009;
				api.Message = "删除失败,不存在此记录！";
				return api;
			}
		}
		#endregion

		#region 添加日志
		/// <summary>
		/// 添加日志
		/// </summary>
		/// <param name="list_menu_functionid"></param>
		/// <param name="IsAdmin"></param>
		/// <returns></returns>
		public async Task<ApiResult> AddLog(string userName,string content)
		{
			var api = new ApiResult();
			var model_Log = new PubLog();
			long admin_masterid = 0;
			if (_accessor.HttpContext.Session.Keys.Contains("AdminID")) admin_masterid = long.Parse(_accessor.HttpContext.Session.GetString("AdminID"));
			if (_accessor.HttpContext.Session.Keys.Contains("AdminName")) userName = _accessor.HttpContext.Session.GetString("AdminName");
			model_Log.AdminMasterid = admin_masterid;
			model_Log.Username = userName;
			model_Log.Content = content;
			model_Log.Ip = IpHelper.GetCurrentIp(_accessor.HttpContext);
			model_Log.Url= _accessor.HttpContext.Request.Scheme + "://" + _accessor.HttpContext.Request.Host + _accessor.HttpContext.Request.Path + _accessor.HttpContext.Request.QueryString;
			_context.Insertable(model_Log).ExecuteCommand();
			return api;
		}
		#endregion
	}
}
