using System.ComponentModel.DataAnnotations;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.System
{
	public partial class MenuAdd : AdminMenu
	{

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Title { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Url { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Parameter { get; set; } = "";

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string DataUrl { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Area { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Controller { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Action { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Method { get; set; }

		public string Lang { get; set; }
	}
}
