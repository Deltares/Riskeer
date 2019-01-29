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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class CalculationWithLocationTest
    {
        [Test]
        public void Constructor_CalculationIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var location = new Point2D(0, 0);

            // Call
            TestDelegate test = () => new CalculationWithLocation(null, location);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void Constructor_LocationIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new CalculationWithLocation(calculation, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("location", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            var location = new Point2D(0, 0);

            // Call
            var calculationWithLocation = new CalculationWithLocation(calculation, location);

            // Assert
            Assert.AreSame(calculation, calculationWithLocation.Calculation);
            Assert.AreSame(location, calculationWithLocation.Location);
            mocks.VerifyAll();
        }
    }
}