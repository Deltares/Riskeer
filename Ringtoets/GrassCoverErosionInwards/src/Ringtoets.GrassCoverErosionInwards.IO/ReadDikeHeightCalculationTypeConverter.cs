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
using Ringtoets.GrassCoverErosionInwards.IO.Properties;

namespace Ringtoets.GrassCoverErosionInwards.IO
{
    /// <summary>
    /// Converts <see cref="ReadDikeHeightCalculationType"/> to <see cref="string"/> and back.
    /// </summary>
    public class ReadDikeHeightCalculationTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var readDikeHeightCalculationType = (ReadDikeHeightCalculationType) value;
                switch (readDikeHeightCalculationType)
                {
                    case ReadDikeHeightCalculationType.NoCalculation:
                        return Resources.ReadDikeHeightCalculationTypeConverter_NoCalculation;
                    case ReadDikeHeightCalculationType.CalculateByAssessmentSectionNorm:
                        return Resources.ReadDikeHeightCalculationTypeConverter_CalculateByAssessmentSectionNorm;
                    case ReadDikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability:
                        return Resources.ReadDikeHeightCalculationTypeConverter_CalculateByProfileSpecificRequiredProbability;
                    default:
                        throw new NotSupportedException();
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
                if (text == Resources.ReadDikeHeightCalculationTypeConverter_NoCalculation)
                {
                    return ReadDikeHeightCalculationType.NoCalculation;
                }
                if (text == Resources.ReadDikeHeightCalculationTypeConverter_CalculateByAssessmentSectionNorm)
                {
                    return ReadDikeHeightCalculationType.CalculateByAssessmentSectionNorm;
                }
                if (text == Resources.ReadDikeHeightCalculationTypeConverter_CalculateByProfileSpecificRequiredProbability)
                {
                    return ReadDikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}