using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.User
{
	public partial class UserLessonReport : WebUserLessonReport
	{
		public bool IsReport { get; set; }
		public string MenkeCourseName { get; set; }
		public string MenkeLessonName { get; set; }
		public DateTime Begintime { get; set; }
		public DateTime Endtime { get; set; }
		public string TeacherName { get; set; }
		public string TeacherMobileCode { get; set; }
		public string TeacherMobile { get; set; }
		public string StudentName { get; set; }
		public string StudentMobileCode { get; set; }
		public string StudentMobile { get; set; }
	}
}
