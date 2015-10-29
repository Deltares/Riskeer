using System.Collections.Generic;
using Core.Common.BaseDelftTools;
using Mono.Addins;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

using WtiFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin
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
                Category = ApplicationResources.WtiApplicationName,
                Image = WtiFormsResources.WtiProjectFolderIcon,
                CreateData = owner => new WtiProject()
            };
        }

        public override IEnumerable<IFileImporter> GetFileImporters()
        {
            yield return new PipingSurfaceLinesCsvImporter();
            yield return new PipingSoilProfilesImporter();
        }
    }
}