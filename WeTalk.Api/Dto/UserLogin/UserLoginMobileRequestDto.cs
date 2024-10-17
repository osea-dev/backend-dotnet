using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	/// <summary>
	/// 学生登录（手机登录）
	/// </summary>
	public class UserLoginMobileRequestDto
	{
		/// <summary>
		/// 手机区号
		/// </summary>
		public string mobileCode { get; set; }
		/// <summary>
		/// 手机
		/// </summary>
		public string mobile { get; set; }
		/// <summary>
		/// 密码
		/// </summary>
		[Required]
		public string userpwd { get; set; }
        /// <summary>
        /// 是否保持登录:1是,0否
        /// </summary>
        public int isLong { get; set; }
    }
}
