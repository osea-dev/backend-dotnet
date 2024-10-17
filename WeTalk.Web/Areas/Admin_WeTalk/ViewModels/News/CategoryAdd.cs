using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.News
{
	public partial class CategoryAdd : Models.WebNewsCategory
	{
		public string Lang { get; set; }
		public string Title{get;set;}
		public List<SelectListItem> LangItem { get; set;}
	}
}
