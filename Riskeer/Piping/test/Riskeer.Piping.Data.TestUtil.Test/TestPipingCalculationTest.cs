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

using NUnit.Framework;

namespace Riskeer.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class TestPipingCalculationTest
    {
        [Test]
        public void Constructor_WithoutParameters_ExpectedValues()
        {
            // Call
            var calculation = new TestPipingCalculation();

            // Assert
            Assert.IsInstanceOf<PipingCalculation<PipingInput>>(calculation);
            Assert.IsInstanceOf<PipingInput>(calculation.InputParameters);
        }

        [Test]
        public void Constructor_WithPipingInput_ExpectedValues()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            var calculation = new TestPipingCalculation(pipingInput);

            // Assert
            Assert.IsInstanceOf<PipingCalculation<PipingInput>>(calculation);
            Assert.AreSame(pipingInput, calculation.InputParameters);
        }
    }
}