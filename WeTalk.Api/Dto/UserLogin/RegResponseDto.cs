using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	/// <summary>
	/// 注册
	/// </summary>
	public class RegResponseDto
	{
		/// <summary>
		/// 用户ID
		/// </summary>
		[Required(ErrorMessage = "用户ID不能为空")]
		public long userid { get; set; }
	}
}
