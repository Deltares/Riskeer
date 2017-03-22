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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Writers
{
    /// <summary>
    /// Base implementation of writing calculation configurations to XML.
    /// </summary>
    /// <typeparam name="T">The type of calculations which are written to file.</typeparam>
    public abstract class CalculationConfigurationWriter<T> where T : class, ICalculation
    {
        /// <summary>
        /// Writes a calculation configuration to an XML file.
        /// </summary>
        /// <param name="configuration">The calculation configuration to write.</param>
        /// <param name="filePath">The path to the target XML file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write to <paramref name="filePath"/>.</exception>
        /// <remarks>The <paramref name="configuration"/> itself will not be part of the written XML, only its children.</remarks>
        public void Write(IEnumerable<ICalculationBase> configuration, string filePath)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
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
        /// Writes the <paramref name="distributions"/> in XML format to file.
        /// </summary>
        /// <param name="distributions">The dictionary of distributions, keyed on name, to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distributions"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is closed.</exception>
        protected static void WriteDistributions(IDictionary<string, IDistribution> distributions, XmlWriter writer)
        {
            if (distributions == null)
            {
                throw new ArgumentNullException(nameof(distributions));
            }

            if (distributions.Count > 0)
            {
                writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastsElement);

                foreach (KeyValuePair<string, IDistribution> keyValuePair in distributions)
                {
                    WriteDistribution(keyValuePair.Value, keyValuePair.Key, writer);
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes the <paramref name="variationCoefficientDistributions"/> in XML format to file.
        /// </summary>
        /// <param name="variationCoefficientDistributions">The dictionary of variation coefficient distributions, keyed on name, to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="variationCoefficientDistributions"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is closed.</exception>
        protected static void WriteVariationCoefficientDistributions(IDictionary<string, IVariationCoefficientDistribution> variationCoefficientDistributions, XmlWriter writer)
        {
            if (variationCoefficientDistributions == null)
            {
                throw new ArgumentNullException(nameof(variationCoefficientDistributions));
            }

            if (variationCoefficientDistributions.Count > 0)
            {
                writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastsElement);

                foreach (KeyValuePair<string, IVariationCoefficientDistribution> keyValuePair in variationCoefficientDistributions)
                {
                    WriteVariationCoefficientDistribution(keyValuePair.Value, keyValuePair.Key, writer);
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes the properties of the <paramref name="breakWater"/> in XML format to file.
        /// </summary>
        /// <param name="breakWater">The break water to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is closed.</exception>
        protected static void WriteBreakWaterProperties(BreakWater breakWater, XmlWriter writer)
        {
            if (breakWater != null)
            {
                writer.WriteElementString(
                    ConfigurationSchemaIdentifiers.BreakWaterType,
                    BreakWaterTypeAsXmlString((ReadBreakWaterType) breakWater.Type));
                writer.WriteElementString(
                    ConfigurationSchemaIdentifiers.BreakWaterHeight,
                    XmlConvert.ToString(breakWater.Height));
            }
        }

        /// <summary>
        /// Writes the <paramref name="configuration"/> in XML format to file.
        /// </summary>
        /// <param name="configuration">The calculation group(s) and/or calculation(s) to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> 
        /// contains a value that is neither <see cref="CalculationGroup"/> nor <see cref="T"/>.</exception>
        private void WriteConfiguration(IEnumerable<ICalculationBase> configuration, XmlWriter writer)
        {
            foreach (ICalculationBase child in configuration)
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

                if (innerGroup == null && calculation == null)
                {
                    throw new ArgumentException($"Cannot write calculation of type '{child.GetType()}' using this writer.");
                }
            }
        }

        private static string BreakWaterTypeAsXmlString(ReadBreakWaterType type)
        {
            return new ReadBreakWaterTypeConverter().ConvertToInvariantString(type);
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

        private static void WriteVariationCoefficientDistribution(IVariationCoefficientDistribution distribution, string elementName, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastElement);

            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, elementName);
            writer.WriteElementString(ConfigurationSchemaIdentifiers.MeanElement,
                                      XmlConvert.ToString(distribution.Mean));
            writer.WriteElementString(ConfigurationSchemaIdentifiers.VariationCoefficientElement,
                                      XmlConvert.ToString(distribution.CoefficientOfVariation));

            writer.WriteEndElement();
        }

        private void WriteCalculationGroup(CalculationGroup calculationGroup, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.FolderElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, calculationGroup.Name);

            WriteConfiguration(calculationGroup.Children, writer);

            writer.WriteEndElement();
        }
    }
}