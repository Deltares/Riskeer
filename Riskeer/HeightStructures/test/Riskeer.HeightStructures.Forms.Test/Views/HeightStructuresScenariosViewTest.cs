﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.HeightStructures.Forms.Views;

namespace Riskeer.HeightStructures.Forms.Test.Views
{
    [TestFixture]
    public class HeightStructuresScenariosViewTest
    {
        private const int isRelevantColumnIndex = 0;
        private const int contributionColumnIndex = 1;
        private const int nameColumnIndex = 2;
        private const int failureProbabilityColumnIndex = 3;

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
            // Setup
            var calculationGroup = new CalculationGroup();

            // Call
            using (var view = new HeightStructuresScenariosView(calculationGroup, new HeightStructuresFailureMechanism()))
            {
                // Assert
                Assert.IsInstanceOf<ScenariosView<StructuresCalculationScenario<HeightStructuresInput>, HeightStructuresInput, HeightStructuresScenarioRow, HeightStructuresFailureMechanism>>(view);
                Assert.AreSame(calculationGroup, view.Data);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowHeightStructuresScenariosView(new HeightStructuresFailureMechanism());

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(4, dataGridView.ColumnCount);
            Assert.AreEqual("In oordeel", dataGridView.Columns[isRelevantColumnIndex].HeaderText);
            Assert.AreEqual("Bijdrage aan\r\nscenario\r\n[%]", dataGridView.Columns[contributionColumnIndex].HeaderText);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans\r\n[1/jaar]", dataGridView.Columns[failureProbabilityColumnIndex].HeaderText);
        }

        [Test]
        public void HeightStructuresScenarioView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredHeightStructuresScenariosView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.5), cells[failureProbabilityColumnIndex].FormattedValue);
        }

        private void ShowFullyConfiguredHeightStructuresScenariosView()
        {
            var structure1 = new TestHeightStructure(new Point2D(0.0, 0.0));
            var structure2 = new TestHeightStructure(new Point2D(5.0, 0.0));

            var failureMechanism = new HeightStructuresFailureMechanism();

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                }),
                new FailureMechanismSection("Section 2", new[]
                {
                    new Point2D(5.0, 0.0),
                    new Point2D(10.0, 0.0)
                })
            });

            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                new StructuresCalculationScenario<HeightStructuresInput>
                {
                    Name = "Calculation 1",
                    InputParameters =
                    {
                        Structure = structure1
                    }
                },
                new StructuresCalculationScenario<HeightStructuresInput>
                {
                    Name = "Calculation 2",
                    InputParameters =
                    {
                        Structure = structure2
                    },
                    Output = new TestStructuresOutput(0.2)
                }
            });

            ShowHeightStructuresScenariosView(failureMechanism);
        }

        private void ShowHeightStructuresScenariosView(HeightStructuresFailureMechanism failureMechanism)
        {
            var scenariosView = new HeightStructuresScenariosView(failureMechanism.CalculationsGroup, failureMechanism);
            testForm.Controls.Add(scenariosView);
            testForm.Show();
        }
    }
}