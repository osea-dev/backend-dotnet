﻿
using System;
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Teacher
{
	/// <summary>
	/// 老师提交课节报告
	/// </summary>
	public class SubmitLessonReportDto
	{
		public SubmitLessonReportDto()
		{
			StudentReports = new List<StudentReport>();
		}
		/// <summary>
		/// 拓课云课节ID
		/// </summary>
		public int MenkeLessonId { get; set; }
		/// <summary>
		/// 操作报告临时授权码
		/// </summary>
		public string Code { get; set; }

		public List<StudentReport> StudentReports { get; set; }
		public class StudentReport
		{
			/// <summary>
			/// 报告ID
			/// </summary>
			public long UserLessonReportid { get; set; }
			/// <summary>
			/// 学生的排课课节ID
			/// </summary>
			public long UserLessonid { get; set; }
			/// <summary>
			/// 作业完成情况
			/// </summary>
			public int? Homework { get; set; }
			/// <summary>
			/// 注意力
			/// </summary>
			public int? Attention { get; set; }
			/// <summary>
			/// 积极性
			/// </summary>
			public int? Enthusiasm { get; set; }
			/// <summary>
			/// 听
			/// </summary>
			public int? Hear { get; set; }
			/// <summary>
			/// 说
			/// </summary>
			public int? Say { get; set; }
			/// <summary>
			/// 读
			/// </summary>
			public int? Read { get; set; }
			/// <summary>
			/// 写
			/// </summary>
			public int? Write { get; set; }
			/// <summary>
			/// 思
			/// </summary>
			public int? Thinking { get; set; }
			/// <summary>
			/// 情商
			/// </summary>
			public int? EmotionalQuotient { get; set; }
			/// <summary>
			/// 爱商
			/// </summary>
			public int? LoveQuotient { get; set; }
			/// <summary>
			/// 逆商
			/// </summary>
			public int? InverseQuotient { get; set; }
			/// <summary>
			/// 综合表现
			/// </summary>
			public int? Performance { get; set; }
			/// <summary>
			/// 文字评价
			/// </summary>
			public string Message { get; set; }
			/// <summary>
			/// 定级(仅试听课需要)
			/// </summary>
			public int? Level { get; set; }
		}
	}
}
