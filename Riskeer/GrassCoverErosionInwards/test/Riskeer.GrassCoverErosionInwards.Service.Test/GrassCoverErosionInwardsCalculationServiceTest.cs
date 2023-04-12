﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Riskeer.GrassCoverErosionInwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsMessageAndReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                new GrassCoverErosionInwardsFailureMechanism(), mockRepository, validFilePath);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            };

            // Call
            var isValid = false;
            void Call() => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Er is geen hydraulische belastingenlocatie geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsMessageAndReturnsFalse()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           invalidFilePath);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            };

            // Call
            var isValid = true;
            void Call() => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsMessageAndReturnsFalse()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           invalidFilePath);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            };

            // Call
            var isValid = false;
            void Call() => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_WithoutImportedHydraulicBoundaryDatabase_LogsMessageAndReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            };

            // Call
            var isValid = false;
            void Call() => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische belastingendatabase geïmporteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_NoDikeProfile_LogsMessageAndReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                new GrassCoverErosionInwardsFailureMechanism(), mockRepository, validFilePath);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    Orientation = (RoundedDouble) 0
                }
            };

            // Call
            var isValid = false;
            void Call() => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Er is geen dijkprofiel geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_ValidInputAndInvalidBreakWaterHeight_LogsMessageAndReturnsFalse(double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetCalculationWithBreakWater(breakWaterHeight);
            calculation.InputParameters.UseBreakWater = true;

            // Call
            var isValid = false;
            void Call() => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidInputAndInvalidOrientation_LogsMessageAndReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties
                                                  {
                                                      Id = "id",
                                                      Orientation = RoundedDouble.NaN
                                                  })
                }
            };

            // Call
            var isValid = false;
            void Call() => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De waarde voor 'Oriëntatie' moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_ValidInputAndInvalidDikeHeight_LogsMessageAndReturnsFalse(double dikeHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties
                                                  {
                                                      Id = "id",
                                                      DikeHeight = dikeHeight
                                                  })
                }
            };

            // Call
            var isValid = false;
            void Call() => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De waarde voor 'Dijkhoogte' moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, 10.0)]
        [TestCase(false, 10.0)]
        [TestCase(false, double.NaN)]
        [TestCase(false, double.PositiveInfinity)]
        [TestCase(false, double.NegativeInfinity)]
        public void Validate_ValidInputAndValidBreakWaterHeight_ReturnsTrue(bool useBreakWater, double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetCalculationWithBreakWater(breakWaterHeight);
            calculation.InputParameters.UseBreakWater = useBreakWater;

            // Call
            var isValid = false;
            void Call() => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
            Assert.IsTrue(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(null, assessmentSection, failureMechanism.GeneralInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, null, failureMechanism.GeneralInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Calculate_GeneralInputNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(new GrassCoverErosionInwardsCalculation(), assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Calculate_CalculationValid_ReturnOutput([Values(true, false)] bool useForeland,
                                                            [Values(true, false)] bool shouldDikeHeightBeCalculated,
                                                            [Values(true, false)] bool shouldOvertoppingRateBeCalculated,
                                                            [Values(true, false)] bool calculateIllustrationPoints)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestOvertoppingCalculator
                             {
                                 IllustrationPointsResult = new TestGeneralResult()
                             });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestHydraulicLoadsCalculator
                             {
                                 IllustrationPointsResult = new TestGeneralResult()
                             });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestHydraulicLoadsCalculator
                             {
                                 IllustrationPointsResult = new TestGeneralResult()
                             });
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    ShouldDikeHeightBeCalculated = shouldDikeHeightBeCalculated,
                    ShouldOvertoppingRateBeCalculated = shouldOvertoppingRateBeCalculated,
                    UseForeshore = useForeland,
                    ShouldDikeHeightIllustrationPointsBeCalculated = calculateIllustrationPoints,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = calculateIllustrationPoints,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = calculateIllustrationPoints
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSection,
                                                                           failureMechanism.GeneralInput);
            }

            // Assert
            OvertoppingOutput overtoppingOutput = calculation.Output.OvertoppingOutput;
            Assert.IsFalse(double.IsNaN(overtoppingOutput.WaveHeight));
            Assert.IsFalse(double.IsNaN(overtoppingOutput.Reliability));
            Assert.IsFalse(overtoppingOutput.IsOvertoppingDominant);
            Assert.AreEqual(calculateIllustrationPoints, calculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(calculateIllustrationPoints, overtoppingOutput.HasGeneralResult);

            if (shouldDikeHeightBeCalculated)
            {
                DikeHeightOutput dikeHeightOutput = calculation.Output.DikeHeightOutput;
                Assert.IsNotNull(dikeHeightOutput);

                Assert.IsFalse(double.IsNaN(dikeHeightOutput.DikeHeight));
                Assert.IsFalse(double.IsNaN(dikeHeightOutput.TargetProbability));
                Assert.IsFalse(double.IsNaN(dikeHeightOutput.TargetReliability));
                Assert.IsFalse(double.IsNaN(dikeHeightOutput.CalculatedProbability));
                Assert.IsFalse(double.IsNaN(dikeHeightOutput.CalculatedReliability));

                Assert.AreEqual(calculateIllustrationPoints, calculation.InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated);
                Assert.AreEqual(calculateIllustrationPoints, dikeHeightOutput.HasGeneralResult);
            }
            else
            {
                Assert.IsNull(calculation.Output.DikeHeightOutput);
            }

            if (shouldOvertoppingRateBeCalculated)
            {
                OvertoppingRateOutput overtoppingRateOutput = calculation.Output.OvertoppingRateOutput;
                Assert.IsNotNull(overtoppingRateOutput);

                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.OvertoppingRate));
                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.TargetProbability));
                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.TargetReliability));
                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.CalculatedProbability));
                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.CalculatedReliability));

                Assert.AreEqual(calculateIllustrationPoints, calculation.InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated);
                Assert.AreEqual(calculateIllustrationPoints, overtoppingRateOutput.HasGeneralResult);
            }
            else
            {
                Assert.IsNull(calculation.Output.OvertoppingRateOutput);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_DikeHeightCalculationFails_OutputNotNull()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);

            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    ShouldDikeHeightBeCalculated = true
                }
            };

            // Call
            void Call()
            {
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                }
            }

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                    GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                    overtoppingCalculator.OutputDirectory,
                    msgs[1]);
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                    GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                    calculation.Name,
                    dikeHeightCalculator.LastErrorFileContent,
                    msgs[2]);
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                    GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                    dikeHeightCalculator.OutputDirectory,
                    msgs[3]);
                CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
            });
            Assert.IsNotNull(calculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingRateCalculationFails_OutputNotNull()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            // Call
            void Call()
            {
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                }
            }

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                    GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                    overtoppingCalculator.OutputDirectory,
                    msgs[1]);
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                    GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                    calculation.Name,
                    overtoppingRateCalculator.LastErrorFileContent,
                    msgs[2]);
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                    GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                    overtoppingRateCalculator.OutputDirectory,
                    msgs[3]);
                CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
            });
            Assert.IsNotNull(calculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelWithValidOvertoppingCalculationInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            };

            var service = new GrassCoverErosionInwardsCalculationService();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                overtoppingCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  assessmentSection,
                                  grassCoverErosionInwardsFailureMechanism.GeneralInput);

                // Assert
                Assert.IsTrue(overtoppingCalculator.IsCanceled);
                Assert.IsNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_CancelDikeHeightCalculation_CancelsCalculatorAndHasNullOutput(
            bool cancelBeforeDikeHeightCalculationStarts)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(),
                    ShouldDikeHeightBeCalculated = true
                }
            };

            var service = new GrassCoverErosionInwardsCalculationService();

            // Call
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                if (cancelBeforeDikeHeightCalculationStarts)
                {
                    overtoppingCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();
                }
                else
                {
                    dikeHeightCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();
                }

                // Call
                service.Calculate(calculation,
                                  assessmentSection,
                                  failureMechanism.GeneralInput);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(overtoppingCalculator.IsCanceled);
                Assert.IsTrue(dikeHeightCalculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_CancelOvertoppingRateCalculation_CancelsCalculatorAndHasNullOutput(
            bool cancelBeforeOvertoppingRateCalculationStarts)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(),
                    ShouldDikeHeightBeCalculated = true,
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            var service = new GrassCoverErosionInwardsCalculationService();

            // Call
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                if (cancelBeforeOvertoppingRateCalculationStarts)
                {
                    overtoppingCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();
                }
                else
                {
                    overtoppingRateCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();
                }

                // Call
                service.Calculate(calculation,
                                  assessmentSection,
                                  failureMechanism.GeneralInput);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(overtoppingCalculator.IsCanceled);
                Assert.IsTrue(overtoppingRateCalculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingCalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                void Call()
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        calculation.Name,
                        overtoppingCalculator.LastErrorFileContent,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(exceptionThrown);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingCalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                void Call()
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        calculation.Name,
                        overtoppingCalculator.LastErrorFileContent,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingCalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = false
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

                // Call
                void Call()
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exception = e;
                    }
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        calculation.Name,
                        overtoppingCalculator.LastErrorFileContent,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsInstanceOf<HydraRingCalculationException>(exception);
                Assert.IsNull(calculation.Output);
                Assert.AreEqual(overtoppingCalculator.LastErrorFileContent, exception.Message);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_DikeHeightCalculationFailedWithExceptionAndLastErrorPresent_LogError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    ShouldDikeHeightBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call()
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        dikeHeightCalculator.LastErrorFileContent,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_DikeHeightCalculationFailedWithExceptionAndNoLastErrorPresent_LogError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository,
                                                                                                           validFilePath);

            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    ShouldDikeHeightBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call()
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        dikeHeightCalculator.LastErrorFileContent,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_DikeHeightCalculationFailedWithoutExceptionAndWithLastErrorPresent_LogError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = false
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var overtoppingCalculator = new TestOvertoppingCalculator();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    ShouldDikeHeightBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call()
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        dikeHeightCalculator.LastErrorFileContent,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingRateCalculationFailedWithExceptionAndLastErrorPresent_LogError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call()
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        overtoppingRateCalculator.LastErrorFileContent,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingRateCalculationFailedWithExceptionAndNoLastErrorPresent_LogError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository,
                                                                                                           validFilePath);

            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call()
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        overtoppingRateCalculator.LastErrorFileContent,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingRateCalculationFailedWithoutExceptionAndWithLastErrorPresent_LogError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var overtoppingCalculator = new TestOvertoppingCalculator();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = false,
                LastErrorFileContent = "An error occurred"
            };
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call()
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        overtoppingRateCalculator.LastErrorFileContent,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButOvertoppingGeneralResultNull_IllustrationPointsNotSetAndLogs()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    Assert.AreEqual(parserError, msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.OvertoppingOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputCalculateOvertoppingIllustrationPointsFalseAndIllustrationPointsParserErrorMessageNotNull_DoesNotLog()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();
            calculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(7, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButOvertoppingRateGeneralResultNull_IllustrationPointsNotSetAndLogs()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[5]);
                    Assert.AreEqual(parserError, msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputCalculateOvertoppingRateIllustrationPointsFalseAndIllustrationPointsParserErrorMessageNotNull_DoesNotLog()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();
            calculation.InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(7, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButDikeHeightGeneralResultNull_IllustrationPointsNotSetAndLogs()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[3]);
                    Assert.AreEqual(parserError, msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputCalculateDikeHeightIllustrationPointsFalseAndIllustrationPointsParserErrorMessageNotNull_DoesNotLog()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();
            calculation.InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(7, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButOvertoppingIllustrationPointResultsOfIncorrectType_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultNotSetMessage(msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);

                    Assert.IsInstanceOf<IllustrationPointConversionException>(tupleArray[2].Item3);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.OvertoppingOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingCalculationRanErrorInSettingIllustrationPoints_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateStochasts(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        calculation.Name,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.OvertoppingOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButOvertoppingRateIllustrationPointResultsOfIncorrectType_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultNotSetMessage(msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);

                    Assert.IsInstanceOf<IllustrationPointConversionException>(tupleArray[6].Item3);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingRateCalculationRanErrorInSettingIllustrationPoints_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateStochasts(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButDikeHeightIllustrationPointResultsOfIncorrectType_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultNotSetMessage(msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);

                    Assert.IsInstanceOf<IllustrationPointConversionException>(tupleArray[4].Item3);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_DikeHeightCalculationRanErrorInSettingIllustrationPoints_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts()
            };
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(null))
                             .IgnoreArguments()
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new GrassCoverErosionInwardsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription,
                        overtoppingCalculator.OutputDirectory,
                        msgs[1]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        dikeHeightCalculator.OutputDirectory,
                        msgs[2]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[3]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateStochasts(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription,
                        calculation.Name,
                        msgs[4]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        overtoppingRateCalculator.OutputDirectory,
                        msgs[5]);
                    GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                        GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription,
                        calculation.Name,
                        msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_HydraulicBoundaryDataSet_ExpectedSettingsSetToCalculators(bool usePreprocessorClosure)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath,
                                                                                                           usePreprocessorClosure);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.Locations.First(hl => hl.Id == 1300001);

            HydraulicBoundaryCalculationSettings calculationSettings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                assessmentSection.HydraulicBoundaryData, hydraulicBoundaryLocation);

            var overtoppingCalculator = new TestOvertoppingCalculator();
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator();
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = GetDikeProfile(),
                    ShouldDikeHeightBeCalculated = true,
                    ShouldOvertoppingRateBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSection,
                                                                           failureMechanism.GeneralInput);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        private static DikeProfile GetDikeProfile()
        {
            return new DikeProfile(
                new Point2D(0, 0),
                new[]
                {
                    new RoughnessPoint(new Point2D(1.1, 2.2), 0.6),
                    new RoughnessPoint(new Point2D(3.3, 4.4), 0.7)
                },
                new[]
                {
                    new Point2D(-2.0, -2.0),
                    new Point2D(-1.0, -1.0)
                }, null, new DikeProfile.ConstructionProperties
                {
                    Id = "id",
                    Orientation = 5.5,
                    DikeHeight = 10
                });
        }

        private static GrassCoverErosionInwardsCalculation GetCalculationWithBreakWater(double breakWaterHeight)
        {
            return new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0),
                                                  new RoughnessPoint[0],
                                                  new Point2D[0],
                                                  new BreakWater(BreakWaterType.Dam, breakWaterHeight),
                                                  new DikeProfile.ConstructionProperties
                                                  {
                                                      Id = "id"
                                                  })
                }
            };
        }

        private static GrassCoverErosionInwardsCalculation GetValidCalculationWithCalculateIllustrationPointsSettings()
        {
            return new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    DikeProfile = GetDikeProfile(),
                    ShouldDikeHeightBeCalculated = true,
                    ShouldOvertoppingRateBeCalculated = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };
        }
    }
}