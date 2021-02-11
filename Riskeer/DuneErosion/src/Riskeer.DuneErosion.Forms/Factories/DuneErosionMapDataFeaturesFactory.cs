﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Util;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.Properties;
using Riskeer.DuneErosion.Forms.Views;
using RiskeerDuneErosionDataResources = Riskeer.DuneErosion.Data.Properties.Resources;
using RiskeerCommonUtilResources = Riskeer.Common.Util.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> for the <see cref="DuneErosionFailureMechanism"/> 
    /// to use in <see cref="FeatureBasedMapData"/> (created via <see cref="RiskeerMapDataFactory"/>).
    /// </summary>
    internal static class DuneErosionMapDataFeaturesFactory
    {
        /// <summary>
        /// Create dune location features based on the provided <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/> to create the location features for.</param>
        /// <returns>A collection of features or an empty collection when <see cref="DuneErosionFailureMechanism"/> does not contain
        /// any dune locations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateDuneLocationFeatures(DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(failureMechanism)
                                                .Select(CreateDuneLocationFeature)
                                                .ToArray();
        }

        private static MapFeature CreateDuneLocationFeature(AggregatedDuneLocation location)
        {
            MapFeature feature = RiskeerMapDataFeaturesFactoryHelper.CreateSinglePointMapFeature(location.Location);
            feature.MetaData[RiskeerCommonUtilResources.MetaData_ID] = location.Id;
            feature.MetaData[RiskeerCommonUtilResources.MetaData_Name] = location.Name;
            feature.MetaData[Resources.MetaData_CoastalAreaId] = location.CoastalAreaId;
            feature.MetaData[Resources.MetaData_Offset] = location.Offset.ToString(RiskeerDuneErosionDataResources.DuneLocation_Offset_format,
                                                                                   CultureInfo.CurrentCulture);
            feature.MetaData[Resources.MetaData_D50] = location.D50.ToString();

            feature.MetaData[Resources.MetaData_WaterLevelForMechanismSpecificFactorizedSignalingNorm] = location.WaterLevelForMechanismSpecificFactorizedSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WaterLevelForMechanismSpecificSignalingNorm] = location.WaterLevelForMechanismSpecificSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WaterLevelForMechanismSpecificLowerLimitNorm] = location.WaterLevelForMechanismSpecificLowerLimitNorm.ToString();
            feature.MetaData[Resources.MetaData_WaterLevelForLowerLimitNorm] = location.WaterLevelForLowerLimitNorm.ToString();
            feature.MetaData[Resources.MetaData_WaterLevelForFactorizedLowerLimitNorm] = location.WaterLevelForFactorizedLowerLimitNorm.ToString();

            feature.MetaData[Resources.MetaData_WaveHeightForMechanismSpecificFactorizedSignalingNorm] = location.WaveHeightForMechanismSpecificFactorizedSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WaveHeightForMechanismSpecificSignalingNorm] = location.WaveHeightForMechanismSpecificSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WaveHeightForMechanismSpecificLowerLimitNorm] = location.WaveHeightForMechanismSpecificLowerLimitNorm.ToString();
            feature.MetaData[Resources.MetaData_WaveHeightForLowerLimitNorm] = location.WaveHeightForLowerLimitNorm.ToString();
            feature.MetaData[Resources.MetaData_WaveHeightForFactorizedLowerLimitNorm] = location.WaveHeightForFactorizedLowerLimitNorm.ToString();

            feature.MetaData[Resources.MetaData_WavePeriodForMechanismSpecificFactorizedSignalingNorm] = location.WavePeriodForMechanismSpecificFactorizedSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WavePeriodForMechanismSpecificSignalingNorm] = location.WavePeriodForMechanismSpecificSignalingNorm.ToString();
            feature.MetaData[Resources.MetaData_WavePeriodForMechanismSpecificLowerLimitNorm] = location.WavePeriodForMechanismSpecificLowerLimitNorm.ToString();
            feature.MetaData[Resources.MetaData_WavePeriodForLowerLimitNorm] = location.WavePeriodForLowerLimitNorm.ToString();
            feature.MetaData[Resources.MetaData_WavePeriodForFactorizedLowerLimitNorm] = location.WavePeriodForFactorizedLowerLimitNorm.ToString();

            return feature;
        }
    }
}