using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	/// <summary>
	/// 学习报告
	/// </summary>
	public class LessonReportDetailDto
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
		/// 老师ID
		/// </summary>
		public long Teacherid { get; set; }
		/// <summary>
		/// 老师头像
		/// </summary>
		public string TeacherHeadImg { get; set; }
		/// <summary>
		/// 老师名称
		/// </summary>
		public string TeacherName { get; set; }
		/// <summary>
		/// 老师评语
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 作业完成情况
		/// </summary>
		public int Homework { get; set; }
		/// <summary>
		/// 注意力
		/// </summary>
		public int Attention { get; set; }
		/// <summary>
		/// 积级性
		/// </summary>
		public int Enthusiasm { get; set; }
		/// <summary>
		/// 听
		/// </summary>
		public int Hear { get; set; }
		/// <summary>
		/// 说
		/// </summary>
		public int Say { get; set; }
		/// <summary>
		/// 读
		/// </summary>
		public int Read { get; set; }
		/// <summary>
		/// 写
		/// </summary>
		public int Write { get; set; }
		/// <summary>
		/// 思
		/// </summary>
		public int Thinking { get; set; }
		/// <summary>
		/// 情商
		/// </summary>
		public int EmotionalQuotient { get; set; }
		/// <summary>
		/// 爱商
		/// </summary>
		public int LoveQuotient { get; set; }
		/// <summary>
		/// 逆商
		/// </summary>
		public int InverseQuotient { get; set; }
		/// <summary>
		/// 综合表现
		/// </summary>
		public int Performance { get; set; }

		/// <summary>
		/// 课节回放视频
		/// </summary>
		public string RecordUrl { get; set; }
		/// <summary>
		/// 是否试听
		/// </summary>
        public int IsTrial { get; set; }
        /// <summary>
		/// 试听定级
		/// </summary>
		public int Level { get; set; }
        /// <summary>
        /// 报告时间
        /// </summary>
        public int Dtime { get; set; }
    }
}
