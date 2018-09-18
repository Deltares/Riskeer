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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Primitives;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Controls;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

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
        private const int failureMechanismProbabilityColumnIndex = 4;
        private const string totalControlName = "totalAssemblyCategoryGroupControl";
        private const string failureMechanismsWithProbabilityControlName = "failureMechanismsWithProbabilityAssemblyControl";
        private const string failureMechanismsWithoutProbabilityControlName = "failureMechanismsWithoutProbabilityAssemblyControl";
        private const string assemblyResultOutdatedWarning = "Toetsoordeel is verouderd. Druk op de \"Toetsoordeel verversen\" knop om opnieuw te berekenen.";
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
                Assert.AreEqual("Toetsoordeel verversen", button.Text);
                Assert.IsFalse(button.Enabled);

                var groupBox = (GroupBox) new ControlTester("assemblyResultGroupBox").TheObject;
                Assert.AreEqual(1, groupBox.Controls.Count);
                Assert.AreEqual(DockStyle.Top, groupBox.Dock);
                Assert.AreEqual("Gecombineerd toetsoordeel", groupBox.Text);

                var tableLayoutPanel = (TableLayoutPanel) groupBox.Controls["assemblyResultTableLayoutPanel"];
                Assert.AreEqual(2, tableLayoutPanel.ColumnCount);
                Assert.AreEqual(3, tableLayoutPanel.RowCount);
                Assert.AreEqual(DockStyle.Fill, tableLayoutPanel.Dock);

                var totalResultLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 0);
                Assert.AreEqual("Veiligheidsoordeel", totalResultLabel.Text);
                var failureMechanismsWithProbabilityLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 1);
                Assert.AreEqual("Toetsoordeel groepen 1 en 2", failureMechanismsWithProbabilityLabel.Text);
                var failureMechanismsWithoutProbabilityLabel = (Label) tableLayoutPanel.GetControlFromPosition(0, 2);
                Assert.AreEqual("Toetsoordeel groepen 3 en 4", failureMechanismsWithoutProbabilityLabel.Text);
                Assert.IsInstanceOf<AssessmentSectionAssemblyCategoryGroupControl>(tableLayoutPanel.GetControlFromPosition(1, 0));
                Assert.IsInstanceOf<FailureMechanismAssemblyControl>(tableLayoutPanel.GetControlFromPosition(1, 1));
                Assert.IsInstanceOf<FailureMechanismAssemblyCategoryGroupControl>(tableLayoutPanel.GetControlFromPosition(1, 2));

                var datagridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl").TheObject;
                Assert.AreEqual(DockStyle.Fill, datagridViewControl.Dock);

                ErrorProvider warningProvider = GetWarningProvider(view);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.warning.ToBitmap(), warningProvider.Icon.ToBitmap());
                Assert.AreEqual(ErrorBlinkStyle.NeverBlink, warningProvider.BlinkStyle);
                Assert.IsEmpty(warningProvider.GetError(view));
                Assert.AreEqual(4, warningProvider.GetIconPadding(button));

                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(new Size(350, 250), view.AutoScrollMinSize);
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
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismProbabilityColumnIndex]);

                Assert.AreEqual("Toetsspoor", dataGridViewColumns[failureMechanismNameColumnIndex].HeaderText);
                Assert.AreEqual("Label", dataGridViewColumns[failureMechanismCodeColumnIndex].HeaderText);
                Assert.AreEqual("Groep", dataGridViewColumns[failureMechanismGroupColumnIndex].HeaderText);
                Assert.AreEqual("Categorie", dataGridViewColumns[failureMechanismAssemblyCategoryColumnIndex].HeaderText);
                Assert.AreEqual("Benaderde faalkans [1/jaar]", dataGridViewColumns[failureMechanismProbabilityColumnIndex].HeaderText);

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
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

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
                AssertAssemblyResultControl(totalControlName, "C");
                AssertFailureMechanismAssemblyCategoryControl("IIIt", "1/1");
                AssertAssemblyResultControl(failureMechanismsWithoutProbabilityControlName, "IIIt");
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenRefreshingAssemblyResults_ThenAssessmentSectionAssemblyResultsUpdatedToNewValues()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultTotalView())
            {
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Properties.Enabled = true;

                // Precondition
                AssertAssemblyResultControl(totalControlName, "C");
                AssertFailureMechanismAssemblyCategoryControl("IIIt", "1/1");
                AssertAssemblyResultControl(failureMechanismsWithoutProbabilityControlName, "IIIt");

                // When
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.AssembleAssessmentSectionCategoryGroupOutput = AssessmentSectionAssemblyCategoryGroup.A;
                calculator.AssembleFailureMechanismsAssemblyOutput = new FailureMechanismAssembly(0.5, FailureMechanismAssemblyCategoryGroup.IIt);
                calculator.AssembleFailureMechanismsAssemblyCategoryGroupOutput = FailureMechanismAssemblyCategoryGroup.IVt;

                buttonTester.Click();

                // Then
                AssertAssemblyResultControl(totalControlName, "A");
                AssertFailureMechanismAssemblyCategoryControl("IIt", "1/2");
                AssertAssemblyResultControl(failureMechanismsWithoutProbabilityControlName, "IVt");
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenRefreshingAssemblyResultsThrowsException_ThenAssessmentSectionAssemblyResultsClearedAndErrorSet()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultTotalView())
            {
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Properties.Enabled = true;

                // Precondition
                AssertAssemblyResultControl(totalControlName, "C");
                AssertFailureMechanismAssemblyCategoryControl("IIIt", "1/1");
                AssertAssemblyResultControl(failureMechanismsWithoutProbabilityControlName, "IIIt");

                // When
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                buttonTester.Click();

                // Then
                AssertAssemblyResultWithoutProbabilityControlWithError(totalControlName);
                AssertFailureMechanismAssemblyWithProbabilityControlWithError();
                AssertAssemblyResultWithoutProbabilityControlWithError(failureMechanismsWithoutProbabilityControlName);
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalViewAndErrorOnAssemblyResult_WhenRefreshingAssemblyResultsWithoutException_ThenAssessmentSectionAssemblyResultsSetAndErrorCleared()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                using (ShowAssemblyResultTotalView())
                {
                    ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                    buttonTester.Properties.Enabled = true;

                    // Precondition
                    AssertAssemblyResultWithoutProbabilityControlWithError(totalControlName);
                    AssertFailureMechanismAssemblyWithProbabilityControlWithError();
                    AssertAssemblyResultWithoutProbabilityControlWithError(failureMechanismsWithoutProbabilityControlName);

                    // When
                    calculator.ThrowExceptionOnCalculate = false;
                    buttonTester.Click();

                    // Then
                    AssertAssemblyResultControl(totalControlName, "C");
                    Assert.IsEmpty(GetError(GetAssemblyResultControl(totalControlName)));

                    AssertFailureMechanismAssemblyCategoryControl("IIIt", "1/1");
                    Assert.IsEmpty(GetError(GetFailureMechanismAssemblyControl()));

                    AssertAssemblyResultControl(failureMechanismsWithoutProbabilityControlName, "IIIt");
                    Assert.IsEmpty(GetError(GetAssemblyResultControl(failureMechanismsWithoutProbabilityControlName)));
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
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Properties.Enabled = true;

                var invalidated = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Invalidated += (sender, args) => invalidated = true;
                object dataSource = dataGridView.DataSource;

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

                buttonTester.Click();

                // Then
                Assert.AreSame(dataSource, dataGridView.DataSource);
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
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Properties.Enabled = true;
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
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView(assessmentSection))
            {
                // Precondition
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                Button button = buttonTester.Properties;
                Assert.IsFalse(button.Enabled);
                ErrorProvider warningProvider = GetWarningProvider(view);
                Assert.IsEmpty(warningProvider.GetError(button));

                // When
                IEnumerable<IFailureMechanism> failureMechanisms = assessmentSection.GetFailureMechanisms();
                failureMechanisms.ElementAt(random.Next(failureMechanisms.Count())).NotifyObservers();

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

        [Test]
        [SetCulture("nl-NL")]
        public void GivenAssessmentSectionObserversNotified_WhenRefreshingAssemblyResults_ThenDataGridViewDataSourceUpdated()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView())
            {
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Properties.Enabled = true;

                DataGridView dataGridView = GetDataGridView();
                object dataSource = dataGridView.DataSource;

                // Precondition
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(view.AssessmentSection.GetFailureMechanisms().Count(), rows.Count);
                AssertAssemblyCells(view.AssessmentSection.Piping, new FailureMechanismAssembly(1, FailureMechanismAssemblyCategoryGroup.IIIt), rows[0].Cells);

                // When
                view.AssessmentSection.Piping = new TestPipingFailureMechanism
                {
                    IsRelevant = false
                };
                view.AssessmentSection.NotifyObservers();
                buttonTester.Click();

                // Then
                Assert.AreNotSame(dataSource, dataGridView.DataSource);
                Assert.AreEqual(view.AssessmentSection.GetFailureMechanisms().Count(), rows.Count);
                AssertAssemblyCells(view.AssessmentSection.Piping, FailureMechanismAssemblyCategoryGroup.NotApplicable, rows[0].Cells);
            }
        }

        #region Use Manual Assembly

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenPipingHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyProbability, calculator.ManualAssemblyProbabilityInput);
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenMacroStabilityInwardsHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            MacroStabilityInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyProbability, calculator.ManualAssemblyProbabilityInput);
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenGrassCoverErosionInwardsHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyProbability, calculator.ManualAssemblyProbabilityInput);
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenClosingStructuresHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyProbability, calculator.ManualAssemblyProbabilityInput);
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenHeightStructuresHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyProbability, calculator.ManualAssemblyProbabilityInput);
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenStabilityPointStructuresHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyProbability, calculator.ManualAssemblyProbabilityInput);
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenGrassCoverErosionOutwardsHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            GrassCoverErosionOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenDuneErosionHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenStabilityStoneCoverHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            StabilityStoneCoverFailureMechanism failureMechanism = assessmentSection.StabilityStoneCover;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            StabilityStoneCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenWaveImpactAsphaltCoverHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaveImpactAsphaltCover;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenGrassCoverSlipOffInwardsHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                    calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenGrassCoverSlipOffOutwardsHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                    calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenMacroStabilityOutwardsHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            MacroStabilityOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                    calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenMicrostabilityHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                    calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenPipingStructureHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                    calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenStrengthStabilityLengthwiseConstructionHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                    calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenTechnicalInnovationHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            TechnicalInnovationFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                    calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenWaterPressureAsphaltCoverHasManualAssembly_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            WaterPressureAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaterPressureAsphaltCover;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // When
                using (ShowAssemblyResultTotalView(assessmentSection))
                {
                    // Then
                    Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                    calculator.FailureMechanismSectionCategories.Single());
                }
            }
        }

        private static AssessmentSection CreateIrrelevantAssessmentSection()
        {
            var random = new Random(39);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm => fm.IsRelevant = false);
            return assessmentSection;
        }

        #endregion

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
            return new ButtonTester("refreshAssemblyResultsButton", testForm);
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private static AssemblyResultControl GetAssemblyResultControl(string controlName)
        {
            return (AssemblyResultControl) new ControlTester(controlName).TheObject;
        }

        private static FailureMechanismAssemblyControl GetFailureMechanismAssemblyControl()
        {
            return (FailureMechanismAssemblyControl) new ControlTester(failureMechanismsWithProbabilityControlName).TheObject;
        }

        private static BorderedLabel GetGroupLabel(AssemblyResultControl control)
        {
            return (BorderedLabel) ((TableLayoutPanel) control.Controls["GroupPanel"]).GetControlFromPosition(0, 0);
        }

        private static BorderedLabel GetProbabilityLabel(FailureMechanismAssemblyControl control)
        {
            return (BorderedLabel) ((TableLayoutPanel) control.Controls["probabilityPanel"]).GetControlFromPosition(0, 0);
        }

        private static string GetError(AssemblyResultControl resultControl)
        {
            var errorProvider = TypeUtils.GetField<ErrorProvider>(resultControl, "errorProvider");
            return errorProvider.GetError(resultControl);
        }

        private static ErrorProvider GetWarningProvider(AssemblyResultTotalView resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "warningProvider");
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
                            cells[failureMechanismProbabilityColumnIndex].FormattedValue);
        }

        private static void AssertAssemblyCells(IFailureMechanism failureMechanism,
                                                FailureMechanismAssemblyCategoryGroup categoryGroup,
                                                DataGridViewCellCollection cells)
        {
            AssertAssemblyCells(failureMechanism, cells);

            Assert.AreEqual(categoryGroup, cells[failureMechanismAssemblyCategoryColumnIndex].Value);
            Assert.AreEqual("-", cells[failureMechanismProbabilityColumnIndex].FormattedValue);
        }

        #endregion

        #region Asserts total assembly control

        private static void AssertFailureMechanismAssemblyWithProbabilityControlWithError()
        {
            Assert.AreEqual("Message", GetError(GetFailureMechanismAssemblyControl()));
            AssertFailureMechanismAssemblyCategoryControl(string.Empty, "-");
        }

        private static void AssertAssemblyResultWithoutProbabilityControlWithError(string controlName)
        {
            Assert.AreEqual("Message", GetError(GetAssemblyResultControl(controlName)));
            AssertAssemblyResultControl(totalControlName, string.Empty);
        }

        private static void AssertAssemblyResultControl(string controlName, string expectedGroup)
        {
            AssemblyResultControl control = GetAssemblyResultControl(controlName);
            Assert.AreEqual(expectedGroup, GetGroupLabel(control).Text);
        }

        private static void AssertFailureMechanismAssemblyCategoryControl(string expectedGroup, string expectedProbability)
        {
            FailureMechanismAssemblyControl control = GetFailureMechanismAssemblyControl();
            Assert.AreEqual(expectedGroup, GetGroupLabel(control).Text);
            Assert.AreEqual(expectedProbability, GetProbabilityLabel(control).Text);
        }

        #endregion
    }
}