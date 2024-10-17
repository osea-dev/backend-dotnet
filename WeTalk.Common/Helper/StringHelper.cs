using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WeTalk.Common.Helper
{
	public class StringHelper
	{
		/// <summary>
		/// 根据分隔符返回前n条数据
		/// </summary>
		/// <param name="content">数据内容</param>
		/// <param name="separator">分隔符</param>
		/// <param name="top">前n条</param>
		/// <param name="isDesc">是否倒序（默认false）</param>
		/// <returns></returns>
		public static List<string> GetTopDataBySeparator(string content, string separator, int top, bool isDesc = false)
		{
			if (string.IsNullOrEmpty(content))
			{
				return new List<string>() { };
			}

			if (string.IsNullOrEmpty(separator))
			{
				throw new ArgumentException("message", nameof(separator));
			}

			var dataArray = content.Split(separator).Where(d => !string.IsNullOrEmpty(d)).ToArray();
			if (isDesc)
			{
				Array.Reverse(dataArray);
			}

			if (top > 0)
			{
				dataArray = dataArray.Take(top).ToArray();
			}

			return dataArray.ToList();
		}
		/// <summary>
		/// 根据字段拼接get参数
		/// </summary>
		/// <param name="dic"></param>
		/// <returns></returns>
		public static string GetPars(Dictionary<string, object> dic)
		{

			StringBuilder sb = new StringBuilder();
			string urlPars = null;
			bool isEnter = false;
			foreach (var item in dic)
			{
				sb.Append($"{(isEnter ? "&" : "")}{item.Key}={item.Value}");
				isEnter = true;
			}
			urlPars = sb.ToString();
			return urlPars;
		}
		/// <summary>
		/// 根据字段拼接get参数
		/// </summary>
		/// <param name="dic"></param>
		/// <returns></returns>
		public static string GetPars(Dictionary<string, string> dic)
		{

			StringBuilder sb = new StringBuilder();
			string urlPars = null;
			bool isEnter = false;
			foreach (var item in dic)
			{
				sb.Append($"{(isEnter ? "&" : "")}{item.Key}={item.Value}");
				isEnter = true;
			}
			urlPars = sb.ToString();
			return urlPars;
		}
		/// <summary>
		/// 获取一个GUID
		/// </summary>
		/// <param name="format">格式-默认为N</param>
		/// <returns></returns>
		public static string GetGUID(string format = "N")
		{
			return Guid.NewGuid().ToString(format);
		}
		/// <summary>  
		/// 根据GUID获取19位的唯一数字序列  
		/// </summary>  
		/// <returns></returns>  
		public static long GetGuidToLongID()
		{
			byte[] buffer = Guid.NewGuid().ToByteArray();
			return BitConverter.ToInt64(buffer, 0);
		}
		/// <summary>
		/// 获取字符串最后X行
		/// </summary>
		/// <param name="resourceStr"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string GetCusLine(string resourceStr, int length)
		{
			string[] arrStr = resourceStr.Split("\r\n");
			return string.Join("", (from q in arrStr select q).Skip(arrStr.Length - length + 1).Take(length).ToArray());
		}

		#region 截取字符串
		/// <summary>
		/// 功能:截取字符串长度
		/// </summary>
		/// <param name="str">要截取的字符串</param>
		/// <param name="length">字符串长度</param>
		/// <param name="flg">true:加...,flase:不加</param>
		/// <returns></returns>
		public static string GetString(string str, int length, bool flg)
		{
			int i = 0, j = 0;
			foreach (char chr in str)
			{
				if ((int)chr > 127)
				{
					i += 2;
				}
				else
				{
					i++;
				}
				if (i > length)
				{
					str = str.Substring(0, j);
					if (flg)
						str += "...";
					break;
				}
				j++;
			}
			return str;
		}
		#endregion

		#region 截断字符串，尾部补上指定字串
		/// <summary>
		/// 截断字符串，尾部补上指定字串
		/// </summary>
		/// <returns></returns>
		public static string GetString(string str, int n, string s)
		{
			return (!string.IsNullOrEmpty(str) && (str.Length > n) ? str.Substring(0, n) + s : str);
		}
		#endregion

		#region 字符串分解
		/// <summary>
		/// 分解字符串为数组1
		/// </summary>
		/// <param name="strID"></param>
		/// <param name="index"></param>
		/// <param name="Separ"></param>
		/// <returns></returns>
		public static string StringSplit(string strings, int index, string Separ)
		{
			string[] s = strings.Split(Separ);
			if (s.Length <= (index+1)) return strings;
			return s[index];
		}
		/// <summary>
		/// 字符串分函数2
		/// </summary>
		/// <param name="str">要分解的字符串</param>
		/// <param name="splitstr">分割符,可以为string类型</param>
		/// <returns>字符数组</returns>
		public static string[] StringSplit(string str, string splitstr)
		{
			if (splitstr != "" & !(splitstr == null) & str != "" & !(str == null))
			{
				System.Collections.ArrayList c = new System.Collections.ArrayList();
				while (true)
				{
					int thissplitindex = str.IndexOf(splitstr);
					if (thissplitindex >= 0)
					{
						c.Add(str.Substring(0, thissplitindex));
						str = str.Substring((thissplitindex + splitstr.Length));
					}
					else
					{
						c.Add(str);
						break; // TODO: might not be correct. Was : Exit While
					}
				}
				string[] d = new string[c.Count];
				int i;
				for (i = 0; i <= c.Count - 1; i++)
				{
					d[i] = c[i].ToString();
				}
				return d;
			}
			else
			{
				return new string[] { str };
			}
		}
		//StringSplit


		/// <summary>
		/// 采用递归将字符串分割成数组
		/// </summary>
		/// <param name="strSource"></param>
		/// <param name="strSplit"></param>
		/// <param name="attachArray"></param>
		/// <returns></returns>
		private string[] StringSplit(string strSource, string strSplit, string[] attachArray)
		{
			string[] strtmp = new string[attachArray.Length + 1];
			attachArray.CopyTo(strtmp, 0);

			int index = strSource.IndexOf(strSplit, 0);
			if (index < 0)
			{
				strtmp[attachArray.Length] = strSource;
				return strtmp;
			}
			else
			{
				strtmp[attachArray.Length] = strSource.Substring(0, index);
				return StringSplit(strSource.Substring(index + strSplit.Length), strSplit, strtmp);
			}
		}
		#endregion

		#region "分隔数组并去重"
		/// <summary>
		/// 分隔成数组，并去重
		/// </summary>
		/// <param name="str"></param>
		/// <param name="splitstr"></param>
		/// <returns></returns>
		public static string ArrayDistinct(string str, string splitstr)
		{
			string[] arr = StringSplit(str, splitstr);
			string[] arr2 = arr.Distinct().ToArray();
			return string.Join(splitstr, arr2);
		}
        #endregion

        #region "将下划线后一个字母转大写"
        public static string ChgToUpper(string str)
        {
            var list_entity = str.Split("_").Select(u => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(u.ToLower())).ToList();
            return String.Join("", list_entity);
        }
        #endregion

        #region "将非第一位的大写字母转小写并在前加下划线"

        public static string ChgToUnderLine(string str)
        {
            return string.Concat(str.Select((x, i) => (i > 0 && char.IsUpper(x)) ? "_" + x.ToString().ToLower() : x.ToString()));
        }
        #endregion
    }
}
