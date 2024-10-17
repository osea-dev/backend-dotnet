using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	public class UserLoginResponseDto
	{
		/// <summary>
		/// 用户ID
		/// </summary>
		[Required]
		public long userid { get; set; }
		/// <summary>
		/// 用户登录状态Token
		/// </summary>
		[Required]
		public string token { get; set; }
	}
}
