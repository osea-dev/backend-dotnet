using System.ComponentModel.DataAnnotations;

namespace WeTalk.Web.ViewModels.System
{
	public partial class Dictionary
	{
		public long admin_menuid { get; set; }
		public long Fid { get; set; }

		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string ScriptStr { get; set; }
	}
}
