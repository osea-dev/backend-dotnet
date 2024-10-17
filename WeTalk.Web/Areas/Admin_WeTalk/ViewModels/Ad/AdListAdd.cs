using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WeTalk.Web.ViewModels.Ad
{
	public partial class AdListAdd : Models.WebAd
    {
        public List<SelectListItem> TeamTypeItems { get; set; }
        public List<SelectListItem> AppletsItem { get; set; }
        public List<SelectListItem> AppletsCompanyItem { get; set; }
    }
}
