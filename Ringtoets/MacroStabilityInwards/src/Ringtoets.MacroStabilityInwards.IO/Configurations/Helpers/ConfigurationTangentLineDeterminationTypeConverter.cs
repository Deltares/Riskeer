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
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationTangentLineDeterminationType"/> to <see cref="string"/> and back.
    /// </summary>
    public class ConfigurationTangentLineDeterminationTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MacroStabilityInwardsTangentLineDeterminationType)
                   || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var type = (ConfigurationTangentLineDeterminationType) value;
                switch (type)
                {
                    case ConfigurationTangentLineDeterminationType.Specified:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeSpecified;
                    case ConfigurationTangentLineDeterminationType.LayerSeparated:
                        return MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeLayerSeparated;
                    default:
                        throw new NotSupportedException();
                }
            }
            if (destinationType == typeof(MacroStabilityInwardsTangentLineDeterminationType))
            {
                var type = (ConfigurationTangentLineDeterminationType) value;
                switch (type)
                {
                    case ConfigurationTangentLineDeterminationType.Specified:
                        return MacroStabilityInwardsTangentLineDeterminationType.Specified;
                    case ConfigurationTangentLineDeterminationType.LayerSeparated:
                        return MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated;
                    default:
                        throw new NotSupportedException();
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}