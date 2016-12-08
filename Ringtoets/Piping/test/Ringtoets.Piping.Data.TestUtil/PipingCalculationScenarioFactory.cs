// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating different instances of <see cref="PipingCalculationScenario"/>
    /// for easier testing.
    /// </summary>
    public static class PipingCalculationScenarioFactory
    {
        /// <summary>
        /// Creates a calculated scenario for which the surface line on the input intersects with <paramref name="section"/>.
        /// </summary>
        /// <param name="probability">The value for <see cref="PipingSemiProbabilisticOutput.PipingProbability"/>.</param>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreatePipingCalculationScenario(double probability, FailureMechanismSection section)
        {
            var scenario = CreateNotCalculatedPipingCalculationScenario(section);
            var random = new Random(21);
            scenario.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                (RoundedDouble) probability,
                random.NextDouble(),
                random.NextDouble());

            scenario.Output = new TestPipingOutput();

            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has failed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreateFailedPipingCalculationScenario(FailureMechanismSection section)
        {
            return CreatePipingCalculationScenario(RoundedDouble.NaN, section);
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// is marked as not relevant for the assessment.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreateIrrelevantPipingCalculationScenario(FailureMechanismSection section)
        {
            var scenario = CreateNotCalculatedPipingCalculationScenario(section);
            scenario.IsRelevant = false;
            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has not been performed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreateNotCalculatedPipingCalculationScenario(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            var p = section.Points.First();
            ringtoetsPipingSurfaceLine.SetGeometry(new[]
            {
                new Point3D(p.X, p.Y, 0),
                new Point3D(p.X + 2, p.Y + 2, 0)
            });
            ringtoetsPipingSurfaceLine.ReferenceLineIntersectionWorldPoint = section.Points.First();

            var scenario = new PipingCalculationScenario(new GeneralPipingInput())
            {
                IsRelevant = true,
                InputParameters =
                {
                    SurfaceLine = ringtoetsPipingSurfaceLine
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
        /// <returns>A new <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario CreatePipingCalculationScenarioWithValidInput()
        {
            var bottom = 1.12;
            var top = 10.56;
            var stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0.0, new[]
                {
                    new PipingSoilLayer(top)
                    {
                        IsAquifer = false,
                        BelowPhreaticLevelDeviation = 0,
                        BelowPhreaticLevelShift = 10,
                        BelowPhreaticLevelMean = 17.5
                    },
                    new PipingSoilLayer(top/2)
                    {
                        IsAquifer = true,
                        DiameterD70Deviation = 0,
                        DiameterD70Mean = 4.0e-4,
                        PermeabilityDeviation = 0.5,
                        PermeabilityMean = 1.0
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var firstCharacteristicPointLocation = new Point3D(0.2, 0.0, bottom + 3*top/4);
            var secondCharacteristicPointLocation = new Point3D(0.3, 0.0, bottom + 2*top/4);
            var thirdCharacteristicPointLocation = new Point3D(0.4, 0.0, bottom + top/4);
            var fourthCharacteristicPointLocation = new Point3D(0.5, 0.0, bottom + 2*top/4);
            var fifthCharacteristicPointLocation = new Point3D(0.6, 0.0, bottom + 3*top/4);
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

            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.0);
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
                    SeepageLength =
                    {
                        Mean = (RoundedDouble) 1.0
                    },
                    ThicknessAquiferLayer =
                    {
                        Mean = (RoundedDouble) 1.0
                    },
                    ThicknessCoverageLayer =
                    {
                        Mean = (RoundedDouble) 1.0
                    },
                    SurfaceLine = surfaceLine,
                    StochasticSoilProfile = stochasticSoilProfile,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
        }
    }
}