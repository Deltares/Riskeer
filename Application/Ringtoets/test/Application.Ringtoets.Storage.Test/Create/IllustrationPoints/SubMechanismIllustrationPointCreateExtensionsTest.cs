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
using NUnit.Framework;
using Application.Ringtoets.Storage.Create.IllustrationPoints;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class SubMechanismIllustrationPointCreateExtensionsTest
    {
        [Test]
        public void CreateSubMechanismIllustrationPoint_IllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((SubMechanismIllustrationPoint) null).CreateSubMechanismIllustrationPointEntity();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("subMechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void CreateSubMechanismIllustrationPoint_SubMechanismIllustrationPointWithoutResultsAndStochasts_ReturnEntity()
        {
            // Setup
            var random = new Random(21);

            const string illustrationPointName = "Illustration point name";
            double beta = random.NextDouble();
            var illustrationPoint = new SubMechanismIllustrationPoint(illustrationPointName,
                                                                      beta, 
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(), 
                                                                      Enumerable.Empty<IllustrationPointResult>());

            // Call
            SubMechanismIllustrationPointEntity entity = illustrationPoint.CreateSubMechanismIllustrationPointEntity();

            // Assert
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.Name, entity.Name);
            Assert.AreEqual(illustrationPoint.Beta, entity.Beta, illustrationPoint.Beta.GetAccuracy());

            CollectionAssert.IsEmpty(entity.IllustrationPointResultEntities);
            CollectionAssert.IsEmpty(entity.SubMechanismIllustrationPointStochastEntities);
        }

        [Test]
        public void CreateSubMechanismIllustrationPoint_ValidIllustrationPointWithStochasts_ReturnEntity()
        {
            // Setup
            var random = new Random(21);

            const string stochastName = "stochast name";
            double duration = random.NextDouble();
            double alpha = random.NextDouble();
            double realization = random.NextDouble();
            var stochastOne = new SubMechanismIllustrationPointStochast(stochastName,
                                                                        duration,
                                                                        alpha,
                                                                        realization);
            var stochastTwo = new SubMechanismIllustrationPointStochast($"{stochastName}_Two",
                                                                        duration + 1,
                                                                        alpha + 1,
                                                                        realization + 1);
            var stochasts = new[]
            {
                stochastOne,
                stochastTwo
            };

            const string illustrationPointName = "Illustration point name";
            double beta = random.NextDouble();
            var illustrationPoint = new SubMechanismIllustrationPoint(illustrationPointName,
                                                                      beta, 
                                                                      stochasts, 
                                                                      Enumerable.Empty<IllustrationPointResult>());

            // Call
            SubMechanismIllustrationPointEntity entity = illustrationPoint.CreateSubMechanismIllustrationPointEntity();

            // Assert
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.Name, entity.Name);
            Assert.AreEqual(illustrationPoint.Beta, entity.Beta, illustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(entity.IllustrationPointResultEntities);

            SubMechanismIllustrationPointStochastEntity[] stochastEntities = entity.SubMechanismIllustrationPointStochastEntities.ToArray();
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
        public void CreateSubMechanismIllustrationPoint_MultipleResultsAndValidIllustrationPoint_ReturnEntity()
        {
            // Setup
            var random = new Random(21);

            const string resultDescription = "result description";
            double value = random.NextDouble();
            var illustrationPointResultOne = new IllustrationPointResult(resultDescription, value);
            var illustrationPointResultTwo = new IllustrationPointResult($"{resultDescription}_Two", value + 1);
            var illustrationPointResults = new[]
            {
                illustrationPointResultOne,
                illustrationPointResultTwo
            };

            const string illustrationPointName = "Illustration point name";
            double beta = random.NextDouble();

            var illustrationPoint = new SubMechanismIllustrationPoint(illustrationPointName,
                                                                      beta, 
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(), 
                                                                      illustrationPointResults);

            // Call
            SubMechanismIllustrationPointEntity entity = illustrationPoint.CreateSubMechanismIllustrationPointEntity();

            // Assert
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.Name, entity.Name);
            Assert.AreEqual(illustrationPoint.Beta, entity.Beta, illustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(entity.SubMechanismIllustrationPointStochastEntities);

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
    }
}