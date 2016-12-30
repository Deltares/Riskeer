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
using System.Linq;
using System.Security.AccessControl;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.IO.Test
{
    [TestFixture]
    public class DuneLocationsWriterTest
    {
        [Test]
        public void WriteDuneLocations_DuneLocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneLocationsWriter.WriteDuneLocations(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneLocations", exception.ParamName);
        }

        [Test]
        public void WriteDuneLocations_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneLocationsWriter.WriteDuneLocations(Enumerable.Empty<DuneLocation>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void WriteWaveConditions_FilePathInvalid_ThrowCriticalFileWriteException(string filePath)
        {
            // Call
            TestDelegate call = () => DuneLocationsWriter.WriteDuneLocations(Enumerable.Empty<DuneLocation>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual(string.Format("Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{0}'.", filePath), exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void WriteDuneLocations_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);

            // Call
            TestDelegate call = () => DuneLocationsWriter.WriteDuneLocations(Enumerable.Empty<DuneLocation>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual(string.Format("Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{0}'.", filePath), exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        [Test]
        public void WriteDuneLocations_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.DuneErosion.IO,
                                                              "WriteDuneLocations_InvalidDirectoryRights_ThrowCriticalFileWriteException");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.bnd");

            // Call
            TestDelegate call = () => DuneLocationsWriter.WriteDuneLocations(Enumerable.Empty<DuneLocation>(), filePath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Assert
                    var exception = Assert.Throws<CriticalFileWriteException>(call);
                    Assert.AreEqual(string.Format("Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{0}'.", filePath), exception.Message);
                    Assert.IsInstanceOf<UnauthorizedAccessException>(exception.InnerException);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void WriteDuneLocations_ValidData_ValidFile()
        {
            // Setup
            DuneLocation[] duneLocations =
            {
                new DuneLocation(1, string.Empty, new Point2D(0, 0), 9, 9740, 0, 1.9583e-4)
                {
                    Output = new DuneLocationOutput(5.89, 8.54, 14.11, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged)
                },
                new DuneLocation(2, string.Empty, new Point2D(0, 0), 9, 9770, 0, 1.9561e-4)
                {
                    Output = new DuneLocationOutput(5.89, 8.53, 14.09, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged)
                }
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.DuneErosion.IO,
                                                              "WriteDuneLocations_ValidData_ValidFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.csv");

            try
            {
                // Call
                DuneLocationsWriter.WriteDuneLocations(duneLocations, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                Assert.AreEqual("Kv\tNr\tRp\tHs\tTp\tD50\r\n" +
                                "9\t9740.0\t5.89\t8.54\t14.11\t0.000196\r\n" +
                                "9\t9770.0\t5.89\t8.53\t14.09\t0.000196\r\n",
                                fileContent);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }
    }
}