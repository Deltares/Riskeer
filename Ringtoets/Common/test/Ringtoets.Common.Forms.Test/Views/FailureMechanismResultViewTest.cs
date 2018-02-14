// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;

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
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (var view = new TestFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestFailureMechanismResultView(new ObservableList<FailureMechanismSectionResult>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup 
            const int nameColumnIndex = 0;

            // Call
            using (ShowFailureMechanismResultsView(new ObservableList<TestFailureMechanismSectionResult>()))
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(1, dataGridView.ColumnCount);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);

                Assert.AreEqual("Vak", dataGridView.Columns[nameColumnIndex].HeaderText);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismSectionResultCollectionUpdated_ThenObserverNotified()
        {
            // Given
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>();
            using (ShowFailureMechanismResultsView(sectionResults))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.RowCount);

                // When
                sectionResults.Add(new TestFailureMechanismSectionResult(new FailureMechanismSection("a", new[]
                {
                    new Point2D(0, 0)
                })));
                sectionResults.NotifyObservers();

                // Then
                Assert.AreEqual(1, dataGridView.RowCount);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenSingleFailureMechanismSectionResultNotifiesObservers_ThenCellFormattingEventFired()
        {
            // Given
            var sectionResult = new TestFailureMechanismSectionResult(new FailureMechanismSection("a", new[]
            {
                new Point2D(0, 0)
            }))
            {
                AssessmentLayerOne = AssessmentLayerOneState.NoVerdict
            };
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (ShowFailureMechanismResultsView(sectionResults))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var cellFormattingEventFired = false;
                dataGridView.CellFormatting += (sender, args) => cellFormattingEventFired = true;

                // When
                sectionResult.NotifyObservers();

                // Then
                Assert.IsTrue(cellFormattingEventFired);
            }
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultsView(IObservableEnumerable<FailureMechanismSectionResult> sectionResults)
        {
            var failureMechanismResultView = new TestFailureMechanismResultView(sectionResults, new TestFailureMechanism());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }

    public class TestFailureMechanismResultView : FailureMechanismResultView<FailureMechanismSectionResult, TestFailureMechanism>
    {
        public TestFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> failureMechanismSectionResults, TestFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism)
        {
            UpdateDataGridViewDataSource();
        }

        protected override object CreateFailureMechanismSectionResultRow(FailureMechanismSectionResult sectionResult)
        {
            return new TestRow(sectionResult);
        }
    }

    public class TestRow : FailureMechanismSectionResultRow<FailureMechanismSectionResult>
    {
        public TestRow(FailureMechanismSectionResult sectionResult) : base(sectionResult) {}
    }
}