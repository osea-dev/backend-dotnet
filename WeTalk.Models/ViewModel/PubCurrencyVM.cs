using System;
using System.Collections.Generic;

namespace WeTalk.Models.ViewModel
{
	public class PubCurrencyVM : PubCurrency
	{
		public string Country { get; set; }
		public string Currency { get; set; }
		public string Lang { get; set; }
	}
}