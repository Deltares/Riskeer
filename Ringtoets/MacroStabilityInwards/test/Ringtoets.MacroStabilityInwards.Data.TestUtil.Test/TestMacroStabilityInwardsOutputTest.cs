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

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class TestMacroStabilityInwardsOutputTest
    {
        [Test]
        public void ParameterlessConstructor_ExpectedValues()
        {
            // Call
            var output = new TestMacroStabilityInwardsOutput();

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsOutput>(output);
            Assert.AreEqual(1.1, output.FactorOfStability);
            Assert.IsNotNull(output.SlidingCurve);
            Assert.IsNotNull(output.SlipPlane);
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var random = new Random(11);
            double factorOfStability = random.NextDouble();
            double zValue = random.NextDouble();
            double forbiddenZonesXEntryMax = random.NextDouble();
            double forbiddenZonesXEntryMin = random.NextDouble();

            var properties = new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = factorOfStability,
                ZValue = zValue,
                ForbiddenZonesXEntryMax = forbiddenZonesXEntryMax,
                ForbiddenZonesXEntryMin = forbiddenZonesXEntryMin
            };

            // Call
            var output = new TestMacroStabilityInwardsOutput(properties);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsOutput>(output);
            Assert.AreEqual(factorOfStability, output.FactorOfStability);
            Assert.AreEqual(zValue, output.ZValue);
            Assert.AreEqual(forbiddenZonesXEntryMax, output.ForbiddenZonesXEntryMax);
            Assert.AreEqual(forbiddenZonesXEntryMin, output.ForbiddenZonesXEntryMin);
        }
    }
}