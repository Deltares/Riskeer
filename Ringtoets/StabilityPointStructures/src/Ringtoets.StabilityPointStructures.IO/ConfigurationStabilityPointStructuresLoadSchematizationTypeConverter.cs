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
    /// Converts <see cref="ConfigurationStabilityPointStructuresLoadSchematizationType"/> to <see cref="string"/> 
    /// or <see cref="LoadSchematizationType"/> and back.
    /// </summary>
    public class ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(LoadSchematizationType))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var type = (ConfigurationStabilityPointStructuresLoadSchematizationType) value;
            if (destinationType == typeof(string))
            {
                return ConvertToString(type);
            }
            if (destinationType == typeof(LoadSchematizationType))
            {
                return ConvertToLoadSchematizationType(type);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            if (sourceType == typeof(LoadSchematizationType))
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
                return ConvertToConfigurationStabilityPointStructuresLoadSchematizationType(text);
            }
            var loadSchematizationType = value as LoadSchematizationType?;
            if (loadSchematizationType != null)
            {
                return ConvertToConfigurationStabilityPointStructuresLoadSchematizationType(loadSchematizationType.Value);
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts <paramref name="loadSchematizationType"/> to <see cref="ConfigurationStabilityPointStructuresLoadSchematizationType"/>.
        /// </summary>
        /// <param name="loadSchematizationType">The <see cref="LoadSchematizationType"/> to convert.</param>
        /// <returns>The converted <paramref name="loadSchematizationType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="loadSchematizationType"/> is not supported.</exception>
        private static ConfigurationStabilityPointStructuresLoadSchematizationType ConvertToConfigurationStabilityPointStructuresLoadSchematizationType(
            LoadSchematizationType loadSchematizationType)
        {
            switch (loadSchematizationType)
            {
                case LoadSchematizationType.Linear:
                    return ConfigurationStabilityPointStructuresLoadSchematizationType.Linear;
                case LoadSchematizationType.Quadratic:
                    return ConfigurationStabilityPointStructuresLoadSchematizationType.Quadratic;
                default:
                    throw new NotSupportedException($"Value '{loadSchematizationType}' is not supported.");
            }
        }

        /// <summary>
        /// Converts <paramref name="text"/> to <see cref="ConfigurationStabilityPointStructuresLoadSchematizationType"/>.
        /// </summary>
        /// <param name="text">The <see cref="string"/> to convert.</param>
        /// <returns>The converted <paramref name="text"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="text"/> is not supported.</exception>
        private static ConfigurationStabilityPointStructuresLoadSchematizationType ConvertToConfigurationStabilityPointStructuresLoadSchematizationType(
            string text)
        {
            switch (text)
            {
                case StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationLinearStructure:
                    return ConfigurationStabilityPointStructuresLoadSchematizationType.Linear;
                case StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationQuadraticStructure:
                    return ConfigurationStabilityPointStructuresLoadSchematizationType.Quadratic;
                default:
                    throw new NotSupportedException($"Value '{text}' is not supported.");
            }
        }

        /// <summary>
        /// Converts the given <paramref name="type"/> to a <see cref="LoadSchematizationType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ConfigurationStabilityPointStructuresLoadSchematizationType"/> to convert.</param>
        /// <returns>The converted <see cref="ConfigurationStabilityPointStructuresLoadSchematizationType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="type"/> is not supported.</exception>
        private static LoadSchematizationType ConvertToLoadSchematizationType(ConfigurationStabilityPointStructuresLoadSchematizationType type)
        {
            switch (type)
            {
                case ConfigurationStabilityPointStructuresLoadSchematizationType.Linear:
                    return LoadSchematizationType.Linear;
                case ConfigurationStabilityPointStructuresLoadSchematizationType.Quadratic:
                    return LoadSchematizationType.Quadratic;
                default:
                    throw new NotSupportedException($"Value '{type}' is not supported.");
            }
        }

        /// <summary>
        /// Converts the given <paramref name="type"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="type">The <see cref="ConfigurationStabilityPointStructuresLoadSchematizationType"/> to convert.</param>
        /// <returns>The converted <see cref="ConfigurationStabilityPointStructuresLoadSchematizationType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="type"/> is not supported.</exception>
        private static string ConvertToString(ConfigurationStabilityPointStructuresLoadSchematizationType type)
        {
            switch (type)
            {
                case ConfigurationStabilityPointStructuresLoadSchematizationType.Linear:
                    return StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationLinearStructure;
                case ConfigurationStabilityPointStructuresLoadSchematizationType.Quadratic:
                    return StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationQuadraticStructure;
                default:
                    throw new NotSupportedException($"Value '{type}' is not supported.");
            }
        }
    }
}