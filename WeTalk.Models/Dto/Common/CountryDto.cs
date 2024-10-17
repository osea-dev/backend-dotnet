
namespace WeTalk.Models.Dto.Common
{
	/// <summary>
	/// 国家信息
	/// </summary>
	public class CountryDto
	{
		/// <summary>
		/// 国家ID
		/// </summary>
		public long Countryid { get; set; }

		/// <summary>
		/// iso_3166_1国家代码（字母2位）
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 国家代码（数字）
		/// </summary>
		public int Number { get; set; }
		/// <summary>
		/// 国家名
		/// </summary>
		public string Country { get; set; }
		/// <summary>
		/// 国家名(英)
		/// </summary>
		public string CountryEn { get; set; }
	}
}
