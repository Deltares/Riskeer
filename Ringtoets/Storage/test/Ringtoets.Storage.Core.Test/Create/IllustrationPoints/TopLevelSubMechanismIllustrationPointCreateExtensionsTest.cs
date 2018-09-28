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
using Ringtoets.Storage.Core.Create.IllustrationPoints;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.IllustrationPoints
{
    [TestFixture]
    public class TopLevelSubMechanismIllustrationPointCreateExtensionsTest
    {
        [Test]
        public void Create_TopLevelSubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((TopLevelSubMechanismIllustrationPoint) null).Create(0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topLevelSubMechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void Create_ValidTopLevelSubMechanismIllustrationPoint_ReturnsEntityWithSubMechanismIllustrationPointEntity()
        {
            // Setup
            var random = new Random(21);
            var illustrationPoint = new SubMechanismIllustrationPoint("Illustration point name",
                                                                      random.NextDouble(),
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                      Enumerable.Empty<IllustrationPointResult>());

            var windDirection = new WindDirection("WindDirection Name", random.NextDouble());
            var topLevelSubMechanismIllustrationPoint = new TopLevelSubMechanismIllustrationPoint(windDirection,
                                                                                                  "Just a situation",
                                                                                                  illustrationPoint);
            int order = random.Next();

            // Call
            TopLevelSubMechanismIllustrationPointEntity entity =
                topLevelSubMechanismIllustrationPoint.Create(order);

            // Assert
            TestHelper.AssertAreEqualButNotSame(topLevelSubMechanismIllustrationPoint.ClosingSituation, entity.ClosingSituation);
            Assert.AreEqual(order, entity.Order);

            TestHelper.AssertAreEqualButNotSame(windDirection.Name, entity.WindDirectionName);
            Assert.AreEqual(windDirection.Angle, entity.WindDirectionAngle, windDirection.Angle.GetAccuracy());

            SubMechanismIllustrationPointEntity subMechanismIllustrationPointEntity = entity.SubMechanismIllustrationPointEntity;
            TestHelper.AssertAreEqualButNotSame(illustrationPoint.Name, subMechanismIllustrationPointEntity.Name);
            Assert.AreEqual(illustrationPoint.Beta, subMechanismIllustrationPointEntity.Beta, illustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(subMechanismIllustrationPointEntity.IllustrationPointResultEntities);
            CollectionAssert.IsEmpty(subMechanismIllustrationPointEntity.SubMechanismIllustrationPointStochastEntities);
        }
    }
}