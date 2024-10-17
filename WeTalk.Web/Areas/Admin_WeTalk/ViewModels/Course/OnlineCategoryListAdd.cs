using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WeTalk.Web.ViewModels.Course
{
	public partial class OnlineCategoryListAdd : Models.WebOnlineCategory
	{
		public string Lang { get; set; }
		public string Title { get; set; }
	}
}
