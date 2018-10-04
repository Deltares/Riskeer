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
using System.ComponentModel;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.Structures;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Service.Test
{
    [TestFixture]
    public class StabilityPointStructuresCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var service = new StabilityPointStructuresCalculationService();

            // Assert
            Assert.IsInstanceOf<StructuresCalculationServiceBase<StabilityPointStructuresValidationRulesRegistry,
                StabilityPointStructuresInput,
                StabilityPointStructure,
                GeneralStabilityPointStructuresInput,
                StructuresStabilityPointCalculationInput>>(service);
        }

        [Test]
        [Combinatorial]
        public void Validate_UseBreakWaterInvalidBreakWaterHeight_LogErrorAndReturnFalse(
            [Values(StabilityPointStructureInflowModelType.FloodedCulvert, StabilityPointStructureInflowModelType.LowSill)]
            StabilityPointStructureInflowModelType inflowModelType,
            [Values(LoadSchematizationType.Quadratic, LoadSchematizationType.Linear)]
            LoadSchematizationType loadSchematizationType,
            [Values(double.NaN, double.PositiveInfinity, double.NegativeInfinity)]
            double breakWaterHeight)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository, validFilePath);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType,
                    LoadSchematizationType = loadSchematizationType,
                    ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight)),
                    UseBreakWater = true,
                    UseForeshore = true
                }
            };

            var isValid = false;

            // Call
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSection);

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
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidLowSillLinearCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            var isValid = false;

            // Call 
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            Assert.IsFalse(isValid);
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(41, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"De waarde voor '{volumicWeightWaterParameterName}' moet een concreet getal zijn.", msgs[1]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stormDurationParameterName}' moet een positief getal zijn.", msgs[2]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stormDurationParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[3]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevelParameterName}' moet een concreet getal zijn.", msgs[4]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[5]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevelFailureConstructionParameterName}' moet een concreet getal zijn.", msgs[6]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevelFailureConstructionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowVelocityStructureClosableParameterName}' moet een concreet getal zijn.", msgs[8]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{flowVelocityStructureClosableParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                Assert.AreEqual($"De waarde voor '{factorStormDurationOpenStructureParameterName}' moet een concreet getal zijn.", msgs[10]);
                Assert.AreEqual($"De waarde voor '{structureNormalOrientationParameterName}' moet een concreet getal zijn.", msgs[11]);
                Assert.AreEqual($"De verwachtingswaarde voor '{widthFlowAperturesParameterName}' moet een concreet getal zijn.", msgs[12]);
                Assert.AreEqual($"De standaardafwijking voor '{widthFlowAperturesParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[13]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowWidthAtBottomProtectionParameterName}' moet een positief getal zijn.", msgs[14]);
                Assert.AreEqual($"De standaardafwijking voor '{flowWidthAtBottomProtectionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                Assert.AreEqual($"De verwachtingswaarde voor '{storageStructureAreaParameterName}' moet een positief getal zijn.", msgs[16]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{storageStructureAreaParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                Assert.AreEqual($"De verwachtingswaarde voor '{allowedLevelIncreaseStorageParameterName}' moet een positief getal zijn.", msgs[18]);
                Assert.AreEqual($"De standaardafwijking voor '{allowedLevelIncreaseStorageParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                Assert.AreEqual($"De verwachtingswaarde voor '{levelCrestStructureParameterName}' moet een concreet getal zijn.", msgs[20]);
                Assert.AreEqual($"De standaardafwijking voor '{levelCrestStructureParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[21]);
                Assert.AreEqual($"De verwachtingswaarde voor '{thresholdHeightOpenWeirParameterName}' moet een concreet getal zijn.", msgs[22]);
                Assert.AreEqual($"De standaardafwijking voor '{thresholdHeightOpenWeirParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[23]);
                Assert.AreEqual($"De verwachtingswaarde voor '{criticalOvertoppingDischargeParameterName}' moet een positief getal zijn.", msgs[24]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{criticalOvertoppingDischargeParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[25]);
                Assert.AreEqual($"De verwachtingswaarde voor '{constructiveStrengthLinearLoadModelParameterName}' moet een positief getal zijn.", msgs[26]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{constructiveStrengthLinearLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[27]);
                Assert.AreEqual($"De verwachtingswaarde voor '{bankWidthParameterName}' moet een concreet getal zijn.", msgs[28]);
                Assert.AreEqual($"De standaardafwijking voor '{bankWidthParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[29]);
                Assert.AreEqual($"De waarde voor '{evaluationLevelParameterName}' moet een concreet getal zijn.", msgs[30]);
                Assert.AreEqual($"De waarde voor '{verticalDistanceParameterName}' moet een concreet getal zijn.", msgs[31]);
                Assert.AreEqual($"De verwachtingswaarde voor '{failureCollisionEnergyParameterName}' moet een positief getal zijn.", msgs[32]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{failureCollisionEnergyParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[33]);
                Assert.AreEqual($"De verwachtingswaarde voor '{shipMassParameterName}' moet een concreet getal zijn.", msgs[34]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{shipMassParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[35]);
                Assert.AreEqual($"De verwachtingswaarde voor '{shipVelocityParameterName}' moet een concreet getal zijn.", msgs[36]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{shipVelocityParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[37]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stabilityLinearLoadModelParameterName}' moet een positief getal zijn.", msgs[38]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stabilityLinearLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[39]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[40]);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            var isValid = false;

            // Call 
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            Assert.IsFalse(isValid);
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(41, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"De waarde voor '{volumicWeightWaterParameterName}' moet een concreet getal zijn.", msgs[1]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stormDurationParameterName}' moet een positief getal zijn.", msgs[2]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stormDurationParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[3]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevelParameterName}' moet een concreet getal zijn.", msgs[4]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[5]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevelFailureConstructionParameterName}' moet een concreet getal zijn.", msgs[6]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevelFailureConstructionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowVelocityStructureClosableParameterName}' moet een concreet getal zijn.", msgs[8]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{flowVelocityStructureClosableParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                Assert.AreEqual($"De waarde voor '{factorStormDurationOpenStructureParameterName}' moet een concreet getal zijn.", msgs[10]);
                Assert.AreEqual($"De waarde voor '{structureNormalOrientationParameterName}' moet een concreet getal zijn.", msgs[11]);
                Assert.AreEqual($"De verwachtingswaarde voor '{widthFlowAperturesParameterName}' moet een concreet getal zijn.", msgs[12]);
                Assert.AreEqual($"De standaardafwijking voor '{widthFlowAperturesParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[13]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowWidthAtBottomProtectionParameterName}' moet een positief getal zijn.", msgs[14]);
                Assert.AreEqual($"De standaardafwijking voor '{flowWidthAtBottomProtectionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                Assert.AreEqual($"De verwachtingswaarde voor '{storageStructureAreaParameterName}' moet een positief getal zijn.", msgs[16]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{storageStructureAreaParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                Assert.AreEqual($"De verwachtingswaarde voor '{allowedLevelIncreaseStorageParameterName}' moet een positief getal zijn.", msgs[18]);
                Assert.AreEqual($"De standaardafwijking voor '{allowedLevelIncreaseStorageParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                Assert.AreEqual($"De verwachtingswaarde voor '{levelCrestStructureParameterName}' moet een concreet getal zijn.", msgs[20]);
                Assert.AreEqual($"De standaardafwijking voor '{levelCrestStructureParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[21]);
                Assert.AreEqual($"De verwachtingswaarde voor '{thresholdHeightOpenWeirParameterName}' moet een concreet getal zijn.", msgs[22]);
                Assert.AreEqual($"De standaardafwijking voor '{thresholdHeightOpenWeirParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[23]);
                Assert.AreEqual($"De verwachtingswaarde voor '{criticalOvertoppingDischargeParameterName}' moet een positief getal zijn.", msgs[24]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{criticalOvertoppingDischargeParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[25]);
                Assert.AreEqual($"De verwachtingswaarde voor '{constructiveStrengthQuadraticLoadModelParameterName}' moet een positief getal zijn.", msgs[26]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{constructiveStrengthQuadraticLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[27]);
                Assert.AreEqual($"De verwachtingswaarde voor '{bankWidthParameterName}' moet een concreet getal zijn.", msgs[28]);
                Assert.AreEqual($"De standaardafwijking voor '{bankWidthParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[29]);
                Assert.AreEqual($"De waarde voor '{evaluationLevelParameterName}' moet een concreet getal zijn.", msgs[30]);
                Assert.AreEqual($"De waarde voor '{verticalDistanceParameterName}' moet een concreet getal zijn.", msgs[31]);
                Assert.AreEqual($"De verwachtingswaarde voor '{failureCollisionEnergyParameterName}' moet een positief getal zijn.", msgs[32]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{failureCollisionEnergyParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[33]);
                Assert.AreEqual($"De verwachtingswaarde voor '{shipMassParameterName}' moet een concreet getal zijn.", msgs[34]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{shipMassParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[35]);
                Assert.AreEqual($"De verwachtingswaarde voor '{shipVelocityParameterName}' moet een concreet getal zijn.", msgs[36]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{shipVelocityParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[37]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stabilityQuadraticLoadModelParameterName}' moet een positief getal zijn.", msgs[38]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stabilityQuadraticLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[39]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[40]);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            var isValid = false;

            // Call 
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            Assert.IsFalse(isValid);
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(43, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"De waarde voor '{volumicWeightWaterParameterName}' moet een concreet getal zijn.", msgs[1]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stormDurationParameterName}' moet een positief getal zijn.", msgs[2]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stormDurationParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[3]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevelParameterName}' moet een concreet getal zijn.", msgs[4]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[5]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevelFailureConstructionParameterName}' moet een concreet getal zijn.", msgs[6]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevelFailureConstructionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowVelocityStructureClosableParameterName}' moet een concreet getal zijn.", msgs[8]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{flowVelocityStructureClosableParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                Assert.AreEqual($"De verwachtingswaarde voor '{drainCoefficientParameterName}' moet een concreet getal zijn.", msgs[10]);
                Assert.AreEqual($"De standaardafwijking voor '{drainCoefficientParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                Assert.AreEqual($"De waarde voor '{factorStormDurationOpenStructureParameterName}' moet een concreet getal zijn.", msgs[12]);
                Assert.AreEqual($"De waarde voor '{structureNormalOrientationParameterName}' moet een concreet getal zijn.", msgs[13]);
                Assert.AreEqual($"De verwachtingswaarde voor '{areaFlowAperturesParameterName}' moet een positief getal zijn.", msgs[14]);
                Assert.AreEqual($"De standaardafwijking voor '{areaFlowAperturesParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowWidthAtBottomProtectionParameterName}' moet een positief getal zijn.", msgs[16]);
                Assert.AreEqual($"De standaardafwijking voor '{flowWidthAtBottomProtectionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                Assert.AreEqual($"De verwachtingswaarde voor '{storageStructureAreaParameterName}' moet een positief getal zijn.", msgs[18]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{storageStructureAreaParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                Assert.AreEqual($"De verwachtingswaarde voor '{allowedLevelIncreaseStorageParameterName}' moet een positief getal zijn.", msgs[20]);
                Assert.AreEqual($"De standaardafwijking voor '{allowedLevelIncreaseStorageParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[21]);
                Assert.AreEqual($"De verwachtingswaarde voor '{levelCrestStructureParameterName}' moet een concreet getal zijn.", msgs[22]);
                Assert.AreEqual($"De standaardafwijking voor '{levelCrestStructureParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[23]);
                Assert.AreEqual($"De verwachtingswaarde voor '{thresholdHeightOpenWeirParameterName}' moet een concreet getal zijn.", msgs[24]);
                Assert.AreEqual($"De standaardafwijking voor '{thresholdHeightOpenWeirParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[25]);
                Assert.AreEqual($"De verwachtingswaarde voor '{criticalOvertoppingDischargeParameterName}' moet een positief getal zijn.", msgs[26]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{criticalOvertoppingDischargeParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[27]);
                Assert.AreEqual($"De verwachtingswaarde voor '{constructiveStrengthLinearLoadModelParameterName}' moet een positief getal zijn.", msgs[28]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{constructiveStrengthLinearLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[29]);
                Assert.AreEqual($"De verwachtingswaarde voor '{bankWidthParameterName}' moet een concreet getal zijn.", msgs[30]);
                Assert.AreEqual($"De standaardafwijking voor '{bankWidthParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[31]);
                Assert.AreEqual($"De waarde voor '{evaluationLevelParameterName}' moet een concreet getal zijn.", msgs[32]);
                Assert.AreEqual($"De waarde voor '{verticalDistanceParameterName}' moet een concreet getal zijn.", msgs[33]);
                Assert.AreEqual($"De verwachtingswaarde voor '{failureCollisionEnergyParameterName}' moet een positief getal zijn.", msgs[34]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{failureCollisionEnergyParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[35]);
                Assert.AreEqual($"De verwachtingswaarde voor '{shipMassParameterName}' moet een concreet getal zijn.", msgs[36]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{shipMassParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[37]);
                Assert.AreEqual($"De verwachtingswaarde voor '{shipVelocityParameterName}' moet een concreet getal zijn.", msgs[38]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{shipVelocityParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[39]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stabilityLinearLoadModelParameterName}' moet een positief getal zijn.", msgs[40]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stabilityLinearLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[41]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[42]);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           validFilePath);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            var isValid = false;

            // Call 
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            Assert.IsFalse(isValid);
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(43, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"De waarde voor '{volumicWeightWaterParameterName}' moet een concreet getal zijn.", msgs[1]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stormDurationParameterName}' moet een positief getal zijn.", msgs[2]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stormDurationParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[3]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevelParameterName}' moet een concreet getal zijn.", msgs[4]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[5]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevelFailureConstructionParameterName}' moet een concreet getal zijn.", msgs[6]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevelFailureConstructionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowVelocityStructureClosableParameterName}' moet een concreet getal zijn.", msgs[8]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{flowVelocityStructureClosableParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                Assert.AreEqual($"De verwachtingswaarde voor '{drainCoefficientParameterName}' moet een concreet getal zijn.", msgs[10]);
                Assert.AreEqual($"De standaardafwijking voor '{drainCoefficientParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                Assert.AreEqual($"De waarde voor '{factorStormDurationOpenStructureParameterName}' moet een concreet getal zijn.", msgs[12]);
                Assert.AreEqual($"De waarde voor '{structureNormalOrientationParameterName}' moet een concreet getal zijn.", msgs[13]);
                Assert.AreEqual($"De verwachtingswaarde voor '{areaFlowAperturesParameterName}' moet een positief getal zijn.", msgs[14]);
                Assert.AreEqual($"De standaardafwijking voor '{areaFlowAperturesParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowWidthAtBottomProtectionParameterName}' moet een positief getal zijn.", msgs[16]);
                Assert.AreEqual($"De standaardafwijking voor '{flowWidthAtBottomProtectionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                Assert.AreEqual($"De verwachtingswaarde voor '{storageStructureAreaParameterName}' moet een positief getal zijn.", msgs[18]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{storageStructureAreaParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                Assert.AreEqual($"De verwachtingswaarde voor '{allowedLevelIncreaseStorageParameterName}' moet een positief getal zijn.", msgs[20]);
                Assert.AreEqual($"De standaardafwijking voor '{allowedLevelIncreaseStorageParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[21]);
                Assert.AreEqual($"De verwachtingswaarde voor '{levelCrestStructureParameterName}' moet een concreet getal zijn.", msgs[22]);
                Assert.AreEqual($"De standaardafwijking voor '{levelCrestStructureParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[23]);
                Assert.AreEqual($"De verwachtingswaarde voor '{thresholdHeightOpenWeirParameterName}' moet een concreet getal zijn.", msgs[24]);
                Assert.AreEqual($"De standaardafwijking voor '{thresholdHeightOpenWeirParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[25]);
                Assert.AreEqual($"De verwachtingswaarde voor '{criticalOvertoppingDischargeParameterName}' moet een positief getal zijn.", msgs[26]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{criticalOvertoppingDischargeParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[27]);
                Assert.AreEqual($"De verwachtingswaarde voor '{constructiveStrengthQuadraticLoadModelParameterName}' moet een positief getal zijn.", msgs[28]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{constructiveStrengthQuadraticLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[29]);
                Assert.AreEqual($"De verwachtingswaarde voor '{bankWidthParameterName}' moet een concreet getal zijn.", msgs[30]);
                Assert.AreEqual($"De standaardafwijking voor '{bankWidthParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[31]);
                Assert.AreEqual($"De waarde voor '{evaluationLevelParameterName}' moet een concreet getal zijn.", msgs[32]);
                Assert.AreEqual($"De waarde voor '{verticalDistanceParameterName}' moet een concreet getal zijn.", msgs[33]);
                Assert.AreEqual($"De verwachtingswaarde voor '{failureCollisionEnergyParameterName}' moet een positief getal zijn.", msgs[34]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{failureCollisionEnergyParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[35]);
                Assert.AreEqual($"De verwachtingswaarde voor '{shipMassParameterName}' moet een concreet getal zijn.", msgs[36]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{shipMassParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[37]);
                Assert.AreEqual($"De verwachtingswaarde voor '{shipVelocityParameterName}' moet een concreet getal zijn.", msgs[38]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{shipVelocityParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[39]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stabilityQuadraticLoadModelParameterName}' moet een positief getal zijn.", msgs[40]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stabilityQuadraticLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[41]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[42]);
            });
        }

        [Test]
        public void Validate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository, validFilePath);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = (StabilityPointStructureInflowModelType) 100
                }
            };

            // Call
            TestDelegate call = () => StabilityPointStructuresCalculationService.Validate(calculation,
                                                                                          assessmentSection);
            // Assert
            const string expectedMessage = "The value of argument 'input' (100) is invalid for Enum type 'StabilityPointStructureInflowModelType'.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                    expectedMessage).ParamName;
            Assert.AreEqual("input", paramName);
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository, validFilePath);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType,
                    LoadSchematizationType = (LoadSchematizationType) 100
                }
            };

            // Call
            TestDelegate call = () => StabilityPointStructuresCalculationService.Validate(calculation,
                                                                                          assessmentSection);
            // Assert
            const string expectedMessage = "The value of argument 'input' (100) is invalid for Enum type 'LoadSchematizationType'.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                    expectedMessage).ParamName;
            Assert.AreEqual("input", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = (StabilityPointStructureInflowModelType) 100
                }
            };

            var service = new StabilityPointStructuresCalculationService();

            // Call
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                TestDelegate call = () => service.Calculate(calculation,
                                                            failureMechanism.GeneralInput,
                                                            validFilePath,
                                                            validPreprocessorDirectory);

                // Assert
                const string expectedMessage = "The value of argument 'structureInput' (100) is invalid for Enum type 'StabilityPointStructureInflowModelType'.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                        expectedMessage).ParamName;
                Assert.AreEqual("structureInput", paramName);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        public void Calculate_InvalidLoadSchematizationType_ThrowsInvalidEnumArgumentException(StabilityPointStructureInflowModelType inflowModelType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType,
                    LoadSchematizationType = (LoadSchematizationType) 100
                }
            };

            var service = new StabilityPointStructuresCalculationService();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                TestDelegate call = () => service.Calculate(calculation,
                                                            failureMechanism.GeneralInput,
                                                            validFilePath,
                                                            validPreprocessorDirectory);

                // Assert
                const string expectedMessage = "The value of argument 'structureInput' (100) is invalid for Enum type 'LoadSchematizationType'.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                        expectedMessage).ParamName;
                Assert.AreEqual("structureInput", paramName);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_VariousLowSillLinearCalculationsWithoutBreakWater_InputPropertiesCorrectlySentToCalculator(bool useForeshore)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           validFilePath,
                                                                           validPreprocessorDirectory);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointLowSillLinearCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    null,
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
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
                    input.FlowVelocityStructureClosable.CoefficientOfVariation,
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
                    input.ConstructiveStrengthLinearLoadModel.Mean,
                    input.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation,
                    input.StabilityLinearLoadModel.Mean,
                    input.StabilityLinearLoadModel.CoefficientOfVariation,
                    input.WidthFlowApertures.Mean,
                    input.WidthFlowApertures.StandardDeviation,
                    generalInput.ModelFactorLongThreshold.Mean,
                    generalInput.ModelFactorLongThreshold.StandardDeviation);

                var actualInput = (StructuresStabilityPointLowSillLinearCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_VariousLowSillLinearCalculationsWithBreakWater_InputPropertiesCorrectlySentToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear,
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Type = breakWaterType
                        }
                    }
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           validFilePath,
                                                                           validPreprocessorDirectory);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointLowSillLinearCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                    new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
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
                    input.FlowVelocityStructureClosable.CoefficientOfVariation,
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
                    input.ConstructiveStrengthLinearLoadModel.Mean,
                    input.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation,
                    input.StabilityLinearLoadModel.Mean,
                    input.StabilityLinearLoadModel.CoefficientOfVariation,
                    input.WidthFlowApertures.Mean,
                    input.WidthFlowApertures.StandardDeviation,
                    generalInput.ModelFactorLongThreshold.Mean,
                    generalInput.ModelFactorLongThreshold.StandardDeviation);

                var actualInput = (StructuresStabilityPointLowSillLinearCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_VariousLowSillQuadraticCalculationsWithoutBreakWater_InputPropertiesCorrectlySentToCalculator(bool useForeshore)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           validFilePath,
                                                                           validPreprocessorDirectory);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointLowSillQuadraticCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    null,
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
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
                    input.FlowVelocityStructureClosable.CoefficientOfVariation,
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
                    input.ConstructiveStrengthQuadraticLoadModel.Mean,
                    input.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation,
                    input.StabilityQuadraticLoadModel.Mean,
                    input.StabilityQuadraticLoadModel.CoefficientOfVariation,
                    input.WidthFlowApertures.Mean,
                    input.WidthFlowApertures.StandardDeviation,
                    generalInput.ModelFactorLongThreshold.Mean,
                    generalInput.ModelFactorLongThreshold.StandardDeviation);

                var actualInput = (StructuresStabilityPointLowSillQuadraticCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_VariousLowSillQuadraticCalculationsWithBreakWater_InputPropertiesCorrectlySentToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Quadratic,
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Type = breakWaterType
                        }
                    }
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           validFilePath,
                                                                           validPreprocessorDirectory);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointLowSillQuadraticCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                    new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
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
                    input.FlowVelocityStructureClosable.CoefficientOfVariation,
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
                    input.ConstructiveStrengthQuadraticLoadModel.Mean,
                    input.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation,
                    input.StabilityQuadraticLoadModel.Mean,
                    input.StabilityQuadraticLoadModel.CoefficientOfVariation,
                    input.WidthFlowApertures.Mean,
                    input.WidthFlowApertures.StandardDeviation,
                    generalInput.ModelFactorLongThreshold.Mean,
                    generalInput.ModelFactorLongThreshold.StandardDeviation);

                var actualInput = (StructuresStabilityPointLowSillQuadraticCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_VariousFloodedCulvertLinearCalculationsWithoutBreakWater_InputPropertiesCorrectlySentToCalculator(bool useForeshore)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           validFilePath,
                                                                           validPreprocessorDirectory);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointFloodedCulvertLinearCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    null,
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
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
                    input.FlowVelocityStructureClosable.CoefficientOfVariation,
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

                var actualInput = (StructuresStabilityPointFloodedCulvertLinearCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_VariousFloodedCulvertLinearCalculationsWithBreakWater_InputPropertiesCorrectlySentToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Linear,
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Type = breakWaterType
                        }
                    }
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           validFilePath,
                                                                           validPreprocessorDirectory);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointFloodedCulvertLinearCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                    new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
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
                    input.FlowVelocityStructureClosable.CoefficientOfVariation,
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

                var actualInput = (StructuresStabilityPointFloodedCulvertLinearCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_VariousFloodedCulvertQuadraticCalculationsWithoutBreakWater_InputPropertiesCorrectlySentToCalculator(bool useForeshore)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           validFilePath,
                                                                           validPreprocessorDirectory);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    null,
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
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
                    input.FlowVelocityStructureClosable.CoefficientOfVariation,
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
                    input.StabilityQuadraticLoadModel.CoefficientOfVariation,
                    generalInput.ModelFactorLongThreshold.Mean,
                    generalInput.ModelFactorLongThreshold.StandardDeviation);

                var actualInput = (StructuresStabilityPointFloodedCulvertQuadraticCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_VariousFloodedCulvertQuadraticCalculationsWithBreakWater_InputPropertiesCorrectlySentToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Quadratic,
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Type = breakWaterType
                        }
                    }
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           validFilePath,
                                                                           validPreprocessorDirectory);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
                StabilityPointStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                    new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                    input.VolumicWeightWater,
                    generalInput.GravitationalAcceleration,
                    input.LevelCrestStructure.Mean,
                    input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.FactorStormDurationOpenStructure,
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
                    input.FlowVelocityStructureClosable.CoefficientOfVariation,
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
                    input.StabilityQuadraticLoadModel.CoefficientOfVariation,
                    generalInput.ModelFactorLongThreshold.Mean,
                    generalInput.ModelFactorLongThreshold.StandardDeviation);

                var actualInput = (StructuresStabilityPointFloodedCulvertQuadraticCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Calculate_PreprocessorDirectorySet_InputPropertiesCorrectlySentToCalculator([Values(true, false)] bool usePreprocessor)
        {
            // Setup
            string preprocessorDirectory = usePreprocessor
                                               ? validPreprocessorDirectory
                                               : string.Empty;

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, preprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           validFilePath,
                                                                           preprocessorDirectory);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                StructuresStabilityPointCalculationInput actualInput = calculationInputs[0];
                Assert.AreEqual(usePreprocessor, actualInput.PreprocessorSetting.RunPreprocessor);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Calculate_ValidCalculation_LogStartAndEndAndReturnOutput(
            [Values(StabilityPointStructureInflowModelType.FloodedCulvert, StabilityPointStructureInflowModelType.LowSill)]
            StabilityPointStructureInflowModelType inflowModelType,
            [Values(LoadSchematizationType.Quadratic, LoadSchematizationType.Linear)]
            LoadSchematizationType loadSchematizationType,
            [Values(CalculationType.NoForeshore, CalculationType.ForeshoreWithValidBreakWater, CalculationType.ForeshoreWithoutBreakWater)]
            CalculationType calculationType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(new TestStructuresCalculator<StructuresStabilityPointCalculationInput>());
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
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
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = () => new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                                               failureMechanism.GeneralInput,
                                                                                               validFilePath,
                                                                                               validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    LoadSchematizationType = LoadSchematizationType.Linear
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
                        new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                                   failureMechanism.GeneralInput,
                                                                                   validFilePath,
                                                                                   validPreprocessorDirectory);
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
                    StringAssert.StartsWith($"De berekening voor kunstwerk puntconstructies '{calculation.Name}' is mislukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>
            {
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    LoadSchematizationType = LoadSchematizationType.Linear
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
                        new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                                   failureMechanism.GeneralInput,
                                                                                   validFilePath,
                                                                                   validPreprocessorDirectory);
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
                    Assert.AreEqual($"De berekening voor kunstwerk puntconstructies '{calculation.Name}' is mislukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                    StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>
            {
                LastErrorFileContent = "An error occurred"
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;
                string exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                                   failureMechanism.GeneralInput,
                                                                                   validFilePath,
                                                                                   validPreprocessorDirectory);
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
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    StringAssert.StartsWith($"De berekening voor kunstwerk puntconstructies '{calculation.Name}' is mislukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }

            mockRepository.VerifyAll();
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
            input.VolumicWeightWater = value;

            input.InsideWaterLevelFailureConstruction.Mean = value;
            input.InsideWaterLevel.Mean = value;
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
                input.FlowVelocityStructureClosable.CoefficientOfVariation = RoundedDouble.NaN;
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
                input.WidthFlowApertures.StandardDeviation = RoundedDouble.NaN;
                input.BankWidth.StandardDeviation = RoundedDouble.NaN;

                input.StormDuration.Mean = RoundedDouble.NaN;
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
                input.VerticalDistance = RoundedDouble.NaN;
            }
            else
            {
                input.InsideWaterLevelFailureConstruction.StandardDeviation = value;
                input.InsideWaterLevel.StandardDeviation = value;
                input.StormDuration.CoefficientOfVariation = value;
                input.FlowVelocityStructureClosable.CoefficientOfVariation = value;
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
                input.WidthFlowApertures.StandardDeviation = value;
                input.BankWidth.StandardDeviation = value;

                input.StormDuration.Mean = value;
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
                input.VerticalDistance = value;
            }
        }

        #region Parametername mappings

        private const string volumicWeightWaterParameterName = "Volumiek gewicht van water";
        private const string insideWaterLevelFailureConstructionParameterName = "Binnenwaterstand bij constructief falen";
        private const string insideWaterLevelParameterName = "Binnenwaterstand";
        private const string stormDurationParameterName = "Stormduur";
        private const string factorStormDurationOpenStructureParameterName = "Factor voor stormduur hoogwater";
        private const string flowVelocityStructureClosableParameterName = "Kritieke stroomsnelheid sluiting eerste keermiddel";
        private const string drainCoefficientParameterName = "Afvoercoëfficiënt";
        private const string structureNormalOrientationParameterName = "Oriëntatie";
        private const string levelCrestStructureParameterName = "Kerende hoogte";
        private const string thresholdHeightOpenWeirParameterName = "Drempelhoogte";
        private const string areaFlowAperturesParameterName = "Doorstroomoppervlak";
        private const string constructiveStrengthLinearLoadModelParameterName = "Lineaire belastingschematisering constructieve sterkte";
        private const string constructiveStrengthQuadraticLoadModelParameterName = "Kwadratische belastingschematisering constructieve sterkte";
        private const string stabilityLinearLoadModelParameterName = "Lineaire belastingschematisering stabiliteit";
        private const string stabilityQuadraticLoadModelParameterName = "Kwadratische belastingschematisering stabiliteit";
        private const string failureCollisionEnergyParameterName = "Bezwijkwaarde aanvaarenergie";
        private const string shipMassParameterName = "Massa van het schip";
        private const string shipVelocityParameterName = "Aanvaarsnelheid";
        private const string allowedLevelIncreaseStorageParameterName = "Toegestane peilverhoging komberging";
        private const string storageStructureAreaParameterName = "Kombergend oppervlak";
        private const string flowWidthAtBottomProtectionParameterName = "Stroomvoerende breedte bodembescherming";
        private const string criticalOvertoppingDischargeParameterName = "Kritiek instromend debiet";
        private const string widthFlowAperturesParameterName = "Breedte van doorstroomopening";
        private const string bankWidthParameterName = "Bermbreedte";
        private const string evaluationLevelParameterName = "Analysehoogte";
        private const string verticalDistanceParameterName = "Afstand onderkant wand en teen van de dijk/berm";

        #endregion
    }
}