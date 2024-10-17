
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Course
{
	/// <summary>
	/// 前台课程详情
	/// </summary>
	public class CourseDetailDto
	{
		public CourseDetailDto()
		{
			Skus = new List<CourseSku>();
			Groups = new List<CourseGroup>();
		}
		/// <summary>
		/// 课程ID
		/// </summary>
		public long Courseid { get; set; }
        /// <summary>
        /// Banner图(PC)
        /// </summary>
        public string Banner { get; set; }
        /// <summary>
        /// Banner图(手机)
        /// </summary>
        public string BannerH5 { get; set; }
        /// <summary>
        /// 课程标题
        /// </summary>
        public string Title { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 课程体系
		/// </summary>
		public string Curricula { get; set; }
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
        /// SKU组合
        /// </summary>
        public List<CourseSku> Skus { get; set; }
		/// <summary>
		/// 课程组
		/// </summary>
		public List<CourseGroup> Groups { get; set; }

		/// <summary>
		/// SKU
		/// </summary>
		public class CourseSku
		{
			/// <summary>
			/// 上课类型ID
			/// </summary>
			public long CourseSkuid { get; set; }
			/// <summary>
			/// 上课类型ID
			/// </summary>
			public long SkuTypeid { get; set; }
			/// <summary>
			/// 上课类型
			/// </summary>
			public string Type { get; set; }
			/// <summary>
			/// 课时
			/// </summary>
			public int ClassHour { get; set; }
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
		}

		/// <summary>
		/// 课程组
		/// </summary>
		public class CourseGroup
		{
			public CourseGroup()
			{
				GroupInfos = new List<CourseGroupInfo>();
			}
			/// <summary>
			/// 课程组ID
			/// </summary>
			public long CourseGroupid { get; set; }
			/// <summary>
			/// 课程组名称
			/// </summary>
			public string GroupName { get; set; }
			/// <summary>
			/// 子课程列表
			/// </summary>
			public List<CourseGroupInfo> GroupInfos { get; set; }
		}

		/// <summary>
		/// 子课程
		/// </summary>
		public class CourseGroupInfo
		{
			/// <summary>
			/// 子课程ID
			/// </summary>
			public long CourseGroupInfoid { get; set; }
			/// <summary>
			/// 课程组ID
			/// </summary>
			public long CourseGroupid { get; set; }
			/// <summary>
			/// 子课程标题
			/// </summary>
			public string Title { get; set; }
			/// <summary>
			/// 缩略
			/// </summary>
			public string Img { get; set; }
			/// <summary>
			/// 简介
			/// </summary>
			public string Message { get; set; }
			/// <summary>
			/// 子课程标签组
			/// </summary>
			public List<string> Keys { get; set; }
			/// <summary>
			/// 等级标签
			/// </summary>
			public string Level { get; set; }
		}
	}
}
