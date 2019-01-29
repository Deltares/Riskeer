// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Data;
using System.Data.SQLite;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using UtilResources = Core.Common.Util.Properties.Resources;

namespace Core.Common.IO.Test.Readers
{
    [TestFixture]
    public class SqLiteDatabaseReaderBaseTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.IO, "SqLiteDatabaseReaderBase");

        [Test]
        public void Constructor_WithNetworkPath_OpensConnection()
        {
            // Setup
            const string fileName = "temp.sqlite";
            string localPath = TestHelper.GetScratchPadPath(fileName);
            string uncPath = TestHelper.ToUncPath(localPath);

            using (new FileDisposeHelper(localPath))
            {
                // Call
                using (var reader = new TestReader(uncPath))
                {
                    // Assert
                    Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
                    Assert.AreEqual(ConnectionState.Open, reader.TestConnection.State);
                }
            }
        }

        [Test]
        public void Constructor_WithParameter_OpensConnection()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "empty.sqlite");

            // Call
            using (var reader = new TestReader(testFile))
            {
                // Assert
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
                Assert.AreEqual(ConnectionState.Open, reader.TestConnection.State);
            }
        }

        [Test]
        public void Constructor_WithInvalidPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "empty.sqlite");

            char[] invalidCharacters = Path.GetInvalidPathChars();

            string corruptPath = testFile.Replace('e', invalidCharacters[0]);

            // Call
            TestDelegate test = () => new TestReader(corruptPath).Dispose();

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .Build("Er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.");
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "none.sqlite");

            // Call
            TestDelegate test = () => new TestReader(testFile).Dispose();

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build(UtilResources.Error_File_does_not_exist);
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Setup
            string expectedMessage = $"Fout bij het lezen van bestand '{fileName}': bestandspad mag niet leeg of ongedefinieerd zijn.";

            // Call
            TestDelegate test = () => new TestReader(fileName).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Path_Always_ReturnsPath()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "empty.sqlite");

            // Call
            var reader = new TestReader(testFile);

            // Assert
            Assert.AreEqual(testFile, reader.Path);
        }

        private class TestReader : SqLiteDatabaseReaderBase
        {
            public TestReader(string databaseFilePath) : base(databaseFilePath) {}

            public SQLiteConnection TestConnection
            {
                get
                {
                    return Connection;
                }
            }
        }
    }
}