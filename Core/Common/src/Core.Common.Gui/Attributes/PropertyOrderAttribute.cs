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

namespace Core.Common.Gui.Attributes
{
    /// <summary>
    /// Attribute that allows for controlling the order that properties appear in a property grid.
    /// Ordering should occur on ascending order.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyOrderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">The ordering value.</param>
        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }

        /// <summary>
        /// Gets the ordering value.
        /// </summary>
        public int Order { get; }
    }
}