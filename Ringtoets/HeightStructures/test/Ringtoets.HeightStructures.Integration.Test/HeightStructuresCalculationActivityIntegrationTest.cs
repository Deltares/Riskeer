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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Service;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Ringtoets.HeightStructures.Integration.Test
{
    [TestFixture]
    public class HeightStructuresCalculationActivityIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Run_HeightStructuresCalculationInvalidHydraulicBoundaryDatabase_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
                }
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var activity = new HeightStructuresCalculationActivity(calculation, "", failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_HeightStructuresCalculationWithoutHydraulicBoundaryLocation_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new StructuresCalculation<HeightStructuresInput>()
            {
                InputParameters =
                {
                    Structure = new TestHeightStructure()
                }
            };
            calculation.InputParameters.DeviationWaveDirection = (RoundedDouble) 0;

            var activity = new HeightStructuresCalculationActivity(calculation, "", failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_HeightStructuresCalculationWithoutStructure_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new StructuresCalculation<HeightStructuresInput>()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1234, "location", 0, 0)
                }
            };

            var activity = new HeightStructuresCalculationActivity(calculation, "", failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen kunstwerk geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_HeightStructuresCalculationInValidDeviationWaveDirection_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            const string parameterName = "afwijking golfrichting";
            string expectedValidationMessage = string.Format("Validatie mislukt: Er is geen concreet getal ingevoerd voor '{0}'.", parameterName);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    Structure = new TestHeightStructure()
                }
            };
            calculation.InputParameters.DeviationWaveDirection = (RoundedDouble) double.NaN;

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
        }

        [Test]
        public void Run_HeightStructuresCalculationInvalidStructureNormalOrientation_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            string expectedValidationMessage = "Validatie mislukt: De waarde voor de oriëntatie moet in het bereik [0, 360] liggen.";

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    Structure = new TestHeightStructure()
                }
            };
            calculation.InputParameters.StructureNormalOrientation = (RoundedDouble) double.NaN;

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);
            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
        }

        [Test]
        [TestCaseSource("NormalDistributionsWithInvalidMean")]
        public void Run_HeightStructuresCalculationInvalidNormalDistributionMeans_LogValidationStartAndEndWithError(double meanOne, double meanTwo, double meanThree, string parameterName)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            string expectedValidationMessage = string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een geldig getal zijn.", parameterName);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    Structure = new TestHeightStructure()
                }
            };

            calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean = (RoundedDouble) meanOne;
            calculation.InputParameters.LevelCrestStructure.Mean = (RoundedDouble) meanTwo;
            calculation.InputParameters.WidthFlowApertures.Mean = (RoundedDouble) meanThree;

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
        }

        [Test]
        [TestCaseSource("LogNormalDistributionsWithInvalidMean")]
        public void Run_HeightStructuresCalculationInvalidLogNormalDistributionMeans_LogValidationStartAndEndWithError(double meanOne, double meanTwo, double meanThree,
                                                                                                                       double meanFour, double meanFive, string parameterName)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            string expectedValidationMessage = string.Format("Validatie mislukt: De verwachtingswaarde voor '{0}' moet een positief getal zijn.", parameterName);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    Structure = new TestHeightStructure()
                }
            };
            calculation.InputParameters.StormDuration.Mean = (RoundedDouble) meanOne;
            calculation.InputParameters.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) meanTwo;
            calculation.InputParameters.StorageStructureArea.Mean = (RoundedDouble) meanThree;
            calculation.InputParameters.FlowWidthAtBottomProtection.Mean = (RoundedDouble) meanFour;
            calculation.InputParameters.CriticalOvertoppingDischarge.Mean = (RoundedDouble) meanFive;

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
        }

        [Test]
        [TestCaseSource("DistributionsWithInvalidDeviation")]
        public void Run_HeightStructuresCalculationDistributionInvalidStandardDeviation_LogValidationStartAndEndWithError(double deviationOne, double deviationTwo,
                                                                                                                          double deviationThree, double deviationFour, string parameterName)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            string expectedValidationMessage = string.Format("Validatie mislukt: De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterName);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    Structure = new TestHeightStructure()
                }
            };
            calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation = (RoundedDouble) deviationOne;
            calculation.InputParameters.LevelCrestStructure.StandardDeviation = (RoundedDouble) deviationTwo;
            calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation = (RoundedDouble) deviationThree;
            calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation = (RoundedDouble) deviationFour;

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
        }

        [Test]
        [TestCaseSource("DistributionsWithInvalidCoefficient")]
        public void Run_HeightStructuresCalculationDistributionInvalidVariationCoefficient_LogValidationStartAndEndWithError(double coefficientOne, double coefficientTwo,
                                                                                                                             double coefficientThree, double coefficientFour, string parameterName)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            string expectedValidationMessage = string.Format("Validatie mislukt: De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", parameterName);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    Structure = new TestHeightStructure()
                }
            };
            calculation.InputParameters.StormDuration.CoefficientOfVariation = (RoundedDouble) coefficientOne;
            calculation.InputParameters.StorageStructureArea.CoefficientOfVariation = (RoundedDouble) coefficientTwo;
            calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation = (RoundedDouble) coefficientThree;
            calculation.InputParameters.WidthFlowApertures.CoefficientOfVariation = (RoundedDouble) coefficientFour;

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(expectedValidationMessage, msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
        }

        [Test]
        public void Run_ValidHeightStructuresCalculation_PerformHeightStructuresValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    Structure = new TestHeightStructure()
                }
            };

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);
            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[4]);
                    StringAssert.StartsWith("Hoogte kunstwerken berekeningsverslag. Klik op details voor meer informatie.", msgs[3]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
        }

        [Test]
        public void Run_InValidHeightStructuresCalculationAndRan_PerformHeightStructuresValidationAndCalculationAndLogStartAndEndAndError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation()
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1),
                    Structure = new TestHeightStructure()
                }
            };

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);
                StringAssert.StartsWith(string.Format("De berekening voor hoogte kunstwerk '{0}' is niet gelukt.", calculation.Name), msgs[3]);
                StringAssert.StartsWith("Hoogte kunstwerken berekeningsverslag. Klik op details voor meer informatie.", msgs[4]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[5]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Finish_ValidHeightStructuresCalculationAndRan_SetsOutputAndNotifyObserversOfHeightStructuresCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    Structure = new TestHeightStructure()
                }
            };

            calculation.Attach(observerMock);

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void Finish_InValidHeightStructuresCalculationAndRan_DoesNotSetOutputAndNotifyObserversOfHeightStructuresCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1),
                    Structure = new TestHeightStructure()
                }
            };

            calculation.Attach(observerMock);

            var activity = new HeightStructuresCalculationActivity(calculation, testDataPath, failureMechanism, assessmentSection);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.IsNull(calculation.Output);
            mocks.VerifyAll();
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
                yield return new TestCaseData(double.NaN, 1, 2, 3, "modelfactor overloopdebiet volkomen overlaat");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, 3, "modelfactor overloopdebiet volkomen overlaat");

                yield return new TestCaseData(1, double.NaN, 2, 3, "kerende hoogte");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, 3, "kerende hoogte");

                yield return new TestCaseData(1, 2, double.NaN, 3, "toegestane peilverhoging komberging");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, 3, "toegestane peilverhoging komberging");

                yield return new TestCaseData(1, 2, 3, double.NaN, "stroomvoerende breedte bodembescherming");
                yield return new TestCaseData(1, 2, 3, double.PositiveInfinity, "stroomvoerende breedte bodembescherming");
            }
        }

        private static IEnumerable<TestCaseData> DistributionsWithInvalidCoefficient
        {
            get
            {
                yield return new TestCaseData(double.NaN, 1, 2, 3, "stormduur");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, 3, "stormduur");

                yield return new TestCaseData(1, double.NaN, 2, 3, "kombergend oppervlak");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, 3, "kombergend oppervlak");

                yield return new TestCaseData(1, 2, double.NaN, 3, "kritiek instromend debiet");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, 3, "kritiek instromend debiet");

                yield return new TestCaseData(1, 2, 3, double.NaN, "breedte van doorstroomopening");
                yield return new TestCaseData(1, 2, 3, double.PositiveInfinity, "breedte van doorstroomopening");
            }
        }

        #endregion
    }
}