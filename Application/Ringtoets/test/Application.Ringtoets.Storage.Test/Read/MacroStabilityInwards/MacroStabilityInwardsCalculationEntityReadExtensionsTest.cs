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

using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.MacroStabilityInwards;
using Application.Ringtoets.Storage.Serializers;
using Application.Ringtoets.Storage.TestUtil.MacroStabilityInwards;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new MacroStabilityInwardsCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        public void Read_EntityWithValidValues_ReturnsCalculationScenarioWithExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var entity = new MacroStabilityInwardsCalculationEntity
            {
                Name = "Calculation name",
                Comment = "Comment here",
                AssessmentLevel = random.NextDouble(),
                UseAssessmentLevelManualInput = Convert.ToByte(true),
                ScenarioContribution = random.NextDouble(),
                RelevantForScenario = Convert.ToByte(random.NextBoolean()),
                SlipPlaneMinimumDepth = random.NextDouble(),
                SlipPlaneMinimumLength = random.NextDouble(),
                MaximumSliceWidth = random.NextDouble(),
                MoveGrid = Convert.ToByte(random.NextBoolean()),
                DikeSoilScenario = Convert.ToByte(random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>()),
                WaterLevelRiverAverage = random.NextDouble(),
                DrainageConstructionPresent = Convert.ToByte(random.NextBoolean()),
                DrainageConstructionCoordinateX = random.NextDouble(),
                DrainageConstructionCoordinateZ = random.NextDouble(),
                MinimumLevelPhreaticLineAtDikeTopRiver = random.NextDouble(),
                MinimumLevelPhreaticLineAtDikeTopPolder = random.NextDouble(),
                LocationInputExtremeWaterLevelPolder = random.NextDouble(),
                LocationInputExtremeUseDefaultOffsets = Convert.ToByte(random.NextBoolean()),
                LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver = random.NextDouble(),
                LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder = random.NextDouble(),
                LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside = random.NextDouble(),
                LocationInputExtremePhreaticLineOffsetDikeToeAtPolder = random.NextDouble(),
                LocationInputExtremePenetrationLength = random.NextDouble(),
                LocationInputDailyWaterLevelPolder = random.NextDouble(),
                LocationInputDailyUseDefaultOffsets = Convert.ToByte(random.NextBoolean()),
                LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver = random.NextDouble(),
                LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder = random.NextDouble(),
                LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside = random.NextDouble(),
                LocationInputDailyPhreaticLineOffsetDikeToeAtPolder = random.NextDouble(),
                AdjustPhreaticLine3And4ForUplift = Convert.ToByte(random.NextBoolean()),
                LeakageLengthOutwardsPhreaticLine4 = random.NextDouble(),
                LeakageLengthInwardsPhreaticLine4 = random.NextDouble(),
                LeakageLengthOutwardsPhreaticLine3 = random.NextDouble(),
                LeakageLengthInwardsPhreaticLine3 = random.NextDouble(),
                PiezometricHeadPhreaticLine2Outwards = random.NextDouble(),
                PiezometricHeadPhreaticLine2Inwards = random.NextDouble(),
                GridDeterminationType = Convert.ToByte(random.NextBoolean()),
                TangentLineDeterminationType = Convert.ToByte(random.NextBoolean()),
                TangentLineZTop = random.NextDouble(),
                TangentLineZBottom = random.NextDouble(),
                TangentLineNumber = random.Next(1, 50),
                LeftGridXLeft = random.NextDouble(),
                LeftGridXRight = random.NextDouble(),
                LeftGridNrOfHorizontalPoints = random.Next(),
                LeftGridZTop = random.NextDouble(),
                LeftGridZBottom = random.NextDouble(),
                LeftGridNrOfVerticalPoints = random.Next(),
                RightGridXLeft = random.NextDouble(),
                RightGridXRight = random.NextDouble(),
                RightGridNrOfHorizontalPoints = random.Next(),
                RightGridZTop = random.NextDouble(),
                RightGridZBottom = random.NextDouble(),
                RightGridNrOfVerticalPoints = random.Next(),
                CreateZones = Convert.ToByte(random.NextBoolean())
            };

            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            Assert.IsNull(calculation.SemiProbabilisticOutput);
            Assert.IsNull(calculation.Output);
            Assert.AreEqual(entity.Name, calculation.Name);
            Assert.AreEqual(entity.Comment, calculation.Comments.Body);

            MacroStabilityInwardsInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            Assert.IsNull(inputParameters.SurfaceLine);

            MacroStabilityInwardsCalculationEntityTestHelper.AssertCalculationScenarioPropertyValues(calculation, entity);
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnsCalculationScenarioWithNaNValues()
        {
            // Setup
            var entity = new MacroStabilityInwardsCalculationEntity
            {
                TangentLineNumber = 1
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            Assert.IsNull(calculation.SemiProbabilisticOutput);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.Comments.Body);

            MacroStabilityInwardsInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            Assert.IsNull(inputParameters.SurfaceLine);

            MacroStabilityInwardsCalculationEntityTestHelper.AssertCalculationScenarioPropertyValues(calculation, entity);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationEntity_ReturnsCalculationScenarioWithInputObjectWithLocationSet()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicLocationEntity = new HydraulicLocationEntity();

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            var entity = new MacroStabilityInwardsCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                TangentLineNumber = 1
            };

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithSurfaceLineEntity_ReturnsCalculationScenarioWithInputObjectWithSurfaceLineSet()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var surfaceLineEntity = new SurfaceLineEntity();

            var collector = new ReadConversionCollector();
            collector.Read(surfaceLineEntity, surfaceLine);

            var entity = new MacroStabilityInwardsCalculationEntity
            {
                SurfaceLineEntity = surfaceLineEntity,
                TangentLineNumber = 1
            };

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(surfaceLine, calculation.InputParameters.SurfaceLine);
        }

        [Test]
        public void Read_EntityWithStochasticSoilModel_ReturnCalculationScenarioWithInputObjectWithStochasticSoilModelPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();
            var stochasticSoilModelEntity = new StochasticSoilModelEntity();

            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(),
                                                                                       MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D());
            var stochasticSoilProfileEntity = new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                StochasticSoilModelEntity = stochasticSoilModelEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(stochasticSoilModelEntity, stochasticSoilModel);
            collector.Read(stochasticSoilProfileEntity, stochasticSoilProfile);

            var entity = new MacroStabilityInwardsCalculationEntity
            {
                MacroStabilityInwardsStochasticSoilProfileEntity = stochasticSoilProfileEntity,
                TangentLineNumber = 1
            };

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            MacroStabilityInwardsInput inputParameters = calculation.InputParameters;
            Assert.AreSame(stochasticSoilModel, inputParameters.StochasticSoilModel);
            Assert.AreSame(stochasticSoilProfile, inputParameters.StochasticSoilProfile);
        }

        [Test]
        public void Read_EntityWithSemiProbabilisticOutput_ReturnsCalculationScenarioWithSemiProbabilisticOutput()
        {
            // Setup
            var tangentLines = new double[0];
            var slices = new MacroStabilityInwardsSlice[0];

            var calculationOutputEntity = new MacroStabilityInwardsCalculationOutputEntity
            {
                SlipPlaneTangentLinesXml = new TangentLinesXmlSerializer().ToXml(tangentLines),
                SlidingCurveSliceXML = new MacroStabilityInwardsSliceXmlSerializer().ToXml(slices)
            };
            var entity = new MacroStabilityInwardsCalculationEntity
            {
                TangentLineNumber = 1,
                MacroStabilityInwardsCalculationOutputEntities =
                {
                    calculationOutputEntity
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            MacroStabilityInwardsOutput output = calculation.Output;
            Assert.IsNotNull(output);
            MacroStabilityInwardsCalculationOutputEntityTestHelper.AssertOutputPropertyValues(output, calculationOutputEntity);
        }

        [Test]
        public void Read_EntityWithOutput_ReturnsCalculationScenarioWithOutput()
        {
            // Setup
            var entity = new MacroStabilityInwardsCalculationEntity
            {
                TangentLineNumber = 1,
                MacroStabilityInwardsSemiProbabilisticOutputEntities =
                {
                    new MacroStabilityInwardsSemiProbabilisticOutputEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            MacroStabilityInwardsSemiProbabilisticOutput output = calculation.SemiProbabilisticOutput;
            Assert.IsNotNull(output);

            Assert.IsNaN(output.FactorOfStability);
            Assert.IsNaN(output.RequiredProbability);
            Assert.IsNaN(output.RequiredReliability);
            Assert.IsNaN(output.MacroStabilityInwardsFactorOfSafety);
            Assert.IsNaN(output.MacroStabilityInwardsProbability);
            Assert.IsNaN(output.MacroStabilityInwardsReliability);
        }
    }
}