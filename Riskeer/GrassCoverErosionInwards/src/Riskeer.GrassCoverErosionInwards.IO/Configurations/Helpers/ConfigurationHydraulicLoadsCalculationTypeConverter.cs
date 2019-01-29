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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.Properties;

namespace Riskeer.GrassCoverErosionInwards.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationHydraulicLoadsCalculationType"/> to <see cref="string"/> and back.
    /// </summary>
    public class ConfigurationHydraulicLoadsCalculationTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(DikeHeightCalculationType)
                   || destinationType == typeof(OvertoppingRateCalculationType)
                   || base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationHydraulicLoadsCalculationType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var hydraulicLoadsCalculationType = (ConfigurationHydraulicLoadsCalculationType) value;
            if (!Enum.IsDefined(typeof(ConfigurationHydraulicLoadsCalculationType), hydraulicLoadsCalculationType))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) hydraulicLoadsCalculationType,
                                                       typeof(ConfigurationHydraulicLoadsCalculationType));
            }

            if (destinationType == typeof(DikeHeightCalculationType))
            {
                switch (hydraulicLoadsCalculationType)
                {
                    case ConfigurationHydraulicLoadsCalculationType.NoCalculation:
                        return DikeHeightCalculationType.NoCalculation;
                    case ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm:
                        return DikeHeightCalculationType.CalculateByAssessmentSectionNorm;
                    case ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability:
                        return DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(OvertoppingRateCalculationType))
            {
                switch (hydraulicLoadsCalculationType)
                {
                    case ConfigurationHydraulicLoadsCalculationType.NoCalculation:
                        return OvertoppingRateCalculationType.NoCalculation;
                    case ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm:
                        return OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm;
                    case ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability:
                        return OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(string))
            {
                switch (hydraulicLoadsCalculationType)
                {
                    case ConfigurationHydraulicLoadsCalculationType.NoCalculation:
                        return Resources.ReadHydraulicLoadsCalculationTypeConverter_NoCalculation;
                    case ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm:
                        return Resources.ReadHydraulicLoadsCalculationTypeConverter_CalculateByAssessmentSectionNorm;
                    case ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability:
                        return Resources.ReadHydraulicLoadsCalculationTypeConverter_CalculateByProfileSpecificRequiredProbability;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                   || sourceType == typeof(DikeHeightCalculationType)
                   || sourceType == typeof(OvertoppingRateCalculationType)
                   || base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="DikeHeightCalculationType"/> or <see cref="OvertoppingRateCalculationType"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is DikeHeightCalculationType)
            {
                var dikeHeightCalculationType = (DikeHeightCalculationType) value;
                if (!Enum.IsDefined(typeof(DikeHeightCalculationType), dikeHeightCalculationType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) dikeHeightCalculationType,
                                                           typeof(DikeHeightCalculationType));
                }

                switch (dikeHeightCalculationType)
                {
                    case DikeHeightCalculationType.NoCalculation:
                        return ConfigurationHydraulicLoadsCalculationType.NoCalculation;
                    case DikeHeightCalculationType.CalculateByAssessmentSectionNorm:
                        return ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm;
                    case DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability:
                        return ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (value is OvertoppingRateCalculationType)
            {
                var overtoppingRateCalculationType = (OvertoppingRateCalculationType) value;
                if (!Enum.IsDefined(typeof(OvertoppingRateCalculationType), overtoppingRateCalculationType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) overtoppingRateCalculationType,
                                                           typeof(OvertoppingRateCalculationType));
                }

                switch (overtoppingRateCalculationType)
                {
                    case OvertoppingRateCalculationType.NoCalculation:
                        return ConfigurationHydraulicLoadsCalculationType.NoCalculation;
                    case OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm:
                        return ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm;
                    case OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability:
                        return ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability;
                    default:
                        throw new NotSupportedException();
                }
            }

            var text = value as string;
            if (text != null)
            {
                if (text == Resources.ReadHydraulicLoadsCalculationTypeConverter_NoCalculation)
                {
                    return ConfigurationHydraulicLoadsCalculationType.NoCalculation;
                }

                if (text == Resources.ReadHydraulicLoadsCalculationTypeConverter_CalculateByAssessmentSectionNorm)
                {
                    return ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm;
                }

                if (text == Resources.ReadHydraulicLoadsCalculationTypeConverter_CalculateByProfileSpecificRequiredProbability)
                {
                    return ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}