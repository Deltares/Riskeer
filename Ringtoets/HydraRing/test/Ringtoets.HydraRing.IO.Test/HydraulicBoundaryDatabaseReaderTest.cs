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
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.HydraRing.IO.Properties;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.HydraRing.IO.Test
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "none.sqlite");

            // Call
            TestDelegate test = () => new HydraulicBoundaryDatabaseReader(testFile).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            var expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build(UtilsResources.Error_File_does_not_exist);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryDatabaseReader(fileName).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            var expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                fileName, UtilsResources.Error_Path_must_be_specified);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("empty.sqlite")]
        public void Constructor_IncorrectFormatFileOrInvalidSchema_ThrowsCriticalFileReadException(string dbName)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, dbName);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => new HydraulicBoundaryDatabaseReader(dbFile).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            var expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(Resources.Error_HydraulicBoundaryLocation_read_from_database);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}