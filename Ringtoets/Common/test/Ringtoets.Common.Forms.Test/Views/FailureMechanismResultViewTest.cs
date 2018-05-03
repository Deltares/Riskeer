﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Views;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.Views
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
        public void DefaultConstructor_DefaultValues()
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
                Assert.AreEqual("Assemblageresultaat voor dit toetsspoor:", assemblyResultLabel.Text);

                Assert.IsInstanceOf<PictureBox>(tableLayoutPanel.GetControlFromPosition(2, 0));

                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);

                Assert.IsTrue(view.AssemblyResultUpdated);
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
                DataGridView dataGridView = GetDataGridView();

                Assert.AreEqual(1, dataGridView.ColumnCount);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);

                Assert.AreEqual("Test", dataGridView.Columns[nameColumnIndex].HeaderText);
            }
        }

        [Test]
        public void Constructor_ToolTipCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (var view = new TestFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                var infoIcon = (PictureBox) new ControlTester("infoIcon").TheObject;
                TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.information, infoIcon.BackgroundImage);
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
        public void GivenFailureMechanismResultView_WhenFailureMechanismNotifiesObservers_ThenDataGridViewInvalidatedAndAssemblyResultUpdated()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                var invalidated = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Invalidated += (sender, args) => invalidated = true;
                view.AssemblyResultUpdated = false;

                // Precondition
                Assert.IsFalse(invalidated);

                // When
                view.FailureMechanism.NotifyObservers();

                // Then
                Assert.IsTrue(invalidated);
                Assert.IsTrue(view.AssemblyResultUpdated);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismSectionResultCollectionUpdated_ThenObserverNotified()
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
        public void GivenFailureMechanismResultView_WhenSingleFailureMechanismSectionResultNotifiesObservers_ThenDataGridViewInvalidatedAndAssemblyResultUpdated()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                var invalidated = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Invalidated += (sender, args) => invalidated = true;
                view.AssemblyResultUpdated = false;

                // Precondition
                Assert.IsFalse(invalidated);
                Assert.IsFalse(view.AssemblyResultUpdated);

                // When
                sectionResult.NotifyObservers();

                // Then
                Assert.IsTrue(invalidated);
                Assert.IsTrue(view.AssemblyResultUpdated);
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
        public void GivenFailureMechanismResultView_WhenRowUpdatedEventFiredAndSectionResultNotified_ThenRowNotUpdatedAndViewInvalidatedAndAssemblyResultUpdated()
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
                view.AssemblyResultUpdated = false;

                // Precondition
                Assert.IsFalse(invalidated);
                Assert.IsFalse(row.Updated);

                // When
                row.RowUpdated?.Invoke(row, EventArgs.Empty);
                sectionResult.NotifyObservers();
                row.RowUpdateDone?.Invoke(row, EventArgs.Empty);

                // Then
                Assert.IsTrue(invalidated);
                Assert.IsFalse(row.Updated);
                Assert.IsTrue(view.AssemblyResultUpdated);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenExceptionThrownDuringUpdate_FailureMechanismAssemblyResultClearedAndErrorSet()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                // Precondition
                BorderedLabel assemblyGroupLabel = GetGroupLabel();
                FailureMechanismAssemblyCategoryGroupControl resultControl = GetFailureMechanismAssemblyCategoryGroupControl();
                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                Assert.AreEqual(Color.FromArgb(255, 255, 0), assemblyGroupLabel.BackColor);
                Assert.IsEmpty(errorProvider.GetError(resultControl));

                // When
                view.ThrowExceptionOnCalculate = true;
                sectionResult.NotifyObservers();

                // Assert
                Assert.AreEqual(Color.White, assemblyGroupLabel.BackColor);
                Assert.AreEqual("Message", errorProvider.GetError(resultControl));
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithFailureMechanismAssemblyError_WhenNoExceptionThrownDuringUpdate_ResultSetAndErrorCleared()
        {
            // Given
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                sectionResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                view.ThrowExceptionOnCalculate = true;
                sectionResult.NotifyObservers();

                // Precondition
                BorderedLabel assemblyGroupLabel = GetGroupLabel();
                FailureMechanismAssemblyCategoryGroupControl resultControl = GetFailureMechanismAssemblyCategoryGroupControl();
                ErrorProvider errorProvider = GetErrorProvider(resultControl);
                Assert.AreEqual(Color.White, assemblyGroupLabel.BackColor);
                Assert.AreEqual("Message", errorProvider.GetError(resultControl));

                // When
                view.ThrowExceptionOnCalculate = false;
                sectionResult.NotifyObservers();

                // Assert
                Assert.AreEqual(Color.FromArgb(255, 255, 0), assemblyGroupLabel.BackColor);
                Assert.IsEmpty(errorProvider.GetError(resultControl));
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

                // Precondition
                Assert.IsNotNull(row.RowUpdated);
                Assert.IsNotNull(row.RowUpdateDone);

                // When
                sectionResults.Remove(sectionResult);
                sectionResults.NotifyObservers();

                // Then
                Assert.IsNull(row.RowUpdated);
                Assert.IsNull(row.RowUpdateDone);
            }
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private static ErrorProvider GetErrorProvider(FailureMechanismAssemblyCategoryGroupControl resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "ErrorProvider");
        }

        private static FailureMechanismAssemblyCategoryGroupControl GetFailureMechanismAssemblyCategoryGroupControl()
        {
            return (FailureMechanismAssemblyCategoryGroupControl) new ControlTester("AssemblyResultControl").TheObject;
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultsView(IObservableEnumerable<FailureMechanismSectionResult> sectionResults)
        {
            var failureMechanismResultView = new TestFailureMechanismResultView(sectionResults, new TestFailureMechanism());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private static BorderedLabel GetGroupLabel()
        {
            return (BorderedLabel) new ControlTester("GroupLabel").TheObject;
        }

        private class TestFailureMechanismResultView : FailureMechanismResultView<FailureMechanismSectionResult, FailureMechanismSectionResultRow<FailureMechanismSectionResult>, TestFailureMechanism, FailureMechanismAssemblyCategoryGroupControl>
        {
            public TestFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> failureMechanismSectionResults, TestFailureMechanism failureMechanism)
                : base(failureMechanismSectionResults, failureMechanism) {}

            public bool ThrowExceptionOnCalculate { private get; set; }

            public bool AssemblyResultUpdated { get; set; }

            protected override FailureMechanismSectionResultRow<FailureMechanismSectionResult> CreateFailureMechanismSectionResultRow(FailureMechanismSectionResult sectionResult)
            {
                return new TestRow(sectionResult);
            }

            protected override void AddDataGridColumns()
            {
                DataGridViewControl.AddTextBoxColumn("Name", "Test", true);
            }

            protected override void UpdateAssemblyResultControl()
            {
                if (ThrowExceptionOnCalculate)
                {
                    throw new AssemblyException("Message");
                }

                FailureMechanismAssemblyResultControl.SetAssemblyResult(FailureMechanismAssemblyCategoryGroup.IIIt);
                AssemblyResultUpdated = true;
            }
        }

        private class TestRow : FailureMechanismSectionResultRow<FailureMechanismSectionResult>
        {
            public TestRow(FailureMechanismSectionResult sectionResult) : base(sectionResult)
            {
                ColumnStateDefinitions.Add(0, new DataGridViewColumnStateDefinition());
            }

            public bool Updated { get; private set; }

            public override void Update()
            {
                Updated = true;
            }
        }
    }
}