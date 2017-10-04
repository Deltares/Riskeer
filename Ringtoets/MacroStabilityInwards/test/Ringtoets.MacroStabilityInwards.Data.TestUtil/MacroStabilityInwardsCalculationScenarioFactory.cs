﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating different instances of <see cref="MacroStabilityInwardsCalculationScenario"/>
    /// for easier testing.
    /// </summary>
    public static class MacroStabilityInwardsCalculationScenarioFactory
    {
        /// <summary>
        /// Creates a calculated scenario for which the surface line on the input intersects with <paramref name="section"/>.
        /// </summary>
        /// <param name="probability">The value for <see cref="MacroStabilityInwardsSemiProbabilisticOutput.MacroStabilityInwardsProbability"/>.</param>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenario(double probability,
                                                                                                              FailureMechanismSection section)
        {
            MacroStabilityInwardsCalculationScenario scenario = CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            var random = new Random(21);
            scenario.SemiProbabilisticOutput = new MacroStabilityInwardsSemiProbabilisticOutput(
                random.NextDouble(),
                random.NextDouble(),
                (RoundedDouble) probability,
                random.NextDouble(),
                random.NextDouble());

            scenario.Output = new TestMacroStabilityInwardsOutput();

            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has failed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateFailedMacroStabilityInwardsCalculationScenario(FailureMechanismSection section)
        {
            return CreateMacroStabilityInwardsCalculationScenario(RoundedDouble.NaN, section);
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// is marked as not relevant for the assessment.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateIrrelevantMacroStabilityInwardsCalculationScenario(FailureMechanismSection section)
        {
            MacroStabilityInwardsCalculationScenario scenario = CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            scenario.IsRelevant = false;
            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has not been performed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateNotCalculatedMacroStabilityInwardsCalculationScenario(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            Point2D p = section.Points.First();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(p.X, p.Y, 0),
                new Point3D(p.X + 2, p.Y + 2, 0)
            });
            surfaceLine.ReferenceLineIntersectionWorldPoint = section.Points.First();

            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                IsRelevant = true,
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };
            return scenario;
        }

        /// <summary>
        /// Creates a scenario with invalid input.
        /// </summary>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput()
        {
            return new MacroStabilityInwardsCalculationScenario();
        }

        /// <summary>
        /// Creates a scenario with valid input.
        /// </summary>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenarioWithValidInput()
        {
            const double top = 10.56;
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, new MacroStabilityInwardsSoilProfile1D(string.Empty, 0.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(top)
                {
                    Data =
                    {
                        IsAquifer = false,
                        Cohesion = new VariationCoefficientLogNormalDistribution(),
                        FrictionAngle = new VariationCoefficientLogNormalDistribution(),
                        AbovePhreaticLevel =
                        {
                            Mean = (RoundedDouble) 0.3,
                            CoefficientOfVariation = (RoundedDouble) 0.2,
                            Shift = (RoundedDouble) 0.1
                        },
                        BelowPhreaticLevel =
                        {
                            Mean = (RoundedDouble) 15,
                            CoefficientOfVariation = (RoundedDouble) 0.5,
                            Shift = (RoundedDouble) 0.2
                        }
                    }
                },
                new MacroStabilityInwardsSoilLayer1D(6.0)
                {
                    Data =
                    {
                        IsAquifer = true,
                        Cohesion = new VariationCoefficientLogNormalDistribution(),
                        FrictionAngle = new VariationCoefficientLogNormalDistribution(),
                        AbovePhreaticLevel =
                        {
                            Mean = (RoundedDouble) 0.3,
                            CoefficientOfVariation = (RoundedDouble) 0.2,
                            Shift = (RoundedDouble) 0.1
                        },
                        BelowPhreaticLevel =
                        {
                            Mean = (RoundedDouble) 15,
                            CoefficientOfVariation = (RoundedDouble) 0.5,
                            Shift = (RoundedDouble) 0.2
                        }
                    }
                },
                new MacroStabilityInwardsSoilLayer1D(0.1)
                {
                    Data =
                    {
                        IsAquifer = false,
                        Cohesion = new VariationCoefficientLogNormalDistribution(),
                        FrictionAngle = new VariationCoefficientLogNormalDistribution(),
                        AbovePhreaticLevel =
                        {
                            Mean = (RoundedDouble) 0.3,
                            CoefficientOfVariation = (RoundedDouble) 0.2,
                            Shift = (RoundedDouble) 0.1
                        },
                        BelowPhreaticLevel =
                        {
                            Mean = (RoundedDouble) 15,
                            CoefficientOfVariation = (RoundedDouble) 0.5,
                            Shift = (RoundedDouble) 0.2
                        }
                    }
                }
            }));

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var firstCharacteristicPointLocation = new Point3D(0.1, 0.0, 2);
            var secondCharacteristicPointLocation = new Point3D(0.2, 0.0, 2);
            var thirdCharacteristicPointLocation = new Point3D(0.3, 0.0, 3);
            var fourthCharacteristicPointLocation = new Point3D(0.4, 0.0, 3);
            var fifthCharacteristicPointLocation = new Point3D(0.5, 0.0, 1);
            var sixthCharacteristicPointLocation = new Point3D(0.6, 0.0, 1);

            surfaceLine.SetGeometry(new[]
            {
                firstCharacteristicPointLocation,
                secondCharacteristicPointLocation,
                thirdCharacteristicPointLocation,
                fourthCharacteristicPointLocation,
                fifthCharacteristicPointLocation,
                sixthCharacteristicPointLocation
            });
            surfaceLine.SetSurfaceLevelOutsideAt(firstCharacteristicPointLocation);
            surfaceLine.SetDikeToeAtRiverAt(secondCharacteristicPointLocation);
            surfaceLine.SetDikeTopAtRiverAt(thirdCharacteristicPointLocation);
            surfaceLine.SetDikeTopAtPolderAt(fourthCharacteristicPointLocation);
            surfaceLine.SetDikeToeAtPolderAt(fifthCharacteristicPointLocation);
            surfaceLine.SetSurfaceLevelInsideAt(sixthCharacteristicPointLocation);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.0);
            return new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilProfile = stochasticSoilProfile,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay,
                    WaterLevelPolder = new RoundedDouble(5, 0.5),
                    PiezometricHeadPhreaticLine2Outwards = new RoundedDouble(5, 1.0),
                    PiezometricHeadPhreaticLine2Inwards = new RoundedDouble(5, 1.0),
                    AdjustPhreaticLine3And4ForUplift = true,
                    UseDefaultOffsets = false,
                    LeakageLengthInwardsPhreaticLine3 = new RoundedDouble(5, 1.0),
                    LeakageLengthOutwardsPhreaticLine3 = new RoundedDouble(5, 1.0),
                    LeakageLengthOutwardsPhreaticLine4 = new RoundedDouble(5, 1.0),
                    LeakageLengthInwardsPhreaticLine4 = new RoundedDouble(5, 1.0),
                    PenetrationLength = new RoundedDouble(5, 1.0),
                    SlipPlaneMinimumLength = new RoundedDouble(5, 1.0),
                    SlipPlaneMinimumDepth = new RoundedDouble(5, 1.0),
                    MinimumLevelPhreaticLineAtDikeTopPolder = new RoundedDouble(5, 1.0),
                    MinimumLevelPhreaticLineAtDikeTopRiver = new RoundedDouble(5, 1.0),
                    PhreaticLineOffsetBelowDikeToeAtPolder = new RoundedDouble(5, 1.0),
                    PhreaticLineOffsetBelowDikeTopAtPolder = new RoundedDouble(5, 1.0),
                    PhreaticLineOffsetBelowDikeTopAtRiver = new RoundedDouble(5, 1.0),
                    PhreaticLineOffsetBelowShoulderBaseInside = new RoundedDouble(5, 1.0)
                }
            };
        }
    }
}