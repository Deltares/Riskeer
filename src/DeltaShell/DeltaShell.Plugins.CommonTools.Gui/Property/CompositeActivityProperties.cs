using DelftTools.Shell.Core.Workflow;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DeltaShell.Plugins.CommonTools.Gui.Properties;
using System.ComponentModel;

namespace DeltaShell.Plugins.CommonTools.Gui.Property
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
            get { return data.Name; }
            set { data.Name = value; }
        }
    }
}