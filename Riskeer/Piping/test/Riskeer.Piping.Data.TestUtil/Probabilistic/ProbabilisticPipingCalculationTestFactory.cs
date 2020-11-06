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
using Core.Common.Base.Data;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Data.TestUtil.Probabilistic
{
    /// <summary>
    /// Helper class for creating instances of <see cref="ProbabilisticPipingCalculation"/>.
    /// </summary>
    public static class ProbabilisticPipingCalculationTestFactory
    {
        /// <summary>
        /// Creates a probabilistic calculation with valid input.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to set to the input.</param>
        /// <typeparam name="T">The type of probabilistic calculation to create.</typeparam>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public static T CreateCalculationWithValidInput<T>(HydraulicBoundaryLocation hydraulicBoundaryLocation)
            where T : ProbabilisticPipingCalculation, new()
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            return new T
            {
                InputParameters =
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
                }
            };
        }

        /// <summary>
        /// Creates a probabilistic calculation with invalid input.
        /// </summary>
        /// <typeparam name="T">The type of probabilistic calculation to create.</typeparam>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        public static T CreateCalculationWithInvalidInput<T>()
            where T : ProbabilisticPipingCalculation, new()
        {
            return new T();
        }
    }
}