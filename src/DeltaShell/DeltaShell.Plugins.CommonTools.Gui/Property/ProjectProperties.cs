using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DeltaShell.Plugins.CommonTools.Gui.Properties;

namespace DeltaShell.Plugins.CommonTools.Gui.Property
{
    [ResourcesDisplayName(typeof(Resources), "ProjectProperties_DisplayName")]
    public class ProjectProperties : ObjectProperties<Project>
    {
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ProjectProperties_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ProjectProperties_Description_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ProjectProperties_Description_Description")]
        public string Description
        {
            get
            {
                return data.Description;
            }
            set
            {
                data.Description = value;
            }
        }
    }
}