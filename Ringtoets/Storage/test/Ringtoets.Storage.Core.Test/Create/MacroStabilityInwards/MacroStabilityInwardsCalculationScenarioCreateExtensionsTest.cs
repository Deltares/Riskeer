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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.TestUtil.MacroStabilityInwards;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioCreateExtensionsTest
    {
        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var scenario = new MacroStabilityInwardsCalculationScenario();

            // Call
            TestDelegate call = () => scenario.Create(null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_MacroStabilityInwardsCalculationScenarioWithNaNValues_ReturnsMacroStabilityInwardsCalculationEntityWithExpectedNullProperties()
        {
            // Setup
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                Contribution = RoundedDouble.NaN,
                InputParameters =
                {
                    SlipPlaneMinimumDepth = RoundedDouble.NaN,
                    SlipPlaneMinimumLength = RoundedDouble.NaN,
                    MaximumSliceWidth = RoundedDouble.NaN,
                    AssessmentLevel = RoundedDouble.NaN,
                    WaterLevelRiverAverage = RoundedDouble.NaN,
                    XCoordinateDrainageConstruction = RoundedDouble.NaN,
                    ZCoordinateDrainageConstruction = RoundedDouble.NaN,
                    LocationInputExtreme =
                    {
                        WaterLevelPolder = RoundedDouble.NaN,
                        PhreaticLineOffsetBelowDikeTopAtRiver = RoundedDouble.NaN,
                        PhreaticLineOffsetBelowDikeTopAtPolder = RoundedDouble.NaN,
                        PhreaticLineOffsetBelowShoulderBaseInside = RoundedDouble.NaN,
                        PhreaticLineOffsetBelowDikeToeAtPolder = RoundedDouble.NaN,
                        PenetrationLength = RoundedDouble.NaN
                    },
                    LocationInputDaily =
                    {
                        WaterLevelPolder = RoundedDouble.NaN,
                        PhreaticLineOffsetBelowDikeTopAtRiver = RoundedDouble.NaN,
                        PhreaticLineOffsetBelowDikeTopAtPolder = RoundedDouble.NaN,
                        PhreaticLineOffsetBelowShoulderBaseInside = RoundedDouble.NaN,
                        PhreaticLineOffsetBelowDikeToeAtPolder = RoundedDouble.NaN
                    },
                    LeakageLengthOutwardsPhreaticLine3 = RoundedDouble.NaN,
                    LeakageLengthInwardsPhreaticLine3 = RoundedDouble.NaN,
                    LeakageLengthOutwardsPhreaticLine4 = RoundedDouble.NaN,
                    LeakageLengthInwardsPhreaticLine4 = RoundedDouble.NaN,
                    PiezometricHeadPhreaticLine2Outwards = RoundedDouble.NaN,
                    PiezometricHeadPhreaticLine2Inwards = RoundedDouble.NaN,
                    TangentLineZTop = RoundedDouble.NaN,
                    TangentLineZBottom = RoundedDouble.NaN,
                    LeftGrid =
                    {
                        XLeft = RoundedDouble.NaN,
                        XRight = RoundedDouble.NaN,
                        ZTop = RoundedDouble.NaN,
                        ZBottom = RoundedDouble.NaN
                    },
                    RightGrid =
                    {
                        XLeft = RoundedDouble.NaN,
                        XRight = RoundedDouble.NaN,
                        ZTop = RoundedDouble.NaN,
                        ZBottom = RoundedDouble.NaN
                    },
                    ZoneBoundaryLeft = RoundedDouble.NaN,
                    ZoneBoundaryRight = RoundedDouble.NaN
                }
            };
            var registry = new PersistenceRegistry();

            // When
            MacroStabilityInwardsCalculationEntity entity = scenario.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);

            Assert.IsNull(entity.ScenarioContribution);

            Assert.IsNull(entity.AssessmentLevel);
            Assert.IsNull(entity.SlipPlaneMinimumDepth);
            Assert.IsNull(entity.SlipPlaneMinimumLength);
            Assert.IsNull(entity.MaximumSliceWidth);
            Assert.IsNull(entity.WaterLevelRiverAverage);
            Assert.IsNull(entity.DrainageConstructionCoordinateX);
            Assert.IsNull(entity.DrainageConstructionCoordinateZ);
            Assert.IsNull(entity.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNull(entity.MinimumLevelPhreaticLineAtDikeTopPolder);

            Assert.IsNull(entity.LocationInputExtremeWaterLevelPolder);
            Assert.IsNull(entity.LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(entity.LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(entity.LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(entity.LocationInputExtremePhreaticLineOffsetDikeToeAtPolder);
            Assert.IsNull(entity.LocationInputExtremePenetrationLength);

            Assert.IsNull(entity.LocationInputDailyWaterLevelPolder);
            Assert.IsNull(entity.LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(entity.LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(entity.LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(entity.LocationInputDailyPhreaticLineOffsetDikeToeAtPolder);

            Assert.IsNull(entity.LeakageLengthOutwardsPhreaticLine3);
            Assert.IsNull(entity.LeakageLengthInwardsPhreaticLine3);
            Assert.IsNull(entity.LeakageLengthOutwardsPhreaticLine4);
            Assert.IsNull(entity.LeakageLengthInwardsPhreaticLine4);
            Assert.IsNull(entity.PiezometricHeadPhreaticLine2Outwards);
            Assert.IsNull(entity.PiezometricHeadPhreaticLine2Inwards);

            Assert.IsNull(entity.TangentLineZTop);
            Assert.IsNull(entity.TangentLineZBottom);

            Assert.IsNull(entity.LeftGridXLeft);
            Assert.IsNull(entity.LeftGridXRight);
            Assert.IsNull(entity.LeftGridZTop);
            Assert.IsNull(entity.LeftGridZBottom);

            Assert.IsNull(entity.RightGridXRight);
            Assert.IsNull(entity.RightGridXRight);
            Assert.IsNull(entity.RightGridZTop);
            Assert.IsNull(entity.RightGridZBottom);

            Assert.IsNull(entity.ZoneBoundaryLeft);
            Assert.IsNull(entity.ZoneBoundaryRight);
        }

        [Test]
        public void Create_MacroStabilityInwardsCalculationScenarioWithNumericAndBooleanPropertiesSet_ReturnsMacroStabilityInwardsCalculationEntity()
        {
            // Setup
            var random = new Random(21);
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                IsRelevant = random.NextBoolean(),
                Contribution = random.NextRoundedDouble(),
                InputParameters =
                {
                    UseAssessmentLevelManualInput = random.NextBoolean(),
                    AssessmentLevel = random.NextRoundedDouble(),
                    SlipPlaneMinimumDepth = random.NextRoundedDouble(),
                    SlipPlaneMinimumLength = random.NextRoundedDouble(),
                    MaximumSliceWidth = random.NextRoundedDouble(),
                    MoveGrid = random.NextBoolean(),
                    DikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>(),
                    WaterLevelRiverAverage = random.NextRoundedDouble(),
                    DrainageConstructionPresent = random.NextBoolean(),
                    XCoordinateDrainageConstruction = random.NextRoundedDouble(),
                    ZCoordinateDrainageConstruction = random.NextRoundedDouble(),
                    MinimumLevelPhreaticLineAtDikeTopRiver = random.NextRoundedDouble(),
                    MinimumLevelPhreaticLineAtDikeTopPolder = random.NextRoundedDouble(),
                    LocationInputExtreme =
                    {
                        WaterLevelPolder = random.NextRoundedDouble(),
                        UseDefaultOffsets = random.NextBoolean(),
                        PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble(),
                        PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble(),
                        PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble(),
                        PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble(),
                        PenetrationLength = random.NextRoundedDouble()
                    },
                    LocationInputDaily =
                    {
                        WaterLevelPolder = random.NextRoundedDouble(),
                        UseDefaultOffsets = random.NextBoolean(),
                        PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble(),
                        PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble(),
                        PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble(),
                        PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble()
                    },
                    AdjustPhreaticLine3And4ForUplift = random.NextBoolean(),
                    LeakageLengthOutwardsPhreaticLine3 = random.NextRoundedDouble(),
                    LeakageLengthInwardsPhreaticLine3 = random.NextRoundedDouble(),
                    LeakageLengthOutwardsPhreaticLine4 = random.NextRoundedDouble(),
                    LeakageLengthInwardsPhreaticLine4 = random.NextRoundedDouble(),
                    PiezometricHeadPhreaticLine2Outwards = random.NextRoundedDouble(),
                    PiezometricHeadPhreaticLine2Inwards = random.NextRoundedDouble(),
                    GridDeterminationType = random.NextEnumValue<MacroStabilityInwardsGridDeterminationType>(),
                    TangentLineDeterminationType = random.NextEnumValue<MacroStabilityInwardsTangentLineDeterminationType>(),
                    TangentLineZTop = random.NextRoundedDouble(2.0, 3.0),
                    TangentLineZBottom = random.NextRoundedDouble(0.0, 1.0),
                    TangentLineNumber = random.Next(1, 50),
                    LeftGrid =
                    {
                        XLeft = random.NextRoundedDouble(0.0, 1.0),
                        XRight = random.NextRoundedDouble(2.0, 3.0),
                        NumberOfHorizontalPoints = random.Next(1, 100),
                        ZTop = random.NextRoundedDouble(2.0, 3.0),
                        ZBottom = random.NextRoundedDouble(0.0, 1.0),
                        NumberOfVerticalPoints = random.Next(1, 100)
                    },
                    RightGrid =
                    {
                        XLeft = random.NextRoundedDouble(0.0, 1.0),
                        XRight = random.NextRoundedDouble(),
                        NumberOfHorizontalPoints = random.Next(1, 100),
                        ZTop = random.NextRoundedDouble(2.0, 3.0),
                        ZBottom = random.NextRoundedDouble(0.0, 1.0),
                        NumberOfVerticalPoints = random.Next(1, 100)
                    },
                    CreateZones = random.NextBoolean(),
                    ZoningBoundariesDeterminationType = random.NextEnumValue<MacroStabilityInwardsZoningBoundariesDeterminationType>(),
                    ZoneBoundaryLeft = random.NextRoundedDouble(),
                    ZoneBoundaryRight = random.NextRoundedDouble()
                }
            };

            var registry = new PersistenceRegistry();
            int order = random.Next();

            // Call
            MacroStabilityInwardsCalculationEntity entity = scenario.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);

            MacroStabilityInwardsCalculationEntityTestHelper.AssertCalculationScenarioPropertyValues(scenario, entity);
            Assert.IsNull(entity.SurfaceLineEntity);
            Assert.IsNull(entity.MacroStabilityInwardsStochasticSoilProfileEntity);
            Assert.IsNull(entity.HydraulicLocationEntity);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "Calculation name";
            const string comment = "I am a comment";
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                Name = name,
                Comments =
                {
                    Body = comment
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsCalculationEntity entity = scenario.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            TestHelper.AssertAreEqualButNotSame(name, entity.Name);
            TestHelper.AssertAreEqualButNotSame(comment, entity.Comment);
        }

        [Test]
        public void Create_CalculationWithAlreadyRegisteredHydraulicBoundaryLocation_ReturnsEntityWithHydraulicBoundaryLocationEntity()
        {
            // Setup
            var hydraulicLocation = new TestHydraulicBoundaryLocation();
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicLocation
                }
            };

            var registry = new PersistenceRegistry();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            registry.Register(hydraulicLocationEntity, hydraulicLocation);

            // Call
            MacroStabilityInwardsCalculationEntity entity = scenario.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_CalculationWithAlreadyRegisteredSurfaceLine_ReturnsEntityWithSurfaceLineEntity()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            var registry = new PersistenceRegistry();
            var surfaceLineEntity = new SurfaceLineEntity();
            registry.Register(surfaceLineEntity, surfaceLine);

            // Call
            MacroStabilityInwardsCalculationEntity entity = scenario.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreSame(surfaceLineEntity, entity.SurfaceLineEntity);
        }

        [Test]
        public void Create_CalculationWithAlreadyRegisteredStochasticSoilProfile_ReturnsEntityWithStochasticSoilModelEntity()
        {
            // Setup
            var random = new Random(21);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(),
                                                                                       MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D());
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilProfile = stochasticSoilProfile
                }
            };

            var registry = new PersistenceRegistry();
            var stochasticSoilProfileEntity = new MacroStabilityInwardsStochasticSoilProfileEntity();
            registry.Register(stochasticSoilProfileEntity, stochasticSoilProfile);

            // Call
            MacroStabilityInwardsCalculationEntity entity = scenario.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            MacroStabilityInwardsStochasticSoilProfileEntity expectedStochasticSoilProfileEntity = registry.Get(stochasticSoilProfile);
            Assert.AreSame(expectedStochasticSoilProfileEntity, entity.MacroStabilityInwardsStochasticSoilProfileEntity);
        }

        [Test]
        public void Create_HasMacroStabilityInwardsOutput_ReturnsEntityWithOutputEntity()
        {
            // Setup
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsCalculationEntity entity = scenario.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            MacroStabilityInwardsCalculationOutputEntity outputEntity = entity.MacroStabilityInwardsCalculationOutputEntities.FirstOrDefault();
            Assert.IsNotNull(outputEntity);
            MacroStabilityInwardsCalculationOutputEntityTestHelper.AssertOutputPropertyValues(scenario.Output, outputEntity);
        }
    }
}