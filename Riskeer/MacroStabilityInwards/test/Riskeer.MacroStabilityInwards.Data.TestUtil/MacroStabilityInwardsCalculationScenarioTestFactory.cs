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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating different instances of <see cref="MacroStabilityInwardsCalculationScenario"/>
    /// for easier testing.
    /// </summary>
    public static class MacroStabilityInwardsCalculationScenarioTestFactory
    {
        /// <summary>
        /// Creates a calculated scenario for which the surface line on the input intersects with <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenario(FailureMechanismSection section)
        {
            MacroStabilityInwardsCalculationScenario scenario = CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            scenario.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            return scenario;
        }

        /// <summary>
        /// Creates a calculated scenario for which the surface line on the input intersects with <paramref name="section"/>.
        /// </summary>
        /// <param name="factorOfStability">The value for <see cref="MacroStabilityInwardsOutput.FactorOfStability"/>.</param>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenario(double factorOfStability,
                                                                                                              FailureMechanismSection section)
        {
            MacroStabilityInwardsCalculationScenario scenario = CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            scenario.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput(new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = factorOfStability
            });

            return scenario;
        }

        /// <summary>
        /// Creates a calculated scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has <see cref="double.NaN"/> as factor of stability.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(FailureMechanismSection section)
        {
            return CreateMacroStabilityInwardsCalculationScenario(double.NaN, section);
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// is marked as not relevant for the assessment.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsCalculationScenario CreateIrrelevantMacroStabilityInwardsCalculationScenario(FailureMechanismSection section)
        {
            MacroStabilityInwardsCalculationScenario scenario = CreateMacroStabilityInwardsCalculationScenario(0.2, section);
            scenario.IsRelevant = false;
            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the surface line on the input intersects with <paramref name="section"/> and
        /// the calculation has not been performed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
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
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to set to the input.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        /// <remarks>The caller is responsible for actually providing a valid hydraulic boundary location
        /// (for instance when it comes to the presence of a normative assessment level).</remarks>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsCalculationScenario CreateMacroStabilityInwardsCalculationScenarioWithValidInput(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            const double top = 10.56;
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, new MacroStabilityInwardsSoilProfile1D("Ondergrondschematisatie", 0.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(top)
                {
                    Data =
                    {
                        MaterialName = "Clay",
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
                        MaterialName = "Sand",
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
                        MaterialName = "Soil",
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

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
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
            surfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0);

            return new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilProfile = stochasticSoilProfile,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay,
                    WaterLevelRiverAverage = (RoundedDouble) 0.4,
                    LocationInputExtreme =
                    {
                        WaterLevelPolder = (RoundedDouble) 0.5,
                        UseDefaultOffsets = false,
                        PenetrationLength = (RoundedDouble) 1.0,
                        PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 1.0,
                        PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 1.0,
                        PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 1.0,
                        PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 1.0
                    },
                    LocationInputDaily =
                    {
                        WaterLevelPolder = new RoundedDouble(5, 0.5),
                        UseDefaultOffsets = false,
                        PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 1.0,
                        PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 1.0,
                        PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 1.0,
                        PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 1.0
                    },
                    PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) 1.0,
                    PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) 1.0,
                    AdjustPhreaticLine3And4ForUplift = true,
                    LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) 1.0,
                    LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) 1.0,
                    LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) 1.0,
                    LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) 1.0,
                    SlipPlaneMinimumLength = (RoundedDouble) 1.0,
                    SlipPlaneMinimumDepth = (RoundedDouble) 1.0,
                    MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) 1.0,
                    MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) 1.0,
                    DrainageConstructionPresent = true,
                    XCoordinateDrainageConstruction = (RoundedDouble) 0.35,
                    ZCoordinateDrainageConstruction = (RoundedDouble) 1.0,
                    GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual,
                    TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.Specified,
                    TangentLineZTop = (RoundedDouble) 2.0,
                    TangentLineZBottom = (RoundedDouble) 1.0,
                    TangentLineNumber = 10,
                    LeftGrid =
                    {
                        XLeft = (RoundedDouble) 0.3,
                        XRight = (RoundedDouble) 0.4,
                        ZTop = (RoundedDouble) 2.0,
                        ZBottom = (RoundedDouble) 1.0,
                        NumberOfVerticalPoints = 1,
                        NumberOfHorizontalPoints = 1
                    },
                    RightGrid =
                    {
                        XLeft = (RoundedDouble) 0.4,
                        XRight = (RoundedDouble) 0.5,
                        ZTop = (RoundedDouble) 2.0,
                        ZBottom = (RoundedDouble) 1.0,
                        NumberOfVerticalPoints = 1,
                        NumberOfHorizontalPoints = 1
                    },
                    CreateZones = true,
                    ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Manual,
                    ZoneBoundaryLeft = (RoundedDouble) 0.1,
                    ZoneBoundaryRight = (RoundedDouble) 0.2
                }
            };
        }
    }
}