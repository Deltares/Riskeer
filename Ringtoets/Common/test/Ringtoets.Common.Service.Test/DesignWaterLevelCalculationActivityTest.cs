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
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

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
            const string activityDescription = "GetActivityDescription";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);

            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Expect(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   1,
                                                                   calculationMessageProvider);

            // Assert
            Assert.IsInstanceOf<HydraRingActivityBase>(activity);
            Assert.AreSame(activityDescription, activity.Description);
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
            TestDelegate call = () => new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                              validFilePath,
                                                                              1,
                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMessageProviderStub = mockRepository.Stub<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationActivity(null,
                                                                              validFilePath,
                                                                              1,
                                                                              calculationMessageProviderStub);

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
            const string activityDescription = "activityDescription";
            const string calculationName = "locationName";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0);

            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Expect(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            calculationMessageProvider.Expect(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   inValidFilePath,
                                                                   1,
                                                                   calculationMessageProvider);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                Assert.AreEqual($"Validatie van '{calculationName}' gestart.", msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                Assert.AreEqual($"Validatie van '{calculationName}' beëindigd.", msgs[2]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidHydraulicBoundaryDatabaseAndHydraulicBoundaryLocation_PerformValidationValidParameters()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "punt_flw_";
            const string activityDescription = "activityDescription";
            const string calculationName = "locationName";
            const double norm = 1.0 / 30;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);
            var calculator = new TestDesignWaterLevelCalculator
            {
                Converged = true
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(calculationName);
            mockRepository.ReplayAll();

            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   norm,
                                                                   calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, m =>
                {
                    string[] messages = m.ToArray();
                    Assert.AreEqual(5, messages.Length);
                    Assert.AreEqual($"Validatie van '{calculationName}' gestart.", messages[0]);
                    Assert.AreEqual($"Validatie van '{calculationName}' beëindigd.", messages[1]);
                    Assert.AreEqual($"Berekening van '{calculationName}' gestart.", messages[2]);
                    StringAssert.StartsWith("Toetspeil berekening is uitgevoerd op de tijdelijke locatie", messages[3]);
                    Assert.AreEqual($"Berekening van '{calculationName}' beëindigd.", messages[4]);
                });

                AssessmentLevelCalculationInput designWaterLevelCalculationInput = calculator.ReceivedInputs.First();

                Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), designWaterLevelCalculationInput.Beta);
            }
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_HydraulicLocationDesignWaterLevelSet_ValidationAndCalculationNotPerformedAndStateSkipped()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "locationName";
            const string activityDescription = "activityDescription";
            const double norm = 1.0 / 30;

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(activityDescription);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0)
            {
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(3.0, norm, double.NaN,
                                                                             double.NaN, double.NaN,
                                                                             CalculationConvergence.CalculatedConverged)
            };

            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
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
        public void Run_ValidCalculationAndRun_SetsDesignWaterLevelAndConvergence()
        {
            // Setup
            const string locationName = "locationName 1";
            const double norm = 1.0 / 30;
            const double expectedDesignWaterLevel = 3.5;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);
            var calculator = new TestDesignWaterLevelCalculator
            {
                DesignWaterLevel = expectedDesignWaterLevel,
                Converged = true
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Expect(calc => calc.GetActivityDescription(locationName)).Return(string.Empty);
            calculationMessageProvider.Expect(calc => calc.GetCalculationName(locationName)).Return(string.Empty).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   norm,
                                                                   calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            Assert.AreEqual(expectedDesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidCalculation_LogsErrorOutputNotUpdated()
        {
            // Setup
            const string locationName = "locationName";
            const string calculationFailedMessage = "Something went wrong";

            var calculator = new TestDesignWaterLevelCalculator
            {
                EndInFailure = true,
                LastErrorFileContent = calculationFailedMessage
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(string.Empty);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(string.Empty);
            calculationMessageProvider.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0)
            {
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN,
                                                                             double.NaN, double.NaN,
                                                                             double.NaN, CalculationConvergence.CalculatedConverged)
            };

            string validFilePath = Path.Combine(testDataPath, validFile);
            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   30,
                                                                   calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, calculationFailedMessage, 6);
                Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
                Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationResultingInNoConvergence_LogWarningNoConvergence()
        {
            // Setup
            const string locationName = "locationName";
            const string activityDescription = "activityDescription";
            const string calculationNotConvergedMessage = "GetCalculatedNotConvergedMessage";

            var calculator = new TestDesignWaterLevelCalculator
            {
                Converged = false
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Expect(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            calculationMessageProvider.Expect(calc => calc.GetCalculationName(locationName)).Return("GetCalculationName").Repeat.AtLeastOnce();
            calculationMessageProvider.Expect(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(calculationNotConvergedMessage);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0)
            {
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN,
                                                                             double.NaN, double.NaN,
                                                                             double.NaN, CalculationConvergence.CalculatedConverged)
            };

            string validFilePath = Path.Combine(testDataPath, validFile);
            const double norm = 1.0 / 300;
            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   norm,
                                                                   calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(6, msgs.Length);
                    StringAssert.StartsWith(calculationNotConvergedMessage, msgs[3]);
                });
                Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
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
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN,
                                                                             double.NaN, double.NaN,
                                                                             double.NaN, CalculationConvergence.NotCalculated)
            };

            var calculator = new TestDesignWaterLevelCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(locationName);
            calculationMessageProvider.Stub(calc => calc.GetCalculationName(locationName)).Return(locationName);
            calculationMessageProvider.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
            calculationMessageProvider.Stub(calc => calc.GetCalculationFailedUnexplainedMessage(locationName)).Return(calculationFailedMessage);
            calculationMessageProvider.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(locationName);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            const double norm = 1.0 / 30;
            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
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

            var activity = new TestDesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                       Path.Combine(testDataPath, validFile),
                                                                       1.0,
                                                                       calculationMessageProvider,
                                                                       state);

            // Call
            activity.Finish();

            // Assert
            mockRepository.VerifyAll();
        }

        private class TestDesignWaterLevelCalculationActivity : DesignWaterLevelCalculationActivity
        {
            public TestDesignWaterLevelCalculationActivity(HydraulicBoundaryLocation hydraulicBoundaryLocation,
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