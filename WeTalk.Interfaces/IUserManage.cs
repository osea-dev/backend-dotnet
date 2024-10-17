using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeTalk.Models.Redis;

namespace WeTalk.Interfaces
{
	public interface IUserManage: IBaseService
	{
		/// <summary>
		/// 用户ID
		/// </summary>
		long Userid { get; set; }
		/// <summary>
		/// 用户Token
		/// </summary>
		string UserToken { get; set; }
		/// <summary>
		/// 展示币种
		/// </summary>
		string CurrencyCode { get; set; }
		/// <summary>
		/// 结算币种
		/// </summary>
		string SettlementCurrencyCode { get; set; }
		/// <summary>
		/// 语言
		/// </summary>
		string Lang { get; set; }
		/// <summary>
		/// UTC名称
		/// </summary>
		public string Utc { get; set; }
		/// <summary>
		/// UTC分差
		/// </summary>
		public int UtcSec { get; set; }

		/// <summary>
		/// 存放httpcontext.request.body
		/// </summary>
		string RequestBody { get; set; }

		string GetCountryCode(string ip = "");
		string GetCurrencyCode(string country_code = "");
		UserToken GetUserToken();
		UserToken SetUserToken(long userid, int expiresin = 7200);
	}
}