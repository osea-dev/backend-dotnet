using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Teacher
{
	/// <summary>
	/// 老师头像
	/// </summary>
	public class TeacherHeadImgRequestDto
    {
		/// <summary>
		/// 老师ID
		/// </summary>
		[Required(ErrorMessage = "ID不能为空")]
		public long teacherid { get; set; }

	}
}
