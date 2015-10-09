using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DeltaShell.Plugins.CommonTools.Gui.Properties;

namespace DeltaShell.Plugins.CommonTools.Gui.Property
{
    [ResourcesDisplayName(typeof(Resources), "UrlProperties_DisplayName")]
    public class UrlProperties : ObjectProperties<Url>
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
                return data.Path;
            }
            set
            {
                data.Path = value;
            }
        }
    }
}