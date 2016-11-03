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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsErrorAndReturnsFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties())
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var invalidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           invalidFilePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties())
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_NoDikeProfile_ReturnsTrue()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    Orientation = (RoundedDouble) 0
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen dijkprofiel geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_ValidInputAndValidateBreakWaterHeight_ReturnsFalse(double breakWaterHeight)
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = GetCalculationWithBreakWater(name, breakWaterHeight);
            calculation.InputParameters.UseBreakWater = true;

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen geldige damhoogte ingevoerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidInputAndInvalidOrientation_ReturnsFalse()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation()
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties()
                                                  {
                                                      Orientation = RoundedDouble.NaN
                                                  })
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen concreet getal ingevoerd voor 'oriëntatie'.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
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
        public void Validate_ValidInputAndValidateBreakWaterHeight_ReturnsTrue(bool useBreakWater, double breakWaterHeight)
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = GetCalculationWithBreakWater(name, breakWaterHeight);
            calculation.InputParameters.UseBreakWater = useBreakWater;

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
            });
            Assert.IsTrue(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidInput_ReturnsTrue()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties())
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = GrassCoverErosionInwardsCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
            });
            Assert.IsTrue(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void Calculate_CalculationValid_ReturnOutput(bool useForeland, bool calculateDikeHeight)
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(grassCoverErosionInwardsFailureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            var dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    CalculateDikeHeight = calculateDikeHeight,
                    UseForeshore = useForeland
                }
            };

            var failureMechanismSection = grassCoverErosionInwardsFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanismSection,
                                                                           grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                                                           grassCoverErosionInwardsFailureMechanism.Contribution,
                                                                           testDataPath);
            }

            // Assert
            Assert.IsFalse(double.IsNaN(calculation.Output.WaveHeight));
            Assert.IsNotNull(calculation.Output.ProbabilityAssessmentOutput);
            Assert.IsFalse(double.IsNaN(calculation.Output.ProbabilityAssessmentOutput.FactorOfSafety));
            Assert.IsFalse(double.IsNaN(calculation.Output.ProbabilityAssessmentOutput.Probability));
            Assert.IsFalse(double.IsNaN(calculation.Output.ProbabilityAssessmentOutput.Reliability));
            Assert.IsFalse(double.IsNaN(calculation.Output.ProbabilityAssessmentOutput.RequiredProbability));
            Assert.IsFalse(double.IsNaN(calculation.Output.ProbabilityAssessmentOutput.RequiredReliability));
            Assert.AreNotEqual(calculateDikeHeight, double.IsNaN(calculation.Output.DikeHeight));
            Assert.AreEqual(calculateDikeHeight, calculation.Output.DikeHeightCalculated);
            Assert.IsFalse(calculation.Output.IsOvertoppingDominant);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_DikeHeightCalculationFails_OutputNaN()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(grassCoverErosionInwardsFailureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            var dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    CalculateDikeHeight = true
                }
            };

            var failureMechanismSection = grassCoverErosionInwardsFailureMechanism.Sections.First();
            bool expectedExceptionThrown = false;

            // Call
            Action call = () =>
            {
                try
                {
                    using (new HydraRingCalculatorFactoryConfig())
                    {
                        var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                        calculator.EndInFailure = true;

                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanismSection,
                                                                                   grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                                                                   grassCoverErosionInwardsFailureMechanism.Contribution,
                                                                                   testDataPath);
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
                var msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[1]);
                StringAssert.StartsWith(string.Format("De HBN berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt.", calculation.Name), msgs[2]);
                StringAssert.StartsWith("Dijkhoogte berekening is uitgevoerd op de tijdelijke locatie:", msgs[3]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[4]);
            });
            Assert.IsFalse(expectedExceptionThrown);
            Assert.IsNotNull(calculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_OvertoppingOnlyAndOvertoppingCalculationFails_OutputNull()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(grassCoverErosionInwardsFailureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            var dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                }
            };

            var failureMechanismSection = grassCoverErosionInwardsFailureMechanism.Sections.First();
            bool expectedExceptionThrown = false;

            // Call
            Action call = () =>
            {
                try
                {
                    using (new HydraRingCalculatorFactoryConfig())
                    {
                        var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                        calculator.EndInFailure = true;

                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanismSection,
                                                                                   grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                                                                   grassCoverErosionInwardsFailureMechanism.Contribution,
                                                                                   testDataPath);
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
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("De berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt.", calculation.Name), msgs[1]);
                StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
            });
            Assert.IsTrue(expectedExceptionThrown);
            Assert.IsNull(calculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelWithValidOvertoppingCalculationInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(grassCoverErosionInwardsFailureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties()),
                }
            };

            var failureMechanismSection = grassCoverErosionInwardsFailureMechanism.Sections.First();
            var service = new GrassCoverErosionInwardsCalculationService();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  assessmentSectionStub,
                                  failureMechanismSection,
                                  grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                  grassCoverErosionInwardsFailureMechanism.Contribution,
                                  testDataPath);

                // Assert
                Assert.IsTrue(calculator.IsCanceled);
                Assert.IsNull(calculation.Output);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_CancelWithValidDikeCalculationInput_CancelsCalculatorAndHasNullOutput(bool cancelBeforeDikeHeightCalculationStarts)
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(grassCoverErosionInwardsFailureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(grassCoverErosionInwardsFailureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            const string name = "<very nice name>";

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties()),
                    CalculateDikeHeight = true
                }
            };

            var failureMechanismSection = grassCoverErosionInwardsFailureMechanism.Sections.First();
            var service = new GrassCoverErosionInwardsCalculationService();

            // Call
            using (new HydraRingCalculatorFactoryConfig())
            {
                var testFactory = (TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance;
                var overToppingCalculator = testFactory.OvertoppingCalculator;
                var dikeHeightCalculator = testFactory.DikeHeightCalculator;

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
                                  failureMechanismSection,
                                  grassCoverErosionInwardsFailureMechanism.GeneralInput,
                                  grassCoverErosionInwardsFailureMechanism.Contribution,
                                  testDataPath);

                // Assert
                Assert.IsNull(calculation.Output);

                if (cancelBeforeDikeHeightCalculationStarts)
                {
                    Assert.IsTrue(overToppingCalculator.IsCanceled);

                    // dikeheightCalculator is initialized after the overtopping calculation successfully finishes.
                    Assert.IsFalse(dikeHeightCalculator.IsCanceled);
                }
                else
                {
                    Assert.IsTrue(overToppingCalculator.IsCanceled);
                    Assert.IsTrue(dikeHeightCalculator.IsCanceled);
                }
            }
        }

        [Test]
        public void Calculate_OvertoppingCalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(failureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            var dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    CalculateDikeHeight = true
                }
            };

            var failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                calculator.LastErrorContent = "An error occured";
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanismSection,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           testDataPath);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("De berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", calculation.Name), msgs[1]);
                    StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_OvertoppingCalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(failureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            var dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    CalculateDikeHeight = true
                }
            };

            var failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanismSection,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           testDataPath);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("De berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt. Er is geen foutrapport beschikbaar.", calculation.Name), msgs[1]);
                    StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_OvertoppingCalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(failureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            var dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    CalculateDikeHeight = true
                }
            };

            var failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorContent = "An error occured";

                var exceptionThrown = false;
                var exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanismSection,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           testDataPath);
                    }
                    catch (HydraRingFileParserException e)
                    {
                        exceptionThrown = true;
                        exceptionMessage = e.Message;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("De berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.",
                                                          calculation.Name), msgs[1]);
                    StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.AreEqual(calculator.LastErrorContent, exceptionMessage);
            }
        }
        [Test]
        public void Calculate_DikeHeightCalculationFailedWithExceptionAndLastErrorPresent_LogError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(failureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            var dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    CalculateDikeHeight = true
                }
            };

            var failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                calculator.LastErrorContent = "An error occured";
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanismSection,
                                                                                   failureMechanism.GeneralInput,
                                                                                   failureMechanism.Contribution,
                                                                                   testDataPath);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[1]);
                    StringAssert.StartsWith(string.Format("De HBN berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", calculation.Name), msgs[2]);
                    StringAssert.StartsWith("Dijkhoogte berekening is uitgevoerd op de tijdelijke locatie:", msgs[3]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[4]);
                });
                Assert.IsFalse(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_DikeHeightCalculationFailedWithExceptionAndNoLastErrorPresent_LogError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(failureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            var dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    CalculateDikeHeight = true
                }
            };

            var failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanismSection,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           testDataPath);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[1]);
                    StringAssert.StartsWith(string.Format("De HBN berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt. Er is geen foutrapport beschikbaar.", calculation.Name), msgs[2]);
                    StringAssert.StartsWith("Dijkhoogte berekening is uitgevoerd op de tijdelijke locatie:", msgs[3]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[4]);
                });
                Assert.IsFalse(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_DikeHeightCalculationFailedWithoutExceptionAndWithLastErrorPresent_LogError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            AddSectionToFailureMechanism(failureMechanism);

            var filePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository,
                                                                                                           filePath);
            mockRepository.ReplayAll();

            var dikeProfile = GetDikeProfile();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = dikeProfile,
                    CalculateDikeHeight = true
                }
            };

            var failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorContent = "An error occured";

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionInwardsCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanismSection,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.Contribution,
                                                                           testDataPath);
                    }
                    catch (HydraRingFileParserException e)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[1]);
                    StringAssert.StartsWith(string.Format("De HBN berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", calculation.Name), msgs[2]);
                    StringAssert.StartsWith("Dijkhoogte berekening is uitgevoerd op de tijdelijke locatie:", msgs[3]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[4]);
                });
                Assert.IsFalse(exceptionThrown);
            }
        }

        private static void AddSectionToFailureMechanism(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));
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
                                                  new DikeProfile.ConstructionProperties()),
                }
            };
        }
    }
}