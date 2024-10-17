using System.ComponentModel.DataAnnotations;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.System
{
	public partial class RoleAdd :ViewAdminRole
	{

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string MenuTabPane { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string MenuTab { get; set; }

	}
}
