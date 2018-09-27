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
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationGridDeterminationType"/> to <see cref="string"/> and back.
    /// </summary>
    public class ConfigurationGridDeterminationTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MacroStabilityInwardsGridDeterminationType)
                   || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(MacroStabilityInwardsGridDeterminationType)
                   || base.CanConvertTo(context, sourceType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationGridDeterminationType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var type = (ConfigurationGridDeterminationType) value;
            if (!Enum.IsDefined(typeof(ConfigurationGridDeterminationType), type))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) type,
                                                       typeof(ConfigurationGridDeterminationType));
            }

            if (destinationType == typeof(string))
            {
                switch (type)
                {
                    case ConfigurationGridDeterminationType.Automatic:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeAutomatic;
                    case ConfigurationGridDeterminationType.Manual:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeManual;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(MacroStabilityInwardsGridDeterminationType))
            {
                switch (type)
                {
                    case ConfigurationGridDeterminationType.Automatic:
                        return MacroStabilityInwardsGridDeterminationType.Automatic;
                    case ConfigurationGridDeterminationType.Manual:
                        return MacroStabilityInwardsGridDeterminationType.Manual;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="MacroStabilityInwardsGridDeterminationType"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                switch (text)
                {
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeAutomatic:
                        return ConfigurationGridDeterminationType.Automatic;
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeManual:
                        return ConfigurationGridDeterminationType.Manual;
                    default:
                        throw new NotSupportedException();
                }
            }

            var gridDeterminationType = value as MacroStabilityInwardsGridDeterminationType?;
            if (gridDeterminationType != null)
            {
                if (!Enum.IsDefined(typeof(MacroStabilityInwardsGridDeterminationType), gridDeterminationType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) gridDeterminationType,
                                                           typeof(MacroStabilityInwardsGridDeterminationType));
                }

                switch (gridDeterminationType)
                {
                    case MacroStabilityInwardsGridDeterminationType.Automatic:
                        return ConfigurationGridDeterminationType.Automatic;
                    case MacroStabilityInwardsGridDeterminationType.Manual:
                        return ConfigurationGridDeterminationType.Manual;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}