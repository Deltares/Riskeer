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
using Core.Common.Base.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Integration.Test
{
    [TestFixture]
    public class PipingCalculationsViewIntegrationTest
    {
        private const int nameColumnIndex = 0;
        private const int stochasticSoilModelsColumnIndex = 1;
        private const int stochasticSoilProfilesColumnIndex = 2;
        private const int stochasticSoilProfilesProbabilityColumnIndex = 3;
        private const int hydraulicBoundaryLocationsColumnIndex = 4;
        private const int exitPointLColumnIndex = 8;

        [Test]
        public void PipingCalculationsView_DataImportedOrChanged_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                // Show the view
                var pipingCalculationsView = new PipingCalculationsView();
                form.Controls.Add(pipingCalculationsView);
                form.Show();

                // Obtain some relevant controls
                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Set all necessary data to the view
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                pipingCalculationsView.Data = assessmentSection.Piping.CalculationsGroup;
                pipingCalculationsView.AssessmentSection = assessmentSection;
                pipingCalculationsView.PipingFailureMechanism = assessmentSection.Piping;

                // Import failure mechanism sections and ensure the listbox is updated
                DataImportHelper.ImportReferenceLine(assessmentSection);
                IFailureMechanism failureMechanism = assessmentSection.Piping;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                Assert.AreEqual(283, listBox.Items.Count);

                // Import surface lines
                DataImportHelper.ImportPipingSurfaceLines(assessmentSection);

                // Setup some calculations
                var pipingCalculation1 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.Piping.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };
                var pipingCalculation2 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.Piping.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };

                // Add a piping calculation and ensure it is shown in the data grid view after selecting the corresponding dike section
                assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculation1);
                listBox.SelectedItem = assessmentSection.Piping.Sections.First(s => s.Name == "6-3_22");
                Assert.AreEqual(1, dataGridView.Rows.Count);

                // Import soil models and profiles and ensure the corresponding combobox items are updated
                DataImportHelper.ImportPipingStochasticSoilModels(assessmentSection);
                PipingStochasticSoilModelCollection stochasticSoilModelCollection = assessmentSection.Piping.StochasticSoilModels;
                pipingCalculation1.InputParameters.StochasticSoilModel = stochasticSoilModelCollection.First(sl => sl.Name == "PK001_0001_Piping");
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex]).Items.Count);
                Assert.AreEqual("PK001_0001_Piping", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual(1, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex]).Items.Count);
                Assert.AreEqual("<geen>", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);

                // Import hydraulic boundary locations and ensure the corresponding combobox items are updated
                DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection);
                assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();
                Assert.AreEqual(19, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[hydraulicBoundaryLocationsColumnIndex]).Items.Count);

                // Add group and ensure the data grid view is not changed
                var nestedPipingCalculationGroup = new CalculationGroup();
                assessmentSection.Piping.CalculationsGroup.Children.Add(nestedPipingCalculationGroup);
                assessmentSection.Piping.CalculationsGroup.NotifyObservers();
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

                pipingCalculation3.InputParameters.SurfaceLine = assessmentSection.Piping.SurfaceLines.First(sl => sl.Name == "PK001_0001");
                pipingCalculation3.InputParameters.NotifyObservers();
                Assert.AreEqual(3, dataGridView.Rows.Count);

                // Change the name of the first calculation and ensure the data grid view is updated
                pipingCalculation1.Name = "New name";
                pipingCalculation1.NotifyObservers();
                Assert.AreEqual("New name", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

                // Change an input parameter of the second calculation and ensure the data grid view is updated
                var exitPointL = (RoundedDouble) 111.11;
                pipingCalculation2.InputParameters.ExitPointL = exitPointL;
                pipingCalculation2.InputParameters.NotifyObservers();
                Assert.AreEqual(exitPointL.ToString(), dataGridView.Rows[1].Cells[exitPointLColumnIndex].FormattedValue);

                // Add another calculation and assign all soil models
                var pipingCalculation4 = new PipingCalculationScenario(new GeneralPipingInput());
                assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculation4);
                assessmentSection.Piping.CalculationsGroup.NotifyObservers();
                pipingCalculation4.InputParameters.SurfaceLine = assessmentSection.Piping.SurfaceLines.First(sl => sl.Name == "PK001_0001");
                pipingCalculation4.InputParameters.NotifyObservers();
                Assert.AreEqual(4, dataGridView.Rows.Count);

                listBox.SelectedItem = assessmentSection.Piping.Sections.First(s => s.Name == "6-3_22");
                pipingCalculation1.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[0];
                pipingCalculation1.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[0].StochasticSoilProfiles.First();
                pipingCalculation1.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0001_Piping", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.Piping.Sections.First(s => s.Name == "6-3_19");
                pipingCalculation2.InputParameters.SurfaceLine = assessmentSection.Piping.SurfaceLines.First(sl => sl.Name == "PK001_0002");
                pipingCalculation2.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[1];
                pipingCalculation2.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[1].StochasticSoilProfiles.First();
                pipingCalculation2.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0002_Piping", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_4_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.Piping.Sections.First(s => s.Name == "6-3_16");
                pipingCalculation3.InputParameters.SurfaceLine = assessmentSection.Piping.SurfaceLines.First(sl => sl.Name == "PK001_0003");
                pipingCalculation3.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[2];
                pipingCalculation3.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[2].StochasticSoilProfiles.First();
                pipingCalculation3.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0003_Piping", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-7_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.Piping.Sections.First(s => s.Name == "6-3_8");
                pipingCalculation4.InputParameters.SurfaceLine = assessmentSection.Piping.SurfaceLines.First(sl => sl.Name == "PK001_0004");
                pipingCalculation4.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[3];
                pipingCalculation4.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[3].StochasticSoilProfiles.First();
                pipingCalculation4.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0004_Piping", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-8_6_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                // Update stochastic soil models
                DataUpdateHelper.UpdatePipingStochasticSoilModels(assessmentSection);

                listBox.SelectedItem = assessmentSection.Piping.Sections.First(s => s.Name == "6-3_22");
                Assert.AreEqual("PK001_0001_Piping", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(50), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.Piping.Sections.First(s => s.Name == "6-3_19");
                Assert.AreEqual("PK001_0002_Piping", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("<geen>", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(0), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.Piping.Sections.First(s => s.Name == "6-3_16");
                Assert.AreEqual("PK001_0003_Piping", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-7_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.Piping.Sections.First(s => s.Name == "6-3_8");
                Assert.AreEqual("<geen>", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("<geen>", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(0), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);
            }
        }

        private static string GetFormattedProbabilityValue(double value)
        {
            return new RoundedDouble(2, value).ToString();
        }
    }
}