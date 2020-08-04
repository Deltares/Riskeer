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

using System.Collections.Generic;

namespace Riskeer.Common.IO.Configurations.Import
{
    /// <summary>
    /// Class that represents calculation configuration schema definitions.
    /// </summary>
    public class CalculationConfigurationSchemaDefinition
    {
        /// <summary>
        /// Creates a new instance of <see cref="CalculationConfigurationSchemaDefinition"/>.
        /// </summary>
        /// <param name="mainSchemaDefinition">The main schema definition xsd.</param>
        /// <param name="nestedSchemaDefinitions">The nested schema definition xsd.</param>
        /// <param name="versionNumber">The version number of the xml that is read.</param>
        public CalculationConfigurationSchemaDefinition(string mainSchemaDefinition, IDictionary<string, string> nestedSchemaDefinitions, int versionNumber = 0)
        {
            MainSchemaDefinition = mainSchemaDefinition;
            NestedSchemaDefinitions = nestedSchemaDefinitions;
            VersionNumber = versionNumber;
        }

        /// <summary>
        /// Gets or sets the version schema definition.
        /// </summary>
        public int VersionNumber { get; private set; }

        /// <summary>
        /// Gets or sets the main schema definition.
        /// </summary>
        public string MainSchemaDefinition { get; private set; }

        /// <summary>
        /// Gets or sets the nested schema definitions.
        /// </summary>
        public IDictionary<string, string> NestedSchemaDefinitions { get; private set; }
    }
}