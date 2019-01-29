// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Forms.Test
{
    [TestFixture]
    public class ScenarioSelectionControlTest
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
        public void DefaultConstructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (ScenarioSelectionControl control = ShowScenariosControl())
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl").TheObject;

                Assert.IsInstanceOf<UserControl>(control);

                Assert.AreEqual(new Size(0, 0), dataGridViewControl.MinimumSize);
                Assert.IsTrue(control.AutoScroll);

                Assert.AreEqual(0, dataGridView.RowCount);
                Assert.AreEqual(2, dataGridView.ColumnCount);

                DataGridViewColumn sectionColumn = dataGridView.Columns[0];
                DataGridViewColumn calculationColumn = dataGridView.Columns[1];

                Assert.AreEqual("Vak", sectionColumn.HeaderText);
                Assert.AreEqual("Berekening", calculationColumn.HeaderText);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(sectionColumn);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(calculationColumn);

                Assert.IsTrue(sectionColumn.ReadOnly);

                var comboBoxColumn = (DataGridViewComboBoxColumn) calculationColumn;
                Assert.AreEqual("WrappedObject", comboBoxColumn.ValueMember);
                Assert.AreEqual("DisplayName", comboBoxColumn.DisplayMember);
            }
        }

        [Test]
        public void UpdateDataGridViewDataSource_WithoutCalculations_ThrowsArgumentNullException()
        {
            // Setup
            using (var control = new ScenarioSelectionControl())
            {
                // Call
                TestDelegate call = () => control.UpdateDataGridViewDataSource<IScenarioRow<ICalculation>>(null, null, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;

                Assert.AreEqual("calculations", paramName);
            }
        }

        [Test]
        public void UpdateDataGridViewDataSource_WithoutScenarioRows_ThrowsArgumentNullException()
        {
            // Setup
            using (var control = new ScenarioSelectionControl())
            {
                // Call
                TestDelegate call = () => control.UpdateDataGridViewDataSource<IScenarioRow<ICalculation>>(Enumerable.Empty<ICalculation>(), null, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;

                Assert.AreEqual("scenarioRows", paramName);
            }
        }

        [Test]
        public void UpdateDataGridViewDataSource_WithoutCalculationsPerSection_ThrowsArgumentNullException()
        {
            // Setup
            using (var control = new ScenarioSelectionControl())
            {
                // Call
                TestDelegate call = () => control.UpdateDataGridViewDataSource(Enumerable.Empty<ICalculation>(), Enumerable.Empty<IScenarioRow<ICalculation>>(), null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;

                Assert.AreEqual("calculationsPerSection", paramName);
            }
        }

        [Test]
        public void UpdateDataGridViewDataSource_WithCalculationsRowsAndCalculationsPerSection_DataGridViewCorrectlyInitialized()
        {
            // Setup
            const string sectionNameA = "sectionNameA";
            const string sectionNameB = "sectionNameB";

            var mockRepository = new MockRepository();
            var calculationA = mockRepository.Stub<ICalculation>();
            var calculationB = mockRepository.Stub<ICalculation>();
            IScenarioRow<ICalculation> rowA = CreateScenarioRow(mockRepository, calculationA, sectionNameA);
            IScenarioRow<ICalculation> rowB = CreateScenarioRow(mockRepository, calculationB, sectionNameB);
            mockRepository.ReplayAll();

            using (ScenarioSelectionControl control = ShowScenariosControl())
            {
                // Call
                control.UpdateDataGridViewDataSource(new[]
                {
                    calculationA,
                    calculationB
                }, new[]
                {
                    rowA,
                    rowB
                }, new Dictionary<string, List<ICalculation>>
                {
                    {
                        sectionNameA, new List<ICalculation>
                        {
                            calculationA
                        }
                    },
                    {
                        sectionNameB, new List<ICalculation>
                        {
                            calculationB
                        }
                    }
                });

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreEqual(2, dataGridView.RowCount);

                var comboBoxColumn = (DataGridViewComboBoxColumn) dataGridView.Columns[1];
                Assert.AreEqual(2, comboBoxColumn.Items.Count);

                var cellA = (DataGridViewComboBoxCell) dataGridView[1, 0];
                CollectionAssert.AreEqual(
                    new[]
                    {
                        "<selecteer>",
                        calculationA.ToString()
                    },
                    cellA.Items.OfType<DataGridViewComboBoxItemWrapper<ICalculation>>().Select(r => r.DisplayName));

                var cellB = (DataGridViewComboBoxCell) dataGridView[1, 1];
                CollectionAssert.AreEqual(
                    new[]
                    {
                        "<selecteer>",
                        calculationB.ToString()
                    },
                    cellB.Items.OfType<DataGridViewComboBoxItemWrapper<ICalculation>>().Select(r => r.DisplayName));
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void ClearDataSource_WithPreviousData_DataGridViewEmpty()
        {
            // Setup
            const string sectionName = "sectionName";

            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            IScenarioRow<ICalculation> row = CreateScenarioRow(mockRepository, calculation, sectionName);
            mockRepository.ReplayAll();

            using (ScenarioSelectionControl control = ShowScenariosControl())
            {
                control.UpdateDataGridViewDataSource(new[]
                {
                    calculation
                }, new[]
                {
                    row
                }, new Dictionary<string, List<ICalculation>>
                {
                    {
                        sectionName, new List<ICalculation>
                        {
                            calculation
                        }
                    }
                });

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                control.ClearDataSource();

                // Assert
                Assert.AreEqual(0, dataGridView.RowCount);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void EndEdit_DataGridViewWithEditableRowsInEditMode_DataGridViewNotInEditMode()
        {
            // Setup
            const string sectionName = "sectionName";

            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            var row = new EditableScenarioRow
            {
                Name = sectionName,
                Calculation = calculation
            };

            using (ScenarioSelectionControl control = ShowScenariosControl())
            {
                control.UpdateDataGridViewDataSource(new[]
                {
                    calculation
                }, new[]
                {
                    row
                }, new Dictionary<string, List<ICalculation>>
                {
                    {
                        sectionName, new List<ICalculation>
                        {
                            calculation
                        }
                    }
                });

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Make sure we can set edit mode programmatically in order for the test to work
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                dataGridView.CurrentCell = dataGridView[1, 0];
                dataGridView.BeginEdit(false);

                // Precondition
                Assert.IsTrue(dataGridView.IsCurrentCellInEditMode, "Current cell should be in edit mode before EndEdit is called.");

                // Call
                control.EndEdit();

                // Assert
                Assert.IsFalse(dataGridView.IsCurrentCellInEditMode);
            }

            mockRepository.VerifyAll();
        }

        private ScenarioSelectionControl ShowScenariosControl()
        {
            var control = new ScenarioSelectionControl();
            testForm.Controls.Add(control);
            testForm.Show();

            return control;
        }

        private static IScenarioRow<ICalculation> CreateScenarioRow(MockRepository mocks, ICalculation calculation, string sectionName)
        {
            var row = mocks.Stub<IScenarioRow<ICalculation>>();
            row.Stub(r => r.Name).Return(sectionName);
            row.Stub(r => r.Calculation).Return(calculation);
            return row;
        }

        private class EditableScenarioRow : IScenarioRow<ICalculation>
        {
            public string Name { get; set; }
            public ICalculation Calculation { get; set; }
        }
    }
}