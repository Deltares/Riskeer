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
using System.Linq;
using Application.Ringtoets.Migration;
using Application.Ringtoets.MigrationConsole.Properties;
using Migration.Console;
using Migration.Scripts.Data.Exceptions;
using Ringtoets.Common.Utils;
using MigrationConsoleResources = Migration.Console.Properties.Resources;

namespace Application.Ringtoets.MigrationConsole
{
    /// <summary>
    /// Console application that can migrate a Ringtoets database file to a newer version.
    /// </summary>
    public static class RingtoetsMigrationTool
    {
        private const string commandMigrate = "--migrate";
        private const string commandVersionSupported = "--supported";
        private const string commandHelp = "--help";
        private const string commandHelpShort = "-h";

        /// <summary>
        /// Main Ringtoets migration application.
        /// </summary>
        /// <param name="args">Arguments </param>
        /// <remarks>Accepted commands:<list type="bullet">
        /// <item>--help Shows help menu,</item>
        /// <item>--supported Returns if the database file is supported,</item>
        /// <item>--migrate Migrates the database file to a newer version.</item>
        /// </list></remarks>
        public static void Main(string[] args)
        {
            try
            {
                ExecuteCommand(args);
            }
            catch (Exception exception)
            {
                DisplayException(exception);
                DisplayAllCommands();

                if (exception is CriticalMigrationException || exception is NotSupportedException)
                {
                    Exit(ErrorCode.ErrorBadCommand);
                    return;
                }
                Exit(ErrorCode.ErrorInvalidCommandLine);
                return;
            }

            Exit(ErrorCode.ErrorSuccess);
        }

        private static void Exit(ErrorCode errorCode)
        {
            EnvironmentControl.Instance.Exit(errorCode);
        }

        private static void DisplayException(Exception exception)
        {
            ConsoleHelper.WriteErrorLine(exception.Message);
            if (exception.InnerException != null)
            {
                ConsoleHelper.WriteErrorLine(MigrationConsoleResources.Message_Inner_Exception_0,
                                             exception.InnerException.Message);
            }
            ConsoleHelper.WriteErrorLine("");
        }

        private static void ExecuteCommand(string[] args)
        {
            string command = args.FirstOrDefault() ?? commandHelp;
            if (command.Equals(commandHelp) || command.Equals(commandHelpShort))
            {
                DisplayAllCommands();
                return;
            }

            var length = args.Length;
            switch (length)
            {
                case 1:
                    IsVersionSupportedCommand(args[0]);
                    break;
                case 2:
                    MigrateCommand(args[0], args[1]);
                    break;
                default:
                    command = string.Join(" ", args);
                    InvalidCommand(command);
                    break;
            }
        }

        private static void DisplayAllCommands()
        {
            Console.WriteLine(Resources.RingtoetsMigrationTool_Info);
            Console.WriteLine();
            Console.WriteLine(Resources.CommandSupported_Command_0_Brief, commandHelpShort);
            Console.WriteLine(Resources.CommandSupported_Command_0_Brief, commandHelp);
            ConsoleHelper.WriteCommandDescriptionLine(Resources.CommandHelp_Detailed);
            ShowMigrateCommand();
            ShowSupportedCommand();
        }

        #region Invalid Command

        private static void InvalidCommand(string command)
        {
            throw new NotSupportedException(string.Format(Resources.CommandInvalid_Command_0_Is_not_valid, command));
        }

        #endregion

        #region Version Supported Command

        private static void IsVersionSupportedCommand(string location)
        {
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();
            var versionedFile = new RingtoetsVersionedFile(location);
            var version = versionedFile.GetVersion();

            bool isSupported = migrator.IsVersionSupported(version);
            Console.WriteLine(isSupported
                                  ? Resources.CommandSupported_File_Supported
                                  : Resources.CommandSupported_File_Not_Supported);
        }

        private static void ShowSupportedCommand()
        {
            Console.WriteLine(Resources.CommandSupported_Brief);
            ConsoleHelper.WriteCommandDescriptionLine(Resources.CommandSupported_Detailed);
        }

        #endregion

        #region Migrate Command

        private static void MigrateCommand(string filepath, string toFilepath)
        {
            string toVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();
            var sourceFile = new RingtoetsVersionedFile(filepath);

            migrator.Migrate(sourceFile, toVersion, toFilepath);
            Console.WriteLine(Resources.CommandMigrate_Successful_Migration_From_Location_0_To_Location_1,
                              filepath, toFilepath);
        }

        private static void ShowMigrateCommand()
        {
            Console.WriteLine(Resources.CommandMigrate_Brief);
            ConsoleHelper.WriteCommandDescriptionLine(Resources.CommandMigrate_Detailed);
        }

        #endregion
    }
}