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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.DuneErosion.Service.Test
{
    [TestFixture]
    public class DuneErosionBoundaryCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Validate_ValidPaths_ReturnsTrue()
        {
            // Setup
            var valid = false;

            // Call
            Action call = () => valid = DuneErosionBoundaryCalculationService.Validate(validFilePath, validPreprocessorDirectory);

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
        public void Validate_InvalidHydraulicBoundaryDatabasePath_LogsErrorAndReturnsFalse()
        {
            // Setup
            string notValidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            var valid = true;

            // Call
            Action call = () => valid = DuneErosionBoundaryCalculationService.Validate(notValidFilePath, validPreprocessorDirectory);

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
            string notValidFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");
            var valid = false;

            // Call
            Action call = () => valid = DuneErosionBoundaryCalculationService.Validate(notValidFilePath, validPreprocessorDirectory);

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
        public void Validate_InvalidPreprocessorDirectory_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string notValidPreprocessorDirectory = "Preprocessor";
            var valid = true;

            // Call
            Action call = () => valid = DuneErosionBoundaryCalculationService.Validate(validFilePath, notValidPreprocessorDirectory);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void Calculate_DuneLocationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneErosionBoundaryCalculationService().Calculate(
                null,
                1,
                validFilePath,
                validPreprocessorDirectory);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneLocation", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidData_CalculationStartedWithRightParameters(bool usePreprocessor)
        {
            // Setup
            string preprocessorDirectory = usePreprocessor
                                               ? validPreprocessorDirectory
                                               : string.Empty;

            var calculator = new TestDunesBoundaryConditionsCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, preprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            double mechanismSpecificNorm = failureMechanism.GetMechanismSpecificNorm(1.0 / 200);

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new DuneErosionBoundaryCalculationService().Calculate(
                    duneLocation,
                    mechanismSpecificNorm,
                    validFilePath,
                    preprocessorDirectory);

                // Assert
                DunesBoundaryConditionsCalculationInput expectedInput = CreateInput(duneLocation, mechanismSpecificNorm);
                DunesBoundaryConditionsCalculationInput actualInput = calculator.ReceivedInputs.Single();
                AssertInput(expectedInput, actualInput);
                Assert.AreEqual(usePreprocessor, actualInput.PreprocessorSetting.RunPreprocessor);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationRan_SetOutput()
        {
            // Setup
            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                ReliabilityIndex = 3.27052,
                WaterLevel = 4.82912,
                WaveHeight = 2.88936,
                WavePeriod = 10.65437,
                Converged = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            double mechanismSpecificNorm = failureMechanism.GetMechanismSpecificNorm(1.0 / 200);

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            // Precondition
            Assert.IsNull(duneLocation.Output);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action test = () => new DuneErosionBoundaryCalculationService().Calculate(
                    duneLocation,
                    mechanismSpecificNorm,
                    validFilePath,
                    validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessages(
                    test,
                    messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(3, msgs.Length);

                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                        StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                    });
                double targetReliability = StatisticsConverter.ProbabilityToReliability(mechanismSpecificNorm);
                double calculatedProbability = StatisticsConverter.ReliabilityToProbability(calculator.ReliabilityIndex);

                Assert.IsNotNull(duneLocation.Output);
                Assert.AreEqual(calculator.ReliabilityIndex, duneLocation.Output.CalculatedReliability.Value);
                Assert.AreEqual(calculatedProbability, duneLocation.Output.CalculatedProbability);
                Assert.AreEqual(mechanismSpecificNorm, duneLocation.Output.TargetProbability);
                Assert.AreEqual(targetReliability, duneLocation.Output.TargetReliability, duneLocation.Output.TargetReliability.GetAccuracy());
                Assert.AreEqual(calculator.WaterLevel, duneLocation.Output.WaterLevel, duneLocation.Output.WaterLevel.GetAccuracy());
                Assert.AreEqual(calculator.WaveHeight, duneLocation.Output.WaveHeight, duneLocation.Output.WaveHeight.GetAccuracy());
                Assert.AreEqual(calculator.WavePeriod, duneLocation.Output.WavePeriod, duneLocation.Output.WavePeriod.GetAccuracy());
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationRanNotConverged_LogMessage()
        {
            // Setup
            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                ReliabilityIndex = 0.01
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action test = () => new DuneErosionBoundaryCalculationService().Calculate(
                    duneLocation,
                    failureMechanism.GetMechanismSpecificNorm(1.0 / 200),
                    validFilePath,
                    validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessages(
                    test,
                    messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(4, msgs.Length);

                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                        Assert.AreEqual($"Hydraulische randvoorwaarden berekening voor locatie '{duneLocation.Name}' is niet geconvergeerd.", msgs[1]);
                        StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                    });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            var calculator = new TestDunesBoundaryConditionsCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new DuneErosionBoundaryCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(
                    duneLocation,
                    failureMechanism.GetMechanismSpecificNorm(1.0 / 200),
                    validFilePath,
                    validPreprocessorDirectory);

                // Assert
                Assert.IsTrue(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DuneErosionBoundaryCalculationService().Calculate(
                            duneLocation,
                            failureMechanism.GetMechanismSpecificNorm(1.0 / 200),
                            validFilePath,
                            validPreprocessorDirectory);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(
                    call,
                    messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(4, msgs.Length);

                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                        StringAssert.StartsWith($"Hydraulische randvoorwaarden berekening voor locatie '{duneLocation.Name}' is mislukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                        StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                    });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(duneLocation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                EndInFailure = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DuneErosionBoundaryCalculationService().Calculate(
                            duneLocation,
                            failureMechanism.GetMechanismSpecificNorm(1.0 / 200),
                            validFilePath,
                            validPreprocessorDirectory);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(
                    call,
                    messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(4, msgs.Length);

                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                        Assert.AreEqual($"Hydraulische randvoorwaarden berekening voor locatie '{duneLocation.Name}' is mislukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                        StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                    });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(duneLocation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                EndInFailure = false,
                LastErrorFileContent = "An error occurred"
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;
                string exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DuneErosionBoundaryCalculationService().Calculate(
                            duneLocation,
                            failureMechanism.GetMechanismSpecificNorm(1.0 / 200),
                            validFilePath,
                            validPreprocessorDirectory);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exceptionThrown = true;
                        exceptionMessage = e.Message;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(
                    call,
                    messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(4, msgs.Length);

                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                        StringAssert.StartsWith($"Hydraulische randvoorwaarden berekening voor locatie '{duneLocation.Name}' is mislukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                        StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                    });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(duneLocation.Output);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }

            mockRepository.VerifyAll();
        }

        private static void AssertInput(DunesBoundaryConditionsCalculationInput expectedInput, DunesBoundaryConditionsCalculationInput actualInput)
        {
            Assert.AreEqual(expectedInput.Section.SectionId, actualInput.Section.SectionId);
            Assert.AreEqual(expectedInput.Section.CrossSectionNormal, actualInput.Section.CrossSectionNormal);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Beta, actualInput.Beta);
        }

        private static DunesBoundaryConditionsCalculationInput CreateInput(DuneLocation duneLocation, double norm)
        {
            return new DunesBoundaryConditionsCalculationInput(1, duneLocation.Id, norm);
        }
    }
}