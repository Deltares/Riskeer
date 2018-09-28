// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Ringtoets.Common.IO.SoilProfile.Schema
{
    /// <summary>
    /// Defines the table and column names of the table 'StochasticSoilProfile' in the D-Soil Model database.
    /// </summary>
    internal static class StochasticSoilProfileTableDefinitions
    {
        /// <summary>
        /// The name of the table.
        /// </summary>
        public const string TableName = "StochasticSoilProfile";

        /// <summary>
        /// The name of the stochastic soil profile 1D id column.
        /// </summary>
        public const string SoilProfile1DId = "SP1D_ID";

        /// <summary>
        /// The name of the stochastic soil profile 2D id column.
        /// </summary>
        public const string SoilProfile2DId = "SP2D_ID";

        /// <summary>
        /// The name of the stochastic soil profile probability column.
        /// </summary>
        public const string Probability = "Probability";

        /// <summary>
        /// The name of the alias used for identifying if the database consists only of 
        /// valid probabilities.
        /// </summary>
        public const string AllProbabilitiesValid = "AllProbabilitiesValid";
    }
}