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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
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
        private const int simpleAssessmentIndex = 1;
        private const int detailedAssessmentProbabilityIndex = 2;
        private const int tailorMadeAssessmentProbabilityIndex = 3;

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
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
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

                Assert.AreEqual(4, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[detailedAssessmentProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[tailorMadeAssessmentProbabilityIndex]);

                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak\r\nFaalkans", dataGridView.Columns[detailedAssessmentProbabilityIndex].HeaderText);
                Assert.AreEqual("Toets op maat\r\nFaalkans", dataGridView.Columns[tailorMadeAssessmentProbabilityIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void FailureMechanismResultView_FailureMechanismSectionResultAssigned_SectionsAddedAsRows()
        {
            // Setup
            var random = new Random(21);
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var result1 = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1"))
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.None,
                DetailedAssessmentProbability = random.NextDouble(),
                TailorMadeAssessmentProbability = random.NextDouble()
            };
            var result2 = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 2"))
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                DetailedAssessmentProbability = random.NextDouble(),
                TailorMadeAssessmentProbability = random.NextDouble()
            };
            var result3 = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 3"))
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible,
                DetailedAssessmentProbability = random.NextDouble(),
                TailorMadeAssessmentProbability = random.NextDouble()
            };
            var result4 = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 4"))
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther,
                DetailedAssessmentProbability = random.NextDouble(),
                TailorMadeAssessmentProbability = random.NextDouble()
            };
            var sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>
            {
                result1,
                result2,
                result3,
                result4
            };

            // Call
            using (var form = new Form())
            using (var view = new MacroStabilityOutwardsResultView(sectionResults, new MacroStabilityOutwardsFailureMechanism(), assessmentSection))
            {
                form.Controls.Add(view);
                form.Show();

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(4, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                string expectedDetailedAssessmentProbabilityString1 = ProbabilityFormattingHelper.Format(result1.DetailedAssessmentProbability);
                Assert.AreEqual(expectedDetailedAssessmentProbabilityString1, cells[detailedAssessmentProbabilityIndex].FormattedValue);
                string expectedTailorMadeAssessmentProbabilityString1 = ProbabilityFormattingHelper.Format(result1.TailorMadeAssessmentProbability);
                Assert.AreEqual(expectedTailorMadeAssessmentProbabilityString1, cells[tailorMadeAssessmentProbabilityIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[detailedAssessmentProbabilityIndex]);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[tailorMadeAssessmentProbabilityIndex]);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                string expectedDetailedAssessmentProbabilityString2 = ProbabilityFormattingHelper.Format(result2.DetailedAssessmentProbability);
                Assert.AreEqual(expectedDetailedAssessmentProbabilityString2, cells[detailedAssessmentProbabilityIndex].FormattedValue);
                string expectedTailorMadeAssessmentProbabilityString2 = ProbabilityFormattingHelper.Format(result2.TailorMadeAssessmentProbability);
                Assert.AreEqual(expectedTailorMadeAssessmentProbabilityString2, cells[tailorMadeAssessmentProbabilityIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[detailedAssessmentProbabilityIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[tailorMadeAssessmentProbabilityIndex]);

                cells = rows[2].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 3", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result3.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                string expectedDetailedAssessmentProbabilityString3 = ProbabilityFormattingHelper.Format(result3.DetailedAssessmentProbability);
                Assert.AreEqual(expectedDetailedAssessmentProbabilityString3, cells[detailedAssessmentProbabilityIndex].FormattedValue);
                string expectedTailorMadeAssessmentProbabilityString3 = ProbabilityFormattingHelper.Format(result3.TailorMadeAssessmentProbability);
                Assert.AreEqual(expectedTailorMadeAssessmentProbabilityString3, cells[tailorMadeAssessmentProbabilityIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[detailedAssessmentProbabilityIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[tailorMadeAssessmentProbabilityIndex]);

                cells = rows[3].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 4", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result4.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                string expectedDetailedAssessmentProbabilityString4 = ProbabilityFormattingHelper.Format(result4.DetailedAssessmentProbability);
                Assert.AreEqual(expectedDetailedAssessmentProbabilityString4, cells[detailedAssessmentProbabilityIndex].FormattedValue);
                string expectedTailorMadeAssessmentProbabilityString4 = ProbabilityFormattingHelper.Format(result4.TailorMadeAssessmentProbability);
                Assert.AreEqual(expectedTailorMadeAssessmentProbabilityString4, cells[tailorMadeAssessmentProbabilityIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[detailedAssessmentProbabilityIndex]);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[tailorMadeAssessmentProbabilityIndex]);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, TestName = "FormWithFailureMechanismResultView_SectionPassesLevel0AndListenersNotified_RowsForSectionDisabled(notAssessed)")]
        [TestCase(SimpleAssessmentResultType.AssessFurther, TestName = "FormWithFailureMechanismResultView_SectionPassesLevel0AndListenersNotified_RowsForSectionDisabled(noVerdict)")]
        public void GivenFormWithFailureMechanismResultView_WhenSectionPassesSimpleAssessmentAndListenersNotified_ThenRowsForSectionDisabled(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Given
            var random = new Random(21);
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                DetailedAssessmentProbability = random.NextDouble(),
                TailorMadeAssessmentProbability = random.NextDouble()
            };
            var sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>
            {
                result
            };

            using (var form = new Form())
            using (var view = new MacroStabilityOutwardsResultView(sectionResults, new MacroStabilityOutwardsFailureMechanism(), assessmentSection))
            {
                form.Controls.Add(view);
                form.Show();

                // When
                result.SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible;
                result.NotifyObservers();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[detailedAssessmentProbabilityIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[tailorMadeAssessmentProbabilityIndex]);
                mocks.VerifyAll();
            }
        }
    }
}