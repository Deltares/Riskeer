// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Forms.Test
{
    [TestFixture]
    public class HydraulicBoundaryLocationSelectionDialogTest
    {
        private const int locationSelectionColumnIndex = 0;
        private const int locationColumnIndex = 1;

        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocationSelectionDialog(null, Enumerable.Empty<HydraulicBoundaryLocation>());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void Constructor_WithoutHydraulicBoundaryLocations_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                TestDelegate test = () => new HydraulicBoundaryLocationSelectionDialog(viewParent, null);

                // Assert
                string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("hydraulicBoundaryLocations", parameter);
            }
        }

        [Test]
        public void Constructor_WithParentAndLocations_DefaultProperties()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                using (var dialog = new HydraulicBoundaryLocationSelectionDialog(viewParent, Enumerable.Empty<HydraulicBoundaryLocation>()))
                {
                    // Assert
                    Assert.IsInstanceOf<SelectionDialogBase<HydraulicBoundaryLocation>>(dialog);
                    CollectionAssert.IsEmpty(dialog.SelectedItems);
                    Assert.AreEqual("Selecteer hydraulische belastingenlocaties", dialog.Text);
                }
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (var viewParent = new Form())
            using (var dialog = new HydraulicBoundaryLocationSelectionDialog(viewParent, Enumerable.Empty<HydraulicBoundaryLocation>()))
            {
                dialog.Show();

                // Assert
                CollectionAssert.IsEmpty(dialog.SelectedItems);

                var dataGridViewControl = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                DataGridView dataGridView = dataGridViewControl.Controls.OfType<DataGridView>().First();
                Assert.AreEqual(2, dataGridView.ColumnCount);

                var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[locationSelectionColumnIndex];
                Assert.AreEqual("Gebruik", locationCalculateColumn.HeaderText);
                Assert.AreEqual("Selected", locationCalculateColumn.DataPropertyName);
                Assert.IsFalse(locationCalculateColumn.ReadOnly);

                var nameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationColumnIndex];
                Assert.AreEqual("Hydraulische belastingenlocatie", nameColumn.HeaderText);
                Assert.AreEqual("Name", nameColumn.DataPropertyName);
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.Fill, nameColumn.AutoSizeMode);
                Assert.IsTrue(nameColumn.ReadOnly);
            }
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationOneEntry_OneRowInGrid()
        {
            // Setup
            const string testname = "testName";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, testname, 0, 0);

            // Call
            using (var viewParent = new Form())
            using (var dialog = new HydraulicBoundaryLocationSelectionDialog(viewParent, new[]
            {
                hydraulicBoundaryLocation
            }))
            {
                // Assert
                dialog.Show();

                var dataGridViewControl = (DataGridViewControl) new ControlTester("DataGridViewControl").TheObject;
                Assert.AreEqual(1, dataGridViewControl.Rows.Count);
                Assert.IsFalse((bool) dataGridViewControl.Rows[0].Cells[locationSelectionColumnIndex].Value);
                Assert.AreEqual(testname, (string) dataGridViewControl.Rows[0].Cells[locationColumnIndex].Value);
            }
        }
    }
}