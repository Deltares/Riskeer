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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    public class MacroStabilityInwardsLocationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var locationInput = new TestMacroStabilityInwardsLocationInput();

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsLocationInput>(locationInput);
            Assert.IsTrue(locationInput.UseDefaultOffsets);

            Assert.IsNaN(locationInput.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(2, locationInput.PhreaticLineOffsetBelowDikeTopAtRiver.NumberOfDecimalPlaces);

            Assert.IsNaN(locationInput.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(2, locationInput.PhreaticLineOffsetBelowDikeTopAtPolder.NumberOfDecimalPlaces);

            Assert.IsNaN(locationInput.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(2, locationInput.PhreaticLineOffsetBelowShoulderBaseInside.NumberOfDecimalPlaces);

            Assert.IsNaN(locationInput.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(2, locationInput.PhreaticLineOffsetBelowDikeToeAtPolder.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_SetProperties_ExpectedValues()
        {
            // Setup
            var random = new Random();
            double phreaticLineOffsetBelowDikeTopAtRiver = random.Next();
            double phreaticLineOffsetBelowDikeTopAtPolder = random.Next();
            double phreaticLineOffsetBelowShoulderBaseInside = random.Next();
            double phreaticLineOffsetBelowDikeToeAtPolder = random.Next();

            // Call
            var locationInput = new TestMacroStabilityInwardsLocationInput
            {
                UseDefaultOffsets = false,
                PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) phreaticLineOffsetBelowDikeTopAtPolder,
                PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) phreaticLineOffsetBelowDikeToeAtPolder,
                PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) phreaticLineOffsetBelowDikeTopAtRiver,
                PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) phreaticLineOffsetBelowShoulderBaseInside
            };

            // Assert
            Assert.IsFalse(locationInput.UseDefaultOffsets);
            Assert.AreEqual(new RoundedDouble(2, phreaticLineOffsetBelowDikeTopAtPolder), locationInput.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(new RoundedDouble(2, phreaticLineOffsetBelowDikeToeAtPolder), locationInput.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(new RoundedDouble(2, phreaticLineOffsetBelowDikeTopAtRiver), locationInput.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(new RoundedDouble(2, phreaticLineOffsetBelowShoulderBaseInside), locationInput.PhreaticLineOffsetBelowShoulderBaseInside);
        }

        private class TestMacroStabilityInwardsLocationInput : MacroStabilityInwardsLocationInput {}
    }
}