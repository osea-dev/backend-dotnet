using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.User
{
	public partial class ListAdd : WebUser
	{
		public List<SelectListItem> CountryItem { get; set; }
		public List<SelectListItem> UtcItem { get; set; }
		public string AllMobile { get; set; }
		public string Message { get; set; }
		public string CountryCode { get; set; }
		public string Timezone { get; set; }
		public int IsModifyMobile { get; set; }
	}
}
