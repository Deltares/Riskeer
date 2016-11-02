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
using System.ComponentModel;
using System.IO;
using System.Linq;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.ClosingStructures.Service.Test
{
    [TestFixture]
    public class ClosingStructuresCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validDataFilepath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            const string name = "<very nice name>";

            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                }
            };

            bool isValid = false;

            // Call
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
        public void Validate_CalculationInputWithoutHydraulicBoundaryLocationValidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = null
                }
            };

            bool isValid = false;

            // Call
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
        public void Validate_CalculationInputWithoutStructure_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    Structure = null
                }
            };

            bool isValid = false;

            // Call
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_InvalidVerticalWallCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation
            {
                Name = name
            };

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            bool isValid = false;
            
            // Call 
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(21, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stormDuration), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stormDuration), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", deviationWaveDirection), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", modelFactorSuperCriticalFlow), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", modelFactorSuperCriticalFlow), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", factorStormDurationOpenStructure), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", widthFlowApertures), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", widthFlowApertures), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", structureNormalOrientation), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", flowWidthAtBottomProtection), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowWidthAtBottomProtection), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", storageStructureArea), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", storageStructureArea), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", allowedLevelIncreaseStorage), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", allowedLevelIncreaseStorage), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", levelCrestStructureNotClosing), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", levelCrestStructureNotClosing), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", criticalOvertoppingDischarge), msgs[18]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", criticalOvertoppingDischarge), msgs[19]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[20]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_InvalidLowSillCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = ClosingStructureInflowModelType.LowSill
                }
            };
            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            bool isValid = false;

            // Call 
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(21, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stormDuration), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stormDuration), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevel), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevel), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", modelFactorSuperCriticalFlow), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", modelFactorSuperCriticalFlow), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", factorStormDurationOpenStructure), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", widthFlowApertures), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", widthFlowApertures), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", flowWidthAtBottomProtection), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowWidthAtBottomProtection), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", storageStructureArea), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", storageStructureArea), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", allowedLevelIncreaseStorage), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", allowedLevelIncreaseStorage), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", thresholdHeightOpenWeir), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", thresholdHeightOpenWeir), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", criticalOvertoppingDischarge), msgs[18]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", criticalOvertoppingDischarge), msgs[19]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[20]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_InvalidFloodedCulvertCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = ClosingStructureInflowModelType.FloodedCulvert
                }
            };
            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            bool isValid = false;

            // Call 
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(19, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stormDuration), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stormDuration), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevel), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevel), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", drainCoefficient), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", drainCoefficient), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", factorStormDurationOpenStructure), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", areaFlowApertures), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", areaFlowApertures), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", flowWidthAtBottomProtection), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowWidthAtBottomProtection), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", storageStructureArea), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", storageStructureArea), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", allowedLevelIncreaseStorage), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", allowedLevelIncreaseStorage), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", criticalOvertoppingDischarge), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", criticalOvertoppingDischarge), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[18]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = (ClosingStructureInflowModelType) 9001
                }
            };

            // Call
            bool isValid = false;
            TestDelegate call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            var exception = Assert.Throws<InvalidEnumArgumentException>(call);
            Assert.AreEqual("inputParameters", exception.ParamName);
            StringAssert.StartsWith("The value of argument 'inputParameters' (9001) is invalid for Enum type 'ClosingStructureInflowModelType'.",
                                    exception.Message);
            Assert.IsFalse(isValid);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation()
            {
                InputParameters =
                {
                    InflowModelType = (ClosingStructureInflowModelType) 100
                }
            };

            var service = new ClosingStructuresCalculationService();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;

                // Call
                TestDelegate call = () => service.Calculate(calculation,
                                                            assessmentSectionStub,
                                                            closingStructuresFailureMechanism,
                                                            testDataPath);

                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();

                // Assert
                Assert.AreEqual(0, calculationInputs.Length);
                const string expectedMessage = "The value of argument 'calculation' (100) is invalid for Enum type 'ClosingStructureInflowModelType'.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                        expectedMessage).ParamName;
                Assert.AreEqual("calculation", paramName);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Validate_UsesBreakWaterAndHasInvalidBreakWaterSettings_ReturnsFalse(
            [Values(ClosingStructureInflowModelType.VerticalWall, ClosingStructureInflowModelType.LowSill, ClosingStructureInflowModelType.FloodedCulvert)] ClosingStructureInflowModelType inflowModelType,
            [Values(double.NaN, double.NegativeInfinity, double.PositiveInfinity)] double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = inflowModelType,
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight)),
                    UseBreakWater = true
                }
            };

            bool isValid = false;

            // Call
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen geldige damhoogte ingevoerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Calculate_VariousVerticalWallCalculations_InputPropertiesCorrectlySentToCalculator(bool useForeshore, bool useBreakWater)
        {
            // Setup
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(closingStructuresFailureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation()
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

            FailureMechanismSection failureMechanismSection = closingStructuresFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;

                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    assessmentSectionStub,
                                                                    closingStructuresFailureMechanism,
                                                                    validDataFilepath);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralClosingStructuresInput generalInput = closingStructuresFailureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureVerticalWallCalculationInput(
                    1300001,
                    new HydraRingSection(1, failureMechanismSection.GetSectionLength(), input.StructureNormalOrientation),
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    useBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null,
                    generalInput.GravitationalAcceleration,
                    input.FactorStormDurationOpenStructure,
                    input.FailureProbabilityOpenStructure,
                    input.FailureProbabilityReparation,
                    input.IdenticalApertures,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation,
                    input.ProbabilityOpenStructureBeforeFlooding,
                    generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.ModelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    input.LevelCrestStructureNotClosing.Mean, input.LevelCrestStructureNotClosing.StandardDeviation,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.CoefficientOfVariation,
                    input.DeviationWaveDirection);

                StructuresClosureVerticalWallCalculationInput actualInput = (StructuresClosureVerticalWallCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Calculate_VariousLowSillCalculations_InputPropertiesCorrectlySentToCalculator(bool useForeshore, bool useBreakWater)
        {
            // Setup
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(closingStructuresFailureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = ClosingStructureInflowModelType.LowSill
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

            FailureMechanismSection failureMechanismSection = closingStructuresFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;

                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    assessmentSectionStub,
                                                                    closingStructuresFailureMechanism,
                                                                    validDataFilepath);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralClosingStructuresInput generalInput = closingStructuresFailureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureLowSillCalculationInput(
                    1300001,
                    new HydraRingSection(1, failureMechanismSection.GetSectionLength(), input.StructureNormalOrientation),
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    useBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null,
                    generalInput.GravitationalAcceleration,
                    input.FactorStormDurationOpenStructure,
                    input.FailureProbabilityOpenStructure,
                    input.FailureProbabilityReparation,
                    input.IdenticalApertures,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation,
                    input.ProbabilityOpenStructureBeforeFlooding,
                    input.ModelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    generalInput.ModelFactorSubCriticalFlow.Mean, generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                    input.ThresholdHeightOpenWeir.Mean, input.ThresholdHeightOpenWeir.StandardDeviation,
                    input.InsideWaterLevel.Mean, input.InsideWaterLevel.StandardDeviation,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.CoefficientOfVariation);

                StructuresClosureLowSillCalculationInput actualInput = (StructuresClosureLowSillCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Calculate_VariousFloodedCulvertCalculations_InputPropertiesCorrectlySentToCalculator(bool useForeshore, bool useBreakWater)
        {
            // Setup
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(closingStructuresFailureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = ClosingStructureInflowModelType.FloodedCulvert
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

            FailureMechanismSection failureMechanismSection = closingStructuresFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;

                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    assessmentSectionStub,
                                                                    closingStructuresFailureMechanism,
                                                                    validDataFilepath);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralClosingStructuresInput generalInput = closingStructuresFailureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureFloodedCulvertCalculationInput(
                    1300001,
                    new HydraRingSection(1, failureMechanismSection.GetSectionLength(), input.StructureNormalOrientation),
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    useBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null,
                    generalInput.GravitationalAcceleration,
                    input.FactorStormDurationOpenStructure,
                    input.FailureProbabilityOpenStructure,
                    input.FailureProbabilityReparation,
                    input.IdenticalApertures,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation,
                    input.ProbabilityOpenStructureBeforeFlooding,
                    input.DrainCoefficient.Mean, input.DrainCoefficient.StandardDeviation,
                    input.AreaFlowApertures.Mean, input.AreaFlowApertures.StandardDeviation,
                    input.InsideWaterLevel.Mean, input.InsideWaterLevel.StandardDeviation);

                StructuresClosureFloodedCulvertCalculationInput actualInput = (StructuresClosureFloodedCulvertCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Calculate_ValidCalculation_LogStartAndEndAndReturnOutput(
            [Values(ClosingStructureInflowModelType.VerticalWall, ClosingStructureInflowModelType.LowSill, ClosingStructureInflowModelType.FloodedCulvert)] ClosingStructureInflowModelType inflowModelType,
            [Values(CalculationType.NoForeshore, CalculationType.ForeshoreWithoutBreakWater, CalculationType.ForeshoreWithValidBreakWater)] CalculationType calculationType)
        {
            // Setup
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(closingStructuresFailureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = inflowModelType,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true
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
                Action call = () => new ClosingStructuresCalculationService().Calculate(calculation,
                                                                                        assessmentSectionStub,
                                                                                        closingStructuresFailureMechanism,
                                                                                        validDataFilepath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie:", msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(ClosingStructureInflowModelType.VerticalWall)]
        [TestCase(ClosingStructureInflowModelType.LowSill)]
        [TestCase(ClosingStructureInflowModelType.FloodedCulvert)]
        public void Calculate_InvalidCalculation_LogStartAndEndAndErrorMessageAndThrowsException(ClosingStructureInflowModelType inflowModelType)
        {
            // Setup
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(closingStructuresFailureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType
                }
            };

            var exception = false;

            // Call
            Action call = () =>
            {
                try
                {
                    new ClosingStructuresCalculationService().Calculate(calculation,
                                                                        assessmentSectionStub,
                                                                        closingStructuresFailureMechanism,
                                                                        testDataPath);
                }
                catch (HydraRingFileParserException)
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
                StringAssert.StartsWith(string.Format("De berekening voor kunstwerk sluiten '{0}' is niet gelukt.", calculation.Name), msgs[1]);
                StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
            });
            Assert.IsNull(calculation.Output);
            Assert.IsTrue(exception);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(closingStructuresFailureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;
                var service = new ClosingStructuresCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  assessmentSectionStub,
                                  closingStructuresFailureMechanism,
                                  testDataPath);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(calculator.IsCanceled);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;
                calculator.LastErrorContent = "An error occured";
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new ClosingStructuresCalculationService().Calculate(calculation,
                                                                            assessmentSectionStub,
                                                                            failureMechanism,
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
                    StringAssert.StartsWith(string.Format("De berekening voor kunstwerk sluiten '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", calculation.Name), msgs[1]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new ClosingStructuresCalculationService().Calculate(calculation,
                                                                            assessmentSectionStub,
                                                                            failureMechanism,
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
                    StringAssert.StartsWith(string.Format("De berekening voor kunstwerk sluiten '{0}' is niet gelukt. Er is geen foutrapport beschikbaar.", calculation.Name), msgs[1]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorContent = "An error occured";

                var exceptionThrown = false;
                var exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new ClosingStructuresCalculationService().Calculate(calculation,
                                                                            assessmentSectionStub,
                                                                            failureMechanism,
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
                    StringAssert.StartsWith(string.Format("De berekening voor kunstwerk sluiten '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.",
                                                          calculation.Name), msgs[1]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.AreEqual(calculator.LastErrorContent, exceptionMessage);
            }
        }

        /// <summary>
        /// Sets all input parameters of <see cref="ClosingStructuresInput"/> to invalid values.
        /// </summary>
        /// <param name="input">The input to be updated.</param>
        /// <param name="value">The invalid value to be set on all input properties.</param>
        /// <remarks>If <paramref name="value"/> cannot be set on an input property, that
        /// value is set to <see cref="RoundedDouble.NaN"/>.</remarks>
        private static void SetInvalidInputParameters(ClosingStructuresInput input, RoundedDouble value)
        {
            input.DeviationWaveDirection = RoundedDouble.NaN;
            input.DrainCoefficient.Mean = value;
            input.FactorStormDurationOpenStructure = value;
            input.InsideWaterLevel.Mean = value;
            input.LevelCrestStructureNotClosing.Mean = value;
            input.ModelFactorSuperCriticalFlow.Mean = value;
            input.StructureNormalOrientation = RoundedDouble.NaN;
            input.ThresholdHeightOpenWeir.Mean = value;
            input.WidthFlowApertures.Mean = value;

            if (double.IsNegativeInfinity(value))
            {
                input.AllowedLevelIncreaseStorage.Mean = RoundedDouble.NaN;
                input.AllowedLevelIncreaseStorage.StandardDeviation = RoundedDouble.NaN;
                input.AreaFlowApertures.Mean = RoundedDouble.NaN;
                input.AreaFlowApertures.StandardDeviation = RoundedDouble.NaN;
                input.CriticalOvertoppingDischarge.Mean = RoundedDouble.NaN;
                input.CriticalOvertoppingDischarge.CoefficientOfVariation = RoundedDouble.NaN;
                input.DrainCoefficient.StandardDeviation = RoundedDouble.NaN;
                input.FlowWidthAtBottomProtection.Mean = RoundedDouble.NaN;
                input.FlowWidthAtBottomProtection.StandardDeviation = RoundedDouble.NaN;
                input.InsideWaterLevel.StandardDeviation = RoundedDouble.NaN;
                input.LevelCrestStructureNotClosing.StandardDeviation = RoundedDouble.NaN;
                input.ModelFactorSuperCriticalFlow.StandardDeviation = RoundedDouble.NaN;
                input.StorageStructureArea.Mean = RoundedDouble.NaN;
                input.StorageStructureArea.CoefficientOfVariation = RoundedDouble.NaN;
                input.StormDuration.Mean = RoundedDouble.NaN;
                input.StormDuration.CoefficientOfVariation = RoundedDouble.NaN;
                input.ThresholdHeightOpenWeir.StandardDeviation = RoundedDouble.NaN;
                input.WidthFlowApertures.CoefficientOfVariation = RoundedDouble.NaN;
            }
            else
            {
                input.AllowedLevelIncreaseStorage.Mean = value;
                input.AllowedLevelIncreaseStorage.StandardDeviation = value;
                input.AreaFlowApertures.Mean = value;
                input.AreaFlowApertures.StandardDeviation = value;
                input.CriticalOvertoppingDischarge.Mean = value;
                input.CriticalOvertoppingDischarge.CoefficientOfVariation = value;
                input.DrainCoefficient.StandardDeviation = value;
                input.FlowWidthAtBottomProtection.Mean = value;
                input.FlowWidthAtBottomProtection.StandardDeviation = value;
                input.InsideWaterLevel.StandardDeviation = value;
                input.LevelCrestStructureNotClosing.StandardDeviation = value;
                input.ModelFactorSuperCriticalFlow.StandardDeviation = value;
                input.StorageStructureArea.Mean = value;
                input.StorageStructureArea.CoefficientOfVariation = value;
                input.StormDuration.Mean = value;
                input.StormDuration.CoefficientOfVariation = value;
                input.ThresholdHeightOpenWeir.StandardDeviation = value;
                input.WidthFlowApertures.CoefficientOfVariation = value;
            }
        }

        #region Parameter name mappings

        private const string insideWaterLevel = "binnenwaterstand";
        private const string stormDuration = "stormduur";
        private const string deviationWaveDirection = "afwijking golfrichting";
        private const string factorStormDurationOpenStructure = "factor voor stormduur hoogwater";
        private const string modelFactorSuperCriticalFlow = "modelfactor overloopdebiet volkomen overlaat";
        private const string drainCoefficient = "afvoercoëfficient";
        private const string structureNormalOrientation = "oriëntatie";
        private const string thresholdHeightOpenWeir = "drempelhoogte";
        private const string areaFlowApertures = "doorstroomoppervlak";
        private const string levelCrestStructureNotClosing = "kruinhoogte niet gesloten kering";
        private const string allowedLevelIncreaseStorage = "toegestane peilverhoging komberging";
        private const string storageStructureArea = "kombergend oppervlak";
        private const string flowWidthAtBottomProtection = "stroomvoerende breedte bodembescherming";
        private const string criticalOvertoppingDischarge = "kritiek instromend debiet";
        private const string widthFlowApertures = "breedte van doorstroomopening";

        #endregion
    }
}