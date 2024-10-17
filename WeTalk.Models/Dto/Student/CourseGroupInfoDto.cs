using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	/// <summary>
	/// 网站子课程信息
	/// </summary>
	public class CourseGroupInfoDto
	{
		/// <summary>
		/// 课程ID
		/// </summary>
		public long Courseid { get; set; }
		/// <summary>
		/// 子课程ID
		/// </summary>
		public long CourseGroupInfoid { get; set; }
		/// <summary>
		/// 子课程缩略图
		/// </summary>
		public string Img { get; set; }
		/// <summary>
		/// 子课程标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 子课程简介
		/// </summary>
		public string Message { get; set; }
	}
}
