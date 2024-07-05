// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Data.TestUtil;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Providers;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class AdoptableFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int isRelevantIndex = 1;
        private const int initialFailureMechanismResultTypeIndex = 2;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 3;
        private const int furtherAnalysisTypeIndex = 4;
        private const int refinedSectionProbabilityIndex = 5;
        private const int sectionProbabilityIndex = 6;
        private const int assemblyGroupIndex = 7;
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
        public void Constructor_PerformFailureMechanismSectionAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestAdoptableFailureMechanism();

            // Call
            void Call() => new TestAdoptableFailureMechanismResultView(
                failureMechanism.SectionResults, failureMechanism, assessmentSection,
                (fm, ass) => CreateFailureMechanismAssemblyResult(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performFailureMechanismSectionAssemblyFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestAdoptableFailureMechanism();

            // Call
            using (var view = new TestAdoptableFailureMechanismResultView(
                       failureMechanism.SectionResults, failureMechanism, assessmentSection,
                       (fm, ass) => CreateFailureMechanismAssemblyResult(),
                       (sr, fm, ass) => FailureMechanismSectionAssemblyResultWrapperTestFactory.Create()))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<AdoptableFailureMechanismSectionResult,
                    AdoptableFailureMechanismSectionResultRow,
                    TestAdoptableFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            var mocks = new MockRepository();
            using (ShowFailureMechanismResultsView(mocks, new TestAdoptableFailureMechanism(),
                                                   (fm, ass) => CreateFailureMechanismAssemblyResult()))
            {
                mocks.ReplayAll();

                // Then
                DataGridView dataGridView = GetDataGridView();

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[isRelevantIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[initialFailureMechanismResultTypeIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[initialFailureMechanismResultSectionProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[furtherAnalysisTypeIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[refinedSectionProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[sectionProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assemblyGroupIndex]);

                Assert.AreEqual("Vaknaam", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Is relevant", dataGridView.Columns[isRelevantIndex].HeaderText);
                Assert.AreEqual("Resultaat initieel mechanisme", dataGridView.Columns[initialFailureMechanismResultTypeIndex].HeaderText);
                Assert.AreEqual("Faalkans initieel\r\nmechanisme per vak\r\n[1/jaar]", dataGridView.Columns[initialFailureMechanismResultSectionProbabilityIndex].HeaderText);
                Assert.AreEqual("Vervolganalyse", dataGridView.Columns[furtherAnalysisTypeIndex].HeaderText);
                Assert.AreEqual("Aangescherpte\r\nfaalkans per vak\r\n[1/jaar]", dataGridView.Columns[refinedSectionProbabilityIndex].HeaderText);
                Assert.AreEqual("Rekenwaarde\r\nfaalkans per vak\r\n[1/jaar]", dataGridView.Columns[sectionProbabilityIndex].HeaderText);
                Assert.AreEqual("Duidingsklasse", dataGridView.Columns[assemblyGroupIndex].HeaderText);

                Assert.IsTrue(dataGridView.Columns[nameColumnIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[isRelevantIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[initialFailureMechanismResultTypeIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[initialFailureMechanismResultSectionProbabilityIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[furtherAnalysisTypeIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[refinedSectionProbabilityIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[sectionProbabilityIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[assemblyGroupIndex].ReadOnly);
            }

            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculateProbabilityStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            calculateProbabilityStrategy.Stub(s => s.CalculateSectionProbability()).Return(0.001);
            var errorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");

            var failureMechanism = new TestAdoptableFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            failureMechanism.CalculationsGroup.Children.Add(new TestCalculationScenario());

            var random = new Random(21);
            var failureMechanismSectionAssemblyResult = new FailureMechanismSectionAssemblyResultWrapper(
                new FailureMechanismSectionAssemblyResult(0.1, FailureMechanismSectionAssemblyGroup.I),
                random.NextEnumValue<AssemblyMethod>(), random.NextEnumValue<AssemblyMethod>());

            // Call
            using (var view = new TestAdoptableFailureMechanismResultView(
                       failureMechanism.SectionResults,
                       failureMechanism,
                       assessmentSection,
                       (fm, ass) => CreateFailureMechanismAssemblyResult(),
                       (sr, fm, ass) => failureMechanismSectionAssemblyResult)
                   {
                       CalculateProbabilityStrategy = calculateProbabilityStrategy,
                       RowErrorProvider = errorProvider
                   })
            {
                testForm.Controls.Add(view);
                testForm.Show();

                DataGridView dataGridView = GetDataGridView();

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(columnCount, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(true, cells[isRelevantIndex].Value);
                Assert.AreEqual(AdoptableInitialFailureMechanismResultType.Adopt, cells[initialFailureMechanismResultTypeIndex].Value);
                Assert.AreEqual("1/1.000", cells[initialFailureMechanismResultSectionProbabilityIndex].FormattedValue);
                Assert.AreEqual(FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, cells[furtherAnalysisTypeIndex].Value);
                Assert.AreEqual("-", cells[refinedSectionProbabilityIndex].FormattedValue);
                Assert.AreEqual("1/10", cells[sectionProbabilityIndex].FormattedValue);
                Assert.AreEqual("+I", cells[assemblyGroupIndex].FormattedValue);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenAdoptableFailureMechanismResultView_WhenCalculationNotifiesObservers_ThenDataGridViewUpdatedAndAAssemblyFunctionPerformed()
        {
            // Given
            var mocks = new MockRepository();

            var failureMechanism = new TestAdoptableFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            var calculationScenario = new TestCalculationScenario();
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            int nrOfAssemblyCalls = 0;
            using (ShowFailureMechanismResultsView(mocks, failureMechanism, (fm, ass) =>
            {
                nrOfAssemblyCalls++;
                return CreateFailureMechanismAssemblyResult();
            }))
            {
                mocks.ReplayAll();

                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.IsFalse(rowsChanged);

                // When
                calculationScenario.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);
                Assert.AreEqual(2, nrOfAssemblyCalls);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenAdoptableFailureMechanismResultView_WhenRootCalculationInputNotifiesObservers_ThenDataGridViewUpdatedAndAssemblyPerformed()
        {
            // Given
            var mocks = new MockRepository();

            var failureMechanism = new TestAdoptableFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            var calculationScenario = new TestCalculationScenario();
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            int nrOfAssemblyCalls = 0;
            using (ShowFailureMechanismResultsView(mocks, failureMechanism, (fm, ass) =>
            {
                nrOfAssemblyCalls++;
                return CreateFailureMechanismAssemblyResult();
            }))
            {
                mocks.ReplayAll();

                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.IsFalse(rowsChanged);

                // When
                calculationScenario.InputParameters.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);
                Assert.AreEqual(2, nrOfAssemblyCalls);
            }
        }

        [Test]
        public void GivenAdoptableFailureMechanismResultView_WhenNestedCalculationInputNotifiesObservers_ThenDataGridViewUpdatedAndAssemblyPerformed()
        {
            // Given
            var mocks = new MockRepository();

            var failureMechanism = new TestAdoptableFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            var calculationScenario = new TestCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculationScenario);
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            int nrOfAssemblyCalls = 0;
            using (ShowFailureMechanismResultsView(mocks, failureMechanism, (fm, ass) =>
            {
                nrOfAssemblyCalls++;
                return CreateFailureMechanismAssemblyResult();
            }))
            {
                mocks.ReplayAll();

                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.IsFalse(rowsChanged);

                // When
                calculationScenario.InputParameters.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);
                Assert.AreEqual(2, nrOfAssemblyCalls);
            }

            mocks.VerifyAll();
        }

        private static FailureMechanismAssemblyResultWrapper CreateFailureMechanismAssemblyResult()
        {
            return new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.Manual);
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private TestAdoptableFailureMechanismResultView ShowFailureMechanismResultsView(
            MockRepository mocks,
            TestAdoptableFailureMechanism failureMechanism,
            Func<TestAdoptableFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
        {
            return ShowFailureMechanismResultsView(mocks, failureMechanism, new AssessmentSectionStub(), performFailureMechanismAssemblyFunc);
        }

        private TestAdoptableFailureMechanismResultView ShowFailureMechanismResultsView(
            MockRepository mocks,
            TestAdoptableFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection,
            Func<TestAdoptableFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
        {
            var calculateProbabilityStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var rowErrorProvider = mocks.Stub<IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider>();

            var failureMechanismResultView = new TestAdoptableFailureMechanismResultView(failureMechanism.SectionResults,
                                                                                         failureMechanism,
                                                                                         assessmentSection,
                                                                                         performFailureMechanismAssemblyFunc,
                                                                                         (sr, fm, ass) => FailureMechanismSectionAssemblyResultWrapperTestFactory.Create())
            {
                CalculateProbabilityStrategy = calculateProbabilityStrategy,
                RowErrorProvider = rowErrorProvider
            };

            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private class TestAdoptableFailureMechanismResultView : AdoptableFailureMechanismResultView<TestAdoptableFailureMechanism, TestCalculationScenario, TestCalculationInput>
        {
            public TestAdoptableFailureMechanismResultView(
                IObservableEnumerable<AdoptableFailureMechanismSectionResult> failureMechanismSectionResults,
                TestAdoptableFailureMechanism failureMechanism,
                IAssessmentSection assessmentSection,
                Func<TestAdoptableFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc,
                Func<AdoptableFailureMechanismSectionResult, TestAdoptableFailureMechanism, IAssessmentSection, FailureMechanismSectionAssemblyResultWrapper> performFailureMechanismSectionAssemblyFunc)
                : base(failureMechanismSectionResults, failureMechanism, assessmentSection, performFailureMechanismAssemblyFunc, performFailureMechanismSectionAssemblyFunc) {}

            public IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider RowErrorProvider { get; set; }
            public IFailureMechanismSectionResultCalculateProbabilityStrategy CalculateProbabilityStrategy { get; set; }

            protected override IFailureMechanismSectionResultCalculateProbabilityStrategy CreateCalculateStrategy(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                                  IEnumerable<TestCalculationScenario> calculationScenarios)
            {
                return CalculateProbabilityStrategy;
            }

            protected override IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider CreateErrorProvider(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                                           IEnumerable<TestCalculationScenario> calculationScenarios)
            {
                return RowErrorProvider;
            }

            protected override IEnumerable<TestCalculationScenario> GetCalculationScenarios(AdoptableFailureMechanismSectionResult sectionResult)
            {
                return Enumerable.Empty<TestCalculationScenario>();
            }
        }

        private class TestAdoptableFailureMechanism : FailureMechanismBase<AdoptableFailureMechanismSectionResult>, ICalculatableFailureMechanism
        {
            public TestAdoptableFailureMechanism() : base("Test", "T")
            {
                CalculationsGroup = new CalculationGroup();
            }

            public IEnumerable<ICalculation> Calculations { get; }
            public CalculationGroup CalculationsGroup { get; }
            public Comment CalculationsInputComments { get; }
        }
    }
}