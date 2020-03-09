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
using Riskeer.GrassCoverErosionOutwards.Data;
using RiskeerGrassCoverErosionOutwardsDataResources = Riskeer.GrassCoverErosionOutwards.Data.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.IO.Configurations.Converters
{
    /// <summary>
    /// Converts <see cref="ConfigurationGrassCoverErosionOutwardsCalculationType"/> to <see cref="string"/>
    /// or <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationType"/> and back.
    /// </summary>
    public class ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType)
                   || base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationGrassCoverErosionOutwardsCalculationType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var calculationType = (ConfigurationGrassCoverErosionOutwardsCalculationType) value;
            if (!Enum.IsDefined(typeof(ConfigurationGrassCoverErosionOutwardsCalculationType), calculationType))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) calculationType,
                                                       typeof(ConfigurationGrassCoverErosionOutwardsCalculationType));
            }

            if (destinationType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType))
            {
                switch (calculationType)
                {
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUp:
                        return GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact:
                        return GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndWaveImpact:
                        return GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.TailorMadeWaveImpact:
                        return GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndTailorMadeWaveImpact:
                        return GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.All:
                        return GrassCoverErosionOutwardsWaveConditionsCalculationType.All;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(string))
            {
                switch (calculationType)
                {
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUp:
                        return RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveRunUp_DisplayName;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact:
                        return RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveImpact_DisplayName;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndWaveImpact:
                        return RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveRunUp_and_WaveImpact_DisplayName;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.TailorMadeWaveImpact:
                        return RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_TailorMadeWaveImpact_DisplayName;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndTailorMadeWaveImpact:
                        return RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveRunUp_and_TailorMadeWaveImpact_DisplayName;
                    case ConfigurationGrassCoverErosionOutwardsCalculationType.All:
                        return RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_All_DisplayName;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                   || sourceType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType)
                   || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is GrassCoverErosionOutwardsWaveConditionsCalculationType)
            {
                return GetConfigurationGrassCoverErosionOutwardsCalculationType(value);
            }

            var stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue == RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveRunUp_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUp;
                }

                if (stringValue == RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveImpact_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact;
                }

                if (stringValue == RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveRunUp_and_WaveImpact_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndWaveImpact;
                }

                if (stringValue == RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_TailorMadeWaveImpact_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.TailorMadeWaveImpact;
                }

                if (stringValue == RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_WaveRunUp_and_TailorMadeWaveImpact_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndTailorMadeWaveImpact;
                }

                if (stringValue == RiskeerGrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsWaveConditionsCalculationType_All_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.All;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        private static object GetConfigurationGrassCoverErosionOutwardsCalculationType(object value)
        {
            var calculationType = (GrassCoverErosionOutwardsWaveConditionsCalculationType) value;
            if (!Enum.IsDefined(typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType), calculationType))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) calculationType,
                                                       typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType));
            }

            switch (calculationType)
            {
                case GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp:
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUp;
                case GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact:
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact;
                case GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact:
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndWaveImpact;
                case GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact:
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.TailorMadeWaveImpact;
                case GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact:
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndTailorMadeWaveImpact;
                case GrassCoverErosionOutwardsWaveConditionsCalculationType.All:
                    return ConfigurationGrassCoverErosionOutwardsCalculationType.All;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}