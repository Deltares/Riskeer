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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerThreeIndex = 3;
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
        public void DefaultConstructor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            using (var view = new MacroStabilityInwardsFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<MacroStabilityInwardsFailureMechanismSectionResult,
                    MacroStabilityInwardsFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (ShowFailureMechanismResultsView(
                new MacroStabilityInwardsFailureMechanism(),
                new ObservableList<MacroStabilityInwardsFailureMechanismSectionResult>()))
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);
                Assert.IsTrue(dataGridView.Columns[assessmentLayerTwoAIndex].ReadOnly);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerTwoAIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[assessmentLayerTwoAIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            // Call
            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultType.None, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultType.None, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.NotApplicable)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultsView_ChangeCheckBox_DataGridViewCorrectlySyncedAndStylingSet(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            // Call
            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                dataGridView.Rows[0].Cells[simpleAssessmentIndex].Value = simpleAssessmentResult;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                DataGridViewCell cellAssessmentLayerTwoA = cells[assessmentLayerTwoAIndex];
                DataGridViewCell cellAssessmentLayerThree = cells[assessmentLayerThreeIndex];

                Assert.AreEqual(simpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cellAssessmentLayerTwoA.FormattedValue);
                Assert.AreEqual("-", cellAssessmentLayerThree.FormattedValue);

                if (simpleAssessmentResult == SimpleAssessmentResultType.ProbabilityNegligible
                    || simpleAssessmentResult == SimpleAssessmentResultType.NotApplicable)
                {
                    DataGridViewTestHelper.AssertCellIsDisabled(cellAssessmentLayerTwoA);
                    DataGridViewTestHelper.AssertCellIsDisabled(cellAssessmentLayerThree);

                    Assert.IsTrue(cellAssessmentLayerThree.ReadOnly);
                }
                else
                {
                    DataGridViewTestHelper.AssertCellIsEnabled(cellAssessmentLayerTwoA, true);
                    DataGridViewTestHelper.AssertCellIsEnabled(cellAssessmentLayerThree);

                    Assert.IsFalse(cellAssessmentLayerThree.ReadOnly);
                }
            }
        }

        [Test]
        [TestCase("test", assessmentLayerThreeIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", assessmentLayerThreeIndex)]
        public void FailureMechanismResultView_EditValueInvalid_ShowsErrorTooltip(string newValue, int cellIndex)
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
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
        public void FailureMechanismResultView_EditValueAssessmentLayerThreeInvalid_ShowErrorToolTip(double newValue)
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerThreeIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.AreEqual("Kans moet in het bereik [0,0, 1,0] liggen.", dataGridView.Rows[0].ErrorText);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void FailureMechanismResultView_EditValueAssessmentLayerThreeValid_DoNotShowErrorToolTipAndEditValue(double newValue)
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerThreeIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
                Assert.AreEqual(newValue, failureMechanism.SectionResults.First().AssessmentLayerThree);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultView_TotalContributionNotHundred_ShowsErrorTooltip(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            const int rowIndex = 0;
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(
                1.0 / 1000.0,
                failureMechanism.Sections.First());
            calculationScenario.Contribution = (RoundedDouble) 0.3;
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentResult;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.NotApplicable)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultView_AssessmentLayerTwoAHasValue_DoesNotShowsErrorTooltip(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            const int rowIndex = 0;
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(
                (RoundedDouble) 1e-3,
                failureMechanism.Sections.First());
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentResult;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.IsEmpty(dataGridViewCell.ErrorText);
                Assert.AreEqual("1/1",
                                formattedValue);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void FailureMechanismResultView_AssessmentLayerTwoANull_ShowsErrorTooltip(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            const int rowIndex = 0;

            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(
                failureMechanism.Sections.First());
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentResult;

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
        public void FailureMechanismResultView_AssessmentLayerTwoANaN_ShowsErrorTooltip(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            const int rowIndex = 0;

            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(
                failureMechanism.Sections.First());
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);
            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentResult;

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
        public void FailureMechanismResultView_NoCalculationScenarios_ShowsErrorTooltip(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            const int rowIndex = 0;
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentResult;

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
        public void FailureMechanismResultView_NoCalculationScenariosRelevant_ShowsErrorTooltip(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            const int rowIndex = 0;
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(
                failureMechanism.Sections.First());
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[simpleAssessmentIndex].Value = simpleAssessmentResult;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden geselecteerd.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        public void FailureMechanismResultView_SimpleAssessmentProbabilityNegligibleAndAssessmentLayerTwoAHasNaNOutput_DoesNotShowError()
        {
            // Setup
            const int rowIndex = 0;
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(
                failureMechanism.Sections.First());
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];

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
            MacroStabilityInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            MacroStabilityInwardsCalculationScenario calculationScenario1 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(
                0.1,
                failureMechanism.Sections.First());
            calculationScenario1.Contribution = (RoundedDouble) 0.6;
            MacroStabilityInwardsCalculationScenario calculationScenario2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(
                0.2,
                failureMechanism.Sections.First());
            calculationScenario2.Contribution = (RoundedDouble) 0.4;
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario1);
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario2);

            using (ShowFailureMechanismResultsView(failureMechanism, failureMechanism.SectionResults))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                MacroStabilityInwardsFailureMechanismSectionResultRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                                                        .Select(r => r.DataBoundItem)
                                                                                                        .Cast<MacroStabilityInwardsFailureMechanismSectionResultRow>()
                                                                                                        .ToArray();

                // When
                failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A = 0.01;
                failureMechanism.NotifyObservers();

                // Then
                MacroStabilityInwardsFailureMechanismSectionResultRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                                                  .Select(r => r.DataBoundItem)
                                                                                                  .Cast<MacroStabilityInwardsFailureMechanismSectionResultRow>()
                                                                                                  .ToArray();

                CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
            }
        }

        private static MacroStabilityInwardsFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
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

        private MacroStabilityInwardsFailureMechanismResultView ShowFailureMechanismResultsView(
            MacroStabilityInwardsFailureMechanism failureMechanism,
            IObservableEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> sectionResults)
        {
            var failureMechanismResultView = new MacroStabilityInwardsFailureMechanismResultView(sectionResults, failureMechanism, new ObservableTestAssessmentSectionStub());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}