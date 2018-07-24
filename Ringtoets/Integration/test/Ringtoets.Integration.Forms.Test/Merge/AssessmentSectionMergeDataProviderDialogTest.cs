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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Forms.Merge;
using Ringtoets.Integration.TestUtil;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergeDataProviderDialogTest : NUnitFormTest
    {
        private const int isSelectedIndex = 0;
        private const int failureMechanismNameIndex = 1;
        private const int isRelevantIndex = 2;
        private const int hasSectionsIndex = 3;
        private const int numberOfCalculationsIndex = 4;
        private const int columnCount = 5;

        [Test]
        public void Constructor_DialogParentNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeDataProviderDialog(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("dialogParent", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            // Call
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.IsInstanceOf<IAssessmentSectionMergeDataProvider>(dialog);

                Assert.AreEqual("Selecteer trajectinformatie", dialog.Text);

                Icon icon = BitmapToIcon(Resources.SelectionDialogIcon);
                Bitmap expectedImage = icon.ToBitmap();
                Bitmap actualImage = dialog.Icon.ToBitmap();
                TestHelper.AssertImagesAreEqual(expectedImage, actualImage);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Show_Always_DefaultProperties()
        {
            // Setup
            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(1, dialog.Controls.Count);

                var tableLayoutPanel = (TableLayoutPanel) new ControlTester("tableLayoutPanelForForm").TheObject;
                Assert.AreEqual(1, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(5, tableLayoutPanel.RowCount);

                var assessmentSectionSelectLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 0);
                Assert.AreEqual("Selecteer traject:", assessmentSectionSelectLabel.Text);

                var assessmentSectionComboBox = (ComboBox) tableLayoutPanel.GetControlFromPosition(0, 1);
                Assert.IsTrue(assessmentSectionComboBox.Enabled);
                Assert.AreEqual(ComboBoxStyle.DropDownList, assessmentSectionComboBox.DropDownStyle);
                CollectionAssert.IsEmpty(assessmentSectionComboBox.Items);

                var tableLayoutPanelForLabels = (TableLayoutPanel) tableLayoutPanel.GetControlFromPosition(0, 2);
                Assert.AreEqual(2, tableLayoutPanelForLabels.ColumnCount);
                Assert.AreEqual(1, tableLayoutPanelForLabels.RowCount);
                var failureMechanismSelectionLabel = (Label) tableLayoutPanelForLabels.GetControlFromPosition(0, 0);
                Assert.AreEqual("Selecteer toetssporen:", failureMechanismSelectionLabel.Text);
                Assert.IsInstanceOf<PictureBox>(tableLayoutPanelForLabels.GetControlFromPosition(1, 0));

                Assert.IsInstanceOf<DataGridViewControl>(tableLayoutPanel.GetControlFromPosition(0, 3));
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreEqual(columnCount, dataGridView.ColumnCount);
                Assert.AreEqual(0, dataGridView.RowCount);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[isSelectedIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[failureMechanismNameIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[isRelevantIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[hasSectionsIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[numberOfCalculationsIndex]);

                Assert.AreEqual("Selecteer", dataGridView.Columns[isSelectedIndex].HeaderText);
                Assert.AreEqual("Toetsspoor", dataGridView.Columns[failureMechanismNameIndex].HeaderText);
                Assert.AreEqual("Is relevant", dataGridView.Columns[isRelevantIndex].HeaderText);
                Assert.AreEqual("Heeft vakindeling", dataGridView.Columns[hasSectionsIndex].HeaderText);
                Assert.AreEqual("Aantal berekeningen", dataGridView.Columns[numberOfCalculationsIndex].HeaderText);

                Assert.IsFalse(dataGridView.Columns[isSelectedIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[failureMechanismNameIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[isRelevantIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[hasSectionsIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[numberOfCalculationsIndex].ReadOnly);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);

                var flowLayoutPanel = (FlowLayoutPanel) tableLayoutPanel.GetControlFromPosition(0, 4);
                Control.ControlCollection flowLayoutPanelControls = flowLayoutPanel.Controls;
                Assert.AreEqual(2, flowLayoutPanelControls.Count);
                CollectionAssert.AllItemsAreInstancesOfType(flowLayoutPanelControls, typeof(Button));

                var buttonSelect = (Button) new ButtonTester("importButton", dialog).TheObject;
                Assert.AreEqual("Importeren", buttonSelect.Text);
                Assert.IsTrue(buttonSelect.Enabled);
                Assert.AreEqual(DialogResult.OK, buttonSelect.DialogResult);

                var buttonCancel = (Button) new ButtonTester("cancelButton", dialog).TheObject;
                Assert.AreEqual("Annuleren", buttonCancel.Text);
                Assert.AreEqual(dialog.CancelButton, buttonCancel);
                Assert.AreEqual(DialogResult.Cancel, buttonCancel.DialogResult);

                Assert.AreEqual(720, dialog.MinimumSize.Width);
                Assert.AreEqual(590, dialog.MinimumSize.Height);
            }
        }

        [Test]
        public void Show_Always_InitializesTooltip()
        {
            // Setup
            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // Call
                dialog.Show();

                // Assert
                var infoIcon = (PictureBox) new ControlTester("infoIcon", dialog).TheObject;
                TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.information, infoIcon.BackgroundImage);
                Assert.AreEqual(ImageLayout.Center, infoIcon.BackgroundImageLayout);

                var toolTip = TypeUtils.GetField<ToolTip>(dialog, "toolTip");
                Assert.AreEqual("Hydraulische belastingen op trajectniveau worden altijd samengevoegd.\r\n" +
                                "Daarbij gaan de huidige berekeningsresultaten voor belastingen op trajectniveau niet verloren.",
                                toolTip.GetToolTip(infoIcon));
                Assert.AreEqual(5000, toolTip.AutoPopDelay);
                Assert.AreEqual(100, toolTip.InitialDelay);
                Assert.AreEqual(100, toolTip.ReshowDelay);
            }
        }

        [Test]
        public void GetMergeData_AssessmentSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // Call
                TestDelegate call = () => dialog.GetMergeData(null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(call);
                Assert.AreEqual("assessmentSections", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetMergeData_WithEmptyAssessmentSections_ThrowsArgumentException()
        {
            // Setup
            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // Call
                TestDelegate call = () => dialog.GetMergeData(Enumerable.Empty<AssessmentSection>());

                // Assert
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "assessmentSections must at least have one element.");
            }
        }

        [Test]
        public void GetMergeData_WithAssessmentSections_SetsDataOnDialog()
        {
            // Setup
            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName)) {}
            };

            var random = new Random(21);
            AssessmentSection[] assessmentSections =
            {
                TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations(),
                new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            };

            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // Call
                dialog.GetMergeData(assessmentSections);

                // Assert
                AssessmentSection expectedDefaultSelectedAssessmentSection = assessmentSections[0];

                var comboBox = (ComboBox) new ComboBoxTester("assessmentSectionComboBox", dialog).TheObject;
                Assert.AreSame(expectedDefaultSelectedAssessmentSection, comboBox.SelectedItem);
                CollectionAssert.AreEqual(assessmentSections, comboBox.Items);

                var dataGridView = (DataGridView) new ControlTester("dataGridView", dialog).TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                AssertFailureMechanismRows(expectedDefaultSelectedAssessmentSection, rows);
            }
        }

        [Test]
        public void GivenValidDialog_WhenGetMergeDataCalledAndCancelPressed_ThenReturnsNull()
        {
            // Given
            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName))
                {
                    var button = new ButtonTester("cancelButton", formName);
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // When
                AssessmentSectionMergeData result = dialog.GetMergeData(new[]
                {
                    TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations()
                });

                // Then
                Assert.IsNull(result);
            }
        }

        [Test]
        public void GivenValidDialog_WhenGetMergeDataCalledAndDataSelectedAndImportPressed_ThenReturnsSelectedData()
        {
            // Given
            var random = new Random(21);
            AssessmentSection selectedAssessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            const int selectedFailureMechanismOne = 5;
            const int selectedFailureMechanismTwo = 8;

            DialogBoxHandler = (formName, wnd) =>
            {
                using (var formTester = new FormTester(formName))
                {
                    var dialog = (AssessmentSectionMergeDataProviderDialog) formTester.TheObject;
                    var comboBox = (ComboBox) new ComboBoxTester("assessmentSectionComboBox", dialog).TheObject;
                    comboBox.SelectedItem = selectedAssessmentSection;

                    var dataGridView = (DataGridView) new ControlTester("dataGridView", dialog).TheObject;

                    DataGridViewRowCollection rows = dataGridView.Rows;
                    rows[selectedFailureMechanismOne].Cells[isSelectedIndex].Value = true;
                    rows[selectedFailureMechanismTwo].Cells[isSelectedIndex].Value = true;

                    var button = new ButtonTester("importButton", formName);
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // When
                AssessmentSectionMergeData result = dialog.GetMergeData(new[]
                {
                    new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()),
                    selectedAssessmentSection
                });

                // Then
                Assert.AreSame(selectedAssessmentSection, result.AssessmentSection);

                IEnumerable<IFailureMechanism> selectedFailureMechanisms = result.FailureMechanisms;
                Assert.AreEqual(2, selectedFailureMechanisms.Count());
                CollectionAssert.AreEquivalent(new IFailureMechanism[]
                {
                    selectedAssessmentSection.StabilityStoneCover,
                    selectedAssessmentSection.GrassCoverErosionOutwards
                }, selectedFailureMechanisms);
            }
        }

        [Test]
        public void GivenDialogWithAssessmentSection_WhenSelectingOtherAssessmentSection_ThenDataUpdated()
        {
            // Given
            var random = new Random(21);
            AssessmentSection[] assessmentSections =
            {
                TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations(),
                new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            };

            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName)) {}
            };

            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                dialog.GetMergeData(assessmentSections);

                var comboBox = (ComboBox) new ComboBoxTester("assessmentSectionComboBox", dialog).TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView", dialog).TheObject;

                // Precondition 
                AssessmentSection defaultSelectedAssessmentSection = assessmentSections[0];
                Assert.AreSame(defaultSelectedAssessmentSection, comboBox.SelectedItem);
                AssertFailureMechanismRows(defaultSelectedAssessmentSection, dataGridView.Rows);

                // When
                AssessmentSection itemToBeSelected = assessmentSections[1];
                comboBox.SelectedItem = itemToBeSelected;

                // Then
                AssertFailureMechanismRows(itemToBeSelected, dataGridView.Rows);
            }
        }

        private static void AssertFailureMechanismRows(AssessmentSection expectedAssessmentSection, DataGridViewRowCollection rows)
        {
            Assert.AreEqual(expectedAssessmentSection.GetFailureMechanisms().Count(), rows.Count);
            AssertDataGridViewRow(expectedAssessmentSection.Piping, rows[0].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.GrassCoverErosionInwards, rows[1].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.MacroStabilityInwards, rows[2].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.MacroStabilityOutwards, rows[3].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.Microstability, rows[4].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.StabilityStoneCover, rows[5].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.WaveImpactAsphaltCover, rows[6].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.WaterPressureAsphaltCover, rows[7].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.GrassCoverErosionOutwards, rows[8].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.GrassCoverSlipOffOutwards, rows[9].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.GrassCoverSlipOffInwards, rows[10].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.HeightStructures, rows[11].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.ClosingStructures, rows[12].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.PipingStructure, rows[13].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.StabilityPointStructures, rows[14].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.StrengthStabilityLengthwiseConstruction, rows[15].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.DuneErosion, rows[16].Cells);
            AssertDataGridViewRow(expectedAssessmentSection.TechnicalInnovation, rows[17].Cells);
        }

        private static void AssertDataGridViewRow(IFailureMechanism expectedFailureMechanism,
                                                  DataGridViewCellCollection cells)
        {
            Assert.AreEqual(false, cells[isSelectedIndex].Value);
            Assert.AreEqual(expectedFailureMechanism.Name, cells[failureMechanismNameIndex].Value);
            Assert.AreEqual(expectedFailureMechanism.IsRelevant, cells[isRelevantIndex].Value);
            Assert.AreEqual(expectedFailureMechanism.Sections.Any(), cells[hasSectionsIndex].Value);
            Assert.AreEqual(expectedFailureMechanism.Calculations.Count(), cells[numberOfCalculationsIndex].Value);
        }

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return Icon.FromHandle(icon.GetHicon());
        }
    }
}