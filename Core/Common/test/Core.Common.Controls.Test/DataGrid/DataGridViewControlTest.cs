// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.Test.DataGrid
{
    [TestFixture]
    public class DataGridViewControlTest
    {
        private const string propertyName = "PropertyName";
        private const string headerText = "HeaderText";

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
                Assert.AreEqual(-1, control.LastSelectedRow);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            using (var control = new DataGridViewControl())
            {
                // Call
                var dataGridView = (DoubleBufferedDataGridView) control.Controls[0];

                // Assert
                Assert.AreEqual(0, dataGridView.ColumnCount);
                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
                Assert.AreEqual(DataGridViewEditMode.EditProgrammatically, dataGridView.EditMode);
                Assert.AreEqual(DataGridViewColumnHeadersHeightSizeMode.DisableResizing, dataGridView.ColumnHeadersHeightSizeMode);
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
        public void Show_DataGridViewCorrectlyInitialized()
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
                Assert.AreEqual(DataGridViewEditMode.EditProgrammatically, dataGridView.EditMode);
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
                control.SetDataSource(new[]
                {
                    ""
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);
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

                control.SetDataSource(new[]
                {
                    ""
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

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
        [TestCase(true)]
        [TestCase(false)]
        public void AddTextBoxColumn_WithoutAutoSizeMode_AddsColumnToDataGridViewWithDefaultAutoSizeMode(bool readOnly)
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddTextBoxColumn(propertyName, headerText, readOnly);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
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
            const DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            const int minimumWidth = 100;
            const string format = "1/#,#";

            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddTextBoxColumn(propertyName, headerText, readOnly, autoSizeMode, minimumWidth, format);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
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
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddCheckBoxColumn(propertyName, headerText);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewCheckBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void AddCheckboxColumn_ReadOnlySet_AddsColumnToDataGridViewWithReadOnlyTrue()
        {
            // Setup
            const DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddCheckBoxColumn(propertyName, headerText, true, autoSizeMode);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewCheckBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.IsTrue(columnData.ReadOnly);
                Assert.AreEqual(autoSizeMode, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void AddCheckboxColumn_AutoSizeModeSet_AddsColumnToDataGridViewWithAutoSizeMode()
        {
            // Setup
            const DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddCheckBoxColumn(propertyName, headerText, false, autoSizeMode);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewCheckBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
                Assert.AreEqual(autoSizeMode, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void AddComboBoxColumn_AutoSizeModeSet_AddsColumnToDataGridViewWithAutoSizeMode()
        {
            const DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddComboBoxColumn<object>(propertyName, headerText, null, null, null, autoSizeMode);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];

                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.IsNull(columnData.DataSource);
                Assert.IsEmpty(columnData.ValueMember);
                Assert.IsEmpty(columnData.DisplayMember);
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
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddComboBoxColumn<object>(propertyName, headerText, null, null, null);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];

                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.IsNull(columnData.DataSource);
                Assert.IsEmpty(columnData.ValueMember);
                Assert.IsEmpty(columnData.DisplayMember);
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
                    nameof(EnumDisplayWrapper<TestEnum>.Value),
                    nameof(EnumDisplayWrapper<TestEnum>.DisplayName)
                );

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];

                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.AreSame(dataSource, columnData.DataSource);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void AddColorColumn_WithoutAutoSizeMode_AddsReadOnlyColumnToDataGridViewWithDefaultAutoSizeMode()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddColorColumn(propertyName, headerText);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewColorColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.IsTrue(columnData.ReadOnly);
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, columnData.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, columnData.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void AddColorColumn_WithAutoSizeMode_AddsColumnToDataGridViewWithAutoSizeMode()
        {
            // Setup
            const DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                control.AddColorColumn(propertyName, headerText, autoSizeMode);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewColorColumn) dataGridView.Columns[0];
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual($"column_{propertyName}", columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
                Assert.IsTrue(columnData.ReadOnly);
                Assert.AreEqual(autoSizeMode, columnData.AutoSizeMode);
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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    "Test"
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    "Test"
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

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

                control.AddTextBoxColumn("Test property", "Test header");

                control.SetDataSource(new[]
                {
                    "Test"
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    "Test"
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

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

                control.SetDataSource(new[]
                {
                    "Test"
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);
                dataGridView.BeginEdit(false);

                // Precondition
                Assert.IsTrue(control.IsCurrentCellInEditMode);

                // Call
                control.EndEdit();

                // Assert
                Assert.IsFalse(control.IsCurrentCellInEditMode);
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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    "Row 1",
                    "Row 2"
                });

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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });

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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });

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
                control.SetDataSource(new[]
                {
                    ""
                });
                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                dataGridView.CurrentCell = dataGridViewCell;

                // Call
                DataGridViewRow row = control.CurrentRow;

                // Assert
                Assert.IsNotNull(row);
            }
        }

        [Test]
        public void GetCurrentRow_CurrentCellNull_ReturnsNull()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });
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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });

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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });

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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });

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

                const string dataPropertyName = "Test property";
                const string testHeader = "Test header";
                control.AddTextBoxColumn(dataPropertyName, testHeader);

                control.SetDataSource(new[]
                {
                    ""
                });

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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });

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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });

                // Call
                control.DisableCell(0, 0);

                // Assert
                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                Assert.IsTrue(dataGridViewCell.ReadOnly);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), dataGridViewCell.Style.BackColor);
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), dataGridViewCell.Style.ForeColor);
            }
        }

        [Test]
        public void ClearCurrentCell_Always_SetsCurrentCellToNullAndResetsLastSelectedRow()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                control.AddTextBoxColumn("Test property", "Test header"); // Set read-only state of the column to the opposite
                control.SetDataSource(new[]
                {
                    ""
                });

                dataGridView.CurrentCell = control.GetCell(0, 0);

                // Precondition
                Assert.IsNotNull(dataGridView.CurrentCell);

                // Call
                control.ClearCurrentCell();

                // Assert
                Assert.IsNull(dataGridView.CurrentCell);
                Assert.AreEqual(-1, control.LastSelectedRow);
            }
        }

        [Test]
        public void SetCurrentCell_ValidCell_SetsCurrentCell()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                control.AddTextBoxColumn("Test property", "Test header");
                control.SelectionMode = DataGridViewSelectionMode.CellSelect;

                control.SetDataSource(new[]
                {
                    ""
                });

                control.ClearCurrentCell();
                DataGridViewCell firstcell = control.GetCell(0, 0);

                // Call
                control.SetCurrentCell(firstcell);

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreSame(firstcell, dataGridView.CurrentCell);
                Assert.IsTrue(firstcell.Selected);
            }
        }

        [Test]
        public void SetCurrentCell_CellIsHeaderCell_ThrowsArgumentException()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });

                DataGridViewCell firstcell = control.CurrentRow.HeaderCell;

                // Call
                TestDelegate test = () => control.SetCurrentCell(firstcell);

                // Assert
                const string message = "Unable to set the cell active.";
                var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
                Assert.AreEqual("cell", exception.ParamName);
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(exception.InnerException);
            }
        }

        [Test]
        public void SetCurrentCell_ValidCellButRowIsHidden_ThrowsArgumentException()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    "",
                    ""
                });

                DataGridViewRow dataGridViewRow = control.Rows[1];
                dataGridViewRow.Visible = false;

                // Call
                TestDelegate test = () => control.SetCurrentCell(dataGridViewRow.Cells[0]);

                // Assert
                const string message = "Unable to set the cell active.";
                var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
                Assert.AreEqual("cell", exception.ParamName);
                Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            }
        }

        [Test]
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

                control.AddTextBoxColumn("Test property", "Test header", columnReadOnlyState);
                control.SetDataSource(new[]
                {
                    ""
                });

                control.DisableCell(0, 0);

                // Precondition
                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
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

        [Test]
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

                control.AddTextBoxColumn("Test property", "Test header", !specificReadOnlyState); // Set read-only state of the column to the opposite
                control.SetDataSource(new[]
                {
                    ""
                });

                control.DisableCell(0, 0);

                // Precondition
                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SetColumnVisibility_IsVisible_SetColumnToGivenVisibility(bool isVisible)
        {
            // Setup
            using (var control = new DataGridViewControl())
            {
                control.AddTextBoxColumn("Test property", "Test header");

                DataGridViewColumn dataGridViewColumn = control.GetColumnFromIndex(0);

                dataGridViewColumn.Visible = !isVisible; // Set visible state of the column to the opposite

                // Precondition
                Assert.AreEqual(!isVisible, dataGridViewColumn.Visible);

                // Call
                control.SetColumnVisibility(0, isVisible);

                // Assert
                Assert.AreEqual(isVisible, dataGridViewColumn.Visible);
            }
        }

        private enum TestEnum
        {
            NoDisplayName,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.DataGridViewControlTest_DisplayNameValueDisplayName))]
            DisplayName
        }

        private class TestDataGridViewRow
        {
            public TestDataGridViewRow(RoundedDouble testRoundedDouble)
            {
                TestRoundedDouble = testRoundedDouble;
            }

            // Property needs to have a setter, otherwise some tests will fail
            public RoundedDouble TestRoundedDouble { get; set; }
        }

        private class TestDataGridViewMultipleRows
        {
            public TestDataGridViewMultipleRows(RoundedDouble testRoundedDouble)
            {
                TestRoundedDouble = testRoundedDouble;
            }

            public TestDataGridViewMultipleRows(RoundedDouble testRoundedDouble, string testString)
            {
                TestRoundedDouble = testRoundedDouble;
                TestString = testString;
            }

            public RoundedDouble TestRoundedDouble { get; }

            public string TestString { get; }
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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

                var counter = 0;
                control.AddCellFormattingHandler((sender, args) => counter++);

                // Precondition
                Assert.AreEqual(0, counter);

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });
                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

                var counter = 0;
                DataGridViewCellFormattingEventHandler dataGridViewCellFormattingEventHandler = (sender, args) => counter++;

                control.AddCellFormattingHandler(dataGridViewCellFormattingEventHandler);

                // Precondition
                Assert.AreEqual(0, counter);
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.
                Assert.AreEqual(1, counter);

                // Call
                control.RemoveCellFormattingHandler(dataGridViewCellFormattingEventHandler);

                // Assert
                object formattedValue2 = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void AddCurrentRowChangedHandler_Always_AddsEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                const double initialValue = 25;

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");

                var gridTester = new ControlTester("dataGridView");

                control.SetDataSource(new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, initialValue))
                });

                var counter = 0;
                control.AddCurrentRowChangedHandler((sender, args) => counter++);

                // Call
                gridTester.FireEvent("CurrentCellChanged", new EventArgs());

                // Assert
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void RemoveCurrentRowChangedHandler_Always_RemovesEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                const double initialValue = 25;

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");
                var gridTester = new ControlTester("dataGridView");

                control.SetDataSource(new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, initialValue))
                });

                var counter = 0;

                EventHandler eventHandler = (sender, args) => counter++;
                control.AddCurrentRowChangedHandler(eventHandler);

                // Precondition
                Assert.AreEqual(0, counter);
                gridTester.FireEvent("CurrentCellChanged", new EventArgs());
                Assert.AreEqual(1, counter);

                // Call
                control.RemoveCurrentRowChangedHandler(eventHandler);

                // Assert
                gridTester.FireEvent("CurrentCellChanged", new EventArgs());
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void AddCurrentCellChangedHandler_Always_AddsEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                const double initialValue = 25;

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");

                var gridTester = new ControlTester("dataGridView");

                control.SetDataSource(new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, initialValue))
                });

                var counter = 0;
                control.AddCurrentCellChangedHandler((sender, args) => counter++);

                // Call
                gridTester.FireEvent("CurrentCellChanged", new EventArgs());

                // Assert
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void RemoveCurrentCellChangedHandler_Always_RemovesEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                const double initialValue = 25;

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");
                var gridTester = new ControlTester("dataGridView");

                control.SetDataSource(new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, initialValue))
                });

                var counter = 0;

                EventHandler eventHandler = (sender, args) => counter++;
                control.AddCurrentCellChangedHandler(eventHandler);

                // Precondition
                Assert.AreEqual(0, counter);
                gridTester.FireEvent("CurrentCellChanged", new EventArgs());
                Assert.AreEqual(1, counter);

                // Call
                control.RemoveCurrentCellChangedHandler(eventHandler);

                // Assert
                gridTester.FireEvent("CurrentCellChanged", new EventArgs());
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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });
                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

                var counter = 0;
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

                control.AddTextBoxColumn("Test property", "Test header");
                control.SetDataSource(new[]
                {
                    ""
                });
                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

                var counter = 0;
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

                control.SetDataSource(new[]
                {
                    false
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);
                dataGridView.BeginEdit(false);
                gridTester.FireEvent("KeyUp", new KeyEventArgs(Keys.Space));

                // Call
                gridTester.FireEvent("CurrentCellDirtyStateChanged", EventArgs.Empty);

                // Assert
                Assert.IsTrue(dataGridViewCell.IsInEditMode);
                Assert.IsTrue(Convert.ToBoolean(dataGridViewCell.FormattedValue));
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
                control.SetDataSource(new[]
                {
                    "Test value"
                });
                DataGridViewCell dataGridViewCell = control.Rows[0].Cells[0];
                control.SetCurrentCell(dataGridViewCell);
                dataGridView.BeginEdit(false);

                // Call
                dataGridViewCell.Value = "New value";

                // Assert
                Assert.IsEmpty(dataGridViewCell.OwningRow.ErrorText);
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
                control.SetDataSource(new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, 25))
                });

                // Call
                control.Rows[0].Cells[0].Value = "test";

                // Assert
                Assert.AreEqual("De tekst moet een getal zijn.", control.Rows[0].ErrorText);
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

                control.SetDataSource(new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, 25))
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);
                const string newValue = "3";

                control.SetCurrentCell(dataGridViewCell);
                dataGridView.BeginEdit(false);
                dataGridViewCell.Value = newValue;

                // Precondition
                Assert.IsTrue(control.IsCurrentCellInEditMode);
                Assert.IsEmpty(control.Rows[0].ErrorText);

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

                const double initialValue = 25;

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                control.SetDataSource(new[]
                {
                    new TestDataGridViewRow(new RoundedDouble(0, initialValue))
                });

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);
                const string newValue = "test";

                control.SetCurrentCell(dataGridViewCell);
                dataGridView.BeginEdit(false);
                dataGridViewCell.Value = newValue;

                // Precondition
                Assert.IsTrue(control.IsCurrentCellInEditMode);
                Assert.AreEqual("De tekst moet een getal zijn.", control.Rows[0].ErrorText);

                // Call
                gridTester.FireEvent("Leave", EventArgs.Empty);

                // Assert
                Assert.IsEmpty(control.Rows[0].ErrorText);
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
                control.SetDataSource(dataSource);

                DataGridViewCell dataGridViewCell = control.GetCell(0, 0);
                control.SetCurrentCell(dataGridViewCell);

                // Precondition
                Assert.IsFalse(control.IsCurrentCellInEditMode);

                // Call
                gridTester.FireEvent("CellClick", new DataGridViewCellEventArgs(0, 0));

                // Assert
                Assert.IsTrue(control.IsCurrentCellInEditMode);
                var combobox = (ComboBox) dataGridView.EditingControl;
                Assert.IsTrue(combobox.DroppedDown);
            }
        }

        [Test]
        public void CurrentRowChangedHandler_SelectedCellInNewRow_ExecuteEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");
                control.AddTextBoxColumn("TestString", "Test string header");
                control.SetDataSource(new[]
                {
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 2.5), "hello world"),
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 8.3), "test")
                });
                control.SetCurrentCell(control.GetCell(0, 0));

                var handlerExecuted = false;
                control.AddCurrentRowChangedHandler((sender, args) => handlerExecuted = true);

                // Call
                control.SetCurrentCell(control.GetCell(1, 0));

                // Assert
                Assert.IsTrue(handlerExecuted);
            }
        }

        [Test]
        public void CurrentRowChangedHandler_FirstSelection_ExecuteEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");
                control.AddTextBoxColumn("TestString", "Test string header");
                control.SetDataSource(new[]
                {
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 2.5), "hello world"),
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 8.3), "test")
                });

                var handlerExecuted = false;
                control.AddCurrentRowChangedHandler((sender, args) => handlerExecuted = true);

                // Call
                control.SetCurrentCell(control.GetCell(0, 0));
                gridTester.FireEvent("CurrentCellChanged", EventArgs.Empty);

                // Assert
                Assert.IsTrue(handlerExecuted);
            }
        }

        [Test]
        public void CurrentRowChangedHandler_SelectCellInSameRow_SkipEventHandler()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");
                control.AddTextBoxColumn("TestString", "Test string header");
                control.SetDataSource(new[]
                {
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 2.5), "hello world"),
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 8.3), "test")
                });

                var handlerExecuted = false;
                control.AddCurrentRowChangedHandler((sender, args) => handlerExecuted = true);

                // Precondition
                control.SetCurrentCell(control.GetCell(0, 0));
                gridTester.FireEvent("CurrentCellChanged", EventArgs.Empty);
                Assert.IsTrue(handlerExecuted);
                handlerExecuted = false;

                // Call
                control.SetCurrentCell(control.GetCell(0, 1));
                gridTester.FireEvent("CurrentCellChanged", EventArgs.Empty);

                // Assert
                Assert.IsFalse(handlerExecuted);
            }
        }

        [Test]
        public void CurrentRowChangedHandler_SetCurrentCellToNull_ResetsLastSelectedRow()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");
                control.AddTextBoxColumn("TestString", "Test string header");
                control.SetDataSource(new[]
                {
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 2.5), "hello world"),
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 8.3), "test")
                });

                control.AddCurrentRowChangedHandler((sender, args) =>
                {
                    var i = 0;
                });

                // Precondition
                control.SetCurrentCell(control.GetCell(0, 0));
                gridTester.FireEvent("CurrentCellChanged", EventArgs.Empty);
                Assert.AreEqual(0, control.LastSelectedRow);

                // Call
                control.SetCurrentCell(null);

                // Assert
                Assert.AreEqual(-1, control.LastSelectedRow);
            }
        }

        [Test]
        public void CurrentRowChangedHandler_SetHandlerToNull_DoesNothing()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();

                var gridTester = new ControlTester("dataGridView");

                control.AddTextBoxColumn("TestRoundedDouble", "Test header");
                control.AddTextBoxColumn("TestString", "Test string header");
                control.SetDataSource(new[]
                {
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 2.5), "hello world"),
                    new TestDataGridViewMultipleRows(new RoundedDouble(0, 8.3), "test")
                });

                control.AddCurrentRowChangedHandler(null);

                // Precondition
                control.SetCurrentCell(control.GetCell(0, 0));
                gridTester.FireEvent("CurrentCellChanged", EventArgs.Empty);
                Assert.AreEqual(-1, control.LastSelectedRow);

                // Call
                control.SetCurrentCell(null);

                // Assert
                Assert.AreEqual(-1, control.LastSelectedRow);
                Assert.IsNull(control.CurrentRow);
            }
        }

        #endregion
    }
}