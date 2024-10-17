using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models;

namespace WeTalk.Web.Services
{
	public interface IPublibService
	{
		PubLog AddLog(string content);
		Task<PubLog> AddLogAsync(string content);
		string ArrayDistinct(string str, string splitstr);
		string ClearHTML(string Htmlstring);
		string GetImgUrl(string HTMLStr);
		string GetSMS(string tel);
		string HtmlDiscode(string theString);
		string HtmlEncode(string theString);
		string Keys(string str, string Url, string CssClass);
		string MakeFileName();
		string ParseTags(string HTMLStr);
		string ReplaceRegex(string content, string regchr, string chr);
		string[] StringSplit(string str, string splitstr);
		string StripHTML(string strHtml);

		string UpType(string uptype = "", int upsize = 0);
		object[] ToHashEntries(object obj);
		T ConvertFromRedis<T>(Dictionary<string, string> dicEntries);
		
		/// <summary>
		/// 缓存JSON字串转DB实体对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		T GetRedisObj<T>(string key);

		/// <summary>
		/// 实体对象转缓存
		/// </summary>
		/// <param name="key"></param>
		/// <param name="obj"></param>
		/// <param name="times"></param>
		/// <returns></returns>
		bool SetRedisObj(string key, object obj, int times = 7200);

		/// <summary>
		/// 删除指定KEY
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool DelRedisObj(string key);

		/// <summary>
		/// 清除指定名片权限对应缓存
		/// </summary>
		/// <param name="list_cardid"></param>
		void DelCardRoleRedis(List<long> list_cardid);

		/// <summary>
		/// 清除指定企业权限对应缓存
		/// </summary>
		/// <param name="list_companyid"></param>
		void ThrowLog(string message, Exception ex);
		string QiniuDomain(string str);
		void AddCookie(string key, string value, int time);
        void DeleteCookie(string key);
        string GetValue(string key);
    }
}
