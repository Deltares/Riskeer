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

using System.Linq;
using Migration.Console.Properties;
using SystemConsole = System.Console;

namespace Migration.Console
{
    public static class RingtoetsMigrationTool
    {
        private const string commandMigrate = "--migrate";
        private const string commandVersionSupported = "--supported";
        private const string commandHelp = "--help";

        public static void Main(string[] args)
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
                {
                    InvalidCommand(command);
                    DisplayAllCommands();
                    break;
                }
            }
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
                return;
            }
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
            if (args.Length != 3)
            {
                ConsoleHelper.WriteErrorLine(Resources.Command_0_Incorrect_number_of_parameters, commandMigrate);
                SystemConsole.WriteLine("");
                ShowMigrateCommand();
                return;
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