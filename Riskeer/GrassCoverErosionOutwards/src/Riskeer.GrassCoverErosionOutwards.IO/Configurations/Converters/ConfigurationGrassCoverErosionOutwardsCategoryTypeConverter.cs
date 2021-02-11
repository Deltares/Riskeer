﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.IO.Configurations.Converters
{
    /// <summary>
    /// Converts <see cref="ConfigurationGrassCoverErosionOutwardsCategoryType"/> to <see cref="string"/>
    /// or <see cref="FailureMechanismCategoryType"/> and back.
    /// </summary>
    public class ConfigurationGrassCoverErosionOutwardsCategoryTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(FailureMechanismCategoryType)
                   || base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationGrassCoverErosionOutwardsCategoryType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var categoryType = (ConfigurationGrassCoverErosionOutwardsCategoryType) value;
            if (!Enum.IsDefined(typeof(ConfigurationGrassCoverErosionOutwardsCategoryType), categoryType))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) categoryType,
                                                       typeof(ConfigurationGrassCoverErosionOutwardsCategoryType));
            }

            if (destinationType == typeof(FailureMechanismCategoryType))
            {
                switch (categoryType)
                {
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificFactorizedSignalingNorm:
                        return FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm;
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificSignalingNorm:
                        return FailureMechanismCategoryType.MechanismSpecificSignalingNorm;
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificLowerLimitNorm:
                        return FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm;
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.LowerLimitNorm:
                        return FailureMechanismCategoryType.LowerLimitNorm;
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.FactorizedLowerLimitNorm:
                        return FailureMechanismCategoryType.FactorizedLowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(string))
            {
                switch (categoryType)
                {
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificFactorizedSignalingNorm:
                        return RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName;
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificSignalingNorm:
                        return RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName;
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificLowerLimitNorm:
                        return RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName;
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.LowerLimitNorm:
                        return RiskeerCommonDataResources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName;
                    case ConfigurationGrassCoverErosionOutwardsCategoryType.FactorizedLowerLimitNorm:
                        return RiskeerCommonDataResources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                   || sourceType == typeof(FailureMechanismCategoryType)
                   || base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationGrassCoverErosionOutwardsCategoryType"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is FailureMechanismCategoryType categoryType)
            {
                if (!Enum.IsDefined(typeof(FailureMechanismCategoryType), categoryType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) categoryType,
                                                           typeof(FailureMechanismCategoryType));
                }

                switch (categoryType)
                {
                    case FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm:
                        return ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificFactorizedSignalingNorm;
                    case FailureMechanismCategoryType.MechanismSpecificSignalingNorm:
                        return ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificSignalingNorm;
                    case FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm:
                        return ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificLowerLimitNorm;
                    case FailureMechanismCategoryType.LowerLimitNorm:
                        return ConfigurationGrassCoverErosionOutwardsCategoryType.LowerLimitNorm;
                    case FailureMechanismCategoryType.FactorizedLowerLimitNorm:
                        return ConfigurationGrassCoverErosionOutwardsCategoryType.FactorizedLowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (value is string stringValue)
            {
                if (stringValue == RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificFactorizedSignalingNorm;
                }

                if (stringValue == RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificSignalingNorm;
                }

                if (stringValue == RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCategoryType.MechanismSpecificLowerLimitNorm;
                }

                if (stringValue == RiskeerCommonDataResources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCategoryType.LowerLimitNorm;
                }

                if (stringValue == RiskeerCommonDataResources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName)
                {
                    return ConfigurationGrassCoverErosionOutwardsCategoryType.FactorizedLowerLimitNorm;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}