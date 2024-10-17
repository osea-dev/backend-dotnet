using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	/// <summary>
	/// 时区列表
	/// </summary>
	public class TimeZoneResponseDto
	{
		/// <summary>
		/// 时区表ID
		/// </summary>
		[Required]
		public long timeZoneid { get; set; }
		/// <summary>
		/// 时区城市
		/// </summary>
		[Required]
		public string title { get; set; }
		/// <summary>
		/// 时区秒差
		/// </summary>
		[Required]
		public string utcSec { get; set; }
	}
}
