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
        public void CreateWindDirection_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            double angle = random.NextDouble();
            var hydraWindDirection = new HydraWindDirection("name", angle);

            // Call
            WindDirection windDirection = WindDirectionConverter.CreateWindDirection(hydraWindDirection);

            // Assert
            Assert.AreEqual(hydraWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(hydraWindDirection.Name, windDirection.Name);
        }
    }
}