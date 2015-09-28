using System.ComponentModel;
using DelftTools.Shell.Gui;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Domain;

namespace DeltaShell.Plugins.DemoGuiPlugin.PropertClasses.Domain
{
    [DisplayName("Hydraulic boundaries database")]
    public class HydraulicBoundariesDatabaseProperties : ObjectProperties<HydraulicBoundariesDatabase>
    {
        [Category("General")]
        [DisplayName("Database path")]
        [Description("The path to the hydraulic boundaries database")]
        public string DataBasePath
        {
            get { return data.DataBasePath; }
        }
    }
}