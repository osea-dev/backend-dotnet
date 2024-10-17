using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels
{
	public partial class Main
	{

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string LeftHtml_f { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string LeftHtml { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string HeadImg { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string AdminName { get; set; }
		
		public string LangTtile { get; set; }

		public string LangList { get; set; }
	}
}
