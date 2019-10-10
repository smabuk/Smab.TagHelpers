using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smab.TagHelpers
{
	[HtmlTargetElement(Attributes = nameof(Hide))]
	public class HideTagHelper : TagHelper
	{
		public bool Hide { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (Hide)
			{
				output.SuppressOutput();
			}
		}
	}
}
