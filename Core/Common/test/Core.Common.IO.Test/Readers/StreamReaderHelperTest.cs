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

using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.IO.Test.Readers
{
    [TestFixture]
    public class StreamReaderHelperTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.IO, "StreamReaderHelper");
        private readonly string notExistingTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.IO, "NotExistingFolder");

        [Test]
        public void InitializeStreamReader_ValidFile_ReturnsStreamReader()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "empty.csv");

            // Call
            using (StreamReader streamReader = StreamReaderHelper.InitializeStreamReader(filePath))
            {
                // Assert
                Assert.IsInstanceOf<StreamReader>(streamReader);
            }
        }

        [Test]
        public void InitializeStreamReader_NotExistingFile_ThrowsCriticalFileReadExceptionWithInnerFileNotFoundException()
        {
            // Setup
            const string filePath = "nothing";
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand bestaat niet.";

            // Call
            TestDelegate call = () => StreamReaderHelper.InitializeStreamReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<FileNotFoundException>(exception.InnerException);
        }

        [Test]
        public void InitializeStreamReader_NotExistingFolder_ThrowsCriticalFileReadExceptionWithInnerDirectoryNotFoundException()
        {
            // Setup
            string filePath = Path.Combine(notExistingTestDataPath, "empty.csv");
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestandspad verwijst naar een map die niet bestaat.";

            // Call
            TestDelegate call = () => StreamReaderHelper.InitializeStreamReader(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<DirectoryNotFoundException>(exception.InnerException);
        }
    }
}