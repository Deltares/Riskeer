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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Core.Common.Util.Reflection;
using NUnit.Framework;

namespace Ringtoets.AssemblyTool.IO.TestUtil
{
    /// <summary>
    /// Test helper for asserting serialization attributes on properties and types.
    /// </summary>
    public static class SerializableAttributeTestHelper
    {
        /// <summary>
        /// Asserts whether the <paramref name="type"/> has a <see cref="XmlTypeAttribute"/> with the correct values.
        /// </summary>
        /// <param name="type">The type to assert.</param>
        /// <param name="typeName">The expected XML type name.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="typeName"/> does not match with the actual attribute;</item>
        /// <item>multiple attributes of type <see cref="XmlTypeAttribute"/> were found.</item>
        /// </list>
        /// </exception>
        public static void AssertXmlTypeAttribute(Type type, string typeName)
        {
            var attribute = (XmlTypeAttribute) type.GetCustomAttributes(typeof(XmlTypeAttribute), false).SingleOrDefault();
            Assert.IsNotNull(attribute);
            Assert.AreEqual(typeName, attribute.TypeName);
        }

        /// <summary>
        /// Asserts whether the property <paramref name="propertyName"/> in class <typeparamref name="T"/>
        /// has a <see cref="XmlElementAttribute"/> with the correct values.
        /// </summary>
        /// <typeparam name="T">The class the <paramref name="propertyName"/> is in.</typeparam>
        /// <param name="propertyName">The name of the property to assert.</param>
        /// <param name="elementName">The expected XML element name.</param>
        /// <param name="namespaceUrl">The expected XML namespace url.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>the <see cref="XmlElementAttribute"/> could not be found</item>
        /// <item>multiple attributes of type <see cref="XmlElementAttribute"/> were found;</item>
        /// <item>the <paramref name="elementName"/> or <paramref name="namespaceUrl"/> do not match
        /// with the actual attribute.</item>
        /// </list>
        /// </exception>
        public static void AssertXmlElementAttribute<T>(string propertyName, string elementName, string namespaceUrl = null)
            where T : class 
        {
            XmlElementAttribute attribute = GetPropertyAttribute<T, XmlElementAttribute>(propertyName);
            Assert.AreEqual(elementName, attribute.ElementName);
            Assert.AreEqual(namespaceUrl, attribute.Namespace);
        }

        /// <summary>
        /// Asserts whether the property <paramref name="propertyName"/> in class <typeparamref name="T"/>
        /// has a <see cref="XmlElementAttribute"/> with the correct values.
        /// </summary>
        /// <typeparam name="T">The class the <paramref name="propertyName"/> is in.</typeparam>
        /// <param name="propertyName">The name of the property to assert.</param>
        /// <param name="elementName">The expected XML element name.</param>
        /// <param name="namespaceUrl">The expected XML namespace url.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>the <see cref="XmlElementAttribute"/> could not be found;</item>
        /// <item>multiple attributes of type <see cref="XmlAttributeAttribute"/> were found;</item>
        /// <item>the actual attribute does not match with the given <paramref name="elementName"/>
        /// and <paramref name="namespaceUrl"/>.</item>
        /// </list>
        /// </exception>
        public static void AssertXmlAttributeAttribute<T>(string propertyName, string elementName, string namespaceUrl = null)
            where T : class 
        {
            XmlAttributeAttribute attribute = GetPropertyAttribute<T, XmlAttributeAttribute>(propertyName);
            Assert.AreEqual(elementName, attribute.AttributeName);
            Assert.AreEqual(namespaceUrl, attribute.Namespace);
        }

        private static TAttribute GetPropertyAttribute<TObject, TAttribute>(string propertyName)
        {
            IEnumerable<TAttribute> attributes = TypeUtils.GetPropertyAttributes<TObject, TAttribute>(propertyName);
            Assert.AreEqual(1, attributes.Count());
            return attributes.Single();
        }
    }
}