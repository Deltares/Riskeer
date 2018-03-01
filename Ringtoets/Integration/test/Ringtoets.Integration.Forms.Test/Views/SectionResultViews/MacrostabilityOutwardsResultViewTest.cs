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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;
using Ringtoets.Integration.Forms.Views.SectionResultViews;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class MacroStabilityOutwardsResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultIndex = 2;
        private const int detailedAssessmentProbabilityIndex = 3;
        private const int tailorMadeAssessmentResultIndex = 4;
        private const int tailorMadeAssessmentProbabilityIndex = 5;
        private const int simpleAssemblyCategoryGroupIndex = 6;
        private const int detailedAssemblyCategoryGroupIndex = 7;
        private const int tailorMadeAssemblyCategoryGroupIndex = 8;
        private const int combinedAssemblyCategoryGroupIndex = 9;
        private const int useManualAssemblyCategoryGroupIndex = 10;
        private const int manualAssemblyCategoryGroupIndex = 11;
        private const int columnCount = 12;

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            // Call
            TestDelegate call = () => new MacroStabilityOutwardsResultView(failureMechanism.SectionResults, failureMechanism, null);

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

            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            // Call
            using (var view = new MacroStabilityOutwardsResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<MacroStabilityOutwardsFailureMechanismSectionResult,
                    MacroStabilityOutwardsSectionResultRow, MacroStabilityOutwardsFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreAdded()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var form = new Form())
            using (var view = new MacroStabilityOutwardsResultView(new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>(),
                                                                   new MacroStabilityOutwardsFailureMechanism(),
                                                                   assessmentSection))
            {
                form.Controls.Add(view);
                form.Show();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[detailedAssessmentProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[tailorMadeAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[tailorMadeAssessmentProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[simpleAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[detailedAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[combinedAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[useManualAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[manualAssemblyCategoryGroupIndex]);

                Assert.AreEqual("Vak", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[detailedAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak\r\nfaalkans", dataGridView.Columns[detailedAssessmentProbabilityIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[tailorMadeAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Toets op maat\r\nfaalkans", dataGridView.Columns[tailorMadeAssessmentProbabilityIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\neenvoudige toets", dataGridView.Columns[simpleAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\ngedetailleerde toets per vak", dataGridView.Columns[detailedAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\ntoets op maat", dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\ngecombineerd", dataGridView.Columns[combinedAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Overschrijf\r\nassemblageresultaat", dataGridView.Columns[useManualAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\nhandmatig", dataGridView.Columns[manualAssemblyCategoryGroupIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void FailureMechanismResultView_FailureMechanismSectionResultAssigned_SectionAddedAsRow()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var random = new Random(21);
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1"))
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.None,
                DetailedAssessmentProbability = random.NextDouble(),
                TailorMadeAssessmentProbability = random.NextDouble()
            };
            var sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>
            {
                result
            };

            // Call
            using (var form = new Form())
            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new MacroStabilityOutwardsResultView(sectionResults, failureMechanism, assessmentSection))
            {
                form.Controls.Add(view);
                form.Show();

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewRow dataGridViewRow = rows[0];
                var rowObject = (MacroStabilityOutwardsSectionResultRow) dataGridViewRow.DataBoundItem;
                DataGridViewCellCollection cells = dataGridViewRow.Cells;

                Assert.AreEqual(columnCount, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);

                Assert.AreEqual(result.SimpleAssessmentResult, cells[simpleAssessmentResultIndex].Value);
                Assert.AreEqual(result.DetailedAssessmentResult, cells[detailedAssessmentResultIndex].Value);
                string expectedDetailedAssessmentProbabilityString = ProbabilityFormattingHelper.Format(result.DetailedAssessmentProbability);
                Assert.AreEqual(expectedDetailedAssessmentProbabilityString, cells[detailedAssessmentProbabilityIndex].FormattedValue);
                Assert.AreEqual(result.TailorMadeAssessmentResult, cells[tailorMadeAssessmentResultIndex].Value);
                string expectedTailorMadeAssessmentProbabilityString = ProbabilityFormattingHelper.Format(result.TailorMadeAssessmentProbability);
                Assert.AreEqual(expectedTailorMadeAssessmentProbabilityString, cells[tailorMadeAssessmentProbabilityIndex].FormattedValue);

                Assert.AreEqual(rowObject.SimpleAssemblyCategoryGroup, cells[simpleAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual(rowObject.DetailedAssemblyCategoryGroup, cells[detailedAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual(rowObject.TailorMadeAssemblyCategoryGroup, cells[tailorMadeAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual(rowObject.CombinedAssemblyCategoryGroup, cells[combinedAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual(rowObject.UseManualAssemblyCategoryGroup, cells[useManualAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual(rowObject.ManualAssemblyCategoryGroup, cells[manualAssemblyCategoryGroupIndex].Value);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, true)]
        [TestCase(SimpleAssessmentResultType.AssessFurther, true)]
        [TestCase(SimpleAssessmentResultType.NotApplicable, false)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible, false)]
        public void FailureMechanismResultView_SimpleAssessmentResultSet_CellsDisabledEnabled(
            SimpleAssessmentResultType simpleAssessmentResult,
            bool cellsEnabled)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability
            };
            var sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>
            {
                result
            };

            // Call
            using (var form = new Form())
            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new MacroStabilityOutwardsResultView(sectionResults, failureMechanism, assessmentSection))
            {
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewCellCollection cells = dataGridView.Rows[0].Cells;

                // Assert
                DataGridViewTestHelper.AssertCellsState(cells, new[]
                {
                    detailedAssessmentResultIndex,
                    detailedAssessmentProbabilityIndex,
                    tailorMadeAssessmentResultIndex,
                    tailorMadeAssessmentProbabilityIndex
                }, cellsEnabled);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(DetailedAssessmentResultType.NotAssessed, false)]
        [TestCase(DetailedAssessmentResultType.Probability, true)]
        public void FailureMechanismResultView_DetailedAssessmentResultSet_CellDisabledEnabled(
            DetailedAssessmentResultType detailedAssessmentResult,
            bool cellEnabled)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                DetailedAssessmentResult = detailedAssessmentResult
            };
            var sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>
            {
                result
            };

            // Call
            using (var form = new Form())
            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new MacroStabilityOutwardsResultView(sectionResults, failureMechanism, assessmentSection))
            {
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewCellCollection cells = dataGridView.Rows[0].Cells;

                // Assert
                DataGridViewTestHelper.AssertCellsState(cells, new[]
                {
                    detailedAssessmentProbabilityIndex
                }, cellEnabled);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.NotAssessed, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Insufficient, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.None, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Sufficient, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability, true)]
        public void FailureMechanismResultView_TailorMadeAssessmentResultSet_CellDisabledEnabled(
            TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult,
            bool cellEnabled)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                TailorMadeAssessmentResult = tailorMadeAssessmentResult
            };
            var sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>
            {
                result
            };

            // Call
            using (var form = new Form())
            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new MacroStabilityOutwardsResultView(sectionResults, failureMechanism, assessmentSection))
            {
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewCellCollection cells = dataGridView.Rows[0].Cells;

                // Assert
                DataGridViewTestHelper.AssertCellsState(cells, new[]
                {
                    tailorMadeAssessmentProbabilityIndex
                }, cellEnabled);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FailureMechanismResultView_UseManualAssemblyCategoryGroupSet_CellDisabledEnabled(bool useManualAssemblyCategoryGroup)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability,
                UseManualAssemblyCategoryGroup = useManualAssemblyCategoryGroup
            };
            var sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>
            {
                result
            };

            // Call
            using (var form = new Form())
            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new MacroStabilityOutwardsResultView(sectionResults, failureMechanism, assessmentSection))
            {
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewCellCollection cells = dataGridView.Rows[0].Cells;

                // Assert
                DataGridViewTestHelper.AssertCellsState(cells, new[]
                {
                    manualAssemblyCategoryGroupIndex
                }, useManualAssemblyCategoryGroup);

                DataGridViewTestHelper.AssertCellsState(cells, new[]
                {
                    simpleAssessmentResultIndex,
                    detailedAssessmentResultIndex,
                    detailedAssessmentProbabilityIndex,
                    tailorMadeAssessmentResultIndex,
                    tailorMadeAssessmentProbabilityIndex
                }, !useManualAssemblyCategoryGroup);

                DataGridViewTestHelper.AssertCellsState(cells, new[]
                {
                    simpleAssemblyCategoryGroupIndex,
                    detailedAssemblyCategoryGroupIndex,
                    tailorMadeAssemblyCategoryGroupIndex,
                    combinedAssemblyCategoryGroupIndex
                }, !useManualAssemblyCategoryGroup, true);
                mocks.VerifyAll();
            }
        }
    }
}