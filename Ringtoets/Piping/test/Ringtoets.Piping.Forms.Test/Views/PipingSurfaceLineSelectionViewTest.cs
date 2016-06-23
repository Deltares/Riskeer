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
        private Form testForm;
        private const int surfaceLineNameColumnIndex = 1;
        private const int selectedColumnIndex = 0;

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

            var surfaceLineDataGrid = (DataGridView)new ControlTester("SurfaceLineDataGrid").TheObject;

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
            var view = new PipingSurfaceLineSelectionView(new []
            {
                ringtoetsPipingSurfaceLine
            });

            // Assert
            ShowPipingCalculationsView(view);
            var surfaceLineDataGrid = (DataGridView)new ControlTester("SurfaceLineDataGrid").TheObject;

            Assert.AreEqual(1, surfaceLineDataGrid.RowCount);
            Assert.IsFalse((bool)surfaceLineDataGrid.Rows[0].Cells[selectedColumnIndex].Value);
            Assert.AreEqual(testname, (string)surfaceLineDataGrid.Rows[0].Cells[surfaceLineNameColumnIndex].Value);
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
            var surfaceLineDataGrid = (DataGridView)new ControlTester("SurfaceLineDataGrid").TheObject;
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

            var surfaceLineDataGrid = (DataGridView)new ControlTester("SurfaceLineDataGrid").TheObject;
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

            var surfaceLineDataGrid = (DataGridView)new ControlTester("SurfaceLineDataGrid").TheObject;
            surfaceLineDataGrid.Rows[1].Cells[selectedColumnIndex].Value = true;
            surfaceLineDataGrid.Rows[3].Cells[selectedColumnIndex].Value = true;

            // Call
            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines = view.GetSelectedSurfaceLines();

            // Assert
            CollectionAssert.AreEqual(new[] { ringtoetsPipingSurfaceLine2, ringtoetsPipingSurfaceLine4 }, surfaceLines);
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

            var surfaceLineDataGrid = (DataGridView)new ControlTester("SurfaceLineDataGrid").TheObject;
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