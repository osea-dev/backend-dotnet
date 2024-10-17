using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Teacher
{
	/// <summary>
	/// 老师详情入参
	/// </summary>
	public class TeacherDetailRequestDto
	{
		/// <summary>
		/// 老师ID
		/// </summary>
		[Required(ErrorMessage = "ID不能为空")]
		public long teacherid { get; set; }

	}
}
