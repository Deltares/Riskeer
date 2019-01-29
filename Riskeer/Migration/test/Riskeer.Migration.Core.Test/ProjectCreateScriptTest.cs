// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Data.SQLite;
using System.IO;
using Core.Common.TestUtil;
using Migration.Scripts.Data;
using Migration.Scripts.Data.Exceptions;
using NUnit.Framework;
using Riskeer.Common.Util;

namespace Riskeer.Migration.Core.Test
{
    [TestFixture]
    public class ProjectCreateScriptTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_VersionNullOrEmpty_ThrowsArgumentException(string version)
        {
            // Setup
            const string query = "Valid query";

            // Call
            TestDelegate call = () => new ProjectCreateScript(version, query);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("version", paramName);
        }

        [Test]
        [TestCase("4")]
        public void Constructor_InvalidVersion_ThrowsArgumentException(string version)
        {
            // Setup
            const string query = "Valid query";

            // Call
            TestDelegate call = () => new ProjectCreateScript(version, query);

            // Assert
            string expectedMessage = $@"'{version}' is geen geldige Ringtoets versie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_InvalidQuery_ThrowsArgumentException(string query)
        {
            // Setup
            string version = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            // Call
            TestDelegate call = () => new ProjectCreateScript(version, query);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("query", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            const string query = "Valid query";
            string version = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            // Call
            var createScript = new ProjectCreateScript(version, query);

            // Assert
            Assert.IsInstanceOf<CreateScript>(createScript);
            Assert.AreEqual(version, createScript.GetVersion());
        }

        [Test]
        public void CreateEmptyVersionedFile_FileDoesNotExist_ReturnsVersionedFile()
        {
            // Setup
            const string query = ";";
            string version = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            string filePath = TestHelper.GetScratchPadPath(nameof(CreateEmptyVersionedFile_FileDoesNotExist_ReturnsVersionedFile));
            var createScript = new ProjectCreateScript(version, query);

            // Call
            IVersionedFile versionedFile = createScript.CreateEmptyVersionedFile(filePath);

            // Assert
            Assert.IsTrue(File.Exists(versionedFile.Location));
            File.Delete(filePath);
        }

        [Test]
        public void CreateEmptyVersionedFile_FileExistsButNotWritable_ThrowsArgumentException()
        {
            // Setup
            const string query = ";";
            string version = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            string filePath = TestHelper.GetScratchPadPath(nameof(CreateEmptyVersionedFile_FileExistsButNotWritable_ThrowsArgumentException));
            var createScript = new ProjectCreateScript(version, query);

            using (new FileDisposeHelper(filePath))
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                File.SetAttributes(filePath, attributes | FileAttributes.ReadOnly);

                // Call
                TestDelegate call = () => createScript.CreateEmptyVersionedFile(filePath);

                // Assert
                var exception = Assert.Throws<ArgumentException>(call);
                Assert.AreEqual("path", exception.ParamName);
                File.SetAttributes(filePath, attributes);
            }
        }

        [Test]
        public void CreateEmptyVersionedFile_QueryFails_ThrowsCriticalMigrationException()
        {
            // Setup
            const string query = "THIS WILL FAIL";
            string version = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            string filePath = TestHelper.GetScratchPadPath(nameof(CreateEmptyVersionedFile_QueryFails_ThrowsCriticalMigrationException));
            var createScript = new ProjectCreateScript(version, query);

            // Call
            TestDelegate call = () => createScript.CreateEmptyVersionedFile(filePath);

            // Assert
            using (new FileDisposeHelper(filePath))
            {
                var exception = Assert.Throws<CriticalMigrationException>(call);
                Assert.AreEqual($"Het aanmaken van het Ringtoets projectbestand met versie '{version}' is mislukt.",
                                exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }
    }
}