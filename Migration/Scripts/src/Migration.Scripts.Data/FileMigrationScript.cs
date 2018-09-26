// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using Migration.Scripts.Data.Exceptions;

namespace Migration.Scripts.Data
{
    /// <summary>
    /// Class that provides methods for migration a <see cref="IVersionedFile"/>.
    /// </summary>
    public class FileMigrationScript
    {
        private readonly CreateScript createScript;
        private readonly UpgradeScript upgradeScript;

        /// <summary>
        /// Creates a new instance of <see cref="FileMigrationScript"/>.
        /// </summary>
        /// <param name="createScript">The create script to use for migrating.</param>
        /// <param name="upgradeScript">The upgrade script to use for migrating.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public FileMigrationScript(CreateScript createScript, UpgradeScript upgradeScript)
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
        /// Gets the version number from which files can be upgraded by the script.
        /// </summary>
        /// <returns>The supported version.</returns>
        public string SupportedVersion()
        {
            return upgradeScript.FromVersion();
        }

        /// <summary>
        /// Gets the version number to which files will be upgraded by the script.
        /// </summary>
        /// <returns>The target version.</returns>
        public string TargetVersion()
        {
            return upgradeScript.ToVersion();
        }

        /// <summary>
        /// Uses <paramref name="sourceVersionedFile"/> to upgrade to a new <see cref="IVersionedFile"/>.
        /// </summary>
        /// <param name="sourceVersionedFile">The <see cref="IVersionedFile"/> used to upgrade.</param>
        /// <returns>The upgraded <see cref="IVersionedFile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sourceVersionedFile"/> is <c>null</c>.</exception>
        /// <exception cref="CriticalMigrationException">Thrown when migration failed.</exception>
        public IVersionedFile Upgrade(IVersionedFile sourceVersionedFile)
        {
            if (sourceVersionedFile == null)
            {
                throw new ArgumentNullException(nameof(sourceVersionedFile));
            }

            string newLocation = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            IVersionedFile newVersionedFile = createScript.CreateEmptyVersionedFile(newLocation);
            upgradeScript.Upgrade(sourceVersionedFile.Location, newVersionedFile.Location);
            return newVersionedFile;
        }
    }
}