// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.IO;
using Core.Common.TestUtil;
using Migration.Scripts.Data.TestUtil;
using NUnit.Framework;

namespace Migration.Scripts.Data.Test
{
    [TestFixture]
    public class CreateScriptTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_InvalidVersion_ThrowsArgumentException(string version)
        {
            // Call
            TestDelegate call = () => new TestCreateScript(version);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("version", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            const string version = "Valid version";

            // Call
            var createScript = new TestCreateScript(version);

            // Assert
            Assert.AreEqual(version, createScript.GetVersion());
        }

        [Test]
        public void CreateEmptyVersionedFile_FileDoesNotExist_ReturnsVersionedFile()
        {
            // Setup
            const string version = "Valid version";

            string filePath = TestHelper.GetScratchPadPath(nameof(CreateEmptyVersionedFile_FileDoesNotExist_ReturnsVersionedFile));
            var createScript = new TestCreateScript(version);

            // Call
            IVersionedFile versionedFile = createScript.CreateEmptyVersionedFile(filePath);

            // Assert
            Assert.IsTrue(File.Exists(versionedFile.Location));
            File.Delete(filePath);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void CreateEmptyVersionedFile_InvalidPath_ThrowsArgumentException(string filePath)
        {
            // Setup
            const string version = "Valid version";
            var createScript = new TestCreateScript(version);

            // Call
            TestDelegate call = () => createScript.CreateEmptyVersionedFile(filePath);

            // Assert
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual($"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet leeg of ongedefinieerd zijn.", message);
        }

        [Test]
        public void CreateEmptyVersionedFile_FileExistsButNotWritable_ThrowsArgumentException()
        {
            // Setup
            const string version = "Valid version";

            string filePath = TestHelper.GetScratchPadPath(nameof(CreateEmptyVersionedFile_FileExistsButNotWritable_ThrowsArgumentException));
            var createScript = new TestCreateScript(version);

            using (var disposeHelper = new FileDisposeHelper(filePath))
            {
                disposeHelper.LockFiles();

                // Call
                TestDelegate call = () => createScript.CreateEmptyVersionedFile(filePath);

                // Assert
                var exception = Assert.Throws<ArgumentException>(call);
                Assert.AreEqual("path", exception.ParamName);
            }
        }
    }
}