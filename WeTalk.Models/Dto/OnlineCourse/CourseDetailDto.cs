
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.OnlineCourse
{
	/// <summary>
	/// 直播课程详情
	/// </summary>
	public class CourseDetailDto
	{
		/// <summary>
		/// 课程ID
		/// </summary>
		public long OnlineCourseid { get; set; }
        /// <summary>
        /// Banner图(PC)
        /// </summary>
        public string Banner { get; set; }
        /// <summary>
        /// Banner图(手机)
        /// </summary>
        public string BannerH5 { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Img { get; set; }
        /// <summary>
        /// 课程标题
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
        /// 币种
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// 币种符号
        /// </summary>
        public string CurrencyIco { get; set; }
		/// <summary>
		/// 原价/市场价
		/// </summary>
		public decimal MarketPrice { get; set; }
		/// <summary>
		/// 价格
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// 折扣
		/// </summary>
		public string Discount { get; set; }
		/// <summary>
		/// 上课时间
		/// </summary>
		public string LessonStart { get; set; }
		/// <summary>
		/// 课时数
		/// </summary>
		public int LessonCount { get; set; }
		/// <summary>
		/// 学习人数
		/// </summary>
		public int StudentCount { get; set; }

		/// <summary>
		/// 内容介绍
		/// </summary>
		public string Intro { get; set; }
	}
}
