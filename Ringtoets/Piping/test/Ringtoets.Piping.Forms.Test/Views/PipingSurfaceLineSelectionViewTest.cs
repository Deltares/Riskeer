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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingSurfaceLineSelectionViewTest
    {
        private const int surfaceLineNameColumnIndex = 1;
        private const int selectedColumnIndex = 0;
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
        public void Constructor_SurfaceLinesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingSurfaceLineSelectionView(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("surfaceLines", parameter);
        }

        [Test]
        public void Constructor_SurfaceLinesEmpty_DefaultProperties()
        {
            // Call
            var view = new PipingSurfaceLineSelectionView(new List<RingtoetsPipingSurfaceLine>());

            // Assert
            ShowPipingCalculationsView(view);

            var surfaceLineDataGrid = (DataGridView) new ControlTester("SurfaceLineDataGrid").TheObject;

            Assert.AreEqual(2, surfaceLineDataGrid.ColumnCount);
            Assert.IsFalse(surfaceLineDataGrid.RowHeadersVisible);

            var selectedColumn = (DataGridViewCheckBoxColumn) surfaceLineDataGrid.Columns[0];
            var surfaceLineNameColumn = (DataGridViewTextBoxColumn) surfaceLineDataGrid.Columns[1];

            Assert.AreEqual("Selected", selectedColumn.DataPropertyName);
            Assert.AreEqual("Gebruiken", selectedColumn.HeaderText);
            Assert.AreEqual(60, selectedColumn.Width);
            Assert.IsFalse(selectedColumn.ReadOnly);

            Assert.AreEqual("Name", surfaceLineNameColumn.DataPropertyName);
            Assert.AreEqual("Profielschematisatie", surfaceLineNameColumn.HeaderText);
            Assert.AreEqual(DataGridViewAutoSizeColumnMode.Fill, surfaceLineNameColumn.AutoSizeMode);
            Assert.IsTrue(surfaceLineNameColumn.ReadOnly);

            Assert.AreEqual(0, surfaceLineDataGrid.RowCount);
        }

        [Test]
        public void Constructor_SurfaceLinesOneEntry_OneRowInGrid()
        {
            // Setup
            var testname = "testName";
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            ringtoetsPipingSurfaceLine.Name = testname;

            // Call
            var view = new PipingSurfaceLineSelectionView(new[]
            {
                ringtoetsPipingSurfaceLine
            });

            // Assert
            ShowPipingCalculationsView(view);
            var surfaceLineDataGrid = (DataGridView) new ControlTester("SurfaceLineDataGrid").TheObject;

            Assert.AreEqual(1, surfaceLineDataGrid.RowCount);
            Assert.IsFalse((bool) surfaceLineDataGrid.Rows[0].Cells[selectedColumnIndex].Value);
            Assert.AreEqual(testname, (string) surfaceLineDataGrid.Rows[0].Cells[surfaceLineNameColumnIndex].Value);
        }

        [Test]
        public void OnSelectAllClicked_WithSurfaceLines_AllSurfaceLinesSelected()
        {
            // Setup
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine2 = new RingtoetsPipingSurfaceLine();

            var view = new PipingSurfaceLineSelectionView(new[]
            {
                ringtoetsPipingSurfaceLine,
                ringtoetsPipingSurfaceLine2
            });

            ShowPipingCalculationsView(view);
            var selectAllButtonTester = new ButtonTester("SelectAllButton");

            // Call
            selectAllButtonTester.Click();

            // Assert
            var surfaceLineDataGrid = (DataGridView) new ControlTester("SurfaceLineDataGrid").TheObject;
            for (int i = 0; i < surfaceLineDataGrid.RowCount; i++)
            {
                var row = surfaceLineDataGrid.Rows[i];
                Assert.IsTrue((bool) row.Cells[selectedColumnIndex].Value);
            }
        }

        [Test]
        public void OnSelectNoneClicked_WithSurfaceLines_AllSurfaceLinesDeselected()
        {
            // Setup
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine2 = new RingtoetsPipingSurfaceLine();

            var view = new PipingSurfaceLineSelectionView(new[]
            {
                ringtoetsPipingSurfaceLine,
                ringtoetsPipingSurfaceLine2
            });

            ShowPipingCalculationsView(view);
            var selectNoneButtonTester = new ButtonTester("SelectNoneButton");

            var surfaceLineDataGrid = (DataGridView) new ControlTester("SurfaceLineDataGrid").TheObject;
            for (int i = 0; i < surfaceLineDataGrid.RowCount; i++)
            {
                var row = surfaceLineDataGrid.Rows[i];
                row.Cells[selectedColumnIndex].Value = true;
            }

            // Call
            selectNoneButtonTester.Click();

            // Assert
            for (int i = 0; i < surfaceLineDataGrid.RowCount; i++)
            {
                var row = surfaceLineDataGrid.Rows[i];
                Assert.IsFalse((bool) row.Cells[selectedColumnIndex].Value);
            }
        }

        [Test]
        public void GetSelectedSurfaceLines_WithSurfaceLinesMultipleSelected_ReturnSelectedSurfaceLines()
        {
            // Setup
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine2 = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine3 = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine4 = new RingtoetsPipingSurfaceLine();

            var view = new PipingSurfaceLineSelectionView(new[]
            {
                ringtoetsPipingSurfaceLine,
                ringtoetsPipingSurfaceLine2,
                ringtoetsPipingSurfaceLine3,
                ringtoetsPipingSurfaceLine4
            });

            ShowPipingCalculationsView(view);

            var surfaceLineDataGrid = (DataGridView) new ControlTester("SurfaceLineDataGrid").TheObject;
            surfaceLineDataGrid.Rows[1].Cells[selectedColumnIndex].Value = true;
            surfaceLineDataGrid.Rows[3].Cells[selectedColumnIndex].Value = true;

            // Call
            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines = view.GetSelectedSurfaceLines();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                ringtoetsPipingSurfaceLine2,
                ringtoetsPipingSurfaceLine4
            }, surfaceLines);
        }

        [Test]
        public void GetSelectedSurfaceLines_WithSurfaceLinesNoneSelected_ReturnEmptySurfaceLinesCollection()
        {
            // Setup
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine2 = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine3 = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine4 = new RingtoetsPipingSurfaceLine();

            var view = new PipingSurfaceLineSelectionView(new[]
            {
                ringtoetsPipingSurfaceLine,
                ringtoetsPipingSurfaceLine2,
                ringtoetsPipingSurfaceLine3,
                ringtoetsPipingSurfaceLine4
            });

            ShowPipingCalculationsView(view);

            // Call
            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines = view.GetSelectedSurfaceLines();

            // Assert
            Assert.IsEmpty(surfaceLines);
        }

        [Test]
        public void GetSelectedSurfaceLines_WithSurfaceLinesAllSelected_ReturnAllSurfaceLines()
        {
            // Setup
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine2 = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine3 = new RingtoetsPipingSurfaceLine();
            var ringtoetsPipingSurfaceLine4 = new RingtoetsPipingSurfaceLine();

            var surfaceLineCollection = new[]
            {
                ringtoetsPipingSurfaceLine,
                ringtoetsPipingSurfaceLine2,
                ringtoetsPipingSurfaceLine3,
                ringtoetsPipingSurfaceLine4
            };
            var view = new PipingSurfaceLineSelectionView(surfaceLineCollection);

            ShowPipingCalculationsView(view);

            var surfaceLineDataGrid = (DataGridView) new ControlTester("SurfaceLineDataGrid").TheObject;
            surfaceLineDataGrid.Rows[0].Cells[selectedColumnIndex].Value = true;
            surfaceLineDataGrid.Rows[1].Cells[selectedColumnIndex].Value = true;
            surfaceLineDataGrid.Rows[2].Cells[selectedColumnIndex].Value = true;
            surfaceLineDataGrid.Rows[3].Cells[selectedColumnIndex].Value = true;

            // Call
            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines = view.GetSelectedSurfaceLines();

            // Assert
            CollectionAssert.AreEqual(surfaceLineCollection, surfaceLines);
        }

        [Test]
        public void GetSelectedSurfaceLines_WithEmptySurfaceLines_ReturnEmptySurfaceLinesCollection()
        {
            // Setup
            var view = new PipingSurfaceLineSelectionView(Enumerable.Empty<RingtoetsPipingSurfaceLine>());

            ShowPipingCalculationsView(view);

            // Call
            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines = view.GetSelectedSurfaceLines();

            // Assert
            Assert.IsEmpty(surfaceLines);
        }

        private void ShowPipingCalculationsView(PipingSurfaceLineSelectionView pipingCalculationsView)
        {
            testForm.Controls.Add(pipingCalculationsView);
            testForm.Show();
        }
    }
}