using System.ComponentModel.DataAnnotations;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.System
{
	public partial class MenuFunctionAdd: AdminMenuFunction
	{

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string MenuTitle { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Title { get; set; }

	}
}
