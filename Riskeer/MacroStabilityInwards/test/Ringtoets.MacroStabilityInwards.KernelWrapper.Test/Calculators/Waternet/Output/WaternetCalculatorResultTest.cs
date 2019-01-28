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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.Waternet.Output
{
    [TestFixture]
    public class WaternetCalculatorResultTest
    {
        [Test]
        public void Constructor_PhreaticLinesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WaternetCalculatorResult(null, new WaternetLineResult[0]);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("phreaticLines", exception.ParamName);
        }

        [Test]
        public void Constructor_WaternetLinesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WaternetCalculatorResult(new WaternetPhreaticLineResult[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waternetLines", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var phreaticLines = new WaternetPhreaticLineResult[0];
            var waternetLines = new WaternetLineResult[0];

            // Call
            var result = new WaternetCalculatorResult(phreaticLines, waternetLines);

            // Assert
            Assert.AreSame(phreaticLines, result.PhreaticLines);
            Assert.AreSame(waternetLines, result.WaternetLines);
        }
    }
}