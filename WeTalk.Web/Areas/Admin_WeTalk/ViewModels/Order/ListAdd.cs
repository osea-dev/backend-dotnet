using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.Order
{
	public partial class ListAdd : Models.WebOrder
	{
		public string Img { get; set; }
		public string PayTitle { get; set; }
		public string Message { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Mobile { get; set; }
		public string SkuType { get; set; }
		public List<SelectListItem> CurrencyCodeItem { get; set; }
        public int LessonCount { get; set; }
        public string LessonStart { get; set; }
        public int StudentCount { get; set; }
    }
}
