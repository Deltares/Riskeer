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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultViews;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class StrengthStabilityLengthwiseConstructionResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentIndex = 1;
        private const int assessmentLayerThreeIndex = 2;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism();

            // Call
            using (var view = new StrengthStabilityLengthwiseConstructionResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult,
                    StrengthStabilityLengthwiseConstructionFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (var form = new Form())
            using (var view = new StrengthStabilityLengthwiseConstructionResultView(
                new ObservableList<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>(),
                new StrengthStabilityLengthwiseConstructionFailureMechanism()))
            {
                form.Controls.Add(view);
                form.Show();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(3, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultView_WithFailureMechanismSectionResultAssigned_SectionsAddedAsRows()
        {
            // Setup
            var section1 = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0, 0)
            });
            var section2 = new FailureMechanismSection("Section 2", new[]
            {
                new Point2D(0, 0)
            });
            var section3 = new FailureMechanismSection("Section 3", new[]
            {
                new Point2D(0, 0)
            });
            var section4 = new FailureMechanismSection("Section 4", new[]
            {
                new Point2D(0, 0)
            });

            var random = new Random(21);
            var result1 = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(section1)
            {
                SimpleAssessmentInput = SimpleAssessmentResultType.None,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result2 = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(section2)
            {
                SimpleAssessmentInput = SimpleAssessmentResultType.NotApplicable,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result3 = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(section3)
            {
                SimpleAssessmentInput = SimpleAssessmentResultType.ProbabilityNegligible,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result4 = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(section4)
            {
                SimpleAssessmentInput = SimpleAssessmentResultType.AssessFurther,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var sectionResults = new ObservableList<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>
            {
                result1,
                result2,
                result3,
                result4
            };

            // Call
            using (var form = new Form())
            using (var view = new StrengthStabilityLengthwiseConstructionResultView(sectionResults,
                                                                                    new StrengthStabilityLengthwiseConstructionFailureMechanism()))
            {
                form.Controls.Add(view);
                form.Show();

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(4, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(3, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.SimpleAssessmentInput, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual(result1.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);

                cells = rows[1].Cells;
                Assert.AreEqual(3, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.SimpleAssessmentInput, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual(result2.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[2].Cells;
                Assert.AreEqual(3, cells.Count);
                Assert.AreEqual("Section 3", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result3.SimpleAssessmentInput, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual(result3.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[3].Cells;
                Assert.AreEqual(3, cells.Count);
                Assert.AreEqual("Section 4", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result4.SimpleAssessmentInput, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual(result4.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void GivenFormWithFailureMechanismResultView_WhenSectionPassesSimpleAssessmentAndListenersNotified_ThenRowsForSectionDisabled(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Given
            var section = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0, 0)
            });
            var random = new Random(21);
            var result = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(section)
            {
                SimpleAssessmentInput = simpleAssessmentResult,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var sectionResults = new ObservableList<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>
            {
                result
            };

            using (var form = new Form())
            using (var view = new StrengthStabilityLengthwiseConstructionResultView(sectionResults,
                                                                                    new StrengthStabilityLengthwiseConstructionFailureMechanism()))
            {
                form.Controls.Add(view);
                form.Show();

                // When
                result.SimpleAssessmentInput = SimpleAssessmentResultType.ProbabilityNegligible;
                result.NotifyObservers();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(3, cells.Count);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);
            }
        }
    }
}