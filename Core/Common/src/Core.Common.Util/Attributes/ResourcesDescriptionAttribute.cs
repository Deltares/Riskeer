// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.ComponentModel;

namespace Core.Common.Util.Attributes
{
    /// <summary>
    /// Variation on <see cref="DescriptionAttribute"/>, enables the description to be fetched from resources.
    /// </summary>
    /// <remarks>Do not combine this with <see cref="DescriptionAttribute"/> on the same item</remarks>
    public sealed class ResourcesDescriptionAttribute : DescriptionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource file.</param>
        /// <param name="resourceName">Name of the string resource property to be used as description.</param>
        /// <exception cref="InvalidOperationException">Thrown when the resource cannot be found 
        /// or does not have the given resource name.</exception>
        public ResourcesDescriptionAttribute(Type resourceType, string resourceName) :
            base(ResourceHelper.GetResourceLookup(resourceType, resourceName)) {}
    }
}