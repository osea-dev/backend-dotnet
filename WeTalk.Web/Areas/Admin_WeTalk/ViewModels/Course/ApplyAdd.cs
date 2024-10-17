using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.Course
{
	public partial class ApplyAdd : Models.WebCourseApply
	{
		public string CourseName { get; set; }
		public string StudentName { get; set; }
		public string MenkeLiveSerial { get; set; }
		public string SkuType { get; set; }
		public int ClassHour { get; set; }
	}
}
