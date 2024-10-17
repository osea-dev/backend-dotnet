using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.Email
{
	public partial class ListAdd : Models.WebEmailTemplate
	{
		public List<SelectListItem> LangItems {get;set;}
	}
}
