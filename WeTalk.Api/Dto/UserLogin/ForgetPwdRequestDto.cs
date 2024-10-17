using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	public class ForgetPwdRequestDto
	{
		/// <summary>
		/// 邮箱
		/// </summary>
		[Required(ErrorMessage = "邮箱不能为空")]
		public string email { get; set; }

	}
}
