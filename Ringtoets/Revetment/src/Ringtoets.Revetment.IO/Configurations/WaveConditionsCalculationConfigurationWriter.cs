﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Xml;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO.Configurations.Helpers;

namespace Ringtoets.Revetment.IO.Configurations
{
    /// <summary>
    /// Base implementation of a writer for calculations that contain <see cref="WaveConditionsInput"/> as input,
    /// to XML format.
    /// </summary>
    /// <typeparam name="T">The type of calculations that are written to file.</typeparam>
    public class WaveConditionsCalculationConfigurationWriter : SchemaCalculationConfigurationWriter<WaveConditionsCalculationConfiguration>
    {
        private readonly ConfigurationWaveConditionsInputStepSizeConverter configurationWaveConditionsInputStepSizeConverter;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationWriter"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to write to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public WaveConditionsCalculationConfigurationWriter(string filePath) : base(filePath)
        {
            configurationWaveConditionsInputStepSizeConverter = new ConfigurationWaveConditionsInputStepSizeConverter();
        }

        /// <summary>
        /// Writes a single calculation with its <paramref name="configuration"/> in XML format to file.
        /// </summary>
        /// <param name="configuration">The input of the calculation to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        protected override void WriteCalculation(WaveConditionsCalculationConfiguration configuration, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, configuration.Name);

            WriteElementWhenContentAvailable(
                writer,
                ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                configuration.HydraulicBoundaryLocation);

            WriteElementWhenContentAvailable(
                writer,
                WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryRevetment,
                configuration.UpperBoundaryRevetment);
            WriteElementWhenContentAvailable(
                writer,
                WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryRevetment,
                configuration.LowerBoundaryRevetment);
            WriteElementWhenContentAvailable(
                writer,
                WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryWaterLevels,
                configuration.UpperBoundaryWaterLevels);
            WriteElementWhenContentAvailable(
                writer,
                WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryWaterLevels,
                configuration.LowerBoundaryWaterLevels);
            WriteConfigurationLoadSchematizationTypeWhenAvailable(
                writer,
                configuration.StepSize);

            WriteElementWhenContentAvailable(
                writer,
                WaveConditionsCalculationConfigurationSchemaIdentifiers.ForeshoreProfile,
                configuration.ForeshoreProfile);

            WriteElementWhenContentAvailable(
                writer,
                ConfigurationSchemaIdentifiers.Orientation,
                configuration.Orientation);

            WriteWaveReductionWhenAvailable(writer, configuration.WaveReduction);

            writer.WriteEndElement();
        }

        private static void WriteConfigurationLoadSchematizationTypeWhenAvailable(
            XmlWriter writer,
            ConfigurationWaveConditionsInputStepSize? configuration)
        {
            if (!configuration.HasValue)
            {
                return;
            }

            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();
            writer.WriteElementString(WaveConditionsCalculationConfigurationSchemaIdentifiers.StepSize,
                                      converter.ConvertToInvariantString(configuration.Value));
        }
    }
}