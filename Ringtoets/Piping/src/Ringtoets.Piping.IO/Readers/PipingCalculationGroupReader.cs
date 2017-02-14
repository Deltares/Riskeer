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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Core.Common.Utils.Reflection;
using Ringtoets.Piping.IO.Properties;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Readers
{
    /// <summary>
    /// This class reads a piping configuration from XML and creates a collection of corresponding <see cref="IReadPipingCalculationItem"/>.
    /// </summary>
    public class PipingCalculationGroupReader
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationGroupReader"/>.
        /// </summary>
        /// <param name="filePath">The file path to the XML file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="filePath"/> points to a file that does not exist.</exception>
        public PipingCalculationGroupReader(string filePath)
        {
            IOUtils.ValidateFilePath(filePath);
            if (!File.Exists(filePath))
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }
        }

        /// <summary>
        /// Reads a piping configuration from XML and creates a collection of corresponding <see cref="IReadPipingCalculationItem"/>.
        /// </summary>
        /// <returns>A collection of read <see cref="IReadPipingCalculationItem"/>.</returns>
        /// <exception cref="PipingConfigurationConversionException">Thrown when the schema validation failed.</exception>
        public IEnumerable<IReadPipingCalculationItem> Read()
        {
            return Enumerable.Empty<IReadPipingCalculationItem>();
        }

        private XmlSchemaSet LoadXmlSchema()
        {
            var schemaFile = AssemblyUtils.GetAssemblyResourceStream(GetType().Assembly,
                                                                     "Ringtoets.Piping.IO.Readers.XMLPipingConfigurationSchema.xsd");

            var xmlSchema = new XmlSchemaSet();
            xmlSchema.Add(XmlSchema.Read(schemaFile, null));

            return xmlSchema;
        }

        private void ValidateToSchema(XDocument document)
        {
            XmlSchemaSet schema = LoadXmlSchema();

            try
            {
                document.Validate(schema, null);
            }
            catch (XmlSchemaValidationException e)
            {
                throw new PipingConfigurationConversionException(Resources.PipingCalculationGroupReader_Configuration_contains_no_valid_xml, e);
            }
        }
    }
}