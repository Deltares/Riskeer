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

using Ringtoets.Common.Service;
using Ringtoets.Common.Service.Structures;
using Ringtoets.HeightStructures.Data;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;

namespace Ringtoets.HeightStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for height structures.
    /// </summary>
    public class HeightStructuresCalculationService : StructuresCalculationServiceBase<HeightStructuresValidationRulesRegistry, HeightStructuresInput,
        HeightStructure, GeneralHeightStructuresInput, StructuresOvertoppingCalculationInput>

    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculationService"/>.
        /// </summary>
        public HeightStructuresCalculationService() : base(new HeightStructuresCalculationMessageProvider()) {}

        protected override StructuresOvertoppingCalculationInput CreateInput(HeightStructuresInput structureInput,
                                                                             GeneralHeightStructuresInput generalInput,
                                                                             string hydraulicBoundaryDatabaseFilePath,
                                                                             bool usePreprocessor)
        {
            var structuresOvertoppingCalculationInput = new StructuresOvertoppingCalculationInput(
                structureInput.HydraulicBoundaryLocation.Id,
                structureInput.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(structureInput),
                HydraRingInputParser.ParseBreakWater(structureInput),
                generalInput.GravitationalAcceleration,
                generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                structureInput.LevelCrestStructure.Mean, structureInput.LevelCrestStructure.StandardDeviation,
                structureInput.StructureNormalOrientation,
                structureInput.ModelFactorSuperCriticalFlow.Mean, structureInput.ModelFactorSuperCriticalFlow.StandardDeviation,
                structureInput.AllowedLevelIncreaseStorage.Mean, structureInput.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                structureInput.StorageStructureArea.Mean, structureInput.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                structureInput.FlowWidthAtBottomProtection.Mean, structureInput.FlowWidthAtBottomProtection.StandardDeviation,
                structureInput.CriticalOvertoppingDischarge.Mean, structureInput.CriticalOvertoppingDischarge.CoefficientOfVariation,
                structureInput.FailureProbabilityStructureWithErosion,
                structureInput.WidthFlowApertures.Mean, structureInput.WidthFlowApertures.StandardDeviation,
                structureInput.DeviationWaveDirection,
                structureInput.StormDuration.Mean, structureInput.StormDuration.CoefficientOfVariation);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(structuresOvertoppingCalculationInput, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            return structuresOvertoppingCalculationInput;
        }
    }
}