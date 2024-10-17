using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models.Dto;
using WeTalk.Models;
using System;
using WeTalk.Models.Dto.User;

namespace WeTalk.Interfaces.Services
{
	public partial interface IUserService : IBaseService
	{
		Task<ApiResult<TokenDto>> Reg(string first_name, string last_name, string email, string userpwd);
		Task<ApiResult> PerfectReg(string countryCode, string mobile_code, string mobile, string sms_code, DateTime birthdate, string native, long timezoneid);
		Task<ApiResult> SendSms(string mobile_code,string mobile);
		Task<ApiResult<TokenDto>> UserLoginEmail(string email, string pwd, int expiresin = 7200);
		Task<ApiResult<TokenDto>> UserLoginMobile(string mobile_code, string mobile, string pwd, int expiresin = 7200);
		Task<ApiResult> ForgetPwd(string email);
		Task<ApiResult> ResetPwd(string tmp_code, string userpwd, string userpwd1);
		Task<ApiResult<TokenDto>> UserLoginSms(string mobile_code, string mobile, string code, int expiresin = 7200);
        Task<ApiResult<TokenDto>> CheckStatus();
        Task<ApiResult> Logout();
		Task<ApiResult<EmailDto>> Email(string tmpCode);
		Task<ApiResult<CheckIsLoginDto>> CheckIsLogin();
	}
}