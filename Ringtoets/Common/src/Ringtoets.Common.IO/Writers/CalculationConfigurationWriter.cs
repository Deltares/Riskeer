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
using Core.Common.Utils.Properties;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Writers
{
    /// <summary>
    /// Base implementation of a writer for calculations to XML format.
    /// </summary>
    /// <typeparam name="T">The type of calculations which are written to file.</typeparam>
    public abstract class CalculationConfigurationWriter<T> where T : class, ICalculation
    {
        /// <summary>
        /// Writes a single <paramref name="calculation"/> in XML format to file.
        /// </summary>
        /// <param name="calculation">The calculation to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is closed.</exception>
        protected abstract void WriteCalculation(T calculation, XmlWriter writer);

        /// <summary>
        /// Writes the <paramref name="distributions"/> in XML format to file.
        /// </summary>
        /// <param name="distributions">The distributions, as tuples of name and distribution, to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is closed.</exception>
        protected static void WriteDistributions(IEnumerable<Tuple<string, IDistribution>> distributions, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastsElement);

            foreach (Tuple<string, IDistribution> distribution in distributions)
            {
                WriteDistribution(distribution.Item2, distribution.Item1, writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a piping configuration to an XML file.
        /// </summary>
        /// <param name="rootCalculationGroup">The root calculation group containing the piping configuration to write.</param>
        /// <param name="filePath">The path to the target XML file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write to <paramref name="filePath"/>.</exception>
        /// <remarks>The <paramref name="rootCalculationGroup"/> itself will not be part of the written XML, only its children.</remarks>
        public void Write(CalculationGroup rootCalculationGroup, string filePath)
        {
            if (rootCalculationGroup == null)
            {
                throw new ArgumentNullException(nameof(rootCalculationGroup));
            }
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
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

                    WriteConfiguration(rootCalculationGroup, writer);

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(Resources.Error_General_output_error_0, filePath), e);
            }
        }

        private void WriteConfiguration(CalculationGroup calculationGroup, XmlWriter writer)
        {
            foreach (ICalculationBase child in calculationGroup.Children)
            {
                var innerGroup = child as CalculationGroup;
                if (innerGroup != null)
                {
                    WriteCalculationGroup(innerGroup, writer);
                }

                var calculation = child as T;
                if (calculation != null)
                {
                    WriteCalculation(calculation, writer);
                }
            }
        }

        private static void WriteDistribution(IDistribution distribution, string elementName, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastElement);

            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, elementName);
            writer.WriteElementString(ConfigurationSchemaIdentifiers.MeanElement,
                                      XmlConvert.ToString(distribution.Mean));
            writer.WriteElementString(ConfigurationSchemaIdentifiers.StandardDeviationElement,
                                      XmlConvert.ToString(distribution.StandardDeviation));

            writer.WriteEndElement();
        }

        private void WriteCalculationGroup(CalculationGroup calculationGroup, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.FolderElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, calculationGroup.Name);

            WriteConfiguration(calculationGroup, writer);

            writer.WriteEndElement();
        }
    }
}