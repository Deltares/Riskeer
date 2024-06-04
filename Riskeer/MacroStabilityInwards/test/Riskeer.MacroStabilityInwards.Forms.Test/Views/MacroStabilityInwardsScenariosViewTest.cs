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
using Riskeer.Common.Forms.Controls;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
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
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsScenariosView(null, new MacroStabilityInwardsFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsScenariosView(new CalculationGroup(), null);

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
            using (var scenarioView = new MacroStabilityInwardsScenariosView(calculationGroup, new MacroStabilityInwardsFailureMechanism()))
            {
                // Assert
                Assert.IsInstanceOf<IView>(scenarioView);
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
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
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
            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(0),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.Items[0]).SectionConfiguration);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(1),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.Items[1]).SectionConfiguration);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(2),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.Items[2]).SectionConfiguration);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(0),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).SectionConfiguration);
        }

        [Test]
        public void Constructor_SectionConfigurationControlCorrectlyInitialized()
        {
            // Call
            ShowMacroStabilityInwardsScenariosView(new MacroStabilityInwardsFailureMechanism());

            // Assert
            ScenarioConfigurationPerFailureMechanismSectionControl sectionConfigurationControl = GetFailureMechanismSectionConfigurationControl();
            Assert.IsTrue(sectionConfigurationControl.Visible);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void MacroStabilityInwardsScenarioView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredMacroStabilityInwardsScenariosView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 13.70).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[sectionFailureProbabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.IsFalse(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[sectionFailureProbabilityColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 37.50).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 3", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("1/93", cells[failureProbabilityColumnIndex].FormattedValue);
            Assert.AreEqual("1/88", cells[sectionFailureProbabilityColumnIndex].FormattedValue);
        }

        [Test]
        public void Constructor_FailureMechanismWithSectionsAndWithVariousRelevantScenarios_TotalContributionScenariosCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredMacroStabilityInwardsScenariosView();

            // Assert
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsTrue(totalScenarioContributionLabel.Visible);
            Assert.AreEqual(ContentAlignment.MiddleLeft, totalScenarioContributionLabel.TextAlign);
            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 51,20%",
                            totalScenarioContributionLabel.Text);
        }

        [Test]
        public void Constructor_FailureMechanismWithoutSectionsAndCalculationScenarios_TotalContributionScenariosLabelCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            // Assert
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        [Test]
        public void Constructor_FailureMechanismWithSectionsAndWithoutCalculationScenarios_TotalContributionScenariosLabelCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            // Call
            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            // Assert
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        [Test]
        public void ScenariosView_ContributionValueInvalid_ShowsErrorTooltip()
        {
            // Setup
            ShowFullyConfiguredMacroStabilityInwardsScenariosView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = "test";

            // Assert
            Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);
        }

        [Test]
        [TestCase(1)]
        [TestCase(1e-6)]
        [TestCase(100)]
        [TestCase(14.3)]
        public void ScenariosView_EditValueValid_DoNotShowErrorToolTipAndEditValue(double newValue)
        {
            // Setup
            ShowFullyConfiguredMacroStabilityInwardsScenariosView();

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ShowFullyConfiguredMacroStabilityInwardsScenariosView(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            ICalculation calculation = calculationGroup.GetCalculations().First();
            calculation.Attach(calculationObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = newValue is double value ? (RoundedDouble) value : newValue;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenMacroStabilityInwardsScenariosView_WhenSelectingDifferentItemInSectionsListBox_ThenControlsUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            MacroStabilityInwardsFailureMechanismSectionConfiguration lastSectionConfiguration = failureMechanism.FailureMechanismSectionConfigurations.Last();
            lastSectionConfiguration.A = (RoundedDouble) 0.7;

            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            TextBoxTester parameterATextBox = GetParameterATextBoxTester();

            ScenarioConfigurationPerFailureMechanismSectionControl sectionConfigurationControl = GetFailureMechanismSectionConfigurationControl();
            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox(sectionConfigurationControl);

            // Precondition
            var selectedItem = ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem);
            Assert.AreSame(failureMechanism.Sections.First(), selectedItem.SectionConfiguration.Section);
            Assert.AreEqual("0,033", parameterATextBox.Text);
            Assert.AreEqual("1,05", lengthEffectNRoundedTextBox.Text);

            MacroStabilityInwardsScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                               .Select(r => r.DataBoundItem)
                                                                               .Cast<MacroStabilityInwardsScenarioRow>()
                                                                               .ToArray();

            // When
            listBox.SelectedItem = listBox.Items[listBox.Items.Count - 1];

            // Then
            Assert.AreEqual("0,700", parameterATextBox.Text);
            Assert.AreEqual("1,14", lengthEffectNRoundedTextBox.Text);

            MacroStabilityInwardsScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                         .Select(r => r.DataBoundItem)
                                                                         .Cast<MacroStabilityInwardsScenarioRow>()
                                                                         .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosView_WhenSettingNewParameterA_ThenDataGridViewUpdated()
        {
            // Given
            ShowFullyConfiguredMacroStabilityInwardsScenariosView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            MacroStabilityInwardsScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                               .Select(r => r.DataBoundItem)
                                                                               .Cast<MacroStabilityInwardsScenarioRow>()
                                                                               .ToArray();

            // When
            var textBoxTester = new TextBoxTester("parameterATextBox");
            textBoxTester.Enter("0,7");

            // Then
            MacroStabilityInwardsScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                         .Select(r => r.DataBoundItem)
                                                                         .Cast<MacroStabilityInwardsScenarioRow>()
                                                                         .ToArray();
            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosViewWithSectionConfigurationControlError_WhenSelectingDifferentItemInSectionsListBox_ThenErrorCleared()
        {
            // Setup
            ShowFullyConfiguredMacroStabilityInwardsScenariosView();

            TextBoxTester textBoxTester = GetParameterATextBoxTester();
            textBoxTester.Enter("NotADouble");

            // Precondition
            ScenarioConfigurationPerFailureMechanismSectionControl sectionConfigurationControl = GetFailureMechanismSectionConfigurationControl();
            ErrorProvider errorProvider = GetParameterAErrorProvider(sectionConfigurationControl);
            var parameterATextBox = (TextBox) textBoxTester.TheObject;
            string errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsNotEmpty(errorMessage);

            // When
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            listBox.SelectedItem = listBox.Items[listBox.Items.Count - 1];

            // Then
            errorMessage = errorProvider.GetError(parameterATextBox);
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosViewWithSections_WhenSectionsClearedAndFailureMechanismNotifiesObserver_ThenScenarioConfigurationPerFailureMechanismSectionControlUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ShowFullyConfiguredMacroStabilityInwardsScenariosView(failureMechanism);

            // Precondition
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            Assert.IsTrue(parameterATextBox.Enabled);
            Assert.IsNotEmpty(parameterATextBox.Text);

            ScenarioConfigurationPerFailureMechanismSectionControl sectionConfigurationControl = GetFailureMechanismSectionConfigurationControl();
            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox(sectionConfigurationControl);
            Assert.IsNotEmpty(lengthEffectNRoundedTextBox.Text);

            // When
            failureMechanism.ClearAllSections();
            failureMechanism.NotifyObservers();

            // Then
            Assert.IsFalse(parameterATextBox.Enabled);
            Assert.IsEmpty(parameterATextBox.Text);

            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosViewWithoutSections_WhenSectionsAddedAndFailureMechanismNotifiesObserver_ThenScenarioConfigurationPerFailureMechanismSectionControlUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            // Precondition
            var parameterATextBox = (TextBox) GetParameterATextBoxTester().TheObject;
            Assert.IsFalse(parameterATextBox.Enabled);
            Assert.IsEmpty(parameterATextBox.Text);

            ScenarioConfigurationPerFailureMechanismSectionControl sectionConfigurationControl = GetFailureMechanismSectionConfigurationControl();
            TextBox lengthEffectNRoundedTextBox = GetLengthEffectNRoundedTextBox(sectionConfigurationControl);
            Assert.IsEmpty(lengthEffectNRoundedTextBox.Text);

            // When
            failureMechanism.SetSections(new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            }, string.Empty);
            failureMechanism.NotifyObservers();

            // Then
            Assert.IsTrue(parameterATextBox.Enabled);
            Assert.IsNotEmpty(parameterATextBox.Text);

            Assert.IsNotEmpty(lengthEffectNRoundedTextBox.Text);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosView_WhenFailureMechanismNotifiesObserver_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ShowMacroStabilityInwardsScenariosView(failureMechanism);

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
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(0),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.Items[0]).SectionConfiguration);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(1),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.Items[1]).SectionConfiguration);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(2),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.Items[2]).SectionConfiguration);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(0),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).SectionConfiguration);
        }

        [Test]
        public void GivenMacroStabilityInwardsViewWithItemSelectedInSectionsListBox_WhenFailureMechanismNotifiesObserverAndSelectedSectionSame_ThenSectionsListBoxUpdatedAndSelectionSame()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

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

            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            listBox.SelectedItem = listBox.Items[1];

            // Precondition
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(1),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).SectionConfiguration);

            // When
            failureMechanism.NotifyObservers();

            // Then
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(1),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).SectionConfiguration);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosViewWithItemSelectedInSectionsListBox_WhenFailureMechanismNotifiesObserverAndSelectedSectionNotSame_ThenSectionsListBoxUpdatedAndSelectionReset()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

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

            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            listBox.SelectedItem = listBox.Items[1];

            // Precondition
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.ElementAt(1),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).SectionConfiguration);

            // When
            var failureMechanismSection4 = new FailureMechanismSection("Section 4", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(3.0, 0.0)
            });
            var failureMechanismSection5 = new FailureMechanismSection("Section 5", new[]
            {
                new Point2D(3.0, 0.0),
                new Point2D(10.0, 0.0)
            });

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection4,
                failureMechanismSection5
            });
            failureMechanism.NotifyObservers();

            // Then
            Assert.AreEqual(2, listBox.Items.Count);
            Assert.AreSame(failureMechanism.FailureMechanismSectionConfigurations.First(),
                           ((MacroStabilityInwardsScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).SectionConfiguration);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosView_WhenFailureMechanismNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ShowFullyConfiguredMacroStabilityInwardsScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            MacroStabilityInwardsScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                               .Select(r => r.DataBoundItem)
                                                                               .Cast<MacroStabilityInwardsScenarioRow>()
                                                                               .ToArray();

            // When
            failureMechanism.NotifyObservers();

            // Then
            MacroStabilityInwardsScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                         .Select(r => r.DataBoundItem)
                                                                         .Cast<MacroStabilityInwardsScenarioRow>()
                                                                         .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosView_WhenCalculationGroupNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            MacroStabilityInwardsScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                               .Select(r => r.DataBoundItem)
                                                                               .Cast<MacroStabilityInwardsScenarioRow>()
                                                                               .ToArray();

            // When
            failureMechanism.CalculationsGroup.NotifyObservers();

            // Then
            MacroStabilityInwardsScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                         .Select(r => r.DataBoundItem)
                                                                         .Cast<MacroStabilityInwardsScenarioRow>()
                                                                         .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosView_WhenCalculationNotifiesObserver_ThenViewUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            ShowMacroStabilityInwardsScenariosView(failureMechanism);
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            var refreshed = 0;
            dataGridView.Invalidated += (sender, args) => refreshed++;

            ICalculation calculation = failureMechanism.CalculationsGroup.GetCalculations().First();

            // When
            calculation.NotifyObservers();

            // Then
            Assert.AreEqual(1, refreshed);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosView_WhenCalculationInputNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ShowFullyConfiguredMacroStabilityInwardsScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            MacroStabilityInwardsScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                               .Select(r => r.DataBoundItem)
                                                                               .Cast<MacroStabilityInwardsScenarioRow>()
                                                                               .ToArray();

            MacroStabilityInwardsCalculationScenario calculation = failureMechanism.CalculationsGroup.GetCalculations()
                                                                                   .Cast<MacroStabilityInwardsCalculationScenario>()
                                                                                   .First();

            // When
            calculation.InputParameters.NotifyObservers();

            // Then
            MacroStabilityInwardsScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                         .Select(r => r.DataBoundItem)
                                                                         .Cast<MacroStabilityInwardsScenarioRow>()
                                                                         .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenMacroStabilityInwardsScenariosViewWithTotalContributionsNotValid_WhenEditingScenarioContributionToValidValue_ThenTotalContributionLabelUpdatedAndErrorNotShown()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.5011
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.75
                }
            });

            MacroStabilityInwardsScenariosView view = ShowMacroStabilityInwardsScenariosView(failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 125,11%",
                            totalScenarioContributionLabel.Text);

            ErrorProvider errorProvider = GetErrorProvider(view);
            Assert.AreEqual("De bijdragen van de maatgevende scenario's voor dit vak moeten opgeteld gelijk zijn aan 100%.",
                            errorProvider.GetError(totalScenarioContributionLabel));

            // When
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = (RoundedDouble) 25;

            // Then
            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 100,00%",
                            totalScenarioContributionLabel.Text);
            Assert.IsEmpty(errorProvider.GetError(totalScenarioContributionLabel));
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(25.01)]
        [TestCase(24.99)]
        [TestCase(50)]
        public void GivenMacroStabilityInwardsScenariosViewWithTotalContributionsValid_WhenEditingScenarioContributionsToInvalidValue_ThenTotalContributionLabelUpdatedAndErrorShown(
            double newContribution)
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.25
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.75
                }
            });

            MacroStabilityInwardsScenariosView view = ShowMacroStabilityInwardsScenariosView(failureMechanism);

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
            Assert.AreEqual($"De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan {newContribution + 75:F2}%",
                            totalScenarioContributionLabel.Text);

            Assert.AreEqual("De bijdragen van de maatgevende scenario's voor dit vak moeten opgeteld gelijk zijn aan 100%.",
                            errorProvider.GetError(totalScenarioContributionLabel));
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenMacroStabilityInwardsScenariosViewWithoutContributingScenarios_WhenMakingScenarioRelevant_ThenTotalContributionLabelUpdatedAndShown()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    IsRelevant = false
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    IsRelevant = false
                }
            });

            ShowMacroStabilityInwardsScenariosView(failureMechanism);

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
        public void GivenMacroStabilityInwardsScenariosViewWithContributingScenarios_WhenMakingContributingScenarioIrrelevant_ThenTotalContributionLabelNotVisible()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    }
                }
            });

            ShowMacroStabilityInwardsScenariosView(failureMechanism);

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
        public void GivenMacroStabilityInwardsScenariosViewWithoutScenarios_WhenAddingRelevantScenarioAndCalculationGroupNotifiesObservers_ThenTotalContributionLabelUpdatedAndShown()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            failureMechanism.CalculationsGroup.Children.Clear();

            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);

            // When
            var calculationToAdd = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = failureMechanism.SurfaceLines.First()
                }
            };

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Add(calculationToAdd);
            calculationGroup.NotifyObservers();

            // Then
            Assert.IsTrue(totalScenarioContributionLabel.Visible);

            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 100,00%",
                            totalScenarioContributionLabel.Text);
        }

        [Test]
        public void GivenMacroStabilityInwardsScenariosViewWithScenarios_WhenCalculationsGroupClearedAndCalculationGroupNotifiesObservers_ThenTotalContributionLabelNotVisible()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    }
                }
            });

            ShowMacroStabilityInwardsScenariosView(failureMechanism);

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
        public void GivenMacroStabilityInwardsScenariosViewWithScenarios_WhenScenariosClearedAndFailureMechanismNotifiesObserver_ThenTotalContributionLabelNotVisible()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    }
                }
            });

            ShowMacroStabilityInwardsScenariosView(failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsTrue(totalScenarioContributionLabel.Visible);

            // When
            calculationGroup.Children.Clear();
            failureMechanism.NotifyObservers();

            // Then
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        private void ShowFullyConfiguredMacroStabilityInwardsScenariosView()
        {
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            ShowFullyConfiguredMacroStabilityInwardsScenariosView(failureMechanism);
        }

        private void ShowFullyConfiguredMacroStabilityInwardsScenariosView(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            ConfigureFailureMechanism(failureMechanism);
            ShowMacroStabilityInwardsScenariosView(failureMechanism);
        }

        private static void ConfigureFailureMechanism(MacroStabilityInwardsFailureMechanism failureMechanism)
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
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, string.Empty);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(80.0, 0.0)
                }),
                new FailureMechanismSection("Section 2", new[]
                {
                    new Point2D(80.0, 0.0),
                    new Point2D(90.0, 0.0)
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
                    },
                    Contribution = (RoundedDouble) 0.13701
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Calculation 2",
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine2
                    },
                    IsRelevant = false
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Calculation 3",
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine2
                    },
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput(new MacroStabilityInwardsOutput.ConstructionProperties
                    {
                        FactorOfStability = 0.8
                    }),
                    Contribution = (RoundedDouble) 0.37503
                }
            });
        }

        private MacroStabilityInwardsScenariosView ShowMacroStabilityInwardsScenariosView(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            var scenarioView = new MacroStabilityInwardsScenariosView(failureMechanism.CalculationsGroup, failureMechanism);

            testForm.Controls.Add(scenarioView);
            testForm.Show();

            return scenarioView;
        }

        #region Control helpers

        private static ErrorProvider GetErrorProvider(MacroStabilityInwardsScenariosView view)
        {
            return TypeUtils.GetField<ErrorProvider>(view, "errorProvider");
        }

        private static ErrorProvider GetParameterAErrorProvider(ScenarioConfigurationPerFailureMechanismSectionControl scenarioConfigurationControl)
        {
            return TypeUtils.GetField<ErrorProvider>(scenarioConfigurationControl, "errorProvider");
        }

        private static TextBoxTester GetParameterATextBoxTester()
        {
            return new TextBoxTester("parameterATextBox");
        }

        private static TextBox GetLengthEffectNRoundedTextBox(ScenarioConfigurationPerFailureMechanismSectionControl scenarioConfigurationControl)
        {
            var tableLayoutPanel = (TableLayoutPanel) scenarioConfigurationControl.Controls["tableLayoutPanel"];
            return (TextBox) tableLayoutPanel.GetControlFromPosition(1, 1);
        }

        private static ScenarioConfigurationPerFailureMechanismSectionControl GetFailureMechanismSectionConfigurationControl()
        {
            return (ScenarioConfigurationPerFailureMechanismSectionControl) new ControlTester("scenarioConfigurationPerFailureMechanismSectionControl").TheObject;
        }

        #endregion
    }
}