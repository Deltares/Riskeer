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
using System.Globalization;
using System.Reflection;

namespace Core.Common.Util.Attributes
{
    /// <summary>
    /// Helper method for interacting with project resources.
    /// </summary>
    internal static class ResourceHelper
    {
        /// <summary>
        /// Retrieve the string resource with the given name.
        /// </summary>
        /// <param name="resourceType">Type of the resource file.</param>
        /// <param name="resourceName">Name of the resource property to be retrieved.</param>
        /// <returns>String resource in the resources file.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the resource cannot be found,
        /// does not have the given resource name, or isn't of type string.</exception>
        internal static string GetResourceLookup(Type resourceType, string resourceName)
        {
            PropertyInfo property = resourceType.GetProperty(resourceName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (property == null)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               "Resource {0} does not have property {1}.",
                                               resourceType, resourceName);
                throw new InvalidOperationException(message);
            }

            if (property.PropertyType != typeof(string))
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               "Resource {0} is not string.",
                                               resourceName);
                throw new InvalidOperationException(message);
            }

            return (string) property.GetValue(null, null);
        }
    }
}