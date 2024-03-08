// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

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
            void Call() => new TestScenariosView(null, new TestCalculatableFailureMechanism());

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
            using (var view = new TestScenariosView(calculationGroup, new TestCalculatableFailureMechanism()))
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
            var failureMechanism = new TestCalculatableFailureMechanism();
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
            ShowFullyConfiguredScenariosView(new CalculationGroup(), new TestCalculatableFailureMechanism());

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
        [SetCulture("nl-NL")]
        public void Constructor_WithVariousCalculationConfigurationsInSection_TotalContributionScenariosCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new TestCalculatableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.AddRange(new[]
            {
                new TestCalculationScenario
                {
                    Name = "Calculation 1",
                    Contribution = (RoundedDouble) 0.13701
                },
                new TestCalculationScenario
                {
                    Name = "Calculation 2",
                    Contribution = (RoundedDouble) 1,
                    IsRelevant = false
                },
                new TestCalculationScenario
                {
                    Name = "Calculation 3",
                    Contribution = (RoundedDouble) 0.37503
                }
            });

            // Call
            ShowScenariosView(calculationGroup, failureMechanism);

            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;

            // Assert
            Assert.IsTrue(totalScenarioContributionLabel.Visible);
            Assert.AreEqual(ContentAlignment.MiddleLeft, totalScenarioContributionLabel.TextAlign);
            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 51,20%",
                            totalScenarioContributionLabel.Text);
        }

        [Test]
        public void Constructor_WithoutFailureMechanismSectionsAndCalculationScenarios_TotalContributionScenariosLabelCorrectlyInitialized()
        {
            // Call
            ShowScenariosView(new CalculationGroup(), new TestCalculatableFailureMechanism());

            // Assert
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        [Test]
        public void Constructor_WithFailureMechanismSectionsAndWithoutRelevantCalculationScenarios_TotalContributionScenariosLabelCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new TestCalculatableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            // Call
            ShowScenariosView(new CalculationGroup(), failureMechanism);

            // Assert
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        [Test]
        public void ScenariosView_ContributionValueInvalid_ShowsErrorTooltip()
        {
            // Setup
            ShowFullyConfiguredScenariosView(new CalculationGroup(), new TestCalculatableFailureMechanism());

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
            ShowFullyConfiguredScenariosView(new CalculationGroup(), new TestCalculatableFailureMechanism());

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
            ShowFullyConfiguredScenariosView(calculationGroup, new TestCalculatableFailureMechanism());

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
            var failureMechanism = new TestCalculatableFailureMechanism();
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
            var failureMechanism = new TestCalculatableFailureMechanism();
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
            var failureMechanism = new TestCalculatableFailureMechanism();
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
            ShowFullyConfiguredScenariosView(calculationGroup, new TestCalculatableFailureMechanism());

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
            ShowFullyConfiguredScenariosView(calculationGroup, new TestCalculatableFailureMechanism());
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
            ShowFullyConfiguredScenariosView(calculationGroup, new TestCalculatableFailureMechanism());

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

        [Test]
        [SetCulture("nl-NL")]
        public void GivenScenariosViewWithTotalContributionsNotValid_WhenEditingScenarioContributionToValidValue_ThenTotalContributionLabelUpdatedAndErrorNotShown()
        {
            // Given
            var failureMechanism = new TestCalculatableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(new TestCalculationScenario
            {
                Contribution = (RoundedDouble) 1.0001
            });

            TestScenariosView view = ShowScenariosView(calculationGroup, failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 100,01%",
                            totalScenarioContributionLabel.Text);

            ErrorProvider errorProvider = GetErrorProvider(view);
            Assert.AreEqual("De bijdragen van de maatgevende scenario's voor dit vak moeten opgeteld gelijk zijn aan 100%.",
                            errorProvider.GetError(totalScenarioContributionLabel));

            // When
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = (RoundedDouble) 100;

            // Then
            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 100,00%",
                            totalScenarioContributionLabel.Text);
            Assert.IsEmpty(errorProvider.GetError(totalScenarioContributionLabel));
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(150)]
        [TestCase(100.01)]
        [TestCase(99.99)]
        [TestCase(50)]
        public void GivenScenariosViewWithTotalContributionsValid_WhenEditingScenarioContributionsToInvalidValue_ThenTotalContributionLabelUpdatedAndErrorShown(
            double newContribution)
        {
            // Given
            var failureMechanism = new TestCalculatableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(new TestCalculationScenario());

            TestScenariosView view = ShowScenariosView(calculationGroup, failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 100,00%",
                            totalScenarioContributionLabel.Text);

            ErrorProvider errorProvider = GetErrorProvider(view);
            Assert.IsEmpty(errorProvider.GetError(totalScenarioContributionLabel));

            // When
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = (RoundedDouble) newContribution;

            // Then
            Assert.AreEqual($"De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan {newContribution:F2}%",
                            totalScenarioContributionLabel.Text);

            Assert.AreEqual("De bijdragen van de maatgevende scenario's voor dit vak moeten opgeteld gelijk zijn aan 100%.",
                            errorProvider.GetError(totalScenarioContributionLabel));
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenScenariosViewWithoutContributingScenarios_WhenMakingScenarioRelevant_ThenTotalContributionLabelUpdatedAndShown()
        {
            // Given
            var failureMechanism = new TestCalculatableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(new TestCalculationScenario
            {
                IsRelevant = false
            });

            ShowScenariosView(calculationGroup, failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);

            // When
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.Rows[0].Cells[isRelevantColumnIndex].Value = true;

            // Then
            Assert.IsTrue(totalScenarioContributionLabel.Visible);

            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 100,00%",
                            totalScenarioContributionLabel.Text);
        }

        [Test]
        public void GivenScenariosViewWithContributingScenarios_WhenMakingContributingScenarioIrrelevant_ThenTotalContributionLabelNotVisible()
        {
            // Given
            var failureMechanism = new TestCalculatableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.AddRange(new[]
            {
                new TestCalculationScenario
                {
                    IsRelevant = true
                }
            });

            ShowScenariosView(calculationGroup, failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsTrue(totalScenarioContributionLabel.Visible);

            // When
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.Rows[0].Cells[isRelevantColumnIndex].Value = false;

            // Then
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenScenariosViewWithoutScenarios_WhenAddingRelevantScenarioAndCalculationGroupNotifiesObservers_ThenTotalContributionLabelUpdatedAndShown()
        {
            // Given
            var failureMechanism = new TestCalculatableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            var calculationGroup = new CalculationGroup();
            ShowScenariosView(calculationGroup, failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);

            // When
            calculationGroup.Children.Add(new TestCalculationScenario());
            calculationGroup.NotifyObservers();

            // Then
            Assert.IsTrue(totalScenarioContributionLabel.Visible);

            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 100,00%",
                            totalScenarioContributionLabel.Text);
        }

        [Test]
        public void GivenScenariosViewWithScenarios_WhenCalculationsGroupClearedAndCalculationGroupNotifiesObservers_ThenTotalContributionLabelNotVisible()
        {
            // Given
            var failureMechanism = new TestCalculatableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(new TestCalculationScenario());
            ShowScenariosView(calculationGroup, failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsTrue(totalScenarioContributionLabel.Visible);

            // When
            calculationGroup.Children.Clear();
            calculationGroup.NotifyObservers();

            // Then
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        [Test]
        public void GivenScenariosViewWithScenarios_WhenScenariosClearedAndFailureMechanismNotifiesObserver_ThenTotalContributionLabelNotVisible()
        {
            // Given
            var failureMechanism = new TestCalculatableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(new TestCalculationScenario());
            ShowScenariosView(calculationGroup, failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsTrue(totalScenarioContributionLabel.Visible);

            // When
            calculationGroup.Children.Clear();
            failureMechanism.NotifyObservers();

            // Then
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        private void ShowFullyConfiguredScenariosView(CalculationGroup calculationGroup, TestCalculatableFailureMechanism failureMechanism)
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

        private TestScenariosView ShowScenariosView(CalculationGroup calculationGroup, TestCalculatableFailureMechanism failureMechanism)
        {
            var scenariosView = new TestScenariosView(calculationGroup, failureMechanism);

            testForm.Controls.Add(scenariosView);
            testForm.Show();

            return scenariosView;
        }

        private static ErrorProvider GetErrorProvider(TestScenariosView view)
        {
            return TypeUtils.GetField<ErrorProvider>(view, "errorProvider");
        }

        private class TestScenariosView : ScenariosView<TestCalculationScenario, TestCalculationInput, TestScenarioRow, TestCalculatableFailureMechanism>
        {
            public TestScenariosView(CalculationGroup calculationGroup, TestCalculatableFailureMechanism failureMechanism)
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