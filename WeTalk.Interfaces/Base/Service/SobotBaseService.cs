using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Models;

namespace WeTalk.Interfaces.Base
{
	public partial class SobotBaseService : BaseService, ISobotBaseService
    {
        private readonly ILogger<SobotBaseService> _logger;
        private readonly IPubConfigBaseService _pubConfigBaseService;
		private readonly SqlSugarScope _context;
		public SobotBaseService(SqlSugarScope dbcontext, ILogger<SobotBaseService> logger,
            IPubConfigBaseService pubConfigBaseService)
		{
			_pubConfigBaseService = pubConfigBaseService;
			_context = dbcontext;
            _logger = logger;
        }


		#region "代客创建工单"
		/// <summary>
		/// 代客创建工单
		/// </summary>
		/// <param name="ticketTitle">工单标题</param>
		/// <param name="ticketContent">工单问题描述</param>
		/// <param name="extendFields">自定义字段,object={fieldid:xxx,field_value:xxx}</param>
		/// <param name="userid">客户ID</param>
		/// <param name="parentid">对接ID</param>
		/// <param name="userEmails">客户邮箱</param>
		/// <param name="userTels">客户电话</param>
		/// <param name="fileStr">附件路径，多个附件，附件之间采用英文分号";"隔开</param>
		/// <returns></returns>
		public async Task<ApiResult<string>> AddUserTicket(string ticketTitle,string ticketContent,
			List<object> extendFields = null,string userid="",string parentid = "", string  userEmails = "", string userTels = "", string fileStr = "")
		{
			var result = new ApiResult<string>();
			string url = Appsettings.app("APIS:sobot_save_user_ticket");
			var dic_config = _pubConfigBaseService.GetConfigs("sobot_token,sobot_companyid,sobot_tick_typeid");
			string token = dic_config.ContainsKey("sobot_token") ? dic_config["sobot_token"] : "";
			string companyid= dic_config.ContainsKey("sobot_companyid") ? dic_config["sobot_companyid"] : "";
			string ticketTypeid= dic_config.ContainsKey("sobot_tick_typeid") ? dic_config["sobot_tick_typeid"] : "";
			string ticketFrom = "1";//工单来源，1 PC客户留言，2 H5客户留言，3 微信公众号客户留言，4 APP客户留言，12 邮件留言，13语音留言，16微信小程序客户留言，17企业微信客户留言
            var dic_data = new Dictionary<string,object>();
			dic_data.Add("companyid", companyid);
			dic_data.Add("ticket_title", ticketTitle);
			dic_data.Add("ticket_content", ticketContent);
			dic_data.Add("ticket_typeid", ticketTypeid);
			dic_data.Add("ticket_from", ticketFrom);
			if(extendFields!=null) dic_data.Add("extend_fields", extendFields);
			if(!string.IsNullOrEmpty(userid))dic_data.Add("userid", userid);
			if (!string.IsNullOrEmpty(parentid)) dic_data.Add("parentid", parentid);
			if (!string.IsNullOrEmpty(userEmails)) dic_data.Add("user_emails", userEmails);
			if (!string.IsNullOrEmpty(userTels)) dic_data.Add("user_tels", userTels);
			if (!string.IsNullOrEmpty(fileStr)) dic_data.Add("file_str", fileStr);
			if (!string.IsNullOrEmpty(token))
			{
				var dic_headers = new Dictionary<string, string>();
				dic_headers.Add("token", token);
				var json = GetRemoteHelper.HttpWebRequestUrl(url, JsonConvert.SerializeObject(dic_data), "utf-8", "post", dic_headers, "application/json");
				try
				{
					var o = JObject.Parse(json);
					if (o != null && o["item"] != null)
					{
						result.Data = (o["item"]["ticketid"] != null) ? o["item"]["ticketid"].ToString() : "";
						result.StatusCode = 0;
					}
					else
					{
						result.StatusCode = 4002;
						result.Message = "创建工单失败:" + json;
					}
				}
				catch (Exception ex) {
					_logger.LogError(ex, "创建工单,解析JSON失败:" + json);
                    result.StatusCode = 4002;
                    result.Message = "创建工单失败:" + json;
                }
			}
			else {
				result.StatusCode = 4009;
				result.Message = "智齿Token不存在";
			}
			return result;
		}
		#endregion

	}
}