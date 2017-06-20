﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class WaveHeightCalculationServiceTest
    {
        private const string validFile = "HRD dutch coast south.sqlite";
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabase_ReturnsTrue()
        {
            // Setup
            const string calculationName = "calculationName";
            string validFilePath = Path.Combine(testDataPath, validFile);
            var valid = false;

            var mockRepository = new MockRepository();
            var messageProviderStub = mockRepository.Stub<ICalculationMessageProvider>();
            messageProviderStub.Stub(mp => mp.GetCalculationName(calculationName)).Return(calculationName);
            mockRepository.ReplayAll();

            // Call
            Action call = () => valid = WaveHeightCalculationService.Validate(calculationName, validFilePath, messageProviderStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(calculationName, msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(calculationName, msgs[1]);
            });
            Assert.IsTrue(valid);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string calculationName = "calculationName";
            string notValidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            var valid = true;

            var mockRepository = new MockRepository();
            var messageProviderStub = mockRepository.Stub<ICalculationMessageProvider>();
            messageProviderStub.Stub(mp => mp.GetCalculationName(calculationName)).Return(calculationName);
            mockRepository.ReplayAll();

            // Call
            Action call = () => valid = WaveHeightCalculationService.Validate(calculationName, notValidFilePath, messageProviderStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(calculationName, msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(calculationName, msgs[2]);
            });
            Assert.IsFalse(valid);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string calculationName = "calculationName";
            string validFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");
            var valid = false;

            var mockRepository = new MockRepository();
            var messageProviderStub = mockRepository.Stub<ICalculationMessageProvider>();
            messageProviderStub.Stub(mp => mp.GetCalculationName(calculationName)).Return(calculationName);
            mockRepository.ReplayAll();

            // Call
            Action call = () => valid = WaveHeightCalculationService.Validate(calculationName, validFilePath, messageProviderStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(calculationName, msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(calculationName, msgs[2]);
            });
            Assert.IsFalse(valid);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_WaveHeightCalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProviderStub = mockRepository.Stub<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new WaveHeightCalculationService().Calculate(null, string.Empty, 1, calculationMessageProviderStub);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("waveHeightCalculation", exception.ParamName);
        }

        [Test]
        public void Calculate_ValidHydraulicBoundaryLocation_StartsCalculationWithRightParameters()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            const long id = 100;
            const string locationName = "punt_flw_ 1";
            const double norm = 1.0 / 30;

            var calculator = new TestWaveHeightCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);

            var calculation = mockRepository.Stub<IWaveHeightCalculation>();
            calculation.Stub(c => c.GetName()).Return(locationName);
            calculation.Expect(c => c.GetId()).Return(id);
            calculation.Expect(c => c.SetOutput(null)).Constraints(new TypeOf(typeof(HydraulicBoundaryLocationOutput)));

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(string.Empty);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(string.Empty);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                new WaveHeightCalculationService().Calculate(calculation,
                                                             validFilePath,
                                                             norm,
                                                             calculationMessageProviderMock);

                // Assert
                AssessmentLevelCalculationInput expectedInput = CreateInput(id, norm);
                AssertInput(expectedInput, calculator.ReceivedInputs.Single());
                Assert.IsFalse(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            var calculator = new TestWaveHeightCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);

            var calculation = mockRepository.Stub<IWaveHeightCalculation>();
            calculation.Stub(c => c.GetName()).Return("name");
            calculation.Expect(c => c.GetId()).Return(0);

            var calculationMessageProviderStub = mockRepository.Stub<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            const double norm = 1.0 / 30;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new WaveHeightCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  validFilePath,
                                  norm,
                                  calculationMessageProviderStub);

                // Assert
                Assert.IsTrue(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            const string locationName = "punt_flw_ 1";
            const string calculationName = "calculationName";
            const string calculationFailedMessage = "calculationFailedMessage";
            const double norm = 1.0 / 30;

            var calculator = new TestWaveHeightCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);

            var calculation = mockRepository.Stub<IWaveHeightCalculation>();
            calculation.Stub(c => c.GetName()).Return(locationName);
            calculation.Expect(c => c.GetId()).Return(0);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(calculationName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new WaveHeightCalculationService().Calculate(calculation,
                                                                     validFilePath,
                                                                     norm,
                                                                     calculationMessageProviderMock);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculationName, msgs[0]);
                    StringAssert.StartsWith(calculationFailedMessage, msgs[1]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(calculationName, msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            const string locationName = "punt_flw_ 1";
            const string calculationName = "calculationName";
            const string calculationFailedUnexplainedMessage = "calculationFailedUnexplainedMessage";
            const double norm = 1.0 / 30;

            var calculator = new TestWaveHeightCalculator
            {
                EndInFailure = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);

            var calculation = mockRepository.Stub<IWaveHeightCalculation>();
            calculation.Stub(c => c.GetName()).Return(locationName);
            calculation.Expect(c => c.GetId()).Return(0);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(calculationName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationFailedUnexplainedMessage(locationName)).Return(calculationFailedUnexplainedMessage);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new WaveHeightCalculationService().Calculate(calculation,
                                                                     validFilePath,
                                                                     norm,
                                                                     calculationMessageProviderMock);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculationName, msgs[0]);
                    StringAssert.StartsWith(calculationFailedUnexplainedMessage, msgs[1]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(calculationName, msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            const string locationName = "punt_flw_ 1";
            const string calculationName = "calculationName";
            const string calculationFailedMessage = "calculationFailedMessage";
            const double norm = 1.0 / 30;

            var calculator = new TestWaveHeightCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = false
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);

            var calculation = mockRepository.Stub<IWaveHeightCalculation>();
            calculation.Stub(c => c.GetName()).Return(locationName);
            calculation.Expect(c => c.GetId()).Return(0);

            var calculationMessageProviderMock = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationName(locationName)).Return(calculationName);
            calculationMessageProviderMock.Stub(calc => calc.GetCalculationFailedMessage(null, null)).IgnoreArguments().Return(calculationFailedMessage);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;
                string exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new WaveHeightCalculationService().Calculate(calculation,
                                                                     validFilePath,
                                                                     norm,
                                                                     calculationMessageProviderMock);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exceptionThrown = true;
                        exceptionMessage = e.Message;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculationName, msgs[0]);
                    StringAssert.StartsWith(calculationFailedMessage, msgs[1]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(calculationName, msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }
            mockRepository.VerifyAll();
        }

        private static void AssertInput(AssessmentLevelCalculationInput expectedInput, HydraRingCalculationInput hydraRingCalculationInput)
        {
            Assert.AreEqual(expectedInput.Section.SectionId, hydraRingCalculationInput.Section.SectionId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, hydraRingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Beta, hydraRingCalculationInput.Beta);
        }

        private static AssessmentLevelCalculationInput CreateInput(long hydraulicBoundaryLocationId, double norm)
        {
            return new AssessmentLevelCalculationInput(1, hydraulicBoundaryLocationId, norm);
        }
    }
}