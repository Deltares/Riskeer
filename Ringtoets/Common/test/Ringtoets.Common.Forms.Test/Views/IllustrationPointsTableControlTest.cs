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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
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
            Assert.IsNull(control.Data);
            Assert.AreEqual(1, control.Controls.Count);
            Assert.IsInstanceOf<DataGridViewControl>(control.Controls[0]);
        }

        [Test]
        public void OnLoad_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            IllustrationPointsTableControl control = ShowControl();

            // Assert
            var dataGridView = (DataGridView) control.Controls.Find("DataGridView", true).Single();
            Assert.AreEqual(4, dataGridView.ColumnCount);
            DataGridViewColumn windDirectionColumn = dataGridView.Columns[windDirectionColumnIndex];
            Assert.AreEqual("Windrichting", windDirectionColumn.HeaderText);
            Assert.IsTrue(windDirectionColumn.ReadOnly);

            DataGridViewColumn closingSituationColumn = dataGridView.Columns[closingScenarioColumnIndex];
            Assert.AreEqual("Sluitscenario", closingSituationColumn.HeaderText);
            Assert.IsTrue(closingSituationColumn.ReadOnly);
            Assert.IsFalse(closingSituationColumn.Visible);

            DataGridViewColumn calculatedProbabilityColumn = dataGridView.Columns[calculatedProbabilityColumnIndex];
            Assert.AreEqual("Berekende kans", calculatedProbabilityColumn.HeaderText);
            Assert.IsTrue(calculatedProbabilityColumn.ReadOnly);

            DataGridViewColumn calculatedReliabilityColumn = dataGridView.Columns[calculatedReliabilityColumnIndex];
            Assert.AreEqual("Berekende beta", calculatedReliabilityColumn.HeaderText);
            Assert.IsTrue(calculatedReliabilityColumn.ReadOnly);
        }

        [Test]
        public void Data_SetNewValueWithDifferentClosingSituations_DataGridViewCorrectlyInitialized()
        {
            // Setup
            GeneralResultSubMechanismIllustrationPoint data = GetGeneralResult();
            IllustrationPointsTableControl control = ShowControl();

            // Call
            control.Data = data;

            // Assert
            var dataGridView = (DataGridView) control.Controls.Find("DataGridView", true).Single();

            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual("SSE", cells[windDirectionColumnIndex].FormattedValue);
            Assert.AreEqual("Regular", cells[closingScenarioColumnIndex].FormattedValue);
            Assert.AreEqual(0.18406, Convert.ToDouble(cells[calculatedProbabilityColumnIndex].FormattedValue), 1e-5);
            Assert.AreEqual(0.9.ToString(CultureInfo.CurrentCulture), cells[calculatedReliabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual("SSE", cells[windDirectionColumnIndex].FormattedValue);
            Assert.AreEqual("Open", cells[closingScenarioColumnIndex].FormattedValue);
            Assert.AreEqual(0.24196, Convert.ToDouble(cells[calculatedProbabilityColumnIndex].FormattedValue), 1e-5);
            Assert.AreEqual(0.7.ToString(CultureInfo.CurrentCulture), cells[calculatedReliabilityColumnIndex].FormattedValue);

            Assert.IsTrue(dataGridView.Columns[closingScenarioColumnIndex].Visible);
        }

        [Test]
        public void Data_SetNewValueWithSameClosingSituations_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var data = new GeneralResultSubMechanismIllustrationPoint(
                new TestWindDirection(),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new SubMechanismIllustrationPoint("Point 1", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                          Enumerable.Empty<IllustrationPointResult>(), 0.9)),
                    new TopLevelSubMechanismIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new SubMechanismIllustrationPoint("Point 2", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                          Enumerable.Empty<IllustrationPointResult>(), 0.7))
                });
            IllustrationPointsTableControl control = ShowControl();

            // Call
            control.Data = data;

            // Assert
            var dataGridView = (DataGridView) control.Controls.Find("DataGridView", true).Single();

            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual("SSE", cells[windDirectionColumnIndex].FormattedValue);
            Assert.AreEqual("Regular", cells[closingScenarioColumnIndex].FormattedValue);
            Assert.AreEqual(0.18406, Convert.ToDouble(cells[calculatedProbabilityColumnIndex].FormattedValue), 1e-5);
            Assert.AreEqual(0.9.ToString(CultureInfo.CurrentCulture), cells[calculatedReliabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual("SSE", cells[windDirectionColumnIndex].FormattedValue);
            Assert.AreEqual("Regular", cells[closingScenarioColumnIndex].FormattedValue);
            Assert.AreEqual(0.24196, Convert.ToDouble(cells[calculatedProbabilityColumnIndex].FormattedValue), 1e-5);
            Assert.AreEqual(0.7.ToString(CultureInfo.CurrentCulture), cells[calculatedReliabilityColumnIndex].FormattedValue);

            Assert.IsFalse(dataGridView.Columns[closingScenarioColumnIndex].Visible);
        }

        [Test]
        public void Data_SetToNull_DataGridViewCleared()
        {
            // Setup
            GeneralResultSubMechanismIllustrationPoint data = GetGeneralResult();
            IllustrationPointsTableControl control = ShowControl();
            control.Data = data;

            var dataGridView = (DataGridView)control.Controls.Find("DataGridView", true).Single();
            DataGridViewRowCollection rows = dataGridView.Rows;

            // Precondition
            Assert.AreEqual(2, rows.Count);

            // Call
            control.Data = null;

            // Assert
            Assert.AreEqual(0, rows.Count);

            Assert.IsFalse(dataGridView.Columns[closingScenarioColumnIndex].Visible);
        }

        private IllustrationPointsTableControl ShowControl()
        {
            var control = new IllustrationPointsTableControl();

            testForm.Controls.Add(control);
            testForm.Show();

            return control;
        }

        private static GeneralResultSubMechanismIllustrationPoint GetGeneralResult()
        {
            return new GeneralResultSubMechanismIllustrationPoint(
                new TestWindDirection(),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(
                        new TestWindDirection(), "Regular",
                        new SubMechanismIllustrationPoint("Point 1", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                          Enumerable.Empty<IllustrationPointResult>(), 0.9)),
                    new TopLevelSubMechanismIllustrationPoint(
                        new TestWindDirection(), "Open",
                        new SubMechanismIllustrationPoint("Point 2", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                          Enumerable.Empty<IllustrationPointResult>(), 0.7))
                });
        }
    }
}