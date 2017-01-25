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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Migration.Core.Storage.Properties;
using Migration.Scripts.Data;
using Migration.Scripts.Data.Exceptions;
using Ringtoets.Common.Utils;
using MigrationScriptsDataResources = Migration.Scripts.Data.Properties.Resources;

namespace Migration.Core.Storage
{
    /// <summary>
    /// Class that provides methods for migrating a <see cref="VersionedFile"/>.
    /// </summary>
    public class VersionedFileMigrator
    {
        private readonly IOrderedEnumerable<MigrationScript> migrationScripts;
        private readonly Assembly scriptResource;
        private readonly RingtoetsVersionComparer ringtoetsVersionComparer;

        /// <summary>
        /// Creates a new instance of the <see cref="VersionedFile"/> class.
        /// </summary>
        public VersionedFileMigrator()
        {
            scriptResource = typeof(MigrationScriptsDataResources).Assembly;
            migrationScripts = GetAvailableMigrations()
                .OrderBy(ms => ms.SupportedVersion())
                .ThenByDescending(ms => ms.TargetVersion());
            ringtoetsVersionComparer = new RingtoetsVersionComparer();
        }

        /// <summary>
        /// Returns if <paramref name="fromVersion"/> is a supported version to migrate from.
        /// </summary>
        /// <param name="fromVersion">Version to validate.</param>
        /// <returns><c>true</c> if <paramref name="fromVersion"/> is supported, <c>false</c> otherwise.</returns>
        public bool IsVersionSupported(string fromVersion)
        {
            return !string.IsNullOrWhiteSpace(fromVersion) && migrationScripts.Any(ms => ms.SupportedVersion().Equals(fromVersion));
        }

        /// <summary>
        /// Returns if <paramref name="versionedFile"/> needs to be upgraded to have equal version 
        /// as <paramref name="toVersion"/>.
        /// </summary>
        /// <param name="versionedFile">The versioned file to validate.</param>
        /// <param name="toVersion">The version to upgrade to.</param>
        /// <returns><c>true</c> if <paramref name="versionedFile"/> needs to be upgraded to <paramref name="toVersion"/>, 
        /// <c>false</c> otherwise.</returns>
        public bool NeedsMigrate(VersionedFile versionedFile, string toVersion)
        {
            return ringtoetsVersionComparer.Compare(versionedFile.GetVersion(), toVersion) < 0;
        }

        /// <summary>
        /// Migrates <paramref name="fromVersionedFile"/> to version <paramref name="toVersion"/> at location <paramref name="newFileLocation"/>.
        /// </summary>
        /// <param name="fromVersionedFile">The source versioned file to migrate from.</param>
        /// <param name="toVersion">The version to upgrade to.</param>
        /// <param name="newFileLocation">The location where the migrated file needs to be saved.</param>
        /// <exception cref="CriticalDatabaseMigrationException">Thrown when migrating <paramref name="fromVersionedFile"/> 
        /// to a new version on location <paramref name="newFileLocation"/> failed.</exception>
        public void Migrate(VersionedFile fromVersionedFile, string toVersion, string newFileLocation)
        {
            if (Path.GetFullPath(fromVersionedFile.Location).Equals(Path.GetFullPath(newFileLocation)))
            {
                throw new CriticalDatabaseMigrationException(Resources.Migrate_Target_File_Path_Must_Differ_From_Source_File_Path);
            }
            string fromVersion = fromVersionedFile.GetVersion();
            if (!IsVersionSupported(fromVersion))
            {
                throw new CriticalDatabaseMigrationException(string.Format(Resources.Upgrade_Version_0_Not_Supported,
                                                                           fromVersion));
            }

            MigrationScript migrationScript = GetMigrationScript(fromVersion, toVersion);
            if (migrationScript == null)
            {
                throw new CriticalDatabaseMigrationException(string.Format(Resources.Migrate_From_Version_0_To_Version_1_Not_Supported,
                                                                           fromVersion, toVersion));
            }

            var upgradedVersionFile = migrationScript.Upgrade(fromVersionedFile);
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
                    throw new CriticalDatabaseMigrationException(message, exception);
                }
            }
        }

        private MigrationScript GetMigrationScript(string fromVersion, string toVersion)
        {
            var supportedMigrationScripts = migrationScripts.Where(ms => ms.SupportedVersion()
                                                                           .Equals(fromVersion));

            if (!supportedMigrationScripts.Any())
            {
                return null;
            }

            return supportedMigrationScripts.FirstOrDefault(ms => ms.TargetVersion().Equals(toVersion))
                   ?? supportedMigrationScripts.FirstOrDefault(ms => ringtoetsVersionComparer.Compare(toVersion, ms.TargetVersion()) > 0);
        }

        private IEnumerable<MigrationScript> GetAvailableMigrations()
        {
            IEnumerable<UpgradeScript> migrationStreams = GetAvailableUpgradeScripts();
            IEnumerable<CreateScript> createScripts = GetAvailableCreateScripts();

            foreach (var migrationScript in migrationStreams)
            {
                CreateScript createScript = createScripts.FirstOrDefault(cs => cs.Version().Equals(migrationScript.ToVersion()));
                if (createScript != null)
                {
                    yield return new MigrationScript(createScript, migrationScript);
                }
            }
        }

        private static string GetStringOfStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        #region UpgradeScript

        private IEnumerable<UpgradeScript> GetAvailableUpgradeScripts()
        {
            return scriptResource.GetManifestResourceNames().Where(r => r.Contains("Migration_"))
                                 .Select(CreateNewUpgradeScript);
        }

        private static string GetMigrationScriptFromVersion(string filename)
        {
            Match match = Regex.Match(filename, @"(Migration_)(.*)(_.*\.sql)$", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[2].Value : null;
        }

        private static string GetMigrationScriptToVersion(string filename)
        {
            Match match = Regex.Match(filename, @"(Migration_.*_)(.*)(\.sql)$", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[2].Value : null;
        }

        private UpgradeScript CreateNewUpgradeScript(string resourceName)
        {
            string fromVersion = GetMigrationScriptFromVersion(resourceName);
            string toVersion = GetMigrationScriptToVersion(resourceName);
            Stream upgradeStream = scriptResource.GetManifestResourceStream(resourceName);

            var upgradeQuery = GetStringOfStream(upgradeStream);

            return new UpgradeScript(fromVersion, toVersion, upgradeQuery);
        }

        #endregion

        #region CreateScript

        private IEnumerable<CreateScript> GetAvailableCreateScripts()
        {
            return scriptResource.GetManifestResourceNames().Where(r => r.Contains("DatabaseStructure"))
                                 .Select(CreateNewCreateScript);
        }

        private static string GetCreateScriptVersion(string filename)
        {
            Match match = Regex.Match(filename, @"(DatabaseStructure)(.*)(\.sql)$", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[2].Value : null;
        }

        private CreateScript CreateNewCreateScript(string resourceName)
        {
            string version = GetCreateScriptVersion(resourceName);
            Stream createStream = scriptResource.GetManifestResourceStream(resourceName);
            string query = GetStringOfStream(createStream);
            return new CreateScript(version, query);
        }

        #endregion
    }
}