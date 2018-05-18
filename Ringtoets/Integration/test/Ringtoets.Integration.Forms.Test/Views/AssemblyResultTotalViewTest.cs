﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Forms.Controls;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyResultTotalViewTest
    {
        private const int expectedColumnCount = 5;
        private const int failureMechanismNameColumnIndex = 0;
        private const int failureMechanismCodeColumnIndex = 1;
        private const int failureMechanismGroupColumnIndex = 2;
        private const int failureMechanismAssemblyCategoryColumnIndex = 3;
        private const int failureMechanisProbabilityColumnIndex = 4;
        private const string totalControlName = "totalAssemblyCategoryGroupControl";
        private const string failureMechanismsWithProbabilityControlName = "failureMechanismsWithProbabilityAssemblyControl";
        private const string failureMechanismsWithoutProbabilityControlName = "failureMechanismsWithoutProbabilityAssemblyControl";
        private const string assemblyResultOutdatedWarning = "Assemblageresultaat is verouderd. Druk op de \"Assemblageresultaat verversen\" knop om opnieuw te berekenen.";
        private Form testForm;

        private static IEnumerable<TestCaseData> CellFormattingStates
        {
            get
            {
                yield return new TestCaseData(true, "", CellStyle.Disabled);
                yield return new TestCaseData(false, "Error", CellStyle.Enabled);
            }
        }

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
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyResultTotalView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAssessmentSection_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            using (var view = new AssemblyResultTotalView(assessmentSection))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreEqual(3, view.Controls.Count);

                Button button = GetRefreshAssemblyResultButtonTester().Properties;
                Assert.AreEqual("Assemblageresultaat verversen", button.Text);
                Assert.IsFalse(button.Enabled);

                var groupBox = (GroupBox) new ControlTester("assemblyResultGroupBox").TheObject;
                Assert.AreEqual(1, groupBox.Controls.Count);
                Assert.AreEqual(DockStyle.Top, groupBox.Dock);
                Assert.AreEqual("Gecombineerd veiligheidsoordeel", groupBox.Text);

                var tableLayoutPanel = (TableLayoutPanel) groupBox.Controls["assemblyResultTableLayoutPanel"];
                Assert.AreEqual(2, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(3, tableLayoutPanel.RowCount);
                Assert.AreEqual(DockStyle.Fill, tableLayoutPanel.Dock);

                var totalResultLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 0);
                Assert.AreEqual("Totaal", totalResultLabel.Text);
                var failureMechanismsWithProbabilityLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 1);
                Assert.AreEqual("Groepen 1 en 2", failureMechanismsWithProbabilityLabel.Text);
                var failureMechanismsWithoutProbablityLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 2);
                Assert.AreEqual("Groepen 3 en 4", failureMechanismsWithoutProbablityLabel.Text);
                Assert.IsInstanceOf<AssessmentSectionAssemblyCategoryGroupControl>(tableLayoutPanel.GetControlFromPosition(1, 0));
                Assert.IsInstanceOf<AssessmentSectionAssemblyControl>(tableLayoutPanel.GetControlFromPosition(1, 1));
                Assert.IsInstanceOf<AssessmentSectionAssemblyCategoryGroupControl>(tableLayoutPanel.GetControlFromPosition(1, 2));

                var datagridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl").TheObject;
                Assert.AreEqual(DockStyle.Fill, datagridViewControl.Dock);

                ErrorProvider warningProvider = GetWarningProvider(view);
                Assert.AreEqual(4, warningProvider.GetIconPadding(button));

                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (ShowAssemblyResultTotalView())
            {
                // Then
                DataGridView dataGridView = GetDataGridView();
                Assert.AreEqual(expectedColumnCount, dataGridView.ColumnCount);

                DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismNameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismCodeColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismGroupColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismAssemblyCategoryColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanisProbabilityColumnIndex]);

                Assert.AreEqual("Toetsspoor", dataGridViewColumns[failureMechanismNameColumnIndex].HeaderText);
                Assert.AreEqual("Label", dataGridViewColumns[failureMechanismCodeColumnIndex].HeaderText);
                Assert.AreEqual("Groep", dataGridViewColumns[failureMechanismGroupColumnIndex].HeaderText);
                Assert.AreEqual("Categorie", dataGridViewColumns[failureMechanismAssemblyCategoryColumnIndex].HeaderText);
                Assert.AreEqual("Benaderde faalkans", dataGridViewColumns[failureMechanisProbabilityColumnIndex].HeaderText);

                Assert.IsTrue(dataGridViewColumns[failureMechanismNameColumnIndex].ReadOnly);
                Assert.IsTrue(dataGridViewColumns[failureMechanismCodeColumnIndex].ReadOnly);
                Assert.IsTrue(dataGridViewColumns[failureMechanismGroupColumnIndex].ReadOnly);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenFormWithAssemblyResultTotalView_ThenExpectedCellsVisible()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;

                using (AssemblyResultTotalView view = ShowAssemblyResultTotalView())
                {
                    // Then
                    DataGridView dataGridView = GetDataGridView();
                    AssertFailureMechanismRows(view.AssessmentSection,
                                               calculator.FailureMechanismAssemblyOutput,
                                               calculator.FailureMechanismAssemblyCategoryGroupOutput.Value,
                                               dataGridView.Rows);
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_ThenExpectedAssessmentSectionAssemblyResultsVisible()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultTotalView())
            {
                // Then
                AssertAssessmentSectionAssemblyCategoryGroupControl(totalControlName, "C");
                AssertAssessmentSectionAssemblyControl(failureMechanismsWithProbabilityControlName, "D", "1/1");
                AssertAssessmentSectionAssemblyCategoryGroupControl(failureMechanismsWithoutProbabilityControlName, "D");
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenRefreshingAssemblyResults_ThenAssessmentSectionAssemblyResultsUpdatedToNewValues()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultTotalView())
            {
                GetRefreshAssemblyResultButtonTester().Properties.Enabled = true;

                // Precondition
                AssertAssessmentSectionAssemblyCategoryGroupControl(totalControlName, "C");
                AssertAssessmentSectionAssemblyControl(failureMechanismsWithProbabilityControlName, "D", "1/1");
                AssertAssessmentSectionAssemblyCategoryGroupControl(failureMechanismsWithoutProbabilityControlName, "D");

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.AssembleAssessmentSectionCategoryGroupOutput = AssessmentSectionAssemblyCategoryGroup.A;
                calculator.AssembleFailureMechanismsAssemblyOutput = new AssessmentSectionAssembly(0.5, AssessmentSectionAssemblyCategoryGroup.APlus);
                calculator.AssembleFailureMechanismsAssemblyCategoryGroupOutput = AssessmentSectionAssemblyCategoryGroup.B;

                GetRefreshAssemblyResultButtonTester().Click();

                // Then
                AssertAssessmentSectionAssemblyCategoryGroupControl(totalControlName, "A");
                AssertAssessmentSectionAssemblyControl(failureMechanismsWithProbabilityControlName, "A+", "1/2");
                AssertAssessmentSectionAssemblyCategoryGroupControl(failureMechanismsWithoutProbabilityControlName, "B");
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenRefreshingAssemblyResultsThrowsException_ThenAssessmentSectionAssemblyResultsClearedAndErrorSet()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultTotalView())
            {
                GetRefreshAssemblyResultButtonTester().Properties.Enabled = true;

                // Precondition
                AssertAssessmentSectionAssemblyCategoryGroupControl(totalControlName, "C");
                AssertAssessmentSectionAssemblyControl(failureMechanismsWithProbabilityControlName, "D", "1/1");
                AssertAssessmentSectionAssemblyCategoryGroupControl(failureMechanismsWithoutProbabilityControlName, "D");

                // When
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                GetRefreshAssemblyResultButtonTester().Click();

                // Then
                AssertAssessmentSectionAssemblyWithoutProbabilityControlWithError(totalControlName);
                AssertAssessmentSectionAssemblyWithProbabilityControlWithError(failureMechanismsWithProbabilityControlName);
                AssertAssessmentSectionAssemblyWithoutProbabilityControlWithError(failureMechanismsWithoutProbabilityControlName);
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalViewAndErrorOnAssemblyResult_WhenRefreshingAssemblyResultsWithoutException_ThenAssessmentSectionAssemblyResultsSetAndErrorCleared()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                using (ShowAssemblyResultTotalView())
                {
                    GetRefreshAssemblyResultButtonTester().Properties.Enabled = true;

                    // Precondition
                    AssertAssessmentSectionAssemblyWithoutProbabilityControlWithError(totalControlName);
                    AssertAssessmentSectionAssemblyWithProbabilityControlWithError(failureMechanismsWithProbabilityControlName);
                    AssertAssessmentSectionAssemblyWithoutProbabilityControlWithError(failureMechanismsWithoutProbabilityControlName);

                    // When
                    calculator.ThrowExceptionOnCalculate = false;
                    GetRefreshAssemblyResultButtonTester().Click();

                    // Then
                    AssertAssessmentSectionAssemblyCategoryGroupControl(totalControlName, "C");
                    Assert.IsEmpty(GetError(GetAssemblyCategoryGroupControl(totalControlName)));

                    AssertAssessmentSectionAssemblyControl(failureMechanismsWithProbabilityControlName, "D", "1/1");
                    Assert.IsEmpty(GetError(GetAssemblyControl(failureMechanismsWithProbabilityControlName)));

                    AssertAssessmentSectionAssemblyCategoryGroupControl(failureMechanismsWithoutProbabilityControlName, "D");
                    Assert.IsEmpty(GetError(GetAssemblyCategoryGroupControl(failureMechanismsWithoutProbabilityControlName)));
                }
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenFormWithAssemblyResultTotalView_WhenRefreshingAssemblyResults_ThenDataGridViewInvalidatedAndCellsUpdatedToNewValues()
        {
            // Given
            var random = new Random(21);

            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;

                GetRefreshAssemblyResultButtonTester().Properties.Enabled = true;

                var invalidated = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // Precondition
                Assert.IsFalse(invalidated);
                DataGridViewRowCollection rows = dataGridView.Rows;
                AssertFailureMechanismRows(view.AssessmentSection,
                                           calculator.FailureMechanismAssemblyOutput,
                                           calculator.FailureMechanismAssemblyCategoryGroupOutput.Value,
                                           rows);

                // When
                var newAssemblyResult = new FailureMechanismAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
                var newCategoryGroup = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();
                calculator.FailureMechanismAssemblyOutput = newAssemblyResult;
                calculator.FailureMechanismAssemblyCategoryGroupOutput = newCategoryGroup;

                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Click();

                // Then 
                Assert.IsTrue(invalidated);
                AssertFailureMechanismRows(view.AssessmentSection, newAssemblyResult, newCategoryGroup, rows);
            }
        }

        [Test]
        [TestCaseSource(nameof(CellFormattingStates))]
        public void GivenFormWithAssemblyResultTotalView_WhenCellFormattingEventFired_ThenCategoryColumnCellStyleSetToColumnDefinition(
            bool readOnly, string errorText, CellStyle style)
        {
            // Given
            using (ShowAssemblyResultTotalView())
            {
                GetRefreshAssemblyResultButtonTester().Properties.Enabled = true;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.CellFormatting += (sender, args) =>
                {
                    var row = (IHasColumnStateDefinitions) dataGridView.Rows[0].DataBoundItem;
                    DataGridViewColumnStateDefinition definition = row.ColumnStateDefinitions[failureMechanismAssemblyCategoryColumnIndex];
                    definition.ReadOnly = readOnly;
                    definition.ErrorText = errorText;
                    definition.Style = style;
                };

                // When
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Click();

                // Then
                DataGridViewCell cell = dataGridView.Rows[0].Cells[failureMechanismAssemblyCategoryColumnIndex];
                Assert.AreEqual(readOnly, cell.ReadOnly);
                Assert.AreEqual(errorText, cell.ErrorText);
                Assert.AreEqual(style.BackgroundColor, cell.Style.BackColor);
                Assert.AreEqual(style.TextColor, cell.Style.ForeColor);
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalViewWithOutdatedContent_WhenRefreshingAssemblyResults_ThenRefreshButtonDisabledAndWarningCleared()
        {
            // Given
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());

            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView(assessmentSection))
            {
                assessmentSection.NotifyObservers();

                // Precondition
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                Button button = buttonTester.Properties;
                Assert.IsTrue(button.Enabled);
                ErrorProvider warningProvider = GetWarningProvider(view);
                Assert.AreEqual(assemblyResultOutdatedWarning, warningProvider.GetError(button));

                // When
                buttonTester.Click();

                // Then 
                Assert.IsFalse(button.Enabled);
                Assert.IsEmpty(warningProvider.GetError(button));
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenAssessmentSectionNotifiesObservers_ThenRefreshButtonEnabledAndWarningSet()
        {
            // Given
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());

            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView(assessmentSection))
            {
                // Precondition
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                Button button = buttonTester.Properties;
                Assert.IsFalse(button.Enabled);
                ErrorProvider warningProvider = GetWarningProvider(view);
                Assert.IsEmpty(warningProvider.GetError(button));

                // When
                assessmentSection.NotifyObservers();

                // Then 
                Assert.IsTrue(buttonTester.Properties.Enabled);
                Assert.AreEqual(assemblyResultOutdatedWarning, warningProvider.GetError(button));
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenFailureMechanismNotifiesObservers_ThenRefreshButtonEnabledAndWarningSet()
        {
            // Given
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());

            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView(assessmentSection))
            {
                // Precondition
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                Button button = buttonTester.Properties;
                Assert.IsFalse(button.Enabled);
                ErrorProvider warningProvider = GetWarningProvider(view);
                Assert.IsEmpty(warningProvider.GetError(button));

                // When
                assessmentSection.StabilityStoneCover.NotifyObservers();

                // Then 
                Assert.IsTrue(buttonTester.Properties.Enabled);
                Assert.AreEqual(assemblyResultOutdatedWarning, warningProvider.GetError(button));
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenCalculationNotifiesObservers_ThenRefreshButtonEnabledAndWarningSet()
        {
            // Given
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());
            var calculation = new TestHeightStructuresCalculation();
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(calculation);

            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView(assessmentSection))
            {
                // Precondition
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                Button button = buttonTester.Properties;
                Assert.IsFalse(button.Enabled);
                ErrorProvider warningProvider = GetWarningProvider(view);
                Assert.IsEmpty(warningProvider.GetError(button));

                // When
                calculation.NotifyObservers();

                // Then 
                Assert.IsTrue(buttonTester.Properties.Enabled);
                Assert.AreEqual(assemblyResultOutdatedWarning, warningProvider.GetError(button));
            }
        }

        private static ErrorProvider GetWarningProvider(AssemblyResultTotalView resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "warningProvider");
        }

        #region View test helpers

        private AssemblyResultTotalView ShowAssemblyResultTotalView()
        {
            return ShowAssemblyResultTotalView(new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>()));
        }

        private AssemblyResultTotalView ShowAssemblyResultTotalView(AssessmentSection assessmentSection)
        {
            var view = new AssemblyResultTotalView(assessmentSection);
            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private ButtonTester GetRefreshAssemblyResultButtonTester()
        {
            return new ButtonTester("RefreshAssemblyResultsButton", testForm);
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private static AssessmentSectionAssemblyCategoryGroupControl GetAssemblyCategoryGroupControl(string controlName)
        {
            return (AssessmentSectionAssemblyCategoryGroupControl) new ControlTester(controlName).TheObject;
        }

        private static AssessmentSectionAssemblyControl GetAssemblyControl(string controlName)
        {
            return (AssessmentSectionAssemblyControl) new ControlTester(controlName).TheObject;
        }

        private static BorderedLabel GetGroupLabel(AssemblyResultControl control)
        {
            return (BorderedLabel) ((TableLayoutPanel) control.Controls["GroupPanel"]).GetControlFromPosition(0, 0);
        }

        private static BorderedLabel GetProbabilityLabel(AssemblyResultWithProbabilityControl control)
        {
            return (BorderedLabel) ((TableLayoutPanel) control.Controls["probabilityPanel"]).GetControlFromPosition(0, 0);
        }

        private static string GetError(AssemblyResultControl resultControl)
        {
            var errorProvider = TypeUtils.GetField<ErrorProvider>(resultControl, "errorProvider");
            return errorProvider.GetError(resultControl);
        }

        #endregion

        #region Asserts datagrid control

        private static void AssertFailureMechanismRows(AssessmentSection assessmentSection,
                                                       FailureMechanismAssembly assemblyOutput,
                                                       FailureMechanismAssemblyCategoryGroup assemblyCategoryGroup,
                                                       DataGridViewRowCollection rows)
        {
            Assert.AreEqual(assessmentSection.GetFailureMechanisms().Count(), rows.Count);

            PipingFailureMechanism piping = assessmentSection.Piping;
            AssertAssemblyCells(piping, assemblyOutput, rows[0].Cells);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = assessmentSection.GrassCoverErosionInwards;
            AssertAssemblyCells(grassCoverErosionInwards, assemblyOutput, rows[1].Cells);

            MacroStabilityInwardsFailureMechanism macroStabilityInwards = assessmentSection.MacroStabilityInwards;
            AssertAssemblyCells(macroStabilityInwards, assemblyOutput, rows[2].Cells);

            MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = assessmentSection.MacroStabilityOutwards;
            AssertAssemblyCells(macroStabilityOutwards, assemblyCategoryGroup, rows[3].Cells);

            MicrostabilityFailureMechanism microStability = assessmentSection.Microstability;
            AssertAssemblyCells(microStability, assemblyCategoryGroup, rows[4].Cells);

            StabilityStoneCoverFailureMechanism stabilityStoneCover = assessmentSection.StabilityStoneCover;
            AssertAssemblyCells(stabilityStoneCover, assemblyCategoryGroup, rows[5].Cells);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = assessmentSection.WaveImpactAsphaltCover;
            AssertAssemblyCells(waveImpactAsphaltCover, assemblyCategoryGroup, rows[6].Cells);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover = assessmentSection.WaterPressureAsphaltCover;
            AssertAssemblyCells(waterPressureAsphaltCover, assemblyCategoryGroup, rows[7].Cells);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = assessmentSection.GrassCoverErosionOutwards;
            AssertAssemblyCells(grassCoverErosionOutwards, assemblyCategoryGroup, rows[8].Cells);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = assessmentSection.GrassCoverSlipOffOutwards;
            AssertAssemblyCells(grassCoverSlipOffOutwards, assemblyCategoryGroup, rows[9].Cells);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = assessmentSection.GrassCoverSlipOffInwards;
            AssertAssemblyCells(grassCoverSlipOffInwards, assemblyCategoryGroup, rows[10].Cells);

            HeightStructuresFailureMechanism heightStructures = assessmentSection.HeightStructures;
            AssertAssemblyCells(heightStructures, assemblyOutput, rows[11].Cells);

            ClosingStructuresFailureMechanism closingStructures = assessmentSection.ClosingStructures;
            AssertAssemblyCells(closingStructures, assemblyOutput, rows[12].Cells);

            PipingStructureFailureMechanism pipingStructure = assessmentSection.PipingStructure;
            AssertAssemblyCells(pipingStructure, assemblyCategoryGroup, rows[13].Cells);

            StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSection.StabilityPointStructures;
            AssertAssemblyCells(stabilityPointStructures, assemblyOutput, rows[14].Cells);

            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction = assessmentSection.StrengthStabilityLengthwiseConstruction;
            AssertAssemblyCells(strengthStabilityLengthwiseConstruction, assemblyCategoryGroup, rows[15].Cells);

            DuneErosionFailureMechanism duneErosion = assessmentSection.DuneErosion;
            AssertAssemblyCells(duneErosion, assemblyCategoryGroup, rows[16].Cells);

            TechnicalInnovationFailureMechanism technicalInnovation = assessmentSection.TechnicalInnovation;
            AssertAssemblyCells(technicalInnovation, assemblyCategoryGroup, rows[17].Cells);
        }

        private static void AssertAssemblyCells(IFailureMechanism failureMechanism, DataGridViewCellCollection cells)
        {
            Assert.AreEqual(expectedColumnCount, cells.Count);

            Assert.AreEqual(failureMechanism.Name, cells[failureMechanismNameColumnIndex].Value);
            Assert.AreEqual(failureMechanism.Code, cells[failureMechanismCodeColumnIndex].Value);
            Assert.AreEqual(failureMechanism.Group, cells[failureMechanismGroupColumnIndex].Value);
        }

        private static void AssertAssemblyCells(IFailureMechanism failureMechanism,
                                                FailureMechanismAssembly assemblyOutput,
                                                DataGridViewCellCollection cells)
        {
            AssertAssemblyCells(failureMechanism, cells);

            Assert.AreEqual(assemblyOutput.Group, cells[failureMechanismAssemblyCategoryColumnIndex].Value);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(assemblyOutput.Probability),
                            cells[failureMechanisProbabilityColumnIndex].FormattedValue);
        }

        private static void AssertAssemblyCells(IFailureMechanism failureMechanism,
                                                FailureMechanismAssemblyCategoryGroup categoryGroup,
                                                DataGridViewCellCollection cells)
        {
            AssertAssemblyCells(failureMechanism, cells);

            Assert.AreEqual(categoryGroup, cells[failureMechanismAssemblyCategoryColumnIndex].Value);
            Assert.AreEqual("-", cells[failureMechanisProbabilityColumnIndex].FormattedValue);
        }

        #endregion

        #region Asserts total assembly control

        private static void AssertAssessmentSectionAssemblyWithProbabilityControlWithError(string controlName)
        {
            Assert.AreEqual("Message", GetError(GetAssemblyControl(controlName)));
            AssertAssessmentSectionAssemblyControl(failureMechanismsWithProbabilityControlName, string.Empty, "-");
        }

        private static void AssertAssessmentSectionAssemblyWithoutProbabilityControlWithError(string controlName)
        {
            Assert.AreEqual("Message", GetError(GetAssemblyCategoryGroupControl(controlName)));
            AssertAssessmentSectionAssemblyCategoryGroupControl(totalControlName, string.Empty);
        }

        private static void AssertAssessmentSectionAssemblyCategoryGroupControl(string controlName, string expectedGroup)
        {
            AssessmentSectionAssemblyCategoryGroupControl control = GetAssemblyCategoryGroupControl(controlName);
            Assert.AreEqual(expectedGroup, GetGroupLabel(control).Text);
        }

        private static void AssertAssessmentSectionAssemblyControl(string controlName, string expectedGroup, string expectedProbability)
        {
            AssessmentSectionAssemblyControl control = GetAssemblyControl(controlName);
            Assert.AreEqual(expectedGroup, GetGroupLabel(control).Text);
            Assert.AreEqual(expectedProbability, GetProbabilityLabel(control).Text);
        }

        #endregion
    }
}