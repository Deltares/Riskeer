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
using System.Data.SQLite;
using Application.Ringtoets.Migration.Properties;
using Migration.Scripts.Data;
using Migration.Scripts.Data.Exceptions;

namespace Application.Ringtoets.Migration
{
    /// <summary>
    /// Class that provides methods for the upgrading a <see cref="RingtoetsVersionedFile"/> for a specific version.
    /// </summary>
    public class RingtoetsUpgradeScript : UpgradeScript
    {
        private readonly string upgradeQuery;

        /// <summary>
        /// Creates a new instance of the <see cref="RingtoetsUpgradeScript"/> class.
        /// </summary>
        /// <param name="fromVersion">The source version <paramref name="query"/> was designed for.</param>
        /// <param name="toVersion">The target version <paramref name="query"/> was designed for.</param>
        /// <param name="query">The SQL query to upgrade from <paramref name="fromVersion"/> to <paramref name="toVersion"/>.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="fromVersion"/> is empty or <c>null</c>,</item>
        /// <item><paramref name="toVersion"/> is empty or <c>null</c>,</item>
        /// <item><paramref name="query"/> is empty, <c>null</c>, or consists out of only whitespace characters.</item>
        /// </list></exception>
        public RingtoetsUpgradeScript(string fromVersion, string toVersion, string query)
            : base(fromVersion, toVersion)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException(@"Query must have a value.", nameof(query));
            }
            upgradeQuery = query;
        }

        protected override void PerformUpgrade(IVersionedFile source, IVersionedFile target)
        {
            try
            {
                var query = string.Format(upgradeQuery, source.Location);
                using (var databaseFile = new RingtoetsDatabaseFile(target.Location))
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