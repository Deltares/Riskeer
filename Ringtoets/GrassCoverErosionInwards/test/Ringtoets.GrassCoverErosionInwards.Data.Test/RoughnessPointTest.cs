// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class RoughnessPointTest
    {
        [Test]
        [TestCase(0.567896)]
        [TestCase(0.5 - 1.0e-3, Description = "Valid roughness due to rounding to 0.5")]
        [TestCase(1.0 + 1.0e-3, Description = "Valid roughness due to rounding to 1.0")]
        public void Constructor_ValidRoughness_ExpectedValues(double roughness)
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            var roughnessPoint = new RoughnessPoint(point, roughness);

            // Assert
            Assert.AreEqual(point, roughnessPoint.Point);
            var numberOfDecimalPlaces = roughnessPoint.Roughness.NumberOfDecimalPlaces;
            Assert.AreEqual(2, numberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(numberOfDecimalPlaces, roughness), roughnessPoint.Roughness);
        }

        [Test]
        [TestCase(0.5 - 1.0e-2)]
        [TestCase(1.0 + 1.0e-2)]
        public void Constructor_IllegalRoughness_ThrowsArgumentOutOfRangeException(double roughness)
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            TestDelegate call = () => new RoughnessPoint(point, roughness);

            // Assert
            string expectedMessage = String.Format("De ruwheid waarde {0} moet in het interval [0.5, 1.0] liggen.",
                                                   new RoundedDouble(2, roughness));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_PointNull_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => new RoughnessPoint(null, 0.0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("point", exception.ParamName);
        }
    }
}
