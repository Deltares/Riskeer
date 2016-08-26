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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Service.Test
{
    [TestFixture]
    public class DesignWaterLevelCalculationServiceTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        private readonly string validFile = "HRD dutch coast south.sqlite";

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabase_ReturnsTrue()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            bool valid = false;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);

            // Call
            Action call = () => valid = DesignWaterLevelCalculationService.Validate(hydraulicBoundaryLocation, validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                var calculationName = string.Format("Toetspeil voor locatie {0}", hydraulicBoundaryLocation.Name);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[1]);
            });
            Assert.IsTrue(valid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            string notValidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            bool valid = false;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);

            // Call
            Action call = () => valid = DesignWaterLevelCalculationService.Validate(hydraulicBoundaryLocation, notValidFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                var calculationName = string.Format("Toetspeil voor locatie {0}", hydraulicBoundaryLocation.Name);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Calculate_ValidHydraulicBoundaryDatabaseAndLocation_LogStartAndEndAndReturnOutput()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            ImportHydraulicBoundaryDatabase(assessmentSectionStub);

            var hydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            ReliabilityIndexCalculationOutput output = null;

            // Call
            Action call = () => output = DesignWaterLevelCalculationService.Calculate(
                hydraulicBoundaryLocation, assessmentSectionStub.HydraulicBoundaryDatabase.FilePath, "", 30);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                var calculationName = string.Format("Toetspeil voor locatie {0}", hydraulicBoundaryLocation.Name);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith("Hydra-Ring berekeningsverslag. Klik op details voor meer informatie.", msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[2]);
            });
            Assert.IsNotNull(output);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidHydraulicBoundaryDatabaseInvalidHydraulicBoundaryLocation_LogStartAndEndAndErrorMessageAndReturnNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            ImportHydraulicBoundaryDatabase(assessmentSectionStub);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);

            ReliabilityIndexCalculationOutput output = null;

            // Call
            Action call = () => output = DesignWaterLevelCalculationService.Calculate(
                hydraulicBoundaryLocation, assessmentSectionStub.HydraulicBoundaryDatabase.FilePath, "", 30);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                var calculationName = string.Format("Toetspeil voor locatie {0}", hydraulicBoundaryLocation.Name);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith("Hydra-Ring berekeningsverslag. Klik op details voor meer informatie.", msgs[1]);
                StringAssert.StartsWith(string.Format("Er is een fout opgetreden tijdens de toetspeil berekening '{0}': inspecteer het logbestand.", hydraulicBoundaryLocation.Name), msgs[2]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[3]);
            });
            Assert.IsNull(output);
            mockRepository.VerifyAll();
        }

        private void ImportHydraulicBoundaryDatabase(IAssessmentSection assessmentSection)
        {
            string validFilePath = Path.Combine(testDataPath, validFile);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }
        }
    }
}