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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Hydraulics.IllustrationPoints
{
    [TestFixture]
    public class WindDirectionTest
    {
        [Test]
        public void Constructor_NameNull_ThrownsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            double value = random.NextDouble();

            // Call
            TestDelegate call = () => new WindDirection(null, value);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(360.005)]
        [TestCase(-0.005)]
        public void Constructor_InvalidWindDirection_ThrowsArgumentOutOfRangeException(double windDirectionAngle)
        {
            // Call
            TestDelegate call = () => new WindDirection("SSE", windDirectionAngle);

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            const string expectedErrorMessage = "De waarde voor de windrichting moet in het bereik van [0.00, 360.00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedErrorMessage);
            Assert.AreEqual(expectedErrorMessage, exception.Message);
        }

        [Test]
        [TestCase(360.004)]
        [TestCase(double.NaN)]
        [TestCase(50)]
        [TestCase(-0.004)]
        public void Constructor_ValidWindDirection_ReturnsExpectedValues(double windDirectionAngle)
        {
            // Call
            var windDirection = new WindDirection("SSE", windDirectionAngle);

            // Assert
            Assert.AreEqual(windDirectionAngle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(2, windDirection.Angle.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_ValidValues_ReturnsExpectedValues()
        {
            // Setup
            const string windDirectionName = "SSE";

            var random = new Random(21);
            double value = random.NextDouble();

            // Call
            var windDirection = new WindDirection(windDirectionName, value);

            // Assert
            Assert.AreEqual(windDirectionName, windDirection.Name);
            Assert.AreEqual(value, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(2, windDirection.Angle.NumberOfDecimalPlaces);
        }
    }
}