using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	public class EmailRequestDto
	{
		/// <summary>
		/// 临时授权码(邮箱码),2小时有效
		/// </summary>
		[Required]
		public string code { get; set; }

	}
}
