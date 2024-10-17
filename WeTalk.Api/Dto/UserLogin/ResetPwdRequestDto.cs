using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	public class ResetPwdRequestDto
	{
		/// <summary>
		/// 临时授权码(邮箱码)
		/// </summary>
		[Required]
		public string code { get; set; }
		/// <summary>
		/// 新密码
		/// </summary>
		[Required]
        [MinLength(6, ErrorMessage = "密码长度不能少于6位")]
        [MaxLength(20, ErrorMessage = "密码长度不能大于20位")]
        public string pwd { get; set; }
		/// <summary>
		/// 确认新密码
		/// </summary>
		[Required]
		[Compare("pwd",ErrorMessage ="两次密码必须相同")]
		public string pwd1 { get; set; }

	}
}
