using System;
using System.Text.RegularExpressions;

namespace WeTalk.Common.Helper
{
	public class VerifyHelper
	{
		#region 判断一个字符串是否是时间格式
		/// <summary>
		/// 判断一个字符串是否是时间格式
		/// </summary>
		/// <param name="strValue"></param>
		/// <returns></returns>
		public static bool IsDatetime(string strValue)
		{
			string strReg = @"([1-2][0-9][0-9][0-9])-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])\ (0[0-9]|1[0-9]|2[0-3]):(0[0-9]|[1-5][0-9]):(0[0-9]|[1-5][0-9])";
			if (strValue == "")
			{
				return false;
			}
			else
			{
				Regex re = new Regex(strReg);
				MatchCollection mc = re.Matches(strValue);
				if (mc.Count == 1)
					foreach (Match m in mc)
					{
						if (m.Value == strValue)
							return true;
					}
			}
			return false;
		}

		/// <summary>
		/// 判断一个字符串是否是日期格式
		/// </summary>
		/// <param name="strValue"></param>
		/// <returns></returns>
		public static bool IsDate(string strValue)
		{
			string strReg = @"([1-2][0-9][0-9][0-9])-(0[1-9]|1[0-2]|[1-9])-(0[1-9]|[12][0-9]|3[01]|[1-9])";
			if (strValue == "")
			{
				return false;
			}
			else
			{
				Regex re = new Regex(strReg);
				MatchCollection mc = re.Matches(strValue);
				if (mc.Count == 1)
					foreach (Match m in mc)
					{
						if (m.Value == strValue)
							return true;
					}
			}
			return false;
		}
		#endregion

		#region 检查一个字符串是否可以转化为日期，一般用于验证用户输入日期的合法性。
		/// <summary>
		/// 检查一个字符串是否可以转化为日期，一般用于验证用户输入日期的合法性。
		/// </summary>
		/// <param name="_value">需验证的字符串。</param>
		/// <returns>是否可以转化为日期的bool值。</returns>
		public static bool IsStringDate(string _value)
		{
			DateTime dt;
			try
			{
				dt = DateTime.Parse(_value);
			}
			catch (FormatException e)
			{
				//日期格式不正确时
				Console.WriteLine(e.Message);
				return false;
			}
			return true;
		}
		#endregion

		#region 检查一个字符串正整型。
		/// <summary>
		/// 检查一个字符串正整型
		/// </summary>
		/// <param name="_value">需验证的字符串。。</param>
		/// <returns>是否合法的bool值。</returns>
		public static bool IsAreInt(string _value)
		{
			return QuickValidate("^[1-9]*[0-9]*$", _value);
		}
		#endregion

		#region 检查一个字符串整型。
		/// <summary>
		/// 检查一个字符串整型
		/// </summary>
		/// <param name="_value">需验证的字符串。。</param>
		/// <returns>是否合法的bool值。</returns>
		public static bool IsInt(string _value)
		{
			return QuickValidate("^[+-]?[1-9]*[0-9]*$", _value);
		}
		#endregion

		#region 检查一个字符串是否是纯字母和数字构成的，一般用于查询字符串参数的有效性验证。
		/// <summary>
		/// 检查一个字符串是否是纯字母和数字构成的，一般用于查询字符串参数的有效性验证。
		/// </summary>
		/// <param name="_value">需验证的字符串。</param>
		/// <returns>是否合法的bool值。</returns>
		public static bool IsLetterOrNumber(string _value)
		{
			return QuickValidate("^[a-zA-Z0-9_]*$", _value);
		}
		#endregion

		#region 6-18位密码较验，纯字母和数字和字符构成。
		/// <summary>
		/// 6-18位密码较验，纯字母和数字和字符构成。
		/// </summary>
		/// <param name="_value">需验证的字符串。</param>
		/// <returns>是否合法的bool值。</returns>
		public static bool IsPassword(string _value)
		{
			return QuickValidate("^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z\\W]{6,18}$", _value);
		}
		#endregion

		#region 判断是否是数字，包括正负小数和整数(浮点数,Decimal)。
		/// <summary>
		/// 判断是否是正,数字，包括小数和整数(浮点数,Decimal)。(Decimal)
		/// </summary>
		/// <param name="_value">需验证的字符串。</param>
		/// <returns>是否合法的bool值。</returns>
		public static bool IsAreDecimal(string _value)
		{
			return QuickValidate("^[+-]?(0|([1-9]+[0-9]*))(.[0-9]+)?$", _value);
		}
		#endregion

		#region 判断是否是数字，包括小数和整数(浮点数,Decimal)。
		/// <summary>
		/// 判断是否是数字，包括小数和整数(浮点数,Decimal)。(Decimal)
		/// </summary>
		/// <param name="_value">需验证的字符串。</param>
		/// <returns>是否合法的bool值。</returns>
		public static bool IsDecimal(string _value)
		{
			return QuickValidate("^(0|([1-9]+[0-9]*))(.[0-9]+)?$", _value);
		}
		#endregion

		#region 中文检测

		/// <summary>
		/// 检测是否有中文字符
		/// </summary>
		/// <param name="inputData"></param>
		/// <returns></returns>
		public static bool IsHasCHZN(string _value)
		{
			return QuickValidate("[\u4e00-\u9fa5]", _value);
		}

		#endregion

		#region 快速验证一个字符串是否符合指定的正则表达式。
		/// <summary>
		/// 快速验证一个字符串是否符合指定的正则表达式。
		/// </summary>
		/// <param name="_express">正则表达式的内容。</param>
		/// <param name="_value">需验证的字符串。</param>
		/// <returns>是否合法的bool值。</returns>
		public static bool QuickValidate(string _express, string _value)
		{
			System.Text.RegularExpressions.Regex myRegex = new System.Text.RegularExpressions.Regex(_express);
			if (_value.Length == 0)
			{
				return false;
			}
			return myRegex.IsMatch(_value);
		}
		#endregion

		#region 判断是否是手机号
		/// <summary>
		/// 判断是否是手机号
		/// </summary>
		/// <param name="mobile"></param>
		/// <returns></returns>
		public static bool IsMobile(string mobile)
		{
			if (mobile.Length < 11) return false;
			if (mobile.Substring(0, 2) == "86") mobile = mobile.Substring(2, mobile.Length - 2);
			if (mobile.Substring(0, 3) == "+86") mobile = mobile.Substring(3, mobile.Length - 3);

			//1[3456789]\d{9}
			//校验手机号，号段主要有(不包括上网卡)：130~139、150~153，155~159，180~189、170~171、176~178。14号段为上网卡专属号段
			string strExp = @"((13[0-9])|(17[0-3,6-8])|(15[^4,\\D])|(18[0-9]))\d{8}";
			// Create a new Regex object.
			Regex r = new Regex(strExp);
			// Find a single match in the string.
			Match m = r.Match(mobile);
			if (m.Success)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		#region 判断是否是电子邮件
		/// <summary>
		/// 判断是否是电子邮件
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public static bool IsEmail(string email)
		{
			string strExp = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
			// Create a new Regex object.
			Regex r = new Regex(strExp);
			// Find a single match in the string.
			Match m = r.Match(email);
			if (m.Success)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion
	}
}
