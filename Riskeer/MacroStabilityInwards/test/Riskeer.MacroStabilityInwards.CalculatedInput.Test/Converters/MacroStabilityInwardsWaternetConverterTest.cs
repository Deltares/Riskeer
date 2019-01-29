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
using Ringtoets.MacroStabilityInwards.CalculatedInput.Converters;
using Ringtoets.MacroStabilityInwards.CalculatedInput.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Output;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.CalculatedInput.Test.Converters
{
    [TestFixture]
    public class MacroStabilityInwardsWaternetConverterTest
    {
        [Test]
        public void Convert_WaternetCalculatorResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsWaternetConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculatorResult", exception.ParamName);
        }

        [Test]
        public void Convert_WithResult_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            WaternetCalculatorResult result = WaternetCalculatorResultTestFactory.Create();

            // Call
            MacroStabilityInwardsWaternet converted = MacroStabilityInwardsWaternetConverter.Convert(result);

            // Assert
            CalculatorOutputAssert.AssertWaternet(result, converted);
        }
    }
}