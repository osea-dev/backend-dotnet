using System;

namespace WeTalk.Models.Redis
{
	/// <summary>
	/// 会员客户端览权逻辑
	/// 1，判断是否过期，过期注销
	/// 2，判断Updatetime是否超过24小时，超过则加更新当前CountryCode，IP定位API刷
	/// 3，刷新Token时间：Updatetime
	/// </summary>
	public partial class UserToken
	{
		#region "用户信息"
		/// <summary>
		/// 用户ID
		/// </summary>
		public long Userid { get; set; }
		/// <summary>
		/// 用户姓名
		/// </summary>
		public string FirstName { get; set; }
		public string LastName { get; set; }
		/// <summary>
		/// 手机号前缀
		/// </summary>
		public string MobileCode { get; set; }
		/// <summary>
		/// 手机号
		/// </summary>
		public string Mobile { get; set; }
		/// <summary>
		/// 邮箱
		/// </summary>
		public string Email { get; set; }
		/// <summary>
		/// 出生日期
		/// </summary>
		public DateTime Birthdate { get; set; }
		/// <summary>
		/// 头像
		/// </summary>
		public string HeadImg { get; set; }
		#endregion

		#region "Token信息"
		/// <summary>
		/// Token刷新时间
		/// </summary>
		public DateTime Updatetime { get; set; }
		/// <summary>
		/// Token有效时长
		/// </summary>
		public int Expiresin { get; set; }
		/// <summary>
		/// 用户Token
		/// </summary>
		public string Token { get; set; }
		#endregion

		#region 语言+展示货币+结算货币+时区
		/// <summary>
		/// 最后请求语言
		/// </summary>
		public string Lang { get; set; }
		/// <summary>
		/// 结算币种
		/// </summary>
		public string SettlementCurrencyCode { get; set; }
		#endregion
	}
}