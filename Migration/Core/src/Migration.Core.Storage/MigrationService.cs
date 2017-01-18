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
using ScriptsDataPropertiesResources = Migration.Scripts.Data.Properties.Resources;

namespace Migration.Core.Storage
{
    /// <summary>
    /// Class responsible for performing migrations.
    /// </summary>
    public static class MigrationService
    {
        /// <summary>
        /// Executes the migration from <paramref name="sourceFile"/> to <paramref name="targetFile"/>.
        /// </summary>
        /// <param name="sourceFile">The source file that needs to be migrated.</param>
        /// <param name="targetFile">The target file that will contain the migrated data from <paramref name="sourceFile"/>.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="sourceFile"/> is the same file as <paramref name="targetFile"/>.</item>
        /// </list></exception>
        public static void Execute(string sourceFile, string targetFile)
        {
            if (sourceFile == targetFile)
            {
                throw new ArgumentException($"{sourceFile} cannot be the same location as {targetFile}");
            }

            using (var source = new MigrationDatabaseSourceFile(sourceFile))
            using (var target = new MigrationDatabaseTargetFile(targetFile))
            {
                target.OpenDatabaseConnection();

                string version = source.GetVersion();
                switch (version)
                {
                    case "4":
                        MigrateVersion4To171(source, target);
                        break;
                    case "17.1":
                        // throw exception?
                        break;
                    default:
                        throw new InvalidOperationException($"Version {version} cannot be upgraded.");
                }
            }
        }

        private static void MigrateVersion4To171(MigrationDatabaseSourceFile source, MigrationDatabaseTargetFile target)
        {
            target.CreateStructure(ScriptsDataPropertiesResources.DatabaseStructure17_1);

            var query = GetMigrationQuery(ScriptsDataPropertiesResources.Migration4_17_1, source.Path);

            target.ExecuteMigration(query);
        }

        private static string GetMigrationQuery(string migrationScript, string sourceFilePath)
        {
            return string.Format(migrationScript, sourceFilePath);
        }
    }
}