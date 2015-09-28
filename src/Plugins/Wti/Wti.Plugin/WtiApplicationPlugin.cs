using DelftTools.Shell.Core;

using Mono.Addins;

namespace Wti.Plugin
{
    [Extension(typeof(IPlugin))]
    public class WtiApplicationPlugin : ApplicationPlugin
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
                return Properties.Resources.WtiApplicationDisplayName;
            }
        }

        public override string Description
        {
            get
            {
                return Properties.Resources.WtiApplicationDescription;
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