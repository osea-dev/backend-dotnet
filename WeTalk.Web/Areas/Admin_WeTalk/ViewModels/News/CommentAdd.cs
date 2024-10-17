using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.News
{
	public partial class CommentAdd : Models.WebNewsComment
	{
		public string Name { get; set; }
		public string Mobile { get; set; }
		public string Message1 { get; set; }
	}
}
