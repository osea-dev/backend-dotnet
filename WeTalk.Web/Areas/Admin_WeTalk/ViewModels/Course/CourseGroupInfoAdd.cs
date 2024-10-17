using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WeTalk.Web.ViewModels.Course
{
	public partial class CourseGroupInfoAdd : Models.WebCourseGroupInfo
	{
		public string Message { get; set; }
		public string Lang { get; set; }
		public string Title { get; set; }
		public string Keys { get; set; }
		public string CourseName { get; set; }
		public string Intro { get; set; }
		public string Objectives { get; set; }
		public string Crowd { get; set; }
		public string Merit { get; set; }
		public string Catalog { get; set; }
		public List<SelectListItem> CourseGroupItem { get; set; }
	}
}
