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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.Properties;

namespace Ringtoets.GrassCoverErosionInwards.IO.Configurations.Helpers
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

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var readHydraulicLoadsCalculationType = (ConfigurationHydraulicLoadsCalculationType) value;

            if (destinationType == typeof(DikeHeightCalculationType))
            {
                switch (readHydraulicLoadsCalculationType)
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
                switch (readHydraulicLoadsCalculationType)
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
                switch (readHydraulicLoadsCalculationType)
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

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is DikeHeightCalculationType)
            {
                switch ((DikeHeightCalculationType) value)
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
                switch ((OvertoppingRateCalculationType) value)
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