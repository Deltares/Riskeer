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
using Core.Common.Base.Data;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.Test
{
    public class MacroStabilityInwardsLocationInputBaseTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var locationInput = new TestMacroStabilityInwardsLocationInput();

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsLocationInput>(locationInput);
            Assert.IsInstanceOf<ICloneable>(locationInput);

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
            var random = new Random(21);
            double phreaticLineOffsetBelowDikeTopAtRiver = random.NextDouble();
            double phreaticLineOffsetBelowDikeTopAtPolder = random.NextDouble();
            double phreaticLineOffsetBelowShoulderBaseInside = random.NextDouble();
            double phreaticLineOffsetBelowDikeToeAtPolder = random.NextDouble();

            // Call
            var locationInput = new TestMacroStabilityInwardsLocationInput
            {
                PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) phreaticLineOffsetBelowDikeTopAtPolder,
                PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) phreaticLineOffsetBelowDikeToeAtPolder,
                PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) phreaticLineOffsetBelowDikeTopAtRiver,
                PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) phreaticLineOffsetBelowShoulderBaseInside
            };

            // Assert
            Assert.AreEqual(2, locationInput.PhreaticLineOffsetBelowDikeTopAtPolder.NumberOfDecimalPlaces);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, locationInput.PhreaticLineOffsetBelowDikeTopAtPolder,
                            locationInput.PhreaticLineOffsetBelowDikeTopAtPolder.GetAccuracy());

            Assert.AreEqual(2, locationInput.PhreaticLineOffsetBelowDikeToeAtPolder.NumberOfDecimalPlaces);
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, locationInput.PhreaticLineOffsetBelowDikeToeAtPolder,
                            locationInput.PhreaticLineOffsetBelowDikeToeAtPolder.GetAccuracy());

            Assert.AreEqual(2, locationInput.PhreaticLineOffsetBelowDikeTopAtRiver.NumberOfDecimalPlaces);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, locationInput.PhreaticLineOffsetBelowDikeTopAtRiver,
                            locationInput.PhreaticLineOffsetBelowDikeTopAtRiver.GetAccuracy());

            Assert.AreEqual(2, locationInput.PhreaticLineOffsetBelowShoulderBaseInside.NumberOfDecimalPlaces);
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, locationInput.PhreaticLineOffsetBelowShoulderBaseInside,
                            locationInput.PhreaticLineOffsetBelowShoulderBaseInside.GetAccuracy());
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            MacroStabilityInwardsLocationInputBase original = new TestMacroStabilityInwardsLocationInput
            {
                PhreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble(),
                PhreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble(),
                PhreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble(),
                PhreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble()
            };

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }

        private class TestMacroStabilityInwardsLocationInput : MacroStabilityInwardsLocationInputBase {}
    }
}