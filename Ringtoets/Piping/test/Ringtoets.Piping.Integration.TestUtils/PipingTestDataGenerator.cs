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

using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Integration.TestUtils
{
    /// <summary>
    /// Class responsible for generating test data configurations.
    /// </summary>
    public static class PipingTestDataGenerator
    {
        /// <summary>
        /// Gets a fully configured <see cref="PipingFailureMechanism"/>.
        /// </summary>
        public static PipingFailureMechanism GetFullyConfiguredPipingFailureMechanism()
        {
            var failureMechanism = new PipingFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            SetFullyConfiguredFailureMechanism(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Configures a <see cref="PipingFailureMechanism"/> to a fully configured failure
        /// mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be updated.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location used
        /// by calculations.</param>
        /// <remarks>This method assumes <paramref name="failureMechanism"/> is a newly
        /// created instance.</remarks>
        public static void SetFullyConfiguredFailureMechanism(PipingFailureMechanism failureMechanism,
                                                              HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var surfaceline1 = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0, 5)
            };
            surfaceline1.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(0, 10, 0)
            });
            var surfaceline2 = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(10, 5)
            };
            surfaceline2.SetGeometry(new[]
            {
                new Point3D(10, 0, 0),
                new Point3D(10, 10, 0)
            });

            failureMechanism.SurfaceLines.Add(surfaceline1);
            failureMechanism.SurfaceLines.Add(surfaceline2);

            var stochasticSoilModel1 = new StochasticSoilModel(1, "A", "B")
            {
                Geometry =
                {
                    new Point2D(-5, 5),
                    new Point2D(5, 5)
                },
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
                }
            };
            var stochasticSoilModel2 = new StochasticSoilModel(2, "C", "D")
            {
                Geometry =
                {
                    new Point2D(5, 5),
                    new Point2D(15, 5)
                },
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(1.0, SoilProfileType.SoilProfile2D, 2)
                }
            };

            failureMechanism.StochasticSoilModels.Add(stochasticSoilModel1);
            failureMechanism.StochasticSoilModels.Add(stochasticSoilModel2);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var calculationWithOutput = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceline1,
                    StochasticSoilModel = stochasticSoilModel1
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var calculationWithSurfaceLineAndSoilModel = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceline1,
                    StochasticSoilModel = stochasticSoilModel1
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceline2,
                    StochasticSoilModel = stochasticSoilModel2
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var calculationWithHydraulicBoundaryLocation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new PipingCalculationScenario(new GeneralPipingInput());
            var subCalculationWithOutput = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceline2,
                    StochasticSoilModel = stochasticSoilModel2
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceline1,
                    StochasticSoilModel = stochasticSoilModel1
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var subCalculationWithHydraulicBoundaryLocation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithSurfaceLineAndSoilModel);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutput,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation
                }
            });
        }
    }
}