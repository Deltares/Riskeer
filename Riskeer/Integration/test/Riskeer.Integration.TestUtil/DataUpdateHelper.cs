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

using System.IO;
using Core.Common.Base.Service;
using Core.Common.Util.IO;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Plugin.FileImporter;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Plugin.FileImporter;
using Riskeer.Integration.Data;

namespace Ringtoets.Integration.TestUtil
{
    /// <summary>
    /// Helper methods related to importing data for integration tests.
    /// </summary>
    public static class DataUpdateHelper
    {
        /// <summary>
        /// Imports the <see cref="PipingStochasticSoilModel"/> data for the <see cref="PipingFailureMechanism"/>
        /// of the given <see cref="IAssessmentSection"/> and updates existing data based upon the imported
        /// data.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <remarks>When data from <see cref="DataImportHelper.ImportPipingStochasticSoilModels"/> is added first,
        /// then calling this method will remove soil model 'PK001_0004_Piping', stochastic soil profile 'W1-6_4_1D1'
        /// and update the probability of stochastic soil profile '6-3_22' (100% to 50%).</remarks>
        public static void UpdatePipingStochasticSoilModels(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataUpdateHelper).Assembly,
                                                                                   true,
                                                                                   "DR6_updated.soil"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_updated.soil");
                var activity = new FileImportActivity(
                    new StochasticSoilModelImporter<PipingStochasticSoilModel>(
                        assessmentSection.Piping.StochasticSoilModels,
                        filePath,
                        new UpdateMessageProvider(),
                        PipingStochasticSoilModelImporterConfigurationFactory.CreateUpdateStrategyConfiguration(assessmentSection.Piping)),
                    "StochasticSoilModelUpdater");
                activity.Run();
                activity.Finish();
            }
        }

        /// <summary>
        /// Imports the <see cref="MacroStabilityInwardsStochasticSoilModel"/> data for the <see cref="MacroStabilityInwards.Data.MacroStabilityInwardsFailureMechanism"/>
        /// of the given <see cref="IAssessmentSection"/> and updates existing data based upon the imported
        /// data.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <remarks>When data from <see cref="DataImportHelper.ImportMacroStabilityInwardsStochasticSoilModels"/> is added first,
        /// then calling this method will remove soil model 'PK001_0004_Piping', stochastic soil profile 'W1-6_4_1D1'
        /// and update the probability of stochastic soil profile '6-3_22' (100% to 50%).</remarks>
        public static void UpdateMacroStabilityInwardsStochasticSoilModels(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DataUpdateHelper).Assembly,
                                                                                   true,
                                                                                   "DR6_updated.soil"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_updated.soil");
                var activity = new FileImportActivity(new StochasticSoilModelImporter<MacroStabilityInwardsStochasticSoilModel>(
                                                          assessmentSection.MacroStabilityInwards.StochasticSoilModels,
                                                          filePath,
                                                          new ImportMessageProvider(),
                                                          MacroStabilityInwardsStochasticSoilModelImporterConfigurationFactory.CreateUpdateStrategyConfiguration(assessmentSection.MacroStabilityInwards)),
                                                      "MacroStabilityInwardsStochasticSoilModelUpdater");
                activity.Run();
                activity.Finish();
            }
        }
    }
}