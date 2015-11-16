using System.Collections.Generic;

using Core.Common.Base;

using Ringtoets.Integration.Data;
using Ringtoets.Piping.Plugin;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin
{
    public class RingtoetsApplicationPlugin : ApplicationPlugin
    {
        public override IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield return new DataItemInfo<DikeAssessmentSection>
            {
                Name = RingtoetsFormsResources.AssessmentSectionProperties_DisplayName,
                Category = RingtoetsFormsResources.Ringtoets_Category,
                Image = RingtoetsFormsResources.AssessmentSectionFolderIcon,
                CreateData = owner => new DikeAssessmentSection()
            };
        }

        public override IEnumerable<IFileImporter> GetFileImporters()
        {
            foreach (var pipingFileImporter in PipingFileImporterProvider.GetFileImporters())
            {
                yield return pipingFileImporter;
            }
        }
    }
}