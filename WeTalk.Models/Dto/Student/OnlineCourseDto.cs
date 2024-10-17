using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	public class OnlineCourseDto
	{
        /// <summary>
        /// 我的课程明细ID
        /// </summary>
        [Required]
        public long UserCourseid { get; set; }
        /// <summary>
        /// 在线直播课程ID
        /// </summary>
        [Required]
		public long OnlineCourseid { get; set; }
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
		/// 学生课程状态:0锁定，1正常
		/// </summary>
		public int Status { get; set; }

    }
}
