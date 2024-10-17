using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Models.Dto.Student
{
	public class StudentDto
	{
		public StudentDto()
		{
		}
		/// <summary>
		/// 用户ID
		/// </summary>
		[Required]
		public long Userid { get; set; }
		/// <summary>
		/// 头像
		/// </summary>
		public string HeadImg { get; set; }
		/// <summary>
		/// 注册时间
		/// </summary>
		public int Regtime { get; set; }
		/// <summary>
		/// 姓
		/// </summary>
		public string FirstName { get; set; }
		/// <summary>
		/// 名
		/// </summary>
		public string LastName { get; set; }
		/// <summary>
		/// 出生日期
		/// </summary>
		public string Birthdate { get; set; }
		/// <summary>
		/// 性别:0女，1男
		/// </summary>
		public int Gender { get; set; }
		/// <summary>
		/// 居住地/时区
		/// </summary>
		public TimeZone Timezone { get; set; }
		/// <summary>
		/// 文化程度
		/// </summary>
		public string Education { get; set; }
		/// <summary>
		/// 出生地
		/// </summary>
		public string Native { get; set; }
		/// <summary>
		/// 出生地对应国家代码
		/// </summary>
		public string NativeCountryCode { get; set; }
		/// <summary>
		/// 监护人姓名
		/// </summary>
		public string GuardianName { get; set; }
		/// <summary>
		/// 监护人手机区号
		/// </summary>
		public string GuardianMobileCode { get; set; }
		/// <summary>
		/// 监护人手机
		/// </summary>
		public string GuardianMobile { get; set; }
		/// <summary>
		/// 监护人关系
		/// </summary>
		public string GuardianshipFee { get; set; }


		/// <summary>
		/// 邮箱
		/// </summary>
		public string Email { get; set; }
		/// <summary>
		/// 手机国家区号
		/// </summary>
		public string MobileCode { get; set; }
		/// <summary>
		/// 手机
		/// </summary>
		public string Mobile { get; set; }

		/// <summary>
		/// 时区对象
		/// </summary>
		public class TimeZone
		{
			/// <summary>
			/// 时区表ID
			/// </summary>
			public long Timezoneid { get; set; }
			/// <summary>
			/// 国家代码
			/// </summary>
			public string Code { get; set; }
            /// <summary>
			/// 国家
			/// </summary>
			public string Country { get; set; }
            /// <summary>
            /// 时区名
            /// </summary>
            public string TimezoneName { get; set; }
            /// <summary>
            /// UTC分差（如北京时间：(0-8)*60=-480)
            /// </summary>
            public int UtcSec { get; set; }
		}
	}
}
