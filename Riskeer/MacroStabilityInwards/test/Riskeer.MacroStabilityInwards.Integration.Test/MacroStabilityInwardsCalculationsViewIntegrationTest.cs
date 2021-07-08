// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Integration.Data;
using Riskeer.Integration.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.Views;

namespace Riskeer.MacroStabilityInwards.Integration.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationsViewIntegrationTest
    {
        private const int nameColumnIndex = 0;
        private const int hydraulicBoundaryLocationsColumnIndex = 1;
        private const int stochasticSoilModelsColumnIndex = 2;
        private const int stochasticSoilProfilesColumnIndex = 3;
        private const int stochasticSoilProfilesProbabilityColumnIndex = 4;

        [Test]
        public void MacroStabilityInwardsCalculationsView_DataImportedOrChanged_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;

                // Show the view
                var calculationsView = new MacroStabilityInwardsCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);
                form.Controls.Add(calculationsView);
                form.Show();

                // Obtain some relevant controls
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Import failure mechanism sections
                DataImportHelper.ImportReferenceLine(assessmentSection);
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);

                // Import surface lines
                DataImportHelper.ImportMacroStabilityInwardsSurfaceLines(assessmentSection);

                // Setup some calculations
                var calculation1 = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };
                var calculation2 = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };

                // Add a calculation and ensure it is shown in the data grid view
                failureMechanism.CalculationsGroup.Children.Add(calculation1);
                failureMechanism.CalculationsGroup.NotifyObservers();
                Assert.AreEqual(1, dataGridView.Rows.Count);

                // Import soil models and profiles and ensure the corresponding combobox items are updated
                DataImportHelper.ImportMacroStabilityInwardsStochasticSoilModels(assessmentSection);
                MacroStabilityInwardsStochasticSoilModelCollection stochasticSoilModelCollection = failureMechanism.StochasticSoilModels;
                calculation1.InputParameters.StochasticSoilModel = stochasticSoilModelCollection.First(sl => sl.Name == "PK001_0001_Stability");
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex]).Items.Count);
                Assert.AreEqual("PK001_0001_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual(1, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex]).Items.Count);
                Assert.AreEqual("<selecteer>", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);

                // Import hydraulic boundary locations and ensure the corresponding combobox items are updated
                DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection);
                assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();
                Assert.AreEqual(19, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[hydraulicBoundaryLocationsColumnIndex]).Items.Count);

                // Add group and ensure the data grid view is not changed
                var nestedCalculationGroup = new CalculationGroup();
                failureMechanism.CalculationsGroup.Children.Add(nestedCalculationGroup);
                failureMechanism.CalculationsGroup.NotifyObservers();
                Assert.AreEqual(1, dataGridView.Rows.Count);

                // Add another, nested calculation and ensure the data grid view is updated
                nestedCalculationGroup.Children.Add(calculation2);
                nestedCalculationGroup.NotifyObservers();
                Assert.AreEqual(2, dataGridView.Rows.Count);

                // Change the name of the first calculation and ensure the data grid view is updated
                calculation1.Name = "New name";
                calculation1.NotifyObservers();
                Assert.AreEqual("New name", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

                // Add another calculation and assign all soil models
                var calculation3 = new MacroStabilityInwardsCalculationScenario();
                failureMechanism.CalculationsGroup.Children.Add(calculation3);
                failureMechanism.CalculationsGroup.NotifyObservers();
                calculation3.InputParameters.SurfaceLine = failureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001");
                calculation3.InputParameters.NotifyObservers();
                Assert.AreEqual(3, dataGridView.Rows.Count);

                calculation1.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[0];
                calculation1.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[0].StochasticSoilProfiles.First();
                calculation1.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0001_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                calculation2.InputParameters.SurfaceLine = failureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0002");
                calculation2.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[1];
                calculation2.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[1].StochasticSoilProfiles.First();
                calculation2.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0002_Stability", dataGridView.Rows[1].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_4_1D1", dataGridView.Rows[1].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[1].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                calculation3.InputParameters.SurfaceLine = failureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0003");
                calculation3.InputParameters.StochasticSoilModel = stochasticSoilModelCollection[2];
                calculation3.InputParameters.StochasticSoilProfile = stochasticSoilModelCollection[2].StochasticSoilProfiles.First();
                calculation3.InputParameters.NotifyObservers();
                Assert.AreEqual("PK001_0003_Stability", dataGridView.Rows[2].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-7_0_1D1", dataGridView.Rows[2].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[2].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                // Update stochastic soil models
                DataUpdateHelper.UpdateMacroStabilityInwardsStochasticSoilModels(assessmentSection);

                Assert.AreEqual("PK001_0001_Stability", dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_0_1D1", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[0].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                Assert.AreEqual("PK001_0002_Stability", dataGridView.Rows[1].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-6_4_1D1", dataGridView.Rows[1].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[1].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

                Assert.AreEqual("PK001_0003_Stability", dataGridView.Rows[2].Cells[stochasticSoilModelsColumnIndex].FormattedValue);
                Assert.AreEqual("W1-7_0_1D1", dataGridView.Rows[2].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);
                Assert.AreEqual(GetFormattedProbabilityValue(100), dataGridView.Rows[2].Cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);
            }
        }

        private static string GetFormattedProbabilityValue(double value)
        {
            return new RoundedDouble(2, value).ToString();
        }
    }
}