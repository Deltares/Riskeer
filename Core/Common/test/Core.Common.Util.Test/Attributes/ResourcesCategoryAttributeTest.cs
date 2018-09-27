// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Attributes;
using Core.Common.Util.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Util.Test.Attributes
{
    [TestFixture]
    public class ResourcesCategoryAttributeTest
    {
        [Test]
        public void ParameteredConstructor_ResourcePropertyDoesNotExist_ThrowInvalidOperationException()
        {
            // Call
            TestDelegate call = () => new ResourcesCategoryAttribute(typeof(Resources), "DoesNotExist");

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            StringAssert.Contains("does not have property", message);
        }

        [Test]
        public void ParameteredConstructor_ResourcePropertyIsNotString_ThrowInvalidOperationException()
        {
            // Call
            TestDelegate call = () => new ResourcesCategoryAttribute(typeof(Resources), "abacus");

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            StringAssert.EndsWith("is not string.", message);
        }

        [Test]
        public void ParameteredConstructor_StringResource_ExpectedValues()
        {
            // Call
            var attribute = new ResourcesCategoryAttribute(typeof(Resources), "SomeStringResource");

            // Assert
            Assert.AreEqual(Resources.SomeStringResource, attribute.Category);
        }

        [Test]
        public void ParameteredConstructor_StringResourceAtPosition_AddTabsForPosition()
        {
            // Call
            var attribute = new ResourcesCategoryAttribute(typeof(Resources), "SomeStringResource", 1, 2);

            // Assert
            Assert.AreEqual($"\t{Resources.SomeStringResource}", attribute.Category);
        }

        [Test]
        public void ParameteredConstructor_StringResourceAtPositionMoreThanCategoryAmount_NoTabsAdded()
        {
            // Call
            var attribute = new ResourcesCategoryAttribute(typeof(Resources), "SomeStringResource", 4, 2);

            // Assert
            Assert.AreEqual(Resources.SomeStringResource, attribute.Category);
        }
    }
}