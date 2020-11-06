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

namespace Riskeer.Piping.IO.Configurations.Converters
{
    /// <summary>
    /// Converts <see cref="PipingCalculationConfigurationType"/> to <see cref="string"/> and back.
    /// </summary>
    public class PipingCalculationConfigurationTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var calculationType = (PipingCalculationConfigurationType) value;
            if (!Enum.IsDefined(typeof(PipingCalculationConfigurationType), calculationType))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) calculationType,
                                                       typeof(PipingCalculationConfigurationType));
            }

            if (destinationType == typeof(string))
            {
                switch (calculationType)
                {
                    case PipingCalculationConfigurationType.SemiProbabilistic:
                        return PipingCalculationConfigurationSchemaIdentifiers.SemiProbabilistic;
                    case PipingCalculationConfigurationType.Probabilistic:
                        return PipingCalculationConfigurationSchemaIdentifiers.Probabilistic;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}