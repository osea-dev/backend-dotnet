using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.AD
{
	/// <summary>
	/// 多广告位
	/// </summary>
	public class AdListRequestDto
	{
		/// <summary>
		/// 广告位置:<br/>
		/// 进贤进能老师介绍:teachers_img<br/>
		/// </summary>
		[Required]
		public string type { get; set; }

	}
}
