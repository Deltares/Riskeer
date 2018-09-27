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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class IllustrationPointsTableControlTest
    {
        private const int windDirectionColumnIndex = 0;
        private const int closingScenarioColumnIndex = 1;
        private const int calculatedProbabilityColumnIndex = 2;
        private const int calculatedReliabilityColumnIndex = 3;

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
        public void Constructor_ExpectedValues()
        {
            // Call
            var control = new IllustrationPointsTableControl();

            // Assert
            Assert.IsInstanceOf<UserControl>(control);
            Assert.IsInstanceOf<ISelectionProvider>(control);
            Assert.IsNull(control.Data);
            Assert.AreEqual(1, control.Controls.Count);
            Assert.IsInstanceOf<DataGridViewControl>(control.Controls[0]);
        }

        [Test]
        public void OnLoad_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowControl();

            // Assert
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");
            Assert.AreEqual(4, dataGridView.ColumnCount);
            DataGridViewColumn windDirectionColumn = dataGridView.Columns[windDirectionColumnIndex];
            Assert.AreEqual("Windrichting", windDirectionColumn.HeaderText);
            Assert.IsTrue(windDirectionColumn.ReadOnly);

            DataGridViewColumn closingSituationColumn = dataGridView.Columns[closingScenarioColumnIndex];
            Assert.AreEqual("Keringsituatie", closingSituationColumn.HeaderText);
            Assert.IsTrue(closingSituationColumn.ReadOnly);
            Assert.IsFalse(closingSituationColumn.Visible);

            DataGridViewColumn calculatedProbabilityColumn = dataGridView.Columns[calculatedProbabilityColumnIndex];
            Assert.AreEqual("Berekende kans [-]", calculatedProbabilityColumn.HeaderText);
            Assert.IsTrue(calculatedProbabilityColumn.ReadOnly);

            DataGridViewColumn calculatedReliabilityColumn = dataGridView.Columns[calculatedReliabilityColumnIndex];
            Assert.AreEqual("Betrouwbaarheidsindex berekende kans [-]", calculatedReliabilityColumn.HeaderText);
            Assert.IsTrue(calculatedReliabilityColumn.ReadOnly);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Data_SetNewValueWithDifferentClosingSituations_DataGridViewCorrectlyInitialized()
        {
            // Setup
            IEnumerable<IllustrationPointControlItem> data = GetControlItems();
            IllustrationPointsTableControl control = ShowControl();

            // Call
            control.Data = data;

            // Assert
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual("SSE", cells[windDirectionColumnIndex].FormattedValue);
            Assert.AreEqual("Regular", cells[closingScenarioColumnIndex].FormattedValue);
            Assert.AreEqual("1/5", cells[calculatedProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual("0,90000", cells[calculatedReliabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual("SSE", cells[windDirectionColumnIndex].FormattedValue);
            Assert.AreEqual("Open", cells[closingScenarioColumnIndex].FormattedValue);
            Assert.AreEqual("1/4", cells[calculatedProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual("0,70000", cells[calculatedReliabilityColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual("NE", cells[windDirectionColumnIndex].FormattedValue);
            Assert.AreEqual("Open", cells[closingScenarioColumnIndex].FormattedValue);
            Assert.AreEqual("1/5", cells[calculatedProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual("0,80000", cells[calculatedReliabilityColumnIndex].FormattedValue);

            Assert.IsTrue(dataGridView.Columns[closingScenarioColumnIndex].Visible);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Data_SetNewValueWithSameClosingSituations_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var data = new[]
            {
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "SSE",
                                                 "Regular",
                                                 Enumerable.Empty<Stochast>(),
                                                 new RoundedDouble(5, 0.9)),
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "SSE",
                                                 "Regular",
                                                 Enumerable.Empty<Stochast>(),
                                                 new RoundedDouble(5, 0.7))
            };
            IllustrationPointsTableControl control = ShowControl();

            // Call
            control.Data = data;

            // Assert
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual("SSE", cells[windDirectionColumnIndex].FormattedValue);
            Assert.AreEqual("Regular", cells[closingScenarioColumnIndex].FormattedValue);
            Assert.AreEqual("1/5", cells[calculatedProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual("0,90000", cells[calculatedReliabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual("SSE", cells[windDirectionColumnIndex].FormattedValue);
            Assert.AreEqual("Regular", cells[closingScenarioColumnIndex].FormattedValue);
            Assert.AreEqual("1/4", cells[calculatedProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual("0,70000", cells[calculatedReliabilityColumnIndex].FormattedValue);

            Assert.IsFalse(dataGridView.Columns[closingScenarioColumnIndex].Visible);
        }

        [Test]
        public void Data_SetToNull_DataGridViewCleared()
        {
            // Setup
            IEnumerable<IllustrationPointControlItem> data = GetControlItems();
            IllustrationPointsTableControl control = ShowControl();
            control.Data = data;

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");
            DataGridViewRowCollection rows = dataGridView.Rows;

            // Precondition
            Assert.AreEqual(3, rows.Count);

            // Call
            control.Data = null;

            // Assert
            Assert.AreEqual(0, rows.Count);

            Assert.IsFalse(dataGridView.Columns[closingScenarioColumnIndex].Visible);
        }

        [Test]
        public void GivenFullyConfiguredControl_WhenSelectingCellInRow_ThenSelectionChangedFired()
        {
            // Given
            IllustrationPointsTableControl control = ShowControl();
            control.Data = GetControlItems();

            var selectionChangedCount = 0;
            control.SelectionChanged += (sender, args) => selectionChangedCount++;

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[calculatedProbabilityColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(1, selectionChangedCount);
        }

        [Test]
        public void Selection_WithoutIllustrationPoints_ReturnsNull()
        {
            // Call
            using (var control = new IllustrationPointsTableControl())
            {
                // Assert
                Assert.IsNull(control.Selection);
            }
        }

        [Test]
        public void Selection_WithIllustrationPoints_ReturnsIllustrationPointControlItem()
        {
            // Setup
            IllustrationPointsTableControl control = ShowControl();
            control.Data = GetControlItems();

            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "illustrationPointsDataGridViewControl");

            // Call
            var selection = control.Selection as IllustrationPointControlItem;

            // Assert
            Assert.NotNull(selection);

            var dataBoundItem = dataGridView.Rows[0].DataBoundItem as IllustrationPointRow;
            Assert.NotNull(dataBoundItem);
            Assert.AreSame(dataBoundItem.IllustrationPointControlItem, selection);
        }

        [Test]
        public void GivenControlWithIllustrationPoints_WhenSelectingSecondRow_ThenSelectionReturnsIllustrationPointControlItem()
        {
            // Given
            IllustrationPointsTableControl control = ShowControl();
            control.Data = GetControlItems();

            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "illustrationPointsDataGridViewControl");

            // When
            DataGridViewRow selectedRow = dataGridView.Rows[1];
            dataGridView.SetCurrentCell(selectedRow.Cells[0]);

            // Then
            var selection = control.Selection as IllustrationPointControlItem;
            var dataBoundItem = selectedRow.DataBoundItem as IllustrationPointRow;

            Assert.NotNull(selection);
            Assert.NotNull(dataBoundItem);
            Assert.AreSame(dataBoundItem.IllustrationPointControlItem, selection);
        }

        private IllustrationPointsTableControl ShowControl()
        {
            var control = new IllustrationPointsTableControl();

            testForm.Controls.Add(control);
            testForm.Show();

            return control;
        }

        private static IEnumerable<IllustrationPointControlItem> GetControlItems()
        {
            return new[]
            {
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "SSE",
                                                 "Regular",
                                                 Enumerable.Empty<Stochast>(),
                                                 new RoundedDouble(5, 0.9)),
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "SSE",
                                                 "Open",
                                                 Enumerable.Empty<Stochast>(),
                                                 new RoundedDouble(5, 0.7)),
                new IllustrationPointControlItem(new TestTopLevelIllustrationPoint(),
                                                 "NE",
                                                 "Open",
                                                 Enumerable.Empty<Stochast>(),
                                                 new RoundedDouble(5, 0.8))
            };
        }
    }
}