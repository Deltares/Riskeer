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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;

namespace Ringtoets.Revetment.Service.Test
{
    [TestFixture]
    public class WaveConditionsCalculationServiceBaseTest
    {
        private const double validNorm = 0.005;
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Validate_InputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate action = () => TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(null,
                                                                                                               GetValidAssessmentLevel(),
                                                                                                               GetValidHydraulicBoundaryDatabase(),
                                                                                                               validNorm,
                                                                                                               string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(action);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void Validate_DesignWaterLevelNameNull_ThrowArgumentNullException()
        {
            // Setup 
            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            };

            // Call
            TestDelegate action = () => TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(input,
                                                                                                               GetValidAssessmentLevel(),
                                                                                                               GetValidHydraulicBoundaryDatabase(),
                                                                                                               validNorm,
                                                                                                               null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(action);
            Assert.AreEqual("designWaterLevelName", exception.ParamName);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabaseNull_ThrowArgumentNullException()
        {
            // Setup 
            var input = new TestWaveConditionsInput();

            // Call
            TestDelegate action = () => TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(input,
                                                                                                               GetValidAssessmentLevel(),
                                                                                                               null,
                                                                                                               validNorm,
                                                                                                               "test");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(action);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabasePathInvalid_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            var isValid = false;

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(new TestWaveConditionsInput(),
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   new HydraulicBoundaryDatabase
                                                                                                                   {
                                                                                                                       FilePath = string.Empty,
                                                                                                                       PreprocessorDirectory = validPreprocessorDirectory
                                                                                                                   },
                                                                                                                   validNorm,
                                                                                                                   string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Fout bij het lezen van bestand '': bestandspad mag niet leeg of ongedefinieerd zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabaseFileLocation_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            var isValid = false;
            string invalidFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(new TestWaveConditionsInput(),
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   new HydraulicBoundaryDatabase
                                                                                                                   {
                                                                                                                       FilePath = invalidFilePath,
                                                                                                                       PreprocessorDirectory = validPreprocessorDirectory
                                                                                                                   },
                                                                                                                   validNorm,
                                                                                                                   string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_InvalidPreprocessorDirectory_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            var isValid = false;
            const string invalidPreprocessorDirectory = "NonExistingPreprocessorDirectory";

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(new TestWaveConditionsInput(),
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   new HydraulicBoundaryDatabase
                                                                                                                   {
                                                                                                                       FilePath = string.Empty,
                                                                                                                       PreprocessorDirectory = invalidPreprocessorDirectory
                                                                                                                   },
                                                                                                                   validNorm,
                                                                                                                   string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsValidationMessageAndReturnFalse()
        {
            // Setup 
            var isValid = false;
            string dbFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(new TestWaveConditionsInput(),
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   new HydraulicBoundaryDatabase
                                                                                                                   {
                                                                                                                       FilePath = dbFilePath,
                                                                                                                       PreprocessorDirectory = validPreprocessorDirectory
                                                                                                                   },
                                                                                                                   validNorm,
                                                                                                                   string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NormInvalid_LogsErrorAndReturnsFalse()
        {
            // Setup
            var isValid = false;

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(new TestWaveConditionsInput(),
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   GetValidHydraulicBoundaryDatabase(),
                                                                                                                   double.NaN,
                                                                                                                   string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kon geen doelkans bepalen voor deze berekening.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NormTooBig_LogsErrorAndReturnsFalse()
        {
            // Setup
            var isValid = false;

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(new TestWaveConditionsInput(),
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   GetValidHydraulicBoundaryDatabase(),
                                                                                                                   1.0,
                                                                                                                   string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Doelkans is te groot om een berekening uit te kunnen voeren.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_ReturnsFalseAndLogsValidationError()
        {
            // Setup
            var isValid = false;

            var input = new TestWaveConditionsInput();

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(input,
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   GetValidHydraulicBoundaryDatabase(),
                                                                                                                   validNorm,
                                                                                                                   string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_AssessmentLevelNaN_ReturnsFalseAndLogsValidationError()
        {
            // Setup
            var isValid = false;

            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            };

            const string designWaterLevelName = "<de arbitraire naam voor designwaterlevel>";

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(input,
                                                                                                                   RoundedDouble.NaN,
                                                                                                                   GetValidHydraulicBoundaryDatabase(),
                                                                                                                   validNorm,
                                                                                                                   designWaterLevelName);

            // Assert
            string expectedMessage = $"Kan {designWaterLevelName} niet afleiden op basis van de invoer";

            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith(expectedMessage, msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN, 10.0, 12.0)]
        [TestCase(1.0, double.NaN, 12.0)]
        public void Validate_NoWaterLevels_ReturnsFalseAndLogsValidationError(double lowerBoundaryRevetments,
                                                                              double upperBoundaryRevetments,
                                                                              double assessmentLevel)
        {
            // Setup
            var isValid = false;

            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                Orientation = (RoundedDouble) 0,
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetments,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetments,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(input,
                                                                                                                   (RoundedDouble) assessmentLevel,
                                                                                                                   GetValidHydraulicBoundaryDatabase(),
                                                                                                                   validNorm,
                                                                                                                   "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_ForeshoreProfileUseBreakWaterAndHasInvalidBreakWaterHeight_ReturnsFalseAndLogsValidationMessages(double breakWaterHeight)
        {
            // Setup
            var isValid = false;

            WaveConditionsInput input = GetDefaultValidationInput();
            input.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight));
            input.UseBreakWater = true;

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(input,
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   GetValidHydraulicBoundaryDatabase(),
                                                                                                                   validNorm,
                                                                                                                   "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_ForeshoreProfileDoesNotUseBreakWaterAndHasInvalidBreakwaterHeight_ReturnsTrueAndLogsValidationStartAndEnd(double breakWaterHeight)
        {
            // Setup
            var isValid = false;

            WaveConditionsInput input = GetDefaultValidationInput();
            input.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Wall, breakWaterHeight));
            input.UseBreakWater = false;

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(input,
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   GetValidHydraulicBoundaryDatabase(),
                                                                                                                   validNorm,
                                                                                                                   "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Validate_ValidInputValidateForeshoreProfile_ReturnsTrueAndLogsValidationStartAndEnd(CalculationType calculationType)
        {
            // Setup 
            var isValid = false;

            WaveConditionsInput input = GetDefaultValidationInput();

            switch (calculationType)
            {
                case CalculationType.NoForeshore:
                    input.ForeshoreProfile = null;
                    input.UseBreakWater = false;
                    input.UseForeshore = false;
                    input.Orientation = (RoundedDouble) 0;
                    break;
                case CalculationType.ForeshoreWithoutBreakWater:
                    input.ForeshoreProfile = new TestForeshoreProfile();
                    input.UseBreakWater = false;
                    break;
                case CalculationType.ForeshoreWithValidBreakWater:
                    break;
            }

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(input,
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   GetValidHydraulicBoundaryDatabase(),
                                                                                                                   validNorm,
                                                                                                                   "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        public void Validate_StructureNormalOrientationInvalid_ReturnsFalse()
        {
            // Setup
            var isValid = false;

            WaveConditionsInput input = GetDefaultValidationInput();
            input.Orientation = RoundedDouble.NaN;

            // Call
            Action action = () => isValid = TestWaveConditionsCalculationService.PublicValidateWaveConditionsInput(input,
                                                                                                                   GetValidAssessmentLevel(),
                                                                                                                   GetValidHydraulicBoundaryDatabase(),
                                                                                                                   validNorm,
                                                                                                                   "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De waarde voor 'oriëntatie' moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Calculate_InputNull_ThrowArgumentNullException()
        {
            // Setup
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            // Call
            TestDelegate test = () => new TestWaveConditionsCalculationService().PublicCalculate(a,
                                                                                                 b,
                                                                                                 c,
                                                                                                 norm,
                                                                                                 null,
                                                                                                 GetValidAssessmentLevel(),
                                                                                                 hcldFilePath,
                                                                                                 string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_WithoutBreakWater_StartsCalculationWithRightParameters(bool useForeshore)
        {
            // Setup
            var waterLevel = (RoundedDouble) 4.20;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;
            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                ForeshoreProfile = new TestForeshoreProfile(true),
                UpperBoundaryRevetment = (RoundedDouble) 4,
                LowerBoundaryRevetment = (RoundedDouble) 3,
                StepSize = WaveConditionsInputStepSize.Two,
                UseBreakWater = false,
                UseForeshore = useForeshore,
                Orientation = (RoundedDouble) 0
            };

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var calculator = new TestWaveConditionsCosineCalculator();
            RoundedDouble[] waterLevels = input.GetWaterLevels(waterLevel).ToArray();
            int nrOfCalculators = waterLevels.Length;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty))
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new TestWaveConditionsCalculationService().PublicCalculate(a,
                                                                           b,
                                                                           c,
                                                                           norm,
                                                                           input,
                                                                           waterLevel,
                                                                           hcldFilePath,
                                                                           string.Empty);

                // Assert
                for (var i = 0; i < nrOfCalculators; i++)
                {
                    WaveConditionsCosineCalculationInput expectedInput = CreateInput(waterLevels[i], a, b, c, norm, input, useForeshore, false);
                    HydraRingDataEqualityHelper.AreEqual(expectedInput, calculator.ReceivedInputs[i]);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_WithBreakWater_StartsCalculationWithRightParameters(BreakWaterType breakWaterType)
        {
            // Setup
            var waterLevel = (RoundedDouble) 4.20;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;
            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                ForeshoreProfile = new TestForeshoreProfile(true),
                UpperBoundaryRevetment = (RoundedDouble) 4,
                LowerBoundaryRevetment = (RoundedDouble) 3,
                StepSize = WaveConditionsInputStepSize.Two,
                UseBreakWater = true,
                UseForeshore = true,
                Orientation = (RoundedDouble) 0,
                BreakWater =
                {
                    Type = breakWaterType
                }
            };

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var calculator = new TestWaveConditionsCosineCalculator();
            RoundedDouble[] waterLevels = input.GetWaterLevels(waterLevel).ToArray();

            int nrOfCalculators = waterLevels.Length;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new TestWaveConditionsCalculationService().PublicCalculate(a,
                                                                           b,
                                                                           c,
                                                                           norm,
                                                                           input,
                                                                           waterLevel,
                                                                           hcldFilePath,
                                                                           validPreprocessorDirectory);

                // Assert
                for (var i = 0; i < nrOfCalculators; i++)
                {
                    WaveConditionsCosineCalculationInput expectedInput = CreateInput(waterLevels[i], a, b, c, norm, input, true, true);
                    HydraRingDataEqualityHelper.AreEqual(expectedInput, calculator.ReceivedInputs[i]);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_PreprocessorDirectorySet_StartsCalculationWithRightParameters(bool usePreprocessor)
        {
            // Setup
            string preprocessorDirectory = usePreprocessor
                                               ? validPreprocessorDirectory
                                               : string.Empty;

            var waterLevel = (RoundedDouble) 4.20;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;
            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                ForeshoreProfile = new TestForeshoreProfile(true),
                UpperBoundaryRevetment = (RoundedDouble) 4,
                LowerBoundaryRevetment = (RoundedDouble) 3,
                StepSize = WaveConditionsInputStepSize.Two,
                Orientation = (RoundedDouble) 0
            };

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var calculator = new TestWaveConditionsCosineCalculator();
            int nrOfCalculators = input.GetWaterLevels(waterLevel).Count();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, preprocessorDirectory))
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new TestWaveConditionsCalculationService().PublicCalculate(a,
                                                                           b,
                                                                           c,
                                                                           norm,
                                                                           input,
                                                                           waterLevel,
                                                                           hcldFilePath,
                                                                           preprocessorDirectory);

                // Assert
                for (var i = 0; i < nrOfCalculators; i++)
                {
                    Assert.AreEqual(usePreprocessor, calculator.ReceivedInputs[i].PreprocessorSetting.RunPreprocessor);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_ThreeCalculationsFail_ThrowsHydraRingCalculationExceptionAndLogError)
        })]
        public void Calculate_ThreeCalculationsFail_ThrowsHydraRingCalculationExceptionAndLogError(bool endInFailure,
                                                                                                   string lastErrorFileContent,
                                                                                                   string detailedReport)
        {
            // Setup
            var waterLevel = (RoundedDouble) 4.20;
            var waterLevelUpperBoundaryRevetment = new RoundedDouble(2, 4.00);
            var waterLevelLowerBoundaryRevetment = new RoundedDouble(2, 3.00);

            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                ForeshoreProfile = new TestForeshoreProfile(),
                UpperBoundaryRevetment = waterLevelUpperBoundaryRevetment,
                LowerBoundaryRevetment = waterLevelLowerBoundaryRevetment
            };

            int nrOfCalculators = input.GetWaterLevels(waterLevel).Count();

            var calculatorThatFails = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty))
                             .Return(calculatorThatFails)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
            HydraRingCalculationException exception = null;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    try
                    {
                        new TestWaveConditionsCalculationService().PublicCalculate(a,
                                                                                   b,
                                                                                   c,
                                                                                   norm,
                                                                                   input,
                                                                                   waterLevel,
                                                                                   hcldFilePath,
                                                                                   string.Empty);
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
                    Assert.AreEqual(13, msgs.Length);

                    var waterLevelMiddleRevetment = new RoundedDouble(2, 3.50);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is gestart.", msgs[0]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[1]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is beëindigd.", msgs[3]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is gestart.", msgs[4]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelMiddleRevetment}'. {detailedReport}", msgs[5]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[6]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is beëindigd.", msgs[7]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is gestart.", msgs[8]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelLowerBoundaryRevetment}'. {detailedReport}", msgs[9]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[10]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is beëindigd.", msgs[11]);

                    Assert.AreEqual("Berekening is mislukt voor alle waterstanden.", msgs[12]);
                });
                Assert.IsInstanceOf<HydraRingCalculationException>(exception);
                Assert.AreEqual("Berekening is mislukt voor alle waterstanden.", exception.Message);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_OneOutOfThreeCalculationsFails_ReturnsOutputsAndLogError)
        })]
        public void Calculate_OneOutOfThreeCalculationsFails_ReturnsOutputsAndLogError(bool endInFailure,
                                                                                       string lastErrorFileContent,
                                                                                       string detailedReport)
        {
            // Setup
            var calculatorThatFails = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(calculatorThatFails);
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(new TestWaveConditionsCosineCalculator());
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(new TestWaveConditionsCosineCalculator());
            mockRepository.ReplayAll();

            var waterLevelUpperBoundary = new RoundedDouble(2, 4.00);
            var waterLevelLowerBoundary = new RoundedDouble(2, 3.00);
            var waterLevel = (RoundedDouble) 4.2;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;
            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                ForeshoreProfile = new TestForeshoreProfile(),
                UpperBoundaryRevetment = waterLevelUpperBoundary,
                LowerBoundaryRevetment = waterLevelLowerBoundary
            };

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestWaveConditionsCalculationService();

                // Call
                Action call = () => service.PublicCalculate(a,
                                                            b,
                                                            c,
                                                            norm,
                                                            input,
                                                            waterLevel,
                                                            hcldFilePath,
                                                            string.Empty);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(10, msgs.Length);

                    var waterLevelMiddle = new RoundedDouble(2, 3.50);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundary}' is gestart.", msgs[0]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundary}'. {detailedReport}", msgs[1]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundary}' is beëindigd.", msgs[3]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddle}' is gestart.", msgs[4]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddle}' is beëindigd.", msgs[6]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundary}' is gestart.", msgs[7]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[8]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundary}' is beëindigd.", msgs[9]);
                });

                WaveConditionsOutput[] waveConditionsOutputs = service.Outputs.ToArray();
                Assert.AreEqual(3, waveConditionsOutputs.Length);

                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevelUpperBoundary,
                                                                  norm,
                                                                  waveConditionsOutputs[0]);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            var waterLevel = (RoundedDouble) 4.2;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;
            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                ForeshoreProfile = new TestForeshoreProfile(),
                UpperBoundaryRevetment = waterLevel,
                LowerBoundaryRevetment = (RoundedDouble) 3
            };
            var calculator = new TestWaveConditionsCosineCalculator();
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, validPreprocessorDirectory)).Return(calculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestWaveConditionsCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.PublicCalculate(a,
                                        b,
                                        c,
                                        norm,
                                        input,
                                        waterLevel,
                                        validFilePath,
                                        validPreprocessorDirectory);

                // Assert
                Assert.IsTrue(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        private static WaveConditionsInput GetDefaultValidationInput()
        {
            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                ForeshoreProfile = new TestForeshoreProfile(true),
                UseBreakWater = true,
                UseForeshore = true,
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0,
                Orientation = (RoundedDouble) 0
            };
            return input;
        }

        private static HydraulicBoundaryDatabase GetValidHydraulicBoundaryDatabase()
        {
            return new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                PreprocessorDirectory = validPreprocessorDirectory
            };
        }

        private static WaveConditionsCosineCalculationInput CreateInput(double waterLevel, double a, double b, double c, double norm, WaveConditionsInput input, bool useForeshore, bool useBreakWater)
        {
            return new WaveConditionsCosineCalculationInput(1,
                                                            input.Orientation,
                                                            input.HydraulicBoundaryLocation.Id,
                                                            norm,
                                                            useForeshore
                                                                ? input.ForeshoreGeometry.Select(coordinate => new HydraRingForelandPoint(coordinate.X, coordinate.Y))
                                                                : new HydraRingForelandPoint[0],
                                                            useBreakWater
                                                                ? new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(input.BreakWater.Type), input.BreakWater.Height)
                                                                : null,
                                                            waterLevel,
                                                            a,
                                                            b,
                                                            c);
        }

        private static RoundedDouble GetValidAssessmentLevel()
        {
            return (RoundedDouble) 12;
        }

        private class TestWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
        {
            public IEnumerable<WaveConditionsOutput> Outputs;

            public static bool PublicValidateWaveConditionsInput(WaveConditionsInput waveConditionsInput,
                                                                 RoundedDouble assessmentLevel,
                                                                 HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                                 double targetProbability,
                                                                 string valueName)
            {
                return ValidateWaveConditionsInput(waveConditionsInput,
                                                   assessmentLevel,
                                                   hydraulicBoundaryDatabase,
                                                   targetProbability,
                                                   valueName);
            }

            public void PublicCalculate(RoundedDouble a,
                                        RoundedDouble b,
                                        RoundedDouble c,
                                        double norm,
                                        WaveConditionsInput input,
                                        RoundedDouble assessmentLevel,
                                        string dbFilePath,
                                        string preprocessorDirectory)
            {
                Outputs = CalculateWaveConditions(input,
                                                  assessmentLevel,
                                                  a,
                                                  b,
                                                  c,
                                                  norm,
                                                  dbFilePath,
                                                  preprocessorDirectory);
            }
        }
    }
}