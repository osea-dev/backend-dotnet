using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WeTalk.Web.ViewModels.Course
{
	public partial class VideoListAdd : Models.WebShortVideo
	{
		public string Lang { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string Keys { get; set; }
		public string Intro { get; set; }
		public string EncryptData { get; set; }
		public List<SelectListItem> TeacherItem { get; set; }
	}
}
