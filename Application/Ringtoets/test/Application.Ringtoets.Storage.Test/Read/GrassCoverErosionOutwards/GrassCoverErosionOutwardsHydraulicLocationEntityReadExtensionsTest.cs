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
using Application.Ringtoets.Storage.Read.GrassCoverErosionOutwards;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read.GrassCoverErosionOutwards
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicLocationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = (GrassCoverErosionOutwardsHydraulicLocationEntity) null;
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => entity.Read(collector);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_WithCollector_ReturnsHydraulicBoundaryLocationWithPropertiesSetAndEntityRegistered()
        {
            // Setup
            var random = new Random(21);
            long testId = random.Next(0, 400);
            const string testName = "testName";
            double x = random.NextDouble();
            double y = random.NextDouble();
            bool shouldDesignWaterLevelIllustrationPointsBeCalculated = random.NextBoolean();
            bool shouldWaveHeightIllustrationPointsBeCalculated = random.NextBoolean();
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity
            {
                LocationId = testId,
                Name = testName,
                LocationX = x,
                LocationY = y,
                ShouldDesignWaterLevelIllustrationPointsBeCalculated = Convert.ToByte(shouldDesignWaterLevelIllustrationPointsBeCalculated),
                ShouldWaveHeightIllustrationPointsBeCalculated = Convert.ToByte(shouldWaveHeightIllustrationPointsBeCalculated)
            };

            var collector = new ReadConversionCollector();

            // Call
            HydraulicBoundaryLocation location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(testId, location.Id);
            Assert.AreEqual(testName, location.Name);
            Assert.AreEqual(x, location.Location.X, 1e-6);
            Assert.AreEqual(y, location.Location.Y, 1e-6);

            HydraulicBoundaryLocationCalculation designWaterLevelCalculation = location.DesignWaterLevelCalculation;
            AssertShouldIllustrationPointsBeCalculated(shouldDesignWaterLevelIllustrationPointsBeCalculated, designWaterLevelCalculation);
            Assert.IsFalse(designWaterLevelCalculation.HasOutput);

            HydraulicBoundaryLocationCalculation waveHeightCalculation = location.WaveHeightCalculation;
            AssertShouldIllustrationPointsBeCalculated(shouldWaveHeightIllustrationPointsBeCalculated, waveHeightCalculation);
            Assert.IsFalse(waveHeightCalculation.HasOutput);

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_WithOutput_ReturnLocationWithOutput()
        {
            // Setup
            var random = new Random(21);
            double designWaterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            var designWaterLevelOutputEntity = new GrassCoverErosionOutwardsHydraulicLocationOutputEntity
            {
                HydraulicLocationOutputType = (byte) HydraulicLocationOutputType.DesignWaterLevel,
                Result = designWaterLevel,
                TargetProbability = random.NextDouble(),
                TargetReliability = random.NextDouble(),
                CalculatedProbability = random.NextDouble(),
                CalculatedReliability = random.NextDouble(),
                CalculationConvergence = (byte) CalculationConvergence.NotCalculated
            };
            var waveheightOutputEntity = new GrassCoverErosionOutwardsHydraulicLocationOutputEntity
            {
                HydraulicLocationOutputType = (byte) HydraulicLocationOutputType.WaveHeight,
                Result = waveHeight,
                TargetProbability = random.NextDouble(),
                TargetReliability = random.NextDouble(),
                CalculatedProbability = random.NextDouble(),
                CalculatedReliability = random.NextDouble(),
                CalculationConvergence = (byte) CalculationConvergence.NotCalculated
            };
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity
            {
                Name = "someName",
                GrassCoverErosionOutwardsHydraulicLocationOutputEntities =
                {
                    designWaterLevelOutputEntity,
                    waveheightOutputEntity
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            HydraulicBoundaryLocation location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(designWaterLevel, location.DesignWaterLevel, location.DesignWaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, location.WaveHeight, location.WaveHeight.GetAccuracy());
            AssertHydraulicBoundaryLocationOutput(designWaterLevelOutputEntity, location.DesignWaterLevelCalculation.Output);
            AssertHydraulicBoundaryLocationOutput(waveheightOutputEntity, location.WaveHeightCalculation.Output);
        }

        [Test]
        public void Read_WithOutputAndIllustrationPoints_ReturnHydraulicBoundaryLocationWithExpectedOutputAndIllustrationPoints()
        {
            // Setup
            var random = new Random(21);

            const string windDirectionName = "Some wind direction";
            double windDirectionAngle = random.NextDouble();
            var designWaterLevelIllustrationPointEntity = new GeneralResultSubMechanismIllustrationPointEntity
            {
                GoverningWindDirectionName = windDirectionName,
                GoverningWindDirectionAngle = windDirectionAngle
            };
            var waveHeightIllustrationPointEntity = new GeneralResultSubMechanismIllustrationPointEntity
            {
                GoverningWindDirectionName = windDirectionName,
                GoverningWindDirectionAngle = windDirectionAngle
            };

            double designWaterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            var designWaterLevelOutputEntity = new GrassCoverErosionOutwardsHydraulicLocationOutputEntity
            {
                HydraulicLocationOutputType = (byte) HydraulicLocationOutputType.DesignWaterLevel,
                Result = designWaterLevel,
                TargetProbability = random.NextDouble(),
                TargetReliability = random.NextDouble(),
                CalculatedProbability = random.NextDouble(),
                CalculatedReliability = random.NextDouble(),
                CalculationConvergence = (byte) CalculationConvergence.NotCalculated,
                GeneralResultSubMechanismIllustrationPointEntity = designWaterLevelIllustrationPointEntity
            };
            var waveheightOutputEntity = new GrassCoverErosionOutwardsHydraulicLocationOutputEntity
            {
                HydraulicLocationOutputType = (byte) HydraulicLocationOutputType.WaveHeight,
                Result = waveHeight,
                TargetProbability = random.NextDouble(),
                TargetReliability = random.NextDouble(),
                CalculatedProbability = random.NextDouble(),
                CalculatedReliability = random.NextDouble(),
                CalculationConvergence = (byte) CalculationConvergence.NotCalculated,
                GeneralResultSubMechanismIllustrationPointEntity = waveHeightIllustrationPointEntity
            };

            bool shouldDesignWaterLevelIllustrationPointsBeCalculated = random.NextBoolean();
            bool shouldWaveHeightIllustrationPointsBeCalculated = random.NextBoolean();
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity
            {
                Name = "someName",
                ShouldDesignWaterLevelIllustrationPointsBeCalculated = Convert.ToByte(shouldDesignWaterLevelIllustrationPointsBeCalculated),
                ShouldWaveHeightIllustrationPointsBeCalculated = Convert.ToByte(shouldWaveHeightIllustrationPointsBeCalculated),
                GrassCoverErosionOutwardsHydraulicLocationOutputEntities =
                {
                    designWaterLevelOutputEntity,
                    waveheightOutputEntity
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            HydraulicBoundaryLocation location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);
            Assert.AreEqual(designWaterLevel, location.DesignWaterLevel, location.DesignWaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, location.WaveHeight, location.WaveHeight.GetAccuracy());

            HydraulicBoundaryLocationCalculation designWaterLevelCalculation = location.DesignWaterLevelCalculation;
            AssertShouldIllustrationPointsBeCalculated(shouldDesignWaterLevelIllustrationPointsBeCalculated, designWaterLevelCalculation);
            AssertHydraulicBoundaryLocationOutput(designWaterLevelOutputEntity, location.DesignWaterLevelCalculation.Output);

            HydraulicBoundaryLocationCalculation waveHeightCalculation = location.WaveHeightCalculation;
            AssertShouldIllustrationPointsBeCalculated(shouldWaveHeightIllustrationPointsBeCalculated, waveHeightCalculation);
            AssertHydraulicBoundaryLocationOutput(waveheightOutputEntity, location.WaveHeightCalculation.Output);
        }

        [Test]
        public void Read_SameHydraulicLocationEntityTwice_ReturnSameHydraulicBoundaryLocation()
        {
            // Setup
            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            HydraulicBoundaryLocation location1 = entity.Read(collector);
            HydraulicBoundaryLocation location2 = entity.Read(collector);

            // Assert
            Assert.AreSame(location1, location2);
        }

        private static void AssertShouldIllustrationPointsBeCalculated(bool shouldIllustrationPointsBeCalculated,
                                                                       HydraulicBoundaryLocationCalculation calculation)
        {
            Assert.AreEqual(shouldIllustrationPointsBeCalculated, calculation.InputParameters.ShouldIllustrationPointsBeCalculated);
        }

        private static void AssertHydraulicBoundaryLocationOutput(IHydraulicLocationOutputEntity expected, HydraulicBoundaryLocationOutput actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }
            Assert.IsNotNull(expected.Result);
            Assert.AreEqual((RoundedDouble) expected.Result, actual.Result, actual.Result.GetAccuracy());
            Assert.IsNotNull(expected.TargetReliability);
            Assert.AreEqual((RoundedDouble) expected.TargetReliability, actual.TargetReliability, actual.TargetReliability.GetAccuracy());
            Assert.IsNotNull(expected.TargetProbability);
            Assert.AreEqual(expected.TargetProbability, actual.TargetProbability);
            Assert.IsNotNull(expected.CalculatedReliability);
            Assert.AreEqual((RoundedDouble) expected.CalculatedReliability, actual.CalculatedReliability, actual.CalculatedReliability.GetAccuracy());
            Assert.IsNotNull(expected.CalculatedProbability);
            Assert.AreEqual(expected.CalculatedProbability, actual.CalculatedProbability);
            Assert.AreEqual((CalculationConvergence) expected.CalculationConvergence, actual.CalculationConvergence);
        }
    }
}