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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Storage.Core.Create.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultCreateExtensionsTest
    {
        [Test]
        public void CreateGeneralResultSubMechanismIllustrationPointEntity_GeneralResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((GeneralResult<TopLevelSubMechanismIllustrationPoint>) null)
                .CreateGeneralResultSubMechanismIllustrationPointEntity();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("generalResult", exception.ParamName);
        }

        [Test]
        public void CreateGeneralResultSubMechanismIllustrationPointEntity_ValidGeneralResult_ReturnsEntityWithoutStochastsAndTopLevelSubMechanismIllustrationPointEntities()
        {
            // Setup
            var random = new Random(21);

            var generalResult = new GeneralResult<TopLevelSubMechanismIllustrationPoint>(
                new WindDirection("SSE", random.NextDouble()),
                Enumerable.Empty<Stochast>(),
                Enumerable.Empty<TopLevelSubMechanismIllustrationPoint>());

            // Call
            GeneralResultSubMechanismIllustrationPointEntity entity =
                generalResult.CreateGeneralResultSubMechanismIllustrationPointEntity();

            // Assert
            AssertGeneralResultSubMechanismIllustrationPointEntity(generalResult, entity);
        }

        [Test]
        public void CreateGeneralResultSubMechanismIllustrationPointEntity_ValidGeneralResultWithStochasts_ReturnsEntityWithStochastsEntities()
        {
            // Setup
            var random = new Random(21);

            var generalResult = new GeneralResult<TopLevelSubMechanismIllustrationPoint>(
                new WindDirection("SSE", random.NextDouble()),
                new[]
                {
                    new Stochast("stochastOne", random.NextDouble(), random.NextDouble()),
                    new Stochast("stochastTwo", random.NextDouble(), random.NextDouble())
                },
                Enumerable.Empty<TopLevelSubMechanismIllustrationPoint>());

            // Call
            GeneralResultSubMechanismIllustrationPointEntity entity =
                generalResult.CreateGeneralResultSubMechanismIllustrationPointEntity();

            // Assert
            AssertGeneralResultSubMechanismIllustrationPointEntity(generalResult, entity);
        }

        [Test]
        public void CreateGeneralResultSubMechanismIllustrationPointEntity_ValidGeneralResultWithIllustrationPoints_ReturnsEntityWithTopLevelSubMechanismIllustrationPointEntities()
        {
            // Setup
            var random = new Random(21);

            var generalResult = new GeneralResult<TopLevelSubMechanismIllustrationPoint>(
                new WindDirection("SSE", random.NextDouble()),
                Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                              "IllustrationPointOne",
                                                              new TestSubMechanismIllustrationPoint()),
                    new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                              "IllustrationPointTwo",
                                                              new TestSubMechanismIllustrationPoint())
                });

            // Call
            GeneralResultSubMechanismIllustrationPointEntity entity =
                generalResult.CreateGeneralResultSubMechanismIllustrationPointEntity();

            // Assert
            AssertGeneralResultSubMechanismIllustrationPointEntity(generalResult, entity);
        }

        [Test]
        public void CreateGeneralResultFaultTreeIllustrationPointEntity_GeneralResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((GeneralResult<TopLevelFaultTreeIllustrationPoint>) null)
                .CreateGeneralResultFaultTreeIllustrationPointEntity();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("generalResult", exception.ParamName);
        }

        [Test]
        public void CreateGeneralResultFaultTreeIllustrationPointEntity_ValidGeneralResult_ReturnsExpectedGeneralResultFaultTreeIllustrationPointEntity()
        {
            // Setup
            var random = new Random(21);

            var generalResult = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                new WindDirection("SSE", random.NextDouble()),
                Enumerable.Empty<Stochast>(),
                Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>());

            // Call
            GeneralResultFaultTreeIllustrationPointEntity entity =
                generalResult.CreateGeneralResultFaultTreeIllustrationPointEntity();

            // Assert
            AssertGeneralResultFaultTreeIllustrationPointEntity(generalResult, entity);
        }

        [Test]
        public void CreateGeneralResultFaultTreeIllustrationPointEntity_ValidGeneralResultWithStochasts_ReturnsEntityWithStochastsEntities()
        {
            // Setup
            var random = new Random(22);

            var generalResult = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                new WindDirection("SSE", random.NextDouble()),
                new[]
                {
                    new Stochast("stochastOne", random.NextDouble(), random.NextDouble()),
                    new Stochast("stochastTwo", random.NextDouble(), random.NextDouble())
                },
                Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>());

            // Call
            GeneralResultFaultTreeIllustrationPointEntity entity =
                generalResult.CreateGeneralResultFaultTreeIllustrationPointEntity();

            // Assert
            AssertGeneralResultFaultTreeIllustrationPointEntity(generalResult, entity);
        }

        [Test]
        public void CreateGeneralResultFaultTreeIllustrationPointEntity_ValidGeneralResultsWithIllustrationPoints_ReturnsEntityWithTopLevelFaultTreeIllustrationPointEntities()
        {
            // Setup
            var random = new Random(22);

            var generalResult = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                new WindDirection("SSE", random.NextDouble()),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new TopLevelFaultTreeIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(),
                        "IllustrationPointOne",
                        new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPoint())),
                    new TopLevelFaultTreeIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(),
                        "IllustrationPointTwo",
                        new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPoint()))
                });

            // Call
            GeneralResultFaultTreeIllustrationPointEntity entity =
                generalResult.CreateGeneralResultFaultTreeIllustrationPointEntity();

            // Assert
            AssertGeneralResultFaultTreeIllustrationPointEntity(generalResult, entity);
        }

        private static void AssertGeneralResultSubMechanismIllustrationPointEntity(
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult,
            GeneralResultSubMechanismIllustrationPointEntity entity)
        {
            AssertWindDirection(generalResult.GoverningWindDirection, entity);

            AssertStochastEntities(generalResult.Stochasts.ToArray(), entity.StochastEntities.ToArray());

            AssertTopLevelIllustrationPointEntities(generalResult.TopLevelIllustrationPoints.ToArray(),
                                                    entity.TopLevelSubMechanismIllustrationPointEntities.ToArray());
        }

        private static void AssertGeneralResultFaultTreeIllustrationPointEntity(
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult,
            GeneralResultFaultTreeIllustrationPointEntity entity)
        {
            AssertWindDirection(generalResult.GoverningWindDirection, entity);

            AssertStochastEntities(generalResult.Stochasts.ToArray(), entity.StochastEntities.ToArray());

            AssertTopLevelIllustrationPointEntities(generalResult.TopLevelIllustrationPoints.ToArray(),
                                                    entity.TopLevelFaultTreeIllustrationPointEntities.ToArray());
        }

        private static void AssertWindDirection(WindDirection expectedWindDirection,
                                                IGeneralResultEntity entity)
        {
            TestHelper.AssertAreEqualButNotSame(expectedWindDirection.Name, entity.GoverningWindDirectionName);
            Assert.AreEqual(expectedWindDirection.Angle, entity.GoverningWindDirectionAngle,
                            expectedWindDirection.Angle.GetAccuracy());
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

        private static void AssertTopLevelIllustrationPointEntities<TTopLevelIllustrationPoint>(
            TTopLevelIllustrationPoint[] illustrationPoints,
            TopLevelSubMechanismIllustrationPointEntity[] illustrationPointEntities)
            where TTopLevelIllustrationPoint : TopLevelIllustrationPointBase
        {
            Assert.AreEqual(illustrationPoints.Length, illustrationPointEntities.Length);

            for (var i = 0; i < illustrationPoints.Length; i++)
            {
                AssertTopLevelIllustrationPointEntity(illustrationPoints[i],
                                                      illustrationPointEntities[i],
                                                      i);
            }
        }

        private static void AssertTopLevelIllustrationPointEntity<TTopLevelIllustrationPoint>(
            TTopLevelIllustrationPoint illustrationPoint,
            TopLevelSubMechanismIllustrationPointEntity illustrationPointEntity,
            int expectedOrder)
            where TTopLevelIllustrationPoint : TopLevelIllustrationPointBase
        {
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.ClosingSituation,
                                                illustrationPointEntity.ClosingSituation);

            WindDirection expectedWindDirection = illustrationPoint.WindDirection;
            TestHelper.AssertAreEqualButNotSame(expectedWindDirection.Name, illustrationPointEntity.WindDirectionName);
            Assert.AreEqual(expectedWindDirection.Angle, illustrationPointEntity.WindDirectionAngle,
                            expectedWindDirection.Angle.GetAccuracy());
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.ClosingSituation, illustrationPointEntity.ClosingSituation);

            Assert.AreEqual(expectedOrder, illustrationPointEntity.Order);
        }

        private static void AssertTopLevelIllustrationPointEntities<TTopLevelIllustrationPoint>(
            TTopLevelIllustrationPoint[] illustrationPoints,
            TopLevelFaultTreeIllustrationPointEntity[] illustrationPointEntities)
            where TTopLevelIllustrationPoint : TopLevelIllustrationPointBase
        {
            Assert.AreEqual(illustrationPoints.Length, illustrationPointEntities.Length);

            for (var i = 0; i < illustrationPoints.Length; i++)
            {
                AssertTopLevelIllustrationPointEntity(illustrationPoints[i],
                                                      illustrationPointEntities[i],
                                                      i);
            }
        }

        private static void AssertTopLevelIllustrationPointEntity<TTopLevelIllustrationPoint>(
            TTopLevelIllustrationPoint illustrationPoint,
            TopLevelFaultTreeIllustrationPointEntity illustrationPointEntity,
            int expectedOrder)
            where TTopLevelIllustrationPoint : TopLevelIllustrationPointBase
        {
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.ClosingSituation,
                                                illustrationPointEntity.ClosingSituation);

            WindDirection expectedWindDirection = illustrationPoint.WindDirection;
            TestHelper.AssertAreEqualButNotSame(expectedWindDirection.Name, illustrationPointEntity.WindDirectionName);
            Assert.AreEqual(expectedWindDirection.Angle, illustrationPointEntity.WindDirectionAngle,
                            expectedWindDirection.Angle.GetAccuracy());
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.ClosingSituation, illustrationPointEntity.ClosingSituation);

            Assert.AreEqual(expectedOrder, illustrationPointEntity.Order);
        }
    }
}