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
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
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
            using (var view = new TestFailureMechanismResultView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (ShowFailureMechanismResultsView())
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(2, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.AreEqual(Resources.FailureMechanismResultView_InitializeDataGridView_Section_name, dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.IsTrue(dataGridView.Columns[nameColumnIndex].ReadOnly);

                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);
                Assert.AreEqual(Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one, dataGridView.Columns[assessmentLayerOneIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void Data_DataAlreadySetNewDataSet_DataSetAndDataGridViewUpdated()
        {
            // Setup
            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var points = new[]
                {
                    new Point2D(1, 2),
                    new Point2D(3, 4)
                };

                var section = new FailureMechanismSection("test", points);
                var sectionResult = new FailureMechanismSectionResult(section);
                var testData = new List<FailureMechanismSectionResult>
                {
                    sectionResult
                };

                // Precondition
                Assert.AreEqual(2, dataGridView.RowCount);

                // Call
                view.Data = testData;

                // Assert
                Assert.AreSame(testData, view.Data);

                Assert.AreEqual(testData.Count, dataGridView.RowCount);
                Assert.AreEqual(sectionResult.Section.Name, dataGridView.Rows[0].Cells[0].Value);
            }
        }

        [Test]
        public void Data_SetOtherThanFailureMechanismSectionResultListData_DataNullAndDataGridViewOneEmtpyRow()
        {
            // Setup
            var testData = new object();
            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                view.Data = testData;

                // Assert
                Assert.IsNull(view.Data);

                Assert.AreEqual(0, dataGridView.RowCount);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                var rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                var cells = rows[0].Cells;
                Assert.AreEqual(2, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(false, cells[assessmentLayerOneIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(2, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(false, cells[assessmentLayerOneIndex].FormattedValue);
            }
        }

        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;

        private TestFailureMechanismResultView ShowFullyConfiguredFailureMechanismResultsView()
        {
            var failureMechanism = new TestFailureMechanism();

            failureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            failureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            var failureMechanismResultView = ShowFailureMechanismResultsView();
            failureMechanismResultView.Data = failureMechanism.SectionResults;
            failureMechanismResultView.FailureMechanism = failureMechanism;

            return failureMechanismResultView;
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultsView()
        {
            TestFailureMechanismResultView failureMechanismResultView = new TestFailureMechanismResultView();
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }

    public class TestFailureMechanismResultView : FailureMechanismResultView<FailureMechanismSectionResult> {

        protected override object CreateFailureMechanismSectionResultRow(FailureMechanismSectionResult sectionResult)
        {
            return new TestRow(sectionResult);
        }
    }

    public class TestRow
    {
        public TestRow(FailureMechanismSectionResult sectionResult)
        {
            Name = sectionResult.Section.Name;
        }

        public string Name { get; private set; }
    }
}