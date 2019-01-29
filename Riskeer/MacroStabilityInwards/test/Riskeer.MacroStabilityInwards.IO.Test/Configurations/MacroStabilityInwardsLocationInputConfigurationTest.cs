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
using Riskeer.MacroStabilityInwards.IO.Configurations;

namespace Riskeer.MacroStabilityInwards.IO.Test.Configurations
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
        public void Constructor_SetProperties_ExpectedValues()
        {
            // Setup
            var random = new Random(31);
            double waterLevelPolder = random.NextDouble();
            bool useDefaultOffsets = random.NextBoolean();
            double phreaticLineOffsetBelowDikeTopAtRiver = random.NextDouble();
            double phreaticLineOffsetBelowDikeTopAtPolder = random.NextDouble();
            double phreaticLineOffsetBelowShoulderBaseInside = random.NextDouble();
            double phreaticLineOffsetBelowDikeToeAtPolder = random.NextDouble();

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