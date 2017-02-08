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
using System.IO;
using Application.Ringtoets.Migration;
using Core.Common.TestUtil;
using Migration.Console;
using Migration.Console.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Utils;

namespace Application.Ringtoets.MigrationConsole.Test
{
    [TestFixture]
    public class RingtoetsMigrationToolTest
    {
        private TestEnvironmentControl environmentControl;

        [SetUp]
        public void SetUp()
        {
            environmentControl = new TestEnvironmentControl();
            EnvironmentControl.Instance = environmentControl;
        }

        [Test]
        public void Main_NoArguments_WritesHelpToConsole()
        {
            // Setup
            string consoleText;

            // Call
            using (var consoleOutput = new ConsoleOutput())
            {
                RingtoetsMigrationTool.Main(new string[]
                                                {});

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            var expectedText = GetConsoleFullDescription();
            Assert.AreEqual(expectedText, consoleText);
            Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void Main_InvalidArguments_WritesHelpToConsoleWithErrorCode()
        {
            // Setup
            string[] invalidCommand =
            {
                "0",
                "1",
                "2"
            };
            string consoleText;

            // Call
            using (var consoleOutput = new ConsoleOutput())
            {
                RingtoetsMigrationTool.Main(invalidCommand);

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            var expectedText = $"{string.Join(" ", invalidCommand)} is geen geldige opdracht."
                               + Environment.NewLine + Environment.NewLine
                               + GetConsoleFullDescription();
            Assert.AreEqual(expectedText, consoleText);
            Assert.AreEqual(ErrorCode.ErrorBadCommand, environmentControl.ErrorCodeCalled);
        }

        [Test]
        [TestCase("FullTestProject164.rtd", true)]
        [TestCase("UnsupportedVersion8.rtd", false)]
        public void GivenConsole_WhenVersionSupportedCall_ThenReturnedIfSupported(string file, bool isSupported)
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, file);
            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                // When
                RingtoetsMigrationTool.Main(new[]
                {
                    sourceFilePath
                });

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Then
            Assert.AreEqual($@"Het projectbestand wordt {(isSupported ? "" : "niet ")}ondersteund."
                            + Environment.NewLine, consoleText);
            Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void GivenConsole_WhenMigrateCalledWithArguments_MigratesToNewVersion()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, Path.GetRandomFileName());

            using (new FileDisposeHelper(targetFilePath))
            {
                string consoleText;
                using (var consoleOutput = new ConsoleOutput())
                {
                    // When
                    RingtoetsMigrationTool.Main(new[]
                    {
                        sourceFilePath,
                        targetFilePath
                    });

                    consoleText = consoleOutput.GetConsoleOutput();
                }

                // Then
                var expected = $"Het bestand '{sourceFilePath}' is succesvol gemigreerd naar '{targetFilePath}'."
                               + Environment.NewLine;
                Assert.AreEqual(expected, consoleText);
                var toVersionedFile = new RingtoetsVersionedFile(targetFilePath);
                string expectedVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
                Assert.AreEqual(expectedVersion, toVersionedFile.GetVersion());
            }
            Assert.AreEqual(ErrorCode.ErrorSuccess, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void GivenConsole_WhenMigrateCalledUnableToSaveTarget_ThenExitWithErrorCode()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, Path.GetRandomFileName());

            string consoleText;
            using (var fileDisposeHelper = new FileDisposeHelper(targetFilePath))
            using (var consoleOutput = new ConsoleOutput())
            {
                fileDisposeHelper.LockFiles();

                // When
                RingtoetsMigrationTool.Main(new[]
                {
                    sourceFilePath,
                    targetFilePath
                });

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Then
            StringAssert.StartsWith("Er is een onverwachte fout opgetreden tijdens het verplaatsen van het gemigreerde bestand '",
                                    consoleText);
            StringAssert.EndsWith($"' naar '{targetFilePath}'." + Environment.NewLine
                                  + "Het besturingssysteem geeft de volgende melding: "
                                  + $"The process cannot access the file '{targetFilePath}' because it is being used by another process."
                                  + Environment.NewLine + Environment.NewLine
                                  + GetConsoleFullDescription(), consoleText);
            Assert.AreEqual(ErrorCode.ErrorBadCommand, environmentControl.ErrorCodeCalled);
        }

        private static string GetConsoleFullDescription()
        {
            return "Dit hulpprogramma kan worden gebruikt om een projectbestand in het formaat van een "
                   + "eerdere versie van Ringtoets te migreren naar het formaat van de huidige versie van Ringtoets."
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
                   + "          lprojectpad."
                   + Environment.NewLine + Environment.NewLine;
        }
    }
}