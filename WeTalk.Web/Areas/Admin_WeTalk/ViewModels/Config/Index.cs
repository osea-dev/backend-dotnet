using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WeTalk.Web.ViewModels.Config
{
	public partial class Index: Models.ViewPubConfig
    {

        public List<SelectListItem> AppletsItem { get; set; }
    }
}
