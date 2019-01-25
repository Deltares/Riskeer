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

using System;
using System.Data.SQLite;
using Migration.Scripts.Data;
using Migration.Scripts.Data.Exceptions;
using Ringtoets.Common.Util;
using Riskeer.Migration.Core.Properties;

namespace Riskeer.Migration.Core
{
    /// <summary>
    /// Class that provides methods for the upgrading a <see cref="RingtoetsVersionedFile"/> for a specific version.
    /// </summary>
    public class RingtoetsUpgradeScript : UpgradeScript
    {
        private readonly string upgradeQuery;
        private readonly string logDatabaseLocation;

        /// <summary>
        /// Creates a new instance of the <see cref="RingtoetsUpgradeScript"/> class.
        /// </summary>
        /// <param name="fromVersion">The source version <paramref name="query"/> was designed for.</param>
        /// <param name="toVersion">The target version <paramref name="query"/> was designed for.</param>
        /// <param name="query">The SQL query to upgrade from <paramref name="fromVersion"/> to <paramref name="toVersion"/>.</param>
        /// <param name="logDatabaseLocation">The location to the log database.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="fromVersion"/> is not a valid Ringtoets database version,</item>
        /// <item><paramref name="toVersion"/> is not a valid Ringtoets database version,</item>
        /// <item><paramref name="query"/> is empty, <c>null</c>, or consists out of only whitespace characters.</item>
        /// </list></exception>
        public RingtoetsUpgradeScript(string fromVersion, string toVersion, string query, string logDatabaseLocation)
            : base(fromVersion, toVersion)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException(@"Query must have a value.", nameof(query));
            }

            RingtoetsVersionHelper.ValidateVersion(fromVersion);
            RingtoetsVersionHelper.ValidateVersion(toVersion);

            upgradeQuery = query;
            this.logDatabaseLocation = logDatabaseLocation;
        }

        protected override void PerformUpgrade(string sourceLocation, string targetLocation)
        {
            try
            {
                string query = string.Format(upgradeQuery, sourceLocation, logDatabaseLocation);
                using (var databaseFile = new ProjectDatabaseFile(targetLocation))
                {
                    databaseFile.OpenDatabaseConnection();
                    databaseFile.ExecuteQuery(query);
                }
            }
            catch (SQLiteException exception)
            {
                throw new CriticalMigrationException(string.Format(Resources.RingtoetsUpgradeScript_Upgrading_Version_0_To_Version_1_Failed,
                                                                   FromVersion(), ToVersion()), exception);
            }
        }
    }
}