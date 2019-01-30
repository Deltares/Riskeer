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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.IllustrationPoints;

namespace Riskeer.Storage.Core.Test.Read.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultFaultTreeIllustrationPointEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((GeneralResultFaultTreeIllustrationPointEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntity_ReturnsGeneralResult()
        {
            // Setup
            var entity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = "SSE",
                GoverningWindDirectionAngle = new Random(213).NextDouble()
            };

            // Call
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = entity.Read();

            // Assert
            AssertWindDirection(entity, generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
            CollectionAssert.IsEmpty(generalResult.TopLevelIllustrationPoints);
        }

        [Test]
        public void Read_ValidEntityWithStochasts_ReturnsGeneralResultFaultTreeIllustrationPoint()
        {
            // Setup
            var random = new Random(21);

            var entity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = "SSE",
                GoverningWindDirectionAngle = random.NextDouble(),
                StochastEntities = new[]
                {
                    new StochastEntity
                    {
                        Name = "stochastEntityOne",
                        Duration = random.NextDouble(),
                        Alpha = random.NextDouble(),
                        Order = 0
                    },
                    new StochastEntity
                    {
                        Name = "stochastEntityTwo",
                        Duration = random.NextDouble(),
                        Alpha = random.NextDouble(),
                        Order = 1
                    }
                }
            };

            // Call
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = entity.Read();

            // Assert
            AssertWindDirection(entity, generalResult.GoverningWindDirection);
            AssertStochasts(entity.StochastEntities.ToArray(), generalResult.Stochasts.ToArray());
        }

        [Test]
        public void Read_ValidEntityWithIllustrationPoints_ReturnsGeneralResultFaultTreeIllustrationPoint()
        {
            // Setup
            var random = new Random(210);

            var topLevelFaultTreeIllustrationPointEntities = new[]
            {
                new TopLevelFaultTreeIllustrationPointEntity
                {
                    WindDirectionName = "WindDirectionOne",
                    WindDirectionAngle = random.NextDouble(),
                    ClosingSituation = "ClosingSituationOne",
                    FaultTreeIllustrationPointEntity = new FaultTreeIllustrationPointEntity
                    {
                        Beta = random.NextDouble(),
                        CombinationType = Convert.ToByte(random.NextEnumValue<CombinationType>()),
                        Name = "IllustrationPointOne"
                    },
                    Order = 0
                },
                new TopLevelFaultTreeIllustrationPointEntity
                {
                    WindDirectionName = "WindDirectionTwo",
                    WindDirectionAngle = random.NextDouble(),
                    ClosingSituation = "ClosingSituationTwo",
                    FaultTreeIllustrationPointEntity = new FaultTreeIllustrationPointEntity
                    {
                        Beta = random.NextDouble(),
                        CombinationType = Convert.ToByte(random.NextEnumValue<CombinationType>()),
                        Name = "IllustrationPointTwo"
                    },
                    Order = 1
                }
            };

            var entity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = "SSE",
                GoverningWindDirectionAngle = random.NextDouble(),
                TopLevelFaultTreeIllustrationPointEntities = topLevelFaultTreeIllustrationPointEntities
            };

            // Call
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = entity.Read();

            // Assert
            AssertWindDirection(entity, generalResult.GoverningWindDirection);
            AssertIllustrationPoints(entity.TopLevelFaultTreeIllustrationPointEntities.ToArray(),
                                     generalResult.TopLevelIllustrationPoints.ToArray());
        }

        private static void AssertWindDirection(IGeneralResultEntity entity, WindDirection windDirection)
        {
            Assert.AreEqual(entity.GoverningWindDirectionName, windDirection.Name);
            Assert.AreEqual(entity.GoverningWindDirectionAngle, windDirection.Angle,
                            windDirection.Angle.GetAccuracy());
        }

        private static void AssertStochasts(StochastEntity[] entities, Stochast[] stochasts)
        {
            Assert.AreEqual(entities.Length, stochasts.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                AssertStochast(entities[i], stochasts[i]);
            }
        }

        private static void AssertStochast(StochastEntity stochastEntity,
                                           Stochast readStochast)
        {
            Assert.AreEqual(stochastEntity.Name, readStochast.Name);
            Assert.AreEqual(stochastEntity.Alpha, readStochast.Alpha, readStochast.Alpha.GetAccuracy());
            Assert.AreEqual(stochastEntity.Duration, readStochast.Duration, readStochast.Duration.GetAccuracy());
        }

        private static void AssertIllustrationPoints(
            TopLevelFaultTreeIllustrationPointEntity[] entities,
            TopLevelFaultTreeIllustrationPoint[] illustrationPoints)
        {
            Assert.AreEqual(entities.Length, illustrationPoints.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                AssertTopLevelFaultTreeIllustrationPoint(entities[i], illustrationPoints[i]);
            }
        }

        private static void AssertTopLevelFaultTreeIllustrationPoint(
            TopLevelFaultTreeIllustrationPointEntity illustrationPointEntity,
            TopLevelFaultTreeIllustrationPoint illustrationPoint)
        {
            Assert.AreEqual(illustrationPointEntity.ClosingSituation, illustrationPoint.ClosingSituation);

            WindDirection actualWindDirection = illustrationPoint.WindDirection;
            Assert.AreEqual(illustrationPointEntity.WindDirectionName, actualWindDirection.Name);
            Assert.AreEqual(illustrationPointEntity.WindDirectionAngle, actualWindDirection.Angle, actualWindDirection.Angle);

            Assert.IsNotNull(illustrationPoint.FaultTreeNodeRoot);
        }
    }
}