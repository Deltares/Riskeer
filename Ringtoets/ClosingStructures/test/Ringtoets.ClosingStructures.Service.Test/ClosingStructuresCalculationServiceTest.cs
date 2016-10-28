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
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
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

        #region Parameter name mappings

        private static readonly Dictionary<string, string> parameterNames = new Dictionary<string, string>()
        {
            {
                "insideWaterLevel", "binnenwaterstand"
            },
            {
                "stormDuration", "stormduur"
            },
            {
                "deviationWaveDirection", "afwijking golfrichting"
            },
            {
                "factorStormDurationOpenStructure", "factor voor stormduur hoogwater"
            },
            {
                "modelFactorSuperCriticalFlow", "modelfactor overloopdebiet volkomen overlaat"
            },
            {
                "drainCoefficient", "afvoercoëfficient"
            },
            {
                "structureNormalOrientation", "oriëntatie"
            },
            {
                "thresholdHeightOpenWeir", "drempelhoogte"
            },
            {
                "areaFlowApertures", "doorstroomoppervlak"
            },
            {
                "levelCrestStructureNotClosing", "kruinhoogte niet gesloten kering"
            },
            {
                "allowedLevelIncreaseStorage", "toegestane peilverhoging komberging"
            },
            {
                "storageStructureArea", "kombergend oppervlak"
            },
            {
                "flowWidthAtBottomProtection", "stroomvoerende breedte bodembescherming"
            },
            {
                "criticalOvertoppingDischarge", "kritiek instromend debiet"
            },
            {
                "widthFlowApertures", "breedte van doorstroomopening"
            }
        };

        #endregion

        [Test]
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(), mockRepository);
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
            var assessmentSectionStub = CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation()
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
            var assessmentSectionStub = CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation()
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
                StringAssert.StartsWith("Validatie mislukt: Er is geen kunstwerk sluiten geselecteerd.", msgs[1]);
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
            var assessmentSectionStub = CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation()
            {
                Name = name
            };

            SetInvalidInputParameters(calculation, (RoundedDouble) value);

            // Call 
            bool isValid = false;
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(21, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["stormDuration"]), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["stormDuration"]), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", parameterNames["deviationWaveDirection"]), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", parameterNames["modelFactorSuperCriticalFlow"]), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["modelFactorSuperCriticalFlow"]), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", parameterNames["factorStormDurationOpenStructure"]), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", parameterNames["widthFlowApertures"]), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["widthFlowApertures"]), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", parameterNames["structureNormalOrientation"]), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["flowWidthAtBottomProtection"]), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["flowWidthAtBottomProtection"]), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["storageStructureArea"]), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["storageStructureArea"]), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["allowedLevelIncreaseStorage"]), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["allowedLevelIncreaseStorage"]), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", parameterNames["levelCrestStructureNotClosing"]), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["levelCrestStructureNotClosing"]), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["criticalOvertoppingDischarge"]), msgs[18]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["criticalOvertoppingDischarge"]), msgs[19]);
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
            var assessmentSectionStub = CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = ClosingStructureInflowModelType.LowSill
                }
            };
            SetInvalidInputParameters(calculation, (RoundedDouble) value);

            bool isValid = false;

            // Call 
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(21, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["stormDuration"]), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["stormDuration"]), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", parameterNames["insideWaterLevel"]), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["insideWaterLevel"]), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", parameterNames["modelFactorSuperCriticalFlow"]), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["modelFactorSuperCriticalFlow"]), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", parameterNames["factorStormDurationOpenStructure"]), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", parameterNames["widthFlowApertures"]), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["widthFlowApertures"]), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["flowWidthAtBottomProtection"]), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["flowWidthAtBottomProtection"]), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["storageStructureArea"]), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["storageStructureArea"]), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["allowedLevelIncreaseStorage"]), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["allowedLevelIncreaseStorage"]), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", parameterNames["thresholdHeightOpenWeir"]), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["thresholdHeightOpenWeir"]), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["criticalOvertoppingDischarge"]), msgs[18]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["criticalOvertoppingDischarge"]), msgs[19]);
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
            var assessmentSectionStub = CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation()
            {
                Name = name,
                InputParameters =
                {
                    InflowModelType = ClosingStructureInflowModelType.FloodedCulvert
                }
            };
            SetInvalidInputParameters(calculation, (RoundedDouble) value);

            bool isValid = false;

            // Call 
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(19, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["stormDuration"]), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["stormDuration"]), msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", parameterNames["insideWaterLevel"]), msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["insideWaterLevel"]), msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", parameterNames["drainCoefficient"]), msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["drainCoefficient"]), msgs[6]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", parameterNames["factorStormDurationOpenStructure"]), msgs[7]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["areaFlowApertures"]), msgs[8]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["areaFlowApertures"]), msgs[9]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["flowWidthAtBottomProtection"]), msgs[10]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["flowWidthAtBottomProtection"]), msgs[11]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["storageStructureArea"]), msgs[12]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["storageStructureArea"]), msgs[13]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["allowedLevelIncreaseStorage"]), msgs[14]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["allowedLevelIncreaseStorage"]), msgs[15]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterNames["criticalOvertoppingDischarge"]), msgs[16]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterNames["criticalOvertoppingDischarge"]), msgs[17]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[18]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidInFlowModelType_ThrowsNotSupportedException()
        {
            // Setup 
            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const string name = "<very nice name>";

            var calculation = new TestClosingStructuresCalculation()
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
            StringAssert.StartsWith("The value of argument 'inputParameters' (9001) is invalid for Enum type 'ClosingStructureInflowModelType'.", exception.Message);
            Assert.IsFalse(isValid);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_InvalidInFlowModelType_ThrowsNotSupportedException()
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

            // Call
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
                var exception = Assert.Throws<InvalidEnumArgumentException>(call);
                Assert.AreEqual("calculation", exception.ParamName);
                StringAssert.StartsWith("The value of argument 'calculation' (100) is invalid for Enum type 'ClosingStructureInflowModelType'.", exception.Message);
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
            var assessmentSectionStub = CreateAssessmentSectionStub(closingStructuresFailureMechanism, mockRepository);
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
            var assessmentSectionStub = CreateAssessmentSectionStub(closingStructuresFailureMechanism, mockRepository);
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
            var assessmentSectionStub = CreateAssessmentSectionStub(closingStructuresFailureMechanism, mockRepository);
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
        [TestCase(ClosingStructureInflowModelType.VerticalWall)]
        [TestCase(ClosingStructureInflowModelType.LowSill)]
        [TestCase(ClosingStructureInflowModelType.FloodedCulvert)]
        public void Calculate_ValidCalculation_LogStartAndEndAndReturnOutput(ClosingStructureInflowModelType inflowModelType)
        {
            // Setup
            var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(closingStructuresFailureMechanism, mockRepository);
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
                    InflowModelType = inflowModelType
                }
            };

            // Call
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
                StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekeningsverslag. Klik op details voor meer informatie.", msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
            Assert.IsNotNull(calculation.Output);

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
            var assessmentSectionStub = CreateAssessmentSectionStub(closingStructuresFailureMechanism, mockRepository);
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
                StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekeningsverslag. Klik op details voor meer informatie.", msgs[2]);
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
            var assessmentSectionStub = CreateAssessmentSectionStub(closingStructuresFailureMechanism, mockRepository);
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

        private static IAssessmentSection CreateAssessmentSectionStub(IFailureMechanism failureMechanism, MockRepository mockRepository)
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

        private static void SetInvalidInputParameters(StructuresCalculation<ClosingStructuresInput> input, RoundedDouble value)
        {
            input.InputParameters.DeviationWaveDirection = RoundedDouble.NaN;
            input.InputParameters.DrainCoefficient.Mean = value;
            input.InputParameters.FactorStormDurationOpenStructure = value;
            input.InputParameters.InsideWaterLevel.Mean = value;
            input.InputParameters.LevelCrestStructureNotClosing.Mean = value;
            input.InputParameters.ModelFactorSuperCriticalFlow.Mean = value;
            input.InputParameters.StructureNormalOrientation = RoundedDouble.NaN;
            input.InputParameters.ThresholdHeightOpenWeir.Mean = value;
            input.InputParameters.WidthFlowApertures.Mean = value;

            if (double.IsNegativeInfinity(value))
            {
                input.InputParameters.AllowedLevelIncreaseStorage.Mean = RoundedDouble.NaN;
                input.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation = RoundedDouble.NaN;
                input.InputParameters.AreaFlowApertures.Mean = RoundedDouble.NaN;
                input.InputParameters.AreaFlowApertures.StandardDeviation = RoundedDouble.NaN;
                input.InputParameters.CriticalOvertoppingDischarge.Mean = RoundedDouble.NaN;
                input.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation = RoundedDouble.NaN;
                input.InputParameters.DrainCoefficient.StandardDeviation = RoundedDouble.NaN;
                input.InputParameters.FlowWidthAtBottomProtection.Mean = RoundedDouble.NaN;
                input.InputParameters.FlowWidthAtBottomProtection.StandardDeviation = RoundedDouble.NaN;
                input.InputParameters.InsideWaterLevel.StandardDeviation = RoundedDouble.NaN;
                input.InputParameters.LevelCrestStructureNotClosing.StandardDeviation = RoundedDouble.NaN;
                input.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation = RoundedDouble.NaN;
                input.InputParameters.StorageStructureArea.Mean = RoundedDouble.NaN;
                input.InputParameters.StorageStructureArea.CoefficientOfVariation = RoundedDouble.NaN;
                input.InputParameters.StormDuration.Mean = RoundedDouble.NaN;
                input.InputParameters.StormDuration.CoefficientOfVariation = RoundedDouble.NaN;
                input.InputParameters.ThresholdHeightOpenWeir.StandardDeviation = RoundedDouble.NaN;
                input.InputParameters.WidthFlowApertures.CoefficientOfVariation = RoundedDouble.NaN;
            }
            else
            {
                input.InputParameters.AllowedLevelIncreaseStorage.Mean = value;
                input.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation = value;
                input.InputParameters.AreaFlowApertures.Mean = value;
                input.InputParameters.AreaFlowApertures.StandardDeviation = value;
                input.InputParameters.CriticalOvertoppingDischarge.Mean = value;
                input.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation = value;
                input.InputParameters.DrainCoefficient.StandardDeviation = value;
                input.InputParameters.FlowWidthAtBottomProtection.Mean = value;
                input.InputParameters.FlowWidthAtBottomProtection.StandardDeviation = value;
                input.InputParameters.InsideWaterLevel.StandardDeviation = value;
                input.InputParameters.LevelCrestStructureNotClosing.StandardDeviation = value;
                input.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation = value;
                input.InputParameters.StorageStructureArea.Mean = value;
                input.InputParameters.StorageStructureArea.CoefficientOfVariation = value;
                input.InputParameters.StormDuration.Mean = value;
                input.InputParameters.StormDuration.CoefficientOfVariation = value;
                input.InputParameters.ThresholdHeightOpenWeir.StandardDeviation = value;
                input.InputParameters.WidthFlowApertures.CoefficientOfVariation = value;
            }
        }
    }
}