using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	/// <summary>
	/// 正式课课节
	/// </summary>
	public class UserLessonDto
	{
		/// <summary>
		/// 我的课节ID
		/// </summary>
		public long UserLessonid { get; set; }
		/// <summary>
		/// 课程名称
		/// </summary>
		public string CourseName { get; set; }
		/// <summary>
		/// 课节名称
		/// </summary>
		public string LessonName { get; set; }
		/// <summary>
		/// 上课开始时间
		/// </summary>
		public int MenkeStarttime { get; set; }
		/// <summary>
		/// 上课结束时间
		/// </summary>
		public int MenkeEndtime { get; set; }
		/// <summary>
		/// 上课类型
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// 是否试听课
		/// </summary>
		public int? Istrial { get; set; }
		/// <summary>
		/// 课节状态（1未开始2进行中3已结课4已过期/缺席）
		/// </summary>
		public int MenkeState { get; set; }
		/// <summary>
		/// 按扭:是否允许取消课节
		/// </summary>
		public int IsCancel { get; set; }
        /// <summary>
        /// 按扭:进入教室URL
        /// </summary>
        public string MenkeEntryurl { get; set; }
        /// <summary>
        /// 提前多长时间进教室（分钟）
        /// </summary>
        public int ClassroomMin { get; set; }
        /// <summary>
        /// 按扭:0未评，1已评，2过期
        /// </summary>
        public int? IsScore { get; set; }
        /// <summary>
        /// 按扭:老师是否布署家庭作业
        /// </summary>
        public int? IsHomeWork { get; set; }
        /// <summary>
        /// 按扭:是否出课节报告
        /// </summary>
        public int? IsReport { get; set; }
		/// <summary>
		/// 课节报告ID
		/// </summary>
		public long UserLessonReportid { get; set; }
		/// <summary>
		/// 按扭:课程回放URL
		/// </summary>
		public string RecordUrl { get; set; }
        /// <summary>
        /// 课节老师
        /// </summary>
        public long? Teacherid { get; set; }
		/// <summary>
		/// 课节老师
		/// </summary>
		public string TeacherName { get; set; }
        /// <summary>
        /// 课节老师头像
        /// </summary>
        public string TeacherHeadImg { get; set; }
        /// <summary>
        /// 课节老师简介
        /// </summary>
        public string TeacherMessage { get; set; }
		/// <summary>
		/// 课节老师标签
		/// </summary>
		public List<string> TeacherKeys { get; set; }

	}
}
