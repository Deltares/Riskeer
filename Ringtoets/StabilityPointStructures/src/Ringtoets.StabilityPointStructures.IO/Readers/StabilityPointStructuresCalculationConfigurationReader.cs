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
using System.Xml.Linq;
using Core.Common.Base.IO;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;
using Ringtoets.StabilityPointStructures.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.IO.Readers
{
    /// <summary>
    /// Reader for reading a stability point structures calculation configuration from XML and creating a collection 
    /// of corresponding <see cref="StabilityPointStructuresCalculationConfiguration"/>.
    /// </summary>
    public class StabilityPointStructuresCalculationConfigurationReader : CalculationConfigurationReader<StabilityPointStructuresCalculationConfiguration>
    {
        private const string hrLocatieSchemaName = "HrLocatieSchema.xsd";
        private const string orientatieSchemaName = "OrientatieSchema.xsd";
        private const string golfReductieSchemaName = "GolfReductieSchema.xsd";
        private const string voorlandProfielSchemaName = "VoorlandProfielSchema.xsd";
        private const string stochastSchemaName = "StochastSchema.xsd";

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationConfigurationReader"/>.
        /// </summary>
        /// <param name="filePath">The file path to the XML file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="filePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="filePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="filePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="filePath"/> points to a file that does not contain configuration elements.</item>
        /// </list>
        /// </exception>
        public StabilityPointStructuresCalculationConfigurationReader(string filePath)
            : base(filePath,
                   Resources.StabilityPointStructuresConfigurationSchema,
                   new Dictionary<string, string>
                   {
                       {
                           hrLocatieSchemaName, RingtoetsCommonIOResources.HrLocatieSchema
                       },
                       {
                           orientatieSchemaName, RingtoetsCommonIOResources.OrientatieSchema
                       },
                       {
                           voorlandProfielSchemaName, RingtoetsCommonIOResources.VoorlandProfielSchema
                       },
                       {
                           golfReductieSchemaName, RingtoetsCommonIOResources.GolfReductieSchema
                       },
                       {
                           stochastSchemaName, RingtoetsCommonIOResources.StochastSchema
                       }
                   }
            ) { }

        protected override StabilityPointStructuresCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            return new StabilityPointStructuresCalculationConfiguration(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value);
        }
    }
}