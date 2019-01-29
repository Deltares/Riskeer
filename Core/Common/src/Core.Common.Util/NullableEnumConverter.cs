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
using System.Linq;
using System.Reflection;
using Core.Common.Util.Attributes;

namespace Core.Common.Util
{
    /// <summary>
    /// A type converter to convert nullable Enum objects to and from various other representations.
    /// </summary>
    public class NullableEnumConverter : NullableConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableEnumConverter"/> class for the given nullable Enum <paramref name="type"/>.
        /// </summary>
        /// <remarks> This class looks for the <see cref="ResourcesDisplayNameAttribute"/> decorator.</remarks>
        /// <param name="type">A <see cref="Type"/> that represents the type of enumeration to associate with this enumeration converter.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not a nullable type.</exception>
        public NullableEnumConverter(Type type) : base(type) {}

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw new NotSupportedException("Cannot convert from (null).");
            }

            var valueString = value as string;
            if (valueString != null)
            {
                FieldInfo fieldInfo = UnderlyingType.GetFields().FirstOrDefault(info => valueString == GetDisplayName(info));

                if (fieldInfo != null)
                {
                    return Enum.Parse(UnderlyingType, fieldInfo.Name);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || value == null)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            FieldInfo fieldInfo = UnderlyingType.GetField(value.ToString());
            return GetDisplayName(fieldInfo);
        }

        /// <summary>
        /// Gets a collection of standard values for the data type this type converter is designed for.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format 
        /// context that can be used to extract additional information about the environment 
        /// from which this converter is invoked. This parameter or properties of this parameter can be <c>null</c>.</param>
        /// <returns>A <see cref="TypeConverter.StandardValuesCollection"/> that holds a standard 
        /// set of valid values, or <c>null</c> if the data type does not support a standard set of values.</returns>
        /// <remarks>Does not add a value for <c>null</c> to the <see cref="TypeConverter.StandardValuesCollection"/>.</remarks>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (UnderlyingTypeConverter == null)
            {
                return base.GetStandardValues(context);
            }

            StandardValuesCollection values = UnderlyingTypeConverter.GetStandardValues(context);
            if (GetStandardValuesSupported(context) && values != null)
            {
                return new StandardValuesCollection(values);
            }

            return base.GetStandardValues(context);
        }

        private static string GetDisplayName(MemberInfo memberInfo)
        {
            var resourcesDisplayNameAttribute = (ResourcesDisplayNameAttribute) Attribute.GetCustomAttribute(memberInfo, typeof(ResourcesDisplayNameAttribute));
            return resourcesDisplayNameAttribute?.DisplayName;
        }
    }
}