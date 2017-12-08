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

using System.Collections.Generic;
using System.Linq;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="MapFeature"/> to use in <see cref="FeatureBasedMapData"/>.
    /// </summary>
    internal static class StabilityStoneCoverMapDataFeaturesFactory
    {
        /// <summary>
        /// Create calculation features based on the provided <paramref name="calculationInputs"/>.
        /// </summary>
        /// <param name="calculationInputs">The collection of <see cref="StabilityStoneCoverWaveConditionsCalculation"/> to create the calculation features for.</param>
        /// <returns>A collection of features or an empty collection when <paramref name="calculationInputs"/> is <c>null</c> or empty.</returns>
        public static IEnumerable<MapFeature> CreateCalculationFeatures(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculationInputs)
        {
            bool hasCalculations = calculationInputs != null && calculationInputs.Any();

            if (!hasCalculations)
            {
                return new MapFeature[0];
            }

            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculationsWithLocationAndHydraulicBoundaryLocation =
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
    }
}