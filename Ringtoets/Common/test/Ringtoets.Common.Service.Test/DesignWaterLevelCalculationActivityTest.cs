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
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
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
            const string activityName = "GetActivityName";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   string.Empty,
                                                                   1,
                                                                   calculationMessageProviderMock);

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
            TestDelegate call = () => new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                              validFilePath,
                                                                              string.Empty,
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
                                                                              string.Empty,
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
            const string activityName = "GetActivityName";
            const string calculationName = "locationName";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return(calculationName).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   inValidFilePath,
                                                                   string.Empty,
                                                                   1,
                                                                   calculationMessageProviderMock);

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
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidHydraulicBoundaryDatabaseAndHydraulicBoundaryLocation_PerformValidationValidParameters()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "punt_flw_";
            const string activityName = "GetActivityName";
            const string calculationName = "locationName";
            const string ringId = "11-1";
            const double norm = 1.0/30;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);

            var calculationMessageProviderMock = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(calculationName);
            mockRepository.ReplayAll();

            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   ringId,
                                                                   norm,
                                                                   calculationMessageProviderMock);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testDesignWaterLevelCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DesignWaterLevelCalculator;
                testDesignWaterLevelCalculator.ReliabilityIndex = StatisticsConverter.ProbabilityToReliability(norm);

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
                    StringAssert.StartsWith("Toetspeil berekening is uitgevoerd op de tijdelijke locatie", messages[3]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculationName), messages[4]);
                });

                var designWaterLevelCalculationInput = testDesignWaterLevelCalculator.ReceivedInputs.First();

                Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(testDataPath, testDesignWaterLevelCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(ringId, testDesignWaterLevelCalculator.RingId);
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
            const string activityName = "GetActivityName";
            const double norm = 1.0/30;

            var calculationMessageProviderMock = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(activityName);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0)
            {
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(3.0, norm, double.NaN,
                                                                             double.NaN, double.NaN,
                                                                             CalculationConvergence.CalculatedConverged)
            };

            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   string.Empty,
                                                                   norm,
                                                                   calculationMessageProviderMock);

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
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(string.Empty);
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return(string.Empty).Repeat.AtLeastOnce();
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            const double norm = 1.0/30;
            double expectedDesignWaterLevel = 3.5;

            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   string.Empty,
                                                                   norm,
                                                                   calculationMessageProviderMock);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testDesignWaterLevelCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DesignWaterLevelCalculator;
                testDesignWaterLevelCalculator.DesignWaterLevel = expectedDesignWaterLevel;
                testDesignWaterLevelCalculator.ReliabilityIndex = StatisticsConverter.ProbabilityToReliability(norm);

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
            string calculationFailedMessage = "Something went wrong";

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(string.Empty);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(string.Empty);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
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
                                                                   string.Empty,
                                                                   30,
                                                                   calculationMessageProviderMock);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DesignWaterLevelCalculator;
                calculator.EndInFailure = true;
                calculator.LastErrorFileContent = calculationFailedMessage;

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
            const string activityName = "getActivityName";
            const string calculationNotConvergedMessage = "GetCalculatedNotConvergedMessage";

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Expect(calc => calc.GetActivityName(locationName)).Return(activityName);
            calculationMessageProviderMock.Expect(calc => calc.GetCalculationName(locationName)).Return("GetCalculationName").Repeat.AtLeastOnce();
            calculationMessageProviderMock.Expect(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(calculationNotConvergedMessage);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0)
            {
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN,
                                                                             double.NaN, double.NaN,
                                                                             double.NaN, CalculationConvergence.CalculatedConverged)
            };

            string validFilePath = Path.Combine(testDataPath, validFile);
            const double norm = 1.0/300;
            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   string.Empty,
                                                                   norm,
                                                                   calculationMessageProviderMock);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DesignWaterLevelCalculator;
                calculator.ReliabilityIndex = 3;

                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
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

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetActivityName(locationName)).Return(locationName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(locationName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationFailedUnexplainedMessage(locationName)).Return(calculationFailedMessage);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(locationName);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            const double norm = 1.0/30;
            var activity = new DesignWaterLevelCalculationActivity(hydraulicBoundaryLocation,
                                                                   validFilePath,
                                                                   string.Empty,
                                                                   norm,
                                                                   calculationMessageProviderMock);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DesignWaterLevelCalculator;
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