using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
	/// <summary>
	/// 学生对课节老师评价打分
	/// </summary>
	public class ToTeacherSorceRequestDto
	{
		/// <summary>
		/// 课节ID
		/// </summary>
		[Required]
		public long userLessonid { get; set; }
		/// <summary>
		/// 申请原因
		/// </summary>
		[Required]
		public int score { get; set; }
    }
}
