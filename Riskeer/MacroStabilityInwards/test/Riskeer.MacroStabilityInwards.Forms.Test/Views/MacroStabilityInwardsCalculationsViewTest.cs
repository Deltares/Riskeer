﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.Views;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationsViewTest : NUnitFormTest
    {
        private const int nameColumnIndex = 0;
        private const int selectableHydraulicBoundaryLocationsColumnIndex = 1;
        private const int stochasticSoilModelsColumnIndex = 2;
        private const int stochasticSoilProfilesColumnIndex = 3;
        private const int stochasticSoilProfilesProbabilityColumnIndex = 4;
        private Form testForm;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            MacroStabilityInwardsCalculationsView view = ShowMacroStabilityInwardsCalculationsView(new CalculationGroup(), new MacroStabilityInwardsFailureMechanism(), new AssessmentSectionStub());

            // Assert
            Assert.IsInstanceOf<CalculationsView<MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsInput, MacroStabilityInwardsCalculationRow, MacroStabilityInwardsFailureMechanism>>(view);

            var button = (Button) new ControlTester("generateButton").TheObject;
            Assert.AreEqual("Genereer &scenario's...", button.Text);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowMacroStabilityInwardsCalculationsView(new CalculationGroup(), new MacroStabilityInwardsFailureMechanism(), new AssessmentSectionStub());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);
            Assert.AreEqual(5, dataGridView.ColumnCount);

            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Hydraulische belastingenlocatie", dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex].HeaderText);
            Assert.AreEqual("Stochastisch ondergrondmodel", dataGridView.Columns[stochasticSoilModelsColumnIndex].HeaderText);
            Assert.AreEqual("Ondergrondschematisatie", dataGridView.Columns[stochasticSoilProfilesColumnIndex].HeaderText);
            Assert.AreEqual("Aandeel van schematisatie\r\nin het stochastische ondergrondmodel\r\n[%]", dataGridView.Columns[stochasticSoilProfilesProbabilityColumnIndex].HeaderText);

            foreach (DataGridViewComboBoxColumn column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
            {
                Assert.AreEqual("This", column.ValueMember);
                Assert.AreEqual("DisplayName", column.DisplayMember);
            }

            var soilProfilesCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[stochasticSoilProfilesColumnIndex];
            DataGridViewComboBoxCell.ObjectCollection soilProfilesComboboxItems = soilProfilesCombobox.Items;
            Assert.AreEqual(1, soilProfilesComboboxItems.Count); // Row dependent

            var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex];
            DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
            Assert.AreEqual(1, hydraulicBoundaryLocationComboboxItems.Count); // Row dependent
        }

        [Test]
        public void CalculationsView_FailureMechanismWithCorrespondingStochasticSoilModels_StochasticSoilModelsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowMacroStabilityInwardsCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
                                                      failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewComboBoxCell.ObjectCollection stochasticSoilModelsComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex]).Items;
            Assert.AreEqual(2, stochasticSoilModelsComboboxItems.Count);
            Assert.AreEqual("<selecteer>", stochasticSoilModelsComboboxItems[0].ToString());
            Assert.AreEqual("Model A", stochasticSoilModelsComboboxItems[1].ToString());

            stochasticSoilModelsComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[1].Cells[stochasticSoilModelsColumnIndex]).Items;
            Assert.AreEqual(3, stochasticSoilModelsComboboxItems.Count);
            Assert.AreEqual("<selecteer>", stochasticSoilModelsComboboxItems[0].ToString());
            Assert.AreEqual("Model A", stochasticSoilModelsComboboxItems[1].ToString());
            Assert.AreEqual("Model E", stochasticSoilModelsComboboxItems[2].ToString());

            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsView_FailureMechanismWithCorrespondingSoilProfiles_SoilProfilesComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowMacroStabilityInwardsCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
                                                      failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewComboBoxCell.ObjectCollection soilProfilesComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex]).Items;
            Assert.AreEqual(3, soilProfilesComboboxItems.Count);
            Assert.AreEqual("<selecteer>", soilProfilesComboboxItems[0].ToString());
            Assert.AreEqual("Profile 1", soilProfilesComboboxItems[1].ToString());
            Assert.AreEqual("Profile 2", soilProfilesComboboxItems[2].ToString());

            soilProfilesComboboxItems = ((DataGridViewComboBoxCell) dataGridView.Rows[1].Cells[stochasticSoilProfilesColumnIndex]).Items;
            Assert.AreEqual(2, soilProfilesComboboxItems.Count);
            Assert.AreEqual("<selecteer>", soilProfilesComboboxItems[0].ToString());
            Assert.AreEqual("Profile 5", soilProfilesComboboxItems[1].ToString());

            mocks.VerifyAll();
        }

        [Test]
        public void CalculationsView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowMacroStabilityInwardsCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
                                                      failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 1 (2 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("Model A", cells[stochasticSoilModelsColumnIndex].FormattedValue);
            Assert.AreEqual("<selecteer>", cells[stochasticSoilProfilesColumnIndex].FormattedValue);
            Assert.AreEqual(GetFormattedProbabilityValue(0), cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 2 (5 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("Model E", cells[stochasticSoilModelsColumnIndex].FormattedValue);
            Assert.AreEqual("Profile 5", cells[stochasticSoilProfilesColumnIndex].FormattedValue);
            Assert.AreEqual(GetFormattedProbabilityValue(30), cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);

            mocks.VerifyAll();
        }

        [Test]
        public void ButtonGenerateScenarios_WithoutSurfaceLines_ButtonDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            }, "path");

            ShowMacroStabilityInwardsCalculationsView(new CalculationGroup(), failureMechanism, assessmentSection);

            var button = (Button) new ControlTester("generateButton").TheObject;

            // Call
            bool state = button.Enabled;

            // Assert
            Assert.IsFalse(state);
            mocks.VerifyAll();
        }

        [Test]
        public void ButtonGenerateScenarios_WithoutSoilModels_ButtonDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine(string.Empty)
            }, "path");

            ShowMacroStabilityInwardsCalculationsView(new CalculationGroup(), failureMechanism, assessmentSection);

            var button = (Button) new ControlTester("generateButton").TheObject;

            // Call
            bool state = button.Enabled;

            // Assert
            Assert.IsFalse(state);
            mocks.VerifyAll();
        }

        [Test]
        public void ButtonGenerateScenarios_WithSurfaceLinesAndSoilModels_ButtonEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            const string arbitrarySourcePath = "path";
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine(string.Empty)
            }, arbitrarySourcePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            }, arbitrarySourcePath);

            ShowMacroStabilityInwardsCalculationsView(new CalculationGroup(), failureMechanism, assessmentSection);

            var button = (Button) new ControlTester("generateButton").TheObject;

            // Call
            bool state = button.Enabled;

            // Assert
            Assert.IsTrue(state);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSoilModelAndNotify_ThenButtonDisabled()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ShowMacroStabilityInwardsCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            // When
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            }, "path");
            failureMechanism.NotifyObservers();

            // Then
            var button = (Button) new ControlTester("generateButton").TheObject;
            Assert.IsFalse(button.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSurfaceLineAndNotify_ThenButtonDisabled()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ShowMacroStabilityInwardsCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            // When
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine(string.Empty)
            }, "path");
            failureMechanism.NotifyObservers();

            // Then
            var button = (Button) new ControlTester("generateButton").TheObject;
            Assert.IsFalse(button.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsView_WhenStochasticSoilModelsUpdatedAndNotified_ThenStochasticSoilModelsAndStochasticSoilProfilesComboboxCorrectlyUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            ShowMacroStabilityInwardsCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism), failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var soilModelsComboBox = (DataGridViewComboBoxColumn) dataGridView.Columns[stochasticSoilModelsColumnIndex];
            var soilProfilesComboBox = (DataGridViewComboBoxColumn) dataGridView.Columns[stochasticSoilProfilesColumnIndex];

            // Precondition
            Assert.AreEqual(4, soilModelsComboBox.Items.Count);
            Assert.AreEqual(6, soilProfilesComboBox.Items.Count);

            // When
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("Model F", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            }, string.Empty);
            failureMechanism.StochasticSoilModels.NotifyObservers();

            // Then
            DataGridViewComboBoxCell.ObjectCollection soilModelItems = soilModelsComboBox.Items;
            DataGridViewComboBoxCell.ObjectCollection soilProfileItems = soilProfilesComboBox.Items;
            Assert.AreEqual(5, soilModelItems.Count);
            Assert.AreEqual("<selecteer>", soilModelItems[0].ToString());
            Assert.AreEqual("Model A", soilModelItems[1].ToString());
            Assert.AreEqual("Model C", soilModelItems[2].ToString());
            Assert.AreEqual("Model E", soilModelItems[3].ToString());
            Assert.AreEqual("Model F", soilModelItems[4].ToString());

            Assert.AreEqual(8, soilProfileItems.Count);
            Assert.AreEqual("<selecteer>", soilProfileItems[0].ToString());
            Assert.AreEqual("Profile 1", soilProfileItems[1].ToString());
            Assert.AreEqual("Profile 2", soilProfileItems[2].ToString());
            Assert.AreEqual("Profile 3", soilProfileItems[3].ToString());
            Assert.AreEqual("Profile 4", soilProfileItems[4].ToString());
            Assert.AreEqual("Profile 5", soilProfileItems[5].ToString());
            Assert.AreEqual("A", soilProfileItems[6].ToString());

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Selection_Always_ReturnsTheSelectedRowObject(int selectedRow)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();

            MacroStabilityInwardsCalculationsView view = ShowMacroStabilityInwardsCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
                                                                                                   failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            dataGridView.CurrentCell = dataGridView.Rows[selectedRow].Cells[0];

            // Call
            object selection = view.Selection;

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsInputContext>(selection);
            var dataRow = (MacroStabilityInwardsCalculationRow) dataGridView.Rows[selectedRow].DataBoundItem;
            Assert.AreSame(dataRow.Calculation, ((MacroStabilityInwardsInputContext) selection).MacroStabilityInwardsCalculation);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(stochasticSoilProfilesColumnIndex, null, true)]
        [TestCase(stochasticSoilProfilesColumnIndex, null, false)]
        [TestCase(selectableHydraulicBoundaryLocationsColumnIndex, null, true)]
        [TestCase(selectableHydraulicBoundaryLocationsColumnIndex, null, false)]
        public void CalculationsView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(
            int cellIndex,
            object newValue,
            bool useCalculationWithOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();

            if (useCalculationWithOutput)
            {
                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new MessageBoxTester(wnd);
                    tester.ClickOk();
                };

                calculationObserver.Expect(o => o.UpdateObserver());
            }

            calculationInputObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            ShowMacroStabilityInwardsCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[1];

            if (useCalculationWithOutput)
            {
                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
            }

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[1].Cells[cellIndex].Value = newValue is double value ? (RoundedDouble) value : newValue;

            // Assert
            Assert.IsNull(calculation.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsViewWithStochasticSoilProfile_WhenProbabilityChangesAndNotified_ThenNewProbabilityVisible()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            ShowMacroStabilityInwardsCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[1];

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            var refreshed = 0;

            // Precondition
            var currentCell = (DataGridViewTextBoxCell) dataGridView.Rows[1].Cells[stochasticSoilProfilesProbabilityColumnIndex];
            Assert.AreEqual(GetFormattedProbabilityValue(30), currentCell.FormattedValue);

            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfileToChange = calculation.InputParameters.StochasticSoilProfile;
            var updatedProfile = new MacroStabilityInwardsStochasticSoilProfile(0.5, stochasticSoilProfileToChange.SoilProfile);
            dataGridView.Invalidated += (sender, args) => refreshed++;

            // When
            stochasticSoilProfileToChange.Update(updatedProfile);
            stochasticSoilProfileToChange.NotifyObservers();

            // Then
            Assert.AreEqual(1, refreshed);
            var cell = (DataGridViewTextBoxCell) dataGridView.Rows[1].Cells[stochasticSoilProfilesProbabilityColumnIndex];
            Assert.AreEqual(GetFormattedProbabilityValue(50), cell.FormattedValue);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsView_WhenGenerateScenariosButtonClicked_ThenShowViewWithSurfaceLines()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            const string arbitraryFilePath = "path";
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine("Line A"),
                new MacroStabilityInwardsSurfaceLine("Line B")
            }, arbitraryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            }, arbitraryFilePath);

            ShowMacroStabilityInwardsCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var button = new ButtonTester("generateButton", testForm);

            MacroStabilityInwardsSurfaceLineSelectionDialog selectionDialog = null;
            DataGridViewControl grid = null;
            var rows = 0;
            DialogBoxHandler = (name, wnd) =>
            {
                selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                rows = grid.Rows.Count;

                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };

            // When
            button.Click();

            // Then
            Assert.NotNull(selectionDialog);
            Assert.NotNull(grid);
            Assert.AreEqual(failureMechanism.SurfaceLines.Count, rows);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenDialogClosed_ThenNotifyCalculationGroup()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            const string arbitraryFilePath = "path";
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new MacroStabilityInwardsSurfaceLine("Line A"),
                new MacroStabilityInwardsSurfaceLine("Line B")
            }, arbitraryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            }, arbitraryFilePath);
            failureMechanism.CalculationsGroup.Attach(observer);

            ShowMacroStabilityInwardsCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var button = new ButtonTester("generateButton", testForm);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                new ButtonTester("DoForSelectedButton", selectionDialog).Click();
            };

            button.Click();

            // Then
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenSurfaceLineSelectedAndDialogClosed_ThenUpdateSectionResultScenarios()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureSimpleFailureMechanism();

            ShowMacroStabilityInwardsCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var button = new ButtonTester("generateButton", testForm);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                new ButtonTester("DoForSelectedButton", selectionDialog).Click();
            };

            button.Click();

            // Then
            MacroStabilityInwardsCalculationScenario[] calculationScenarios = failureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>().ToArray();
            AdoptableWithProfileProbabilityFailureMechanismSectionResult failureMechanismSectionResult1 = failureMechanism.SectionResults.First();
            AdoptableWithProfileProbabilityFailureMechanismSectionResult failureMechanismSectionResult2 = failureMechanism.SectionResults.ElementAt(1);

            Func<MacroStabilityInwardsCalculationScenario, IEnumerable<Segment2D>, bool> intersectionFunc =
                (scenario, lineSegments) => scenario.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments);

            Assert.AreEqual(2, failureMechanismSectionResult1.GetRelevantCalculationScenarios(calculationScenarios, intersectionFunc).Count());
            CollectionAssert.IsEmpty(failureMechanismSectionResult2.GetRelevantCalculationScenarios(calculationScenarios, intersectionFunc));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosCancelButtonClicked_WhenDialogClosed_CalculationsNotUpdatedAndCalculationGroupNotNotified()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureSimpleFailureMechanism();
            ShowMacroStabilityInwardsCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            failureMechanism.CalculationsGroup.Attach(observer);

            var button = new ButtonTester("generateButton", testForm);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };

            button.Click();

            // Then
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            mocks.VerifyAll(); // No observer notified
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CalculationsViewWithHydraulicLocation_SpecificUseAssessmentLevelManualInputState_SelectableHydraulicLocationReadonlyAccordingly(bool useAssessmentLevelManualInput)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryData(assessmentSection);
            mocks.ReplayAll();

            MacroStabilityInwardsFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            ShowMacroStabilityInwardsCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (MacroStabilityInwardsCalculationScenario) calculationGroup.Children.First();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            calculation.InputParameters.UseAssessmentLevelManualInput = useAssessmentLevelManualInput;
            calculation.InputParameters.NotifyObservers();

            // Assert
            Assert.IsFalse(dataGridView.Rows[0].ReadOnly);

            var currentCellUpdated = (DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[selectableHydraulicBoundaryLocationsColumnIndex];
            Assert.AreEqual(useAssessmentLevelManualInput, currentCellUpdated.ReadOnly);

            mocks.VerifyAll();
        }

        public override void Setup()
        {
            base.Setup();

            testForm = new Form();
        }

        public override void TearDown()
        {
            base.TearDown();

            testForm.Dispose();
        }

        private static void ConfigureHydraulicBoundaryData(IAssessmentSection assessmentSection)
        {
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(new HydraulicBoundaryData
            {
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        Locations =
                        {
                            new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2),
                            new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4)
                        }
                    }
                }
            });
        }

        private static MacroStabilityInwardsFailureMechanism ConfigureSimpleFailureMechanism()
        {
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine("Surface line 1")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine("Surface line 2")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            const string sourcePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, sourcePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("name", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            }, sourcePath);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                }),
                new FailureMechanismSection("Section 2", new[]
                {
                    new Point2D(5.0, 0.0),
                    new Point2D(10.0, 0.0)
                })
            });

            return failureMechanism;
        }

        private static CalculationGroup ConfigureCalculationGroup(IAssessmentSection assessmentSection, MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModelForCalculation2 = failureMechanism.StochasticSoilModels.Last();
            return new CalculationGroup
            {
                Children =
                {
                    new MacroStabilityInwardsCalculationScenario
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            SurfaceLine = failureMechanism.SurfaceLines.First(),
                            StochasticSoilModel = failureMechanism.StochasticSoilModels.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First()
                        }
                    },
                    new MacroStabilityInwardsCalculationScenario
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            SurfaceLine = failureMechanism.SurfaceLines.Last(),
                            StochasticSoilModel = stochasticSoilModelForCalculation2,
                            StochasticSoilProfile = stochasticSoilModelForCalculation2.StochasticSoilProfiles.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().Last()
                        }
                    }
                }
            };
        }

        private static MacroStabilityInwardsFailureMechanism ConfigureFailureMechanism()
        {
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine("Surface line 1")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine("Surface line 2")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            const string arbitraryFilePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, arbitraryFilePath);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                }),
                new FailureMechanismSection("Section 2", new[]
                {
                    new Point2D(5.0, 0.0),
                    new Point2D(10.0, 0.0)
                })
            });

            var stochasticSoilProfile1 = new MacroStabilityInwardsStochasticSoilProfile(0.3, new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            }));

            var stochasticSoilModelA = new MacroStabilityInwardsStochasticSoilModel("Model A", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                stochasticSoilProfile1,
                new MacroStabilityInwardsStochasticSoilProfile(0.7, new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(-4.0),
                    new MacroStabilityInwardsSoilLayer1D(0.0),
                    new MacroStabilityInwardsSoilLayer1D(4.0)
                }))
            });

            var stochasticSoilProfile5 = new MacroStabilityInwardsStochasticSoilProfile(0.3, new MacroStabilityInwardsSoilProfile1D("Profile 5", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            }));

            var stochasticSoilModelE = new MacroStabilityInwardsStochasticSoilModel("Model E", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(6.0, 0.0)
            }, new[]
            {
                stochasticSoilProfile5
            });

            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModelA,
                new MacroStabilityInwardsStochasticSoilModel("Model C", new[]
                {
                    new Point2D(1.0, 0.0),
                    new Point2D(4.0, 0.0)
                }, new[]
                {
                    new MacroStabilityInwardsStochasticSoilProfile(0.3, new MacroStabilityInwardsSoilProfile1D("Profile 3", -10.0, new[]
                    {
                        new MacroStabilityInwardsSoilLayer1D(-5.0),
                        new MacroStabilityInwardsSoilLayer1D(-2.0),
                        new MacroStabilityInwardsSoilLayer1D(1.0)
                    })),
                    new MacroStabilityInwardsStochasticSoilProfile(0.7, new MacroStabilityInwardsSoilProfile1D("Profile 4", -8.0, new[]
                    {
                        new MacroStabilityInwardsSoilLayer1D(-4.0),
                        new MacroStabilityInwardsSoilLayer1D(0.0),
                        new MacroStabilityInwardsSoilLayer1D(4.0)
                    }))
                }),
                stochasticSoilModelE
            }, arbitraryFilePath);
            return failureMechanism;
        }

        private MacroStabilityInwardsCalculationsView ShowMacroStabilityInwardsCalculationsView(CalculationGroup calculationGroup, MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                                                IAssessmentSection assessmentSection)
        {
            var view = new MacroStabilityInwardsCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static string GetFormattedProbabilityValue(double value)
        {
            return new RoundedDouble(2, value).ToString();
        }
    }
}