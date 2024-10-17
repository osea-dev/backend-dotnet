using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Api.Dto.Student
{
	public class StudentRequestDto
	{
		public StudentRequestDto()
		{
		}
		/// <summary>
		/// 密码
		/// </summary>
		public string userpwd { get; set; }
		/// <summary>
		/// 头像
		/// </summary>
		public string headImg { get; set; }
		/// <summary>
		/// 姓
		/// </summary>
		public string firstName { get; set; }
		/// <summary>
		/// 名
		/// </summary>
		public string lastName { get; set; }
		/// <summary>
		/// 出生日期
		/// </summary>
		public DateTime? birthdate { get; set; }
		/// <summary>
		/// 性别:0女，1男
		/// </summary>
		public int? gender { get; set; }
		/// <summary>
		/// 出生地国家代码
		/// </summary>
		public string native { get; set; }
		/// <summary>
		/// 居住地时区ID
		/// </summary>
		public long timezoneid { get; set; }
		/// <summary>
		/// 文化程度
		/// </summary>
		public string education { get; set; }
		/// <summary>
		/// 母语(语言代码)
		/// </summary>
		public string nativeLang { get; set; }
		/// <summary>
		/// 监护人姓名
		/// </summary>
		public string guardianName { get; set; }
		/// <summary>
		/// 监护人手机区号
		/// </summary>
		public string guardianMobileCode { get; set; }
		/// <summary>
		/// 监护人手机
		/// </summary>
		public string guardianMobile { get; set; }
		/// <summary>
		/// 监护人手机验证码（如要修改监护人手机则必填）
		/// </summary>
		public string guardianSmsCode { get; set; }
		/// <summary>
		/// 监护人关系
		/// </summary>
		public string guardianshipFee { get; set; }


		/// <summary>
		/// 邮箱
		/// </summary>
		public string email { get; set; }
		/// <summary>
		/// 手机所在地国家代码，如CN
		/// </summary>
		public string countryCode { get; set; }
		/// <summary>
		/// 手机国家区号
		/// </summary>
		public string mobileCode { get; set; }
		/// <summary>
		/// 手机
		/// </summary>
		public string mobile { get; set; }
		/// <summary>
		/// 手机验证码（如要修改手机则必填）
		/// </summary>
		public string smsCode { get; set; }
		
	}
}
