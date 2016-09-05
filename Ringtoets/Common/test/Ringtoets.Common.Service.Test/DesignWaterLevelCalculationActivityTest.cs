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
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class DesignWaterLevelCalculationActivityTest
    {
        private const string validFile = "HRD dutch coast south.sqlite";
        private MockRepository mockRepository;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            const string locationName = "locationName";
            const string activityName = "GetActivityName";

            var hydraulicBoundaryLocationMock = mockRepository.Stub<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            var activity = new DesignWaterLevelCalculationActivity(calculationMessageProviderMock, hydraulicBoundaryLocationMock, validFilePath, "", 1);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreSame(activityName, activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_NullCalculationServiceMessageProvider_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocationMock = mockRepository.StrictMock<IHydraulicBoundaryLocation>();
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationActivity(null, hydraulicBoundaryLocationMock, validFilePath, "", 1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("messageProvider", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationActivity(calculationMessageProviderMock, null, validFilePath, "", 1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            string inValidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            const string locationName = "testLocation";
            const string activityName = "GetActivityName";
            const string calculationName = "locationName";

            var hydraulicBoundaryLocationMock = mockRepository.Stub<IHydraulicBoundaryLocation>();

            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Id).Return(1);
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.DesignWaterLevel = new RoundedDouble(2, double.NaN);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var activity = new DesignWaterLevelCalculationActivity(calculationMessageProviderMock,
                                                                   hydraulicBoundaryLocationMock, inValidFilePath, "", 1);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), msgs[2]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_ValidHydraulicBoundaryHydraulicBoundaryLocation_PerformValidationValidParameters()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "punt_flw_";
            const string activityName = "GetActivityName";
            const string calculationName = "locationName";

            var hydraulicBoundaryLocationMock = mockRepository.Stub<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.DesignWaterLevel = new RoundedDouble(2, double.NaN);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var activity = new DesignWaterLevelCalculationActivity(calculationMessageProviderMock,
                                                                   hydraulicBoundaryLocationMock,
                                                                   validFilePath, "", 30);

            using (new DesignWaterLevelCalculationServiceConfig())
            {
                var testService = (TestDesignWaterLevelCalculationService) DesignWaterLevelCalculationService.Instance;

                // Call
                activity.Run();

                // Assert
                Assert.AreSame(calculationMessageProviderMock, testService.MessageProvider);
            }
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationAlreadyRan_ValidationAndCalculationNotPerformedAndStateSkipped()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "locationName";
            const string activityName = "GetActivityName";

            var hydraulicBoundaryLocationMock = mockRepository.Stub<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.DesignWaterLevel = new RoundedDouble(2, 3.0);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            mockRepository.ReplayAll();

            var activity = new DesignWaterLevelCalculationActivity(calculationMessageProviderMock,
                                                                   hydraulicBoundaryLocationMock, validFilePath, "", 30);

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
        public void Finish_ValidCalculationAndRun_SetsDesignWaterLevelAndConvergence()
        {
            // Setup
            const string locationName = "locationName 1";
            var hydraulicBoundaryLocationMock = mockRepository.Stub<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.DesignWaterLevel = new RoundedDouble(2, double.NaN);
            hydraulicBoundaryLocationMock.DesignWaterLevelCalculationConvergence = CalculationConvergence.CalculatedNotConverged;

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return("");
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return("").Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);
            var activity = new DesignWaterLevelCalculationActivity(calculationMessageProviderMock, hydraulicBoundaryLocationMock,
                                                                   validFilePath, "", 30);

            using (new DesignWaterLevelCalculationServiceConfig())
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsFalse(double.IsNaN(hydraulicBoundaryLocationMock.DesignWaterLevel));
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocationMock.DesignWaterLevelCalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_InvalidCalculationAndRun_DoesNotSetDesignWaterlevelAndConvergence()
        {
            // Setup
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            const string locationName = "locationName";
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return("");
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return("").Repeat.AtLeastOnce();

            var hydraulicBoundaryLocationMock = mockRepository.Stub<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.DesignWaterLevel = new RoundedDouble(2, double.NaN);
            hydraulicBoundaryLocationMock.DesignWaterLevelCalculationConvergence = CalculationConvergence.CalculatedConverged;
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);
            var activity = new DesignWaterLevelCalculationActivity(calculationMessageProviderMock, hydraulicBoundaryLocationMock,
                                                                   validFilePath, "", 30);

            using (new DesignWaterLevelCalculationServiceConfig())
            {
                var testService = (TestDesignWaterLevelCalculationService) DesignWaterLevelCalculationService.Instance;
                testService.SetCalculationConvergenceOutput = CalculationConvergence.NotCalculated;

                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNaN(hydraulicBoundaryLocationMock.DesignWaterLevel);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocationMock.DesignWaterLevelCalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_ValidCalculationAndRun_LogWarningNoConvergence()
        {
            // Setup
            const string locationName = "locationName";
            const string activityName = "getActivityName";
            const string calculationNotConvergedMessage = "GetCalculatedNotConvergedMessage";

            var hydraulicBoundaryLocationMock = mockRepository.Stub<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.DesignWaterLevel = new RoundedDouble(2, double.NaN);
            hydraulicBoundaryLocationMock.DesignWaterLevelCalculationConvergence = CalculationConvergence.CalculatedConverged;

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return("GetCalculationName").Repeat.AtLeastOnce();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(calculationNotConvergedMessage);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);
            const int norm = 300;
            var activity = new DesignWaterLevelCalculationActivity(calculationMessageProviderMock, hydraulicBoundaryLocationMock,
                                                                   validFilePath, "", norm);

            using (new DesignWaterLevelCalculationServiceConfig())
            {
                var testService = (TestDesignWaterLevelCalculationService) DesignWaterLevelCalculationService.Instance;
                testService.SetCalculationConvergenceOutput = CalculationConvergence.CalculatedNotConverged;

                activity.Run();
            }

            // Precondition
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocationMock.DesignWaterLevelCalculationConvergence);

            // Call
            Action call = () => activity.Finish();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(calculationNotConvergedMessage, msgs[0]);
                StringAssert.StartsWith(string.Format("Uitvoeren van '{0}' is gelukt.", activityName), msgs[1]);
            });
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, hydraulicBoundaryLocationMock.DesignWaterLevelCalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_CalculationAlreadyRan_FinishNotPerformed()
        {
            // Setup
            RoundedDouble designWaterLevel = (RoundedDouble) 3.0;
            const string locationName = "Name";
            var hydraulicBoundaryLocationMock = mockRepository.StrictMock<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.Name).Return(locationName).Repeat.AtLeastOnce();
            hydraulicBoundaryLocationMock.Expect(hbl => hbl.DesignWaterLevel).Return(designWaterLevel).Repeat.AtLeastOnce();

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return("");
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            var activity = new DesignWaterLevelCalculationActivity(calculationMessageProviderMock, hydraulicBoundaryLocationMock, validFilePath, "", 30);

            using (new DesignWaterLevelCalculationServiceConfig())
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            mockRepository.VerifyAll();
        }
    }
}