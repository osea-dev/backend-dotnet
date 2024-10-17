using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels
{
	public partial class Error
	{

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Code { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Msg { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string ReturnUrl { get; set; }
	}
}
