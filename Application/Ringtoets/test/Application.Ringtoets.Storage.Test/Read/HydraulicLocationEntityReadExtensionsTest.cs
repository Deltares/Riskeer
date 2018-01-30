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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class HydraulicLocationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((HydraulicLocationEntity) null).Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new HydraulicLocationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_WithCollectorAndEntitiesWithoutOutput_ReturnsHydraulicBoundaryLocationWithPropertiesSetAndEntityRegistered()
        {
            // Setup
            var random = new Random(21);
            long testId = random.Next(0, 400);
            const string testName = "testName";
            double x = random.NextDouble();
            double y = random.NextDouble();

            var entity = new HydraulicLocationEntity
            {
                LocationId = testId,
                Name = testName,
                LocationX = x,
                LocationY = y,
                HydraulicLocationCalculationEntity = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity1 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity2 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity3 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity4 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity5 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity6 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity7 = CreateCalculationEntity(random.Next())
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

            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity7,
                                                       location.DesignWaterLevelCalculation1);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity6,
                                                       location.DesignWaterLevelCalculation2);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity5,
                                                       location.DesignWaterLevelCalculation3);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity4,
                                                       location.DesignWaterLevelCalculation4);

            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity3,
                                                       location.WaveHeightCalculation1);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity2,
                                                       location.WaveHeightCalculation2);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity1,
                                                       location.WaveHeightCalculation3);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity,
                                                       location.WaveHeightCalculation4);

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_WithOutput_ReturnHydraulicBoundaryLocationWithExpectedOutput()
        {
            // Setup
            var random = new Random(21);
            var entity = new HydraulicLocationEntity
            {
                Name = "someName",
                HydraulicLocationCalculationEntity = CreateCalculationEntityWithOutputEntity(random.Next()),
                HydraulicLocationCalculationEntity1 = CreateCalculationEntityWithOutputEntity(random.Next()),
                HydraulicLocationCalculationEntity2 = CreateCalculationEntityWithOutputEntity(random.Next()),
                HydraulicLocationCalculationEntity3 = CreateCalculationEntityWithOutputEntity(random.Next()),
                HydraulicLocationCalculationEntity4 = CreateCalculationEntityWithOutputEntity(random.Next()),
                HydraulicLocationCalculationEntity5 = CreateCalculationEntityWithOutputEntity(random.Next()),
                HydraulicLocationCalculationEntity6 = CreateCalculationEntityWithOutputEntity(random.Next()),
                HydraulicLocationCalculationEntity7 = CreateCalculationEntityWithOutputEntity(random.Next())
            };

            var collector = new ReadConversionCollector();

            // Call
            HydraulicBoundaryLocation location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);

            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity7,
                                                       location.DesignWaterLevelCalculation1);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity6,
                                                       location.DesignWaterLevelCalculation2);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity5,
                                                       location.DesignWaterLevelCalculation3);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity4,
                                                       location.DesignWaterLevelCalculation4);

            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity3,
                                                       location.WaveHeightCalculation1);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity2,
                                                       location.WaveHeightCalculation2);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity1,
                                                       location.WaveHeightCalculation3);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity,
                                                       location.WaveHeightCalculation4);
        }

        [Test]
        public void Read_WithOutputAndIllustrationPoints_ReturnHydraulicBoundaryLocationWithExpectedOutputAndIllustrationPoints()
        {
            // Setup
            var random = new Random(21);
            var entity = new HydraulicLocationEntity
            {
                Name = "someName",
                HydraulicLocationCalculationEntity = CreateCalculationEntityWithOutputAndGeneralResultEntity(random.Next()),
                HydraulicLocationCalculationEntity1 = CreateCalculationEntityWithOutputAndGeneralResultEntity(random.Next()),
                HydraulicLocationCalculationEntity2 = CreateCalculationEntityWithOutputAndGeneralResultEntity(random.Next()),
                HydraulicLocationCalculationEntity3 = CreateCalculationEntityWithOutputAndGeneralResultEntity(random.Next()),
                HydraulicLocationCalculationEntity4 = CreateCalculationEntityWithOutputAndGeneralResultEntity(random.Next()),
                HydraulicLocationCalculationEntity5 = CreateCalculationEntityWithOutputAndGeneralResultEntity(random.Next()),
                HydraulicLocationCalculationEntity6 = CreateCalculationEntityWithOutputAndGeneralResultEntity(random.Next()),
                HydraulicLocationCalculationEntity7 = CreateCalculationEntityWithOutputAndGeneralResultEntity(random.Next())
            };

            var collector = new ReadConversionCollector();

            // Call
            HydraulicBoundaryLocation location = entity.Read(collector);

            // Assert
            Assert.IsNotNull(location);

            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity7,
                                                       location.DesignWaterLevelCalculation1);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity6,
                                                       location.DesignWaterLevelCalculation2);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity5,
                                                       location.DesignWaterLevelCalculation3);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity4,
                                                       location.DesignWaterLevelCalculation4);

            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity3,
                                                       location.WaveHeightCalculation1);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity2,
                                                       location.WaveHeightCalculation2);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity1,
                                                       location.WaveHeightCalculation3);
            AssertHydraulicBoundaryLocationCalculation(entity.HydraulicLocationCalculationEntity,
                                                       location.WaveHeightCalculation4);
        }

        [Test]
        public void Read_SameHydraulicLocationEntityTwice_ReturnSameHydraulicBoundaryLocation()
        {
            // Setup
            var random = new Random(21);
            var entity = new HydraulicLocationEntity
            {
                Name = "A",
                HydraulicLocationCalculationEntity = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity1 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity2 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity3 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity4 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity5 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity6 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity7 = CreateCalculationEntity(random.Next())
            };

            var collector = new ReadConversionCollector();

            // Call
            HydraulicBoundaryLocation location1 = entity.Read(collector);
            HydraulicBoundaryLocation location2 = entity.Read(collector);

            // Assert
            Assert.AreSame(location1, location2);
        }

        private static HydraulicLocationCalculationEntity CreateCalculationEntity(int seed)
        {
            var random = new Random(seed);

            return new HydraulicLocationCalculationEntity
            {
                ShouldIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean())
            };
        }

        private static HydraulicLocationOutputEntity CreateHydraulicOutputEntity(int seed)
        {
            var random = new Random(seed);
            var hydraulicLocationOutputEntity = new HydraulicLocationOutputEntity
            {
                Result = random.NextDouble(),
                TargetProbability = random.NextDouble(),
                TargetReliability = random.NextDouble(),
                CalculatedProbability = random.NextDouble(),
                CalculatedReliability = random.NextDouble(),
                CalculationConvergence = (byte) CalculationConvergence.NotCalculated
            };
            return hydraulicLocationOutputEntity;
        }

        private static HydraulicLocationCalculationEntity CreateCalculationEntityWithOutputEntity(int seed)
        {
            HydraulicLocationCalculationEntity calculationEntity = CreateCalculationEntity(seed);
            calculationEntity.HydraulicLocationOutputEntities.Add(CreateHydraulicOutputEntity(seed));

            return calculationEntity;
        }

        private static HydraulicLocationCalculationEntity CreateCalculationEntityWithOutputAndGeneralResultEntity(int seed)
        {
            var random = new Random(seed);
            var generalResultEntity = new GeneralResultSubMechanismIllustrationPointEntity
            {
                GoverningWindDirectionName = "A wind direction",
                GoverningWindDirectionAngle = random.NextDouble()
            };

            HydraulicLocationOutputEntity hydraulicLocationOutputEntity = CreateHydraulicOutputEntity(seed);
            hydraulicLocationOutputEntity.GeneralResultSubMechanismIllustrationPointEntity = generalResultEntity;

            HydraulicLocationCalculationEntity calculationEntity = CreateCalculationEntity(seed);
            calculationEntity.HydraulicLocationOutputEntities.Add(hydraulicLocationOutputEntity);

            return calculationEntity;
        }

        private static void AssertHydraulicBoundaryLocationCalculation(HydraulicLocationCalculationEntity expected,
                                                                       HydraulicBoundaryLocationCalculation actual)
        {
            Assert.AreEqual(Convert.ToBoolean(expected.ShouldIllustrationPointsBeCalculated),
                            actual.InputParameters.ShouldIllustrationPointsBeCalculated);

            AssertHydraulicBoundaryLocationOutput(expected.HydraulicLocationOutputEntities.SingleOrDefault(),
                                                  actual.Output);
        }

        private static void AssertHydraulicBoundaryLocationOutput(HydraulicLocationOutputEntity expected, HydraulicBoundaryLocationOutput actual)
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

            AssertGeneralResult(expected.GeneralResultSubMechanismIllustrationPointEntity, actual.GeneralResult);
        }

        private static void AssertGeneralResult(GeneralResultSubMechanismIllustrationPointEntity expected,
                                                GeneralResult<TopLevelSubMechanismIllustrationPoint> illustrationPoint)
        {
            if (expected == null)
            {
                Assert.IsNull(illustrationPoint);
                return;
            }

            WindDirection actualGoverningWindDirection = illustrationPoint.GoverningWindDirection;
            Assert.AreEqual(expected.GoverningWindDirectionName, actualGoverningWindDirection.Name);
            Assert.AreEqual(expected.GoverningWindDirectionAngle, actualGoverningWindDirection.Angle,
                            actualGoverningWindDirection.Angle.GetAccuracy());

            Assert.AreEqual(expected.TopLevelSubMechanismIllustrationPointEntities.Count,
                            illustrationPoint.TopLevelIllustrationPoints.Count());
            Assert.AreEqual(expected.StochastEntities.Count, illustrationPoint.Stochasts.Count());
        }
    }
}