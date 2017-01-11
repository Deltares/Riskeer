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

using System.Linq;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;
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
        private const int offssetColumnIndex = 5;
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

            var offssetColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[offssetColumnIndex];
            Assert.AreEqual("Metrering [dam]", offssetColumn.HeaderText);

            var waterLevelColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[waterLevelColumnIndex];
            Assert.AreEqual("Rekenwaarde waterstand [m+NAP]", waterLevelColumn.HeaderText);

            var waveHeightColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[waveHeightColumnIndex];
            Assert.AreEqual("Hs [m]", waveHeightColumn.HeaderText);

            var wavePeriodColumn = (DataGridViewTextBoxColumn)dataGridView.Columns[wavePeriodColumnIndex];
            Assert.AreEqual("Tp [s]", wavePeriodColumn.HeaderText);

            var d50Column = (DataGridViewTextBoxColumn)dataGridView.Columns[d50ColumnIndex];
            Assert.AreEqual("d50 [m]", d50Column.HeaderText);

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

        private DuneLocationsView ShowDuneLocationsView()
        {
            var view = new DuneLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}