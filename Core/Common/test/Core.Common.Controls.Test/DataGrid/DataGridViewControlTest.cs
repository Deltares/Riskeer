// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.Test.DataGrid
{
    [TestFixture]
    public class DataGridViewControlTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            using (var control = new DataGridViewControl())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(control);
                Assert.AreEqual(1, control.Controls.Count);

                var dataGridView = control.Controls[0];
                Assert.IsInstanceOf<DoubleBufferedDataGridView>(dataGridView);
                Assert.AreEqual(DockStyle.Fill, dataGridView.Dock);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            using (var form = new Form())
            {
                // Call
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                Assert.AreEqual(0, dataGridView.ColumnCount);
                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
                Assert.AreEqual(DataGridViewEditMode.EditOnEnter, dataGridView.EditMode);
                Assert.AreEqual(DataGridViewColumnHeadersHeightSizeMode.AutoSize, dataGridView.ColumnHeadersHeightSizeMode);
                Assert.AreEqual(DataGridViewRowHeadersWidthSizeMode.DisableResizing, dataGridView.RowHeadersWidthSizeMode);
                Assert.IsFalse(dataGridView.AllowUserToResizeColumns);
                Assert.IsFalse(dataGridView.AllowUserToResizeRows);
                Assert.IsFalse(dataGridView.AllowUserToAddRows);
                Assert.IsFalse(dataGridView.AllowUserToDeleteRows);
                Assert.IsFalse(dataGridView.AutoGenerateColumns);
                Assert.IsTrue(dataGridView.StandardTab);
            }
        }

        [Test]
        public void IsCurrentCellInEditMode_CurrentCellInEditMode_ReturnsTrue()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;
                dataGridView.BeginEdit(false);

                // Call
                bool editMode = control.IsCurrentCellInEditMode;

                // Assert
                Assert.IsTrue(editMode);
            }
        }

        [Test]
        public void IsCurrentCellInEditMode_CurrentCellNotInEditMode_ReturnsFalse()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                // Call
                bool editMode = control.IsCurrentCellInEditMode;

                // Assert
                Assert.IsFalse(editMode);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddTextBoxColumn_Always_AddsColumnToDataGridView(bool readOnly)
        {
            // Setup
            using (var form = new Form())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddTextBoxColumn(propertyName, headerText, readOnly);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                DataGridViewTextBoxColumn columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual(string.Format("column_{0}", propertyName), columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.AreEqual(readOnly, columnData.ReadOnly);
            }
        }

        [Test]
        public void AddCheckBoxColumn_Always_AddsColumnToDataGridView()
        {
            // Setup
            using (var form = new Form())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddCheckBoxColumn(propertyName, headerText);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                DataGridViewCheckBoxColumn columnData = (DataGridViewCheckBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual(string.Format("column_{0}", propertyName), columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
            }
        }

        [Test]
        public void AddComboBoxColumn_DataSourceValueMemberAndDisplayMemberNull_AddsColumnToDataGridViewWithoutDataSourceValueMemberAndDisplayMember()
        {
            using (var form = new Form())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddComboBoxColumn<object>(propertyName, headerText, null, null, null);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                DataGridViewComboBoxColumn columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];

                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual(string.Format("column_{0}", propertyName), columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.IsNull(columnData.DataSource);
                Assert.AreEqual(string.Empty, columnData.ValueMember);
                Assert.AreEqual(string.Empty, columnData.DisplayMember);
            }
        }

        [Test]
        public void AddComboBoxColumn_DataSourceValueMemberAndDisplayMemberSet_AddsColumnToDataGridViewWithDataSourceValueMemberAndDisplayMember()
        {
            using (var form = new Form())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                List<EnumDisplayWrapper<TestEnum>> dataSource = Enum.GetValues(typeof(TestEnum))
                                           .OfType<TestEnum>()
                                           .Select(el => new EnumDisplayWrapper<TestEnum>(el))
                                           .ToList();

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddComboBoxColumn(propertyName, headerText, dataSource, ds => ds.Value, ds => ds.DisplayName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                DataGridViewComboBoxColumn columnData = (DataGridViewComboBoxColumn)dataGridView.Columns[0];

                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual(string.Format("column_{0}", propertyName), columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.AreSame(dataSource, columnData.DataSource);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);
            }
        }

        [Test]
        public void SetDataSource_Always_SetsDataSourceOnDataGridView()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddCheckBoxColumn("Test property", "Test header");

                var dataSource = new[]
                {
                    false
                };

                // Call
                control.SetDataSource(dataSource);

                // Assert
                Assert.AreEqual(dataSource, dataGridView.DataSource);
            }
        }

        [Test]
        public void EndEdit_CurrentCellInEditMode_EndEditAndSetCurrentCellToNull()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;
                dataGridView.BeginEdit(false);

                // Precondition
                Assert.IsTrue(dataGridView.IsCurrentCellInEditMode);

                // Call
                control.EndEdit();

                // Assert
                Assert.IsFalse(dataGridView.IsCurrentCellInEditMode);
                Assert.IsNull(dataGridView.CurrentCell);
            }
        }

        [Test]
        public void GetRows_Always_ReturnsAllDataGridViewRows()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "Row 1", "Row 2" };

                // Call
                DataGridViewRowCollection rows = control.GetRows();

                // Assert
                Assert.AreEqual(2, rows.Count);
            }
        }

        [Test]
        public void GetRowFromIndex_RowDoesExist_ReturnsRow()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                // Call
                DataGridViewRow row = control.GetRowFromIndex(0);

                // Assert
                Assert.IsNotNull(row);
            }
        }

        [Test]
        public void GetRowFromIndex_RowDoesNotExist_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                // Call
                TestDelegate call = () => control.GetRowFromIndex(5);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(call);
            }
        }

        [Test]
        public void GetCurrentRow_CurrentCellSet_ReturnsRow()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };
                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                // Call
                DataGridViewRow row = control.GetCurrentRow();

                // Assert
                Assert.IsNotNull(row);
            }
        }

        [Test]
        public void GetCurrentRow_CurrentCellNotSet_ReturnsNull()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };
                dataGridView.CurrentCell = null;

                // Call
                DataGridViewRow row = control.GetCurrentRow();

                // Assert
                Assert.IsNull(row);
            }
        }

        [Test]
        public void GetCell_RowAndCellDoesExist_ReturnsCell()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                // Call
                DataGridViewCell cell = control.GetCell(0, 0);

                // Assert
                Assert.IsNotNull(cell);
            }
        }

        [Test]
        public void GetCell_RowDoesNotExist_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                // Call
                TestDelegate call = () => control.GetCell(5, 0);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(call);
            }
        }

        [Test]
        public void GetCell_CellDoesNotExist_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                // Call
                TestDelegate call = () => control.GetCell(0, 5);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(call);
            }
        }

        [Test]
        public void GetColumnFromIndex_ColumnDoesExist_ReturnsColumn()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                var dataPropertyName = "Test property";
                var testHeader = "Test header";
                control.AddTextBoxColumn(dataPropertyName, testHeader);

                dataGridView.DataSource = new[] { "" };

                // Call
                DataGridViewColumn column = control.GetColumnFromIndex(0);

                // Assert
                Assert.IsNotNull(column);
                Assert.AreEqual(dataPropertyName, column.DataPropertyName);
                Assert.AreEqual(testHeader, column.HeaderText);
            }
        }

        [Test]
        public void GetColumnFromIndex_ColumnDoesNotExist_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                // Call
                TestDelegate call = () => control.GetColumnFromIndex(5);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(call);
            }
        }

        [Test]
        public void DisableCell_Always_DisablesCell()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                // Call
                control.DisableCell(0, 0);

                // Assert
                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                Assert.IsTrue(dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), dataGridViewCell.Style.ForeColor);
            }
        }

        [Test]
        public void RestoreCell_ReadOnlyFalse_SetsCellStyleToEnabledWithReadOnlyFalse()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                control.DisableCell(0, 0);

                // Precondition
                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                Assert.IsTrue(dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), dataGridViewCell.Style.ForeColor);

                // Call
                control.RestoreCell(0, 0, false);

                // Assert
                Assert.IsFalse(dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.White), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), dataGridViewCell.Style.ForeColor);
            }
        }

        [Test]
        public void RestoreCell_ReadOnlyTrue_SetsCellStyleToEnabledWithReadOnlyTrue()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                control.DisableCell(0, 0);

                // Precondition
                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                Assert.IsTrue(dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), dataGridViewCell.Style.ForeColor);

                // Call
                control.RestoreCell(0, 0, true);

                // Assert
                Assert.IsTrue(dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.White), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), dataGridViewCell.Style.ForeColor);
            }
        }

        #region Event handling

        [Test]
        public void AddCellFormattingHandler_Always_AddsEventHandler()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };
                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int counter = 0;

                control.AddCellFormattingHandler((sender, args) => counter++);

                // Precondition
                Assert.AreEqual(0, counter);

                // Call
                var formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void AddCellClickHandler_Always_AddsEventHandler()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };
                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int counter = 0;

                control.AddCellClickHandler((sender, args) => counter++);

                // Precondition
                Assert.AreEqual(0, counter);

                // Call
                gridTester.FireEvent("CellClick", new DataGridViewCellEventArgs(0, 0));

                // Assert
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void DataGridViewControlCheckBoxColumn_EditValueDirtyStateChangedEventFired_ValueCommittedCellInEditMode()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                control.AddCheckBoxColumn("Test property", "Test header");

                // Precondition
                Assert.AreEqual(1, dataGridView.ColumnCount);

                dataGridView.DataSource = new[] { false };

                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;
                dataGridView.BeginEdit(false);
                gridTester.FireEvent("KeyUp", new KeyEventArgs(Keys.Space));

                // Call
                gridTester.FireEvent("CurrentCellDirtyStateChanged", EventArgs.Empty);

                // Assert
                Assert.IsTrue(dataGridViewCell.IsInEditMode);
                Assert.IsTrue((bool)dataGridViewCell.FormattedValue);
            }
        }

        [Test]
        public void DataGridView_GotFocusCurrentCellSet_CellInEditMode()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;
                
                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };

                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                // Precondition
                Assert.IsFalse(dataGridView.IsCurrentCellInEditMode);

                // Call
                gridTester.FireEvent("GotFocus", EventArgs.Empty);

                // Assert
                Assert.IsTrue(dataGridView.IsCurrentCellInEditMode);
            }
        }

        [Test]
        public void DataGridView_GotFocusCurrentCellNull_CellNotInEditMode()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "" };
                dataGridView.CurrentCell = null;

                // Precondition
                Assert.IsFalse(dataGridView.IsCurrentCellInEditMode);

                // Call
                gridTester.FireEvent("GotFocus", EventArgs.Empty);

                // Assert
                Assert.IsFalse(dataGridView.IsCurrentCellInEditMode);
            }
        }

        [Test]
        public void DataGridView_CellValidatingValueValid_DoesNotShowErrorToolTip()
        {
            // Setup
            using (var form = new Form())
            {
                var control = new DataGridViewControl();
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView)gridTester.TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[] { "Test value" };
                var dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;
                dataGridView.BeginEdit(false);

                // Call
                dataGridViewCell.Value = "New value";

                // Assert
                Assert.AreEqual(string.Empty, dataGridViewCell.OwningRow.ErrorText);
            }
        }

        #endregion

        enum TestEnum
        {
            NoDisplayName,

            [ResourcesDisplayName(typeof(Resources), "DataGridViewControlTest_DisplayNameValueDisplayName")]
            DisplayName
        }
    }
}