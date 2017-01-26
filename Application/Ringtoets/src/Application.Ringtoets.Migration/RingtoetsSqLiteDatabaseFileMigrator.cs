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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Migration.Core.Storage;
using Migration.Scripts.Data;
using MigrationScriptsDataResources = Migration.Scripts.Data.Properties.Resources;

namespace Application.Ringtoets.Migration
{
    /// <summary>
    /// Class that provides methods for migrating a <see cref="RingtoetsVersionedFile"/>.
    /// </summary>
    public class RingtoetsSqLiteDatabaseFileMigrator : VersionedFileMigrator
    {
        private readonly Assembly scriptResource;

        /// <summary>
        /// Creates a new instance of the <see cref="RingtoetsSqLiteDatabaseFileMigrator"/> class.
        /// </summary>
        public RingtoetsSqLiteDatabaseFileMigrator()
        {
            scriptResource = typeof(MigrationScriptsDataResources).Assembly;
        }

        private static string GetStringOfStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        #region UpgradeScript

        protected override IEnumerable<UpgradeScript> GetAvailableUpgradeScripts()
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

        protected override IEnumerable<CreateScript> GetAvailableCreateScripts()
        {
            return scriptResource.GetManifestResourceNames().Where(r => r.Contains("DatabaseStructure"))
                                 .Select(CreateNewCreateScript);
        }

        private static string GetCreateScriptVersion(string filename)
        {
            Match match = Regex.Match(filename, @"(DatabaseStructure)(.*)(\.sql)$", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[2].Value : null;
        }

        private RingtoetsCreateScript CreateNewCreateScript(string resourceName)
        {
            string version = GetCreateScriptVersion(resourceName);
            Stream createStream = scriptResource.GetManifestResourceStream(resourceName);
            string query = GetStringOfStream(createStream);
            return new RingtoetsCreateScript(version, query);
        }

        #endregion
    }
}