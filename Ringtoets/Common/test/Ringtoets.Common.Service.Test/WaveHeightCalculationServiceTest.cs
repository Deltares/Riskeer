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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

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
            string validFilePath = Path.Combine(testDataPath, validFile);
            var valid = false;

            // Call
            Action call = () => valid = WaveHeightCalculationService.Validate(validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
            Assert.IsTrue(valid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            string notValidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            var valid = true;

            // Call
            Action call = () => valid = WaveHeightCalculationService.Validate(notValidFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsErrorAndReturnsFalse()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");
            var valid = false;

            // Call
            Action call = () => valid = WaveHeightCalculationService.Validate(validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Calculate_WaveHeightCalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new WaveHeightCalculationService().Calculate(null, string.Empty, 1, calculationMessageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("waveHeightCalculation", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidWaveHeightCalculation_StartsCalculationWithRightParameters(bool readIllustrationPoints)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            const long id = 100;
            const string locationName = "punt_flw_ 1";
            const double norm = 1.0 / 30;

            var calculator = new TestWaveHeightCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);

            var calculation = mockRepository.Stub<IHydraulicBoundaryWrapperCalculation>();
            calculation.Stub(c => c.Name).Return(locationName);
            calculation.Expect(c => c.Id).Return(id);
            calculation.Expect(c => c.CalculateIllustrationPoints).Return(readIllustrationPoints);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(string.Empty);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                new WaveHeightCalculationService().Calculate(calculation,
                                                             validFilePath,
                                                             norm,
                                                             calculationMessageProvider);

                // Assert
                AssessmentLevelCalculationInput expectedInput = CreateInput(id, norm);
                AssertInput(expectedInput, calculator.ReceivedInputs.Single());
                Assert.IsFalse(calculator.IsCanceled);

                Assert.AreEqual(readIllustrationPoints, calculation.Output.HasIllustrationPoints);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidWaveHeightCalculationThrowsException_ThrowsHydraRingFileParserException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            const string locationName = "punt_flw_ 1";
            var expectedException = new HydraRingFileParserException();

            var mockRepository = new MockRepository();
            var calculator = mockRepository.Stub<IWaveHeightCalculator>();
            calculator.Expect(c => c.Calculate(Arg<WaveHeightCalculationInput>.Is.TypeOf))
                      .Throw(expectedException);
            calculator.Stub(c => c.LastErrorFileContent).Return(string.Empty);
            calculator.Stub(c => c.OutputDirectory).Return(string.Empty);

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);

            var calculation = mockRepository.Stub<IHydraulicBoundaryWrapperCalculation>();
            calculation.Stub(c => c.Name).Return(locationName);
            calculation.Expect(c => c.Id).Return(100);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(string.Empty);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                TestDelegate call = () => new WaveHeightCalculationService()
                    .Calculate(calculation,
                               validFilePath,
                               1.0 / 30,
                               calculationMessageProvider);

                // Assert
                var thrownException = Assert.Throws<HydraRingFileParserException>(call);
                Assert.AreSame(expectedException, thrownException);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidWaveHeightCalculationWithIllustrationPointsButIsNull_IllustrationPointsNotSet()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            const string locationName = "punt_flw_ 1";

            var mockRepository = new MockRepository();
            var calculator = new TestWaveHeightCalculator();

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);

            var calculation = mockRepository.Stub<IHydraulicBoundaryWrapperCalculation>();
            calculation.Stub(c => c.Name).Return(locationName);
            calculation.Expect(c => c.Id).Return(100);
            calculation.Expect(c => c.CalculateIllustrationPoints).Return(true);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(string.Empty);
            calculationMessageProvider.Stub(calc => calc.GetCalculationFailedMessage(
                                                locationName, "Er konden geen illustratiepunten worden uitgelezen."))
                                      .Return(string.Empty);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new WaveHeightCalculationService()
                    .Calculate(calculation,
                               validFilePath,
                               1.0 / 30,
                               calculationMessageProvider);

                // Assert
                Assert.IsFalse(calculation.Output.HasIllustrationPoints);
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

            var calculation = mockRepository.Stub<IHydraulicBoundaryWrapperCalculation>();
            calculation.Stub(c => c.Name).Return("name");
            calculation.Expect(c => c.Id).Return(0);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
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
                                  calculationMessageProvider);

                // Assert
                Assert.IsTrue(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Run_InvalidCalculation_LogsErrorAndThrowException)
        })]
        public void Run_InvalidCalculation_LogsErrorAndThrowException(bool endInFailure, string lastErrorFileContent, string detailedReport)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);

            const string locationName = "punt_flw_ 1";
            const string calculationFailedMessage = "calculationFailedMessage";
            const double norm = 1.0 / 30;

            var calculator = new TestWaveHeightCalculator
            {
                LastErrorFileContent = lastErrorFileContent,
                EndInFailure = endInFailure
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);

            var calculation = mockRepository.Stub<IHydraulicBoundaryWrapperCalculation>();
            calculation.Stub(c => c.Name).Return(locationName);
            calculation.Expect(c => c.Id).Return(0);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Expect(calc => calc.GetCalculationFailedMessage(locationName,
                                                                                       endInFailure && string.IsNullOrEmpty(lastErrorFileContent)
                                                                                           ? calculator.HydraRingCalculationException.Message
                                                                                           : lastErrorFileContent
                                              )).Return(calculationFailedMessage);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new WaveHeightCalculationService().Calculate(calculation,
                                                                     validFilePath,
                                                                     norm,
                                                                     calculationMessageProvider);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exception = e;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(calculationFailedMessage, msgs[1]);
                    Assert.AreEqual($"Golfhoogte berekening is uitgevoerd op de tijdelijke locatie '{calculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsInstanceOf<HydraRingCalculationException>(exception);
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