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

namespace Core.Common.Utils.Attributes
{
    /// <summary>
    /// Enables the display name to be fetched from resources on <see cref="Enum"/> types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public sealed class ResourcesEnumDisplayNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesEnumDisplayNameAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource file.</param>
        /// <param name="resourceName">Name of the string resource property to be used as display name.</param>
        /// <exception cref="InvalidOperationException">Resource cannot be found or does not have the given resource name.</exception>
        public ResourcesEnumDisplayNameAttribute(Type resourceType, string resourceName)
        {
            DisplayName = ResourceHelper.GetResourceLookup(resourceType, resourceName);
        }

        /// <summary>
        /// Gets the display name of the <see cref="Enum"/>.
        /// </summary>
        public string DisplayName { get; private set; }
    }
}