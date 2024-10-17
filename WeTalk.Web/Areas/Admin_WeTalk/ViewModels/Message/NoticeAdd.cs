using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.News
{
	public partial class NoticeAdd : Models.WebMessage
	{
		public string SendName { get; set; }
		public string AccName { get; set; }
	}
}
