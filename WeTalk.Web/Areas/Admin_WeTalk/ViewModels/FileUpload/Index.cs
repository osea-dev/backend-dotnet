using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.FileUpload
{
	public partial class Index
	{
		public int FileCount { get; set; }
		public decimal FileSize { get; set; }
		public decimal FileTotalSize { get; set; }
		public string Obj { get; set; }
	}
}
