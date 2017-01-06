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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
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

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationService.Validate(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new TestStabilityPointStructuresCalculation();

            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationService.Validate(calculation, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            var calculation = new TestStabilityPointStructuresCalculation();

            bool isValid = false;

            // Call
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
                                         {
                                             var msgs = messages.ToArray();
                                             Assert.AreEqual(3, msgs.Length);
                                             var name = calculation.Name;
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                                             StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
                                         });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidCalculationValidHydraulicBoundaryDatabaseNoSettings_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var calculation = new TestStabilityPointStructuresCalculation();

            bool isValid = false;

            // Call
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
                                         {
                                             var msgs = messages.ToArray();
                                             Assert.AreEqual(3, msgs.Length);
                                             var name = calculation.Name;
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
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
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new StabilityPointStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation
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
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                                             StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
                                         });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_CalculationWithoutStructuresValidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;
            mockRepository.ReplayAll();

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
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             Assert.AreEqual("Validatie mislukt: Er is geen kunstwerk geselecteerd.", msgs[1]);
                                             StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
                                         });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_UseBreakWaterInvalidBreakWaterHeight_LogErrorAndReturnFalse(
            [Values(StabilityPointStructureInflowModelType.FloodedCulvert, StabilityPointStructureInflowModelType.LowSill)] StabilityPointStructureInflowModelType inflowModelType,
            [Values(LoadSchematizationType.Quadratic, LoadSchematizationType.Linear)] LoadSchematizationType loadSchematizationType,
            [Values(double.NaN, double.PositiveInfinity, double.NegativeInfinity)] double breakWaterHeight)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;
            mockRepository.ReplayAll();

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
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             Assert.AreEqual("Validatie mislukt: Er is geen geldige damhoogte ingevoerd.", msgs[1]);
                                             StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation
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
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{volumicWeightWaterParameterName}'.", msgs[1]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stormDurationParameterName}' moet een positief getal zijn.", msgs[2]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stormDurationParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[3]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevelParameterName}' moet een concreet getal zijn.", msgs[4]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[5]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevelFailureConstructionParameterName}' moet een concreet getal zijn.", msgs[6]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevelFailureConstructionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowVelocityStructureClosableParameterName}' moet een concreet getal zijn.", msgs[8]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowVelocityStructureClosableParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{modelFactorSuperCriticalFlowParameterName}' moet een concreet getal zijn.", msgs[10]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{modelFactorSuperCriticalFlowParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{factorStormDurationOpenStructureParameterName}'.", msgs[12]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{structureNormalOrientationParameterName}'.", msgs[13]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{widthFlowAperturesParameterName}' moet een concreet getal zijn.", msgs[14]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{widthFlowAperturesParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowWidthAtBottomProtectionParameterName}' moet een positief getal zijn.", msgs[16]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowWidthAtBottomProtectionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{storageStructureAreaParameterName}' moet een positief getal zijn.", msgs[18]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{storageStructureAreaParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{allowedLevelIncreaseStorageParameterName}' moet een positief getal zijn.", msgs[20]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{allowedLevelIncreaseStorageParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[21]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{levelCrestStructureParameterName}' moet een concreet getal zijn.", msgs[22]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{levelCrestStructureParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[23]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{thresholdHeightOpenWeirParameterName}' moet een concreet getal zijn.", msgs[24]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{thresholdHeightOpenWeirParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[25]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{criticalOvertoppingDischargeParameterName}' moet een positief getal zijn.", msgs[26]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{criticalOvertoppingDischargeParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[27]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{constructiveStrengthLinearLoadModelParameterName}' moet een positief getal zijn.", msgs[28]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{constructiveStrengthLinearLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[29]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{bankWidthParameterName}' moet een concreet getal zijn.", msgs[30]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{bankWidthParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[31]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{evaluationLevelParameterName}'.", msgs[32]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{verticalDistanceParameterName}'.", msgs[33]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{failureCollisionEnergyParameterName}' moet een positief getal zijn.", msgs[34]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{failureCollisionEnergyParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[35]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{shipMassParameterName}' moet een concreet getal zijn.", msgs[36]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{shipMassParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[37]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{shipVelocityParameterName}' moet een concreet getal zijn.", msgs[38]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{shipVelocityParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[39]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stabilityLinearLoadModelParameterName}' moet een positief getal zijn.", msgs[40]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stabilityLinearLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[41]);
                                             StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[42]);
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

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation
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
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{volumicWeightWaterParameterName}'.", msgs[1]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stormDurationParameterName}' moet een positief getal zijn.", msgs[2]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stormDurationParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[3]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevelParameterName}' moet een concreet getal zijn.", msgs[4]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[5]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevelFailureConstructionParameterName}' moet een concreet getal zijn.", msgs[6]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevelFailureConstructionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowVelocityStructureClosableParameterName}' moet een concreet getal zijn.", msgs[8]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowVelocityStructureClosableParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{modelFactorSuperCriticalFlowParameterName}' moet een concreet getal zijn.", msgs[10]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{modelFactorSuperCriticalFlowParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{factorStormDurationOpenStructureParameterName}'.", msgs[12]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{structureNormalOrientationParameterName}'.", msgs[13]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{widthFlowAperturesParameterName}' moet een concreet getal zijn.", msgs[14]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{widthFlowAperturesParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowWidthAtBottomProtectionParameterName}' moet een positief getal zijn.", msgs[16]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowWidthAtBottomProtectionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{storageStructureAreaParameterName}' moet een positief getal zijn.", msgs[18]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{storageStructureAreaParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{allowedLevelIncreaseStorageParameterName}' moet een positief getal zijn.", msgs[20]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{allowedLevelIncreaseStorageParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[21]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{levelCrestStructureParameterName}' moet een concreet getal zijn.", msgs[22]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{levelCrestStructureParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[23]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{thresholdHeightOpenWeirParameterName}' moet een concreet getal zijn.", msgs[24]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{thresholdHeightOpenWeirParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[25]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{criticalOvertoppingDischargeParameterName}' moet een positief getal zijn.", msgs[26]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{criticalOvertoppingDischargeParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[27]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{constructiveStrengthQuadraticLoadModelParameterName}' moet een positief getal zijn.", msgs[28]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{constructiveStrengthQuadraticLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[29]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{bankWidthParameterName}' moet een concreet getal zijn.", msgs[30]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{bankWidthParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[31]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{evaluationLevelParameterName}'.", msgs[32]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{verticalDistanceParameterName}'.", msgs[33]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{failureCollisionEnergyParameterName}' moet een positief getal zijn.", msgs[34]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{failureCollisionEnergyParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[35]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{shipMassParameterName}' moet een concreet getal zijn.", msgs[36]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{shipMassParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[37]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{shipVelocityParameterName}' moet een concreet getal zijn.", msgs[38]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{shipVelocityParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[39]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stabilityQuadraticLoadModelParameterName}' moet een positief getal zijn.", msgs[40]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stabilityQuadraticLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[41]);
                                             StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[42]);
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

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation
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
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{volumicWeightWaterParameterName}'.", msgs[1]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stormDurationParameterName}' moet een positief getal zijn.", msgs[2]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stormDurationParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[3]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevelParameterName}' moet een concreet getal zijn.", msgs[4]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[5]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevelFailureConstructionParameterName}' moet een concreet getal zijn.", msgs[6]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevelFailureConstructionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowVelocityStructureClosableParameterName}' moet een concreet getal zijn.", msgs[8]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowVelocityStructureClosableParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{drainCoefficientParameterName}' moet een concreet getal zijn.", msgs[10]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{drainCoefficientParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{factorStormDurationOpenStructureParameterName}'.", msgs[12]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{structureNormalOrientationParameterName}'.", msgs[13]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{areaFlowAperturesParameterName}' moet een positief getal zijn.", msgs[14]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{areaFlowAperturesParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowWidthAtBottomProtectionParameterName}' moet een positief getal zijn.", msgs[16]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowWidthAtBottomProtectionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{storageStructureAreaParameterName}' moet een positief getal zijn.", msgs[18]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{storageStructureAreaParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{allowedLevelIncreaseStorageParameterName}' moet een positief getal zijn.", msgs[20]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{allowedLevelIncreaseStorageParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[21]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{levelCrestStructureParameterName}' moet een concreet getal zijn.", msgs[22]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{levelCrestStructureParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[23]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{thresholdHeightOpenWeirParameterName}' moet een concreet getal zijn.", msgs[24]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{thresholdHeightOpenWeirParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[25]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{criticalOvertoppingDischargeParameterName}' moet een positief getal zijn.", msgs[26]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{criticalOvertoppingDischargeParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[27]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{constructiveStrengthLinearLoadModelParameterName}' moet een positief getal zijn.", msgs[28]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{constructiveStrengthLinearLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[29]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{bankWidthParameterName}' moet een concreet getal zijn.", msgs[30]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{bankWidthParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[31]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{evaluationLevelParameterName}'.", msgs[32]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{verticalDistanceParameterName}'.", msgs[33]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{failureCollisionEnergyParameterName}' moet een positief getal zijn.", msgs[34]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{failureCollisionEnergyParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[35]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{shipMassParameterName}' moet een concreet getal zijn.", msgs[36]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{shipMassParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[37]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{shipVelocityParameterName}' moet een concreet getal zijn.", msgs[38]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{shipVelocityParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[39]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stabilityLinearLoadModelParameterName}' moet een positief getal zijn.", msgs[40]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stabilityLinearLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[41]);
                                             StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[42]);
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

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new TestStabilityPointStructuresCalculation
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
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{volumicWeightWaterParameterName}'.", msgs[1]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stormDurationParameterName}' moet een positief getal zijn.", msgs[2]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stormDurationParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[3]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevelParameterName}' moet een concreet getal zijn.", msgs[4]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[5]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevelFailureConstructionParameterName}' moet een concreet getal zijn.", msgs[6]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevelFailureConstructionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowVelocityStructureClosableParameterName}' moet een concreet getal zijn.", msgs[8]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowVelocityStructureClosableParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{drainCoefficientParameterName}' moet een concreet getal zijn.", msgs[10]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{drainCoefficientParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{factorStormDurationOpenStructureParameterName}'.", msgs[12]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{structureNormalOrientationParameterName}'.", msgs[13]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{areaFlowAperturesParameterName}' moet een positief getal zijn.", msgs[14]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{areaFlowAperturesParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowWidthAtBottomProtectionParameterName}' moet een positief getal zijn.", msgs[16]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowWidthAtBottomProtectionParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{storageStructureAreaParameterName}' moet een positief getal zijn.", msgs[18]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{storageStructureAreaParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{allowedLevelIncreaseStorageParameterName}' moet een positief getal zijn.", msgs[20]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{allowedLevelIncreaseStorageParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[21]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{levelCrestStructureParameterName}' moet een concreet getal zijn.", msgs[22]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{levelCrestStructureParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[23]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{thresholdHeightOpenWeirParameterName}' moet een concreet getal zijn.", msgs[24]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{thresholdHeightOpenWeirParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[25]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{criticalOvertoppingDischargeParameterName}' moet een positief getal zijn.", msgs[26]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{criticalOvertoppingDischargeParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[27]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{constructiveStrengthQuadraticLoadModelParameterName}' moet een positief getal zijn.", msgs[28]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{constructiveStrengthQuadraticLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[29]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{bankWidthParameterName}' moet een concreet getal zijn.", msgs[30]);
                                             Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{bankWidthParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[31]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{evaluationLevelParameterName}'.", msgs[32]);
                                             Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{verticalDistanceParameterName}'.", msgs[33]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{failureCollisionEnergyParameterName}' moet een positief getal zijn.", msgs[34]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{failureCollisionEnergyParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[35]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{shipMassParameterName}' moet een concreet getal zijn.", msgs[36]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{shipMassParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[37]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{shipVelocityParameterName}' moet een concreet getal zijn.", msgs[38]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{shipVelocityParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[39]);
                                             Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stabilityQuadraticLoadModelParameterName}' moet een positief getal zijn.", msgs[40]);
                                             Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stabilityQuadraticLoadModelParameterName}' moet groter zijn dan of gelijk zijn aan 0.", msgs[41]);
                                             StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[42]);
                                         });
        }

        [Test]
        public void Validate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;
            mockRepository.ReplayAll();

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
        public void Validate_InvalidLoadSchematizationType_LogsErrorAndReturnsFalse(StabilityPointStructureInflowModelType inflowModelType)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;
            mockRepository.ReplayAll();

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

            bool isValid = false;

            // Call 
            Action call = () => isValid = StabilityPointStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            Assert.IsFalse(isValid);
            TestHelper.AssertLogMessages(call, messages =>
                                         {
                                             var msgs = messages.ToArray();
                                             Assert.AreEqual(3, msgs.Length);
                                             StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                                             Assert.AreEqual("Validatie mislukt: Er is geen belastingschematisering geselecteerd.", msgs[1]);
                                             StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
                                         });
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            TestDelegate test = () => new StabilityPointStructuresCalculationService().Calculate(null,
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
            var calculation = new TestStabilityPointStructuresCalculation();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            TestDelegate test = () => new StabilityPointStructuresCalculationService().Calculate(calculation,
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

            var calculation = new TestStabilityPointStructuresCalculation();

            // Call
            TestDelegate test = () => new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                                                 assessmentSection,
                                                                                                 null,
                                                                                                 string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
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
                                                            failureMechanism,
                                                            testDataPath);

                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();

                // Assert
                Assert.AreEqual(0, calculationInputs.Length);
                const string expectedMessage = "The value of argument 'calculation' (100) is invalid for Enum type 'StabilityPointStructureInflowModelType'.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                        expectedMessage).ParamName;
                Assert.AreEqual("calculation", paramName);
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
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                TestDelegate call = () => service.Calculate(calculation,
                                                            assessmentSectionStub,
                                                            failureMechanism,
                                                            testDataPath);

                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();

                // Assert
                Assert.AreEqual(0, calculationInputs.Length);
                const string expectedMessage = "The value of argument 'calculation' (100) is invalid for Enum type 'LoadSchematizationType'.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                        expectedMessage).ParamName;
                Assert.AreEqual("calculation", paramName);
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
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
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(useBreakWater);
            }

            FailureMechanismSection failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanism,
                                                                           validFilePath);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
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
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(useBreakWater);
            }

            FailureMechanismSection failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanism,
                                                                           validFilePath);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
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
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(useBreakWater);
            }

            FailureMechanismSection failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanism,
                                                                           validFilePath);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

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
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(useBreakWater);
            }

            FailureMechanismSection failureMechanismSection = failureMechanism.Sections.First();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresStabilityPointCalculator;

                // Call
                new StabilityPointStructuresCalculationService().Calculate(calculation,
                                                                           assessmentSectionStub,
                                                                           failureMechanism,
                                                                           validFilePath);

                // Assert
                StructuresStabilityPointCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralStabilityPointStructuresInput generalInput = failureMechanism.GeneralInput;
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
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
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
                                                                                               validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                                             {
                                                 var msgs = messages.ToArray();
                                                 Assert.AreEqual(3, msgs.Length);
                                                 StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                                                 StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                                                 StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[2]);
                                             });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestStabilityPointStructuresCalculation
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
                                  validFilePath);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

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
                calculator.LastErrorFileContent = "An error occurred";
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
                                                 StringAssert.StartsWith($"De berekening voor kunstwerk puntconstructies '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                                                 StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

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
                                                 StringAssert.StartsWith($"De berekening voor kunstwerk puntconstructies '{calculation.Name}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                                                 StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
                                                                    {
                                                                        new Point2D(0, 0),
                                                                        new Point2D(1, 1)
                                                                    }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

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
                calculator.LastErrorFileContent = "An error occurred";

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
                                                 StringAssert.StartsWith($"De berekening voor kunstwerk puntconstructies '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                                                 StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                                                 StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[3]);
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

        private const string volumicWeightWaterParameterName = "volumiek gewicht van water";
        private const string insideWaterLevelFailureConstructionParameterName = "binnenwaterstand bij constructief falen";
        private const string insideWaterLevelParameterName = "binnenwaterstand";
        private const string stormDurationParameterName = "stormduur";
        private const string factorStormDurationOpenStructureParameterName = "factor voor stormduur hoogwater";
        private const string modelFactorSuperCriticalFlowParameterName = "modelfactor overloopdebiet volkomen overlaat";
        private const string flowVelocityStructureClosableParameterName = "kritieke stroomsnelheid sluiting eerste keermiddel";
        private const string drainCoefficientParameterName = "afvoercoëfficiënt";
        private const string structureNormalOrientationParameterName = "oriëntatie";
        private const string levelCrestStructureParameterName = "kerende hoogte";
        private const string thresholdHeightOpenWeirParameterName = "drempelhoogte";
        private const string areaFlowAperturesParameterName = "doorstroomoppervlak";
        private const string constructiveStrengthLinearLoadModelParameterName = "lineaire belastingschematisering constructieve sterkte";
        private const string constructiveStrengthQuadraticLoadModelParameterName = "kwadratische belastingschematisering constructieve sterkte";
        private const string stabilityLinearLoadModelParameterName = "lineaire belastingschematisering stabiliteit";
        private const string stabilityQuadraticLoadModelParameterName = "kwadratische belastingschematisering stabiliteit";
        private const string failureCollisionEnergyParameterName = "bezwijkwaarde aanvaarenergie";
        private const string shipMassParameterName = "massa van het schip";
        private const string shipVelocityParameterName = "aanvaarsnelheid";
        private const string allowedLevelIncreaseStorageParameterName = "toegestane peilverhoging komberging";
        private const string storageStructureAreaParameterName = "kombergend oppervlak";
        private const string flowWidthAtBottomProtectionParameterName = "stroomvoerende breedte bodembescherming";
        private const string criticalOvertoppingDischargeParameterName = "kritiek instromend debiet";
        private const string widthFlowAperturesParameterName = "breedte van doorstroomopening";
        private const string bankWidthParameterName = "bermbreedte";
        private const string evaluationLevelParameterName = "analysehoogte";
        private const string verticalDistanceParameterName = "afstand onderkant wand en teen van de dijk/berm";

        #endregion
    }
}