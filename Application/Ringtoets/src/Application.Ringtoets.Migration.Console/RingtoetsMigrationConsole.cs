// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Migration.Console.Properties;
using Core.Common.Util;
using Migration.Console;
using Ringtoets.Common.Util;
using Ringtoets.Migration.Core;
using MigrationCoreStorageResources = Migration.Core.Storage.Properties.Resources;
using RingtoetsMigrationCoreResources = Ringtoets.Migration.Core.Properties.Resources;

namespace Application.Ringtoets.Migration.Console
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
            ValidateIsVersionSupportedArgument(location);

            var versionedFile = new RingtoetsVersionedFile(location);
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();
            string version = versionedFile.GetVersion();

            bool isSupported = migrator.IsVersionSupported(version);

            if (isSupported)
            {
                System.Console.WriteLine(Resources.CommandSupported_File_Able_To_Migrate_To_Version_0, currentDatabaseVersion);
            }
            else
            {
                ConsoleHelper.WriteErrorLine(MigrationCoreStorageResources.Migrate_From_Version_0_To_Version_1_Not_Supported,
                                             version, currentDatabaseVersion);
            }
        }

        private static void ValidateIsVersionSupportedArgument(string location)
        {
            if (!IOUtils.IsValidFilePath(location))
            {
                throw new ArgumentException(RingtoetsMigrationCoreResources.CommandSupported_Source_Not_Valid_Path);
            }
        }

        private static void ShowSupportedCommand()
        {
            System.Console.WriteLine(Resources.CommandSupported_Brief);
            ConsoleHelper.WriteCommandDescriptionLine(Resources.CommandSupported_Detailed);
        }

        private static void MigrateCommand(string filePath, string toFilePath)
        {
            ValidateMigrationArguments(filePath, toFilePath);
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();
            var sourceFile = new RingtoetsVersionedFile(filePath);

            migrator.Migrate(sourceFile, currentDatabaseVersion, toFilePath);
            System.Console.WriteLine(Resources.CommandMigrate_Successful_Migration_From_Location_0_To_Location_1_Version_2,
                                     filePath, toFilePath, currentDatabaseVersion);
        }

        private static void ValidateMigrationArguments(string filePath, string toFilePath)
        {
            if (!(IOUtils.IsValidFilePath(filePath) && IOUtils.IsValidFilePath(toFilePath)))
            {
                throw new ArgumentException(Resources.CommandMigrate_Source_Or_Destination_Not_Valid_Path);
            }
        }

        private static void ShowMigrateCommand()
        {
            System.Console.WriteLine(Resources.CommandMigrate_Brief);
            ConsoleHelper.WriteCommandDescriptionLine(Resources.CommandMigrate_Detailed);
        }

        #endregion
    }
}