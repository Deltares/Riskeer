using System.Collections.Generic;
using Core.Common.Base;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

using WtiFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin
{
    public class WtiApplicationPlugin : ApplicationPlugin
    {
        public override string DisplayName
        {
            get
            {
                return Properties.Resources.Wti_application_DisplayName;
            }
        }

        public override string Description
        {
            get
            {
                return Properties.Resources.Wti_application_Description;
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
                Name = WtiFormsResources.WtiProjectProperties_DisplayName,
                Category = ApplicationResources.Wti_application_name,
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