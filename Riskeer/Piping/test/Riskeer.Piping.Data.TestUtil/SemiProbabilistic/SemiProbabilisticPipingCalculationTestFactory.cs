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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Data.TestUtil.SemiProbabilistic
{
    /// <summary>
    /// Helper class for creating instances of <see cref="SemiProbabilisticPipingCalculation"/>.
    /// </summary>
    public static class SemiProbabilisticPipingCalculationTestFactory
    {
        /// <summary>
        /// Creates a calculated semi-probabilistic calculation for which the surface line on the input intersects with <paramref name="section"/>.
        /// </summary>
        /// <typeparam name="T">The type of semi-probabilistic calculation to create.</typeparam>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static T CreateCalculation<T>(FailureMechanismSection section)
            where T : SemiProbabilisticPipingCalculation, new()
        {
            var calculation = CreateNotCalculatedCalculation<T>(section);

            calculation.Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();

            return calculation;
        }

        /// <summary>
        /// Creates a semi-probabilistic calculation for which the surface line on the input intersects with <paramref name="section"/>
        /// and the calculation has not been performed.
        /// </summary>
        /// <typeparam name="T">The type of semi-probabilistic calculation to create.</typeparam>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static T CreateNotCalculatedCalculation<T>(FailureMechanismSection section)
            where T : SemiProbabilisticPipingCalculation, new()
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            var pipingSurfaceLine = new PipingSurfaceLine(string.Empty);
            Point2D p = section.Points.First();
            pipingSurfaceLine.SetGeometry(new[]
            {
                new Point3D(p.X, p.Y, 0),
                new Point3D(p.X + 2, p.Y + 2, 0)
            });
            pipingSurfaceLine.ReferenceLineIntersectionWorldPoint = section.Points.First();

            return new T
            {
                InputParameters =
                {
                    SurfaceLine = pipingSurfaceLine
                }
            };
        }

        /// <summary>
        /// Creates a semi-probabilistic calculation with invalid input.
        /// </summary>
        /// <typeparam name="T">The type of semi-probabilistic calculation to create.</typeparam>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        public static T CreateCalculationWithInvalidInput<T>()
            where T : SemiProbabilisticPipingCalculation, new()
        {
            return new T();
        }

        /// <summary>
        /// Creates a semi-probabilistic calculation with valid input.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to set to the input.</param>
        /// <typeparam name="T">The type of semi-probabilistic calculation to create.</typeparam>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        /// <remarks>The caller is responsible for actually providing a valid hydraulic boundary location
        /// (for instance when it comes to the presence of a normative assessment level).</remarks>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public static T CreateCalculationWithValidInput<T>(HydraulicBoundaryLocation hydraulicBoundaryLocation)
            where T : SemiProbabilisticPipingCalculation, new()
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
                    SurfaceLine = PipingCalculationScenarioTestFactory.GetSurfaceLine(),
                    StochasticSoilProfile = PipingCalculationScenarioTestFactory.GetStochasticSoilProfile(),
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
        }
    }
}