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
    public class TopLevelSubMechanismIllustrationPointTest
    {
        [Test]
        public void CreateTopLevelSubMechanismIllustrationPointEntity_TopLevelSubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((TopLevelSubMechanismIllustrationPoint) null).CreateTopLevelSubMechanismIllustrationPointEntity(0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topLevelSubMechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void CreateTopLevelSubMechanismIllustrationPointEntity_ValidTopLevelSubMechanismIllustrationPoint_ReturnsEntity()
        {
            // Setup
            var random = new Random(21);

            const string illustrationPointName = "Illustration point name";
            double beta = random.NextDouble();
            var illustrationPoint = new SubMechanismIllustrationPoint(illustrationPointName,
                                                                      Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                      Enumerable.Empty<IllustrationPointResult>(),
                                                                      beta);

            const string windDirectionName = "WindDirection Name";
            double angle = random.NextDouble();
            var windDirection = new WindDirection(windDirectionName, angle);

            const string closingSituation = "Just a situation";
            var topLevelSubMechanismIllustrationPoint = new TopLevelSubMechanismIllustrationPoint(windDirection,
                                                                                                  closingSituation,
                                                                                                  illustrationPoint);
            int order = random.Next();

            // Call
            TopLevelSubMechanismIllustrationPointEntity entity =
                topLevelSubMechanismIllustrationPoint.CreateTopLevelSubMechanismIllustrationPointEntity(order);

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