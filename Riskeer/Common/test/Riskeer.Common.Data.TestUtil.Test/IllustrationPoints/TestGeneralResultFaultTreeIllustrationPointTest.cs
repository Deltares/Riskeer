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

using System.Linq;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Data.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class TestGeneralResultFaultTreeIllustrationPointTest
    {
        [Test]
        public void Constructor_ReturnsExpectedElements()
        {
            // Call
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();

            // Assert
            Assert.IsInstanceOf<GeneralResult<TopLevelFaultTreeIllustrationPoint>>(generalResult);
            AssertWindDirection(WindDirectionTestFactory.CreateTestWindDirection(), generalResult.GoverningWindDirection);
            Assert.AreEqual("A", generalResult.Stochasts.First().Name);
            Assert.AreEqual(5.0, generalResult.Stochasts.First().Alpha);
            Assert.AreEqual(10.0, generalResult.Stochasts.First().Duration);
            Assert.AreEqual(generalResult.TopLevelIllustrationPoints.Count(), 1);
            AssertWindDirection(WindDirectionTestFactory.CreateTestWindDirection(), generalResult.TopLevelIllustrationPoints.First().WindDirection);
            Assert.AreEqual("closing situation", generalResult.TopLevelIllustrationPoints.First().ClosingSituation);
            var illustrationPointNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
            Assert.AreEqual(illustrationPointNode.Data.Name, generalResult.TopLevelIllustrationPoints.First().FaultTreeNodeRoot.Data.Name);
            Assert.AreEqual(illustrationPointNode.Data.Beta, generalResult.TopLevelIllustrationPoints.First().FaultTreeNodeRoot.Data.Beta);
            CollectionAssert.IsEmpty(generalResult.TopLevelIllustrationPoints.First().FaultTreeNodeRoot.Children);
        }

        private static void AssertWindDirection(WindDirection expected, WindDirection actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Angle, actual.Angle);
        }
    }
}