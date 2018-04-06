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
using Core.Components.Gis.Features;
using Ringtoets.Common.Util;
using RingtoetsCommonUtilResources = Ringtoets.Common.Util.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Util
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> for
    /// <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/>.
    /// </summary>
    public static class GrassCoverErosionOutwardsHydraulicBoundaryLocationMapDataFeaturesFactory
    {
        /// <summary>
        /// Creates a hydraulic boundary location feature based on the given <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The location to create the feature for.</param>
        /// <param name="nameProvider">The provider of the labels.</param>
        /// <returns>A feature based on the given <paramref name="location"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when all parameters are <c>null</c>.</exception>
        public static MapFeature CreateHydraulicBoundaryLocationFeature(GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation location,
                                                                        IGrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProvider nameProvider)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (nameProvider == null)
            {
                throw new ArgumentNullException(nameof(nameProvider));
            }

            MapFeature feature = RingtoetsMapDataFeaturesFactoryHelper.CreateSinglePointMapFeature(location.Location);
            feature.MetaData[RingtoetsCommonUtilResources.MetaData_ID] = location.Id;
            feature.MetaData[RingtoetsCommonUtilResources.MetaData_Name] = location.Name;
            feature.MetaData[nameProvider.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName] = location.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm.Value;
            feature.MetaData[nameProvider.WaterLevelCalculationForMechanismSpecificSignalingNormAttributeName] = location.WaterLevelCalculationForMechanismSpecificSignalingNorm.Value;
            feature.MetaData[nameProvider.WaterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName] = location.WaterLevelCalculationForMechanismSpecificLowerLimitNorm.Value;
            feature.MetaData[nameProvider.WaterLevelCalculationForLowerLimitNormAttributeName] = location.WaterLevelCalculationForLowerLimitNorm.Value;
            feature.MetaData[nameProvider.WaterLevelCalculationForFactorizedLowerLimitNormAttributeName] = location.WaterLevelCalculationForFactorizedLowerLimitNorm.Value;
            feature.MetaData[nameProvider.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName] = location.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm.Value;
            feature.MetaData[nameProvider.WaveHeightCalculationForMechanismSpecificSignalingNormAttributeName] = location.WaveHeightCalculationForMechanismSpecificSignalingNorm.Value;
            feature.MetaData[nameProvider.WaveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName] = location.WaveHeightCalculationForMechanismSpecificLowerLimitNorm.Value;
            feature.MetaData[nameProvider.WaveHeightCalculationForLowerLimitNormAttributeName] = location.WaveHeightCalculationForLowerLimitNorm.Value;
            feature.MetaData[nameProvider.WaveHeightCalculationForFactorizedLowerLimitNormAttributeName] = location.WaveHeightCalculationForFactorizedLowerLimitNorm.Value;
            return feature;
        }
    }
}