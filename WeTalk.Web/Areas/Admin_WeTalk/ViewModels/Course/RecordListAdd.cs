using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WeTalk.Web.ViewModels.Course
{
	public partial class RecordListAdd : Models.WebRecordCourse
	{
		public string Lang { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string Keys { get; set; }
		public string IntroUp { get; set; }
		public string IntroLow { get; set; }
		public string Content { get; set; }
		public string EncryptDataUp { get; set; }
		public string EncryptDataLow { get; set; }
		public List<PubCurrencyPrice> ListPubCurrency { get; set; }
		public List<SelectListItem> TeacherItem { get; set; }
	}
}
