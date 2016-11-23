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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Integration.TestUtils
{
    /// <summary>
    /// Helper methods related to importing data for integration tests.
    /// </summary>
    public static class DataImportHelper
    {
        /// <summary>
        /// Imports the <see cref="ReferenceLine"/> on the <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import the reference line on.</param>
        public static void ImportReferenceLine(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataImportHelper).Assembly,
                                                                                   true,
                                                                                   "traject_6-3.shp",
                                                                                   "traject_6-3.dbf",
                                                                                   "traject_6-3.prj",
                                                                                   "traject_6-3.shx"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3.shp");
                var activity = new FileImportActivity(new ReferenceLineImporter(assessmentSection,
                                                                                new ReferenceLineReplacementHandler(),
                                                                                filePath),
                                                      "ReferenceLineImporter");
                activity.Run();
                activity.Finish();
            }
        }

        /// <summary>
        /// Imports the <see cref="FailureMechanismSection"/> data for a given <see cref="IFailureMechanism"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to import on.</param>
        /// <remarks>This will import 283 failure mechanism sections.</remarks>
        public static void ImportFailureMechanismSections(AssessmentSection assessmentSection, IFailureMechanism failureMechanism)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataImportHelper).Assembly,
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

        /// <summary>
        /// Imports the <see cref="HydraulicBoundaryDatabase"/> for the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <remarks>This will import 19 Hydraulic boundary locations.</remarks>
        public static void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataImportHelper).Assembly,
                                                                                   false,
                                                                                   "HRD dutch coast south.sqlite",
                                                                                   "HLCD.sqlite",
                                                                                   "HRD dutch coast south.config.sqlite"))
            using (var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryDatabaseImporter())
            {
                var filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "HRD dutch coast south.sqlite");
                hydraulicBoundaryDatabaseImporter.Import(assessmentSection, filePath);
            }
        }

        #region Piping Specific Imports

        /// <summary>
        /// Imports the <see cref="RingtoetsPipingSurfaceLine"/> data for the <see cref="PipingFailureMechanism"/>
        /// of the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <remarks>This will import 4 surface lines.</remarks>
        public static void ImportPipingSurfaceLines(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataImportHelper).Assembly,
                                                                                   true,
                                                                                   "DR6_surfacelines.csv",
                                                                                   "DR6_surfacelines.krp.csv"))
            {
                var filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_surfacelines.csv");
                var activity = new FileImportActivity(new PipingSurfaceLinesCsvImporter(assessmentSection.PipingFailureMechanism.SurfaceLines, assessmentSection.ReferenceLine, filePath),
                                                      "PipingSurfaceLinesCsvImporter");
                activity.Run();
                activity.Finish();
            }
        }

        /// <summary>
        /// Imports the <see cref="StochasticSoilModel"/> data for the <see cref="PipingFailureMechanism"/>
        /// of the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <remarks>This will import 4 soil models with one profile each.</remarks>
        public static void ImportPipingStochasticSoilModels(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataImportHelper).Assembly,
                                                                                   true,
                                                                                   "DR6.soil"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6.soil");
                var activity = new FileImportActivity(new PipingSoilProfilesImporter(assessmentSection.PipingFailureMechanism.StochasticSoilModels, filePath),
                                                      "PipingSoilProfilesImporter");
                activity.Run();
                activity.Finish();
            }
        }

        #endregion

    }
}