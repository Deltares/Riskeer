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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsScenariosViewTest : NUnitFormTest
    {
        private const int isRelevantColumnIndex = 0;
        private const int contributionColumnIndex = 1;
        private const int nameColumnIndex = 2;
        private const int failureProbabilityColumnIndex = 3;
        private Form testForm;

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsScenariosView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (var scenarioView = new MacroStabilityInwardsScenariosView(assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(scenarioView);
                Assert.IsInstanceOf<IView>(scenarioView);
                Assert.IsNull(scenarioView.Data);
                Assert.IsNull(scenarioView.MacroStabilityInwardsFailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowMacroStabilityInwardsScenarioView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);
            Assert.AreEqual(4, dataGridView.ColumnCount);

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
            ShowMacroStabilityInwardsScenarioView();

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Assert
            Assert.AreEqual(0, listBox.Items.Count);
        }

        [Test]
        public void Data_SetToNull_DoesNotThrow()
        {
            // Setup
            MacroStabilityInwardsScenariosView macroStabilityInwardsScenarioView = ShowMacroStabilityInwardsScenarioView();

            // Call
            var testDelegate = new TestDelegate(() => macroStabilityInwardsScenarioView.Data = null);

            // Assert
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void MacroStabilityInwardsFailureMechanism_FailureMechanismWithSections_SectionsListBoxCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
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

            failureMechanism.AddSections(new[]
            {
                failureMechanismSection1,
                failureMechanismSection2,
                failureMechanismSection3
            });

            MacroStabilityInwardsScenariosView macroStabilityInwardsScenarioView = ShowMacroStabilityInwardsScenarioView();

            // Call
            macroStabilityInwardsScenarioView.MacroStabilityInwardsFailureMechanism = failureMechanism;

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
        }

        [Test]
        public void GivenScenariosViewWithFailureMechanism_WhenSectionsAddedAndFailureMechanismNotified_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var failureMechanismWithSections = new MacroStabilityInwardsFailureMechanism();
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

            MacroStabilityInwardsScenariosView macroStabilityInwardsScenarioView = ShowMacroStabilityInwardsScenarioView();
            macroStabilityInwardsScenarioView.MacroStabilityInwardsFailureMechanism = failureMechanismWithSections;

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Precondition
            Assert.AreEqual(0, listBox.Items.Count);

            failureMechanismWithSections.AddSections(new[]
            {
                failureMechanismSection1,
                failureMechanismSection2,
                failureMechanismSection3
            });

            // When
            failureMechanismWithSections.NotifyObservers();

            // Then
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
        }

        [Test]
        public void MacroStabilityInwardsScenarioView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredMacroStabilityInwardsScenarioView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(100.ToString(CultureInfo.CurrentCulture), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(100.ToString(CultureInfo.CurrentCulture), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(1), cells[failureProbabilityColumnIndex].FormattedValue);
        }

        [Test]
        public void MacroStabilityInwardsScenarioView_ContributionValueInvalid_ShowsErrorTooltip()
        {
            // Setup
            ShowFullyConfiguredMacroStabilityInwardsScenarioView();

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
            ShowFullyConfiguredMacroStabilityInwardsScenarioView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
        }

        [Test]
        [TestCase(isRelevantColumnIndex, true)]
        [TestCase(contributionColumnIndex, 30.0)]
        public void MacroStabilityInwardsScenarioView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(int cellIndex, object newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            MacroStabilityInwardsScenariosView macroStabilityInwardsCalculationView = ShowFullyConfiguredMacroStabilityInwardsScenarioView();

            var data = (CalculationGroup) macroStabilityInwardsCalculationView.Data;
            var calculation = (MacroStabilityInwardsCalculationScenario) data.Children.First();

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = newValue is double ? (RoundedDouble) (double) newValue : newValue;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GivenMacroStabilityInwardsScenarioView_WhenFailureMechanismNotifiesObserver_ThenViewUpdated()
        {
            // Given
            using (MacroStabilityInwardsScenariosView view = ShowFullyConfiguredMacroStabilityInwardsScenarioView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                MacroStabilityInwardsScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                                   .Select(r => r.DataBoundItem)
                                                                                   .Cast<MacroStabilityInwardsScenarioRow>()
                                                                                   .ToArray();

                // When
                view.MacroStabilityInwardsFailureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A = 0.01;
                view.MacroStabilityInwardsFailureMechanism.NotifyObservers();

                // Then
                MacroStabilityInwardsScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                             .Select(r => r.DataBoundItem)
                                                                             .Cast<MacroStabilityInwardsScenarioRow>()
                                                                             .ToArray();

                CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
            }
        }

        [Test]
        public void GivenMacroStabilityInwardscenarioView_WhenCalculationNotifiesObserver_ThenViewUpdated()
        {
            // Given
            using (ShowFullyConfiguredMacroStabilityInwardsScenarioView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var refreshed = 0;
                dataGridView.Invalidated += (sender, args) => refreshed++;

                DataGridViewRowCollection rows = dataGridView.Rows;
                DataGridViewRow calculationRow = rows[1];
                MacroStabilityInwardsCalculationScenario calculation = ((MacroStabilityInwardsScenarioRow) calculationRow.DataBoundItem).Calculation;

                // Precondition
                DataGridViewCellCollection cells = calculationRow.Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual(ProbabilityFormattingHelper.Format(1), cells[failureProbabilityColumnIndex].FormattedValue);

                // When
                calculation.ClearOutput();
                calculation.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshed);
                Assert.AreEqual("-", cells[failureProbabilityColumnIndex].FormattedValue);
            }
        }

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

        private MacroStabilityInwardsScenariosView ShowFullyConfiguredMacroStabilityInwardsScenarioView()
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

            failureMechanism.AddSections(new[]
            {
                new FailureMechanismSection("Section 1", new List<Point2D>
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                }),
                new FailureMechanismSection("Section 2", new List<Point2D>
                {
                    new Point2D(5.0, 0.0),
                    new Point2D(10.0, 0.0)
                })
            });

            MacroStabilityInwardsScenariosView macroStabilityInwardsScenarioView = ShowMacroStabilityInwardsScenarioView();

            macroStabilityInwardsScenarioView.Data = new CalculationGroup
            {
                Children =
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
                }
            };

            macroStabilityInwardsScenarioView.MacroStabilityInwardsFailureMechanism = failureMechanism;

            return macroStabilityInwardsScenarioView;
        }

        private MacroStabilityInwardsScenariosView ShowMacroStabilityInwardsScenarioView()
        {
            var scenarioView = new MacroStabilityInwardsScenariosView(new AssessmentSectionStub());

            testForm.Controls.Add(scenarioView);
            testForm.Show();

            return scenarioView;
        }
    }
}