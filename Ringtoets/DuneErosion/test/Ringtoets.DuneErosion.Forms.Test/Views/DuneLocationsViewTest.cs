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

using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.Views;

namespace Ringtoets.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int locationNameColumnIndex = 1;
        private const int locationIdColumnIndex = 2;
        private const int locationColumnIndex = 3;
        private const int coastalAreaIdColumnIndex = 4;
        private const int offsetColumnIndex = 5;
        private const int waterLevelColumnIndex = 6;
        private const int waveHeightColumnIndex = 7;
        private const int wavePeriodColumnIndex = 8;
        private const int d50ColumnIndex = 9;

        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new DuneLocationsView())
            {
                // Assert
                Assert.IsInstanceOf<CalculatableView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowDuneLocationsView();

            // Assert
            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(10, dataGridView.ColumnCount);

            var locationCalculateColumn = (DataGridViewCheckBoxColumn)dataGridView.Columns[locationCalculateColumnIndex];
            Assert.AreEqual("Berekenen", locationCalculateColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[locationColumnIndex];
            Assert.AreEqual("Coördinaten [m]", locationColumn.HeaderText);

            var coastalAreaIdColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[coastalAreaIdColumnIndex];
            Assert.AreEqual("Kustvaknummer", coastalAreaIdColumn.HeaderText);

            var offssetColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[offsetColumnIndex];
            Assert.AreEqual("Metrering [dam]", offssetColumn.HeaderText);

            var waterLevelColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[waterLevelColumnIndex];
            Assert.AreEqual("Rekenwaarde waterstand [m+NAP]", waterLevelColumn.HeaderText);

            var waveHeightColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[waveHeightColumnIndex];
            Assert.AreEqual("Rekenwaarde Hs [m]", waveHeightColumn.HeaderText);

            var wavePeriodColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[wavePeriodColumnIndex];
            Assert.AreEqual("Rekenwaarde Tp [s]", wavePeriodColumn.HeaderText);

            var d50Column = (DataGridViewTextBoxColumn)dataGridView.Columns[d50ColumnIndex];
            Assert.AreEqual("Rekenwaarde d50 [m]", d50Column.HeaderText);

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);
            var button = (Button)buttonTester.TheObject;
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Data_DuneLocations_DataSet()
        {
            // Setup
            using (var view = new DuneLocationsView())
            {
                var hydraulicBoundaryLocations = Enumerable.Empty<DuneLocation>();

                // Call
                view.Data = hydraulicBoundaryLocations;

                // Assert
                Assert.AreSame(hydraulicBoundaryLocations, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanDuneLocations_DataNull()
        {
            // Setup
            using (var view = new DuneLocationsView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void DuneLocationsView_DataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredDuneLocationsView();

            // Assert
            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            var cells = rows[0].Cells;
            Assert.AreEqual(10, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("50", cells[coastalAreaIdColumnIndex].FormattedValue);
            Assert.AreEqual("320", cells[offsetColumnIndex].FormattedValue);
            Assert.AreEqual(0.000837.ToString(CultureInfo.CurrentCulture), cells[d50ColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[wavePeriodColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(10, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("60", cells[coastalAreaIdColumnIndex].FormattedValue);
            Assert.AreEqual("230", cells[offsetColumnIndex].FormattedValue);
            Assert.AreEqual(0.000123.ToString(CultureInfo.CurrentCulture), cells[d50ColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual(2.34.ToString(CultureInfo.CurrentCulture), cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual(3.45.ToString(CultureInfo.CurrentCulture), cells[wavePeriodColumnIndex].FormattedValue);
        }

        [Test]
        public void Selection_WithoutLocations_ReturnsNull()
        {
            // Call
            using (var view = new DuneLocationsView())
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void Selection_WithLocations_ReturnsSelectedLocationWrappedInContext()
        {
            // Call
            using (var view = ShowFullyConfiguredDuneLocationsView())
            {
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
                var selectedLocationRow = dataGridView.Rows[0];
                selectedLocationRow.Cells[0].Value = true;
        
                // Assert
                var selection = view.Selection as DuneLocationContext;
                var dataBoundItem = selectedLocationRow.DataBoundItem as DuneLocationRow;
        
                Assert.NotNull(selection);
                Assert.NotNull(dataBoundItem);
                Assert.AreSame(dataBoundItem.DuneLocation, selection.DuneLocation);
            }
        }

        private DuneLocationsView ShowFullyConfiguredDuneLocationsView()
        {
            var view = ShowDuneLocationsView();
            view.Data = new ObservableList<DuneLocation>
            {
                new DuneLocation(1, "1", new Point2D(1.0, 1.0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 50,
                    Offset = 320,
                    D50 = 0.000837
                }),
                new DuneLocation(2, "2", new Point2D(2.0, 2.0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 60,
                    Offset = 230,
                    D50 = 0.000123
                })
                {
                    Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                    {
                        WaterLevel = 1.23,
                        WaveHeight = 2.34,
                        WavePeriod = 3.45
                    })
                }
            };

            return view;
        }

        private DuneLocationsView ShowDuneLocationsView()
        {
            var view = new DuneLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}