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

using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Forms;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test
{
    [TestFixture]
    public class PipingSurfaceLineSelectionDialogTest
    {
        private const int selectItemColumnIndex = 0;
        private const int nameColumnIndex = 1;

        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingSurfaceLineSelectionDialog(null, Enumerable.Empty<PipingSurfaceLine>());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void Constructor_WithoutSurfaceLines_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                TestDelegate test = () => new PipingSurfaceLineSelectionDialog(viewParent, null);

                // Assert
                string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("surfaceLines", parameter);
            }
        }

        [Test]
        public void Constructor_WithParentAndSurfaceLines_DefaultProperties()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                using (var dialog = new PipingSurfaceLineSelectionDialog(viewParent, Enumerable.Empty<PipingSurfaceLine>()))
                {
                    // Assert
                    Assert.IsInstanceOf<SelectionDialogBase<PipingSurfaceLine>>(dialog);
                    CollectionAssert.IsEmpty(dialog.SelectedItems);
                    Assert.AreEqual("Selecteer profielschematisaties", dialog.Text);
                }
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (var viewParent = new Form())
            using (var dialog = new PipingSurfaceLineSelectionDialog(viewParent, Enumerable.Empty<PipingSurfaceLine>()))
            {
                dialog.Show();

                // Assert
                CollectionAssert.IsEmpty(dialog.SelectedItems);

                var dataGridViewControl = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                DataGridView dataGridView = dataGridViewControl.Controls.OfType<DataGridView>().First();
                Assert.AreEqual(2, dataGridView.ColumnCount);

                var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[selectItemColumnIndex];
                const string expectedLocationCalculateHeaderText = "Gebruik";
                Assert.AreEqual(expectedLocationCalculateHeaderText, locationCalculateColumn.HeaderText);
                Assert.AreEqual("Selected", locationCalculateColumn.DataPropertyName);
                Assert.IsFalse(locationCalculateColumn.ReadOnly);

                var nameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[nameColumnIndex];
                const string expectedNameHeaderText = "Profielschematisatie";
                Assert.AreEqual(expectedNameHeaderText, nameColumn.HeaderText);
                Assert.AreEqual("Name", nameColumn.DataPropertyName);
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.Fill, nameColumn.AutoSizeMode);
                Assert.IsTrue(nameColumn.ReadOnly);
            }
        }

        [Test]
        public void Constructor_SurfaceLinesOneEntry_OneRowInGrid()
        {
            // Setup
            const string testname = "testName";
            var pipingSurfaceLine = new PipingSurfaceLine(testname);

            // Call
            using (var viewParent = new Form())
            using (var dialog = new PipingSurfaceLineSelectionDialog(viewParent, new[]
            {
                pipingSurfaceLine
            }))
            {
                // Assert
                dialog.Show();

                var dataGridViewControl = (DataGridViewControl) new ControlTester("DataGridViewControl").TheObject;
                Assert.AreEqual(1, dataGridViewControl.Rows.Count);
                Assert.IsFalse((bool) dataGridViewControl.Rows[0].Cells[selectItemColumnIndex].Value);
                Assert.AreEqual(testname, (string) dataGridViewControl.Rows[0].Cells[nameColumnIndex].Value);
            }
        }
    }
}