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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.Views;

namespace Ringtoets.StabilityPointStructures.Forms.Test.Views
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultIndex = 2;
        private const int detailedAssessmentProbabilityIndex = 3;
        private const int tailorMadeAssessmentResultIndex = 4;
        private const int tailorMadeAssessmentProbabilityIndex = 5;
        private const int simpleAssemblyCategoryGroupIndex = 6;
        private const int detailedAssemblyCategoryGroupIndex = 7;
        private const int columnCount = 8;

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
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            using (var view = new StabilityPointStructuresFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<StabilityPointStructuresFailureMechanismSectionResult,
                    StabilityPointStructuresFailureMechanismSectionResultRow, StabilityPointStructuresFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            TestDelegate call = () => new StabilityPointStructuresFailureMechanismResultView(failureMechanism.SectionResults,
                                                                                             failureMechanism,
                                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GivenFormWithStabilityPointStructuresFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (ShowFailureMechanismResultsView(new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>()))
            {
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

                Assert.AreEqual("Vak", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[detailedAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak\r\nfaalkans", dataGridView.Columns[detailedAssessmentProbabilityIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[tailorMadeAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Toets op maat\r\nfaalkans", dataGridView.Columns[tailorMadeAssessmentProbabilityIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\neenvoudige toets", dataGridView.Columns[simpleAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\ngedetailleerde toets per vak", dataGridView.Columns[detailedAssemblyCategoryGroupIndex].HeaderText);

                Assert.IsTrue(dataGridView.Columns[nameColumnIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[simpleAssessmentResultIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[detailedAssessmentResultIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[detailedAssessmentProbabilityIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[tailorMadeAssessmentResultIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[tailorMadeAssessmentProbabilityIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[simpleAssemblyCategoryGroupIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[detailedAssemblyCategoryGroupIndex].ReadOnly);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var results = new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>
            {
                new StabilityPointStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1"))
            };

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(results))
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(columnCount, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultValidityOnlyType.None, cells[simpleAssessmentResultIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.Probability, cells[detailedAssessmentResultIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentProbabilityIndex].FormattedValue);
                Assert.AreEqual(TailorMadeAssessmentProbabilityCalculationResultType.None, cells[tailorMadeAssessmentResultIndex].Value);
                Assert.AreEqual("-", cells[tailorMadeAssessmentProbabilityIndex].FormattedValue);
                Assert.AreEqual("VIIv", cells[simpleAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual("VIv", cells[detailedAssemblyCategoryGroupIndex].Value);
            }
        }

        private StabilityPointStructuresFailureMechanismResultView ShowFailureMechanismResultsView(
            IObservableEnumerable<StabilityPointStructuresFailureMechanismSectionResult> sectionResults)
        {
            var failureMechanismResultView = new StabilityPointStructuresFailureMechanismResultView(sectionResults,
                                                                                                    new StabilityPointStructuresFailureMechanism(),
                                                                                                    new ObservableTestAssessmentSectionStub());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}