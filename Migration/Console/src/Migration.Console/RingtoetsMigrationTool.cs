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
using Migration.Console.Properties;
using Migration.Core.Storage;
using Migration.Scripts.Data;
using SystemConsole = System.Console;

namespace Migration.Console
{
    /// <summary>
    /// Console application that can migrate a Ringtoets database file to a newer version.
    /// </summary>
    public static class RingtoetsMigrationTool
    {
        private const string commandMigrate = "--migrate";
        private const string commandVersionSupported = "--supported";
        private const string commandHelp = "--help";

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
            catch (ArgumentException exception)
            {
                ConsoleHelper.WriteErrorLine(exception.Message);
                Exit(ErrorCode.ErrorBadArguments);
            }
        }

        private static void ExecuteCommand(string[] args)
        {
            string command = args.FirstOrDefault() ?? commandHelp;
            switch (command)
            {
                case commandMigrate:
                    MigrateCommand(args);
                    break;
                case commandVersionSupported:
                    IsVersionSupportedCommand(args);
                    break;
                case commandHelp:
                    DisplayAllCommands();
                    break;
                default:
                    InvalidCommand(command);
                    DisplayAllCommands();
                    Exit(ErrorCode.ErrorBadCommand);
                    break;
            }
        }

        private static void Exit(ErrorCode errorCode)
        {
            EnvironmentControl.Instance.Exit((int) errorCode);
        }

        private static void DisplayAllCommands()
        {
            ConsoleHelper.WriteInfoLine(commandHelp
                                        + "\t"
                                        + Resources.CommandHelp_Command_0_Detailed, commandHelp);
            ShowMigrateCommand();
            ShowSupportedCommand();
        }

        #region Invalid Command

        private static void InvalidCommand(string command)
        {
            ConsoleHelper.WriteErrorLine(Resources.CommandInvalid_Command_0_Is_not_valid, command);
            SystemConsole.WriteLine("");
        }

        #endregion

        #region Version Supported Command

        private static void IsVersionSupportedCommand(string[] args)
        {
            if (args.Length != 2)
            {
                ConsoleHelper.WriteErrorLine(Resources.Command_0_Incorrect_number_of_parameters, commandVersionSupported);
                SystemConsole.WriteLine("");
                ShowSupportedCommand();
                Exit(ErrorCode.ErrorBadArguments);
                return;
            }

            string version = args[1];

            var migrator = new VersionedFileMigrator();
            bool isSupported = migrator.IsVersionSupported(version);
            SystemConsole.WriteLine(isSupported
                                        ? "Version '{0}' is supported."
                                        : "Version '{0}' is not supported.", version);
        }

        private static void ShowSupportedCommand()
        {
            ConsoleHelper.WriteInfoLine(Resources.CommandSupported_Command_0_Brief
                                        + "\t"
                                        + Resources.CommandSupported_Detailed, commandVersionSupported);
        }

        #endregion

        #region Migrate Command

        private static void MigrateCommand(string[] args)
        {
            if (args.Length != 4)
            {
                ConsoleHelper.WriteErrorLine(Resources.Command_0_Incorrect_number_of_parameters, commandMigrate);
                SystemConsole.WriteLine("");
                ShowMigrateCommand();
                Exit(ErrorCode.ErrorBadArguments);
                return;
            }

            var migrator = new VersionedFileMigrator();

            string filepath = args[1];
            string toVersion = args[2];
            string toFilepath = args[3];

            var sourceFile = new VersionedFile(filepath);

            try
            {
                migrator.Migrate(sourceFile, toVersion, toFilepath);
            }
            catch (Exception exception)
            {
                ConsoleHelper.WriteErrorLine(exception.Message);
                if (exception is ArgumentException)
                {
                    Exit(ErrorCode.ErrorBadArguments);
                    return;
                }
                Exit(ErrorCode.ErrorBadCommand);
            }
        }

        private static void ShowMigrateCommand()
        {
            ConsoleHelper.WriteInfoLine(Resources.CommandMigrate_Command_0_Brief
                                        + "\t"
                                        + Resources.CommandMigrate_Detailed, commandMigrate);
        }

        #endregion
    }
}