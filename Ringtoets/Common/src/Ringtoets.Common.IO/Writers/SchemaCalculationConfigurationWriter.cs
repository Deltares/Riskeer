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
using System.Collections.Generic;
using System.Xml;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Properties;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Writers
{
    /// <summary>
    /// Base implementation of writing calculation configurations to XML.
    /// </summary>
    /// <typeparam name="T">The type of calculations which are written to file.</typeparam>
    public abstract class SchemaCalculationConfigurationWriter<T> where T : class, IConfigurationItem
    {
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="SchemaCalculationConfigurationWriter{T}"/>.
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
        protected SchemaCalculationConfigurationWriter(string filePath)
        {
            this.filePath = filePath;
            IOUtils.ValidateFilePath(filePath);
        }

        /// <summary>
        /// Writes a calculation configuration to an XML file.
        /// </summary>
        /// <param name="configuration">The calculation configuration to write.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write the file to the file path provided to
        /// the <see cref="SchemaCalculationConfigurationWriter{T}"/>.</exception>
        public void Write(IEnumerable<IConfigurationItem> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            try
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true
                };

                using (XmlWriter writer = XmlWriter.Create(filePath, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(ConfigurationSchemaIdentifiers.ConfigurationElement);

                    WriteConfiguration(configuration, writer);

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
        /// Writes a single <paramref name="calculation"/> in XML format to file.
        /// </summary>
        /// <param name="calculation">The calculation to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is closed.</exception>
        protected abstract void WriteCalculation(T calculation, XmlWriter writer);

        /// <summary>
        /// Writes the <paramref name="configuration"/> in XML format to file.
        /// </summary>
        /// <param name="configuration">The calculation group(s) and/or calculation(s) to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> 
        /// contains a value that is neither <see cref="CalculationConfigurationGroup"/> nor <see cref="T"/>.</exception>
        private void WriteConfiguration(IEnumerable<IConfigurationItem> configuration, XmlWriter writer)
        {
            foreach (IConfigurationItem child in configuration)
            {
                var innerGroup = child as CalculationConfigurationGroup;
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

        private void WriteCalculationGroup(CalculationConfigurationGroup group, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.FolderElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, group.Name);

            WriteConfiguration(group.Items, writer);

            writer.WriteEndElement();
        }
    }
}