// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Ringtoets.GrassCoverErosionInwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            string invalidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_InvalidPreprocessorDirectory_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "NonExistingPreprocessorDirectory";

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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

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

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            string invalidFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_WithoutImportedHydraulicBoundaryDatabase_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_NoDikeProfile_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_ValidInputAndInvalidBreakWaterHeight_LogsMessageAndReturnFalse(double breakWaterHeight)
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetCalculationWithBreakWater(breakWaterHeight);
            calculation.InputParameters.UseBreakWater = true;

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

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

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidInputAndInvalidOrientation_LogsMessageAndReturnFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_ValidInputAndInvalidDikeHeight_LogsMessageAndReturnFalse(double dikeHeight)
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_InvalidNormForDikeHeightCalculation_LogsMessageAndReturnTrue()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(),
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability
                }
            };

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("De HBN berekening kan niet worden uitgevoerd. " +
                                        "Doelkans is te klein om een berekening uit te kunnen voeren.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsTrue(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidNormForOvertoppingRateCalculation_LogsMessageAndReturnTrue()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(),
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability
                }
            };

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("De overslagdebiet berekening kan niet worden uitgevoerd. " +
                                        "Doelkans is te klein om een berekening uit te kunnen voeren.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsTrue(isValid);

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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetCalculationWithBreakWater(breakWaterHeight);
            calculation.InputParameters.UseBreakWater = useBreakWater;

            // Call
            var isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

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
        public void Validate_ValidInputWithCanUseProcessorFalse_ReturnsTrue()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

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
        public void Validate_ValidInputWithUseProcessorTrue_ReturnsTrue()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = testDataPath;

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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

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
        public void Validate_ValidInputWithUseProcessorFalse_ReturnsTrue()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

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
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation,
                                                                                              grassCoverErosionInwardsFailureMechanism,
                                                                                              assessmentSection);

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
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationService().Calculate(null,
                                                                                                 assessmentSection,
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
        public void Calculate_GeneralInputNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                                 assessmentSection,
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
                                                                DikeHeightCalculationType.NoCalculation)]
                                                            DikeHeightCalculationType dikeHeightCalculationType,
                                                            [Values(OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                                                                OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability,
                                                                OvertoppingRateCalculationType.NoCalculation)]
                                                            OvertoppingRateCalculationType overtoppingRateCalculationType,
                                                            [Values(true, false)] bool calculateIllustrationPoints)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(new TestOvertoppingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            calculatorFactory.Stub(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestHydraulicLoadsCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            });
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
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
                                                                           assessmentSection,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           validFilePath);
            }

            // Assert
            OvertoppingOutput overtoppingOutput = calculation.Output.OvertoppingOutput;
            Assert.IsFalse(double.IsNaN(overtoppingOutput.WaveHeight));
            Assert.IsFalse(double.IsNaN(overtoppingOutput.Reliability));
            Assert.IsFalse(overtoppingOutput.IsOvertoppingDominant);
            Assert.AreEqual(calculateIllustrationPoints, calculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(calculateIllustrationPoints, overtoppingOutput.HasGeneralResult);

            if (dikeHeightCalculationType != DikeHeightCalculationType.NoCalculation)
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

            if (overtoppingRateCalculationType != OvertoppingRateCalculationType.NoCalculation)
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
        [TestCase(DikeHeightCalculationType.CalculateByAssessmentSectionNorm, TestName = "Calculate_DikeHeightCalculationFails_OutputNotNull(AssessmentSectionNorm)")]
        [TestCase(DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability, TestName = "Calculate_DikeHeightCalculationFails_OutputNotNull(ProfileProbability)")]
        public void Calculate_DikeHeightCalculationFails_OutputNotNull(DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            // Call
            Action call = () =>
            {
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                               assessmentSection,
                                                                               failureMechanism.GeneralInput,
                                                                               failureMechanism.Contribution,
                                                                               validFilePath);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            // Call
            Action call = () =>
            {
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                               assessmentSection,
                                                                               failureMechanism.GeneralInput,
                                                                               failureMechanism.Contribution,
                                                                               validFilePath);
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
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
                                  grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                  grassCoverErosionInwardsFailureMechanism.Contribution,
                                  validFilePath);

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
                DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability)]
            DikeHeightCalculationType dikeHeightCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(),
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
                                  assessmentSection,
                                  failureMechanism.GeneralInput,
                                  failureMechanism.Contribution,
                                  validFilePath);

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
                OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability)]
            OvertoppingRateCalculationType overtoppingRateCalculationType)
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(),
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
                                  assessmentSection,
                                  failureMechanism.GeneralInput,
                                  failureMechanism.Contribution,
                                  validFilePath);

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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
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
                                                                                   assessmentSection,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFilePath);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
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
                                                                                   assessmentSection,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   validFilePath);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = false
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
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
                                                                                   assessmentSection,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                               assessmentSection,
                                                                               failureMechanism.GeneralInput,
                                                                               failureMechanism.Contribution,
                                                                               validFilePath);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository,
                                                                                                           validFilePath);

            var dikeHeightCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                               assessmentSection,
                                                                               failureMechanism.GeneralInput,
                                                                               failureMechanism.Contribution,
                                                                               validFilePath);
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = dikeHeightCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                               assessmentSection,
                                                                               failureMechanism.GeneralInput,
                                                                               failureMechanism.Contribution,
                                                                               validFilePath);
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

            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             }).Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                               assessmentSection,
                                                                               failureMechanism.GeneralInput,
                                                                               failureMechanism.Contribution,
                                                                               validFilePath);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository,
                                                                                                           validFilePath);

            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = true
            };
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(new TestHydraulicLoadsCalculator());
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                               assessmentSection,
                                                                               failureMechanism.GeneralInput,
                                                                               failureMechanism.Contribution,
                                                                               validFilePath);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var overtoppingCalculator = new TestOvertoppingCalculator();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator
            {
                EndInFailure = false,
                LastErrorFileContent = "An error occurred"
            };
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            DikeProfile dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    OvertoppingRateCalculationType = overtoppingRateCalculationType
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                               assessmentSection,
                                                                               failureMechanism.GeneralInput,
                                                                               failureMechanism.Contribution,
                                                                               validFilePath);
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
            }
        }

        [Test]
        public void Calculate_ValidInputButOvertoppingGeneralResultNull_IllustrationPointsNotSetAndLogs()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();
            calculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();
            calculation.InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();
            calculation.InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
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
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();
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
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);
            mockRepository.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = GetValidCalculationWithCalculateIllustrationPointsSettings();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                               assessmentSection,
                                                                                               failureMechanism.GeneralInput,
                                                                                               failureMechanism.Contribution,
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
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
        public void Calculate_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculators()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator();
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);

            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = GetDikeProfile(),
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSection,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           validFilePath);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_ExpectedPreprocessorDirectorySetToCalculators()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator();
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, validPreprocessorDirectory)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, validPreprocessorDirectory)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.AreEqual(validPreprocessorDirectory, settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = validPreprocessorDirectory;

            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = GetDikeProfile(),
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSection,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           validFilePath);
            }

            // Assert
            Assert.IsTrue(overtoppingCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            Assert.IsTrue(dikeHeightCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            Assert.IsTrue(overtoppingRateCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculators()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateGrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var dikeHeightCalculator = new TestHydraulicLoadsCalculator();
            var overtoppingRateCalculator = new TestHydraulicLoadsCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            calculatorFactory.Expect(cf => cf.CreateDikeHeightCalculator(testDataPath, string.Empty)).Return(dikeHeightCalculator);
            calculatorFactory.Expect(cf => cf.CreateOvertoppingRateCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(overtoppingRateCalculator);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "NonExistingPreprocessorDirectory";

            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = GetDikeProfile(),
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSection,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           validFilePath);
            }

            // Assert
            Assert.IsFalse(overtoppingCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            Assert.IsFalse(dikeHeightCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            Assert.IsFalse(overtoppingRateCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculateDikeHeightWithInvalidNorm_DikeHeightOutputNull()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(),
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability
                }
            };

            var service = new GrassCoverErosionInwardsCalculationService();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                service.Calculate(calculation,
                                  assessmentSection,
                                  grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                  grassCoverErosionInwardsFailureMechanism.Contribution,
                                  validFilePath);

                // Assert
                Assert.IsNull(calculation.Output.DikeHeightOutput);
            }
        }

        [Test]
        public void Calculate_CalculateOvertoppingRateWithInvalidNorm_OvertoppingRateOutputNull()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            var overtoppingCalculator = new TestOvertoppingCalculator();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath, string.Empty)).Return(overtoppingCalculator);
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile(),
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability
                }
            };

            var service = new GrassCoverErosionInwardsCalculationService();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                service.Calculate(calculation,
                                  assessmentSection,
                                  grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                  grassCoverErosionInwardsFailureMechanism.Contribution,
                                  validFilePath);

                // Assert
                Assert.IsNull(calculation.Output.OvertoppingRateOutput);
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
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                    ShouldDikeHeightIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                }
            };
        }
    }
}