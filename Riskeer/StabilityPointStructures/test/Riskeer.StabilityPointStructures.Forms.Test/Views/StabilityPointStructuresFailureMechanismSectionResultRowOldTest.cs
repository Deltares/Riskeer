﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.Forms.Views;

namespace Riskeer.StabilityPointStructures.Forms.Test.Views
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismSectionResultRowOldTest
    {
        private static StabilityPointStructuresFailureMechanismSectionResultRowOld.ConstructionProperties ConstructionProperties =>
            new StabilityPointStructuresFailureMechanismSectionResultRowOld.ConstructionProperties
            {
                SimpleAssessmentResultIndex = 1,
                DetailedAssessmentResultIndex = 2,
                DetailedAssessmentProbabilityIndex = 3,
                TailorMadeAssessmentResultIndex = 4,
                TailorMadeAssessmentProbabilityIndex = 5,
                SimpleAssemblyCategoryGroupIndex = 6,
                DetailedAssemblyCategoryGroupIndex = 7,
                TailorMadeAssemblyCategoryGroupIndex = 8,
                CombinedAssemblyCategoryGroupIndex = 9,
                CombinedAssemblyProbabilityIndex = 10,
                ManualAssemblyProbabilityIndex = 11
            };

        [Test]
        public void Constructor_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            // Call
            void Call() => new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                result, null, new StabilityPointStructuresFailureMechanism(),
                assessmentSection, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            // Call
            void Call() => new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                null, assessmentSection, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            // Call
            void Call() => new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                new StabilityPointStructuresFailureMechanism(), null, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            // Call
            void Call() => new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                new StabilityPointStructuresFailureMechanism(), assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios =
            {
                StabilityPointStructuresCalculationScenarioTestFactory.CreateStabilityPointStructuresCalculationScenario(section)
            };

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, calculationScenarios, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionResultRowOld<StabilityPointStructuresFailureMechanismSectionResultOld>>(row);

                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(11, columnStateDefinitions.Count);

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SimpleAssessmentResultIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.TailorMadeAssessmentResultIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.TailorMadeAssessmentProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SimpleAssemblyCategoryGroupIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssemblyCategoryGroupIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.CombinedAssemblyCategoryGroupIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.CombinedAssemblyProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.ManualAssemblyProbabilityIndex);

                Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
                Assert.AreEqual(result.DetailedAssessmentResult, row.DetailedAssessmentResult);
                Assert.AreEqual(result.GetDetailedAssessmentProbability(calculationScenarios), row.DetailedAssessmentProbability);
                Assert.AreEqual(result.TailorMadeAssessmentResult, row.TailorMadeAssessmentResult);
                Assert.AreEqual(row.TailorMadeAssessmentProbability, result.TailorMadeAssessmentProbability);
                Assert.AreEqual(result.UseManualAssembly, row.UseManualAssembly);
                Assert.AreEqual(result.ManualAssemblyProbability, row.ManualAssemblyProbability);

                TestHelper.AssertTypeConverter<StabilityPointStructuresFailureMechanismSectionResultRowOld, NoProbabilityValueDoubleConverter>(
                    nameof(StabilityPointStructuresFailureMechanismSectionResultRowOld.DetailedAssessmentProbability));
                TestHelper.AssertTypeConverter<StabilityPointStructuresFailureMechanismSectionResultRowOld, NoProbabilityValueDoubleConverter>(
                    nameof(StabilityPointStructuresFailureMechanismSectionResultRowOld.TailorMadeAssessmentProbability));
                TestHelper.AssertTypeConverter<StabilityPointStructuresFailureMechanismSectionResultRowOld, NoProbabilityValueDoubleConverter>(
                    nameof(StabilityPointStructuresFailureMechanismSectionResultRowOld.CombinedAssemblyProbability));
                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_AssemblyRan_ReturnCategoryGroups()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssemblyOld(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssemblyOld(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssemblyOld(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.CombinedAssemblyOutput = new FailureMechanismSectionAssemblyOld(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.SimpleAssessmentAssemblyOutput.Group),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.DetailedAssessmentAssemblyOutput.Group),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.TailorMadeAssessmentAssemblyOutput.Group),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.CombinedAssemblyOutput.Group),
                                row.CombinedAssemblyCategoryGroup);
                Assert.AreEqual(calculator.CombinedAssemblyOutput.Probability, row.CombinedAssemblyProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenAssemblyGroupSetToNone()
        {
            // Given
            var random = new Random(39);
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssemblyOld(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssemblyOld(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssemblyOld(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.CombinedAssemblyOutput = new FailureMechanismSectionAssemblyOld(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Precondition
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.SimpleAssessmentAssemblyOutput.Group),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.DetailedAssessmentAssemblyOutput.Group),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.TailorMadeAssessmentAssemblyOutput.Group),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.CombinedAssemblyOutput.Group),
                                row.CombinedAssemblyCategoryGroup);
                Assert.AreEqual(calculator.CombinedAssemblyOutput.Probability, row.CombinedAssemblyProbability);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.Applicable;

                // Then
                string expectedAssemblyDisplayName = FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(FailureMechanismSectionAssemblyCategoryGroup.None);
                Assert.AreEqual(expectedAssemblyDisplayName, row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(expectedAssemblyDisplayName, row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(expectedAssemblyDisplayName, row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(expectedAssemblyDisplayName, row.CombinedAssemblyCategoryGroup);
                Assert.IsNaN(row.CombinedAssemblyProbability);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndAssemblyDoesNotThrowException_ExpectedColumnStates()
        {
            // Given
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Precondition
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                const string expectedErrorText = "Message";

                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.CombinedAssemblyProbabilityIndex].ErrorText);

                // When
                calculator.ThrowExceptionOnCalculate = false;
                row.SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.Applicable;

                // Then
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.CombinedAssemblyProbabilityIndex].ErrorText);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void UseManualAssembly_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);
                bool originalValue = result.UseManualAssembly;
                bool newValue = !originalValue;

                // Call
                row.UseManualAssembly = newValue;

                // Assert
                Assert.AreEqual(newValue, result.UseManualAssembly);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void ManualAssemblyProbability_ValidValue_NotifyObserversAndPropertyChanged(double value)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                row.ManualAssemblyProbability = value;

                // Assert
                Assert.AreEqual(value, row.ManualAssemblyProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void ManualAssemblyProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                void Call() => row.ManualAssemblyProbability = value;

                // Assert
                const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
                mocks.VerifyAll();
            }
        }

        #region Column States

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None, true)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.NotApplicable, false)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable, true)]
        public void Constructor_WithSimpleAssessmentResultSet_ExpectedColumnStates(SimpleAssessmentValidityOnlyResultType simpleAssessmentResult,
                                                                                   bool cellsEnabled)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability
            };

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios =
            {
                StabilityPointStructuresCalculationScenarioTestFactory.CreateStabilityPointStructuresCalculationScenario(section)
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, calculationScenarios, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultIndex],
                                                                                     cellsEnabled);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex],
                                                                                     cellsEnabled,
                                                                                     true);

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex],
                                                                                     cellsEnabled);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex],
                                                                                     cellsEnabled);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(DetailedAssessmentProbabilityOnlyResultType.NotAssessed, false)]
        [TestCase(DetailedAssessmentProbabilityOnlyResultType.Probability, true)]
        public void Constructor_WithDetailedAssessmentResultSet_ExpectedColumnStates(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                                     bool cellEnabled)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                DetailedAssessmentResult = detailedAssessmentResult
            };

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios =
            {
                StabilityPointStructuresCalculationScenarioTestFactory.CreateStabilityPointStructuresCalculationScenario(section)
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, calculationScenarios, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex],
                                                                                     cellEnabled,
                                                                                     true);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.None, false)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed, false)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible, false)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.Probability, true)]
        public void Constructor_WithTailorMadeAssessmentResultSet_ExpectedColumnStates(
            TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
            bool cellEnabled)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                TailorMadeAssessmentResult = tailorMadeAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex],
                                                                                     cellEnabled);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithUseManualAssembly_ExpectedColumnStates(bool useManualAssembly)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                UseManualAssembly = useManualAssembly
            };

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios =
            {
                StabilityPointStructuresCalculationScenarioTestFactory.CreateStabilityPointStructuresCalculationScenario(section)
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, calculationScenarios, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.SimpleAssessmentResultIndex],
                                                                                     !useManualAssembly);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultIndex],
                                                                                     !useManualAssembly);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex],
                                                                                     !useManualAssembly, true);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex],
                                                                                     !useManualAssembly);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex],
                                                                                     !useManualAssembly);

                if (useManualAssembly)
                {
                    DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateIsDisabled(
                        columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex]);
                    DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateIsDisabled(
                        columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex]);
                    DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateIsDisabled(
                        columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex]);
                    DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateIsDisabled(
                        columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex]);
                }

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.CombinedAssemblyProbabilityIndex],
                                                                                     !useManualAssembly, true);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.ManualAssemblyProbabilityIndex],
                                                                                     useManualAssembly);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.FailureMechanismSectionAssemblyCategoryGroupColorCases))]
        public void Constructor_WithAssemblyCategoryGroupsSet_ExpectedColumnStates(FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup,
                                                                                   Color expectedBackgroundColor)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                var assemblyOutput = new FailureMechanismSectionAssemblyOld(0, assemblyCategoryGroup);
                calculator.SimpleAssessmentAssemblyOutput = assemblyOutput;
                calculator.DetailedAssessmentAssemblyOutput = assemblyOutput;
                calculator.TailorMadeAssessmentAssemblyOutput = assemblyOutput;
                calculator.CombinedAssemblyCategoryOutput = assemblyCategoryGroup;

                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(
                    columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex], expectedBackgroundColor);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(
                    columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex], expectedBackgroundColor);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(
                    columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex], expectedBackgroundColor);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(
                    columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex], expectedBackgroundColor);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable)]
        public void Constructor_TotalContributionNotOne_DetailedAssessmentProbabilityHasErrorText(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            StructuresCalculationScenario<StabilityPointStructuresInput> calculation = StabilityPointStructuresCalculationScenarioTestFactory.CreateStabilityPointStructuresCalculationScenario(section);
            calculation.Contribution = (RoundedDouble) 0.3;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult,
                    new[]
                    {
                        calculation
                    },
                    failureMechanism,
                    assessmentSection,
                    ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.AreEqual("De bijdragen van de maatgevende scenario's voor dit vak moeten opgeteld gelijk zijn aan 100%.",
                                resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable)]
        public void Constructor_NoCalculatedScenario_DetailedAssessmentProbabilityHasErrorText(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult,
                    new[]
                    {
                        StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section)
                    },
                    failureMechanism,
                    assessmentSection,
                    ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.AreEqual("Alle maatgevende berekeningen voor dit vak moeten uitgevoerd zijn.",
                                resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable)]
        public void Constructor_DetailedAssessmentProbabilityNaN_DetailedAssessmentProbabilityHasErrorText(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            StructuresCalculationScenario<StabilityPointStructuresInput> calculationScenario =
                StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section);
            calculationScenario.Output = new TestStructuresOutput(double.NaN);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult,
                    new[]
                    {
                        calculationScenario
                    },
                    failureMechanism,
                    assessmentSection,
                    ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.AreEqual("Alle maatgevende berekeningen voor dit vak moeten een geldige uitkomst hebben.",
                                resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable)]
        public void Constructor_NoCalculationScenarios_DetailedAssessmentProbabilityHasErrorText(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult,
                    Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism,
                    assessmentSection,
                    ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden gedefinieerd.",
                                resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable)]
        public void Constructor_NoCalculationScenariosRelevant_DetailedAssessmentProbabilityHasErrorText(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            StructuresCalculationScenario<StabilityPointStructuresInput> calculationScenario =
                StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section);
            calculationScenario.IsRelevant = false;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult,
                    new[]
                    {
                        calculationScenario
                    },
                    failureMechanism,
                    assessmentSection,
                    ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden gedefinieerd.",
                                resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_SectionResultAndSuccessfulCalculation_DetailedAssessmentProbabilityNoError()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios =
            {
                StabilityPointStructuresCalculationScenarioTestFactory.CreateStabilityPointStructuresCalculationScenario(section)
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult, calculationScenarios, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.AreEqual(sectionResult.GetDetailedAssessmentProbability(calculationScenarios), resultRow.DetailedAssessmentProbability);
                Assert.IsEmpty(resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_SectionResultWithManualAssemblyAndNotCalculated_DetailedAssessmentProbabilityNoError()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                UseManualAssembly = true
            };

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios =
            {
                StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section)
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call 
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult, calculationScenarios, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.IsEmpty(resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_SectionResultWithDetailedAssessmentNotAssessedAndNotCalculated_DetailedAssessmentProbabilityNoError()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                DetailedAssessmentResult = DetailedAssessmentProbabilityOnlyResultType.NotAssessed
            };

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios =
            {
                StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section)
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult, calculationScenarios, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.IsEmpty(resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(nameof(SimpleAssessmentResultIsSufficientVariousSectionResults))]
        public void Constructor_SectionResultAndAssessmentSimpleAssessmentSufficient_DetailedAssessmentProbabilityNoError(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResultType,
            Func<FailureMechanismSection, IEnumerable<StructuresCalculationScenario<StabilityPointStructuresInput>>> getCalculationScenariosFunc)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section)
            {
                SimpleAssessmentResult = simpleAssessmentResultType
            };

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios = getCalculationScenariosFunc(section).ToArray();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult, calculationScenarios, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.AreEqual(sectionResult.GetDetailedAssessmentProbability(calculationScenarios), resultRow.DetailedAssessmentProbability);
                Assert.IsEmpty(resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        private static IEnumerable<TestCaseData> SimpleAssessmentResultIsSufficientVariousSectionResults()
        {
            yield return new TestCaseData(
                SimpleAssessmentValidityOnlyResultType.NotApplicable,
                new Func<FailureMechanismSection, IEnumerable<StructuresCalculationScenario<StabilityPointStructuresInput>>>(
                    section => Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>()));
            yield return new TestCaseData(
                SimpleAssessmentValidityOnlyResultType.NotApplicable,
                new Func<FailureMechanismSection, IEnumerable<StructuresCalculationScenario<StabilityPointStructuresInput>>>(
                    section => new[]
                    {
                        StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section)
                    }));
            yield return new TestCaseData(
                SimpleAssessmentValidityOnlyResultType.NotApplicable,
                new Func<FailureMechanismSection, IEnumerable<StructuresCalculationScenario<StabilityPointStructuresInput>>>(
                    section =>
                    {
                        StructuresCalculationScenario<StabilityPointStructuresInput> calculation = StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section);
                        calculation.Output = new TestStructuresOutput(double.NaN);
                        return new[]
                        {
                            calculation
                        };
                    }));
            yield return new TestCaseData(
                SimpleAssessmentValidityOnlyResultType.NotApplicable,
                new Func<FailureMechanismSection, IEnumerable<StructuresCalculationScenario<StabilityPointStructuresInput>>>(
                    section => new[]
                    {
                        StabilityPointStructuresCalculationScenarioTestFactory.CreateStabilityPointStructuresCalculationScenario(section)
                    }));
        }

        #endregion

        #region Registration

        [Test]
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);
            result.Attach(observer);

            var newValue = new Random(21).NextEnumValue<SimpleAssessmentValidityOnlyResultType>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                row.SimpleAssessmentResult = newValue;

                // Assert
                Assert.AreEqual(newValue, result.SimpleAssessmentResult);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                row.DetailedAssessmentResult = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResult);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentProbability_NoCalculationScenarios_ReturnNaN()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                double detailedAssessmentProbability = resultRow.DetailedAssessmentProbability;

                // Assert
                Assert.IsNaN(detailedAssessmentProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentProbability_CalculationScenarioWithoutOutput_ReturnNaN()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            StructuresCalculationScenario<StabilityPointStructuresInput>[] calculationScenarios =
            {
                StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section)
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult, calculationScenarios, failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                double detailedAssessmentProbability = resultRow.DetailedAssessmentProbability;

                // Assert
                Assert.IsNaN(detailedAssessmentProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentProbability_CalculationScenarioWithNaNOutput_ReturnNaN()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            StructuresCalculationScenario<StabilityPointStructuresInput> calculation = StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section);
            calculation.Output = new TestStructuresOutput(double.NaN);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult,
                    new[]
                    {
                        calculation
                    },
                    failureMechanism,
                    assessmentSection,
                    ConstructionProperties);

                // Call
                double detailedAssessmentProbability = resultRow.DetailedAssessmentProbability;

                // Assert
                Assert.IsNaN(detailedAssessmentProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentProbability_CalculationScenarioSuccessful_ReturnDetailedAssessmentProbability()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            const double reliability = 0.95;

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            StructuresCalculationScenario<StabilityPointStructuresInput> calculation = StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(section);
            calculation.Output = new TestStructuresOutput(reliability);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    sectionResult,
                    new[]
                    {
                        calculation
                    },
                    failureMechanism,
                    assessmentSection,
                    ConstructionProperties);

                // Call
                double detailedAssessmentProbability = resultRow.DetailedAssessmentProbability;

                // Assert
                Assert.AreEqual(0.17105612630848185, detailedAssessmentProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void TailorMadeAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                row.TailorMadeAssessmentResult = newValue;

                // Assert
                Assert.AreEqual(newValue, result.TailorMadeAssessmentResult);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void TailorMadeAssessmentProbability_ValidValue_NotifyObserversAndPropertyChanged(double value)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    new StabilityPointStructuresFailureMechanism(), assessmentSection, ConstructionProperties);

                // Call
                row.TailorMadeAssessmentProbability = value;

                // Assert
                Assert.AreEqual(value, row.TailorMadeAssessmentProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void TailorMadeAssessmentProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResultOld(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRowOld(
                    result, Enumerable.Empty<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
                    new StabilityPointStructuresFailureMechanism(), assessmentSection, ConstructionProperties);

                // Call
                void Call() => row.TailorMadeAssessmentProbability = value;

                // Assert
                const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}