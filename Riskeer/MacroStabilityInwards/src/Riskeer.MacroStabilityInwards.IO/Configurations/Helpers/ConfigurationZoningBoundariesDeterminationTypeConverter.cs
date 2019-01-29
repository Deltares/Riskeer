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
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationZoningBoundariesDeterminationType"/> to <see cref="string"/> and back.
    /// </summary>
    public class ConfigurationZoningBoundariesDeterminationTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MacroStabilityInwardsZoningBoundariesDeterminationType)
                   || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(MacroStabilityInwardsZoningBoundariesDeterminationType)
                   || base.CanConvertTo(context, sourceType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationZoningBoundariesDeterminationType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var type = (ConfigurationZoningBoundariesDeterminationType) value;
            if (!Enum.IsDefined(typeof(ConfigurationZoningBoundariesDeterminationType), type))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) type,
                                                       typeof(ConfigurationZoningBoundariesDeterminationType));
            }

            if (destinationType == typeof(string))
            {
                switch (type)
                {
                    case ConfigurationZoningBoundariesDeterminationType.Automatic:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZoningBoundariesDeterminationTypeAutomatic;
                    case ConfigurationZoningBoundariesDeterminationType.Manual:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZoningBoundariesDeterminationTypeManual;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(MacroStabilityInwardsZoningBoundariesDeterminationType))
            {
                switch (type)
                {
                    case ConfigurationZoningBoundariesDeterminationType.Automatic:
                        return MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic;
                    case ConfigurationZoningBoundariesDeterminationType.Manual:
                        return MacroStabilityInwardsZoningBoundariesDeterminationType.Manual;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="MacroStabilityInwardsZoningBoundariesDeterminationType"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                switch (text)
                {
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZoningBoundariesDeterminationTypeAutomatic:
                        return ConfigurationZoningBoundariesDeterminationType.Automatic;
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZoningBoundariesDeterminationTypeManual:
                        return ConfigurationZoningBoundariesDeterminationType.Manual;
                    default:
                        throw new NotSupportedException();
                }
            }

            var zoningBoundariesDeterminationType = value as MacroStabilityInwardsZoningBoundariesDeterminationType?;
            if (zoningBoundariesDeterminationType != null)
            {
                if (!Enum.IsDefined(typeof(MacroStabilityInwardsZoningBoundariesDeterminationType), zoningBoundariesDeterminationType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) zoningBoundariesDeterminationType,
                                                           typeof(MacroStabilityInwardsZoningBoundariesDeterminationType));
                }

                switch (zoningBoundariesDeterminationType)
                {
                    case MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic:
                        return ConfigurationZoningBoundariesDeterminationType.Automatic;
                    case MacroStabilityInwardsZoningBoundariesDeterminationType.Manual:
                        return ConfigurationZoningBoundariesDeterminationType.Manual;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}