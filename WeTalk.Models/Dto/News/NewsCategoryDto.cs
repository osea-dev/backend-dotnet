
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.News
{
	/// <summary>
	/// 新闻分类
	/// </summary>
	public class NewsCategoryDto
	{
		/// <summary>
		/// 新闻分类ID
		/// </summary>
		public long NewsCategoryid { get; set; }
		/// <summary>
		/// 分类标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 分类下新闻数量
		/// </summary>
		public int Total { get; set; }
	}
}
