using System.ComponentModel;
using Core.Common.BaseDelftTools.Workflow;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Gui.Properties;

namespace Core.Plugins.CommonTools.Gui.Property
{
    [ResourcesDisplayName(typeof(Resources), "CompositeActivityProperties_DisplayName")]
    public class CompositeActivityProperties : ObjectProperties<ICompositeActivity>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Common_Name_Description")]
        [PropertyOrder(1)]
        [ReadOnly(true)]
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
    }
}