using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.System
{
	public partial class DictionaryData
	{
		public long pub_configid { get; set; }

		public string applets { get; set; }

		public string app_name { get; set; }

		public int isadmin { get; set; }

		public string title { get; set; }

		public string description { get; set; }

		public string name { get; set; }

		public string val { get; set; }

		public string dtime { get; set; }

		public string status { get; set; }
	}
}
