using Aspose.Cells;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeTalk.Common;
using WeTalk.Common.Helper;
using WeTalk.Extensions;
using WeTalk.Interfaces.Base;
using WeTalk.Models;
using WeTalk.Models.Dto;
using WeTalk.Models.Dto.Menke;
using WeTalk.Models.Dto.User;

namespace WeTalk.Interfaces.Services
{
	public partial class UserService : BaseService, IUserService
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly SqlSugarScope _context;
		private readonly ILogger<UserService> _logger;
		private readonly IWebHostEnvironment _env;
		private readonly IStringLocalizer<LangResource> _localizer;//语言包

		private readonly IUserManage _userManage;
		private readonly IMessageBaseService _messageBaseService;
		private readonly IMenkeBaseService _menkeBaseService;
        private readonly ISobotBaseService _sobotBaseService;
		private readonly ICommonBaseService _commonBaseService;
		public UserService(SqlSugarScope dbcontext, ILogger<UserService> logger,IWebHostEnvironment env, IStringLocalizer<LangResource> localizer, IHttpContextAccessor accessor,
			IMessageBaseService messageBaseService, ISobotBaseService sobotBaseService, ICommonBaseService commonBaseService,
            IMenkeBaseService menkeBaseService, IUserManage userManage)
		{
			_accessor = accessor;
			_context = dbcontext;
			_logger = logger;
			_localizer = localizer;
			_env = env;

			_messageBaseService = messageBaseService;
			_menkeBaseService = menkeBaseService;
			_userManage = userManage;
			_sobotBaseService = sobotBaseService;
			_commonBaseService = commonBaseService;
		}
        #region "品牌网站"
        #region "注册（第一步）"
        /// <summary>
        /// 注册（第一步）
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<TokenDto>> Reg(string firstName,string lastName,string email,string userpwd)
		{
			var result = new ApiResult<TokenDto>();
			var model_User = await _context.Queryable<WebUser>().FirstAsync(u => u.Email == email);
			if (model_User != null)
			{
				result.StatusCode = 4008;
				result.Message = _localizer["注册邮箱已存在"];
			}
			else
			{
				if (!VerifyHelper.IsPassword(userpwd))
				{
					result.StatusCode = 4003;
					result.Message = _localizer["密码必需包含字母和数字，且在6-18位之间"];
					return result;
				}

				model_User = new WebUser();
				model_User.FirstName = firstName;
				model_User.LastName = lastName;
                model_User.Username = email;
				model_User.Email = email;
				model_User.Userpwd = MD5Helper.MD5Encrypt32(userpwd);
				model_User.Status = 0;
				var userid = _context.Insertable(model_User).ExecuteReturnBigIdentity();
				var model_Token = _userManage.SetUserToken(userid);
				if (model_Token != null) {
					result.Data = new TokenDto();
                    result.Data.UserToken = model_Token.Token;
				}
			}
			return result;
		}
		#endregion

		#region "注册-发送短信验证"
		/// <summary>
		/// 注册-发送短信验证
		/// </summary>
		/// <param name="mobile_code"></param>
		/// <param name="mobile"></param>
		/// <returns></returns>
		public async Task<ApiResult> SendSms(string mobile_code, string mobile)
		{
			var result = new ApiResult();
			mobile = mobile.Replace(" ", "");
			//var model_token = _userManage.GetUserToken();
			if (_userManage.Userid<=0)
            {
                result.StatusCode = 4001;
                result.Message = _localizer["登录超时，请重新登录"];
                return result;
            }
            var model_User = _context.Queryable<WebUser>().InSingle(_userManage.Userid);
			if (model_User != null)
			{
				string mobile1 = mobile_code + mobile;
				string sms_code = RandomHelper.RandNum(6);

				if (mobile_code == "86")
				{
					var result_sms = await _messageBaseService.SendSms(mobile1, "{\"code\":\"" + sms_code + "\"}");
					if (result_sms.StatusCode == 0)
					{
						model_User.MobileSmscode = sms_code;
						model_User.MobileSmstime = DateTime.Now;
						_context.Updateable(model_User).ExecuteCommand();
					}
					else
					{
						result.StatusCode = result_sms.StatusCode;
						result.Message = result_sms.Message;
					}
				}
				else {
					string senderid = "";
					switch (mobile_code) {
						case "65":
							senderid = "WeTalk";//WETALK,WeTalk.新加坡注册了两个
							break;
                        case "886":
                            senderid = "WETALK";//台湾
                            break;
						//case "63":
						//    senderid = "WETALK";//菲律宾
						//    break;
						case "44":
							senderid = "WETALK";//英国
							break;
						case "357":
							senderid = "WETALK";//塞浦路斯
							break;
                        case "1":
                            senderid = "WETALK";//美国，加拿大
                            break;
                    }
					string msg = "";
					if (!string.IsNullOrEmpty(senderid))
					{
						msg = "[" + senderid + "] Thank you for registering on WETALK International Global Sinology education communication platform. Your verification code is {0}. For the security of your account, please do not disclose it to others.";
					}
					else {
						msg = "Thank you for registering on WETALK International Global Sinology education communication platform. Your verification code is {0}. For the security of your account, please do not disclose it to others.";
					}
					
					var result_sms = await _messageBaseService.SendGlobeSms(mobile1, string.Format(msg, sms_code), senderid);
					if (result_sms.StatusCode == 0)
					{
						model_User.MobileSmscode = sms_code;
						model_User.MobileSmstime = DateTime.Now;
						_context.Updateable(model_User).ExecuteCommand();
					}
					else
					{
						result.StatusCode = result_sms.StatusCode;
						result.Message = result_sms.Message;
					}
				}
			}
			else
			{
				result.StatusCode = 4009;
				result.Message = _localizer["用户ID不存在"];
			}
			return result;
		}
		#endregion

		#region "注册(第二步)-完善信息"
		/// <summary>
		/// 注册-完善信息
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="mobileCode"></param>
		/// <param name="mobile"></param>
		/// <param name="smsCode"></param>
		/// <param name="birthdate"></param>
		/// <param name="native"></param>
		/// <param name="countryCode"></param>
		/// <param name="timezoneid"></param>
		/// <returns></returns>
		public async Task<ApiResult> PerfectReg(string countryCode, string mobileCode, string mobile, string smsCode, DateTime birthdate, string native,long timezoneid)
		{
			var result = new ApiResult();
			//var model_Token = _userManage.GetUserToken();
			if (_userManage.Userid<=0)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["用户未登录或已超时退出"];
			}
			mobile = mobile.Replace("-", "").Replace(" ", "");
			if (_context.Queryable<WebUser>().Any(u => u.Userid != _userManage.Userid && u.Mobile == mobile && u.MobileCode == mobileCode))
			{
				result.StatusCode = 4008;
				result.Message = _localizer["手机号码已注册"];
				return result;
			}
			if (!_context.Queryable<PubCountry>().Any(u=>u.MobileCode==mobileCode && u.Code== countryCode)) {
				_logger.LogError($"注意：用户手机国家区号与国家代码不存在:{countryCode},{mobileCode}-{mobile}");
			}
			var model_User = _context.Queryable<WebUser>().InSingle(_userManage.Userid);
			if (model_User != null)
			{
				if ((model_User.MobileSmscode + "").ToLower() != smsCode.ToLower())
				{
					result.StatusCode = 4003;
					result.Message = _localizer["手机短信验证码错误"];
					return result;
				}
				else if (model_User.MobileSmstime.AddMinutes(5) < DateTime.Now) { 
					result.StatusCode = 4002;
					result.Message = _localizer["短信验证码码过期失效"];
					return result;
				}
				model_User.MobileSmscode = "";
                model_User.Mobile = mobile;
				model_User.MobileCode = mobileCode;
				model_User.Birthdate = birthdate;
				model_User.Native = native;
				model_User.Timezoneid = timezoneid;
				model_User.Status = 1;
				var model_TimeZone = _context.Queryable<PubTimezone>().First(u => u.Timezoneid == timezoneid);
				if (model_TimeZone != null) {
					model_User.Utc = model_TimeZone.Title;
					model_User.UtcSec = -model_TimeZone.UtcSec / 60;
                }

                var result_menke = await _menkeBaseService.CreateStudents(new MenkeStudentDto
				{
					name = model_User.FirstName + " " + model_User.LastName,
					nickname = model_User.FirstName + " " + model_User.LastName,
					sex = model_User.Gender,
					birthday = birthdate.ToString("d"),
					locale= countryCode.ToUpper(),
                    code = mobileCode,
					mobile = mobile,
					p_name = model_User.GuardianName + ""
                });
				if (result_menke.StatusCode == 0)
				{
					model_User.MenkeUserId = result_menke.Data;
                    //触发注册成功邮件
                    var dic_data = new Dictionary<string, string>();
                    dic_data.Add("name", model_User.FirstName + " " + model_User.LastName);
                    string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
                    dic_data.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
					dic_data.Add("url", $"{string.Format(Appsettings.app("Web:ServiceUrl"), _userManage.Lang)}");
					await _messageBaseService.AddEmailTask("Reg", model_User.Email, dic_data, model_User.Lang);
                    var model_Native = _context.Queryable<PubCountry>().First(u => u.Code.ToLower() == model_User.Native.ToLower());

					//创建工单提醒
					var dic_obj = new Dictionary<string, object>();
					dic_obj.Add(_localizer["姓名"], model_User.FirstName + " " + model_User.LastName);
                    dic_obj.Add(_localizer["手机"], model_User.MobileCode + "-" + model_User.Mobile);
                    dic_obj.Add(_localizer["出生日期"], birthdate.ToString("d"));
                    dic_obj.Add(_localizer["性别"], model_User.Gender==0? _localizer["女"] : _localizer["男"]);
                    dic_obj.Add(_localizer["母语"], _userManage.Lang=="zh-cn"? model_Native?.Country: model_Native?.CountryEn);
                    dic_obj.Add(_localizer["所在地时区"], model_User.Utc);
                    dic_obj.Add(_localizer["邮箱"], model_User.Email);
                    var body = string.Join("<br>", dic_obj.ToList());
                    var result_ticket = await _sobotBaseService.AddUserTicket(_localizer["新用户注册提醒"], body);
					if (result_ticket.StatusCode == 0) {
						model_User.IsTicket = 1;
						model_User.TicketId = result_ticket.Data;
					}
					else
					{
						_logger.LogError(_localizer["新用户注册提醒"] + "," + result_ticket.Message);
					}
					_context.Updateable(model_User).ExecuteCommand();
				}
				else
				{
					result.StatusCode = result_menke.StatusCode;
					result.Message = result_menke.Message;
				}
			}
			else
			{
				result.StatusCode = 4009;
				result.Message = _localizer["用户ID不存在"];
			}
			return result;
		}
		#endregion

		#region "忘记密码(提交邮箱)"
		/// <summary>
		/// 忘记密码(提交邮箱)
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> ForgetPwd(string email)
		{
			var result = new ApiResult();
			var model_User = await _context.Queryable<WebUser>().FirstAsync(u => u.Email == email);
			if (model_User == null)
			{
				result.StatusCode = 4009;
				result.Message = _localizer["您所填写的邮箱不存在,请尝试其它邮箱"];
				return result;
			}
			model_User.EmailCode = RandomHelper.GetMd5Code(email);
			model_User.EmailCodetime = DateTime.Now;
			var dic_data=new Dictionary<string, string>();
			dic_data.Add("name", model_User.FirstName + " " + model_User.LastName);
            string head_img = string.IsNullOrEmpty(model_User.HeadImg) ? "/Upfile/images/none.png" : model_User.HeadImg;
            dic_data.Add("head_img", PicHelper.ImageToBase64(_commonBaseService.ResourceDomain(head_img)));
            dic_data.Add("url", Appsettings.app("Web:Host") + "/user/pw/" + model_User.EmailCode); 
			var result_email = await _messageBaseService.AddEmailTask("ForgetPwd", email, dic_data, model_User.Lang);
			if (result_email.StatusCode == 0) {
				_context.Updateable(model_User).UpdateColumns(u=>new { u.EmailCode,u.EmailCodetime}).ExecuteCommand();
			}
			return result_email;
		}
		#endregion

		#region "重置密码"
		/// <summary>
		/// 通过临时授权码获取邮箱
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<EmailDto>> Email(string tmpCode)
		{
			var result = new ApiResult<EmailDto>();
			if (string.IsNullOrEmpty(tmpCode) && tmpCode.Length != 32)
			{
				result.StatusCode = 4003;
				result.Message = _localizer["授权码异常!"];
				return result;
			}
			var model_User = await _context.Queryable<WebUser>().FirstAsync(u => u.EmailCode.ToLower() == tmpCode.ToLower() && u.Status != -1);
			if (model_User != null)
			{
				if (model_User.EmailCodetime.AddDays(1) > DateTime.Now)
				{
					result.Data = new EmailDto();
					result.Data.Email = model_User.Email;
				}
				else {
					result.StatusCode = 4007;
					result.Message = _localizer["授权码超出24小时,已失效"];
				}
			}
			else
			{
				result.StatusCode = 4007;
				result.Message = _localizer["授权用户不存在"];
			}
			return result;
		}
		/// <summary>
		/// 重置密码
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> ResetPwd(string tmpCode,string userpwd,string userpwd1)
		{
			var result = new ApiResult();
			if (string.IsNullOrEmpty(tmpCode) && tmpCode.Length!=32)
			{
				result.StatusCode = 4003;
				result.Message = _localizer["授权码异常"];
				return result;
			}
			if (userpwd != userpwd1)
			{
				result.StatusCode = 4003;
				result.Message = _localizer["两次填写的密码不一致"];
				return result;
			}

			var model_User = await _context.Queryable<WebUser>().FirstAsync(u => u.EmailCode.ToLower() == tmpCode.ToLower() && u.Status!=-1);
			if (model_User != null)
			{
				if (model_User.EmailCodetime.AddDays(1) < DateTime.Now) {
					result.StatusCode = 4003;
					result.Message = _localizer["授权码超出24小时,已失效"];
					return result;
				}
				model_User.EmailCode = "";
				model_User.Userpwd = MD5Helper.MD5Encrypt32(userpwd);
				_context.Updateable(model_User).UpdateColumns(u => new { u.Userpwd, u.EmailCode }).ExecuteCommand();
			}
			else {
				result.StatusCode = 4007;
				result.Message = _localizer["临时授权码失效"];
				return result;
			}
			return result;
		}
		#endregion

		#region "检测学生登录状态"
		/// <summary>
		/// 检测学生登录状态
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<TokenDto>> CheckStatus()
		{
			var result = new ApiResult<TokenDto>();
			var model_token = _userManage.GetUserToken();
			if (model_token == null)
			{
                result.StatusCode = 4001;
                result.Message = _localizer["登录超时，请重新登录"];
                return result;
			}
			var model_User = _context.Queryable<WebUser>().InSingle(model_token.Userid);
			if (model_User == null)
			{
				result.StatusCode = 4001;
				result.Message = _localizer["登录超时，请重新登录"];
				return result;
			}
			result.Data = new TokenDto()
			{
				UserToken = model_token.Token,
				Status = model_User.Status,
				FirstName = model_token.FirstName,
				LastName = model_token.LastName
			};
			return result;
		}
		/// <summary>
		/// 登录口转用检测学生登录状态
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<CheckIsLoginDto>> CheckIsLogin()
		{
			var result = new ApiResult<CheckIsLoginDto>();
			try
			{
				result.Data = new CheckIsLoginDto();
				if (_userManage.Userid <= 0)
				{
					result.Data.Status = 0;
					result.Message = _localizer["未登录"];
					return result;
				}
				var model_User = _context.Queryable<WebUser>().InSingle(_userManage.Userid);
				if (model_User != null)
				{
					result.Data.Status = 1;
					result.Message = _localizer["已登录"];
				}
				else
				{
					result.Data.Status = 0;
					result.Message = _localizer["未登录"];
				}
			}
			catch (Exception ex)
			{
				result.Data.Status = 0;
				result.Message = _localizer["登录异常"];
			}
			return result;
		}
		#endregion

		#region "学生登录"
		/// <summary>
		/// 学生登录（邮箱）
		/// </summary>
		/// <param name="email"></param>
		/// <param name="pwd"></param>
		/// <param name="expiresin">-1为7天</param>
		/// <returns></returns>
		public async Task<ApiResult<TokenDto>> UserLoginEmail(string email, string pwd ,int expiresin = 7200)
		{
			var result = new ApiResult<TokenDto>();
			string userpwd = MD5Helper.MD5Encrypt32(pwd).ToLower();
			var model_User = _context.Queryable<WebUser>().First(u => u.Email == email && u.Status != -1);
			if (model_User != null)
			{
				if (model_User.Userpwd.ToLower() == userpwd)
                {
                    if (expiresin == -1) expiresin = 7 * 24 * 3600;
					var model_token = _userManage.SetUserToken(model_User.Userid, expiresin);
					result.Data = new TokenDto()
					{
						UserToken = model_token.Token,
						Status = model_User.Status,
						FirstName = model_token.FirstName,
						LastName = model_token.LastName
					};
					if (model_User.MenkeUserId > 0)
					{
						//同步该学生的所有课节
						await _menkeBaseService.LessonSync("", model_User.MenkeUserId);
					}
				}
				else {
					result.StatusCode = 4000;
					result.Message = _localizer["密码较验失败"];
				}
			}
			else
			{
				result.StatusCode = 4000;
				result.Message = _localizer["不存在此用户"];
			}
			return result;
		}
		/// <summary>
		/// 学生登录（手机+密码）
		/// </summary>
		/// <param name="mobileCode">手机区号</param>
		/// <param name="mobile">手机号</param>
		/// <param name="pwd"></param>
		/// <returns></returns>
		public async Task<ApiResult<TokenDto>> UserLoginMobile(string mobileCode,string mobile, string pwd, int expiresin = 7200)
		{
			var result = new ApiResult<TokenDto>();
			string userpwd = MD5Helper.MD5Encrypt32(pwd).ToLower();
			var model_User = _context.Queryable<WebUser>().First(u => u.MobileCode == mobileCode && u.Mobile== mobile && (u.Userpwd.ToLower() == userpwd || u.Userpwd==pwd) && u.Status != -1);
			if (model_User != null)
            {
                if (expiresin == -1) expiresin = 7 * 24 * 3600;
                var model_token = _userManage.SetUserToken(model_User.Userid, expiresin);

                result.Data = new TokenDto()
				{
					UserToken = model_token.Token,
					Status = model_User.Status,
					FirstName = model_token.FirstName,
					LastName = model_token.LastName
				};
				if (model_User.MenkeUserId > 0)
				{
					//同步该学生的所有课节
					await _menkeBaseService.LessonSync("", model_User.MenkeUserId);
				}
			}
			else
			{
				result.StatusCode = 4001;
				result.Message = _localizer["登录失败"];
			}
			return result;
		}

		/// <summary>
		/// 学生登录（手机+短信）
		/// </summary>
		/// <param name="mobileCode">手机区号</param>
		/// <param name="mobile">手机号</param>
		/// <param name="code">短信</param>
		/// <returns></returns>
		public async Task<ApiResult<TokenDto>> UserLoginSms(string mobileCode, string mobile, string code, int expiresin = 7200)
        {
            var result = new ApiResult<TokenDto>();
            if (string.IsNullOrEmpty(code) && code.Length != 6)
            {
                result.StatusCode = 4003;
                result.Message = _localizer["短信验证码异常!"];
                return result;
            }
            var model_User = _context.Queryable<WebUser>().First(u => u.MobileCode == mobileCode && u.Mobile == mobile && u.MobileSmscode== code && u.Status != -1);
			if (model_User != null)
            {
                if (expiresin == -1) expiresin = 7 * 24 * 3600;
                var model_token = _userManage.SetUserToken(model_User.Userid, expiresin);
				result.Data = new TokenDto()
				{
					UserToken = model_token.Token,
					Status = model_User.Status,
					FirstName = model_token.FirstName,
					LastName = model_token.LastName
				};
				model_User.MobileSmscode = "";
				_context.Updateable(model_User).UpdateColumns(u=>u.MobileSmscode).ExecuteCommand();
            }
			else
			{
				result.StatusCode = 4001;
				result.Message = _localizer["登录失败"];
			}
			return result;
		}
		#endregion

		#region "学生退出登录"
		/// <summary>
		/// 学生退出登录
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult> Logout()
		{
			var result = new ApiResult();

			if (_userManage.Userid == 0)
			{
				return result;
			}
			var model_User = _context.Queryable<WebUser>().InSingle(_userManage.Userid);
			if (model_User == null)
			{
				return result;
			}
			_context.Updateable<WebUser>().SetColumns(u => u.Token == "").Where(u => u.Userid == _userManage.Userid).ExecuteCommand();
			RedisServer.Token.Del(_userManage.UserToken);
			return result;
		}
		#endregion
		#endregion

		#region "平台网站"
		#region "普通用户注册(手机)"
		/// <summary>
		/// 普通用户注册
		/// </summary>
		/// <returns></returns>
		public async Task<ApiResult<TokenDto>> RegUser(string firstName, string lastName, string mobileCountry, string mobileCode, string mobile,string smsCode, string userpwd)
		{
			var result = new ApiResult<TokenDto>();
			var model_User = await _context.Queryable<WebUser>().FirstAsync(u => u.Mobile == mobile && u.MobileCode==mobileCode);
			if (model_User != null)
			{
				result.StatusCode = 4008;
				result.Message = _localizer["注册手机已存在"];
			}
			else
			{
				if (!VerifyHelper.IsPassword(userpwd))
				{
					result.StatusCode = 4003;
					result.Message = _localizer["密码必需包含字母和数字，且在6-18位之间"];
					return result;
				}
				if (model_User.MobileSmscode != smsCode || string.IsNullOrEmpty(smsCode)) {
                    result.StatusCode = 4003;
                    result.Message = _localizer["短信验证失败"];
                    return result;
                }
				if (model_User.MobileSmstime.AddMinutes(10) < DateTime.Now) {
                    result.StatusCode = 4003;
                    result.Message = _localizer["短信验证码超时失效"];
                    return result;
                }

				model_User = new WebUser();
				model_User.FirstName = firstName;
				model_User.LastName = lastName;
				model_User.Username = mobileCode + "-" + mobile;
                model_User.Mobile = mobile;
                model_User.MobileCode = mobileCode;
                model_User.MobileCountry = mobileCountry;
				model_User.MobileSmscode = "";

                model_User.Userpwd = MD5Helper.MD5Encrypt32(userpwd);
				model_User.Status = 1;
				model_User.Type = 1;//普通用户
                var userid = _context.Insertable(model_User).ExecuteReturnBigIdentity();
				var model_Token = _userManage.SetUserToken(userid);
				if (model_Token != null)
				{
					result.Data = new TokenDto();
					result.Data.UserToken = model_Token.Token;
				}
			}
			return result;
		}
		#endregion
		#endregion
	}
}