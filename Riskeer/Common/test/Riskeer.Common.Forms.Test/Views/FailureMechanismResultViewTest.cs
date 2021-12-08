﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismResultViewTest
    {
        private Form testForm;

        private static IEnumerable<TestCaseData> CellFormattingStates
        {
            get
            {
                yield return new TestCaseData(true, "", CellStyle.Disabled);
                yield return new TestCaseData(false, "Error", CellStyle.Enabled);
            }
        }

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
        public void Constructor_FailureMechanismSectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestFailureMechanismResultView(null, new TestFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResults", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestFailureMechanismResultView(new ObservableList<FailureMechanismSectionResult>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (var view = new TestFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreEqual(2, view.Controls.Count);

                var tableLayoutPanel = (TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject;
                Assert.AreEqual(3, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(1, tableLayoutPanel.RowCount);

                var assemblyResultLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 0);
                Assert.IsTrue(assemblyResultLabel.AutoSize);
                Assert.AreEqual(DockStyle.Fill, assemblyResultLabel.Dock);
                Assert.AreEqual(ContentAlignment.MiddleLeft, assemblyResultLabel.TextAlign);
                Assert.AreEqual("Toetsoordeel voor dit toetsspoor:", assemblyResultLabel.Text);

                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(new Size(500, 0), view.AutoScrollMinSize);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup 
            const int nameColumnIndex = 0;
            const int stringColumnIndex = 1;

            // Call
            using (ShowFailureMechanismResultsView(new ObservableList<TestFailureMechanismSectionResult>()))
            {
                // Assert
                DataGridView dataGridView = GetDataGridView();

                Assert.AreEqual(2, dataGridView.ColumnCount);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);

                Assert.AreEqual("Test", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("TestString", dataGridView.Columns[stringColumnIndex].HeaderText);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismNotifiesObservers_ThenDataGridViewUpdated()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.IsFalse(rowsChanged);

                // When
                view.FailureMechanism.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismSectionResultCollectionUpdatedAndNotifiesObservers_ThenDataGridViewUpdated()
        {
            // Given
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>();
            using (ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();

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
                DataGridView dataGridView = GetDataGridView();
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
        [TestCaseSource(nameof(CellFormattingStates))]
        public void GivenFailureMechanismResultView_WhenCellFormattingEventFired_ThenCellStyleSetToColumnDefinition(
            bool readOnly, string errorText, CellStyle style)
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();
                var row = (TestRow) dataGridView.Rows[0].DataBoundItem;
                DataGridViewColumnStateDefinition definition = row.ColumnStateDefinitions[0];
                definition.ReadOnly = readOnly;
                definition.ErrorText = errorText;
                definition.Style = style;

                // When
                sectionResult.NotifyObservers();

                // Then
                DataGridViewCell cell = dataGridView.Rows[0].Cells[0];
                Assert.AreEqual(readOnly, cell.ReadOnly);
                Assert.AreEqual(errorText, cell.ErrorText);
                Assert.AreEqual(style.BackgroundColor, cell.Style.BackColor);
                Assert.AreEqual(style.TextColor, cell.Style.ForeColor);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenRowUpdatingAndSectionResultNotified_ThenNothingUpdates()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();
                var row = (TestRow) dataGridView.Rows[0].DataBoundItem;
                var invalidated = false;
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // Precondition
                Assert.IsFalse(invalidated);
                Assert.IsFalse(row.Updated);

                // When
                TypeUtils.SetField(view, "rowUpdating", true);
                sectionResult.NotifyObservers();

                // Then
                Assert.IsFalse(invalidated);
                Assert.IsFalse(row.Updated);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenSectionResultNotified_ThenAllRowsUpdatedAndViewInvalidated()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult1 = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            TestFailureMechanismSectionResult sectionResult2 = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult1,
                sectionResult2
            };

            using (ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();
                var row1 = (TestRow) dataGridView.Rows[0].DataBoundItem;
                var row2 = (TestRow) dataGridView.Rows[1].DataBoundItem;
                var invalidated = false;
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // Precondition
                Assert.IsFalse(invalidated);
                Assert.IsFalse(row1.Updated);
                Assert.IsFalse(row2.Updated);

                // When
                sectionResult1.NotifyObservers();

                // Then
                Assert.IsTrue(invalidated);
                Assert.IsTrue(row1.Updated);
                Assert.IsTrue(row2.Updated);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenRowUpdated_ThenColumnDoesNotAutoResize()
        {
            // Given
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult()
            };

            using (ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();
                var row = (TestRow) dataGridView.Rows[0].DataBoundItem;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[1];
                row.TestString = "a";
                int initialWidth = dataGridViewCell.OwningColumn.Width;

                // When
                row.TestString = "Looooooooooooong testing value";

                // Then
                int newWidth = dataGridViewCell.OwningColumn.Width;
                Assert.AreEqual(initialWidth, newWidth);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenSectionResultNotified_ThenColumnDoesAutoResize()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();
                var row = (TestRow) dataGridView.Rows[0].DataBoundItem;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[1];
                row.TestString = "a";
                int initialWidth = dataGridViewCell.OwningColumn.Width;

                // When
                row.TestString = "Looooooooooooong testing value";
                sectionResult.NotifyObservers();

                // Then
                int newWidth = dataGridViewCell.OwningColumn.Width;
                Assert.Greater(newWidth, initialWidth);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenResultRemovedAndSectionResultsNotified_ThenEventHandlersDisconnected()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();
                var row = (TestRow) dataGridView.Rows[0].DataBoundItem;

                var rowUpdated = false;
                row.RowUpdated += (sender, args) => rowUpdated = true;
                var rowUpdateDone = false;
                row.RowUpdateDone += (sender, args) => rowUpdateDone = true;

                // Precondition
                row.UpdateInternal();
                Assert.IsTrue(rowUpdated);
                Assert.IsTrue(rowUpdateDone);

                // When
                rowUpdated = false;
                rowUpdateDone = false;
                sectionResults.Remove(sectionResult);
                sectionResults.NotifyObservers();

                // Then
                Assert.IsFalse(rowUpdated);
                Assert.IsFalse(rowUpdateDone);
            }
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultsView(IObservableEnumerable<FailureMechanismSectionResult> sectionResults)
        {
            var failureMechanismResultView = new TestFailureMechanismResultView(sectionResults, new TestFailureMechanism());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private class TestFailureMechanismResultView : FailureMechanismResultView<FailureMechanismSectionResult, TestRow, TestFailureMechanism>
        {
            public TestFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> failureMechanismSectionResults,
                                                  TestFailureMechanism failureMechanism)
                : base(failureMechanismSectionResults, failureMechanism) {}

            protected override TestRow CreateFailureMechanismSectionResultRow(
                FailureMechanismSectionResult sectionResult)
            {
                return new TestRow(sectionResult);
            }

            protected override void AddDataGridColumns()
            {
                DataGridViewControl.AddTextBoxColumn(nameof(TestRow.Name), "Test", true);
                DataGridViewControl.AddTextBoxColumn(nameof(TestRow.TestString), "TestString");
            }
        }

        private class TestRow : FailureMechanismSectionResultRow<FailureMechanismSectionResult>
        {
            private string testString;

            public TestRow(FailureMechanismSectionResult sectionResult) : base(sectionResult)
            {
                ColumnStateDefinitions.Add(0, new DataGridViewColumnStateDefinition());
            }

            public string TestString
            {
                get => testString;
                set
                {
                    testString = value;
                    UpdateInternalData();
                }
            }

            public bool Updated { get; private set; }

            public void UpdateInternal()
            {
                UpdateInternalData();
            }

            public override void Update()
            {
                Updated = true;
            }

            public override FailureMechanismSectionAssemblyResult AssemblyResult { get; protected set; }
        }
    }
}