using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Smab.TagHelpers
{
    /// <summary>
    /// TagHelper to create the <i></i> link for font awesome 5.x.
    /// Make sure the CDN has been added in your <head>.
    ///     E.g. <script defer src="https://use.fontawesome.com/releases/v5.0.6/js/all.js"></script>
    /// </summary>
    [HtmlTargetElement("fa")]
    [HtmlTargetElement("font-awesome")]
    public class FontAwesomeTagHelper : TagHelper
    {
        /// <summary>
        /// Font-awesome icon name without the style and fa-prefix
        /// </summary>
        [HtmlAttributeName("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Type represents the icon style to be used.
        /// One of
        ///     Solid, Regular, Light or Brand
        /// replacing
        ///     fas, far, fal or fab
        /// </summary>
        [HtmlAttributeName("type")]
        public FontAwesomeType Type { get; set; } = FontAwesomeType.Solid;

        /// <summary>
        /// Relative size of the fonts to be used
        /// replacing
        ///     fa-2x, fa-3x, fa-lg etc.
		/// Specify either an integer or lg, sm, xs    
        /// </summary>
        [HtmlAttributeName("size")]
        public string Size { get; set; }

        [HtmlAttributeName("class")]
        public string CssClass { get; set; }

        private string iconName;
		private string faTypeString;
		private TagHelperContent childContent;

        public override int Order => base.Order + 200;

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Icon))
            {
				childContent = await output.GetChildContentAsync();
				if (childContent.IsEmptyOrWhiteSpace)
				{
					await base.ProcessAsync(context, output);
					return;
				}
				else
				{
					Icon = "ignore";
				}
            }

            output.TagName = "i";
            output.TagMode = TagMode.StartTagAndEndTag;

            iconName = Icon.Trim();

            // "fa" plus the 1st character of the FontAwesomeType (Solid, Brand etc.)
            faTypeString = $"fa{Type.ToString().ToLower().Substring(0, 1)}";

			if (!string.IsNullOrWhiteSpace(Size))
			{
				if (int.TryParse(Size, out int sizeValue) && sizeValue > 0)
				{
					CssClass += $" fa-{sizeValue}x";
				}
				else
				{
					CssClass += $" fa-{Size}";
				}
			}	

            output.Attributes.SetAttribute("class", $"{CssClass} {faTypeString} fa-{iconName}".Trim());



			await base.ProcessAsync(context, output);
			return;
        }

    }
}
