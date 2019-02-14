// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.ComponentModel;
using NUnit.Framework;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.IO.Configurations.Converters;

namespace Riskeer.StabilityStoneCover.IO.Test.Configurations.Converters
{
    [TestFixture]
    public class ConfigurationStabilityStoneCoverCalculationTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_StabilityStoneCoverWaveConditionsCalculationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(StabilityStoneCoverWaveConditionsCalculationType));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertTo);
        }
    }
}