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
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Application.Ringtoets.Storage.Test.Read.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultSubMechanismIllustrationPointEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((GeneralResultSubMechanismIllustrationPointEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntityWithoutStochastsAndIllustrationPoints_ReturnsGeneralResultSubMechanismIllustrationPoint()
        {
            // Setup
            var random = new Random(21);

            const string windDirectionName = "SSE";
            double windDirectionAngle = random.NextDouble();
            var entity = new GeneralResultSubMechanismIllustrationPointEntity
            {
                GoverningWindDirectionName = windDirectionName,
                GoverningWindDirectionAngle = windDirectionAngle
            };

            // Call
            GeneralResultSubMechanismIllustrationPoint generalResult = entity.Read();

            // Assert
            WindDirection actualGoverningWindDirection = generalResult.GoverningWindDirection;
            Assert.AreEqual(entity.GoverningWindDirectionName, actualGoverningWindDirection.Name);
            Assert.AreEqual(entity.GoverningWindDirectionAngle, actualGoverningWindDirection.Angle,
                            actualGoverningWindDirection.Angle.GetAccuracy());
            CollectionAssert.IsEmpty(generalResult.Stochasts);
            CollectionAssert.IsEmpty(generalResult.TopLevelSubMechanismIllustrationPoints);
        }

        [Test]
        public void Read_ValidEntityWithStochasts_ReturnsGeneralResultSubMechanismIllustrationPoint()
        {
            // Setup
            var random = new Random(21);

            const string stochastName = "Stochast Name";
            double alpha = random.NextDouble();
            double duration = random.NextDouble();
            var stochastEntityOne = new StochastEntity
            {
                Name = stochastName,
                Duration = duration,
                Alpha = alpha,
                Order = 0
            };
            var stochastEntityTwo = new StochastEntity
            {
                Name = $"{stochastName}_Two",
                Duration = duration + 1,
                Alpha = alpha + 1,
                Order = 1
            };

            const string windDirectionName = "SSE";
            double windDirectionAngle = random.NextDouble();
            var entity = new GeneralResultSubMechanismIllustrationPointEntity
            {
                GoverningWindDirectionName = windDirectionName,
                GoverningWindDirectionAngle = windDirectionAngle,
                StochastEntities = new[]
                {
                    stochastEntityTwo,
                    stochastEntityOne
                }
            };

            // Call
            GeneralResultSubMechanismIllustrationPoint generalResult = entity.Read();

            // Assert
            WindDirection actualGoverningWindDirection = generalResult.GoverningWindDirection;
            Assert.AreEqual(entity.GoverningWindDirectionName, actualGoverningWindDirection.Name);
            Assert.AreEqual(entity.GoverningWindDirectionAngle, actualGoverningWindDirection.Angle,
                            actualGoverningWindDirection.Angle.GetAccuracy());
            CollectionAssert.IsEmpty(generalResult.TopLevelSubMechanismIllustrationPoints);

            Stochast[] stochasts = generalResult.Stochasts.ToArray();
            Assert.AreEqual(2, stochasts.Length);
            AssertReadStochast(stochastEntityOne, stochasts[0]);
            AssertReadStochast(stochastEntityTwo, stochasts[1]);
        }

        [Test]
        public void Read_ValidEntityWithIllustrationPoints_ReturnsGeneralResultSubMechanismIllustrationPoint()
        {
            // Setup
            const string stochastName = "Stochast Name";
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            SubMechanismIllustrationPoint illustrationPoint = new TestSubMechanismIllustrationPoint();
            var illustrationPointEntityOne = new TopLevelSubMechanismIllustrationPointEntity
            {
                WindDirectionName = windDirection.Name,
                WindDirectionAngle = windDirection.Angle,
                ClosingSituation = stochastName,
                SubMechanismIllustrationPointEntity = new SubMechanismIllustrationPointEntity
                {
                    Beta = illustrationPoint.Beta,
                    Name = illustrationPoint.Name
                },
                Order = 0
            };
            var illustrationPointEntityTwo = new TopLevelSubMechanismIllustrationPointEntity
            {
                WindDirectionName = windDirection.Name,
                WindDirectionAngle = windDirection.Angle,
                ClosingSituation = stochastName,
                SubMechanismIllustrationPointEntity = new SubMechanismIllustrationPointEntity
                {
                    Beta = illustrationPoint.Beta,
                    Name = illustrationPoint.Name
                },
                Order = 1
            };

            var random = new Random(21);
            double windDirectionAngle = random.NextDouble();
            const string windDirectionName = "SSE";
            var entity = new GeneralResultSubMechanismIllustrationPointEntity
            {
                GoverningWindDirectionName = windDirectionName,
                GoverningWindDirectionAngle = windDirectionAngle,
                TopLevelSubMechanismIllustrationPointEntities = new[]
                {
                    illustrationPointEntityTwo,
                    illustrationPointEntityOne
                }
            };

            // Call
            GeneralResultSubMechanismIllustrationPoint generalResult = entity.Read();

            // Assert
            WindDirection actualGoverningWindDirection = generalResult.GoverningWindDirection;
            Assert.AreEqual(entity.GoverningWindDirectionName, actualGoverningWindDirection.Name);
            Assert.AreEqual(entity.GoverningWindDirectionAngle, actualGoverningWindDirection.Angle,
                            actualGoverningWindDirection.Angle.GetAccuracy());
            CollectionAssert.IsEmpty(generalResult.Stochasts);

            TopLevelSubMechanismIllustrationPoint[] illustrationPoints =
                generalResult.TopLevelSubMechanismIllustrationPoints.ToArray();
            Assert.AreEqual(2, illustrationPoints.Length);
            AssertReadTopLevelSubMechanismIllustrationPoint(illustrationPointEntityOne, illustrationPoints[0]);
            AssertReadTopLevelSubMechanismIllustrationPoint(illustrationPointEntityOne, illustrationPoints[1]);
        }

        private static void AssertReadTopLevelSubMechanismIllustrationPoint(
            TopLevelSubMechanismIllustrationPointEntity illustrationPointEntity,
            TopLevelSubMechanismIllustrationPoint readTopLevelSubMechanismIllustrationPoint)
        {
            Assert.AreEqual(illustrationPointEntity.ClosingSituation, readTopLevelSubMechanismIllustrationPoint.ClosingSituation);

            WindDirection actualWindDirection = readTopLevelSubMechanismIllustrationPoint.WindDirection;
            Assert.AreEqual(illustrationPointEntity.WindDirectionName, actualWindDirection.Name);
            Assert.AreEqual(illustrationPointEntity.WindDirectionAngle, actualWindDirection.Angle, actualWindDirection.Angle);

            Assert.IsNotNull(readTopLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint);
        }

        private static void AssertReadStochast(StochastEntity stochastEntity,
                                               Stochast readStochast)
        {
            Assert.AreEqual(stochastEntity.Name, readStochast.Name);
            Assert.AreEqual(stochastEntity.Alpha, readStochast.Alpha, readStochast.Alpha.GetAccuracy());
            Assert.AreEqual(stochastEntity.Duration, readStochast.Duration, readStochast.Duration.GetAccuracy());
        }
    }
}