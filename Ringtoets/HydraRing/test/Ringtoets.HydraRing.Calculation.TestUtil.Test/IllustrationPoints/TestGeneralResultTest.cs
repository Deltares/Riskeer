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

using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

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

            var expectedWindDirection = new WindDirection
            {
                Name = "TestWindDirection"
            };
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWind);
            Assert.IsEmpty(generalResult.Stochasts);
            Assert.IsEmpty(generalResult.IllustrationPoints);
        }

        private static void AssertWindDirection(WindDirection expected, WindDirection actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Angle, actual.Angle);
        }
    }
}