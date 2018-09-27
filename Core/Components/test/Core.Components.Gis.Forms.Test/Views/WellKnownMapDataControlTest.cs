// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Components.Gis.Forms.Test.Views
{
    [TestFixture]
    public class WellKnownMapDataControlTest
    {
        [Test]
        public void Constructor_ActiveWellKnownTileSourceMapDataNull_DefaultValues()
        {
            // Call
            using (var control = new WellKnownMapDataControl(null))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(control);
                Assert.IsInstanceOf<BackgroundMapDataSelectionControl>(control);
                Assert.AreEqual("Bekende kaartlagen", control.DisplayName);
                Assert.IsNull(control.SelectedMapData);
            }
        }

        [Test]
        public void Constructor_ValidWellKnownTileSourceMapData_ExpectedProperties()
        {
            // Setup
            var activeWellKnownTileSourceMapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);

            // Call
            using (var control = new WellKnownMapDataControl(activeWellKnownTileSourceMapData))
            {
                // Assert
                AssertAreEqual(activeWellKnownTileSourceMapData, control.SelectedMapData);
            }
        }

        [Test]
        public void Constructor_InvalidWellKnownTileSourceMapData_ExpectedProperties()
        {
            // Setup
            var activeWellKnownTileSourceMapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)
            {
                Name = "invalid"
            };

            // Call
            using (var control = new WellKnownMapDataControl(activeWellKnownTileSourceMapData))
            {
                // Assert
                Assert.IsNull(control.SelectedMapData);
            }
        }

        [Test]
        public void WellKnownMapDataControl_WithData_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var random = new Random(124);
            var selectedWellKnownTileSourceMapData = new WellKnownTileSourceMapData(random.NextEnumValue<WellKnownTileSource>());

            using (var form = new Form())
            {
                // Call
                using (var control = new WellKnownMapDataControl(selectedWellKnownTileSourceMapData))
                {
                    form.Controls.Add(control);

                    // Assert
                    var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                    DataGridViewRowCollection rows = dataGridViewControl.Rows;
                    Assert.AreEqual(6, rows.Count);

                    string[] blaat =
                    {
                        "Bing Maps - Satelliet",
                        "Bing Maps - Satelliet + Wegen",
                        "Bing Maps - Wegen",
                        "Esri World - Reliëf",
                        "Esri World - Topografisch",
                        "OpenStreetMap"
                    };

                    string selectedWellKnownTileSourceMapDataDisplayName = TypeUtils.GetDisplayName(selectedWellKnownTileSourceMapData.TileSource);
                    int expectedIndex = -1;
                    for (var index = 0; index < blaat.Length; index++)
                    {
                        DataGridViewCellCollection cells = rows[index].Cells;
                        Assert.AreEqual(1, cells.Count);
                        Assert.AreEqual(blaat[index], cells[0].FormattedValue);
                        if (selectedWellKnownTileSourceMapDataDisplayName.Equals(cells[0].FormattedValue))
                        {
                            expectedIndex = index;
                        }
                    }

                    DataGridViewRow currentRow = dataGridViewControl.CurrentRow;
                    Assert.IsNotNull(currentRow);
                    Assert.AreEqual(expectedIndex, currentRow.Cells[0].RowIndex);
                }
            }
        }

        [Test]
        public void SelectedMapData_DifferentRowSelected_UpdatesSelectedMapData()
        {
            // Setup
            var selectedWellKnownTileSourceMapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);
            var selectionChanged = 0;

            using (var form = new Form())
            using (var control = new WellKnownMapDataControl(selectedWellKnownTileSourceMapData))
            {
                form.Controls.Add(control);
                var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl", form).TheObject;
                dataGridViewControl.CurrentRowChanged += (sender, args) => selectionChanged++;
                DataGridViewRow row = dataGridViewControl.GetRowFromIndex(2);

                dataGridViewControl.SetCurrentCell(row.Cells[0]);

                // Call
                ImageBasedMapData actualImageBasedMapData = control.SelectedMapData;

                // Assert
                AssertAreEqual(row.DataBoundItem as WellKnownTileSourceMapData, actualImageBasedMapData);
                Assert.AreEqual(1, selectionChanged);
            }
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup

            // Call
            TestDelegate call = () =>
            {
                using (var control = new WellKnownMapDataControl(null))
                {
                    control.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
        }

        private static void AssertAreEqual(WellKnownTileSourceMapData expected, ImageBasedMapData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            var actualWellKnownTileSourceMapData = (WellKnownTileSourceMapData) actual;
            Assert.AreEqual(expected.Name, actualWellKnownTileSourceMapData.Name);
            Assert.AreEqual(expected.TileSource, actualWellKnownTileSourceMapData.TileSource);
        }
    }
}