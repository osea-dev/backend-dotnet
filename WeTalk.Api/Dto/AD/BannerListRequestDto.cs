using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.AD
{
	/// <summary>
	/// Banner广告
	/// </summary>
	public class BannerListRequestDto
	{
		/// <summary>
		/// 广告位置:<br/>
		/// 进贤进能Banner:teachers_banner<br/>
		/// </summary>
		[Required]
		public string type { get; set; }

	}
}
