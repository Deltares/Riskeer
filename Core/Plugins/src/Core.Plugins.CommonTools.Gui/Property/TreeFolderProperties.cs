using Core.Common.Gui;
using Core.Common.Gui.Swf;
using Core.Common.Utils.Attributes;
using Core.Plugins.CommonTools.Gui.Properties;

namespace Core.Plugins.CommonTools.Gui.Property
{
    [ResourcesDisplayName(typeof(Resources), "TreeFolderProperties_DisplayName")]
    public class TreeFolderProperties : ObjectProperties<TreeFolder>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "TreeFolderProperties_Name_Description")]
        public string Text
        {
            get
            {
                return data.Text;
            }
        }
    }
}