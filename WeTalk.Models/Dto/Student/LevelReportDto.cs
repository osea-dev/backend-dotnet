using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	/// <summary>
	/// 定级报告(仅针对试听)
	/// </summary>
	public class LevelReportDto
	{
		/// <summary>
		/// 学习报告ID
		/// </summary>
		public long UserLessonReportid { get; set; }
		/// <summary>
		/// 排课课节ID
		/// </summary>
		public long UserLessonid { get; set; }
		/// <summary>
		/// 课节名称
		/// </summary>
		public string MenkeLessonName { get; set; }
		/// <summary>
		/// 级别
		/// </summary>
		public int Level { get; set; }
		/// <summary>
		/// 试听
		/// </summary>
		public int Istrial { get; set; }
		/// <summary>
		/// 出报告时间
		/// </summary>
		public int Dtime { get; set; }
	}
}
