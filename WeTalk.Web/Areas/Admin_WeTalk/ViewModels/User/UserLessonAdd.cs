using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.User
{
	public partial class UserLessonAdd : WebUserLesson
	{
		public string Message { get; set; }
		public string TeacherCode { get; set; }
		public int IsReport { get; set; }
	}
}
