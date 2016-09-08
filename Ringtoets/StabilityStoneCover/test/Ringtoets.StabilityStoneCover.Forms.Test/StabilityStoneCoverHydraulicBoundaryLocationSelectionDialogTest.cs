// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Rhino.Mocks;
using Ringtoets.Common.Forms;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.Test
{
    [TestFixture]
    public class StabilityStoneCoverHydraulicBoundaryLocationSelectionDialogTest
    {
        private const int locationSelectionColumnIndex = 0;
        private const int locationColumnIndex = 1;
        
        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(null, Enumerable.Empty<IHydraulicBoundaryLocation>());

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
                TestDelegate test = () => new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, null);

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
                using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, Enumerable.Empty<IHydraulicBoundaryLocation>()))
                {
                    // Assert
                    Assert.IsInstanceOf<SelectionDialogBase<IHydraulicBoundaryLocation>>(dialog);
                    Assert.IsEmpty(dialog.SelectedItems);
                    Assert.AreEqual("Selecteer hydraulische randvoorwaardenlocaties", dialog.Text);
                }
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (var viewParent = new Form())
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, Enumerable.Empty<IHydraulicBoundaryLocation>()))
            {
                dialog.Show();

                // Assert
                Assert.IsEmpty(dialog.SelectedItems);

                var dataGridViewControl = (DataGridViewControl)new ControlTester("DataGridViewControl", dialog).TheObject;
                var dataGridView = dataGridViewControl.Controls.OfType<DataGridView>().First();
                Assert.AreEqual(2, dataGridView.ColumnCount);

                var locationCalculateColumn = (DataGridViewCheckBoxColumn)dataGridView.Columns[locationSelectionColumnIndex];
                const string expectedLocationCalculateHeaderText = "Gebruik";
                Assert.AreEqual(expectedLocationCalculateHeaderText, locationCalculateColumn.HeaderText);
                Assert.AreEqual("Selected", locationCalculateColumn.DataPropertyName);
                Assert.IsFalse(locationCalculateColumn.ReadOnly);

                var nameColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[locationColumnIndex];
                const string expectedNameHeaderText = "Hydraulische randvoorwaardenlocatie";
                Assert.AreEqual(expectedNameHeaderText, nameColumn.HeaderText);
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
            
            var mock = new MockRepository();
            var hydraulicBoundaryLocationMock = mock.Stub<IHydraulicBoundaryLocation>();
            hydraulicBoundaryLocationMock.Expect(h => h.Name).Return(testname);
            mock.ReplayAll();

            // Call
            using (var viewParent = new Form())
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(viewParent, new[]
            {
                hydraulicBoundaryLocationMock
            }))
            {
                // Assert
                dialog.Show();

                var dataGridViewControl = (DataGridViewControl)new ControlTester("DataGridViewControl").TheObject;
                Assert.AreEqual(1, dataGridViewControl.Rows.Count);
                Assert.IsFalse((bool)dataGridViewControl.Rows[0].Cells[locationSelectionColumnIndex].Value);
                Assert.AreEqual(testname, (string)dataGridViewControl.Rows[0].Cells[locationColumnIndex].Value);
            }
            mock.VerifyAll();
        }
    }
}