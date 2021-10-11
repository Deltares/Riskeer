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

            AddTargetProbabilityMetaData(feature, location.WaterLevelCalculationsForTargetProbabilities);

            AddTargetProbabilityMetaData(feature, location.WaveHeightCalculationsForTargetProbabilities);

            return feature;
        }

        /// <summary>
        /// Adds target probability related meta data to the given <paramref name="feature"/>.
        /// </summary>
        /// <param name="feature">The feature to add the meta data to.</param>
        /// <param name="targetProbabilities">The collection of target probabilities to add.</param>
        /// <param name="displayNameFormat">The display name format of the meta data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when <paramref name="displayNameFormat"/> is invalid;
        /// or the index of a format item is not zero.</exception>
        public static void AddTargetProbabilityMetaData(MapFeature feature, IEnumerable<Tuple<double, RoundedDouble>> targetProbabilities, string displayNameFormat)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (targetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(targetProbabilities));
            }

            if (displayNameFormat == null)
            {
                throw new ArgumentNullException(nameof(displayNameFormat));
            }

            var addedMetaDataItems = new List<string>();

            foreach (Tuple<double, RoundedDouble> calculationOutputForTargetProbability in targetProbabilities)
            {
                string uniqueName = NamingHelper.GetUniqueName(
                    addedMetaDataItems, string.Format(displayNameFormat, ProbabilityFormattingHelper.Format(calculationOutputForTargetProbability.Item1)),
                    v => v);

                feature.MetaData[uniqueName] = calculationOutputForTargetProbability.Item2.ToString();
                addedMetaDataItems.Add(uniqueName);
            }
        }

        /// <summary>
        /// Adds target probability related meta data to the given <paramref name="feature"/>.
        /// </summary>
        /// <param name="feature">The feature to add the meta data to.</param>
        /// <param name="targetProbabilities">The collection of target probabilities to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void AddTargetProbabilityMetaData(MapFeature feature, IEnumerable<Tuple<string, RoundedDouble>> targetProbabilities)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (targetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(targetProbabilities));
            }

            foreach (Tuple<string, RoundedDouble> calculationOutputForTargetProbability in targetProbabilities)
            {
                string uniqueName = calculationOutputForTargetProbability.Item1;

                feature.MetaData[uniqueName] = calculationOutputForTargetProbability.Item2.ToString();
            }
        }
    }
}