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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.ClosingStructures.Service.Test
{
    [TestFixture]
    public class ClosingStructuresCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => ClosingStructuresCalculationService.Validate(null, assessmentSectionStub);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ClosingStructuresCalculationService.Validate(new TestClosingStructuresCalculation(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            var calculation = new TestClosingStructuresCalculation();

            bool isValid = false;

            // Call
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var calculation = new TestClosingStructuresCalculation();

            bool isValid = false;

            // Call
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen kunstwerk geselecteerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
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
                Assert.AreEqual(20, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stormDuration}' moet een positief getal zijn.", msgs[1]);
                Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stormDuration}' moet groter zijn dan of gelijk zijn aan 0.", msgs[2]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{modelFactorSuperCriticalFlow}' moet een concreet getal zijn.", msgs[3]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{modelFactorSuperCriticalFlow}' moet groter zijn dan of gelijk zijn aan 0.", msgs[4]);
                Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{factorStormDurationOpenStructure}'.", msgs[5]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{widthFlowApertures}' moet een concreet getal zijn.", msgs[6]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{widthFlowApertures}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{structureNormalOrientation}'.", msgs[8]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowWidthAtBottomProtection}' moet een positief getal zijn.", msgs[9]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowWidthAtBottomProtection}' moet groter zijn dan of gelijk zijn aan 0.", msgs[10]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{storageStructureArea}' moet een positief getal zijn.", msgs[11]);
                Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{storageStructureArea}' moet groter zijn dan of gelijk zijn aan 0.", msgs[12]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{allowedLevelIncreaseStorage}' moet een positief getal zijn.", msgs[13]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{allowedLevelIncreaseStorage}' moet groter zijn dan of gelijk zijn aan 0.", msgs[14]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{levelCrestStructureNotClosing}' moet een concreet getal zijn.", msgs[15]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{levelCrestStructureNotClosing}' moet groter zijn dan of gelijk zijn aan 0.", msgs[16]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{criticalOvertoppingDischarge}' moet een positief getal zijn.", msgs[17]);
                Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{criticalOvertoppingDischarge}' moet groter zijn dan of gelijk zijn aan 0.", msgs[18]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[19]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stormDuration}' moet een positief getal zijn.", msgs[1]);
                Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stormDuration}' moet groter zijn dan of gelijk zijn aan 0.", msgs[2]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevel}' moet een concreet getal zijn.", msgs[3]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevel}' moet groter zijn dan of gelijk zijn aan 0.", msgs[4]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{modelFactorSuperCriticalFlow}' moet een concreet getal zijn.", msgs[5]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{modelFactorSuperCriticalFlow}' moet groter zijn dan of gelijk zijn aan 0.", msgs[6]);
                Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{factorStormDurationOpenStructure}'.", msgs[7]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{widthFlowApertures}' moet een concreet getal zijn.", msgs[8]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{widthFlowApertures}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowWidthAtBottomProtection}' moet een positief getal zijn.", msgs[10]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowWidthAtBottomProtection}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{storageStructureArea}' moet een positief getal zijn.", msgs[12]);
                Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{storageStructureArea}' moet groter zijn dan of gelijk zijn aan 0.", msgs[13]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{allowedLevelIncreaseStorage}' moet een positief getal zijn.", msgs[14]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{allowedLevelIncreaseStorage}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{thresholdHeightOpenWeir}' moet een concreet getal zijn.", msgs[16]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{thresholdHeightOpenWeir}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{criticalOvertoppingDischarge}' moet een positief getal zijn.", msgs[18]);
                Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{criticalOvertoppingDischarge}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[20]);
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{stormDuration}' moet een positief getal zijn.", msgs[1]);
                Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{stormDuration}' moet groter zijn dan of gelijk zijn aan 0.", msgs[2]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{insideWaterLevel}' moet een concreet getal zijn.", msgs[3]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{insideWaterLevel}' moet groter zijn dan of gelijk zijn aan 0.", msgs[4]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{drainCoefficient}' moet een concreet getal zijn.", msgs[5]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{drainCoefficient}' moet groter zijn dan of gelijk zijn aan 0.", msgs[6]);
                Assert.AreEqual($"Validatie mislukt: Er is geen concreet getal ingevoerd voor '{factorStormDurationOpenStructure}'.", msgs[7]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{areaFlowApertures}' moet een positief getal zijn.", msgs[8]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{areaFlowApertures}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{flowWidthAtBottomProtection}' moet een positief getal zijn.", msgs[10]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{flowWidthAtBottomProtection}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{storageStructureArea}' moet een positief getal zijn.", msgs[12]);
                Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{storageStructureArea}' moet groter zijn dan of gelijk zijn aan 0.", msgs[13]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{allowedLevelIncreaseStorage}' moet een positief getal zijn.", msgs[14]);
                Assert.AreEqual($"Validatie mislukt: De standaardafwijking voor '{allowedLevelIncreaseStorage}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                Assert.AreEqual($"Validatie mislukt: De verwachtingswaarde voor '{criticalOvertoppingDischarge}' moet een positief getal zijn.", msgs[16]);
                Assert.AreEqual($"Validatie mislukt: De variatiecoëfficiënt voor '{criticalOvertoppingDischarge}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[18]);
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
            const string expectedMessage = "The value of argument 'inputParameters' (9001) is invalid for Enum type 'ClosingStructureInflowModelType'.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                    expectedMessage).ParamName;
            Assert.AreEqual("inputParameters", paramName);
            Assert.IsFalse(isValid);
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Validate_UsesBreakWaterAndHasInvalidBreakWaterSettings_LogsErrorAndReturnsFalse(
            [Values(ClosingStructureInflowModelType.VerticalWall, ClosingStructureInflowModelType.LowSill, ClosingStructureInflowModelType.FloodedCulvert)] ClosingStructureInflowModelType inflowModelType,
            [Values(double.NaN, double.NegativeInfinity, double.PositiveInfinity)] double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation
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
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen geldige damhoogte ingevoerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ClosingStructuresCalculationService().Calculate(null, assessmentSectionStub, new ClosingStructuresFailureMechanism(), string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ClosingStructuresCalculationService().Calculate(new TestClosingStructuresCalculation(),
                                                                                          null,
                                                                                          new ClosingStructuresFailureMechanism(),
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
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ClosingStructuresCalculationService().Calculate(new TestClosingStructuresCalculation(),
                                                                                          assessmentSectionStub,
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
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();
            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
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
                                                            validFilePath);

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

            var calculation = new TestClosingStructuresCalculation
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
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;

                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    assessmentSectionStub,
                                                                    closingStructuresFailureMechanism,
                                                                    validFilePath);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralClosingStructuresInput generalInput = closingStructuresFailureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureVerticalWallCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
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
                    input.ProbabilityOrFrequencyOpenStructureBeforeFlooding,
                    generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.ModelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    input.LevelCrestStructureNotClosing.Mean, input.LevelCrestStructureNotClosing.StandardDeviation,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.StandardDeviation,
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
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(useBreakWater);
            }

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;

                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    assessmentSectionStub,
                                                                    closingStructuresFailureMechanism,
                                                                    validFilePath);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralClosingStructuresInput generalInput = closingStructuresFailureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureLowSillCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
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
                    input.ProbabilityOrFrequencyOpenStructureBeforeFlooding,
                    input.ModelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    generalInput.ModelFactorSubCriticalFlow.Mean, generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                    input.ThresholdHeightOpenWeir.Mean, input.ThresholdHeightOpenWeir.StandardDeviation,
                    input.InsideWaterLevel.Mean, input.InsideWaterLevel.StandardDeviation,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.StandardDeviation);

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
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(useBreakWater);
            }

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).StructuresClosureCalculator;

                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    assessmentSectionStub,
                                                                    closingStructuresFailureMechanism,
                                                                    validFilePath);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);
                Assert.AreEqual(testDataPath, calculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, calculator.RingId);

                GeneralClosingStructuresInput generalInput = closingStructuresFailureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureFloodedCulvertCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
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
                    input.ProbabilityOrFrequencyOpenStructureBeforeFlooding,
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
            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(closingStructuresFailureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
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
                                                                                        validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
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
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();
            closingStructuresFailureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(closingStructuresFailureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

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
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

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
                calculator.LastErrorFileContent = "An error occurred";
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
                    StringAssert.StartsWith($"De berekening voor kunstwerk sluiten '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
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
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

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
                    StringAssert.StartsWith($"De berekening voor kunstwerk sluiten '{calculation.Name}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
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
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

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
                calculator.LastErrorFileContent = "An error occurred";

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
                    StringAssert.StartsWith($"De berekening voor kunstwerk sluiten '{calculation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }
            mockRepository.VerifyAll();
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
                input.WidthFlowApertures.StandardDeviation = RoundedDouble.NaN;
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
                input.WidthFlowApertures.StandardDeviation = value;
            }
        }

        #region Parameter name mappings

        private const string insideWaterLevel = "binnenwaterstand";
        private const string stormDuration = "stormduur";
        private const string factorStormDurationOpenStructure = "factor voor stormduur hoogwater";
        private const string modelFactorSuperCriticalFlow = "modelfactor overloopdebiet volkomen overlaat";
        private const string drainCoefficient = "afvoercoëfficiënt";
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