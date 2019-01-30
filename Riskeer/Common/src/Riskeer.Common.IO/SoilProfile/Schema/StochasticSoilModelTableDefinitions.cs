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

namespace Riskeer.Common.IO.SoilProfile.Schema
{
    /// <summary>
    /// Defines the table and column names of the table 'StochasticSoilModel' in the D-Soil Model database.
    /// </summary>
    internal static class StochasticSoilModelTableDefinitions
    {
        /// <summary>
        /// The name of the table.
        /// </summary>
        public const string TableName = "StochasticSoilModel";

        /// <summary>
        /// The name of the stochastic soil model id column.
        /// </summary>
        public const string StochasticSoilModelId = "SSM_ID";

        /// <summary>
        /// The name of the stochastic soil model name column.
        /// </summary>
        public const string StochasticSoilModelName = "SSM_Name";

        /// <summary>
        /// The name of the alias used for identifying if segments are unique.
        /// </summary>
        public const string AreSegmentsUnique = "AreSegmentsUnique";

        /// <summary>
        /// The name of the alias used for the number of rows that can be read.
        /// </summary>
        public const string Count = "nrOfRows";
    }
}