using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Merge;

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
        public void ShowDialog_Always_DefaultProperties()
        {
            // Setup
            using (var dialogParent = new Form())
            using (var dialog = new AssessmentSectionProviderDialog(dialogParent))
            {
                // Call
                dialog.Show();

                // Assert
                var tableLayoutPanel = (TableLayoutPanel) new ControlTester("tableLayoutPanel").TheObject;
                Assert.AreEqual(1, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(5, tableLayoutPanel.RowCount);

                var assessmentSectionSelectLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 0);
                Assert.AreEqual("Selecteer traject:", assessmentSectionSelectLabel.Text);

                var assessmentSectionComboBox = (ComboBox) tableLayoutPanel.GetControlFromPosition(0, 1);
                Assert.IsTrue(assessmentSectionComboBox.Enabled);

                var failureMechanismSelectionLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 2);
                Assert.AreEqual("Selecteer toetssporen:", failureMechanismSelectionLabel.Text);

                Assert.IsInstanceOf<DataGridViewControl>(tableLayoutPanel.GetControlFromPosition(0, 3));

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreEqual(columnCount, dataGridView.ColumnCount);
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

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return Icon.FromHandle(icon.GetHicon());
        }
    }
}