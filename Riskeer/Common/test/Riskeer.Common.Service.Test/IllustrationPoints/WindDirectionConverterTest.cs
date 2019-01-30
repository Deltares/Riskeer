// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.IllustrationPoints;
using HydraRingWindDirection = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;

namespace Riskeer.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class WindDirectionConverterTest
    {
        [Test]
        public void Convert_HydraRingWindDirectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => WindDirectionConverter.Convert(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingWindDirection", paramName);
        }

        [Test]
        public void Convert_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraRingWindDirection = new HydraRingWindDirection("name", random.NextDouble());

            // Call
            WindDirection windDirection = WindDirectionConverter.Convert(hydraRingWindDirection);

            // Assert
            Assert.AreEqual(hydraRingWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(hydraRingWindDirection.Name, windDirection.Name);
        }
    }
}