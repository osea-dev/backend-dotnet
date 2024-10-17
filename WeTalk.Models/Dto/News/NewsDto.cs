
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.News
{
	/// <summary>
	/// 新闻列表项
	/// </summary>
	public class NewsDto
	{
		/// <summary>
		/// 新闻ID
		/// </summary>
		public long Newsid { get; set; }
		/// <summary>
		/// 缩略图
		/// </summary>
		public string Img { get; set; }
		/// <summary>
		/// 视频地址
		/// </summary>
		public string Video { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 外链
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 发布时间
		/// </summary>
		public int Sendtime { get; set; }
	}
}
