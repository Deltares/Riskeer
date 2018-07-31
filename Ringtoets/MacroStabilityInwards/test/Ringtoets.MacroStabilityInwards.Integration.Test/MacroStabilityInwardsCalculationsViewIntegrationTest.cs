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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.Views;

namespace Ringtoets.MacroStabilityInwards.Integration.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationsViewIntegrationTest
    {
        private const int nameColumnIndex = 0;
        private const int stochasticSoilModelsColumnIndex = 1;
        private const int stochasticSoilProfilesColumnIndex = 2;
        private const int stochasticSoilProfilesProbabilityColumnIndex = 3;
        private const int hydraulicBoundaryLocationsColumnIndex = 4;

        [Test]
        public void MacroStabilityInwardsCalculationsView_DataImportedOrChanged_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                // Show the view
                var calculationsView = new MacroStabilityInwardsCalculationsView();
                form.Controls.Add(calculationsView);
                form.Show();

                // Obtain some relevant controls
                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Set all necessary data to the view
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                calculationsView.Data = assessmentSection.MacroStabilityInwards.CalculationsGroup;
                calculationsView.AssessmentSection = assessmentSection;
                calculationsView.MacroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;

                // Import failure mechanism sections and ensure the listbox is updated
                DataImportHelper.ImportReferenceLine(assessmentSection);
                IFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                Assert.AreEqual(283, listBox.Items.Count);

                // Import surface lines
                DataImportHelper.ImportMacroStabilityInwardsSurfaceLines(assessmentSection);

                // Setup some calculations
                var calculation1 = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };
                var calculation2 = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };

                // Add a calculation and ensure it is shown in the data grid view after selecting the corresponding dike section
                assessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(calculation1);
                listBox.SelectedItem = assessmentSection.MacroStabilityInwards.Sections.First(s => s.Name == "6-3_22");
                Assert.AreEqual(1, dataGridView.Rows.Count);

                // Import soil models and profiles and ensure the corresponding combobox items are updated
                DataImportHelper.ImportMacroStabilityInwardsStochasticSoilModels(assessmentSection);
                MacroStabilityInwardsStochasticSoilModelCollection stochasticSoilModelCollection = assessmentSection.MacroStabilityInwards.StochasticSoilModels;
                calculation1.InputParameters.StochasticSoilModel = stochasticSoilModelCollection.First(sl => sl.Name == "PK001_0001_Stability");
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex]).Items.Count);
                Assert.AreEqual("PK001_0001_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual(1, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex]).Items.Count);
                Assert.AreEqual("<geen>", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);

                // Import hydraulic boundary locations and ensure the corresponding combobox items are updated
                DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection);
                assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();
                Assert.AreEqual(19, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[hydraulicBoundaryLocationsColumnIndex]).Items.Count);

                // Add group and ensure the data grid view is not changed
                var nestedCalculationGroup = new CalculationGroup();
                assessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(nestedCalculationGroup);
                assessmentSection.MacroStabilityInwards.CalculationsGroup.NotifyObservers();
                Assert.AreEqual(1, dataGridView.Rows.Count);

                // Add another, nested calculation and ensure the data grid view is updated
                nestedCalculationGroup.Children.Add(calculation2);
                nestedCalculationGroup.NotifyObservers();
                Assert.AreEqual(2, dataGridView.Rows.Count);

                // Add another, nested calculation without surface line and ensure the data grid view is updated when the surface line is set.
                var calculation3 = new MacroStabilityInwardsCalculationScenario();
                nestedCalculationGroup.Children.Add(calculation3);
                nestedCalculationGroup.NotifyObservers();
                Assert.AreEqual(2, dataGridView.Rows.Count);

                calculation3.InputParameters.SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(sl => sl.Name == "PK001_0001");
                calculation3.InputParameters.NotifyObservers();
                Assert.AreEqual(3, dataGridView.Rows.Count);

                // Change the name of the first calculation and ensure the data grid view is updated
                calculation1.Name = "New name";
                calculation1.NotifyObservers();
                Assert.AreEqual("New name", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

                // Add another calculation and assign all soil models
                var calculation4 = new MacroStabilityInwardsCalculationScenario();
                assessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(calculation4);
                assessmentSection.MacroStabilityInwards.CalculationsGroup.NotifyObservers();
                calculation4.InputParameters.SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(sl => sl.Name == "PK001_0001");
                calculation4.InputParameters.NotifyObservers();
                Assert.AreEqual(4, dataGridView.Rows.Count);

                listBox.SelectedItem = assessmentSection.MacroStabilityInwards.Sections.First(s => s.Name == "6-3_22");
                calculation1.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[0];
                calculation1.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[0].StochasticSoilProfiles.First();
                calculation1.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0001_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.MacroStabilityInwards.Sections.First(s => s.Name == "6-3_19");
                calculation2.InputParameters.SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(sl => sl.Name == "PK001_0002");
                calculation2.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[1];
                calculation2.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[1].StochasticSoilProfiles.First();
                calculation2.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0002_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_4_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.MacroStabilityInwards.Sections.First(s => s.Name == "6-3_16");
                calculation3.InputParameters.SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(sl => sl.Name == "PK001_0003");
                calculation3.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[2];
                calculation3.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[2].StochasticSoilProfiles.First();
                calculation3.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0003_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-7_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.MacroStabilityInwards.Sections.First(s => s.Name == "6-3_8");
                calculation4.InputParameters.SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(sl => sl.Name == "PK001_0004");
                calculation4.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[3];
                calculation4.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[3].StochasticSoilProfiles.First();
                calculation4.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0004_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-8_6_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                // Update stochastic soil models
                DataUpdateHelper.UpdateMacroStabilityInwardsStochasticSoilModels(assessmentSection);

                listBox.SelectedItem = assessmentSection.MacroStabilityInwards.Sections.First(s => s.Name == "6-3_22");
                Assert.AreEqual("PK001_0001_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.MacroStabilityInwards.Sections.First(s => s.Name == "6-3_19");
                Assert.AreEqual("PK001_0002_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_4_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.MacroStabilityInwards.Sections.First(s => s.Name == "6-3_16");
                Assert.AreEqual("PK001_0003_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-7_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                listBox.SelectedItem = assessmentSection.MacroStabilityInwards.Sections.First(s => s.Name == "6-3_8");
                Assert.AreEqual("PK001_0004_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-8_6_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);
            }
        }

        private static string GetFormattedProbabilityValue(double value)
        {
            return new RoundedDouble(2, value).ToString();
        }
    }
}