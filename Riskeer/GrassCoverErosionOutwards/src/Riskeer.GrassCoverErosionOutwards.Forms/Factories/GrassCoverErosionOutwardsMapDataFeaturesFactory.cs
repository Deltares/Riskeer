// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Util;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> to use in <see cref="FeatureBasedMapData"/>.
    /// </summary>
    internal static class GrassCoverErosionOutwardsMapDataFeaturesFactory
    {
        /// <summary>
        /// Create calculation features based on the provided <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>
        /// to create the calculation features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="calculations"/> 
        /// is <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateCalculationFeatures(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations)
        {
            if (calculations == null || !calculations.Any())
            {
                return new MapFeature[0];
            }

            IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculationsWithLocationAndHydraulicBoundaryLocation =
                calculations.Where(calculation =>
                                       calculation.InputParameters.ForeshoreProfile != null &&
                                       calculation.InputParameters.HydraulicBoundaryLocation != null);

            MapCalculationData[] calculationData =
                calculationsWithLocationAndHydraulicBoundaryLocation.Select(
                    calculation => new MapCalculationData(
                        calculation.Name,
                        calculation.InputParameters.ForeshoreProfile.WorldReferencePoint,
                        calculation.InputParameters.HydraulicBoundaryLocation)).ToArray();

            return RiskeerMapDataFeaturesFactory.CreateCalculationFeatures(calculationData);
        }

        /// <summary>
        /// Create hydraulic boundary location features based on the provided <paramref name="assessmentSection"/>
        /// and <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create the location features for.</param>
        /// <param name="failureMechanism">The failure mechanism to create the location features for.</param>
        /// <returns>A collection of features.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<MapFeature> CreateHydraulicBoundaryLocationsFeatures(IAssessmentSection assessmentSection,
                                                                                       GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var metaDataAttributeNameProvider = new GrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProvider();

            return GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(assessmentSection, failureMechanism)
                                                                                      .Select(location => GrassCoverErosionOutwardsHydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(
                                                                                                  location, metaDataAttributeNameProvider))
                                                                                      .ToArray();
        }
    }
}