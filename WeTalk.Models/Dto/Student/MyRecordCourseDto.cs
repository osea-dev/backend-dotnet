using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	public class MyRecordCourseDto
	{
		public MyRecordCourseDto()
		{
			Videos = new List<Info>();
		}
        /// <summary>
        /// 对应订单ID（调详情页时使用）
        /// </summary>
        [Required]
        public long Orderid { get; set; }
		/// <summary>
		/// 录播课程ID
		/// </summary>
		public long RecordCourseid { get; set; }
		/// <summary>
		/// 课程缩略图
		/// </summary>
		public string Img { get; set; }
		/// <summary>
		/// 课程标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 课程简介
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 课节数
		/// </summary>
		public int LessonCount { get; set; }
		/// <summary>
		/// 学习人数
		/// </summary>
		public int StudentCount { get; set; }
		/// <summary>
		/// 关联老师ID
		/// </summary>
		public long Teacherid { get; set; }
		/// <summary>
		/// 关联老师姓名
		/// </summary>
		public string TeacherName { get; set; }
		/// <summary>
		/// 标签数组
		/// </summary>
		public List<string> Keys { get; set; }
		/// <summary>
		/// 视频课程明细
		/// </summary>
		public List<Info> Videos { get; set; }

		public class Info
		{
			/// <summary>
			/// 录播课程ID
			/// </summary>
			public long RecordCourseid { get; set; }
			/// <summary>
			/// 视频标题
			/// </summary>
			public string Title { get; set; }
			/// <summary>
			/// 视频Url
			/// </summary>
			public string Video { get; set; }
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
