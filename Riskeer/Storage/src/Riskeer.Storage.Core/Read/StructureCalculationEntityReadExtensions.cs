// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for the common code
    /// among all <see cref="StructuresCalculation{T}"/> instances based on the <see cref="IStructuresCalculationEntity"/>.
    /// </summary>
    internal static class StructureCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads all the information from the entity and updated the given input object,
        /// with the exception of the <see cref="StructuresInputBase{T}.Structure"/> property.
        /// </summary>
        /// <typeparam name="T">The type of structure residing in <paramref name="inputToUpdate"/>.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="inputToUpdate">The input object to update.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputToUpdate"/>
        /// or <paramref name="collector"/> is <c>null</c>.</exception>
        internal static void Read<T>(this IStructuresCalculationEntity entity,
                                     StructuresInputBase<T> inputToUpdate,
                                     ReadConversionCollector collector) where T : StructureBase
        {
            if (inputToUpdate == null)
            {
                throw new ArgumentNullException(nameof(inputToUpdate));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (entity.ForeshoreProfileEntity != null)
            {
                inputToUpdate.ForeshoreProfile = entity.ForeshoreProfileEntity.Read(collector);
            }

            if (entity.HydraulicLocationEntity != null)
            {
                inputToUpdate.HydraulicBoundaryLocation = entity.HydraulicLocationEntity.Read(collector);
            }

            inputToUpdate.StructureNormalOrientation = (RoundedDouble) entity.StructureNormalOrientation.ToNullAsNaN();
            inputToUpdate.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) entity.AllowedLevelIncreaseStorageMean.ToNullAsNaN();
            inputToUpdate.AllowedLevelIncreaseStorage.StandardDeviation = (RoundedDouble) entity.AllowedLevelIncreaseStorageStandardDeviation.ToNullAsNaN();
            inputToUpdate.StorageStructureArea.Mean = (RoundedDouble) entity.StorageStructureAreaMean.ToNullAsNaN();
            inputToUpdate.StorageStructureArea.CoefficientOfVariation = (RoundedDouble) entity.StorageStructureAreaCoefficientOfVariation.ToNullAsNaN();
            inputToUpdate.FlowWidthAtBottomProtection.Mean = (RoundedDouble) entity.FlowWidthAtBottomProtectionMean.ToNullAsNaN();
            inputToUpdate.FlowWidthAtBottomProtection.StandardDeviation = (RoundedDouble) entity.FlowWidthAtBottomProtectionStandardDeviation.ToNullAsNaN();
            inputToUpdate.CriticalOvertoppingDischarge.Mean = (RoundedDouble) entity.CriticalOvertoppingDischargeMean.ToNullAsNaN();
            inputToUpdate.CriticalOvertoppingDischarge.CoefficientOfVariation = (RoundedDouble) entity.CriticalOvertoppingDischargeCoefficientOfVariation.ToNullAsNaN();
            inputToUpdate.FailureProbabilityStructureWithErosion = entity.FailureProbabilityStructureWithErosion;
            inputToUpdate.WidthFlowApertures.Mean = (RoundedDouble) entity.WidthFlowAperturesMean.ToNullAsNaN();
            inputToUpdate.WidthFlowApertures.StandardDeviation = (RoundedDouble) entity.WidthFlowAperturesStandardDeviation.ToNullAsNaN();
            inputToUpdate.StormDuration.Mean = (RoundedDouble) entity.StormDurationMean.ToNullAsNaN();

            inputToUpdate.UseBreakWater = Convert.ToBoolean(entity.UseBreakWater);
            inputToUpdate.BreakWater.Type = (BreakWaterType) entity.BreakWaterType;
            inputToUpdate.BreakWater.Height = (RoundedDouble) entity.BreakWaterHeight.ToNullAsNaN();
            inputToUpdate.UseForeshore = Convert.ToBoolean(entity.UseForeshore);

            inputToUpdate.ShouldIllustrationPointsBeCalculated = Convert.ToBoolean(entity.ShouldIllustrationPointsBeCalculated);
        }
    }
}