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
using Core.Common.Util.Properties;

namespace Core.Common.Util
{
    /// <summary>
    /// A type converter to convert Enum objects to and from various other representations.
    /// </summary>
    public class EnumTypeConverter : EnumConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumTypeConverter"/> class for the given Enum <paramref name="type"/>.
        /// </summary>
        /// <remarks>This class is designed such that it looks for <see cref="ResourcesDisplayNameAttribute"/> on each Enum value.</remarks>
        /// <param name="type">A <see cref="Type"/> that represents the type of enumeration to associate with this enumeration converter.</param>
        public EnumTypeConverter(Type type) : base(type) {}

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var valueString = value as string;
            if (valueString != null)
            {
                FieldInfo fieldInfo = EnumType.GetFields().FirstOrDefault(info => valueString == GetDisplayName(info, info.Name));

                if (fieldInfo == null)
                {
                    throw new FormatException(string.Format(CultureInfo.CurrentCulture,
                                                            Resources.ConvertFrom_Only_following_values_are_accepted_ParameterValues_0_,
                                                            string.Join(", ", EnumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                                                                                      .Select(fi => GetDisplayName(fi, fi.Name)))));
                }

                return Enum.Parse(EnumType, fieldInfo.Name);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || value == null)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            string valueString = value.ToString();
            FieldInfo fieldInfo = EnumType.GetField(valueString);
            return fieldInfo != null ? GetDisplayName(fieldInfo, valueString) : string.Empty;
        }

        private static string GetDisplayName(MemberInfo memberInfo, string valueString)
        {
            var resourcesDisplayNameAttribute = (ResourcesDisplayNameAttribute) Attribute.GetCustomAttribute(memberInfo, typeof(ResourcesDisplayNameAttribute));
            return resourcesDisplayNameAttribute != null ? resourcesDisplayNameAttribute.DisplayName : valueString;
        }
    }
}