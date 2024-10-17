using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeTalk.Api.Dto.UserLogin;
using WeTalk.Interfaces.Services;
using WeTalk.Models;
using WeTalk.Models.Dto;
using WeTalk.Common.Helper;
using Microsoft.Extensions.Logging;
using System;
using WeTalk.Common;
using System.Linq;
using WeTalk.Models.Dto.User;

namespace WeTalk.Api
{
    /// <summary>
    /// 用户登录注册
    /// </summary>
    [Area("V1")]
	[Route("Api/[area]/[controller]/[action]")]
	[ApiController]
	public partial class UserLoginController : Base.WebApiController
	{
		private readonly IUserService _userService;
		private readonly IHttpContextAccessor _accessor;
		private readonly ILogger<UserLoginController> _logger;
		public UserLoginController(IHttpContextAccessor accessor,IUserService userService, ILogger<UserLoginController> logger) :base(accessor)
		{
			_userService = userService;
			_accessor = accessor;
			_logger = logger;
		}

		#region "注册"
		/// <summary>
		/// 注册（第一步）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<TokenDto>> Reg([FromBody] RegRequestDto data)
		{
			try
			{
				return await _userService.Reg(data.firstName, data.lastName, data.email, data.userpwd);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<TokenDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "注册-发送短信验证"
		/// <summary>
		/// 给当前登录用户发送短信验证
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult> SendSms([FromBody] SendSmsRequestDto data)
		{
			try
			{
				return await _userService.SendSms(data.MobileCode, data.Mobile);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "注册-完善信息"
		/// <summary>
		/// 注册-完善信息（同步创建拓课云账号）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
        [UserToken(Power = "")]
        public async Task<ApiResult> PerfectReg([FromBody] PerfectRegRequestDto data)
		{
			try
			{
				return await _userService.PerfectReg(data.countryCode, data.mobileCode, data.mobile, data.smsCode, data.birthdate, data.native, data.timezoneid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "忘记密码"
		/// <summary>
		/// 忘记密码(提交邮箱)
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult> ForgetPwd([FromBody] ForgetPwdRequestDto data)
		{
			try
			{
				return await _userService.ForgetPwd(data.email);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "重置密码"
		/// <summary>
		/// 通过临时授权码获取邮箱
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<EmailDto>> Email([FromBody] EmailRequestDto data)
		{
			try
			{
				return await _userService.Email(data.code);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<EmailDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		/// <summary>
		/// 重置密码
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult> ResetPwd([FromBody] ResetPwdRequestDto data)
		{
			return await _userService.ResetPwd(data.code, data.pwd, data.pwd1);
		}
		#endregion

		#region "检测学生登录状态"
		/// <summary>
		/// 检测学生登录状态
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<TokenDto>> CheckStatus()
		{
			try
			{
				return await _userService.CheckStatus();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<TokenDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "登录口专用检测学生登录状态"
		/// <summary>
		/// 登录口专用检测学生登录状态
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<CheckIsLoginDto>> CheckIsLogin()
		{
			return await _userService.CheckIsLogin();
		}
		#endregion

		#region "学生登录"
		/// <summary>
		/// 学生登录（邮箱）
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<ApiResult<TokenDto>> UserLoginEmail([FromBody] UserLoginEmailRequestDto data)
		{
			try
			{
				return await _userService.UserLoginEmail(data.email, data.userpwd, data.isLong == 1 ? -1 : 7200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<TokenDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}

        /// <summary>
        /// 学生登录（手机+密码）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
		public async Task<ApiResult<TokenDto>> UserLoginMobile([FromBody] UserLoginMobileRequestDto data)
		{
			try
			{
				return await _userService.UserLoginMobile(data.mobileCode, data.mobile, data.userpwd, data.isLong == 1 ? -1 : 7200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<TokenDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}

        /// <summary>
        /// 学生登录（手机+短信）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
		public async Task<ApiResult<TokenDto>> UserLoginSms([FromBody] UserLoginSmsRequestDto data)
		{
			try
			{
				return await _userService.UserLoginSms(data.mobileCode, data.mobile, data.smsCode, data.isLong == 1 ? -1 : 7200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult<TokenDto>() { StatusCode = 4012, Message = ex.Message };
			}
		}
		#endregion

		#region "学生退出登录"
		/// <summary>
		/// 学生退出登录
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[UserToken(Power = "")]
		public async Task<ApiResult> Logout()
		{
			try
			{
				return await _userService.Logout();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, GSRequestHelper.GetUrl(_accessor.HttpContext.Request));
				return new ApiResult() { StatusCode = 4012, Message = ex.Message };
			}
		}
        #endregion

        #region "后台模拟登录"
        /// <summary>
        /// 后台模拟登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> Redirect(string token)
		{
            var Headers = _accessor.HttpContext.Request.Headers.ToDictionary(k => k.Key, v => v.Value);
            var model = Headers.FirstOrDefault(u => u.Key == "Referer");
            var val = "";
            try
            {
                val = model.Value.ToString();
            }
            catch { }
            if (val.Contains("admin.wetalk.com") || val.Contains("localhost:5001"))
            {
                CookieHelper.SetCookie(_accessor.HttpContext, "UserToken", token);
                _accessor.HttpContext.Response.Redirect(Appsettings.app("Web:Host") + "/user/");
				return "";
            }
            else
            {
                _logger.LogError("非法模拟登录:" + IpHelper.GetCurrentIp(_accessor.HttpContext));
                return "Warning: illegal operation!";
            }
        }
		#endregion
	}
}
