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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
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

        private IllustrationPointsTableControl ShowControl()
        {
            var control = new IllustrationPointsTableControl();

            testForm.Controls.Add(control);
            testForm.Show();

            return control;
        }
    }
}