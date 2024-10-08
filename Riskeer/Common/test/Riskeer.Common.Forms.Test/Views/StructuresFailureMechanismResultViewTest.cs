﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class StructuresFailureMechanismResultViewTest
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
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestStructuresFailureMechanism();

            // Call
            using (var view = new StructuresFailureMechanismResultView<TestStructuresFailureMechanism, TestStructuresInput>(
                       failureMechanism.SectionResults, failureMechanism, assessmentSection, (fm, section) => new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.BOI1A1)))
            {
                // Assert
                Assert.IsInstanceOf<AdoptableFailureMechanismResultView<TestStructuresFailureMechanism,
                    StructuresCalculationScenario<TestStructuresInput>,
                    TestStructuresInput>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");

            var failureMechanism = new TestStructuresFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            failureMechanism.CalculationsGroup.Children.Add(new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                }
            });

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                DataGridView dataGridView = GetDataGridView();

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                double probability = failureMechanism.SectionResults.First().GetInitialFailureMechanismResultProbability(
                    failureMechanism.Calculations.OfType<TestStructuresCalculationScenario>());

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(columnCount, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(true, cells[isRelevantIndex].Value);
                Assert.AreEqual(AdoptableInitialFailureMechanismResultType.Adopt, cells[initialFailureMechanismResultTypeIndex].Value);
                Assert.AreEqual(probability, cells[initialFailureMechanismResultSectionProbabilityIndex].Value);
                Assert.AreEqual(FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, cells[furtherAnalysisTypeIndex].Value);
                Assert.AreEqual("-", cells[refinedSectionProbabilityIndex].FormattedValue);
                Assert.AreEqual("1/10", cells[sectionProbabilityIndex].FormattedValue);
                Assert.AreEqual("+I", cells[assemblyGroupIndex].FormattedValue);
            }
        }

        [Test]
        public void GivenStructuresFailureMechanismResultView_WhenCalculationNotifiesObservers_ThenDataGridViewUpdatedAndAssemblyPerformed()
        {
            // Given
            var failureMechanism = new TestStructuresFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            var calculationScenario = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                }
            };
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            int nrOfCalls = 0;
            Func<TestStructuresFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc = (fm, ass) =>
            {
                nrOfCalls++;
                return new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.BOI1A1);
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism, performFailureMechanismAssemblyFunc))
            {
                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.AreEqual(1, nrOfCalls);
                Assert.IsFalse(rowsChanged);

                // When
                calculationScenario.NotifyObservers();

                // Then
                Assert.AreEqual(2, nrOfCalls);
                Assert.IsTrue(rowsChanged);
            }
        }

        [Test]
        public void GivenStructuresFailureMechanismResultView_WhenRootCalculationInputNotifiesObservers_ThenDataGridViewUpdatedAndAssemblyPerformed()
        {
            // Given
            var failureMechanism = new TestStructuresFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            var calculationScenario = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                }
            };
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            int nrOfCalls = 0;
            Func<TestStructuresFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc = (fm, ass) =>
            {
                nrOfCalls++;
                return new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.BOI1A1);
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism, performFailureMechanismAssemblyFunc))
            {
                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.AreEqual(1, nrOfCalls);
                Assert.IsFalse(rowsChanged);

                // When
                calculationScenario.InputParameters.NotifyObservers();

                // Then
                Assert.AreEqual(2, nrOfCalls);
                Assert.IsTrue(rowsChanged);
            }
        }

        [Test]
        public void GivenStructuresFailureMechanismResultView_WhenNestedCalculationInputNotifiesObservers_ThenDataGridViewUpdatedAndAssemblyPerformed()
        {
            // Given
            var failureMechanism = new TestStructuresFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            var calculationScenario = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                }
            };
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculationScenario);
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            int nrOfCalls = 0;
            Func<TestStructuresFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc = (fm, ass) =>
            {
                nrOfCalls++;
                return new FailureMechanismAssemblyResultWrapper(double.NaN, AssemblyMethod.BOI1A1);
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism, performFailureMechanismAssemblyFunc))
            {
                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.AreEqual(1, nrOfCalls);
                Assert.IsFalse(rowsChanged);

                // When
                calculationScenario.InputParameters.NotifyObservers();

                // Then
                Assert.AreEqual(2, nrOfCalls);
                Assert.IsTrue(rowsChanged);
            }
        }

        private StructuresFailureMechanismResultView<TestStructuresFailureMechanism, TestStructuresInput> ShowFailureMechanismResultsView(
            TestStructuresFailureMechanism failureMechanism)
        {
            return ShowFailureMechanismResultsView(failureMechanism, new AssessmentSectionStub());
        }

        private StructuresFailureMechanismResultView<TestStructuresFailureMechanism, TestStructuresInput> ShowFailureMechanismResultsView(
            TestStructuresFailureMechanism failureMechanism, Func<TestStructuresFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
        {
            return ShowFailureMechanismResultsView(failureMechanism, new AssessmentSectionStub(), performFailureMechanismAssemblyFunc);
        }

        private StructuresFailureMechanismResultView<TestStructuresFailureMechanism, TestStructuresInput> ShowFailureMechanismResultsView(
            TestStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return ShowFailureMechanismResultsView(failureMechanism, assessmentSection, (fm, section) => new FailureMechanismAssemblyResultWrapper(1.2345, AssemblyMethod.Manual));
        }

        private StructuresFailureMechanismResultView<TestStructuresFailureMechanism, TestStructuresInput> ShowFailureMechanismResultsView(
            TestStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
            Func<TestStructuresFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
        {
            var failureMechanismResultView = new StructuresFailureMechanismResultView<TestStructuresFailureMechanism, TestStructuresInput>(
                failureMechanism.SectionResults,
                failureMechanism,
                assessmentSection,
                performFailureMechanismAssemblyFunc);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private class TestStructuresFailureMechanism : FailureMechanismBase<AdoptableFailureMechanismSectionResult>, ICalculatableFailureMechanism
        {
            public TestStructuresFailureMechanism()
                : base("Test", "T")
            {
                CalculationsGroup = new CalculationGroup();
                AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P1;
            }

            public IEnumerable<ICalculation> Calculations => CalculationsGroup.GetCalculations();

            public CalculationGroup CalculationsGroup { get; }

            public Comment CalculationsInputComments { get; }
        }
    }
}