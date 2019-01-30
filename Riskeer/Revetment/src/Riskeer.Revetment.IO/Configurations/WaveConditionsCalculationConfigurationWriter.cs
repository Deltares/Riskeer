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
using System.Xml;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Export;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.Configurations.Converters;

namespace Riskeer.Revetment.IO.Configurations
{
    /// <summary>
    /// Base implementation of a writer for calculations that contain <see cref="WaveConditionsInput"/> as input,
    /// to XML format.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    public abstract class WaveConditionsCalculationConfigurationWriter<T> : CalculationConfigurationWriter<T>
        where T : WaveConditionsCalculationConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationWriter{T}"/>.
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
        protected WaveConditionsCalculationConfigurationWriter(string filePath)
            : base(filePath) {}

        protected override void WriteCalculation(T configuration, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, configuration.Name);

            WriteElementWhenContentAvailable(
                writer,
                ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElementNew,
                configuration.HydraulicBoundaryLocationName);

            WriteConfigurationCategoryTypeWhenAvailable(writer, configuration);

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
            WriteConfigurationWaveConditionsInputStepSizeWhenAvailable(
                writer,
                configuration.StepSize);

            WriteElementWhenContentAvailable(
                writer,
                WaveConditionsCalculationConfigurationSchemaIdentifiers.ForeshoreProfile,
                configuration.ForeshoreProfileId);

            WriteElementWhenContentAvailable(
                writer,
                ConfigurationSchemaIdentifiers.Orientation,
                configuration.Orientation);

            WriteWaveReductionWhenAvailable(writer, configuration.WaveReduction);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the category type in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration to get the category type from.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of the category type 
        /// cannot be performed.</exception>
        protected abstract void WriteConfigurationCategoryTypeWhenAvailable(XmlWriter writer, T configuration);

        /// <summary>
        /// Writes the <paramref name="stepSize"/> in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="stepSize">The stepsize to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion
        /// of <paramref name="stepSize"/> cannot be performed.</exception>
        private static void WriteConfigurationWaveConditionsInputStepSizeWhenAvailable(
            XmlWriter writer,
            ConfigurationWaveConditionsInputStepSize? stepSize)
        {
            if (!stepSize.HasValue)
            {
                return;
            }

            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();
            writer.WriteElementString(WaveConditionsCalculationConfigurationSchemaIdentifiers.StepSize,
                                      converter.ConvertToInvariantString(stepSize.Value));
        }
    }
}