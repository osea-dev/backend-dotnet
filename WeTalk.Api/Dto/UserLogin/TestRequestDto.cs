using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	/// <summary>
	/// 测试
	/// </summary>
	public class TestRequestDto
	{
		/// <summary>
		/// 测试数据
		/// </summary>
		[Required]
		public string data { get; set; }
	}
}
