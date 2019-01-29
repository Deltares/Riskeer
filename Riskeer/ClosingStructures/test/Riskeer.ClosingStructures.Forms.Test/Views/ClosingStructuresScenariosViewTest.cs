﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms;

namespace Riskeer.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresScenariosViewTest
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
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsTrue(view.AutoScroll);
                Assert.IsNull(view.Data);
                Assert.IsNull(view.FailureMechanism);

                var scenarioSelectionControl = new ControlTester("scenarioSelectionControl").TheObject as ScenarioSelectionControl;

                Assert.NotNull(scenarioSelectionControl);
                Assert.AreEqual(new Size(0, 0), scenarioSelectionControl.MinimumSize);
            }
        }

        [Test]
        public void Data_ValidDataSet_ValidData()
        {
            // Setup
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                var calculationGroup = new CalculationGroup();

                // Call
                view.Data = calculationGroup;

                // Assert
                Assert.AreSame(calculationGroup, view.Data);
            }
        }

        [Test]
        public void FailureMechanism_ValidFailureMechanismSet_ValidFailureMechanism()
        {
            // Setup
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                var failureMechanism = new ClosingStructuresFailureMechanism();

                // Call
                view.FailureMechanism = failureMechanism;

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void Data_WithFailureMechanism_UpdateScenarioControl()
        {
            // Setup
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                ClosingStructuresFailureMechanism failureMechanism = CreateCompleteFailureMechanism();
                view.FailureMechanism = failureMechanism;

                // Call
                view.Data = failureMechanism.CalculationsGroup;

                // Assert
                AssertDataGridView(failureMechanism, false, new[]
                {
                    new[]
                    {
                        "<selecteer>",
                        "CalculationA"
                    },
                    new[]
                    {
                        "<selecteer>",
                        "CalculationB"
                    }
                });
            }
        }

        [Test]
        public void Data_SetToNullAfterGridViewShowsData_ClearsScenarioControl()
        {
            // Setup
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                ClosingStructuresFailureMechanism failureMechanism = CreateCompleteFailureMechanism();
                view.FailureMechanism = failureMechanism;
                view.Data = failureMechanism.CalculationsGroup;

                // Call
                view.Data = null;

                // Assert
                AssertDataGridView(failureMechanism, true);
            }
        }

        [Test]
        public void FailureMechanism_WithData_UpdateScenarioControl()
        {
            // Setup
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                ClosingStructuresFailureMechanism failureMechanism = CreateCompleteFailureMechanism();
                view.Data = failureMechanism.CalculationsGroup;

                // Call
                view.FailureMechanism = failureMechanism;

                // Assert
                AssertDataGridView(failureMechanism, false, new[]
                {
                    new[]
                    {
                        "<selecteer>",
                        "CalculationA"
                    },
                    new[]
                    {
                        "<selecteer>",
                        "CalculationB"
                    }
                });
            }
        }

        [Test]
        public void FailureMechanism_WithoutData_ClearsScenarioControl()
        {
            // Setup
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                ClosingStructuresFailureMechanism failureMechanism = CreateCompleteFailureMechanism();
                view.Data = failureMechanism.CalculationsGroup;
                view.FailureMechanism = failureMechanism;

                // Call
                view.FailureMechanism = null;

                // Assert
                AssertDataGridView(failureMechanism, true);
            }
        }

        [Test]
        public void NotifyFailureMechanism_SectionsSetAfterFullInitialization_NewRowAddedToView()
        {
            // Setup
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                ClosingStructuresFailureMechanism failureMechanism = CreateCompleteFailureMechanism();
                view.Data = failureMechanism.CalculationsGroup;
                view.FailureMechanism = failureMechanism;

                List<FailureMechanismSection> newSections = view.FailureMechanism.Sections.ToList();
                newSections.Add(new FailureMechanismSection("SectionC", new[]
                {
                    view.FailureMechanism.Sections.Last().EndPoint,
                    new Point2D(30, 30)
                }));

                FailureMechanismTestHelper.SetSections(view.FailureMechanism, newSections);

                // Call
                failureMechanism.NotifyObservers();

                // Assert
                AssertDataGridView(failureMechanism, false, new[]
                {
                    new[]
                    {
                        "<selecteer>",
                        "CalculationA"
                    },
                    new[]
                    {
                        "<selecteer>",
                        "CalculationB"
                    },
                    new[]
                    {
                        "<selecteer>"
                    }
                });
            }
        }

        [Test]
        public void NotifyCalculation_CalculationChangedDikeProfile_CalculationMovedToOtherSectionResultOptions()
        {
            // Setup
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                ClosingStructuresFailureMechanism failureMechanism = CreateCompleteFailureMechanism();
                view.Data = failureMechanism.CalculationsGroup;
                view.FailureMechanism = failureMechanism;

                var calculationA = (StructuresCalculation<ClosingStructuresInput>) failureMechanism.CalculationsGroup.Children[0];
                var calculationB = (StructuresCalculation<ClosingStructuresInput>) failureMechanism.CalculationsGroup.Children[1];

                calculationA.InputParameters.Structure = calculationB.InputParameters.Structure;

                // Call
                calculationA.NotifyObservers();

                // Assert
                AssertDataGridView(failureMechanism, false, new[]
                {
                    new[]
                    {
                        "<selecteer>"
                    },
                    new[]
                    {
                        "<selecteer>",
                        "CalculationA",
                        "CalculationB"
                    }
                });
            }
        }

        [Test]
        public void NotifyCalculationGroup_CalculationAdded_CalculationAddedToSectionResultOptions()
        {
            // Setup
            using (ClosingStructuresScenariosView view = ShowScenariosView())
            {
                ClosingStructuresFailureMechanism failureMechanism = CreateCompleteFailureMechanism();
                view.Data = failureMechanism.CalculationsGroup;
                view.FailureMechanism = failureMechanism;

                var calculationB = (StructuresCalculation<ClosingStructuresInput>) failureMechanism.CalculationsGroup.Children[1];
                var calculationC = new StructuresCalculation<ClosingStructuresInput>
                {
                    Name = "CalculationC",
                    InputParameters =
                    {
                        Structure = calculationB.InputParameters.Structure
                    }
                };
                failureMechanism.CalculationsGroup.Children.Add(calculationC);

                calculationC.InputParameters.Structure = calculationC.InputParameters.Structure;

                // Call
                failureMechanism.CalculationsGroup.NotifyObservers();

                // Assert
                AssertDataGridView(failureMechanism, false, new[]
                {
                    new[]
                    {
                        "<selecteer>",
                        "CalculationA"
                    },
                    new[]
                    {
                        "<selecteer>",
                        "CalculationB",
                        "CalculationC"
                    }
                });
            }
        }

        private void AssertDataGridView(
            ClosingStructuresFailureMechanism failureMechanism,
            bool shouldBeCleared,
            string[][] expectedComboBoxItemTexts = null)
        {
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            int rowCount = dataGridView.RowCount;

            if (shouldBeCleared)
            {
                Assert.AreEqual(0, rowCount);
            }
            else
            {
                Assert.NotNull(expectedComboBoxItemTexts);
                var dataGridViewColumn = (DataGridViewComboBoxColumn) dataGridView.Columns[1];

                Assert.AreEqual(failureMechanism.SectionResults.Count(), rowCount);
                Assert.AreEqual(failureMechanism.Calculations.Count(), dataGridViewColumn.Items.Count);

                for (var i = 0; i < rowCount; i++)
                {
                    var cell = (DataGridViewComboBoxCell) dataGridView[1, i];
                    IEnumerable<DataGridViewComboBoxItemWrapper<ICalculation>> items = cell.Items.OfType<DataGridViewComboBoxItemWrapper<ICalculation>>();
                    Assert.AreEqual(expectedComboBoxItemTexts[i], items.Select(r => r.DisplayName));
                }
            }
        }

        private ClosingStructuresFailureMechanism CreateCompleteFailureMechanism()
        {
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var matchingPointA = new Point2D(0, 0);
            var matchingPointB = new Point2D(20, 20);
            var calculationA = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = "CalculationA",
                InputParameters =
                {
                    Structure = CreateStructure(matchingPointA)
                }
            };
            var calculationB = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = "CalculationB",
                InputParameters =
                {
                    Structure = CreateStructure(matchingPointB)
                }
            };
            var connectionPoint = new Point2D(10, 10);
            var failureMechanismSectionA = new FailureMechanismSection("sectionA", new[]
            {
                matchingPointA,
                connectionPoint
            });
            var failureMechanismSectionB = new FailureMechanismSection("sectionB", new[]
            {
                connectionPoint,
                matchingPointB
            });

            failureMechanism.CalculationsGroup.Children.Add(calculationA);
            failureMechanism.CalculationsGroup.Children.Add(calculationB);
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSectionA,
                failureMechanismSectionB
            });

            return failureMechanism;
        }

        private ClosingStructure CreateStructure(Point2D location)
        {
            return new ClosingStructure(new ClosingStructure.ConstructionProperties
            {
                Id = "1",
                Name = "<Awesome structure>",
                Location = location
            });
        }

        private ClosingStructuresScenariosView ShowScenariosView()
        {
            var scenariosView = new ClosingStructuresScenariosView();
            testForm.Controls.Add(scenariosView);
            testForm.Show();

            return scenariosView;
        }
    }
}