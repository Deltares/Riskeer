// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.GuiServices;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryCalculationsViewTest
    {
        private const int calculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int hydraulicBoundaryDatabaseFileNameColumnIndex = 4;

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
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraulicBoundaryCalculationsView(null,
                                                                                new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraulicBoundaryCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new TestHydraulicBoundaryCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                        new AssessmentSectionStub()))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);

                Assert.AreEqual(1, view.Controls.Count);

                var splitContainer = view.Controls[0] as SplitContainer;
                Assert.IsNotNull(splitContainer);
                Control.ControlCollection verticalSplitContainerPanel1Controls = splitContainer.Panel1.Controls;
                Assert.AreEqual(new Size(535, 0), splitContainer.Panel1.AutoScrollMinSize);
                Assert.AreEqual(1, verticalSplitContainerPanel1Controls.Count);
                Assert.IsInstanceOf<TableLayoutPanel>(verticalSplitContainerPanel1Controls[0]);

                var tableLayoutPanel = (TableLayoutPanel) verticalSplitContainerPanel1Controls[0];
                Assert.AreEqual(1, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(3, tableLayoutPanel.RowCount);
                Assert.IsInstanceOf<CheckBox>(tableLayoutPanel.GetControlFromPosition(0, 0));
                Assert.IsInstanceOf<DataGridViewControl>(tableLayoutPanel.GetControlFromPosition(0, 1));
                Assert.IsInstanceOf<GroupBox>(tableLayoutPanel.GetControlFromPosition(0, 2));

                Control.ControlCollection verticalSplitContainerPanel2Controls = splitContainer.Panel2.Controls;
                Assert.AreEqual(1, verticalSplitContainerPanel2Controls.Count);
                Assert.IsInstanceOf<IllustrationPointsControl>(verticalSplitContainerPanel2Controls[0]);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowTestHydraulicBoundaryCalculationsView();

            // Assert
            DataGridView dataGridView = GetDataGridView();
            Assert.AreEqual(5, dataGridView.ColumnCount);

            var calculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[calculateColumnIndex];
            Assert.AreEqual("Berekenen", calculateColumn.HeaderText);
            Assert.IsTrue(calculateColumn.Visible);

            var includeIllustrationPointsColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[includeIllustrationPointsColumnIndex];
            Assert.AreEqual("Illustratiepunten inlezen", includeIllustrationPointsColumn.HeaderText);
            Assert.IsTrue(includeIllustrationPointsColumn.Visible);

            var locationNameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);
            Assert.IsTrue(locationNameColumn.Visible);

            var locationIdColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);
            Assert.IsTrue(locationIdColumn.Visible);

            var hydraulicBoundaryDatabaseFileNameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[hydraulicBoundaryDatabaseFileNameColumnIndex];
            Assert.AreEqual("HRD bestand", hydraulicBoundaryDatabaseFileNameColumn.HeaderText);
            Assert.IsFalse(hydraulicBoundaryDatabaseFileNameColumn.Visible);
        }

        [Test]
        public void Constructor_CalculateAllButtonCorrectlyInitialized()
        {
            // Setup & Call
            TestHydraulicBoundaryCalculationsView view = ShowTestHydraulicBoundaryCalculationsView();

            // Assert
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Constructor_CheckBoxCorrectlyInitialized()
        {
            // Setup & Call
            ShowTestHydraulicBoundaryCalculationsView();

            // Assert
            CheckBox checkBox = GetSHowHydraulicBoundaryDatabaseFileNameCheckBox();
            Assert.AreEqual("Toon HRD bestand", checkBox.Text);
            Assert.IsFalse(checkBox.Checked);
        }

        [Test]
        public void Constructor_WithCalculations_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredTestHydraulicBoundaryCalculationsView();

            // Assert
            DataGridView dataGridView = GetDataGridView();
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual("database1", cells[hydraulicBoundaryDatabaseFileNameColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual("database1", cells[hydraulicBoundaryDatabaseFileNameColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual("database2", cells[hydraulicBoundaryDatabaseFileNameColumnIndex].FormattedValue);
        }

        [Test]
        public void Constructor_CalculationsWithIllustrationPointsOutput_IllustrationPointControlDataCorrectlySet()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };

            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            var output = new TestHydraulicBoundaryLocationCalculationOutput(generalResult);

            var calculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                Output = output
            };
            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                calculation
            };

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Add(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            });

            // Call
            ShowTestHydraulicBoundaryCalculationsView(calculations, assessmentSection);

            // Assert
            IllustrationPointsControl illustrationPointControl = GetIllustrationPointsControl();
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointControl.Data,
                                      new IllustrationPointControlItemComparer());
        }

        [Test]
        public void Selection_WithoutCalculations_ReturnsNull()
        {
            // Call
            using (TestHydraulicBoundaryCalculationsView view = ShowTestHydraulicBoundaryCalculationsView())
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCellInRow_ThenSelectionChangedFired()
        {
            // Given
            TestHydraulicBoundaryCalculationsView view = ShowFullyConfiguredTestHydraulicBoundaryCalculationsView();

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            DataGridView dataGridView = GetDataGridView();

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[calculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(1, selectionChangedCount);
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllCalculationsSelected()
        {
            // Setup
            ShowFullyConfiguredTestHydraulicBoundaryCalculationsView();

            DataGridView dataGridView = GetDataGridView();
            DataGridViewRowCollection rows = dataGridView.Rows;
            var button = new ButtonTester("SelectAllButton", testForm);

            // Precondition
            Assert.IsFalse((bool) rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[calculateColumnIndex].Value);

            // Call
            button.Click();

            // Assert
            Assert.IsTrue((bool) rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsTrue((bool) rows[1].Cells[calculateColumnIndex].Value);
        }

        [Test]
        public void DeselectAllButton_AllCalculationsSelectedDeselectAllButtonClicked_AllCalculationsNotSelected()
        {
            // Setup
            ShowFullyConfiguredTestHydraulicBoundaryCalculationsView();

            DataGridView dataGridView = GetDataGridView();
            var button = new ButtonTester("DeselectAllButton", testForm);

            DataGridViewRowCollection rows = dataGridView.Rows;
            foreach (DataGridViewRow row in rows)
            {
                row.Cells[calculateColumnIndex].Value = true;
            }

            // Precondition
            Assert.IsTrue((bool) rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsTrue((bool) rows[1].Cells[calculateColumnIndex].Value);

            // Call
            button.Click();

            // Assert
            Assert.IsFalse((bool) rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[calculateColumnIndex].Value);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenNoRowsSelected_ThenCalculateForSelectedButtonDisabledAndErrorMessageProvided()
        {
            // Given & When
            TestHydraulicBoundaryCalculationsView view = ShowFullyConfiguredTestHydraulicBoundaryCalculationsView();

            // Then
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsFalse(button.Enabled);
            ErrorProvider errorProvider = GetErrorProvider(view);
            Assert.AreEqual("Er zijn geen berekeningen geselecteerd.", errorProvider.GetError(button));
        }

        [Test]
        public void GivenFullyConfiguredView_WhenRowsSelected_ThenCalculateForSelectedButtonEnabledAndNoErrorMessageProvided()
        {
            // Given
            TestHydraulicBoundaryCalculationsView view = ShowFullyConfiguredTestHydraulicBoundaryCalculationsView();
            DataGridView dataGridView = GetDataGridView();

            // When
            dataGridView.Rows[0].Cells[calculateColumnIndex].Value = true;

            // Then
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsTrue(button.Enabled);
            ErrorProvider errorProvider = GetErrorProvider(view);
            Assert.AreEqual("", errorProvider.GetError(button));
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateHandleCalculateSelectedCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var guiService = mocks.Stub<IHydraulicBoundaryLocationCalculationGuiService>();
            mocks.ReplayAll();

            TestHydraulicBoundaryCalculationsView view = ShowFullyConfiguredTestHydraulicBoundaryCalculationsView();
            view.CalculationGuiService = guiService;

            DataGridView dataGridView = GetDataGridView();

            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.AreEqual(1, view.ObjectsToCalculate.Count());

            HydraulicBoundaryLocationCalculationRow[] rowsToBeCalculated = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                                       .Select(r => r.DataBoundItem)
                                                                                       .Cast<HydraulicBoundaryLocationCalculationRow>()
                                                                                       .ToArray();
            Assert.AreEqual(rowsToBeCalculated.First().CalculatableObject, view.ObjectsToCalculate.First());
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedAndCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredTestHydraulicBoundaryCalculationsView();

            DataGridView dataGridView = GetDataGridView();
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void GivenView_WhenCheckingShowHydraulicBoundaryDatabaseFileNameColumnCheckBox_ThenColumnVisible()
        {
            // Given
            ShowTestHydraulicBoundaryCalculationsView();

            DataGridView dataGridView = GetDataGridView();

            // Precondition
            var hydraulicBoundaryDatabaseFileNameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[hydraulicBoundaryDatabaseFileNameColumnIndex];
            Assert.IsFalse(hydraulicBoundaryDatabaseFileNameColumn.Visible);

            // When
            CheckBox checkBox = GetSHowHydraulicBoundaryDatabaseFileNameCheckBox();
            checkBox.Checked = true;

            // Then
            Assert.IsTrue(hydraulicBoundaryDatabaseFileNameColumn.Visible);
        }

        [Test]
        public void GivenView_WhenCalculationUpdated_ThenDataGridViewCorrectlyUpdated()
        {
            // Given
            var location = new TestHydraulicBoundaryLocation();
            var calculation = new HydraulicBoundaryLocationCalculation(location);

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Add(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            });

            ShowTestHydraulicBoundaryCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                calculation
            }, assessmentSection);

            // Precondition
            DataGridViewControl calculationsDataGridViewControl = GetDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);

            // When
            calculation.InputParameters.ShouldIllustrationPointsBeCalculated = true;
            calculation.NotifyObservers();

            // Then
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
        }

        [Test]
        public void GivenView_WhenCalculationUpdated_ThenIllustrationPointsControlCorrectlyUpdated()
        {
            // Given
            var location = new TestHydraulicBoundaryLocation();
            var calculation = new HydraulicBoundaryLocationCalculation(location);

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Add(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            });

            ShowTestHydraulicBoundaryCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                calculation
            }, assessmentSection);

            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl calculationsDataGridViewControl = GetDataGridViewControl();

            calculationsDataGridViewControl.SetCurrentCell(calculationsDataGridViewControl.GetCell(0, 0));

            // Precondition
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);

            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            var output = new TestHydraulicBoundaryLocationCalculationOutput(generalResult);

            // Call
            calculation.Output = output;
            calculation.NotifyObservers();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }

        private DataGridView GetDataGridView()
        {
            return ControlTestHelper.GetDataGridView(testForm, "dataGridView");
        }

        private DataGridViewControl GetDataGridViewControl()
        {
            return ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
        }

        private static ErrorProvider GetErrorProvider(TestHydraulicBoundaryCalculationsView view)
        {
            return TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
        }

        private CheckBox GetSHowHydraulicBoundaryDatabaseFileNameCheckBox()
        {
            return ControlTestHelper.GetControls<CheckBox>(testForm, "showHydraulicBoundaryDatabaseFileNameColumnCheckBox").Single();
        }

        private IllustrationPointsControl GetIllustrationPointsControl()
        {
            return ControlTestHelper.GetControls<IllustrationPointsControl>(testForm, "IllustrationPointsControl").Single();
        }

        private static IEnumerable<IllustrationPointControlItem> CreateControlItems(
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult)
        {
            return generalResult.TopLevelIllustrationPoints
                                .Select(topLevelIllustrationPoint =>
                                {
                                    SubMechanismIllustrationPoint illustrationPoint = topLevelIllustrationPoint.SubMechanismIllustrationPoint;
                                    return new IllustrationPointControlItem(topLevelIllustrationPoint,
                                                                            topLevelIllustrationPoint.WindDirection.Name,
                                                                            topLevelIllustrationPoint.ClosingSituation,
                                                                            illustrationPoint.Stochasts,
                                                                            illustrationPoint.Beta);
                                });
        }

        private TestHydraulicBoundaryCalculationsView ShowTestHydraulicBoundaryCalculationsView()
        {
            var view = new TestHydraulicBoundaryCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                 new AssessmentSectionStub());

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private TestHydraulicBoundaryCalculationsView ShowTestHydraulicBoundaryCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                                                IAssessmentSection assessmentSection)
        {
            var view = new TestHydraulicBoundaryCalculationsView(calculations, assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private TestHydraulicBoundaryCalculationsView ShowFullyConfiguredTestHydraulicBoundaryCalculationsView()
        {
            var location1 = new HydraulicBoundaryLocation(1, "1", 1.0, 1.0);
            var location2 = new HydraulicBoundaryLocation(2, "2", 2.0, 2.0);
            var location3 = new HydraulicBoundaryLocation(3, "3", 3.0, 3.0);

            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(location1),
                new HydraulicBoundaryLocationCalculation(location2)
                {
                    Output = new TestHydraulicBoundaryLocationCalculationOutput(1.23)
                },
                new HydraulicBoundaryLocationCalculation(location3)
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    }
                }
            };

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.AddRange(new[]
            {
                new HydraulicBoundaryDatabase
                {
                    FilePath = @"path\to\database1.sqlite",
                    Locations =
                    {
                        location1,
                        location2
                    }
                },
                new HydraulicBoundaryDatabase
                {
                    FilePath = @"path\to\database2.sqlite",
                    Locations =
                    {
                        location3
                    }
                }
            });

            return ShowTestHydraulicBoundaryCalculationsView(calculations, assessmentSection);
        }

        private class TestHydraulicBoundaryCalculationsView : HydraulicBoundaryCalculationsView
        {
            public TestHydraulicBoundaryCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                         IAssessmentSection assessmentSection)
                : base(calculations, assessmentSection)
            {
                ObjectsToCalculate = new List<HydraulicBoundaryLocationCalculation>();
            }

            public IEnumerable<HydraulicBoundaryLocationCalculation> ObjectsToCalculate { get; private set; }

            protected override void PerformSelectedCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
            {
                ObjectsToCalculate = calculations;
            }

            protected override object CreateSelectedItemFromCurrentRow()
            {
                return null;
            }
        }
    }
}