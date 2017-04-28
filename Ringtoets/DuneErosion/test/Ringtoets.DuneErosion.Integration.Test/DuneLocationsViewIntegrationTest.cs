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

using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.Views;
using Ringtoets.Integration.Data;

namespace Ringtoets.DuneErosion.Integration.Test
{
    [TestFixture]
    public class DuneLocationsViewIntegrationTest
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
        public void GivenDuneLocationsView_WhenFailureMechanismContributionChanged_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                              bool contributionAfterChangeNotZero,
                                                                                                                              string expectedErrorMessage)
        {
            // Given
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();
            view.AssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            if (rowSelected)
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
            }

            var failureMechanism = new DuneErosionFailureMechanism();
            if (!contributionAfterChangeNotZero)
            {
                failureMechanism.Contribution = 5;
            }
            view.FailureMechanism = failureMechanism;

            // Precondition
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.AreEqual(rowSelected && !contributionAfterChangeNotZero, button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreNotEqual(expectedErrorMessage, errorProvider.GetError(button));

            // When
            failureMechanism.Contribution = contributionAfterChangeNotZero ? 5 : 0;
            view.AssessmentSection.NotifyObservers();

            // Then
            Assert.AreEqual(rowSelected && contributionAfterChangeNotZero, button.Enabled);
            Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
        }

        private DuneLocationsView ShowFullyConfiguredDuneLocationsView()
        {
            DuneLocationsView view = ShowDuneLocationsView();
            view.Data = new ObservableList<DuneLocation>
            {
                new DuneLocation(1, "1", new Point2D(1.0, 1.0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 50,
                    Offset = 320,
                    D50 = 0.000837
                }),
                new DuneLocation(2, "2", new Point2D(2.0, 2.0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 60,
                    Offset = 230,
                    D50 = 0.000123
                })
                {
                    Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                    {
                        WaterLevel = 1.23,
                        WaveHeight = 2.34,
                        WavePeriod = 3.45
                    })
                }
            };

            return view;
        }

        private DuneLocationsView ShowDuneLocationsView()
        {
            var view = new DuneLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}