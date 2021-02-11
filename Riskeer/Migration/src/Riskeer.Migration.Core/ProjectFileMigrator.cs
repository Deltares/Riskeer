﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Util;
using Riskeer.Migration.Core.Properties;

namespace Riskeer.Migration.Core
{
    /// <summary>
    /// Class that provides methods for migrating a <see cref="ProjectVersionedFile"/>.
    /// </summary>
    public class ProjectFileMigrator : VersionedFileMigrator
    {
        private readonly Assembly scriptResource;

        /// <summary>
        /// Creates a new instance of the <see cref="ProjectFileMigrator"/> class.
        /// </summary>
        public ProjectFileMigrator() : base(new ProjectVersionComparer())
        {
            scriptResource = typeof(Resources).Assembly;
        }

        /// <summary>
        /// Gets or sets the path to the logging database.
        /// </summary>
        public string LogPath { private get; set; }

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
            return scriptResource.GetManifestResourceNames()
                                 .Where(r => r.Contains("Migration_"))
                                 .Select(CreateNewUpgradeScript)
                                 .ToArray();
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

            string upgradeQuery = GetStringOfStream(upgradeStream);

            return new ProjectUpgradeScript(fromVersion, toVersion, upgradeQuery, LogPath);
        }

        #endregion

        #region CreateScript

        protected override IEnumerable<CreateScript> GetAvailableCreateScripts()
        {
            return scriptResource.GetManifestResourceNames()
                                 .Where(r => r.Contains("DatabaseStructure"))
                                 .Select(CreateNewCreateScript)
                                 .ToArray();
        }

        private static string GetCreateScriptVersion(string filename)
        {
            Match match = Regex.Match(filename, @"(DatabaseStructure)(.*)(\.sql)$", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[2].Value : null;
        }

        private ProjectCreateScript CreateNewCreateScript(string resourceName)
        {
            string version = GetCreateScriptVersion(resourceName);
            Stream createStream = scriptResource.GetManifestResourceStream(resourceName);
            string query = GetStringOfStream(createStream);
            return new ProjectCreateScript(version, query);
        }

        #endregion
    }
}