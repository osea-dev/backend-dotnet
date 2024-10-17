using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	/// <summary>
	/// 学生登录（邮箱登录）
	/// </summary>
	public class UserLoginEmailRequestDto
	{
        /// <summary>
        /// 邮箱或手机
        /// </summary>
        public string email { get; set; }
		/// <summary>
		/// 密码
		/// </summary>
		public string userpwd { get; set; }
		/// <summary>
		/// 是否保持登录:1是,0否
		/// </summary>
		public int isLong { get; set; }
	}
}
