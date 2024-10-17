using System;
using System.ComponentModel.DataAnnotations;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.General
{
	public partial class KeysAdd:PubKeys
	{
		public string Title { get; set; }
		public string Lang { get; set; }
	}
}
