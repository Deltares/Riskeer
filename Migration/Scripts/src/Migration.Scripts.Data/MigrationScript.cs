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
using System.IO;
using Migration.Scripts.Data.Exceptions;
using Migration.Scripts.Data.Properties;

namespace Migration.Scripts.Data
{
    /// <summary>
    /// Class that provides methods for migration a <see cref="VersionedFile"/>.
    /// </summary>
    public class MigrationScript
    {
        private readonly CreateScript createScript;
        private readonly UpgradeScript upgradeScript;

        /// <summary>
        /// Creates a new instance of <see cref="MigrationScript"/>.
        /// </summary>
        /// <param name="createScript">The create script to use for migrating.</param>
        /// <param name="upgradeScript">The upgrade script to use for migrating.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public MigrationScript(CreateScript createScript, UpgradeScript upgradeScript)
        {
            if (createScript == null)
            {
                throw new ArgumentNullException(nameof(createScript));
            }
            if (upgradeScript == null)
            {
                throw new ArgumentNullException(nameof(upgradeScript));
            }
            this.createScript = createScript;
            this.upgradeScript = upgradeScript;
        }

        /// <summary>
        /// Gets the supported version.
        /// </summary>
        /// <returns>The supported version.</returns>
        public string SupportedVersion()
        {
            return upgradeScript.FromVersion();
        }

        /// <summary>
        /// Gets the target version.
        /// </summary>
        /// <returns>The target version.</returns>
        public string TargetVersion()
        {
            return upgradeScript.ToVersion();
        }

        /// <summary>
        /// Uses <paramref name="sourceVersionedFile"/> to upgrade to a new <see cref="VersionedFile"/>.
        /// </summary>
        /// <param name="sourceVersionedFile"></param>
        /// <returns>The upgraded <see cref="VersionedFile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sourceVersionedFile"/> is <c>null</c>.</exception>
        /// <exception cref="CriticalDatabaseMigrationException">Thrown when migration failed.</exception>
        public VersionedFile Upgrade(VersionedFile sourceVersionedFile)
        {
            if (sourceVersionedFile == null)
            {
                throw new ArgumentNullException(nameof(sourceVersionedFile));
            }
            var newLocation = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                VersionedFile newVersionedFile = createScript.CreateEmptyVersionedFile(newLocation);
                upgradeScript.Upgrade(sourceVersionedFile, newVersionedFile);
                return newVersionedFile;
            }
            catch (SQLiteException exception)
            {
                throw new CriticalDatabaseMigrationException(Resources.Migrate_failed, exception);
            }
        }
    }
}