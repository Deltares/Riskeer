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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismResultViewTest
    {
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
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var view = new PipingFailureMechanismResultView();

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
            Assert.IsNull(view.Data);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            var failureMechanismResultView = ShowFailureMechanismResultsView();

            // Assert
            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

            Assert.AreEqual(5, dataGridView.ColumnCount);

            foreach (var column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
            {
                Assert.AreEqual("This", column.ValueMember);
                Assert.AreEqual("DisplayName", column.DisplayMember);
            }

            foreach (var column in dataGridView.Columns.OfType<DataGridViewColumn>())
            {
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, column.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, column.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void Data_SetPipingFailureMechanismSectionResultListData_DataSet()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            var section = new FailureMechanismSection("test", points);
            var testData = new List<PipingFailureMechanismSectionResult> { new PipingFailureMechanismSectionResult(section) };

            var view = new PipingFailureMechanismResultView();

            // Call
            view.Data = testData;

            // Assert
            Assert.AreSame(testData, view.Data);
        }

        [Test]
        public void Data_SetOtherThanPipingFailureMechanismSectionResultListData_DataNull()
        {
            // Setup
            var testData = new object();
            var view = new PipingFailureMechanismResultView();

            // Call
            view.Data = testData;

            // Assert
            Assert.IsNull(view.Data);
        }

        [Test]
        public void PipingFailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredPipingFailureMechanismResultsView();

            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

            // Assert
            var rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            var cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
            Assert.IsFalse((bool) cells[assessmentLayerOneIndex].FormattedValue);
            Assert.AreEqual(string.Format("{0}", double.NaN), cells[assessmentLayerTwoAIndex].FormattedValue);
            Assert.AreEqual(string.Format("{0}", double.NaN), cells[assessmentLayerTwoBIndex].FormattedValue);
            Assert.AreEqual(string.Format("{0}", double.NaN), cells[assessmentLayerThreeIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
            Assert.IsFalse((bool) cells[assessmentLayerOneIndex].FormattedValue);
            Assert.AreEqual(string.Format("{0}", double.NaN), cells[assessmentLayerTwoAIndex].FormattedValue);
            Assert.AreEqual(string.Format("{0}", double.NaN), cells[assessmentLayerTwoBIndex].FormattedValue);
            Assert.AreEqual(string.Format("{0}", double.NaN), cells[assessmentLayerThreeIndex].FormattedValue);
        }

        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerTwoBIndex = 3;
        private const int assessmentLayerThreeIndex = 4;

        private PipingFailureMechanismResultView ShowFullyConfiguredPipingFailureMechanismResultsView()
        {
            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            var pipingFailureMechanismResultView = ShowFailureMechanismResultsView();
            pipingFailureMechanismResultView.Data = pipingFailureMechanism.PipingFailureMechanismSectionResults;

            return pipingFailureMechanismResultView;
        }

        private PipingFailureMechanismResultView ShowFailureMechanismResultsView()
        {
            PipingFailureMechanismResultView pipingFailureMechanismResultView = new PipingFailureMechanismResultView();
            testForm.Controls.Add(pipingFailureMechanismResultView);
            testForm.Show();

            return pipingFailureMechanismResultView;
        }
    }
}