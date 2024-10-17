using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.User
{
	public partial class UserCourseAdject : WebUserCourse
	{
		public string Students { get; set; }
		public string MenkeName { get; set; }
		public string Message1 { get; set; }
		public List<SelectListItem> MenkeCourseItem { get; set; }
		public List<SelectListItem> CourseItem { get; set; }
		public List<SelectListItem> SkuTypeItem { get; set; }
		public List<SelectListItem> StudentItem { get; set; }
	}
}
