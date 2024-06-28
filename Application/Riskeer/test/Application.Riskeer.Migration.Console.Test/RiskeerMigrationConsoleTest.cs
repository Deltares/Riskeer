﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.TestUtil;
using Migration.Console;
using Migration.Console.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Util;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Application.Riskeer.Migration.Console.Test
{
    [TestFixture]
    [Explicit("Migration Console tests are disabled.")]
    public class RiskeerMigrationConsoleTest
    {
        private TestEnvironmentControl environmentControl;

        [SetUp]
        public void SetUp()
        {
            environmentControl = new TestEnvironmentControl();
            EnvironmentControl.Instance = environmentControl;
        }

        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var console = new RiskeerMigrationConsole();

            // Assert
            Assert.IsInstanceOf<ConsoleBase>(console);
        }

        [Test]
        public void ExecuteConsoleTool_NoArguments_WritesHelpToConsole()
        {
            // Setup
            var console = new RiskeerMigrationConsole();

            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                console.ExecuteConsoleTool(new string[]
                                               {});

                // Assert
                string expectedText = Environment.NewLine + GetConsoleFullDescription();
                string consoleText = consoleOutput.GetConsoleOutput();
                Assert.AreEqual(expectedText, consoleText);
                Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
            }
        }

        [Test]
        public void ExecuteConsoleTool_InvalidArguments_WritesHelpToConsoleWithErrorCode()
        {
            // Setup
            var console = new RiskeerMigrationConsole();
            string[] invalidCommand =
            {
                "0",
                "1",
                "2"
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                console.ExecuteConsoleTool(invalidCommand);

                // Assert
                string expectedText = Environment.NewLine
                                      + $"{string.Join(" ", invalidCommand)} is geen geldige opdracht."
                                      + Environment.NewLine + Environment.NewLine
                                      + GetConsoleFullDescription();
                string consoleText = consoleOutput.GetConsoleOutput();
                Assert.AreEqual(expectedText, consoleText);
                Assert.AreEqual(ErrorCode.ErrorBadCommand, environmentControl.ErrorCodeCalled);
            }
        }

        [Test]
        public void ExecuteConsoleTool_InvalidArgumentsForMigrate_WritesHelpToConsoleWithErrorCode()
        {
            // Setup
            var console = new RiskeerMigrationConsole();
            string[] invalidCommand =
            {
                "",
                ""
            };

            using (var consoleOutput = new ConsoleOutput())
            {
                // Call
                console.ExecuteConsoleTool(invalidCommand);

                // Assert
                string expectedText = Environment.NewLine
                                      + "Bron- en doelprojectpad moeten geldige bestandspaden zijn."
                                      + Environment.NewLine + Environment.NewLine
                                      + GetConsoleFullDescription();
                string consoleText = consoleOutput.GetConsoleOutput();
                Assert.AreEqual(expectedText, consoleText);
                Assert.AreEqual(ErrorCode.ErrorInvalidCommandLine, environmentControl.ErrorCodeCalled);
            }
        }

        [Test]
        [TestCaseSource(nameof(ProjectFilesToMigrate))]
        public void GivenConsole_WhenVersionSupportedCall_ThenReturnedIfSupported(string file, string fileVersion, bool isSupported)
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core, file);
            var console = new RiskeerMigrationConsole();
            string expectedVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();

            using (var consoleOutput = new ConsoleOutput())
            {
                // When
                console.ExecuteConsoleTool(new[]
                {
                    sourceFilePath
                });

                // Then
                string consoleText = consoleOutput.GetConsoleOutput();
                string expectedText = isSupported
                                          ? Environment.NewLine
                                            + $@"Het projectbestand kan gemigreerd worden naar versie '{expectedVersion}'."
                                            + Environment.NewLine
                                          : Environment.NewLine
                                            + $"Het migreren van een projectbestand met versie '{fileVersion}' naar versie '{expectedVersion}' is niet ondersteund."
                                            + Environment.NewLine;

                Assert.AreEqual(expectedText, consoleText);
                Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
            }
        }

        [Test]
        public void GivenConsole_WhenMigrateCalledWithArguments_ThenMigratesToNewVersion()
        {
            // Given
            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            string targetFilePath = TestHelper.GetScratchPadPath($"{nameof(RiskeerMigrationConsoleTest)}.{nameof(GivenConsole_WhenMigrateCalledWithArguments_ThenMigratesToNewVersion)}");
            var console = new RiskeerMigrationConsole();
            string expectedVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();

            using (new FileDisposeHelper(targetFilePath))
            {
                using (var consoleOutput = new ConsoleOutput())
                {
                    // When
                    console.ExecuteConsoleTool(new[]
                    {
                        sourceFilePath,
                        targetFilePath
                    });

                    // Then
                    string expected = Environment.NewLine
                                      + $"Het projectbestand '{sourceFilePath}' is succesvol gemigreerd naar '{targetFilePath}' (versie {expectedVersion})."
                                      + Environment.NewLine;
                    string consoleText = consoleOutput.GetConsoleOutput();
                    Assert.AreEqual(expected, consoleText);

                    var toVersionedFile = new ProjectVersionedFile(targetFilePath);
                    Assert.AreEqual(expectedVersion, toVersionedFile.GetVersion());
                }
            }

            Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void GivenConsole_WhenMigrateCalledUnableToSaveTarget_ThenExitWithErrorCode()
        {
            // Given
            string sourceFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            string targetFilePath = TestHelper.GetScratchPadPath($"{nameof(RiskeerMigrationConsoleTest)}.{nameof(GivenConsole_WhenMigrateCalledUnableToSaveTarget_ThenExitWithErrorCode)}");

            var console = new RiskeerMigrationConsole();

            using (var fileDisposeHelper = new FileDisposeHelper(targetFilePath))
            using (var consoleOutput = new ConsoleOutput())
            {
                fileDisposeHelper.LockFiles();

                // When
                console.ExecuteConsoleTool(new[]
                {
                    sourceFilePath,
                    targetFilePath
                });

                // Then
                string consoleText = consoleOutput.GetConsoleOutput();
                StringAssert.StartsWith(Environment.NewLine + "Het gemigreerde projectbestand is aangemaakt op '",
                                        consoleText);
                StringAssert.EndsWith($"', maar er is een onverwachte fout opgetreden tijdens het verplaatsen naar '{targetFilePath}'."
                                      + Environment.NewLine
                                      + "Het besturingssysteem geeft de volgende melding: "
                                      + Environment.NewLine
                                      + $"The process cannot access the file '{targetFilePath}' because it is being used by another process."
                                      + Environment.NewLine + Environment.NewLine
                                      + GetConsoleFullDescription(), consoleText);
                Assert.AreEqual(ErrorCode.ErrorBadCommand, environmentControl.ErrorCodeCalled);
            }
        }

        private static string GetConsoleFullDescription()
        {
            string currentDatabaseVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();
            return "Dit hulpprogramma kan worden gebruikt om een projectbestand in het formaat van een "
                   + $"eerdere versie van Riskeer of Ringtoets projectbestand te migreren naar het formaat van de huidige versie van Riskeer ({currentDatabaseVersion})."
                   + Environment.NewLine + Environment.NewLine
                   + "MIGRATIEHULPPROGRAMMA -h" + Environment.NewLine
                   + "MIGRATIEHULPPROGRAMMA --help" + Environment.NewLine
                   + "          Geeft deze informatie weer." + Environment.NewLine + Environment.NewLine
                   + "MIGRATIEHULPPROGRAMMA bronprojectpad" + Environment.NewLine
                   + "          Controleert of het projectbestand dat te vinden is in het bronproject"
                   + Environment.NewLine
                   + "          pad gemigreerd kan worden." + Environment.NewLine + Environment.NewLine
                   + "MIGRATIEHULPPROGRAMMA bronprojectpad doelprojectpad" + Environment.NewLine
                   + "          Voert de migratie uit van het projectbestand dat te vinden is in het "
                   + Environment.NewLine
                   + "          bronprojectpad en slaat het resulterende projectbestand op in het doe"
                   + Environment.NewLine
                   + "          lprojectpad." + Environment.NewLine + Environment.NewLine;
        }

        #region Test cases

        private static IEnumerable<TestCaseData> ProjectFilesToMigrate()
        {
            string unsupportedProjectFilePath = ProjectMigrationTestHelper.GetOutdatedUnSupportedProjectFilePath();
            var unsupportedVersionedFile = new ProjectVersionedFile(unsupportedProjectFilePath);
            string unsupportedVersion = unsupportedVersionedFile.GetVersion();

            string supportedProjectFilePath = ProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var supportedVersionedFile = new ProjectVersionedFile(supportedProjectFilePath);
            string supportedVersion = supportedVersionedFile.GetVersion();

            yield return new TestCaseData(unsupportedProjectFilePath, unsupportedVersion, false).SetName("UnsupportedProjectVersion");
            yield return new TestCaseData(supportedProjectFilePath, supportedVersion, true).SetName("SupportedProjectVersion");
        }

        #endregion
    }
}