using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.News
{
	public class NewsListRequestDto
	{
		/// <summary>
		/// 新闻 分类ID
		/// </summary>
		[Required]
		public long newsCategoryid { get; set; }
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
