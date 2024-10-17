
namespace WeTalk.Models.Dto.AD
{
	/// <summary>
	/// 广告信息
	/// </summary>
	public class AdDto
	{
		/// <summary>
		/// 广告ID
		/// </summary>
		public long Adid { get; set; }

		/// <summary>
		/// 图片
		/// </summary>
		public string Img { get; set; }
		/// <summary>
		/// 外链
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// 标题备注(alt)
		/// </summary>
		public string Title { get; set; }
	}
}
