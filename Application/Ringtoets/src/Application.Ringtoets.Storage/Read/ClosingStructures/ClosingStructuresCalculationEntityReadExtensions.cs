﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;

namespace Application.Ringtoets.Storage.Read.ClosingStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="StructuresCalculation{T}"/>
    /// based on the <see cref="ClosingStructuresCalculationEntity"/>.
    /// </summary>
    internal static class ClosingStructuresCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="ClosingStructuresCalculationEntity"/> and use the
        /// information to update a <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructuresCalculationEntity"/>
        /// to create <see cref="StructuresCalculation{T}"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StructuresCalculation{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static StructuresCalculation<ClosingStructuresInput> Read(this ClosingStructuresCalculationEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = entity.Name,
                Comments = entity.Comments
            };
            ReadInputParameters(calculation.InputParameters, entity, collector);

            return calculation;
        }

        private static void ReadInputParameters(ClosingStructuresInput inputParameters, ClosingStructuresCalculationEntity entity, ReadConversionCollector collector)
        {
            ReadBaseStructuresInputParameters(inputParameters, entity, collector);
            ReadClosingStructuresSpecificInputParameters(inputParameters, entity);
        }

        private static void ReadBaseStructuresInputParameters(StructuresInputBase<ClosingStructure> structuresInputBase,
                                                              ClosingStructuresCalculationEntity entity,
                                                              ReadConversionCollector collector)
        {
            if (entity.ForeshoreProfileEntity != null)
            {
                structuresInputBase.ForeshoreProfile = entity.ForeshoreProfileEntity.Read(collector);
            }
            if (entity.ClosingStructureEntity != null)
            {
                structuresInputBase.Structure = entity.ClosingStructureEntity.Read(collector);
            }
            if (entity.HydraulicLocationEntity != null)
            {
                structuresInputBase.HydraulicBoundaryLocation = entity.HydraulicLocationEntity.Read(collector);
            }

            structuresInputBase.StructureNormalOrientation = (RoundedDouble) entity.StructureNormalOrientation.ToNullAsNaN();
            structuresInputBase.ModelFactorSuperCriticalFlow.Mean = (RoundedDouble) entity.ModelFactorSuperCriticalFlowMean.ToNullAsNaN();
            structuresInputBase.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) entity.AllowedLevelIncreaseStorageMean.ToNullAsNaN();
            structuresInputBase.AllowedLevelIncreaseStorage.StandardDeviation = (RoundedDouble) entity.AllowedLevelIncreaseStorageStandardDeviation.ToNullAsNaN();
            structuresInputBase.StorageStructureArea.Mean = (RoundedDouble) entity.StorageStructureAreaMean.ToNullAsNaN();
            structuresInputBase.StorageStructureArea.CoefficientOfVariation = (RoundedDouble) entity.StorageStructureAreaCoefficientOfVariation.ToNullAsNaN();
            structuresInputBase.FlowWidthAtBottomProtection.Mean = (RoundedDouble) entity.FlowWidthAtBottomProtectionMean.ToNullAsNaN();
            structuresInputBase.FlowWidthAtBottomProtection.StandardDeviation = (RoundedDouble) entity.FlowWidthAtBottomProtectionStandardDeviation.ToNullAsNaN();
            structuresInputBase.CriticalOvertoppingDischarge.Mean = (RoundedDouble) entity.CriticalOvertoppingDischargeMean.ToNullAsNaN();
            structuresInputBase.CriticalOvertoppingDischarge.CoefficientOfVariation = (RoundedDouble) entity.CriticalOvertoppingDischargeCoefficientOfVariation.ToNullAsNaN();
            structuresInputBase.FailureProbabilityStructureWithErosion = entity.FailureProbabilityStructureWithErosion;
            structuresInputBase.WidthFlowApertures.Mean = (RoundedDouble) entity.WidthFlowAperturesMean.ToNullAsNaN();
            structuresInputBase.WidthFlowApertures.CoefficientOfVariation = (RoundedDouble) entity.WidthFlowAperturesCoefficientOfVariation.ToNullAsNaN();
            structuresInputBase.StormDuration.Mean = (RoundedDouble) entity.StormDurationMean.ToNullAsNaN();

            structuresInputBase.UseBreakWater = Convert.ToBoolean(entity.UseBreakWater);
            structuresInputBase.BreakWater.Type = (BreakWaterType)entity.BreakWaterType;
            structuresInputBase.BreakWater.Height = (RoundedDouble)entity.BreakWaterHeight.ToNullAsNaN();
            structuresInputBase.UseForeshore = Convert.ToBoolean(entity.UseForeshore);
        }

        private static void ReadClosingStructuresSpecificInputParameters(ClosingStructuresInput inputParameters, ClosingStructuresCalculationEntity entity)
        {
            inputParameters.InflowModelType = (ClosingStructureInflowModelType) entity.InflowModelType;
            inputParameters.InsideWaterLevel.Mean = (RoundedDouble) entity.InsideWaterLevelMean.ToNullAsNaN();
            inputParameters.InsideWaterLevel.StandardDeviation = (RoundedDouble) entity.InsideWaterLevelStandardDeviation.ToNullAsNaN();
            inputParameters.DeviationWaveDirection = (RoundedDouble) entity.DeviationWaveDirection.ToNullAsNaN();
            inputParameters.DrainCoefficient.Mean = (RoundedDouble) entity.DrainCoefficientMean.ToNullAsNaN();
            inputParameters.FactorStormDurationOpenStructure = (RoundedDouble) entity.FactorStormDurationOpenStructure.ToNullAsNaN();
            inputParameters.ThresholdHeightOpenWeir.Mean = (RoundedDouble) entity.ThresholdHeightOpenWeirMean.ToNullAsNaN();
            inputParameters.ThresholdHeightOpenWeir.StandardDeviation = (RoundedDouble) entity.ThresholdHeightOpenWeirStandardDeviation.ToNullAsNaN();
            inputParameters.AreaFlowApertures.Mean = (RoundedDouble) entity.AreaFlowAperturesMean.ToNullAsNaN();
            inputParameters.AreaFlowApertures.StandardDeviation = (RoundedDouble) entity.AreaFlowAperturesStandardDeviation.ToNullAsNaN();
            inputParameters.FailureProbabilityOpenStructure = entity.FailureProbabilityOpenStructure;
            inputParameters.FailureProbabilityReparation = entity.FailureProbablityReparation;
            inputParameters.IdenticalApertures = entity.IdenticalApertures;
            inputParameters.LevelCrestStructureNotClosing.Mean = (RoundedDouble) entity.LevelCrestStructureNotClosingMean.ToNullAsNaN();
            inputParameters.LevelCrestStructureNotClosing.StandardDeviation = (RoundedDouble) entity.LevelCrestStructureNotClosingStandardDeviation.ToNullAsNaN();
            inputParameters.ProbabilityOpenStructureBeforeFlooding = entity.ProbabilityOpenStructureBeforeFlooding;
        }
    }
}