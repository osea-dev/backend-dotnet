using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.User
{
	public class StudentHomePageDto
	{
		public StudentHomePageDto()
		{
			TrialLessonReport = new LessonReport();
			LastUserLesson = new UserLesson();
			UserCourses = new List<UserCourse>();
			EndUserLessons = new List<UserLesson>();
		}
		///// <summary>
		///// 用户ID
		///// </summary>
		//[Required]
		//public long Userid { get; set; }
		///// <summary>
		///// 邮箱
		///// </summary>
		//public string Email { get; set; }
		///// <summary>
		///// 手机国家区号
		///// </summary>
		//public string MobileCode { get; set; }
		///// <summary>
		///// 手机
		///// </summary>
		//public string Mobile { get; set; }
		///// <summary>
		///// 头像
		///// </summary>
		//public string HeadImg { get; set; }
		///// <summary>
		///// 姓
		///// </summary>
		//public string FirstName { get; set; }
		///// <summary>
		///// 名
		///// </summary>
		//public string LastName { get; set; }
		///// <summary>
		///// 出生地
		///// </summary>
		//[Required]
		//public string native { get; set; }
		///// <summary>
		///// 居住地（通过IP获得）
		///// </summary>
		//public string Residence { get; set; }
		/// <summary>
		/// 学习报告总数
		/// </summary>
		public int ReportTotal { get; set; }
		/// <summary>
		/// 最近试听课报告
		/// </summary>
		public LessonReport TrialLessonReport { get; set; }
		/// <summary>
		/// 最近一节课
		/// </summary>
		public UserLesson LastUserLesson { get; set; }
		/// <summary>
		/// 我的课程列表
		/// </summary>
		public List<UserCourse> UserCourses{get;set; }
		/// <summary>
		/// 近期已完成课节
		/// </summary>
		public List<UserLesson> EndUserLessons { get; set; }

		/// <summary>
		/// 试听课报告
		/// </summary>
		public class LessonReport
		{
			/// <summary>
			/// 试听状态：
			/// 0未试听过(或已申请了超过1小时，客服还没有排课)，
			/// 1已申请但还未试听(已排课状态:未开始\进行中\已结课)，
			/// 2已试听但报告未出(已排课状态:已结课)
			/// 3已出报告(有报告)
			/// </summary>
			public int Status { get;set;}
			/// <summary>
			/// 课程报告ID
			/// </summary>
			public long UserLessonReportid { get; set; }
			/// <summary>
			/// 定级
			/// </summary>
			public int Level { get; set; }
		}

		/// <summary>
		/// 已购课程信息
		/// </summary>
		public class UserCourse
		{
			/// <summary>
			/// 已购课程ID
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
			/// 已上课时
			/// </summary>
			public int Classes { get; set; }
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
			/// 1正常，2锁定
			/// </summary>
			public int Status { get; set; }
		}
		/// <summary>
		/// 课节排课信息
		/// </summary>
		public class UserLesson
		{
			/// <summary>
			/// 课节排课ID
			/// </summary>
			public long UserLessonid { get; set; }
			/// <summary>
			/// 课程名称
			/// </summary>
			public string MenkeName { get; set; }
            /// <summary>
            /// 课程图片
            /// </summary>
            public string Img { get; set; }
            /// <summary>
            /// 课节名称
            /// </summary>
            public string MenkeLessonName { get; set; }
			/// <summary>
			/// 上课开始时间
			/// </summary>
			public int MenkeStarttime { get; set; }
			/// <summary>
			/// 上课结束时间
			/// </summary>
			public int MenkeEndtime { get; set; }
			/// <summary>
			/// 是否允许申请取消
			/// </summary>
			public int? IsCancel { get; set; }
			/// <summary>
			/// 上课老师
			/// </summary>
			public string MenkeTeacherName { get; set; }
            /// <summary>
            /// 课程开始前多久进入教室（分钟）
            /// </summary>
            public int? ClassroomMin { get; set; }
			/// <summary>
			/// 教室免密URL（学生端）
			/// </summary>
			public string MenkeEntryurl { get; set; }
			/// <summary>
			/// 学生对老师的评分,0未评
			/// </summary>
			public int IsScore { get; set; }
			/// <summary>
			/// 课节报告状态，0未出，1已出
			/// </summary>
			public int IsReport { get; set; }
			/// <summary>
			/// 课节报告ID
			/// </summary>
			public long UserLessonReportid { get; set; }
			/// <summary>
			/// 课节回放URL
			/// </summary>
			public string ReplayUrl { get; set; }
		}
	}
}
