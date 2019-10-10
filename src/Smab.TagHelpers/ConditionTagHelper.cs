using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

/// <summary>
/// From the Microsoft samples at 
///     https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/authoring
///     https://github.com/aspnet/Mvc/tree/dev/src/Microsoft.AspNetCore.Mvc.TagHelpers
/// </summary>
namespace Smab.TagHelpers
{
    [HtmlTargetElement(Attributes = ConditionAttributeName)]
    public class ConditionTagHelper : TagHelper
    {
		private const string ConditionAttributeName = "condition";

		[HtmlAttributeName(ConditionAttributeName)]
        public bool Condition { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Condition)
            {
                output.SuppressOutput();
            }
        }
    }
}
