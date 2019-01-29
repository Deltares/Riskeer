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
    public class MacroStabilityInwardsLocationInputExtremeTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var locationInput = new MacroStabilityInwardsLocationInputExtreme();

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsLocationInputBase>(locationInput);
            Assert.IsInstanceOf<IMacroStabilityInwardsLocationInputExtreme>(locationInput);

            Assert.IsNaN(locationInput.PenetrationLength);
            Assert.AreEqual(2, locationInput.PenetrationLength.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_SetPenetrationLength_ExpectedValue()
        {
            // Setup
            var random = new Random(21);
            double penetrationLength = random.NextDouble();

            // Call
            var locationInput = new MacroStabilityInwardsLocationInputExtreme
            {
                PenetrationLength = (RoundedDouble) penetrationLength
            };

            // Assert
            Assert.AreEqual(2, locationInput.PenetrationLength.NumberOfDecimalPlaces);
            Assert.AreEqual(penetrationLength, locationInput.PenetrationLength, locationInput.PenetrationLength.GetAccuracy());
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new MacroStabilityInwardsLocationInputExtreme
            {
                PenetrationLength = random.NextRoundedDouble()
            };

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }
    }
}