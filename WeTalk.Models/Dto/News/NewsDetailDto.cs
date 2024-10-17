
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.News
{
	/// <summary>
	/// 新闻
	/// </summary>
	public class NewsDetailDto
	{
        public NewsDetailDto()
        {
			Relations = new List<Relation>();
		}
		/// <summary>
		/// 新闻ID
		/// </summary>
		public long Newsid { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 视频
		/// </summary>
		public string Video { get; set; }
		/// <summary>
		/// 来源
		/// </summary>
		public string Source { get; set; }
		/// <summary>
		/// 详情介绍
		/// </summary>
		public string Intro { get; set; }
		/// <summary>
		/// 发布时间
		/// </summary>
		public int Sendtime { get; set; }
		/// <summary>
		/// 标签
		/// </summary>
		public List<string> Keys { get; set; }

		/// <summary>
		/// 上一篇
		/// </summary>
		public NewsAround BackNews { get; set; }
		/// <summary>
		/// 下一篇
		/// </summary>
		public NewsAround NextNews { get; set; }

		/// <summary>
		/// 相关新闻,最多5条
		/// </summary>
		public List<Relation> Relations { get; set; }

		/// <summary>
		/// 相关新闻
		/// </summary>
		public class NewsAround
		{
			/// <summary>
			/// 新闻ID
			/// </summary>
			public long Newsid { get; set; }
			/// <summary>
			/// 标题
			/// </summary>
			public string Title { get; set; }
			/// <summary>
			/// 外链
			/// </summary>
			public string Url { get; set; }
		}
		public class Relation
		{
			/// <summary>
			/// 新闻ID
			/// </summary>
			public long Newsid { get; set; }
			/// <summary>
			/// 标题
			/// </summary>
			public string Title { get; set; }
			/// <summary>
			/// 外链
			/// </summary>
			public string Url { get; set; }
			/// <summary>
			/// 缩略图
			/// </summary>
			public string Img { get; set; }
			/// <summary>
			/// 视频
			/// </summary>
			public string Video { get; set; }
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
}
