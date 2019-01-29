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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.IllustrationPoints;

namespace Riskeer.Storage.Core.Test.Read.IllustrationPoints
{
    [TestFixture]
    public class TopLevelFaultTreeIllustrationPointEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((TopLevelFaultTreeIllustrationPointEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntity_ReturnsTopLevelFaultTreeIllustrationPoint()
        {
            // Setup
            var random = new Random(21);

            var combinationType = random.NextEnumValue<CombinationType>();
            var illustrationPointEntity = new FaultTreeIllustrationPointEntity
            {
                Name = "Illustration point name",
                Beta = random.NextDouble(),
                CombinationType = Convert.ToByte(combinationType)
            };

            var entity = new TopLevelFaultTreeIllustrationPointEntity
            {
                ClosingSituation = "closingSituation",
                WindDirectionName = "WindDirectionName",
                WindDirectionAngle = random.NextDouble(),
                FaultTreeIllustrationPointEntity = illustrationPointEntity
            };

            // Call
            TopLevelFaultTreeIllustrationPoint topLevelIllustrationPoint = entity.Read();

            // Assert
            Assert.AreEqual(entity.ClosingSituation, topLevelIllustrationPoint.ClosingSituation);

            WindDirection actualWindDirection = topLevelIllustrationPoint.WindDirection;
            Assert.AreEqual(entity.WindDirectionName, actualWindDirection.Name);
            Assert.AreEqual(entity.WindDirectionAngle, actualWindDirection.Angle,
                            actualWindDirection.Angle.GetAccuracy());

            IllustrationPointNode rootNode = topLevelIllustrationPoint.FaultTreeNodeRoot;
            var illustrationPoint = rootNode.Data as FaultTreeIllustrationPoint;
            Assert.IsNotNull(illustrationPoint);
            Assert.AreEqual(illustrationPointEntity.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(illustrationPointEntity.Name, illustrationPoint.Name);
            Assert.AreEqual(combinationType, illustrationPoint.CombinationType);

            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(rootNode.Children);
        }
    }
}