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
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
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
                Assert.IsInstanceOf<DataGridView>(dataGridView);
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
                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
                Assert.AreEqual(DataGridViewEditMode.EditOnEnter, dataGridView.EditMode);
                Assert.AreEqual(DataGridViewColumnHeadersHeightSizeMode.AutoSize, dataGridView.ColumnHeadersHeightSizeMode);
                Assert.AreEqual(DataGridViewRowHeadersWidthSizeMode.DisableResizing, dataGridView.RowHeadersWidthSizeMode);
                Assert.IsFalse(dataGridView.AllowUserToResizeColumns);
                Assert.IsFalse(dataGridView.AllowUserToResizeRows);
                Assert.IsFalse(dataGridView.AllowUserToAddRows);
                Assert.IsFalse(dataGridView.AllowUserToDeleteRows);
                Assert.IsTrue(dataGridView.StandardTab);
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

                var columnData = dataGridView.Columns[0];
                Assert.IsTrue(columnData.GetType() == typeof(DataGridViewTextBoxColumn));
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

                var columnData = dataGridView.Columns[0];
                Assert.IsTrue(columnData.GetType() == typeof(DataGridViewCheckBoxColumn));
                Assert.AreEqual(propertyName, columnData.DataPropertyName);
                Assert.AreEqual(string.Format("column_{0}", propertyName), columnData.Name);
                Assert.AreEqual(headerText, columnData.HeaderText);
            }
        }

        [Test]
        public void DataGridViewControl_EditValueDirtyStateChangedEventFired_ValueCommittedCellInEditMode()
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
            }
        }
    }
}