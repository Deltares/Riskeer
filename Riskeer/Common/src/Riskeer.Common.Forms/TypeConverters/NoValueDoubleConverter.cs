﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Globalization;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.TypeConverters
{
    /// <summary>
    /// Converter to display <see cref="double"/> as <see cref="string"/>.
    /// </summary>
    public class NoValueDoubleConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                return string.IsNullOrWhiteSpace(text) || text.Trim() == Resources.RoundedDouble_No_result_dash
                           ? double.NaN
                           : Convert.ToDouble(text);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var doubleValue = (double) value;
            if (destinationType == typeof(string))
            {
                return double.IsNaN(doubleValue)
                           ? Resources.RoundedDouble_No_result_dash
                           : Convert.ToString(doubleValue, CultureInfo.CurrentCulture);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}