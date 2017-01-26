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
using Core.Common.TestUtil;
using Migration.Scripts.Data;
using NUnit.Framework;

namespace Application.Ringtoets.Migration.Test
{
    [TestFixture]
    public class RingtoetsCreateScriptTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_InvalidVersion_ThrowsArgumentException(string version)
        {
            // Setup
            const string query = "Valid query";

            // Call
            TestDelegate call = () => new RingtoetsCreateScript(version, query);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("version", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_InvalidQuery_ThrowsArgumentException(string query)
        {
            // Setup
            const string version = "Valid version";

            // Call
            TestDelegate call = () => new RingtoetsCreateScript(version, query);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("query", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            const string query = "Valid query";
            const string version = "Valid version";

            // Call
            var createScript = new RingtoetsCreateScript(version, query);

            // Assert
            Assert.IsInstanceOf<CreateScript>(createScript);
            Assert.AreEqual(version, createScript.Version());
        }

        [Test]
        public void CreateEmptyVersionedFile_FileDoesNotExist_ReturnsVersionedFile()
        {
            // Setup
            const string query = ";";
            const string version = "Valid version";

            string targetFilename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, targetFilename);
            var createScript = new RingtoetsCreateScript(version, query);

            // Call
            IVersionedFile versionedFile = createScript.CreateEmptyVersionedFile(filePath);

            // Assert
            Assert.IsTrue(File.Exists(versionedFile.Location));
            using (new FileDisposeHelper(filePath)) {}
        }

        [Test]
        public void CreateEmptyVersionedFile_FileExistsButNotWritable_ThrowsArgumentException()
        {
            // Setup
            const string query = ";";
            const string version = "Valid version";

            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, filename);
            var createScript = new RingtoetsCreateScript(version, query);

            using (new FileDisposeHelper(filePath))
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                File.SetAttributes(filePath, attributes | FileAttributes.ReadOnly);

                // Call
                TestDelegate call = () => createScript.CreateEmptyVersionedFile(filePath);

                // Assert
                ArgumentException exception = Assert.Throws<ArgumentException>(call);
                Assert.AreEqual("path", exception.ParamName);
                File.SetAttributes(filePath, attributes);
            }
        }
    }
}