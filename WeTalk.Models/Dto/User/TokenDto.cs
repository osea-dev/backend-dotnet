using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.User
{
	/// <summary>
	/// 注册
	/// </summary>
	public class TokenDto
	{
		/// <summary>
		/// 用户Token
		/// </summary>
		public string UserToken { get; set; }
		/// <summary>
		/// 姓
		/// </summary>
		public string FirstName { get; set; }
		/// <summary>
		/// 名
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// 0未完善资料，1正常
		/// </summary>
		public int Status { get; set; }
	}
}
