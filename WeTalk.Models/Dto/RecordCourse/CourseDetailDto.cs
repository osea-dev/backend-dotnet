
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace WeTalk.Models.Dto.RecordCourse
{
	/// <summary>
	/// 录播课程详情
	/// </summary>
	public class CourseDetailDto
	{
		public CourseDetailDto()
		{
			Videos = new List<Info>();
		}

		/// <summary>
		/// 课程ID
		/// </summary>
		public long RecordCourseid { get; set; }
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
		/// 学习人数
		/// </summary>
		public int StudentCount { get; set; }
		/// <summary>
		/// 课时数
		/// </summary>
		public int LessonCount { get; set; }

		/// <summary>
		/// 内容介绍(上半部)
		/// </summary>
		public string IntroUp { get; set; }
		/// <summary>
		/// 内容介绍（下半部）
		/// </summary>
		public string IntroLow { get; set; }
		/// <summary>
		/// 视频课程明细
		/// </summary>
		public List<Info> Videos { get; set; }

		public class Info { 
			/// <summary>
			/// 视频标题
			/// </summary>
			public string Title { get; set; }
			/// <summary>
			/// 视频时长
			/// </summary>
			public string Duration { get; set; }
			/// <summary>
			/// 观看人次
			/// </summary>
			public int ViewCount { get; set; }

		}
	}
}
