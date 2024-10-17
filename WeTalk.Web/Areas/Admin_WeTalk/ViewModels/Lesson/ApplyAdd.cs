using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.Lesson
{
	public partial class CancelApplyAdd : Models.WebUserLessonCancel
	{
		public string MenkeCourseName { get; set; }
		public string MenkeLessonName { get; set; }
		public string MenkeLiveSerial { get; set; }
		public string MenkeStudentName { get; set; }
		public string MenkeStudentMobile { get; set; }
		public string MenkeTeacherName { get; set; }
		public string MenkeTeacherMobile { get; set; }
	}
}
