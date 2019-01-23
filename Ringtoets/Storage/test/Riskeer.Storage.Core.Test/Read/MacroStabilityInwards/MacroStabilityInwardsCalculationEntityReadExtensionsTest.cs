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
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.MacroStabilityInwards;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsCalculationEntity entity = CreateValidCalculationEntity();

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
                UseAssessmentLevelManualInput = Convert.ToByte(random.NextBoolean()),
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
                GridDeterminationType = Convert.ToByte(random.NextEnumValue<MacroStabilityInwardsGridDeterminationType>()),
                TangentLineDeterminationType = Convert.ToByte(random.NextEnumValue<MacroStabilityInwardsTangentLineDeterminationType>()),
                TangentLineZTop = random.NextDouble(2.0, 3.0),
                TangentLineZBottom = random.NextDouble(0.0, 1.0),
                TangentLineNumber = random.Next(1, 50),
                LeftGridXLeft = random.NextDouble(0.0, 1.0),
                LeftGridXRight = random.NextDouble(2.0, 3.0),
                LeftGridNrOfHorizontalPoints = random.Next(1, 100),
                LeftGridZTop = random.NextDouble(2.0, 3.0),
                LeftGridZBottom = random.NextDouble(0.0, 1.0),
                LeftGridNrOfVerticalPoints = random.Next(1, 100),
                RightGridXLeft = random.NextDouble(0.0, 1.0),
                RightGridXRight = random.NextDouble(2.0, 3.0),
                RightGridNrOfHorizontalPoints = random.Next(1, 100),
                RightGridZTop = random.NextDouble(2.0, 3.0),
                RightGridZBottom = random.NextDouble(0.0, 1.0),
                RightGridNrOfVerticalPoints = random.Next(1, 100),
                CreateZones = Convert.ToByte(random.NextBoolean()),
                ZoningBoundariesDeterminationType = Convert.ToByte(random.NextEnumValue<MacroStabilityInwardsZoningBoundariesDeterminationType>()),
                ZoneBoundaryLeft = random.NextDouble(),
                ZoneBoundaryRight = random.NextDouble()
            };

            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
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
            MacroStabilityInwardsCalculationEntity entity = CreateValidCalculationEntity();
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
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

            MacroStabilityInwardsCalculationEntity entity = CreateValidCalculationEntity();
            entity.HydraulicLocationEntity = hydraulicLocationEntity;

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

            MacroStabilityInwardsCalculationEntity entity = CreateValidCalculationEntity();
            entity.SurfaceLineEntity = surfaceLineEntity;

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

            MacroStabilityInwardsCalculationEntity entity = CreateValidCalculationEntity();
            entity.MacroStabilityInwardsStochasticSoilProfileEntity = stochasticSoilProfileEntity;

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            MacroStabilityInwardsInput inputParameters = calculation.InputParameters;
            Assert.AreSame(stochasticSoilModel, inputParameters.StochasticSoilModel);
            Assert.AreSame(stochasticSoilProfile, inputParameters.StochasticSoilProfile);
        }

        [Test]
        public void Read_EntityWithOutput_ReturnsCalculationScenarioWithOutput()
        {
            // Setup
            var random = new Random(31);
            var tangentLines = new RoundedDouble[0];
            var slices = new MacroStabilityInwardsSlice[0];

            var calculationOutputEntity = new MacroStabilityInwardsCalculationOutputEntity
            {
                SlipPlaneTangentLinesXml = new TangentLineCollectionXmlSerializer().ToXml(tangentLines),
                SlidingCurveSliceXML = new MacroStabilityInwardsSliceCollectionXmlSerializer().ToXml(slices),
                SlipPlaneLeftGridNrOfHorizontalPoints = random.Next(1, 100),
                SlipPlaneLeftGridNrOfVerticalPoints = random.Next(1, 100),
                SlipPlaneRightGridNrOfHorizontalPoints = random.Next(1, 100),
                SlipPlaneRightGridNrOfVerticalPoints = random.Next(1, 100)
            };

            MacroStabilityInwardsCalculationEntity entity = CreateValidCalculationEntity();
            entity.MacroStabilityInwardsCalculationOutputEntities.Add(calculationOutputEntity);

            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsCalculationScenario calculation = entity.Read(collector);

            // Assert
            MacroStabilityInwardsOutput output = calculation.Output;
            Assert.IsNotNull(output);
            MacroStabilityInwardsCalculationOutputEntityTestHelper.AssertOutputPropertyValues(output, calculationOutputEntity);
        }

        private static MacroStabilityInwardsCalculationEntity CreateValidCalculationEntity()
        {
            var random = new Random(21);
            return new MacroStabilityInwardsCalculationEntity
            {
                TangentLineNumber = 1,
                LeftGridNrOfHorizontalPoints = random.Next(1, 100),
                LeftGridNrOfVerticalPoints = random.Next(1, 100),
                RightGridNrOfHorizontalPoints = random.Next(1, 100),
                RightGridNrOfVerticalPoints = random.Next(1, 100)
            };
        }
    }
}