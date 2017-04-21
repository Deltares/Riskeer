﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;

namespace Ringtoets.ClosingStructures.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationClosingStructureInflowModelType"/> to <see cref="string"/> 
    /// or <see cref="ClosingStructureInflowModelType"/> and back.
    /// </summary>
    public class ConfigurationClosingStructureInflowModelTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(ClosingStructureInflowModelType))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var type = (ConfigurationClosingStructureInflowModelType) value;
                switch (type)
                {
                    case ConfigurationClosingStructureInflowModelType.FloodedCulvert:
                        return ClosingStructuresConfigurationSchemaIdentifiers.FloodedCulvert;
                    case ConfigurationClosingStructureInflowModelType.LowSill:
                        return ClosingStructuresConfigurationSchemaIdentifiers.LowSill;
                    case ConfigurationClosingStructureInflowModelType.VerticalWall:
                        return ClosingStructuresConfigurationSchemaIdentifiers.VerticalWall;
                    default:
                        throw new NotSupportedException();
                }
            }
            if (destinationType == typeof(ClosingStructureInflowModelType))
            {
                var type = (ConfigurationClosingStructureInflowModelType) value;
                switch (type)
                {
                    case ConfigurationClosingStructureInflowModelType.FloodedCulvert:
                        return ClosingStructureInflowModelType.FloodedCulvert;
                    case ConfigurationClosingStructureInflowModelType.LowSill:
                        return ClosingStructureInflowModelType.LowSill;
                    case ConfigurationClosingStructureInflowModelType.VerticalWall:
                        return ClosingStructureInflowModelType.VerticalWall;
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
            if (sourceType == typeof(ClosingStructureInflowModelType))
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
                switch (text)
                {
                    case ClosingStructuresConfigurationSchemaIdentifiers.FloodedCulvert:
                        return ConfigurationClosingStructureInflowModelType.FloodedCulvert;
                    case ClosingStructuresConfigurationSchemaIdentifiers.LowSill:
                        return ConfigurationClosingStructureInflowModelType.LowSill;
                    case ClosingStructuresConfigurationSchemaIdentifiers.VerticalWall:
                        return ConfigurationClosingStructureInflowModelType.VerticalWall;
                }
            }
            var inflowModelType = value as ClosingStructureInflowModelType?;
            if (inflowModelType != null)
            {
                switch (inflowModelType)
                {
                    case ClosingStructureInflowModelType.FloodedCulvert:
                        return ConfigurationClosingStructureInflowModelType.FloodedCulvert;
                    case ClosingStructureInflowModelType.LowSill:
                        return ConfigurationClosingStructureInflowModelType.LowSill;
                    case ClosingStructureInflowModelType.VerticalWall:
                        return ConfigurationClosingStructureInflowModelType.VerticalWall;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}