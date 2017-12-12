﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.ComponentModel;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.Structures;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;

namespace Ringtoets.ClosingStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-ring calculations for closing structures.
    /// </summary>
    public class ClosingStructuresCalculationService : StructuresCalculationServiceBase<ClosingStructuresValidationRulesRegistry,
        ClosingStructuresInput, ClosingStructure, GeneralClosingStructuresInput, StructuresClosureCalculationInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationService"/>.
        /// </summary>
        public ClosingStructuresCalculationService() : base(new ClosingStructuresCalculationMessageProvider()) {}

        protected override StructuresClosureCalculationInput CreateInput(ClosingStructuresInput structureInput,
                                                                         GeneralClosingStructuresInput generalInput,
                                                                         string hydraulicBoundaryDatabaseFilePath,
                                                                         bool usePreprocessor)
        {
            StructuresClosureCalculationInput input;
            ClosingStructureInflowModelType closingStructureInflowModelType = structureInput.InflowModelType;
            if (!Enum.IsDefined(typeof(ClosingStructureInflowModelType), closingStructureInflowModelType))
            {
                throw new InvalidEnumArgumentException(nameof(structureInput),
                                                       (int) closingStructureInflowModelType,
                                                       typeof(ClosingStructureInflowModelType));
            }

            switch (closingStructureInflowModelType)
            {
                case ClosingStructureInflowModelType.VerticalWall:
                    input = CreateClosureVerticalWallCalculationInput(structureInput, generalInput);
                    break;
                case ClosingStructureInflowModelType.LowSill:
                    input = CreateLowSillCalculationInput(structureInput, generalInput);
                    break;
                case ClosingStructureInflowModelType.FloodedCulvert:
                    input = CreateFloodedCulvertCalculationInput(structureInput, generalInput);
                    break;
                default:
                    throw new NotSupportedException($"The enum value {nameof(ClosingStructureInflowModelType)}.{closingStructureInflowModelType} is not supported.");
            }

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(input, hydraulicBoundaryDatabaseFilePath, usePreprocessor);
            return input;
        }

        private static StructuresClosureVerticalWallCalculationInput CreateClosureVerticalWallCalculationInput(
            ClosingStructuresInput structureInput,
            GeneralClosingStructuresInput generalInput)
        {
            return new StructuresClosureVerticalWallCalculationInput(
                structureInput.HydraulicBoundaryLocation.Id,
                structureInput.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(structureInput),
                HydraRingInputParser.ParseBreakWater(structureInput),
                generalInput.GravitationalAcceleration,
                structureInput.FactorStormDurationOpenStructure,
                structureInput.FailureProbabilityOpenStructure,
                structureInput.FailureProbabilityReparation,
                structureInput.IdenticalApertures,
                structureInput.AllowedLevelIncreaseStorage.Mean, structureInput.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                structureInput.StorageStructureArea.Mean, structureInput.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                structureInput.FlowWidthAtBottomProtection.Mean, structureInput.FlowWidthAtBottomProtection.StandardDeviation,
                structureInput.CriticalOvertoppingDischarge.Mean, structureInput.CriticalOvertoppingDischarge.CoefficientOfVariation,
                structureInput.FailureProbabilityStructureWithErosion,
                structureInput.StormDuration.Mean, structureInput.StormDuration.CoefficientOfVariation,
                structureInput.ProbabilityOrFrequencyOpenStructureBeforeFlooding,
                generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                structureInput.StructureNormalOrientation,
                structureInput.ModelFactorSuperCriticalFlow.Mean, structureInput.ModelFactorSuperCriticalFlow.StandardDeviation,
                structureInput.LevelCrestStructureNotClosing.Mean, structureInput.LevelCrestStructureNotClosing.StandardDeviation,
                structureInput.WidthFlowApertures.Mean, structureInput.WidthFlowApertures.StandardDeviation,
                structureInput.DeviationWaveDirection);
        }

        private static StructuresClosureLowSillCalculationInput CreateLowSillCalculationInput(
            ClosingStructuresInput structureInput,
            GeneralClosingStructuresInput generalInput)
        {
            return new StructuresClosureLowSillCalculationInput(
                structureInput.HydraulicBoundaryLocation.Id,
                structureInput.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(structureInput),
                HydraRingInputParser.ParseBreakWater(structureInput),
                generalInput.GravitationalAcceleration,
                structureInput.FactorStormDurationOpenStructure,
                structureInput.FailureProbabilityOpenStructure,
                structureInput.FailureProbabilityReparation,
                structureInput.IdenticalApertures,
                structureInput.AllowedLevelIncreaseStorage.Mean, structureInput.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                structureInput.StorageStructureArea.Mean, structureInput.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                structureInput.FlowWidthAtBottomProtection.Mean, structureInput.FlowWidthAtBottomProtection.StandardDeviation,
                structureInput.CriticalOvertoppingDischarge.Mean, structureInput.CriticalOvertoppingDischarge.CoefficientOfVariation,
                structureInput.FailureProbabilityStructureWithErosion,
                structureInput.StormDuration.Mean, structureInput.StormDuration.CoefficientOfVariation,
                structureInput.ProbabilityOrFrequencyOpenStructureBeforeFlooding,
                structureInput.ModelFactorSuperCriticalFlow.Mean, structureInput.ModelFactorSuperCriticalFlow.StandardDeviation,
                generalInput.ModelFactorSubCriticalFlow.Mean, generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                structureInput.ThresholdHeightOpenWeir.Mean, structureInput.ThresholdHeightOpenWeir.StandardDeviation,
                structureInput.InsideWaterLevel.Mean, structureInput.InsideWaterLevel.StandardDeviation,
                structureInput.WidthFlowApertures.Mean, structureInput.WidthFlowApertures.StandardDeviation);
        }

        private static StructuresClosureFloodedCulvertCalculationInput CreateFloodedCulvertCalculationInput(
            ClosingStructuresInput structureInput,
            GeneralClosingStructuresInput generalInput)
        {
            return new StructuresClosureFloodedCulvertCalculationInput(
                structureInput.HydraulicBoundaryLocation.Id,
                structureInput.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(structureInput),
                HydraRingInputParser.ParseBreakWater(structureInput),
                generalInput.GravitationalAcceleration,
                structureInput.FactorStormDurationOpenStructure,
                structureInput.FailureProbabilityOpenStructure,
                structureInput.FailureProbabilityReparation,
                structureInput.IdenticalApertures,
                structureInput.AllowedLevelIncreaseStorage.Mean, structureInput.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                structureInput.StorageStructureArea.Mean, structureInput.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                structureInput.FlowWidthAtBottomProtection.Mean, structureInput.FlowWidthAtBottomProtection.StandardDeviation,
                structureInput.CriticalOvertoppingDischarge.Mean, structureInput.CriticalOvertoppingDischarge.CoefficientOfVariation,
                structureInput.FailureProbabilityStructureWithErosion,
                structureInput.StormDuration.Mean, structureInput.StormDuration.CoefficientOfVariation,
                structureInput.ProbabilityOrFrequencyOpenStructureBeforeFlooding,
                structureInput.DrainCoefficient.Mean, structureInput.DrainCoefficient.StandardDeviation,
                structureInput.AreaFlowApertures.Mean, structureInput.AreaFlowApertures.StandardDeviation,
                structureInput.InsideWaterLevel.Mean, structureInput.InsideWaterLevel.StandardDeviation);
        }
    }
}