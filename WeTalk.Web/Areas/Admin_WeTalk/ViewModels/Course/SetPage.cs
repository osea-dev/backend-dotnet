using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.Course
{
	public partial class SetPage : Models.WebCourseGroupInfo
	{
		public string Lang { get; set; }
		public string Content { get; set; }
		public string EncryptData { get; set; }
		public string Sty { get; set; }
	}
}
