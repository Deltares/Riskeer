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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen dijkprofiel geselecteerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'oriëntatie' moet een concreet getal zijn.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'dijkhoogte' moet een concreet getal zijn.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[1]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[1]);
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
                                                                DikeHeightCalculationType.NoCalculation)] DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
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
                    UseForeshore = useForeland
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           validFile);
            }

            // Assert
            Assert.IsFalse(double.IsNaN(calculation.Output.WaveHeight));
            ProbabilityAssessmentOutput probabilityAssessmentOutput = calculation.Output.ProbabilityAssessmentOutput;
            Assert.IsNotNull(probabilityAssessmentOutput);
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.FactorOfSafety));
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.Probability));
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.Reliability));
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.RequiredProbability));
            Assert.IsFalse(double.IsNaN(probabilityAssessmentOutput.RequiredReliability));
            Assert.IsFalse(calculation.Output.IsOvertoppingDominant);
            if (dikeHeightCalculationType != DikeHeightCalculationType.NoCalculation)
            {
                SubCalculationAssessmentOutput dikeHeightAssessmentOutput = calculation.Output.DikeHeightAssessmentOutput;
                Assert.IsNotNull(dikeHeightAssessmentOutput);

                Assert.IsFalse(double.IsNaN(dikeHeightAssessmentOutput.Result));
                Assert.IsFalse(double.IsNaN(dikeHeightAssessmentOutput.TargetProbability));
                Assert.IsFalse(double.IsNaN(dikeHeightAssessmentOutput.TargetReliability));
                Assert.IsFalse(double.IsNaN(dikeHeightAssessmentOutput.CalculatedProbability));
                Assert.IsFalse(double.IsNaN(dikeHeightAssessmentOutput.CalculatedReliability));
            }
            else
            {
                Assert.IsNull(calculation.Output.DikeHeightAssessmentOutput);
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
                    using (new HydraRingCalculatorFactoryConfig())
                    {
                        TestDikeHeightCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                        calculator.EndInFailure = true;
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
                StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                StringAssert.StartsWith($"De HBN berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt.", msgs[2]);
                StringAssert.StartsWith("Dijkhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[4]);
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestOvertoppingCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  assessmentSectionStub,
                                  grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                  grassCoverErosionInwardsFailureMechanism.Contribution,
                                  validFile);

                // Assert
                Assert.IsTrue(calculator.IsCanceled);
                Assert.IsNull(calculation.Output);
            }
        }

        [Test]
        [Combinatorial]
        public void Calculate_CancelWithValidDikeCalculationInput_CancelsCalculatorAndHasNullOutput(
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
            using (new HydraRingCalculatorFactoryConfig())
            {
                var testFactory = (TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance;
                TestOvertoppingCalculator overToppingCalculator = testFactory.OvertoppingCalculator;
                TestDikeHeightCalculator dikeHeightCalculator = testFactory.DikeHeightCalculator;

                if (cancelBeforeDikeHeightCalculationStarts)
                {
                    overToppingCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();
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
                Assert.IsTrue(overToppingCalculator.IsCanceled);
                Assert.IsTrue(dikeHeightCalculator.IsCanceled);
            }
        }

        [Test]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_OvertoppingCalculationFailedWithExceptionLastError_LogErrorAndThrowException(AssessmentSectionNorm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_OvertoppingCalculationFailedWithExceptionLastError_LogErrorAndThrowException(ProfileProbability)")]
        [TestCase(DikeHeightCalculationType.NoCalculation, TestName = "Calculate_OvertoppingCalculationFailedWithExceptionLastError_LogErrorAndThrowException(NoCalculation)")]
        public void Calculate_OvertoppingCalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
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
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestOvertoppingCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                calculator.LastErrorFileContent = "An error occurred";
                calculator.EndInFailure = true;

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
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"De overloop en overslag berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[3]);
                });
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(exceptionThrown);
            }
        }

        [Test]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_OvertoppingCalculationFailedWithExceptionNoLastError_LogErrorThrowException(AssessmentSectionNorm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_OvertoppingCalculationFailedWithExceptionNoLastError_LogErrorThrowException(ProfileProbability)")]
        [TestCase(DikeHeightCalculationType.NoCalculation, TestName = "Calculate_OvertoppingCalculationFailedWithExceptionNoLastError_LogErrorThrowException(NoCalculation)")]
        public void Calculate_OvertoppingCalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
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
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestOvertoppingCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                calculator.EndInFailure = true;

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
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"De overloop en overslag berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
            }
        }

        [Test]
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_OvertoppingCalculationFailedNoExceptionWithLastError_LogErrorThrowException(AssessmentSectionNorm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_OvertoppingCalculationFailedNoExceptionWithLastError_LogErrorThrowException(ProfileProbability)")]
        [TestCase(DikeHeightCalculationType.NoCalculation, TestName = "Calculate_OvertoppingCalculationFailedNoExceptionWithLastError_LogErrorThrowException(NoCalculation)")]
        public void Calculate_OvertoppingCalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
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
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestOvertoppingCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorFileContent = "An error occurred";

                var exceptionThrown = false;
                string exceptionMessage = string.Empty;

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
                        exceptionThrown = true;
                        exceptionMessage = e.Message;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"De overloop en overslag berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
                Assert.IsNull(calculation.Output);
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDikeHeightCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                calculator.LastErrorFileContent = "An error occurred";
                calculator.EndInFailure = true;
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
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                    StringAssert.StartsWith($"De HBN berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[2]);
                    StringAssert.StartsWith("Dijkhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[4]);
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDikeHeightCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                calculator.EndInFailure = true;
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
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                    StringAssert.StartsWith($"De HBN berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[2]);
                    StringAssert.StartsWith("Dijkhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[4]);
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDikeHeightCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorFileContent = "An error occurred";
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
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                    StringAssert.StartsWith($"De HBN berekening voor grasbekleding erosie kruin en binnentalud '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[2]);
                    StringAssert.StartsWith("Dijkhoogte berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[4]);
                    Assert.AreEqual(5, msgs.Length);
                });
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(exceptionThrown);
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