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
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Attributes
{
    /// <summary>
    /// Marks property as a conditionally ordered property. When this attribute is declared
    /// on a property, the declaring class should have a public method marked with 
    /// <see cref="DynamicPropertyOrderEvaluationMethodAttribute"/> to be used to evaluate the
    /// order of the property.
    /// </summary>
    /// <seealso cref="DynamicPropertyOrderEvaluationMethodAttribute"/>
    /// <seealso cref="PropertyOrderAttribute"/>
    /// <remarks>This attribute provides a run-time alternative to <see cref="PropertyOrderAttribute"/>.</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DynamicPropertyOrderAttribute : Attribute
    {
        /// <summary>
        /// Determines the order of the property.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">The name of the property of <paramref name="obj"/>.</param>
        /// <returns>The order of the property.</returns>
        /// <exception cref="MissingMemberException">Thrown when <paramref name="propertyName"/>
        /// does not correspond to a public property of <paramref name="obj"/>.</exception>
        /// <exception cref="MissingMethodException">Thrown when there isn't a single method
        /// declared on <paramref name="obj"/> marked with <see cref="DynamicPropertyOrderEvaluationMethodAttribute"/>
        /// that is matching the signature defined by <see cref="DynamicPropertyOrderEvaluationMethodAttribute.PropertyOrder"/>.</exception>
        public static int PropertyOrder(object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return 0;
            }

            if (!IsPropertyDynamicOrdered(obj, propertyName))
            {
                return 0;
            }

            DynamicPropertyOrderEvaluationMethodAttribute.PropertyOrder propertyOrder = DynamicPropertyOrderEvaluationMethodAttribute.CreatePropertyOrderMethod(obj);

            return propertyOrder(propertyName);
        }

        private static bool IsPropertyDynamicOrdered(object obj, string propertyName)
        {
            MemberInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new MissingMemberException(string.Format(CultureInfo.CurrentCulture,
                                                               Resources.Could_not_find_property_0_on_type_1_,
                                                               propertyName, obj.GetType()));
            }

            return IsDefined(propertyInfo, typeof(DynamicPropertyOrderAttribute));
        }
    }
}