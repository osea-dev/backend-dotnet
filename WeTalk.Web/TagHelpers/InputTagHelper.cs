using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WeTalk.Web.TagHelpers
{
	public class InputTagHelper : TagHelper
	{
		public bool Disabled { get; set; }
		public bool Required { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (Disabled)
			{
				output.Attributes.SetAttribute("disabled", true);
			}
			if (Required)
			{
				output.Attributes.SetAttribute("required", true);
			}
			//output.SuppressOutput();
		}
	}
}
