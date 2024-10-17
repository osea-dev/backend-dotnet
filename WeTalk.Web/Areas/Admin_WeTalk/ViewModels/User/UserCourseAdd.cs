using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.User
{
	public partial class UserCourseAdd : WebUserCourse
	{
		public string Message1 { get; set; }
		public string MenkeName { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string MobileCode { get; set; }
		public string Mobile { get; set; }
		public string CountryCode { get; set; }
		public string SkuType { get; set; }
		/// <summary>
		/// 小班课合并方式,0全新建课，1自动合并
		/// </summary>
		public int IsAuto { get; set; }
		public List<SelectListItem> MenkeCourseItem { get; set; }
		public List<SelectListItem> CourseItem { get; set; }
		public List<SelectListItem> SkuTypeItem { get; set; }
		public List<SelectListItem> StudentItem { get; set; }
		public int MenkeType { get; set; }
	}
}
