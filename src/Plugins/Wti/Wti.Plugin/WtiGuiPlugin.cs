using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;

using Mono.Addins;

namespace Wti.Plugin
{
    [Extension(typeof(IPlugin))]
    public class WtiGuiPlugin : GuiPlugin
    {
        public override string Name
        {
            get
            {
                return Properties.Resources.WtiApplicationName;
            }
        }

        public override string DisplayName
        {
            get
            {
                return Properties.Resources.wtiGuiPluginDisplayName;
            }
        }

        public override string Description
        {
            get
            {
                return Properties.Resources.wtiGuiPluginDescription;
            }
        }

        public override string Version
        {
            get
            {
                return "0.5.0.0";
            }
        }
    }
}