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

using System.Data;
using System.Data.SQLite;
using System.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using UtilsResources = Core.Common.Utils.Properties.Resources;

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
            var localPath = Path.Combine(@"c:\", fileName);
            var uncPath = Path.Combine(@"\\localhost\c$", fileName);

            // Call
            using (new FileDisposeHelper(localPath))
            {
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
            var testFile = Path.Combine(testDataPath, "empty.sqlite");

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
            var testFile = Path.Combine(testDataPath, "empty.sqlite");

            var invalidCharacters = Path.GetInvalidPathChars();

            var corruptPath = testFile.Replace('e', invalidCharacters[0]);

            // Call
            TestDelegate test = () => new TestReader(corruptPath).Dispose();

            // Assert
            var expectedMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .Build(string.Format(UtilsResources.Error_Path_cannot_contain_Characters_0_,
                                     string.Join(", ", Path.GetInvalidPathChars())));
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "none.sqlite");

            // Call
            TestDelegate test = () => new TestReader(testFile).Dispose();

            // Assert
            var expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build(UtilsResources.Error_File_does_not_exist);
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Setup
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                fileName, "bestandspad mag niet leeg of ongedefinieerd zijn.");

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
            var testFile = Path.Combine(testDataPath, "empty.sqlite");

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