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
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.Common.IO.Test.Configurations
{
    [TestFixture]
    public class WaveReductionConfigurationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var configuration = new WaveReductionConfiguration();

            // Assert
            Assert.IsNull(configuration.UseBreakWater);
            Assert.IsNull(configuration.BreakWaterType);
            Assert.IsNull(configuration.BreakWaterHeight);
            Assert.IsNull(configuration.UseForeshoreProfile);
        }

        [Test]
        public void SimpleProperties_SetNewValue_GetsNewlySetValue()
        {
            // Setup
            var configuration = new WaveReductionConfiguration();
            var random = new Random(21);

            bool useBreakWater = random.NextBoolean();
            var breakWaterType = random.NextEnumValue<ConfigurationBreakWaterType>();
            double breakWaterHeight = random.NextDouble();
            bool useForeshore = random.NextBoolean();

            // Call
            configuration.UseBreakWater = useBreakWater;
            configuration.BreakWaterType = breakWaterType;
            configuration.BreakWaterHeight = breakWaterHeight;
            configuration.UseForeshoreProfile = useForeshore;

            // Assert
            Assert.AreEqual(useBreakWater, configuration.UseBreakWater);
            Assert.AreEqual(breakWaterType, configuration.BreakWaterType);
            Assert.AreEqual(breakWaterHeight, configuration.BreakWaterHeight);
            Assert.AreEqual(useForeshore, configuration.UseForeshoreProfile);
        }
    }
}