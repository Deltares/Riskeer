// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Util.Attributes;
using Core.Common.Util.Exceptions;

namespace Core.Common.Util
{
    /// <summary>
    /// This class wraps a Enum value of type <typeparamref name="T"/> so that a display name can be
    /// obtained for that value.
    /// </summary>
    /// <typeparam name="T">The enum type to wrap.</typeparam>
    public class EnumDisplayWrapper<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="EnumDisplayWrapper{T}"/>.
        /// </summary>
        /// <param name="value">The enum value to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/>
        /// is not an Enum type.</exception>
        public EnumDisplayWrapper(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), @"An Enum type value is required.");
            }

            if (!typeof(Enum).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidTypeParameterException(@"The type parameter has to be an Enum type.", nameof(T));
            }

            Value = value;
            SetDisplayName(value);
        }

        /// <summary>
        /// The actual value of <typeparamref name="T"/> that has been wrapped.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Returns the name to use to display the enum value.
        /// </summary>
        /// <returns>The display name of the enum value or the default string representation of the value
        /// if no <see cref="ResourcesDisplayNameAttribute"/> was defined for the enum value.</returns>
        public string DisplayName { get; private set; }

        private void SetDisplayName(T value)
        {
            DisplayName = new EnumTypeConverter(typeof(T)).ConvertToString(value) ?? value.ToString();
        }
    }
}