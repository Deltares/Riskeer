// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.IO;
using Application.Ringtoets.Migration.Core;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    [TestFixture]
    public class MigrationIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core);

        [Test]
        [TestCaseSource(nameof(GetFileNamesWithSpecialCharacters))]
        public void GivenProject_WhenSpecialCharacterInPath_DoesNotThrowException(char specialCharacter,
                                                                                  string sourceFile,
                                                                                  string newVersion)
        {
            // Given
            string sourceFilePath = CreateSourceFilePathWithSpecialCharacter(sourceFile, specialCharacter);
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string name = $"{nameof(GivenProject_WhenSpecialCharacterInPath_DoesNotThrowException)} {specialCharacter}";
            string targetFilePath = TestHelper.GetScratchPadPath(name);
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(name, sourceFile, ".log"));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
            {
                LogPath = logFilePath
            };

            try
            {
                using (new FileDisposeHelper(logFilePath))
                using (new FileDisposeHelper(targetFilePath))
                {
                    // When
                    TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                    // Then
                    Assert.DoesNotThrow(call);
                }
            }
            finally
            {
                File.Delete(sourceFilePath);
            }
        }

        private string CreateSourceFilePathWithSpecialCharacter(string sourceFile, char specialCharacter)
        {
            string fileToCopy = Path.Combine(testDataPath, sourceFile);
            string sourceFilePath = TestHelper.GetScratchPadPath($"{specialCharacter} {sourceFile}");
            File.Copy(fileToCopy, sourceFilePath, true);

            // Precondition
            Assert.IsTrue(File.Exists(sourceFilePath));
            return sourceFilePath;
        }

        private static IEnumerable<TestCaseData> GetFileNamesWithSpecialCharacters()
        {
            foreach (char character in GetSpecialCharactersToValidate())
            {
                foreach (FileToMigrate fileToMigrate in GetFilesToMigrate())
                {
                    yield return new TestCaseData(character,
                                                  fileToMigrate.OriginalPath,
                                                  fileToMigrate.ToVersion)
                        .SetName($"Migrate{fileToMigrate.OriginalPath}WithChar{character}");
                }
            }
        }

        private static IEnumerable<FileToMigrate> GetFilesToMigrate()
        {
            yield return new FileToMigrate("Empty valid Release 16.4.rtd", "17.1");
            yield return new FileToMigrate("Empty valid Release 17.1.rtd", "17.2");
        }

        private static IEnumerable<char> GetSpecialCharactersToValidate()
        {
            yield return '\'';
            yield return '[';
            yield return ']';
            yield return '!';
            yield return '`';
            yield return '~';
            yield return '@';
            yield return '#';
            yield return '$';
            yield return '%';
            yield return '€';
            yield return '^';
            yield return '&';
            yield return '(';
            yield return ')';
            yield return '-';
            yield return '_';
            yield return '=';
            yield return '+';
            yield return ';';
            yield return ',';
            yield return '两';
            yield return '​'; // zero width space
            yield return 'Ⓚ';
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