// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Ringtoets.Common.IO.Readers
{
    /// <summary>
    /// Helper methods related to <see cref="ConfigurationReader{TReadCalculation}"/> instances.
    /// </summary>
    public static class ConfigurationReaderHelper
    {
        /// <summary>
        /// Gets the double value from a child element.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the child element.</param>
        /// <param name="childElementName">The name of the child element.</param>
        /// <returns>The value of the element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have child elements of <paramref name="childElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double? GetDoubleValueFromChildElement(XElement parentElement, string childElementName)
        {
            if (parentElement == null)
            {
                throw new ArgumentNullException(nameof(parentElement));
            }
            if (childElementName == null)
            {
                throw new ArgumentNullException(nameof(childElementName));
            }

            XElement childElement = GetChildElement(parentElement, childElementName);

            return childElement != null
                       ? (double?) XmlConvert.ToDouble(childElement.Value)
                       : null;
        }

        /// <summary>
        /// Gets the string value from a child element.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the child element.</param>
        /// <param name="childElementName">The name of the child element.</param>
        /// <returns>The value of the element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have child elements of <paramref name="childElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static string GetStringValueFromChildElement(XElement parentElement, string childElementName)
        {
            if (parentElement == null)
            {
                throw new ArgumentNullException(nameof(parentElement));
            }
            if (childElementName == null)
            {
                throw new ArgumentNullException(nameof(childElementName));
            }

            XElement childElement = GetChildElement(parentElement, childElementName);

            return childElement?.Value;
        }

        private static XElement GetChildElement(XElement parentElement, string childElementName)
        {
            return parentElement.Elements(childElementName).FirstOrDefault();
        }
    }
}