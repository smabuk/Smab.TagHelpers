using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Smab.TagHelpers
{
	/// <summary>
	/// TagHelper to create complex font-awesome combinations
	/// Make sure the CDN has been added in your <head>.
	///     E.g. <script defer src="https://use.fontawesome.com/releases/v5.0.6/js/all.js"></script>
	/// </summary>
	[HtmlTargetElement("fa-custom")]
    [HtmlTargetElement("font-awesome-custom")]
    public class FontAwesomeCustomTagHelper : TagHelper
    {
        /// <summary>
        /// Name of the custom font-awesome mix-up to render
        /// </summary>
        [HtmlAttributeName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Font-awesome icon name without the style and fa-prefix
        /// </summary>
        [HtmlAttributeName("icon")]
        public string Icon { get; set; }

        [HtmlAttributeName("date")]
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Relative size of the fonts to be used
        /// replacing
        ///     fa-2x, fa-3x etc.
        /// </summary>
        [HtmlAttributeName("size")]
        public int Size { get; set; }

        /// <summary>
        /// Type represents the icon style to be used.
        /// One of
        ///     Solid, Regular, Light or Brand
        /// replacing
        ///     fas, far, fal or fab
        /// </summary>
        [HtmlAttributeName("type")]
        public FontAwesomeType Type { get; set; } = FontAwesomeType.Solid;

        [HtmlAttributeName("position")]
        public FontAwesomePosition Position { get; set; } = FontAwesomePosition.TopRight;

        [HtmlAttributeName("value")]
        public object Value { get; set; }

        [HtmlAttributeName("class")]
        public string CssClass { get; set; }

        private string iconName;
        private string faTypeString;
        private TagHelperContent childContent;

        public override int Order => base.Order;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Name))
            {
                await base.ProcessAsync(context, output);
                return;
            }
            if (!string.IsNullOrEmpty(Icon))
            {
                iconName = Icon.Trim();
            }

            output.TagMode = TagMode.StartTagAndEndTag;
            // "fa" plus the 1st character of the FontAwesomeType (Solid, Brand etc.)
            faTypeString = $"fa{Type.ToString().ToLower().Substring(0, 1)}";
            if (Size > 0)
            {
                CssClass += $" fa-{Size}x";
            }
            output.TagName = "span";
            output.Attributes.SetAttribute("class", $"{CssClass}".Trim());

            childContent = await output.GetChildContentAsync();

            switch (Name.ToLowerInvariant())
            {
                case "calendar-date":
                    MakeCalendar(output);
                    break;
                case "counter":
                    MakeCounter(output);
                    break;
                case "today":
                    Date = DateTime.UtcNow;
                    MakeCalendar(output);
                    break;
                default:
                    break;
            }

            await base.ProcessAsync(context, output);
            return;
        }

        private void MakeCalendar(TagHelperOutput output)
        {
            output.Attributes.SetAttribute("class", $"{CssClass} fa-layers fa-fw".Trim());

            // This code would be better, but I can't chain TagHelpers it seems
                //TagBuilder fa1 = new TagBuilder("fa");
                //fa1.Attributes.Add("icon", "calendar");
                //fa1.Attributes.Add("type", Type.ToString());
                //fa1.RenderSelfClosingTag();
                //output.Content.AppendHtml(fa1);

            TagBuilder fa1 = new TagBuilder("i");
            fa1.AddCssClass($"{faTypeString} fa-calendar");
            output.Content.AppendHtml(fa1);

            TagBuilder span1 = new TagBuilder("span");
            span1.AddCssClass($"fa-layers-text");
            switch (Type)
            {
                case FontAwesomeType.Solid:
                    span1.AddCssClass($"fa-inverse");
                    span1.Attributes.Add("data-fa-transform", "shrink-13.5 up-4.8");
                    span1.Attributes.Add("style", "font-weight:500");
                    break;
                case FontAwesomeType.Regular:
                    span1.AddCssClass($"fa-inverse");
                    span1.Attributes.Add("data-fa-transform", "shrink-13.5 up-4.6");
                    span1.Attributes.Add("style", "font-weight:500");
                    break;
                case FontAwesomeType.Light:
                    span1.Attributes.Add("data-fa-transform", "shrink-14 up-4.2");
                    span1.Attributes.Add("style", "font-weight:900");
                    break;
            }
            span1.InnerHtml.Append(Date.ToString("MMMM"));
            output.Content.AppendHtml(span1);

            switch (Type)
            {
                case FontAwesomeType.Solid:
                    span1 = new TagBuilder("span");
                    span1.AddCssClass($"fa-layers-text fa-inverse");
                    span1.Attributes.Add("data-fa-transform", "shrink-8.5 down-2.5");
                    break;
                case FontAwesomeType.Light:
                    span1 = new TagBuilder("span");
                    span1.AddCssClass($"fa-layers-text");
                    span1.Attributes.Add("data-fa-transform", "shrink-8.5 down-2");
                    break;
                case FontAwesomeType.Regular:
                    span1 = new TagBuilder("span");
                    span1.AddCssClass($"fa-layers-text");
                    span1.Attributes.Add("data-fa-transform", "shrink-8.5 down-1.3");
                    break;
            }

            span1.Attributes.Add("style", "font-weight:900");
            span1.InnerHtml.Append(Date.Day.ToString());
            output.Content.AppendHtml(span1);
        }

        private void MakeCounter(TagHelperOutput output)
        {
            output.Attributes.SetAttribute("class", $"{CssClass} fa-layers fa-fw".Trim());

            TagBuilder fa1 = new TagBuilder("i");
            fa1.AddCssClass($"{faTypeString} fa-{iconName}");
            output.Content.AppendHtml(fa1);

            string position = Position.ToString().Replace("R","-R").Replace("L","-L").ToLowerInvariant();

            TagBuilder span1 = new TagBuilder("span");
            span1.AddCssClass($"fa-layers-counter fa-layers-{position}");
            span1.InnerHtml.AppendHtml(childContent);
            output.Content.AppendHtml(span1);
        }

    }
}
