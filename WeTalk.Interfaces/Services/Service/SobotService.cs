using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Base;
using WeTalk.Models;

namespace WeTalk.Interfaces.Services
{
	public partial class SobotService : BaseService, ISobotService
	{
		private readonly IPubConfigBaseService _pubConfigBaseService;
		private readonly SqlSugarScope _context;
		private readonly ISobotBaseService _sobotBaseService;
        public SobotService(SqlSugarScope dbcontext, ISobotBaseService sobotBaseService,
            IPubConfigBaseService pubConfigBaseService)
		{
			_pubConfigBaseService = pubConfigBaseService;
			_sobotBaseService = sobotBaseService;
            _context = dbcontext;
		}

		#region "刷新智齿接口Token"
		/// <summary>
		/// 刷新智齿接口Token
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> UpdateToken()
		{
			var result = new ApiResult();
			var dic_config = _pubConfigBaseService.GetConfigs("sobot_appid,sobot_app_key,sobot_token,sobot_updatetime,sobot_expires_in");
			string token = (dic_config.ContainsKey("sobot_token") && !string.IsNullOrEmpty(dic_config["sobot_token"]))?dic_config["sobot_token"]:"";
			string expires_in = (dic_config.ContainsKey("sobot_expires_in") && !string.IsNullOrEmpty(dic_config["sobot_expires_in"])) ? dic_config["sobot_expires_in"] : "";
			string updatetime = (dic_config.ContainsKey("sobot_updatetime") && !string.IsNullOrEmpty(dic_config["sobot_updatetime"])) ? dic_config["sobot_updatetime"] : "";
			if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(expires_in) && !string.IsNullOrEmpty(updatetime))
			{
				if (DateTime.Parse(updatetime).AddSeconds(int.Parse(expires_in)-3600) > DateTime.Now) return result;
			}
			string appid = dic_config["sobot_appid"], app_key = dic_config["sobot_app_key"];
			int create_time = DateHelper.ConvertDateTimeInt(DateTime.Now);
			string sign = MD5Helper.MD5Encrypt32(appid + create_time + app_key).ToLower();
			string url = string.Format(Appsettings.app("APIS:sobot_token"), appid, create_time, sign);
			var json = GetRemoteHelper.HttpWebRequestUrl(url, "", "utf-8", "get");
			var o = JObject.Parse(json);
			if (o != null && o["item"] != null)
			{
				token = (o["item"]["token"] != null) ? o["item"]["token"].ToString() : "";
				expires_in = (o["item"]["expires_in"] != null) ? o["item"]["expires_in"].ToString() : "";
				dic_config.Clear();
				dic_config.Add("sobot_updatetime", DateTime.Now.ToString());
				dic_config.Add("sobot_token", token);
				dic_config.Add("sobot_expires_in", expires_in);
				_pubConfigBaseService.UpdateConfigs(dic_config);
				result.StatusCode = 0;
			}
			else {
				result.StatusCode = 4002;
				result.Message = "刷新智齿Token:"+json;
			}
			return result;
		}
		#endregion


		#region "查询数据字典"
		/// <summary>
		/// 查询数据字典
		/// </summary>
		/// <returns>字典的JSON</returns>
		public async Task<ApiResult<string>> GetDataDict()
		{
			await UpdateToken();
            var result = new ApiResult<string>();
			string url = "https://global.sobot.com/api/ws/5/ticket/get_data_dict";
			string token = _pubConfigBaseService.GetConfig("sobot_token");
			if (!string.IsNullOrEmpty(token))
			{
				var dic_headers = new Dictionary<string, string>();
				dic_headers.Add("token", token);
				var json = GetRemoteHelper.HttpWebRequestUrl(url, "", "utf-8", "get", dic_headers);
				var o = JObject.Parse(json);
				if (o != null && o["item"] != null)
				{
					result.Data = o["item"].ToString();
					result.StatusCode = 0;
				}
				else
				{
					result.StatusCode = 4002;
					result.Message = "获取智齿数据字典失败:" + json;
				}
			}
			else
			{
				result.StatusCode = 4009;
				result.Message = "智齿Token不存在";
			}
			return result;
		}
        /// <summary>
        /// 更新工单分类
        /// </summary>
        /// <returns></returns>
        public void UpdateSobotTicketType() {
            var result_dict = GetDataDict().Result;
			if (result_dict.StatusCode == 0) {
				var o = JObject.Parse(result_dict.Data);
				if (o != null && o["item"] != null) {
					if (o["item"]["ticket_type_list"] == null) return;
					var list_SobotTicketType = _context.Queryable<SobotTicketType>().ToList();
					var jarray = (JArray)o["item"]["ticket_type_list"];
					foreach (var item in jarray)
					{
						if (item["typeid"] == null) continue;
						var model = list_SobotTicketType.FirstOrDefault(u => u.SobotTypeid == item["typeid"].ToString());
						if (model != null)
						{
							model.SobotCompanyid = (item["companyid"] != null) ? item["companyid"].ToString() : "";
							model.SobotNodeFlag = (item["node_flag"] != null) ? item["node_flag"].ToString() : "";
							model.SobotParentid = (item["parentid"] != null) ? item["parentid"].ToString() : "";
							model.SobotTypeLevel = (item["type_level"] != null) ? item["type_level"].ToString() : "";
							model.SobotTypeName = (item["type_name"] != null) ? item["type_name"].ToString() : "";
							model.SobotTypeid = (item["typeid"] != null) ? item["typeid"].ToString() : "";
							model.Lasttime = DateTime.Now;
						}
						else {
							model = new SobotTicketType();
							model.SobotCompanyid = (item["companyid"] != null) ? item["companyid"].ToString() : "";
							model.SobotNodeFlag = (item["node_flag"] != null) ? item["node_flag"].ToString() : "";
							model.SobotParentid = (item["parentid"] != null) ? item["parentid"].ToString() : "";
							model.SobotTypeLevel = (item["type_level"] != null) ? item["type_level"].ToString() : "";
							model.SobotTypeName = (item["type_name"] != null) ? item["type_name"].ToString() : "";
							model.SobotTypeid = (item["typeid"] != null) ? item["typeid"].ToString() : "";
							model.Lasttime = DateTime.Now;
							list_SobotTicketType.Add(model);
						}
					}
					var x = _context.Storageable(list_SobotTicketType).ToStorage();
					x.AsInsertable.ExecuteCommand();
					x.AsUpdateable.ExecuteCommand();
                }
			}
        }
		#endregion

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
			return await _sobotBaseService.AddUserTicket(ticketTitle,ticketContent,extendFields,userid,parentid,userEmails,userTels,fileStr);

        }
		#endregion

		//#region "坐席创建工单"
		///// <summary>
		///// 工作台创建工单
		///// </summary>
		///// <returns></returns>
		//public async Task<ApiResult<long>> AddAgentTicket()
		//{
		//	var result = new ApiResult<long>();

		//	return result;
		//}
		//#endregion
	}
}