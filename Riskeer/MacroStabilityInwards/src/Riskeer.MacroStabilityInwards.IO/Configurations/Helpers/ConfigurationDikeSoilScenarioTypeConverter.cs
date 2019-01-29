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
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationDikeSoilScenario"/> to <see cref="string"/> and back.
    /// </summary>
    public class ConfigurationDikeSoilScenarioTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MacroStabilityInwardsDikeSoilScenario)
                   || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(MacroStabilityInwardsDikeSoilScenario)
                   || base.CanConvertTo(context, sourceType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationDikeSoilScenario"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var type = (ConfigurationDikeSoilScenario) value;
            if (!Enum.IsDefined(typeof(ConfigurationDikeSoilScenario), type))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) type,
                                                       typeof(ConfigurationDikeSoilScenario));
            }

            if (destinationType == typeof(string))
            {
                switch (type)
                {
                    case ConfigurationDikeSoilScenario.ClayDikeOnClay:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioClayDikeOnClay;
                    case ConfigurationDikeSoilScenario.SandDikeOnClay:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioSandDikeOnClay;
                    case ConfigurationDikeSoilScenario.ClayDikeOnSand:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioClayDikeOnSand;
                    case ConfigurationDikeSoilScenario.SandDikeOnSand:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioSandDikeOnSand;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(MacroStabilityInwardsDikeSoilScenario))
            {
                switch (type)
                {
                    case ConfigurationDikeSoilScenario.ClayDikeOnClay:
                        return MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay;
                    case ConfigurationDikeSoilScenario.SandDikeOnClay:
                        return MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay;
                    case ConfigurationDikeSoilScenario.ClayDikeOnSand:
                        return MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand;
                    case ConfigurationDikeSoilScenario.SandDikeOnSand:
                        return MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="MacroStabilityInwardsDikeSoilScenario"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                switch (text)
                {
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioClayDikeOnClay:
                        return ConfigurationDikeSoilScenario.ClayDikeOnClay;
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioSandDikeOnClay:
                        return ConfigurationDikeSoilScenario.SandDikeOnClay;
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioClayDikeOnSand:
                        return ConfigurationDikeSoilScenario.ClayDikeOnSand;
                    case MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioSandDikeOnSand:
                        return ConfigurationDikeSoilScenario.SandDikeOnSand;
                    default:
                        throw new NotSupportedException();
                }
            }

            var dikeSoilScenario = value as MacroStabilityInwardsDikeSoilScenario?;
            if (dikeSoilScenario != null)
            {
                if (!Enum.IsDefined(typeof(MacroStabilityInwardsDikeSoilScenario), dikeSoilScenario))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) dikeSoilScenario,
                                                           typeof(MacroStabilityInwardsDikeSoilScenario));
                }

                switch (dikeSoilScenario)
                {
                    case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay:
                        return ConfigurationDikeSoilScenario.ClayDikeOnClay;
                    case MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay:
                        return ConfigurationDikeSoilScenario.SandDikeOnClay;
                    case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand:
                        return ConfigurationDikeSoilScenario.ClayDikeOnSand;
                    case MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand:
                        return ConfigurationDikeSoilScenario.SandDikeOnSand;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}