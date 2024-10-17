using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WeTalk.Web.ViewModels.Course
{
	public partial class ListAdd : Models.WebCourse
	{
		public string Lang { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string Curricula { get; set; }
		public string Keys { get; set; }
	}
}
