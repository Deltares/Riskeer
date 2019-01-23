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
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Storage.Core.Create.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class TopLevelFaultTreeIllustrationPointCreateExtensionsTest
    {
        [Test]
        public void Create_TopLevelFaultTreeIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((TopLevelFaultTreeIllustrationPoint) null).Create(0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topLevelFaultTreeIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void Create_ValidTopLevelFaultTreeIllustrationPoint_ReturnsTopLevelFaultTreeIllustrationPointEntity()
        {
            // Setup
            var random = new Random(21);

            var windDirection = new WindDirection("WindDirection Name", random.NextDouble());
            var illustrationPoint = new TopLevelFaultTreeIllustrationPoint(
                windDirection,
                "Just a situation",
                new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPoint()));
            int order = random.Next();

            // Call
            TopLevelFaultTreeIllustrationPointEntity entity = illustrationPoint.Create(order);

            // Assert
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.ClosingSituation, entity.ClosingSituation);
            TestHelper.AssertAreEqualButNotSame(windDirection.Name, entity.WindDirectionName);
            Assert.AreEqual(windDirection.Angle, entity.WindDirectionAngle, windDirection.Angle.GetAccuracy());
            CollectionAssert.IsEmpty(entity.FaultTreeIllustrationPointEntity.SubMechanismIllustrationPointEntities);
            CollectionAssert.IsEmpty(entity.FaultTreeIllustrationPointEntity.FaultTreeIllustrationPointEntity1);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_ValidTopLevelFaultTreeIllustrationPointWithChildren_ReturnsTopLevelFaultTreeIllustrationPointEntityWithChildren()
        {
            // Setup
            var random = new Random(21);

            var windDirection = new WindDirection("WindDirection Name", random.NextDouble());
            var illustrationPointNode = new IllustrationPointNode(FaultTreeIllustrationPointTestFactory.CreateTestFaultTreeIllustrationPoint());
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("Point A")),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("Point B"))
            });

            var topLevelIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(
                windDirection,
                "Just a situation",
                illustrationPointNode);
            int order = random.Next();

            // Call
            TopLevelFaultTreeIllustrationPointEntity entity = topLevelIllustrationPoint.Create(order);

            // Assert
            TestHelper.AssertAreEqualButNotSame(topLevelIllustrationPoint.ClosingSituation, entity.ClosingSituation);
            TestHelper.AssertAreEqualButNotSame(windDirection.Name, entity.WindDirectionName);
            Assert.AreEqual(windDirection.Angle, entity.WindDirectionAngle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(order, entity.Order);

            SubMechanismIllustrationPointEntity[] subMechanismIllustrationPointEntities =
                entity.FaultTreeIllustrationPointEntity.SubMechanismIllustrationPointEntities.ToArray();
            Assert.AreEqual(2, subMechanismIllustrationPointEntities.Length);

            for (var i = 0; i < 2; i++)
            {
                var expectedIllustrationPoint = new TestSubMechanismIllustrationPoint(subMechanismIllustrationPointEntities[i].Name);
                SubMechanismIllustrationPointEntity illustrationPointEntity = subMechanismIllustrationPointEntities[i];
                Assert.AreEqual(expectedIllustrationPoint.Name, illustrationPointEntity.Name);
                Assert.AreEqual(expectedIllustrationPoint.Beta, illustrationPointEntity.Beta, expectedIllustrationPoint.Beta.GetAccuracy());
                Assert.AreEqual(expectedIllustrationPoint.IllustrationPointResults.Count(),
                                illustrationPointEntity.IllustrationPointResultEntities.Count);
                Assert.AreEqual(expectedIllustrationPoint.Stochasts.Count(),
                                illustrationPointEntity.IllustrationPointResultEntities.Count);
                Assert.AreEqual(i, illustrationPointEntity.Order);
            }
        }
    }
}