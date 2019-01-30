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
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating different instances of <see cref="PipingCalculationScenario"/>
    /// for easier testing.
    /// </summary>
    public static class PipingCalculationScenarioTestFactory
    {
        /// <summary>
        /// Creates a calculated scenario for which the surface line on the input intersects with <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static PipingCalculationScenario CreatePipingCalculationScenario(FailureMechanismSection section)
        {
            PipingCalculationScenario scenario = CreateNotCalculatedPipingCalculationScenario(section);
            scenario.Output = PipingOutputTestFactory.Create();

            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// is marked as not relevant for the assessment.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static PipingCalculationScenario CreateIrrelevantPipingCalculationScenario(FailureMechanismSection section)
        {
            PipingCalculationScenario scenario = CreateNotCalculatedPipingCalculationScenario(section);
            scenario.IsRelevant = false;
            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has not been performed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static PipingCalculationScenario CreateNotCalculatedPipingCalculationScenario(FailureMechanismSection section)
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

            var scenario = new PipingCalculationScenario(new GeneralPipingInput())
            {
                IsRelevant = true,
                InputParameters =
                {
                    SurfaceLine = pipingSurfaceLine
                }
            };
            return scenario;
        }

        /// <summary>
        /// Creates a scenario with invalid input.
        /// </summary>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreatePipingCalculationScenarioWithInvalidInput()
        {
            return new PipingCalculationScenario(new GeneralPipingInput());
        }

        /// <summary>
        /// Creates a scenario with valid input.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to set to the input.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        /// <remarks>The caller is responsible for actually providing a valid hydraulic boundary location
        /// (for instance when it comes to the presence of a normative assessment level).</remarks>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public static PipingCalculationScenario CreatePipingCalculationScenarioWithValidInput(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            const double bottom = 1.12;
            const double top = 10.56;
            var stochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0.0, new[]
                {
                    new PipingSoilLayer(top)
                    {
                        IsAquifer = false,
                        BelowPhreaticLevel = new LogNormalDistribution
                        {
                            Mean = (RoundedDouble) 17.5,
                            StandardDeviation = (RoundedDouble) 0,
                            Shift = (RoundedDouble) 10
                        }
                    },
                    new PipingSoilLayer(top / 2)
                    {
                        IsAquifer = true,
                        DiameterD70 = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) 4.0e-4,
                            CoefficientOfVariation = (RoundedDouble) 0
                        },
                        Permeability = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) 1.0,
                            CoefficientOfVariation = (RoundedDouble) 0.5
                        }
                    }
                }, SoilProfileType.SoilProfile1D));
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            var firstCharacteristicPointLocation = new Point3D(0.2, 0.0, bottom + 3 * top / 4);
            var secondCharacteristicPointLocation = new Point3D(0.3, 0.0, bottom + 2 * top / 4);
            var thirdCharacteristicPointLocation = new Point3D(0.4, 0.0, bottom + top / 4);
            var fourthCharacteristicPointLocation = new Point3D(0.5, 0.0, bottom + 2 * top / 4);
            var fifthCharacteristicPointLocation = new Point3D(0.6, 0.0, bottom + 3 * top / 4);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 0.0),
                firstCharacteristicPointLocation,
                secondCharacteristicPointLocation,
                thirdCharacteristicPointLocation,
                fourthCharacteristicPointLocation,
                fifthCharacteristicPointLocation,
                new Point3D(1.0, 0.0, top)
            });
            surfaceLine.SetDikeToeAtPolderAt(firstCharacteristicPointLocation);
            surfaceLine.SetDitchDikeSideAt(secondCharacteristicPointLocation);
            surfaceLine.SetBottomDitchDikeSideAt(thirdCharacteristicPointLocation);
            surfaceLine.SetBottomDitchPolderSideAt(fourthCharacteristicPointLocation);
            surfaceLine.SetDitchPolderSideAt(fifthCharacteristicPointLocation);
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0);

            return new PipingCalculationScenario(new GeneralPipingInput())
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
                    SurfaceLine = surfaceLine,
                    StochasticSoilProfile = stochasticSoilProfile,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
        }
    }
}