using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels
{
	public partial class Login
	{
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Lang { get; set; }

		public List<SelectListItem> LangItem { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string UserName { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Password { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string IsSMS { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string ReturnUrl { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string SafeCode { get; set; }
		public bool IsCookie { get; set; }
	}
}
