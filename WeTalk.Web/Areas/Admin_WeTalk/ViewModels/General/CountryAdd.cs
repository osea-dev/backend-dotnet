using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.General
{
	public partial class CurrencyAdd : PubCurrency
	{
		public string Lang { get; set; }
		public string Currency { get; set; }
		public string Country { get; set; }
	}
}
