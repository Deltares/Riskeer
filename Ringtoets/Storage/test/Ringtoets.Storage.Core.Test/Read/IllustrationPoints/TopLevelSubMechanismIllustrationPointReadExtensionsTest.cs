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
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.IllustrationPoints;

namespace Ringtoets.Storage.Core.Test.Read.IllustrationPoints
{
    [TestFixture]
    public class TopLevelSubMechanismIllustrationPointReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((TopLevelSubMechanismIllustrationPointEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntity_ReturnsTopLevelSubMechanismIllustrationPoint()
        {
            // Setup
            var random = new Random(21);

            var subMechanismIllustrationPointEntity = new SubMechanismIllustrationPointEntity
            {
                Name = "Illustration point name",
                Beta = random.NextDouble()
            };

            var entity = new TopLevelSubMechanismIllustrationPointEntity
            {
                ClosingSituation = "closingSituation",
                WindDirectionName = "WindDirectionName",
                WindDirectionAngle = random.NextDouble(),
                SubMechanismIllustrationPointEntity = subMechanismIllustrationPointEntity
            };

            // Call
            TopLevelSubMechanismIllustrationPoint topLevelSubMechanismIllustrationPoint = entity.Read();

            // Assert
            Assert.AreEqual(entity.ClosingSituation, topLevelSubMechanismIllustrationPoint.ClosingSituation);

            WindDirection actualWindDirection = topLevelSubMechanismIllustrationPoint.WindDirection;
            Assert.AreEqual(entity.WindDirectionName, actualWindDirection.Name);
            Assert.AreEqual(entity.WindDirectionAngle, actualWindDirection.Angle,
                            actualWindDirection.Angle.GetAccuracy());

            SubMechanismIllustrationPoint actualIllustrationPoint = topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint;
            Assert.AreEqual(subMechanismIllustrationPointEntity.Name, actualIllustrationPoint.Name);
            Assert.AreEqual(subMechanismIllustrationPointEntity.Beta, actualIllustrationPoint.Beta,
                            actualIllustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(actualIllustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(actualIllustrationPoint.IllustrationPointResults);
        }
    }
}