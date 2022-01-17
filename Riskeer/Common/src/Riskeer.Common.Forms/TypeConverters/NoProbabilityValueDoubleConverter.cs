// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Data;
using Riskeer.Common.Forms.Exceptions;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.TypeConverters
{
    /// <summary>
    /// This class is a variant of <see cref="NoValueRoundedDoubleConverter"/>,
    /// intended to display probabilities with <see cref="double"/> values instead of general 
    /// <see cref="RoundedDouble"/> variables.
    /// </summary>
    public class NoProbabilityValueDoubleConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var doubleValue = (double) value;
            if (destinationType == typeof(string))
            {
                return ProbabilityFormattingHelper.FormatWithDiscreteNumbers(doubleValue);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                try
                {
                    return ProbabilityParsingHelper.Parse(text);
                }
                catch (ProbabilityParsingException exception)
                {
                    throw new NotSupportedException(exception.Message, exception);
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