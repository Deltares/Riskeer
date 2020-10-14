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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class TestPipingInputTest
    {
        [Test]
        public void Constructor_WithoutParameters_ExpectedValues()
        {
            // Call
            var pipingInput = new TestPipingInput();

            // Assert
            Assert.IsInstanceOf<PipingInput>(pipingInput);

            double expectedWaterVolumetricWeight = new GeneralPipingInput().WaterVolumetricWeight;
            Assert.AreEqual(expectedWaterVolumetricWeight, pipingInput.WaterVolumetricWeight);
        }

        [Test]
        public void Constructor_WithGeneralPipingInput_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var generalInputParameters = new GeneralPipingInput
            {
                WaterVolumetricWeight = random.NextRoundedDouble()
            };

            // Call
            var pipingInput = new TestPipingInput(generalInputParameters);

            // Assert
            Assert.IsInstanceOf<PipingInput>(pipingInput);

            Assert.AreEqual(generalInputParameters.WaterVolumetricWeight, pipingInput.WaterVolumetricWeight);
        }
    }
}