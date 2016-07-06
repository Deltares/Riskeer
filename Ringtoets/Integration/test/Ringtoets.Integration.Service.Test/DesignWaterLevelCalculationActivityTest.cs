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
using Core.Common.Base;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
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
            Assert.AreEqual(string.Format(Resources.DesignWaterLevelCalculationService_Name_Calculate_assessment_level_for_location_0_, hydraulicBoundaryLocation.Id), activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);

            mocks.VerifyAll();
        }

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
                }
            };

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 0, 0);
            var activity = new DesignWaterLevelCalculationActivity(assessmentSection, hydraulicBoundaryLocation);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", hydraulicBoundaryLocation.Id), msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", hydraulicBoundaryLocation.Id), msgs[2]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_ValidHydraulicBoundaryDatabaseAndHydraulicBoundaryLocation_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(loc => loc.Id == 1300001);

            var activity = new DesignWaterLevelCalculationActivity(assessmentSection, hydraulicBoundaryLocation);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", hydraulicBoundaryLocation.Id), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", hydraulicBoundaryLocation.Id), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", hydraulicBoundaryLocation.Id), msgs[2]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", hydraulicBoundaryLocation.Id), msgs[3]);
            });
            Assert.AreEqual(ActivityState.Executed, activity.State);
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
                Assert.AreEqual(5, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", hydraulicBoundaryLocation.Id), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", hydraulicBoundaryLocation.Id), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", hydraulicBoundaryLocation.Id), msgs[2]);
                StringAssert.StartsWith(string.Format("Er is een fout opgetreden tijdens de toetspeil berekening '{0}': inspecteer het logbestand.", hydraulicBoundaryLocation.Id), msgs[3]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", hydraulicBoundaryLocation.Id), msgs[4]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
        }

        [Test]
        public void Run_CalculationAlreadyRan_ValidationAndCalculationNotPerformedAndStateSkipped()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var designWaterLevel = 3.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
            {
                DesignWaterLevel = designWaterLevel
            };

            var activity = new DesignWaterLevelCalculationActivity(assessmentSection, hydraulicBoundaryLocation);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(0, msgs.Length);
            });
            Assert.AreEqual(ActivityState.Skipped, activity.State);
        }

        [Test]
        public void Finish_ValidCalculationAndRun_SetsDesignWaterLevelAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(loc => loc.Id == 1300001);
            hydraulicBoundaryLocation.Attach(observerMock);

            var activity = new DesignWaterLevelCalculationActivity(assessmentSection, hydraulicBoundaryLocation);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.IsFalse(double.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel));
            mocks.VerifyAll();
        }

        [Test]
        public void Finish_InvalidCalculationAndRun_DoesNotSetDesignWaterlevelAndDoesNotNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1);
            hydraulicBoundaryLocation.Attach(observerMock);

            var activity = new DesignWaterLevelCalculationActivity(assessmentSection, hydraulicBoundaryLocation);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            mocks.VerifyAll();
        }

        [Test]
        public void Finish_CalculationAlreadyRan_FinishNotPerformed()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var designWaterLevel = 3.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
            {
                DesignWaterLevel = designWaterLevel
            };

            var activity = new DesignWaterLevelCalculationActivity(assessmentSection, hydraulicBoundaryLocation);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel);
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