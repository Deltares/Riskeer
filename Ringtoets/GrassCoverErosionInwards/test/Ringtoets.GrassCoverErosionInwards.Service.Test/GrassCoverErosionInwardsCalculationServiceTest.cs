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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Ringtoets.GrassCoverErosionInwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFile = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    DikeProfile = new TestDikeProfile()
                }
            };

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            string invalidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           invalidFilePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new TestDikeProfile()
                }
            };

            // Call
            var isValid = true;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            string invalidFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           invalidFilePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new TestDikeProfile()
                }
            };

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_NoDikeProfile_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    Orientation = (RoundedDouble) 0
                }
            };

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen dijkprofiel geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_ValidInputAndInvalidBreakWaterHeight_LogsMessageAndReturnFalse(double breakWaterHeight)
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = GetCalculationWithBreakWater(name, breakWaterHeight);
            calculation.InputParameters.UseBreakWater = true;

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidInputAndInvalidOrientation_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'oriëntatie' moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_ValidInputAndInvalidDikeHeight_LogsMessageAndReturnFalse(double dikeHeight)
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'dijkhoogte' moet een concreet getal zijn.", msgs[1]);
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
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = GetCalculationWithBreakWater(name, breakWaterHeight);
            calculation.InputParameters.UseBreakWater = useBreakWater;

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_ValidInput_ReturnsTrue()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new TestDikeProfile()
                }
            };

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationService().Calculate(null,
                                                                                                 assessmentSectionStub,
                                                                                                 failureMechanism.GeneralInput,
                                                                                                 failureMechanism.Contribution,
                                                                                                 string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                                 null,
                                                                                                 failureMechanism.GeneralInput,
                                                                                                 failureMechanism.Contribution,
                                                                                                 string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Calculate_GeneralinputNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                                 assessmentSectionStub,
                                                                                                 null,
                                                                                                 failureMechanism.Contribution,
                                                                                                 string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalInput", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Calculate_CalculationValid_ReturnOutput([Values(true, false)] bool useForeland,
                                                            [Values(DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                                                                DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability,
                                                                DikeHeightCalculationType.NoCalculation)] DikeHeightCalculationType dikeHeightCalculationType,
                                                            [Values(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                                                                OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability,
                                                                OvertoppingRateCalculationType.NoCalculation)] OvertoppingRateCalculationType overtoppingRateCalculationType,
                                                            [Values(true, false)] bool calculateIllustrationPoints)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType,
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
                                                                           assessmentSectionStub,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           validFile);
            }

            // Assert
            GrassCoverErosionInwardsOvertoppingOutput overtoppingOutput = calculation.Output.OvertoppingOutput;
            Assert.IsFalse(double.IsNaN(overtoppingOutput.WaveHeight));
            ProbabilityAssessmentOutput probabilityAssessmentOutput = overtoppingOutput.ProbabilityAssessmentOutput;
            Assert.IsNotNull(probabilityAssessmentOutput);
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.FactorOfSafety));
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.Probability));
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.Reliability));
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.RequiredProbability));
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.RequiredReliability));
            Assert.IsFalse(overtoppingOutput.IsOvertoppingDominant);
            if (calculateIllustrationPoints)
            {
                Assert.IsTrue(calculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
                Assert.IsTrue(overtoppingOutput.HasGeneralResult);
            }
            else
            {
                Assert.IsFalse(calculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
                Assert.IsFalse(overtoppingOutput.HasGeneralResult);
            }

            if (dikeHeightCalculationType != DikeHeightCalculationType.NoCalculation)
            {
                DikeHeightOutput dikeHeightOutput = calculation.Output.DikeHeightOutput;
                Assert.IsNotNull(dikeHeightOutput);

                Assert.IsFalse(double.IsNaN(dikeHeightOutput.DikeHeight));
                Assert.IsFalse(double.IsNaN(dikeHeightOutput.TargetProbability));
                Assert.IsFalse(double.IsNaN(dikeHeightOutput.TargetReliability));
                Assert.IsFalse(double.IsNaN(dikeHeightOutput.CalculatedProbability));
                Assert.IsFalse(double.IsNaN(dikeHeightOutput.CalculatedReliability));

                if (calculateIllustrationPoints)
                {
                    Assert.IsTrue(calculation.InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated);
                    Assert.IsTrue(dikeHeightOutput.HasGeneralResult);
                }
                else
                {
                    Assert.IsFalse(calculation.InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated);
                    Assert.IsFalse(dikeHeightOutput.HasGeneralResult);
                }
            }
            else
            {
                Assert.IsNull(calculation.Output.DikeHeightOutput);
            }

            if (overtoppingRateCalculationType != OvertoppingRateCalculationType.NoCalculation)
            {
                OvertoppingRateOutput overtoppingRateOutput = calculation.Output.OvertoppingRateOutput;
                Assert.IsNotNull(overtoppingRateOutput);

                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.OvertoppingRate));
                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.TargetProbability));
                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.TargetReliability));
                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.CalculatedProbability));
                Assert.IsFalse(double.IsNaN(overtoppingRateOutput.CalculatedReliability));

                if (calculateIllustrationPoints)
                {
                    Assert.IsTrue(calculation.InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated);
                    Assert.IsTrue(overtoppingRateOutput.HasGeneralResult);
                }
                else
                {
                    Assert.IsFalse(calculation.InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated);
                    Assert.IsFalse(overtoppingRateOutput.HasGeneralResult);
                }
            }
            else
            {
                Assert.IsNull(calculation.Output.OvertoppingRateOutput);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_DikeHeightCalculationFails_OutputNotNull(AssessmentSectionNorm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_DikeHeightCalculationFails_OutputNotNull(ProfileProbability)")]
        public void Calculate_DikeHeightCalculationFails_OutputNotNull(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);

            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            var expectedExceptionThrown = false;

            // Call
            Action call = () =>
            {
                try
                {
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                }
                catch (HydraRingFileParserException)
                {
                    expectedExceptionThrown = true;
                }
            };

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
            Assert.IsFalse(expectedExceptionThrown);
            Assert.IsNotNull(calculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_OvertoppingRateCalculationFails_OutputNotNull(AssessmentSectionNorm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_OvertoppingRateCalculationFails_OutputNotNull(ProfileProbability)")]
        public void Calculate_OvertoppingRateCalculationFails_OutputNotNull(OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            var expectedExceptionThrown = false;

            // Call
            Action call = () =>
            {
                try
                {
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                }
                catch (HydraRingFileParserException)
                {
                    expectedExceptionThrown = true;
                }
            };

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
            Assert.IsFalse(expectedExceptionThrown);
            Assert.IsNotNull(calculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelWithValidOvertoppingCalculationInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new TestDikeProfile()
                }
            };

            var service = new GrassCoverErosionInwardsCalculationService();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                overtoppingCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  assessmentSectionStub,
                                  grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                  grassCoverErosionInwardsFailureMechanism.Contribution,
                                  validFile);

                // Assert
                Assert.IsTrue(overtoppingCalculator.IsCanceled);
                Assert.IsNull(calculation.Output);
            }
        }

        [Test]
        [Combinatorial]
        public void Calculate_CancelDikeHeightCalculation_CancelsCalculatorAndHasNullOutput(
            [Values(true, false)] bool cancelBeforeDikeHeightCalculationStarts,
            [Values(DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability)] DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new TestDikeProfile(),
                    DikeHeightCalculationType = dikeHeightCalculationType
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
                                  assessmentSectionStub,
                                  failureMechanism.GeneralInput,
                                  failureMechanism.Contribution,
                                  validFile);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(overtoppingCalculator.IsCanceled);
                Assert.IsTrue(dikeHeightCalculator.IsCanceled);
            }
        }

        [Test]
        [Combinatorial]
        public void Calculate_CancelOvertoppingRateCalculation_CancelsCalculatorAndHasNullOutput(
            [Values(true, false)] bool cancelBeforeOvertoppingRateCalculationStarts,
            [Values(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability)] OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new TestDikeProfile(),
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
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
                                  assessmentSectionStub,
                                  failureMechanism.GeneralInput,
                                  failureMechanism.Contribution,
                                  validFile);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(overtoppingCalculator.IsCanceled);
                Assert.IsTrue(overtoppingRateCalculator.IsCanceled);
            }
        }

        [Test]
        public void Calculate_OvertoppingCalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
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
        }

        [Test]
        public void Calculate_OvertoppingCalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
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
        }

        [Test]
        public void Calculate_OvertoppingCalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = false
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
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
        }

        [Test]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_DikeHeightCalculationFailedWithExceptionLastError_LogError(AssessmentSectionNorm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_DikeHeightCalculationFailedWithExceptionLastError_LogError(ProfileProbability)")]
        public void Calculate_DikeHeightCalculationFailedWithExceptionAndLastErrorPresent_LogError(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
                Assert.IsFalse(exceptionThrown);
            }
        }

        [Test]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_DikeHeightCalculationFailedWithExceptionNoLastError_LogError(AssessmentSectionNorm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_DikeHeightCalculationFailedWithExceptionNoLastError_LogError(ProfileProbability)")]
        public void Calculate_DikeHeightCalculationFailedWithExceptionAndNoLastErrorPresent_LogError(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository,
                                                                                                           validFile);

            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
                Assert.IsFalse(exceptionThrown);
            }
        }

        [Test]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_DikeHeightCalculationFailedNoExceptionWithLastError_LogError(AssessmentSectionNorm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_DikeHeightCalculationFailedNoExceptionWithLastError_LogError(ProfileProbability)")]
        public void Calculate_DikeHeightCalculationFailedWithoutExceptionAndWithLastErrorPresent_LogError(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = false
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var overtoppingCalculator = new TestOvertoppingCalculator();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
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
                    Assert.AreEqual(5, msgs.Length);
                });
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(exceptionThrown);
            }
        }

        [Test]
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_OvertoppingRateCalculationFailedWithExceptionLastError_LogError(AssessmentSectionNorm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_OvertoppingRateCalculationFailedWithExceptionLastError_LogError(ProfileProbability)")]
        public void Calculate_OvertoppingRateCalculationFailedWithExceptionAndLastErrorPresent_LogError(OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();

            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
                Assert.IsFalse(exceptionThrown);
            }
        }

        [Test]
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_OvertoppingRateCalculationFailedWithExceptionNoLastError_LogError(AssessmentSectionNorm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_OvertoppingRateCalculationFailedWithExceptionNoLastError_LogError(ProfileProbability)")]
        public void Calculate_OvertoppingRateCalculationFailedWithExceptionAndNoLastErrorPresent_LogError(OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository,
                                                                                                           validFile);

            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
                Assert.IsFalse(exceptionThrown);
            }
        }

        [Test]
        [TestCase(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_OvertoppingRateCalculationFailedNoExceptionWithLastError_LogError(AssessmentSectionNorm)")]
        [TestCase(OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_OvertoppingRateCalculationFailedNoExceptionWithLastError_LogError(ProfileProbability)")]
        public void Calculate_OvertoppingRateCalculationFailedWithoutExceptionAndWithLastErrorPresent_LogError(OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var overtoppingCalculator = new TestOvertoppingCalculator();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(overtoppingCalculator);
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = false,
                LastErrorFileContent = "An error occurred"
            };
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
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
                    Assert.AreEqual(5, msgs.Length);
                });
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(exceptionThrown);
            }
        }

        [Test]
        public void CalculateAndConvertIllustrationPointsResult_OvertoppingGeneralResultNull_WarnErrorMessage()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = null,
                IllustrationPointsParserErrorMessage = parserError
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var hydraulicBoundary = new TestHydraulicBoundaryLocation();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundary,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    UseForeshore = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                var exceptionThrown = false;
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (ArgumentException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("De HBN berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("De HBN berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[3]);
                    Assert.AreEqual("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[4]);
                    Assert.AreEqual("De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[5]);
                    Assert.AreEqual(parserError, msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsFalse(exceptionThrown);
                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }
        }

        [Test]
        public void DontCalculateAndConvertIllustrationPointsResult_OvertoppingGeneralResultNull_WarnErrorMessage()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = null,
                IllustrationPointsParserErrorMessage = parserError
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var hydraulicBoundary = new TestHydraulicBoundaryLocation();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundary,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    UseForeshore = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = false,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                var exceptionThrown = false;
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (ArgumentException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(7, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("De HBN berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("De HBN berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[3]);
                    Assert.AreEqual("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[4]);
                    Assert.AreEqual("De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                });

                Assert.IsFalse(exceptionThrown);
                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }
        }

        [Test]
        public void ConvertIllustrationPointsResult_OvertoppingRateGeneralResultNull_WarnErrorMessage()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = null,
                IllustrationPointsParserErrorMessage = parserError
            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var hydraulicBoundary = new TestHydraulicBoundaryLocation();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundary,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    UseForeshore = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                var exceptionThrown = false;
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (ArgumentException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("De HBN berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("De HBN berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[3]);
                    Assert.AreEqual("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[4]);
                    Assert.AreEqual("De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[5]);
                    Assert.AreEqual(parserError, msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsFalse(exceptionThrown);
                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }
        }

        [Test]
        public void DontCalculateAndConvertIllustrationPointsResult_OvertoppingRateGeneralResultNull_WarnErrorMessage()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = null,
                IllustrationPointsParserErrorMessage = parserError
            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var hydraulicBoundary = new TestHydraulicBoundaryLocation();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundary,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    UseForeshore = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = false
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                var exceptionThrown = false;
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (ArgumentException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(7, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("De HBN berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("De HBN berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[3]);
                    Assert.AreEqual("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[4]);
                    Assert.AreEqual("De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                });

                Assert.IsFalse(exceptionThrown);
                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }
        }

        [Test]
        public void ConvertIllustrationPointsResult_DikeHeightGeneralResultNull_WarnErrorMessage()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = null,
                IllustrationPointsParserErrorMessage = parserError
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var hydraulicBoundary = new TestHydraulicBoundaryLocation();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundary,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    UseForeshore = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                var exceptionThrown = false;
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (ArgumentException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("De HBN berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("De HBN berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[3]);
                    Assert.AreEqual(parserError, msgs[4]);
                    Assert.AreEqual("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[5]);
                    Assert.AreEqual("De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsFalse(exceptionThrown);
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }
        }

        [Test]
        public void DontCalculateAndConvertIllustrationPointsResult_DikeHeightGeneralResultNull_WarnErrorMessage()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            const string parserError = "Parser error message";
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = null,
                IllustrationPointsParserErrorMessage = parserError
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()

            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var hydraulicBoundary = new TestHydraulicBoundaryLocation();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundary,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    UseForeshore = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = false,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                var exceptionThrown = false;
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (ArgumentException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(7, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("De HBN berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("De HBN berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[3]);
                    Assert.AreEqual("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[4]);
                    Assert.AreEqual("De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                });

                Assert.IsFalse(exceptionThrown);
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }
        }

        [Test]
        public void Calculate_ValidInputButOvertoppingIllustrationPointResultsInvalid_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var hydraulicBoundary = new TestHydraulicBoundaryLocation();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundary,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    UseForeshore = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                var exceptionThrown = false;
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (ArgumentException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("De HBN berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("De HBN berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[3]);
                    Assert.AreEqual("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[4]);
                    Assert.AreEqual("De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[5]);
                    Assert.AreEqual("Het uitlezen van illustratiepunten is mislukt.", msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsFalse(exceptionThrown);
                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }
        }

        [Test]
        public void Calculate_ValidInputButOvertoppingRateIllustrationPointResultsInvalid_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var hydraulicBoundary = new TestHydraulicBoundaryLocation();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundary,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    UseForeshore = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                var exceptionThrown = false;
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (ArgumentException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("De HBN berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("De HBN berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[3]);
                    Assert.AreEqual("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[4]);
                    Assert.AreEqual("De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[5]);
                    Assert.AreEqual("Het uitlezen van illustratiepunten is mislukt.", msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsFalse(exceptionThrown);
                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }
        }

        [Test]
        public void Calculate_ValidInputButDikeHeightIllustrationPointResultsInvalid_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(testDataPath)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFile);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var hydraulicBoundary = new TestHydraulicBoundaryLocation();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundary,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    UseForeshore = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                var exceptionThrown = false;
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFile);
                    }
                    catch (ArgumentException e)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("De HBN berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("De HBN berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[3]);
                    Assert.AreEqual("Het uitlezen van illustratiepunten is mislukt.", msgs[4]);
                    Assert.AreEqual("De overslagdebiet berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[5]);
                    Assert.AreEqual("De overslagdebiet berekening voor grasbekleding erosie kruin en binnentalud 'Nieuwe berekening' is niet geconvergeerd.", msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
                });

                Assert.IsFalse(exceptionThrown);
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.DikeHeightOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingRateOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.OvertoppingOutput.HasGeneralResult);
            }
        }

        private static GrassCoverErosionInwardsFailureMechanism CreateGrassCoverErosionInwardsFailureMechanism()
        {
            return new GrassCoverErosionInwardsFailureMechanism
            {
                Contribution = 10
            };
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

        private static GrassCoverErosionInwardsCalculation GetCalculationWithBreakWater(string name,
                                                                                        double breakWaterHeight)
        {
            return new GrassCoverErosionInwardsCalculation
            {
                Name = name,
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
    }
}