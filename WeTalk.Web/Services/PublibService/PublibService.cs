using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeTalk.Common.Helper;
using WeTalk.Extensions;
using WeTalk.Models;

namespace WeTalk.Web.Services
{
	public partial class PublibService : IPublibService
	{

		private readonly SqlSugarScope _context;
		private readonly IConfiguration _config;
		private HttpContext _httpcontent;
		private static bool _isdevelopment = false;
		private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(PublibService));
		public string LoginPrefix = "";//登录状态的前缀
		private readonly IWebHostEnvironment _env;

		public PublibService(IHttpContextAccessor httpContext, IWebHostEnvironment env, IConfiguration config,SqlSugarScope dbcontext)
		{
			_context = dbcontext;
			_config = config;
			_httpcontent = httpContext.HttpContext;
			_isdevelopment = env.IsDevelopment();
			_env = env;
			LoginPrefix = _config["Web:LoginPrefix"];
		}

		//===========================================================================
		#region "添加业务日志"
		public PubLog AddLog(string content)
		{
			PubLog model_PubLog = new PubLog();
			model_PubLog.Content = content;
			model_PubLog.Dtime = DateTime.Now;
			model_PubLog.Ip = IpHelper.GetCurrentIp(_httpcontent);
			model_PubLog.Url = GSRequestHelper.GetUrl(_httpcontent.Request);
			if (!string.IsNullOrEmpty(_httpcontent.Session.GetString(LoginPrefix + "AdminID"))) model_PubLog.AdminMasterid = long.Parse(_httpcontent.Session.GetString(LoginPrefix + "AdminID"));
			if (!string.IsNullOrEmpty(_httpcontent.Session.GetString(LoginPrefix + "AdminName"))) model_PubLog.Username = _httpcontent.Session.GetString(LoginPrefix + "AdminName");

			model_PubLog = _context.Insertable(model_PubLog).ExecuteReturnEntity();
			return model_PubLog;
		}

		public async Task<PubLog> AddLogAsync(string content)
		{
			PubLog model_PubLog = new PubLog();
			model_PubLog.Content = content;
			model_PubLog.Dtime = DateTime.Now;
			model_PubLog.Ip = IpHelper.GetCurrentIp(_httpcontent);
			model_PubLog.Url = GSRequestHelper.GetUrl(_httpcontent.Request);
			if (!string.IsNullOrEmpty(_httpcontent.Session.GetString(LoginPrefix + "AdminID"))) model_PubLog.AdminMasterid = long.Parse(_httpcontent.Session.GetString(LoginPrefix + "AdminID"));
			if (!string.IsNullOrEmpty(_httpcontent.Session.GetString(LoginPrefix + "AdminName"))) model_PubLog.Username = _httpcontent.Session.GetString(LoginPrefix + "AdminName");

			model_PubLog = _context.Insertable(model_PubLog).ExecuteReturnEntity();
			return model_PubLog;
		}
		#endregion

		#region "日志处理"
		public void ThrowLog(string message, Exception ex)
		{
			string str = "\r\n";
			str += "Source:" + ex.Source + "\r\n";
			str += "Message:" + message + "," + ex.Message + "\r\n";
			str += "StackTrace:" + ex.StackTrace + "\r\n";
			log.Info(str);
		}
		#endregion

		#region "清理HTML代码"
		public string HtmlDiscode(string theString)
		{
			if (theString != "")
			{
				theString = theString.Replace("&gt;", ">");
				theString = theString.Replace("&lt;", "<");
				theString = theString.Replace("&nbsp;", " ");
				theString = theString.Replace(" &nbsp;", "  ");
				theString = theString.Replace("&quot;", "\"");
				theString = theString.Replace("&#39;", "'");
				theString = theString.Replace("<br/> ", "\r\n");
				return theString;
			}
			else
			{
				return string.Empty;
			}
		}
		//HtmlDiscode

		public string HtmlEncode(string theString)
		{
			if (theString != "")
			{
				theString = theString.Replace(">", "&gt;");
				theString = theString.Replace("<", "&lt;");
				theString = theString.Replace("  ", " &nbsp;");
				theString = theString.Replace("  ", " &nbsp;");
				theString = theString.Replace("\"", "&quot;");
				theString = theString.Replace("'", "&#39;");
				theString = theString.Replace("\r\n", "<br/> ");
				return theString;
			}
			else
			{
				return string.Empty;
			}
		}
		//HtmlEncode
		#endregion

		#region "字符串分函数"
		/// <summary>
		/// 字符串分函数2
		/// </summary>
		/// <param name="str">要分解的字符串</param>
		/// <param name="splitstr">分割符,可以为string类型</param>
		/// <returns>字符数组</returns>
		public string[] StringSplit(string str, string splitstr)
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
		#endregion

		#region "分隔数组并去重"
		/// <summary>
		/// 分隔成数组，并去重
		/// </summary>
		/// <param name="str"></param>
		/// <param name="splitstr"></param>
		/// <returns></returns>
		public string ArrayDistinct(string str, string splitstr)
		{
			string[] arr = StringSplit(str, splitstr);
			string[] arr2 = arr.Distinct().ToArray();
			return string.Join(splitstr, arr2);
		}
		#endregion

		#region "取上传文件名，不可重复数"

		public string MakeFileName()
		{
			string fname;
			Random Rnd1 = new Random();
			fname = DateTime.Now.Year.ToString();
			fname = fname + DateTime.Now.Month;
			fname = fname + DateTime.Now.Day;
			fname = fname + DateTime.Now.Hour;
			fname = fname + DateTime.Now.Minute;
			fname = fname + DateTime.Now.Second;
			fname = fname + DateTime.Now.Millisecond;
			fname = fname + Rnd1.Next(1, 7).ToString();
			return fname;
		}
		#endregion

		#region "将汉字转为ANSI码，如:上海=%C9%CF%BA%A3"
		//public string ANSI(string str)
		//{
		//    string sb = "";
		//    ArrayList arr = new ArrayList(str.Length);
		//    int i;
		//    for (i = 0; i <= str.Length - 1; i++)
		//    {
		//        //arr.Add(Mid(str, i + 1, 1));
		//        arr.Add(str.Substring(i + 1, 1));
		//        if (Int16.Parse(Asc(arr[i])) < 0)
		//        {
		//            sb = sb + "%" + Left(Hex(Int16.Parse(Asc(arr[i]))), 2);
		//            sb = sb + "%" + Right(Hex(Int16.Parse(Asc(arr[i]))), 2);
		//        }
		//        else
		//        {
		//            sb = sb + arr[i];
		//        }
		//    }
		//    return sb;
		//}
		#endregion

		#region "分解标签"
		//str:标签集
		//Url:标签链接地址
		//CssClass:样式
		public string Keys(string str, string Url, string CssClass)
		{
			Array arr;
			int i = 0;
			string temp = "";
			if (str.Contains(","))
			{
				arr = str.Split(",");
				for (i = 0; i <= arr.Length - 1; i++)
				{
					temp = temp + "<a href='" + Url + "&Keys=" + WebUtility.UrlEncode(arr.GetValue(i).ToString()) + "' class='" + CssClass + "'>" + arr.GetValue(i).ToString() + "</a> ";
				}
			}
			else
			{
				temp = "<a href=\"" + Url + "&Keys=" + WebUtility.UrlEncode(str) + "\" Class=\"" + CssClass + "\">" + str + "</a>";
			}
			return temp;
		}
		#endregion

		#region "正则替换"
		public string ReplaceRegex(string content, string regchr, string chr)
		{
			if (content.ToString().Trim() == "") return "";
			if (regchr.ToString().Trim() == "") return content;

			Regex reg = new Regex(regchr.ToString().Trim(), RegexOptions.IgnoreCase);
			if (reg.Match(content).ToString().Length > 0) content = content.Replace("" + reg.Match(content).ToString(), chr.ToString().Trim() + "");
			return content;
		}
		#endregion

		#region "处理html代码"

		////   <summary>   
		////   去除HTML标记   
		////   </summary>   
		////   <param   name="NoHTML">包括HTML的源码   </param>   
		////   <returns>已经去除后的文字</returns>   
		public string ClearHTML(string Htmlstring)
		{
			//删除脚本 
			Htmlstring = Regex.Replace(Htmlstring, "<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
			//删除HTML 
			Htmlstring = Regex.Replace(Htmlstring, "<(.[^>]*)>", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "-->", "", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "<!--.*", "", RegexOptions.IgnoreCase);

			Htmlstring = Regex.Replace(Htmlstring, "&(quot|#34);", "\"", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(amp|#38);", "&", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(lt|#60);", "<", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(gt|#62);", ">", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(iexcl|#161);", "¡", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(cent|#162);", "¢", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(pound|#163);", "£", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(copy|#169);", "©", RegexOptions.IgnoreCase);
			Htmlstring = Regex.Replace(Htmlstring, "&(\\d+);", "", RegexOptions.IgnoreCase);

			Htmlstring.Replace("<", "");
			Htmlstring.Replace(">", "");
			Htmlstring.Replace("/n/r", "");
			Htmlstring = WebUtility.HtmlEncode(Htmlstring).Trim();

			return Htmlstring;
		}

		//提取HTML代码中文字 
		// <summary> 
		// 去除HTML标记 
		// </summary> 
		// <param name="strHtml">包括HTML的源码 </param> 
		// <returns>已经去除后的文字</returns> 


		public string StripHTML(string strHtml)
		{
			string[] aryReg = { "<script[^>]*?>.*?</script>", "<(\\/\\s*)?!?((\\w+:)?\\w+)(\\w+(\\s*=?\\s*(([\"'])(\\\\[\"'tbnr]|[^\\7])*?\\7|\\w+)|.{0})|\\s)*?(\\/\\s*)?>", "([\\r\\n])[\\s]+", "&(quot|#34);", "&(amp|#38);", "&(lt|#60);", "&(gt|#62);", "&(nbsp|#160);", "&(iexcl|#161);", "&(cent|#162);",
			"&(pound|#163);", "&(copy|#169);", "&(\\d+);", "-->", "<!--.*\\n" };

			//chr(161), 
			//chr(162), 
			//chr(163), 
			//chr(169), 
			string[] aryRep = { "", "", "", "\"", "&", "<", ">", " ", "¡", "¢",
			"£", "©", "", "/n/r", "" };

			string newReg = aryReg[0];
			string strOutput = strHtml;
			for (int i = 0; i <= aryReg.Length - 1; i++)
			{
				Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
				strOutput = regex.Replace(strOutput, aryRep[i]);
			}
			strOutput.Replace("<", "");
			strOutput.Replace(">", "");
			strOutput.Replace("/n/r", "");
			return strOutput;
		}



		/// <summary> 
		/// 移除HTML标签 
		/// </summary> 
		/// <param name="HTMLStr">HTMLStr</param> 
		public string ParseTags(string HTMLStr)
		{
			return System.Text.RegularExpressions.Regex.Replace(HTMLStr, "<[^>]*>", "");
		}


		/// <summary> 
		/// 取出文本中的图片地址 
		/// </summary> 
		/// <param name="HTMLStr">HTMLStr</param> 
		public string GetImgUrl(string HTMLStr)
		{
			string str = string.Empty;
			//string sPattern = "^<img\\s+[^>]*>";
			Regex r = new Regex("<img\\s+[^>]*\\s*src\\s*=\\s*([']?)(?<url>\\S+)'?[^>]*>", RegexOptions.Compiled);
			Match m = r.Match(HTMLStr.ToLower());
			if (m.Success)
			{
				str = m.Result("${url}");
			}
			return str;
		}


		#endregion


		#region "获取短信验证码"
		/// <summary>
		/// 获取短信验证码
		/// </summary>
		/// <returns>返回Code</returns>
		public string GetSMS(string tel)
		{
			if (_isdevelopment)
			{
				return "123456";
			}
			else
			{
				return "654321";
			}
		}


		#endregion


		#region "判断上传格式和大小是否通过"
		/// <summary>
		/// 判断上传格式和大小是否通过
		/// </summary>
		/// <returns></returns>
		public string UpType(string uptype = "", int upsize = 0)
		{
			string msg = "";
			for (int i = 0; i < _httpcontent.Request.Form.Files.Count; i++)
			{
				if (_httpcontent.Request.Form.Files[i].Length > 0)
				{
					if (!uptype.Contains("|" + Path.GetExtension(_httpcontent.Request.Form.Files[i].FileName).Replace(".", "") + "|") && uptype != "")
					{
						return "您上传的文件[" + _httpcontent.Request.Form.Files[i].FileName + "]格式不符合要求，请重新上传！";
					}
					if ((decimal.Parse(_httpcontent.Request.Form.Files[i].Length.ToString()) / 1024 / 1024) > upsize && upsize > 0)
					{
						return "您上传的文件[" + _httpcontent.Request.Form.Files[i].FileName + "]大小为(" + (decimal.Parse(_httpcontent.Request.Form.Files[i].Length.ToString()) / 1024 / 1024).ToString("0.000") + "M)超出要求，请重新处理后上传！";
					}
				}
			}
			return msg;
		}

		#endregion


		#region "哈希与C#对象互转"
		/// <summary>
		/// 对象转哈希
		/// </summary>
		/// <returns></returns>
		public object[] ToHashEntries(object obj)
		{
			PropertyInfo[] properties = obj.GetType().GetProperties();
			List<string> attrs = new List<string>();
			foreach (var attr in properties)
			{
				attrs.Add(attr.Name);
				attrs.Add(attr.GetValue(obj) + "");
			}
			return attrs.ToArray();
		}

		/// <summary>
		/// 哈希转对象
		/// </summary>
		/// <returns></returns>
		public T ConvertFromRedis<T>(Dictionary<string, string> dicEntries)
		{
			PropertyInfo[] properties = typeof(T).GetProperties();
			var obj = Activator.CreateInstance(typeof(T));
			foreach (var property in properties)
			{
				var entry = dicEntries.FirstOrDefault(g => g.Key.ToString().Equals(property.Name));
				if (entry.Equals(new Dictionary<string, string>())) continue;
				property.SetValue(obj, Convert.ChangeType(entry.Value, property.PropertyType));
			}
			return (T)obj;
		}
		#endregion

		//设置权限相关缓存
		//===========================================================================
		#region "缓存与DB实体对象互转 "
		/// <summary>
		/// 缓存转实体对象
		/// </summary>
		/// <returns></returns>
		public T GetRedisObj<T>(string key)
		{
			object obj = null;
			if (RedisServer.Cache.Exists(key))
			{
				//try
				//{
				return JsonConvert.DeserializeObject<T>(RedisServer.Cache.Get(key));
				//}
				//catch {
				//	AddLog("缓存JSON字串转DB实体对象出错:key=" + key);
				//}
			}
			return (T)obj;
		}

		/// <summary>
		/// 实体对象转缓存
		/// </summary>
		/// <param name="key"></param>
		/// <param name="obj">对象</param>
		/// <param name="times">有效时长，默认1天</param>
		/// <returns></returns>
		public bool SetRedisObj(string key, object obj, int times = 86400)
		{
			bool isok = RedisServer.Cache.Set(key, JsonConvert.SerializeObject(obj));
			if (times > 0) RedisServer.Cache.Expire(key, times);
			return isok;
		}

		/// <summary>
		/// 删除指定KEY
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool DelRedisObj(string key)
		{
			var isok = RedisServer.Cache.Del(key);
			return isok > 0;

			//bool isok = false;
			//if (key.Contains("*"))
			//{
			//	var pattern = key;
			//	var redisResult = _db.ScriptEvaluate(LuaScript.Prepare(
			//					//Redis的keys模糊查询：
			//					" local res = redis.call('KEYS', @keypattern) " +
			//					" return res "), new { @keypattern = pattern });

			//	if (!redisResult.IsNull)
			//	{
			//		_db.KeyDelete((RedisKey[])redisResult);  //删除一组key
			//	}
			//}
			//else
			//{
			//	isok = _db.KeyDelete(key);
			//}
			//return isok;
		}
		public long DelRedisObj(string[] keys)
		{
			var count = RedisServer.Cache.Del(keys);
			return count;
		}
		#endregion

		#region "角色权限相关"
		/// <summary>
		/// 删除指定角色
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public void DelCardRoleRedis(List<long> list_cardid)
		{
			list_cardid.ForEach(cardid =>
			{
				DelRedisObj("Functionid_Cardid_Data:" + cardid);
			});
		}

		public void DelCompanyRoleRedis(List<long> list_companyid)
		{
			list_companyid.ForEach(companyid =>
			{
				DelRedisObj("Functionid_Companyid:" + companyid);
				DelRedisObj("Functionid_Companyid_Data:" + companyid);
			});
		}
		#endregion


		#region "变量替换成域名"
		public string QiniuDomain(string str)
		{
			if (string.IsNullOrEmpty(str)) return str;
			Dictionary<string, string> dic_domain = new Dictionary<string, string>();//取全局域名替换变量
			string key = "dic_domain";
			if (!RedisServer.Cache.Exists(key))
			{
				string keys = "qiniu_prefix,qiniu_secure,qiniu_domain";
				var list_key = keys.Split(',').ToList();
				var list_pubConfig = _context.Queryable<PubConfig>().Where(u => list_key.Contains(u.Key.ToLower().Trim()) && u.Status == 1).ToList();
				if (list_pubConfig == null) return "";
				var list_prefix = list_pubConfig.Where(u => u.Key.ToLower() == "qiniu_prefix").ToList();
				foreach (var model_prefix in list_prefix)
				{
					string val = "";
					var model_domain = list_pubConfig.FirstOrDefault(u => u.Key.ToLower() == "qiniu_domain");
					val = (model_domain != null) ? model_domain.Val : "";
					if (val == "") continue;
					var model_secure = list_pubConfig.FirstOrDefault(u => u.Key.ToLower() == "qiniu_secure");
					if (model_secure != null && bool.Parse(model_secure.Val))
					{
						val = "https://" + val + "/";
					}
					else
					{
						val = "http://" + val + "/";
					}
					if (!dic_domain.Keys.Contains(model_prefix.Val.ToLower()))
						dic_domain.Add(model_prefix.Val.ToLower(), val);

					str = str.Replace(model_prefix.Val.ToLower(), val);
					if (!RedisServer.Cache.HExists(key, model_prefix.Val.ToLower())) RedisServer.Cache.HSet(key, model_prefix.Val.ToLower(), val);
				}
				RedisServer.Cache.Expire(key, 360);//1小时
			}
			else
			{
				dic_domain = RedisServer.Cache.HGetAll(key);//取全局域名替换变量
				foreach (var item in dic_domain)
				{
					str = str.Replace(item.Key, item.Value);
					if (!RedisServer.Cache.HExists(key, item.Key)) RedisServer.Cache.HSet(key, item.Key, item.Value);
				}
			}
			return str;
		}
		#endregion


		#region "Cookies操作"
		/// <summary>
		/// 添加cookie缓存设置过期时间
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="time"></param>
		public void AddCookie(string key, string value, int time)
		{
			_httpcontent.Response.Cookies.Append(key, value, new CookieOptions
			{
				Expires = DateTime.Now.AddMilliseconds(time)
			});
		}
		/// <summary>
		/// 删除cookie缓存
		/// </summary>
		/// <param name="key"></param>
		public void DeleteCookie(string key)
		{
			_httpcontent.Response.Cookies.Delete(key);
		}
		/// <summary>
		/// 根据键获取对应的cookie
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetValue(string key)
		{
			var value = "";
			_httpcontent.Request.Cookies.TryGetValue(key, out value);
			if (string.IsNullOrWhiteSpace(value))
			{
				value = string.Empty;
			}
			return value;
		}
		#endregion
	}
}
