namespace WeTalk.Common.Helper
{
	public static class HtmlHelper
	{
		#region 去除富文本中的HTML标签
		/// <summary>
		/// 去除富文本中的HTML标签
		/// </summary>
		/// <param name="html"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string ReplaceHtmlTag(string html, int length = 0,string default_str="...")
		{
			string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
			strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

			if (length > 0 && strText.Length > length)
				return strText.Substring(0, length)+ default_str;

			return strText;
		}
		#endregion
	}
}
