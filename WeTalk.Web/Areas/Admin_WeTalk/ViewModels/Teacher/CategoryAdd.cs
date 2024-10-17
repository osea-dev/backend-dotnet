using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.Teacher
{
	public partial class CategoryAdd : Models.WebTeacherCategory
	{
		public string Lang { get; set; }
		public string Title { get; set; }
	}
}
