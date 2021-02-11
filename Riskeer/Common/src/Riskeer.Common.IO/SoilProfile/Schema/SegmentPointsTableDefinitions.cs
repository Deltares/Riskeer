﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    /// Defines the table and column names of the table 'SegmentPoints' of the D-Soil Model database.
    /// </summary>
    internal static class SegmentPointsTableDefinitions
    {
        /// <summary>
        /// The name of the table.
        /// </summary>
        public const string TableName = "SegmentPoints";

        /// <summary>
        /// The name of the world x coordinate column.
        /// </summary>
        public const string CoordinateX = "XWorld";

        /// <summary>
        /// The name of the world Y coordinate column.
        /// </summary>
        public const string CoordinateY = "YWorld";
    }
}