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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.HeightStructures;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Read.HeightStructures
{
    [TestFixture]
    public class HeightStructuresCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new HeightStructuresCalculationEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        [TestCase("I have no comments", null, 827364)]
        [TestCase("I have a comment", "I am comment", 231)]
        public void Read_ValidEntity_ReturnCalculation(string name, string comments, int randomSeed)
        {
            // Setup
            var random = new Random(randomSeed);
            var entity = new HeightStructuresCalculationEntity
            {
                Name = name,
                RelevantForScenario = Convert.ToByte(true),
                ScenarioContribution = random.NextDouble(),
                Comments = comments,
                UseForeshore = Convert.ToByte(false),
                UseBreakWater = Convert.ToByte(false),
                StructureNormalOrientation = random.NextDouble(0, 360),
                ModelFactorSuperCriticalFlowMean = random.NextDouble(-9999.9999, 9999.9999),
                AllowedLevelIncreaseStorageMean = random.NextDouble(1e-6, 9999.9999),
                AllowedLevelIncreaseStorageStandardDeviation = random.NextDouble(1e-6, 9999.9999),
                FlowWidthAtBottomProtectionMean = random.NextDouble(1e-6, 9999.9999),
                FlowWidthAtBottomProtectionStandardDeviation = random.NextDouble(1e-6, 9999.9999),
                CriticalOvertoppingDischargeMean = random.NextDouble(1e-6, 9999.9999),
                CriticalOvertoppingDischargeCoefficientOfVariation = random.NextDouble(1e-6, 9999.9999),
                FailureProbabilityStructureWithErosion = random.NextDouble(),
                WidthFlowAperturesMean = random.NextDouble(1e-6, 9999.9999),
                WidthFlowAperturesStandardDeviation = random.NextDouble(1e-6, 9999.9999),
                StormDurationMean = random.NextDouble(1e-6, 9999.9999),
                LevelCrestStructureMean = random.NextDouble(1e-6, 9999.9999),
                LevelCrestStructureStandardDeviation = random.NextDouble(1e-6, 9999.9999),
                DeviationWaveDirection = random.NextDouble(-360, 360)
            };

            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculationScenario<HeightStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, calculation.Name);
            Assert.AreEqual(Convert.ToBoolean(entity.RelevantForScenario), calculation.IsRelevant);
            AssertRoundedDouble(entity.ScenarioContribution, calculation.Contribution);
            Assert.AreEqual(comments, calculation.Comments.Body);

            HeightStructuresInput input = calculation.InputParameters;
            Assert.IsFalse(input.UseForeshore);
            Assert.IsFalse(input.UseBreakWater);

            AssertRoundedDouble(entity.StructureNormalOrientation, input.StructureNormalOrientation);
            AssertRoundedDouble(entity.ModelFactorSuperCriticalFlowMean, input.ModelFactorSuperCriticalFlow.Mean);
            AssertRoundedDouble(entity.AllowedLevelIncreaseStorageMean, input.AllowedLevelIncreaseStorage.Mean);
            AssertRoundedDouble(entity.AllowedLevelIncreaseStorageStandardDeviation, input.AllowedLevelIncreaseStorage.StandardDeviation);
            AssertRoundedDouble(entity.FlowWidthAtBottomProtectionMean, input.FlowWidthAtBottomProtection.Mean);
            AssertRoundedDouble(entity.FlowWidthAtBottomProtectionStandardDeviation, input.FlowWidthAtBottomProtection.StandardDeviation);
            AssertRoundedDouble(entity.CriticalOvertoppingDischargeMean, input.CriticalOvertoppingDischarge.Mean);
            AssertRoundedDouble(entity.CriticalOvertoppingDischargeCoefficientOfVariation, input.CriticalOvertoppingDischarge.CoefficientOfVariation);
            Assert.AreEqual(entity.FailureProbabilityStructureWithErosion, input.FailureProbabilityStructureWithErosion);
            AssertRoundedDouble(entity.WidthFlowAperturesMean, input.WidthFlowApertures.Mean);
            AssertRoundedDouble(entity.WidthFlowAperturesStandardDeviation, input.WidthFlowApertures.StandardDeviation);
            AssertRoundedDouble(entity.StormDurationMean, input.StormDuration.Mean);
            AssertRoundedDouble(entity.LevelCrestStructureMean, input.LevelCrestStructure.Mean);
            AssertRoundedDouble(entity.LevelCrestStructureStandardDeviation, input.LevelCrestStructure.StandardDeviation);
            AssertRoundedDouble(entity.DeviationWaveDirection, input.DeviationWaveDirection);

            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.IsNull(input.ForeshoreProfile);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.IsNull(input.Structure);
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void Read_EntityWithNullParameters_ReturnHeightStructuresCalculationWithInputParametersNaN()
        {
            // Setup
            var entity = new HeightStructuresCalculationEntity
            {
                ScenarioContribution = null,
                StructureNormalOrientation = null,
                ModelFactorSuperCriticalFlowMean = null,
                AllowedLevelIncreaseStorageMean = null,
                AllowedLevelIncreaseStorageStandardDeviation = null,
                FlowWidthAtBottomProtectionMean = null,
                FlowWidthAtBottomProtectionStandardDeviation = null,
                CriticalOvertoppingDischargeMean = null,
                CriticalOvertoppingDischargeCoefficientOfVariation = null,
                WidthFlowAperturesMean = null,
                WidthFlowAperturesStandardDeviation = null,
                StormDurationMean = null,
                LevelCrestStructureMean = null,
                LevelCrestStructureStandardDeviation = null,
                DeviationWaveDirection = null
            };
            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculationScenario<HeightStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.IsNaN(calculation.Contribution);

            HeightStructuresInput inputParameters = calculation.InputParameters;
            Assert.IsNaN(inputParameters.StructureNormalOrientation);
            Assert.IsNaN(inputParameters.ModelFactorSuperCriticalFlow.Mean);
            Assert.IsNaN(inputParameters.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(inputParameters.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNaN(inputParameters.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(inputParameters.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNaN(inputParameters.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(inputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.WidthFlowApertures.Mean);
            Assert.IsNaN(inputParameters.WidthFlowApertures.StandardDeviation);
            Assert.IsNaN(inputParameters.StormDuration.Mean);
            Assert.IsNaN(inputParameters.LevelCrestStructure.Mean);
            Assert.IsNaN(inputParameters.LevelCrestStructure.StandardDeviation);
            Assert.IsNaN(inputParameters.DeviationWaveDirection);
        }

        [Test]
        public void Read_EntityWithStructureEntity_ReturnCalculationWithStructure()
        {
            // Setup
            HeightStructure structure = new TestHeightStructure();
            var structureEntity = new HeightStructureEntity();
            var entity = new HeightStructuresCalculationEntity
            {
                HeightStructureEntity = structureEntity,
            };
            var collector = new ReadConversionCollector();
            collector.Read(structureEntity, structure);

            // Call
            StructuresCalculationScenario<HeightStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(structure, calculation.InputParameters.Structure);
        }

        [Test]
        public void Read_EntityWithHydraulicLocationEntity_ReturnCalculationWithHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2, 3);
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var entity = new HeightStructuresCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            StructuresCalculationScenario<HeightStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        [TestCase(true, BreakWaterType.Dam, 123)]
        [TestCase(false, BreakWaterType.Wall, 456)]
        public void Read_EntityWithForeshoreProfile_ReturnCalculation(bool flagUsage, BreakWaterType type, int randomSeed)
        {
            // Setup
            var random = new Random(randomSeed);

            double breakWaterHeight = random.NextDouble();
            var points = new[]
            {
                new Point2D(0, 0)
            };
            string pointXml = new Point2DCollectionXmlSerializer().ToXml(points);
            var foreshoreEntity = new ForeshoreProfileEntity
            {
                Id = "id",
                BreakWaterHeight = breakWaterHeight,
                BreakWaterType = Convert.ToByte(type),
                GeometryXml = pointXml
            };

            var entity = new HeightStructuresCalculationEntity
            {
                UseForeshore = Convert.ToByte(flagUsage),
                UseBreakWater = Convert.ToByte(!flagUsage),
                ForeshoreProfileEntity = foreshoreEntity,
                BreakWaterType = Convert.ToByte(type),
                BreakWaterHeight = breakWaterHeight
            };

            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculation<HeightStructuresInput> calculation = entity.Read(collector);

            // Assert
            HeightStructuresInput input = calculation.InputParameters;
            Assert.AreEqual(flagUsage, input.UseForeshore);
            Assert.AreEqual(!flagUsage, input.UseBreakWater);
            Assert.AreEqual(type, input.BreakWater.Type);
            Assert.AreEqual(breakWaterHeight, input.BreakWater.Height, input.BreakWater.Height.GetAccuracy());
            CollectionAssert.AreEqual(points, input.ForeshoreProfile.Geometry);
            Assert.IsNotNull(input.ForeshoreProfile);
        }

        [Test]
        public void Read_ValidEntityWithOutputEntity_ReturnCalculationWithOutput()
        {
            // Setup
            var entity = new HeightStructuresCalculationEntity
            {
                HeightStructuresOutputEntities =
                {
                    new HeightStructuresOutputEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculation<HeightStructuresInput> calculation = entity.Read(collector);

            // Assert
            StructuresOutput calculationOutput = calculation.Output;
            Assert.IsNaN(calculationOutput.Reliability);
            Assert.IsFalse(calculationOutput.HasGeneralResult);
        }

        private static void AssertRoundedDouble(double? entityValue, RoundedDouble roundedDouble)
        {
            Assert.AreEqual((RoundedDouble) entityValue.ToNullAsNaN(), roundedDouble, roundedDouble.GetAccuracy());
        }
    }
}