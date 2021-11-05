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
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.Views;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsScenariosViewTest
    {
        private const int isRelevantColumnIndex = 0;
        private const int contributionColumnIndex = 1;
        private const int nameColumnIndex = 2;
        private const int failureProbabilityColumnIndex = 3;
        private const int sectionFailureProbabilityColumnIndex = 4;
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
            using (var scenarioView = new MacroStabilityInwardsScenariosView(calculationGroup, new MacroStabilityInwardsFailureMechanism()))
            {
                // Assert
                Assert.IsInstanceOf<ScenariosView<MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsInput, MacroStabilityInwardsScenarioRow, MacroStabilityInwardsFailureMechanism>>(scenarioView);
                Assert.AreSame(calculationGroup, scenarioView.Data);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowMacroStabilityInwardsScenariosView(new MacroStabilityInwardsFailureMechanism());

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(5, dataGridView.ColumnCount);
            Assert.AreEqual("In oordeel", dataGridView.Columns[isRelevantColumnIndex].HeaderText);
            Assert.AreEqual("Bijdrage aan\r\nscenario\r\n[%]", dataGridView.Columns[contributionColumnIndex].HeaderText);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans per doorsnede\r\n[1/jaar]", dataGridView.Columns[failureProbabilityColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans per vak\r\n[1/jaar]", dataGridView.Columns[sectionFailureProbabilityColumnIndex].HeaderText);
        }

        [Test]
        public void MacroStabilityInwardsScenarioView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredMacroStabilityInwardsScenariosView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[sectionFailureProbabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(1), cells[failureProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(5.8), cells[sectionFailureProbabilityColumnIndex].FormattedValue);
        }

        private void ShowFullyConfiguredMacroStabilityInwardsScenariosView()
        {
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine("Surface line 1")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine("Surface line 2")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(8000.0, 0.0)
                }),
                new FailureMechanismSection("Section 2", new[]
                {
                    new Point2D(8000.0, 0.0),
                    new Point2D(9000.0, 0.0)
                })
            });

            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Calculation 1",
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine1
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Calculation 2",
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine2
                    },
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput(new MacroStabilityInwardsOutput.ConstructionProperties
                    {
                        FactorOfStability = 0.2
                    })
                }
            });

            ShowMacroStabilityInwardsScenariosView(failureMechanism);
        }

        private void ShowMacroStabilityInwardsScenariosView(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            var scenarioView = new MacroStabilityInwardsScenariosView(failureMechanism.CalculationsGroup, failureMechanism);

            testForm.Controls.Add(scenarioView);
            testForm.Show();
        }
    }
}