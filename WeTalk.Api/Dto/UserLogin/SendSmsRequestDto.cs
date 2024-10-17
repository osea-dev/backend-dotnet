using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	/// <summary>
	/// 手机短信
	/// </summary>
	public class SendSmsRequestDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string MobileCode { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Mobile { get; set; }

	}
}
