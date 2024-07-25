// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.ObjectModel;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Util;
using Riskeer.Migration.Core;
using Riskeer.Migration.Core.TestUtil;

namespace Riskeer.Migration.Integration.Test
{
    [TestFixture]
    public class MigrationIntegrationTest
    {
        private readonly string latestVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core);

        [Test]
        [TestCaseSource(nameof(GetFileNamesWithSpecialCharacters))]
        public void GivenProject_WhenSpecialCharacterInPath_ThenDoesNotThrowException(string sourceFile,
                                                                                      string newVersion)
        {
            // Given
            string fileToCopy = Path.Combine(testDataPath, sourceFile);
            string sourceFilePath = TestHelper.GetScratchPadPath($"\'[]!`~@#$%^€&()-_=+;, {sourceFile}");
            File.Copy(fileToCopy, sourceFilePath, true);

            // Precondition
            Assert.IsTrue(File.Exists(sourceFilePath));
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string name = $"{nameof(GivenProject_WhenSpecialCharacterInPath_ThenDoesNotThrowException)} \'[]!`~@#$%^€&()-_=+;,";
            string targetFilePath = TestHelper.GetScratchPadPath(name);
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(name, sourceFile, ".log"));
            var migrator = new ProjectFileMigrator
            {
                LogPath = logFilePath
            };

            try
            {
                using (new FileDisposeHelper(logFilePath))
                using (new FileDisposeHelper(targetFilePath))
                {
                    // When
                    void Call() => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                    // Then
                    Assert.DoesNotThrow(Call);
                }
            }
            finally
            {
                File.Delete(sourceFilePath);
            }
        }

        [Test]
        public void GivenEmpty164Project_WhenNoChangesMadeAndMigratingToLatestVersion_ThenLogDatabaseContainsExpectedMessages()
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Migration.Core,
                                                               "Empty valid Release 16.4.rtd");
            var fromVersionedFile = new ProjectVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(
                nameof(GivenEmpty164Project_WhenNoChangesMadeAndMigratingToLatestVersion_ThenLogDatabaseContainsExpectedMessages));
            string logFilePath = TestHelper.GetScratchPadPath(
                string.Concat(nameof(GivenEmpty164Project_WhenNoChangesMadeAndMigratingToLatestVersion_ThenLogDatabaseContainsExpectedMessages), ".log"));
            var migrator = new ProjectFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                // When
                migrator.Migrate(fromVersionedFile, latestVersion, targetFilePath);

                using (var reader = new MigrationLogDatabaseReader(logFilePath))
                {
                    ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();
                    Assert.AreEqual(20, messages.Count);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("5", "17.1", "Gevolgen van de migratie van versie 16.4 naar versie 17.1:"),
                        messages[0]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("5", "17.1", "* Geen aanpassingen."),
                        messages[1]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "Gevolgen van de migratie van versie 17.1 naar versie 17.2:"),
                        messages[2]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.1", "17.2", "* Geen aanpassingen."),
                        messages[3]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.2", "17.3", "Gevolgen van de migratie van versie 17.2 naar versie 17.3:"),
                        messages[4]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.2", "17.3", "* Geen aanpassingen."),
                        messages[5]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.3", "18.1", "Gevolgen van de migratie van versie 17.3 naar versie 18.1:"),
                        messages[6]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("17.3", "18.1", "* Geen aanpassingen."),
                        messages[7]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("18.1", "19.1", "Gevolgen van de migratie van versie 18.1 naar versie 19.1:"),
                        messages[8]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("18.1", "19.1", "* Geen aanpassingen."),
                        messages[9]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("19.1", "21.1", "Gevolgen van de migratie van versie 19.1 naar versie 21.1:"),
                        messages[10]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("19.1", "21.1", "* Geen aanpassingen."),
                        messages[11]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("21.1", "22.1", "Gevolgen van de migratie van versie 21.1 naar versie 22.1:"),
                        messages[12]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("21.1", "22.1", $"* De oorspronkelijke faalmechanismen zijn omgezet naar het nieuwe formaat.{Environment.NewLine}* Alle toetsoordelen zijn verwijderd."),
                        messages[13]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("22.1", "23.1", "Gevolgen van de migratie van versie 22.1 naar versie 23.1:"),
                        messages[14]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("22.1", "23.1", "* Geen aanpassingen."),
                        messages[15]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("23.1", "24.1", "Gevolgen van de migratie van versie 23.1 naar versie 24.1:"),
                        messages[16]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("23.1", "24.1",
                                                $"* Omdat alleen faalkansen op vakniveau een rol spelen in de assemblage, zijn de assemblageresultaten voor de faalmechanismen aangepast:{Environment.NewLine}" +
                                                $"  + De initiële faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Handmatig invullen'.{Environment.NewLine}" +
                                                $"  + De aangescherpte faalkansen per doorsnede zijn verwijderd in het geval van de optie 'Per doorsnede' of 'Beide'.{Environment.NewLine}" +
                                                $"  + De assemblagemethode 'Automatisch berekenen o.b.v. slechtste doorsnede of vak' is vervangen door 'Automatisch berekenen o.b.v. slechtste vak'.{Environment.NewLine}" +
                                                "* Voor HLCD bestanden waarbij geen tabel 'ScenarioInformation' aanwezig is, worden niet langer standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie."),
                        messages[17]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("24.1", $"{latestVersion}", $"Gevolgen van de migratie van versie 24.1 naar versie {latestVersion}:"),
                        messages[18]);
                    MigrationLogTestHelper.AssertMigrationLogMessageEqual(
                        new MigrationLogMessage("24.1", $"{latestVersion}", "* Geen aanpassingen."),
                        messages[19]);
                }
            }
        }

        private static IEnumerable<TestCaseData> GetFileNamesWithSpecialCharacters()
        {
            foreach (FileToMigrate fileToMigrate in GetFilesToMigrate())
            {
                yield return new TestCaseData(fileToMigrate.OriginalPath, fileToMigrate.ToVersion)
                    .SetName($"Migrate{fileToMigrate.OriginalPath}WithSpecialChars");
            }
        }

        private static IEnumerable<FileToMigrate> GetFilesToMigrate()
        {
            yield return new FileToMigrate("Empty valid Release 16.4.rtd", "17.1");
            yield return new FileToMigrate("Empty valid Release 17.1.rtd", "17.2");
            yield return new FileToMigrate("Empty valid Release 17.2.rtd", "17.3");
            yield return new FileToMigrate("Empty valid Release 17.3.rtd", "18.1");
            yield return new FileToMigrate("Empty valid Release 18.1.rtd", "19.1");
            yield return new FileToMigrate("Empty valid Release 19.1.risk", "21.1");
            yield return new FileToMigrate("Empty valid Release 21.1.risk", "22.1");
            yield return new FileToMigrate("Empty valid Release 22.1.risk", "23.1");
            yield return new FileToMigrate("Empty valid Release 23.1.risk", "24.1");
            yield return new FileToMigrate("Empty valid Release 24.1.risk", "24.2");
        }

        private class FileToMigrate
        {
            public FileToMigrate(string path, string toVersion)
            {
                OriginalPath = path;
                ToVersion = toVersion;
            }

            public string OriginalPath { get; }
            public string ToVersion { get; }
        }
    }
}