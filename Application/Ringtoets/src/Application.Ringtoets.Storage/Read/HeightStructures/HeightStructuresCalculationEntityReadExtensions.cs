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

using System;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;

namespace Application.Ringtoets.Storage.Read.HeightStructures
{
    internal static class HeightStructuresCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="HeightStructuresCalculationEntity"/> and use the
        /// information to update a <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructuresCalculationEntity"/>
        /// to create <see cref="StructuresCalculation{T}"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StructuresCalculation{T}"/>.</returns>
        internal static StructuresCalculation<HeightStructuresInput> Read(this HeightStructuresCalculationEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = entity.Name,
                Comments = entity.Comments
            };
            ReadInputParameters(calculation.InputParameters, entity, collector);

            return calculation;
        }

        private static void ReadInputParameters(HeightStructuresInput inputParameters, HeightStructuresCalculationEntity entity, ReadConversionCollector collector)
        {
            if (entity.ForeshoreProfileEntity != null)
            {
                inputParameters.ForeshoreProfile = entity.ForeshoreProfileEntity.Read(collector);
            }
            if (entity.HeightStructureEntity != null)
            {
                inputParameters.Structure = entity.HeightStructureEntity.Read(collector);
            }
            if (entity.HydraulicLocationEntity != null)
            {
                inputParameters.HydraulicBoundaryLocation = entity.HydraulicLocationEntity.Read(collector);
            }

            inputParameters.StructureNormalOrientation = (RoundedDouble) entity.StructureNormalOrientation.ToNullAsNaN();
            inputParameters.ModelFactorSuperCriticalFlow.Mean = (RoundedDouble) entity.ModelFactorSuperCriticalFlowMean.ToNullAsNaN();
            inputParameters.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) entity.AllowedLevelIncreaseStorageMean.ToNullAsNaN();
            inputParameters.AllowedLevelIncreaseStorage.StandardDeviation = (RoundedDouble) entity.AllowedLevelIncreaseStorageStandardDeviation.ToNullAsNaN();
            inputParameters.StorageStructureArea.Mean = (RoundedDouble) entity.StorageStructureAreaMean.ToNullAsNaN();
            inputParameters.StorageStructureArea.CoefficientOfVariation = (RoundedDouble) entity.StorageStructureAreaCoefficientOfVariation.ToNullAsNaN();
            inputParameters.FlowWidthAtBottomProtection.Mean = (RoundedDouble) entity.FlowWidthAtBottomProtectionMean.ToNullAsNaN();
            inputParameters.FlowWidthAtBottomProtection.StandardDeviation = (RoundedDouble) entity.FlowWidthAtBottomProtectionStandardDeviation.ToNullAsNaN();
            inputParameters.CriticalOvertoppingDischarge.Mean = (RoundedDouble) entity.CriticalOvertoppingDischargeMean.ToNullAsNaN();
            inputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation = (RoundedDouble) entity.CriticalOvertoppingDischargeCoefficientOfVariation.ToNullAsNaN();
            inputParameters.FailureProbabilityStructureWithErosion = entity.FailureProbabilityStructureWithErosion;
            inputParameters.WidthFlowApertures.Mean = (RoundedDouble) entity.WidthFlowAperturesMean.ToNullAsNaN();
            inputParameters.WidthFlowApertures.CoefficientOfVariation = (RoundedDouble) entity.WidthFlowAperturesCoefficientOfVariation.ToNullAsNaN();

            inputParameters.UseBreakWater = Convert.ToBoolean(entity.UseBreakWater);
            inputParameters.UseForeshore = Convert.ToBoolean(entity.UseForeshore);
        }
    }
}