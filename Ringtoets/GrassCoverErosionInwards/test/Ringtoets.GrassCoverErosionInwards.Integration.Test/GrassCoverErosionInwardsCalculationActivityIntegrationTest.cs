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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.GrassCoverErosionInwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationActivityIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Run_InvalidGrassCoverErosionInwardsCalculationNoHydraulicBoundaryDatabase_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

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
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaarde locatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

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

            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

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
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaarde locatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_InvalidGrassCoverErosionInwardsCalculationNoDikeProfile_LogValidationStartAndEndWithError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen dijkprofiel geselecteerd.", msgs[1]);
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

            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
            });
            Assert.AreEqual(ActivityState.Executed, activity.State);
        }

        [Test]
        public void Run_InvalidGrassCoverErosionInwardsCalculationAndRan_PerformGrassCoverErosionInwardsValidationAndCalculationAndLogStartAndEndAndError()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

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

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);
                StringAssert.StartsWith(String.Format("De berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt.", calculation.Name), msgs[3]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[4]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
            mocks.VerifyAll(); // Expect no calls on the observer
        }

        [Test]
        public void Finish_ValidGrassCoverErosionInwardsCalculationAndRan_SetsOutputAndNotifyObserversOfGrassCoverErosionInwardsCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    DikeProfile = CreateDikeProfile()
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.AreEqual((RoundedDouble) 0.625, calculation.Output.FactorOfSafety);
            Assert.AreEqual((RoundedDouble) 382.04, calculation.Output.Probability);
            Assert.AreEqual((RoundedDouble) 2.792, calculation.Output.Reliability);
            Assert.AreEqual((RoundedDouble) 250000.00, calculation.Output.RequiredProbability);
            Assert.AreEqual((RoundedDouble) 4.465, calculation.Output.RequiredReliability);
            mocks.VerifyAll();
        }

        [Test]
        public void Finish_InvalidGrassCoverErosionInwardsCalculationAndRan_DoesNotSetOutputAndDoesNotNotifyObserversOfGrassCoverErosionInwardsCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            assessmentSection.GrassCoverErosionInwards.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1, 1)
                }
            };

            calculation.Attach(observerMock);

            var activity = new GrassCoverErosionInwardsCalculationActivity(calculation, testDataPath, assessmentSection.GrassCoverErosionInwards, assessmentSection);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.IsNull(calculation.Output);
            mocks.VerifyAll(); // Expect no calls on the observer
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
            return new DikeProfile(new Point2D(0, 0))
            {
                Orientation = (RoundedDouble) 5.5,
                BreakWater = new BreakWater(BreakWaterType.Dam, 10.0),
                DikeGeometry =
                {
                    new RoughnessPoint(new Point2D(1.1, 2.2), 0.6),
                    new RoughnessPoint(new Point2D(3.3, 4.4), 0.7)
                },
                ForeshoreGeometry =
                {
                    new Point2D(3.3, 4.4),
                    new Point2D(5.5, 6.6)
                },
                DikeHeight = (RoundedDouble) 10
            };
        }
    }
}