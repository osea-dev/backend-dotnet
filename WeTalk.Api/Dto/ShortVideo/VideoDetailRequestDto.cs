using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.ShortVideo
{
	/// <summary>
	/// 短视频播放入参
	/// </summary>
	public class VideoDetailRequestDto
	{
        /// <summary>
        /// 短视频ID
        /// </summary>
        [Required]
        public long shortVideoid { get; set; }

		/// <summary>
		/// 排序方式:hot,views
		/// </summary>
		public string sortType { get; set; }
		/// <summary>
		/// 搜索关键词
		/// </summary>
		public string key { get; set; }
	}
}
