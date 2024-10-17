using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeTalk.Common.Helper
{
	/// <summary>
	/// 远程文件抓取类
	/// </summary>
	public class GetRemoteHelper
	{

		#region 是否手机浏览器
		/// <summary>
		/// 是否手机浏览器
		/// </summary>
		/// <returns>0PC，1手机，2微信</returns>
		public static int GetBrowser(HttpRequest req)
		{
			int n = 0;
			Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

			var userAgent = req.Headers["User-Agent"].ToString().ToLower();
			if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
			{
				n = 1;
			}
			if (userAgent.Contains("micromessenger")) n = 2;
			return n;
		}
		#endregion

		#region 日期随机函数
		/**********************************
         * 函数名称:DateRndName
         * 功能说明:日期随机函数
         * 参    数:ra:随机数
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          Random ra = new Random();
         *          string s = o.DateRndName(ra);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/
		/// <summary>
		/// 日期随机函数
		/// </summary>
		/// <param name="ra">随机数</param>
		/// <returns></returns>
		public string DateRndName(Random ra)
		{
			DateTime d = DateTime.Now;
			string s = null, y, m, dd, h, mm, ss;
			y = d.Year.ToString();
			m = d.Month.ToString();
			if (m.Length < 2) m = "0" + m;
			dd = d.Day.ToString();
			if (dd.Length < 2) dd = "0" + dd;
			h = d.Hour.ToString();
			if (h.Length < 2) h = "0" + h;
			mm = d.Minute.ToString();
			if (mm.Length < 2) mm = "0" + mm;
			ss = d.Second.ToString();
			if (ss.Length < 2) ss = "0" + ss;
			s += y + m + dd + h + mm + ss;
			s += ra.Next(100, 999).ToString();
			return s;
		}
		#endregion

		#region 取得文件后缀
		/**********************************
         * 函数名称:GetFileExtends
         * 功能说明:取得文件后缀
         * 参    数:filename:文件名称
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string url = @"http://www.baidu.com/img/logo.gif";
         *          string s = o.GetFileExtends(url);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/
		/// <summary>
		/// 取得文件后缀
		/// </summary>
		/// <param name="filename">文件名称</param>
		/// <returns></returns>
		public string GetFileExtends(string filename)
		{
			string ext = null;
			if (filename.IndexOf('.') > 0)
			{
				string[] fs = filename.Split('.');
				ext = fs[fs.Length - 1];
			}
			return ext;
		}
		#endregion

		#region 获取远程文件源代码
		/**********************************
         * 函数名称:GetRemoteHtmlCode
         * 功能说明:获取远程文件源代码
         * 参    数:Url:远程url
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string url = @"http://www.baidu.com";
         *          string s = o.GetRemoteHtmlCode(url);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/


		/// <summary>
		/// 获取远程文件源代码
		/// </summary>
		/// <param name="Url"></param>
		/// <param name="Charset"></param>
		/// <returns></returns>
		public static string GetRemoteCode(string Url, string Charset)
		{
			if (Charset.Trim() == "") Charset = "UTF-8";
			HttpWebRequest webRequest2;
			HttpWebResponse response2;
			StreamReader sr2;
			
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
			//						   | SecurityProtocolType.Tls
			//						   | (SecurityProtocolType)0x300 //Tls11
			//						   | (SecurityProtocolType)0xC00; //Tls12
			webRequest2 = (HttpWebRequest)WebRequest.Create(Url);
			webRequest2.Referer = "https://mp.weixin.qq.com";
			//webRequest2.ContentType = "text/html; charset=" + Charset;
			webRequest2.Method = "GET";
			webRequest2.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.104 Safari/537.36";
			webRequest2.ContentType = "application/x-www-form-urlencoded";
			response2 = (HttpWebResponse)webRequest2.GetResponse();
			sr2 = new StreamReader(response2.GetResponseStream(), System.Text.Encoding.UTF8);
			return sr2.ReadToEnd();
		}

		public static (HttpStatusCode code,string ContentType, object html) GetRemoteCodeMobile(string Url)
		{
			string Charset = "UTF-8";
			HttpWebRequest webRequest2;
			HttpWebResponse response2;
			StreamReader sr2;

			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
			//						   | SecurityProtocolType.Tls
			//						   | (SecurityProtocolType)0x300 //Tls11
			//						   | (SecurityProtocolType)0xC00; //Tls12
			webRequest2 = (HttpWebRequest)WebRequest.Create(Url);
			webRequest2.ContentType = "text/html; charset=" + Charset;
			//webRequest2.AllowAutoRedirect = false;
			webRequest2.Method = "GET";
			webRequest2.UserAgent = "Mozilla/5.0 (Linux; Android 10; CDY-AN20 Build/HUAWEICDY-AN20; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/78.0.3904.62 XWEB/2759 MMWEBSDK/201201 Mobile Safari/537.36 MMWEBID/5345 MicroMessenger/8.0.1.1840(0x2800013B) Process/appbrand2 WeChat/arm64 Weixin NetType/WIFI Language/zh_CN ABI/arm64 MiniProgramEnv/android";
			webRequest2.ContentType = "application/x-www-form-urlencoded";
			response2 = (HttpWebResponse)webRequest2.GetResponse();
			if (response2.StatusCode == HttpStatusCode.OK)
			{
				sr2 = new StreamReader(response2.GetResponseStream(), System.Text.Encoding.Default);
				if (response2.ContentType.Contains("image")) {
					return (response2.StatusCode, response2.ContentType, response2.GetResponseStream());
				} else
				{
					return (response2.StatusCode, response2.ContentType, sr2.ReadToEnd());
				}
			} else
			{
				return (response2.StatusCode, response2.ContentType, "");
			}
		}
		#endregion

		#region 请求远程URL
		public static string HttpWebRequestUrl(string url, string data, string Charset = "utf-8", string Method = "post",Dictionary<string,string> Headers = null, string ContentType = "application/x-www-form-urlencoded", int timeout = 30000)//发送方法 
		{
			HttpWebRequest webRequest2;
			HttpWebResponse response2;
			StreamReader sr2;

			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
			//						   | SecurityProtocolType.Tls
			//						   | (SecurityProtocolType)0x300 //Tls11
			//						   | (SecurityProtocolType)0xC00; //Tls12

			webRequest2 = (HttpWebRequest)WebRequest.Create(url);
			webRequest2.Headers.Add("charset:" + Charset);
			if (Headers != null) {
				foreach (var item in Headers)
				{
					webRequest2.Headers.Add(item.Key + ":" + item.Value);
				}
			}
			webRequest2.ContentType = ContentType;
			webRequest2.Method = Method;
			webRequest2.Timeout = timeout;//豪秒
										  //webRequest2.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
			if (!string.IsNullOrEmpty(data))
			{
				var encoding = Encoding.GetEncoding(Charset);
				byte[] postData = encoding.GetBytes(data);
				webRequest2.ContentLength = postData.Length;
				Stream myRequestStream = webRequest2.GetRequestStream();
				myRequestStream.Write(postData, 0, postData.Length);
				myRequestStream.Close();
			}
			string str = "";
			try
			{
				response2 = (HttpWebResponse)webRequest2.GetResponse();
				sr2 = new StreamReader(response2.GetResponseStream(), System.Text.Encoding.UTF8);
				str = sr2.ReadToEnd();
			}
			catch (Exception ex)
			{
			}
			return str;
		}

		public static Stream HttpWebRequestUrlStream(string url, string data, string Charset = "utf-8", string Method = "post", string ContentType = "application/x-www-form-urlencoded", int timeout = 30000)//发送方法 
		{
			HttpWebRequest webRequest2;
			HttpWebResponse response2;
			//StreamReader sr2;

			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
			//						   | SecurityProtocolType.Tls
			//						   | (SecurityProtocolType)0x300 //Tls11
			//						   | (SecurityProtocolType)0xC00; //Tls12

			webRequest2 = (HttpWebRequest)WebRequest.Create(url);
			webRequest2.Headers.Add("charset:" + Charset);
			webRequest2.ContentType = ContentType;
			webRequest2.Method = Method;
			webRequest2.Timeout = timeout;//豪秒
										  //webRequest2.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
			if (!string.IsNullOrEmpty(data))
			{
				var encoding = Encoding.GetEncoding(Charset);
				byte[] postData = encoding.GetBytes(data);
				webRequest2.ContentLength = postData.Length;
				Stream myRequestStream = webRequest2.GetRequestStream();
				myRequestStream.Write(postData, 0, postData.Length);
				myRequestStream.Close();
			}
			try
			{
				response2 = (HttpWebResponse)webRequest2.GetResponse();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return response2.GetResponseStream();
		}

		public static string WebRequestUrl(string url, string data, string Charset = "utf-8", string Method = "post", string ContentType = "application/x-www-form-urlencoded", int timeout = 30000)//发送方法 
		{
			var json = "";
			var request = WebRequest.Create(url);
			request.Method = Method;
			request.ContentType = ContentType;
			request.Headers.Add("charset:" + Charset);
			request.Timeout = timeout;
			var encoding = Encoding.GetEncoding(Charset);
			if (data != null)
			{
				byte[] buffer = encoding.GetBytes(data);
				request.ContentLength = buffer.Length;
				request.GetRequestStream().Write(buffer, 0, buffer.Length);
			}
			else
			{
				request.ContentLength = 0;
			}
			using (HttpWebResponse wr = request.GetResponse() as HttpWebResponse)
			{
				using (StreamReader reader = new StreamReader(wr.GetResponseStream(), encoding))
				{
					json = reader.ReadToEnd();
				}
			}

			return json;
		}

		/// <summary>
		/// 微信上传素材库
		/// </summary>
		/// <param name="url"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string HttpUploadFile(string url, string path)//这个方法是两个URL第一个url是条到微信的，第二个是本地图片路径
		{
			// 设置参数
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			CookieContainer cookieContainer = new CookieContainer();
			request.CookieContainer = cookieContainer;
			request.AllowAutoRedirect = true;
			request.Method = "POST";
			string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
			request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
			byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
			byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
			int pos = path.LastIndexOf("\\");
			string fileName = path.Substring(pos + 1);
			//请求头部信息
			StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
			byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());
			FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
			byte[] bArr = new byte[fs.Length];
			fs.Read(bArr, 0, bArr.Length);
			fs.Close();
			Stream postStream = request.GetRequestStream();
			postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
			postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
			postStream.Write(bArr, 0, bArr.Length);
			postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
			postStream.Close();
			//发送请求并获取相应回应数据
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			//直到request.GetResponse()程序才开始向目标网页发送Post请求
			Stream instream = response.GetResponseStream();
			StreamReader sr = new StreamReader(instream, Encoding.UTF8);
			//返回结果网页（html）代码
			string content = sr.ReadToEnd();
			return content;
		}
		#endregion

		#region "下载文件到本地"
		/// <summary>
		/// 下载文件到本地
		/// </summary>
		/// <param name="filePath">本地文件路径</param>
		/// <param name="url">远程URL</param>
		/// <param name="data">请求数据</param>
		/// <param name="Charset"></param>
		/// <param name="Method"></param>
		/// <param name="ContentType"></param>
		/// <param name="timeout"></param>
		public static void DownFile(string filePath, string url)//发送方法
		{
			//var stream = HttpWebRequestUrlStream(url, data, Charset, Method, ContentType, timeout);

			////创建本地文件写入流
			//Stream fs = new FileStream(filePath, FileMode.Create);
			//byte[] bArr = new byte[1024];
			//int size = stream.Read(bArr, 0, (int)bArr.Length);
			//while (size > 0)
			//{
			//	fs.Write(bArr, 0, size);
			//	size = stream.Read(bArr, 0, (int)bArr.Length);
			//}
			//fs.Close();
			//stream.Close();
			using (var webClient = new WebClient())
			{
				webClient.DownloadFile(new Uri(url), filePath);
			}
		}
		#endregion

		#region 替换网页中的换行和引号
		/**********************************
         * 函数名称:ReplaceEnter
         * 功能说明:替换网页中的换行和引号
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.baidu.com";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.ReplaceEnter(HtmlCode);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/
		/// <summary>
		/// 替换网页中的换行和引号
		/// </summary>
		/// <param name="HtmlCode">HTML源代码</param>
		/// <returns></returns>
		public string ReplaceEnter(string HtmlCode)
		{
			string s = "";
			if (HtmlCode == null || HtmlCode == "")
				s = "";
			else
				s = HtmlCode.Replace("\"", "");
			s = s.Replace("\r\n", "");
			return s;
		}

		#endregion

		#region 执行正则提取出值
		/**********************************
         * 函数名称:GetRegValue
         * 功能说明:执行正则提取出值
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.baidu.com";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.ReplaceEnter(HtmlCode);
         *          string Reg="<title>.+?</title>";
         *          string GetValue=o.GetRegValue(Reg,HtmlCode)
         *          Response.Write(GetValue);
         *          o.Dispose();
         * ********************************/
		/// <summary>
		/// 执行正则提取出值
		/// </summary>
		/// <param name="RegexString">正则表达式</param>
		/// <param name="RemoteStr">HtmlCode源代码</param>
		/// <returns></returns>
		public static string GetRegValue(string RegexString, string RemoteStr)
		{
			string MatchVale = "";
			Regex r = new Regex(RegexString);
			Match m = r.Match(RemoteStr);
			if (m.Success)
			{
				MatchVale = m.Value;
			}
			return MatchVale;
		}
		#endregion

		#region 替换HTML源代码
		/**********************************
         * 函数名称:RemoveHTML
         * 功能说明:替换HTML源代码
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.baidu.com";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.ReplaceEnter(HtmlCode);
         *          string Reg="<title>.+?</title>";
         *          string GetValue=o.GetRegValue(Reg,HtmlCode)
         *          Response.Write(GetValue);
         *          o.Dispose();
         * ********************************/
		/// <summary>
		/// 替换HTML源代码
		/// </summary>
		/// <param name="HtmlCode">html源代码</param>
		/// <returns></returns>
		public string RemoveHTML(string HtmlCode)
		{
			string MatchVale = HtmlCode;
			foreach (Match s in Regex.Matches(HtmlCode, "<.+?>"))
			{
				MatchVale = MatchVale.Replace(s.Value, "");
			}
			return MatchVale;
		}

		#endregion

		#region 匹配页面的链接
		/**********************************
         * 函数名称:GetHref
         * 功能说明:匹配页面的链接
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.baidu.com";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.GetHref(HtmlCode);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/
		/// <summary>
		/// 获取页面的链接正则
		/// </summary>
		/// <param name="HtmlCode"></param>
		/// <returns></returns>
		public string GetHref(string HtmlCode)
		{
			string MatchVale = "";
			string Reg = @"(h|H)(r|R)(e|E)(f|F) *= *('|"")?((\w|\\|\/|\.|:|-|_)+)('|""| *|>)?";
			foreach (Match m in Regex.Matches(HtmlCode, Reg))
			{
				MatchVale += (m.Value).ToLower().Replace("href=", "").Trim() + "||";
			}
			return MatchVale;
		}
		#endregion

		#region 匹配页面的图片地址
		/**********************************
         * 函数名称:GetImgSrc
         * 功能说明:匹配页面的图片地址
         * 参    数:HtmlCode:html源代码;imgHttp:要补充的http.当比如:<img src="bb/x.gif">则要补充http://www.baidu.com/,当包含http信息时,则可以为空
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.baidu.com";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.GetImgSrc(HtmlCode,"http://www.baidu.com/");
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/
		/// <summary>
		/// 匹配页面的图片地址
		/// </summary>
		/// <param name="HtmlCode"></param>
		/// <param name="imgHttp">要补充的http://路径信息</param>
		/// <returns></returns>
		public string GetImgSrc(string HtmlCode, string imgHttp)
		{
			string MatchVale = "";
			string Reg = @"<img.+?>";
			foreach (Match m in Regex.Matches(HtmlCode, Reg))
			{
				MatchVale += GetImg((m.Value).ToLower().Trim(), imgHttp) + "||";
			}
			return MatchVale;
		}
		/// <summary>
		/// 匹配<img src="" />中的图片路径实际链接
		/// </summary>
		/// <param name="ImgString"><img src="" />字符串</param>
		/// <returns></returns>
		public string GetImg(string ImgString, string imgHttp)
		{
			string MatchVale = "";
			string Reg = @"src=.+\.(bmp|jpg|gif|png|)";
			foreach (Match m in Regex.Matches(ImgString.ToLower(), Reg))
			{
				MatchVale += (m.Value).ToLower().Trim().Replace("src=", "");
			}
			return (imgHttp + MatchVale);
		}

		#endregion


		/// <summary>
		/// 获取服务器上指定路径的文件内容，注意取的是源码
		/// </summary>
		/// <param name="filePath">物理路径</param>
		/// <returns></returns>
		public static string Get_Html(HttpContext httpcontext, string filePath)
		{
			string fileContent = string.Empty;
			using (var reader = new StreamReader(filePath))
			{
				fileContent = reader.ReadToEnd();
			}
			return fileContent;
		}

	}
}

