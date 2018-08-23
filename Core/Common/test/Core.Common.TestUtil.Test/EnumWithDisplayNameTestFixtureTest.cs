// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using Core.Common.TestUtil.Test.Properties;
using Core.Common.Util.Attributes;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class EnumWithDisplayNameTestFixtureTest
    {
        [Test]
        public void Constructor_ExpectedResult()
        {
            // Call
            var displayNameEnumTest = new DisplayNameEnumTest();

            // Assert
            Assert.IsInstanceOf<EnumValuesTestFixture<EnumDisplayName, int>>(displayNameEnumTest);
        }

        [TestFixture]
        private class DisplayNameEnumTest : EnumWithResourcesDisplayNameTestFixture<EnumDisplayName>
        {
            protected override IDictionary<EnumDisplayName, string> ExpectedDisplayNameForEnumValues
            {
                get
                {
                    return new Dictionary<EnumDisplayName, string>
                    {
                        {
                            EnumDisplayName.NoDisplayName, null
                        },
                        {
                            EnumDisplayName.HasResourcesDisplayName, Resources.SomeDisplayName
                        }
                    };
                }
            }

            protected override IDictionary<EnumDisplayName, int> ExpectedValueForEnumValues
            {
                get
                {
                    return new Dictionary<EnumDisplayName, int>
                    {
                        {
                            EnumDisplayName.NoDisplayName, 0
                        },
                        {
                            EnumDisplayName.HasResourcesDisplayName, 4
                        }
                    };
                }
            }
        }

        private enum EnumDisplayName
        {
            NoDisplayName,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.SomeDisplayName))]
            HasResourcesDisplayName = 4
        }
    }
}