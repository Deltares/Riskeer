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
using Core.Components.Gis.Features;
using Ringtoets.Common.Util.Properties;

namespace Ringtoets.Common.Util
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> for
    /// <see cref="AggregatedHydraulicBoundaryLocation"/>.
    /// </summary>
    public static class HydraulicBoundaryLocationMapDataFeaturesFactory
    {
        /// <summary>
        /// Creates a hydraulic boundary location feature based on the given <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The location to create the feature for.</param>
        /// <returns>A feature based on the given <paramref name="location"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> is <c>null</c>.</exception>
        public static MapFeature CreateHydraulicBoundaryLocationFeature(AggregatedHydraulicBoundaryLocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            MapFeature feature = RingtoetsMapDataFeaturesFactoryHelper.CreateSinglePointMapFeature(location.Location);
            feature.MetaData[Resources.MetaData_ID] = location.Id;
            feature.MetaData[Resources.MetaData_Name] = location.Name;
            feature.MetaData[Resources.MetaData_WaterLevelCalculationForFactorizedSignalingNorm] = location.WaterLevelCalculationForFactorizedSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WaterLevelCalculationForSignalingNorm] = location.WaterLevelCalculationForSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WaterLevelCalculationForLowerLimitNorm] = location.WaterLevelCalculationForLowerLimitNorm.ToString();
            feature.MetaData[Resources.MetaData_WaterLevelCalculationForFactorizedLowerLimitNorm] = location.WaterLevelCalculationForFactorizedLowerLimitNorm.ToString();
            feature.MetaData[Resources.MetaData_WaveHeightCalculationForFactorizedSignalingNorm] = location.WaveHeightCalculationForFactorizedSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WaveHeightCalculationForSignalingNorm] = location.WaveHeightCalculationForSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WaveHeightCalculationForLowerLimitNorm] = location.WaveHeightCalculationForLowerLimitNorm.ToString();
            feature.MetaData[Resources.MetaData_WaveHeightCalculationForFactorizedLowerLimitNorm] = location.WaveHeightCalculationForFactorizedLowerLimitNorm.ToString();
            return feature;
        }
    }
}