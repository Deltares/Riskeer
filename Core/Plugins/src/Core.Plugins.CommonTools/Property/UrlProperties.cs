using System;

using Core.Common.Gui;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Plugins.CommonTools.Properties;

namespace Core.Plugins.CommonTools.Property
{
    [ResourcesDisplayName(typeof(Resources), "UrlProperties_DisplayName")]
    public class UrlProperties : ObjectProperties<WebLink>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "UrlProperties_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "UrlProperties_Path_DisplayName")]
        [ResourcesDescription(typeof(Resources), "UrlProperties_Path_Description")]
        public string Path
        {
            get
            {
                return data.Path.ToString();
            }
            set
            {
                data.Path = new Uri(value);
            }
        }
    }
}