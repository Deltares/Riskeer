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

using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class TestTopLevelIllustrationPointTest
    {
        [Test]
        public void DefaultConstructor_ReturnsExpectedProperties()
        {
            // Call
            var topLevelIllustrationPoint = new TestTopLevelIllustrationPoint();

            // Assert
            Assert.IsInstanceOf<TopLevelIllustrationPointBase>(topLevelIllustrationPoint);
            Assert.AreEqual("Closing situation", topLevelIllustrationPoint.ClosingSituation);

            WindDirection expectedWindDirection = WindDirectionTestFactory.CreateTestWindDirection();
            AssertWindDirection(expectedWindDirection, topLevelIllustrationPoint.WindDirection);
        }

        [Test]
        public void Constructor_WithClosingSituation_ReturnsExpectedProperties()
        {
            // Call
            var topLevelIllustrationPoint = new TestTopLevelIllustrationPoint("test situation");

            // Assert
            Assert.IsInstanceOf<TopLevelIllustrationPointBase>(topLevelIllustrationPoint);
            Assert.AreEqual("test situation", topLevelIllustrationPoint.ClosingSituation);

            WindDirection expectedWindDirection = WindDirectionTestFactory.CreateTestWindDirection();
            AssertWindDirection(expectedWindDirection, topLevelIllustrationPoint.WindDirection);
        }

        private static void AssertWindDirection(WindDirection expected, WindDirection actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Angle, actual.Angle);
        }
    }
}