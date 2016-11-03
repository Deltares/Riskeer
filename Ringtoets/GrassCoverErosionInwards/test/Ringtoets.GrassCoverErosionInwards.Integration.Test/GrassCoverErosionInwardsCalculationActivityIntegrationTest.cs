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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Ringtoets.GrassCoverErosionInwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationActivityIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Run_InvalidGrassCoverErosionInwardsCalculationInvalidHydraulicBoundaryDatabase_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = Path.Combine(testDataPath, "notexisting.sqlite")
                }
            };
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = CreateDikeProfile()
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, "", assessmentSection.GrassCoverErosionInwards, assessmentSection);

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
        public void Run_ValidGrassCoverErosionInwardsCalculation_PerformGrassCoverErosionInwardsValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

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
                    StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[3]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[4]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
        }

        [Test]
        public void Run_InvalidGrassCoverErosionInwardsCalculationAndRan_PerformGrassCoverErosionInwardsValidationAndCalculationAndLogStartAndEndAndErrorAndDoesNotPerformDikeHeightCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1),
                    DikeProfile = CreateDikeProfile()
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).OvertoppingCalculator;
                calculator.EndInFailure = true;

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
                    StringAssert.StartsWith(string.Format("De berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt.", calculation.Name), msgs[3]);
                    StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[4]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[5]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
                mocks.VerifyAll(); // Expect no calls on the observer
            }
        }

        [Test]
        public void Run_CalculateDikeHeightTrueAndValidProbabilityCalculationAndInvalidDikeHeightCalculationAndRan_PerformGrassCoverErosionInwardsValidationAndCalculationAndLogStartAndEndAndError()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    Norm = 1
                }
            };
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    CalculateDikeHeight = true
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                calculator.DikeHeight = double.NaN;
                calculator.EndInFailure = true;

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);
                    StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie:", msgs[3]);
                    StringAssert.StartsWith(string.Format("De HBN berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt.", calculation.Name), msgs[4]);
                    StringAssert.StartsWith("Dijkhoogte berekening is uitgevoerd op de tijdelijke locatie:", msgs[5]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[6]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
                mocks.VerifyAll(); // Expect no calls on the observer
            }
        }

        [Test]
        public void Run_CalculateDikeHeightFalse_ProgressTextSetToProbabilityCalculation()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    CalculateDikeHeight = false
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                activity.Run();
            }

            // Assert
            Assert.AreEqual("Stap 1 van 1 | Uitvoeren overloop en overslag berekening", activity.ProgressText);
        }

        [Test]
        public void Run_CalculateDikeHeightTrueAndInvalidProbabilityCalculation_ProgressTextSetToProbabilityCalculation()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1),
                    DikeProfile = CreateDikeProfile(),
                    CalculateDikeHeight = true
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                activity.Run();
            }

            // Assert
            Assert.AreEqual("Stap 1 van 2 | Uitvoeren overloop en overslag berekening", activity.ProgressText);
        }

        [Test]
        public void Run_CalculateDikeHeightTrue_ProgressTextSetToDikeHeightCalculation()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    CalculateDikeHeight = true
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                activity.Run();
            }

            // Assert
            Assert.AreEqual("Stap 2 van 2 | Uitvoeren dijkhoogte berekening", activity.ProgressText);
        }

        [Test]
        public void Finish_CalculateDikeHeightFalseAndValidGrassCoverErosionInwardsCalculationAndRan_SetsOutputAndNotifyObserversOfGrassCoverErosionInwardsCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    CalculateDikeHeight = false
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.IsFalse(double.IsNaN(calculation.Output.ProbabilityAssessmentOutput.Reliability));
            Assert.IsNaN(calculation.Output.DikeHeight);
            Assert.IsFalse(calculation.Output.DikeHeightCalculated);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false, TestName = "Finish_InvalidCalculationAndRan_DoesNotSetOutputAndNotifyObservers(false)")]
        [TestCase(true, TestName = "Finish_InvalidCalculationAndRan_DoesNotSetOutputAndNotifyObservers(true)")]
        public void Finish_InvalidGrassCoverErosionInwardsCalculationAndRan_DoesNotSetOutputAndNotifyObservers(bool calculateDikeHeight)
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1),
                    CalculateDikeHeight = calculateDikeHeight
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNull(calculation.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void Finish_CalculateDikeHeightTrueAndValidCalculationsAndRan_OutputSetAndObserversNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    CalculateDikeHeight = true
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            ProbabilityAssessmentOutput probabilisticAssessmentOutput = calculation.Output.ProbabilityAssessmentOutput;
            Assert.IsFalse(double.IsNaN(probabilisticAssessmentOutput.Reliability));
            Assert.IsFalse(double.IsNaN(calculation.Output.DikeHeight));
            Assert.IsTrue(calculation.Output.DikeHeightCalculated);
            mocks.VerifyAll();
        }

        [Test]
        public void Finish_CalculateDikeHeightTrueAndInvalidDikeHeightCalculationAndRan_OutputSetAndObserversNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    Norm = 1
                }
            };
            ImportHydraulicBoundaryDatabase(assessmentSection);
            AddSectionToAssessmentSection(assessmentSection);

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile(),
                    CalculateDikeHeight = true
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DikeHeightCalculator;
                calculator.DikeHeight = double.NaN;
                calculator.EndInFailure = true;

                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.IsNaN(calculation.Output.DikeHeight);
            Assert.IsTrue(calculation.Output.DikeHeightCalculated);
            mocks.VerifyAll();
        }

        private static void AddSectionToAssessmentSection(AssessmentSection assessmentSection)
        {
            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));
        }

        private void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }
        }

        private static DikeProfile CreateDikeProfile()
        {
            return new DikeProfile(new Point2D(0, 0),
                                   new[]
                                   {
                                       new RoughnessPoint(new Point2D(1.1, 2.2), 0.6),
                                       new RoughnessPoint(new Point2D(3.3, 4.4), 0.7)
                                   }, new[]
                                   {
                                       new Point2D(3.3, 4.4),
                                       new Point2D(5.5, 6.6)
                                   },
                                   new BreakWater(BreakWaterType.Dam, 10.0),
                                   new DikeProfile.ConstructionProperties
                                   {
                                       Orientation = 5.5,
                                       DikeHeight = 10
                                   });
        }
    }
}