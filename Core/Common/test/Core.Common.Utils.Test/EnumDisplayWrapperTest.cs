// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Exceptions;
using Core.Common.Utils.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Utils.Test
{
    [TestFixture]
    public class EnumDisplayWrapperTest
    {
        [Test]
        public void Constructor_WithoutValue_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new EnumDisplayWrapper<object>(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void Constructor_WithTypeParameterNotOfEnumType_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new EnumDisplayWrapper<object>(TestEnum.DisplayName);

            // Assert
            string paramName = Assert.Throws<InvalidTypeParameterException>(test).TypeParamName;
            Assert.AreEqual("T", paramName);
        }

        [Test]
        public void DisplayName_ConstructedWithEnumTypeValueWithoutResourceDisplayNameAttribute_EqualsToDefaultStringRepresentation()
        {
            // Setup
            var value = TestEnum.NoDisplayName;
            var wrapper = new EnumDisplayWrapper<TestEnum>(value);

            // Call
            var displayName = wrapper.DisplayName;

            // Assert
            Assert.AreEqual(value.ToString(), displayName);
        }

        [Test]
        public void DisplayName_ConstructedWithEnumTypeValueWithResourceDisplayNameAttribute_EqualsToResourceString()
        {
            // Setup
            var wrapper = new EnumDisplayWrapper<TestEnum>(TestEnum.DisplayName);

            // Call
            var displayName = wrapper.DisplayName;

            // Assert
            Assert.AreEqual(Resources.EnumDisplayWrapperTest_DisplayNameValueDisplayName, displayName);
        }

        private enum TestEnum
        {
            NoDisplayName,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.EnumDisplayWrapperTest_DisplayNameValueDisplayName))]
            DisplayName
        }
    }
}