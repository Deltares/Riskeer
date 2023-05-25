// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Enums;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismResultViewTest
    {
        private const int errorIconPadding = 5;
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
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestFailureMechanismResultView(null, new TestFailureMechanism(), assessmentSection, (mechanism, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResults", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestFailureMechanismResultView(new ObservableList<FailureMechanismSectionResult>(), null, assessmentSection, (mechanism, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestFailureMechanismResultView(new ObservableList<FailureMechanismSectionResult>(), new TestFailureMechanism(), null, (mechanism, section) => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_PerformFailureMechanismAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestFailureMechanismResultView(new ObservableList<FailureMechanismSectionResult>(), new TestFailureMechanism(), assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performFailureMechanismAssemblyFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.AreEqual(2, view.Controls.Count);

                var tableLayoutPanel = (TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject;
                Assert.AreEqual(4, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(1, tableLayoutPanel.RowCount);

                var assemblyResultLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 0);
                Assert.IsTrue(assemblyResultLabel.AutoSize);
                Assert.AreEqual(DockStyle.Fill, assemblyResultLabel.Dock);
                Assert.AreEqual(ContentAlignment.MiddleLeft, assemblyResultLabel.TextAlign);
                Assert.AreEqual("Faalkans van dit faalmechanisme voor het traject [1/jaar]", assemblyResultLabel.Text);

                var comboBox = (ComboBox) tableLayoutPanel.GetControlFromPosition(1, 0);
                Assert.AreEqual(DockStyle.Fill, comboBox.Dock);
                Assert.AreEqual(ComboBoxStyle.DropDownList, comboBox.DropDownStyle);

                var textBox = (TextBox) tableLayoutPanel.GetControlFromPosition(2, 0);
                Assert.AreEqual(DockStyle.Fill, textBox.Dock);

                ErrorProvider errorProvider = GetErrorProvider(view);
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ErrorIcon.ToBitmap(), errorProvider.Icon.ToBitmap());
                Assert.AreEqual(ErrorBlinkStyle.NeverBlink, errorProvider.BlinkStyle);

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
            using (ShowFailureMechanismResultView(new ObservableList<FailureMechanismSectionResult>()))
            {
                // Assert
                DataGridView dataGridView = GetDataGridView();

                Assert.AreEqual(2, dataGridView.ColumnCount);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);

                Assert.AreEqual("Test", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("TestString", dataGridView.Columns[stringColumnIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void Constructor_ComboBoxCorrectlyInitialized()
        {
            // Setup 
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (ShowFailureMechanismResultView(failureMechanism.SectionResults))
            {
                // Assert
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.AreEqual(nameof(EnumDisplayWrapper<FailureMechanismAssemblyProbabilityResultType>.DisplayName), comboBox.DisplayMember);
                Assert.AreEqual(nameof(EnumDisplayWrapper<FailureMechanismAssemblyProbabilityResultType>.Value), comboBox.ValueMember);
                Assert.IsInstanceOf<EnumDisplayWrapper<FailureMechanismAssemblyProbabilityResultType>>(comboBox.SelectedItem);

                var configurationTypes = (EnumDisplayWrapper<FailureMechanismAssemblyProbabilityResultType>[]) comboBox.DataSource;
                Assert.AreEqual(4, configurationTypes.Length);
                Assert.AreEqual(FailureMechanismAssemblyProbabilityResultType.None, configurationTypes[0].Value);
                Assert.AreEqual(FailureMechanismAssemblyProbabilityResultType.P1, configurationTypes[1].Value);
                Assert.AreEqual(FailureMechanismAssemblyProbabilityResultType.P2, configurationTypes[2].Value);
                Assert.AreEqual(FailureMechanismAssemblyProbabilityResultType.Manual, configurationTypes[3].Value);
                Assert.AreEqual(FailureMechanismAssemblyProbabilityResultType.None,
                                ((EnumDisplayWrapper<FailureMechanismAssemblyProbabilityResultType>) comboBox.SelectedItem).Value);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void GivenFailureMechanismResultViewWithProbabilityResultTypeNotManual_WhenFailureMechanismObserversNotified_ThenDataGridViewUpdatedAndPerformsAssemblyCalculation(
            FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            // Given
            FailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            var sectionResults = new ObservableList<FailureMechanismSectionResult>
            {
                sectionResult
            };
            var testFailureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType
                }
            };
            int nrOfCalls = 0;
            Func<TestFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc = (fm, ass) =>
            {
                nrOfCalls++;
                return new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.BOI1A1);
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(sectionResults, testFailureMechanism, performFailureMechanismAssemblyFunc))
            {
                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.AreEqual(1, nrOfCalls);
                Assert.IsFalse(rowsChanged);

                // When
                view.FailureMechanism.NotifyObservers();

                // Then
                Assert.AreEqual(2, nrOfCalls);
                Assert.IsTrue(rowsChanged);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void GivenFailureMechanismResultViewWithProbabilityResultTypeNotManual_WhenFailureMechanismSectionResultCollectionUpdatedAndObserversNotified_ThenDataGridViewUpdatedAndPerformsAssemblyCalculation(
            FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            // Given
            var sectionResults = new ObservableList<FailureMechanismSectionResult>();

            var testFailureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType
                }
            };

            int nrOfCalls = 0;
            Func<TestFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc = (fm, ass) =>
            {
                nrOfCalls++;
                return new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.BOI1A1);
            };

            using (ShowFailureMechanismResultView(sectionResults, testFailureMechanism, performFailureMechanismAssemblyFunc))
            {
                DataGridView dataGridView = GetDataGridView();

                // Precondition
                Assert.AreEqual(1, nrOfCalls);
                Assert.AreEqual(0, dataGridView.RowCount);

                // When
                sectionResults.Add(FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult());
                sectionResults.NotifyObservers();

                // Then
                Assert.AreEqual(2, nrOfCalls);
                Assert.AreEqual(1, dataGridView.RowCount);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenSingleFailureMechanismSectionResultObserversNotified_ThenDataGridViewInvalidatedAndAssemblyCalculationPerformed()
        {
            // Given
            FailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            var sectionResults = new ObservableList<FailureMechanismSectionResult>
            {
                sectionResult
            };

            var testFailureMechanism = new TestFailureMechanism();

            int nrOfCalls = 0;
            Func<TestFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc = (fm, ass) =>
            {
                nrOfCalls++;
                return new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.BOI1A1);
            };

            using (ShowFailureMechanismResultView(sectionResults, testFailureMechanism, performFailureMechanismAssemblyFunc))
            {
                var invalidated = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // Precondition
                Assert.AreEqual(1, nrOfCalls);
                Assert.IsFalse(invalidated);

                // When
                sectionResult.NotifyObservers();

                // Then
                Assert.AreEqual(2, nrOfCalls);
                Assert.IsTrue(invalidated);
            }
        }

        [Test]
        [TestCaseSource(nameof(CellFormattingStates))]
        public void GivenFailureMechanismResultView_WhenCellFormattingEventFired_ThenCellStyleSetToColumnDefinition(
            bool readOnly, string errorText, CellStyle style)
        {
            // Given
            FailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            var sectionResults = new ObservableList<FailureMechanismSectionResult>
            {
                sectionResult
            };

            using (ShowFailureMechanismResultView(sectionResults))
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
        public void GivenFailureMechanismResultView_WhenRowUpdatingAndSectionResultObserversNotified_ThenNothingUpdates()
        {
            // Given
            FailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            var sectionResults = new ObservableList<FailureMechanismSectionResult>
            {
                sectionResult
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(sectionResults))
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
        public void GivenFailureMechanismResultView_WhenSectionResultObserversNotified_ThenAllRowsUpdatedAndViewInvalidated()
        {
            // Given
            FailureMechanismSectionResult sectionResult1 = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            FailureMechanismSectionResult sectionResult2 = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            var sectionResults = new ObservableList<FailureMechanismSectionResult>
            {
                sectionResult1,
                sectionResult2
            };

            using (ShowFailureMechanismResultView(sectionResults))
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
            var sectionResults = new ObservableList<FailureMechanismSectionResult>
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult()
            };

            using (ShowFailureMechanismResultView(sectionResults))
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
        public void GivenFailureMechanismResultView_WhenSectionResultObserversNotified_ThenColumnDoesAutoResize()
        {
            // Given
            FailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            var sectionResults = new ObservableList<FailureMechanismSectionResult>
            {
                sectionResult
            };

            using (ShowFailureMechanismResultView(sectionResults))
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
        public void GivenFailureMechanismResultView_WhenResultRemovedAndSectionResultsObserversNotified_ThenEventHandlersDisconnected()
        {
            // Given
            FailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            var sectionResults = new ObservableList<FailureMechanismSectionResult>
            {
                sectionResult
            };

            using (ShowFailureMechanismResultView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();
                var row = (TestRow) dataGridView.Rows[0].DataBoundItem;

                var rowUpdated = false;
                row.RowUpdated += (sender, args) => rowUpdated = true;
                var rowUpdateDone = false;
                row.RowUpdateDone += (sender, args) => rowUpdateDone = true;

                // When
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

        private static ComboBox GetProbabilityResultTypeComboBox()
        {
            return (ComboBox) new ComboBoxTester("probabilityResultTypeComboBox").TheObject;
        }

        private static TextBox GetFailureMechanismAssemblyProbabilityTextBox()
        {
            return (TextBox) new ControlTester("failureMechanismAssemblyProbabilityTextBox").TheObject;
        }

        private static ErrorProvider GetErrorProvider(TestFailureMechanismResultView view)
        {
            return TypeUtils.GetField<ErrorProvider>(view, "errorProvider");
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> sectionResults)
        {
            return ShowFailureMechanismResultView(sectionResults, new TestFailureMechanism());
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> sectionResults,
                                                                              TestFailureMechanism testFailureMechanism,
                                                                              Func<TestFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
        {
            return ShowFailureMechanismResultView(sectionResults, testFailureMechanism, new AssessmentSectionStub(), performFailureMechanismAssemblyFunc);
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> sectionResults,
                                                                              TestFailureMechanism failureMechanism)
        {
            return ShowFailureMechanismResultView(sectionResults, failureMechanism, new AssessmentSectionStub(),
                                                  (fm, ass) => new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual));
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> sectionResults,
                                                                              TestFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection,
                                                                              Func<TestFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
        {
            var failureMechanismResultView = new TestFailureMechanismResultView(sectionResults, failureMechanism, assessmentSection, performFailureMechanismAssemblyFunc);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private class TestFailureMechanismResultView : FailureMechanismResultView<FailureMechanismSectionResult, TestRow, TestFailureMechanism>
        {
            public TestFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> failureMechanismSectionResults,
                                                  TestFailureMechanism failureMechanism,
                                                  IAssessmentSection assessmentSection,
                                                  Func<TestFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
                : base(failureMechanismSectionResults, failureMechanism, assessmentSection, performFailureMechanismAssemblyFunc) {}

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
                AssemblyResult = new FailureMechanismSectionAssemblyResult(0, 0, 0, FailureMechanismSectionAssemblyGroup.NoResult);
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

            public override void Update()
            {
                Updated = true;
                AssemblyResult = new FailureMechanismSectionAssemblyResult(1, 1, 1, FailureMechanismSectionAssemblyGroup.III);
            }
        }

        #region Assembly

        [Test]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void FailureMechanismResultView_AllDataSet_PassesInputToPerformAssemblyFunc(FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType
                }
            };

            TestFailureMechanism failureMechanismInput = null;
            IAssessmentSection assessmentSectionInput = null;
            Func<TestFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc = (fm, ass) =>
            {
                failureMechanismInput = fm;
                assessmentSectionInput = ass;
                return new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual);
            };

            // Call
            using (ShowFailureMechanismResultView(new ObservableList<FailureMechanismSectionResult>(), failureMechanism, assessmentSection, performFailureMechanismAssemblyFunc))
            {
                // Assert
                Assert.AreSame(failureMechanism, failureMechanismInput);
                Assert.AreSame(assessmentSection, assessmentSectionInput);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.None, double.NaN, "-")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.None, double.NegativeInfinity, "-Oneindig")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.None, double.PositiveInfinity, "Oneindig")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.None, 0.0001, "1/10.000")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.None, 0.000000123456789, "1/8.100.000")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P1, double.NaN, "-")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P1, double.NegativeInfinity, "-Oneindig")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P1, double.PositiveInfinity, "Oneindig")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P1, 0.0001, "1/10.000")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P1, 0.000000123456789, "1/8.100.000")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P2, double.NaN, "-")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P2, double.NegativeInfinity, "-Oneindig")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P2, double.PositiveInfinity, "Oneindig")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P2, 0.0001, "1/10.000")]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P2, 0.000000123456789, "1/8.100.000")]
        public void GivenFailureMechanismResultView_WhenPerformFailureMechanismAssemblySuccessfullyCalled_ThenResultSetOnFailureMechanismAssemblyProbability(
            FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType, double assemblyResult, string expectedString)
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType
                }
            };

            // When
            using (ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection, (fm, ass) => new FailureMechanismAssemblyResultWrapper(
                                                      assemblyResult, AssemblyMethod.BOI1A1)))
            {
                // Precondition
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.AreEqual(failureMechanismAssemblyProbabilityResultType, comboBox.SelectedValue);

                // Then
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.AreEqual(expectedString, failureMechanismAssemblyProbabilityTextBox.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void GivenFailureMechanismResultView_WhenPerformFailureMechanismAssemblyThrowsAssemblyException_ThenDefaultFailureMechanismAssemblyProbabilitySetWithError(FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            // Given
            const string exceptionMessage = "Message";

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType
                }
            };

            // When
            using (TestFailureMechanismResultView view =
                   ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection, (fm, ass) => throw new AssemblyException(exceptionMessage)))
            {
                // Precondition
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.AreEqual(failureMechanismAssemblyProbabilityResultType, comboBox.SelectedValue);

                // Then
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.AreEqual("-", failureMechanismAssemblyProbabilityTextBox.Text);

                ErrorProvider errorProvider = GetErrorProvider(view);
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual(exceptionMessage, errorMessage);
                Assert.AreEqual(errorIconPadding, errorProvider.GetIconPadding(failureMechanismAssemblyProbabilityTextBox));
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void GivenFailureMechanismResultViewWithAssemblyError_WhenPerformFailureMechanismAssemblySuccessfullyCalled_ThenErrorCleared(FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            // Setup
            const string exceptionMessage = "Message";

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType
                }
            };

            int i = 0;
            Func<TestFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc = (fm, ass) =>
            {
                if (i == 0)
                {
                    i++;
                    throw new AssemblyException(exceptionMessage);
                }

                return new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.BOI1A1);
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection, performFailureMechanismAssemblyFunc))
            {
                // Precondition
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.AreEqual(failureMechanismAssemblyProbabilityResultType, comboBox.SelectedValue);

                ErrorProvider errorProvider = GetErrorProvider(view);
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual(exceptionMessage, errorMessage);

                // When
                failureMechanism.NotifyObservers();

                // Then
                errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual("", errorMessage);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void GivenFailureMechanismResultViewWithAssemblyErrorAndProbabilityTypeNotManual_WhenProbabilityTypeSetToManual_ThenErrorCleared(FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            // Setup
            const string exceptionMessage = "Message";

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType,
                    ManualFailureMechanismAssemblyProbability = 0.1
                }
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection, (fm, ass) => throw new AssemblyException(exceptionMessage)))
            {
                // Precondition
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.AreEqual(failureMechanismAssemblyProbabilityResultType, comboBox.SelectedValue);

                ErrorProvider errorProvider = GetErrorProvider(view);
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual(exceptionMessage, errorMessage);

                // When
                comboBox.SelectedValue = FailureMechanismAssemblyProbabilityResultType.Manual;

                // Then
                errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual("", errorMessage);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismResultViewAndProbabilityResultTypeNotManual_WhenProbabilityResultTypeSetToManual_ThenFailureMechanismAssemblyProbabilityTextBoxSetWithCorrectStateAndObserversNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P2
                }
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            failureMechanism.AssemblyResult.Attach(observer);

            using (ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();

                // Precondition
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.ReadOnly);

                // When
                const FailureMechanismAssemblyProbabilityResultType newResultType = FailureMechanismAssemblyProbabilityResultType.Manual;
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                comboBox.SelectedValue = newResultType;

                // Then
                Assert.AreEqual(newResultType, failureMechanism.AssemblyResult.ProbabilityResultType);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.ReadOnly);
            }

            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetToAutomaticResultTypeCases))]
        public void GivenFailureMechanismResultView_WhenChangingProbabilityResultTypeToAutomatic_ThenFailureMechanismAssemblyProbabilityUpdated(
            FailureMechanismAssemblyProbabilityResultType initialResultType, FailureMechanismAssemblyProbabilityResultType targetResultType,
            double initialProbability, double expectedProbability)
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = initialResultType,
                    ManualFailureMechanismAssemblyProbability = 0.3
                }
            };

            using (ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection, (fm, ass) => new FailureMechanismAssemblyResultWrapper(
                                                      expectedProbability, AssemblyMethod.BOI1A1)))
            {
                // Precondition
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.AreEqual(ProbabilityFormattingHelper.FormatWithDiscreteNumbers(initialProbability), failureMechanismAssemblyProbabilityTextBox.Text);

                // When
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                comboBox.SelectedValue = targetResultType;

                // Then
                Assert.AreEqual(ProbabilityFormattingHelper.FormatWithDiscreteNumbers(expectedProbability), failureMechanismAssemblyProbabilityTextBox.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void GivenFailureMechanismResultView_WhenChangingProbabilityResultTypeToManual_ThenFailureMechanismAssemblyProbabilityUpdated(
            FailureMechanismAssemblyProbabilityResultType initialResultType)
        {
            // Given
            const double assemblyResult = 0.1;
            const string assemblyResultText = "1/10";

            const double manualProbability = 0.2;
            const string manualProbabilityText = "1/5";

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = initialResultType,
                    ManualFailureMechanismAssemblyProbability = manualProbability
                }
            };

            using (ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection, (fm, ass) => new FailureMechanismAssemblyResultWrapper(
                                                      assemblyResult, AssemblyMethod.BOI1A1)))
            {
                // Precondition
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.AreEqual(assemblyResultText, failureMechanismAssemblyProbabilityTextBox.Text);

                // When
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                comboBox.SelectedValue = FailureMechanismAssemblyProbabilityResultType.Manual;

                // Then
                Assert.AreEqual(manualProbabilityText, failureMechanismAssemblyProbabilityTextBox.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("NotAProbability", "De waarde kon niet geïnterpreteerd worden als een kans.")]
        [TestCase("30", "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.")]
        [TestCase("-1", "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.")]
        public void GivenFailureMechanismResultTypeManualAndWithoutError_WhenSettingInvalidValue_ThenInitialFailureMechanismAssemblyProbabilitySetWithError(
            string invalidValue,
            string expectedErrorMessage)
        {
            // Given
            const double manualProbability = 0.2;
            const string manualProbabilityText = "1/5";

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual,
                    ManualFailureMechanismAssemblyProbability = manualProbability
                }
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });
            failureMechanism.AssemblyResult.Attach(observer);

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Precondition
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.AreEqual(manualProbabilityText, failureMechanismAssemblyProbabilityTextBox.Text);

                ErrorProvider errorProvider = GetErrorProvider(view);
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual("", errorMessage);

                // When
                var textBoxTester = new TextBoxTester("failureMechanismAssemblyProbabilityTextBox");
                textBoxTester.Enter(invalidValue);

                // Then
                Assert.AreEqual(manualProbability, failureMechanism.AssemblyResult.ManualFailureMechanismAssemblyProbability);

                errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual(expectedErrorMessage, errorMessage);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismResultTypeManualAndWithoutError_WhenSettingNaNValue_ThenSetFailureMechanismAssemblyProbabilityWithErrorAndObserversNotified()
        {
            // Given
            const double manualProbability = 0.2;
            const string manualProbabilityText = "1/5";

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual,
                    ManualFailureMechanismAssemblyProbability = manualProbability
                }
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });
            failureMechanism.AssemblyResult.Attach(observer);

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Precondition
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.AreEqual(manualProbabilityText, failureMechanismAssemblyProbabilityTextBox.Text);

                ErrorProvider errorProvider = GetErrorProvider(view);
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual("", errorMessage);

                // When
                var textBoxTester = new TextBoxTester("failureMechanismAssemblyProbabilityTextBox");
                textBoxTester.Enter("-");

                // Then
                Assert.IsNaN(failureMechanism.AssemblyResult.ManualFailureMechanismAssemblyProbability);

                errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual("Er moet een waarde worden ingevuld voor de faalkans.", errorMessage);
            }

            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("0,1", "1/10", 0.1)]
        [TestCase("1/10", "1/10", 0.1)]
        public void GivenFailureMechanismResultTypeManualAndWithError_WhenSettingValidValue_ThenFailureMechanismAssemblyProbabilitySetAndObserversNotified(
            string validValue,
            string formattedValidValue,
            double expectedProbability)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver())
                    .Repeat.Twice();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual
                }
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Precondition
                var textBoxTester = new TextBoxTester("failureMechanismAssemblyProbabilityTextBox");
                textBoxTester.Enter("NotAProbability");

                ErrorProvider errorProvider = GetErrorProvider(view);
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.IsNotEmpty(errorMessage);

                failureMechanism.AssemblyResult.Attach(observer);

                // When
                textBoxTester.Enter(validValue);

                // Then
                errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.IsEmpty(errorMessage);

                Assert.AreEqual(formattedValidValue, failureMechanismAssemblyProbabilityTextBox.Text);
                Assert.AreEqual(expectedProbability, failureMechanism.AssemblyResult.ManualFailureMechanismAssemblyProbability);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void GivenFailureMechanismResultViewWithAssemblyErrorAndProbabilityTypeManual_WhenProbabilityTypeSetToNotManual_ThenErrorCleared(FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual
                }
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Precondition
                var textBoxTester = new TextBoxTester("failureMechanismAssemblyProbabilityTextBox");
                textBoxTester.Enter("NotAProbability");

                ErrorProvider errorProvider = GetErrorProvider(view);
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.IsNotEmpty(errorMessage);

                // When
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                comboBox.SelectedValue = failureMechanismAssemblyProbabilityResultType;

                // Then
                errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual("", errorMessage);
            }
        }

        [Test]
        public void GivenFailureMechanismResultTypeManual_WhenInvalidValueEnteredAndEscPressed_ThenFailureMechanismAssemblyProbabilitySetToInitialValue()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            const double initialValue = 0.1;
            const string initialValueText = "1/10";
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual,
                    ManualFailureMechanismAssemblyProbability = initialValue
                }
            };

            using (ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                var textBoxTester = new ControlTester("failureMechanismAssemblyProbabilityTextBox");
                const Keys keyData = Keys.Escape;

                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                failureMechanismAssemblyProbabilityTextBox.TextChanged += (sender, args) =>
                {
                    textBoxTester.FireEvent("KeyDown", new KeyEventArgs(keyData));
                };

                // Precondition
                Assert.AreEqual(initialValueText, failureMechanismAssemblyProbabilityTextBox.Text);

                failureMechanism.AssemblyResult.Attach(observer);

                // When
                failureMechanismAssemblyProbabilityTextBox.Text = "NotAProbability";

                // Then
                Assert.AreEqual(initialValueText, failureMechanismAssemblyProbabilityTextBox.Text);
                Assert.AreEqual(initialValue, failureMechanism.AssemblyResult.ManualFailureMechanismAssemblyProbability);
            }

            mocks.VerifyAll();
        }

        #endregion

        #region FailureMechanismAssemblyResultControls

        [Test]
        public void FailureMechanismResultView_WithoutSections_FailureMechanismAssemblyResultsCorrectState()
        {
            // Setup 
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Precondition
                CollectionAssert.IsEmpty(failureMechanism.Sections);

                // Assert
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.IsFalse(comboBox.Enabled);

                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.ReadOnly);
            }
        }

        [Test]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.None)]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P1)]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P2)]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.Manual)]
        public void FailureMechanismResultView_WithSections_FailureMechanismAssemblyResultsCorrectState(
            FailureMechanismAssemblyProbabilityResultType resultType)
        {
            // Setup 
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = resultType
                }
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            // Call
            using (ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.IsTrue(comboBox.Enabled);

                FailureMechanismAssemblyResult assemblyResult = failureMechanism.AssemblyResult;
                bool isManualAssemblyProbability = assemblyResult.IsManualProbability();
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.AreEqual(isManualAssemblyProbability, failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.AreEqual(!isManualAssemblyProbability, failureMechanismAssemblyProbabilityTextBox.ReadOnly);
            }
        }

        [Test]
        public void FailureMechanismResultView_WithoutSectionsAndProbabilityTypeManual_ReturnsCorrectError()
        {
            // Setup
            const string expectedErrorMessage = "Om een oordeel te kunnen invoeren moet voor het faalmechanisme een vakindeling zijn geïmporteerd.";
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual
                }
            };

            // Call
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                ErrorProvider errorProvider = GetErrorProvider(view);
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);

                // Assert
                Assert.AreEqual(expectedErrorMessage, errorMessage);
            }
        }

        [Test]
        public void GivenFailureMechanismResultViewWithoutSectionsAndProbabilityTypeManual_WhenSectionsAddedAndObserversNotified_ThenErrorUpdatedAndSetsFailureMechanismAssemblyResultsCorrectState()
        {
            // Given
            const string initialErrorMessage = "Om een oordeel te kunnen invoeren moet voor het faalmechanisme een vakindeling zijn geïmporteerd.";
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual
                }
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Precondition
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.ReadOnly);

                ErrorProvider errorProvider = GetErrorProvider(view);
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual(initialErrorMessage, errorMessage);

                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.IsFalse(comboBox.Enabled);

                // When
                FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
                FailureMechanismTestHelper.SetSections(failureMechanism, new[]
                {
                    section
                });
                failureMechanism.NotifyObservers();

                // Assert
                errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreNotEqual(initialErrorMessage, errorMessage);

                Assert.IsTrue(comboBox.Enabled);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.ReadOnly);
            }
        }

        [Test]
        public void GivenFailureMechanismResultViewWithSectionsAndProbabilityTypeManual_WhenSectionsClearedAndObserversNotified_ThenErrorUpdatedAndSetsFailureMechanismAssemblyResultsCorrectState()
        {
            // Given
            const string expectedErrorMessage = "Om een oordeel te kunnen invoeren moet voor het faalmechanisme een vakindeling zijn geïmporteerd.";
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual
                }
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Precondition
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.ReadOnly);

                ErrorProvider errorProvider = GetErrorProvider(view);
                string errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreNotEqual(expectedErrorMessage, errorMessage);

                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.IsTrue(comboBox.Enabled);

                // When
                FailureMechanismTestHelper.SetSections(failureMechanism, Enumerable.Empty<FailureMechanismSection>());
                failureMechanism.NotifyObservers();

                // Assert
                errorMessage = errorProvider.GetError(failureMechanismAssemblyProbabilityTextBox);
                Assert.AreEqual(expectedErrorMessage, errorMessage);

                Assert.IsFalse(comboBox.Enabled);
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.ReadOnly);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void GivenFailureMechanismResultViewWithoutSectionsAndProbabilityTypeNotManual_WhenSectionsAddedAndObserversNotified_ThenFailureMechanismAssemblyResultsCorrectStateSet(
            FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            // Given
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType
                }
            };

            using (ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Precondition
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.ReadOnly);

                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.IsFalse(comboBox.Enabled);

                // When
                FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
                FailureMechanismTestHelper.SetSections(failureMechanism, new[]
                {
                    section
                });
                failureMechanism.NotifyObservers();

                // Assert
                Assert.IsTrue(comboBox.Enabled);
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.ReadOnly);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetPerformFailureMechanismAssemblyTestCases))]
        public void GivenFailureMechanismResultViewWithSectionsAndProbabilityTypeNotManual_WhenSectionsClearedAndObserversNotified_ThenFailureMechanismAssemblyResultsCorrectStateSet(
            FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            // Given
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType
                }
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            using (ShowFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Precondition
                TextBox failureMechanismAssemblyProbabilityTextBox = GetFailureMechanismAssemblyProbabilityTextBox();
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.ReadOnly);

                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.IsTrue(comboBox.Enabled);

                // When
                FailureMechanismTestHelper.SetSections(failureMechanism, Enumerable.Empty<FailureMechanismSection>());
                failureMechanism.NotifyObservers();

                // Assert
                Assert.IsFalse(comboBox.Enabled);
                Assert.IsFalse(failureMechanismAssemblyProbabilityTextBox.Enabled);
                Assert.IsTrue(failureMechanismAssemblyProbabilityTextBox.ReadOnly);
            }
        }

        private static IEnumerable<TestCaseData> GetPerformFailureMechanismAssemblyTestCases()
        {
            return new[]
            {
                new TestCaseData(FailureMechanismAssemblyProbabilityResultType.None),
                new TestCaseData(FailureMechanismAssemblyProbabilityResultType.P1),
                new TestCaseData(FailureMechanismAssemblyProbabilityResultType.P2)
            };
        }

        private static IEnumerable<TestCaseData> GetToAutomaticResultTypeCases()
        {
            yield return new TestCaseData(FailureMechanismAssemblyProbabilityResultType.None, FailureMechanismAssemblyProbabilityResultType.P1, 0.1, 0.1);
            yield return new TestCaseData(FailureMechanismAssemblyProbabilityResultType.None, FailureMechanismAssemblyProbabilityResultType.P2, 0.2, 0.2);
            yield return new TestCaseData(FailureMechanismAssemblyProbabilityResultType.Manual, FailureMechanismAssemblyProbabilityResultType.P1, 0.3, 0.1);
            yield return new TestCaseData(FailureMechanismAssemblyProbabilityResultType.Manual, FailureMechanismAssemblyProbabilityResultType.P2, 0.3, 0.2);
        }

        #endregion
    }
}