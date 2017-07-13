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
using Application.Ringtoets.Storage.Create.IllustrationPoints;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Application.Ringtoets.Storage.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultCreateExtensionsTest
    {
        [Test]
        public void CreateGeneralResultSubMechanismIllustrationPointEntity_GeneralResultSubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Setup
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = null;

            // Call
            TestDelegate call = () => generalResult.CreateGeneralResultSubMechanismIllustrationPointEntity();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("generalResultSubMechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void CreateGeneralResultSubMechanismIllustrationPointEntity_ValidGeneralResultSubMechanismIllustrationPoint_ReturnsEntityWithoutStochastsAndTopLevelSubMechanismIllustrationPointEntities()
        {
            // Setup
            var random = new Random(21);
            var governingWindDirection = new WindDirection("SSE", random.NextDouble());

            var generalResult =
                new GeneralResult<TopLevelSubMechanismIllustrationPoint>(governingWindDirection,
                                                                         Enumerable.Empty<Stochast>(),
                                                                         Enumerable.Empty<TopLevelSubMechanismIllustrationPoint>());

            // Call
            GeneralResultSubMechanismIllustrationPointEntity entity =
                generalResult.CreateGeneralResultSubMechanismIllustrationPointEntity();

            // Assert
            AssertWindDirection(governingWindDirection, entity);

            CollectionAssert.IsEmpty(entity.StochastEntities);
            CollectionAssert.IsEmpty(entity.TopLevelSubMechanismIllustrationPointEntities);
        }

        [Test]
        public void CreateGeneralResultSubMechanismIllustrationPointEntity_ValidGeneralResultSubMechanismIllustrationPointWithStochasts_ReturnsEntityWithStochastsEntities()
        {
            // Setup
            var random = new Random(21);
            var governingWindDirection = new WindDirection("SSE", random.NextDouble());

            var stochastOne = new Stochast("stochastOne", random.NextDouble(), random.NextDouble());
            var stochastTwo = new Stochast("stochastTwo", random.NextDouble(), random.NextDouble());
            var stochasts = new[]
            {
                stochastOne,
                stochastTwo
            };

            var generalResult =
                new GeneralResult<TopLevelSubMechanismIllustrationPoint>(governingWindDirection,
                                                                         stochasts,
                                                                         Enumerable.Empty<TopLevelSubMechanismIllustrationPoint>());

            // Call
            GeneralResultSubMechanismIllustrationPointEntity entity =
                generalResult.CreateGeneralResultSubMechanismIllustrationPointEntity();

            // Assert
            AssertWindDirection(governingWindDirection, entity);

            StochastEntity[] stochastEntities = entity.StochastEntities.ToArray();
            Assert.AreEqual(stochasts.Length, stochastEntities.Length);
            for (var i = 0; i < stochasts.Length; i++)
            {
                Stochast stochast = stochasts[i];
                StochastEntity stochastEntity = stochastEntities[i];

                AssertCreatedStochastEntity(stochast, i, stochastEntity);
            }
        }

        [Test]
        public void CreateGeneralResultSubMechanismIllustrationPointEntity_ValidGeneralResultSubMechanismIllustrationPointWithIllustrationPoints_ReturnsEntityWithTopLevelSubMechanismIllustrationPointEntities()
        {
            // Setup
            var random = new Random(21);
            var governingWindDirection = new WindDirection("SSE", random.NextDouble());

            var illustrationPointOne = new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                 "IllustrationPointOne",
                                                                                 new TestSubMechanismIllustrationPoint());
            var illustrationPointTwo = new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                 "IllustrationPointTwo",
                                                                                 new TestSubMechanismIllustrationPoint());
            var illustrationPoints = new[]
            {
                illustrationPointOne,
                illustrationPointTwo
            };

            var generalResult =
                new GeneralResult<TopLevelSubMechanismIllustrationPoint>(governingWindDirection,
                                                                         Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                         illustrationPoints);

            // Call
            GeneralResultSubMechanismIllustrationPointEntity entity =
                generalResult.CreateGeneralResultSubMechanismIllustrationPointEntity();

            // Assert
            AssertWindDirection(governingWindDirection, entity);

            TopLevelSubMechanismIllustrationPointEntity[] illustrationPointEntities =
                entity.TopLevelSubMechanismIllustrationPointEntities.ToArray();
            Assert.AreEqual(illustrationPoints.Length, illustrationPointEntities.Length);
            for (var i = 0; i < illustrationPoints.Length; i++)
            {
                TopLevelSubMechanismIllustrationPoint illustrationPoint = illustrationPoints[i];
                TopLevelSubMechanismIllustrationPointEntity illustrationPointEntity = illustrationPointEntities[i];

                AssertCreatedTopLevelSubMechanismIllustrationPointEntity(illustrationPoint, i, illustrationPointEntity);
            }
        }

        private static void AssertWindDirection(WindDirection expectedWindDirection,
                                                GeneralResultSubMechanismIllustrationPointEntity entity)
        {
            TestHelper.AssertAreEqualButNotSame(expectedWindDirection.Name, entity.GoverningWindDirectionName);
            Assert.AreEqual(expectedWindDirection.Angle, entity.GoverningWindDirectionAngle,
                            expectedWindDirection.Angle.GetAccuracy());
        }

        private static void AssertCreatedStochastEntity(Stochast stochast,
                                                        int expectedOrder,
                                                        StochastEntity createdStochastEntity)
        {
            TestHelper.AssertAreEqualButNotSame(stochast.Name, createdStochastEntity.Name);
            Assert.AreEqual(stochast.Duration, createdStochastEntity.Duration,
                            stochast.Duration.GetAccuracy());
            Assert.AreEqual(stochast.Alpha, createdStochastEntity.Alpha,
                            stochast.Alpha.GetAccuracy());
            Assert.AreEqual(expectedOrder, createdStochastEntity.Order);
        }

        private static void AssertCreatedTopLevelSubMechanismIllustrationPointEntity(TopLevelSubMechanismIllustrationPoint illustrationPoint,
                                                                                     int expectedOrder,
                                                                                     TopLevelSubMechanismIllustrationPointEntity illustrationPointEntity)
        {
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.ClosingSituation,
                                                illustrationPointEntity.ClosingSituation);

            WindDirection expectedWindDirection = illustrationPoint.WindDirection;
            TestHelper.AssertAreEqualButNotSame(expectedWindDirection.Name, illustrationPointEntity.WindDirectionName);
            Assert.AreEqual(expectedWindDirection.Angle, illustrationPointEntity.WindDirectionAngle,
                            expectedWindDirection.Angle.GetAccuracy());

            Assert.IsNotNull(illustrationPointEntity.SubMechanismIllustrationPointEntity);

            Assert.AreEqual(expectedOrder, illustrationPointEntity.Order);
        }
    }
}