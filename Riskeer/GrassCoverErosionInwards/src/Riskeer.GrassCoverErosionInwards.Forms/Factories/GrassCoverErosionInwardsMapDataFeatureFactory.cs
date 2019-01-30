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

using System.Collections.Generic;
using System.Linq;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> to use in <see cref="FeatureBasedMapData"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsMapDataFeaturesFactory
    {
        /// <summary>
        /// Create calculation features based on the provided <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="GrassCoverErosionInwardsCalculation"/> to create the calculation features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="calculations"/> is <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateCalculationFeatures(IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            bool hasCalculations = calculations != null && calculations.Any();

            if (!hasCalculations)
            {
                return new MapFeature[0];
            }

            IEnumerable<GrassCoverErosionInwardsCalculation> calculationsWithLocationAndHydraulicBoundaryLocation =
                calculations.Where(c => c.InputParameters.DikeProfile != null &&
                                        c.InputParameters.HydraulicBoundaryLocation != null);

            MapCalculationData[] calculationData =
                calculationsWithLocationAndHydraulicBoundaryLocation.Select(
                    calculation => new MapCalculationData(
                        calculation.Name,
                        calculation.InputParameters.DikeProfile.WorldReferencePoint,
                        calculation.InputParameters.HydraulicBoundaryLocation)).ToArray();

            return RingtoetsMapDataFeaturesFactory.CreateCalculationFeatures(calculationData);
        }
    }
}