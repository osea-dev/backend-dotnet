using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.Teacher
{
	public partial class ListAdd : Models.WebTeacher
	{
		public List<SelectListItem> LangItems { get; set; }
		public List<SelectListItem> TimezoneItems { get; set; }
		public string Lang { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		public string Tag3 { get; set; }
		public string Tag4 { get; set; }
		public string Tag5 { get; set; }
		public string Tag6 { get; set; }
		public string Tag7 { get; set; }
		public string Tag8 { get; set; }
		public string Tag9 { get; set; }
		public string Tag10 { get; set; }
		public string Tag11 { get; set; }
		public string Tag12 { get; set; }
		public string Name { get; set; }
		public string Message { get; set; }
		public string Keys { get; set; }
		public string Comefrom { get; set; }
		public string Edu { get; set; }
		public string Religion { get; set; }
		public string Advantage { get; set; }
		public string Motto { get; set; }
		public string Philosophy { get; set; }
		public List<SelectListItem> CountryCodeItem { get; set; }
	}
}
