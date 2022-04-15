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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Forms.Controls;
using Riskeer.Integration.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;
using CoreGuiResources = Core.Gui.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyResultTotalViewTest
    {
        private const int expectedColumnCount = 3;
        private const int failureMechanismNameColumnIndex = 0;
        private const int failureMechanismCodeColumnIndex = 1;
        private const int failureMechanismProbabilityColumnIndex = 2;
        private const string assemblyResultOutdatedWarning = "De resultaten zijn verouderd. Druk op de \"Resultaten verversen\" knop om opnieuw te berekenen.";
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
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssemblyResultTotalView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
                Assert.AreEqual(4, view.Controls.Count);

                Button button = GetRefreshAssemblyResultButtonTester().Properties;
                Assert.AreEqual("Resultaten verversen", button.Text);
                Assert.IsFalse(button.Enabled);

                var assemblyResultControl = (AssessmentSectionAssemblyResultControl) new ControlTester("assessmentSectionAssemblyControl").TheObject;
                Assert.AreEqual(DockStyle.Top, assemblyResultControl.Dock);

                var label = (Label) new ControlTester("label").TheObject;
                Assert.AreEqual("Samenvatting resultaten per faalmechanisme:", label.Text);

                var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl").TheObject;
                Assert.AreEqual(DockStyle.Fill, dataGridViewControl.Dock);

                ErrorProvider warningProvider = GetWarningProvider(view);
                TestHelper.AssertImagesAreEqual(CoreGuiResources.warning.ToBitmap(), warningProvider.Icon.ToBitmap());
                Assert.AreEqual(ErrorBlinkStyle.NeverBlink, warningProvider.BlinkStyle);
                Assert.IsEmpty(warningProvider.GetError(button));
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
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismProbabilityColumnIndex]);

                Assert.AreEqual("Faalmechanisme", dataGridViewColumns[failureMechanismNameColumnIndex].HeaderText);
                Assert.AreEqual("Label", dataGridViewColumns[failureMechanismCodeColumnIndex].HeaderText);
                Assert.AreEqual("Faalkans [1/jaar]", dataGridViewColumns[failureMechanismProbabilityColumnIndex].HeaderText);

                Assert.IsTrue(dataGridViewColumns[failureMechanismNameColumnIndex].ReadOnly);
                Assert.IsTrue(dataGridViewColumns[failureMechanismCodeColumnIndex].ReadOnly);
                Assert.IsTrue(dataGridViewColumns[failureMechanismProbabilityColumnIndex].ReadOnly);
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
                                               calculator.AssemblyResult,
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
                AssertAssessmentSectionAssemblyResultControl("A+", "1/7");
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
                AssertAssessmentSectionAssemblyResultControl("A+", "1/7");

                // When
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.AssessmentSectionAssemblyResult = new AssessmentSectionAssemblyResult(0.5, AssessmentSectionAssemblyGroup.A);

                buttonTester.Click();

                // Then
                AssertAssessmentSectionAssemblyResultControl("A", "1/2");
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
                AssertAssessmentSectionAssemblyResultControl("A+", "1/7");

                // When
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                buttonTester.Click();

                // Then
                AssertAssessmentSectionAssemblyResultControlWithError();
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
                    AssertAssessmentSectionAssemblyResultControlWithError();

                    // When
                    calculator.ThrowExceptionOnCalculate = false;
                    buttonTester.Click();

                    // Then
                    AssertAssessmentSectionAssemblyResultControl("A+", "1/7");
                    Assert.IsEmpty(GetProbabilityError(GetAssessmentSectionAssemblyResultControl()));
                    Assert.IsEmpty(GetGroupError(GetAssessmentSectionAssemblyResultControl()));
                }
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalViewWithOutdatedContent_WhenRefreshingAssemblyResults_ThenRefreshButtonDisabledAndWarningCleared()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

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
        public void GivenFormWithAssemblyResultTotalView_WhenSpecificFailureMechanismCollectionNotifiesObservers_ThenRefreshButtonEnabledAndWarningSet()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView(assessmentSection))
            {
                // Precondition
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                Button button = buttonTester.Properties;
                Assert.IsFalse(button.Enabled);
                ErrorProvider warningProvider = GetWarningProvider(view);
                Assert.IsEmpty(warningProvider.GetError(button));

                // When
                assessmentSection.SpecificFailureMechanisms.NotifyObservers();

                // Then 
                Assert.IsTrue(buttonTester.Properties.Enabled);
                Assert.AreEqual(assemblyResultOutdatedWarning, warningProvider.GetError(button));
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenSpecificFailureMechanismNotifiesObservers_ThenRefreshButtonEnabledAndWarningSet()
        {
            // Given
            var failureMechanism = new SpecificFailureMechanism();
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.SpecificFailureMechanisms.Add(failureMechanism);

            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView(assessmentSection))
            {
                // Precondition
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                Button button = buttonTester.Properties;
                Assert.IsFalse(button.Enabled);
                ErrorProvider warningProvider = GetWarningProvider(view);
                Assert.IsEmpty(warningProvider.GetError(button));

                // When
                failureMechanism.NotifyObservers();

                // Then 
                Assert.IsTrue(buttonTester.Properties.Enabled);
                Assert.AreEqual(assemblyResultOutdatedWarning, warningProvider.GetError(button));
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultTotalView_WhenAssessmentSectionNotifiesObservers_ThenRefreshButtonEnabledAndWarningSet()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

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
            AssessmentSection assessmentSection = CreateAssessmentSection();

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
            AssessmentSection assessmentSection = CreateAssessmentSection();
            var calculation = new TestHeightStructuresCalculationScenario();
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
                AssertAssemblyCells(view.AssessmentSection.Piping, 0, rows[0].Cells);

                // When
                view.AssessmentSection.Piping = new TestPipingFailureMechanism
                {
                    InAssembly = false
                };
                view.AssessmentSection.NotifyObservers();
                buttonTester.Click();

                // Then
                Assert.AreNotSame(dataSource, dataGridView.DataSource);
                Assert.AreEqual(view.AssessmentSection.GetFailureMechanisms().Count(), rows.Count);
                AssertAssemblyCells(view.AssessmentSection.Piping, double.NaN, rows[0].Cells);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenFormWithAssemblyResultTotalView_WhenSpecificFailureMechanismAddedAndRefreshingAssemblyResults_ThenDataGridViewDataSourceAndRowsUpdated()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView(assessmentSection))
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                DataGridView dataGridView = GetDataGridView();
                object dataSource = dataGridView.DataSource;

                // Precondition
                DataGridViewRowCollection rows = dataGridView.Rows;
                AssertFailureMechanismRows(view.AssessmentSection, calculator.AssemblyResult, rows);

                // When
                ObservableList<SpecificFailureMechanism> specificFailureMechanisms = assessmentSection.SpecificFailureMechanisms;
                specificFailureMechanisms.Add(new SpecificFailureMechanism());
                specificFailureMechanisms.NotifyObservers();

                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Click();

                // Then
                Assert.AreNotSame(dataSource, dataGridView.DataSource);
                AssertFailureMechanismRows(view.AssessmentSection, calculator.AssemblyResult, rows);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenFormWithAssemblyResultTotalView_WhenSpecificFailureMechanismRemovedAndRefreshingAssemblyResults_ThenDataGridViewDataSourceAndRowsUpdated()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView(assessmentSection))
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                DataGridView dataGridView = GetDataGridView();
                object dataSource = dataGridView.DataSource;

                // Precondition
                DataGridViewRowCollection rows = dataGridView.Rows;
                AssertFailureMechanismRows(view.AssessmentSection, calculator.AssemblyResult, rows);

                // When
                ObservableList<SpecificFailureMechanism> specificFailureMechanisms = assessmentSection.SpecificFailureMechanisms;
                SpecificFailureMechanism failureMechanismToRemove = specificFailureMechanisms.Last();
                specificFailureMechanisms.Remove(failureMechanismToRemove);
                specificFailureMechanisms.NotifyObservers();

                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Click();

                // Then
                Assert.AreNotSame(dataSource, dataGridView.DataSource);
                AssertFailureMechanismRows(view.AssessmentSection, calculator.AssemblyResult, rows);
            }
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());
            assessmentSection.SpecificFailureMechanisms.AddRange(new[]
            {
                new SpecificFailureMechanism(),
                new SpecificFailureMechanism()
            });
            return assessmentSection;
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
            return new ButtonTester("refreshAssemblyResultsButton", testForm);
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private static BorderedLabel GetGroupLabel(AssessmentSectionAssemblyResultControl resultControl)
        {
            return (BorderedLabel) GetResultPanel(resultControl).GetControlFromPosition(1, 0);
        }

        private static BorderedLabel GetProbabilityLabel(AssessmentSectionAssemblyResultControl resultControl)
        {
            return (BorderedLabel) GetResultPanel(resultControl).GetControlFromPosition(1, 1);
        }

        private static TableLayoutPanel GetResultPanel(AssessmentSectionAssemblyResultControl resultControl)
        {
            return (TableLayoutPanel) resultControl.Controls["resultLayoutPanel"];
        }

        private static ErrorProvider GetErrorProvider(AssessmentSectionAssemblyResultControl resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "errorProvider");
        }

        private static string GetProbabilityError(AssessmentSectionAssemblyResultControl resultControl)
        {
            ErrorProvider errorProvider = GetErrorProvider(resultControl);
            return errorProvider.GetError(GetProbabilityLabel(resultControl));
        }

        private static string GetGroupError(AssessmentSectionAssemblyResultControl resultControl)
        {
            ErrorProvider errorProvider = GetErrorProvider(resultControl);
            return errorProvider.GetError(GetGroupLabel(resultControl));
        }

        private static ErrorProvider GetWarningProvider(AssemblyResultTotalView resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "warningProvider");
        }

        #endregion

        #region Asserts datagrid control

        private static void AssertFailureMechanismRows(AssessmentSection assessmentSection,
                                                       double assemblyOutput,
                                                       DataGridViewRowCollection rows)
        {
            int nrOfExpectedRows = assessmentSection.GetFailureMechanisms().Count() +
                                   assessmentSection.SpecificFailureMechanisms.Count;
            Assert.AreEqual(nrOfExpectedRows, rows.Count);

            PipingFailureMechanism piping = assessmentSection.Piping;
            AssertAssemblyCells(piping, assemblyOutput, rows[0].Cells);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = assessmentSection.GrassCoverErosionInwards;
            AssertAssemblyCells(grassCoverErosionInwards, assemblyOutput, rows[1].Cells);

            MacroStabilityInwardsFailureMechanism macroStabilityInwards = assessmentSection.MacroStabilityInwards;
            AssertAssemblyCells(macroStabilityInwards, assemblyOutput, rows[2].Cells);

            MicrostabilityFailureMechanism microStability = assessmentSection.Microstability;
            AssertAssemblyCells(microStability, assemblyOutput, rows[3].Cells);

            StabilityStoneCoverFailureMechanism stabilityStoneCover = assessmentSection.StabilityStoneCover;
            AssertAssemblyCells(stabilityStoneCover, assemblyOutput, rows[4].Cells);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = assessmentSection.WaveImpactAsphaltCover;
            AssertAssemblyCells(waveImpactAsphaltCover, assemblyOutput, rows[5].Cells);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover = assessmentSection.WaterPressureAsphaltCover;
            AssertAssemblyCells(waterPressureAsphaltCover, assemblyOutput, rows[6].Cells);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = assessmentSection.GrassCoverErosionOutwards;
            AssertAssemblyCells(grassCoverErosionOutwards, assemblyOutput, rows[7].Cells);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = assessmentSection.GrassCoverSlipOffOutwards;
            AssertAssemblyCells(grassCoverSlipOffOutwards, assemblyOutput, rows[8].Cells);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = assessmentSection.GrassCoverSlipOffInwards;
            AssertAssemblyCells(grassCoverSlipOffInwards, assemblyOutput, rows[9].Cells);

            HeightStructuresFailureMechanism heightStructures = assessmentSection.HeightStructures;
            AssertAssemblyCells(heightStructures, assemblyOutput, rows[10].Cells);

            ClosingStructuresFailureMechanism closingStructures = assessmentSection.ClosingStructures;
            AssertAssemblyCells(closingStructures, assemblyOutput, rows[11].Cells);

            PipingStructureFailureMechanism pipingStructure = assessmentSection.PipingStructure;
            AssertAssemblyCells(pipingStructure, assemblyOutput, rows[12].Cells);

            StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSection.StabilityPointStructures;
            AssertAssemblyCells(stabilityPointStructures, assemblyOutput, rows[13].Cells);

            DuneErosionFailureMechanism duneErosion = assessmentSection.DuneErosion;
            AssertAssemblyCells(duneErosion, assemblyOutput, rows[14].Cells);

            int startIndexFailureMechanismRow = 15;
            for (int i = startIndexFailureMechanismRow; i < nrOfExpectedRows; i++)
            {
                SpecificFailureMechanism specificFailureMechanism = assessmentSection.SpecificFailureMechanisms[i - startIndexFailureMechanismRow];
                AssertAssemblyCells(specificFailureMechanism, assemblyOutput, rows[i].Cells);
            }
        }

        private static void AssertAssemblyCells(IFailureMechanism failureMechanism, double assemblyResult, DataGridViewCellCollection cells)
        {
            Assert.AreEqual(expectedColumnCount, cells.Count);

            Assert.AreEqual(failureMechanism.Name, cells[failureMechanismNameColumnIndex].Value);
            Assert.AreEqual(failureMechanism.Code, cells[failureMechanismCodeColumnIndex].Value);
            Assert.AreEqual(ProbabilityFormattingHelper.FormatWithDiscreteNumbers(assemblyResult),
                            cells[failureMechanismProbabilityColumnIndex].FormattedValue);
        }

        #endregion

        #region Asserts total assembly control

        private static void AssertAssessmentSectionAssemblyResultControlWithError()
        {
            Assert.AreEqual("Message", GetProbabilityError(GetAssessmentSectionAssemblyResultControl()));
            Assert.AreEqual("Message", GetGroupError(GetAssessmentSectionAssemblyResultControl()));
            AssertAssessmentSectionAssemblyResultControl("-", "-");
        }

        private static void AssertAssessmentSectionAssemblyResultControl(string expectedGroup, string expectedProbability)
        {
            AssessmentSectionAssemblyResultControl control = GetAssessmentSectionAssemblyResultControl();
            Assert.AreEqual(expectedGroup, GetGroupLabel(control).Text);
            Assert.AreEqual(expectedProbability, GetProbabilityLabel(control).Text);
        }

        private static AssessmentSectionAssemblyResultControl GetAssessmentSectionAssemblyResultControl()
        {
            return (AssessmentSectionAssemblyResultControl) new ControlTester("assessmentSectionAssemblyControl").TheObject;
        }

        #endregion
    }
}