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
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Revetment.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationAssessmentSectionCategoryType"/> to <see cref="string"/>
    /// or <see cref="AssessmentSectionCategoryType"/> and back.
    /// </summary>
    public class ConfigurationAssessmentSectionCategoryTypeConverter : TypeConverter
    {
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

            return base.ConvertFrom(context, culture, value);
        }
    }
}