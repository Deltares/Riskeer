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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Test.Properties;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
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

                Control dataGridView = control.Controls[0];
                Assert.IsInstanceOf<DoubleBufferedDataGridView>(dataGridView);
                Assert.AreEqual(DockStyle.Fill, dataGridView.Dock);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                // Call
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
                Assert.IsTrue(dataGridView.MultiSelect);
            }
        }

        [Test]
        public void IsCurrentCellInEditMode_CurrentCellInEditMode_ReturnsTrue()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
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
        public void MultiSelect_MultiSelectSetToFalse_ReturnsFalse(bool multiSelect)
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                control.MultiSelect = multiSelect;

                // Assert
                Assert.AreEqual(multiSelect, control.MultiSelect);
                Assert.AreEqual(multiSelect, dataGridView.MultiSelect);
            }
        }

        [Test]
        [TestCase(DataGridViewSelectionMode.RowHeaderSelect)]
        [TestCase(DataGridViewSelectionMode.CellSelect)]
        [TestCase(DataGridViewSelectionMode.ColumnHeaderSelect)]
        [TestCase(DataGridViewSelectionMode.FullColumnSelect)]
        [TestCase(DataGridViewSelectionMode.FullRowSelect)]
        public void SelectionMode_ValidSelectionMode_SetsDataGridViewSelectionMode(DataGridViewSelectionMode mode)
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                control.SelectionMode = mode;

                // Assert
                Assert.AreEqual(mode, control.SelectionMode);
                Assert.AreEqual(mode, dataGridView.SelectionMode);
            }
        }

        [Test]
        public void DataSource_Always_ReturnsDataGridViewSource()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataSource = new object();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.DataSource = dataSource;

                // Assert
                Assert.AreSame(dataSource, dataGridView.DataSource);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddTextBoxColumn_WithoutAutoSizeMode_AddsColumnToDataGridViewWithDefaultAutoSizeMode(bool readOnly)
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

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
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddTextBoxColumn_AllParametersSet_AddsColumnToDataGridViewWithAutoSizeModeAndMinimumWidthAndFormat(bool readOnly)
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var autoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                var minimumWidth = 100;
                var format = "1/#,#";

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddTextBoxColumn(propertyName, headerText, readOnly, autoSizeMode, minimumWidth, format);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                DataGridViewTextBoxColumn columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual(string.Format("column_{0}", propertyName), columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.AreEqual(readOnly, columnData.ReadOnly);
                Assert.AreEqual(autoSizeMode, columnData.AutoSizeMode);
                Assert.AreEqual(minimumWidth, columnData.MinimumWidth);
                Assert.AreEqual(format, columnData.DefaultCellStyle.Format);
            }
        }

        [Test]
        public void AddCheckBoxColumn_WithoutAutoSizeMode_AddsColumnToDataGridViewWithDefaultAutoSizeMode()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

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
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void AddCheckboxColumn_AutoSizeModeSet_AddsColumnToDataGridViewWithAutoSizeMode()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var autoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddCheckBoxColumn(propertyName, headerText, autoSizeMode);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                DataGridViewCheckBoxColumn columnData = (DataGridViewCheckBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual(string.Format("column_{0}", propertyName), columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.AreEqual(autoSizeMode, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void AddComboBoxColumn_AutoSizeModeSet_AddsColumnToDataGridViewWithAutoSizeMode()
        {
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                var autoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

                // Call
                control.AddComboBoxColumn<object>(propertyName, headerText, null, null, null, autoSizeMode);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                DataGridViewComboBoxColumn columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];

                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual(string.Format("column_{0}", propertyName), columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.IsNull(columnData.DataSource);
                Assert.AreEqual(string.Empty, columnData.ValueMember);
                Assert.AreEqual(string.Empty, columnData.DisplayMember);
                Assert.AreEqual(autoSizeMode, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void AddComboBoxColumn_DataSourceValueMemberAndDisplayMemberNull_AddsColumnToDataGridViewWithoutDataSourceValueMemberAndDisplayMember()
        {
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

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
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void AddComboBoxColumn_DataSourceValueMemberAndDisplayMemberSet_AddsColumnToDataGridViewWithDataSourceValueMemberAndDisplayMember()
        {
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                var propertyName = "PropertyName";
                var headerText = "HeaderText";

                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                List<EnumDisplayWrapper<TestEnum>> dataSource = Enum.GetValues(typeof(TestEnum))
                                                                    .OfType<TestEnum>()
                                                                    .Select(el => new EnumDisplayWrapper<TestEnum>(el))
                                                                    .ToList();

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddComboBoxColumn(
                    propertyName,
                    headerText,
                    dataSource,
                    TypeUtils.GetMemberName<EnumDisplayWrapper<TestEnum>>(ds => ds.Value),
                    TypeUtils.GetMemberName<EnumDisplayWrapper<TestEnum>>(ds => ds.DisplayName)
                    );

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                DataGridViewComboBoxColumn columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];

                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual(string.Format("column_{0}", propertyName), columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.AreSame(dataSource, columnData.DataSource);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void SetDataSource_Always_SetsDataSourceOnDataGridView()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

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
        public void RefreshDataGridView_AddLongerText_IncreasesColumnWidth()
        {
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                var dataSource = new[]
                {
                    "Test"
                };

                dataGridView.DataSource = dataSource;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int initialWidth = dataGridViewCell.OwningColumn.Width;
                dataGridViewCell.Value = "This is a long text.";

                // Call
                control.RefreshDataGridView();

                // Assert
                int longTextWidth = dataGridViewCell.OwningColumn.Width;
                Assert.Greater(longTextWidth, initialWidth);
            }
        }

        [Test]
        public void RefreshDataGridView_AddShorterText_DecreasesColumnWidth()
        {
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                var dataSource = new[]
                {
                    "Test"
                };

                dataGridView.DataSource = dataSource;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int initialWidth = dataGridViewCell.OwningColumn.Width;
                dataGridViewCell.Value = "This is a long text.";

                control.RefreshDataGridView();

                // Precondition
                int longTextWidth = dataGridViewCell.OwningColumn.Width;
                Assert.Greater(longTextWidth, initialWidth);

                dataGridViewCell.Value = string.Empty;

                // Call
                control.RefreshDataGridView();

                // Assert
                Assert.Less(dataGridViewCell.OwningColumn.Width, longTextWidth);
            }
        }

        [Test]
        public void AutoResizeColumns_AddLongerText_IncreasesColumnWidth()
        {
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                var dataSource = new[]
                {
                    "Test"
                };

                dataGridView.DataSource = dataSource;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int initialWidth = dataGridViewCell.OwningColumn.Width;
                dataGridViewCell.Value = "This is a long text.";

                // Call
                control.AutoResizeColumns();

                // Assert
                int longTextWidth = dataGridViewCell.OwningColumn.Width;
                Assert.Greater(longTextWidth, initialWidth);
            }
        }

        [Test]
        public void AutoResizeColumns_AddShorterText_DecreasesColumnWidth()
        {
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                var dataSource = new[]
                {
                    "Test"
                };

                dataGridView.DataSource = dataSource;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int initialWidth = dataGridViewCell.OwningColumn.Width;
                dataGridViewCell.Value = "This is a long text.";

                control.AutoResizeColumns();

                // Precondition
                int longTextWidth = dataGridViewCell.OwningColumn.Width;
                Assert.Greater(longTextWidth, initialWidth);

                dataGridViewCell.Value = string.Empty;

                // Call
                control.AutoResizeColumns();

                // Assert
                Assert.Less(dataGridViewCell.OwningColumn.Width, longTextWidth);
            }
        }

        [Test]
        public void EndEdit_CurrentCellInEditMode_EndEditAndSetCurrentCellToNull()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;
                dataGridView.BeginEdit(false);

                // Precondition
                Assert.IsTrue(dataGridView.IsCurrentCellInEditMode);

                // Call
                control.EndEdit();

                // Assert
                Assert.IsFalse(dataGridView.IsCurrentCellInEditMode);
                Assert.AreSame(dataGridViewCell, dataGridView.CurrentCell);
            }
        }

        [Test]
        public void GetRows_Always_ReturnsAllDataGridViewRows()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    "Row 1",
                    "Row 2"
                };

                // Call
                DataGridViewRowCollection rows = control.Rows;

                // Assert
                Assert.AreEqual(2, rows.Count);
            }
        }

        [Test]
        public void GetRowFromIndex_RowDoesExist_ReturnsRow()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                // Call
                DataGridViewRow row = control.CurrentRow;

                // Assert
                Assert.IsNotNull(row);
            }
        }

        [Test]
        public void GetCurrentRow_CurrentCellNotSet_ReturnsNull()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };
                dataGridView.CurrentCell = null;

                // Call
                DataGridViewRow row = control.CurrentRow;

                // Assert
                Assert.IsNull(row);
            }
        }

        [Test]
        public void GetCell_RowAndCellDoesExist_ReturnsCell()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var dataPropertyName = "Test property";
                var testHeader = "Test header";
                control.AddTextBoxColumn(dataPropertyName, testHeader);

                dataGridView.DataSource = new[]
                {
                    ""
                };

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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

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
        public void ClearCurrentCell_Always_SetsCurrentCellToNull()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header"); // Set read-only state of the column to the opposite

                dataGridView.DataSource = new[]
                {
                    ""
                };

                dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];

                // Precondition
                Assert.IsNotNull(dataGridView.CurrentCell);

                // Call
                control.ClearCurrentCell();

                // Assert
                Assert.IsNull(dataGridView.CurrentCell);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RestoreCell_WithoutSpecificReadOnlyState_SetsCellStyleToEnabledWithColumnReadOnlyState(bool columnReadOnlyState)
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header", columnReadOnlyState);

                dataGridView.DataSource = new[]
                {
                    ""
                };

                control.DisableCell(0, 0);

                // Precondition
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                Assert.IsTrue(dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), dataGridViewCell.Style.ForeColor);

                // Call
                control.RestoreCell(0, 0);

                // Assert
                Assert.AreEqual(columnReadOnlyState, dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.White), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), dataGridViewCell.Style.ForeColor);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RestoreCell_WithSpecificReadOnlyState_SetsCellStyleToEnabledWithSpecificReadOnlyState(bool specificReadOnlyState)
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header", !specificReadOnlyState); // Set read-only state of the column to the opposite

                dataGridView.DataSource = new[]
                {
                    ""
                };

                control.DisableCell(0, 0);

                // Precondition
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                Assert.IsTrue(dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), dataGridViewCell.Style.ForeColor);

                // Call
                control.RestoreCell(0, 0, specificReadOnlyState);

                // Assert
                Assert.AreEqual(specificReadOnlyState, dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.White), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), dataGridViewCell.Style.ForeColor);
            }
        }

        private enum TestEnum
        {
            NoDisplayName,

            [ResourcesDisplayName(typeof(Resources), "DataGridViewControlTest_DisplayNameValueDisplayName")]
            DisplayName
        }

        private class TestDataGridViewRow
        {
            public TestDataGridViewRow(RoundedDouble testRoundedDouble)
            {
                TestRoundedDouble = testRoundedDouble;
            }

            public RoundedDouble TestRoundedDouble { get; set; }
        }

        #region Event handling

        [Test]
        public void AddCellFormattingHandler_Always_AddsEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
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
        public void RemoveCellFormattingHandler_Always_RemovesEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int counter = 0;

                DataGridViewCellFormattingEventHandler dataGridViewCellFormattingEventHandler = (sender, args) => counter++;

                control.AddCellFormattingHandler(dataGridViewCellFormattingEventHandler);

                // Precondition
                Assert.AreEqual(0, counter);
                var formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.
                Assert.AreEqual(1, counter);

                // Call
                control.RemoveCellFormattingHandler(dataGridViewCellFormattingEventHandler);

                // Assert
                var formattedValue2 = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void AddCellClickHandler_Always_AddsEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
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
        public void RemoveCellClickHandler_Always_RemovesEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int counter = 0;

                DataGridViewCellEventHandler dataGridViewCellEventHandler = (sender, args) => counter++;

                control.AddCellClickHandler(dataGridViewCellEventHandler);

                // Precondition
                Assert.AreEqual(0, counter);
                gridTester.FireEvent("CellClick", new DataGridViewCellEventArgs(0, 0));
                Assert.AreEqual(1, counter);

                // Call
                control.RemoveCellClickHandler(dataGridViewCellEventHandler);
                gridTester.FireEvent("CellClick", new DataGridViewCellEventArgs(0, 0));

                // Assert
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void AddCellValueChangedHandler_Always_AddsEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int counter = 0;

                control.AddCellValueChangedHandler((sender, args) => counter++);

                // Precondition
                Assert.AreEqual(0, counter);

                // Call
                gridTester.FireEvent("CellValueChanged", new DataGridViewCellEventArgs(0, 0));

                // Assert
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void RemoveCellValueChangedHandler_Always_RemovesEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                int counter = 0;

                DataGridViewCellEventHandler dataGridViewCellEventHandler = (sender, args) => counter++;
                control.AddCellValueChangedHandler(dataGridViewCellEventHandler);

                // Precondition
                Assert.AreEqual(0, counter);
                gridTester.FireEvent("CellValueChanged", new DataGridViewCellEventArgs(0, 0));
                Assert.AreEqual(1, counter);

                // Call
                control.RemoveCellValueChangedHandler(dataGridViewCellEventHandler);
                gridTester.FireEvent("CellValueChanged", new DataGridViewCellEventArgs(0, 0));

                // Assert
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void DataGridViewControlCheckBoxColumn_EditValueDirtyStateChangedEventFired_ValueCommittedCellInEditMode()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                control.AddCheckBoxColumn("Test property", "Test header");

                // Precondition
                Assert.AreEqual(1, dataGridView.ColumnCount);

                dataGridView.DataSource = new[]
                {
                    false
                };

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;
                dataGridView.BeginEdit(false);
                gridTester.FireEvent("KeyUp", new KeyEventArgs(Keys.Space));

                // Call
                gridTester.FireEvent("CurrentCellDirtyStateChanged", EventArgs.Empty);

                // Assert
                Assert.IsTrue(dataGridViewCell.IsInEditMode);
                Assert.IsTrue((bool) dataGridViewCell.FormattedValue);
            }
        }

        [Test]
        public void DataGridView_GotFocusCurrentCellSet_CellInEditMode()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    ""
                };
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
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                control.AddTextBoxColumn("Test property", "Test header");

                dataGridView.DataSource = new[]
                {
                    "Test value"
                };
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;
                dataGridView.BeginEdit(false);

                // Call
                dataGridViewCell.Value = "New value";

                // Assert
                Assert.AreEqual(string.Empty, dataGridViewCell.OwningRow.ErrorText);
            }
        }

        [Test]
        public void DataGridView_ErrorWhenCommittingValue_DoesShowErrorToolTip()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                dataGridView.DataSource = new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, 25))
                };

                // Call
                dataGridView.Rows[0].Cells[0].Value = "test";

                // Assert
                Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);
            }
        }

        [Test]
        public void DataGridView_OnLeaveValidEditValue_ValueCommitted()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;
                dataGridView.DataSource = new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, 25))
                };

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;
                string newValue = "3";

                dataGridViewCell.Value = newValue;

                // Precondition
                Assert.IsTrue(dataGridView.IsCurrentCellInEditMode);
                Assert.AreEqual(string.Empty, dataGridView.Rows[0].ErrorText);

                // Call
                gridTester.FireEvent("Leave", EventArgs.Empty);

                // Assert
                Assert.AreEqual(new RoundedDouble(2, Convert.ToDouble(newValue)), new RoundedDouble(2, Convert.ToDouble(dataGridViewCell.FormattedValue)));
            }
        }

        [Test]
        public void DataGridView_OnLeaveInvalidEditValue_EditCanceledValueNotCommitted()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                double initialValue = 25;

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;
                dataGridView.DataSource = new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, initialValue))
                };

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;
                string newValue = "test";

                dataGridViewCell.Value = newValue;

                // Precondition
                Assert.IsTrue(dataGridView.IsCurrentCellInEditMode);
                Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);

                // Call
                gridTester.FireEvent("Leave", EventArgs.Empty);

                // Assert
                Assert.AreEqual(string.Empty, dataGridView.Rows[0].ErrorText);
                Assert.AreEqual(initialValue.ToString(CultureInfo.CurrentCulture), dataGridViewCell.FormattedValue);
            }
        }

        [Test]
        public void DataGridView_OnCellClickWithCombobox_ComboBoxDroppedDownIsTrue()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                var dataSource = new[]
                {
                    "a"
                };
                control.AddComboBoxColumn("Test property", "Test header", dataSource, "", "");

                // Make sure the cell is not in edit mode when setting the current cell.
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
                dataGridView.DataSource = dataSource;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[0];
                dataGridView.CurrentCell = dataGridViewCell;

                // Precondition
                Assert.IsFalse(dataGridView.IsCurrentCellInEditMode);

                // Call
                gridTester.FireEvent("CellClick", new DataGridViewCellEventArgs(0, 0));

                // Assert
                Assert.IsTrue(dataGridView.IsCurrentCellInEditMode);
                var combobox = (ComboBox) dataGridView.EditingControl;
                Assert.IsTrue(combobox.DroppedDown);
            }
        }

        #endregion
    }
}