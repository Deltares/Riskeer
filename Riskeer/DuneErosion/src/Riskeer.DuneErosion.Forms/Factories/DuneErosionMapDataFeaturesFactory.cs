// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
        /// Create dune location features based on the provided <paramref name="locations"/>.
        /// </summary>
        /// <param name="locations">The collection of <see cref="AggregatedDuneLocation"/> to create the location features for.</param>
        /// <returns>A collection of features.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locations"/> is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateDuneLocationFeatures(IEnumerable<AggregatedDuneLocation> locations)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            return locations.Select(CreateDuneLocationFeature).ToArray();
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

            HydraulicBoundaryLocationMapDataFeaturesFactory.AddTargetProbabilityMetaData(feature, location.WaterLevelCalculationsForTargetProbabilities,
                                                                                         Resources.MetaData_WaterLevel_TargetProbability_0);
            HydraulicBoundaryLocationMapDataFeaturesFactory.AddTargetProbabilityMetaData(feature, location.WaveHeightCalculationsForTargetProbabilities,
                                                                                         Resources.MetaData_WaveHeight_TargetProbability_0);
            HydraulicBoundaryLocationMapDataFeaturesFactory.AddTargetProbabilityMetaData(feature, location.WavePeriodCalculationsForTargetProbabilities,
                                                                                         Resources.MetaData_WavePeriod_TargetProbability_0);

            return feature;
        }
    }
}