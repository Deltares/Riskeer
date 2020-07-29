// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Globalization;
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
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingCalculationsViewTest : NUnitFormTest
    {
        private const int nameColumnIndex = 0;
        private const int selectableHydraulicBoundaryLocationsColumnIndex = 1;
        private const int stochasticSoilModelsColumnIndex = 2;
        private const int stochasticSoilProfilesColumnIndex = 3;
        private const int stochasticSoilProfilesProbabilityColumnIndex = 4;
        private const int dampingFactorExitMeanColumnIndex = 5;
        private const int phreaticLevelExitMeanColumnIndex = 6;
        private const int entryPointLColumnIndex = 7;
        private const int exitPointLColumnIndex = 8;
        private Form testForm;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            PipingCalculationsView view = ShowPipingCalculationsView(new CalculationGroup(), new PipingFailureMechanism(), new AssessmentSectionStub());

            // Assert
            Assert.IsInstanceOf<CalculationsView<PipingCalculationScenario, PipingInput, PipingCalculationRow, PipingFailureMechanism>>(view);

            var button = (Button) new ControlTester("generateButton").TheObject;
            Assert.AreEqual("Genereer &scenario's...", button.Text);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowPipingCalculationsView(new CalculationGroup(), new PipingFailureMechanism(), new AssessmentSectionStub());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);
            Assert.AreEqual(9, dataGridView.ColumnCount);

            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Hydraulische belastingenlocatie", dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex].HeaderText);
            Assert.AreEqual("Stochastisch ondergrondmodel", dataGridView.Columns[stochasticSoilModelsColumnIndex].HeaderText);
            Assert.AreEqual("Ondergrondschematisatie", dataGridView.Columns[stochasticSoilProfilesColumnIndex].HeaderText);
            Assert.AreEqual("Aandeel van schematisatie\r\nin het stochastische ondergrondmodel\r\n[%]", dataGridView.Columns[stochasticSoilProfilesProbabilityColumnIndex].HeaderText);
            Assert.AreEqual("Verwachtingswaarde\r\ndempingsfactor bij uittredepunt\r\n[-]", dataGridView.Columns[dampingFactorExitMeanColumnIndex].HeaderText);
            Assert.AreEqual("Verwachtingswaarde\r\npolderpeil\r\n[m+NAP]", dataGridView.Columns[phreaticLevelExitMeanColumnIndex].HeaderText);
            Assert.AreEqual("Intredepunt", dataGridView.Columns[entryPointLColumnIndex].HeaderText);
            Assert.AreEqual("Uittredepunt", dataGridView.Columns[exitPointLColumnIndex].HeaderText);

            foreach (DataGridViewComboBoxColumn column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
            {
                Assert.AreEqual("This", column.ValueMember);
                Assert.AreEqual("DisplayName", column.DisplayMember);
            }

            var soilProfilesCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[stochasticSoilProfilesColumnIndex];
            DataGridViewComboBoxCell.ObjectCollection soilProfilesComboboxItems = soilProfilesCombobox.Items;
            Assert.AreEqual(0, soilProfilesComboboxItems.Count); // Row dependent

            var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex];
            DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
            Assert.AreEqual(0, hydraulicBoundaryLocationComboboxItems.Count); // Row dependent
        }

        [Test]
        public void PipingCalculationsView_CalculationsWithCorrespondingStochasticSoilModel_StochasticSoilModelsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowPipingCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
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
        public void PipingCalculationsView_CalculationsWithCorrespondingSoilProfiles_SoilProfilesComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowPipingCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
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
        public void PipingCalculationsView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowPipingCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
                                       failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(9, cells.Count);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 1 (2 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("Model A", cells[stochasticSoilModelsColumnIndex].FormattedValue);
            Assert.AreEqual("<selecteer>", cells[stochasticSoilProfilesColumnIndex].FormattedValue);
            Assert.AreEqual(GetFormattedProbabilityValue(0), cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual(1.111.ToString(CultureInfo.CurrentCulture), cells[dampingFactorExitMeanColumnIndex].FormattedValue);
            Assert.AreEqual(2.222.ToString(CultureInfo.CurrentCulture), cells[phreaticLevelExitMeanColumnIndex].FormattedValue);
            Assert.AreEqual(3.33.ToString(CultureInfo.CurrentCulture), cells[entryPointLColumnIndex].FormattedValue);
            Assert.AreEqual(4.44.ToString(CultureInfo.CurrentCulture), cells[exitPointLColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(9, cells.Count);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 2 (5 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            Assert.AreEqual("Model E", cells[stochasticSoilModelsColumnIndex].FormattedValue);
            Assert.AreEqual("Profile 5", cells[stochasticSoilProfilesColumnIndex].FormattedValue);
            Assert.AreEqual(GetFormattedProbabilityValue(30), cells[stochasticSoilProfilesProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual(5.556.ToString(CultureInfo.CurrentCulture), cells[dampingFactorExitMeanColumnIndex].FormattedValue);
            Assert.AreEqual(6.667.ToString(CultureInfo.CurrentCulture), cells[phreaticLevelExitMeanColumnIndex].FormattedValue);
            Assert.AreEqual(7.78.ToString(CultureInfo.CurrentCulture), cells[entryPointLColumnIndex].FormattedValue);
            Assert.AreEqual(8.89.ToString(CultureInfo.CurrentCulture), cells[exitPointLColumnIndex].FormattedValue);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase("test", dampingFactorExitMeanColumnIndex)]
        [TestCase("test", phreaticLevelExitMeanColumnIndex)]
        [TestCase("test", entryPointLColumnIndex)]
        [TestCase("test", exitPointLColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", dampingFactorExitMeanColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", phreaticLevelExitMeanColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", entryPointLColumnIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", exitPointLColumnIndex)]
        public void PipingCalculationsView_EditValueInvalid_ShowsErrorTooltip(string newValue, int cellIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();

            ShowPipingCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
                                       failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = newValue;

            // Assert
            Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1, dampingFactorExitMeanColumnIndex)]
        [TestCase(1e-2, dampingFactorExitMeanColumnIndex)]
        [TestCase(1e+6, dampingFactorExitMeanColumnIndex)]
        [TestCase(14.3, dampingFactorExitMeanColumnIndex)]
        [TestCase(1, phreaticLevelExitMeanColumnIndex)]
        [TestCase(1e-6, phreaticLevelExitMeanColumnIndex)]
        [TestCase(1e+6, phreaticLevelExitMeanColumnIndex)]
        [TestCase(14.3, phreaticLevelExitMeanColumnIndex)]
        [TestCase(2.2, entryPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(2.2)")]
        [TestCase(0.022e+2, entryPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(0.022e+2)")]
        [TestCase(220e-2, entryPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(220e-2)")]
        [TestCase(5.5, exitPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(5.5)")]
        [TestCase(0.055e+2, exitPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(0.055e+2)")]
        [TestCase(550e-2, exitPointLColumnIndex, TestName = "FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(550e-2)")]
        public void FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(double newValue, int cellIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();

            // Call
            ShowPipingCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
                                       failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.IsEmpty(dataGridView.Rows[0].ErrorText);

            mocks.VerifyAll();
        }

        [Test]
        public void ButtonGenerateScenarios_WithoutSurfaceLines_ButtonDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            }, "path");

            ShowPipingCalculationsView(new CalculationGroup(), failureMechanism, assessmentSection);

            var button = (Button)new ControlTester("generateButton").TheObject;

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
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new PipingSurfaceLine(string.Empty)
            }, "path");

            ShowPipingCalculationsView(new CalculationGroup(), failureMechanism, assessmentSection);

            var button = (Button)new ControlTester("generateButton").TheObject;

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
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            const string arbitrarySourcePath = "path";
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new PipingSurfaceLine(string.Empty)
            }, arbitrarySourcePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            }, arbitrarySourcePath);

            ShowPipingCalculationsView(new CalculationGroup(), failureMechanism, assessmentSection);

            var button = (Button) new ControlTester("generateButton").TheObject;

            // Call
            bool state = button.Enabled;

            // Assert
            Assert.IsTrue(state);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationsView_WhenGenerateScenariosButtonClicked_ThenShowViewWithSurfaceLines()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            const string arbitraryFilePath = "path";
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new PipingSurfaceLine("Line A"),
                new PipingSurfaceLine("Line B")
            }, arbitraryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            }, arbitraryFilePath);

            ShowPipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var button = new ButtonTester("generateButton", testForm);

            PipingSurfaceLineSelectionDialog selectionDialog = null;
            DataGridViewControl grid = null;
            DialogBoxHandler = (name, wnd) =>
            {
                selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };

            // When
            button.Click();

            // Then
            Assert.NotNull(selectionDialog);
            Assert.NotNull(grid);
            Assert.AreEqual(2, grid.Rows.Count);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("DoForSelectedButton")]
        [TestCase("CustomCancelButton")]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenDialogClosed_ThenNotifyCalculationGroup(string buttonName)
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            const string arbitraryFilePath = "path";
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new PipingSurfaceLine("Line A"),
                new PipingSurfaceLine("Line B")
            }, arbitraryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            }, arbitraryFilePath);
            failureMechanism.CalculationsGroup.Attach(observer);

            ShowPipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var button = new ButtonTester("generateButton", testForm);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                new ButtonTester(buttonName, selectionDialog).Click();
            };

            button.Click();

            // Then
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenSurfaceLineSelectedAndDialogClosed_ThenUpdateSectionResultScenarios()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureSimpleFailureMechanism();

            ShowPipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var button = new ButtonTester("generateButton", testForm);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                new ButtonTester("DoForSelectedButton", selectionDialog).Click();
            };

            button.Click();

            // Then
            PipingCalculationScenario[] pipingCalculationScenarios = failureMechanism.Calculations.OfType<PipingCalculationScenario>().ToArray();
            PipingFailureMechanismSectionResult failureMechanismSectionResult1 = failureMechanism.SectionResults.First();
            PipingFailureMechanismSectionResult failureMechanismSectionResult2 = failureMechanism.SectionResults.ElementAt(1);

            Assert.AreEqual(2, failureMechanismSectionResult1.GetCalculationScenarios(pipingCalculationScenarios).Count());

            foreach (PipingCalculationScenario calculationScenario in failureMechanismSectionResult1.GetCalculationScenarios(pipingCalculationScenarios))
            {
                Assert.IsInstanceOf<ICalculationScenario>(calculationScenario);
            }

            CollectionAssert.IsEmpty(failureMechanismSectionResult2.GetCalculationScenarios(pipingCalculationScenarios));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosCancelButtonClicked_WhenDialogClosed_CalculationsNotUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureSimpleFailureMechanism();
            ShowPipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var button = new ButtonTester("generateButton", testForm);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };

            button.Click();

            // Then
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSoilModelAndNotify_ThenButtonDisabled()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            ShowPipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            // When
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            }, "path");
            failureMechanism.NotifyObservers();

            // Then
            var button = (Button)new ControlTester("generateButton").TheObject;
            Assert.IsFalse(button.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSurfaceLineAndNotify_ThenButtonDisabled()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            ShowPipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            // When
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new PipingSurfaceLine(string.Empty)
            }, "path");
            failureMechanism.NotifyObservers();

            // Then
            var button = (Button)new ControlTester("generateButton").TheObject;
            Assert.IsFalse(button.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismWithoutSurfaceLinesAndSoilModels_WhenAddSurfaceLineAndSoilModelAndDoNotNotifyObservers_ThenButtonDisabled()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            ShowPipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            // When
            const string arbitraryFilePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                new PipingSurfaceLine(string.Empty)
            }, arbitraryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
            }, arbitraryFilePath);

            // Then
            var button = (Button)new ControlTester("generateButton").TheObject;
            Assert.IsFalse(button.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(entryPointLColumnIndex, 6.6)]
        [TestCase(entryPointLColumnIndex, 4.44)]
        [TestCase(exitPointLColumnIndex, 2.22)]
        [TestCase(exitPointLColumnIndex, 1.1)]
        public void PipingCalculationsView_InvalidEntryOrExitPoint_ShowsErrorTooltip(int cellIndex, double newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            ShowPipingCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (PipingCalculationScenario) calculationGroup.Children.First();

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual("Het uittredepunt moet landwaarts van het intredepunt liggen.", dataGridView.Rows[0].ErrorText);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(entryPointLColumnIndex, -0.1)]
        [TestCase(entryPointLColumnIndex, -1.0)]
        [TestCase(exitPointLColumnIndex, 10.1)]
        [TestCase(exitPointLColumnIndex, 11.0)]
        public void PipingCalculationsView_EntryOrExitPointNotOnSurfaceLine_ShowsErrorToolTip(int cellIndex, double newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var pipingCalculationObserver = mocks.StrictMock<IObserver>();
            var pipingCalculationInputObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            ShowPipingCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (PipingCalculationScenario) calculationGroup.Children.First();

            calculation.Attach(pipingCalculationObserver);
            calculation.InputParameters.Attach(pipingCalculationInputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = (RoundedDouble) newValue;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 10,0]).";
            Assert.AreEqual(expectedMessage, dataGridView.Rows[0].ErrorText);

            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Selection_Always_ReturnsTheSelectedRowObject(int selectedRow)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();

            PipingCalculationsView view = ShowPipingCalculationsView(ConfigureCalculationGroup(assessmentSection, failureMechanism),
                                                                     failureMechanism, assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            dataGridView.CurrentCell = dataGridView.Rows[selectedRow].Cells[0];

            // Call
            object selection = view.Selection;

            // Assert
            Assert.IsInstanceOf<PipingInputContext>(selection);
            var dataRow = (PipingCalculationRow) dataGridView.Rows[selectedRow].DataBoundItem;
            Assert.AreSame(dataRow.Calculation, ((PipingInputContext) selection).PipingCalculation);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(stochasticSoilProfilesColumnIndex, null, true)]
        [TestCase(stochasticSoilProfilesColumnIndex, null, false)]
        [TestCase(selectableHydraulicBoundaryLocationsColumnIndex, null, true)]
        [TestCase(selectableHydraulicBoundaryLocationsColumnIndex, null, false)]
        [TestCase(dampingFactorExitMeanColumnIndex, 1.1, true)]
        [TestCase(dampingFactorExitMeanColumnIndex, 1.1, false)]
        [TestCase(phreaticLevelExitMeanColumnIndex, 1.1, true)]
        [TestCase(phreaticLevelExitMeanColumnIndex, 1.1, false)]
        [TestCase(entryPointLColumnIndex, 1.1, true)]
        [TestCase(entryPointLColumnIndex, 1.1, false)]
        [TestCase(exitPointLColumnIndex, 8.0, true)]
        [TestCase(exitPointLColumnIndex, 8.0, false)]
        public void PipingCalculationsView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(
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
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            ShowPipingCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (PipingCalculationScenario) calculationGroup.Children[1];

            if (useCalculationWithOutput)
            {
                calculation.Output = PipingOutputTestFactory.Create();
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
        public void GivenPipingCalculationsViewWithStochasticSoilProfile_WhenProbabilityChangesAndNotified_ThenNewProbabilityVisible()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);
            
            ShowPipingCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (PipingCalculationScenario) calculationGroup.Children[1];

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            var refreshed = 0;

            // Precondition
            var currentCell = (DataGridViewTextBoxCell) dataGridView.Rows[1].Cells[stochasticSoilProfilesProbabilityColumnIndex];
            Assert.AreEqual(GetFormattedProbabilityValue(30), currentCell.FormattedValue);

            PipingStochasticSoilProfile stochasticSoilProfileToChange = calculation.InputParameters.StochasticSoilProfile;
            var updatedProfile = new PipingStochasticSoilProfile(0.5, stochasticSoilProfileToChange.SoilProfile);
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
        public void GivenPipingCalculationsViewWithCalculations_WhenSurfaceLineLocatedOutsideSectionAfterUpdateAndObserversNotified_ThenDataGridViewUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            PipingCalculationsView view = ShowPipingCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (PipingCalculationScenario) calculationGroup.Children[0];

            DataGridViewControl dataGridView = view.Controls.Find("dataGridViewControl", true).OfType<DataGridViewControl>().First();
            ListBox listBox = view.Controls.Find("listBox", true).OfType<ListBox>().First();

            // Precondition
            listBox.SelectedIndex = 0;
            Assert.AreEqual(2, dataGridView.Rows.Count);
            Assert.AreEqual("Calculation 1", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", dataGridView.Rows[1].Cells[nameColumnIndex].FormattedValue);

            listBox.SelectedIndex = 1;
            Assert.AreEqual(1, dataGridView.Rows.Count);
            Assert.AreEqual("Calculation 2", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

            PipingSurfaceLine surfaceLineToChange = calculation.InputParameters.SurfaceLine;
            var updatedSurfaceLine = new PipingSurfaceLine(surfaceLineToChange.Name)
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(9.0, 0.0)
            };
            updatedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(9.0, 5.0, 0.0),
                new Point3D(9.0, 0.0, 1.0),
                new Point3D(9.0, -5.0, 0.0)
            });

            // When
            surfaceLineToChange.CopyProperties(updatedSurfaceLine);
            surfaceLineToChange.NotifyObservers();

            // Then
            listBox.SelectedIndex = 0;
            Assert.AreEqual(1, dataGridView.Rows.Count);
            Assert.AreEqual("Calculation 2", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

            listBox.SelectedIndex = 1;
            Assert.AreEqual(2, dataGridView.Rows.Count);
            Assert.AreEqual("Calculation 1", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", dataGridView.Rows[1].Cells[nameColumnIndex].FormattedValue);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PipingCalculationsViewWithHydraulicLocation_SpecificUseAssessmentLevelManualInputState_SelectableHydraulicLocationReadonlyAccordingly(bool useAssessmentLevelManualInput)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            PipingFailureMechanism failureMechanism = ConfigureFailureMechanism();
            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection, failureMechanism);

            ShowPipingCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            var calculation = (PipingCalculationScenario) calculationGroup.Children.First();

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

        private static void ConfigureHydraulicBoundaryDatabase(IAssessmentSection assessmentSection)
        {
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2),
                    new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4)
                }
            });
        }

        private static PipingFailureMechanism ConfigureSimpleFailureMechanism()
        {
            var surfaceLine1 = new PipingSurfaceLine("Surface line 1")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new PipingSurfaceLine("Surface line 2")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var failureMechanism = new PipingFailureMechanism();
            const string arbitraryFilePath = "path";
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, arbitraryFilePath);
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new PipingStochasticSoilModel("PipingStochasticSoilModel", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                }, new[]
                {
                    new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("A")),
                    new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("B"))
                })
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

            return failureMechanism;
        }

        private static CalculationGroup ConfigureCalculationGroup(IAssessmentSection assessmentSection, PipingFailureMechanism failureMechanism)
        {
            PipingStochasticSoilModel stochasticSoilModelForCalculation2 = failureMechanism.StochasticSoilModels.Last();
            return new CalculationGroup
            {
                Children =
                {
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            SurfaceLine = failureMechanism.SurfaceLines.First(),
                            StochasticSoilModel = failureMechanism.StochasticSoilModels.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                            DampingFactorExit =
                            {
                                Mean = (RoundedDouble) 1.1111
                            },
                            PhreaticLevelExit =
                            {
                                Mean = (RoundedDouble) 2.2222
                            },
                            EntryPointL = (RoundedDouble) 3.3333,
                            ExitPointL = (RoundedDouble) 4.4444
                        }
                    },
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            SurfaceLine = failureMechanism.SurfaceLines.Last(),
                            StochasticSoilModel = stochasticSoilModelForCalculation2,
                            StochasticSoilProfile = stochasticSoilModelForCalculation2.StochasticSoilProfiles.First(),
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.Last(),
                            DampingFactorExit =
                            {
                                Mean = (RoundedDouble) 5.5555
                            },
                            PhreaticLevelExit =
                            {
                                Mean = (RoundedDouble) 6.6666
                            },
                            EntryPointL = (RoundedDouble) 7.7777,
                            ExitPointL = (RoundedDouble) 8.8888
                        }
                    }
                }
            };
        }

        private static PipingFailureMechanism ConfigureFailureMechanism()
        {
            var surfaceLine1 = new PipingSurfaceLine("Surface line 1")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new PipingSurfaceLine("Surface line 2")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var failureMechanism = new PipingFailureMechanism();
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

            var stochasticSoilProfile1 = new PipingStochasticSoilProfile(
                0.3, new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D));

            var stochasticSoilModelA = new PipingStochasticSoilModel("Model A", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                stochasticSoilProfile1,
                new PipingStochasticSoilProfile(0.7, new PipingSoilProfile("Profile 2", -8.0, new[]
                {
                    new PipingSoilLayer(-4.0),
                    new PipingSoilLayer(0.0),
                    new PipingSoilLayer(4.0)
                }, SoilProfileType.SoilProfile1D))
            });

            var stochasticSoilProfile5 = new PipingStochasticSoilProfile(
                0.3, new PipingSoilProfile("Profile 5", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D));

            var stochasticSoilModelE = new PipingStochasticSoilModel("Model E", new[]
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
                new PipingStochasticSoilModel("Model C", new[]
                {
                    new Point2D(1.0, 0.0),
                    new Point2D(4.0, 0.0)
                }, new[]
                {
                    new PipingStochasticSoilProfile(0.3, new PipingSoilProfile("Profile 3", -10.0, new[]
                    {
                        new PipingSoilLayer(-5.0),
                        new PipingSoilLayer(-2.0),
                        new PipingSoilLayer(1.0)
                    }, SoilProfileType.SoilProfile1D)),
                    new PipingStochasticSoilProfile(0.7, new PipingSoilProfile("Profile 4", -8.0, new[]
                    {
                        new PipingSoilLayer(-4.0),
                        new PipingSoilLayer(0.0),
                        new PipingSoilLayer(4.0)
                    }, SoilProfileType.SoilProfile1D))
                }),
                stochasticSoilModelE
            }, arbitraryFilePath);
            return failureMechanism;
        }

        private PipingCalculationsView ShowPipingCalculationsView(CalculationGroup calculationGroup, PipingFailureMechanism failureMechanism,
                                                                  IAssessmentSection assessmentSection)
        {
            var view = new PipingCalculationsView(calculationGroup, failureMechanism, assessmentSection);

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