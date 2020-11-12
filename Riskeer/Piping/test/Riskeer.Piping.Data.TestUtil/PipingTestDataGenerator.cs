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
using Core.Common.TestUtil;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Data.TestUtil
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
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydraulicBoundaryLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a <see cref="SemiProbabilisticPipingCalculationScenario"/> without hydraulic boundary location or design water level.
        /// </summary>
        /// <returns>A <see cref="SemiProbabilisticPipingCalculationScenario"/> without hydraulic boundary location or design water level.</returns>
        public static SemiProbabilisticPipingCalculationScenario GetPipingCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel()
        {
            var calculation = GetPipingCalculationScenario<SemiProbabilisticPipingCalculationScenario>();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="SemiProbabilisticPipingCalculationScenario"/> with manual design water level set.
        /// </summary>
        /// <returns>A <see cref="SemiProbabilisticPipingCalculationScenario"/> with a manual design water level.</returns>
        public static SemiProbabilisticPipingCalculationScenario GetPipingCalculationScenarioWithAssessmentLevel()
        {
            var calculation = GetPipingCalculationScenario<SemiProbabilisticPipingCalculationScenario>();
            calculation.InputParameters.HydraulicBoundaryLocation = null;
            calculation.InputParameters.UseAssessmentLevelManualInput = true;
            calculation.InputParameters.AssessmentLevel = (RoundedDouble) 3.0;

            return calculation;
        }

        /// <summary>
        /// Gets a <typeparamref name="TCalculationScenario"/> without surface line.
        /// </summary>
        /// <typeparam name="TCalculationScenario">The type of the calculation scenario.</typeparam>
        /// <returns>A <typeparamref name="TCalculationScenario"/> without surface line.</returns>
        public static TCalculationScenario GetPipingCalculationScenarioWithoutSurfaceLine<TCalculationScenario>()
            where TCalculationScenario : IPipingCalculationScenario<PipingInput>, new()
        {
            var calculation = GetPipingCalculationScenario<TCalculationScenario>();
            calculation.InputParameters.SurfaceLine = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <typeparamref name="TCalculationScenario"/> without soil model.
        /// </summary>
        /// <typeparam name="TCalculationScenario">The type of the calculation scenario.</typeparam>
        /// <returns>A <typeparamref name="TCalculationScenario"/> without soil model .</returns>
        public static TCalculationScenario GetPipingCalculationScenarioWithoutSoilModel<TCalculationScenario>()
            where TCalculationScenario : IPipingCalculationScenario<PipingInput>, new()
        {
            var calculation = GetPipingCalculationScenario<TCalculationScenario>();
            calculation.InputParameters.StochasticSoilModel = null;
            calculation.InputParameters.StochasticSoilProfile = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <typeparamref name="TCalculationScenario"/> without soil profile.
        /// </summary>
        /// <typeparam name="TCalculationScenario">The type of the calculation scenario.</typeparam>
        /// <returns>A <typeparamref name="TCalculationScenario"/> without soil profile.</returns>
        public static TCalculationScenario GetPipingCalculationScenarioWithoutSoilProfile<TCalculationScenario>()
            where TCalculationScenario : IPipingCalculationScenario<PipingInput>, new()
        {
            var calculation = GetPipingCalculationScenario<TCalculationScenario>();
            calculation.InputParameters.StochasticSoilProfile = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <typeparamref name="TCalculationScenario"/> with relevance set to <c>false</c>.
        /// </summary>
        /// <typeparam name="TCalculationScenario">The type of the calculation scenario.</typeparam>
        /// <returns>A <typeparamref name="TCalculationScenario"/> with relevance set to <c>false</c>.</returns>
        public static TCalculationScenario GetIrrelevantPipingCalculationScenario<TCalculationScenario>()
            where TCalculationScenario : IPipingCalculationScenario<PipingInput>, new()
        {
            var calculation = GetPipingCalculationScenario<TCalculationScenario>();
            calculation.Contribution = (RoundedDouble) 0.5432;
            calculation.IsRelevant = false;

            return calculation;
        }

        /// <summary>
        /// Gets a <typeparamref name="TCalculationScenario"/>.
        /// </summary>
        /// <typeparam name="TCalculationScenario">The type of the calculation scenario.</typeparam>
        /// <returns>A <typeparamref name="TCalculationScenario"/>.</returns>
        public static TCalculationScenario GetPipingCalculationScenario<TCalculationScenario>()
            where TCalculationScenario : IPipingCalculationScenario<PipingInput>, new()
        {
            var surfaceLine = new PipingSurfaceLine("PK001_0001")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0, 5)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(0, 10, 0)
            });

            var calculation = new TCalculationScenario
            {
                Name = "PK001_0001 W1-6_0_1D1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_KAT_18", 0, 0),
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("PK001_0001_Piping"),
                    StochasticSoilProfile = new PipingStochasticSoilProfile(
                        0, new PipingSoilProfile("W1-6_0_1D1", 0, new[]
                        {
                            new PipingSoilLayer(0)
                        }, SoilProfileType.SoilProfile1D)),
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
        /// Gets a <see cref="SemiProbabilisticPipingCalculationScenario"/> with <c>double.NaN</c> values set.
        /// </summary>
        /// <returns>A <see cref="SemiProbabilisticPipingCalculationScenario"/> with <c>double.NaN</c> values.</returns>
        public static SemiProbabilisticPipingCalculationScenario GetPipingCalculationScenarioWithNaNs()
        {
            SemiProbabilisticPipingCalculationScenario calculation = GetPipingCalculationScenarioWithAssessmentLevel();
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
        /// Gets a <see cref="SemiProbabilisticPipingCalculationScenario"/> with <c>double.NegativeInfinity</c> 
        /// and <c>double.PositiveInfinity</c> values set.
        /// </summary>
        /// <returns>A <see cref="SemiProbabilisticPipingCalculationScenario"/> with <c>double.NegativeInfinity</c> 
        /// and <c>double.PositiveInfinity</c> values.</returns>
        public static SemiProbabilisticPipingCalculationScenario GetPipingCalculationScenarioWithInfinities()
        {
            SemiProbabilisticPipingCalculationScenario calculation = GetPipingCalculationScenarioWithAssessmentLevel();
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
            var surfaceLine1 = new PipingSurfaceLine("Line A")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0, 5)
            };
            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(0, 10, 0)
            });
            var surfaceLine2 = new PipingSurfaceLine("Line B")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(10, 5)
            };
            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(10, 0, 0),
                new Point3D(10, 10, 0)
            });

            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, "some/path/to/surfacelines");
            var stochasticSoilModel1 = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(-5, 5),
                new Point2D(5, 5)
            }, new[]
            {
                new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            });
            var stochasticSoilModel2 = new PipingStochasticSoilModel("C", new[]
            {
                new Point2D(5, 5),
                new Point2D(15, 5)
            }, new[]
            {
                new PipingStochasticSoilProfile(1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            });

            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel1,
                stochasticSoilModel2
            }, "some/path/to/stochasticsoilmodels");

            var calculation = new TestPipingCalculationScenario();
            var calculationWithOutput = new TestPipingCalculationScenario(true)
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel1
                }
            };
            var calculationWithSurfaceLineAndSoilModel = new TestPipingCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel1,
                    StochasticSoilProfile = stochasticSoilModel1.StochasticSoilProfiles.First()
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new TestPipingCalculationScenario(true)
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine2,
                    StochasticSoilModel = stochasticSoilModel2
                }
            };
            var calculationWithHydraulicBoundaryLocation = new TestPipingCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var calculationWithSurfaceLineAndStochasticSoilModel = new TestPipingCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles.First()
                }
            };

            var subCalculation = new TestPipingCalculationScenario();
            var subCalculationWithOutput = new TestPipingCalculationScenario(true)
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine2,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles.First()
                }
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new TestPipingCalculationScenario(true)
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel1,
                    StochasticSoilProfile = stochasticSoilModel1.StochasticSoilProfiles.First()
                }
            };
            var subCalculationWithHydraulicBoundaryLocation = new TestPipingCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var subCalculationWithSurfaceLineAndStochasticSoilModel = new TestPipingCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles.First()
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

            var section1 = new FailureMechanismSection("1",
                                                       new[]
                                                       {
                                                           new Point2D(-1, -1),
                                                           new Point2D(5, 5)
                                                       });
            var section2 = new FailureMechanismSection("2",
                                                       new[]
                                                       {
                                                           new Point2D(5, 5),
                                                           new Point2D(15, 15)
                                                       });
            failureMechanism.SetSections(new[]
            {
                section1,
                section2
            }, "path/to/sections");
        }

        /// <summary>
        /// This method sets random data values to all properties of <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input to set the random data values to.</param>
        public static void SetRandomDataToPipingInput(PipingInput input)
        {
            var random = new Random(21);

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(random.Next(0, 5), random.Next(0, 5), random.Next(0, 5)),
                new Point3D(random.Next(5, 10), random.Next(5, 10), random.Next(5, 10))
            });

            input.EntryPointL = random.NextRoundedDouble();
            input.ExitPointL = random.NextRoundedDouble();
            input.SurfaceLine = surfaceLine;
            input.StochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("model");
            input.StochasticSoilProfile = new PipingStochasticSoilProfile(random.NextDouble(),
                                                                          PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            input.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            input.PhreaticLevelExit = new NormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };
            input.DampingFactorExit = new LogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };
        }

        /// <summary>
        /// This method sets random data values to all properties of <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input to set the random data values to.</param>
        public static void SetRandomDataToPipingInput(SemiProbabilisticPipingInput input)
        {
            SetRandomDataToPipingInput((PipingInput) input);

            var random = new Random(21);
            input.UseAssessmentLevelManualInput = random.NextBoolean();
            input.AssessmentLevel = random.NextRoundedDouble();
        }

        /// <summary>
        /// This method sets random data values to all properties of <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input to set the random data values to.</param>
        public static void SetRandomDataToPipingInput(ProbabilisticPipingInput input)
        {
            SetRandomDataToPipingInput((PipingInput) input);

            var random = new Random(21);
            input.ShouldProfileSpecificIllustrationPointsBeCalculated = random.NextBoolean();
            input.ShouldSectionSpecificIllustrationPointsBeCalculated = random.NextBoolean();
        }

        /// <summary>
        /// Creates a random instance of <see cref="SemiProbabilisticPipingOutput"/>.
        /// </summary>
        /// <returns>A random instance of <see cref="SemiProbabilisticPipingOutput"/>.</returns>
        public static SemiProbabilisticPipingOutput GetRandomSemiProbabilisticPipingOutput()
        {
            var random = new Random(22);

            return new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties
            {
                UpliftFactorOfSafety = random.NextDouble(),
                HeaveFactorOfSafety = random.NextDouble(),
                SellmeijerFactorOfSafety = random.NextDouble(),
                UpliftEffectiveStress = random.NextDouble(),
                HeaveGradient = random.NextDouble(),
                SellmeijerCreepCoefficient = random.NextDouble(),
                SellmeijerCriticalFall = random.NextDouble(),
                SellmeijerReducedFall = random.NextDouble()
            });
        }

        /// <summary>
        /// Creates an instance of <see cref="SemiProbabilisticPipingOutput"/> with the given values.
        /// </summary>
        /// <param name="heaveFactorOfSafety">The heave factor of safety.</param>
        /// <param name="upliftFactorOfSafety">The uplift factor of safety.</param>
        /// <param name="sellmeijerFactorOfSafety">The sellmeijer factor of safety.</param>
        /// <returns>The created <see cref="SemiProbabilisticPipingOutput"/>.</returns>
        public static SemiProbabilisticPipingOutput GetSemiProbabilisticPipingOutput(double heaveFactorOfSafety, double upliftFactorOfSafety, double sellmeijerFactorOfSafety)
        {
            return new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties
            {
                HeaveFactorOfSafety = heaveFactorOfSafety,
                UpliftFactorOfSafety = upliftFactorOfSafety,
                SellmeijerFactorOfSafety = sellmeijerFactorOfSafety
            });
        }

        /// <summary>
        /// Creates a random instance of <see cref="PartialProbabilisticPipingOutput"/>.
        /// </summary>
        /// <returns>A random instance of <see cref="PartialProbabilisticPipingOutput"/>.</returns>
        public static PartialProbabilisticPipingOutput GetRandomPartialProbabilisticPipingOutput()
        {
            return GetRandomPartialProbabilisticPipingOutput(new TestGeneralResultFaultTreeIllustrationPoint());
        }

        /// <summary>
        /// Creates a random instance of <see cref="PartialProbabilisticPipingOutput"/>.
        /// </summary>
        /// <param name="generalResult">The general result to set to the output.</param>
        /// <returns>A random instance of <see cref="PartialProbabilisticPipingOutput"/>.</returns>
        public static PartialProbabilisticPipingOutput GetRandomPartialProbabilisticPipingOutput(TestGeneralResultFaultTreeIllustrationPoint generalResult)
        {
            var random = new Random(21);

            return new PartialProbabilisticPipingOutput(random.NextDouble(),
                                                        generalResult);
        }

        /// <summary>
        /// Creates a random instance of <see cref="ProbabilisticPipingOutput"/> with illustration points.
        /// </summary>
        /// <returns>A random instance of <see cref="ProbabilisticPipingOutput"/> with illustration points.</returns>
        public static ProbabilisticPipingOutput GetRandomProbabilisticPipingOutputWithIllustrationPoints()
        {
            return new ProbabilisticPipingOutput(GetRandomPartialProbabilisticPipingOutput(),
                                                 GetRandomPartialProbabilisticPipingOutput());
        }

        /// <summary>
        /// Creates a random instance of <see cref="ProbabilisticPipingOutput"/> without illustration points.
        /// </summary>
        /// <returns>A random instance of <see cref="ProbabilisticPipingOutput"/> without illustration points.</returns>
        public static ProbabilisticPipingOutput GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
        {
            return new ProbabilisticPipingOutput(GetRandomPartialProbabilisticPipingOutput(null),
                                                 GetRandomPartialProbabilisticPipingOutput(null));
        }
    }
}