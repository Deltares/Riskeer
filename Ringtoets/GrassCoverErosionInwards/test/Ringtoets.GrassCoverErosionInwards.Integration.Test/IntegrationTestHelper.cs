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

using System.IO;
using Core.Common.Base.Service;
using Core.Common.Utils.IO;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.Integration.Data;

namespace Ringtoets.GrassCoverErosionInwards.Integration.Test
{
    /// <summary>
    /// Helper methods related to integration tests.
    /// </summary>
    public static class IntegrationTestHelper
    {
        /// <summary>
        /// Imports the reference line on the <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import the reference line on.</param>
        public static void ImportReferenceLine(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(IntegrationTestHelper).Assembly,
                                                                                   true,
                                                                                   "traject_6-3.shp",
                                                                                   "traject_6-3.dbf",
                                                                                   "traject_6-3.prj",
                                                                                   "traject_6-3.shx"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3.shp");
                var activity = new FileImportActivity(new ReferenceLineImporter(assessmentSection, filePath),
                                                      "ReferenceLineImporter");

                activity.Run();
                activity.Finish();
            }
        }

        /// <summary>
        /// Imports the failure mechanism sections.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to import on.</param>
        /// <remarks>This will import 283 failure mechanism sections.</remarks>
        public static void ImportFailureMechanismSections(AssessmentSection assessmentSection, IFailureMechanism failureMechanism)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(IntegrationTestHelper).Assembly,
                                                                                   true,
                                                                                   "traject_6-3_vakken.shp",
                                                                                   "traject_6-3_vakken.dbf",
                                                                                   "traject_6-3_vakken.prj",
                                                                                   "traject_6-3_vakken.shx"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3_vakken.shp");
                var activity = new FileImportActivity(new FailureMechanismSectionsImporter(failureMechanism, assessmentSection.ReferenceLine, filePath),
                                                      "FailureMechanismSectionsImporter");

                activity.Run();
                activity.Finish();
            }
        }
    }
}