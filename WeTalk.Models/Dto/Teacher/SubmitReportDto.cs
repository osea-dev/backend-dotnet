
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Teacher
{
	/// <summary>
	/// 老师提交课节报告返回的对象
	/// </summary>
	public class SubmitReportDto
    {
        /// <summary>
        /// 报告ID
        /// </summary>
        public long UserLessonReportid { get; set; }
        /// <summary>
        /// 学生的排课课节ID
        /// </summary>
        public long UserLessonid { get; set; }
    }
}
