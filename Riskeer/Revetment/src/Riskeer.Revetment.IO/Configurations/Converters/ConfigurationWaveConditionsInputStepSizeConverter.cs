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
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.IO.Configurations.Converters
{
    /// <summary>
    /// Converts <see cref="ConfigurationWaveConditionsInputStepSize"/> to <see cref="string"/> and back.
    /// </summary>
    public class ConfigurationWaveConditionsInputStepSizeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(WaveConditionsInputStepSize)
                   || base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationWaveConditionsInputStepSize"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var stepSize = (ConfigurationWaveConditionsInputStepSize) value;
            if (!Enum.IsDefined(typeof(ConfigurationWaveConditionsInputStepSize), stepSize))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) stepSize,
                                                       typeof(ConfigurationWaveConditionsInputStepSize));
            }

            if (destinationType == typeof(WaveConditionsInputStepSize))
            {
                switch (stepSize)
                {
                    case ConfigurationWaveConditionsInputStepSize.Half:
                        return WaveConditionsInputStepSize.Half;
                    case ConfigurationWaveConditionsInputStepSize.One:
                        return WaveConditionsInputStepSize.One;
                    case ConfigurationWaveConditionsInputStepSize.Two:
                        return WaveConditionsInputStepSize.Two;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(string))
            {
                switch (stepSize)
                {
                    case ConfigurationWaveConditionsInputStepSize.Half:
                        return FormatStepSizeValue(culture, 0.5);
                    case ConfigurationWaveConditionsInputStepSize.One:
                        return FormatStepSizeValue(culture, 1.0);
                    case ConfigurationWaveConditionsInputStepSize.Two:
                        return FormatStepSizeValue(culture, 2.0);
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(double?)
                   || sourceType == typeof(WaveConditionsInputStepSize)
                   || base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="WaveConditionsInputStepSize"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is WaveConditionsInputStepSize)
            {
                var stepSize = (WaveConditionsInputStepSize) value;
                if (!Enum.IsDefined(typeof(WaveConditionsInputStepSize), stepSize))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) stepSize,
                                                           typeof(WaveConditionsInputStepSize));
                }

                switch (stepSize)
                {
                    case WaveConditionsInputStepSize.Half:
                        return ConfigurationWaveConditionsInputStepSize.Half;
                    case WaveConditionsInputStepSize.One:
                        return ConfigurationWaveConditionsInputStepSize.One;
                    case WaveConditionsInputStepSize.Two:
                        return ConfigurationWaveConditionsInputStepSize.Two;
                    default:
                        throw new NotSupportedException();
                }
            }

            var doubleValue = value as double?;
            if (doubleValue != null)
            {
                if (Math.Abs(doubleValue.Value - 0.5) < double.Epsilon)
                {
                    return ConfigurationWaveConditionsInputStepSize.Half;
                }

                if (Math.Abs(doubleValue.Value - 1) < double.Epsilon)
                {
                    return ConfigurationWaveConditionsInputStepSize.One;
                }

                if (Math.Abs(doubleValue.Value - 2) < double.Epsilon)
                {
                    return ConfigurationWaveConditionsInputStepSize.Two;
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