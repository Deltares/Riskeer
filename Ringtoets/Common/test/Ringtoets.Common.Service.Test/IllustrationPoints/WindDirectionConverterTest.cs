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
using Ringtoets.Common.Service.IllustrationPoints;
using HydraWindDirection = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class WindDirectionConverterTest
    {
        [Test]
        public void CreateWindDirection_HydraWindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => WindDirectionConverter.CreateWindDirection(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraWindDirection", paramName);
        }

        [Test]
        public void CreateWindDirection_HydraWindDirectionNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraWindDirection = new HydraWindDirection
            {
                Angle = 0
            };

            // Call
            TestDelegate call = () => WindDirectionConverter.CreateWindDirection(hydraWindDirection);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(361)]
        [SetCulture("nl-NL")]
        public void CreateWindDirection_InvalidAngle_ThrowsArgumentOutOfRangeException(double angle)
        {
            // Setup
            var hydraWindDirection = new HydraWindDirection
            {
                Angle = angle,
                Name = "Name"
            };

            // Call
            TestDelegate call = () => WindDirectionConverter.CreateWindDirection(hydraWindDirection);

            // Assert
            const string expectedErrorMessage = "De waarde voor de windrichting moet in het bereik van [0,00, 360,00] liggen.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedErrorMessage);
            Assert.AreEqual("angle", exception.ParamName);
        }

        [Test]
        public void CreateWindDirection_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraWindDirection = new HydraWindDirection
            {
                Angle = random.GetFromRange(0.0, 360.0),
                Name = "Name"
            };

            // Call
            WindDirection windDirection = WindDirectionConverter.CreateWindDirection(hydraWindDirection);

            // Assert
            Assert.AreEqual(hydraWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(hydraWindDirection.Name, windDirection.Name);
        }
    }
}