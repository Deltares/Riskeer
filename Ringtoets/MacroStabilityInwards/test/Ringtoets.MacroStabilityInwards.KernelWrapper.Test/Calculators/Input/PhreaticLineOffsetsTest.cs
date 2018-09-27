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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.Input
{
    [TestFixture]
    public class PhreaticLineOffsetsTest
    {
        [Test]
        public void ParameterlessConstructor_ExpectedValues()
        {
            // Call
            var offsets = new PhreaticLineOffsets();

            // Assert
            Assert.IsTrue(offsets.UseDefaults);
            Assert.IsNaN(offsets.BelowDikeTopAtRiver);
            Assert.IsNaN(offsets.BelowDikeTopAtPolder);
            Assert.IsNaN(offsets.BelowDikeToeAtPolder);
            Assert.IsNaN(offsets.BelowShoulderBaseInside);
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var random = new Random(11);
            double belowDikeTopAtRiver = random.NextDouble();
            double belowDikeTopAtPolder = random.NextDouble();
            double belowDikeToeAtPolder = random.NextDouble();
            double belowShoulderBaseInside = random.NextDouble();

            // Call
            var offsets = new PhreaticLineOffsets(belowDikeTopAtRiver, belowDikeTopAtPolder, belowDikeToeAtPolder, belowShoulderBaseInside);

            // Assert
            Assert.IsFalse(offsets.UseDefaults);
            Assert.AreEqual(belowDikeTopAtRiver, offsets.BelowDikeTopAtRiver);
            Assert.AreEqual(belowDikeTopAtPolder, offsets.BelowDikeTopAtPolder);
            Assert.AreEqual(belowDikeToeAtPolder, offsets.BelowDikeToeAtPolder);
            Assert.AreEqual(belowShoulderBaseInside, offsets.BelowShoulderBaseInside);
        }
    }
}