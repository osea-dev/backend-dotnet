
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Course
{
	/// <summary>
	/// 众语课程信息
	/// </summary>
	public class CourseDto
	{
		public CourseDto()
		{
			SkuTypes = new List<SkuType>();
		}
		/// <summary>
		/// 课程ID
		/// </summary>
		public long Courseid { get; set; }
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
        /// 币种符号
        /// </summary>
        public string Ico { get; set; }
        /// <summary>
        /// 最低价格
        /// </summary>
        public decimal MinPrice { get; set; }
		/// <summary>
		/// 标签组
		/// </summary>
		public List<string> Keys { get; set; }

		/// <summary>
		/// 上课方式
		/// </summary>
		public List<SkuType> SkuTypes { get; set; }

		public class SkuType
        {
            /// <summary>
            /// 上课类型ID
            /// </summary>
            public long SkuTypeid { get; set; }
            /// <summary>
            /// 上课类型
            /// </summary>
            public string Type { get; set; }
		}
	}
}
