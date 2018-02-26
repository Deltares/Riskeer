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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
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
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (var view = new TestFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void Constructor_FailureMechanismSectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestFailureMechanismResultView(null, new TestFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResults", exception.ParamName);
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

                Assert.AreEqual("Test", dataGridView.Columns[nameColumnIndex].HeaderText);
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
                sectionResults.Add(FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult());
                sectionResults.NotifyObservers();

                // Then
                Assert.AreEqual(1, dataGridView.RowCount);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenSingleFailureMechanismSectionResultNotifiesObservers_ThenDataGridViewInvalidated()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (ShowFailureMechanismResultsView(sectionResults))
            {
                var invalidated = false;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // Precondition
                Assert.IsFalse(invalidated);

                // When
                sectionResult.NotifyObservers();

                // Then
                Assert.IsTrue(invalidated);
            }
        }

        [Test]
        public void GivenFailureMechanismResultViewWithFormattingRules_WhenCellFormattingEventFired_ThenFormattingRulesEvaluated()
        {
            // Given
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult()
            };

            // When
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                // Then
                Assert.IsTrue(view.Evaluated);
            }
        }

        [Test]
        public void GivenFailureMechanismResultViewWithFormatting_WhenRulesMeet_ThenRuleMeetActionPerformed()
        {
            // Given
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult()
            };

            // When
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                // Then
                Assert.IsTrue(view.RulesMeetActionPerformed);
                Assert.IsFalse(view.RulesDoNotMeetActionPerformed);
            }
        }

        [Test]
        public void GivenFailureMechanismResultViewWithFormatting_WhenRulesDoNotMeet_ThenRuleDoNotMeetActionPerformed()
        {
            // Given
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult("Other name")
            };

            // When
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                // Then
                Assert.IsFalse(view.RulesMeetActionPerformed);
                Assert.IsTrue(view.RulesDoNotMeetActionPerformed);
            }
        }

        [Test]
        public void GivenFailureMechanismResultViewWithFormatting_WhenRulesDoNotMeetAndNoActionGiven_ThenNoActionPerformed()
        {
            // Given
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult()
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                //Precondition
                Assert.IsFalse(view.RulesDoNotMeetActionPerformed);

                // When
                dataGridView.Refresh();

                // Then
                Assert.IsFalse(view.RulesDoNotMeetActionPerformed);
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

    public class TestFailureMechanismResultView : FailureMechanismResultView<FailureMechanismSectionResult, FailureMechanismSectionResultRow<FailureMechanismSectionResult>, TestFailureMechanism>
    {
        public TestFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> failureMechanismSectionResults, TestFailureMechanism failureMechanism)
            : base(failureMechanismSectionResults, failureMechanism) {}

        public bool Evaluated { get; private set; }

        public bool RulesMeetActionPerformed { get; private set; }

        public bool RulesDoNotMeetActionPerformed { get; private set; }

        protected override FailureMechanismSectionResultRow<FailureMechanismSectionResult> CreateFailureMechanismSectionResultRow(FailureMechanismSectionResult sectionResult)
        {
            return new TestRow(sectionResult);
        }

        protected override void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn("Name", "Test", true);
        }

        protected override IEnumerable<DataGridViewColumnFormattingRule<FailureMechanismSectionResultRow<FailureMechanismSectionResult>>> GetFormattingRules()
        {
            yield return new DataGridViewColumnFormattingRule<FailureMechanismSectionResultRow<FailureMechanismSectionResult>>(
                new[]
                {
                    0
                },
                new Func<FailureMechanismSectionResultRow<FailureMechanismSectionResult>, bool>[]
                {
                    row =>
                    {
                        Evaluated = true;
                        return row.Name.Equals("test");
                    }
                },
                (i, i1) => RulesMeetActionPerformed = true,
                (i, i1) => RulesDoNotMeetActionPerformed = true);

            yield return new DataGridViewColumnFormattingRule<FailureMechanismSectionResultRow<FailureMechanismSectionResult>>(
                new[]
                {
                    0
                },
                new Func<FailureMechanismSectionResultRow<FailureMechanismSectionResult>, bool>[]
                {
                    row => row.Name.Equals("Vak 1")
                },
                (i, i1) => {},
                null);
        }
    }

    public class TestRow : FailureMechanismSectionResultRow<FailureMechanismSectionResult>
    {
        public TestRow(FailureMechanismSectionResult sectionResult) : base(sectionResult) {}
    }
}