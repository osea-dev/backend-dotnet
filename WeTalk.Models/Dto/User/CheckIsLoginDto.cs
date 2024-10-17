using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.User
{
	/// <summary>
	/// 是否登录
	/// </summary>
	public class CheckIsLoginDto
	{
		/// <summary>
		/// 0未登录，1已登录
		/// </summary>
		public int Status { get; set; }
	}
}
