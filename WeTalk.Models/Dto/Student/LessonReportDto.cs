using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	/// <summary>
	/// 学习报告
	/// </summary>
	public class LessonReportDto
	{
		public LessonReportDto()
		{
			LessonReports=new List<Report>();
		}
		/// <summary>
		/// 我的课程ID
		/// </summary>
		public long UserCourseid { get; set; }
		/// <summary>
		/// 课程名称
		/// </summary>
		public string CourseName { get; set; }
		/// <summary>
		/// 总课时
		/// </summary>
		public int ClassHour { get; set; }
		/// <summary>
		/// 上课类型ID
		/// </summary>
		public long SkuTypeid { get; set; }
		/// <summary>
		/// 上课类型
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// 上课类型图标
		/// </summary>
		public string IcoFont { get; set; }
		/// <summary>
		/// 课节报告列表
		/// </summary>
		public List<Report> LessonReports{ get; set; }
		public class Report
		{
			/// <summary>
			/// 学习报告ID
			/// </summary>
			public long UserLessonReportid { get; set; }
			/// <summary>
			/// 开始上课时间
			/// </summary>
			public int MenkeStarttime { get; set; }
			/// <summary>
			/// 结束上课时间
			/// </summary>
			public int MenkeEndtime { get; set; }
			/// <summary>
			/// 课节名称
			/// </summary>
			public string LessonName { get; set; }
			/// <summary>
			/// 出报告时间
			/// </summary>
			public int Dtime { get; set; }
			/// <summary>
			/// 学生是否对老师评价
			/// </summary>
			public int IsScore { get; set; }
			/// <summary>
			/// 课节回放视频
			/// </summary>
			public string RecordUrl { get; set; }
		}
	}
}
