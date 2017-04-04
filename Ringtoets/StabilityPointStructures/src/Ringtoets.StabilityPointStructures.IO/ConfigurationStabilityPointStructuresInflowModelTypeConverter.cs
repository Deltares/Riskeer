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
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.IO
{
    /// <summary>
    /// Converts <see cref="ConfigurationStabilityPointStructuresInflowModelType"/> to <see cref="string"/> 
    /// or <see cref="StabilityPointStructureInflowModelType"/> and back.
    /// </summary>
    public class ConfigurationStabilityPointStructuresInflowModelTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(StabilityPointStructureInflowModelType))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var type = (ConfigurationStabilityPointStructuresInflowModelType) value;
            if (destinationType == typeof(string))
            {
                return ConvertToString(type);
            }
            if (destinationType == typeof(StabilityPointStructureInflowModelType))
            {
                return ConvertToStabilityPointStructureInflowModelType(type);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            if (sourceType == typeof(StabilityPointStructureInflowModelType))
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
                return ConvertToConfigurationInflowModelType(text);
            }
            var inflowModelType = value as StabilityPointStructureInflowModelType?;
            if (inflowModelType != null)
            {
                return ConvertToConfigurationInflowModelType(inflowModelType.Value);
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given <paramref name="type"/> to a <see cref="StabilityPointStructureInflowModelType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ConfigurationStabilityPointStructuresInflowModelType"/> to convert.</param>
        /// <returns>The converted <see cref="ConfigurationStabilityPointStructuresInflowModelType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="type"/> is not supported.</exception>
        private static StabilityPointStructureInflowModelType ConvertToStabilityPointStructureInflowModelType(ConfigurationStabilityPointStructuresInflowModelType type)
        {
            switch (type)
            {
                case ConfigurationStabilityPointStructuresInflowModelType.LowSill:
                    return StabilityPointStructureInflowModelType.LowSill;
                case ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert:
                    return StabilityPointStructureInflowModelType.FloodedCulvert;
                default:
                    throw new NotSupportedException($"Value '{type}' is not supported.");
            }
        }

        /// <summary>
        /// Converts the given <paramref name="type"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="type">The <see cref="ConfigurationStabilityPointStructuresInflowModelType"/> to convert.</param>
        /// <returns>The converted <see cref="ConfigurationStabilityPointStructuresInflowModelType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="type"/> is not supported.</exception>
        private static string ConvertToString(ConfigurationStabilityPointStructuresInflowModelType type)
        {
            switch (type)
            {
                case ConfigurationStabilityPointStructuresInflowModelType.LowSill:
                    return StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelLowSillStructure;
                case ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert:
                    return StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelFloodedCulvertStructure;
                default:
                    throw new NotSupportedException($"Value '{type}' is not supported.");
            }
        }

        /// <summary>
        /// Converts <paramref name="text"/> to <see cref="ConfigurationStabilityPointStructuresInflowModelType"/>.
        /// </summary>
        /// <param name="text">The <see cref="string"/> to convert.</param>
        /// <returns>The converted <paramref name="text"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="text"/> is not supported.</exception>
        private static ConfigurationStabilityPointStructuresInflowModelType ConvertToConfigurationInflowModelType(string text)
        {
            switch (text)
            {
                case StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelLowSillStructure:
                    return ConfigurationStabilityPointStructuresInflowModelType.LowSill;
                case StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelFloodedCulvertStructure:
                    return ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert;
                default:
                    throw new NotSupportedException($"Value '{text}' is not supported.");
            }
        }

        /// <summary>
        /// Converts <paramref name="inflowModelType"/> to <see cref="ConfigurationStabilityPointStructuresInflowModelType"/>.
        /// </summary>
        /// <param name="inflowModelType">The <see cref="StabilityPointStructureInflowModelType"/> to convert.</param>
        /// <returns>The converted <paramref name="inflowModelType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="inflowModelType"/> is not supported.</exception>
        private static ConfigurationStabilityPointStructuresInflowModelType ConvertToConfigurationInflowModelType(StabilityPointStructureInflowModelType inflowModelType)
        {
            switch (inflowModelType)
            {
                case StabilityPointStructureInflowModelType.LowSill:
                    return ConfigurationStabilityPointStructuresInflowModelType.LowSill;
                case StabilityPointStructureInflowModelType.FloodedCulvert:
                    return ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert;
                default:
                    throw new NotSupportedException($"Value '{inflowModelType}' is not supported.");
            }
        }
    }
}