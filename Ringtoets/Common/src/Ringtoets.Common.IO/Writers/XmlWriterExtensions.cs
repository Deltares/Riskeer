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
using System.Xml;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Writers
{
    /// <summary>
    /// Extension methods for an <see cref="XmlWriter"/>, for writing generic data components in XML format
    /// to file.
    /// </summary>
    public static class XmlWriterExtensions
    {
        /// <summary>
        /// Writes a single <see cref="MeanStandardDeviationStochastConfiguration"/> as a stochast element in file.
        /// </summary>
        /// <param name="writer">The writer to use to write the distribution.</param>
        /// <param name="name">The name of the distribution to write.</param>
        /// <param name="distribution">The distribution to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is 
        /// in an invalid state for writing.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public static void WriteDistribution(this XmlWriter writer, string name, MeanStandardDeviationStochastConfiguration distribution)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, name);

            if (distribution.Mean.HasValue)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.MeanElement, XmlConvert.ToString(distribution.Mean.Value));
            }
            if (distribution.StandardDeviation.HasValue)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.StandardDeviationElement, XmlConvert.ToString(distribution.StandardDeviation.Value));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a single <see cref="MeanVariationCoefficientStochastConfiguration"/> as a stochast element in file.
        /// </summary>
        /// <param name="writer">The writer to use to write the distribution.</param>
        /// <param name="name">The name of the distribution to write.</param>
        /// <param name="distribution">The distribution to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is 
        /// in an invalid state for writing.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public static void WriteDistribution(this XmlWriter writer, string name, MeanVariationCoefficientStochastConfiguration distribution)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, name);

            if (distribution.Mean.HasValue)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.MeanElement, XmlConvert.ToString(distribution.Mean.Value));
            }
            if (distribution.VariationCoefficient.HasValue)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.VariationCoefficientElement, XmlConvert.ToString(distribution.VariationCoefficient.Value));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a single <see cref="WaveReductionConfiguration"/> as a wave reduction element in file.
        /// </summary>
        /// <param name="writer">The writer to use to write the wave reduction.</param>
        /// <param name="waveReduction">The wave reduction to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is 
        /// in an invalid state for writing.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public static void WriteWaveReduction(this XmlWriter writer, WaveReductionConfiguration waveReduction)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (waveReduction == null)
            {
                throw new ArgumentNullException(nameof(waveReduction));
            }

            writer.WriteStartElement(ConfigurationSchemaIdentifiers.WaveReduction);

            if (waveReduction.UseBreakWater.HasValue)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.UseBreakWater, XmlConvert.ToString(waveReduction.UseBreakWater.Value));
            }
            if (waveReduction.BreakWaterType.HasValue)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.BreakWaterType, new ConfigurationBreakWaterTypeConverter().ConvertToInvariantString(waveReduction.BreakWaterType.Value));
            }
            if (waveReduction.BreakWaterHeight.HasValue)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.BreakWaterHeight, XmlConvert.ToString(waveReduction.BreakWaterHeight.Value));
            }
            if (waveReduction.UseForeshoreProfile.HasValue)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.UseForeshore, XmlConvert.ToString(waveReduction.UseForeshoreProfile.Value));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the start tag of a folder element.
        /// </summary>
        /// <param name="writer">The writer to use to write the folder.</param>
        /// <param name="name">The name of the folder.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public static void WriteStartFolder(this XmlWriter writer, string name)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            writer.WriteStartElement(ConfigurationSchemaIdentifiers.FolderElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, name);
        }
    }
}