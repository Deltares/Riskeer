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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Service;

namespace Ringtoets.GrassCoverErosionOutwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationServiceTest
    {
        private const double validNorm = 0.005;
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Constructor_ReturnsNewInstance()
        {
            // Call
            var service = new GrassCoverErosionOutwardsWaveConditionsCalculationService();

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationServiceBase>(service);
        }

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(null,
                                                                                                         GetValidAssessmentLevel(),
                                                                                                         GetValidHydraulicBoundaryDatabase(),
                                                                                                         validNorm);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabasePathInvalid_LogValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            string testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                                                 GetValidAssessmentLevel(),
                                                                                                                 new HydraulicBoundaryDatabase
                                                                                                                 {
                                                                                                                     FilePath = testFilePath,
                                                                                                                     CanUsePreprocessor = true,
                                                                                                                     PreprocessorDirectory = validPreprocessorDirectory
                                                                                                                 },
                                                                                                                 validNorm);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. " +
                                    $"Fout bij het lezen van bestand '{testFilePath}': het bestand bestaat niet.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            string invalidFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                                                 GetValidAssessmentLevel(),
                                                                                                                 new HydraulicBoundaryDatabase
                                                                                                                 {
                                                                                                                     FilePath = invalidFilePath,
                                                                                                                     CanUsePreprocessor = true,
                                                                                                                     PreprocessorDirectory = validPreprocessorDirectory
                                                                                                                 },
                                                                                                                 validNorm);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. " +
                                    $"Fout bij het lezen van bestand '{invalidFilePath}': kon geen locaties verkrijgen van de database.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidPreprocessorDirectory_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            const string invalidPreprocessorDirectory = "NonExistingPreprocessorDirectory";

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                                                 GetValidAssessmentLevel(),
                                                                                                                 new HydraulicBoundaryDatabase
                                                                                                                 {
                                                                                                                     FilePath = validFilePath,
                                                                                                                     CanUsePreprocessor = true,
                                                                                                                     UsePreprocessor = true,
                                                                                                                     PreprocessorDirectory = invalidPreprocessorDirectory
                                                                                                                 },
                                                                                                                 validNorm);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            string testFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                                                 GetValidAssessmentLevel(),
                                                                                                                 new HydraulicBoundaryDatabase
                                                                                                                 {
                                                                                                                     FilePath = testFilePath,
                                                                                                                     CanUsePreprocessor = true,
                                                                                                                     PreprocessorDirectory = validPreprocessorDirectory
                                                                                                                 },
                                                                                                                 validNorm);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. " +
                                            "Fout bij het lezen van bestand", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_WithoutImportedHydraulicBoundaryDatabase_LogValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                                                 GetValidAssessmentLevel(),
                                                                                                                 new HydraulicBoundaryDatabase(),
                                                                                                                 validNorm);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                                                 GetValidAssessmentLevel(),
                                                                                                                 GetValidHydraulicBoundaryDatabase(),
                                                                                                                 validNorm);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_AssessmentLevelNaN_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                                                 RoundedDouble.NaN,
                                                                                                                 GetValidHydraulicBoundaryDatabase(),
                                                                                                                 validNorm);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("Kan de waterstand bij doorsnede-eis niet afleiden op basis van de invoer.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN, 10.0)]
        [TestCase(1.0, double.NaN)]
        public void Validate_NoWaterLevels_LogsValidationMessageAndReturnFalse(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment;
            calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                                                 GetValidAssessmentLevel(),
                                                                                                                 GetValidHydraulicBoundaryDatabase(),
                                                                                                                 validNorm);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(NaN)")]
        public void Validate_CalculationWithForeshoreAndUsesBreakWaterAndHasInvalidBreakWaterHeight_LogsValidationMessageAndReturnFalse(double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                                                 GetValidAssessmentLevel(),
                                                                                                                 GetValidHydraulicBoundaryDatabase(),
                                                                                                                 validNorm);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);

                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                    Assert.AreEqual("De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                });
                Assert.IsFalse(isValid);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(null,
                                                                                                                failureMechanism,
                                                                                                                assessmentSection,
                                                                                                                validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_FailureMechniamsNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                null,
                                                                                                                assessmentSection,
                                                                                                                validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                failureMechanism,
                                                                                                                null,
                                                                                                                validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterNoInvalidBreakWaterHeight_PerformAndLog(NaN)")]
        public void Calculate_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Twice();
            mockRepository.ReplayAll();

            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                              failureMechanism,
                                                                                                              assessmentSection,
                                                                                                              validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    var i = 0;
                    foreach (RoundedDouble waterLevel in GetWaterLevels(calculation, failureMechanism, assessmentSection))
                    {
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i + 2]);
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", msgs[i + 3]);

                        i = i + 3;
                    }

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Run_CalculationWithValidInputAndValidForeshore_LogStartAndEnd(CalculationType calculationType)
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Twice();
            mockRepository.ReplayAll();

            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            switch (calculationType)
            {
                case CalculationType.NoForeshore:
                    calculation.InputParameters.ForeshoreProfile = null;
                    calculation.InputParameters.UseForeshore = false;
                    calculation.InputParameters.UseBreakWater = false;
                    break;
                case CalculationType.ForeshoreWithoutBreakWater:
                    calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
                    calculation.InputParameters.UseBreakWater = false;
                    break;
                case CalculationType.ForeshoreWithValidBreakWater:
                    break;
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                              failureMechanism,
                                                                                                              assessmentSection,
                                                                                                              validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    var i = 0;
                    foreach (RoundedDouble waterLevel in GetWaterLevels(calculation, failureMechanism, assessmentSection))
                    {
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i + 2]);
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", msgs[i + 3]);

                        i = i + 3;
                    }

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_Always_InputPropertiesCorrectlySendToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.BreakWater.Type = breakWaterType;

            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(waveConditionsCosineCalculator).Repeat.Times(3);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection,
                                                                                          validFilePath);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = waveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(3, testWaveConditionsInputs.Length);

                var waterLevelIndex = 0;
                foreach (WaveConditionsCosineCalculationInput actualInput in testWaveConditionsInputs)
                {
                    GeneralGrassCoverErosionOutwardsInput generalInput = failureMechanism.GeneralInput;

                    double expectedNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                        assessmentSection.FailureMechanismContribution.SignalingNorm / 30,
                        failureMechanism.Contribution,
                        failureMechanism.GeneralInput.N);

                    WaveConditionsInput input = calculation.InputParameters;
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 expectedNorm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 GetWaterLevels(calculation, failureMechanism, assessmentSection).ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralWaveConditionsInput.A,
                                                                                 generalInput.GeneralWaveConditionsInput.B,
                                                                                 generalInput.GeneralWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Canceled_HasNoOutput()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var grassCoverErosionOutwardsWaveConditionsCalculationService = new GrassCoverErosionOutwardsWaveConditionsCalculationService();
                grassCoverErosionOutwardsWaveConditionsCalculationService.Cancel();

                // Call
                grassCoverErosionOutwardsWaveConditionsCalculationService.Calculate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection,
                                                                                    validFilePath);

                // Assert
                Assert.IsFalse(calculation.HasOutput);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(waveConditionsCosineCalculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var grassCoverErosionOutwardsWaveConditionsCalculationService = new GrassCoverErosionOutwardsWaveConditionsCalculationService();
                waveConditionsCosineCalculator.CalculationFinishedHandler += (s, e) => grassCoverErosionOutwardsWaveConditionsCalculationService.Cancel();

                // Call
                grassCoverErosionOutwardsWaveConditionsCalculationService.Calculate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection,
                                                                                    validFilePath);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(waveConditionsCosineCalculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_WithValidInput_SetsOutput()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(3);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection,
                                                                                          validFilePath);

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.Items.Count());
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

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
                             .Times(3);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection,
                                                                                                  validFilePath);
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
                    Assert.AreEqual(15, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
                    RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                    RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                    RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is gestart.", msgs[1]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[2]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is beëindigd.", msgs[4]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is gestart.", msgs[5]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelMiddleRevetment}'. {detailedReport}", msgs[6]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[7]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is beëindigd.", msgs[8]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is gestart.", msgs[9]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelLowerBoundaryRevetment}'. {detailedReport}", msgs[10]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[11]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is beëindigd.", msgs[12]);

                    Assert.AreEqual("Berekening is mislukt voor alle waterstanden.", msgs[13]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[14]);
                });
                Assert.IsInstanceOf<HydraRingCalculationException>(exception);
                Assert.AreEqual("Berekening is mislukt voor alle waterstanden.", exception.Message);
                Assert.IsNull(calculation.Output);
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var calculatorThatFails = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty))
                             .Return(calculatorThatFails);
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty))
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Twice();
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                              failureMechanism,
                                                                                              assessmentSection,
                                                                                              validFilePath);
                };

                // Assert
                RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
                RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(12, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is gestart.", msgs[1]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[2]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is beëindigd.", msgs[4]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is gestart.", msgs[5]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[6]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is beëindigd.", msgs[7]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is gestart.", msgs[8]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[9]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is beëindigd.", msgs[10]);

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[11]);
                });

                WaveConditionsOutput[] waveConditionsOutputs = calculation.Output.Items.ToArray();
                Assert.AreEqual(3, waveConditionsOutputs.Length);

                double targetNorm = CalculateTargetNorm(assessmentSection.FailureMechanismContribution.SignalingNorm / 30,
                                                        failureMechanism.Contribution,
                                                        failureMechanism.GeneralInput.N);
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevelUpperBoundaryRevetment,
                                                                  targetNorm,
                                                                  waveConditionsOutputs[0]);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(3);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection,
                                                                                          validFilePath);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, validPreprocessorDirectory)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(3);
            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = validPreprocessorDirectory;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection,
                                                                                          validFilePath);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(3);
            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection,
                                                                                          validFilePath);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        private static AssessmentSectionStub CreateAssessmentSection()
        {
            return new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
        }

        private static void ConfigureFailureMechanismWithHydraulicBoundaryOutput(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            failureMechanism.Contribution = 20;
            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);
        }

        private static double CalculateTargetNorm(double norm, double contribution, RoundedDouble N)
        {
            return norm * (contribution / 100) / N;
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation(HydraulicBoundaryLocation location)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = location,
                    CategoryType = FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetDefaultCalculation(HydraulicBoundaryLocation location)
        {
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(location);
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 5.4;

            return calculation;
        }

        private static RoundedDouble GetValidAssessmentLevel()
        {
            return (RoundedDouble) 9.3;
        }

        private static HydraulicBoundaryDatabase GetValidHydraulicBoundaryDatabase()
        {
            return new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                CanUsePreprocessor = true,
                PreprocessorDirectory = validPreprocessorDirectory
            };
        }

        private static IEnumerable<RoundedDouble> GetWaterLevels(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                 GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                 IAssessmentSection assessmentSection)
        {
            return calculation.InputParameters.GetWaterLevels(failureMechanism.GetAssessmentLevel(
                                                                  assessmentSection,
                                                                  calculation.InputParameters.HydraulicBoundaryLocation,
                                                                  calculation.InputParameters.CategoryType));
        }
    }
}