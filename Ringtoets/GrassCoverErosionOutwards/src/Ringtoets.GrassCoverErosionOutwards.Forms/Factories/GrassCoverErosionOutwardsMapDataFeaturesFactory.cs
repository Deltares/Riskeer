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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> to use in <see cref="FeatureBasedMapData"/>.
    /// </summary>
    internal static class GrassCoverErosionOutwardsMapDataFeaturesFactory
    {
        /// <summary>
        /// Create calculation features based on the provided <paramref name="calculationInputs"/>.
        /// </summary>
        /// <param name="calculationInputs">The collection of <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> to create the calculation features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="calculationInputs"/> is <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateCalculationFeatures(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculationInputs)
        {
            bool hasCalculations = calculationInputs != null && calculationInputs.Any();

            if (!hasCalculations)
            {
                return new MapFeature[0];
            }

            IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculationsWithLocationAndHydraulicBoundaryLocation =
                calculationInputs.Where(calculation =>
                                            calculation.InputParameters.ForeshoreProfile != null &&
                                            calculation.InputParameters.HydraulicBoundaryLocation != null);

            MapCalculationData[] calculationData =
                calculationsWithLocationAndHydraulicBoundaryLocation.Select(
                    calculation => new MapCalculationData(
                        calculation.Name,
                        calculation.InputParameters.ForeshoreProfile.WorldReferencePoint,
                        calculation.InputParameters.HydraulicBoundaryLocation)).ToArray();

            return RingtoetsMapDataFeaturesFactory.CreateCalculationFeatures(calculationData);
        }

        /// <summary>
        /// Create hydraulic boundary location features based on the provided <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The collection of <see cref="HydraulicBoundaryLocation"/>
        /// to create the location features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="hydraulicBoundaryLocations"/> is empty.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/> is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateHydraulicBoundaryLocationsFeatures(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            int hydraulicBoundaryLocationsCount = hydraulicBoundaryLocations.Count();
            var features = new MapFeature[hydraulicBoundaryLocationsCount];

            for (var i = 0; i < hydraulicBoundaryLocationsCount; i++)
            {
                HydraulicBoundaryLocation location = hydraulicBoundaryLocations.ElementAt(i);

                MapFeature feature = RingtoetsMapDataFeaturesFactory.CreateSinglePointMapFeature(location.Location);

                feature.MetaData[RingtoetsCommonFormsResources.MetaData_ID] = location.Id;
                feature.MetaData[RingtoetsCommonFormsResources.MetaData_Name] = location.Name;
                feature.MetaData[Resources.DesignWaterLevel_DisplayName] = location.DesignWaterLevelCalculation1.Output?.Result ?? RoundedDouble.NaN;
                feature.MetaData[Resources.MetaData_WaveHeight] = location.WaveHeightCalculation1.Output?.Result ?? RoundedDouble.NaN;

                features[i] = feature;
            }

            return features;
        }
    }
}