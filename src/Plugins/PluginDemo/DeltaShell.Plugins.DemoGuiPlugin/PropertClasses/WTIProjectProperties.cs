using System.ComponentModel;
using DelftTools.Shell.Gui;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects;

namespace DeltaShell.Plugins.DemoGuiPlugin.PropertClasses
{
    [DisplayName("WTI project")]
    public class WTIProjectProperties : ObjectProperties<WTIProject>
    {
        [Category("General")]
        [DisplayName("Name")]
        [Description("The name of the WTI project")]
        public string Name
        {
            get { return data.Name; }
            set
            {
                data.Name = value;
                data.NotifyObservers();
            }
        }
    }
}