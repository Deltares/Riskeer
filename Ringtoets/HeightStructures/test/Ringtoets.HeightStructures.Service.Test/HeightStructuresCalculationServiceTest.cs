﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// You should have received meanOne copy of the GNU General Public License
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Service.Test
{
    [TestFixture]
    public class HeightStructuresCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        #region Testcases

        private static IEnumerable<TestCaseData> NormalDistributionsWithInvalidMeans
        {
            get
            {
                yield return new TestCaseData(double.NaN, 1, 2, "modelfactor overloopdebiet volkomen overlaat");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, "modelfactor overloopdebiet volkomen overlaat");
                yield return new TestCaseData(double.NegativeInfinity, 1, 2, "modelfactor overloopdebiet volkomen overlaat");

                yield return new TestCaseData(1, double.NaN, 2, "kerende hoogte");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, "kerende hoogte");
                yield return new TestCaseData(1, double.NegativeInfinity, 2, "kerende hoogte");

                yield return new TestCaseData(1, 2, double.NaN, "breedte van doorstroomopening");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, "breedte van doorstroomopening");
                yield return new TestCaseData(1, 2, double.NegativeInfinity, "breedte van doorstroomopening");
            }
        }

        private static IEnumerable<TestCaseData> LogNormalDistributionsWithInvalidMeans
        {
            get
            {
                yield return new TestCaseData(double.NaN, 1, 2, 3, 4, "stormduur");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, 3, 4, "stormduur");

                yield return new TestCaseData(1, double.NaN, 2, 3, 4, "toegestane peilverhoging komberging");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, 3, 4, "toegestane peilverhoging komberging");

                yield return new TestCaseData(1, 2, double.NaN, 3, 4, "kombergend oppervlak");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, 3, 4, "kombergend oppervlak");

                yield return new TestCaseData(1, 2, 3, double.NaN, 4, "stroomvoerende breedte bodembescherming");
                yield return new TestCaseData(1, 2, 3, double.PositiveInfinity, 4, "stroomvoerende breedte bodembescherming");

                yield return new TestCaseData(1, 2, 3, 4, double.NaN, "kritiek instromend debiet");
                yield return new TestCaseData(1, 2, 3, 4, double.PositiveInfinity, "kritiek instromend debiet");
            }
        }

        #endregion

        [Test]
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            const string name = "<very nice name>";

            HeightStructuresCalculation calculation = new HeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    Structure =  new TestHeightStructure()
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = new HeightStructuresCalculationService().Validate(calculation, assessmentSectionStub);

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
        public void Validate_CalculationInputWithoutStructureValidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            HeightStructuresCalculation calculation = new HeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = new HeightStructuresCalculationService().Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen kunstwerk geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_CalculationInputWithoutHydraulicBoundaryLocationValidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            HeightStructuresCalculation calculation = new HeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    Structure = new TestHeightStructure()
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = new HeightStructuresCalculationService().Validate(calculation, assessmentSectionStub);

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
        [TestCaseSource("NormalDistributionsWithInvalidMeans")]
        public void Validate_NormalDistributionMeanInvalid_ReturnsFalse(double meanOne, double meanTwo, double meanThree, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";
            string expectedValidationMessage = string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een geldig getal zijn.", parameterName);

            var calculation = new TestHeightStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    Structure = new TestHeightStructure()
                },
            };

            calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean = (RoundedDouble) meanOne;
            calculation.InputParameters.LevelCrestStructure.Mean = (RoundedDouble) meanTwo;
            calculation.InputParameters.WidthFlowApertures.Mean = (RoundedDouble) meanThree;

            // Call
            bool isValid = false; 
            Action call = () => isValid = new HeightStructuresCalculationService().Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource("LogNormalDistributionsWithInvalidMeans")]
        public void Validate_LogNormalDistributionMeanInvalid_ReturnsFalse(double meanOne, double meanTwo, double meanThree,
                                                                           double meanFour, double meanFive, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";
            string expectedValidationMessage = string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterName);

            var calculation = new TestHeightStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    Structure = new TestHeightStructure()
                }
            };

            calculation.InputParameters.StormDuration.Mean = (RoundedDouble) meanOne;
            calculation.InputParameters.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) meanTwo;
            calculation.InputParameters.StorageStructureArea.Mean = (RoundedDouble) meanThree;
            calculation.InputParameters.FlowWidthAtBottomProtection.Mean = (RoundedDouble) meanFour;
            calculation.InputParameters.CriticalOvertoppingDischarge.Mean = (RoundedDouble) meanFive;

            // Call
            bool isValid = false;
            Action call = () => isValid = new HeightStructuresCalculationService().Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidCalculationInputAndHydraulicBoundaryDatabase_ReturnsTrue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            HeightStructuresCalculation calculation = new HeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    Structure = new TestHeightStructure()
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = new HeightStructuresCalculationService().Validate(calculation, assessmentSectionStub);

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
        public void Calculate_ValidCalculation_LogStartAndEndAndReturnOutput()
        {
            // Setup
            var heightStructuresFailureMechanism = new HeightStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(heightStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            heightStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            var failureMechanismSection = heightStructuresFailureMechanism.Sections.First();

            // Call
            Action call = () => new HeightStructuresCalculationService().Calculate(calculation,
                                                                                   assessmentSectionStub,
                                                                                   failureMechanismSection,
                                                                                   heightStructuresFailureMechanism.GeneralInput,
                                                                                   heightStructuresFailureMechanism.Contribution,
                                                                                   testDataPath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith("Hoogte kunstwerken berekeningsverslag. Klik op details voor meer informatie.", msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
            Assert.IsNotNull(calculation.Output);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_InvalidCalculation_LogStartAndEndAndErrorMessageAndThrowsException()
        {
            // Setup
            var heightStructuresFailureMechanism = new HeightStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(heightStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            heightStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
                }
            };

            var failureMechanismSection = heightStructuresFailureMechanism.Sections.First();
            var exception = false;

            // Call
            Action call = () =>
            {
                try
                {
                    new HeightStructuresCalculationService().Calculate(calculation,
                                                                       assessmentSectionStub,
                                                                       failureMechanismSection,
                                                                       heightStructuresFailureMechanism.GeneralInput,
                                                                       heightStructuresFailureMechanism.Contribution,
                                                                       testDataPath);
                }
                catch
                {
                    exception = true;
                }
            };

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("De berekening voor hoogte kunstwerk '{0}' is niet gelukt.", calculation.Name), msgs[1]);
                StringAssert.StartsWith("Hoogte kunstwerken berekeningsverslag. Klik op details voor meer informatie.", msgs[2]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
            });
            Assert.IsNull(calculation.Output);
            Assert.IsTrue(exception);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Calculate_VariousCalculations_InputPropertiesCorrectlySentToCalculator(bool useForeshore, bool useBreakWater)
        {
            // Setup
            var heightStructuresFailureMechanism = new HeightStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(heightStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            heightStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            HeightStructuresCalculation calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                                                    new[]
                                                                                    {
                                                                                        new Point2D(1, 1),
                                                                                        new Point2D(2, 2)
                                                                                    },
                                                                                    useBreakWater ? new BreakWater(BreakWaterType.Wall, 3.0) : null,
                                                                                    new ForeshoreProfile.ConstructionProperties());
            }

            FailureMechanismSection failureMechanismSection = heightStructuresFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testStructuresOvertoppingCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresOvertoppingCalculator;

                // Call
                new HeightStructuresCalculationService().Calculate(calculation,
                                                                   assessmentSectionStub,
                                                                   failureMechanismSection,
                                                                   heightStructuresFailureMechanism.GeneralInput,
                                                                   heightStructuresFailureMechanism.Contribution,
                                                                   testDataPath);

                // Assert
                StructuresOvertoppingCalculationInput[] overtoppingCalculationInputs = testStructuresOvertoppingCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, overtoppingCalculationInputs.Length);
                Assert.AreEqual(testDataPath, testStructuresOvertoppingCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, testStructuresOvertoppingCalculator.RingId);

                GeneralHeightStructuresInput generalInput = heightStructuresFailureMechanism.GeneralInput;
                HeightStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresOvertoppingCalculationInput(
                    1300001,
                    new HydraRingSection(1, failureMechanismSection.GetSectionLength(), input.StructureNormalOrientation),
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    useBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null,
                    generalInput.GravitationalAcceleration,
                    generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                    input.LevelCrestStructure.Mean, input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.ModelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.CoefficientOfVariation,
                    input.DeviationWaveDirection,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation);

                StructuresOvertoppingCalculationInput actualInput = overtoppingCalculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(testStructuresOvertoppingCalculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            var heightStructuresFailureMechanism = new HeightStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(heightStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            heightStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            HeightStructuresCalculation calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            FailureMechanismSection failureMechanismSection = heightStructuresFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testStructuresOvertoppingCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresOvertoppingCalculator;
                var service = new HeightStructuresCalculationService();
                testStructuresOvertoppingCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  assessmentSectionStub,
                                  failureMechanismSection,
                                  heightStructuresFailureMechanism.GeneralInput,
                                  heightStructuresFailureMechanism.Contribution,
                                  testDataPath);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(testStructuresOvertoppingCalculator.IsCanceled);
            }
        }

        private static
            IAssessmentSection CreateAssessmentSectionStub(IFailureMechanism failureMechanism, MockRepository mockRepository)
        {
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Stub(a => a.Id).Return("21");
            assessmentSectionStub.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, 2));
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0)
                }
            };
            return assessmentSectionStub;
        }
    }
}