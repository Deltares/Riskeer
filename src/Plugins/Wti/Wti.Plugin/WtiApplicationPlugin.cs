using System.Collections.Generic;
using DelftTools.Shell.Core;
using Mono.Addins;
using Wti.Data;
using WtiFormsResources = Wti.Forms.Properties.Resources;

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

        public override IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield return new DataItemInfo<WtiProject>
            {
                Name = WtiFormsResources.WtiProjectPropertiesDisplayName,
                Category = "WTI",
                Image = WtiFormsResources.WtiProjectFolderIcon,
                CreateData = owner => new WtiProject()
            };
            yield return new DataItemInfo<PipingFailureMechanism>
            {
                Name = WtiFormsResources.PipingFailureMechanismDisplayName,
                Category = "WTI",
                Image = WtiFormsResources.PipingIcon,
                CreateData = owner => new PipingFailureMechanism()
            };
        }
    }
}