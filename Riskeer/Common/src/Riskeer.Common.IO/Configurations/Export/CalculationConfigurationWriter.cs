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
using System.Collections.Generic;
using System.Xml;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using Core.Common.Util.Properties;
using Riskeer.Common.IO.Configurations.Helpers;

namespace Riskeer.Common.IO.Configurations.Export
{
    /// <summary>
    /// Base implementation of writing calculation configurations to XML.
    /// </summary>
    /// <typeparam name="T">The type of calculations which are written to file.</typeparam>
    public abstract class CalculationConfigurationWriter<T> where T : class, IConfigurationItem
    {
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="CalculationConfigurationWriter{T}"/>.
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
        protected CalculationConfigurationWriter(string filePath)
        {
            this.filePath = filePath;
            IOUtils.ValidateFilePath(filePath);
        }

        /// <summary>
        /// Writes calculation configurations to an XML file.
        /// </summary>
        /// <param name="configurations">The calculation configuration to write.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write the file to the provided file path.</exception>
        public void Write(IEnumerable<IConfigurationItem> configurations)
        {
            if (configurations == null)
            {
                throw new ArgumentNullException(nameof(configurations));
            }

            try
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true
                };

                using (var writer = XmlWriter.Create(filePath, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(ConfigurationSchemaIdentifiers.ConfigurationElement);
                    writer.WriteAttributeString(ConfigurationSchemaIdentifiers.VersionAttribute, GetConfigurationVersion().ToString());

                    WriteConfiguration(configurations, writer);

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(Resources.Error_General_output_error_0, filePath), e);
            }
        }

        /// <summary>
        /// Gets the version of the configuration to write.
        /// </summary>
        /// <returns>The version.</returns>
        protected abstract int GetConfigurationVersion();

        /// <summary>
        /// Writes a single <paramref name="configuration"/> in XML format to file.
        /// </summary>
        /// <param name="configuration">The calculation configuration to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of 
        /// <paramref name="configuration"/> cannot be performed.</exception>
        protected abstract void WriteCalculation(T configuration, XmlWriter writer);

        /// <summary>
        /// Writes a distribution configuration when it has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="distributionName">The name of the distribution.</param>
        /// <param name="configuration">The configuration for the distribution that can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> or <paramref name="distributionName"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        protected static void WriteDistributionWhenAvailable(XmlWriter writer, string distributionName, StochastConfiguration configuration)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (distributionName == null)
            {
                throw new ArgumentNullException(nameof(distributionName));
            }

            if (configuration != null)
            {
                writer.WriteDistribution(distributionName, configuration);
            }
        }

        /// <summary>
        /// Writes an element with some content when the content has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="elementContent">The content of the element that can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> or <paramref name="elementName"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        protected static void WriteElementWhenContentAvailable(XmlWriter writer, string elementName, string elementContent)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (elementName == null)
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            if (elementContent != null)
            {
                writer.WriteElementString(
                    elementName,
                    elementContent);
            }
        }

        /// <summary>
        /// Writes an element with some content when the content has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="elementContent">The content of the element that can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> or <paramref name="elementName"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        protected static void WriteElementWhenContentAvailable(XmlWriter writer, string elementName, double? elementContent)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (elementName == null)
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            if (elementContent.HasValue)
            {
                writer.WriteElementString(
                    elementName,
                    XmlConvert.ToString(elementContent.Value));
            }
        }

        /// <summary>
        /// Writes an element with some content when the content has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="elementContent">The content of the element that can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> or <paramref name="elementName"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        protected static void WriteElementWhenContentAvailable(XmlWriter writer, string elementName, bool? elementContent)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (elementName == null)
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            if (elementContent.HasValue)
            {
                writer.WriteElementString(
                    elementName,
                    XmlConvert.ToString(elementContent.Value));
            }
        }

        /// <summary>
        /// Writes a wave reduction configuration when it has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration for the wave reduction that can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        protected static void WriteWaveReductionWhenAvailable(XmlWriter writer, WaveReductionConfiguration configuration)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (configuration != null)
            {
                writer.WriteWaveReduction(configuration);
            }
        }

        /// <summary>
        /// Writes a scenario configuration when it has a value.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The configuration for the scenario that can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        protected static void WriteScenarioWhenAvailable(XmlWriter writer, ScenarioConfiguration configuration)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (configuration != null)
            {
                writer.WriteScenario(configuration);
            }
        }

        /// <summary>
        /// Writes the <paramref name="configurations"/> in XML format to file.
        /// </summary>
        /// <param name="configurations">The calculation group(s) and/or calculation(s) to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="configurations"/> 
        /// contains a value that is neither <see cref="CalculationGroupConfiguration"/> nor <see cref="T"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of an element in 
        /// <paramref name="configurations"/> cannot be performed.</exception>
        private void WriteConfiguration(IEnumerable<IConfigurationItem> configurations, XmlWriter writer)
        {
            foreach (IConfigurationItem child in configurations)
            {
                var innerGroup = child as CalculationGroupConfiguration;
                if (innerGroup != null)
                {
                    WriteCalculationGroup(innerGroup, writer);
                }

                var calculation = child as T;
                if (calculation != null)
                {
                    WriteCalculation(calculation, writer);
                }

                if (innerGroup == null && calculation == null)
                {
                    throw new ArgumentException($"Cannot write calculation of type '{child.GetType()}' using this writer.");
                }
            }
        }

        /// <summary>
        /// Writes the <paramref name="group"/> in XML format to file.
        /// </summary>
        /// <param name="group">The calculation group to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="group"/> 
        /// contains a child that is neither <see cref="CalculationGroupConfiguration"/> nor 
        /// <see cref="T"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> 
        /// is closed.</exception>
        /// <exception cref="NotSupportedException">Thrown when the conversion of an element in 
        /// <paramref name="group"/> cannot be performed.</exception>
        private void WriteCalculationGroup(CalculationGroupConfiguration group, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.FolderElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, group.Name);

            WriteConfiguration(group.Items, writer);

            writer.WriteEndElement();
        }
    }
}