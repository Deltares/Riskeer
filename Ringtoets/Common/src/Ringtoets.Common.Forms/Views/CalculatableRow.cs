// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row that has data that can be calculated.
    /// </summary>
    /// <typeparam name="T">The type of the calculatable object.</typeparam>
    public abstract class CalculatableRow<T> where T : class
    {
        /// <summary>
        /// Creates a new instance of <see cref="CalculatableRow{T}"/>.
        /// </summary>
        /// <param name="calculatableObject">The calculatable object to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculatableObject"/>
        /// is <c>null</c>.</exception>
        protected CalculatableRow(T calculatableObject)
        {
            if (calculatableObject == null)
            {
                throw new ArgumentNullException(nameof(calculatableObject));
            }

            CalculatableObject = calculatableObject;
        }

        /// <summary>
        /// Gets or sets whether the <see cref="CalculatableRow{T}"/> is set to be calculated.
        /// </summary>
        public bool ShouldCalculate { get; set; }

        /// <summary>
        /// Gets the wrapped calculatable object.
        /// </summary>
        public T CalculatableObject { get; }
    }
}