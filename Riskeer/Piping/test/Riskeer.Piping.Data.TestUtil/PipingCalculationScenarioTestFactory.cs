﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Data;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating instances of <see cref="IPipingCalculationScenario{TPipingInput}"/>.
    /// </summary>
    public static class PipingCalculationScenarioTestFactory
    {
        /// <summary>
        /// Creates a calculation scenario with valid input.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to set to the input.</param>
        /// <param name="hasOutput">Indicator whether the calculation has output.</param>
        /// <returns>A new <see cref="IPipingCalculationScenario{PipingInput}"/>.</returns>
        /// <remarks>The caller is responsible for actually providing a valid hydraulic boundary location
        /// (for instance when it comes to the presence of a normative assessment level).</remarks>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public static IPipingCalculationScenario<PipingInput> CreateCalculationWithValidInput(HydraulicBoundaryLocation hydraulicBoundaryLocation, bool hasOutput = false)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            return new TestPipingCalculationScenario(new TestPipingInput
            {
                DampingFactorExit =
                {
                    Mean = (RoundedDouble) 1.0
                },
                PhreaticLevelExit =
                {
                    Mean = (RoundedDouble) 2.0
                },
                SurfaceLine = PipingCalculationTestFactory.GetSurfaceLine(),
                StochasticSoilProfile = PipingCalculationTestFactory.GetStochasticSoilProfile(),
                HydraulicBoundaryLocation = hydraulicBoundaryLocation
            }, hasOutput);
        }
    }
}