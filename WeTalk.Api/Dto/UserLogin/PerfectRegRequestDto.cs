using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.UserLogin
{
	/// <summary>
	/// 注册
	/// </summary>
	public class PerfectRegRequestDto
	{

        /// <summary>
        /// 手机所在地的国家代码
        /// </summary>
        public string countryCode { get; set; }
        /// <summary>
        /// 手机区号
        /// </summary>
		public string mobileCode { get; set; }
		/// <summary>
		/// 手机号码
		/// </summary>
		public string mobile { get; set; }

		/// <summary>
		/// 验证码
		/// </summary>
		public string smsCode { get; set; }

		/// <summary>
		/// 生日
		/// </summary>
		public DateTime birthdate { get; set; }

		/// <summary>
		/// 出生地国家代码,例CN,EN(国家列表调)
		/// </summary>
		public string native { get; set; }
		/// <summary>
		/// 时区ID
		/// </summary>
		public long timezoneid { get; set; }

	}
}
