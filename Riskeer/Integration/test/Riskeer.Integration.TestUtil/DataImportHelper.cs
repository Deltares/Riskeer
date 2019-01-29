// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.Gui.Commands;
using Core.Common.Util.IO;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.ReferenceLines;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SurfaceLines;
using Riskeer.DuneErosion.Plugin.Handlers;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Plugin.FileImporter;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Plugin.FileImporter;
using Riskeer.Piping.Primitives;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Importers;
using Riskeer.Integration.Plugin.Handlers;
using PipingSurfaceLinesCsvImporterConfigurationFactory = Ringtoets.Piping.Plugin.FileImporter.SurfaceLinesCsvImporterConfigurationFactory;
using MacroStabilityInwardsSurfaceLinesCsvImporterConfigurationFactory = Ringtoets.MacroStabilityInwards.Plugin.FileImporter.SurfaceLinesCsvImporterConfigurationFactory;

namespace Riskeer.Integration.TestUtil
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
                var mocks = new MockRepository();
                var viewCommands = mocks.Stub<IViewCommands>();
                mocks.ReplayAll();
                var activity = new FileImportActivity(new ReferenceLineImporter(assessmentSection.ReferenceLine,
                                                                                new ReferenceLineUpdateHandler(assessmentSection, viewCommands),
                                                                                filePath),
                                                      "ReferenceLineImporter");
                activity.Run();
                activity.Finish();
                mocks.VerifyAll();
            }
        }

        /// <summary>
        /// Imports the <see cref="FailureMechanismSection"/> data for a given <see cref="IFailureMechanism"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to import on.</param>
        /// <remarks>
        /// <para>This will import 283 failure mechanism sections.</para>
        /// <para>Imports using <see cref="FileImportActivity"/>.</para>
        /// </remarks>
        /// <seealso cref="ImportFailureMechanismSections(AssessmentSection, IEnumerable{IFailureMechanism})"/>
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
                var activity = new FileImportActivity(new FailureMechanismSectionsImporter(failureMechanism,
                                                                                           assessmentSection.ReferenceLine,
                                                                                           filePath,
                                                                                           new FailureMechanismSectionReplaceStrategy(failureMechanism),
                                                                                           new ImportMessageProvider()),
                                                      "FailureMechanismSectionsImporter");
                activity.Run();
                activity.Finish();
            }
        }

        /// <summary>
        /// Imports the <see cref="FailureMechanismSection"/> data for a given <see cref="IFailureMechanism"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <param name="targetFailureMechanisms">The <see cref="IFailureMechanism"/> to import on.</param>
        /// <remarks>
        /// <para>This will import the same 283 failure mechanism sections on all failure mechanisms.</para>
        /// <para>Does not import using <see cref="FileImportActivity"/>.</para>
        /// </remarks>
        /// <seealso cref="ImportFailureMechanismSections(AssessmentSection, IFailureMechanism)"/>
        public static void ImportFailureMechanismSections(AssessmentSection assessmentSection, IEnumerable<IFailureMechanism> targetFailureMechanisms)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataImportHelper).Assembly,
                                                                                   true,
                                                                                   "traject_6-3_vakken.shp",
                                                                                   "traject_6-3_vakken.dbf",
                                                                                   "traject_6-3_vakken.prj",
                                                                                   "traject_6-3_vakken.shx"))
            {
                IFailureMechanism[] failureMechanisms = targetFailureMechanisms.ToArray();
                for (var i = 0; i < failureMechanisms.Length; i++)
                {
                    IFailureMechanism failureMechanism = failureMechanisms[i];
                    if (i == 0)
                    {
                        string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath,
                                                       "traject_6-3_vakken.shp");
                        var importer = new FailureMechanismSectionsImporter(failureMechanism,
                                                                            assessmentSection.ReferenceLine,
                                                                            filePath,
                                                                            new FailureMechanismSectionReplaceStrategy(failureMechanism),
                                                                            new ImportMessageProvider());
                        importer.Import();
                    }
                    else
                    {
                        // Copy same FailureMechanismSection instances to other failure mechanisms
                        FailureMechanismTestHelper.SetSections(failureMechanism, failureMechanisms[0].Sections.Select(DeepCloneSection).ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Imports the <see cref="HydraulicBoundaryDatabase"/> for the given <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <remarks>This will import 18 Hydraulic boundary locations.</remarks>
        public static void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataImportHelper).Assembly,
                                                                                   false,
                                                                                   "HRD dutch coast south.sqlite",
                                                                                   "HLCD.sqlite",
                                                                                   "HRD dutch coast south.config.sqlite"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "HRD dutch coast south.sqlite");

                ImportHydraulicBoundaryDatabase(assessmentSection, filePath);
            }
        }

        /// <summary>
        /// Imports the <see cref="HydraulicBoundaryDatabase"/> for the given <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <param name="filePath">The filePath to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public static void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection, string filePath)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryDatabaseImporter(assessmentSection.HydraulicBoundaryDatabase,
                                                                                          new HydraulicBoundaryDatabaseUpdateHandler(
                                                                                              assessmentSection,
                                                                                              new DuneLocationsReplacementHandler(viewCommands, assessmentSection.DuneErosion)),
                                                                                          filePath);
            hydraulicBoundaryDatabaseImporter.Import();
            mocks.VerifyAll();
        }

        private static FailureMechanismSection DeepCloneSection(FailureMechanismSection section)
        {
            return new FailureMechanismSection(section.Name,
                                               section.Points.Select(p => new Point2D(p)));
        }

        #region Piping Specific Imports

        /// <summary>
        /// Imports the <see cref="PipingSurfaceLine"/> data for the <see cref="PipingFailureMechanism"/>
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
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_surfacelines.csv");
                var activity = new FileImportActivity(new SurfaceLinesCsvImporter<PipingSurfaceLine>(
                                                          assessmentSection.Piping.SurfaceLines,
                                                          filePath,
                                                          new ImportMessageProvider(),
                                                          PipingSurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(assessmentSection.Piping, assessmentSection.ReferenceLine)),
                                                      "PipingSurfaceLinesCsvImporter")
                    ;
                activity.Run();
                activity.Finish();
            }
        }

        /// <summary>
        /// Imports the <see cref="PipingStochasticSoilModel"/> data for the <see cref="PipingFailureMechanism"/>
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
                var activity = new FileImportActivity(
                    new StochasticSoilModelImporter<PipingStochasticSoilModel>(
                        assessmentSection.Piping.StochasticSoilModels,
                        filePath,
                        new ImportMessageProvider(),
                        PipingStochasticSoilModelImporterConfigurationFactory.CreateReplaceStrategyConfiguration(assessmentSection.Piping)),
                    "StochasticSoilModelImporter");
                activity.Run();
                activity.Finish();
            }
        }

        #endregion

        #region MacroStabilityInwards Specific Imports

        /// <summary>
        /// Imports the <see cref="MacroStabilityInwardsSurfaceLine"/> data for the <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// of the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <remarks>This will import 4 surface lines.</remarks>
        public static void ImportMacroStabilityInwardsSurfaceLines(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataImportHelper).Assembly,
                                                                                   true,
                                                                                   "DR6_surfacelines.csv",
                                                                                   "DR6_surfacelines.krp.csv"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_surfacelines.csv");
                var activity = new FileImportActivity(new SurfaceLinesCsvImporter<MacroStabilityInwardsSurfaceLine>(
                                                          assessmentSection.MacroStabilityInwards.SurfaceLines,
                                                          filePath,
                                                          new ImportMessageProvider(),
                                                          MacroStabilityInwardsSurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(assessmentSection.MacroStabilityInwards, assessmentSection.ReferenceLine)),
                                                      "MacroStabilityInwardsSurfaceLinesCsvImporter");
                activity.Run();
                activity.Finish();
            }
        }

        /// <summary>
        /// Imports the <see cref="MacroStabilityInwardsStochasticSoilModel"/> data for the <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// of the given <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <remarks>This will import 4 soil models with one profile each.</remarks>
        public static void ImportMacroStabilityInwardsStochasticSoilModels(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataImportHelper).Assembly,
                                                                                   true,
                                                                                   "DR6.soil"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6.soil");
                var activity = new FileImportActivity(new StochasticSoilModelImporter<MacroStabilityInwardsStochasticSoilModel>(
                                                          assessmentSection.MacroStabilityInwards.StochasticSoilModels,
                                                          filePath,
                                                          new ImportMessageProvider(),
                                                          MacroStabilityInwardsStochasticSoilModelImporterConfigurationFactory.CreateReplaceStrategyConfiguration(assessmentSection.MacroStabilityInwards)),
                                                      "MacroStabilityInwardsStochasticSoilModelImporter");
                activity.Run();
                activity.Finish();
            }
        }

        #endregion
    }
}