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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Service.Test
{
    public class WaveHeightCalculationServiceTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabase_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            bool valid = false;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);

            // Call
            Action call = () => valid = WaveHeightCalculationService.Validate(assessmentSection.HydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", hydraulicBoundaryLocation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", hydraulicBoundaryLocation.Name), msgs[1]);
            });
            Assert.IsTrue(valid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
            };

            bool valid = false;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);

            // Call
            Action call = () => valid = WaveHeightCalculationService.Validate(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", hydraulicBoundaryLocation.Name), msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", hydraulicBoundaryLocation.Name), msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void CalculateWaveHeight_ValidHydraulicBoundaryDatabaseAndLocation_LogStartAndEndAndReturnOutput()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            TargetProbabilityCalculationOutput output = null;

            // Call
            Action call = () => output = WaveHeightCalculationService.Calculate(assessmentSection,
                                                                                      assessmentSection.HydraulicBoundaryDatabase,
                                                                                      hydraulicBoundaryLocation,
                                                                                      assessmentSection.Name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", hydraulicBoundaryLocation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", hydraulicBoundaryLocation.Name), msgs[1]);
            });
            Assert.IsNotNull(output);
        }

        [Test]
        public void CalculateWaveHeight_ValidHydraulicBoundaryDatabaseInvalidHydraulicBoundaryLocation_LogStartAndEndAndErrorMessageAndReturnNull()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);

            TargetProbabilityCalculationOutput output = null;

            // Call
            Action call = () => output = WaveHeightCalculationService.Calculate(assessmentSection,
                                                                                      assessmentSection.HydraulicBoundaryDatabase,
                                                                                      hydraulicBoundaryLocation,
                                                                                      assessmentSection.Name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", hydraulicBoundaryLocation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("Er is een fout opgetreden tijdens de golfhoogte berekening '{0}': inspecteer het logbestand.", hydraulicBoundaryLocation.Name), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", hydraulicBoundaryLocation.Name), msgs[2]);
            });
            Assert.IsNull(output);
        }

        private void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }
        } 
    }
}