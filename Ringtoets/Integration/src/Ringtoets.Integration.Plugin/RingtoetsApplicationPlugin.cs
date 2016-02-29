// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Plugin;

using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;

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
                    var project = (Project) owner;
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
                    var project = (Project) owner;
                    var duneAssessmentSection = new DuneAssessmentSection();
                    duneAssessmentSection.Name = GetUniqueForAssessmentSectionName(project, duneAssessmentSection.Name);
                    return duneAssessmentSection;
                }
            };
        }

        public override IEnumerable<IFileImporter> GetFileImporters()
        {
            yield return new ReferenceLineImporter();
            yield return new FailureMechanismSectionsImporter();
        }

        private static string GetUniqueForAssessmentSectionName(Project project, string baseName)
        {
            return NamingHelper.GetUniqueName(project.Items.OfType<AssessmentSectionBase>(), baseName, a => a.Name);
        }
    }
}