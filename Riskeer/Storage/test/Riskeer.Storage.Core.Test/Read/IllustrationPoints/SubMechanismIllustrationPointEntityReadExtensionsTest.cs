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
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.IllustrationPoints;

namespace Riskeer.Storage.Core.Test.Read.IllustrationPoints
{
    [TestFixture]
    public class SubMechanismIllustrationPointEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((SubMechanismIllustrationPointEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntityWithoutStochastsAndIllustrationPointResults_ReturnsSubMechanismIllustrationPoint()
        {
            // Setup
            var random = new Random(21);
            var entity = new SubMechanismIllustrationPointEntity
            {
                Name = "Name",
                Beta = random.NextDouble()
            };

            // Call
            SubMechanismIllustrationPoint illustrationPoint = entity.Read();

            // Assert
            AssertCommonProperties(entity, illustrationPoint);

            CollectionAssert.IsEmpty(illustrationPoint.IllustrationPointResults);
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
        }

        [Test]
        public void Read_ValidEntityWithIllustrationPointResults_ReturnsSubMechanismIllustrationPointWithResults()
        {
            // Setup
            var random = new Random(21);

            var illustrationPointResultEntityOne = new IllustrationPointResultEntity
            {
                Description = "Description",
                Value = random.NextDouble(),
                Order = 0
            };
            var illustrationPointResultEntityTwo = new IllustrationPointResultEntity
            {
                Description = "Description_Two",
                Value = random.NextDouble(),
                Order = 1
            };

            var entity = new SubMechanismIllustrationPointEntity
            {
                Name = "Name",
                Beta = random.NextDouble(),
                IllustrationPointResultEntities = new[]
                {
                    illustrationPointResultEntityTwo,
                    illustrationPointResultEntityOne
                }
            };

            // Call
            SubMechanismIllustrationPoint illustrationPoint = entity.Read();

            // Assert
            AssertCommonProperties(entity, illustrationPoint);

            IllustrationPointResult[] illustrationPointResults = illustrationPoint.IllustrationPointResults.ToArray();
            Assert.AreEqual(2, illustrationPointResults.Length);
            AssertReadIllustrationPointResult(illustrationPointResultEntityOne, illustrationPointResults[0]);
            AssertReadIllustrationPointResult(illustrationPointResultEntityTwo, illustrationPointResults[1]);
        }

        [Test]
        public void Read_ValidEntityWithStochasts_ReturnsSubMechanismIllustrationPointWithStochasts()
        {
            // Setup
            var random = new Random(21);

            var stochastEntityOne = new SubMechanismIllustrationPointStochastEntity
            {
                Name = "Stochast",
                Alpha = random.NextDouble(),
                Duration = random.NextDouble(),
                Realization = random.NextDouble(),
                Order = 0
            };
            var stochastEntityTwo = new SubMechanismIllustrationPointStochastEntity
            {
                Name = "Stochast_Two",
                Alpha = random.NextDouble(),
                Duration = random.NextDouble(),
                Realization = random.NextDouble(),
                Order = 1
            };

            const string illustrationPointName = "Name";
            double beta = random.NextDouble();
            var entity = new SubMechanismIllustrationPointEntity
            {
                Name = illustrationPointName,
                Beta = beta,
                SubMechanismIllustrationPointStochastEntities = new[]
                {
                    stochastEntityTwo,
                    stochastEntityOne
                }
            };

            // Call
            SubMechanismIllustrationPoint illustrationPoint = entity.Read();

            // Assert
            AssertCommonProperties(entity, illustrationPoint);

            SubMechanismIllustrationPointStochast[] stochasts = illustrationPoint.Stochasts.ToArray();
            Assert.AreEqual(2, stochasts.Length);
            AssertReadStochast(stochastEntityOne, stochasts[0]);
            AssertReadStochast(stochastEntityTwo, stochasts[1]);
        }

        private static void AssertCommonProperties(SubMechanismIllustrationPointEntity expectedEntity,
                                                   SubMechanismIllustrationPoint illustrationPoint)
        {
            Assert.AreEqual(expectedEntity.Name, illustrationPoint.Name);
            Assert.AreEqual(expectedEntity.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
        }

        private static void AssertReadIllustrationPointResult(IllustrationPointResultEntity illustrationPointResultEntity,
                                                              IllustrationPointResult readIllustrationPointResult)
        {
            Assert.AreEqual(illustrationPointResultEntity.Description, readIllustrationPointResult.Description);
            Assert.AreEqual(illustrationPointResultEntity.Value, readIllustrationPointResult.Value,
                            readIllustrationPointResult.Value.GetAccuracy());
        }

        private static void AssertReadStochast(SubMechanismIllustrationPointStochastEntity stochastEntity,
                                               SubMechanismIllustrationPointStochast readStochast)
        {
            Assert.AreEqual(stochastEntity.Name, readStochast.Name);
            Assert.AreEqual(stochastEntity.Alpha, readStochast.Alpha, readStochast.Alpha.GetAccuracy());
            Assert.AreEqual(stochastEntity.Duration, readStochast.Duration, readStochast.Duration.GetAccuracy());
            Assert.AreEqual(stochastEntity.Realization, readStochast.Realization, readStochast.Realization.GetAccuracy());
        }
    }
}