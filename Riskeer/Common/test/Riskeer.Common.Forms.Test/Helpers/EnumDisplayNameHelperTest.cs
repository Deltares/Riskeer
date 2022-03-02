// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Util.Attributes;
using NUnit.Framework;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Test.Properties;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class EnumDisplayNameHelperTest
    {
        [Test]
        [TestCase(TestEnum.Type1, "Naam 1")]
        [TestCase(TestEnum.Type2, "Type2")]
        [TestCase(TestEnum.Type3, "Type3")]
        public void GetDisplayName_WithValidEnumWithAndWithoutDisplayNameAttribute_ReturnsExpectedDisplayName(TestEnum value, string expectedDisplayName)
        {
            // Call
            string displayName = EnumDisplayNameHelper.GetDisplayName(value);

            // Assert
            Assert.AreEqual(expectedDisplayName, displayName);
        }

        [Test]
        public void GetDisplayName_WithInvalidEnum_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const TestEnum invalidValue = (TestEnum) 99;

            // Call
            void Call() => EnumDisplayNameHelper.GetDisplayName(invalidValue);

            // Assert
            var expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(TestEnum)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        public enum TestEnum
        {
            [ResourcesDisplayName(typeof(Resources), nameof(Resources.TestEnum_Type1_DisplayName))]
            Type1 = 1,
            Type2 = 2,
            Type3 = 3
        }
    }
}