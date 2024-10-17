using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.System
{
	public partial class Config
	{
		public bool IsClose { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string CloseTips { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string SiteName { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string SiteUrl { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string SiteTitle { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string SiteKeyWords { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string SiteDescription { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string UpFileType { get; set; }
		public int UpFileSize { get; set; }
		public string Sendname { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string MailService { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string SendMail { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string SendPwd { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Fooder { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string SiteCopyRight { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string ICP { get; set; }
		public string Lang { get; set; }
	}
}
