
using System.Collections.Generic;

namespace WeTalk.Models.Dto.Course
{
	/// <summary>
	/// 课程SKU价格信息
	/// </summary>
	public class CourseSkuPriceDto
	{
		/// <summary>
		/// 课程SkuID
		/// </summary>
		public long CourseSkuid { get; set; }
		/// <summary>
		/// 原价/市场价
		/// </summary>
		public decimal MarketPrice { get; set; }
		/// <summary>
		/// 展示价
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// 币种
		/// </summary>
		public string CurrencyCode { get; set; }
		/// <summary>
		/// 币种符号
		/// </summary>
		public string CurrencyIco { get; set; }
	}
}
