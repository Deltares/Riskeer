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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentIndex = 1;
        private const int detailedAssessmentIndex = 2;
        private const int tailorMadeAssessmentProbabilityIndex = 3;
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
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => new PipingFailureMechanismResultView(new ObservableList<PipingFailureMechanismSectionResult>(),
                                                                           failureMechanism,
                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            // Call
            using (var view = new PipingFailureMechanismResultView(pipingFailureMechanism.SectionResults, pipingFailureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<PipingFailureMechanismSectionResult, PipingFailureMechanismSectionResultRow, PipingFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(pipingFailureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);
                Assert.IsTrue(dataGridView.Columns[detailedAssessmentIndex].ReadOnly);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[detailedAssessmentIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[tailorMadeAssessmentProbabilityIndex]);

                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[detailedAssessmentIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[tailorMadeAssessmentProbabilityIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();

            // Call
            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultType.None, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual("-", cells[tailorMadeAssessmentProbabilityIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultType.None, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual("-", cells[tailorMadeAssessmentProbabilityIndex].FormattedValue);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.NotApplicable)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultsView_ChangeComboBox_DataGridViewCorrectlySyncedAndStylingSet(
            SimpleAssessmentResultType simpleAssessmentType)
        {
            // Setup
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();

            // Call
            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                dataGridView.Rows[0].Cells[simpleAssessmentIndex].Value = simpleAssessmentType;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                DataGridViewCell cellDetailedAssessment = cells[detailedAssessmentIndex];
                DataGridViewCell cellTailorMadeAssessmentProbability = cells[tailorMadeAssessmentProbabilityIndex];

                Assert.AreEqual(simpleAssessmentType, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cellDetailedAssessment.FormattedValue);
                Assert.AreEqual("-", cellTailorMadeAssessmentProbability.FormattedValue);

                if (simpleAssessmentType == SimpleAssessmentResultType.ProbabilityNegligible
                    || simpleAssessmentType == SimpleAssessmentResultType.NotApplicable)
                {
                    DataGridViewTestHelper.AssertCellIsDisabled(cellDetailedAssessment);
                    DataGridViewTestHelper.AssertCellIsDisabled(cellTailorMadeAssessmentProbability);

                    Assert.IsTrue(cellTailorMadeAssessmentProbability.ReadOnly);
                }
                else
                {
                    DataGridViewTestHelper.AssertCellIsEnabled(cellDetailedAssessment, true);
                    DataGridViewTestHelper.AssertCellIsEnabled(cellTailorMadeAssessmentProbability);

                    Assert.IsFalse(cellTailorMadeAssessmentProbability.ReadOnly);
                }
            }
        }

        [Test]
        [TestCase("test", tailorMadeAssessmentProbabilityIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", tailorMadeAssessmentProbabilityIndex)]
        public void FailureMechanismResultView_EditValueInvalid_ShowsErrorTooltip(string newValue, int cellIndex)
        {
            // Setup
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[cellIndex].Value = newValue;

                // Assert
                Assert.AreEqual("De waarde kon niet geïnterpreteerd worden als een kans.", dataGridView.Rows[0].ErrorText);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1.01)]
        [TestCase(-0.01)]
        [TestCase(5)]
        [TestCase(-10)]
        public void FailureMechanismResultView_EditValueTailorMadeAssessmentProbabilityInvalid_ShowErrorToolTip(double newValue)
        {
            // Setup
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[tailorMadeAssessmentProbabilityIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.AreEqual("De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.", dataGridView.Rows[0].ErrorText);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1)]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void FailureMechanismResultView_EditValueTailorMadeAssessmentProbabilityValid_DoNotShowErrorToolTipAndEditValue(double newValue)
        {
            // Setup
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[tailorMadeAssessmentProbabilityIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
                Assert.AreEqual(newValue, pipingFailureMechanism.SectionResults.First().TailorMadeAssessmentProbability);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultView_TotalContributionNotHundred_ShowsErrorTooltip(
            SimpleAssessmentResultType simpleAssessmentType)
        {
            // Setup
            const int rowIndex = 0;
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            PipingCalculationScenario calculationScenario = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenario(
                pipingFailureMechanism.Sections.First());
            calculationScenario.Contribution = (RoundedDouble) 0.3;
            pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[detailedAssessmentIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentType;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultView_DetailedAssessmentProbabilityHasValue_DoesNotShowsErrorTooltip(
            SimpleAssessmentResultType simpleAssessmentType)
        {
            // Setup
            const int rowIndex = 0;
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            PipingCalculationScenario calculationScenario = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenario(
                pipingFailureMechanism.Sections.First());
            pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[detailedAssessmentIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentType;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.IsEmpty(dataGridViewCell.ErrorText);
                Assert.AreEqual("1/4.123",
                                formattedValue);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultView_NoCalculatedScenario_ShowsErrorTooltip(SimpleAssessmentResultType simpleAssessmentType)
        {
            // Setup
            const int rowIndex = 0;
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            PipingCalculationScenario calculationScenario = PipingCalculationScenarioTestFactory.CreateNotCalculatedPipingCalculationScenario(
                pipingFailureMechanism.Sections.First());
            pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[detailedAssessmentIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentType;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultView_DetailedAssessmentProbabilityNaN_ShowsErrorTooltip(SimpleAssessmentResultType simpleAssessmentType)
        {
            // Setup
            const int rowIndex = 0;
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            PipingCalculationScenario calculationScenario = PipingCalculationScenarioTestFactory.CreateNotCalculatedPipingCalculationScenario(
                pipingFailureMechanism.Sections.First());
            calculationScenario.Output = new PipingOutput(new PipingOutput.ConstructionProperties());
            pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[detailedAssessmentIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentType;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Alle berekeningen voor dit vak moeten een geldige uitkomst hebben.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultView_NoCalculationScenarios_ShowsErrorTooltip(SimpleAssessmentResultType simpleAssessmentType)
        {
            // Setup
            const int rowIndex = 0;
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[detailedAssessmentIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentType;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden geselecteerd.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultView_NoCalculationScenariosRelevant_ShowsErrorTooltip(
            SimpleAssessmentResultType simpleAssessmentType)
        {
            // Setup
            const int rowIndex = 0;
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            PipingCalculationScenario calculationScenario = PipingCalculationScenarioTestFactory.CreateIrrelevantPipingCalculationScenario(
                pipingFailureMechanism.Sections.First());
            pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[detailedAssessmentIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentType;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden geselecteerd.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        public void FailureMechanismResultView_SimpleAssessmentProbabilityNegligibleAndDetailedAssessmentNaN_DoesNotShowError()
        {
            // Setup
            const int rowIndex = 0;
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            PipingCalculationScenario calculationScenario = PipingCalculationScenarioTestFactory.CreateNotCalculatedPipingCalculationScenario(
                pipingFailureMechanism.Sections.First());
            calculationScenario.Output = new PipingOutput(new PipingOutput.ConstructionProperties());
            pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[detailedAssessmentIndex];

                // Call
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = SimpleAssessmentResultType.ProbabilityNegligible;
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.IsEmpty(dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismNotifiesObserver_ThenViewUpdated()
        {
            // Given
            PipingFailureMechanism pipingFailureMechanism = GetFullyConfiguredFailureMechanism();
            PipingCalculationScenario calculationScenario1 = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenario(
                pipingFailureMechanism.Sections.First());
            PipingCalculationScenario calculationScenario2 = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenario(
                pipingFailureMechanism.Sections.First());
            pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario1);
            pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario2);

            using (ShowFailureMechanismResultsView(pipingFailureMechanism))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                PipingFailureMechanismSectionResultRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                                         .Select(r => r.DataBoundItem)
                                                                                         .Cast<PipingFailureMechanismSectionResultRow>()
                                                                                         .ToArray();

                // When
                pipingFailureMechanism.PipingProbabilityAssessmentInput.A = 0.01;
                pipingFailureMechanism.NotifyObservers();

                // Then
                PipingFailureMechanismSectionResultRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                                   .Select(r => r.DataBoundItem)
                                                                                   .Cast<PipingFailureMechanismSectionResultRow>()
                                                                                   .ToArray();

                CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
            }
        }

        private static PipingFailureMechanism GetFullyConfiguredFailureMechanism()
        {
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            failureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            return failureMechanism;
        }

        private PipingFailureMechanismResultView ShowFailureMechanismResultsView(PipingFailureMechanism failureMechanism)
        {
            var failureMechanismResultView = new PipingFailureMechanismResultView(failureMechanism.SectionResults,
                                                                                  failureMechanism,
                                                                                  new ObservableTestAssessmentSectionStub());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}