﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class CalculationsViewTest
    {
        private const int nameColumnIndex = 0;
        private const int selectableHydraulicBoundaryLocationsColumnIndex = 1;

        private Form testForm;

        [SetUp]
        public void SetUp()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        #region Cell editing

        [Test]
        public void CalculationsView_EditingNameViaDataGridView_ObserversCorrectlyNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(o => o.UpdateObserver());
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            TestCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(assessmentSection);

            var data = (CalculationGroup) calculationsView.Data;
            var calculation = (TestCalculation) data.Children.First();

            calculation.Attach(calculationObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[nameColumnIndex].Value = "New name";

            // Assert
            mocks.VerifyAll();
        }

        #endregion

        [Test]
        public void GivenCalculationsView_WhenGenerateCalculationsButtonClicked_ThenGenerateCalculationsCalled()
        {
            // Given
            var view = new TestCalculationsView(new CalculationGroup(), new TestCalculatableFailureMechanism(), new AssessmentSectionStub())
            {
                CanGenerateCalculationState = true
            };

            testForm.Controls.Add(view);
            testForm.Show();

            var button = new ButtonTester("generateButton", testForm);

            // Precondition
            Assert.IsFalse(view.GenerateButtonClicked);

            // When
            button.Click();

            // Then
            Assert.IsTrue(view.GenerateButtonClicked);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculationsView_ChangingSubscribedRow_ListenerCorrectlyNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            TestCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[selectableHydraulicBoundaryLocationsColumnIndex];
            dataGridView.CurrentCell = dataGridViewCell;
            dataGridView.BeginEdit(false);

            // Call
            dataGridViewCell.Value = dataGridView.Rows[1].Cells[selectableHydraulicBoundaryLocationsColumnIndex].Value;

            // Assert
            Assert.AreEqual(1, calculationsView.HydraulicBoundaryLocationChangedCounter);
            mocks.VerifyAll();
        }

        private static void ConfigureHydraulicBoundaryDatabase(IAssessmentSection assessmentSection)
        {
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2),
                    new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4)
                }
            });
        }

        private static CalculationGroup ConfigureCalculationGroup(IAssessmentSection assessmentSection)
        {
            return new CalculationGroup
            {
                Children =
                {
                    new TestCalculation
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First()
                        },
                        Output = null
                    },
                    new TestCalculation
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.Last()
                        }
                    }
                }
            };
        }

        private TestCalculationsView ShowFullyConfiguredCalculationsView(IAssessmentSection assessmentSection)
        {
            ConfigureHydraulicBoundaryDatabase(assessmentSection);

            return ShowCalculationsView(ConfigureCalculationGroup(assessmentSection), new TestCalculatableFailureMechanism(), assessmentSection);
        }

        private TestCalculationsView ShowCalculationsView(CalculationGroup calculationGroup, TestCalculatableFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var calculationsView = new TestCalculationsView(calculationGroup, failureMechanism, assessmentSection);

            testForm.Controls.Add(calculationsView);
            testForm.Show();

            return calculationsView;
        }

        private abstract class TestCalculationsViewBase<TCalculationRow> : CalculationsView<TestCalculation, TestCalculationInput, TCalculationRow, TestCalculatableFailureMechanism>
            where TCalculationRow : CalculationRow<TestCalculation>
        {
            protected TestCalculationsViewBase(CalculationGroup calculationGroup, TestCalculatableFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(calculationGroup, failureMechanism, assessmentSection) {}

            public bool CanGenerateCalculationState { get; set; }

            public bool GenerateButtonClicked { get; private set; }

            protected override object CreateSelectedItemFromCurrentRow(TCalculationRow currentRow)
            {
                return currentRow;
            }

            protected override IEnumerable<Point2D> GetReferenceLocations()
            {
                return new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                };
            }

            protected override bool CanGenerateCalculations()
            {
                return CanGenerateCalculationState;
            }

            protected override void GenerateCalculations()
            {
                GenerateButtonClicked = true;
            }

            protected override void AddColumns(Action addNameColumn, Action addHydraulicBoundaryLocationColumn)
            {
                addNameColumn();
                addHydraulicBoundaryLocationColumn();
            }
        }

        private class TestCalculationsView : TestCalculationsViewBase<TestCalculationRow>
        {
            public TestCalculationsView(CalculationGroup calculationGroup, TestCalculatableFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(calculationGroup, failureMechanism, assessmentSection) {}

            public int HydraulicBoundaryLocationChangedCounter { get; private set; }

            protected override TestCalculationRow CreateRow(TestCalculation calculation)
            {
                return new TestCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, calculation.InputParameters));
            }

            protected override void SubscribeToCalculationRow(TestCalculationRow calculationRow)
            {
                base.SubscribeToCalculationRow(calculationRow);

                calculationRow.HydraulicBoundaryLocationChanged += OnHydraulicBoundaryLocationChanged;
            }

            protected override void UnsubscribeFromCalculationRow(TestCalculationRow calculationRow)
            {
                base.UnsubscribeFromCalculationRow(calculationRow);

                calculationRow.HydraulicBoundaryLocationChanged -= OnHydraulicBoundaryLocationChanged;
            }

            private void OnHydraulicBoundaryLocationChanged(object sender, EventArgs e)
            {
                HydraulicBoundaryLocationChangedCounter++;
            }
        }

        private class MissingNameColumnTestCalculationsView : TestCalculationsViewBase<TestCalculationRow>
        {
            public MissingNameColumnTestCalculationsView(CalculationGroup calculationGroup, TestCalculatableFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(calculationGroup, failureMechanism, assessmentSection) {}

            protected override TestCalculationRow CreateRow(TestCalculation calculation)
            {
                throw new NotImplementedException();
            }

            protected override void AddColumns(Action addNameColumn, Action addHydraulicBoundaryLocationColumn)
            {
                base.AddColumns(() => {}, addHydraulicBoundaryLocationColumn);
            }
        }

        private class MissingHydraulicBoundaryLocationTestCalculationsView : TestCalculationsViewBase<TestCalculationRow>
        {
            public MissingHydraulicBoundaryLocationTestCalculationsView(CalculationGroup calculationGroup, TestCalculatableFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(calculationGroup, failureMechanism, assessmentSection) {}

            protected override TestCalculationRow CreateRow(TestCalculation calculation)
            {
                throw new NotImplementedException();
            }

            protected override void AddColumns(Action addNameColumn, Action addHydraulicBoundaryLocationColumn)
            {
                base.AddColumns(addNameColumn, () => {});
            }
        }

        #region Initialization

        [Test]
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestCalculationsView(null, new TestCalculatableFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestCalculationsView(new CalculationGroup(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestCalculationsView(new CalculationGroup(), new TestCalculatableFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            TestCalculationsView view = ShowCalculationsView(new CalculationGroup(), new TestCalculatableFailureMechanism(), new AssessmentSectionStub());

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<ISelectionProvider>(view);
            Assert.IsInstanceOf<IView>(view);

            var button = (Button) new ControlTester("generateButton").TheObject;
            Assert.AreEqual("Genereer &berekeningen...", button.Text);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(assessmentSection), new TestCalculatableFailureMechanism(), assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);
            Assert.AreEqual(2, dataGridView.ColumnCount);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Hydraulische belastingenlocatie", dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex].HeaderText);

            foreach (DataGridViewComboBoxColumn column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
            {
                Assert.AreEqual("DisplayName", column.DisplayMember);
                Assert.AreEqual("This", ((IReadOnlyList<DataGridViewComboBoxColumn>) dataGridView.Columns.OfType<DataGridViewComboBoxColumn>().ToArray())[0].ValueMember);
            }

            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(2, cells.Count);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 1 (2 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(2, cells.Count);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("Location 2 (6 m)", cells[selectableHydraulicBoundaryLocationsColumnIndex].FormattedValue);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HydraulicBoundaryDatabaseWithLocations_SelectableHydraulicBoundaryLocationsComboboxCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            // Call
            ShowCalculationsView(ConfigureCalculationGroup(assessmentSection), new TestCalculatableFailureMechanism(), assessmentSection);

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex];
            DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
            Assert.AreEqual(5, hydraulicBoundaryLocationComboboxItems.Count);
            Assert.AreEqual("<selecteer>", hydraulicBoundaryLocationComboboxItems[0].ToString());
            Assert.AreEqual("Location 1 (2 m)", hydraulicBoundaryLocationComboboxItems[1].ToString());
            Assert.AreEqual("Location 2 (6 m)", hydraulicBoundaryLocationComboboxItems[2].ToString());
            Assert.AreEqual("Location 1 (4 m)", hydraulicBoundaryLocationComboboxItems[3].ToString());
            Assert.AreEqual("Location 2 (5 m)", hydraulicBoundaryLocationComboboxItems[4].ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_IncorrectNameIndex_ThrowsInvalidOperationException()
        {
            // Call
            void Call() => new MissingNameColumnTestCalculationsView(new CalculationGroup(), new TestCalculatableFailureMechanism(), new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(Call);
            Assert.AreEqual("Both the name column and the hydraulic boundary database column need to be added to the data grid view.", exception.Message);
        }

        [Test]
        public void Constructor_IncorrectHydraulicBoundaryLocationIndex_ThrowsInvalidOperationException()
        {
            // Call
            void Call() => new MissingHydraulicBoundaryLocationTestCalculationsView(new CalculationGroup(), new TestCalculatableFailureMechanism(), new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(Call);
            Assert.AreEqual("Both the name column and the hydraulic boundary database column need to be added to the data grid view.", exception.Message);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GenerateButton_Always_HasCorrectState(bool state)
        {
            // Setup
            var view = new TestCalculationsView(new CalculationGroup(), new TestCalculatableFailureMechanism(), new AssessmentSectionStub())
            {
                CanGenerateCalculationState = state
            };

            testForm.Controls.Add(view);
            testForm.Show();

            var button = (Button) new ControlTester("generateButton").TheObject;

            // Call
            bool buttonState = button.Enabled;

            // Assert
            Assert.AreEqual(state, buttonState);
        }

        #endregion

        #region Selection

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculationsView_SelectingCellInRow_SelectionChangedFired()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            TestCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(assessmentSection);

            var selectionChangedCount = 0;
            calculationsView.SelectionChanged += (sender, args) => selectionChangedCount++;

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[0];

            // Assert
            Assert.AreEqual(1, selectionChangedCount);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Selection_DataGridViewCellSelected_ReturnsTheSelectedRowObject(int selectedRow)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            TestCalculationsView calculationsView = ShowFullyConfiguredCalculationsView(assessmentSection);

            // Call
            object selection = calculationsView.Selection;

            // Assert
            Assert.IsInstanceOf<TestCalculationRow>(selection);
            mocks.VerifyAll();
        }

        #endregion

        #region Observers

        [Test]
        public void GivenCalculationsView_WhenHydraulicBoundaryDatabaseWithLocationsUpdatedAndNotified_ThenSelectableHydraulicBoundaryLocationsComboboxCorrectlyUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            ShowCalculationsView(ConfigureCalculationGroup(assessmentSection), new TestCalculatableFailureMechanism(), assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var hydraulicBoundaryLocationCombobox = (DataGridViewComboBoxColumn) dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex];

            // Precondition
            Assert.AreEqual(5, hydraulicBoundaryLocationCombobox.Items.Count);

            // When
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(3, "Location 3", 5.5, 6.6));
            assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();

            // Then
            DataGridViewComboBoxCell.ObjectCollection hydraulicBoundaryLocationComboboxItems = hydraulicBoundaryLocationCombobox.Items;
            Assert.AreEqual(7, hydraulicBoundaryLocationComboboxItems.Count);
            Assert.AreEqual("<selecteer>", hydraulicBoundaryLocationComboboxItems[0].ToString());
            Assert.AreEqual("Location 1 (2 m)", hydraulicBoundaryLocationComboboxItems[1].ToString());
            Assert.AreEqual("Location 2 (6 m)", hydraulicBoundaryLocationComboboxItems[2].ToString());
            Assert.AreEqual("Location 3 (9 m)", hydraulicBoundaryLocationComboboxItems[3].ToString());
            Assert.AreEqual("Location 1 (4 m)", hydraulicBoundaryLocationComboboxItems[4].ToString());
            Assert.AreEqual("Location 2 (5 m)", hydraulicBoundaryLocationComboboxItems[5].ToString());
            Assert.AreEqual("Location 3 (7 m)", hydraulicBoundaryLocationComboboxItems[6].ToString());
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsView_WhenCalculationInputUpdatedAndNotified_ThenDataGridViewCorrectlyUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection);

            ShowCalculationsView(calculationGroup, new TestCalculatableFailureMechanism(), assessmentSection);
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            var dataSourceUpdated = 0;
            dataGridView.DataSourceChanged += (sender, args) => dataSourceUpdated++;

            // When
            TestCalculation calculation = calculationGroup.Children.Cast<TestCalculation>().First();
            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.Last();
            calculation.InputParameters.NotifyObservers();

            // Then
            Assert.AreEqual(1, dataSourceUpdated);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsView_WhenCalculationUpdatedAndNotified_ThenDataGridViewCorrectlyUpdated()
        {
            // Given
            const string calculationName = "New name";

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection);

            ShowCalculationsView(calculationGroup, new TestCalculatableFailureMechanism(), assessmentSection);
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            var invalidated = 0;
            dataGridView.Invalidated += (sender, args) => invalidated++;

            // When
            TestCalculation calculation = calculationGroup.Children.Cast<TestCalculation>().First();
            calculation.Name = calculationName;
            calculation.NotifyObservers();

            // Then
            Assert.AreEqual(calculationName, dataGridView.Rows[0].Cells[nameColumnIndex].Value);
            Assert.AreEqual(1, invalidated);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsView_WhenCalculationGroupUpdatedAndNotified_ThenDataGridViewCorrectlyUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            ConfigureHydraulicBoundaryDatabase(assessmentSection);
            mocks.ReplayAll();

            CalculationGroup calculationGroup = ConfigureCalculationGroup(assessmentSection);

            ShowCalculationsView(calculationGroup, new TestCalculatableFailureMechanism(), assessmentSection);
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            var dataSourceUpdated = 0;
            dataGridView.DataSourceChanged += (sender, args) => dataSourceUpdated++;

            // Precondition
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            // When
            calculationGroup.Children.Add(new TestCalculation());
            calculationGroup.NotifyObservers();

            // Then
            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual(1, dataSourceUpdated);
            mocks.VerifyAll();
        }

        #endregion

        #region Column state definitions

        private class TestCalculationRowWithColumnStateDefinitions : TestCalculationRow, IHasColumnStateDefinitions
        {
            public TestCalculationRowWithColumnStateDefinitions(TestCalculation calculation,
                                                                IObservablePropertyChangeHandler propertyChangeHandler,
                                                                bool initialReadOnlyState)
                : base(calculation, propertyChangeHandler)
            {
                ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>
                {
                    {
                        nameColumnIndex, new DataGridViewColumnStateDefinition()
                    },
                    {
                        selectableHydraulicBoundaryLocationsColumnIndex, new DataGridViewColumnStateDefinition()
                    }
                };

                SetReadOnlyState(initialReadOnlyState);
            }

            public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

            public void SetReadOnlyState(bool readOnlyState)
            {
                ColumnStateHelper.SetColumnState(ColumnStateDefinitions[selectableHydraulicBoundaryLocationsColumnIndex], readOnlyState);
            }
        }

        private class TestCalculationsViewWithColumnStateDefinitions : TestCalculationsViewBase<TestCalculationRowWithColumnStateDefinitions>
        {
            private readonly bool initialReadOnlyState;

            public TestCalculationsViewWithColumnStateDefinitions(CalculationGroup calculationGroup,
                                                                  TestCalculatableFailureMechanism failureMechanism,
                                                                  IAssessmentSection assessmentSection,
                                                                  bool initialReadOnlyState)
                : base(calculationGroup, failureMechanism, assessmentSection)
            {
                this.initialReadOnlyState = initialReadOnlyState;
            }

            protected override TestCalculationRowWithColumnStateDefinitions CreateRow(TestCalculation calculation)
            {
                return new TestCalculationRowWithColumnStateDefinitions(calculation,
                                                                        new ObservablePropertyChangeHandler(calculation, calculation.InputParameters),
                                                                        initialReadOnlyState);
            }
        }

        private void ShowFullyConfiguredCalculationsViewWithColumnStateDefinitions(IAssessmentSection assessmentSection, bool initialReadOnlyState)
        {
            ConfigureHydraulicBoundaryDatabase(assessmentSection);

            var calculationsView = new TestCalculationsViewWithColumnStateDefinitions(ConfigureCalculationGroup(assessmentSection),
                                                                                      new TestCalculatableFailureMechanism(),
                                                                                      assessmentSection,
                                                                                      initialReadOnlyState);

            testForm.Controls.Add(calculationsView);
            testForm.Show();
        }

        [Test]
        public void GivenCalculationsViewWithoutColumnStateDefinitions_ThenColumnStatesAsExpected()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ShowFullyConfiguredCalculationsView(assessmentSection);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Then
            Assert.AreEqual(false, dataGridView.Rows[0].Cells[nameColumnIndex].ReadOnly);
            Assert.AreEqual(false, dataGridView.Rows[0].Cells[selectableHydraulicBoundaryLocationsColumnIndex].ReadOnly);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculationsViewWithColumnStateDefinitions_ThenColumnStatesAsExpected(bool initialReadOnlyState)
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ShowFullyConfiguredCalculationsViewWithColumnStateDefinitions(assessmentSection, initialReadOnlyState);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Then
            Assert.AreEqual(false, dataGridView.Rows[0].Cells[nameColumnIndex].ReadOnly);
            Assert.AreEqual(initialReadOnlyState, dataGridView.Rows[0].Cells[selectableHydraulicBoundaryLocationsColumnIndex].ReadOnly);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculationsViewWithColumnStateDefinitions_WhenColumnStateChangedAndCalculationInputObserversNotifiedDuringEditAction_ThenColumnStatesAsExpected(bool initialReadOnlyState)
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ShowFullyConfiguredCalculationsViewWithColumnStateDefinitions(assessmentSection, initialReadOnlyState);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Precondition
            Assert.AreEqual(initialReadOnlyState, dataGridView.Rows[0].Cells[selectableHydraulicBoundaryLocationsColumnIndex].ReadOnly);

            // When
            var testCalculationRowWithColumnStateDefinitions = (TestCalculationRowWithColumnStateDefinitions) dataGridView.Rows[0].DataBoundItem;
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[nameColumnIndex];
            dataGridView.BeginEdit(false);
            testCalculationRowWithColumnStateDefinitions.SetReadOnlyState(!initialReadOnlyState);
            testCalculationRowWithColumnStateDefinitions.Calculation.InputParameters.NotifyObservers();

            // Then
            Assert.AreEqual(!initialReadOnlyState, dataGridView.Rows[0].Cells[selectableHydraulicBoundaryLocationsColumnIndex].ReadOnly);
        }

        #endregion
    }
}