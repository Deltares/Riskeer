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

namespace Ringtoets.Common.IO.Schema
{
    /// <summary>
    /// Container of general identifiers related to configuration schema definitions.
    /// </summary>
    public static class ConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The identifier for configuration elements.
        /// </summary>
        public const string ConfigurationElement = "configuratie";

        /// <summary>
        /// The identifier for calculation elements.
        /// </summary>
        public const string CalculationElement = "berekening";

        /// <summary>
        /// The identifier for folder elements.
        /// </summary>
        public const string FolderElement = "map";

        /// <summary>
        /// The identifier for name attributes.
        /// </summary>
        public const string NameAttribute = "naam";

        /// <summary>
        /// The identifier for hydraulic boundary location elements.
        /// </summary>
        public const string HydraulicBoundaryLocationElement = "hrlocatie";

        /// <summary>
        /// The identifier for stochasts elements.
        /// </summary>
        public const string StochastsElement = "stochasten";

        /// <summary>
        /// The identifier for stochast elements.
        /// </summary>
        public const string StochastElement = "stochast";

        /// <summary>
        /// The identifier for mean elements.
        /// </summary>
        public const string MeanElement = "verwachtingswaarde";

        /// <summary>
        /// The identifier for standard deviation elements.
        /// </summary>
        public const string StandardDeviationElement = "standaardafwijking";
    }
}