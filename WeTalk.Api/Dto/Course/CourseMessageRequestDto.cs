using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Course
{
	/// <summary>
	/// 课程留言
	/// </summary>
	public class CourseMessageRequestDto
	{
		/// <summary>
		/// 子课程ID
		/// </summary>
		[Required]
		public long courseGroupInfoid { get; set; }

		/// <summary>
		/// 手机区号
		/// </summary>
		[Required(ErrorMessage = "请输入正确的手机区号")]
		[StringLength(3,MinimumLength = 2,ErrorMessage = "请输入正确的手机区号")]
		public string mobileCode { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage = "请输入正确的手机号")]
        [StringLength(20,MinimumLength = 6, ErrorMessage = "请输入正确的手机号")]
        public string mobile { get; set; }

		/// <summary>
		/// 邮箱
		/// </summary>
		[Required(ErrorMessage = "请输入邮箱")]
		[EmailAddress(ErrorMessage = "邮箱格式错误")]
		public string email { get; set; }

	}
}
