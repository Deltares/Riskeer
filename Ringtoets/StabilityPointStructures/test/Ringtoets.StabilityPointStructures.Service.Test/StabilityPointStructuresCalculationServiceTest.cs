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
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Service.Test
{
    [TestFixture]
    public class StabilityPointStructuresCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validDataFilepath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            const string name = "<very nice name>";

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                }
            };

            bool isValid = false;

            // Call
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = null,
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            bool isValid = false;

            // Call
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
        public void Validate_CalculationWithoutStructuresValidHydraulicBoundaryDatabase_LogStartAndEndAndErrorMessage()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            const string name = "<a very nice name>";
            var calculation = new TestStabilityPointStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Linear,
                    Structure = null
                }
            };

            bool isValid = false;

            // Call
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
        [Combinatorial]
        public void Validate_UseBreakWaterWithInvalidBreakWaterHeight_LogStartAndEndAndErrorMessageAndThrowsException(
            [Values(StabilityPointStructureInflowModelType.FloodedCulvert, StabilityPointStructureInflowModelType.LowSill)] StabilityPointStructureInflowModelType inflowModelType,
            [Values(LoadSchematizationType.Quadratic, LoadSchematizationType.Linear)] LoadSchematizationType loadSchematizationType,
            [Values(double.NaN, double.PositiveInfinity, double.NegativeInfinity)] double breakWaterHeight)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            const string name = "<a very nice name>";
            var calculation = new TestStabilityPointStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = inflowModelType,
                    LoadSchematizationType = loadSchematizationType,
                    ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight)),
                    UseBreakWater = true,
                    UseForeshore = true
                }
            };

            bool isValid = false;

            // Call
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidLowSillLinearCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            bool isValid = false;

            // Call 
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            Assert.IsFalse(isValid);
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(43, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", volumicWeightWater), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stormDuration), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stormDuration), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevel), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevel), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevelFailureConstruction), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevelFailureConstruction), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", flowVelocityStructureClosable), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowVelocityStructureClosable), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", modelFactorSuperCriticalFlow), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", modelFactorSuperCriticalFlow), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", factorStormDurationOpenStructure), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", structureNormalOrientation), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", widthFlowApertures), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", widthFlowApertures), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", flowWidthAtBottomProtection), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowWidthAtBottomProtection), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", storageStructureArea), msgs[18]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", storageStructureArea), msgs[19]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", allowedLevelIncreaseStorage), msgs[20]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", allowedLevelIncreaseStorage), msgs[21]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", levelCrestStructure), msgs[22]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", levelCrestStructure), msgs[23]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", thresholdHeightOpenWeir), msgs[24]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", thresholdHeightOpenWeir), msgs[25]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", criticalOvertoppingDischarge), msgs[26]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", criticalOvertoppingDischarge), msgs[27]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", constructiveStrengthLinearLoadModel), msgs[28]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", constructiveStrengthLinearLoadModel), msgs[29]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", bankWidth), msgs[30]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", bankWidth), msgs[31]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", evaluationLevel), msgs[32]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", verticalDistance), msgs[33]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", failureCollisionEnergy), msgs[34]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", failureCollisionEnergy), msgs[35]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", shipMass), msgs[36]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", shipMass), msgs[37]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", shipVelocity), msgs[38]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", shipVelocity), msgs[39]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stabilityLinearLoadModel), msgs[40]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stabilityLinearLoadModel), msgs[41]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[42]);
            });
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidLowSillQuadraticCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            bool isValid = false;

            // Call 
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            Assert.IsFalse(isValid);
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(43, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", volumicWeightWater), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stormDuration), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stormDuration), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevel), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevel), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevelFailureConstruction), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevelFailureConstruction), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", flowVelocityStructureClosable), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowVelocityStructureClosable), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", modelFactorSuperCriticalFlow), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", modelFactorSuperCriticalFlow), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", factorStormDurationOpenStructure), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", structureNormalOrientation), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", widthFlowApertures), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", widthFlowApertures), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", flowWidthAtBottomProtection), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowWidthAtBottomProtection), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", storageStructureArea), msgs[18]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", storageStructureArea), msgs[19]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", allowedLevelIncreaseStorage), msgs[20]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", allowedLevelIncreaseStorage), msgs[21]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", levelCrestStructure), msgs[22]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", levelCrestStructure), msgs[23]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", thresholdHeightOpenWeir), msgs[24]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", thresholdHeightOpenWeir), msgs[25]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", criticalOvertoppingDischarge), msgs[26]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", criticalOvertoppingDischarge), msgs[27]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", constructiveStrengthQuadraticLoadModel), msgs[28]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", constructiveStrengthQuadraticLoadModel), msgs[29]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", bankWidth), msgs[30]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", bankWidth), msgs[31]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", evaluationLevel), msgs[32]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", verticalDistance), msgs[33]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", failureCollisionEnergy), msgs[34]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", failureCollisionEnergy), msgs[35]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", shipMass), msgs[36]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", shipMass), msgs[37]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", shipVelocity), msgs[38]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", shipVelocity), msgs[39]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stabilityQuadraticLoadModel), msgs[40]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stabilityQuadraticLoadModel), msgs[41]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[42]);
            });
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidFloodedCulvertLinearCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            bool isValid = false;

            // Call 
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            Assert.IsFalse(isValid);
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(43, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", volumicWeightWater), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stormDuration), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stormDuration), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevel), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevel), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevelFailureConstruction), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevelFailureConstruction), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", flowVelocityStructureClosable), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowVelocityStructureClosable), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", drainCoefficient), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", drainCoefficient), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", factorStormDurationOpenStructure), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", structureNormalOrientation), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", areaFlowApertures), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", areaFlowApertures), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", flowWidthAtBottomProtection), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowWidthAtBottomProtection), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", storageStructureArea), msgs[18]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", storageStructureArea), msgs[19]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", allowedLevelIncreaseStorage), msgs[20]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", allowedLevelIncreaseStorage), msgs[21]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", levelCrestStructure), msgs[22]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", levelCrestStructure), msgs[23]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", thresholdHeightOpenWeir), msgs[24]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", thresholdHeightOpenWeir), msgs[25]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", criticalOvertoppingDischarge), msgs[26]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", criticalOvertoppingDischarge), msgs[27]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", constructiveStrengthLinearLoadModel), msgs[28]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", constructiveStrengthLinearLoadModel), msgs[29]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", bankWidth), msgs[30]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", bankWidth), msgs[31]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", evaluationLevel), msgs[32]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", verticalDistance), msgs[33]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", failureCollisionEnergy), msgs[34]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", failureCollisionEnergy), msgs[35]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", shipMass), msgs[36]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", shipMass), msgs[37]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", shipVelocity), msgs[38]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", shipVelocity), msgs[39]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stabilityLinearLoadModel), msgs[40]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stabilityLinearLoadModel), msgs[41]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[42]);
            });
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidQuadraticCulvertLinearCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            bool isValid = false;

            // Call 
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            Assert.IsFalse(isValid);
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(43, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", volumicWeightWater), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stormDuration), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stormDuration), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevel), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevel), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", insideWaterLevelFailureConstruction), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", insideWaterLevelFailureConstruction), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", flowVelocityStructureClosable), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowVelocityStructureClosable), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", drainCoefficient), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", drainCoefficient), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", factorStormDurationOpenStructure), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", structureNormalOrientation), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", areaFlowApertures), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", areaFlowApertures), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", flowWidthAtBottomProtection), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", flowWidthAtBottomProtection), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", storageStructureArea), msgs[18]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", storageStructureArea), msgs[19]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", allowedLevelIncreaseStorage), msgs[20]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", allowedLevelIncreaseStorage), msgs[21]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", levelCrestStructure), msgs[22]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", levelCrestStructure), msgs[23]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", thresholdHeightOpenWeir), msgs[24]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", thresholdHeightOpenWeir), msgs[25]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", criticalOvertoppingDischarge), msgs[26]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", criticalOvertoppingDischarge), msgs[27]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", constructiveStrengthQuadraticLoadModel), msgs[28]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", constructiveStrengthQuadraticLoadModel), msgs[29]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", bankWidth), msgs[30]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", bankWidth), msgs[31]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", evaluationLevel), msgs[32]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", verticalDistance), msgs[33]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", failureCollisionEnergy), msgs[34]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", failureCollisionEnergy), msgs[35]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", shipMass), msgs[36]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", shipMass), msgs[37]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", shipVelocity), msgs[38]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", shipVelocity), msgs[39]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", stabilityQuadraticLoadModel), msgs[40]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", stabilityQuadraticLoadModel), msgs[41]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[42]);
            });
        }

        [Test]
        public void Validate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            const string name = "<a very nice name>";
            var calculation = new TestStabilityPointStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = (StabilityPointStructureInflowModelType) 100
                }
            };

            // Call
            TestDelegate call = () => StabilityPointStructuresCalculationService.Validate(calculation,
                                                                                          assessmentSectionStub);
            // Assert
            const string expectedMessage = "The value of argument 'inputParameters' (100) is invalid for Enum type 'StabilityPointStructureInflowModelType'.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                    expectedMessage).ParamName;
            Assert.AreEqual("inputParameters", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        public void Validate_InvalidLoadSchematizationType_ThrowsInvalidEnumArgumentException(StabilityPointStructureInflowModelType inflowModelType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            const string name = "<a very nice name>";
            var calculation = new TestStabilityPointStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = inflowModelType,
                    LoadSchematizationType = (LoadSchematizationType) 100
                }
            };

            // Call
            TestDelegate call = () => StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            const string expectedMessage = "The value of argument 'inputParameters' (100) is invalid for Enum type 'LoadSchematizationType'.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                    expectedMessage).ParamName;
            Assert.AreEqual("inputParameters", paramName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var stabilityPointStructuresFailureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            stabilityPointStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                InputParameters =
                {
                    InflowModelType = (StabilityPointStructureInflowModelType) 100
                }
            };

            var service = new StabilityPointStructuresCalculationService();

            // Call
            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                TestDelegate call = () => service.Calculate(calculation,
                                                            assessmentSectionStub,
                                                            stabilityPointStructuresFailureMechanism,
                                                            testDataPath);

                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();

                // Assert
                Assert.AreEqual(0, calculationInputs.Length);
                var exception = Assert.Throws<InvalidEnumArgumentException>(call);
                Assert.AreEqual("calculation", exception.ParamName);
                StringAssert.StartsWith("The value of argument 'calculation' (100) is invalid for Enum type 'StabilityPointStructureInflowModelType'.", exception.Message);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        public void Calculate_InvalidLoadSchematizationType_ThrowsInvalidEnumArgumentException(StabilityPointStructureInflowModelType inflowModelType)
        {
            // Setup
            var stabilityPointStructuresFailureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            stabilityPointStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType,
                    LoadSchematizationType = (LoadSchematizationType) 100
                }
            };

            var service = new StabilityPointStructuresCalculationService();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                TestDelegate call = () => service.Calculate(calculation,
                                                            assessmentSectionStub,
                                                            stabilityPointStructuresFailureMechanism,
                                                            testDataPath);

                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();

                // Assert
                Assert.AreEqual(0, calculationInputs.Length);
                var exception = Assert.Throws<InvalidEnumArgumentException>(call);
                Assert.AreEqual("calculation", exception.ParamName);
                StringAssert.StartsWith("The value of argument 'calculation' (100) is invalid for Enum type 'LoadSchematizationType'.", exception.Message);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Calculate_VariousLowSillLinearCalculations_InputPropertiesCorrectlySentToCalculator(bool useForeshore, bool useBreakWater)
        {
            // Setup
            var stabilityPointStructuresFailureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(stabilityPointStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            stabilityPointStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
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

            FailureMechanismSection failureMechanismSection = stabilityPointStructuresFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           stabilityPointStructuresFailureMechanism,
                                                                           validDataFilepath);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralStabilityPointStructuresInput generalInput = stabilityPointStructuresFailureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointLowSillLinearCalculationInput(
                    1300001,
                    new HydraRingSection(1, failureMechanismSection.GetSectionLength(), input.StructureNormalOrientation),
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    useBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null,
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
                    generalInput.ModelFactorSubCriticalFlow.Mean,
                    generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                    input.ThresholdHeightOpenWeir.Mean,
                    input.ThresholdHeightOpenWeir.StandardDeviation,
                    input.InsideWaterLevelFailureConstruction.Mean,
                    input.InsideWaterLevelFailureConstruction.StandardDeviation,
                    input.FailureProbabilityRepairClosure,
                    input.FailureCollisionEnergy.Mean,
                    input.FailureCollisionEnergy.CoefficientOfVariation,
                    generalInput.ModelFactorCollisionLoad.Mean,
                    generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                    input.ShipMass.Mean,
                    input.ShipMass.CoefficientOfVariation,
                    input.ShipVelocity.Mean,
                    input.ShipVelocity.CoefficientOfVariation,
                    input.LevellingCount,
                    input.ProbabilityCollisionSecondaryStructure,
                    input.FlowVelocityStructureClosable.Mean,
                    input.FlowVelocityStructureClosable.StandardDeviation,
                    input.InsideWaterLevel.Mean,
                    input.InsideWaterLevel.StandardDeviation,
                    input.AllowedLevelIncreaseStorage.Mean,
                    input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean,
                    generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean,
                    input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean,
                    input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean,
                    input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean,
                    input.StormDuration.CoefficientOfVariation,
                    input.BankWidth.Mean,
                    input.BankWidth.StandardDeviation,
                    input.EvaluationLevel,
                    generalInput.ModelFactorLoadEffect.Mean,
                    generalInput.ModelFactorLoadEffect.StandardDeviation,
                    generalInput.WaveRatioMaxHN,
                    generalInput.WaveRatioMaxHStandardDeviation,
                    input.VerticalDistance,
                    generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                    generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                    input.ModelFactorSuperCriticalFlow.Mean,
                    input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    input.ConstructiveStrengthLinearLoadModel.Mean,
                    input.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation,
                    input.StabilityLinearLoadModel.Mean,
                    input.StabilityLinearLoadModel.CoefficientOfVariation,
                    input.WidthFlowApertures.Mean,
                    input.WidthFlowApertures.CoefficientOfVariation);

                StructuresStabilityPointLowSillLinearCalculationInput actualInput = (StructuresStabilityPointLowSillLinearCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Calculate_VariousLowSillQuadraticCalculations_InputPropertiesCorrectlySentToCalculator(bool useForeshore, bool useBreakWater)
        {
            // Setup
            var stabilityPointStructuresFailureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(stabilityPointStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            stabilityPointStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
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

            FailureMechanismSection failureMechanismSection = stabilityPointStructuresFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           stabilityPointStructuresFailureMechanism,
                                                                           validDataFilepath);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralStabilityPointStructuresInput generalInput = stabilityPointStructuresFailureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointLowSillQuadraticCalculationInput(
                    1300001,
                    new HydraRingSection(1, failureMechanismSection.GetSectionLength(), input.StructureNormalOrientation),
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    useBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null,
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
                    generalInput.ModelFactorSubCriticalFlow.Mean,
                    generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                    input.ThresholdHeightOpenWeir.Mean,
                    input.ThresholdHeightOpenWeir.StandardDeviation,
                    input.InsideWaterLevelFailureConstruction.Mean,
                    input.InsideWaterLevelFailureConstruction.StandardDeviation,
                    input.FailureProbabilityRepairClosure,
                    input.FailureCollisionEnergy.Mean,
                    input.FailureCollisionEnergy.CoefficientOfVariation,
                    generalInput.ModelFactorCollisionLoad.Mean,
                    generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                    input.ShipMass.Mean,
                    input.ShipMass.CoefficientOfVariation,
                    input.ShipVelocity.Mean,
                    input.ShipVelocity.CoefficientOfVariation,
                    input.LevellingCount,
                    input.ProbabilityCollisionSecondaryStructure,
                    input.FlowVelocityStructureClosable.Mean,
                    input.FlowVelocityStructureClosable.StandardDeviation,
                    input.InsideWaterLevel.Mean,
                    input.InsideWaterLevel.StandardDeviation,
                    input.AllowedLevelIncreaseStorage.Mean,
                    input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean,
                    generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean,
                    input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean,
                    input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean,
                    input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean,
                    input.StormDuration.CoefficientOfVariation,
                    input.BankWidth.Mean,
                    input.BankWidth.StandardDeviation,
                    input.EvaluationLevel,
                    generalInput.ModelFactorLoadEffect.Mean,
                    generalInput.ModelFactorLoadEffect.StandardDeviation,
                    generalInput.WaveRatioMaxHN,
                    generalInput.WaveRatioMaxHStandardDeviation,
                    input.VerticalDistance,
                    generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                    generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                    input.ModelFactorSuperCriticalFlow.Mean,
                    input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    input.ConstructiveStrengthQuadraticLoadModel.Mean,
                    input.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation,
                    input.StabilityQuadraticLoadModel.Mean,
                    input.StabilityQuadraticLoadModel.CoefficientOfVariation,
                    input.WidthFlowApertures.Mean,
                    input.WidthFlowApertures.CoefficientOfVariation);

                StructuresStabilityPointLowSillQuadraticCalculationInput actualInput = (StructuresStabilityPointLowSillQuadraticCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Calculate_VariousFloodedCulvertLinearCalculations_InputPropertiesCorrectlySentToCalculator(bool useForeshore, bool useBreakWater)
        {
            // Setup
            var stabilityPointStructuresFailureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(stabilityPointStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            stabilityPointStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Linear
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

            FailureMechanismSection failureMechanismSection = stabilityPointStructuresFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           stabilityPointStructuresFailureMechanism,
                                                                           validDataFilepath);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralStabilityPointStructuresInput generalInput = stabilityPointStructuresFailureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointFloodedCulvertLinearCalculationInput(
                    1300001,
                    new HydraRingSection(1, failureMechanismSection.GetSectionLength(), input.StructureNormalOrientation),
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    useBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null,
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
                    generalInput.ModelFactorSubCriticalFlow.Mean,
                    generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                    input.ThresholdHeightOpenWeir.Mean,
                    input.ThresholdHeightOpenWeir.StandardDeviation,
                    input.InsideWaterLevelFailureConstruction.Mean,
                    input.InsideWaterLevelFailureConstruction.StandardDeviation,
                    input.FailureProbabilityRepairClosure,
                    input.FailureCollisionEnergy.Mean,
                    input.FailureCollisionEnergy.CoefficientOfVariation,
                    generalInput.ModelFactorCollisionLoad.Mean,
                    generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                    input.ShipMass.Mean,
                    input.ShipMass.CoefficientOfVariation,
                    input.ShipVelocity.Mean,
                    input.ShipVelocity.CoefficientOfVariation,
                    input.LevellingCount,
                    input.ProbabilityCollisionSecondaryStructure,
                    input.FlowVelocityStructureClosable.Mean,
                    input.FlowVelocityStructureClosable.StandardDeviation,
                    input.InsideWaterLevel.Mean,
                    input.InsideWaterLevel.StandardDeviation,
                    input.AllowedLevelIncreaseStorage.Mean,
                    input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean,
                    generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean,
                    input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean,
                    input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean,
                    input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean,
                    input.StormDuration.CoefficientOfVariation,
                    input.BankWidth.Mean,
                    input.BankWidth.StandardDeviation,
                    input.EvaluationLevel,
                    generalInput.ModelFactorLoadEffect.Mean,
                    generalInput.ModelFactorLoadEffect.StandardDeviation,
                    generalInput.WaveRatioMaxHN,
                    generalInput.WaveRatioMaxHStandardDeviation,
                    input.VerticalDistance,
                    generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                    generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                    input.DrainCoefficient.Mean,
                    input.DrainCoefficient.StandardDeviation,
                    input.AreaFlowApertures.Mean,
                    input.AreaFlowApertures.StandardDeviation,
                    input.ConstructiveStrengthLinearLoadModel.Mean,
                    input.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation,
                    input.StabilityLinearLoadModel.Mean,
                    input.StabilityLinearLoadModel.CoefficientOfVariation);

                StructuresStabilityPointFloodedCulvertLinearCalculationInput actualInput = (StructuresStabilityPointFloodedCulvertLinearCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Calculate_VariousFloodedCulvertQuadraticCalculations_InputPropertiesCorrectlySentToCalculator(bool useForeshore, bool useBreakWater)
        {
            // Setup
            var stabilityPointStructuresFailureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(stabilityPointStructuresFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            stabilityPointStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
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

            FailureMechanismSection failureMechanismSection = stabilityPointStructuresFailureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           stabilityPointStructuresFailureMechanism,
                                                                           validDataFilepath);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralStabilityPointStructuresInput generalInput = stabilityPointStructuresFailureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(
                    1300001,
                    new HydraRingSection(1, failureMechanismSection.GetSectionLength(), input.StructureNormalOrientation),
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    useBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null,
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
                    generalInput.ModelFactorSubCriticalFlow.Mean,
                    generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                    input.ThresholdHeightOpenWeir.Mean,
                    input.ThresholdHeightOpenWeir.StandardDeviation,
                    input.InsideWaterLevelFailureConstruction.Mean,
                    input.InsideWaterLevelFailureConstruction.StandardDeviation,
                    input.FailureProbabilityRepairClosure,
                    input.FailureCollisionEnergy.Mean,
                    input.FailureCollisionEnergy.CoefficientOfVariation,
                    generalInput.ModelFactorCollisionLoad.Mean,
                    generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                    input.ShipMass.Mean,
                    input.ShipMass.CoefficientOfVariation,
                    input.ShipVelocity.Mean,
                    input.ShipVelocity.CoefficientOfVariation,
                    input.LevellingCount,
                    input.ProbabilityCollisionSecondaryStructure,
                    input.FlowVelocityStructureClosable.Mean,
                    input.FlowVelocityStructureClosable.StandardDeviation,
                    input.InsideWaterLevel.Mean,
                    input.InsideWaterLevel.StandardDeviation,
                    input.AllowedLevelIncreaseStorage.Mean,
                    input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean,
                    generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean,
                    input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean,
                    input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean,
                    input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean,
                    input.StormDuration.CoefficientOfVariation,
                    input.BankWidth.Mean,
                    input.BankWidth.StandardDeviation,
                    input.EvaluationLevel,
                    generalInput.ModelFactorLoadEffect.Mean,
                    generalInput.ModelFactorLoadEffect.StandardDeviation,
                    generalInput.WaveRatioMaxHN,
                    generalInput.WaveRatioMaxHStandardDeviation,
                    input.VerticalDistance,
                    generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                    generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                    input.DrainCoefficient.Mean,
                    input.DrainCoefficient.StandardDeviation,
                    input.AreaFlowApertures.Mean,
                    input.AreaFlowApertures.StandardDeviation,
                    input.ConstructiveStrengthQuadraticLoadModel.Mean,
                    input.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation,
                    input.StabilityQuadraticLoadModel.Mean,
                    input.StabilityQuadraticLoadModel.CoefficientOfVariation);

                StructuresStabilityPointFloodedCulvertQuadraticCalculationInput actualInput = (StructuresStabilityPointFloodedCulvertQuadraticCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Calculate_ValidCalculation_LogStartAndEndAndReturnOutput(
            [Values(StabilityPointStructureInflowModelType.FloodedCulvert, StabilityPointStructureInflowModelType.LowSill)] StabilityPointStructureInflowModelType inflowModelType,
            [Values(LoadSchematizationType.Quadratic, LoadSchematizationType.Linear)] LoadSchematizationType loadSchematizationType,
            [Values(CalculationType.NoForeshore, CalculationType.ForeshoreWithValidBreakWater, CalculationType.ForeshoreWithoutBreakWater)] CalculationType calculationType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = inflowModelType,
                    LoadSchematizationType = loadSchematizationType,
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
                Action call = () => new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                                               assessmentSectionStub,
                                                                                               failureMechanism,
                                                                                               validDataFilepath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie:", msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Calculate_InvalidCalculation_LogStartAndEndAndErrorMessageAndThrowsException(
            [Values(StabilityPointStructureInflowModelType.FloodedCulvert, StabilityPointStructureInflowModelType.LowSill)] StabilityPointStructureInflowModelType inflowModelType,
            [Values(LoadSchematizationType.Quadratic, LoadSchematizationType.Linear)] LoadSchematizationType loadSchematizationType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType,
                    LoadSchematizationType = loadSchematizationType
                }
            };

            var exception = false;

            // Call
            Action call = () =>
            {
                try
                {
                    new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                               assessmentSectionStub,
                                                                               failureMechanism,
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
                StringAssert.StartsWith(string.Format("De berekening voor kunstwerk puntconstructies '{0}' is niet gelukt.", calculation.Name), msgs[1]);
                StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;
                var service = new StabilityPointStructuresCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  assessmentSectionStub,
                                  failureMechanism,
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;
                calculator.LastErrorContent = "An error occured";
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new StabilityPointStructuresCalculationService().Calculate(calculation,
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
                    StringAssert.StartsWith(string.Format("De berekening voor kunstwerk puntconstructies '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", calculation.Name), msgs[1]);
                    StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new StabilityPointStructuresCalculationService().Calculate(calculation,
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
                    StringAssert.StartsWith(string.Format("De berekening voor kunstwerk puntconstructies '{0}' is niet gelukt. Er is geen foutrapport beschikbaar.", calculation.Name), msgs[1]);
                    StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorContent = "An error occured";

                var exceptionThrown = false;
                var exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new StabilityPointStructuresCalculationService().Calculate(calculation,
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
                    StringAssert.StartsWith(string.Format("De berekening voor kunstwerk puntconstructies '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.",
                                                          calculation.Name), msgs[1]);
                    StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie:", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.AreEqual(calculator.LastErrorContent, exceptionMessage);
            }
        }

        /// <summary>
        /// Sets all input parameters of <see cref="StabilityPointStructuresInput"/> to invalid values.
        /// </summary>
        /// <param name="input">The input to be updated.</param>
        /// <param name="value">The invalid value to be set on all input properties.</param>
        /// <remarks>If <paramref name="value"/> cannot be set on an input property, that
        /// value is set to <see cref="RoundedDouble.NaN"/>.</remarks>
        private static void SetInvalidInputParameters(StabilityPointStructuresInput input, RoundedDouble value)
        {
            input.FactorStormDurationOpenStructure = value;
            input.StructureNormalOrientation = RoundedDouble.NaN;
            input.EvaluationLevel = value;
            input.VerticalDistance = value;
            input.VolumicWeightWater = value;

            input.InsideWaterLevelFailureConstruction.Mean = value;
            input.InsideWaterLevel.Mean = value;
            input.ModelFactorSuperCriticalFlow.Mean = value;
            input.FlowVelocityStructureClosable.Mean = value;
            input.DrainCoefficient.Mean = value;
            input.LevelCrestStructure.Mean = value;
            input.ThresholdHeightOpenWeir.Mean = value;
            input.ShipMass.Mean = value;
            input.ShipVelocity.Mean = value;
            input.WidthFlowApertures.Mean = value;
            input.BankWidth.Mean = value;

            if (double.IsNegativeInfinity(value))
            {
                input.InsideWaterLevelFailureConstruction.StandardDeviation = RoundedDouble.NaN;
                input.InsideWaterLevel.StandardDeviation = RoundedDouble.NaN;
                input.StormDuration.CoefficientOfVariation = RoundedDouble.NaN;
                input.ModelFactorSuperCriticalFlow.StandardDeviation = RoundedDouble.NaN;
                input.FlowVelocityStructureClosable.StandardDeviation = RoundedDouble.NaN;
                input.DrainCoefficient.StandardDeviation = RoundedDouble.NaN;
                input.LevelCrestStructure.StandardDeviation = RoundedDouble.NaN;
                input.ThresholdHeightOpenWeir.StandardDeviation = RoundedDouble.NaN;
                input.AreaFlowApertures.StandardDeviation = RoundedDouble.NaN;
                input.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation = RoundedDouble.NaN;
                input.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation = RoundedDouble.NaN;
                input.StabilityLinearLoadModel.CoefficientOfVariation = RoundedDouble.NaN;
                input.StabilityQuadraticLoadModel.CoefficientOfVariation = RoundedDouble.NaN;
                input.FailureCollisionEnergy.CoefficientOfVariation = RoundedDouble.NaN;
                input.ShipMass.CoefficientOfVariation = RoundedDouble.NaN;
                input.ShipVelocity.CoefficientOfVariation = RoundedDouble.NaN;
                input.AllowedLevelIncreaseStorage.StandardDeviation = RoundedDouble.NaN;
                input.StorageStructureArea.CoefficientOfVariation = RoundedDouble.NaN;
                input.FlowWidthAtBottomProtection.StandardDeviation = RoundedDouble.NaN;
                input.CriticalOvertoppingDischarge.CoefficientOfVariation = RoundedDouble.NaN;
                input.WidthFlowApertures.CoefficientOfVariation = RoundedDouble.NaN;
                input.BankWidth.StandardDeviation = RoundedDouble.NaN;

                input.StormDuration.Mean = RoundedDouble.NaN;
                input.ModelFactorSuperCriticalFlow.Mean = RoundedDouble.NaN;
                input.FlowVelocityStructureClosable.Mean = RoundedDouble.NaN;
                input.DrainCoefficient.Mean = RoundedDouble.NaN;
                input.LevelCrestStructure.Mean = RoundedDouble.NaN;
                input.ThresholdHeightOpenWeir.Mean = RoundedDouble.NaN;
                input.AreaFlowApertures.Mean = RoundedDouble.NaN;
                input.ConstructiveStrengthLinearLoadModel.Mean = RoundedDouble.NaN;
                input.ConstructiveStrengthQuadraticLoadModel.Mean = RoundedDouble.NaN;
                input.StabilityLinearLoadModel.Mean = RoundedDouble.NaN;
                input.StabilityQuadraticLoadModel.Mean = RoundedDouble.NaN;
                input.FailureCollisionEnergy.Mean = RoundedDouble.NaN;
                input.ShipMass.Mean = RoundedDouble.NaN;
                input.ShipVelocity.Mean = RoundedDouble.NaN;
                input.AllowedLevelIncreaseStorage.Mean = RoundedDouble.NaN;
                input.StorageStructureArea.Mean = RoundedDouble.NaN;
                input.FlowWidthAtBottomProtection.Mean = RoundedDouble.NaN;
                input.CriticalOvertoppingDischarge.Mean = RoundedDouble.NaN;
            }
            else
            {
                input.InsideWaterLevelFailureConstruction.StandardDeviation = value;
                input.InsideWaterLevel.StandardDeviation = value;
                input.StormDuration.CoefficientOfVariation = value;
                input.ModelFactorSuperCriticalFlow.StandardDeviation = value;
                input.FlowVelocityStructureClosable.StandardDeviation = value;
                input.DrainCoefficient.StandardDeviation = value;
                input.LevelCrestStructure.StandardDeviation = value;
                input.ThresholdHeightOpenWeir.StandardDeviation = value;
                input.AreaFlowApertures.StandardDeviation = value;
                input.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation = value;
                input.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation = value;
                input.StabilityLinearLoadModel.CoefficientOfVariation = value;
                input.StabilityQuadraticLoadModel.CoefficientOfVariation = value;
                input.FailureCollisionEnergy.CoefficientOfVariation = value;
                input.ShipMass.CoefficientOfVariation = value;
                input.ShipVelocity.CoefficientOfVariation = value;
                input.AllowedLevelIncreaseStorage.StandardDeviation = value;
                input.StorageStructureArea.CoefficientOfVariation = value;
                input.FlowWidthAtBottomProtection.StandardDeviation = value;
                input.CriticalOvertoppingDischarge.CoefficientOfVariation = value;
                input.WidthFlowApertures.CoefficientOfVariation = value;
                input.BankWidth.StandardDeviation = value;

                input.StormDuration.Mean = value;
                input.ModelFactorSuperCriticalFlow.Mean = value;
                input.FlowVelocityStructureClosable.Mean = value;
                input.DrainCoefficient.Mean = value;
                input.LevelCrestStructure.Mean = value;
                input.ThresholdHeightOpenWeir.Mean = value;
                input.AreaFlowApertures.Mean = value;
                input.ConstructiveStrengthLinearLoadModel.Mean = value;
                input.ConstructiveStrengthQuadraticLoadModel.Mean = value;
                input.StabilityLinearLoadModel.Mean = value;
                input.StabilityQuadraticLoadModel.Mean = value;
                input.FailureCollisionEnergy.Mean = value;
                input.ShipMass.Mean = value;
                input.ShipVelocity.Mean = value;
                input.AllowedLevelIncreaseStorage.Mean = value;
                input.StorageStructureArea.Mean = value;
                input.FlowWidthAtBottomProtection.Mean = value;
                input.CriticalOvertoppingDischarge.Mean = value;
            }
        }

        #region Parametername mappings

        private const string volumicWeightWater = "volumiek gewicht van water";
        private const string insideWaterLevelFailureConstruction = "binnenwaterstand bij constructief falen";
        private const string insideWaterLevel = "binnenwaterstand";
        private const string stormDuration = "stormduur";
        private const string factorStormDurationOpenStructure = "factor voor stormduur hoogwater";
        private const string modelFactorSuperCriticalFlow = "modelfactor overloopdebiet volkomen overlaat";
        private const string flowVelocityStructureClosable = "kritieke stroomsnelheid sluiting eerste keermiddel";
        private const string drainCoefficient = "afvoercoëfficient";
        private const string structureNormalOrientation = "oriëntatie";
        private const string levelCrestStructure = "kerende hoogte";
        private const string thresholdHeightOpenWeir = "drempelhoogte";
        private const string areaFlowApertures = "doorstroomoppervlak";
        private const string constructiveStrengthLinearLoadModel = "lineaire belastingschematisering constructieve sterkte";
        private const string constructiveStrengthQuadraticLoadModel = "kwadratische belastingschematisering constructieve sterkte";
        private const string stabilityLinearLoadModel = "lineaire belastingschematisering stabiliteit";
        private const string stabilityQuadraticLoadModel = "kwadratische belastingschematisering stabiliteit";
        private const string failureCollisionEnergy = "bezwijkwaarde aanvaarenergie";
        private const string shipMass = "massa van het schip";
        private const string shipVelocity = "aanvaarsnelheid";
        private const string allowedLevelIncreaseStorage = "toegestane peilverhoging komberging";
        private const string storageStructureArea = "kombergend oppervlak";
        private const string flowWidthAtBottomProtection = "stroomvoerende breedte bodembescherming";
        private const string criticalOvertoppingDischarge = "kritiek instromend debiet";
        private const string widthFlowApertures = "breedte van doorstroomopening";
        private const string bankWidth = "bermbreedte";
        private const string evaluationLevel = "analysehoogte";
        private const string verticalDistance = "afstand onderkant wand en teen van de dijk/berm";

        #endregion
    }
}