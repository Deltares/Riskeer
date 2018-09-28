// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Util;
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
        /// Creates a new instance of the <see cref="VersionedFileMigrator"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use to compare versions.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparer"/> is 
        /// <c>null</c>.</exception>
        protected VersionedFileMigrator(IComparer comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            fileMigrationScripts = GetAvailableMigrations().OrderBy(ms => ms.SupportedVersion())
                                                           .ThenByDescending(ms => ms.TargetVersion());
            versionedFileComparer = comparer;
        }

        /// <summary>
        /// Returns if a <see cref="FileMigrationScript"/> has been found where <paramref name="fromVersion"/> is the version to migrate from.
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
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public bool NeedsMigrate(IVersionedFile versionedFile, string toVersion)
        {
            if (versionedFile == null)
            {
                throw new ArgumentNullException(nameof(versionedFile));
            }

            if (toVersion == null)
            {
                throw new ArgumentNullException(nameof(toVersion));
            }

            return versionedFileComparer.Compare(versionedFile.GetVersion(), toVersion) < 0;
        }

        /// <summary>
        /// Migrates <paramref name="versionedFile"/> to version <paramref name="toVersion"/> at location <paramref name="newFileLocation"/>.
        /// </summary>
        /// <param name="versionedFile">The source versioned file to migrate from.</param>
        /// <param name="toVersion">The version to upgrade to.</param>
        /// <param name="newFileLocation">The location where the migrated file needs to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the path of the <paramref name="versionedFile"/> or
        /// the <paramref name="newFileLocation"/> is invalid.</exception>
        /// <exception cref="CriticalMigrationException">Thrown when migrating <paramref name="versionedFile"/> 
        /// to a new version on location <paramref name="newFileLocation"/> failed.</exception>
        public void Migrate(IVersionedFile versionedFile, string toVersion, string newFileLocation)
        {
            ValidateMigrationArguments(versionedFile, toVersion, newFileLocation);
            FileMigrationScript migrationScript = TryGetMigrationScript(versionedFile, toVersion);

            IVersionedFile upgradedVersionFile = migrationScript.Upgrade(versionedFile);
            if (!upgradedVersionFile.GetVersion().Equals(toVersion))
            {
                Migrate(upgradedVersionFile, toVersion, newFileLocation);
            }
            else
            {
                MoveMigratedFile(upgradedVersionFile.Location, newFileLocation);
            }
        }

        protected abstract IEnumerable<UpgradeScript> GetAvailableUpgradeScripts();

        protected abstract IEnumerable<CreateScript> GetAvailableCreateScripts();

        /// <summary>
        /// Validate the arguments of the migration command.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the path of the <paramref name="versionedFile"/> or
        /// the <paramref name="newFileLocation"/> is invalid.</exception>
        private static void ValidateMigrationArguments(IVersionedFile versionedFile, string toVersion, string newFileLocation)
        {
            if (versionedFile == null)
            {
                throw new ArgumentNullException(nameof(versionedFile));
            }

            if (toVersion == null)
            {
                throw new ArgumentNullException(nameof(toVersion));
            }

            if (newFileLocation == null)
            {
                throw new ArgumentNullException(nameof(newFileLocation));
            }

            if (IOUtils.GetFullPath(versionedFile.Location).Equals(IOUtils.GetFullPath(newFileLocation)))
            {
                throw new CriticalMigrationException(Resources.Migrate_Target_File_Path_Must_Differ_From_Source_File_Path);
            }
        }

        private FileMigrationScript TryGetMigrationScript(IVersionedFile versionedFile, string toVersion)
        {
            string fromVersion = versionedFile.GetVersion();
            if (!IsVersionSupported(fromVersion))
            {
                throw new CriticalMigrationException(string.Format(Resources.Migrate_From_Version_0_To_Version_1_Not_Supported,
                                                                   fromVersion, toVersion));
            }

            FileMigrationScript migrationScript = GetMigrationScript(fromVersion, toVersion);
            if (migrationScript == null)
            {
                throw new CriticalMigrationException(string.Format(Resources.Migrate_From_Version_0_To_Version_1_Not_Supported,
                                                                   fromVersion, toVersion));
            }

            return migrationScript;
        }

        private static void MoveMigratedFile(string sourceFileName, string destinationFileName)
        {
            try
            {
                File.Copy(sourceFileName, destinationFileName, true);
                File.Delete(sourceFileName);
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                string message = string.Format(Resources.Migrate_Unable_To_Move_From_Location_0_To_Location_1,
                                               sourceFileName, destinationFileName);
                throw new CriticalMigrationException(message, exception);
            }
        }

        private FileMigrationScript GetMigrationScript(string fromVersion, string toVersion)
        {
            IEnumerable<FileMigrationScript> supportedMigrationScripts = fileMigrationScripts.Where(ms => ms.SupportedVersion()
                                                                                                            .Equals(fromVersion));

            return supportedMigrationScripts.FirstOrDefault(ms => ms.TargetVersion().Equals(toVersion))
                   ?? supportedMigrationScripts.FirstOrDefault(ms => versionedFileComparer.Compare(toVersion, ms.TargetVersion()) > 0);
        }

        private IEnumerable<FileMigrationScript> GetAvailableMigrations()
        {
            IEnumerable<UpgradeScript> upgradeScripts = GetAvailableUpgradeScripts();

            foreach (UpgradeScript upgradeScript in upgradeScripts)
            {
                CreateScript createScript = GetAvailableCreateScripts().FirstOrDefault(cs => cs.GetVersion().Equals(upgradeScript.ToVersion()));
                if (createScript != null)
                {
                    yield return new FileMigrationScript(createScript, upgradeScript);
                }
            }
        }
    }
}