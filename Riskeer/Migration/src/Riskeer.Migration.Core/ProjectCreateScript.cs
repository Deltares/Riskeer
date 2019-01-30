// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Data.SQLite;
using Migration.Scripts.Data;
using Migration.Scripts.Data.Exceptions;
using Riskeer.Common.Util;
using Riskeer.Migration.Core.Properties;

namespace Riskeer.Migration.Core
{
    /// <summary>
    /// Class that provides methods for creating a <see cref="ProjectVersionedFile"/> for a specific version.
    /// </summary>
    public class ProjectCreateScript : CreateScript
    {
        private readonly string createQuery;

        /// <summary>
        /// Creates a new instance of the <see cref="ProjectCreateScript"/> class.
        /// </summary>
        /// <param name="version">The version <paramref name="query"/> was designed for.</param>
        /// <param name="query">The SQL query that belongs to <paramref name="version"/>.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="version"/> is not a valid project version,</item>
        /// <item><paramref name="query"/> is empty, <c>null</c>, or consist out of only whitespace characters.</item>
        /// </list></exception>
        public ProjectCreateScript(string version, string query) : base(version)
        {
            ProjectVersionHelper.ValidateVersion(version);

            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException(@"Query must have a value.", nameof(query));
            }

            createQuery = query;
        }

        protected override IVersionedFile GetEmptyVersionedFile(string location)
        {
            try
            {
                using (var databaseFile = new ProjectDatabaseFile(location))
                {
                    databaseFile.OpenDatabaseConnection();
                    databaseFile.ExecuteQuery(createQuery);
                }

                return new ProjectVersionedFile(location);
            }
            catch (SQLiteException exception)
            {
                throw new CriticalMigrationException(string.Format(Resources.RingtoetsCreateScript_Creating_Version_0_Failed,
                                                                   GetVersion()), exception);
            }
        }
    }
}