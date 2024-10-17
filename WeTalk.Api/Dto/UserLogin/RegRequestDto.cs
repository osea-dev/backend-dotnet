using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	/// <summary>
	/// 注册
	/// </summary>
	public class RegRequestDto 
	{
		/// <summary>
		/// 姓
		/// </summary>
		[Required(ErrorMessage = "姓名不能为空")]
		public string firstName { get; set; }

		/// <summary>
		/// 名字
		/// </summary>
		[Required(ErrorMessage = "名字不能为空")]
		public string lastName { get; set; }

		/// <summary>
		/// 邮箱
		/// </summary>
		[Required(ErrorMessage = "邮箱不能为空")]
		public string email { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		[Required(ErrorMessage = "密码不能为空")]
		public string userpwd { get; set; }
	}
}
