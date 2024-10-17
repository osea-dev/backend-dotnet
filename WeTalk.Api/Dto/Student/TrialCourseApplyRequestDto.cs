using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
	/// <summary>
	/// 试听课申请入参
	/// </summary>
	public class TrialCourseApplyRequestDto
	{
		/// <summary>
		/// 试听课程名称
		/// </summary>
		public string courseName { get; set; }

		/// <summary>
		/// 手机区号
		/// </summary>
		[Required]
		public string mobileCode { get; set; }

		/// <summary>
		/// 手机号
		/// </summary>
		[Required]
		public string mobile { get; set; }

		/// <summary>
		/// 邮箱
		/// </summary>
		[Required(ErrorMessage = "邮箱不能为空")]
		public string email { get; set; }
        /// <summary>
        /// 学员出生日期
        /// </summary>
        [Required]
        public string birthdate { get; set; }
		/// <summary>
		/// 性别0女，1男
		/// </summary>
		public int gender { get; set; }
		/// <summary>
		/// 是否有汉语基础
		/// </summary>
		public int isChinese { get; set; }

	}
}
