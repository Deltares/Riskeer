// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.ComponentModel;
using System.Globalization;
using Core.Common.Base.Data;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.TypeConverters
{
    public class NoProbabilityValueRoundedDoubleConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var doubleValue = (RoundedDouble) value;
            if (destinationType == typeof(string))
            {
                if (double.IsNaN(doubleValue))
                {
                    return Resources.RoundedRouble_No_result_dash;
                }
                if (double.IsNegativeInfinity(doubleValue))
                {
                    return Core.Common.Base.Properties.Resources.RoundedDouble_ToString_NegativeInfinity;
                }
                if (double.IsPositiveInfinity(doubleValue))
                {
                    return Core.Common.Base.Properties.Resources.RoundedDouble_ToString_PositiveInfinity;
                }

                return ProbabilityFormattingHelper.Format(doubleValue);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                if (string.IsNullOrWhiteSpace(text) || text.Trim() == Resources.RoundedRouble_No_result_dash)
                {
                    return RoundedDouble.NaN;
                }

                try
                {
                    return (RoundedDouble)Convert.ToDouble(text);
                }
                catch (FormatException exception)
                {
                    throw new NotSupportedException(Core.Common.Base.Properties.Resources.RoundedDoubleConverter_ConvertFrom_String_must_represent_number,
                                                    exception);
                }
                catch (OverflowException exception)
                {
                    throw new NotSupportedException(Core.Common.Base.Properties.Resources.RoundedDoubleConverter_ConvertFrom_String_too_small_or_too_big_to_represent_as_double,
                                                    exception);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
    }
}