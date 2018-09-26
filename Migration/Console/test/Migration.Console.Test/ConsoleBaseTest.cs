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
        public void DisplayCommands_WritesNoCommandsToConsole()
        {
            // Setup
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                consoleBase.WriteDisplayCommands();

                // Assert
                string consoleText = consoleOutput.GetConsoleOutput();
                Assert.IsEmpty(consoleText);
                Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
            }
        }

        [Test]
        public void ExecuteConsoleTool_ArgsNull_WritesAllCommandsToConsole()
        {
            // Setup
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                consoleBase.SimpleExecuteConsoleTool(null);

                // Assert
                string consoleText = consoleOutput.GetConsoleOutput();
                string expectedText = Environment.NewLine + GetConsoleFullDescription();
                Assert.AreEqual(expectedText, consoleText);
                Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
            }
        }

        [Test]
        [TestCase("-h")]
        [TestCase("--help")]
        public void ExecuteConsoleTool_ArgsIsHelp_WritesAllCommandsToConsole(string command)
        {
            // Setup
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                consoleBase.SimpleExecuteConsoleTool(new[]
                {
                    command
                });

                // Assert
                string consoleText = consoleOutput.GetConsoleOutput();
                string expectedText = Environment.NewLine + GetConsoleFullDescription();
                Assert.AreEqual(expectedText, consoleText);
                Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
            }
        }

        [Test]
        public void ExecuteConsoleTool_ArgsIsInvalid_CallsExecuteCommand()
        {
            // Setup
            const string command = "invalid command";
            string[] commandArgs =
            {
                command
            };
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                consoleBase.SimpleExecuteConsoleTool(commandArgs);

                // Assert
                Assert.AreEqual(Environment.NewLine, consoleOutput.GetConsoleOutput());
                Assert.AreEqual(commandArgs, consoleBase.ExecuteCommandArguments);
                Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
            }
        }

        [Test]
        public void ExecuteConsoleTool_ThrowsExceptionWithoutInnerException_WritesErrorWithErrorCode()
        {
            // Setup
            const string command = "invalid command";
            const string exceptionMessage = "I was told to be thrown.";
            string[] commandArgs =
            {
                command
            };
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            using (var consoleOutput = new ConsoleOutput())
            {
                consoleBase.ExceptionToBeThrown = new CriticalMigrationException(exceptionMessage);

                // Call
                consoleBase.SimpleExecuteConsoleTool(commandArgs);

                // Assert
                string expectedtext = Environment.NewLine + exceptionMessage + Environment.NewLine
                                      + Environment.NewLine + GetConsoleFullDescription();
                string consoleText = consoleOutput.GetConsoleOutput();
                Assert.AreEqual(expectedtext, consoleText);

                Assert.AreEqual(commandArgs, consoleBase.ExecuteCommandArguments);
                Assert.AreEqual(ErrorCode.ErrorBadCommand, environmentControl.ErrorCodeCalled);
            }
        }

        [Test]
        public void ExecuteConsoleTool_ThrowsExceptionWithInnerException_WritesErrorsWithErrorCode()
        {
            // Setup
            const string command = "invalid command";
            const string exceptionMessage = "I was told to be thrown.";
            const string innerExceptionMessage = "inner exception.";

            string[] commandArgs =
            {
                command
            };
            var consoleBase = new SimpleConsoleBase(applicationName, applicationDescription);

            using (var consoleOutput = new ConsoleOutput())
            {
                consoleBase.ExceptionToBeThrown = new Exception(exceptionMessage, new Exception(innerExceptionMessage));

                // Call
                consoleBase.SimpleExecuteConsoleTool(commandArgs);

                // Assert
                string expectedtext = Environment.NewLine + exceptionMessage + Environment.NewLine
                                      + "Het besturingssysteem geeft de volgende melding: " + Environment.NewLine
                                      + $"{innerExceptionMessage}"
                                      + Environment.NewLine + Environment.NewLine
                                      + GetConsoleFullDescription();
                string consoleText = consoleOutput.GetConsoleOutput();
                Assert.AreEqual(expectedtext, consoleText);

                Assert.AreEqual(commandArgs, consoleBase.ExecuteCommandArguments);
                Assert.AreEqual(ErrorCode.ErrorInvalidCommandLine, environmentControl.ErrorCodeCalled);
            }
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

            public void WriteDisplayCommands()
            {
                DisplayCommands();
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