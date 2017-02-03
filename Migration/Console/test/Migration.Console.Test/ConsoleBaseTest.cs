// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Migration.Console.TestUtil;
using Migration.Scripts.Data.Exceptions;
using NUnit.Framework;

namespace Migration.Console.Test
{
    [TestFixture]
    public class ConsoleBaseTest
    {
        private const string applicationName = "name";
        private const string applicationDescription = "applicationDescription";
        private TestEnvironmentControl environmentControl;

        [SetUp]
        public void SetUp()
        {
            environmentControl = new TestEnvironmentControl();
            EnvironmentControl.Instance = environmentControl;
        }

        [Test]
        public void Constructor_ApplicationNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleConsoleBase(null, applicationDescription);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("applicationName", paramName);
        }

        [Test]
        public void Constructor_ApplicationDescriptionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleConsoleBase(applicationName, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("applicationDescription", paramName);
        }

        [Test]
        public void DisplayAllCommands_WritesAllCommandsToConsole()
        {
            // Setup
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                consoleBase.WriteDisplayAllCommands();

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            Assert.AreEqual(GetConsoleFullDescription(), consoleText);
            Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void ExecuteConsoleTool_ArgsNull_WritesAllCommandsToConsole()
        {
            // Setup
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                consoleBase.SimpleExecuteConsoleTool(null);

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            Assert.AreEqual(GetConsoleFullDescription(), consoleText);
            Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
        }

        [Test]
        [TestCase("-h")]
        [TestCase("--help")]
        public void ExecuteConsoleTool_ArgsIsHelp_WritesAllCommandsToConsole(string command)
        {
            // Setup
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                consoleBase.SimpleExecuteConsoleTool(new[]
                {
                    command
                });

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            Assert.AreEqual(GetConsoleFullDescription(), consoleText);
            Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void ExecuteConsoleTool_ArgsIsInvalid_CallsExecuteCommand()
        {
            // Setup
            const string command = "invalid command";
            var commandArgs = new[]
            {
                command
            };
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                // Call

                consoleBase.SimpleExecuteConsoleTool(commandArgs);

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            Assert.AreEqual("", consoleText);
            Assert.AreEqual(commandArgs, consoleBase.ExecuteCommandArguments);
            Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void ExecuteConsoleTool_BadCommand_WritesErrorWithErrorCode()
        {
            // Setup
            const string command = "invalid command";
            var commandArgs = new[]
            {
                command
            };
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                consoleBase.ExceptionToBeThrown = new CriticalMigrationException("I was told to be thrown.");

                // Call
                consoleBase.SimpleExecuteConsoleTool(commandArgs);

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            string expectedtext = "I was told to be thrown." + Environment.NewLine
                                  + Environment.NewLine + GetConsoleFullDescription();
            Assert.AreEqual(expectedtext, consoleText);
            Assert.AreEqual(commandArgs, consoleBase.ExecuteCommandArguments);
            Assert.AreEqual(ErrorCode.ErrorBadCommand, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void ExecuteConsoleTool_ThrowsException_WritesErrorWithErrorCode()
        {
            // Setup
            const string command = "invalid command";
            var commandArgs = new[]
            {
                command
            };
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                consoleBase.ExceptionToBeThrown = new Exception("I was told to be thrown.", new Exception("inner exception."));

                // Call
                consoleBase.SimpleExecuteConsoleTool(commandArgs);

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            string expectedtext = "I was told to be thrown." + Environment.NewLine
                                  + "Het besturingssysteem geeft de volgende melding: inner exception."
                                  + Environment.NewLine + Environment.NewLine
                                  + GetConsoleFullDescription();
            Assert.AreEqual(expectedtext, consoleText);
            Assert.AreEqual(commandArgs, consoleBase.ExecuteCommandArguments);
            Assert.AreEqual(ErrorCode.ErrorInvalidCommandLine, environmentControl.ErrorCodeCalled);
        }

        private static string GetConsoleFullDescription()
        {
            return applicationDescription
                   + Environment.NewLine + Environment.NewLine
                   + $"{applicationName} -h" + Environment.NewLine
                   + $"{applicationName} --help" + Environment.NewLine
                   + "          Geeft deze informatie weer." + Environment.NewLine + Environment.NewLine;
        }

        private class SimpleConsoleBase : ConsoleBase
        {
            public SimpleConsoleBase(string applicationName, string applicationDescription)
                : base(applicationName, applicationDescription) {}

            public Exception ExceptionToBeThrown { private get; set; }

            public string[] ExecuteCommandArguments { get; private set; }

            public void SimpleExecuteConsoleTool(string[] args)
            {
                ExecuteConsoleTool(args);
            }

            public void WriteDisplayAllCommands()
            {
                DisplayAllCommands();
            }

            protected override void ExecuteCommand(string[] args)
            {
                ExecuteCommandArguments = args;
                if (ExceptionToBeThrown != null)
                {
                    throw ExceptionToBeThrown;
                }
            }
        }
    }
}