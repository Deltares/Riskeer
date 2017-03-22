// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Revetment.IO
{
    /// <summary>
    /// Converts <see cref="ReadWaveConditionsInputStepSize"/> to <see cref="string"/> and back.
    /// </summary>
    public class ReadWaveConditionsInputStepSizeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                switch ((ReadWaveConditionsInputStepSize) value)
                {
                    case ReadWaveConditionsInputStepSize.Half:
                        return FormatStepSizeValue(culture, 0.5);
                    case ReadWaveConditionsInputStepSize.One:
                        return FormatStepSizeValue(culture, 1.0);
                    case ReadWaveConditionsInputStepSize.Two:
                        return FormatStepSizeValue(culture, 2.0);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

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
                string decimalSeperator = culture.NumberFormat.NumberDecimalSeparator;
                if ($"0{decimalSeperator}5".Equals(text))
                {
                    return ReadWaveConditionsInputStepSize.Half;
                }
                if ($"1{decimalSeperator}0".Equals(text))
                {
                    return ReadWaveConditionsInputStepSize.One;
                }
                if ($"2{decimalSeperator}0".Equals(text))
                {
                    return ReadWaveConditionsInputStepSize.Two;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        private static string FormatStepSizeValue(CultureInfo culture, double stepSizeValue)
        {
            return string.Format(culture, "{0:0.0}", stepSizeValue);
        }
    }
}