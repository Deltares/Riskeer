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

using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.TestUtils;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Integration.Test
{
    [TestFixture]
    public class PipingCalculationsViewIntegrationTest
    {
        private const int nameColumnIndex = 0;
        private const int stochasticSoilModelsColumnIndex = 1;
        private const int stochasticSoilProfilesColumnIndex = 2;
        private const int hydraulicBoundaryLocationsColumnIndex = 4;
        private const int exitPointLColumnIndex = 8;

        [Test]
        public void PipingCalculationsView_DataImportedOrChanged_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var handler = new CalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>();

                // Show the view
                var pipingCalculationsView = new PipingCalculationsView(handler);
                form.Controls.Add(pipingCalculationsView);
                form.Show();

                // Obtain some relevant controls
                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Set all necessary data to the view
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                pipingCalculationsView.Data = assessmentSection.PipingFailureMechanism.CalculationsGroup;
                pipingCalculationsView.AssessmentSection = assessmentSection;
                pipingCalculationsView.PipingFailureMechanism = assessmentSection.PipingFailureMechanism;

                // Import failure mechanism sections and ensure the listbox is updated
                DataImportHelper.ImportReferenceLine(assessmentSection);
                IFailureMechanism failureMechanism = assessmentSection.PipingFailureMechanism;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                Assert.AreEqual(283, listBox.Items.Count);

                // Import surface lines
                DataImportHelper.ImportPipingSurfaceLines(assessmentSection);

                // Setup some calculations
                var pipingCalculation1 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };
                var pipingCalculation2 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };

                // Add a piping calculation and ensure it is shown in the data grid view after selecting the corresponding dike section
                assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation1);
                listBox.SelectedItem = assessmentSection.PipingFailureMechanism.Sections.First(s => s.Name == "6-3_22");
                Assert.AreEqual(1, dataGridView.Rows.Count);

                // Import soil models and profiles and ensure the corresponding combobox items are updated
                DataImportHelper.ImportPipingStochasticSoilModels(assessmentSection);
                pipingCalculation1.InputParameters.StochasticSoilModel = assessmentSection.PipingFailureMechanism.StochasticSoilModels.First(sl => sl.Name == "PK001_0001_Piping");
                Assert.AreEqual(1, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex]).Items.Count);
                Assert.AreEqual("PK001_0001_Piping", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual(1, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex]).Items.Count);
                Assert.AreEqual("<geen>", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);

                // Import hydraulic boundary locations and ensure the corresponding combobox items are updated
                DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection);
                Assert.AreEqual(19, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[hydraulicBoundaryLocationsColumnIndex]).Items.Count);

                // Add group and ensure the data grid view is not changed
                var nestedPipingCalculationGroup = new CalculationGroup("New group", false);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(nestedPipingCalculationGroup);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.NotifyObservers();
                Assert.AreEqual(1, dataGridView.Rows.Count);

                // Add another, nested calculation and ensure the data grid view is updated
                nestedPipingCalculationGroup.Children.Add(pipingCalculation2);
                nestedPipingCalculationGroup.NotifyObservers();
                Assert.AreEqual(2, dataGridView.Rows.Count);

                // Add another, nested calculation without surface line and ensure the data grid view is updated when the surface line is set.
                var pipingCalculation3 = new PipingCalculationScenario(new GeneralPipingInput());
                nestedPipingCalculationGroup.Children.Add(pipingCalculation3);
                nestedPipingCalculationGroup.NotifyObservers();
                Assert.AreEqual(2, dataGridView.Rows.Count);

                pipingCalculation3.InputParameters.SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001");
                pipingCalculation3.InputParameters.NotifyObservers();
                Assert.AreEqual(3, dataGridView.Rows.Count);

                // Change the name of the first calculation and ensure the data grid view is updated
                pipingCalculation1.Name = "New name";
                pipingCalculation1.NotifyObservers();
                Assert.AreEqual("New name", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

                // Change an input parameter of the second calculation and ensure the data grid view is updated
                pipingCalculation2.InputParameters.ExitPointL = (RoundedDouble) 111.11;
                pipingCalculation2.InputParameters.NotifyObservers();
                Assert.AreEqual(111.11.ToString(CultureInfo.CurrentCulture), dataGridView.Rows[1].Cells[exitPointLColumnIndex].FormattedValue);
            }
        }
    }
}