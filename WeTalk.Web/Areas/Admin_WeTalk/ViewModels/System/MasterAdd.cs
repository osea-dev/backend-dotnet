using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.System
{
	public partial class MasterAdd :Models.AdminMaster
	{
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Userpwd1 { get; set; } = "";

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Userpwd2 { get; set; } = "";

		public bool IsAdmin { get; set; } = false;

		public List<SelectListItem> Roleids { get; set; }
		public string Lang { get; set; } = "";
	}
}
