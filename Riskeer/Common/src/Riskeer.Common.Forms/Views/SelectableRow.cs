// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="T"/> that can be selected.
    /// </summary>
    public class SelectableRow<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="SelectableRow{T}"/>.
        /// </summary>
        /// <param name="item">The <see cref="T"/> this row contains.</param>
        /// <param name="name">The name representation of <paramref name="item"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public SelectableRow(T item, string name)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Item = item;
        }

        /// <summary>
        /// Gets or sets whether the <see cref="SelectableRow{T}"/> is selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets the name of the <see cref="Item"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public T Item { get; }
    }
}