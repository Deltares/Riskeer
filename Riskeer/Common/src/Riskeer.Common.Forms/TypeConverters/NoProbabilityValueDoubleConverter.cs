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
using System.ComponentModel;
using System.Globalization;
using Core.Common.Base.Data;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Properties;
using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Common.Forms.TypeConverters
{
    /// <summary>
    /// This class is a variant of <see cref="NoValueRoundedDoubleConverter"/>,
    /// intended to display probabilities with <see cref="double"/> values instead of general 
    /// <see cref="RoundedDouble"/> variables.
    /// </summary>
    public class NoProbabilityValueDoubleConverter : TypeConverter
    {
        private const string returnPeriodNotation = "1/";

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var doubleValue = (double) value;
            if (destinationType == typeof(string))
            {
                if (double.IsNaN(doubleValue))
                {
                    return Resources.RoundedDouble_No_result_dash;
                }

                if (double.IsNegativeInfinity(doubleValue))
                {
                    return CommonBaseResources.RoundedDouble_ToString_NegativeInfinity;
                }

                if (double.IsPositiveInfinity(doubleValue))
                {
                    return CommonBaseResources.RoundedDouble_ToString_PositiveInfinity;
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
                if (string.IsNullOrWhiteSpace(text) || text.Trim() == Resources.RoundedDouble_No_result_dash)
                {
                    return double.NaN;
                }

                try
                {
                    if (!text.StartsWith(returnPeriodNotation))
                    {
                        return Convert.ToDouble(text);
                    }

                    string returnPeriodValue = text.Substring(2).ToLower();
                    return returnPeriodValue != CommonBaseResources.RoundedDouble_ToString_PositiveInfinity.ToLower()
                               ? 1 / Convert.ToDouble(returnPeriodValue)
                               : 0.0;
                }
                catch (FormatException exception)
                {
                    throw new NotSupportedException(Resources.Probability_Could_not_parse_string_to_probability,
                                                    exception);
                }
                catch (OverflowException exception)
                {
                    throw new NotSupportedException(Resources.Probability_Value_too_large,
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