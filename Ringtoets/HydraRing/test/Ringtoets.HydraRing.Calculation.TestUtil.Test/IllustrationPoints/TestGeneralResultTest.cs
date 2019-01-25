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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class TestGeneralResultTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var generalResult = new TestGeneralResult();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0, generalResult.Beta);

            var expectedWindDirection = new TestWindDirection();
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
            CollectionAssert.IsEmpty(generalResult.IllustrationPoints);
        }

        [Test]
        public void CreateGeneralResultWithFaultTreeIllustrationPoints_ExpectedProperties()
        {
            // Call
            TestGeneralResult generalResult = TestGeneralResult.CreateGeneralResultWithFaultTreeIllustrationPoints();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0, generalResult.Beta);

            var expectedWindDirection = new TestWindDirection();
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.Stochasts);

            KeyValuePair<WindDirectionClosingSituation, IllustrationPointTreeNode> topLevelIllustrationPoint =
                generalResult.IllustrationPoints.Single();
            WindDirectionClosingSituation actualWindDirectionClosingSituation = topLevelIllustrationPoint.Key;
            AssertWindDirection(expectedWindDirection, actualWindDirectionClosingSituation.WindDirection);
            Assert.AreEqual("closing situation", actualWindDirectionClosingSituation.ClosingSituation);
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(topLevelIllustrationPoint.Value.Data);
        }

        [Test]
        public void CreateGeneralResultWithSubMechanismIllustrationPoints_ExpectedProperties()
        {
            // Call
            TestGeneralResult generalResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0, generalResult.Beta);

            var expectedWindDirection = new TestWindDirection();
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.Stochasts);

            KeyValuePair<WindDirectionClosingSituation, IllustrationPointTreeNode> topLevelIllustrationPoint =
                generalResult.IllustrationPoints.Single();
            WindDirectionClosingSituation actualWindDirectionClosingSituation = topLevelIllustrationPoint.Key;
            AssertWindDirection(expectedWindDirection, actualWindDirectionClosingSituation.WindDirection);
            Assert.AreEqual("closing situation", actualWindDirectionClosingSituation.ClosingSituation);
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(topLevelIllustrationPoint.Value.Data);
        }

        private static void AssertWindDirection(WindDirection expected, WindDirection actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Angle, actual.Angle);
        }
    }
}