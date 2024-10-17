using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.System
{
	public partial class MasterData : Models.AdminMaster
	{
		public string RoleName { get; set; }
	}
}
