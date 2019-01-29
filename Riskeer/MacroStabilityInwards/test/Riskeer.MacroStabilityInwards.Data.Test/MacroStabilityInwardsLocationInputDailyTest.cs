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

using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.Test
{
    public class MacroStabilityInwardsLocationInputDailyTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var locationInput = new MacroStabilityInwardsLocationInputDaily();

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsLocationInputBase>(locationInput);
            Assert.IsInstanceOf<IMacroStabilityInwardsLocationInputDaily>(locationInput);

            Assert.AreEqual(0.0, locationInput.PenetrationLength);
            Assert.AreEqual(2, locationInput.PhreaticLineOffsetBelowDikeToeAtPolder.NumberOfDecimalPlaces);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new MacroStabilityInwardsLocationInputDaily();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }
    }
}