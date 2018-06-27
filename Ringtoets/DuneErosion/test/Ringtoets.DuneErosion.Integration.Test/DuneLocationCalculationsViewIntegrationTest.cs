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
using Core.Common.Util.Reflection;
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
    public class DuneLocationCalculationsViewIntegrationTest
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
        public void GivenDuneLocationCalculationsView_WhenFailureMechanismContributionChanged_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                         bool contributionAfterChangeNotZero,
                                                                                                                                         string expectedErrorMessage)
        {
            // Given
            DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView();

            if (rowSelected)
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
            }

            if (!contributionAfterChangeNotZero)
            {
                view.FailureMechanism.Contribution = 5;
                view.FailureMechanism.NotifyObservers();
            }

            // Precondition
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.AreEqual(rowSelected && !contributionAfterChangeNotZero, button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreNotEqual(expectedErrorMessage, errorProvider.GetError(button));

            // When
            view.FailureMechanism.Contribution = contributionAfterChangeNotZero ? 5 : 0;
            view.FailureMechanism.NotifyObservers();

            // Then
            Assert.AreEqual(rowSelected && contributionAfterChangeNotZero, button.Enabled);
            Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
        }

        private DuneLocationCalculationsView ShowFullyConfiguredDuneLocationCalculationsView()
        {
            var calculations = new ObservableList<DuneLocationCalculation>
            {
                new DuneLocationCalculation(new DuneLocation(1, "1", new Point2D(1.0, 1.0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 50,
                    Offset = 320,
                    D50 = 0.000837
                })),
                new DuneLocationCalculation(new DuneLocation(2, "2", new Point2D(2.0, 2.0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 60,
                    Offset = 230,
                    D50 = 0.000123
                }))
                {
                    Output = new DuneLocationCalculationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationCalculationOutput.ConstructionProperties
                    {
                        WaterLevel = 1.23,
                        WaveHeight = 2.34,
                        WavePeriod = 3.45
                    })
                }
            };

            return ShowDuneLocationCalculationsView(calculations,
                                                    new DuneErosionFailureMechanism(),
                                                    new AssessmentSection(AssessmentSectionComposition.Dike),
                                                    "A");
        }

        private DuneLocationCalculationsView ShowDuneLocationCalculationsView(IObservableEnumerable<DuneLocationCalculation> calculations,
                                                                              DuneErosionFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection,
                                                                              string categoryBoundaryName)
        {
            var view = new DuneLocationCalculationsView(calculations,
                                                        failureMechanism,
                                                        assessmentSection,
                                                        () => 0.01,
                                                        categoryBoundaryName);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}