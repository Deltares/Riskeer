using System.Collections.Generic;

using Core.Common.Base;

using Ringtoets.Integration.Data;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin
{
    /// <summary>
    /// The application plugin for Ringtoets.
    /// </summary>
    public class RingtoetsApplicationPlugin : ApplicationPlugin
    {
        public override IEnumerable<DataItemInfo> GetDataItemInfos()
        {
            yield return new DataItemInfo<DikeAssessmentSection>
            {
                Name = RingtoetsFormsResources.DikeAssessmentSection_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsFormsResources.AssessmentSectionFolderIcon,
                CreateData = owner => new DikeAssessmentSection()
            };
        }
    }
}