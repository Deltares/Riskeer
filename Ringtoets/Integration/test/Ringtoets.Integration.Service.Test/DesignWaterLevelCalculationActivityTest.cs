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
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Integration.Service.Properties;

namespace Ringtoets.Integration.Service.Test
{
    [TestFixture]
    public class DesignWaterLevelCalculationActivityTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);

            // Call
            var activity = new DesignWaterLevelCalculationActivity(assessmentSectionMock, hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            string expectedName = string.Format(Resources.DesignWaterLevelCalculationService_Name_Calculate_assessment_level_for_location_0_,
                                                hydraulicBoundaryLocation.Name);
            Assert.AreEqual(expectedName, activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);

            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationActivity(null, hydraulicBoundaryLocation);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationActivity(assessmentSectionMock, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
            };
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);
            var activity = new DesignWaterLevelCalculationActivity(assessmentSectionStub, hydraulicBoundaryLocation);

            // Call
            Action call = () => activity.Run();

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
            Assert.AreEqual(ActivityState.Failed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidHydraulicBoundaryDatabaseAndHydraulicBoundaryLocation_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Expect(o => o.Id).Return(null);
            assessmentSectionStub.Expect(o => o.NotifyObservers());

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 30000);
            assessmentSectionStub.Expect(asm => asm.FailureMechanismContribution).Return(failureMechanismContribution);
            mockRepository.ReplayAll();

            ImportHydraulicBoundaryDatabase(assessmentSectionStub);

            var hydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(loc => loc.Id == 1300001);

            var activity = new DesignWaterLevelCalculationActivity(assessmentSectionStub, hydraulicBoundaryLocation);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                var calculationName = string.Format("Toetspeil voor locatie {0}", hydraulicBoundaryLocation.Name);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[2]);
                StringAssert.StartsWith("HydraRing berekeningsverslag. Klik op details voor meer informatie.", msgs[3]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[4]);
            });
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidHydraulicBoundaryDatabaseInvalidHydraulicBoundaryLocation_PerformValidationAndCalculationAndLogStartAndEndAndError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);

            var activity = new DesignWaterLevelCalculationActivity(assessmentSection, hydraulicBoundaryLocation);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                var calculationName = string.Format("Toetspeil voor locatie {0}", hydraulicBoundaryLocation.Name);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), msgs[2]);
                StringAssert.StartsWith("HydraRing berekeningsverslag. Klik op details voor meer informatie.", msgs[3]);
                StringAssert.StartsWith(string.Format("Er is een fout opgetreden tijdens de toetspeil berekening '{0}': inspecteer het logbestand.", hydraulicBoundaryLocation.Name), msgs[4]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), msgs[5]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
        }

        [Test]
        public void Run_CalculationAlreadyRan_ValidationAndCalculationNotPerformedAndStateSkipped()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            ImportHydraulicBoundaryDatabase(assessmentSectionStub);

            var designWaterLevel = 3.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
            {
                DesignWaterLevel = designWaterLevel
            };

            var activity = new DesignWaterLevelCalculationActivity(assessmentSectionStub, hydraulicBoundaryLocation);

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
        public void Finish_ValidCalculationAndRun_SetsDesignWaterLevel()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Expect(o => o.Id).Return(null);
            assessmentSectionStub.Expect(o => o.NotifyObservers());

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 30000);
            assessmentSectionStub.Expect(asm => asm.FailureMechanismContribution).Return(failureMechanismContribution);
            mockRepository.ReplayAll();

            ImportHydraulicBoundaryDatabase(assessmentSectionStub);

            var hydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(loc => loc.Id == 1300001);

            var activity = new DesignWaterLevelCalculationActivity(assessmentSectionStub, hydraulicBoundaryLocation);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.IsFalse(double.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel));
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_InvalidCalculationAndRun_DoesNotSetDesignWaterlevel()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Expect(o => o.Id).Return(null);
            assessmentSectionStub.Expect(o => o.NotifyObservers());

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 30000);
            assessmentSectionStub.Expect(asm => asm.FailureMechanismContribution).Return(failureMechanismContribution);
            mockRepository.ReplayAll();

            ImportHydraulicBoundaryDatabase(assessmentSectionStub);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);

            var activity = new DesignWaterLevelCalculationActivity(assessmentSectionStub, hydraulicBoundaryLocation);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_CalculationAlreadyRan_FinishNotPerformed()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            ImportHydraulicBoundaryDatabase(assessmentSectionStub);

            const double designWaterLevel = 3.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
            {
                DesignWaterLevel = designWaterLevel
            };

            var activity = new DesignWaterLevelCalculationActivity(assessmentSectionStub, hydraulicBoundaryLocation);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel);
            mockRepository.VerifyAll();
        }

        private void ImportHydraulicBoundaryDatabase(IAssessmentSection assessmentSection)
        {
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
                importer.Import(assessmentSection, validFilePath);
        }
    }
}