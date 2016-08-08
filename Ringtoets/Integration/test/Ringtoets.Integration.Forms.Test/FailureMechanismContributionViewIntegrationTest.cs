﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Windows.Forms;
using Core.Common.Base;
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
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class FailureMechanismContributionViewIntegrationTest
    {
        [Test]
        public void NormTextBox_ValueChanged_ClearsDependentData()
        {
            // Setup
            const int normValue = 200;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 0.0, 0.0)
            {
                WaveHeight = 3.0,
                DesignWaterLevel = 4.2
            });

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            });
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation()
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            });
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(new HeightStructuresCalculation()
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            });

            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            MockRepository mockRepository = new MockRepository();
            IObserver observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            failureMechanismContribution.Attach(observerMock);

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
                PipingCalculation pipingCalculation = assessmentSection.PipingFailureMechanism.CalculationsGroup.Children[0] as PipingCalculation;
                GrassCoverErosionInwardsCalculation grassCoverErosionInwardsCalculation = assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children[0] as GrassCoverErosionInwardsCalculation;
                HeightStructuresCalculation heightStructuresCalculation = assessmentSection.HeightStructures.CalculationsGroup.Children[0] as HeightStructuresCalculation;

                // Precondition
                Assert.AreEqual(failureMechanismContribution.Norm.ToString(), normTester.Text);
                Assert.AreEqual(3.0, hydraulicBoundaryLocation.WaveHeight);
                Assert.AreEqual(4.2, hydraulicBoundaryLocation.DesignWaterLevel);
                Assert.IsNotNull(pipingCalculation.Output);
                Assert.IsNotNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNotNull(heightStructuresCalculation.Output);

                // Call
                normTester.Properties.Text = normValue.ToString();

                // Assert
                Assert.AreEqual(normValue, failureMechanismContribution.Norm);
                Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
                Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
                Assert.IsNull(pipingCalculation.Output);
                Assert.IsNull(grassCoverErosionInwardsCalculation.Output);
                Assert.IsNull(heightStructuresCalculation.Output);
            }
            mockRepository.VerifyAll();
        }   
    }
}