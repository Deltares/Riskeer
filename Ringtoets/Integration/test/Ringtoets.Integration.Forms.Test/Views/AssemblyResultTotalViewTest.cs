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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
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
            TestDelegate call = () => new AssemblyResultTotalView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAssessmentSection_ExpectedValuesSet()
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
                Assert.AreEqual(2, view.Controls.Count);

                var button = (Button) new ControlTester("RefreshAssemblyResultsButton").TheObject;
                Assert.AreEqual("Assemblageresultaat verversen", button.Text);
                Assert.IsTrue(button.Enabled);

                var datagridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl").TheObject;
                Assert.AreEqual(DockStyle.Fill, datagridViewControl.Dock);

                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }

        [Test]
        public void GivenWithAssemblyResultTotalView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (ShowAssemblyResultTotalView())
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
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
        public void GivenFormWithAssemblyResultTotalView_ThenExpectedRowsVisible()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;

                using (AssemblyResultTotalView view = ShowAssemblyResultTotalView())
                {
                    // Then
                    var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                    AssertFailureMechanismRows(view.AssessmentSection,
                                               calculator.FailureMechanismAssemblyOutput,
                                               calculator.FailureMechanismAssemblyCategoryGroupOutput.Value,
                                               dataGridView.Rows);
                }
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenFormWithAssemblyResultTotalView_WhenRefreshingAssemblyResultsAndNoExceptionThrown_ThenRowsUpdatedToNewValues()
        {
            // Given
            var random = new Random(21);

            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Precondition
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
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
                var buttonTester = new ButtonTester("RefreshAssemblyResultsButton", testForm);
                buttonTester.Click();

                // Then 
                AssertFailureMechanismRows(view.AssessmentSection, newAssemblyResult, newCategoryGroup, rows);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GivenFormWithAssemblyResultTotalViewWithoutErrors_WhenRefreshingAssemblyResultsAndExceptionThrown_ThenErrorTextSet()
        {
            // Given
            using (new AssemblyToolCalculatorFactoryConfig())
            using (AssemblyResultTotalView view = ShowAssemblyResultTotalView())
            {
                testForm.Controls.Add(view);
                testForm.Show();

                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Precondition
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                AssertFailureMechanismRowsWithoutErrorText(view.AssessmentSection, rows);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                const string exceptionMessage = "Message";
                var buttonTester = new ButtonTester("RefreshAssemblyResultsButton", testForm);
                buttonTester.Click();

                // Then 
                AssertFailureMechanismRowsWithErrorText(view.AssessmentSection, exceptionMessage, rows);
            }
        }

        [Test]
        public void GivenAssemblyResultTotalView_WhenRefreshingAssemblyResults_ThenDataGridViewInvalidated()
        {
            // Given
            using (ShowAssemblyResultTotalView())
            {
                var invalidated = false;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                dataGridView.Invalidated += (sender, args) => invalidated = true;

                // Precondition
                Assert.IsFalse(invalidated);

                var buttonTester = new ButtonTester("RefreshAssemblyResultsButton", testForm);

                // When
                buttonTester.Click();

                // Then
                Assert.IsTrue(invalidated);
            }
        }

        private static void AssertFailureMechanismRows(AssessmentSection assessmentSection,
                                                       FailureMechanismAssembly assemblyOutput,
                                                       FailureMechanismAssemblyCategoryGroup assemblyCategoryGroup,
                                                       DataGridViewRowCollection rows)
        {
            Assert.AreEqual(assessmentSection.GetFailureMechanisms().Count(), rows.Count);

            PipingFailureMechanism piping = assessmentSection.Piping;
            AssertAssemblyRow(piping, assemblyOutput, rows[0].Cells);

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = assessmentSection.GrassCoverErosionInwards;
            AssertAssemblyRow(grassCoverErosionInwards, assemblyOutput, rows[1].Cells);

            MacroStabilityInwardsFailureMechanism macroStabilityInwards = assessmentSection.MacroStabilityInwards;
            AssertAssemblyRow(macroStabilityInwards, assemblyOutput, rows[2].Cells);

            MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = assessmentSection.MacroStabilityOutwards;
            AssertAssemblyRow(macroStabilityOutwards, assemblyCategoryGroup, rows[3].Cells);

            MicrostabilityFailureMechanism microStability = assessmentSection.Microstability;
            AssertAssemblyRow(microStability, assemblyCategoryGroup, rows[4].Cells);

            StabilityStoneCoverFailureMechanism stabilityStoneCover = assessmentSection.StabilityStoneCover;
            AssertAssemblyRow(stabilityStoneCover, assemblyCategoryGroup, rows[5].Cells);

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = assessmentSection.WaveImpactAsphaltCover;
            AssertAssemblyRow(waveImpactAsphaltCover, assemblyCategoryGroup, rows[6].Cells);

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover = assessmentSection.WaterPressureAsphaltCover;
            AssertAssemblyRow(waterPressureAsphaltCover, assemblyCategoryGroup, rows[7].Cells);

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = assessmentSection.GrassCoverErosionOutwards;
            AssertAssemblyRow(grassCoverErosionOutwards, assemblyCategoryGroup, rows[8].Cells);

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = assessmentSection.GrassCoverSlipOffOutwards;
            AssertAssemblyRow(grassCoverSlipOffOutwards, assemblyCategoryGroup, rows[9].Cells);

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = assessmentSection.GrassCoverSlipOffInwards;
            AssertAssemblyRow(grassCoverSlipOffInwards, assemblyCategoryGroup, rows[10].Cells);

            HeightStructuresFailureMechanism heightStructures = assessmentSection.HeightStructures;
            AssertAssemblyRow(heightStructures, assemblyOutput, rows[11].Cells);

            ClosingStructuresFailureMechanism closingStructures = assessmentSection.ClosingStructures;
            AssertAssemblyRow(closingStructures, assemblyOutput, rows[12].Cells);

            PipingStructureFailureMechanism pipingStructure = assessmentSection.PipingStructure;
            AssertAssemblyRow(pipingStructure, assemblyCategoryGroup, rows[13].Cells);

            StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSection.StabilityPointStructures;
            AssertAssemblyRow(stabilityPointStructures, assemblyOutput, rows[14].Cells);

            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction = assessmentSection.StrengthStabilityLengthwiseConstruction;
            AssertAssemblyRow(strengthStabilityLengthwiseConstruction, assemblyCategoryGroup, rows[15].Cells);

            DuneErosionFailureMechanism duneErosion = assessmentSection.DuneErosion;
            AssertAssemblyRow(duneErosion, assemblyCategoryGroup, rows[16].Cells);

            TechnicalInnovationFailureMechanism technicalInnovation = assessmentSection.TechnicalInnovation;
            AssertAssemblyRow(technicalInnovation, assemblyCategoryGroup, rows[17].Cells);
        }

        private static void AssertFailureMechanismRowsWithoutErrorText(AssessmentSection assessmentSection,
                                                                       DataGridViewRowCollection rows)
        {
            Assert.AreEqual(assessmentSection.GetFailureMechanisms().Count(), rows.Count);

            for (var i = 0; i < rows.Count; i++)
            {
                DataGridViewCell categoryGroupCell = GetCategoryGroupCellFromRow(rows[i].Cells);
                Assert.IsEmpty(categoryGroupCell.ErrorText);
            }
        }

        private static void AssertFailureMechanismRowsWithErrorText(AssessmentSection assessmentSection,
                                                                    string errorText,
                                                                    DataGridViewRowCollection rows)
        {
            Assert.AreEqual(assessmentSection.GetFailureMechanisms().Count(), rows.Count);

            for (var i = 0; i < rows.Count; i++)
            {
                DataGridViewCell categoryGroupCell = GetCategoryGroupCellFromRow(rows[i].Cells);
                Assert.AreEqual(errorText, categoryGroupCell.ErrorText);
            }
        }

        private static void AssertAssemblyRow(IFailureMechanism failureMechanism, DataGridViewCellCollection row)
        {
            Assert.AreEqual(expectedColumnCount, row.Count);

            Assert.AreEqual(failureMechanism.Name, row[failureMechanismNameColumnIndex].Value);
            Assert.AreEqual(failureMechanism.Code, row[failureMechanismCodeColumnIndex].Value);
            Assert.AreEqual(failureMechanism.Group, row[failureMechanismGroupColumnIndex].Value);
        }

        private static void AssertAssemblyRow(IFailureMechanism failureMechanism,
                                              FailureMechanismAssembly assemblyOutput,
                                              DataGridViewCellCollection row)
        {
            AssertAssemblyRow(failureMechanism, row);

            AssertCategoryGroupCell(assemblyOutput.Group, row);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(assemblyOutput.Probability),
                            row[failureMechanisProbabilityColumnIndex].FormattedValue);
        }

        private static void AssertAssemblyRow(IFailureMechanism failureMechanism,
                                              FailureMechanismAssemblyCategoryGroup categoryGroup,
                                              DataGridViewCellCollection row)
        {
            AssertAssemblyRow(failureMechanism, row);

            AssertCategoryGroupCell(categoryGroup, row);
            Assert.AreEqual("-", row[failureMechanisProbabilityColumnIndex].FormattedValue);
        }

        private static void AssertCategoryGroupCell(FailureMechanismAssemblyCategoryGroup categoryGroup, DataGridViewCellCollection row)
        {
            DataGridViewCell categoryColumnCell = GetCategoryGroupCellFromRow(row);

            Assert.AreEqual(categoryGroup, categoryColumnCell.Value);
            Assert.IsTrue(categoryColumnCell.ReadOnly);
            Assert.IsEmpty(categoryColumnCell.ErrorText);

            DataGridViewCellStyle categoryColumnCellStyle = categoryColumnCell.Style;
            Assert.AreEqual(AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(categoryGroup),
                            categoryColumnCellStyle.BackColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), categoryColumnCellStyle.ForeColor);
        }

        private static DataGridViewCell GetCategoryGroupCellFromRow(DataGridViewCellCollection rowCells)
        {
            return rowCells[failureMechanismAssemblyCategoryColumnIndex];
        }

        private AssemblyResultTotalView ShowAssemblyResultTotalView()
        {
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            var view = new AssemblyResultTotalView(assessmentSection);
            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}