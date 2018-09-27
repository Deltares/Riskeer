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
using System.Linq;
using System.Reflection;
using Core.Common.Util.Attributes;
using Core.Common.Util.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Util.Test.Attributes
{
    [TestFixture]
    public class ResourcesDisplayNameAttributeTest
    {
        [Test]
        public void ParameteredConstructor_ResourcePropertyDoesNotExist_ThrowInvalidOperationException()
        {
            // Call
            TestDelegate call = () => new ResourcesDisplayNameAttribute(typeof(Resources), "DoesNotExist");

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            StringAssert.Contains("does not have property", message);
        }

        [Test]
        public void ParameteredConstructor_ResourcePropertyIsNotString_ThrowInvalidOperationException()
        {
            // Call
            TestDelegate call = () => new ResourcesDisplayNameAttribute(typeof(Resources), "abacus");

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            StringAssert.EndsWith("is not string.", message);
        }

        [Test]
        public void ParameteredConstructor_StringResource_ExpectedValues()
        {
            // Call
            var attribute = new ResourcesDisplayNameAttribute(typeof(Resources), "SomeStringResource");

            // Assert
            Assert.AreEqual(Resources.SomeStringResource, attribute.DisplayName);
        }

        [Test]
        public void Enum_StringResource_ExpectedValues()
        {
            // Call
            FieldInfo fieldInfo = typeof(SimpleEnum).GetFields().FirstOrDefault(fi => fi.FieldType == typeof(SimpleEnum));
            Assert.IsNotNull(fieldInfo);
            var resourcesDisplayNameAttribute = (ResourcesDisplayNameAttribute) Attribute.GetCustomAttribute(fieldInfo, typeof(ResourcesDisplayNameAttribute));

            // Assert
            Assert.IsNotNull(resourcesDisplayNameAttribute);
            Assert.AreEqual(Resources.EnumStringResource, resourcesDisplayNameAttribute.DisplayName);
        }

        [Test]
        public void Property_StringResource_ExpectedValues()
        {
            // Call
            PropertyInfo fieldInfo = typeof(SimpleClass).GetProperty("Name");
            Assert.IsNotNull(fieldInfo);
            var resourcesDisplayNameAttribute = (ResourcesDisplayNameAttribute) Attribute.GetCustomAttribute(fieldInfo, typeof(ResourcesDisplayNameAttribute));

            // Assert
            Assert.IsNotNull(resourcesDisplayNameAttribute);
            Assert.AreEqual(Resources.MethodStringResource, resourcesDisplayNameAttribute.DisplayName);
        }

        [Test]
        public void Class_StringResource_ExpectedValues()
        {
            // Call
            var resourcesDisplayNameAttribute = (ResourcesDisplayNameAttribute) Attribute.GetCustomAttribute(typeof(SimpleClass), typeof(ResourcesDisplayNameAttribute));

            // Assert
            Assert.IsNotNull(resourcesDisplayNameAttribute);
            Assert.AreEqual(Resources.ClassStringResource, resourcesDisplayNameAttribute.DisplayName);
        }

        private enum SimpleEnum
        {
            [ResourcesDisplayName(typeof(Resources), nameof(Resources.EnumStringResource))]
            FirstValue
        }

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ClassStringResource))]
        private class SimpleClass
        {
            [ResourcesDisplayName(typeof(Resources), nameof(Resources.MethodStringResource))]
            public string Name
            {
                get
                {
                    return string.Empty;
                }
            }
        }
    }
}