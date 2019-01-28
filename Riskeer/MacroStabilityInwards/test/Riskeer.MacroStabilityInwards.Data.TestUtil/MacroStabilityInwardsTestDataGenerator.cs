// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
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
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydraulicBoundaryLocation);

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

            calculation.InputParameters.HydraulicBoundaryLocation = null;
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
            calculation.Contribution = (RoundedDouble) 0.5432;
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

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = "PK001_0001 W1-6_0_1D1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_KAT_18", 0, 0),
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("PK001_0001_Macrostabiliteit"),
                    StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0, new MacroStabilityInwardsSoilProfile1D("W1-6_0_1D1", 0, new[]
                    {
                        new MacroStabilityInwardsSoilLayer1D(0)
                    })),
                    LocationInputDaily =
                    {
                        UseDefaultOffsets = true,
                        WaterLevelPolder = (RoundedDouble) 2.2,
                        PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 2.21,
                        PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 2.22,
                        PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 2.23,
                        PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 2.24
                    },
                    LocationInputExtreme =
                    {
                        UseDefaultOffsets = false,
                        PenetrationLength = (RoundedDouble) 16.2,
                        WaterLevelPolder = (RoundedDouble) 15.2,
                        PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 15.21,
                        PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 15.22,
                        PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 15.23,
                        PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 15.24
                    },
                    WaterLevelRiverAverage = (RoundedDouble) 10.5,
                    DrainageConstructionPresent = true,
                    XCoordinateDrainageConstruction = (RoundedDouble) 10.6,
                    ZCoordinateDrainageConstruction = (RoundedDouble) 10.7,
                    MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) 10.8,
                    MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) 10.9,
                    LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) 10.1,
                    LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) 10.2,
                    LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) 10.3,
                    LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) 10.4,
                    PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) 20.1,
                    PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) 20.2,
                    SlipPlaneMinimumDepth = (RoundedDouble) 0.4,
                    SlipPlaneMinimumLength = (RoundedDouble) 0.5,
                    MaximumSliceWidth = (RoundedDouble) 0.6,
                    TangentLineZTop = (RoundedDouble) 10,
                    TangentLineZBottom = (RoundedDouble) 1,
                    TangentLineNumber = 5,
                    CreateZones = true,
                    ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Manual,
                    ZoneBoundaryLeft = (RoundedDouble) 0,
                    ZoneBoundaryRight = (RoundedDouble) 100
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

            MacroStabilityInwardsInput input = calculation.InputParameters;

            input.WaterLevelRiverAverage = RoundedDouble.NaN;
            input.XCoordinateDrainageConstruction = RoundedDouble.NaN;
            input.ZCoordinateDrainageConstruction = RoundedDouble.NaN;

            input.MinimumLevelPhreaticLineAtDikeTopPolder = RoundedDouble.NaN;
            input.MinimumLevelPhreaticLineAtDikeTopRiver = RoundedDouble.NaN;

            input.LeakageLengthInwardsPhreaticLine3 = RoundedDouble.NaN;
            input.LeakageLengthOutwardsPhreaticLine3 = RoundedDouble.NaN;
            input.LeakageLengthInwardsPhreaticLine4 = RoundedDouble.NaN;
            input.LeakageLengthOutwardsPhreaticLine4 = RoundedDouble.NaN;
            input.PiezometricHeadPhreaticLine2Inwards = RoundedDouble.NaN;
            input.PiezometricHeadPhreaticLine2Outwards = RoundedDouble.NaN;

            input.AssessmentLevel = RoundedDouble.NaN;
            input.SlipPlaneMinimumDepth = RoundedDouble.NaN;
            input.SlipPlaneMinimumLength = RoundedDouble.NaN;
            input.MaximumSliceWidth = RoundedDouble.NaN;

            input.TangentLineZTop = RoundedDouble.NaN;
            input.TangentLineZBottom = RoundedDouble.NaN;

            input.LeftGrid.XLeft = RoundedDouble.NaN;
            input.LeftGrid.XRight = RoundedDouble.NaN;
            input.LeftGrid.ZTop = RoundedDouble.NaN;
            input.LeftGrid.ZBottom = RoundedDouble.NaN;

            input.RightGrid.XLeft = RoundedDouble.NaN;
            input.RightGrid.XRight = RoundedDouble.NaN;
            input.RightGrid.ZTop = RoundedDouble.NaN;
            input.RightGrid.ZBottom = RoundedDouble.NaN;

            IMacroStabilityInwardsLocationInputDaily inputDaily = input.LocationInputDaily;
            inputDaily.WaterLevelPolder = RoundedDouble.NaN;
            inputDaily.PhreaticLineOffsetBelowDikeTopAtRiver = RoundedDouble.NaN;
            inputDaily.PhreaticLineOffsetBelowDikeTopAtPolder = RoundedDouble.NaN;
            inputDaily.PhreaticLineOffsetBelowShoulderBaseInside = RoundedDouble.NaN;
            inputDaily.PhreaticLineOffsetBelowDikeToeAtPolder = RoundedDouble.NaN;

            IMacroStabilityInwardsLocationInputExtreme inputExtreme = input.LocationInputExtreme;
            inputExtreme.PenetrationLength = RoundedDouble.NaN;
            inputExtreme.WaterLevelPolder = RoundedDouble.NaN;
            inputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver = RoundedDouble.NaN;
            inputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder = RoundedDouble.NaN;
            inputExtreme.PhreaticLineOffsetBelowShoulderBaseInside = RoundedDouble.NaN;
            inputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder = RoundedDouble.NaN;

            input.ZoneBoundaryLeft = RoundedDouble.NaN;
            input.ZoneBoundaryRight = RoundedDouble.NaN;

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

            MacroStabilityInwardsInput input = calculation.InputParameters;
            input.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, double.NegativeInfinity, 0),
                new Point3D(0, double.PositiveInfinity, 0)
            });

            input.WaterLevelRiverAverage = (RoundedDouble) double.PositiveInfinity;
            input.XCoordinateDrainageConstruction = (RoundedDouble) double.PositiveInfinity;
            input.ZCoordinateDrainageConstruction = (RoundedDouble) double.NegativeInfinity;

            input.MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) double.PositiveInfinity;
            input.MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) double.NegativeInfinity;

            input.LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) double.PositiveInfinity;
            input.LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) double.NegativeInfinity;
            input.LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) double.PositiveInfinity;
            input.LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) double.NegativeInfinity;
            input.PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) double.PositiveInfinity;
            input.PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) double.NegativeInfinity;

            input.AssessmentLevel = (RoundedDouble) double.NegativeInfinity;

            input.SlipPlaneMinimumDepth = (RoundedDouble) double.NegativeInfinity;
            input.SlipPlaneMinimumLength = (RoundedDouble) double.PositiveInfinity;
            input.MaximumSliceWidth = (RoundedDouble) double.NegativeInfinity;

            input.TangentLineZTop = (RoundedDouble) double.PositiveInfinity;
            input.TangentLineZBottom = (RoundedDouble) double.NegativeInfinity;

            input.LeftGrid.XLeft = (RoundedDouble) double.NegativeInfinity;
            input.LeftGrid.XRight = (RoundedDouble) double.PositiveInfinity;
            input.LeftGrid.ZTop = (RoundedDouble) double.PositiveInfinity;
            input.LeftGrid.ZBottom = (RoundedDouble) double.NegativeInfinity;

            input.RightGrid.XLeft = (RoundedDouble) double.NegativeInfinity;
            input.RightGrid.XRight = (RoundedDouble) double.PositiveInfinity;
            input.RightGrid.ZTop = (RoundedDouble) double.PositiveInfinity;
            input.RightGrid.ZBottom = (RoundedDouble) double.NegativeInfinity;

            IMacroStabilityInwardsLocationInputDaily inputDaily = input.LocationInputDaily;
            inputDaily.WaterLevelPolder = (RoundedDouble) double.PositiveInfinity;
            inputDaily.PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) double.PositiveInfinity;
            inputDaily.PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) double.PositiveInfinity;
            inputDaily.PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) double.NegativeInfinity;
            inputDaily.PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) double.NegativeInfinity;

            IMacroStabilityInwardsLocationInputExtreme inputExtreme = input.LocationInputExtreme;
            inputExtreme.PenetrationLength = (RoundedDouble) double.PositiveInfinity;
            inputExtreme.WaterLevelPolder = (RoundedDouble) double.NegativeInfinity;
            inputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) double.NegativeInfinity;
            inputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) double.NegativeInfinity;
            inputExtreme.PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) double.PositiveInfinity;
            inputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) double.PositiveInfinity;

            input.ZoneBoundaryLeft = (RoundedDouble) double.NegativeInfinity;
            input.ZoneBoundaryRight = (RoundedDouble) double.PositiveInfinity;

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
            var stochasticSoilModel1 = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(-5, 5),
                new Point2D(5, 5)
            }, new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile1D("test", 3, new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(4)
                }))
            });
            var stochasticSoilModel2 = new MacroStabilityInwardsStochasticSoilModel("C", new[]
            {
                new Point2D(5, 5),
                new Point2D(15, 5)
            }, new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile2D("test", new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 0),
                        new Point2D(0, 0)
                    }))
                }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>()))
            });

            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel1,
                stochasticSoilModel2
            }, "some/path/to/stochasticsoilmodels");

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var calculationWithOutput = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel1
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };
            var calculationWithSurfaceLineAndSoilModel = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel1,
                    StochasticSoilProfile = stochasticSoilModel1.StochasticSoilProfiles.ElementAt(0)
                }
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine2,
                    StochasticSoilModel = stochasticSoilModel2
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };
            var calculationWithHydraulicBoundaryLocation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var calculationWithSurfaceLineAndStochasticSoilModel = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles.ElementAt(0)
                }
            };

            var subCalculation = new MacroStabilityInwardsCalculationScenario();
            var subCalculationWithOutput = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine2,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles.ElementAt(0)
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel1,
                    StochasticSoilProfile = stochasticSoilModel1.StochasticSoilProfiles.ElementAt(0)
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };
            var subCalculationWithHydraulicBoundaryLocation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var subCalculationWithSurfaceLineAndStochasticSoilModel = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine1,
                    StochasticSoilModel = stochasticSoilModel2,
                    StochasticSoilProfile = stochasticSoilModel2.StochasticSoilProfiles.ElementAt(0)
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
        /// Sets random values to all setters, including the nested ones, 
        /// of the <see cref="MacroStabilityInwardsInput"/>.
        /// </summary>
        /// <param name="input">The input to set the random data to.</param>
        public static void SetRandomMacroStabilityInwardsInput(MacroStabilityInwardsInput input)
        {
            var random = new Random(21);

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Surface line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 1, 1)
            });

            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = stochasticSoilModel.StochasticSoilProfiles.First();
            input.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            input.StochasticSoilModel = stochasticSoilModel;
            input.StochasticSoilProfile = stochasticSoilProfile;
            input.SurfaceLine = surfaceLine;
            input.UseAssessmentLevelManualInput = random.NextBoolean();
            input.AssessmentLevel = random.NextRoundedDouble();
            input.SlipPlaneMinimumDepth = random.NextRoundedDouble();
            input.SlipPlaneMinimumLength = random.NextRoundedDouble();
            input.MaximumSliceWidth = random.NextRoundedDouble();
            input.MoveGrid = random.NextBoolean();
            input.DikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>();
            input.WaterLevelRiverAverage = random.NextRoundedDouble();
            input.DrainageConstructionPresent = random.NextBoolean();
            input.XCoordinateDrainageConstruction = random.NextRoundedDouble();
            input.ZCoordinateDrainageConstruction = random.NextRoundedDouble();
            input.MinimumLevelPhreaticLineAtDikeTopRiver = random.NextRoundedDouble();
            input.MinimumLevelPhreaticLineAtDikeTopPolder = random.NextRoundedDouble();

            input.LocationInputExtreme.WaterLevelPolder = random.NextRoundedDouble();
            input.LocationInputExtreme.UseDefaultOffsets = random.NextBoolean();
            input.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble();
            input.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble();
            input.LocationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble();
            input.LocationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble();
            input.LocationInputExtreme.PenetrationLength = random.NextRoundedDouble();

            input.LocationInputDaily.WaterLevelPolder = random.NextRoundedDouble();
            input.LocationInputDaily.UseDefaultOffsets = random.NextBoolean();
            input.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble();
            input.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble();
            input.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble();
            input.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble();

            input.AdjustPhreaticLine3And4ForUplift = random.NextBoolean();
            input.LeakageLengthOutwardsPhreaticLine3 = random.NextRoundedDouble();
            input.LeakageLengthInwardsPhreaticLine3 = random.NextRoundedDouble();
            input.LeakageLengthOutwardsPhreaticLine4 = random.NextRoundedDouble();
            input.LeakageLengthInwardsPhreaticLine4 = random.NextRoundedDouble();
            input.PiezometricHeadPhreaticLine2Outwards = random.NextRoundedDouble();
            input.PiezometricHeadPhreaticLine2Inwards = random.NextRoundedDouble();
            input.GridDeterminationType = random.NextEnumValue<MacroStabilityInwardsGridDeterminationType>();
            input.TangentLineDeterminationType = random.NextEnumValue<MacroStabilityInwardsTangentLineDeterminationType>();
            input.TangentLineZTop = random.NextRoundedDouble();
            input.TangentLineZBottom = random.NextRoundedDouble();
            input.TangentLineNumber = random.Next(1, 50);

            input.LeftGrid.XLeft = random.NextRoundedDouble(0.0, 1.0);
            input.LeftGrid.XRight = random.NextRoundedDouble(2.0, 3.0);
            input.LeftGrid.NumberOfHorizontalPoints = random.Next(1, 100);
            input.LeftGrid.ZTop = random.NextRoundedDouble(2.0, 3.0);
            input.LeftGrid.ZBottom = random.NextRoundedDouble(0.0, 1.0);
            input.LeftGrid.NumberOfVerticalPoints = random.Next(1, 100);

            input.RightGrid.XLeft = random.NextRoundedDouble(0.0, 1.0);
            input.RightGrid.XRight = random.NextRoundedDouble(2.0, 3.0);
            input.RightGrid.NumberOfHorizontalPoints = random.Next(1, 100);
            input.RightGrid.ZTop = random.NextRoundedDouble(2.0, 3.0);
            input.RightGrid.ZBottom = random.NextRoundedDouble(0.0, 1.0);
            input.RightGrid.NumberOfVerticalPoints = random.Next(1, 100);

            input.CreateZones = random.NextBoolean();
            input.ZoningBoundariesDeterminationType = random.NextEnumValue<MacroStabilityInwardsZoningBoundariesDeterminationType>();
            input.ZoneBoundaryLeft = random.NextRoundedDouble();
            input.ZoneBoundaryRight = random.NextRoundedDouble();
        }
    }
}