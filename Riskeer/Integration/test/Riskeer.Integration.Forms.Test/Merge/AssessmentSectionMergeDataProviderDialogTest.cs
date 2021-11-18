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

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Properties;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;
using Riskeer.Integration.Forms.Merge;
using Riskeer.Integration.TestUtil;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.Integration.Forms.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergeDataProviderDialogTest : NUnitFormTest
    {
        private const int isSelectedIndex = 0;
        private const int failurePathNameIndex = 1;
        private const int inAssemblyIndex = 2;
        private const int hasSectionsIndex = 3;
        private const int numberOfCalculationsIndex = 4;
        private const int columnCount = 5;

        [Test]
        public void Constructor_DialogParentNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionMergeDataProviderDialog(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
                Assert.AreEqual(4, tableLayoutPanel.RowCount);

                var tableLayoutPanelForLabels = (TableLayoutPanel) tableLayoutPanel.GetControlFromPosition(0, 1);
                Assert.AreEqual(2, tableLayoutPanelForLabels.ColumnCount);
                Assert.AreEqual(1, tableLayoutPanelForLabels.RowCount);
                var failureMechanismSelectionLabel = (Label) tableLayoutPanelForLabels.GetControlFromPosition(0, 0);
                Assert.AreEqual("Selecteer toetssporen:", failureMechanismSelectionLabel.Text);

                Assert.IsInstanceOf<DataGridViewControl>(tableLayoutPanel.GetControlFromPosition(0, 2));
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreEqual(columnCount, dataGridView.ColumnCount);
                Assert.AreEqual(0, dataGridView.RowCount);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[isSelectedIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[failurePathNameIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[inAssemblyIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[hasSectionsIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[numberOfCalculationsIndex]);

                Assert.AreEqual("Selecteer", dataGridView.Columns[isSelectedIndex].HeaderText);
                Assert.AreEqual("Toetsspoor", dataGridView.Columns[failurePathNameIndex].HeaderText);
                Assert.AreEqual("In assemblage", dataGridView.Columns[inAssemblyIndex].HeaderText);
                Assert.AreEqual("Heeft vakindeling", dataGridView.Columns[hasSectionsIndex].HeaderText);
                Assert.AreEqual("Aantal berekeningen", dataGridView.Columns[numberOfCalculationsIndex].HeaderText);

                Assert.IsFalse(dataGridView.Columns[isSelectedIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[failurePathNameIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[inAssemblyIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[hasSectionsIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[numberOfCalculationsIndex].ReadOnly);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);

                var flowLayoutPanel = (FlowLayoutPanel) tableLayoutPanel.GetControlFromPosition(0, 3);
                Control.ControlCollection flowLayoutPanelControls = flowLayoutPanel.Controls;
                Assert.AreEqual(3, flowLayoutPanelControls.Count);
                Control pictureBox = flowLayoutPanel.Controls[2];
                Assert.IsInstanceOf<PictureBox>(pictureBox);

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
                TestHelper.AssertImagesAreEqual(CoreGuiResources.information, infoIcon.BackgroundImage);
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
        public void GetMergeData_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // Call
                void Call() => dialog.GetMergeData(null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("assessmentSection", exception.ParamName);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetMergeData_WithAssessmentSection_SetsDataOnDialog()
        {
            // Setup
            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName)) {}
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsAndFailurePaths();

            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // Call
                dialog.GetMergeData(assessmentSection);

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView", dialog).TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;

                int expectedNrOfRows = assessmentSection.GetFailureMechanisms().Count() +
                                       assessmentSection.SpecificFailurePaths.Count;
                Assert.AreEqual(expectedNrOfRows, rows.Count);
                AssertFailureMechanismRows(assessmentSection, rows);
                AssertFailurePathRows(assessmentSection, rows);
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
                AssessmentSectionMergeData result = dialog.GetMergeData(TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsAndFailurePaths());

                // Then
                Assert.IsNull(result);
            }
        }

        [Test]
        public void GivenValidDialog_WhenGetMergeDataCalledAndOnlyAssessmentSectionSelectedAndImportPressed_ThenReturnsSelectedData()
        {
            // Given
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsAndFailurePaths();

            DialogBoxHandler = (formName, wnd) =>
            {
                using (new FormTester(formName))
                {
                    var button = new ButtonTester("importButton", formName);
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // When
                AssessmentSectionMergeData result = dialog.GetMergeData(assessmentSection);

                // Then
                Assert.AreSame(assessmentSection, result.AssessmentSection);

                Assert.IsFalse(result.MergePiping);
                Assert.IsFalse(result.MergeGrassCoverErosionInwards);
                Assert.IsFalse(result.MergeMacroStabilityInwards);
                Assert.IsFalse(result.MergeMacroStabilityOutwards);
                Assert.IsFalse(result.MergeMicrostability);
                Assert.IsFalse(result.MergeStabilityStoneCover);
                Assert.IsFalse(result.MergeWaveImpactAsphaltCover);
                Assert.IsFalse(result.MergeWaterPressureAsphaltCover);
                Assert.IsFalse(result.MergeGrassCoverErosionOutwards);
                Assert.IsFalse(result.MergeGrassCoverSlipOffOutwards);
                Assert.IsFalse(result.MergeGrassCoverSlipOffInwards);
                Assert.IsFalse(result.MergeHeightStructures);
                Assert.IsFalse(result.MergeClosingStructures);
                Assert.IsFalse(result.MergePipingStructure);
                Assert.IsFalse(result.MergeStabilityPointStructures);
                Assert.IsFalse(result.MergeStrengthStabilityLengthwiseConstruction);
                Assert.IsFalse(result.MergeDuneErosion);
                Assert.IsFalse(result.MergeTechnicalInnovation);
                CollectionAssert.IsEmpty(result.MergeSpecificFailurePaths);
            }
        }

        [Test]
        public void GivenValidDialog_WhenGetMergeDataCalledAndAllDataSelectedAndImportPressed_ThenReturnsSelectedData()
        {
            // Given
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurationsAndFailurePaths();

            DialogBoxHandler = (formName, wnd) =>
            {
                using (var formTester = new FormTester(formName))
                {
                    var dialog = (AssessmentSectionMergeDataProviderDialog) formTester.TheObject;

                    var dataGridView = (DataGridView) new ControlTester("dataGridView", dialog).TheObject;

                    DataGridViewRowCollection rows = dataGridView.Rows;

                    foreach (DataGridViewRow row in rows)
                    {
                        row.Cells[isSelectedIndex].Value = true;
                    }

                    var button = new ButtonTester("importButton", formName);
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionMergeDataProviderDialog(dialogParent))
            {
                // When
                AssessmentSectionMergeData result = dialog.GetMergeData(assessmentSection);

                // Then
                Assert.AreSame(assessmentSection, result.AssessmentSection);

                Assert.IsTrue(result.MergePiping);
                Assert.IsTrue(result.MergeGrassCoverErosionInwards);
                Assert.IsTrue(result.MergeMacroStabilityInwards);
                Assert.IsTrue(result.MergeMacroStabilityOutwards);
                Assert.IsTrue(result.MergeMicrostability);
                Assert.IsTrue(result.MergeStabilityStoneCover);
                Assert.IsTrue(result.MergeWaveImpactAsphaltCover);
                Assert.IsTrue(result.MergeWaterPressureAsphaltCover);
                Assert.IsTrue(result.MergeGrassCoverErosionOutwards);
                Assert.IsTrue(result.MergeGrassCoverSlipOffOutwards);
                Assert.IsTrue(result.MergeGrassCoverSlipOffInwards);
                Assert.IsTrue(result.MergeHeightStructures);
                Assert.IsTrue(result.MergeClosingStructures);
                Assert.IsTrue(result.MergePipingStructure);
                Assert.IsTrue(result.MergeStabilityPointStructures);
                Assert.IsTrue(result.MergeStrengthStabilityLengthwiseConstruction);
                Assert.IsTrue(result.MergeDuneErosion);
                Assert.IsTrue(result.MergeTechnicalInnovation);
                CollectionAssert.AreEqual(assessmentSection.SpecificFailurePaths, result.MergeSpecificFailurePaths);
            }
        }

        private static void AssertFailureMechanismRows(AssessmentSection expectedAssessmentSection, DataGridViewRowCollection rows)
        {
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

        private static void AssertFailurePathRows(AssessmentSection expectedAssessmentSection, DataGridViewRowCollection rows)
        {
            int offset = expectedAssessmentSection.GetFailureMechanisms().Count();
            ObservableList<IFailurePath> failurePaths = expectedAssessmentSection.SpecificFailurePaths;
            for (int i = 0; i < failurePaths.Count; i++)
            {
                AssertDataGridViewRow(failurePaths[i], rows[i + offset].Cells);
            }
        }

        private static void AssertDataGridViewRow(IFailureMechanism expectedFailureMechanism,
                                                  DataGridViewCellCollection cells)
        {
            Assert.AreEqual(false, cells[isSelectedIndex].Value);
            Assert.AreEqual(expectedFailureMechanism.Name, cells[failurePathNameIndex].Value);
            Assert.AreEqual(expectedFailureMechanism.InAssembly, cells[inAssemblyIndex].Value);
            Assert.AreEqual(expectedFailureMechanism.Sections.Any(), cells[hasSectionsIndex].Value);
            Assert.AreEqual(expectedFailureMechanism.Calculations.Count(), cells[numberOfCalculationsIndex].Value);
        }

        private static void AssertDataGridViewRow(IFailurePath expectedFailurePath,
                                                  DataGridViewCellCollection cells)
        {
            Assert.AreEqual(false, cells[isSelectedIndex].Value);
            Assert.AreEqual(expectedFailurePath.Name, cells[failurePathNameIndex].Value);
            Assert.AreEqual(expectedFailurePath.InAssembly, cells[inAssemblyIndex].Value);
            Assert.AreEqual(expectedFailurePath.Sections.Any(), cells[hasSectionsIndex].Value);
            Assert.AreEqual(0, cells[numberOfCalculationsIndex].Value);
        }

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return Icon.FromHandle(icon.GetHicon());
        }
    }
}