using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
	/// <summary>
	/// 取消课节申请入参
	/// </summary>
	public class ApplyCanalLessonRequestDto
    {
		/// <summary>
		/// 课节ID
		/// </summary>
		[Required]
		public long userLessonid { get; set; }
        /// <summary>
		/// 标签组
		/// </summary>
		public string[] keys { get; set; }
        /// <summary>
		/// 申请原因
		/// </summary>
		public string message { get; set; }
    }
}
