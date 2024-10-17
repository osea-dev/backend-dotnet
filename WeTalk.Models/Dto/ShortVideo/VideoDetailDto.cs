
using System.Collections.Generic;

namespace WeTalk.Models.Dto.ShortVideo
{
	/// <summary>
	/// 短视频详情信息
	/// </summary>
	public class VideoDetailDto
	{
		/// <summary>
		/// 短视频ID
		/// </summary>
		public long ShortVideoid { get; set; }
		/// <summary>
		/// 缩略图
		/// </summary>
		public string Img { get; set; }
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
		/// <summary>
		/// 视频路径
		/// </summary>
		public string Video { get; set; }

		/// <summary>
		/// 访问/播放量
		/// </summary>
		public int Hits { get; set; }
		/// <summary>
		/// 上一个短视频ID
		/// </summary>
		public long BackShortVideoid { get; set; }
		/// <summary>
		/// 下一个短视频ID
		/// </summary>
		public long NextShortVideoid { get; set; }
	}
}
