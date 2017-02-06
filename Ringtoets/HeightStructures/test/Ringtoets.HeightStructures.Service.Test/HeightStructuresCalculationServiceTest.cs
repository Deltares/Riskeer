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
using System.Collections.Generic;
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.HeightStructures.Service.Test
{
    [TestFixture]
    public class HeightStructuresCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HeightStructuresCalculationService.Validate(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }
 
        [Test]
        public void Validate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new TestHeightStructuresCalculation();

            // Call
            TestDelegate test = () => HeightStructuresCalculationService.Validate(calculation, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            var calculation = new TestHeightStructuresCalculation();

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                var calculationName = calculation.Name;
                StringAssert.StartsWith($"Validatie van '{calculationName}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{calculationName}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidCalculationValidHydraulicBoundaryDatabaseNoSettings_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var calculation = new TestHeightStructuresCalculation();

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                var calculationName = calculation.Name;
                StringAssert.StartsWith($"Validatie van '{calculationName}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{calculationName}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_CalculationInputWithoutStructureValidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen kunstwerk geselecteerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_CalculationInputWithoutHydraulicBoundaryLocationValidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new TestHeightStructuresCalculation
            {
                Name = name
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_StructureNormalOrientationInvalid_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";
            string expectedValidationMessage = "Validatie mislukt: De waarde voor 'oriëntatie' moet een concreet getal zijn.";

            var calculation = new TestHeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    Structure = new TestHeightStructure()
                }
            };

            calculation.InputParameters.StructureNormalOrientation = RoundedDouble.NaN;

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(NormalDistributionsWithInvalidMean))]
        public void Validate_NormalDistributionMeanInvalid_LogsErrorAndReturnsFalse(double meanOne, double meanTwo, double meanThree, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";
            string expectedValidationMessage = $"Validatie mislukt: De verwachtingswaarde voor '{parameterName}' moet een concreet getal zijn.";

            var calculation = new TestHeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean = (RoundedDouble) meanOne;
            calculation.InputParameters.LevelCrestStructure.Mean = (RoundedDouble) meanTwo;
            calculation.InputParameters.WidthFlowApertures.Mean = (RoundedDouble) meanThree;

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(LogNormalDistributionsWithInvalidMean))]
        public void Validate_LogNormalDistributionMeanInvalid_LogsErrorAndReturnsFalse(double meanOne, double meanTwo, double meanThree,
                                                                                       double meanFour, double meanFive, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";
            string expectedValidationMessage = $"Validatie mislukt: De verwachtingswaarde voor '{parameterName}' moet een positief getal zijn.";

            var calculation = new TestHeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            calculation.InputParameters.StormDuration.Mean = (RoundedDouble) meanOne;
            calculation.InputParameters.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) meanTwo;
            calculation.InputParameters.StorageStructureArea.Mean = (RoundedDouble) meanThree;
            calculation.InputParameters.FlowWidthAtBottomProtection.Mean = (RoundedDouble) meanFour;
            calculation.InputParameters.CriticalOvertoppingDischarge.Mean = (RoundedDouble) meanFive;

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(DistributionsWithInvalidDeviation))]
        public void Validate_DistributionStandardDeviationInvalid_LogsErrorAndReturnsFalse(double deviationOne, double deviationTwo,
                                                                                           double deviationThree, double deviationFour,
                                                                                           double deviationFive, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";
            string expectedValidationMessage = $"Validatie mislukt: De standaardafwijking voor '{parameterName}' moet groter zijn dan of gelijk zijn aan 0.";

            var calculation = new TestHeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation = (RoundedDouble) deviationOne;
            calculation.InputParameters.LevelCrestStructure.StandardDeviation = (RoundedDouble) deviationTwo;
            calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation = (RoundedDouble) deviationThree;
            calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation = (RoundedDouble) deviationFour;
            calculation.InputParameters.WidthFlowApertures.StandardDeviation = (RoundedDouble) deviationFive;

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(DistributionsWithInvalidCoefficient))]
        public void Validate_DistributionVariationCoefficientInvalid_LogsErrorAndReturnsFalse(
            double coefficientOne, double coefficientTwo, double coefficientThree, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";
            string expectedValidationMessage = $"Validatie mislukt: De variatiecoëfficiënt voor '{parameterName}' moet groter zijn dan of gelijk zijn aan 0.";

            var calculation = new TestHeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            calculation.InputParameters.StormDuration.CoefficientOfVariation = (RoundedDouble) coefficientOne;
            calculation.InputParameters.StorageStructureArea.CoefficientOfVariation = (RoundedDouble) coefficientTwo;
            calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation = (RoundedDouble) coefficientThree;

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_UsesBreakWaterAndHasInvalidBreakWaterSettings_LogsErrorAndReturnsFalse(double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new TestHeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight)),
                    UseBreakWater = true
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidCalculationInputAndHydraulicBoundaryDatabase_ReturnsTrue()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new TestHeightStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            // Call
            bool isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
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
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            TestDelegate test = () => new HeightStructuresCalculationService().Calculate(null,
                                                                                         assessmentSection,
                                                                                         failureMechanism,
                                                                                         string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new TestHeightStructuresCalculation();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            TestDelegate test = () => new HeightStructuresCalculationService().Calculate(calculation,
                                                                                         null,
                                                                                         failureMechanism,
                                                                                         string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Calculate_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestHeightStructuresCalculation();

            // Call
            TestDelegate test = () => new HeightStructuresCalculationService().Calculate(calculation,
                                                                                         assessmentSection,
                                                                                         null,
                                                                                         string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        public void Calculate_ValidCalculationInputAndForeshoreWithValidBreakWater_LogStartAndEndAndReturnOutput(CalculationType calculationType)
        {
            // Setup
            var heightStructuresFailureMechanism = new HeightStructuresFailureMechanism();
            heightStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(heightStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseBreakWater = true,
                    UseForeshore = true
                }
            };

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

            // Call
            using (new HydraRingCalculatorFactoryConfig())
            {
                Action call = () => new HeightStructuresCalculationService().Calculate(calculation,
                                                                                       assessmentSectionStub,
                                                                                       heightStructuresFailureMechanism,
                                                                                       validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie" +
                                            "" +
                                            "", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[2]);
                });
                Assert.IsNotNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Calculate_ValidCalculationInputAndForeshoreWithInvalidBreakWater_LogStartAndEndAndReturnOutput(double height)
        {
            // Setup
            var heightStructuresFailureMechanism = new HeightStructuresFailureMechanism();
            heightStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(heightStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseBreakWater = false,
                    UseForeshore = true
                }
            };

            // Call
            using (new HydraRingCalculatorFactoryConfig())
            {
                Action call = () => new HeightStructuresCalculationService().Calculate(calculation,
                                                                                       assessmentSectionStub,
                                                                                       heightStructuresFailureMechanism,
                                                                                       validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[2]);
                });
                Assert.IsNotNull(calculation.Output);
            }
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
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(heightStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(useBreakWater);
            }

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testStructuresOvertoppingCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresOvertoppingCalculator;

                // Call
                new HeightStructuresCalculationService().Calculate(calculation,
                                                                   assessmentSectionStub,
                                                                   heightStructuresFailureMechanism,
                                                                   validFilePath);

                // Assert
                StructuresOvertoppingCalculationInput[] overtoppingCalculationInputs = testStructuresOvertoppingCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, overtoppingCalculationInputs.Length);
                Assert.AreEqual(testDataPath, testStructuresOvertoppingCalculator.HydraulicBoundaryDatabaseDirectory);

                GeneralHeightStructuresInput generalInput = heightStructuresFailureMechanism.GeneralInput;
                HeightStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresOvertoppingCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
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
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.StandardDeviation,
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
            heightStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(heightStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testStructuresOvertoppingCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresOvertoppingCalculator;
                var service = new HeightStructuresCalculationService();
                testStructuresOvertoppingCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  assessmentSectionStub,
                                  heightStructuresFailureMechanism,
                                  validFilePath);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(testStructuresOvertoppingCalculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresOvertoppingCalculator;
                calculator.LastErrorFileContent = "An error occurred";
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new HeightStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanism,
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
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"De berekening voor hoogte kunstwerk '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresOvertoppingCalculator;
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new HeightStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanism,
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
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"De berekening voor hoogte kunstwerk '{calculation.Name}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresOvertoppingCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorFileContent = "An error occurred";

                var exceptionThrown = false;
                var exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new HeightStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanism,
                                                                           validFilePath);
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
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith($"De berekening voor hoogte kunstwerk '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }
            mockRepository.VerifyAll();
        }

        #region Testcases

        private static IEnumerable<TestCaseData> NormalDistributionsWithInvalidMean
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

        private static IEnumerable<TestCaseData> LogNormalDistributionsWithInvalidMean
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

        private static IEnumerable<TestCaseData> DistributionsWithInvalidDeviation
        {
            get
            {
                yield return new TestCaseData(double.NaN, 1, 2, 3, 4, "modelfactor overloopdebiet volkomen overlaat");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, 3, 4, "modelfactor overloopdebiet volkomen overlaat");

                yield return new TestCaseData(1, double.NaN, 2, 3, 4, "kerende hoogte");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, 3, 4, "kerende hoogte");

                yield return new TestCaseData(1, 2, double.NaN, 3, 4, "toegestane peilverhoging komberging");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, 3, 4, "toegestane peilverhoging komberging");

                yield return new TestCaseData(1, 2, 3, double.NaN, 4, "stroomvoerende breedte bodembescherming");
                yield return new TestCaseData(1, 2, 3, double.PositiveInfinity, 4, "stroomvoerende breedte bodembescherming");

                yield return new TestCaseData(1, 2, 3, 4, double.NaN, "breedte van doorstroomopening");
                yield return new TestCaseData(1, 2, 3, 4, double.PositiveInfinity, "breedte van doorstroomopening");
            }
        }

        private static IEnumerable<TestCaseData> DistributionsWithInvalidCoefficient
        {
            get
            {
                yield return new TestCaseData(double.NaN, 1, 2, "stormduur");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, "stormduur");

                yield return new TestCaseData(1, double.NaN, 2, "kombergend oppervlak");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, "kombergend oppervlak");

                yield return new TestCaseData(1, 2, double.NaN, "kritiek instromend debiet");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, "kritiek instromend debiet");
            }
        }

        #endregion
    }
}