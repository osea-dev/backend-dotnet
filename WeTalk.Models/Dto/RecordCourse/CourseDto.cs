
using System.Collections.Generic;

namespace WeTalk.Models.Dto.RecordCourse
{
	/// <summary>
	/// 在线录播课程信息
	/// </summary>
	public class CourseDto
	{
		/// <summary>
		/// 在线录播课ID
		/// </summary>
		public long RecordCourseid { get; set; }
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
        /// 市场格
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 售格
        /// </summary>
        public decimal Price { get; set; }
		/// <summary>
		/// 标签组
		/// </summary>
		public List<string> Keys { get; set; }

		/// <summary>
		/// 课时数
		/// </summary>
		public int LessonCount { get; set; }
        /// <summary>
        /// 学习人数
        /// </summary>
        public int StudentCount { get; set; }

	}
}
