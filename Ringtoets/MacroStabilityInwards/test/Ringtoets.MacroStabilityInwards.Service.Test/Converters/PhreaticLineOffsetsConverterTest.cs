﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.Service.Converters;

namespace Ringtoets.MacroStabilityInwards.Service.Test.Converters
{
    [TestFixture]
    public class PhreaticLineOffsetsConverterTest
    {
        [Test]
        public void Convert_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PhreaticLineOffsetsConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Convert_UseDefaultOffsetsTrue_ReturnUpliftVanPhreaticLineOffsets()
        {
            // Setup
            var random = new Random(11);
            var input = new MacroStabilityInwardsLocationInputExtreme
            {
                UseDefaultOffsets = true,
                PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble(),
                PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble(),
                PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble(),
                PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble()
            };

            // Call
            PhreaticLineOffsets offsets = PhreaticLineOffsetsConverter.Convert(input);

            // Assert
            Assert.IsTrue(offsets.UseDefaults);
            Assert.IsNaN(offsets.BelowDikeTopAtRiver);
            Assert.IsNaN(offsets.BelowDikeTopAtPolder);
            Assert.IsNaN(offsets.BelowDikeToeAtPolder);
            Assert.IsNaN(offsets.BelowShoulderBaseInside);
        }

        [Test]
        public void Convert_UseDefaultOffsetsFalse_ReturnUpliftVanPhreaticLineOffsets()
        {
            // Setup
            var random = new Random(11);
            var input = new MacroStabilityInwardsLocationInputExtreme
            {
                UseDefaultOffsets = false,
                PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble(),
                PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble(),
                PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble(),
                PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble()
            };

            // Call
            PhreaticLineOffsets offsets = PhreaticLineOffsetsConverter.Convert(input);

            // Assert
            Assert.IsFalse(offsets.UseDefaults);
            Assert.AreEqual(input.PhreaticLineOffsetBelowDikeTopAtRiver, offsets.BelowDikeTopAtRiver);
            Assert.AreEqual(input.PhreaticLineOffsetBelowDikeTopAtPolder, offsets.BelowDikeTopAtPolder);
            Assert.AreEqual(input.PhreaticLineOffsetBelowDikeToeAtPolder, offsets.BelowDikeToeAtPolder);
            Assert.AreEqual(input.PhreaticLineOffsetBelowShoulderBaseInside, offsets.BelowShoulderBaseInside);
        }
    }
}