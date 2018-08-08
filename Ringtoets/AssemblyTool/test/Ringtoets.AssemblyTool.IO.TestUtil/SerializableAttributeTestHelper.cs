﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Linq;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Ringtoets.AssemblyTool.IO.TestUtil
{
    /// <summary>
    /// Test helper for asserting serialization attributes on properties and types.
    /// </summary>
    public static class SerializableAttributeTestHelper
    {
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
        /// <item>the <see cref="XmlElementAttribute"/> could not be found, or multiple attributes are defined;</item>
        /// <item>the <paramref name="elementName"/> or <paramref name="namespaceUrl"/> do not match
        /// with the actual attribute.</item>
        /// </list>
        /// </exception>
        public static void AssertXmlElementAttribute<T>(string propertyName, string elementName, string namespaceUrl = null)
        {
            XmlElementAttribute attribute = GetPropertyAttribute<T, XmlElementAttribute>(propertyName);
            Assert.AreEqual(elementName, attribute.ElementName);
            Assert.AreEqual(namespaceUrl, attribute.Namespace);
        }

        private static TAttribute GetPropertyAttribute<TObject, TAttribute>(string propertyName)
        {
            var attribute = (TAttribute) typeof(TObject).GetProperty(propertyName)?.GetCustomAttributes(typeof(TAttribute), false).SingleOrDefault();
            Assert.IsNotNull(attribute);
            return attribute;
        }
    }
}