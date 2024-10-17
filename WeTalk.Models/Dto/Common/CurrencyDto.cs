
namespace WeTalk.Models.Dto.Common
{
	/// <summary>
	/// 币种信息
	/// </summary>
	public class CurrencyDto
	{
		/// <summary>
		/// 币种ID
		/// </summary>
		public long Currencyid { get; set; }

		/// <summary>
		/// iso_3166_1国家代码（字母2位）
		/// </summary>
		public string CountryCode { get; set; }
		/// <summary>
		/// 币种代码
		/// </summary>
		public string CurrencyCode { get; set; }
		/// <summary>
		/// 币种
		/// </summary>
		public string Currency { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 符号
        /// </summary>
        public string Ico { get; set; }
        /// <summary>
        /// 当前默认币种
        /// </summary>
        public int IsDefault { get; set; }
	}
}
