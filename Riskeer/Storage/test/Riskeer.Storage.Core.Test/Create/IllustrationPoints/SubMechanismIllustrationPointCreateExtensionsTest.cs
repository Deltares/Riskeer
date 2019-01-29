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
using Riskeer.Storage.Core.Create.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class SubMechanismIllustrationPointCreateExtensionsTest
    {
        [Test]
        public void Create_IllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((SubMechanismIllustrationPoint) null).Create(0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("subMechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void Create_SubMechanismIllustrationPointWithoutResultsAndStochasts_ReturnSubMechanismIllustrationPointEntityWithoutResultsAndStochastsEntities()
        {
            // Setup
            var random = new Random(21);

            var illustrationPoint = new SubMechanismIllustrationPoint("Illustration point name",
                                                                      random.NextDouble(),
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                      Enumerable.Empty<IllustrationPointResult>());
            int order = random.Next();

            // Call
            SubMechanismIllustrationPointEntity entity = illustrationPoint.Create(order);

            // Assert
            AssertCommonProperties(illustrationPoint, entity);
            Assert.AreEqual(order, entity.Order);
            CollectionAssert.IsEmpty(entity.IllustrationPointResultEntities);
            CollectionAssert.IsEmpty(entity.SubMechanismIllustrationPointStochastEntities);
        }

        [Test]
        public void Create_ValidIllustrationPointWithStochasts_ReturnSubMechanismIllustrationPointEntityWithStochastEntities()
        {
            // Setup
            var random = new Random(21);
            var stochastOne = new SubMechanismIllustrationPointStochast("stochast name",
                                                                        random.NextDouble(),
                                                                        random.NextDouble(),
                                                                        random.NextDouble());
            var stochastTwo = new SubMechanismIllustrationPointStochast("Stochast name two",
                                                                        random.NextDouble(),
                                                                        random.NextDouble(),
                                                                        random.NextDouble());
            SubMechanismIllustrationPointStochast[] stochasts =
            {
                stochastOne,
                stochastTwo
            };

            var illustrationPoint = new SubMechanismIllustrationPoint("Illustration point name",
                                                                      random.NextDouble(),
                                                                      stochasts,
                                                                      Enumerable.Empty<IllustrationPointResult>());
            int order = random.Next();

            // Call
            SubMechanismIllustrationPointEntity entity = illustrationPoint.Create(order);

            // Assert
            AssertCommonProperties(illustrationPoint, entity);
            Assert.AreEqual(order, entity.Order);
            SubMechanismIllustrationPointStochastEntity[] stochastEntities =
                entity.SubMechanismIllustrationPointStochastEntities.ToArray();
            int expectedNrOfStochasts = stochasts.Length;
            Assert.AreEqual(expectedNrOfStochasts, stochastEntities.Length);
            for (var i = 0; i < expectedNrOfStochasts; i++)
            {
                SubMechanismIllustrationPointStochast stochast = stochasts[i];
                SubMechanismIllustrationPointStochastEntity stochastEntity = stochastEntities[i];

                TestHelper.AssertAreEqualButNotSame(stochast.Name, stochastEntity.Name);
                Assert.AreEqual(stochast.Duration, stochastEntity.Duration, stochast.Duration.GetAccuracy());
                Assert.AreEqual(stochast.Alpha, stochastEntity.Alpha, stochast.Alpha.GetAccuracy());
                Assert.AreEqual(stochast.Realization, stochastEntity.Realization, stochast.Realization.GetAccuracy());
                Assert.AreEqual(i, stochastEntity.Order);
            }
        }

        [Test]
        public void Create_MultipleResultsAndValidIllustrationPoint_ReturnSubMechanismIllustrationPointEntityWithResultEntities()
        {
            // Setup
            var random = new Random(21);

            var illustrationPointResultOne = new IllustrationPointResult("result description", random.NextDouble());
            var illustrationPointResultTwo = new IllustrationPointResult("result description two", random.NextDouble());
            IllustrationPointResult[] illustrationPointResults =
            {
                illustrationPointResultOne,
                illustrationPointResultTwo
            };

            var illustrationPoint = new SubMechanismIllustrationPoint("Illustration point name",
                                                                      random.NextDouble(),
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                      illustrationPointResults);
            int order = random.Next();

            // Call
            SubMechanismIllustrationPointEntity entity = illustrationPoint.Create(order);

            // Assert
            AssertCommonProperties(illustrationPoint, entity);
            Assert.AreEqual(order, entity.Order);
            IllustrationPointResultEntity[] resultEntities = entity.IllustrationPointResultEntities.ToArray();
            int expectedNrOfIllustrationPoints = illustrationPointResults.Length;
            Assert.AreEqual(expectedNrOfIllustrationPoints, resultEntities.Length);
            for (var i = 0; i < expectedNrOfIllustrationPoints; i++)
            {
                IllustrationPointResult illustrationPointResult = illustrationPointResults[i];
                IllustrationPointResultEntity illustrationPointResultEntity = resultEntities[i];

                TestHelper.AssertAreEqualButNotSame(illustrationPointResult.Description, illustrationPointResultEntity.Description);
                Assert.AreEqual(illustrationPointResult.Value, illustrationPointResultEntity.Value,
                                illustrationPointResult.Value.GetAccuracy());
                Assert.AreEqual(i, illustrationPointResultEntity.Order);
            }
        }

        private static void AssertCommonProperties(SubMechanismIllustrationPoint illustrationPoint,
                                                   SubMechanismIllustrationPointEntity entity)
        {
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.Name, entity.Name);
            Assert.AreEqual(illustrationPoint.Beta, entity.Beta, illustrationPoint.Beta.GetAccuracy());
        }
    }
}