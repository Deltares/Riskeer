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
using Core.Common.Base;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.DuneErosion.Data.TestUtil;

namespace Riskeer.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneLocationCalculationTest
    {
        [Test]
        public void Constructor_DuneLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DuneLocationCalculation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("duneLocation", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var duneLocation = new TestDuneLocation();

            // Call
            var calculation = new DuneLocationCalculation(duneLocation);

            // Assert
            Assert.IsInstanceOf<ICalculatable>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreSame(duneLocation, calculation.DuneLocation);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void ShouldCalculate_OutputNull_ReturnsTrue()
        {
            // Setup
            var calculation = new DuneLocationCalculation(new TestDuneLocation())
            {
                Output = null
            };

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsTrue(shouldCalculate);
        }

        [Test]
        public void ShouldCalculate_OutputSet_ReturnsFalse()
        {
            // Setup
            var calculation = new DuneLocationCalculation(new TestDuneLocation())
            {
                Output = new TestDuneLocationCalculationOutput()
            };

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsFalse(shouldCalculate);
        }
    }
}