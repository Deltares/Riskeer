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
using System.Globalization;
using System.Reflection;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Attributes
{
    /// <summary>
    /// Marks property as a conditional read-only property. When this attribute is declared
    /// on a property, the declaring class should have a public method marked with 
    /// <see cref="DynamicReadOnlyValidationMethodAttribute"/> to be used to evaluate if
    /// that property should be read-only or not.
    /// </summary>
    /// <seealso cref="DynamicReadOnlyValidationMethodAttribute"/>
    /// <seealso cref="ReadOnlyAttribute"/>
    /// <remarks>This attribute provides a run-time alternative to <see cref="ReadOnlyAttribute"/>.</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DynamicReadOnlyAttribute : Attribute
    {
        /// <summary>
        /// Determines whether the property is read-only or not.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">The name of the property of <paramref name="obj"/>.</param>
        /// <returns><c>true</c> if the property is read-only, <c>false</c> otherwise.</returns>
        /// <exception cref="MissingMemberException">Thrown when <paramref name="propertyName"/>
        /// does not correspond to a public property of <paramref name="obj"/>.</exception>
        /// <exception cref="MissingMethodException">Thrown when there isn't a single method
        /// declared on <paramref name="obj"/> marked with <see cref="DynamicReadOnlyValidationMethodAttribute"/>
        /// that is matching the signature defined by <see cref="DynamicReadOnlyValidationMethodAttribute.IsPropertyReadOnly"/>.</exception>
        public static bool IsReadOnly(object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return ReadOnlyAttribute.Default.IsReadOnly;
            }

            if (!IsPropertyDynamicallyReadOnly(obj, propertyName))
            {
                return ReadOnlyAttribute.Default.IsReadOnly;
            }

            DynamicReadOnlyValidationMethodAttribute.IsPropertyReadOnly isPropertyReadOnlyDelegate = DynamicReadOnlyValidationMethodAttribute.CreateIsReadOnlyMethod(obj);
            return isPropertyReadOnlyDelegate(propertyName);
        }

        private static bool IsPropertyDynamicallyReadOnly(object obj, string propertyName)
        {
            MemberInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new MissingMemberException(string.Format(CultureInfo.CurrentCulture,
                                                               Resources.Could_not_find_property_0_on_type_1_,
                                                               propertyName, obj.GetType()));
            }

            return IsDefined(propertyInfo, typeof(DynamicReadOnlyAttribute));
        }
    }
}