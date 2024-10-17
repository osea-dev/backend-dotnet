using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.Course
{
	public partial class ListSubAdd : Models.WebCourseSku
	{
		public List<SelectListItem> SkuTypeItem { get; set; }
		public List<SelectListItem> MenkeCourseItem { get; set; }

		public List<PubCurrencyPrice> ListPubCurrency { get; set; }
	}

	public class PubCurrencyPrice
	{
		public long PubCurrencyid { get; set; } = 0L;
		public string Country { get; set; }
		public string CountryCode { get; set; }
		public string CurrencyCode { get; set; }
		public decimal Price { get; set; } = 0.00M;
		public decimal MarketPrice { get; set; } = 0.00M;

	}
}
