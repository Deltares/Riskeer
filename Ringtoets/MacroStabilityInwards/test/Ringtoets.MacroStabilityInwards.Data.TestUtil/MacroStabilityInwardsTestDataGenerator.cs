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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Class responsible for generating test data configurations.
    /// </summary>
    public static class MacroStabilityInwardsTestDataGenerator
    {
        /// <summary>
        /// Gets a fully configured <see cref="MacroStabilityInwardsFailureMechanism"/> with all
        /// possible parent and nested calculation configurations.
        /// </summary>
        public static MacroStabilityInwardsFailureMechanism GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations()
        {
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var hydroLocation = new HydraulicBoundaryLocation(1, "<hydro location>", 0, 0);
            ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydroLocation);

            return failureMechanism;
        }

        /// <summary>
        /// Gets a <see cref="MacroStabilityInwardsCalculationScenario"/> without hydraulic boundary location or design water level.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculationScenario"/> without hydraulic boundary location or design water level.</returns>
        public static MacroStabilityInwardsCalculationScenario GetMacroStabilityInwardsCalculationScenarioWithoutHydraulicLocationAndAssessmentLevel()
        {
            MacroStabilityInwardsCalculationScenario calculation = GetMacroStabilityInwardsCalculationScenario();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="MacroStabilityInwardsCalculationScenario"/> with manual design water level set.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculationScenario"/> with a manual design water level.</returns>
        public static MacroStabilityInwardsCalculationScenario GetMacroStabilityInwardsCalculationScenarioWithAssessmentLevel()
        {
            MacroStabilityInwardsCalculationScenario calculation = GetMacroStabilityInwardsCalculationScenario();
            calculation.InputParameters.UseAssessmentLevelManualInput = true;
            calculation.InputParameters.AssessmentLevel = (RoundedDouble) 3.0;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="MacroStabilityInwardsCalculationScenario"/> without surface line.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculationScenario"/> without surface line.</returns>
        public static MacroStabilityInwardsCalculationScenario GetMacroStabilityInwardsCalculationScenarioWithoutSurfaceLine()
        {
            MacroStabilityInwardsCalculationScenario calculation = GetMacroStabilityInwardsCalculationScenario();
            calculation.InputParameters.SurfaceLine = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="MacroStabilityInwardsCalculationScenario"/> without soil model.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculationScenario"/> without soil model.</returns>
        public static MacroStabilityInwardsCalculationScenario GetMacroStabilityInwardsCalculationScenarioWithoutSoilModel()
        {
            MacroStabilityInwardsCalculationScenario calculation = GetMacroStabilityInwardsCalculationScenario();
            calculation.InputParameters.StochasticSoilModel = null;
            calculation.InputParameters.StochasticSoilProfile = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="MacroStabilityInwardsCalculationScenario"/> without soil profile.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculationScenario"/> without soil profile.</returns>
        public static MacroStabilityInwardsCalculationScenario GetMacroStabilityInwardsCalculationScenarioWithoutSoilProfile()
        {
            MacroStabilityInwardsCalculationScenario calculation = GetMacroStabilityInwardsCalculationScenario();
            calculation.InputParameters.StochasticSoilProfile = null;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="MacroStabilityInwardsCalculationScenario"/> with relevance set to <c>false</c>.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculationScenario"/> with relevance set to <c>false</c>.</returns>
        public static MacroStabilityInwardsCalculationScenario GetIrrelevantMacroStabilityInwardsCalculationScenario()
        {
            MacroStabilityInwardsCalculationScenario calculation = GetMacroStabilityInwardsCalculationScenario();
            calculation.Contribution = (RoundedDouble) 0.5;
            calculation.IsRelevant = false;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario GetMacroStabilityInwardsCalculationScenario()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("PK001_0001")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0, 5)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(0, 10, 0)
            });

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                Name = "PK001_0001 W1-6_0_1D1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_KAT_18", 0, 0),
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel("PK001_0001_Macrostabiliteit"),
                    StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0, SoilProfileType.SoilProfile1D, 0)
                    {
                        SoilProfile = new MacroStabilityInwardsSoilProfile1D("W1-6_0_1D1", 0, new[]
                        {
                            new MacroStabilityInwardsSoilLayer1D(0)
                        }, SoilProfileType.SoilProfile1D, 0)
                    }
                }
            };

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="MacroStabilityInwardsCalculationScenario"/> with <c>double.NaN</c> values set.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculationScenario"/> with <c>double.NaN</c> values.</returns>
        public static MacroStabilityInwardsCalculationScenario GetMacroStabilityInwardsCalculationScenarioWithNaNs()
        {
            MacroStabilityInwardsCalculationScenario calculation = GetMacroStabilityInwardsCalculationScenarioWithAssessmentLevel();
            calculation.Contribution = RoundedDouble.NaN;
            calculation.InputParameters.AssessmentLevel = RoundedDouble.NaN;

            return calculation;
        }

        /// <summary>
        /// Gets a <see cref="MacroStabilityInwardsCalculationScenario"/> with <c>double.NegativeInfinity</c> 
        /// and <c>double.PositiveInfinity</c> values set.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculationScenario"/> with <c>double.NegativeInfinity</c> 
        /// and <c>double.PositiveInfinity</c> values.</returns>
        public static MacroStabilityInwardsCalculationScenario GetMacroStabilityInwardsCalculationScenarioWithInfinities()
        {
            MacroStabilityInwardsCalculationScenario calculation = GetMacroStabilityInwardsCalculationScenarioWithAssessmentLevel();
            calculation.Contribution = (RoundedDouble) double.PositiveInfinity;

            calculation.InputParameters.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, double.NegativeInfinity, 0),
                new Point3D(0, double.PositiveInfinity, 0)
            });

            calculation.InputParameters.AssessmentLevel = (RoundedDouble) double.NegativeInfinity;

            return calculation;
        }

        /// <summary>
        /// Configures a <see cref="MacroStabilityInwardsFailureMechanism"/> to a fully configured failure
        /// mechanism with all possible parent and nested calculation configurations.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be updated.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location used
        /// by calculations.</param>
        /// <remarks>This method assumes <paramref name="failureMechanism"/> is a newly
        /// created instance.</remarks>
        public static void ConfigureFailureMechanismWithAllCalculationConfigurations(MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                                     HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine("Line A")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0, 5)
            };
            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(0, 10, 0)
            });
            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine("Line B")
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
            var stochasticSoilModel1 = new MacroStabilityInwardsStochasticSoilModel("A")
            {
                Geometry =
                {
                    new Point2D(-5, 5),
                    new Point2D(5, 5)
                },
                StochasticSoilProfiles =
                {
                    new MacroStabilityInwardsStochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 1)
                }
            };
            var stochasticSoilModel2 = new MacroStabilityInwardsStochasticSoilModel("C")
            {
                Geometry =
                {
                    new Point2D(5, 5),
                    new Point2D(15, 5)
                },
                StochasticSoilProfiles =
                {
                    new MacroStabilityInwardsStochasticSoilProfile(1.0, SoilProfileType.SoilProfile2D, 2)
                }
            };

            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel1,
                stochasticSoilModel2
            }, "some/path/to/stochasticsoilmodels");

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationWithOutput = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel1
                },
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };
            var calculationWithSurfaceLineAndSoilModel = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel1,
                    StochasticSoilProfile = stochasticSoilModel1.StochasticSoilProfiles[0]
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine2,
                    StochasticSoilModel = stochasticSoilModel2
                },
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };
            var calculationWithHydraulicBoundaryLocation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var calculationWithSurfaceLineAndStochasticSoilModel = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles[0]
                }
            };

            var subCalculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var subCalculationWithOutput = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine2,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles[0]
                },
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel1,
                    StochasticSoilProfile = stochasticSoilModel1.StochasticSoilProfiles[0]
                },
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };
            var subCalculationWithHydraulicBoundaryLocation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var subCalculationWithSurfaceLineAndStochasticSoilModel = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine1,
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