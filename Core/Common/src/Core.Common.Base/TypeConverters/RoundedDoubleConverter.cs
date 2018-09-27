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
using System.ComponentModel;
using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Base.Properties;

namespace Core.Common.Base.TypeConverters
{
    /// <summary>
    /// Class that converts <see cref="string"/> representations of a number to a corresponding
    /// instance of <see cref="RoundedDouble"/>.
    /// </summary>
    public class RoundedDoubleConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                try
                {
                    return (RoundedDouble) Convert.ToDouble(text, CultureInfo.CurrentCulture);
                }
                catch (FormatException exception)
                {
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        throw new NotSupportedException(Resources.RoundedDoubleConverter_ConvertFrom_String_cannot_be_empty,
                                                        exception);
                    }

                    throw new NotSupportedException(Resources.RoundedDoubleConverter_ConvertFrom_String_must_represent_number,
                                                    exception);
                }
                catch (OverflowException exception)
                {
                    throw new NotSupportedException(Resources.RoundedDoubleConverter_ConvertFrom_String_too_small_or_too_big_to_represent_as_double,
                                                    exception);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}