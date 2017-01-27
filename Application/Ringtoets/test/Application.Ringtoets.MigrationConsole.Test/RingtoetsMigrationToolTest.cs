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

        [TearDown]
        public void TearDown()
        {
            EnvironmentControl.ResetToDefault();
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
            Assert.AreEqual("--help\tGeef het hulp menu weer."
                            + Environment.NewLine
                            + "--migrate RINGTOETSBESTANDSPAD NIEUWEVERSIE UITVOERPAD"
                            + "\t"
                            + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand dat gemigreerd moet worden. "
                            + "NIEUWEVERSIE is de versie naar waar gemigreerd moet worden. "
                            + "UITVOERPAD is het pad waar de het gemigreerde Ringtoetsbestand opgeslagen zal worden."
                            + Environment.NewLine
                            + "--supported RINGTOETSBESTANDSPAD\t"
                            + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand waarvan de versie gevalideerd moet worden."
                            + Environment.NewLine
                            , consoleText);
            Assert.AreEqual(0, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void Main_InvalidArguments_WritesHelpToConsoleWithErrorCode()
        {
            // Setup
            const string invalidCommand = "ThisIsNoCommand";
            string consoleText;

            // Call
            using (var consoleOutput = new ConsoleOutput())
            {
                RingtoetsMigrationTool.Main(new[]
                {
                    invalidCommand
                });

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            Assert.AreEqual($"{invalidCommand} is geen geldige opdracht."
                            + Environment.NewLine + Environment.NewLine
                            + "--help\tGeef het hulp menu weer."
                            + Environment.NewLine
                            + "--migrate RINGTOETSBESTANDSPAD NIEUWEVERSIE UITVOERPAD"
                            + "\t"
                            + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand dat gemigreerd moet worden. "
                            + "NIEUWEVERSIE is de versie naar waar gemigreerd moet worden. "
                            + "UITVOERPAD is het pad waar de het gemigreerde Ringtoetsbestand opgeslagen zal worden."
                            + Environment.NewLine
                            + "--supported RINGTOETSBESTANDSPAD\t"
                            + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand waarvan de versie gevalideerd moet worden."
                            + Environment.NewLine
                            , consoleText);
            Assert.AreEqual(22, environmentControl.ErrorCodeCalled);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        public void GivenConsole_WhenSupportedCalledWithInvalidArguments_ExitWitErrorCode(int numberOfArguments)
        {
            const string supportedCommand = "--supported";
            string[] arguments = new string[numberOfArguments];
            arguments[0] = supportedCommand;

            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                // Event
                RingtoetsMigrationTool.Main(new[]
                {
                    supportedCommand
                });

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Result
            Assert.AreEqual($"Er is een verkeerd aantal parameters opgegeven voor de opdracht '{supportedCommand}'"
                            + Environment.NewLine + Environment.NewLine
                            + "--help\tGeef het hulp menu weer."
                            + Environment.NewLine
                            + "--migrate RINGTOETSBESTANDSPAD NIEUWEVERSIE UITVOERPAD"
                            + "\t"
                            + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand dat gemigreerd moet worden. "
                            + "NIEUWEVERSIE is de versie naar waar gemigreerd moet worden. "
                            + "UITVOERPAD is het pad waar de het gemigreerde Ringtoetsbestand opgeslagen zal worden."
                            + Environment.NewLine
                            + "--supported RINGTOETSBESTANDSPAD\t"
                            + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand waarvan de versie gevalideerd moet worden."
                            + Environment.NewLine
                            , consoleText);
            Assert.AreEqual(160, environmentControl.ErrorCodeCalled);
        }

        [Test]
        [TestCase("4", true)]
        [TestCase("3", false)]
        public void GivenConsole_WhenVersionSupportedCall_ThenReturnedIfSupported(string version, bool isSupported)
        {
            // Scenario
            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                // Event
                RingtoetsMigrationTool.Main(new[]
                {
                    "--supported",
                    version
                });

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Result
            Assert.AreEqual(isSupported
                                ? $"Version '{version}' is supported." + Environment.NewLine
                                : $"Version '{version}' is not supported." + Environment.NewLine,
                            consoleText);
            Assert.AreEqual(0, environmentControl.ErrorCodeCalled);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        public void GivenConsole_WhenMigrateCalledWithInvalidArguments_ExitWitErrorCode(int numberOfArguments)
        {
            // Scenario
            const string migrateCommand = "--migrate";
            string[] arguments = new string[numberOfArguments];
            arguments[0] = migrateCommand;

            string consoleText;
            using (var consoleOutput = new ConsoleOutput())
            {
                // Event
                RingtoetsMigrationTool.Main(arguments);

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Result
            Assert.AreEqual($"Er is een verkeerd aantal parameters opgegeven voor de opdracht '{migrateCommand}'"
                            + Environment.NewLine + Environment.NewLine
                            + "--help\tGeef het hulp menu weer."
                            + Environment.NewLine
                            + "--migrate RINGTOETSBESTANDSPAD NIEUWEVERSIE UITVOERPAD"
                            + "\t"
                            + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand dat gemigreerd moet worden. "
                            + "NIEUWEVERSIE is de versie naar waar gemigreerd moet worden. "
                            + "UITVOERPAD is het pad waar de het gemigreerde Ringtoetsbestand opgeslagen zal worden."
                            + Environment.NewLine
                            + "--supported RINGTOETSBESTANDSPAD\t"
                            + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand waarvan de versie gevalideerd moet worden."
                            + Environment.NewLine, consoleText);
            Assert.AreEqual(160, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void GivenConsole_WhenMigrateCalledWithArguments_MigratesToNewVersion()
        {
            // Scenario
            const string migrateCommand = "--migrate";

            const string newVersion = "17.1";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, Path.GetRandomFileName());

            using (new FileDisposeHelper(targetFilePath))
            {
                string consoleText;
                using (var consoleOutput = new ConsoleOutput())
                {
                    // Event
                    RingtoetsMigrationTool.Main(new[]
                    {
                        migrateCommand,
                        sourceFilePath,
                        newVersion,
                        targetFilePath
                    });

                    consoleText = consoleOutput.GetConsoleOutput();
                }

                // Result
                Assert.AreEqual($"Het bestand '{sourceFilePath}' is succesvol gemigreerd naar versie"
                                + $" {newVersion} op locatie '{targetFilePath}'." + Environment.NewLine,
                                consoleText);
                var toVersionedFile = new RingtoetsVersionedFile(targetFilePath);
                Assert.AreEqual(newVersion, toVersionedFile.GetVersion());
            }
            Assert.AreEqual(0, environmentControl.ErrorCodeCalled);
        }

        [Test]
        public void GivenConsole_WhenMigrateCalledUnableToSaveTarget_ThenExitWithErrorCode()
        {
            // Scenario
            const string migrateCommand = "--migrate";

            const string newVersion = "17.1";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, Path.GetRandomFileName());

            string consoleText;
            using (new FileDisposeHelper(targetFilePath))
            using (File.Create(targetFilePath))
            using (var consoleOutput = new ConsoleOutput())
            {
                // Event
                RingtoetsMigrationTool.Main(new[]
                {
                    migrateCommand,
                    sourceFilePath,
                    newVersion,
                    targetFilePath
                });

                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Result
            Assert.That(consoleText.StartsWith("Er is een onverwachte fout opgetreden tijdens het verplaatsen van het gemigreerde bestand '"));
            Assert.That(consoleText.EndsWith($"' naar '{targetFilePath}'." + Environment.NewLine
                                             + "Het besturingssysteem geeft de volgende melding: "
                                             + $"The process cannot access the file '{targetFilePath}' because it is being used by another process."
                                             + Environment.NewLine + Environment.NewLine
                                             + "--help\tGeef het hulp menu weer."
                                             + Environment.NewLine
                                             + "--migrate RINGTOETSBESTANDSPAD NIEUWEVERSIE UITVOERPAD"
                                             + "\t"
                                             + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand dat gemigreerd moet worden. "
                                             + "NIEUWEVERSIE is de versie naar waar gemigreerd moet worden. "
                                             + "UITVOERPAD is het pad waar de het gemigreerde Ringtoetsbestand opgeslagen zal worden."
                                             + Environment.NewLine
                                             + "--supported RINGTOETSBESTANDSPAD\t"
                                             + "RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand waarvan de versie gevalideerd moet worden."
                                             + Environment.NewLine));
            Assert.AreEqual(22, environmentControl.ErrorCodeCalled);
        }
    }
}