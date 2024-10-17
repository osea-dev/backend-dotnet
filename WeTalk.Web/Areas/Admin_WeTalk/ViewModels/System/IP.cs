using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.System
{
	public partial class IP
	{
		public IP()
		{
			WhileIp = "";
			BlackIp = "";
		}

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string WhileIp { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string BlackIp { get; set; }
	}
}
