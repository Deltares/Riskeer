﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.Integration.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsViewIntegrationTest
    {
        private const int locationCalculateColumnIndex = 0;

        private Form testForm;
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
            mockRepository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(false, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_ContributionChanged_SyncedAccordingly(false, false, message)")]
        [TestCase(true, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_ContributionChanged_SyncedAccordingly(true, false, message)")]
        [TestCase(false, true, "Er zijn geen berekeningen geselecteerd.", TestName = "CalculateButton_ContributionChanged_SyncedAccordingly(false, true, message)")]
        [TestCase(true, true, "", TestName = "CalculateButton_ContributionChanged_SyncedAccordingly(true, true, message)")]
        public void GivenDesignWaterLevelLocationsView_WhenFailureMechanismContributionChanged_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                          bool contributionAfterChangeNotZero,
                                                                                                                                          string expectedErrorMessage)
        {
            // Given
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();

            if (rowSelected)
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true).First();
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
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

        private GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowFullyConfiguredDesignWaterLevelLocationsView()
        {
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            failureMechanism.HydraulicBoundaryLocations.AddRange(new[]
            {
                new HydraulicBoundaryLocation(1, "1", 1.0, 1.0),
                new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(1.23)
                    }
                },
                new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    WaveHeightCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(2.45)
                    }
                }
            });

            var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(failureMechanism,
                                                                                  new AssessmentSection(AssessmentSectionComposition.Dike));

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}