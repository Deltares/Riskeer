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
    public class PhreaticLineConfigurationTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Call
            var configuration = new PhreaticLineConfiguration();

            // Assert
            Assert.IsNull(configuration.Inwards);
            Assert.IsNull(configuration.Outwards);
        }

        [Test]
        public void SimpleProperties_SetNewValues_NewValuesSet()
        {
            // Setup
            const double inwards = 2.2;
            const double outwards = 3.3;

            // Call
            var configuration = new PhreaticLineConfiguration
            {
                Inwards = inwards,
                Outwards = outwards
            };

            // Assert
            Assert.AreEqual(inwards, configuration.Inwards);
            Assert.AreEqual(outwards, configuration.Outwards);
        }
    }
}