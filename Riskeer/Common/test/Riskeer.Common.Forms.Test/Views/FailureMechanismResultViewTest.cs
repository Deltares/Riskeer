// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
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
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                // Assert
                Assert.IsInstanceOf<INotifyPropertyChanged>(view);
                Assert.AreEqual(2, view.Controls.Count);

                var tableLayoutPanel = (TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject;
                Assert.AreEqual(4, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(1, tableLayoutPanel.RowCount);

                var assemblyResultLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 0);
                Assert.IsTrue(assemblyResultLabel.AutoSize);
                Assert.AreEqual(DockStyle.Fill, assemblyResultLabel.Dock);
                Assert.AreEqual(ContentAlignment.MiddleLeft, assemblyResultLabel.TextAlign);
                Assert.AreEqual("Gecombineerde faalkans (1/jaar)", assemblyResultLabel.Text);

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
                Assert.IsInstanceOf<INotifyPropertyChanged>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(new Size(500, 0), view.AutoScrollMinSize);
                Assert.AreSame(failureMechanism, view.FailureMechanism);

                TestHelper.AssertTypeConverter<TestFailureMechanismResultView, NoProbabilityValueDoubleConverter>(
                    nameof(TestFailureMechanismResultView.FailurePathAssemblyProbability));
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
        public void Constructor_ComboBoxCorrectlyInitialized()
        {
            // Setup 
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                // Assert
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.AreEqual(nameof(EnumDisplayWrapper<FailurePathAssemblyProbabilityResultType>.DisplayName), comboBox.DisplayMember);
                Assert.AreEqual(nameof(EnumDisplayWrapper<FailurePathAssemblyProbabilityResultType>.Value), comboBox.ValueMember);
                Assert.IsInstanceOf<EnumDisplayWrapper<FailurePathAssemblyProbabilityResultType>>(comboBox.SelectedItem);

                var configurationTypes = (EnumDisplayWrapper<FailurePathAssemblyProbabilityResultType>[]) comboBox.DataSource;
                Assert.AreEqual(2, configurationTypes.Length);
                Assert.AreEqual(FailurePathAssemblyProbabilityResultType.Automatic, configurationTypes[0].Value);
                Assert.AreEqual(FailurePathAssemblyProbabilityResultType.Manual, configurationTypes[1].Value);
                Assert.AreEqual(failureMechanism.AssemblyResult.ProbabilityResultType,
                                ((EnumDisplayWrapper<FailurePathAssemblyProbabilityResultType>) comboBox.SelectedItem).Value);
            }
        }

        [Test]
        public void Constructor_TextBoxCorrectlyInitialized()
        {
            // Setup 
            var failureMechanism = new TestFailureMechanism();

            // Call
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                // Assert
                TextBox textBox = GetFailurePathAssemblyProbabilityTextBox();
                Binding textBoxDataBinding = textBox.DataBindings[nameof(TextBox.Text)];
                Assert.IsNotNull(textBoxDataBinding);

                Assert.AreSame(view, textBoxDataBinding.DataSource);
                Assert.AreEqual(nameof(TestFailureMechanismResultView.FailurePathAssemblyProbability),
                                textBoxDataBinding.BindingMemberInfo.BindingField);
                Assert.IsTrue(textBoxDataBinding.FormattingEnabled);
                Assert.AreEqual(DataSourceUpdateMode.OnValidation, textBoxDataBinding.DataSourceUpdateMode);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismNotifiesObservers_ThenDataGridViewUpdatedAndNotifyPropertyChangedEventFired()
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

                var eventArgs = new List<PropertyChangedEventArgs>();
                view.PropertyChanged += (sender, args) => eventArgs.Add(args);

                // Precondition
                Assert.IsFalse(rowsChanged);
                CollectionAssert.IsEmpty(eventArgs);

                // When
                view.FailureMechanism.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);

                PropertyChangedEventArgs propertyChangedEventArgs = eventArgs.Single();
                Assert.AreEqual(nameof(TestFailureMechanismResultView.FailurePathAssemblyProbability), propertyChangedEventArgs.PropertyName);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismSectionResultCollectionUpdatedAndNotifiesObservers_ThenDataGridViewUpdatedAndNotifyPropertyChangedEventFired()
        {
            // Given
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>();
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(sectionResults))
            {
                DataGridView dataGridView = GetDataGridView();

                var eventArgs = new List<PropertyChangedEventArgs>();
                view.PropertyChanged += (sender, args) => eventArgs.Add(args);

                // Precondition
                CollectionAssert.IsEmpty(eventArgs);
                Assert.AreEqual(0, dataGridView.RowCount);

                // When
                sectionResults.Add(FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult());
                sectionResults.NotifyObservers();

                // Then
                Assert.AreEqual(1, dataGridView.RowCount);

                PropertyChangedEventArgs propertyChangedEventArgs = eventArgs.Single();
                Assert.AreEqual(nameof(TestFailureMechanismResultView.FailurePathAssemblyProbability), propertyChangedEventArgs.PropertyName);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenSingleFailureMechanismSectionResultNotifiesObservers_ThenDataGridViewInvalidatedAndNotifyPropertyChangedEventFired()
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

                var eventArgs = new List<PropertyChangedEventArgs>();
                view.PropertyChanged += (sender, args) => eventArgs.Add(args);

                // Precondition
                Assert.IsFalse(invalidated);
                CollectionAssert.IsEmpty(eventArgs);

                // When
                sectionResult.NotifyObservers();

                // Then
                Assert.IsTrue(invalidated);

                PropertyChangedEventArgs propertyChangedEventArgs = eventArgs.Single();
                Assert.AreEqual(nameof(TestFailureMechanismResultView.FailurePathAssemblyProbability), propertyChangedEventArgs.PropertyName);
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

        private static ComboBox GetProbabilityResultTypeComboBox()
        {
            return (ComboBox) new ComboBoxTester("probabilityResultTypeComboBox").TheObject;
        }

        private static TextBox GetFailurePathAssemblyProbabilityTextBox()
        {
            return (TextBox) new ControlTester("failurePathAssemblyProbabilityTextBox").TheObject;
        }

        private static ErrorProvider GetErrorProvider(TestFailureMechanismResultView view)
        {
            return TypeUtils.GetField<ErrorProvider>(view, "errorProvider");
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultsView(IObservableEnumerable<FailureMechanismSectionResult> sectionResults)
        {
            return ShowFailureMechanismResultsView(new TestFailureMechanism(), sectionResults);
        }

        private TestFailureMechanismResultView ShowFailureMechanismResultsView(TestFailureMechanism failureMechanism,
                                                                               IObservableEnumerable<FailureMechanismSectionResult> sectionResults)
        {
            var failureMechanismResultView = new TestFailureMechanismResultView(sectionResults, failureMechanism);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private class TestFailureMechanismResultView : FailureMechanismResultView<FailureMechanismSectionResult, TestRow, TestFailureMechanism>
        {
            public TestFailureMechanismResultView(IObservableEnumerable<FailureMechanismSectionResult> failureMechanismSectionResults,
                                                  TestFailureMechanism failureMechanism)
                : base(failureMechanismSectionResults, failureMechanism) {}

            public double N { get; private set; }

            public bool GetNCalled { get; private set; }

            protected override TestRow CreateFailureMechanismSectionResultRow(
                FailureMechanismSectionResult sectionResult)
            {
                return new TestRow(sectionResult);
            }

            protected override double GetN()
            {
                GetNCalled = true;

                N = 13.37;
                return N;
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
                AssemblyResult = new FailureMechanismSectionAssemblyResult(0, 0, 0, FailureMechanismSectionAssemblyGroup.Gr);
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
        }

        # region Assembly

        [Test]
        public void FailureMechanismResultView_Always_SetsCorrectInputOnCalculator()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
                }
            };

            // Call
            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult(),
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, sectionResults))
            {
                // Precondition
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                Assert.AreEqual(FailurePathAssemblyProbabilityResultType.Automatic, comboBox.SelectedValue);

                // Assert
                DataGridView dataGrid = GetDataGridView();
                var rowAssemblyResults = new List<FailureMechanismSectionAssemblyResult>();
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    rowAssemblyResults.Add(((TestRow) row.DataBoundItem).AssemblyResult);
                }

                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailurePathAssemblyCalculatorStub calculator = testFactory.LastCreatedFailurePathAssemblyCalculator;

                CollectionAssert.AreEqual(rowAssemblyResults, calculator.SectionAssemblyResultsInput);
                Assert.IsTrue(view.GetNCalled);
                Assert.AreEqual(view.N, calculator.FailurePathN);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenCalculatorRanSuccessfully_ThenCalculatorOutputSetOnFailurePathAssemblyProbability()
        {
            // Givem
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
                }
            };

            var sectionResults = new ObservableList<TestFailureMechanismSectionResult>
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult(),
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailurePathAssemblyCalculatorStub calculator = testFactory.LastCreatedFailurePathAssemblyCalculator;
                calculator.AssemblyResult = 0.3;

                // When
                using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, sectionResults))
                {
                    // Precondition
                    ComboBox comboBox = GetProbabilityResultTypeComboBox();
                    Assert.AreEqual(FailurePathAssemblyProbabilityResultType.Automatic, comboBox.SelectedValue);

                    // Then
                    Assert.AreEqual(calculator.AssemblyResult, view.FailurePathAssemblyProbability);
                }
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenCalculatorFails_ThenSetsDefaultFailurePathAssemblyProbabilityWithError()
        {
            // Given
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
                }
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailurePathAssemblyCalculatorStub calculator = testFactory.LastCreatedFailurePathAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // When
                using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
                {
                    // Precondition
                    ComboBox comboBox = GetProbabilityResultTypeComboBox();
                    Assert.AreEqual(FailurePathAssemblyProbabilityResultType.Automatic, comboBox.SelectedValue);

                    // Then
                    Assert.IsNaN(view.FailurePathAssemblyProbability);

                    ErrorProvider errorProvider = GetErrorProvider(view);
                    TextBox failurePathAssemblyProbabilityTextBox = GetFailurePathAssemblyProbabilityTextBox();
                    string errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                    Assert.AreEqual("Message", errorMessage);
                    Assert.AreEqual(errorIconPadding, errorProvider.GetIconPadding(failurePathAssemblyProbabilityTextBox));
                }
            }
        }

        [Test]
        public void GivenFailureMechanismResultViewWithAssemblyError_WhenCalculatorRunsSuccessfully_ThenErrorCleared()
        {
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
                }
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailurePathAssemblyCalculatorStub calculator = testFactory.LastCreatedFailurePathAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
                {
                    // Precondition
                    ComboBox comboBox = GetProbabilityResultTypeComboBox();
                    Assert.AreEqual(FailurePathAssemblyProbabilityResultType.Automatic, comboBox.SelectedValue);

                    ErrorProvider errorProvider = GetErrorProvider(view);
                    TextBox failurePathAssemblyProbabilityTextBox = GetFailurePathAssemblyProbabilityTextBox();
                    string errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                    Assert.AreEqual("Message", errorMessage);

                    // When
                    calculator.ThrowExceptionOnCalculate = false;
                    failureMechanism.NotifyObservers();

                    // Then
                    errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                    Assert.AreEqual("", errorMessage);
                }
            }
        }

        [Test]
        public void GivenFailureMechanismResultViewWithAssemblyErrorAndProbabilityTypeAutomatic_WhenProbabilityTypeSetToManual_ThenErrorCleared()
        {
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
                }
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailurePathAssemblyCalculatorStub calculator = testFactory.LastCreatedFailurePathAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
                {
                    // Precondition
                    ComboBox comboBox = GetProbabilityResultTypeComboBox();
                    Assert.AreEqual(FailurePathAssemblyProbabilityResultType.Automatic, comboBox.SelectedValue);

                    ErrorProvider errorProvider = GetErrorProvider(view);
                    TextBox failurePathAssemblyProbabilityTextBox = GetFailurePathAssemblyProbabilityTextBox();
                    string errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                    Assert.AreEqual("Message", errorMessage);

                    // When
                    comboBox.SelectedValue = FailurePathAssemblyProbabilityResultType.Manual;

                    // Then
                    errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                    Assert.AreEqual("", errorMessage);
                }
            }
        }

        [Test]
        public void GivenFailureMechanismResultViewAndProbabilityResultTypeAutomatic_WhenProbabilityResultTypeSetToManual_ThenSetsFailurePathAssemblyProbabilityTextBoxWithCorrectStateAndNotifiesObservers()
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
                    ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
                }
            };
            failureMechanism.AssemblyResult.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                TextBox failurePathAssemblyProbabilityTextBox = GetFailurePathAssemblyProbabilityTextBox();

                // Precondition
                Assert.IsFalse(failurePathAssemblyProbabilityTextBox.Enabled);
                Assert.IsTrue(failurePathAssemblyProbabilityTextBox.ReadOnly);

                // When
                const FailurePathAssemblyProbabilityResultType newResultType = FailurePathAssemblyProbabilityResultType.Manual;
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                comboBox.SelectedValue = newResultType;

                // Then
                Assert.AreEqual(newResultType, failureMechanism.AssemblyResult.ProbabilityResultType);
                Assert.IsTrue(failurePathAssemblyProbabilityTextBox.Enabled);
                Assert.IsFalse(failurePathAssemblyProbabilityTextBox.ReadOnly);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(FailurePathAssemblyProbabilityResultType.Automatic, FailurePathAssemblyProbabilityResultType.Manual)]
        [TestCase(FailurePathAssemblyProbabilityResultType.Manual, FailurePathAssemblyProbabilityResultType.Automatic)]
        public void GivenFailureMechanismResultView_WhenChangingProbabilityResultType_ThenFailurePathAssemblyProbabilityAndNotifyPropertyChangedEventRaised(
            FailurePathAssemblyProbabilityResultType initialResultType,
            FailurePathAssemblyProbabilityResultType newResultType)
        {
            // Given
            var random = new Random(21);
            double calculatorOutput = random.NextDouble();
            double manualProbability = random.NextDouble();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = initialResultType,
                    ManualFailurePathAssemblyProbability = manualProbability
                }
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailurePathAssemblyCalculatorStub calculator = testFactory.LastCreatedFailurePathAssemblyCalculator;
                calculator.AssemblyResult = calculatorOutput;

                using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
                {
                    var eventArgs = new List<PropertyChangedEventArgs>();
                    view.PropertyChanged += (sender, args) => eventArgs.Add(args);

                    // Precondition
                    CollectionAssert.IsEmpty(eventArgs);
                    double expectedProbability = initialResultType == FailurePathAssemblyProbabilityResultType.Automatic
                                                     ? calculatorOutput
                                                     : manualProbability;
                    Assert.AreEqual(expectedProbability, view.FailurePathAssemblyProbability);

                    // When
                    ComboBox comboBox = GetProbabilityResultTypeComboBox();
                    comboBox.SelectedValue = newResultType;

                    // Then
                    expectedProbability = newResultType == FailurePathAssemblyProbabilityResultType.Automatic
                                              ? calculatorOutput
                                              : manualProbability;
                    Assert.AreEqual(expectedProbability, view.FailurePathAssemblyProbability);

                    PropertyChangedEventArgs eventArg = eventArgs.Single();
                    Assert.AreEqual(nameof(TestFailureMechanismResultView.FailurePathAssemblyProbability), eventArg.PropertyName);
                }
            }
        }

        [Test]
        [TestCase("NotAProbability", "De waarde kon niet geïnterpreteerd worden als een kans.")]
        [TestCase("30", "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.")]
        [TestCase("-1", "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.")]
        public void GivenFailureMechanismResultTypeManualAndWithoutError_WhenSettingInvalidValue_ThenSetsDefaultFailurePathAssemblyProbabilityWithErrorAndNotifiesObservers(
            string invalidValue,
            string expectedErrorMessage)
        {
            // Given
            var random = new Random(21);
            double manualProbability = random.NextDouble();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver())
                    .Repeat.Twice();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual,
                    ManualFailurePathAssemblyProbability = manualProbability
                }
            };
            failureMechanism.AssemblyResult.Attach(observer);

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                // Precondition
                Assert.AreEqual(manualProbability, view.FailurePathAssemblyProbability);

                ErrorProvider errorProvider = GetErrorProvider(view);
                TextBox failurePathAssemblyProbabilityTextBox = GetFailurePathAssemblyProbabilityTextBox();
                string errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                Assert.AreEqual("", errorMessage);

                // When
                var textBoxTester = new TextBoxTester("failurePathAssemblyProbabilityTextBox");
                textBoxTester.Enter(invalidValue);

                // Then
                Assert.IsNaN(view.FailurePathAssemblyProbability);
                Assert.IsNaN(failureMechanism.AssemblyResult.ManualFailurePathAssemblyProbability);

                errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                Assert.AreEqual(expectedErrorMessage, errorMessage);
            }

            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("0,1", 0.1)]
        [TestCase("1/10", 0.1)]
        public void GivenFailureMechanismResultTypeManualAndWithError_WhenSettingValidValue_ThenSetsFailurePathAssemblyProbabilityAndNotifiesObservers(
            string validValue,
            double expectedProbability)
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
                    ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual
                }
            };

            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                // Precondition
                var textBoxTester = new TextBoxTester("failurePathAssemblyProbabilityTextBox");
                textBoxTester.Enter("NotAProbability");

                ErrorProvider errorProvider = GetErrorProvider(view);
                TextBox failurePathAssemblyProbabilityTextBox = GetFailurePathAssemblyProbabilityTextBox();
                string errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                Assert.IsNotEmpty(errorMessage);

                failureMechanism.AssemblyResult.Attach(observer);

                // When
                textBoxTester.Enter(validValue);

                // Then
                errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                Assert.IsEmpty(errorMessage);

                Assert.AreEqual(expectedProbability, view.FailurePathAssemblyProbability);
                Assert.AreEqual(expectedProbability, failureMechanism.AssemblyResult.ManualFailurePathAssemblyProbability);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismResultViewWithAssemblyErrorAndProbabilityTypeManual_WhenProbabilityTypeSetToAutomatic_ThenErrorCleared()
        {
            var failureMechanism = new TestFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual
                }
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            using (TestFailureMechanismResultView view = ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                // Precondition
                var textBoxTester = new TextBoxTester("failurePathAssemblyProbabilityTextBox");
                textBoxTester.Enter("NotAProbability");

                ErrorProvider errorProvider = GetErrorProvider(view);
                TextBox failurePathAssemblyProbabilityTextBox = GetFailurePathAssemblyProbabilityTextBox();
                string errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                Assert.IsNotEmpty(errorMessage);

                // When
                ComboBox comboBox = GetProbabilityResultTypeComboBox();
                comboBox.SelectedValue = FailurePathAssemblyProbabilityResultType.Automatic;

                // Then
                errorMessage = errorProvider.GetError(failurePathAssemblyProbabilityTextBox);
                Assert.AreEqual("", errorMessage);
            }
        }

        #endregion
    }
}