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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Properties;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class FailureMechanismContributionViewIntegrationTest
    {
        [Test]
        public void NormTextBox_ValueChanged_ClearsDependentDataAndNotifiesObserversAndLogsMessages()
        {
            // Setup
            const int normValue = 200;

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 0.0, 0.0)
            {
                WaveHeight = 3.0,
                DesignWaterLevel = 4.2
            });

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };

            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            PipingCalculation pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };
            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            HeightStructuresCalculation emptyHeightStructuresCalculation = new HeightStructuresCalculation();
            HeightStructuresCalculation heightStructuresCalculation = new HeightStructuresCalculation
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(grassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(heightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            MockRepository mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            IObserver calculationObserver = mockRepository.StrictMock<IObserver>();
            calculationObserver.Expect(co => co.UpdateObserver()).Repeat.Times(3);
            IObserver hydraulicBoundaryDatabaseObserver = mockRepository.StrictMock<IObserver>();
            hydraulicBoundaryDatabaseObserver.Expect(hbdo => hbdo.UpdateObserver());
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            hydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);
            
            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            pipingCalculation.Attach(calculationObserver);
            grassCoverErosionInwardsCalculation.Attach(calculationObserver);
            heightStructuresCalculation.Attach(calculationObserver);

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester("normInput");

                HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);
                Assert.AreEqual(3.0, hydraulicBoundaryLocation.WaveHeight);
                Assert.AreEqual(4.2, hydraulicBoundaryLocation.DesignWaterLevel);
                Assert.IsNotNull(pipingCalculation.Output);
                Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNotNull(heightStructuresCalculation.Output);

                // Call
                Action call = () => normTester.Properties.Text = normValue.ToString();

                // Assert
                TestHelper.AssertLogMessages(call, msgs =>
                {
                    string[] messages = msgs.ToArray();
                    Assert.AreEqual(string.Format(Resources.FailureMechanismContributionView_NormValueChanged_Results_of_0_calculations_cleared, 3), messages[0]);
                    Assert.AreEqual(Resources.FailureMechanismContributionView_NormValueChanged_Waveheight_and_design_water_level_results_cleared, messages[1]);
                });
                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
                Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
                Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
                Assert.IsNull(pipingCalculation.Output);
                Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNull(heightStructuresCalculation.Output);
            }
            mockRepository.VerifyAll();
        }
        
        [Test]
        public void NormTextBox_NoHydraulicBoundaryDatabaseAndNoCalculationsWithOutputAndValueChanged_NoObserversNotifiedAndMessagesLogged()
        {
            // Setup
            const int normValue = 200;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            PipingCalculation emptyPipingCalculation = new PipingCalculation(new GeneralPipingInput());
            GrassCoverErosionInwardsCalculation emptyGrassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();
            HeightStructuresCalculation emptyHeightStructuresCalculation = new HeightStructuresCalculation();

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(emptyPipingCalculation);
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(emptyGrassCoverErosionInwardsCalculation);
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(emptyHeightStructuresCalculation);

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            MockRepository mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            IObserver calculationObserver = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);
            emptyPipingCalculation.Attach(calculationObserver);
            emptyGrassCoverErosionInwardsCalculation.Attach(calculationObserver);
            emptyHeightStructuresCalculation.Attach(calculationObserver);

            using (Form form = new Form())
            using (FailureMechanismContributionView distributionView = new FailureMechanismContributionView
            {
                Data = failureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                form.Controls.Add(distributionView);
                form.Show();

                ControlTester normTester = new ControlTester("normInput");

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);

                // Call
                Action call = () => normTester.Properties.Text = normValue.ToString();

                // Assert
                TestHelper.AssertLogMessagesCount(call, 0);
                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
            }
            mockRepository.VerifyAll();
        }
    }
}