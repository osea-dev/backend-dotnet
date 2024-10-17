using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	public class MyOfflineCourseDto
	{
        /// <summary>
        /// 线下课程ID
        /// </summary>
        [Required]
        public long OfflineCourseid { get; set; }
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
		/// 上课时间
		/// </summary>
		public string LessonStart { get; set; }
		/// <summary>
		/// 关联老师ID
		/// </summary>
		public long Teacherid { get; set; }
		/// <summary>
		/// 关联老师姓名
		/// </summary>
		public string TeacherName { get; set; }

    }
}
