// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.TestUtil
{
    /// <summary>
    /// Class responsible for generating test data configurations.
    /// </summary>
    public static class PipingTestDataGenerator
    {
        /// <summary>
        /// Gets a fully configured <see cref="PipingFailureMechanism"/> with all
        /// possible parent and nested calculation configurations.
        /// </summary>
        public static PipingFailureMechanism GetPipingFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new PipingFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a <see cref="PipingCalculationScenario"/> without hydraulic boundary location or design water level.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationScenario"/> without hydraulic boundary location or design water level.</returns>
        public static PipingCalculationScenario GetPipingCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel()
        {
            PipingCalculationScenario calculation = GetPipingCalculationScenario();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="PipingCalculationScenario"/> with manual design water level set.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationScenario"/> with a manual design water level.</returns>
        public static PipingCalculationScenario GetPipingCalculationScenarioWithAssessmentLevel()
        {
            PipingCalculationScenario calculation = GetPipingCalculationScenario();
            calculation.InputParameters.UseAssessmentLevelManualInput = true;
            calculation.InputParameters.AssessmentLevel = (RoundedDouble) 3.0;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="PipingCalculationScenario"/> without surface line.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationScenario"/> without surface line.</returns>
        public static PipingCalculationScenario GetPipingCalculationScenarioWithoutSurfaceLine()
        {
            PipingCalculationScenario calculation = GetPipingCalculationScenario();
            calculation.InputParameters.SurfaceLine = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="PipingCalculationScenario"/> without soil model.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationScenario"/> without soil model .</returns>
        public static PipingCalculationScenario GetPipingCalculationScenarioWithoutSoilModel()
        {
            PipingCalculationScenario calculation = GetPipingCalculationScenario();
            calculation.InputParameters.StochasticSoilModel = null;
            calculation.InputParameters.StochasticSoilProfile = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="PipingCalculationScenario"/> without soil profile.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationScenario"/> without soil profile.</returns>
        public static PipingCalculationScenario GetPipingCalculationScenarioWithoutSoilProfile()
        {
            PipingCalculationScenario calculation = GetPipingCalculationScenario();
            calculation.InputParameters.StochasticSoilProfile = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="PipingCalculationScenario"/> with relevance set to <c>false</c>.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationScenario"/> with relevance set to <c>false</c>
        /// .</returns>
        public static PipingCalculationScenario GetIrrelevantPipingCalculationScenario()
        {
            PipingCalculationScenario calculation = GetPipingCalculationScenario();
            calculation.Contribution = (RoundedDouble) 0.5;
            calculation.IsRelevant = false;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="PipingCalculationScenario"/>.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationScenario"/>.</returns>
        public static PipingCalculationScenario GetPipingCalculationScenario()
        {
            var surfaceline = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0, 5),
                Name = "PK001_0001"
            };
            surfaceline.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(0, 10, 0)
            });

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Name = "PK001_0001 W1-6_0_1D1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_KAT_18", 0, 0),
                    SurfaceLine = surfaceline,
                    StochasticSoilModel = new StochasticSoilModel("PK001_0001_Piping"),
                    StochasticSoilProfile = new StochasticSoilProfile(0, SoilProfileType.SoilProfile1D, 0)
                    {
                        SoilProfile = new PipingSoilProfile("W1-6_0_1D1", 0, new[]
                        {
                            new PipingSoilLayer(0)
                        }, SoilProfileType.SoilProfile1D, 0)
                    },
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble) 0,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble) 0.7,
                        StandardDeviation = (RoundedDouble) 0.1
                    }
                }
            };

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="PipingCalculationScenario"/> with <c>double.NaN</c> values set.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationScenario"/> with <c>double.NaN</c> values.</returns>
        public static PipingCalculationScenario GetPipingCalculationScenarioWithNaNs()
        {
            PipingCalculationScenario calculation = GetPipingCalculationScenarioWithAssessmentLevel();
            calculation.Contribution = RoundedDouble.NaN;
            calculation.InputParameters.AssessmentLevel = RoundedDouble.NaN;
            calculation.InputParameters.EntryPointL = RoundedDouble.NaN;
            calculation.InputParameters.ExitPointL = RoundedDouble.NaN;
            calculation.InputParameters.PhreaticLevelExit = new NormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };
            calculation.InputParameters.DampingFactorExit = new LogNormalDistribution
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN
            };

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="PipingCalculationScenario"/> with <c>double.NegativeInfinity</c> 
        /// and <c>double.PositiveInfinity</c> values set.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationScenario"/> with <c>double.NegativeInfinity</c> 
        /// and <c>double.PositiveInfinity</c> values.</returns>
        public static PipingCalculationScenario GetPipingCalculationScenarioWithInfinities()
        {
            PipingCalculationScenario calculation = GetPipingCalculationScenarioWithAssessmentLevel();
            calculation.Contribution = (RoundedDouble) double.PositiveInfinity;

            calculation.InputParameters.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, double.NegativeInfinity, 0),
                new Point3D(0, double.PositiveInfinity, 0)
            });

            calculation.InputParameters.AssessmentLevel = (RoundedDouble) double.NegativeInfinity;
            calculation.InputParameters.EntryPointL = (RoundedDouble) double.NegativeInfinity;
            calculation.InputParameters.ExitPointL = (RoundedDouble) double.PositiveInfinity;
            calculation.InputParameters.PhreaticLevelExit = new NormalDistribution
            {
                Mean = (RoundedDouble) double.NegativeInfinity,
                StandardDeviation = (RoundedDouble) double.PositiveInfinity
            };
            calculation.InputParameters.DampingFactorExit = new LogNormalDistribution
            {
                Mean = (RoundedDouble) double.PositiveInfinity,
                StandardDeviation = (RoundedDouble) double.PositiveInfinity
            };

            return calculation;
        }

        /// <summary>
        /// Configures a <see cref="PipingFailureMechanism"/> to a fully configured failure
        /// mechanism with all possible parent and nested calculation configurations.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be updated.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location used
        /// by calculations.</param>
        /// <remarks>This method assumes <paramref name="failureMechanism"/> is a newly
        /// created instance.</remarks>
        public static void ConfigureFailureMechanismWithAllCalculationConfigurations(PipingFailureMechanism failureMechanism,
                                                                                     HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var surfaceline1 = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0, 5),
                Name = "Line A"
            };
            surfaceline1.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(0, 10, 0)
            });
            var surfaceline2 = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(10, 5),
                Name = "Line B"
            };
            surfaceline2.SetGeometry(new[]
            {
                new Point3D(10, 0, 0),
                new Point3D(10, 10, 0)
            });

            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceline1,
                surfaceline2
            }, "some/path/to/surfacelines");
            var stochasticSoilModel1 = new StochasticSoilModel("A")
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
            var stochasticSoilModel2 = new StochasticSoilModel("C")
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

            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel1,
                stochasticSoilModel2
            }, "some/path/to/stochasticsoilmodels");

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
                    StochasticSoilModel = stochasticSoilModel1,
                    StochasticSoilProfile = stochasticSoilModel1.StochasticSoilProfiles[0]
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
            var calculationWithSurfaceLineAndStochasticSoilModel = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceline1,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles[0]
                }
            };

            var subCalculation = new PipingCalculationScenario(new GeneralPipingInput());
            var subCalculationWithOutput = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceline2,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles[0]
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
                    StochasticSoilModel = stochasticSoilModel1,
                    StochasticSoilProfile = stochasticSoilModel1.StochasticSoilProfiles[0]
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
            var subCalculationWithSurfaceLineAndStochasticSoilModel = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceline1,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles[0]
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithSurfaceLineAndSoilModel);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithSurfaceLineAndStochasticSoilModel);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutput,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithSurfaceLineAndStochasticSoilModel
                }
            });
        }
    }
}