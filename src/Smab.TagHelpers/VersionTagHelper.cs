using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Smab.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("assembly-info")]
    [HtmlTargetElement("version")]
    public class VersionTagHelper : TagHelper
    {
        public enum VersionType
        {
            FileVersion = 1,
            ProductVersion = 2,
            InformationalVersion = 2,
            AssemblyVersion = 3,

            Location = 6,
            CodeBase = 7,

            Company = 11,
            Configuration = 12,
            Copyright = 13,
            Description = 14,
            Product = 15,
            Title = 16,
            Trademark = 17
        }

        /// <summary>
        /// Assembly represents the type of the assembly to be versioned.
        /// </summary>
        [HtmlAttributeName("assembly")]
        public System.Type? AssemblyType { get; set; }

        /// <summary>
        /// Type represents the type of version to be returned
        /// One of FileVersion, ProductVersion or AssemblyVersion
        ///   or any of the extra Custom Attributes
        /// Defaults to ProductVersion (InformationalVersion)
        /// </summary>
        [HtmlAttributeName("type")]
        public VersionType Type { get; set; } = VersionType.ProductVersion;

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string versionString = "";
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            var childContent = await output.GetChildContentAsync();
            output.Content.AppendHtml(childContent);
            if (AssemblyType == null)
            {
                AssemblyType = GetType();
            }
            switch (Type)
            {
                case VersionType.FileVersion:
                    versionString = AssemblyType
                        .GetTypeInfo().Assembly
                        .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                        .Version ?? "";
                    break;
                case VersionType.ProductVersion:    // also covers VersionType.InformationalVersion
                    versionString = AssemblyType
                        .GetTypeInfo().Assembly
                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                        .InformationalVersion ?? "";
                    break;
                case VersionType.AssemblyVersion:
                    versionString = AssemblyType
                        .Assembly.GetName().Version.ToString();
                    break;
                case VersionType.Location:
                    if (!AssemblyType.Assembly.IsDynamic)
                    {
                        versionString = AssemblyType.Assembly.Location ?? "";
                    }
                    break;
                case VersionType.CodeBase:
                    versionString = AssemblyType.Assembly.CodeBase ?? "";
                    break;
                default:
                    //SortedDictionary<string, string> customAttributes = new SortedDictionary<string, string>();
                    foreach (var ca in AssemblyType.Assembly.CustomAttributes)
                    {
                        if (Type.ToString().ToLowerInvariant() == ca.AttributeType.ToString().Replace("System.Reflection.Assembly", "").Replace("Attribute", "").ToLowerInvariant())
                        {
                            versionString = ca.ConstructorArguments[0].Value.ToString();
                        }
                    }
                    break;
            }
            output.Content.Append(versionString);

            await base.ProcessAsync(context, output);
            return;
        }
    }
}
