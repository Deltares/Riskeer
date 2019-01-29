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
using Ringtoets.Common.Data.AssessmentSection;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Riskeer.Revetment.IO.Configurations.Converters
{
    /// <summary>
    /// Converts <see cref="ConfigurationAssessmentSectionCategoryType"/> to <see cref="string"/>
    /// or <see cref="AssessmentSectionCategoryType"/> and back.
    /// </summary>
    public class ConfigurationAssessmentSectionCategoryTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(AssessmentSectionCategoryType)
                   || base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationAssessmentSectionCategoryType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var categoryType = (ConfigurationAssessmentSectionCategoryType) value;
            if (!Enum.IsDefined(typeof(ConfigurationAssessmentSectionCategoryType), categoryType))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) categoryType,
                                                       typeof(ConfigurationAssessmentSectionCategoryType));
            }

            if (destinationType == typeof(AssessmentSectionCategoryType))
            {
                switch (categoryType)
                {
                    case ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm:
                        return AssessmentSectionCategoryType.FactorizedSignalingNorm;
                    case ConfigurationAssessmentSectionCategoryType.SignalingNorm:
                        return AssessmentSectionCategoryType.SignalingNorm;
                    case ConfigurationAssessmentSectionCategoryType.LowerLimitNorm:
                        return AssessmentSectionCategoryType.LowerLimitNorm;
                    case ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm:
                        return AssessmentSectionCategoryType.FactorizedLowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(string))
            {
                switch (categoryType)
                {
                    case ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm:
                        return RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedSignalingNorm_DisplayName;
                    case ConfigurationAssessmentSectionCategoryType.SignalingNorm:
                        return RingtoetsCommonDataResources.AssessmentSectionCategoryType_SignalingNorm_DisplayName;
                    case ConfigurationAssessmentSectionCategoryType.LowerLimitNorm:
                        return RingtoetsCommonDataResources.AssessmentSectionCategoryType_LowerLimitNorm_DisplayName;
                    case ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm:
                        return RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedLowerLimitNorm_DisplayName;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                   || sourceType == typeof(AssessmentSectionCategoryType)
                   || base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationAssessmentSectionCategoryType"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is AssessmentSectionCategoryType)
            {
                var categoryType = (AssessmentSectionCategoryType) value;
                if (!Enum.IsDefined(typeof(AssessmentSectionCategoryType), categoryType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) categoryType,
                                                           typeof(AssessmentSectionCategoryType));
                }

                switch (categoryType)
                {
                    case AssessmentSectionCategoryType.FactorizedSignalingNorm:
                        return ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm;
                    case AssessmentSectionCategoryType.SignalingNorm:
                        return ConfigurationAssessmentSectionCategoryType.SignalingNorm;
                    case AssessmentSectionCategoryType.LowerLimitNorm:
                        return ConfigurationAssessmentSectionCategoryType.LowerLimitNorm;
                    case AssessmentSectionCategoryType.FactorizedLowerLimitNorm:
                        return ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            var stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue == RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedSignalingNorm_DisplayName)
                {
                    return ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm;
                }

                if (stringValue == RingtoetsCommonDataResources.AssessmentSectionCategoryType_SignalingNorm_DisplayName)
                {
                    return ConfigurationAssessmentSectionCategoryType.SignalingNorm;
                }

                if (stringValue == RingtoetsCommonDataResources.AssessmentSectionCategoryType_LowerLimitNorm_DisplayName)
                {
                    return ConfigurationAssessmentSectionCategoryType.LowerLimitNorm;
                }

                if (stringValue == RingtoetsCommonDataResources.AssessmentSectionCategoryType_FactorizedLowerLimitNorm_DisplayName)
                {
                    return ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}