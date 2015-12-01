using System.Collections.Generic;
using System.Linq;

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Ringtoets.Common.Forms.Helpers;
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
                CreateData = owner =>
                {
                    var project = (Project)owner;
                    var dikeAssessmentSection = new DikeAssessmentSection();
                    dikeAssessmentSection.Name = GetUniqueForAssessmentSectionName(project, dikeAssessmentSection.Name);
                    return dikeAssessmentSection;
                }
            };
            yield return new DataItemInfo<DuneAssessmentSection>
            {
                Name = RingtoetsFormsResources.DuneAssessmentSection_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsFormsResources.AssessmentSectionFolderIcon,
                CreateData = owner =>
                {
                    var project = (Project)owner;
                    var duneAssessmentSection = new DuneAssessmentSection();
                    duneAssessmentSection.Name = GetUniqueForAssessmentSectionName(project, duneAssessmentSection.Name);
                    return duneAssessmentSection;
                }
            };
        }

        private static string GetUniqueForAssessmentSectionName(Project project, string baseName)
        {
            return NamingHelper.GetUniqueName(project.Items.OfType<AssessmentSectionBase>(), baseName, a => a.Name);
        }
    }
}