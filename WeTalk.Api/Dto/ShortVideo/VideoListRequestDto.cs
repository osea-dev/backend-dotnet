using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.ShortVideo
{
	/// <summary>
	/// 短视频列表入参
	/// </summary>
	public class VideoListRequestDto
	{
		/// <summary>
		/// 排序方式:hot,views
		/// </summary>
		public string sortType { get; set; }
		/// <summary>
		/// 搜索关键词
		/// </summary>
		public string key { get; set; }

		/// <summary>
		/// 页码
		/// </summary>
		[DefaultValue(1)]
		public int page { get; set; } = 1;

		/// <summary>
		/// 每页条数
		/// </summary>
		[DefaultValue(10)]
		public int pageSize { get; set; } = 10;
	}
}
