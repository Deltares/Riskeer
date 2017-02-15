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
using Application.Ringtoets.Migration;
using Application.Ringtoets.MigrationConsole.Properties;
using Migration.Console;
using Ringtoets.Common.Utils;
using MigrationCoreStorageResources = Migration.Core.Storage.Properties.Resources;

namespace Application.Ringtoets.MigrationConsole
{
    /// <summary>
    /// Console application that can migrate a Ringtoets database file to a newer version.
    /// </summary>
    public class RingtoetsMigrationConsole : ConsoleBase
    {
        private static readonly string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsMigrationConsole"/>. 
        /// </summary>
        public RingtoetsMigrationConsole() : base(Resources.RingtoetsMigrationTool_ApplicationName,
                                                  GetApplicationDescription()) {}

        protected override void ExecuteCommand(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    IsVersionSupportedCommand(args[0]);
                    break;
                case 2:
                    MigrateCommand(args[0], args[1]);
                    break;
                default:
                    string command = string.Join(" ", args);
                    InvalidCommand(command);
                    break;
            }
        }

        protected override void DisplayCommands()
        {
            ShowSupportedCommand();
            ShowMigrateCommand();
        }

        private static string GetApplicationDescription()
        {
            return string.Format(Resources.RingtoetsMigrationTool_ApplicationDescription_Version_0, currentDatabaseVersion);
        }

        #region Commands

        private static void InvalidCommand(string command)
        {
            throw new NotSupportedException(string.Format(Resources.CommandInvalid_Command_0_Is_not_valid, command));
        }

        private static void IsVersionSupportedCommand(string location)
        {
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();
            var versionedFile = new RingtoetsVersionedFile(location);
            var version = versionedFile.GetVersion();

            bool isSupported = migrator.IsVersionSupported(version);

            if (isSupported)
            {
                Console.WriteLine(Resources.CommandSupported_File_Able_To_Migrate_To_Version_0, currentDatabaseVersion);
            }
            else
            {
                ConsoleHelper.WriteErrorLine(MigrationCoreStorageResources.Migrate_From_Version_0_To_Version_1_Not_Supported,
                                             version, currentDatabaseVersion);
            }
        }

        private static void ShowSupportedCommand()
        {
            Console.WriteLine(Resources.CommandSupported_Brief);
            ConsoleHelper.WriteCommandDescriptionLine(Resources.CommandSupported_Detailed);
        }

        private static void MigrateCommand(string filepath, string toFilepath)
        {
            if (string.IsNullOrEmpty(filepath) || string.IsNullOrEmpty(toFilepath))
            {
                throw new ArgumentException(Resources.CommandMigrate_Source_Or_Destination_Null_Or_Empty);
            }
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();
            var sourceFile = new RingtoetsVersionedFile(filepath);

            migrator.Migrate(sourceFile, currentDatabaseVersion, toFilepath);
            Console.WriteLine(Resources.CommandMigrate_Successful_Migration_From_Location_0_To_Location_1_Version_2,
                              filepath, toFilepath, currentDatabaseVersion);
        }

        private static void ShowMigrateCommand()
        {
            Console.WriteLine(Resources.CommandMigrate_Brief);
            ConsoleHelper.WriteCommandDescriptionLine(Resources.CommandMigrate_Detailed);
        }

        #endregion
    }
}