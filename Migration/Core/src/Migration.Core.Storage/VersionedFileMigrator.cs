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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Migration.Core.Storage.Properties;
using Migration.Scripts.Data;
using Migration.Scripts.Data.Exceptions;

namespace Migration.Core.Storage
{
    /// <summary>
    /// Class that provides methods for migrating a <see cref="IVersionedFile"/>.
    /// </summary>
    public abstract class VersionedFileMigrator
    {
        private readonly IOrderedEnumerable<FileMigrationScript> fileMigrationScripts;
        private readonly IComparer versionedFileComparer;

        /// <summary>
        /// Creates a new instance of the <see cref="IVersionedFile"/> class.
        /// </summary>
        protected VersionedFileMigrator(IComparer comparer)
        {
            fileMigrationScripts = GetAvailableMigrations()
                .OrderBy(ms => ms.SupportedVersion())
                .ThenByDescending(ms => ms.TargetVersion());
            versionedFileComparer = comparer;
        }

        /// <summary>
        /// Returns if <paramref name="fromVersion"/> is a supported version to migrate from.
        /// </summary>
        /// <param name="fromVersion">Version to validate.</param>
        /// <returns><c>true</c> if <paramref name="fromVersion"/> is supported, <c>false</c> otherwise.</returns>
        public bool IsVersionSupported(string fromVersion)
        {
            return !string.IsNullOrWhiteSpace(fromVersion) && fileMigrationScripts.Any(ms => ms.SupportedVersion().Equals(fromVersion));
        }

        /// <summary>
        /// Returns if <paramref name="versionedFile"/> needs to be upgraded to have equal version 
        /// as <paramref name="toVersion"/>.
        /// </summary>
        /// <param name="versionedFile">The versioned file to validate.</param>
        /// <param name="toVersion">The version to upgrade to.</param>
        /// <returns><c>true</c> if <paramref name="versionedFile"/> needs to be upgraded to <paramref name="toVersion"/>, 
        /// <c>false</c> otherwise.</returns>
        public bool NeedsMigrate(IVersionedFile versionedFile, string toVersion)
        {
            return versionedFileComparer.Compare(versionedFile.GetVersion(), toVersion) < 0;
        }

        /// <summary>
        /// Migrates <paramref name="fromVersionedFile"/> to version <paramref name="toVersion"/> at location <paramref name="newFileLocation"/>.
        /// </summary>
        /// <param name="fromVersionedFile">The source versioned file to migrate from.</param>
        /// <param name="toVersion">The version to upgrade to.</param>
        /// <param name="newFileLocation">The location where the migrated file needs to be saved.</param>
        /// <exception cref="CriticalMigrationException">Thrown when migrating <paramref name="fromVersionedFile"/> 
        /// to a new version on location <paramref name="newFileLocation"/> failed.</exception>
        public void Migrate(IVersionedFile fromVersionedFile, string toVersion, string newFileLocation)
        {
            if (Path.GetFullPath(fromVersionedFile.Location).Equals(Path.GetFullPath(newFileLocation)))
            {
                throw new CriticalMigrationException(Resources.Migrate_Target_File_Path_Must_Differ_From_Source_File_Path);
            }
            string fromVersion = fromVersionedFile.GetVersion();
            if (!IsVersionSupported(fromVersion))
            {
                throw new CriticalMigrationException(string.Format(Resources.Upgrade_Version_0_Not_Supported,
                                                                   fromVersion));
            }

            FileMigrationScript migrationScript = GetMigrationScript(fromVersion, toVersion);
            if (migrationScript == null)
            {
                throw new CriticalMigrationException(string.Format(Resources.Migrate_From_Version_0_To_Version_1_Not_Supported,
                                                                   fromVersion, toVersion));
            }

            IVersionedFile upgradedVersionFile = migrationScript.Upgrade(fromVersionedFile);
            if (!upgradedVersionFile.GetVersion().Equals(toVersion))
            {
                Migrate(upgradedVersionFile, toVersion, newFileLocation);
            }
            else
            {
                try
                {
                    File.Copy(upgradedVersionFile.Location, newFileLocation, true);
                    File.Delete(upgradedVersionFile.Location);
                }
                catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
                {
                    var message = string.Format(Resources.Migrate_Unable_To_Move_From_Location_0_To_Location_1,
                                                upgradedVersionFile.Location, newFileLocation);
                    throw new CriticalMigrationException(message, exception);
                }
            }
        }

        protected abstract IEnumerable<UpgradeScript> GetAvailableUpgradeScripts();

        protected abstract IEnumerable<CreateScript> GetAvailableCreateScripts();

        private FileMigrationScript GetMigrationScript(string fromVersion, string toVersion)
        {
            var supportedMigrationScripts = fileMigrationScripts.Where(ms => ms.SupportedVersion()
                                                                               .Equals(fromVersion));

            if (!supportedMigrationScripts.Any())
            {
                return null;
            }

            return supportedMigrationScripts.FirstOrDefault(ms => ms.TargetVersion().Equals(toVersion))
                   ?? supportedMigrationScripts.FirstOrDefault(ms => versionedFileComparer.Compare(toVersion, ms.TargetVersion()) > 0);
        }

        private IEnumerable<FileMigrationScript> GetAvailableMigrations()
        {
            IEnumerable<UpgradeScript> migrationStreams = GetAvailableUpgradeScripts();
            IEnumerable<CreateScript> createScripts = GetAvailableCreateScripts();

            foreach (var migrationScript in migrationStreams)
            {
                CreateScript createScript = createScripts.FirstOrDefault(cs => cs.Version().Equals(migrationScript.ToVersion()));
                if (createScript != null)
                {
                    yield return new FileMigrationScript(createScript, migrationScript);
                }
            }
        }
    }
}