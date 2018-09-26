// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using Migration.Console.Properties;
using Migration.Scripts.Data.Exceptions;
using SystemConsole = System.Console;

namespace Migration.Console
{
    /// <summary>
    /// Base console application.
    /// </summary>
    public abstract class ConsoleBase
    {
        private const string commandHelp = "--help";
        private const string commandHelpShort = "-h";
        private readonly string applicationName;
        private readonly string applicationDescription;

        /// <summary>
        /// Creates a new instance of <see cref="ConsoleBase"/>.
        /// </summary>
        /// <param name="applicationName">The name of the application as it can be called in the command line.</param>
        /// <param name="applicationDescription">The description of the application.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        protected ConsoleBase(string applicationName, string applicationDescription)
        {
            if (applicationName == null)
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            if (applicationDescription == null)
            {
                throw new ArgumentNullException(nameof(applicationDescription));
            }

            this.applicationName = applicationName;
            this.applicationDescription = applicationDescription;
        }

        /// <summary>
        /// Executes a command based upon the arguments provided.
        /// </summary>
        /// <param name="args">The arguments that determine which command to execute.</param>
        /// <remarks>By default, the help command is executed.</remarks>
        public void ExecuteConsoleTool(string[] args)
        {
            SystemConsole.WriteLine();
            try
            {
                ParseCommand(args);
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

        /// <summary>
        /// Writes all other commands to the <see cref="SystemConsole"/>.
        /// </summary>
        protected virtual void DisplayCommands() {}

        /// <summary>
        /// Executes a command based upon the arguments provided.
        /// </summary>
        /// <param name="args">The arguments that determine which command to execute.</param>
        /// <exception cref="Exception">Thrown when the command failed.</exception>
        protected abstract void ExecuteCommand(string[] args);

        /// <summary>
        /// Writes all commands to the <see cref="SystemConsole"/>.
        /// </summary>
        private void DisplayAllCommands()
        {
            SystemConsole.WriteLine(applicationDescription);
            SystemConsole.WriteLine();
            SystemConsole.WriteLine(string.Concat(applicationName, " ", commandHelpShort));
            SystemConsole.WriteLine(string.Concat(applicationName, " ", commandHelp));
            ConsoleHelper.WriteCommandDescriptionLine(Resources.CommandHelp_Detailed);
            DisplayCommands();
        }

        private void ParseCommand(string[] args)
        {
            string command = args?.FirstOrDefault() ?? commandHelp;
            if (command.Equals(commandHelp) || command.Equals(commandHelpShort))
            {
                DisplayAllCommands();
                return;
            }

            ExecuteCommand(args);
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
                ConsoleHelper.WriteErrorLine(Resources.Message_Inner_Exception_0, exception.InnerException.Message);
            }

            SystemConsole.WriteLine();
        }
    }
}