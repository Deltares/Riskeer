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
using System.Reflection;

namespace Core.Common.Gui.Converters
{
    /// <summary>
    /// Attribute when using the <see cref="KeyValueExpandableArrayConverter"/> to define what is
    /// shown as name and value for each element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyValueElementAttribute : Attribute
    {
        private readonly string namePropertyName;

        /// <summary>
        /// Creates a new instance of <see cref="KeyValueElementAttribute"/>.
        /// </summary>
        /// <param name="namePropertyName">The name of the property to show as name.</param>
        /// <param name="valuePropertyName">The name of the property to show as value.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public KeyValueElementAttribute(string namePropertyName, string valuePropertyName)
        {
            this.namePropertyName = namePropertyName;
            ValuePropertyName = valuePropertyName;
            if (valuePropertyName == null)
            {
                throw new ArgumentNullException(nameof(valuePropertyName));
            }

            if (namePropertyName == null)
            {
                throw new ArgumentNullException(nameof(namePropertyName));
            }
        }

        /// <summary>
        /// Gets the property value from the <paramref name="source"/> that is used
        /// as name.
        /// </summary>
        /// <param name="source">The source to obtain the property value of.</param>
        /// <returns>The value used as name of the property.</returns>
        /// <exception cref="ArgumentException">Thrown when the property used for the name of
        /// the <see cref="KeyValueElementAttribute"/> is not found on the <paramref name="source"/>.
        /// </exception>
        public string GetName(object source)
        {
            PropertyInfo namePropertyInfo = source.GetType().GetProperty(namePropertyName);
            if (namePropertyInfo == null)
            {
                throw new ArgumentException($"Name property '{namePropertyName}' was not found on type {source.GetType().Name}.");
            }

            return Convert.ToString(namePropertyInfo.GetValue(source, new object[0]));
        }

        /// <summary>
        /// Gets the property value from the <paramref name="source"/> that is used
        /// as value.
        /// </summary>
        /// <param name="source">The source to obtain the property value of.</param>
        /// <returns>The value used as value of the property.</returns>
        /// <exception cref="ArgumentException">Thrown when the property used for the value of
        /// the <see cref="KeyValueElementAttribute"/> is not found on the <paramref name="source"/>.
        /// </exception>
        public virtual string GetValue(object source)
        {
            PropertyInfo valuePropertyInfo = source.GetType().GetProperty(ValuePropertyName);
            if (valuePropertyInfo == null)
            {
                throw new ArgumentException($"Value property '{ValuePropertyName}' was not found on type {source.GetType().Name}.");
            }

            return Convert.ToString(valuePropertyInfo.GetValue(source, new object[0]));
        }

        /// <summary>
        /// Gets the name of the property to show as value.
        /// </summary>
        protected string ValuePropertyName { get; }
    }
}