using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
    /// <summary>
    /// 课节详情
    /// </summary>
    public class UserLessonDetailRequestDto
    {
		/// <summary>
		/// 课节ID
		/// </summary>
		[Required]
		public long userLessonid { get; set; }
    }
}
