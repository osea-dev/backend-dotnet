
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Course
{
	/// <summary>
	/// 前台子课程信息
	/// </summary>
	public class SubCourseDetailDto
	{
		/// <summary>
		/// 子课程ID
		/// </summary>
		public long CourseGroupInfoid { get; set; }
		/// <summary>
		/// 课程ID
		/// </summary>
		public long Courseid { get; set; }
		/// <summary>
		/// 缩略图
		/// </summary>
		public string Img { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 标签组
		/// </summary>
		public List<string> Keys { get; set; }
		/// <summary>
		/// 课程介绍
		/// </summary>
		public string Intro { get; set; }
		/// <summary>
		/// 课程目标
		/// </summary>
		public string Objectives { get; set; }
		/// <summary>
		/// 适合人群
		/// </summary>
		public string Crowd { get; set; }
		/// <summary>
		/// 课程特色
		/// </summary>
		public string Merit { get; set; }
		/// <summary>
		/// 开课时间
		/// </summary>
		public string Begintime { get; set; }
		/// <summary>
		/// 课程目录
		/// </summary>
		public string Catalog { get; set; }

	}
}
