using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Merge;
using Ringtoets.Integration.TestUtil;

namespace Ringtoets.Integration.Forms.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionProviderDialogTest : NUnitFormTest
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
            TestDelegate call = () => new AssessmentSectionProviderDialog(null);

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
            using (var dialog = new AssessmentSectionProviderDialog(dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.IsInstanceOf<IMergeDataProvider>(dialog);

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
            using (var dialog = new AssessmentSectionProviderDialog(dialogParent))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(1, dialog.Controls.Count);

                var tableLayoutPanel = (TableLayoutPanel) new ControlTester("tableLayoutPanel").TheObject;
                Assert.AreEqual(1, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(5, tableLayoutPanel.RowCount);

                var assessmentSectionSelectLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 0);
                Assert.AreEqual("Selecteer traject:", assessmentSectionSelectLabel.Text);

                var assessmentSectionComboBox = (ComboBox) tableLayoutPanel.GetControlFromPosition(0, 1);
                Assert.IsTrue(assessmentSectionComboBox.Enabled);
                Assert.AreEqual(ComboBoxStyle.DropDownList, assessmentSectionComboBox.DropDownStyle);
                CollectionAssert.IsEmpty(assessmentSectionComboBox.Items);

                var failureMechanismSelectionLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 2);
                Assert.AreEqual("Selecteer toetssporen:", failureMechanismSelectionLabel.Text);

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

                var buttonCancel = (Button) new ButtonTester("cancelButton", dialog).TheObject;
                Assert.AreEqual("Annuleren", buttonCancel.Text);

                Assert.AreEqual(500, dialog.MinimumSize.Width);
                Assert.AreEqual(350, dialog.MinimumSize.Height);
            }
        }

        [Test]
        public void SelectData_WithEmptyAssessmentSections_SetsDataOnDialog()
        {
            // Setup
            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionProviderDialog(dialogParent))
            {
                // Call
                dialog.SelectData(Enumerable.Empty<AssessmentSection>());

                // Assert
                var comboBox = (ComboBox) new ComboBoxTester("assessmentSectionComboBox", dialog).TheObject;
                Assert.IsNull(comboBox.SelectedItem);
                CollectionAssert.IsEmpty(comboBox.Items);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(0, rows.Count);
            }
        }

        [Test]
        public void SelectData_WithAssessmentSections_SetsDataOnDialog()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection[] assessmentSections =
            {
                TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations(),
                new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            };

            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionProviderDialog(dialogParent))
            {
                // Call
                dialog.SelectData(assessmentSections);

                // Assert
                AssessmentSection expectedDefaultSelectedAssessmentSection = assessmentSections[0];

                var comboBox = (ComboBox) new ComboBoxTester("assessmentSectionComboBox", dialog).TheObject;
                Assert.AreSame(expectedDefaultSelectedAssessmentSection, comboBox.SelectedItem);
                CollectionAssert.AreEqual(assessmentSections, comboBox.Items);

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                AssertFailureMechanismRows(expectedDefaultSelectedAssessmentSection, rows);
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

            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionProviderDialog(dialogParent))
            {
                dialog.SelectData(assessmentSections);

                var comboBox = (ComboBox) new ComboBoxTester("assessmentSectionComboBox", dialog).TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                
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