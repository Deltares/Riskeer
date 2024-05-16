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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Enums;
using Core.Common.Util.Extensions;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingScenariosViewTest
    {
        private const int isRelevantColumnIndex = 0;
        private const int contributionColumnIndex = 1;
        private const int nameColumnIndex = 2;
        private const int failureProbabilityUpliftColumnIndex = 3;
        private const int failureProbabilityHeaveColumnIndex = 4;
        private const int failureProbabilitySellmeijerColumnIndex = 5;
        private const int failureProbabilityPipingColumnIndex = 6;
        private const int sectionFailureProbabilityPipingColumnIndex = 7;
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
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new PipingScenariosView(null, new PipingFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new PipingScenariosView(new CalculationGroup(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingScenariosView(new CalculationGroup(), new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            // Call
            using (var pipingScenarioView = new PipingScenariosView(calculationGroup, new PipingFailureMechanism(), assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(pipingScenarioView);
                Assert.IsInstanceOf<IView>(pipingScenarioView);
                Assert.AreSame(calculationGroup, pipingScenarioView.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
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
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.Items[0]).Section);
            Assert.AreSame(failureMechanismSection2, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.Items[1]).Section);
            Assert.AreSame(failureMechanismSection3, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.Items[2]).Section);
            Assert.AreSame(failureMechanismSection1, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).Section);
        }

        [Test]
        public void Constructor_ComboBoxCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var comboBox = (ComboBox) new ComboBoxTester("selectConfigurationTypeComboBox").TheObject;
            Assert.AreEqual(nameof(EnumDisplayWrapper<PipingScenarioConfigurationType>.DisplayName), comboBox.DisplayMember);
            Assert.AreEqual(nameof(EnumDisplayWrapper<PipingScenarioConfigurationType>.Value), comboBox.ValueMember);
            Assert.IsInstanceOf<EnumDisplayWrapper<PipingScenarioConfigurationType>>(comboBox.SelectedItem);

            var configurationTypes = (EnumDisplayWrapper<PipingScenarioConfigurationType>[]) comboBox.DataSource;
            Assert.AreEqual(3, configurationTypes.Length);
            Assert.AreEqual(PipingScenarioConfigurationType.SemiProbabilistic, configurationTypes[0].Value);
            Assert.AreEqual(PipingScenarioConfigurationType.Probabilistic, configurationTypes[1].Value);
            Assert.AreEqual(PipingScenarioConfigurationType.PerFailureMechanismSection, configurationTypes[2].Value);
            Assert.AreEqual(failureMechanism.ScenarioConfigurationType,
                            ((EnumDisplayWrapper<PipingScenarioConfigurationType>) comboBox.SelectedItem).Value);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic, false)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic, false)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection, true)]
        public void Constructor_RadioButtonsCorrectlyInitialized(PipingScenarioConfigurationType scenarioConfigurationType, bool radioButtonsShouldBeVisible)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };

            // Call
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var radioButtonsPanel = (Panel) new PanelTester("radioButtonsPanel").TheObject;
            Assert.AreEqual(radioButtonsShouldBeVisible, radioButtonsPanel.Visible);

            var radioButtonSemiProbabilistic = (RadioButton) new RadioButtonTester("radioButtonSemiProbabilistic").TheObject;
            Assert.AreEqual("Semi-probabilistisch", radioButtonSemiProbabilistic.Text);
            Assert.IsTrue(radioButtonSemiProbabilistic.Checked);
            var radioButtonProbabilistic = (RadioButton) new RadioButtonTester("radioButtonProbabilistic").TheObject;
            Assert.AreEqual("Probabilistisch", radioButtonProbabilistic.Text);
            Assert.IsFalse(radioButtonProbabilistic.Checked);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic, true)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic, false)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection, true)]
        public void Constructor_LengthEffectControlsCorrectlyInitialized(PipingScenarioConfigurationType scenarioConfigurationType, bool controlsShouldBeVisible)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };

            // Call
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var tableLayoutPanel = (TableLayoutPanel) new ControlTester("lengthEffectTableLayoutPanel").TheObject;
            Assert.AreEqual(controlsShouldBeVisible, tableLayoutPanel.Visible);

            var lengthEffectALabel = (Label) new LabelTester("lengthEffectALabel").TheObject;
            Assert.AreEqual("Lengte-effect parameter a (-)", lengthEffectALabel.Text);

            var lengthEffectNRoundedLabel = (Label) new LabelTester("lengthEffectNRoundedLabel").TheObject;
            Assert.AreEqual("Lengte-effect parameter Nvak* (-)", lengthEffectNRoundedLabel.Text);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic, false)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic, false)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection, true)]
        public void Constructor_WarningIconAndTooltipCorrectlyInitialized(PipingScenarioConfigurationType scenarioConfigurationType, bool warningIconShouldBeVisible)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };

            // Call
            PipingScenariosView view = ShowPipingScenariosView(failureMechanism);

            // Assert
            var warningIcon = (PictureBox) new ControlTester("warningIcon").TheObject;
            TestHelper.AssertImagesAreEqual(CoreGuiResources.warning.ToBitmap(), warningIcon.BackgroundImage);
            Assert.AreEqual(ImageLayout.Center, warningIcon.BackgroundImageLayout);
            Assert.AreEqual(warningIconShouldBeVisible, warningIcon.Visible);

            var toolTip = TypeUtils.GetField<ToolTip>(view, "toolTip");
            string expectedToolTipMessage = $"In het geval van 'Per vak instelbaar' is een onderbouwing nodig,{Environment.NewLine}" +
                                            $"die aantoont dat de gekozen combinatie van probabilistisch{Environment.NewLine}" +
                                            $"en semi-probabilistisch getoetste vakken niet leidt tot{Environment.NewLine}" +
                                            "een te onveilig rekenresultaat op het trajectniveau.";
            Assert.AreEqual(expectedToolTipMessage,
                            toolTip.GetToolTip(warningIcon));
            Assert.AreEqual(5000, toolTip.AutoPopDelay);
            Assert.AreEqual(100, toolTip.InitialDelay);
            Assert.AreEqual(100, toolTip.ReshowDelay);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic, true)]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic, true)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic, false)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic, false)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection, PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic, true)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection, PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic, false)]
        public void Constructor_FailureMechanismWithSections_DataGridViewCorrectlyInitialized(PipingScenarioConfigurationType scenarioConfigurationType,
                                                                                              PipingScenarioConfigurationPerFailureMechanismSectionType scenarioConfigurationPerFailureMechanismSectionType,
                                                                                              bool semiProbabilisticColumnsShouldBeVisible)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };
            ConfigureFailureMechanism(failureMechanism);
            failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.ForEachElementDo(sc => sc.ScenarioConfigurationType = scenarioConfigurationPerFailureMechanismSectionType);

            // Call
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(8, dataGridView.ColumnCount);
            Assert.AreEqual("In oordeel", dataGridView.Columns[isRelevantColumnIndex].HeaderText);
            Assert.AreEqual("Bijdrage aan\r\nscenario\r\n[%]", dataGridView.Columns[contributionColumnIndex].HeaderText);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nopbarsten\r\n[1/jaar]", dataGridView.Columns[failureProbabilityUpliftColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nheave\r\n[1/jaar]", dataGridView.Columns[failureProbabilityHeaveColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nterugschrijdende erosie\r\n[1/jaar]", dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans per doorsnede\r\n[1/jaar]", dataGridView.Columns[failureProbabilityPipingColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans per vak\r\n[1/jaar]", dataGridView.Columns[sectionFailureProbabilityPipingColumnIndex].HeaderText);

            Assert.AreEqual(semiProbabilisticColumnsShouldBeVisible, dataGridView.Columns[failureProbabilityUpliftColumnIndex].Visible);
            Assert.AreEqual(semiProbabilisticColumnsShouldBeVisible, dataGridView.Columns[failureProbabilityHeaveColumnIndex].Visible);
            Assert.AreEqual(semiProbabilisticColumnsShouldBeVisible, dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].Visible);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic, true)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic, false)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection, true)]
        public void Constructor_FailureMechanismWithoutSections_DataGridViewCorrectlyInitialized(PipingScenarioConfigurationType scenarioConfigurationType,
                                                                                                 bool semiProbabilisticColumnsShouldBeVisible)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };

            // Call
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(8, dataGridView.ColumnCount);
            Assert.AreEqual("In oordeel", dataGridView.Columns[isRelevantColumnIndex].HeaderText);
            Assert.AreEqual("Bijdrage aan\r\nscenario\r\n[%]", dataGridView.Columns[contributionColumnIndex].HeaderText);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nopbarsten\r\n[1/jaar]", dataGridView.Columns[failureProbabilityUpliftColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nheave\r\n[1/jaar]", dataGridView.Columns[failureProbabilityHeaveColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nterugschrijdende erosie\r\n[1/jaar]", dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans per doorsnede\r\n[1/jaar]", dataGridView.Columns[failureProbabilityPipingColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans per vak\r\n[1/jaar]", dataGridView.Columns[sectionFailureProbabilityPipingColumnIndex].HeaderText);

            Assert.AreEqual(semiProbabilisticColumnsShouldBeVisible, dataGridView.Columns[failureProbabilityUpliftColumnIndex].Visible);
            Assert.AreEqual(semiProbabilisticColumnsShouldBeVisible, dataGridView.Columns[failureProbabilityHeaveColumnIndex].Visible);
            Assert.AreEqual(semiProbabilisticColumnsShouldBeVisible, dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].Visible);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection, PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection, PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic)]
        public void Constructor_FailureMechanismWithSectionsAndWithVariousRelevantScenarios_TotalContributionScenariosCorrectlyInitialized(PipingScenarioConfigurationType scenarioConfigurationType,
                                                                                                                                           PipingScenarioConfigurationPerFailureMechanismSectionType scenarioConfigurationPerFailureMechanismSectionType)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };
            ConfigureFailureMechanism(failureMechanism);
            failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.ForEachElementDo(sc => sc.ScenarioConfigurationType = scenarioConfigurationPerFailureMechanismSectionType);

            // Call
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsTrue(totalScenarioContributionLabel.Visible);
            Assert.AreEqual(ContentAlignment.MiddleLeft, totalScenarioContributionLabel.TextAlign);
            Assert.AreEqual("De som van de bijdragen van de maatgevende scenario's voor dit vak is gelijk aan 51,20%",
                            totalScenarioContributionLabel.Text);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection)]
        public void Constructor_FailureMechanismWithoutSectionsAndCalculationScenarios_TotalContributionScenariosLabelCorrectlyInitialized(
            PipingScenarioConfigurationType scenarioConfigurationType)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };

            // Call
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic)]
        [TestCase(PipingScenarioConfigurationType.PerFailureMechanismSection)]
        public void Constructor_FailureMechanismWithSectionsAndWithoutCalculationScenarios_TotalContributionScenariosLabelCorrectlyInitialized(
            PipingScenarioConfigurationType scenarioConfigurationType)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            });

            // Call
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        [Test]
        public void PipingScenarioView_SemiProbabilisticCalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredPipingScenariosView(new PipingFailureMechanism());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(8, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 13.701).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityUpliftColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityHeaveColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilitySellmeijerColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[sectionFailureProbabilityPipingColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(8, cells.Count);
            Assert.IsFalse(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityUpliftColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityHeaveColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilitySellmeijerColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[sectionFailureProbabilityPipingColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(8, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 37.503).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 3", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(2.425418e-4), cells[failureProbabilityUpliftColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.038461838), cells[failureProbabilityHeaveColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.027777778), cells[failureProbabilitySellmeijerColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(2.425418e-4), cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(2.44140625e-4), cells[sectionFailureProbabilityPipingColumnIndex].FormattedValue);
        }

        [Test]
        public void PipingScenarioView_ProbabilisticCalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.Probabilistic
            };

            // Call
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(8, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 13.701).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 4", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(null, cells[failureProbabilityUpliftColumnIndex].Value);
            Assert.AreEqual(null, cells[failureProbabilityHeaveColumnIndex].Value);
            Assert.AreEqual(null, cells[failureProbabilitySellmeijerColumnIndex].Value);
            Assert.AreEqual("-", cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[sectionFailureProbabilityPipingColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(8, cells.Count);
            Assert.IsFalse(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 5", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(null, cells[failureProbabilityUpliftColumnIndex].Value);
            Assert.AreEqual(null, cells[failureProbabilityHeaveColumnIndex].Value);
            Assert.AreEqual(null, cells[failureProbabilitySellmeijerColumnIndex].Value);
            Assert.AreEqual("-", cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[sectionFailureProbabilityPipingColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(8, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 37.503).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 6", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(null, cells[failureProbabilityUpliftColumnIndex].Value);
            Assert.AreEqual(null, cells[failureProbabilityHeaveColumnIndex].Value);
            Assert.AreEqual(null, cells[failureProbabilitySellmeijerColumnIndex].Value);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.25), cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.25), cells[sectionFailureProbabilityPipingColumnIndex].FormattedValue);
        }

        [Test]
        public void GivenPipingScenarioView_WhenSelectingItemInComboBox_ThenDataSetAndObserversNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            ShowPipingScenariosView(failureMechanism);

            failureMechanism.Attach(observer);

            // Precondition
            Assert.AreEqual(PipingScenarioConfigurationType.SemiProbabilistic, failureMechanism.ScenarioConfigurationType);

            // When
            var newValue = new Random(21).NextEnumValue<PipingScenarioConfigurationType>();
            var comboBox = (ComboBox) new ComboBoxTester("selectConfigurationTypeComboBox").TheObject;
            comboBox.SelectedValue = newValue;

            // Then
            Assert.AreEqual(newValue, failureMechanism.ScenarioConfigurationType);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic, PipingScenarioConfigurationType.Probabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic, PipingScenarioConfigurationType.SemiProbabilistic)]
        public void GivenPipingScenarioView_WhenSelectingItemInComboBox_ThenDataGridViewUpdated(PipingScenarioConfigurationType initialScenarioConfigurationType,
                                                                                                PipingScenarioConfigurationType newScenarioConfigurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = initialScenarioConfigurationType
            };
            ShowPipingScenariosView(failureMechanism);

            var comboBox = (ComboBox) new ComboBoxTester("selectConfigurationTypeComboBox").TheObject;
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Precondition
            Type initialRowType = initialScenarioConfigurationType == PipingScenarioConfigurationType.SemiProbabilistic
                                      ? typeof(SemiProbabilisticPipingScenarioRow)
                                      : typeof(ProbabilisticPipingScenarioRow);
            foreach (object row in dataGridView.Rows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem))
            {
                Assert.IsInstanceOf(initialRowType, row);
            }

            bool initialSemiProbabilisticColumnShouldBeVisible = initialScenarioConfigurationType == PipingScenarioConfigurationType.SemiProbabilistic;
            Assert.AreEqual(initialSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilityUpliftColumnIndex].Visible);
            Assert.AreEqual(initialSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilityHeaveColumnIndex].Visible);
            Assert.AreEqual(initialSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].Visible);

            // When
            comboBox.SelectedValue = newScenarioConfigurationType;

            // Then
            Type updatedRowType = newScenarioConfigurationType == PipingScenarioConfigurationType.SemiProbabilistic
                                      ? typeof(SemiProbabilisticPipingScenarioRow)
                                      : typeof(ProbabilisticPipingScenarioRow);
            foreach (object row in dataGridView.Rows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem))
            {
                Assert.IsInstanceOf(updatedRowType, row);
            }

            bool updatedSemiProbabilisticColumnShouldBeVisible = newScenarioConfigurationType == PipingScenarioConfigurationType.SemiProbabilistic;
            Assert.AreEqual(updatedSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilityUpliftColumnIndex].Visible);
            Assert.AreEqual(updatedSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilityHeaveColumnIndex].Visible);
            Assert.AreEqual(updatedSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].Visible);
        }

        [Test]
        public void GivenPipingScenarioViewWithSections_WhenSelectingRadioButton_ThenDataSetAndObserversNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.PerFailureMechanismSection
            };
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationPerFailureMechanismSection = failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.First();
            scenarioConfigurationPerFailureMechanismSection.Attach(observer);

            // Precondition
            Assert.AreEqual(PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic,
                            scenarioConfigurationPerFailureMechanismSection.ScenarioConfigurationType);

            // When
            var radioButtonProbabilistic = (RadioButton) new RadioButtonTester("radioButtonProbabilistic").TheObject;
            radioButtonProbabilistic.Checked = true;

            // Then
            Assert.AreEqual(PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic,
                            scenarioConfigurationPerFailureMechanismSection.ScenarioConfigurationType);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic)]
        [TestCase(PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic)]
        public void GivenPipingScenarioView_WhenSelectingRadioButton_ThenDataGridViewUpdated(PipingScenarioConfigurationPerFailureMechanismSectionType initialScenarioConfigurationType,
                                                                                             PipingScenarioConfigurationPerFailureMechanismSectionType newScenarioConfigurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.PerFailureMechanismSection
            };
            ConfigureFailureMechanism(failureMechanism);
            failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.ForEachElementDo(sc => sc.ScenarioConfigurationType = initialScenarioConfigurationType);

            ShowPipingScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Precondition
            Type initialRowType = initialScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic
                                      ? typeof(SemiProbabilisticPipingScenarioRow)
                                      : typeof(ProbabilisticPipingScenarioRow);
            foreach (object row in dataGridView.Rows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem))
            {
                Assert.IsInstanceOf(initialRowType, row);
            }

            bool initialSemiProbabilisticColumnShouldBeVisible = initialScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic;
            Assert.AreEqual(initialSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilityUpliftColumnIndex].Visible);
            Assert.AreEqual(initialSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilityHeaveColumnIndex].Visible);
            Assert.AreEqual(initialSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].Visible);

            // When
            if (newScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic)
            {
                var radioButtonSemiProbabilistic = (RadioButton) new RadioButtonTester("radioButtonSemiProbabilistic").TheObject;
                radioButtonSemiProbabilistic.Checked = true;
            }
            else
            {
                var radioButtonProbabilistic = (RadioButton) new RadioButtonTester("radioButtonProbabilistic").TheObject;
                radioButtonProbabilistic.Checked = true;
            }

            // Then
            Type updatedRowType = newScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic
                                      ? typeof(SemiProbabilisticPipingScenarioRow)
                                      : typeof(ProbabilisticPipingScenarioRow);
            foreach (object row in dataGridView.Rows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem))
            {
                Assert.IsInstanceOf(updatedRowType, row);
            }

            bool updatedSemiProbabilisticColumnShouldBeVisible = newScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic;
            Assert.AreEqual(updatedSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilityUpliftColumnIndex].Visible);
            Assert.AreEqual(updatedSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilityHeaveColumnIndex].Visible);
            Assert.AreEqual(updatedSemiProbabilisticColumnShouldBeVisible, dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].Visible);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic)]
        [TestCase(PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic, PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic)]
        public void GivenPipingScenarioView_WhenSelectingRadioButton_ThenLengthEffectControlsUpdated(PipingScenarioConfigurationPerFailureMechanismSectionType initialScenarioConfigurationType,
                                                                                                     PipingScenarioConfigurationPerFailureMechanismSectionType newScenarioConfigurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.PerFailureMechanismSection
            };
            ConfigureFailureMechanism(failureMechanism);
            failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.ForEachElementDo(sc => sc.ScenarioConfigurationType = initialScenarioConfigurationType);

            ShowPipingScenariosView(failureMechanism);

            // Precondition
            var tableLayoutPanel = (TableLayoutPanel) new ControlTester("lengthEffectTableLayoutPanel").TheObject;
            bool initialSemiProbabilisticColumnShouldBeVisible = initialScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic;
            Assert.AreEqual(initialSemiProbabilisticColumnShouldBeVisible, tableLayoutPanel.Visible);

            // When
            if (newScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic)
            {
                var radioButtonSemiProbabilistic = (RadioButton) new RadioButtonTester("radioButtonSemiProbabilistic").TheObject;
                radioButtonSemiProbabilistic.Checked = true;
            }
            else
            {
                var radioButtonProbabilistic = (RadioButton) new RadioButtonTester("radioButtonProbabilistic").TheObject;
                radioButtonProbabilistic.Checked = true;
            }

            // Then
            bool updatedSemiProbabilisticControlsShouldBeVisible = newScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic;
            Assert.AreEqual(updatedSemiProbabilisticControlsShouldBeVisible, tableLayoutPanel.Visible);
        }

        [Test]
        public void PipingScenariosView_ContributionValueInvalid_ShowsErrorTooltip()
        {
            // Setup
            ShowFullyConfiguredPipingScenariosView(new PipingFailureMechanism());

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
        public void PipingScenariosView_EditValueValid_DoNotShowErrorToolTipAndEditValue(double newValue)
        {
            // Setup
            ShowFullyConfiguredPipingScenariosView(new PipingFailureMechanism());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
        }

        [Test]
        [TestCase(isRelevantColumnIndex, true)]
        [TestCase(contributionColumnIndex, 30.0)]
        public void PipingScenariosView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(int cellIndex, object newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            ICalculation calculation = failureMechanism.CalculationsGroup.GetCalculations().First();
            calculation.Attach(calculationObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = newValue is double value ? (RoundedDouble) value : newValue;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenPipingScenariosView_WhenSelectingDifferentItemInSectionsListBox_ThenControlsUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            PipingScenarioConfigurationPerFailureMechanismSection lastConfigurationPerSection = failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.Last();
            lastConfigurationPerSection.ScenarioConfigurationType = PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic;
            lastConfigurationPerSection.A = 0.7;

            ShowPipingScenariosView(failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var radioButtonSemiProbabilistic = (RadioButton) new RadioButtonTester("radioButtonSemiProbabilistic").TheObject;
            var radioButtonProbabilistic = (RadioButton) new RadioButtonTester("radioButtonProbabilistic").TheObject;
            var lengthEffectATextBox = (TextBox) new ControlTester("lengthEffectATextBox").TheObject;

            // Precondition
            Assert.AreSame(failureMechanism.Sections.First(), ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).Section);
            Assert.IsTrue(radioButtonSemiProbabilistic.Checked);
            Assert.IsFalse(radioButtonProbabilistic.Checked);
            Assert.AreEqual("0,4", lengthEffectATextBox.Text);

            IPipingScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                 .Select(r => r.DataBoundItem)
                                                                 .Cast<IPipingScenarioRow>()
                                                                 .ToArray();

            // When
            listBox.SelectedItem = listBox.Items[listBox.Items.Count - 1];

            // Then
            Assert.IsFalse(radioButtonSemiProbabilistic.Checked);
            Assert.IsTrue(radioButtonProbabilistic.Checked);
            Assert.AreEqual("0,7", lengthEffectATextBox.Text);

            IPipingScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                           .Select(r => r.DataBoundItem)
                                                           .Cast<IPipingScenarioRow>()
                                                           .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenPipingScenariosViewWithLengthEffectError_WhenSettingValidValue_ThenErrorClearedAndLengthEffectControlsUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            PipingScenariosView view = ShowPipingScenariosView(failureMechanism);

            // Precondition
            var textBoxTester = new TextBoxTester("lengthEffectATextBox");
            textBoxTester.Enter("NotADouble");

            ErrorProvider errorProvider = GetLengthEffectErrorProvider(view);
            var lengthEffectATextBox = (TextBox) new ControlTester("lengthEffectATextBox").TheObject;
            string errorMessage = errorProvider.GetError(lengthEffectATextBox);
            Assert.IsNotEmpty(errorMessage);

            // When
            textBoxTester.Enter("0,6");

            // Then
            errorMessage = errorProvider.GetError(lengthEffectATextBox);
            Assert.IsEmpty(errorMessage);

            Assert.AreEqual("0,6", lengthEffectATextBox.Text);
        }

        [Test]
        public void GivenPipingScenariosViewWithoutLengthEffectError_WhenSettingInvalidValue_ThenErrorSetAndLengthEffectControlsUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            PipingScenariosView view = ShowPipingScenariosView(failureMechanism);

            // Precondition
            ErrorProvider errorProvider = GetLengthEffectErrorProvider(view);
            var lengthEffectATextBox = (TextBox) new ControlTester("lengthEffectATextBox").TheObject;
            string errorMessage = errorProvider.GetError(lengthEffectATextBox);
            Assert.IsEmpty(errorMessage);

            // When
            var textBoxTester = new TextBoxTester("lengthEffectATextBox");
            textBoxTester.Enter("NotADouble");

            // Then
            errorMessage = errorProvider.GetError(lengthEffectATextBox);
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        public void GivenPipingScenariosViewWithLengthEffectError_WhenSelectingDifferentItemInSectionsListBox_ThenErrorCleared()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            PipingScenariosView view = ShowPipingScenariosView(failureMechanism);

            var textBoxTester = new TextBoxTester("lengthEffectATextBox");
            textBoxTester.Enter("NotADouble");

            // Precondition
            ErrorProvider errorProvider = GetLengthEffectErrorProvider(view);
            var lengthEffectATextBox = (TextBox) new ControlTester("lengthEffectATextBox").TheObject;
            string errorMessage = errorProvider.GetError(lengthEffectATextBox);
            Assert.IsNotEmpty(errorMessage);

            // When
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            listBox.SelectedItem = listBox.Items[listBox.Items.Count - 1];

            // Then
            errorMessage = errorProvider.GetError(lengthEffectATextBox);
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenPipingScenariosView_WhenSettingInvalidValueAndEscPressed_ThenLengthEffectControlsSetToInitialValues()
        {
            // Given
            const double initialValue = 0.5;
            const string initialValueText = "0,5";

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            PipingScenarioConfigurationPerFailureMechanismSection firstConfigurationPerSection =
                failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.First();
            firstConfigurationPerSection.A = initialValue;

            ShowPipingScenariosView(failureMechanism);

            var textBoxTester = new ControlTester("lengthEffectATextBox");
            const Keys keyData = Keys.Escape;

            var lengthEffectATextBox = (TextBox) new ControlTester("lengthEffectATextBox").TheObject;
            lengthEffectATextBox.TextChanged += (sender, args) =>
            {
                textBoxTester.FireEvent("KeyDown", new KeyEventArgs(keyData));
            };

            // Precondition
            Assert.AreEqual(initialValueText, lengthEffectATextBox.Text);

            failureMechanism.AssemblyResult.Attach(observer);

            // When
            lengthEffectATextBox.Text = "NotAProbability";

            // Then
            Assert.AreEqual(initialValueText, lengthEffectATextBox.Text);
            Assert.AreEqual(initialValue, firstConfigurationPerSection.A);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingScenariosView_WhenFailureMechanismNotifiesObserver_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowPipingScenariosView(failureMechanism);

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
            Assert.AreSame(failureMechanismSection1, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.Items[0]).Section);
            Assert.AreSame(failureMechanismSection2, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.Items[1]).Section);
            Assert.AreSame(failureMechanismSection3, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.Items[2]).Section);
            Assert.AreSame(failureMechanismSection1, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).Section);
        }

        [Test]
        public void GivenPipingScenariosViewWithItemSelectedInSectionsListBox_WhenFailureMechanismNotifiesObserverAndSelectedSectionSame_ThenSectionsListBoxUpdatedAndSelectionSame()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();

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

            ShowPipingScenariosView(failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            listBox.SelectedItem = listBox.Items[1];

            // Precondition
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection2, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).Section);

            // When
            failureMechanism.NotifyObservers();

            // Then
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection2, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).Section);
        }

        [Test]
        public void GivenPipingScenariosViewWithItemSelectedInSectionsListBox_WhenFailureMechanismNotifiesObserverAndSelectedSectionNotSame_ThenSectionsListBoxUpdatedAndSelectionReset()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();

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

            ShowPipingScenariosView(failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            listBox.SelectedItem = listBox.Items[1];

            // Precondition
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection2, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).Section);

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
            Assert.AreSame(failureMechanismSection4, ((PipingScenariosViewFailureMechanismSectionViewModel) listBox.SelectedItem).Section);
        }

        [Test]
        public void GivenPipingScenariosView_WhenFailureMechanismNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            IPipingScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                 .Select(r => r.DataBoundItem)
                                                                 .Cast<IPipingScenarioRow>()
                                                                 .ToArray();

            // When
            failureMechanism.NotifyObservers();

            // Then
            IPipingScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                           .Select(r => r.DataBoundItem)
                                                           .Cast<IPipingScenarioRow>()
                                                           .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenPipingScenariosView_WhenPipingScenarioConfigurationPerFailureMechanismSection_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.PerFailureMechanismSection
            };
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
            ShowPipingScenariosView(failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Precondition
            Assert.AreEqual("Section 1 (semi-probabilistisch)", listBox.Items[0].ToString());
            Assert.AreEqual("Section 2 (semi-probabilistisch)", listBox.Items[1].ToString());
            Assert.AreEqual("Section 3 (semi-probabilistisch)", listBox.Items[2].ToString());

            // When
            PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationPerFailureMechanismSection = failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.ElementAt(1);
            scenarioConfigurationPerFailureMechanismSection.ScenarioConfigurationType = PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic;
            scenarioConfigurationPerFailureMechanismSection.NotifyObservers();

            // Then
            Assert.AreEqual("Section 1 (semi-probabilistisch)", listBox.Items[0].ToString());
            Assert.AreEqual("Section 2 (probabilistisch)", listBox.Items[1].ToString());
            Assert.AreEqual("Section 3 (semi-probabilistisch)", listBox.Items[2].ToString());
        }

        [Test]
        public void GivenPipingScenariosView_WhenCalculationGroupNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            IPipingScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                 .Select(r => r.DataBoundItem)
                                                                 .Cast<IPipingScenarioRow>()
                                                                 .ToArray();

            // When
            failureMechanism.CalculationsGroup.NotifyObservers();

            // Then
            IPipingScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                           .Select(r => r.DataBoundItem)
                                                           .Cast<IPipingScenarioRow>()
                                                           .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenPipingScenariosView_WhenCalculationNotifiesObserver_ThenViewUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);
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
        public void GivenPipingScenariosView_WhenCalculationInputNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            IPipingScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                 .Select(r => r.DataBoundItem)
                                                                 .Cast<IPipingScenarioRow>()
                                                                 .ToArray();

            IPipingCalculationScenario<PipingInput> calculation = failureMechanism.CalculationsGroup.GetCalculations()
                                                                                  .Cast<IPipingCalculationScenario<PipingInput>>()
                                                                                  .First();

            // When
            calculation.InputParameters.NotifyObservers();

            // Then
            IPipingScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                           .Select(r => r.DataBoundItem)
                                                           .Cast<IPipingScenarioRow>()
                                                           .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic)]
        public void GivenPipingScenariosViewWithTotalContributionsNotValid_WhenEditingScenarioContributionToValidValue_ThenTotalContributionLabelUpdatedAndErrorNotShown(
            PipingScenarioConfigurationType configurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = configurationType
            };
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.5011
                },
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.75
                },
                new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.5011
                },
                new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.75
                }
            });

            PipingScenariosView view = ShowPipingScenariosView(failureMechanism);

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
        [Combinatorial]
        public void GivenPipingScenariosViewWithTotalContributionsValid_WhenEditingScenarioContributionsToInvalidValue_ThenTotalContributionLabelUpdatedAndErrorShown(
            [Values(25.01, 24.99, 50)] double newContribution,
            [Values(PipingScenarioConfigurationType.SemiProbabilistic, PipingScenarioConfigurationType.Probabilistic)]
            PipingScenarioConfigurationType configurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = configurationType
            };
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.25
                },
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.75
                },
                new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.25
                },
                new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    Contribution = (RoundedDouble) 0.75
                }
            });

            PipingScenariosView view = ShowPipingScenariosView(failureMechanism);

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
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic)]
        public void GivenPipingScenariosViewWithoutContributingScenarios_WhenMakingScenarioRelevant_ThenTotalContributionLabelUpdatedAndShown(
            PipingScenarioConfigurationType configurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = configurationType
            };
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    IsRelevant = false
                },
                new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    },
                    IsRelevant = false
                }
            });

            ShowPipingScenariosView(failureMechanism);

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
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic)]
        public void GivenPipingScenariosViewWithContributingScenarios_WhenMakingContributingScenarioIrrelevant_ThenTotalContributionLabelNotVisible(
            PipingScenarioConfigurationType configurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = configurationType
            };
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    }
                },
                new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    }
                }
            });

            ShowPipingScenariosView(failureMechanism);

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
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic)]
        public void GivenPipingScenariosViewWithoutScenarios_WhenAddingRelevantScenarioAndCalculationGroupNotifiesObservers_ThenTotalContributionLabelUpdatedAndShown(
            PipingScenarioConfigurationType configurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = configurationType
            };
            ConfigureFailureMechanism(failureMechanism);
            failureMechanism.CalculationsGroup.Children.Clear();

            ShowPipingScenariosView(failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsFalse(totalScenarioContributionLabel.Visible);

            // When
            IPipingCalculationScenario<PipingInput> calculationToAdd =
                configurationType == PipingScenarioConfigurationType.SemiProbabilistic
                    ? (IPipingCalculationScenario<PipingInput>) new SemiProbabilisticPipingCalculationScenario
                    {
                        InputParameters =
                        {
                            SurfaceLine = failureMechanism.SurfaceLines.First()
                        }
                    }
                    : new ProbabilisticPipingCalculationScenario
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
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic)]
        public void GivenPipingScenariosViewWithScenarios_WhenCalculationsGroupClearedAndCalculationGroupNotifiesObservers_ThenTotalContributionLabelNotVisible(
            PipingScenarioConfigurationType configurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = configurationType
            };
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    }
                },
                new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    }
                }
            });

            ShowPipingScenariosView(failureMechanism);

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
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic)]
        public void GivenPipingScenariosViewWithScenarios_WhenScenariosClearedAndFailureMechanismNotifiesObserver_ThenTotalContributionLabelNotVisible(
            PipingScenarioConfigurationType configurationType)
        {
            // Given
            var failureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = configurationType
            };
            ConfigureFailureMechanism(failureMechanism);

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Clear();
            calculationGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    }
                },
                new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = failureMechanism.SurfaceLines.First()
                    }
                }
            });

            ShowPipingScenariosView(failureMechanism);

            // Precondition
            var totalScenarioContributionLabel = (Label) new ControlTester("labelTotalScenarioContribution").TheObject;
            Assert.IsTrue(totalScenarioContributionLabel.Visible);

            // When
            calculationGroup.Children.Clear();
            failureMechanism.NotifyObservers();

            // Then
            Assert.IsFalse(totalScenarioContributionLabel.Visible);
        }

        private static ErrorProvider GetErrorProvider(PipingScenariosView view)
        {
            return TypeUtils.GetField<ErrorProvider>(view, "errorProvider");
        }

        private static ErrorProvider GetLengthEffectErrorProvider(PipingScenariosView view)
        {
            return TypeUtils.GetField<ErrorProvider>(view, "lengthEffectErrorProvider");
        }

        private static void ConfigureFailureMechanism(PipingFailureMechanism failureMechanism)
        {
            var surfaceLine1 = new PipingSurfaceLine("Surface line 1")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new PipingSurfaceLine("Surface line 2")
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
            }, "Path");

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

            failureMechanism.CalculationsGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
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
                    },
                    Contribution = (RoundedDouble) 0.13701
                },
                new SemiProbabilisticPipingCalculationScenario
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
                    IsRelevant = false
                },
                new SemiProbabilisticPipingCalculationScenario
                {
                    Name = "Calculation 3",
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
                    Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(0.26065, 0.81398, 0.38024),
                    Contribution = (RoundedDouble) 0.37503
                },
                new ProbabilisticPipingCalculationScenario
                {
                    Name = "Calculation 4",
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
                    },
                    Contribution = (RoundedDouble) 0.13701
                },
                new ProbabilisticPipingCalculationScenario
                {
                    Name = "Calculation 5",
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
                    },
                    IsRelevant = false
                },
                new ProbabilisticPipingCalculationScenario
                {
                    Name = "Calculation 6",
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
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints(),
                    Contribution = (RoundedDouble) 0.37503
                }
            });
        }

        private void ShowFullyConfiguredPipingScenariosView(PipingFailureMechanism failureMechanism)
        {
            ConfigureFailureMechanism(failureMechanism);

            ShowPipingScenariosView(failureMechanism);
        }

        private PipingScenariosView ShowPipingScenariosView(PipingFailureMechanism failureMechanism)
        {
            var pipingScenarioView = new PipingScenariosView(failureMechanism.CalculationsGroup, failureMechanism, new AssessmentSectionStub());

            testForm.Controls.Add(pipingScenarioView);
            testForm.Show();

            return pipingScenarioView;
        }
    }
}