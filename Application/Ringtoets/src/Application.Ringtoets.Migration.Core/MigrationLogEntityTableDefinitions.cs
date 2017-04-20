﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Application.Ringtoets.Migration.Core
{
    /// <summary>
    /// Defines the table and column names of the table 'MigrationLogEntity' in the Ringtoets 
    /// migration log database.
    /// </summary>
    internal static class MigrationLogEntityTableDefinitions
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        internal const string TableName = "MigrationLogEntity";

        /// <summary>
        /// Gets the name of the 'migrated from version' column.
        /// </summary>
        internal const string FromVersion = "FromVersion";

        /// <summary>
        /// Gets the name of the 'migrated to version' column.
        /// </summary>
        internal const string ToVersion = "ToVersion";

        /// <summary>
        /// Gets the name of the 'message during migration' column.
        /// </summary>
        internal const string LogMessage = "LogMessage";
    }
}