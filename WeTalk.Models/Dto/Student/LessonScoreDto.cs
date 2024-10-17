using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace WeTalk.Models.Dto.Student
{
	/// <summary>
	/// 取学生对老师的评分
	/// </summary>
	public class LessonScoreDto
    {
		/// <summary>
		/// 分值
		/// </summary>
		public int Score { get; set; }
		/// <summary>
		/// 0可评分，1已评分可修改，2过期不能改
		/// </summary>
        public int IsScore { get; set; }
		/// <summary>
		/// 老师头像
		/// </summary>
        public string TeacherHeadImg { get; set; }
        /// <summary>
        /// 老师名称
        /// </summary>
        public string TeacherName { get; set; }
		/// <summary>
		/// 有效修改时间(天)
		/// </summary>
        public int StudentScoreDays { get; set; }
    }
}
