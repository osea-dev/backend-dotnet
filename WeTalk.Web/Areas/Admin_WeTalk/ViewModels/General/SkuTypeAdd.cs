using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.General
{
	public partial class SkuTypeAdd:WebSkuType
	{
		public string Lang { get; set; }
		public string Title { get; set; }
		public List<SelectListItem> MenkeTemplateItem { get; set; }
	}
}
