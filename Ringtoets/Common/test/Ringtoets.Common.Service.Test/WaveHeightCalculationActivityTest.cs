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
    public class WaveHeightCalculationActivityTest
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
            
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, validFilePath, "", 1, calculationMessageProviderMock);

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
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            TestDelegate call = () => new WaveHeightCalculationActivity(hydraulicBoundaryLocation, validFilePath, "", 1, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            TestDelegate call = () => new WaveHeightCalculationActivity(null, validFilePath, "", 1, calculationMessageProviderMock);

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

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, double.NaN)
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, inValidFilePath, "", 1, calculationMessageProviderMock);

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
        public void Run_ValidHydraulicBoundaryLocation_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "locationName";
            const string activityName = "GetActivityName";
            const string calculationName = "locationName";
            const string ringId = "11-1";
            const double norm = 30;

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, double.NaN)
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             ringId,
                                                             norm, calculationMessageProviderMock);

            using (new WaveHeightCalculationServiceConfig())
            {
                var testService = (TestHydraulicBoundaryLocationCalculationService) WaveHeightCalculationService.Instance;

                // Call
                activity.Run();

                // Assert
                Assert.AreSame(calculationMessageProviderMock, testService.MessageProvider);
                Assert.AreSame(hydraulicBoundaryLocation, testService.HydraulicBoundaryLocation);
                Assert.AreEqual(validFilePath, testService.HydraulicBoundaryDatabaseFilePath);
                Assert.AreEqual(ringId, testService.RingId);
                Assert.AreEqual(norm, testService.Norm);
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

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, 3.0)
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             string.Empty,
                                                             30,
                                                             calculationMessageProviderMock);

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
            const string locationName = "locationName";

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return("");
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return("").Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, double.NaN),
                WaveHeightCalculationConvergence = CalculationConvergence.CalculatedNotConverged
            };

            string validFilePath = Path.Combine(testDataPath, validFile);

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             string.Empty,
                                                             30,
                                                             calculationMessageProviderMock);

            using (new WaveHeightCalculationServiceConfig())
            {
                activity.Run();
            }

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
            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            const string locationName = "locationName";
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return("");
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return("").Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, double.NaN),
                WaveHeightCalculationConvergence = CalculationConvergence.CalculatedConverged
            };

            string validFilePath = Path.Combine(testDataPath, validFile);

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             string.Empty,
                                                             30,
                                                             calculationMessageProviderMock);

            using (new WaveHeightCalculationServiceConfig())
            {
                var testService = (TestHydraulicBoundaryLocationCalculationService) WaveHeightCalculationService.Instance;
                testService.CalculationConvergenceOutput = CalculationConvergence.NotCalculated;

                activity.Run();
            }

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
            const string locationName = "locationName";
            const string activityName = "getActivityName";
            const string calculationNotConvergedMessage = "GetCalculatedNotConvergedMessage";

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return("GetCalculationName").Repeat.AtLeastOnce();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(calculationNotConvergedMessage);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, double.NaN),
                WaveHeightCalculationConvergence = CalculationConvergence.CalculatedConverged
            };

            string validFilePath = Path.Combine(testDataPath, validFile);
            const int norm = 300;
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             string.Empty, 
                                                             norm, calculationMessageProviderMock);

            using (new WaveHeightCalculationServiceConfig())
            {
                var testService = (TestHydraulicBoundaryLocationCalculationService) WaveHeightCalculationService.Instance;
                testService.CalculationConvergenceOutput = CalculationConvergence.CalculatedNotConverged;
                activity.Run();
            }

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
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Finish_CalculationAlreadyRan_FinishNotPerformed()
        {
            // Setup
            RoundedDouble waveHeight = (RoundedDouble) 3.0;
            const string locationName = "Name";

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return("");
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeight = waveHeight
            };

            string validFilePath = Path.Combine(testDataPath, validFile);

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             string.Empty,
                                                             30,
                                                             calculationMessageProviderMock);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            mockRepository.VerifyAll();
        }
    }
}