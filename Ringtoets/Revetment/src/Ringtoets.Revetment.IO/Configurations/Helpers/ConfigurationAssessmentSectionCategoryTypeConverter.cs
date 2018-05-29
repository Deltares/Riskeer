﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Revetment.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationAssessmentSectionCategoryType"/> to <see cref="string"/>
    /// or <see cref="AssessmentSectionCategoryType"/> and back.
    /// </summary>
    public class ConfigurationAssessmentSectionCategoryTypeConverter : TypeConverter
    {
        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationAssessmentSectionCategoryType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var stepSize = (ConfigurationAssessmentSectionCategoryType)value;
            if (!Enum.IsDefined(typeof(ConfigurationAssessmentSectionCategoryType), stepSize))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int)stepSize,
                                                       typeof(ConfigurationAssessmentSectionCategoryType));
            }

            if (destinationType == typeof(string))
            {
                switch (stepSize)
                {
                    case ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm:
                        return "A+->A";
                    case ConfigurationAssessmentSectionCategoryType.SignalingNorm:
                        return "A->B";
                    case ConfigurationAssessmentSectionCategoryType.LowerLimitNorm:
                        return "B->C";
                    case ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm:
                        return "C->D";
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
                var categoryType = (AssessmentSectionCategoryType)value;
                if (!Enum.IsDefined(typeof(AssessmentSectionCategoryType), categoryType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int)categoryType,
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
                if (stringValue == "A+->A")
                {
                    return ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm;
                }
                if (stringValue == "A->B")
                {
                    return ConfigurationAssessmentSectionCategoryType.SignalingNorm;
                }
                if (stringValue == "B->C")
                {
                    return ConfigurationAssessmentSectionCategoryType.LowerLimitNorm;
                }
                if (stringValue == "C->D")
                {
                    return ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}