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
using Core.Common.Base.Data;
using Core.Components.Gis.Features;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Util;
using Riskeer.Common.Util.Helpers;
using RiskeerCommonUtilResources = Riskeer.Common.Util.Properties.Resources;

namespace Riskeer.Common.Forms.Factories
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

            MapFeature feature = RiskeerMapDataFeaturesFactoryHelper.CreateSinglePointMapFeature(location.Location);
            feature.MetaData[RiskeerCommonUtilResources.MetaData_ID] = location.Id;
            feature.MetaData[RiskeerCommonUtilResources.MetaData_Name] = location.Name;

            AddTargetProbabilityMetaData(feature, location.WaterLevelCalculationForTargetProbabilities,
                                         Resources.MetaData_WaterLevel_TargetProbability_0);

            AddTargetProbabilityMetaData(feature, location.WaveHeightCalculationForTargetProbabilities,
                                         Resources.MetaData_WaveHeight_TargetProbability_0);

            return feature;
        }

        private static void AddTargetProbabilityMetaData(MapFeature feature, IEnumerable<Tuple<double, RoundedDouble>> targetProbabilities, string displayName)
        {
            var addedMetaDataItems = new List<string>();

            foreach (Tuple<double, RoundedDouble> calculationOutputForTargetProbability in targetProbabilities)
            {
                string uniqueName = NamingHelper.GetUniqueName(
                    addedMetaDataItems, string.Format(displayName, ProbabilityFormattingHelper.Format(calculationOutputForTargetProbability.Item1)),
                    v => v);

                feature.MetaData[uniqueName] = calculationOutputForTargetProbability.Item2.ToString();
                addedMetaDataItems.Add(uniqueName);
            }
        }
    }
}