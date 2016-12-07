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
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.Revetment.IO.Test
{
    [TestFixture]
    public class WaveConditionsWriterTest
    {
        [Test]
        public void WriteWaveConditions_ExportableWaveConditionsCollectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WaveConditionsWriter.WriteWaveConditions(null, "afilePath");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("exportableWaveConditionsCollection", exception.ParamName);
        }

        [Test]
        public void WriteWaveConditions_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WaveConditionsWriter.WriteWaveConditions(Enumerable.Empty<ExportableWaveConditions>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void WriteWaveConditions_FilePathInvalid_ThrowCriticalFileWriteException(string filePath)
        {
            // Call
            TestDelegate call = () => WaveConditionsWriter.WriteWaveConditions(Enumerable.Empty<ExportableWaveConditions>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual(string.Format("Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{0}'.", filePath), exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void WriteWaveConditions_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);

            // Call
            TestDelegate call = () => WaveConditionsWriter.WriteWaveConditions(Enumerable.Empty<ExportableWaveConditions>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual(string.Format("Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{0}'.", filePath), exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        [Test]
        public void WriteWaveConditions_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO,
                                                              "WriteWaveConditions_InvalidDirectoryRights_ThrowCriticalFileWriteException");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.csv");

            // Call
            TestDelegate call = () => WaveConditionsWriter.WriteWaveConditions(Enumerable.Empty<ExportableWaveConditions>(), filePath);

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
        public void WriteWaveConditions_ValidData_ValidFile()
        {
            // Setup
            ExportableWaveConditions[] exportableWaveConditions =
            {
                new ExportableWaveConditions("blocksName", new WaveConditionsInput()
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                    {
                        DesignWaterLevel = (RoundedDouble) 12.34567
                    },
                    LowerBoundaryRevetment = (RoundedDouble) 5.68,
                    UpperBoundaryRevetment = (RoundedDouble) 7.214,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryWaterLevels = (RoundedDouble) 2.689,
                    UpperBoundaryWaterLevels = (RoundedDouble) 77.8249863247,
                    UseBreakWater = true
                }, new TestWaveConditionsOutput(1.11111, 2.22222, 3.33333, 4.44444), CoverType.StoneCoverBlocks),
                new ExportableWaveConditions("columnsName", new WaveConditionsInput
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(8, "aLocation", 44, 123.456)
                    {
                        DesignWaterLevel = (RoundedDouble) 28.36844
                    },
                    LowerBoundaryRevetment = (RoundedDouble) 1.384,
                    UpperBoundaryRevetment = (RoundedDouble) 11.54898963,
                    StepSize = WaveConditionsInputStepSize.One,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1.98699,
                    UpperBoundaryWaterLevels = (RoundedDouble) 84.26548
                }, new TestWaveConditionsOutput(3.33333, 1.11111, 4.44444, 2.22222), CoverType.StoneCoverColumns)
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO,
                                                              "WriteWaveConditions_ValidData_ValidFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.csv");

            try
            {
                // Call
                WaveConditionsWriter.WriteWaveConditions(exportableWaveConditions, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                Assert.AreEqual("Naam berekening, Naam HR locatie, X HR locatie (RD) [m], Y HR locatie (RD) [m], Naam voorlandprofiel, Dam aanwezig, Voorlandgeometrie aanwezig, Type bekleding, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting [°]\r\n" +
                                "blocksName, , 0.000, 0.000, , ja, nee, Steen (blokken), 1.11, 2.22, 3.33, 4.44\r\n" +
                                "columnsName, aLocation, 44.000, 123.456, , nee, nee, Steen (zuilen), 3.33, 1.11, 4.44, 2.22\r\n",
                                fileContent);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }
    }
}