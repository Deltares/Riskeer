// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.StabilityStoneCover.Data;
using RiskeerStabilityStoneCoverDataResources = Riskeer.StabilityStoneCover.Data.Properties.Resources;

namespace Riskeer.StabilityStoneCover.IO.Configurations.Converters
{
    /// <summary>
    /// Converts <see cref="ConfigurationStabilityStoneCoverCalculationType"/> to <see cref="string"/>
    /// or <see cref="StabilityStoneCoverWaveConditionsCalculationType"/> and back.
    /// </summary>
    public class ConfigurationStabilityStoneCoverCalculationTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(StabilityStoneCoverWaveConditionsCalculationType)
                   || base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationStabilityStoneCoverCalculationType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var calculationType = (ConfigurationStabilityStoneCoverCalculationType) value;
            if (!Enum.IsDefined(typeof(ConfigurationStabilityStoneCoverCalculationType), calculationType))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) calculationType,
                                                       typeof(ConfigurationStabilityStoneCoverCalculationType));
            }

            if (destinationType == typeof(StabilityStoneCoverWaveConditionsCalculationType))
            {
                switch (calculationType)
                {
                    case ConfigurationStabilityStoneCoverCalculationType.Columns:
                        return StabilityStoneCoverWaveConditionsCalculationType.Columns;
                    case ConfigurationStabilityStoneCoverCalculationType.Blocks:
                        return StabilityStoneCoverWaveConditionsCalculationType.Blocks;
                    case ConfigurationStabilityStoneCoverCalculationType.Both:
                        return StabilityStoneCoverWaveConditionsCalculationType.Both;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(string))
            {
                switch (calculationType)
                {
                    case ConfigurationStabilityStoneCoverCalculationType.Columns:
                        return RiskeerStabilityStoneCoverDataResources.StabilityStoneCoverWaveConditionsCalculationType_Columns_DisplayName;
                    case ConfigurationStabilityStoneCoverCalculationType.Blocks:
                        return RiskeerStabilityStoneCoverDataResources.StabilityStoneCoverWaveConditionsCalculationType_Blocks_DisplayName;
                    case ConfigurationStabilityStoneCoverCalculationType.Both:
                        return RiskeerStabilityStoneCoverDataResources.StabilityStoneCoverWaveConditionsCalculationType_Both_DisplayName;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                   || sourceType == typeof(StabilityStoneCoverWaveConditionsCalculationType)
                   || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is StabilityStoneCoverWaveConditionsCalculationType)
            {
                var calculationType = (StabilityStoneCoverWaveConditionsCalculationType) value;
                if (!Enum.IsDefined(typeof(StabilityStoneCoverWaveConditionsCalculationType), calculationType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) calculationType,
                                                           typeof(StabilityStoneCoverWaveConditionsCalculationType));
                }

                switch (calculationType)
                {
                    case StabilityStoneCoverWaveConditionsCalculationType.Columns:
                        return ConfigurationStabilityStoneCoverCalculationType.Columns;
                    case StabilityStoneCoverWaveConditionsCalculationType.Blocks:
                        return ConfigurationStabilityStoneCoverCalculationType.Blocks;
                    case StabilityStoneCoverWaveConditionsCalculationType.Both:
                        return ConfigurationStabilityStoneCoverCalculationType.Both;
                    default:
                        throw new NotSupportedException();
                }
            }

            var stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue == RiskeerStabilityStoneCoverDataResources.StabilityStoneCoverWaveConditionsCalculationType_Columns_DisplayName)
                {
                    return ConfigurationStabilityStoneCoverCalculationType.Columns;
                }
                if (stringValue == RiskeerStabilityStoneCoverDataResources.StabilityStoneCoverWaveConditionsCalculationType_Blocks_DisplayName)
                {
                    return ConfigurationStabilityStoneCoverCalculationType.Blocks;
                }
                if (stringValue == RiskeerStabilityStoneCoverDataResources.StabilityStoneCoverWaveConditionsCalculationType_Both_DisplayName)
                {
                    return ConfigurationStabilityStoneCoverCalculationType.Both;
                }
            }
            
            return base.ConvertFrom(context, culture, value);
        }
    }
}