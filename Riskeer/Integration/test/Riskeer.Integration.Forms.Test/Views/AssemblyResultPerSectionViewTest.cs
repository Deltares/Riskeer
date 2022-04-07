﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Views;
using Riskeer.Integration.TestUtil;
using CoreGuiResources = Core.Gui.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyResultPerSectionViewTest
    {
        private const int sectionStartColumnIndex = 0;
        private const int sectionEndColumnIndex = 1;
        private const int pipingColumnIndex = 2;
        private const int grassCoverErosionInwardsColumnIndex = 3;
        private const int macroStabilityInwardsColumnIndex = 4;
        private const int microStabilityColumnIndex = 5;
        private const int stabilityStoneCoverColumnIndex = 6;
        private const int waveImpactAsphaltCoverColumnIndex = 7;
        private const int waterPressureAsphaltCoverColumnIndex = 8;
        private const int grassCoverErosionOutwardsColumnIndex = 9;
        private const int grassCoverSlipOffOutwardsColumnIndex = 10;
        private const int grassCoverSlipOffInwardsColumnIndex = 11;
        private const int heightStructuresColumnIndex = 12;
        private const int closingStructures = 13;
        private const int pipingStructures = 14;
        private const int stabilityPointStructuresColumnIndex = 15;
        private const int duneErosionColumnIndex = 16;
        private const int specificFailureMechanismStartIndex = 17;
        private const int specificFailureMechanism2ColumnIndex = 18;
        private const int worstAssemblyResultPerSectionColumnIndex = 19;
        private const int expectedColumnCount = 20;
        private const string assemblyResultOutdatedWarning = "De resultaten zijn verouderd. Druk op de \"Resultaten verversen\" knop om opnieuw te berekenen.";

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
            void Call() => new AssemblyResultPerSectionView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAssessmentSection_ExpectedValues()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new AssemblyResultPerSectionView(assessmentSection))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreEqual(2, view.Controls.Count);

                var button = (Button) GetRefreshAssemblyResultButtonTester().TheObject;
                Assert.AreEqual("Resultaten verversen", button.Text);
                Assert.IsFalse(button.Enabled);

                ErrorProvider errorProvider = GetErrorProvider(view);
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ErrorIcon.ToBitmap(), errorProvider.Icon.ToBitmap());
                Assert.AreEqual(ErrorBlinkStyle.NeverBlink, errorProvider.BlinkStyle);
                Assert.IsEmpty(errorProvider.GetError(button));
                Assert.AreEqual(4, errorProvider.GetIconPadding(button));

                ErrorProvider warningProvider = GetWarningProvider(view);
                TestHelper.AssertImagesAreEqual(CoreGuiResources.warning.ToBitmap(), warningProvider.Icon.ToBitmap());
                Assert.AreEqual(ErrorBlinkStyle.NeverBlink, warningProvider.BlinkStyle);
                Assert.IsEmpty(warningProvider.GetError(button));
                Assert.AreEqual(4, warningProvider.GetIconPadding(button));

                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(new Size(300, 250), view.AutoScrollMinSize);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultPerSectionView_ThenExpectedColumnsAreVisible()
        {
            // Given
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultPerSectionView(assessmentSection))
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;

                AssertColumns(dataGridViewColumns, expectedColumnCount, assessmentSection.SpecificFailureMechanisms);
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultPerSectionView_ThenExpectedFailureMechanismCellData()
        {
            // Given
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultPerSectionView(assessmentSection))
            {
                DataGridView dataGridView = GetDataGridView();
                object actualFirstFailureMechanismValue = dataGridView.Rows[0].Cells[specificFailureMechanismStartIndex].Value;
                object actualSecondFailureMechanismValue = dataGridView.Rows[0].Cells[specificFailureMechanism2ColumnIndex].Value;

                // Then
                Assert.AreEqual("Do", actualFirstFailureMechanismValue);
                Assert.AreEqual("Do", actualSecondFailureMechanismValue);
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultPerSectionView_WhenSpecificFailureMechanismAdded_ThenColumnAdded()
        {
            // Given
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultPerSectionView(assessmentSection))
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Precondition
                DataGridView dataGridView = GetDataGridView();
                Assert.AreEqual(expectedColumnCount, dataGridView.ColumnCount);

                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();

                // When
                const string testCode = "TestCode";
                var failureMechanism = new SpecificFailureMechanism
                {
                    Code = testCode
                };

                assessmentSection.SpecificFailureMechanisms.Add(failureMechanism);
                assessmentSection.SpecificFailureMechanisms.NotifyObservers();
                calculator.CombinedFailureMechanismSectionAssemblyOutput = null;
                buttonTester.Click();

                // Then
                AssertColumns(dataGridView.Columns, expectedColumnCount + 1, assessmentSection.SpecificFailureMechanisms);
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultPerSectionView_WhenSpecificFailureMechanismRemoved_ThenColumnRemoved()
        {
            // Given
            var random = new Random(21);
            const string code = "TestCode";
            var failureMechanismToRemove = new SpecificFailureMechanism
            {
                Code = code
            };

            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());
            assessmentSection.SpecificFailureMechanisms.Add(failureMechanismToRemove);

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultPerSectionView(assessmentSection))
            {
                // Precondition
                DataGridView dataGridView = GetDataGridView();
                
                DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;
                AssertColumns(dataGridViewColumns, expectedColumnCount + 1, assessmentSection.SpecificFailureMechanisms);
                
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();

                // When
                assessmentSection.SpecificFailureMechanisms.Remove(failureMechanismToRemove);
                assessmentSection.NotifyObservers();
                buttonTester.Click();

                // Then
                AssertColumns(dataGridViewColumns, expectedColumnCount, assessmentSection.SpecificFailureMechanisms);
            }
        }

        [Test]
        public void Constructor_AssessmentSectionWithReferenceLine_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());
            
            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultPerSectionView(assessmentSection))
            {
                DataGridView dataGridView = GetDataGridView();
                DataGridViewRowCollection rows = dataGridView.Rows;

                // Assert
                Assert.AreEqual(1, rows.Count);
            }
        }

        [Test]
        public void Constructor_AssessmentSectionWithoutReferenceLine_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            using (ShowAssemblyResultPerSectionView(assessmentSection))
            {
                DataGridView dataGridView = GetDataGridView();
                DataGridViewRowCollection rows = dataGridView.Rows;

                // Assert
                Assert.AreEqual(0, rows.Count);
            }
        }

        [Test]
        [TestCaseSource(nameof(CellFormattingStates))]
        public void GivenFormWithAssemblyResultPerSectionView_WhenRefreshingAssemblyResults_ThenCategoryColumnSetToColumnStateDefinition(
            bool readOnly, string errorText, CellStyle style)
        {
            // Given
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultPerSectionView(assessmentSection))
            {
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Properties.Enabled = true;

                DataGridView dataGridView = GetDataGridView();
                dataGridView.CellFormatting += (sender, args) =>
                {
                    var row = (IHasColumnStateDefinitions) dataGridView.Rows[0].DataBoundItem;
                    DataGridViewColumnStateDefinition definition = row.ColumnStateDefinitions[worstAssemblyResultPerSectionColumnIndex];
                    definition.ReadOnly = readOnly;
                    definition.ErrorText = errorText;
                    definition.Style = style;
                };

                // When
                buttonTester.Click();

                // Then
                DataGridViewCell cell = dataGridView.Rows[0].Cells[worstAssemblyResultPerSectionColumnIndex];
                Assert.AreEqual(readOnly, cell.ReadOnly);
                Assert.AreEqual(errorText, cell.ErrorText);
                Assert.AreEqual(style.BackgroundColor, cell.Style.BackColor);
                Assert.AreEqual(style.TextColor, cell.Style.ForeColor);
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultPerSectionViewWithOutdatedContent_WhenRefreshingAssemblyResults_ThenRefreshButtonDisabledAndWarningCleared()
        {
            // Given
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultPerSectionView view = ShowAssemblyResultPerSectionView(assessmentSection))
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
        public void GivenFormWithAssemblyResultPerSectionView_WhenAssessmentSectionNotifiesObservers_ThenRefreshButtonEnabledAndWarningSet()
        {
            // Given
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultPerSectionView view = ShowAssemblyResultPerSectionView(assessmentSection))
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
        public void GivenFormWithAssemblyResultPerSectionView_WhenFailureMechanismNotifiesObservers_ThenRefreshButtonEnabledAndWarningSet()
        {
            // Given
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultPerSectionView view = ShowAssemblyResultPerSectionView(assessmentSection))
            {
                // Precondition
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                Button button = buttonTester.Properties;
                Assert.IsFalse(button.Enabled);
                ErrorProvider warningProvider = GetWarningProvider(view);
                Assert.IsEmpty(warningProvider.GetError(button));

                // When
                IFailureMechanism[] failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();
                failureMechanisms[random.Next(failureMechanisms.Length)].NotifyObservers();

                // Then 
                Assert.IsTrue(buttonTester.Properties.Enabled);
                Assert.AreEqual(assemblyResultOutdatedWarning, warningProvider.GetError(button));
            }
        }

        [Test]
        public void GivenFormWithAssemblyResultPerSectionView_WhenCalculationNotifiesObservers_ThenRefreshButtonEnabledAndWarningSet()
        {
            // Given
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());
            var calculation = new TestHeightStructuresCalculationScenario();
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(calculation);

            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultPerSectionView view = ShowAssemblyResultPerSectionView(assessmentSection))
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
        [TestCase(true, 24)]
        [TestCase(false, 4)]
        public void GivenFormWithAssemblyResultPerSectionView_WithOrWithoutErrorSetAndObserverNotified_ThenWarningSetWithPadding(bool withError, int expectedPadding)
        {
            // Given
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = withError;

                using (AssemblyResultPerSectionView view = ShowAssemblyResultPerSectionView(assessmentSection))
                {
                    // Precondition
                    ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                    Button button = buttonTester.Properties;
                    Assert.IsFalse(button.Enabled);
                    ErrorProvider errorProvider = GetErrorProvider(view);
                    Assert.AreEqual(withError, !string.IsNullOrEmpty(errorProvider.GetError(button)));
                    ErrorProvider warningProvider = GetWarningProvider(view);
                    Assert.IsEmpty(warningProvider.GetError(button));

                    // When
                    assessmentSection.NotifyObservers();

                    // Then
                    Assert.AreEqual(assemblyResultOutdatedWarning, warningProvider.GetError(button));
                    Assert.AreEqual(expectedPadding, warningProvider.GetIconPadding(button));
                }
            }
        }

        private ButtonTester GetRefreshAssemblyResultButtonTester()
        {
            return new ButtonTester("refreshAssemblyResultsButton", testForm);
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private static void AssertColumn(DataGridViewColumn column, string headerText)
        {
            Assert.IsInstanceOf<DataGridViewTextBoxColumn>(column);
            Assert.AreEqual(headerText, column.HeaderText);
            Assert.IsTrue(column.ReadOnly);
        }

        private static ErrorProvider GetErrorProvider(AssemblyResultPerSectionView resultView)
        {
            return TypeUtils.GetField<ErrorProvider>(resultView, "errorProvider");
        }

        private static ErrorProvider GetWarningProvider(AssemblyResultPerSectionView resultControl)
        {
            return TypeUtils.GetField<ErrorProvider>(resultControl, "warningProvider");
        }

        private AssemblyResultPerSectionView ShowAssemblyResultPerSectionView(AssessmentSection assessmentSection)
        {
            var view = new AssemblyResultPerSectionView(assessmentSection);
            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static void AssertColumns(DataGridViewColumnCollection dataGridViewColumns, int totalColumnCount, IEnumerable<SpecificFailureMechanism> specificFailureMechanisms)
        {
            Assert.AreEqual(totalColumnCount, dataGridViewColumns.Count);
            AssertColumn(dataGridViewColumns[sectionStartColumnIndex], "Metrering van* [m]");
            AssertColumn(dataGridViewColumns[sectionEndColumnIndex], "Metrering tot* [m]");
            AssertColumn(dataGridViewColumns[pipingColumnIndex], "STPH");
            AssertColumn(dataGridViewColumns[grassCoverErosionInwardsColumnIndex], "GEKB");
            AssertColumn(dataGridViewColumns[macroStabilityInwardsColumnIndex], "STBI");
            AssertColumn(dataGridViewColumns[microStabilityColumnIndex], "STMI");
            AssertColumn(dataGridViewColumns[stabilityStoneCoverColumnIndex], "ZST");
            AssertColumn(dataGridViewColumns[waveImpactAsphaltCoverColumnIndex], "AGK");
            AssertColumn(dataGridViewColumns[waterPressureAsphaltCoverColumnIndex], "AWO");
            AssertColumn(dataGridViewColumns[grassCoverErosionOutwardsColumnIndex], "GEBU");
            AssertColumn(dataGridViewColumns[grassCoverSlipOffOutwardsColumnIndex], "GABU");
            AssertColumn(dataGridViewColumns[grassCoverSlipOffInwardsColumnIndex], "GABI");
            AssertColumn(dataGridViewColumns[heightStructuresColumnIndex], "HTKW");
            AssertColumn(dataGridViewColumns[closingStructures], "BSKW");
            AssertColumn(dataGridViewColumns[pipingStructures], "PKW");
            AssertColumn(dataGridViewColumns[stabilityPointStructuresColumnIndex], "STKWp");
            AssertColumn(dataGridViewColumns[duneErosionColumnIndex], "DA");
            for (int i = 0; i < specificFailureMechanisms.Count(); i++)
            {
                AssertColumn(dataGridViewColumns[specificFailureMechanismStartIndex + i], specificFailureMechanisms.ElementAt(i).Code);
            }
            AssertColumn(dataGridViewColumns[specificFailureMechanismStartIndex + specificFailureMechanisms.Count()], "Slechtste duidingsklasse per deelvak");
        }
    }
}