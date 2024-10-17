using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	public class UserCourseDto
    {
        /// <summary>
        /// 我的课程明细ID
        /// </summary>
        [Required]
        public long UserCourseid { get; set; }
        /// <summary>
        /// 课程ID
        /// </summary>
        [Required]
		public long Courseid { get; set; }
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
		/// 上课方式
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// 上课方式ID
		/// </summary>
		public long SkuTypeid { get; set; }
		/// <summary>
		/// 总课时
		/// </summary>
		public int ClassHour { get; set; }
		/// <summary>
		/// 已上课时
		/// </summary>
		public int Classes { get; set; }
		/// <summary>
		/// 学生课程状态:0锁定，1正常
		/// </summary>
		public int Status { get; set; }

		public List<string> Keys { get; set; }

    }
}
