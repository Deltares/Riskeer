// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.Integration.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightLocationsViewIntegrationTest
    {
        private const int calculateColumnIndex = 0;

        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        [TestCase(false, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_ContributionChanged_SyncedAccordingly(false, false, message)")]
        [TestCase(true, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_ContributionChanged_SyncedAccordingly(true, false, message)")]
        [TestCase(false, true, "Er zijn geen berekeningen geselecteerd.", TestName = "CalculateButton_ContributionChanged_SyncedAccordingly(false, true, message)")]
        [TestCase(true, true, "", TestName = "CalculateButton_ContributionChanged_SyncedAccordingly(true, true, message)")]
        public void GivenWaveHeightLocationsView_WhenFailureMechanismContributionChanged_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                    bool contributionAfterChangeNotZero,
                                                                                                                                    string expectedErrorMessage)
        {
            // Given
            GrassCoverErosionOutwardsWaveHeightCalculationsView view = ShowFullyConfiguredWaveHeightLocationsView();

            if (rowSelected)
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true).First();
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;
            }

            GrassCoverErosionOutwardsFailureMechanism failureMechanism = view.FailureMechanism;
            if (contributionAfterChangeNotZero)
            {
                failureMechanism.Contribution = 0;
                failureMechanism.NotifyObservers();
            }

            // Precondition
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.AreEqual(rowSelected && !contributionAfterChangeNotZero, button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreNotEqual(expectedErrorMessage, errorProvider.GetError(button));

            // When
            failureMechanism.Contribution = contributionAfterChangeNotZero ? 5 : 0;
            failureMechanism.NotifyObservers();

            // Then
            Assert.AreEqual(rowSelected && contributionAfterChangeNotZero, button.Enabled);
            Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
        }

        private GrassCoverErosionOutwardsWaveHeightCalculationsView ShowFullyConfiguredWaveHeightLocationsView()
        {
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(1, "1", 1.0, 1.0)),
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(2, "2", 2.0, 2.0))
                {
                    Output = new TestHydraulicBoundaryLocationOutput(1.23)
                },
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0))
                {
                    Output = new TestHydraulicBoundaryLocationOutput(2.45)
                }
            };

            var view = new GrassCoverErosionOutwardsWaveHeightCalculationsView(calculations,
                                                                               failureMechanism,
                                                                               new AssessmentSection(AssessmentSectionComposition.Dike),
                                                                               () => 0.01);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}