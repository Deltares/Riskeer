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
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismResultViewTest
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
            // Setup 
            const int nameColumnIndex = 0;
            const int assessmentLayerOneIndex = 1;

            // Call
            using (ShowFailureMechanismResultsView())
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(2, dataGridView.ColumnCount);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);

                Assert.AreEqual("Vak", dataGridView.Columns[nameColumnIndex].HeaderText);                
                Assert.AreEqual("Toetslaag 1", dataGridView.Columns[assessmentLayerOneIndex].HeaderText);
            }
        }

        [Test]
        public void Data_SetOtherThanFailureMechanismSectionResultListData_DataNullAndDataGridViewEmtpy()
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

    public class TestFailureMechanismResultView : FailureMechanismResultView<FailureMechanismSectionResult>
    {
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