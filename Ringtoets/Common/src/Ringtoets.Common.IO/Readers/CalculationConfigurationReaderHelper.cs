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
    /// Helper methods related to <see cref="CalculationConfigurationReader{TReadCalculation}"/> instances.
    /// </summary>
    public static class CalculationConfigurationReaderHelper
    {
        /// <summary>
        /// Gets the double value from a descendant element.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The value of the element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double? GetDoubleValueFromDescendantElement(XElement parentElement, string descendantElementName)
        {
            if (parentElement == null)
            {
                throw new ArgumentNullException(nameof(parentElement));
            }
            if (descendantElementName == null)
            {
                throw new ArgumentNullException(nameof(descendantElementName));
            }

            XElement descendantElement = GetDescendantElement(parentElement, descendantElementName);

            return descendantElement != null
                       ? (double?) XmlConvert.ToDouble(descendantElement.Value)
                       : null;
        }

        /// <summary>
        /// Gets the string value from a descendant element.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The value of the element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static string GetStringValueFromDescendantElement(XElement parentElement, string descendantElementName)
        {
            if (parentElement == null)
            {
                throw new ArgumentNullException(nameof(parentElement));
            }
            if (descendantElementName == null)
            {
                throw new ArgumentNullException(nameof(descendantElementName));
            }

            XElement descendantElement = GetDescendantElement(parentElement, descendantElementName);

            return descendantElement?.Value;
        }

        /// <summary>
        /// Gets a descendant element with the given <paramref name="descendantElementName"/>.
        /// </summary>
        /// <param name="parentElement">The <see cref="XElement"/> that contains the descendant element.</param>
        /// <param name="descendantElementName">The name of the descendant element.</param>
        /// <returns>The element, or <c>null</c> when the <paramref name="parentElement"/>
        /// does not have descendant elements of <paramref name="descendantElementName"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static XElement GetDescendantElement(XElement parentElement, string descendantElementName)
        {
            if (parentElement == null)
            {
                throw new ArgumentNullException(nameof(parentElement));
            }
            if (descendantElementName == null)
            {
                throw new ArgumentNullException(nameof(descendantElementName));
            }

            return parentElement.Descendants(descendantElementName).FirstOrDefault();
        }
    }
}