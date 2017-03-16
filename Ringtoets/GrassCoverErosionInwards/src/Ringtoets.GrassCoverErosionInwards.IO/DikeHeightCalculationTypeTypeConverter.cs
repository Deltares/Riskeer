// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.GrassCoverErosionInwards.IO
{
    /// <summary>
    /// Converts <see cref="DikeHeightCalculationType"/> to <see cref="string"/> and back.
    /// </summary>
    public class DikeHeightCalculationTypeTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var dikeHeightCalculationType = (DikeHeightCalculationType)value;
                switch (dikeHeightCalculationType)
                {
                    case DikeHeightCalculationType.NoCalculation:
                        return Resources.DikeHeightCalculationTypeTypeConverter_NoCalculation;
                    case DikeHeightCalculationType.CalculateByAssessmentSectionNorm:
                        return Resources.DikeHeightCalculationTypeTypeConverter_CalculateByAssessmentSectionNorm;
                    case DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability:
                        return Resources.DikeHeightCalculationTypeTypeConverter_CalculateByProfileSpecificRequiredProbability;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                if (text == Resources.DikeHeightCalculationTypeTypeConverter_NoCalculation)
                {
                    return DikeHeightCalculationType.NoCalculation;
                }
                if(text == Resources.DikeHeightCalculationTypeTypeConverter_CalculateByAssessmentSectionNorm)
                {
                    return DikeHeightCalculationType.CalculateByAssessmentSectionNorm;
                }
                if (text == Resources.DikeHeightCalculationTypeTypeConverter_CalculateByProfileSpecificRequiredProbability)
                {
                    return DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}