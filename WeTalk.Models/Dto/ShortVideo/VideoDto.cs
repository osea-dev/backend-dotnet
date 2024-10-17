
using System.Collections.Generic;

namespace WeTalk.Models.Dto.ShortVideo
{
	/// <summary>
	/// 短视频信息
	/// </summary>
	public class VideoDto
	{
		/// <summary>
		/// 在线直播课ID
		/// </summary>
		public long ShortVideoid { get; set; }
		/// <summary>
		/// 缩略图
		/// </summary>
		public string Img { get; set; }
		/// <summary>
		/// 短视频URL
		/// </summary>
		public string Video { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		public string Message { get; set; }
        
		/// <summary>
		/// 标签组
		/// </summary>
		public List<string> Keys { get; set; }


	}
}
