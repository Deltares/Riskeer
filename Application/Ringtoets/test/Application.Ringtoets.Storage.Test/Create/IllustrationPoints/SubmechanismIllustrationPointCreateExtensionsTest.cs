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
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Application.Ringtoets.Storage.Create.IllustrationPoints;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class SubmechanismIllustrationPointCreateExtensionsTest
    {
        [Test]
        public void CreateSubmechanismIllustrationPoint_IllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((SubmechanismIllustrationPoint) null).CreateSubmechanismIllustrationPointEntity();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("submechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void CreateSubmechanismIllustrationPoint_SubmechanismIllustrationPointWithoutResultsAndStochasts_ReturnEntity()
        {
            // Setup
            var random = new Random(21);

            const string resultDescription = "result description";
            double value = random.NextDouble();
            var illustrationPointResult = new IllustrationPointResult(resultDescription, value);

            const string illustrationPointName = "Illustration point name";
            double beta = random.NextDouble();
            var illustrationPoint = new SubmechanismIllustrationPoint(illustrationPointName,
                                                                      Enumerable.Empty<SubmechanismIllustrationPointStochast>(),
                                                                      Enumerable.Empty<IllustrationPointResult>(),
                                                                      beta);

            // Call
            SubmechanismIllustrationPointEntity entity = illustrationPoint.CreateSubmechanismIllustrationPointEntity();

            // Assert
            Assert.AreEqual(illustrationPoint.Name, entity.Name);
            Assert.AreEqual(illustrationPoint.Beta, entity.Beta, illustrationPoint.Beta.GetAccuracy());

            CollectionAssert.IsEmpty(entity.IllustrationPointResultEntities);
            CollectionAssert.IsEmpty(entity.SubmechanismIllustrationPointStochastEntities);
        }

        [Test]
        public void CreateSubmechanismIllustrationPoint_ValidIllustrationPointWithStochasts_ReturnEntity()
        {
            // Setup
            var random = new Random(21);

            const string stochastName = "stochast name";
            double duration = random.NextDouble();
            double alpha = random.NextDouble();
            double realization = random.NextDouble();
            var stochastOne = new SubmechanismIllustrationPointStochast(stochastName,
                                                                        duration,
                                                                        alpha,
                                                                        realization);
            var stochastTwo = new SubmechanismIllustrationPointStochast($"{stochastName}_Two",
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
            var illustrationPoint = new SubmechanismIllustrationPoint(illustrationPointName,
                                                                      stochasts,
                                                                      Enumerable.Empty<IllustrationPointResult>(),
                                                                      beta);

            // Call
            SubmechanismIllustrationPointEntity entity = illustrationPoint.CreateSubmechanismIllustrationPointEntity();

            // Assert
            Assert.AreEqual(illustrationPoint.Name, entity.Name);
            Assert.AreEqual(illustrationPoint.Beta, entity.Beta, illustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(entity.IllustrationPointResultEntities);

            SubmechanismIllustrationPointStochastEntity[] stochastEntities = entity.SubmechanismIllustrationPointStochastEntities.ToArray();
            int expectedNrOfStochasts = stochasts.Length;
            Assert.AreEqual(expectedNrOfStochasts, stochastEntities.Length);
            for (var i = 0; i < expectedNrOfStochasts; i++)
            {
                SubmechanismIllustrationPointStochast stochast = stochasts[i];
                SubmechanismIllustrationPointStochastEntity stochastEntity = stochastEntities[i];

                Assert.AreEqual(stochast.Name, stochastEntity.Name);
                Assert.AreEqual(stochast.Duration, stochastEntity.Duration, stochast.Duration.GetAccuracy());
                Assert.AreEqual(stochast.Alpha, stochastEntity.Alpha, stochast.Alpha.GetAccuracy());
                Assert.AreEqual(stochast.Realization, stochastEntity.Realization, stochast.Realization.GetAccuracy());
            }
        }

        [Test]
        public void CreateSubmechanismIllustrationPoint_MultipleResultsAndValidIllustrationPoint_ReturnEntity()
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

            var illustrationPoint = new SubmechanismIllustrationPoint(illustrationPointName,
                                                                      Enumerable.Empty<SubmechanismIllustrationPointStochast>(),
                                                                      illustrationPointResults,
                                                                      beta);

            // Call
            SubmechanismIllustrationPointEntity entity = illustrationPoint.CreateSubmechanismIllustrationPointEntity();

            // Assert
            Assert.AreEqual(illustrationPoint.Name, entity.Name);
            Assert.AreEqual(illustrationPoint.Beta, entity.Beta, illustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(entity.SubmechanismIllustrationPointStochastEntities);

            IllustrationPointResultEntity[] resultEntities = entity.IllustrationPointResultEntities.ToArray();
            int expectedNrOfIllustrationPoints = illustrationPointResults.Length;
            Assert.AreEqual(expectedNrOfIllustrationPoints, resultEntities.Length);
            for (var i = 0; i < expectedNrOfIllustrationPoints; i++)
            {
                IllustrationPointResult illustrationPointResult = illustrationPointResults[i];
                IllustrationPointResultEntity illustrationPointResultEntity = resultEntities[i];

                Assert.AreEqual(illustrationPointResult.Description, illustrationPointResultEntity.Description);
                Assert.AreEqual(illustrationPointResult.Value, illustrationPointResultEntity.Value,
                                illustrationPointResult.Value.GetAccuracy());
            }
        }
    }
}