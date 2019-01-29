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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.CalculatedInput.Converters;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.CalculatedInput.Test.Converters
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
        public void Convert_UseDefaultOffsetsTrue_ReturnPhreaticLineOffsets()
        {
            // Setup
            var random = new Random(11);
            var mockRepository = new MockRepository();
            var input = mockRepository.Stub<IMacroStabilityInwardsLocationInput>();
            mockRepository.ReplayAll();

            input.UseDefaultOffsets = true;
            input.PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble();
            input.PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble();
            input.PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble();
            input.PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble();

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
        public void Convert_UseDefaultOffsetsFalse_ReturnPhreaticLineOffsets()
        {
            // Setup
            var random = new Random(11);
            var mockRepository = new MockRepository();
            var input = mockRepository.Stub<IMacroStabilityInwardsLocationInput>();
            mockRepository.ReplayAll();

            input.UseDefaultOffsets = false;
            input.PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble();
            input.PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble();
            input.PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble();
            input.PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble();

            // Call
            PhreaticLineOffsets offsets = PhreaticLineOffsetsConverter.Convert(input);

            // Assert
            Assert.IsFalse(offsets.UseDefaults);
            Assert.AreEqual(input.PhreaticLineOffsetBelowDikeTopAtRiver, offsets.BelowDikeTopAtRiver);
            Assert.AreEqual(input.PhreaticLineOffsetBelowDikeTopAtPolder, offsets.BelowDikeTopAtPolder);
            Assert.AreEqual(input.PhreaticLineOffsetBelowDikeToeAtPolder, offsets.BelowDikeToeAtPolder);
            Assert.AreEqual(input.PhreaticLineOffsetBelowShoulderBaseInside, offsets.BelowShoulderBaseInside);

            mockRepository.VerifyAll();
        }
    }
}