// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

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

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, validFilePath, 1, calculationMessageProvider);

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
            TestDelegate call = () => new WaveHeightCalculationActivity(hydraulicBoundaryLocation, validFilePath, 1, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            TestDelegate call = () => new WaveHeightCalculationActivity(null, validFilePath, 1, calculationMessageProvider);

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

            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0);

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation, inValidFilePath, 1, calculationMessageProvider);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{calculationName}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{calculationName}' beëindigd om: ", msgs[2]);
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
            const double norm = 1.0 / 30;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);
            var testWaveHeightCalculator = new TestWaveHeightCalculator
            {
                Converged = true
            };

            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(testWaveHeightCalculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(calculationName);
            mockRepository.ReplayAll();

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             norm, calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, m =>
                {
                    string[] messages = m.ToArray();
                    Assert.AreEqual(5, messages.Length);
                    StringAssert.StartsWith($"Validatie van '{calculationName}' gestart om: ", messages[0]);
                    StringAssert.StartsWith($"Validatie van '{calculationName}' beëindigd om: ", messages[1]);
                    StringAssert.StartsWith($"Berekening van '{calculationName}' gestart om: ", messages[2]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", messages[3]);
                    StringAssert.StartsWith($"Berekening van '{calculationName}' beëindigd om: ", messages[4]);
                });
                WaveHeightCalculationInput waveHeightCalculationInput = testWaveHeightCalculator.ReceivedInputs.First();

                Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), waveHeightCalculationInput.Beta);
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
            const double norm = 1.0 / 30;

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(activityName);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeightOutput = new HydraulicBoundaryLocationOutput(3.0, norm, double.NaN,
                                                                       double.NaN, double.NaN,
                                                                       CalculationConvergence.CalculatedConverged)
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             norm,
                                                             calculationMessageProvider);

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
            const double norm = 1.0 / 30;
            const double expectedWaveHeight = 3.5;

            var testWaveHeightCalculator = new TestWaveHeightCalculator
            {
                Converged = true,
                WaveHeight = expectedWaveHeight
            };

            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(testWaveHeightCalculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityName(locationName)).Return(string.Empty);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(string.Empty).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0);

            string validFilePath = Path.Combine(testDataPath, validFile);

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             norm,
                                                             calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
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
            const string calculationFailedMessage = "Something went wrong";

            var testWaveHeightCalculator = new TestWaveHeightCalculator
            {
                EndInFailure = true,
                LastErrorFileContent = calculationFailedMessage
            };

            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(testWaveHeightCalculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityName(locationName)).Return(string.Empty);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(string.Empty);
            calculationMessageProvider.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeightOutput = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN,
                                                                       double.NaN, double.NaN,
                                                                       double.NaN, CalculationConvergence.CalculatedConverged)
            };

            string validFilePath = Path.Combine(testDataPath, validFile);

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             30,
                                                             calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
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

            var testWaveHeightCalculator = new TestWaveHeightCalculator
            {
                Converged = false
            };

            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(testWaveHeightCalculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return("GetCalculationName").Repeat.AtLeastOnce();
            calculationMessageProvider.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(calculationNotConvergedMessage);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeightOutput = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN,
                                                                       double.NaN, double.NaN,
                                                                       double.NaN, CalculationConvergence.CalculatedConverged)
            };

            string validFilePath = Path.Combine(testDataPath, validFile);
            const double norm = 1.0 / 300;
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             norm, calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
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
            const string calculationFailedMessage = "Something went wrong";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0)
            {
                WaveHeightOutput = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN,
                                                                       double.NaN, double.NaN,
                                                                       double.NaN, CalculationConvergence.NotCalculated)
            };

            var testWaveHeightCalculator = new TestWaveHeightCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(testWaveHeightCalculator);
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityName(locationName)).Return(locationName);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(locationName);
            calculationMessageProvider.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
            calculationMessageProvider.Stub(calc => calc.GetCalculationFailedUnexplainedMessage(locationName)).Return(calculationFailedMessage);
            calculationMessageProvider.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(locationName);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            const double norm = 1.0 / 30;
            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                             validFilePath,
                                                             norm,
                                                             calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(ActivityState.Executed)]
        [TestCase(ActivityState.Failed)]
        [TestCase(ActivityState.Canceled)]
        [TestCase(ActivityState.Skipped)]
        public void Finish_ActivityWithSpecificState_NotifyHydraulicBoundaryLocation(ActivityState state)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            hydraulicBoundaryLocation.Attach(observer);
            mockRepository.ReplayAll();

            var activity = new TestWaveHeightCalculationActivity(hydraulicBoundaryLocation,
                                                                 Path.Combine(testDataPath, validFile),
                                                                 1.0,
                                                                 calculationMessageProvider,
                                                                 state);

            // Call
            activity.Finish();

            // Assert
            mockRepository.VerifyAll();
        }

        private class TestWaveHeightCalculationActivity : WaveHeightCalculationActivity
        {
            public TestWaveHeightCalculationActivity(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                     string hydraulicBoundaryDatabaseFilePath,
                                                     double norm,
                                                     ICalculationMessageProvider messageProvider,
                                                     ActivityState state)
                : base(hydraulicBoundaryLocation,
                       hydraulicBoundaryDatabaseFilePath,
                       norm,
                       messageProvider)
            {
                State = state;
            }
        }
    }
}