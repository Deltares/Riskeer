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
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.HeightStructures.IO
{
    /// <summary>
    /// Extension methods for an <see cref="XmlWriter"/>, for writing <see cref="HeightStructureCalculationConfiguration"/>
    /// in XML format to file.
    /// </summary>
    public static class HeightStructuresCalculationConfigurationXmlWriterExtensions
    {
        /// <summary>
        /// Writes the <paramref name="configuration"/> in XML format to file.
        /// </summary>
        /// <param name="writer">The writer to use for writing.</param>
        /// <param name="configuration">The dictionary of distributions, keyed on name, to write.</param>
        public static void WriteHeightStructure(this XmlWriter writer, HeightStructureCalculationConfiguration configuration)
        {
            writer.WriteStructure(configuration, WriteProperties, WriteStochasts);
        }

        private static void WriteStochasts(HeightStructureCalculationConfiguration configuration, XmlWriter writer)
        {
            if (configuration.LevelCrestStructure != null)
            {
                writer.WriteDistribution(HeightStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName, configuration.LevelCrestStructure);
            }
        }

        private static void WriteProperties(HeightStructureCalculationConfiguration configuration, XmlWriter writer) {}
    }
}