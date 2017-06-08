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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingScenariosViewTest : NUnitFormTest
    {
        private const int isRelevantColumnIndex = 0;
        private const int contributionColumnIndex = 1;
        private const int nameColumnIndex = 2;
        private const int failureProbabilityPipingColumnIndex = 3;
        private const int failureProbabilityUpliftColumnIndex = 4;
        private const int failureProbabilityHeaveColumnIndex = 5;
        private const int failureProbabilitySellmeijerColumnIndex = 6;
        private Form testForm;

        public override void Setup()
        {
            base.Setup();

            testForm = new Form();
        }

        public override void TearDown()
        {
            base.TearDown();

            testForm.Dispose();
        }

        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            using (var pipingScenarioView = new PipingScenariosView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(pipingScenarioView);
                Assert.IsInstanceOf<IView>(pipingScenarioView);
                Assert.IsNull(pipingScenarioView.Data);
                Assert.IsNull(pipingScenarioView.PipingFailureMechanism);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowPipingScenarioView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);
            Assert.AreEqual(7, dataGridView.ColumnCount);

            foreach (DataGridViewComboBoxColumn column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
            {
                Assert.AreEqual("This", column.ValueMember);
                Assert.AreEqual("DisplayName", column.DisplayMember);
            }
        }

        [Test]
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Setup & Call
            ShowPipingScenarioView();

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Assert
            Assert.AreEqual(0, listBox.Items.Count);
        }

        [Test]
        public void Data_SetToNull_DoesNotThrow()
        {
            // Setup
            PipingScenariosView pipingScenarioView = ShowPipingScenarioView();

            // Call
            var testDelegate = new TestDelegate(() => pipingScenarioView.Data = null);

            // Assert
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void PipingFailureMechanism_PipingFailureMechanismWithSections_SectionsListBoxCorrectlyInitialized()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new List<Point2D>
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            pipingFailureMechanism.AddSection(failureMechanismSection1);
            pipingFailureMechanism.AddSection(failureMechanismSection2);
            pipingFailureMechanism.AddSection(failureMechanismSection3);

            PipingScenariosView pipingScenarioView = ShowPipingScenarioView();

            // Call
            pipingScenarioView.PipingFailureMechanism = pipingFailureMechanism;

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
        }

        [Test]
        public void GivenPipingScenariosViewWithPipingFailureMechanism_WhenSectionsAddedAndPipingFailureMechanismNotified_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var pipingFailureMechanismWithSections = new PipingFailureMechanism();
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new List<Point2D>
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            PipingScenariosView pipingScenarioView = ShowPipingScenarioView();
            pipingScenarioView.PipingFailureMechanism = pipingFailureMechanismWithSections;

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Precondition
            Assert.AreEqual(0, listBox.Items.Count);

            pipingFailureMechanismWithSections.AddSection(failureMechanismSection1);
            pipingFailureMechanismWithSections.AddSection(failureMechanismSection2);
            pipingFailureMechanismWithSections.AddSection(failureMechanismSection3);

            // When
            pipingFailureMechanismWithSections.NotifyObservers();

            // Then
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
        }

        [Test]
        public void PipingScenarioView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredPipingScenarioView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(7, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(100.ToString(CultureInfo.CurrentCulture), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityUpliftColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityHeaveColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilitySellmeijerColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(7, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(100.ToString(CultureInfo.CurrentCulture), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(1.5e-3), cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(double.NaN), cells[failureProbabilityUpliftColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.0005), cells[failureProbabilityHeaveColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(1.5e-3), cells[failureProbabilitySellmeijerColumnIndex].FormattedValue);
        }

        [Test]
        public void PipingScenarioView_ContributionValueInvalid_ShowsErrorTooltip()
        {
            // Setup
            ShowFullyConfiguredPipingScenarioView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = "test";

            // Assert
            Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);
        }

        [Test]
        [TestCase(1)]
        [TestCase(1e-6)]
        [TestCase(1e+6)]
        [TestCase(14.3)]
        public void FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(double newValue)
        {
            // Setup
            ShowFullyConfiguredPipingScenarioView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
        }

        [TestCase(isRelevantColumnIndex, true)]
        [TestCase(contributionColumnIndex, 30.0)]
        public void PipingScenarioView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(int cellIndex, object newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var pipingCalculationObserver = mocks.StrictMock<IObserver>();
            var pipingCalculationInputObserver = mocks.StrictMock<IObserver>();
            pipingCalculationObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            PipingScenariosView pipingCalculationView = ShowFullyConfiguredPipingScenarioView();

            var data = (CalculationGroup) pipingCalculationView.Data;
            var pipingCalculation = (PipingCalculationScenario) data.Children.First();

            pipingCalculation.Attach(pipingCalculationObserver);
            pipingCalculation.InputParameters.Attach(pipingCalculationInputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = newValue is double ? (RoundedDouble) (double) newValue : newValue;

            // Assert
            mocks.VerifyAll();
        }

        private PipingScenariosView ShowFullyConfiguredPipingScenarioView()
        {
            var surfaceLine1 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 2",
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            PipingScenariosView pipingScenarioView = ShowPipingScenarioView();

            pipingScenarioView.Data = new CalculationGroup("Group", true)
            {
                Children =
                {
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 1",
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine1,
                            DampingFactorExit =
                            {
                                Mean = (RoundedDouble) 1.1111
                            },
                            PhreaticLevelExit =
                            {
                                Mean = (RoundedDouble) 2.2222
                            },
                            EntryPointL = (RoundedDouble) 3.3333,
                            ExitPointL = (RoundedDouble) 4.4444
                        }
                    },
                    new PipingCalculationScenario(new GeneralPipingInput())
                    {
                        Name = "Calculation 2",
                        InputParameters =
                        {
                            SurfaceLine = surfaceLine2,
                            DampingFactorExit =
                            {
                                Mean = (RoundedDouble) 5.5555
                            },
                            PhreaticLevelExit =
                            {
                                Mean = (RoundedDouble) 6.6666
                            },
                            EntryPointL = (RoundedDouble) 7.7777,
                            ExitPointL = (RoundedDouble) 8.8888
                        },
                        SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                            double.NaN,
                            double.NaN,
                            double.NaN,
                            double.NaN,
                            double.NaN,
                            0.0005,
                            double.NaN,
                            double.NaN,
                            1.5e-3,
                            double.NaN,
                            double.NaN,
                            1.5e-3,
                            double.NaN,
                            double.NaN)
                    }
                }
            };

            pipingScenarioView.PipingFailureMechanism = pipingFailureMechanism;

            return pipingScenarioView;
        }

        private PipingScenariosView ShowPipingScenarioView()
        {
            var pipingScenarioView = new PipingScenariosView();

            testForm.Controls.Add(pipingScenarioView);
            testForm.Show();

            return pipingScenarioView;
        }
    }
}