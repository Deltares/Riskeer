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
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationTangentLineDeterminationType"/> to <see cref="string"/> and back.
    /// </summary>
    public class ConfigurationTangentLineDeterminationTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MacroStabilityInwardsTangentLineDeterminationType)
                   || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(MacroStabilityInwardsTangentLineDeterminationType)
                   || base.CanConvertTo(context, sourceType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationTangentLineDeterminationType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var type = (ConfigurationTangentLineDeterminationType) value;
            if (!Enum.IsDefined(typeof(ConfigurationTangentLineDeterminationType), type))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) type,
                                                       typeof(ConfigurationTangentLineDeterminationType));
            }

            if (destinationType == typeof(string))
            {
                switch (type)
                {
                    case ConfigurationTangentLineDeterminationType.Specified:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeSpecified;
                    case ConfigurationTangentLineDeterminationType.LayerSeparated:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeLayerSeparated;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(MacroStabilityInwardsTangentLineDeterminationType))
            {
                switch (type)
                {
                    case ConfigurationTangentLineDeterminationType.Specified:
                        return MacroStabilityInwardsTangentLineDeterminationType.Specified;
                    case ConfigurationTangentLineDeterminationType.LayerSeparated:
                        return MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="MacroStabilityInwardsTangentLineDeterminationType"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                switch (text)
                {
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeLayerSeparated:
                        return ConfigurationTangentLineDeterminationType.LayerSeparated;
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeSpecified:
                        return ConfigurationTangentLineDeterminationType.Specified;
                    default:
                        throw new NotSupportedException();
                }
            }

            var tangentLineDeterminationType = value as MacroStabilityInwardsTangentLineDeterminationType?;
            if (tangentLineDeterminationType != null)
            {
                if (!Enum.IsDefined(typeof(MacroStabilityInwardsTangentLineDeterminationType), tangentLineDeterminationType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) tangentLineDeterminationType,
                                                           typeof(MacroStabilityInwardsTangentLineDeterminationType));
                }

                switch (tangentLineDeterminationType)
                {
                    case MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated:
                        return ConfigurationTangentLineDeterminationType.LayerSeparated;
                    case MacroStabilityInwardsTangentLineDeterminationType.Specified:
                        return ConfigurationTangentLineDeterminationType.Specified;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}