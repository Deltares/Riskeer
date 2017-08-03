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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class HydraulicBoundaryLocationOutputCreateExtensionsTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateHydraulicLocationOutputEntity_WithValidParameters_ReturnsHydraulicLocationEntityWithOutputSet(
            bool withIllustrationPoints)
        {
            // Setup
            var random = new Random(21);

            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = withIllustrationPoints
                                                                                     ? GetGeneralResult()
                                                                                     : null;
            var output = new HydraulicBoundaryLocationOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), random.NextEnumValue<CalculationConvergence>(), generalResult);

            var outputType = random.NextEnumValue<HydraulicLocationOutputType>();

            // Call
            var entity = output.Create<HydraulicLocationOutputEntity>(outputType);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((byte) outputType, entity.HydraulicLocationOutputType);
            Assert.AreEqual(output.Result, entity.Result, output.Result.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);

            AssertGeneralResult(output.GeneralResult,
                                entity.GeneralResultSubMechanismIllustrationPointEntity);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateHydraulicLocationOutputEntity_WithNaNParameters_ReturnsHydraulicLocationEntityWithOutputNaN(
            bool withIllustrationPoints)
        {
            // Setup
            var random = new Random(21);
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = withIllustrationPoints
                                                                                     ? GetGeneralResult()
                                                                                     : null;
            var output = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN, double.NaN,
                                                             double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>(), generalResult);

            var outputType = random.NextEnumValue<HydraulicLocationOutputType>();

            // Call
            var entity = output.Create<HydraulicLocationOutputEntity>(outputType);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((byte) outputType, entity.HydraulicLocationOutputType);
            Assert.IsNull(entity.Result);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);

            AssertGeneralResult(output.GeneralResult,
                                entity.GeneralResultSubMechanismIllustrationPointEntity);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateGrassCoverErosionOutwardsHydraulicLocationOutputEntity_WithValidParameters_ReturnsHydraulicLocationEntityWithOutputSet(
            bool withIllustrationPoints)
        {
            // Setup
            var random = new Random(21);

            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = withIllustrationPoints
                                                                                     ? GetGeneralResult()
                                                                                     : null;
            var output = new HydraulicBoundaryLocationOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), random.NextEnumValue<CalculationConvergence>(), generalResult);

            var outputType = random.NextEnumValue<HydraulicLocationOutputType>();

            // Call
            var entity = output.Create<GrassCoverErosionOutwardsHydraulicLocationOutputEntity>(outputType);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((byte) outputType, entity.HydraulicLocationOutputType);
            Assert.AreEqual(output.Result, entity.Result, output.Result.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);

            AssertGeneralResult(output.GeneralResult,
                                entity.GeneralResultSubMechanismIllustrationPointEntity);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateGrassCoverErosionOutwardsHydraulicLocationOutputEntity_WithNaNParameters_ReturnsHydraulicLocationEntityWithOutputNaN(
            bool withIllustrationPoints)
        {
            // Setup
            var random = new Random(21);

            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = withIllustrationPoints
                                                                                     ? GetGeneralResult()
                                                                                     : null;
            var output = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN, double.NaN,
                                                             double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>(), generalResult);

            var outputType = random.NextEnumValue<HydraulicLocationOutputType>();

            // Call
            var entity = output.Create<GrassCoverErosionOutwardsHydraulicLocationOutputEntity>(
                outputType);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((byte) outputType, entity.HydraulicLocationOutputType);
            Assert.IsNull(entity.Result);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);

            AssertGeneralResult(output.GeneralResult,
                                entity.GeneralResultSubMechanismIllustrationPointEntity);
        }

        private static GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResult()
        {
            var random = new Random(55);
            var generalResult = new GeneralResult<TopLevelSubMechanismIllustrationPoint>(
                new WindDirection("SSE", random.NextDouble()),
                new[]
                {
                    new Stochast("stochastOne", random.NextDouble(), random.NextDouble())
                },
                new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                              "IllustrationPointTwo",
                                                              new TestSubMechanismIllustrationPoint())
                });

            return generalResult;
        }

        private static void AssertGeneralResult(GeneralResult<TopLevelSubMechanismIllustrationPoint> illustrationPoint,
                                                GeneralResultSubMechanismIllustrationPointEntity entity)
        {
            if (illustrationPoint == null)
            {
                Assert.IsNull(entity);
                return;
            }

            Assert.IsNotNull(entity);
            WindDirection governingWindDirection = illustrationPoint.GoverningWindDirection;
            TestHelper.AssertAreEqualButNotSame(governingWindDirection.Name, entity.GoverningWindDirectionName);
            Assert.AreEqual(governingWindDirection.Angle, entity.GoverningWindDirectionAngle,
                            governingWindDirection.Angle.GetAccuracy());

            AssertStochastEntities(illustrationPoint.Stochasts.ToArray(), entity.StochastEntities.ToArray());
            Assert.AreEqual(illustrationPoint.TopLevelIllustrationPoints.Count(),
                            entity.TopLevelSubMechanismIllustrationPointEntities.Count);
        }

        private static void AssertStochastEntities(Stochast[] stochasts, StochastEntity[] stochastEntities)
        {
            Assert.AreEqual(stochasts.Length, stochastEntities.Length);
            for (var i = 0; i < stochasts.Length; i++)
            {
                AssertStochastEntity(stochasts[i], stochastEntities[i], i);
            }
        }

        private static void AssertStochastEntity(Stochast stochast,
                                                 StochastEntity stochastEntity,
                                                 int expectedOrder)
        {
            TestHelper.AssertAreEqualButNotSame(stochast.Name, stochastEntity.Name);
            Assert.AreEqual(stochast.Duration, stochastEntity.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(stochast.Alpha, stochastEntity.Alpha, stochast.Alpha.GetAccuracy());

            Assert.AreEqual(expectedOrder, stochastEntity.Order);
        }
    }
}