using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
	/// <summary>
	/// 正课申请入参
	/// </summary>
	public class CourseApplyRequestDto
    {
		/// <summary>
		/// 我的课程明细ID
		/// </summary>
		public long userCourseid { get; set; }

	}
}
