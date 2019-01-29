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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;

namespace Riskeer.Common.IO.Test.Configurations
{
    [TestFixture]
    public class CalculationGroupConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new CalculationGroupConfiguration(null, Enumerable.Empty<IConfigurationItem>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("name", paramName);
        }

        [Test]
        public void Constructor_ItemsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new CalculationGroupConfiguration("name", null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("items", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var nestedItems = new List<IConfigurationItem>
            {
                new TestReadConfigurationItem(),
                new CalculationGroupConfiguration("Nested calculation group", Enumerable.Empty<IConfigurationItem>())
            };

            // Call
            var readCalculationGroup = new CalculationGroupConfiguration("Calculation group", nestedItems);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readCalculationGroup);
            Assert.AreEqual("Calculation group", readCalculationGroup.Name);
            Assert.AreSame(nestedItems, readCalculationGroup.Items);
        }

        private class TestReadConfigurationItem : IConfigurationItem
        {
            public string Name
            {
                get
                {
                    return "Fixed name";
                }
            }
        }
    }
}