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
using Application.Ringtoets.Storage.Read.IllustrationPoints;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read.IllustrationPoints
{
    [TestFixture]
    public class SubmechanismIllustrationPointReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((SubmechanismIllustrationPointEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntityWithoutStochastsAndIllustrationPointResults_ReturnsSubmechanismIllustrationPoint()
        {
            // Setup
            var random = new Random(21);

            const string illustrationPointName = "Name";
            double beta = random.NextDouble();
            var entity = new SubmechanismIllustrationPointEntity
            {
                Name = illustrationPointName,
                Beta = beta
            };

            // Call
            SubmechanismIllustrationPoint illustrationPoint = entity.Read();

            // Assert
            Assert.AreEqual(entity.Name, illustrationPoint.Name);
            Assert.AreEqual(entity.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());

            CollectionAssert.IsEmpty(illustrationPoint.IllustrationPointResults);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
        }

        [Test]
        public void Read_ValidEntityWithIllustrationPointResults_ReturnsSubmechanismIllustrationPoint()
        {
            // Setup
            var random = new Random(21);

            const string illustrationPointResultDescription = "Description";
            double value = random.NextDouble();
            var illustrationPointResultEntityOne = new IllustrationPointResultEntity
            {
                Description = illustrationPointResultDescription,
                Value = value,
                Order = 0
            };
            var illustrationPointResultEntityTwo = new IllustrationPointResultEntity
            {
                Description = $"{illustrationPointResultDescription}_Two",
                Value = value + 1,
                Order = 1
            };

            const string illustrationPointName = "Name";
            double beta = random.NextDouble();
            var entity = new SubmechanismIllustrationPointEntity
            {
                Name = illustrationPointName,
                Beta = beta,
                IllustrationPointResultEntities = new[]
                {
                    illustrationPointResultEntityTwo,
                    illustrationPointResultEntityOne
                }
            };

            // Call
            SubmechanismIllustrationPoint illustrationPoint = entity.Read();

            // Assert
            Assert.AreEqual(entity.Name, illustrationPoint.Name);
            Assert.AreEqual(entity.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);

            IllustrationPointResult[] illustrationPointResults = illustrationPoint.IllustrationPointResults.ToArray();
            Assert.AreEqual(2, illustrationPointResults.Length);

            IllustrationPointResult resultOne = illustrationPointResults[0];
            AssertReadIllustrationPointResult(illustrationPointResultEntityOne, resultOne);

            IllustrationPointResult resultTwo = illustrationPointResults[1];
            AssertReadIllustrationPointResult(illustrationPointResultEntityTwo, resultTwo);
        }

        [Test]
        public void Read_ValidEntityWithStochasts_ReturnsSubmechanismIllustrationPoint()
        {
            // Setup
            var random = new Random(21);

            const string stochastName = "Stochast";
            double alpha = random.NextDouble();
            double realization = random.NextDouble();
            double duration = random.NextDouble();
            var stochastEntityOne = new SubmechanismIllustrationPointStochastEntity
            {
                Name = stochastName,
                Alpha = alpha,
                Duration = duration,
                Realization = realization,
                Order = 0
            };
            var stochastEntityTwo = new SubmechanismIllustrationPointStochastEntity
            {
                Name = $"{stochastName}_Two",
                Alpha = alpha + 1,
                Duration = duration + 1,
                Realization = realization + 1,
                Order = 1
            };

            const string illustrationPointName = "Name";
            double beta = random.NextDouble();
            var entity = new SubmechanismIllustrationPointEntity
            {
                Name = illustrationPointName,
                Beta = beta,
                SubmechanismIllustrationPointStochastEntities = new[]
                {
                    stochastEntityTwo,
                    stochastEntityOne
                }
            };

            // Call
            SubmechanismIllustrationPoint illustrationPoint = entity.Read();

            // Assert
            Assert.AreEqual(entity.Name, illustrationPoint.Name);
            Assert.AreEqual(entity.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(illustrationPoint.IllustrationPointResults);

            SubmechanismIllustrationPointStochast[] stochasts = illustrationPoint.Stochasts.ToArray();
            Assert.AreEqual(2, stochasts.Length);

            SubmechanismIllustrationPointStochast readStochastOne = stochasts[0];
            AssertReadStochast(stochastEntityOne, readStochastOne);

            SubmechanismIllustrationPointStochast readStochastTwo = stochasts[1];
            AssertReadStochast(stochastEntityTwo, readStochastTwo);
        }

        private static void AssertReadIllustrationPointResult(IllustrationPointResultEntity illustrationPointResultEntity,
                                                              IllustrationPointResult readIllustrationPointResult)
        {
            Assert.AreEqual(illustrationPointResultEntity.Description, readIllustrationPointResult.Description);
            Assert.AreEqual(illustrationPointResultEntity.Value, readIllustrationPointResult.Value,
                            readIllustrationPointResult.Value.GetAccuracy());
        }

        private static void AssertReadStochast(SubmechanismIllustrationPointStochastEntity stochastEntity,
                                               SubmechanismIllustrationPointStochast readStochast)
        {
            Assert.AreEqual(stochastEntity.Name, readStochast.Name);
            Assert.AreEqual(stochastEntity.Alpha, readStochast.Alpha, readStochast.Alpha.GetAccuracy());
            Assert.AreEqual(stochastEntity.Duration, readStochast.Duration, readStochast.Duration.GetAccuracy());
            Assert.AreEqual(stochastEntity.Realization, readStochast.Realization, readStochast.Realization.GetAccuracy());
        }
    }
}