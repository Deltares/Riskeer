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

namespace Riskeer.Common.IO.SoilProfile.Schema
{
    /// <summary>
    /// Defines the column and table names of the table 'PreconsolidationStresses' in 
    /// the D-Soil Model database.
    /// </summary>
    internal static class PreconsolidationStressTableDefinitions
    {
        /// <summary>
        /// The name of the table.
        /// </summary>
        public const string TableName = "PreconsolidationStresses";

        /// <summary>
        /// The name of the X coordinate column. 
        /// </summary>
        public const string PreconsolidationStressXCoordinate = "PreconsolidationStressXCoordinate";

        /// <summary>
        /// The name of the Z coordinate column. 
        /// </summary>
        public const string PreconsolidationStressZCoordinate = "PreconsolidationStressZCoordinate";

        /// <summary>
        /// The name of the preconsolidation stress distribution column.
        /// </summary>
        public const string PreconsolidationStressDistributionType = "PreconsolidationStressDistributionType";

        /// <summary>
        /// The name of the preconsolidation stress mean column.
        /// </summary>
        public const string PreconsolidationStressMean = "PreconsolidationStressMean";

        /// <summary>
        /// The name of the preconsolidation stress coefficient of variation column.
        /// </summary>
        public const string PreconsolidationStressCoefficientOfVariation = "PreconsolidationStressCoefficientOfVariation";

        /// <summary>
        /// The name of the preconsolidation stress shift column.
        /// </summary>
        public const string PreconsolidationStressShift = "PreconsolidationStressShift";
    }
}