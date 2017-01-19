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
using Migration.Scripts.Data;
using Migration.Scripts.Data.Exceptions;
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

        /// <summary>
        /// Creates a new instance of the <see cref="VersionedFile"/> class.
        /// </summary>
        public VersionedFileMigrator()
        {
            scriptResource = typeof(MigrationScriptsDataResources).Assembly;
            migrationScripts = GetAvailableMigrations()
                .OrderBy(ms => ms.SupportedVersion())
                .ThenByDescending(ms => ms.TargetVersion());
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
        public static bool NeedsMigrade(VersionedFile versionedFile, string toVersion)
        {
            return string.Compare(versionedFile.GetVersion(), toVersion, StringComparison.InvariantCulture) > 0;
        }

        /// <summary>
        /// Migrates <paramref name="fromVersionedFile"/> to version <paramref name="toVersion"/> at location <paramref name="newFileLocation"/>.
        /// </summary>
        /// <param name="fromVersionedFile">The source versioned file to migrate from.</param>
        /// <param name="toVersion">The version to upgrade to.</param>
        /// <param name="newFileLocation"></param>
        public void Migrate(VersionedFile fromVersionedFile, string toVersion, string newFileLocation)
        {
            var supportedMigrationScripts = migrationScripts
                .Where(ms => ms.SupportedVersion()
                               .Equals(fromVersionedFile.GetVersion()));

            if (!supportedMigrationScripts.Any())
            {
                return;
            }

            MigrationScript migrationScript = supportedMigrationScripts.FirstOrDefault(ms => ms.TargetVersion().Equals(toVersion))
                                              ?? supportedMigrationScripts.First();

            var upgradedVersionFile = migrationScript.Upgrade(fromVersionedFile);
            if (!upgradedVersionFile.GetVersion().Equals(toVersion))
            {
                Migrate(upgradedVersionFile, toVersion, newFileLocation);
            }
            else
            {
                try
                {
                    File.Move(upgradedVersionFile.Location, newFileLocation);
                }
                catch (IOException exception)
                {
                    throw new CriticalDatabaseMigrationException("Er is een onverwachte fout opgetreden tijdens het verplaatsen van het gemigreerde bestand.", exception);
                }
            }
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