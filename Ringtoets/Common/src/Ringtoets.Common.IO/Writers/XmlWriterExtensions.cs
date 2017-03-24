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
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Writers
{
    public static class XmlWriterExtensions
    {
        /// <summary>
        /// Writes a single distribution as a stochast element in file.
        /// </summary>
        /// <param name="writer">The writer to use to write the distribution.</param>
        /// <param name="name"></param>
        /// <param name="distribution">The distribution to write.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="writer"/> is 
        /// in an invalid state for writing.</exception>
        public static void WriteDistribution(this XmlWriter writer, string name, MeanStandardDeviationStochastConfiguration distribution)
        {
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

        public static void WriteDistribution(this XmlWriter writer, string name, MeanVariationCoefficientStochastConfiguration distribution)
        {
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
    }
}