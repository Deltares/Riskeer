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
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
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

            var calculationMessageProviderMock = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, validFilePath, string.Empty, 1, calculationMessageProviderMock);

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
            TestDelegate call = () => new WaveHeightCalculationActivity(hydraulicBoundaryLocation, validFilePath, string.Empty, 1, null);

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
            TestDelegate call = () => new WaveHeightCalculationActivity(null, validFilePath, string.Empty, 1, calculationMessageProviderMock);

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
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, double.NaN)
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, inValidFilePath, string.Empty, 1, calculationMessageProviderMock);

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

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, double.NaN)
            };

            var calculationMessageProviderMock = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(calculationName);
            mockRepository.ReplayAll();

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             ringId,
                                                             norm, calculationMessageProviderMock);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testWaveHeightCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveHeightCalculator;
                testWaveHeightCalculator.ReliabilityIndex = StatisticsConverter.NormToBeta(norm);

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, m =>
                {
                    var messages = m.ToArray();
                    Assert.AreEqual(5, messages.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculationName), messages[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculationName), messages[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculationName), messages[2]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", messages[3]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), messages[4]);
                });
                var waveHeightCalculationInput = testWaveHeightCalculator.ReceivedInputs.First();

                Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(testDataPath, testWaveHeightCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(ringId, testWaveHeightCalculator.RingId);
                Assert.AreEqual(StatisticsConverter.NormToBeta(norm), waveHeightCalculationInput.Beta);
            }
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_HydraulicLocationWaveHeightSet_ValidationAndCalculationNotPerformedAndStateSkipped()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "locationName";
            const string activityName = "GetActivityName";

            var calculationMessageProviderMock = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(activityName);
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
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.AreEqual(ActivityState.Skipped, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidCalculation_SetsWaveHeightAndConvergence()
        {
            // Setup
            const string locationName = "locationName";

            var calculationMessageProviderMock = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(string.Empty);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(string.Empty).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, double.NaN),
                WaveHeightCalculationConvergence = CalculationConvergence.CalculatedNotConverged
            };

            string validFilePath = Path.Combine(testDataPath, validFile);

            var norm = 30;
            double expectedWaveHeight = 3.5;

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             string.Empty,
                                                             norm,
                                                             calculationMessageProviderMock);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testWaveHeightCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveHeightCalculator;
                testWaveHeightCalculator.WaveHeight = expectedWaveHeight;
                testWaveHeightCalculator.ReliabilityIndex = StatisticsConverter.NormToBeta(norm);

                // Call
                activity.Run();
            }

            // Assert
            Assert.AreEqual(expectedWaveHeight, hydraulicBoundaryLocation.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidCalculation_LogsErrorOutputNotUpdated()
        {
            // Setup
            const string locationName = "locationName";
            string calculationFailedMessage = "Something went wrong";

            var calculationMessageProviderMock = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(string.Empty);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(string.Empty);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveHeightCalculator;
                calculator.EndInFailure = true;
                calculator.LastErrorFileContent = calculationFailedMessage;

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, calculationFailedMessage, 6);
                Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
                Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationResultingInNoConvergence_LogWarningNoConvergence()
        {
            // Setup
            const string locationName = "locationName";
            const string activityName = "getActivityName";
            const string calculationNotConvergedMessage = "GetCalculatedNotConvergedMessage";

            var calculationMessageProviderMock = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return("GetCalculationName").Repeat.AtLeastOnce();
            calculationMessageProviderMock.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(calculationNotConvergedMessage);
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveHeightCalculator;
                calculator.ReliabilityIndex = 3;

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(6, msgs.Length);
                    StringAssert.StartsWith(calculationNotConvergedMessage, msgs[3]);
                });
                Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, null)]
        [TestCase(true, "An error occurred")]
        [TestCase(false, "An error occurred")]
        public void Run_ErrorInCalculation_PerformValidationAndCalculationAndLogStartAndEndAndError(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            const string locationName = "locationName 1";
            string calculationFailedMessage = "Something went wrong";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0)
            {
                WaveHeight = new RoundedDouble(2, double.NaN),
                WaveHeightCalculationConvergence = CalculationConvergence.NotCalculated
            };

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(locationName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(locationName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationFailedUnexplainedMessage(locationName)).Return(calculationFailedMessage);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(locationName);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            var norm = 30;

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             string.Empty,
                                                             norm,
                                                             calculationMessageProviderMock);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).WaveHeightCalculator;
                calculator.EndInFailure = endInFailure;
                calculator.LastErrorFileContent = lastErrorFileContent;

                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
            mockRepository.VerifyAll();
        }
    }
}