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

using System;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraulicDatabaseHelperTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        [Test]
        public void ValidatePathForCalculation_ExistingFileWithHlcd_ReturnsNull()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            string result = HydraulicDatabaseHelper.ValidatePathForCalculation(validFilePath);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ValidatePathForCalculation_NonExistingFile_ReturnsMessageWithError()
        {
            // Setup
            const string nonexistingSqlite = "nonexisting.sqlite";
            string filePath = Path.Combine(testDataPath, nonexistingSqlite);

            // Call
            string result = HydraulicDatabaseHelper.ValidatePathForCalculation(filePath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", filePath), result);
        }

        [Test]
        public void ValidatePathForCalculation_InvalidFile_ReturnsMessageWithError()
        {
            // Setup
            string invalidPath = Path.Combine(testDataPath, "complete.sqlite");
            invalidPath = invalidPath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            string result = HydraulicDatabaseHelper.ValidatePathForCalculation(invalidPath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", invalidPath), result);
        }

        [Test]
        public void ValidatePathForCalculation_FileIsDirectory_ReturnsMessageWithError()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "/");

            // Call
            string result = HydraulicDatabaseHelper.ValidatePathForCalculation(filePath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", filePath), result);
        }

        [Test]
        public void ValidatePathForCalculation_ExistingFileWithoutHlcd_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutHLCD", "empty.sqlite");

            // Call
            string result = HydraulicDatabaseHelper.ValidatePathForCalculation(validFilePath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", validFilePath), result);
        }

        [Test]
        public void ValidatePathForCalculation_ExistingFileWithoutSettings_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutSettings", "empty.sqlite");

            // Call
            string result = HydraulicDatabaseHelper.ValidatePathForCalculation(validFilePath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", validFilePath), result);
        }

        [Test]
        public void ValidatePathForCalculation_ExistingFileWithInvalidSettingsDatabase_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", "complete.sqlite");

            // Call
            string result = HydraulicDatabaseHelper.ValidatePathForCalculation(validFilePath);

            // Assert
            StringAssert.StartsWith("De rekeninstellingen database heeft niet het juiste schema.", result);
        }

        [Test]
        public void ValidatePathForCalculation_InvalidFilePath_ReturnsMessageWithError()
        {
            // Setup
            const string invalidFilePath = "C:\\Thisissomeverylongpath\\toadirectorywhich\\doesntevenexist\\Nowlets\\finishwithsomelongname\\" +
                                           "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong" +
                                           "naaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaame" +
                                           "\\followedbythefile";

            // Call
            string result = HydraulicDatabaseHelper.ValidatePathForCalculation(invalidFilePath);

            // Assert
            StringAssert.StartsWith(string.Format("Het opgegeven bestandspad ({0}) is niet geldig.", invalidFilePath), result);
        }

        [Test]
        public void HaveEqualVersion_InvalidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string invalidPath = Path.Combine(testDataPath, "complete.sqlite");
            invalidPath = invalidPath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            TestDelegate test = () => HydraulicDatabaseHelper.HaveEqualVersion(new HydraulicBoundaryDatabase(), invalidPath);

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void HaveEqualVersion_PathNotSet_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraulicDatabaseHelper.HaveEqualVersion(new HydraulicBoundaryDatabase(), null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("pathToDatabase", parameter);
        }

        [Test]
        public void HaveEqualVersion_DatabaseNotSet_ThrowsArgumentNullException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            TestDelegate test = () => HydraulicDatabaseHelper.HaveEqualVersion(null, validFilePath);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("database", parameter);
        }

        [Test]
        public void HaveEqualVersion_ValidFileEqualVersion_ReturnsTrue()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var database = new HydraulicBoundaryDatabase
            {
                Version = "Dutch coast South19-11-2015 12:0013"
            };

            // Call
            bool isEqual = HydraulicDatabaseHelper.HaveEqualVersion(database, validFilePath);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void HaveEqualVersion_ValidFileDifferentVersion_ReturnsFalse()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var database = new HydraulicBoundaryDatabase
            {
                Version = "Dutch coast South19-11-2015 12:0113"
            };

            // Call
            bool isEqual = HydraulicDatabaseHelper.HaveEqualVersion(database, validFilePath);

            // Assert
            Assert.IsFalse(isEqual);
        }
    }
}