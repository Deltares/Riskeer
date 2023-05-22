﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int isRelevantIndex = 1;
        private const int initialFailureMechanismResultTypeIndex = 2;
        private const int initialFailureMechanismResultProfileProbabilityIndex = 3;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 4;
        private const int furtherAnalysisTypeIndex = 5;
        private const int probabilityRefinementTypeIndex = 6;
        private const int refinedProfileProbabilityIndex = 7;
        private const int refinedSectionProbabilityIndex = 8;
        private const int profileProbabilityIndex = 9;
        private const int sectionProbabilityIndex = 10;
        private const int sectionNIndex = 11;
        private const int assemblyGroupIndex = 12;
        private const int columnCount = 13;
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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            using (var view = new GrassCoverErosionInwardsFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<AdoptableWithProfileProbabilityFailureMechanismSectionResult,
                    AdoptableWithProfileProbabilityFailureMechanismSectionResultRow,
                    GrassCoverErosionInwardsFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFormWithGrassCoverErosionInwardsFailureMechanismResultView_ThenExpectedColumnsAreVisible(bool useLengthEffect)
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                GeneralInput =
                {
                    ApplyLengthEffectInSection = useLengthEffect
                }
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Then
                DataGridView dataGridView = GetDataGridView();

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[isRelevantIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[initialFailureMechanismResultTypeIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[initialFailureMechanismResultSectionProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[furtherAnalysisTypeIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[probabilityRefinementTypeIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[refinedProfileProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[refinedSectionProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[profileProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[sectionProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[sectionNIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assemblyGroupIndex]);

                Assert.AreEqual("Vaknaam", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Is relevant", dataGridView.Columns[isRelevantIndex].HeaderText);
                Assert.AreEqual("Resultaat initieel mechanisme", dataGridView.Columns[initialFailureMechanismResultTypeIndex].HeaderText);
                Assert.AreEqual("Faalkans initieel\r\nmechanisme per doorsnede\r\n[1/jaar]", dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].HeaderText);
                Assert.AreEqual("Faalkans initieel\r\nmechanisme per vak\r\n[1/jaar]", dataGridView.Columns[initialFailureMechanismResultSectionProbabilityIndex].HeaderText);
                Assert.AreEqual("Vervolganalyse", dataGridView.Columns[furtherAnalysisTypeIndex].HeaderText);
                Assert.AreEqual("Aanscherpen faalkans", dataGridView.Columns[probabilityRefinementTypeIndex].HeaderText);
                Assert.AreEqual("Aangescherpte\r\nfaalkans per doorsnede\r\n[1/jaar]", dataGridView.Columns[refinedProfileProbabilityIndex].HeaderText);
                Assert.AreEqual("Aangescherpte\r\nfaalkans per vak\r\n[1/jaar]", dataGridView.Columns[refinedSectionProbabilityIndex].HeaderText);
                Assert.AreEqual("Rekenwaarde\r\nfaalkans per doorsnede\r\n[1/jaar]", dataGridView.Columns[profileProbabilityIndex].HeaderText);
                Assert.AreEqual("Rekenwaarde\r\nfaalkans per vak\r\n[1/jaar]", dataGridView.Columns[sectionProbabilityIndex].HeaderText);
                Assert.AreEqual("Rekenwaarde Nvak*\r\n[-]", dataGridView.Columns[sectionNIndex].HeaderText);
                Assert.AreEqual("Duidingsklasse", dataGridView.Columns[assemblyGroupIndex].HeaderText);

                Assert.IsTrue(dataGridView.Columns[nameColumnIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[isRelevantIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[initialFailureMechanismResultTypeIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[initialFailureMechanismResultSectionProbabilityIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[furtherAnalysisTypeIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[probabilityRefinementTypeIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[refinedProfileProbabilityIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[refinedSectionProbabilityIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[profileProbabilityIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[sectionProbabilityIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[sectionNIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[assemblyGroupIndex].ReadOnly);

                Assert.AreEqual(useLengthEffect, dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[probabilityRefinementTypeIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[refinedProfileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[profileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[sectionNIndex].Visible);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                GeneralInput =
                {
                    ApplyLengthEffectInSection = true
                }
            };

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            failureMechanism.CalculationsGroup.Children.Add(GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section));

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                DataGridView dataGridView = GetDataGridView();

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                double probability = failureMechanism.SectionResults.First().GetInitialFailureMechanismResultProbability(
                    failureMechanism.Calculations.OfType<GrassCoverErosionInwardsCalculationScenario>());

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(columnCount, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(true, cells[isRelevantIndex].Value);
                Assert.AreEqual(AdoptableInitialFailureMechanismResultType.Adopt, cells[initialFailureMechanismResultTypeIndex].Value);
                Assert.AreEqual(probability, cells[initialFailureMechanismResultProfileProbabilityIndex].Value);
                Assert.AreEqual(probability, cells[initialFailureMechanismResultSectionProbabilityIndex].Value);
                Assert.AreEqual(FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, cells[furtherAnalysisTypeIndex].Value);
                Assert.AreEqual(ProbabilityRefinementType.Section, cells[probabilityRefinementTypeIndex].Value);
                Assert.AreEqual("<afgeleid>", cells[refinedProfileProbabilityIndex].FormattedValue);
                Assert.AreEqual("-", cells[refinedSectionProbabilityIndex].FormattedValue);
                Assert.AreEqual("1/100", cells[profileProbabilityIndex].FormattedValue);
                Assert.AreEqual("1/10", cells[sectionProbabilityIndex].FormattedValue);
                Assert.AreEqual("10,00", cells[sectionNIndex].FormattedValue);
                Assert.AreEqual("+I", cells[assemblyGroupIndex].FormattedValue);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_SetsCorrectInputOnCalculator()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticWorstSectionOrProfile;
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            var assessmentSection = new AssessmentSectionStub();

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism, assessmentSection))
            {
                // Assert
                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = testFactory.LastCreatedFailureMechanismAssemblyCalculator;

                Assert.AreEqual(failureMechanism.GeneralInput.N, calculator.FailureMechanismN);
            }
        }

        [Test]
        public void GivenGrassCoverErosionInwardsFailureMechanismResultView_WhenCalculationNotifiesObservers_ThenDataGridViewUpdatedAndAssemblyPerformed()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            GrassCoverErosionInwardsCalculationScenario calculationScenario = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section);
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);
            failureMechanism.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections;
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = testFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                failureMechanismSectionAssemblyCalculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResultWrapper(
                    new FailureMechanismSectionAssemblyResult(1, 1, 1, FailureMechanismSectionAssemblyGroup.III),
                    AssemblyMethod.BOI0A1, AssemblyMethod.BOI0B1);

                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = testFactory.LastCreatedFailureMechanismAssemblyCalculator;
                IEnumerable<FailureMechanismSectionAssemblyResult> initialCalculatorInput = failureMechanismAssemblyCalculator.SectionAssemblyResultsInput
                                                                                                                              .ToArray();

                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.IsFalse(rowsChanged);

                // When
                calculationScenario.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);
                IEnumerable<FailureMechanismSectionAssemblyResult> updatedCalculatorInput = failureMechanismAssemblyCalculator.SectionAssemblyResultsInput
                                                                                                                              .ToArray();
                CollectionAssert.AreNotEqual(initialCalculatorInput, updatedCalculatorInput);
            }
        }

        [Test]
        public void GivenGrassCoverErosionInwardsFailureMechanismResultView_WhenRootCalculationInputNotifiesObservers_ThenDataGridViewUpdatedAndAssemblyPerformed()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            GrassCoverErosionInwardsCalculationScenario calculationScenario = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section);
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);
            failureMechanism.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections;
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = testFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                failureMechanismSectionAssemblyCalculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResultWrapper(
                    new FailureMechanismSectionAssemblyResult(1, 1, 1, FailureMechanismSectionAssemblyGroup.III),
                    AssemblyMethod.BOI0A1, AssemblyMethod.BOI0B1);

                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = testFactory.LastCreatedFailureMechanismAssemblyCalculator;
                IEnumerable<FailureMechanismSectionAssemblyResult> initialCalculatorInput = failureMechanismAssemblyCalculator.SectionAssemblyResultsInput
                                                                                                                              .ToArray();

                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.IsFalse(rowsChanged);

                // When
                calculationScenario.InputParameters.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);
                IEnumerable<FailureMechanismSectionAssemblyResult> updatedCalculatorInput = failureMechanismAssemblyCalculator.SectionAssemblyResultsInput
                                                                                                                              .ToArray();
                CollectionAssert.AreNotEqual(initialCalculatorInput, updatedCalculatorInput);
            }
        }

        [Test]
        public void GivenGrassCoverErosionInwardsFailureMechanismResultView_WhenNestedCalculationInputNotifiesObservers_ThenDataGridViewUpdatedAndAssemblyPerformed()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            GrassCoverErosionInwardsCalculationScenario calculationScenario = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section);
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculationScenario);
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);
            failureMechanism.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections;
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                var testFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = testFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                failureMechanismSectionAssemblyCalculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResultWrapper(
                    new FailureMechanismSectionAssemblyResult(1, 1, 1, FailureMechanismSectionAssemblyGroup.III),
                    AssemblyMethod.BOI0A1, AssemblyMethod.BOI0B1);

                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = testFactory.LastCreatedFailureMechanismAssemblyCalculator;
                IEnumerable<FailureMechanismSectionAssemblyResult> initialCalculatorInput = failureMechanismAssemblyCalculator.SectionAssemblyResultsInput
                                                                                                                              .ToArray();

                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.IsFalse(rowsChanged);

                // When
                calculationScenario.InputParameters.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);
                IEnumerable<FailureMechanismSectionAssemblyResult> updatedCalculatorInput = failureMechanismAssemblyCalculator.SectionAssemblyResultsInput
                                                                                                                              .ToArray();
                CollectionAssert.AreNotEqual(initialCalculatorInput, updatedCalculatorInput);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenGrassCoverErosionInwardsFailureMechanismResultView_WhenApplyLengthEffectChangedAndFailureMechanismObserversNotified_ThenDataGridViewUpdated(
            bool useLengthEffect)
        {
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                GeneralInput =
                {
                    ApplyLengthEffectInSection = !useLengthEffect
                }
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            GrassCoverErosionInwardsCalculationScenario calculationScenario = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section);
            failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.IsFalse(rowsChanged);
                Assert.AreEqual(!useLengthEffect, dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].Visible);
                Assert.AreEqual(!useLengthEffect, dataGridView.Columns[probabilityRefinementTypeIndex].Visible);
                Assert.AreEqual(!useLengthEffect, dataGridView.Columns[refinedProfileProbabilityIndex].Visible);
                Assert.AreEqual(!useLengthEffect, dataGridView.Columns[profileProbabilityIndex].Visible);
                Assert.AreEqual(!useLengthEffect, dataGridView.Columns[sectionNIndex].Visible);

                // When
                failureMechanism.GeneralInput.ApplyLengthEffectInSection = useLengthEffect;
                failureMechanism.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[probabilityRefinementTypeIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[refinedProfileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[profileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[sectionNIndex].Visible);
            }
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private GrassCoverErosionInwardsFailureMechanismResultView ShowFailureMechanismResultsView(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            return ShowFailureMechanismResultsView(failureMechanism, new AssessmentSectionStub());
        }

        private GrassCoverErosionInwardsFailureMechanismResultView ShowFailureMechanismResultsView(GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                                                   IAssessmentSection assessmentSection)
        {
            var failureMechanismResultView = new GrassCoverErosionInwardsFailureMechanismResultView(failureMechanism.SectionResults,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}