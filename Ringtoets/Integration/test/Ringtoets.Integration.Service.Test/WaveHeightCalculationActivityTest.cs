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
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Service.Test
{
    [TestFixture]
    public class WaveHeightCalculationActivityTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);

            // Call
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, validFilePath, "", 1);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            string expectedName = string.Format("Golfhoogte berekenen voor locatie '{0}'",
                                                hydraulicBoundaryLocation.Name);
            Assert.AreEqual(expectedName, activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void ParameteredConstructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            TestDelegate call = () => new WaveHeightCalculationActivity(null, validFilePath, "", 1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
        }

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            string inValidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, inValidFilePath, "", 1);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                var calculationName = string.Format("Golfhoogte voor locatie {0}", hydraulicBoundaryLocation.Name);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[2]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_ValidHydraulicBoundaryDatabaseAndHydraulicBoundaryLocation_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            ImportHydraulicBoundaryDatabase(assessmentSectionMock, validFile);
            mockRepository.ReplayAll();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSectionMock.HydraulicBoundaryDatabase.Locations.First(loc => loc.Id == 1300001);
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, assessmentSectionMock.HydraulicBoundaryDatabase.FilePath, "", 30);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                var calculationName = string.Format("Golfhoogte voor locatie {0}", hydraulicBoundaryLocation.Name);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[2]);
                StringAssert.StartsWith("Hydra-Ring berekeningsverslag. Klik op details voor meer informatie.", msgs[3]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[4]);
            });
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidHydraulicBoundaryDatabaseInvalidHydraulicBoundaryLocation_PerformValidationAndCalculationAndLogStartAndEndAndError()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            ImportHydraulicBoundaryDatabase(assessmentSectionMock, validFile);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, assessmentSectionMock.HydraulicBoundaryDatabase.FilePath, "", 30);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                var calculationName = string.Format("Golfhoogte voor locatie {0}", hydraulicBoundaryLocation.Name);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[2]);
                StringAssert.StartsWith("Hydra-Ring berekeningsverslag. Klik op details voor meer informatie.", msgs[3]);
                StringAssert.StartsWith(string.Format("Er is een fout opgetreden tijdens de golfhoogte berekening '{0}': inspecteer het logbestand.", hydraulicBoundaryLocation.Name), msgs[4]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[5]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationAlreadyRan_ValidationAndCalculationNotPerformedAndStateSkipped()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            ImportHydraulicBoundaryDatabase(assessmentSectionMock, validFile);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
            {
                WaveHeight = (RoundedDouble) 3.0
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, assessmentSectionMock.HydraulicBoundaryDatabase.FilePath, "", 30);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(0, msgs.Length);
            });
            Assert.AreEqual(ActivityState.Skipped, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_ValidCalculationAndRun_SetsProperties()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            ImportHydraulicBoundaryDatabase(assessmentSectionMock, validFile);
            mockRepository.ReplayAll();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSectionMock.HydraulicBoundaryDatabase.Locations.First(loc => loc.Id == 1300001);
            hydraulicBoundaryLocation.WaveHeightCalculationConvergence = CalculationConvergence.CalculatedNotConverged;

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, assessmentSectionMock.HydraulicBoundaryDatabase.FilePath, "", 30);

            activity.Run();

            // Precondition
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            // Call
            activity.Finish();

            // Assert
            Assert.IsFalse(double.IsNaN(hydraulicBoundaryLocation.WaveHeight));
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_InvalidCalculationAndRun_DoesNotSetProperties()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            ImportHydraulicBoundaryDatabase(assessmentSectionMock, validFile);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
            {
                WaveHeightCalculationConvergence = CalculationConvergence.CalculatedConverged
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, assessmentSectionMock.HydraulicBoundaryDatabase.FilePath, "", 30);

            activity.Run();

            // Precondition
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            // Call
            activity.Finish();

            // Assert
            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_ValidCalculationAndRun_LogWarningNoConvergence()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            ImportHydraulicBoundaryDatabase(assessmentSectionMock, "HRD ijsselmeer.sqlite");
            mockRepository.ReplayAll();

            const string locationName = "HRbasis_ijsslm_1000";
            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSectionMock.HydraulicBoundaryDatabase.Locations.First(loc => loc.Name == locationName);
            hydraulicBoundaryLocation.WaveHeightCalculationConvergence = CalculationConvergence.CalculatedConverged;

            int norm = 300;
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, assessmentSectionMock.HydraulicBoundaryDatabase.FilePath, "", norm);

            activity.Run();

            // Precondition
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);

            // Call
            Action call = () => activity.Finish();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Golfhoogte berekening voor locatie {0} is niet geconvergeerd.", locationName), msgs[0]);
                StringAssert.StartsWith(string.Format("Uitvoeren van 'Golfhoogte berekenen voor locatie '{0}'' is gelukt.", locationName), msgs[1]);
            });
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_CalculationAlreadyRan_FinishNotPerformed()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            ImportHydraulicBoundaryDatabase(assessmentSectionMock, validFile);
            mockRepository.ReplayAll();

            RoundedDouble waveHeight = (RoundedDouble) 3.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
            {
                WaveHeight = waveHeight
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, assessmentSectionMock.HydraulicBoundaryDatabase.FilePath, "", 30);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.AreEqual(waveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
            mockRepository.VerifyAll();
        }

        private const string validFile = "HRD dutch coast south.sqlite";

        private void ImportHydraulicBoundaryDatabase(IAssessmentSection assessmentSection, string fileName)
        {
            string validFilePath = Path.Combine(testDataPath, fileName);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
                importer.Import(assessmentSection, validFilePath);
        }
    }
}