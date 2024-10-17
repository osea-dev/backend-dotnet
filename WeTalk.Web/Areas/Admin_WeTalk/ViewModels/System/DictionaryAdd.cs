using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeTalk.Models;

namespace WeTalk.Web.ViewModels.System
{
	public partial class DictionaryAdd : ViewPubConfig
	{
		public bool IsCurrency { get; set; }
	}
}
