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
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.TestUtil;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyResultPerSectionViewTest
    {
        private const int sectionStartColumnIndex = 0;
        private const int sectionEndColumnIndex = 1;
        private const int sectionTotalAssemblyResultColumnIndex = 2;
        private const int pipingColumnIndex = 3;
        private const int grassCoverErosionInwardsColumnIndex = 4;
        private const int macroStabilityInwardsColumnIndex = 5;
        private const int macroStabilityOutwardsColumnIndex = 6;
        private const int microStabilityColumnIndex = 7;
        private const int stabilityStoneCoverColumnIndex = 8;
        private const int waveImpactAsphaltCoverColumnIndex = 9;
        private const int waterPressureAsphaltCoverColumnIndex = 10;
        private const int grassCoverErosionOutwardsColumnIndex = 11;
        private const int grassCoverSlipOffOutwardsColumnIndex = 12;
        private const int grassCoverSlipOffInwardsColumnIndex = 13;
        private const int heightStructuresColumnIndex = 14;
        private const int closingStructures = 15;
        private const int pipingStructures = 16;
        private const int stabilityPointStructuresColumnIndex = 17;
        private const int strengthStabilityLengthwiseColumnIndex = 18;
        private const int duneErosionColumnIndex = 19;
        private const int technicalInnovationColumnIndex = 20;
        private const int expectedColumnCount = 21;
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
            TestDelegate call = () => new AssemblyResultPerSectionView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
                Assert.AreEqual("Assemblageresultaat verversen", button.Text);
                Assert.IsFalse(button.Enabled);

                ErrorProvider errorProvider = GetErrorProvider(view);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ErrorIcon.ToBitmap(), errorProvider.Icon.ToBitmap());
                Assert.AreEqual(ErrorBlinkStyle.NeverBlink, errorProvider.BlinkStyle);
                Assert.IsEmpty(errorProvider.GetError(button));
                Assert.AreEqual(4, errorProvider.GetIconPadding(button));

                ErrorProvider warningProvider = GetWarningProvider(view);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.warning.ToBitmap(), warningProvider.Icon.ToBitmap());
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
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultPerSectionView())
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreEqual(expectedColumnCount, dataGridView.ColumnCount);

                DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;

                AssertColumn(dataGridViewColumns[sectionStartColumnIndex], "Kilometrering* van [km]");
                AssertColumn(dataGridViewColumns[sectionEndColumnIndex], "Kilometrering* tot [km]");
                AssertColumn(dataGridViewColumns[sectionTotalAssemblyResultColumnIndex], "Totaal vakoordeel");
                AssertColumn(dataGridViewColumns[pipingColumnIndex], "STPH");
                AssertColumn(dataGridViewColumns[grassCoverErosionInwardsColumnIndex], "GEKB");
                AssertColumn(dataGridViewColumns[macroStabilityInwardsColumnIndex], "STBI");
                AssertColumn(dataGridViewColumns[macroStabilityOutwardsColumnIndex], "STBU");
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
                AssertColumn(dataGridViewColumns[strengthStabilityLengthwiseColumnIndex], "STKWl");
                AssertColumn(dataGridViewColumns[duneErosionColumnIndex], "DA");
                AssertColumn(dataGridViewColumns[technicalInnovationColumnIndex], "INN");
            }
        }

        [Test]
        public void Constructor_AssessmentSectionWithReferenceLine_ExpectedValues()
        {
            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultPerSectionView())
            {
                DataGridView dataGridView = GetDataGridView();
                DataGridViewRowCollection rows = dataGridView.Rows;

                // Assert
                Assert.AreEqual(1, rows.Count);
            }
        }

        [Test]
        public void Constructor_ReferenceLineNull_ExpectedValues()
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
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowAssemblyResultPerSectionView())
            {
                ButtonTester buttonTester = GetRefreshAssemblyResultButtonTester();
                buttonTester.Properties.Enabled = true;

                DataGridView dataGridView = GetDataGridView();
                dataGridView.CellFormatting += (sender, args) =>
                {
                    var row = (IHasColumnStateDefinitions) dataGridView.Rows[0].DataBoundItem;
                    DataGridViewColumnStateDefinition definition = row.ColumnStateDefinitions[sectionTotalAssemblyResultColumnIndex];
                    definition.ReadOnly = readOnly;
                    definition.ErrorText = errorText;
                    definition.Style = style;
                };

                // When
                buttonTester.Click();

                // Then
                DataGridViewCell cell = dataGridView.Rows[0].Cells[sectionTotalAssemblyResultColumnIndex];
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
            var calculation = new TestHeightStructuresCalculation();
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
                var calculatorfactory = (TestAssemblyToolCalculatorFactory )AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
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

        private AssemblyResultPerSectionView ShowAssemblyResultPerSectionView()
        {
            return ShowAssemblyResultPerSectionView(TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                                                        new Random(21).NextEnumValue<AssessmentSectionComposition>()));
        }

        private AssemblyResultPerSectionView ShowAssemblyResultPerSectionView(AssessmentSection assessmentSection)
        {
            var view = new AssemblyResultPerSectionView(assessmentSection);
            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}