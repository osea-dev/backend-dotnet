using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.News
{
	public partial class ListAdd : Models.WebNews
	{
		public string Lang { get; set; }
        public List<SelectListItem> LangItem { get; set; }
        public List<SelectListItem> CategoryItems { get; set; }
		public string EncryptData { get; set; }
		public string Title { get; set; }
		public string Keys { get; set; }
		public string Message { get; set; }
		public string Intro { get; set; }
		public string PreView { get; set; }
	}
}
