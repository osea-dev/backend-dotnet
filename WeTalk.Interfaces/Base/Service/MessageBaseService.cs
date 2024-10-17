using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tea;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Models;
using WeTalk.Models.Dto.Student;

namespace WeTalk.Interfaces.Base
{
	public partial class MessageBaseService : BaseService, IMessageBaseService
	{
		private readonly SqlSugarScope _context;
		private readonly ILogger<MessageBaseService> _logger;
		private readonly IHttpContextAccessor _accessor;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		private readonly IUserManage _userManage;
		private readonly IPubConfigBaseService _pubConfigBaseService;
		private readonly ICommonBaseService _commonBaseService;

        public MessageBaseService(SqlSugarScope dbcontext,IHttpContextAccessor accessor, ILogger<MessageBaseService> logger, IStringLocalizer<LangResource> localizer, IUserManage userManage,
			IPubConfigBaseService pubConfigBaseService, ICommonBaseService commonBaseService)
		{
			_context = dbcontext;
			_logger = logger;
			_accessor = accessor;
			_localizer = localizer;

			_userManage = userManage;
			_pubConfigBaseService = pubConfigBaseService;
			_commonBaseService = commonBaseService;

        }

		#region "发短信"
		/// <summary>
		/// 发短信（中国大陆）
		/// </summary>
		/// <param name="mobile">+86,86或不加前缀均可</param>
		/// <param name="templateParam">模板变量JSON串,例{\"code\":\"测试\"}</param>
		/// <param name="templateCode">模板CODE</param>
		/// <returns></returns>
		public async Task<ApiResult> SendSms(string mobile,string templateParam= "{\"code\":\"测试\"}", string templateCode = "SMS_268600471")
		{
			var result = new ApiResult();
			var dic_config = _pubConfigBaseService.GetConfigs("aliyun_accesskey,aliyun_secret,sms_template_code,sendname,mailservice,sendmail,sendpwd");
			if (dic_config.ContainsKey("sms_template_code")) templateCode = dic_config["sms_template_code"].ToString();
			var config = new AlibabaCloud.OpenApiClient.Models.Config
			{
				// 必填，您的 AccessKey ID
				AccessKeyId = dic_config["aliyun_accesskey"],
				// 必填，您的 AccessKey Secret
				AccessKeySecret = dic_config["aliyun_secret"],
			};
			// 访问的域名
			config.Endpoint = "dysmsapi.aliyuncs.com"; 
			var client = new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
			
			var sendSmsRequest = new AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest
			{
				PhoneNumbers = mobile,
				SignName = "WeTalk",//短信签名
				TemplateCode = templateCode,
				TemplateParam = templateParam
			};
			try
			{
				var sms_result = client.SendSmsWithOptions(sendSmsRequest, new AlibabaCloud.TeaUtil.Models.RuntimeOptions());
				if (sms_result.StatusCode == 200)
				{
					if (sms_result.Body.Code.ToUpper() != "OK")
					{
						result.StatusCode = 4002;
						result.Message = sms_result.Body.Message;
					}
				}
				else
				{
                    
                    //异常通知
                    EmailHelper.Sentmail("系统报警:国内短信发送异常", "手机号:" + mobile + "<br/>templateCode:" + templateCode + "<br />返回值:" + JsonConvert.SerializeObject(sms_result.Body) + "<br /><br />", dic_config["sendname"], dic_config["sendmail"],
                        Appsettings.app("Email:WarnEmail"), "", dic_config["mailservice"], dic_config["sendmail"], dic_config["sendpwd"]);
                    _logger.LogInformation(JsonConvert.SerializeObject(sms_result));

                    result.StatusCode = 4002;
					result.Message = "发送短信失败,返回状态码" + sms_result.StatusCode;
				}
			}
			catch (TeaException error)
			{
				result.Message = _localizer["发送短信失败"] + ":" + error.Message;
				result.StatusCode = 4002;
			}
			catch (Exception _error)
			{
				result.Message = _localizer["发送短信失败"] + ":" + _error.Message;
				result.StatusCode = 4002;
			}
			return result;
		}

		/// <summary>
		/// 发短信(国际港澳台地区)
		/// </summary>
		/// <param name="mobile">国际区号+号码,例如：8521245567****</param>
		/// <param name="message">短信的内容</param>
		/// <returns></returns>
		public async Task<ApiResult> SendGlobeSms(string mobile,string message,string senderid="")
		{
			var result = new ApiResult();
			var dic_config = _pubConfigBaseService.GetConfigs("alibaba_accesskey,alibaba_secret,sendname,mailservice,sendmail,sendpwd");
			
			IClientProfile profile = DefaultProfile.GetProfile("ap-southeast-1", dic_config["alibaba_accesskey"], dic_config["alibaba_secret"]);//ap-southeast-1写死不能改
			DefaultAcsClient client = new DefaultAcsClient(profile);
			CommonRequest request = new CommonRequest();
			request.Method = MethodType.POST;
			request.Domain = "dysmsapi.ap-southeast-1.aliyuncs.com";//写死不能改,dysmsapi.ap-southeast-1.aliyuncs.com,sms-intl.ap-southeast-1.aliyuncs.com
			request.Version = "2018-05-01";//写死不能改
			request.Action = "SendMessageToGlobe";//接口号，即API请求的接口
			request.AddQueryParameters("To", mobile);
			request.AddQueryParameters("Message", message);
			if (!string.IsNullOrEmpty(senderid))
			{
				request.AddQueryParameters("From", senderid);
			}
			request.AddQueryParameters("Type", "OTP");//NOTIFY：通知短信,MKT：推广短信,OTP:验证码

			// request.Protocol = ProtocolType.HTTP;
			try
			{
				CommonResponse response = client.GetCommonResponse(request);
				var json = Encoding.Default.GetString(response.HttpResponse.Content);
                _logger.LogInformation(json);
                var o = JObject.Parse(json);
				if (o["ResponseCode"] != null && o["ResponseCode"].ToString().ToUpper() == "OK")
				{
				}
				else {
					//异常通知
					EmailHelper.Sentmail("系统报警:国际短信发送异常", "手机号:" + mobile + "<br/>信息:" + message + "<br/>senderid:" + senderid+"<br />返回值:" + json + "<br /><br />", dic_config["sendname"], dic_config["sendmail"], 
						Appsettings.app("Email:WarnEmail"), "", dic_config["mailservice"], dic_config["sendmail"], dic_config["sendpwd"]);

                }
			}
			catch (ServerException e)
			{
				result.Message = _localizer["发送国际短信失败"] + ":" + e.Message;
				result.StatusCode = 4002;
			}
			catch (ClientException e)
			{
				result.Message = _localizer["发送国际短信失败"] + ":" + e.Message;
				result.StatusCode = 4002;
			}
			return result;
		}
		#endregion

		#region "按模板发送邮件"
		/// <summary>
		/// 按模板将邮件添入任务队列
		/// </summary>
		/// <param name="code">模板代码</param>
		/// <param name="email">收件人邮箱</param>
		/// <param name="dic_data">变量参数</param>
		/// <returns></returns>
		public async Task<ApiResult> AddEmailTask(string code, string email,Dictionary<string,string> dic_data, string lang = "")
		{
			//发Mail
			var result = new ApiResult();
			string title = "", body = "";
			lang = !string.IsNullOrEmpty(lang) ? lang : _userManage.Lang;
			var model_Template = _context.Queryable<WebEmailTemplate>().First(u =>u.Status==1 && u.Code.ToLower() == code.ToLower() && u.Lang.ToLower() == lang.ToLower());
			if (model_Template != null)
			{
				title = model_Template.Title+"";
				body = model_Template.Content+"";
				foreach (var item in dic_data)
				{
					title = title.Replace("{"+ item.Key + "}", item.Value);
					body = body.Replace("{" + item.Key + "}", item.Value);
                }
                //var model_Token = _userManage.GetUserToken();
                //var userid = model_Token != null ? model_Token.Userid : 0;
                _context.Insertable(new WebTaskEmail()
                {
                    Userid = _userManage.Userid,
                    Code = code,
                    Subject = title,
                    Body = body,
					Email = email
                }).ExecuteCommand();
                
                return result;
			}
			else {
				result.StatusCode = 4009;
				result.Message = _localizer["模板不存在"];
				return result;
			}			
		}
		
		/// <summary>
		/// 执行邮件任务
		/// </summary>
		/// <returns></returns>
		public async Task SendEmailTask()
        {
            var dic_config = _pubConfigBaseService.GetConfigs("sendname,mailservice,sendmail,sendpwd");
            var list_Task = _context.Queryable<WebTaskEmail>().Where(u => u.Status == 0 || (u.Status == 2 && u.Count < 3)).ToList();
			foreach (var item in list_Task) {
				item.Lasttime = DateTime.Now;
                var result_email = EmailHelper.Sentmail(item.Subject, item.Body, dic_config["sendname"], dic_config["sendmail"], item.Email, "", dic_config["mailservice"], dic_config["sendmail"], dic_config["sendpwd"]);
				if (result_email.isok)
				{
					item.Status = 1;
					item.Body = "";
				}else{
					item.Count++;
					item.Remarks = $"[{DateTime.Now}]发送邮件失败:"+result_email.msg + "<hr>" + item.Remarks;
				}
            }
			_context.Updateable(list_Task).UpdateColumns(u=>new { u.Status,u.Count,u.Remarks,u.Body}).ExecuteCommand();
        }
        #endregion

        #region 消息列表
        /// <summary>
        /// 我的消息列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<Pages<MessageDto>>> MessageList(int page = 1, int pageSize = 10)
        {
            var result = new ApiResult<Pages<MessageDto>>();
            //var model_token = _userManage.GetUserToken();
            if (_userManage.Userid <= 0)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }
            var model_User = _context.Queryable<WebUser>().InSingle(_userManage.Userid);
            var readTime = model_User != null ? DateHelper.ConvertIntToDateTime(model_User.MessageTime.ToString(), "Local") : DateTime.Parse("1970-1-1");

            var list = new List<MessageDto>();
            int total = 0;
            var list_Message = _context.Queryable<WebMessage>()
                .Where(u => (u.AccUserid == _userManage.Userid && u.AccStatus == 1) || (u.AccUserid == 0 && u.SendUserid == 0 && u.AccStatus==1))
                .GroupBy(u => u.SendUserid)
                .Select(u => new { u.SendUserid, MaxTime = SqlFunc.AggregateMax(u.Sendtime), MaxMessageid = SqlFunc.AggregateMax(u.Messageid) })
                .OrderBy(u => u.MaxTime, OrderByType.Desc)
                .ToPageList(page, pageSize, ref total);
            var list_MaxMessage = _context.Queryable<WebMessage>().Where(u => list_Message.Select(s => s.MaxMessageid).Contains(u.Messageid)).ToList();
            var list_User = _context.Queryable<WebUser>().Where(u => list_Message.Select(s => s.SendUserid).Contains(u.Userid)).ToList();
            foreach (var model_Message in list_Message)
            {
                var message = list_MaxMessage.FirstOrDefault(u => u.Messageid == model_Message.MaxMessageid)?.Message;
                var model_SendUser = list_User.FirstOrDefault(u => u.Userid == model_Message.SendUserid);
                list.Add(new MessageDto()
                {
                    Messageid = model_Message.MaxMessageid,
                    SendUserid = model_Message.SendUserid,
                    SendName = model_Message.SendUserid == 0?"WeTalk": model_SendUser?.FirstName + " " + model_SendUser?.LastName,
                    HeadImg = _commonBaseService.ResourceDomain(model_Message.SendUserid==0 ? "/Upfile/images/none.png" : model_SendUser?.HeadImg),
                    Message = message,
                    Sendtime = DateHelper.ConvertDateTimeInt(model_Message.MaxTime),
                    IsRead = model_Message.MaxTime > readTime ? 0 : 1
                });
            }
            result.Data = new Pages<MessageDto>();
            result.Data.Total = total;
            result.Data.List = list;
            return result;
        }

        /// <summary>
        /// 指定发送方的消息列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<Pages<MessageDto>>> MessageDetail(long sendUserid, int page = 1,int pageSize = 10)
        {
            var result = new ApiResult<Pages<MessageDto>>();
            //var model_token = _userManage.GetUserToken();
            if (_userManage.Userid <= 0)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }
            var model_User = _context.Queryable<WebUser>().InSingle(_userManage.Userid);
            var readTime =DateTime.Parse("1970-1-1");
			if (model_User != null) {
				readTime = DateHelper.ConvertIntToDateTime(model_User.MessageTime.ToString(), "Local");
            }
            var list = new List<MessageDto>();
            int total = 0;
            var list_Message = _context.Queryable<WebMessage>()
                .Where(u => ((u.AccUserid == _userManage.Userid && u.AccStatus == 1) ||(u.SendUserid==0 && u.AccStatus == 1)) && u.SendUserid == sendUserid)
                .OrderBy(u => u.Sendtime, OrderByType.Desc)
                .ToPageList(page, pageSize, ref total);
            foreach (var model_Message in list_Message)
            {
                list.Add(new MessageDto()
                {
                    Messageid = model_Message.Messageid,
                    SendUserid = model_Message.SendUserid,
                    Message = model_Message.Message,
                    Sendtime = DateHelper.ConvertDateTimeInt(model_Message.Sendtime),
                    IsRead = model_Message.Sendtime > readTime ? 0 : 1
                });
            }
            result.Data = new Pages<MessageDto>();
            result.Data.Total = total;
            result.Data.List = list;

			if (model_User != null)
			{
				model_User.MessageTime = DateHelper.ConvertDateTimeLong13(DateTime.Now) ;
				_context.Updateable(model_User).ExecuteCommand(); ;
			}
			return result;
        }

        /// <summary>
        /// 删除指定发送方所有消息
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> DelUserMessage(long sendUserid)
        {
            var result = new ApiResult();
            //var model_token = _userManage.GetUserToken();
            if (_userManage.Userid <= 0)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }
			int count = _context.Updateable<WebMessage>()
				.SetColumns(u => u.AccStatus == -1)
				.Where(u => u.AccUserid == _userManage.Userid && u.SendUserid == sendUserid && u.AccStatus == 1)
				.ExecuteCommand();
            return result;
        }
        /// <summary>
        /// 删除单条消息
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> DelMessage(long messageid)
        {
            var result = new ApiResult();
            //var model_token = _userManage.GetUserToken();
            if (_userManage.Userid <= 0)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["用户登录超时退出"];
                return result;
            }
			var model_Message = _context.Queryable<WebMessage>().First(u => u.AccUserid == _userManage.Userid && u.Messageid == messageid);
			if (model_Message != null)
			{
				model_Message.AccStatus = -1;
				_context.Updateable(model_Message).UpdateColumns(u=>u.AccStatus).ExecuteCommand();
            }
			else
            {
                result.StatusCode = 4009;
                result.Message = _localizer["消息不存在"];
            }
            return result;
        }
        #endregion

    }
}
