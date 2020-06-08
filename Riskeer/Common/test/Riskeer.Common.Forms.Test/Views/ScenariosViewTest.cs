// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class ScenariosViewTest
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
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestScenariosView(null, new TestFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestScenariosView(new CalculationGroup(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            // Call
            using (var view = new TestScenariosView(calculationGroup, new TestFailureMechanism()))
            {
                // Assert
                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<UserControl>(view);
                Assert.AreSame(calculationGroup, view.Data);
            }
        }

        [Test]
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new[]
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection1,
                failureMechanismSection2,
                failureMechanismSection3
            });

            // Call
            ShowScenariosView(new CalculationGroup(), failureMechanism);

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            Assert.AreEqual(nameof(FailureMechanismSection.Name), listBox.DisplayMember);
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            Assert.AreSame(failureMechanismSection1, listBox.SelectedItem);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredScenariosView(new CalculationGroup(), new TestFailureMechanism());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            Assert.IsFalse(dataGridView.AutoGenerateColumns);
            Assert.AreEqual(4, dataGridView.ColumnCount);
            Assert.AreEqual("In oordeel", dataGridView.Columns[isRelevantColumnIndex].HeaderText);
            Assert.AreEqual("Bijdrage aan\r\nscenario\r\n[%]", dataGridView.Columns[contributionColumnIndex].HeaderText);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans\r\n[1/jaar]", dataGridView.Columns[failureProbabilityColumnIndex].HeaderText);

            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(1), cells[failureProbabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.IsFalse(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(1), cells[failureProbabilityColumnIndex].FormattedValue);
        }

        [Test]
        public void ScenariosView_ContributionValueInvalid_ShowsErrorTooltip()
        {
            // Setup
            ShowFullyConfiguredScenariosView(new CalculationGroup(), new TestFailureMechanism());

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
        public void ScenariosView_EditValueValid_DoNotShowErrorToolTipAndEditValue(double newValue)
        {
            // Setup
            ShowFullyConfiguredScenariosView(new CalculationGroup(), new TestFailureMechanism());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
        }

        [Test]
        [TestCase(isRelevantColumnIndex, true)]
        [TestCase(contributionColumnIndex, 30.0)]
        public void ScenariosView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(int cellIndex, object newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            ShowFullyConfiguredScenariosView(calculationGroup, new TestFailureMechanism());

            ICalculation calculation = calculationGroup.GetCalculations().First();
            calculation.Attach(calculationObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = newValue is double value ? (RoundedDouble) value : newValue;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenariosView_WhenSelectingDifferentItemInSectionsListBox_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            ShowFullyConfiguredScenariosView(new CalculationGroup(), failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Precondition
            Assert.AreSame(failureMechanism.Sections.First(), listBox.SelectedItem);

            TestScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                              .Select(r => r.DataBoundItem)
                                                              .Cast<TestScenarioRow>()
                                                              .ToArray();

            // When
            listBox.SelectedItem = failureMechanism.Sections.Last();

            // Then
            TestScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                        .Select(r => r.DataBoundItem)
                                                        .Cast<TestScenarioRow>()
                                                        .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenScenariosView_WhenFailureMechanismNotifiesObserver_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            ShowScenariosView(new CalculationGroup(), failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Precondition
            CollectionAssert.IsEmpty(listBox.Items);

            // When
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new[]
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection1,
                failureMechanismSection2,
                failureMechanismSection3
            });
            failureMechanism.NotifyObservers();

            // Then
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            Assert.AreSame(failureMechanismSection1, listBox.SelectedItem);
        }

        [Test]
        public void GivenScenariosView_WhenFailureMechanismNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            ShowFullyConfiguredScenariosView(new CalculationGroup(), failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            TestScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                              .Select(r => r.DataBoundItem)
                                                              .Cast<TestScenarioRow>()
                                                              .ToArray();

            // When
            failureMechanism.NotifyObservers();

            // Then
            TestScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                        .Select(r => r.DataBoundItem)
                                                        .Cast<TestScenarioRow>()
                                                        .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenScenariosView_WhenCalculationGroupNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var calculationGroup = new CalculationGroup();
            ShowFullyConfiguredScenariosView(calculationGroup, new TestFailureMechanism());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            TestScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                              .Select(r => r.DataBoundItem)
                                                              .Cast<TestScenarioRow>()
                                                              .ToArray();

            // When
            calculationGroup.NotifyObservers();

            // Then
            TestScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                        .Select(r => r.DataBoundItem)
                                                        .Cast<TestScenarioRow>()
                                                        .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenScenariosView_WhenCalculationNotifiesObserver_ThenViewUpdated()
        {
            // Given
            var calculationGroup = new CalculationGroup();
            ShowFullyConfiguredScenariosView(calculationGroup, new TestFailureMechanism());
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            var refreshed = 0;
            dataGridView.Invalidated += (sender, args) => refreshed++;

            var scenarioRows = new List<TestScenarioRow>();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                scenarioRows.Add((TestScenarioRow) row.DataBoundItem);
            }

            // Precondition
            Assert.IsTrue(scenarioRows.All(row => !row.Updated));

            ICalculation calculation = calculationGroup.GetCalculations().First();

            // When
            calculation.NotifyObservers();

            // Then
            Assert.AreEqual(1, refreshed);
            Assert.IsTrue(scenarioRows.All(row => row.Updated));
        }

        [Test]
        public void GivenScenariosView_WhenCalculationInputNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var calculationGroup = new CalculationGroup();
            ShowFullyConfiguredScenariosView(calculationGroup, new TestFailureMechanism());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            TestScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                              .Select(r => r.DataBoundItem)
                                                              .Cast<TestScenarioRow>()
                                                              .ToArray();

            TestCalculationScenario calculation = calculationGroup.GetCalculations().Cast<TestCalculationScenario>().First();

            // When
            calculation.InputParameters.NotifyObservers();

            // Then
            TestScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                        .Select(r => r.DataBoundItem)
                                                        .Cast<TestScenarioRow>()
                                                        .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        private void ShowFullyConfiguredScenariosView(CalculationGroup calculationGroup, TestFailureMechanism failureMechanism)
        {
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

            calculationGroup.Children.AddRange(new[]
            {
                new TestCalculationScenario
                {
                    Name = "Calculation 1"
                },
                new TestCalculationScenario
                {
                    Name = "Calculation 2",
                    IsRelevant = false
                }
            });

            ShowScenariosView(calculationGroup, failureMechanism);
        }

        private void ShowScenariosView(CalculationGroup calculationGroup, TestFailureMechanism failureMechanism)
        {
            var scenariosView = new TestScenariosView(calculationGroup, failureMechanism);

            testForm.Controls.Add(scenariosView);
            testForm.Show();
        }

        private class TestScenariosView : ScenariosView<TestCalculationScenario, TestCalculationInput, TestScenarioRow, TestFailureMechanism>
        {
            public TestScenariosView(CalculationGroup calculationGroup, TestFailureMechanism failureMechanism)
                : base(calculationGroup, failureMechanism) {}
            
            protected override TestCalculationInput GetCalculationInput(TestCalculationScenario calculationScenario)
            {
                return calculationScenario.InputParameters;
            }

            protected override IEnumerable<TestScenarioRow> GetScenarioRows(FailureMechanismSection failureMechanismSection)
            {
                return CalculationGroup.Children.OfType<TestCalculationScenario>()
                                       .Select(calculationScenario => new TestScenarioRow(calculationScenario))
                                       .ToList();
            }
        }

        private class TestScenarioRow : ScenarioRow<TestCalculationScenario>
        {
            public TestScenarioRow(TestCalculationScenario calculationScenario)
                : base(calculationScenario) {}

            public override double FailureProbability => 1;

            public bool Updated { get; private set; }

            public override void Update()
            {
                Updated = true;
            }
        }
    }
}