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

using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.IO.Configurations;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsLocationInputConfigurationTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Call
            var configuration = new MacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.IsNull(configuration.WaterLevelPolder);
            Assert.IsNull(configuration.UseDefaultOffsets);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNull(configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        public void SimpleProperties_SetNewValues_NewValuesSet()
        {
            // Setup
            const double waterLevelPolder = 2.2;
            const bool useDefaultOffsets = false;
            const double phreaticLineOffsetBelowDikeTopAtRiver = 4.4;
            const double phreaticLineOffsetBelowDikeTopAtPolder = 5.5;
            const double phreaticLineOffsetBelowShoulderBaseInside = 6.6;
            const double phreaticLineOffsetBelowDikeToeAtPolder = 7.7;

            // Call
            var configuration = new MacroStabilityInwardsLocationInputConfiguration
            {
                WaterLevelPolder = waterLevelPolder,
                UseDefaultOffsets = useDefaultOffsets,
                PhreaticLineOffsetBelowDikeTopAtRiver = phreaticLineOffsetBelowDikeTopAtRiver,
                PhreaticLineOffsetBelowDikeTopAtPolder = phreaticLineOffsetBelowDikeTopAtPolder,
                PhreaticLineOffsetBelowShoulderBaseInside = phreaticLineOffsetBelowShoulderBaseInside,
                PhreaticLineOffsetBelowDikeToeAtPolder = phreaticLineOffsetBelowDikeToeAtPolder
            };

            // Assert
            Assert.AreEqual(waterLevelPolder, configuration.WaterLevelPolder);
            Assert.AreEqual(useDefaultOffsets, configuration.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, configuration.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, configuration.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, configuration.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, configuration.PhreaticLineOffsetBelowDikeToeAtPolder);
        }
    }
}