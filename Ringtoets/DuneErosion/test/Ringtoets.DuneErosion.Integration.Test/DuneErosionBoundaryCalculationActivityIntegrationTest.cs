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
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Service;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Integration.Data;

namespace Ringtoets.DuneErosion.Integration.Test
{
    [TestFixture]
    
    public class DuneErosionBoundaryCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Run_ValidCalculation_PerformCalculationAndLogStartAndEnd()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "13-1"
            };

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            var activity = new DuneErosionBoundaryCalculationActivity(duneLocation, failureMechanism, assessmentSection);
            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", duneLocation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Duinafslag berekening voor locatie '{0}' is niet geconvergeerd.", duneLocation.Name), msgs[1]);
                    StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", duneLocation.Name), msgs[3]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
        }

        [Test]
        public void Run_FailureMechanismContributionNull_LogMessageAndActivityStateFailed()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "13-1"
            };

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new DuneErosionFailureMechanism();
            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            var activity = new DuneErosionBoundaryCalculationActivity(duneLocation, failureMechanism, assessmentSection);
            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", duneLocation.Name), msgs[0]);
                    Assert.AreEqual("De bijdrage van dit toetsspoor is nul. Daardoor kunnen de berekeningen niet worden uitgevoerd.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", duneLocation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        [TestCase(true, "An error occurred")]
        [TestCase(true, null)]
        [TestCase(false, "An error occurred")]
        public void Run_InvalidCalculationAndRan_PerformCalculationAndActivityStateFailed(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "13-1"
            };

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new DuneErosionFailureMechanism();
            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            var activity = new DuneErosionBoundaryCalculationActivity(duneLocation, failureMechanism, assessmentSection);
            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                calculator.EndInFailure = endInFailure;
                calculator.LastErrorFileContent = lastErrorFileContent;

                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        public void Run_CalculationAlreadyPerformed_CalculationNotPerformedAndActivityStateSkipped()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "13-1"
            };

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            var initialOutput = new DuneLocationOutput(0, 0, 0, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged);
            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007)
            {
                Output = initialOutput
            };

            var activity = new DuneErosionBoundaryCalculationActivity(duneLocation, failureMechanism, assessmentSection);
            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Skipped, activity.State);
                Assert.AreSame(initialOutput, duneLocation.Output);
            }
        }
    }
}