using System.Collections.Generic;
using Core.Common.Base;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

using RingtoetsFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin
{
    public class RingtoetsApplicationPlugin : ApplicationPlugin
    {
        public override IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield return new DataItemInfo<AssessmentSection>
            {
                Name = RingtoetsFormsResources.AssessmentSectionProperties_DisplayName,
                Category = RingtoetsFormsResources.AssessmentSectionProperties_Category,
                Image = RingtoetsFormsResources.AssessmentSectionFolderIcon,
                CreateData = owner => new AssessmentSection()
            };
        }

        public override IEnumerable<IFileImporter> GetFileImporters()
        {
            yield return new PipingSurfaceLinesCsvImporter();
            yield return new PipingSoilProfilesImporter();
        }
    }
}