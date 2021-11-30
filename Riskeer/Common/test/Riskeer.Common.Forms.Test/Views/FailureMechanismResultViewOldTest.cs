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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Controls;
using Riskeer.Common.Forms.Views;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismResultViewOldTest
    {
        private const string manualAssemblyWarning = "Toetsoordeel is (deels) gebaseerd op handmatig overschreven toetsoordelen.";
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
        public void DefaultConstructor_DefaultValues()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (var view = new TestFailureMechanismResultViewOld(failureMechanism.SectionResultsOld, failureMechanism))
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

                Assert.IsInstanceOf<PictureBox>(tableLayoutPanel.GetControlFromPosition(2, 0));

                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(new Size(500, 0), view.AutoScrollMinSize);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void Constructor_FailureMechanismSectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestFailureMechanismResultViewOld(null, new TestFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResults", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestFailureMechanismResultViewOld(new ObservableList<FailureMechanismSectionResultOld>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup 
            const int nameColumnIndex = 0;
            const int stringColumnIndex = 1;

            // Call
            using (ShowFailureMechanismResultsView(new ObservableList<TestFailureMechanismSectionResultOld>()))
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
        public void Constructor_ToolTipCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (var view = new TestFailureMechanismResultViewOld(failureMechanism.SectionResultsOld, failureMechanism))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                var infoIcon = (PictureBox) new ControlTester("infoIcon").TheObject;
                TestHelper.AssertImagesAreEqual(CoreGuiResources.information, infoIcon.BackgroundImage);
                Assert.AreEqual(ImageLayout.Center, infoIcon.BackgroundImageLayout);

                var toolTip = TypeUtils.GetField<ToolTip>(view, "toolTip");
                Assert.AreEqual("NVT - Niet Van Toepassing\r\n" +
                                "WVT - Wel Van Toepassing\r\n" +
                                "FV - Faalkans Verwaarloosbaar\r\n" +
                                "VB - Verder Beoordelen\r\n\r\n" +
                                "V - Voldoet\r\n" +
                                "VN - Voldoet Niet\r\n" +
                                "NGO - Nog Geen Oordeel\r\n" +
                                "Faalkans - Faalkans gespecificeerd of uitgerekend", toolTip.GetToolTip(infoIcon));
                Assert.AreEqual(5000, toolTip.AutoPopDelay);
                Assert.AreEqual(100, toolTip.InitialDelay);
                Assert.AreEqual(100, toolTip.ReshowDelay);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismNotifiesObservers_ThenDataGridViewInvalidatedAndAssemblyResultControlUpdated()
        {
            // Given
            TestFailureMechanismSectionResultOld sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
            {
                sectionResult
            };

            using (TestFailureMechanismResultViewOld view = ShowFailureMechanismResultsView(sectionResults))
            {
                var invalidated = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Invalidated += (sender, args) => invalidated = true;
                view.AssemblyResultControlUpdated = false;

                // Precondition
                Assert.IsFalse(invalidated);
                Assert.IsFalse(view.AssemblyResultControlUpdated);

                // When
                view.FailureMechanism.NotifyObservers();

                // Then
                Assert.IsTrue(invalidated);
                Assert.IsTrue(view.AssemblyResultControlUpdated);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismSectionResultCollectionUpdatedAndNotifiesObservers_ThenDataGridViewUpdated()
        {
            // Given
            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>();
            using (ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();

                // Precondition
                Assert.AreEqual(0, dataGridView.RowCount);

                // When
                sectionResults.Add(FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld());
                sectionResults.NotifyObservers();

                // Then
                Assert.AreEqual(1, dataGridView.RowCount);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenSingleFailureMechanismSectionResultNotifiesObservers_ThenDataGridViewInvalidatedAndAssemblyResultControlUpdated()
        {
            // Given
            TestFailureMechanismSectionResultOld sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
            {
                sectionResult
            };

            using (TestFailureMechanismResultViewOld view = ShowFailureMechanismResultsView(sectionResults))
            {
                var invalidated = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Invalidated += (sender, args) => invalidated = true;
                view.AssemblyResultControlUpdated = false;

                // Precondition
                Assert.IsFalse(invalidated);
                Assert.IsFalse(view.AssemblyResultControlUpdated);

                // When
                sectionResult.NotifyObservers();

                // Then
                Assert.IsTrue(invalidated);
                Assert.IsTrue(view.AssemblyResultControlUpdated);
            }
        }

        [Test]
        [TestCaseSource(nameof(CellFormattingStates))]
        public void GivenFailureMechanismResultView_WhenCellFormattingEventFired_ThenCellStyleSetToColumnDefinition(
            bool readOnly, string errorText, CellStyle style)
        {
            // Given
            TestFailureMechanismSectionResultOld sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
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
            TestFailureMechanismSectionResultOld sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
            {
                sectionResult
            };

            using (TestFailureMechanismResultViewOld view = ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();
                var row = (TestRow) dataGridView.Rows[0].DataBoundItem;
                var invalidated = false;
                dataGridView.Invalidated += (sender, args) => invalidated = true;
                view.AssemblyResultControlUpdated = false;

                // Precondition
                Assert.IsFalse(invalidated);
                Assert.IsFalse(row.Updated);

                // When
                TypeUtils.SetField(view, "rowUpdating", true);
                sectionResult.NotifyObservers();

                // Then
                Assert.IsFalse(invalidated);
                Assert.IsFalse(row.Updated);
                Assert.IsFalse(view.AssemblyResultControlUpdated);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenExceptionThrownDuringUpdate_ThenErrorSet()
        {
            // Given
            TestFailureMechanismSectionResultOld sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
            {
                sectionResult
            };

            using (TestFailureMechanismResultViewOld view = ShowFailureMechanismResultsView(sectionResults))
            {
                // Precondition
                TestAssemblyResultControl resultControl = GetFailureMechanismAssemblyCategoryGroupControl();
                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                Assert.IsEmpty(errorProvider.GetError(resultControl));

                // When
                view.ThrowExceptionOnUpdate = true;
                sectionResult.NotifyObservers();

                // Then
                Assert.AreEqual("Message", errorProvider.GetError(resultControl));
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenHasManualAssemblyResultsTrueDuringUpdate_ThenWarningSet()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            FailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();

            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                TestAssemblyResultControl resultControl = GetFailureMechanismAssemblyCategoryGroupControl();
                ErrorProvider manualAssemblyWarningProvider = GetManualAssemblyWarningProvider(resultControl);
                Assert.IsEmpty(manualAssemblyWarningProvider.GetError(resultControl));

                // When
                sectionResult.UseManualAssembly = true;
                sectionResult.NotifyObservers();

                // Then
                Assert.AreEqual(manualAssemblyWarning, manualAssemblyWarningProvider.GetError(resultControl));
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenExceptionThrownAndHasManualAssemblyResultsTrueDuringUpdate_ThenMessagesSet()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            FailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();

            using (TestFailureMechanismResultViewOld view = ShowFailureMechanismResultsView(failureMechanism))
            {
                // Precondition
                TestAssemblyResultControl resultControl = GetFailureMechanismAssemblyCategoryGroupControl();
                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                Assert.IsEmpty(errorProvider.GetError(resultControl));

                ErrorProvider manualAssemblyWarningProvider = GetManualAssemblyWarningProvider(resultControl);
                Assert.IsEmpty(manualAssemblyWarningProvider.GetError(resultControl));

                // When
                sectionResult.UseManualAssembly = true;
                view.ThrowExceptionOnUpdate = true;
                sectionResult.NotifyObservers();

                // Then
                Assert.AreEqual("Message", errorProvider.GetError(resultControl));
                Assert.AreEqual(manualAssemblyWarning, manualAssemblyWarningProvider.GetError(resultControl));
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenFailureMechanismAssemblyControlUpdated_ThenDataCleared()
        {
            // Given
            TestFailureMechanismSectionResultOld sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
            {
                sectionResult
            };

            using (ShowFailureMechanismResultsView(sectionResults))
            {
                var groupLabel = (BorderedLabel) new ControlTester("GroupLabel").TheObject;
                groupLabel.Text = "aaa";

                // When
                sectionResult.NotifyObservers();

                // Then
                Assert.IsEmpty(groupLabel.Text);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithMessages_WhenNoExceptionThrownAndManualSectionAssemblyResultsFalseDuringUpdate_ThenMessagesCleared()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            FailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();

            using (TestFailureMechanismResultViewOld view = ShowFailureMechanismResultsView(failureMechanism))
            {
                view.ThrowExceptionOnUpdate = true;
                sectionResult.UseManualAssembly = true;
                sectionResult.NotifyObservers();

                // Precondition
                TestAssemblyResultControl resultControl = GetFailureMechanismAssemblyCategoryGroupControl();
                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                Assert.AreEqual("Message", errorProvider.GetError(resultControl));

                ErrorProvider manualAssemblyWarningProvider = GetManualAssemblyWarningProvider(resultControl);
                Assert.AreEqual(manualAssemblyWarning, manualAssemblyWarningProvider.GetError(resultControl));

                // When
                view.ThrowExceptionOnUpdate = false;
                sectionResult.UseManualAssembly = false;
                sectionResult.NotifyObservers();

                // Then
                Assert.IsEmpty(errorProvider.GetError(resultControl));
                Assert.IsEmpty(manualAssemblyWarningProvider.GetError(resultControl));
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenSectionResultNotified_ThenAllRowsUpdatedAndViewInvalidated()
        {
            // Given
            TestFailureMechanismSectionResultOld sectionResult1 = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();
            TestFailureMechanismSectionResultOld sectionResult2 = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
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
            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld()
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
            TestFailureMechanismSectionResultOld sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();
            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
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
            TestFailureMechanismSectionResultOld sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResultOld();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResultOld>
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

        private static ErrorProvider GetErrorProvider(TestAssemblyResultControl resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "errorProvider");
        }

        private static ErrorProvider GetManualAssemblyWarningProvider(TestAssemblyResultControl resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "manualAssemblyWarningProvider");
        }

        private static TestAssemblyResultControl GetFailureMechanismAssemblyCategoryGroupControl()
        {
            return (TestAssemblyResultControl) new ControlTester("AssemblyResultControl").TheObject;
        }

        private TestFailureMechanismResultViewOld ShowFailureMechanismResultsView(TestFailureMechanism failureMechanism)
        {
            var failureMechanismResultView = new TestFailureMechanismResultViewOld(failureMechanism.SectionResultsOld, failureMechanism);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private TestFailureMechanismResultViewOld ShowFailureMechanismResultsView(IObservableEnumerable<FailureMechanismSectionResultOld> sectionResults)
        {
            var failureMechanismResultView = new TestFailureMechanismResultViewOld(sectionResults, new TestFailureMechanism());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private class TestFailureMechanismResultViewOld : FailureMechanismResultViewOld<FailureMechanismSectionResultOld,
            TestRow,
            TestFailureMechanism,
            TestAssemblyResultControl>
        {
            public TestFailureMechanismResultViewOld(IObservableEnumerable<FailureMechanismSectionResultOld> failureMechanismSectionResults,
                                                  TestFailureMechanism failureMechanism)
                : base(failureMechanismSectionResults, failureMechanism) {}

            public bool ThrowExceptionOnUpdate { private get; set; }

            public bool AssemblyResultControlUpdated { get; set; }

            protected override TestRow CreateFailureMechanismSectionResultRow(
                FailureMechanismSectionResultOld sectionResult)
            {
                return new TestRow(sectionResult);
            }

            protected override void AddDataGridColumns()
            {
                DataGridViewControl.AddTextBoxColumn(nameof(TestRow.Name), "Test", true);
                DataGridViewControl.AddTextBoxColumn(nameof(TestRow.TestString), "TestString");
            }

            protected override void UpdateAssemblyResultControl()
            {
                if (ThrowExceptionOnUpdate)
                {
                    throw new AssemblyException("Message");
                }

                AssemblyResultControlUpdated = true;
            }
        }

        private class TestAssemblyResultControl : AssemblyResultControl {}

        private class TestRow : FailureMechanismSectionResultRowOld<FailureMechanismSectionResultOld>
        {
            private string testString;

            public TestRow(FailureMechanismSectionResultOld sectionResult) : base(sectionResult)
            {
                ColumnStateDefinitions.Add(0, new DataGridViewColumnStateDefinition());
            }

            public string TestString
            {
                get
                {
                    return testString;
                }
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
        }
    }
}