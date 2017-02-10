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
using System.Security.AccessControl;
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Exporters;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Exporters
{
    [TestFixture]
    public class CalculationGroupWriterTest
    {
        [Test]
        public void WriteCalculationGroups_CalculationGroupNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationGroupWriter.WriteCalculationGroups(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("rootCalculationGroup", exception.ParamName);
        }

        [Test]
        public void WriteCalculationGroups_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationGroupWriter.WriteCalculationGroups(new CalculationGroup(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void WriteCalculationGroups_FilePathInvalid_ThrowCriticalFileWriteException(string filePath)
        {
            // Call
            TestDelegate call = () => CalculationGroupWriter.WriteCalculationGroups(new CalculationGroup(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void WriteCalculationGroups_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);

            // Call
            TestDelegate call = () => CalculationGroupWriter.WriteCalculationGroups(new CalculationGroup(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        [Test]
        public void WriteCalculationGroups_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
                                                              "WriteCalculationGroups_InvalidDirectoryRights_ThrowCriticalFileWriteException");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.xml");

            // Call
            TestDelegate call = () => CalculationGroupWriter.WriteCalculationGroups(new CalculationGroup(), filePath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Assert
                    var exception = Assert.Throws<CriticalFileWriteException>(call);
                    Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
                    Assert.IsInstanceOf<UnauthorizedAccessException>(exception.InnerException);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void WriteCalculationGroups_ValidData_ValidFile()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
                                                              "CalculationGroupWriter");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.xml");

            var calculationGroup = new CalculationGroup("PK001_0001", false);

            var calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.Name = "PK001_0001 W1-6_0_1D1";
            calculation.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_KAT_18", 0, 0);
            calculation.InputParameters.SurfaceLine.Name = "PK001_0001";
            calculation.InputParameters.StochasticSoilModel = new StochasticSoilModel(1, "PK001_0001_Piping", string.Empty);
            calculation.InputParameters.StochasticSoilProfile = new StochasticSoilProfile(0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile("W1-6_0_1D1", 0, new[]
                {
                    new PipingSoilLayer(0)
                }, SoilProfileType.SoilProfile1D, 0)
            };
            calculation.InputParameters.PhreaticLevelExit.Mean = (RoundedDouble) 0;
            calculation.InputParameters.PhreaticLevelExit.StandardDeviation = (RoundedDouble) 0.1;
            calculation.InputParameters.DampingFactorExit.Mean = (RoundedDouble) 0.7;
            calculation.InputParameters.DampingFactorExit.StandardDeviation = (RoundedDouble) 0.1;

            calculationGroup.Children.Add(calculation);

            var calculation2 = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation2.Name = "PK001_0002 W1-6_4_1D1";
            calculation2.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_SCH_17", 0, 0);
            calculation2.InputParameters.SurfaceLine.Name = "PK001_0002";
            calculation2.InputParameters.StochasticSoilModel = new StochasticSoilModel(1, "PK001_0002_Piping", string.Empty);
            calculation2.InputParameters.StochasticSoilProfile = new StochasticSoilProfile(0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile("W1-6_4_1D1", 0, new []
                {
                    new PipingSoilLayer(0)
                }, SoilProfileType.SoilProfile1D, 0)
            };
            calculation2.InputParameters.PhreaticLevelExit.Mean = (RoundedDouble)0;
            calculation2.InputParameters.PhreaticLevelExit.StandardDeviation = (RoundedDouble)0.1;
            calculation2.InputParameters.DampingFactorExit.Mean = (RoundedDouble)0.7;
            calculation2.InputParameters.DampingFactorExit.StandardDeviation = (RoundedDouble)0.1;

            var calculationGroup2 = new CalculationGroup("PK001_0002", false);
            calculationGroup2.Children.Add(calculation2);
            calculationGroup.Children.Add(calculationGroup2);

            try
            {
                // Call
                CalculationGroupWriter.WriteCalculationGroups(calculationGroup, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                var actualXml = File.ReadAllText(filePath);
                var expectedXml = File.ReadAllText(Path.Combine(directoryPath, "folder_with_subfolder_and_calculation.xml"));


                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}