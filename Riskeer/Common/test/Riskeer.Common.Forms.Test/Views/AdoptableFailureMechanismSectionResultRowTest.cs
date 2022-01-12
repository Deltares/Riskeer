// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.Probability;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class AdoptableFailureMechanismSectionResultRowTest
    {
        private static AdoptableFailureMechanismSectionResultRow.ConstructionProperties ConstructionProperties =>
            new AdoptableFailureMechanismSectionResultRow.ConstructionProperties
            {
                InitialFailureMechanismResultIndex = 2,
                InitialFailureMechanismResultSectionProbabilityIndex = 3,
                FurtherAnalysisNeededIndex = 4,
                RefinedSectionProbabilityIndex = 5,
                SectionProbabilityIndex = 6,
                AssemblyGroupIndex = 7
            };

        [Test]
        public void Constructor_CalculateInitialFailureMechanismResultProbabilityFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableFailureMechanismSectionResultRow(result, null, errorProvider, new AssessmentSectionStub(),
                                                                         ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculateInitialFailureMechanismResultProbabilityFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InitialFailureMechanismResultErrorProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, null, new AssessmentSectionStub(),
                                                                         ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("initialFailureMechanismResultErrorProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                         null, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                         new AssessmentSectionStub(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            double initialFailureMechanismResultProbability = new Random(21).NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => initialFailureMechanismResultProbability, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                Assert.IsInstanceOf<AdoptableFailureMechanismSectionResultRow>(row);
                Assert.AreEqual(result.IsRelevant, row.IsRelevant);
                Assert.AreEqual(result.InitialFailureMechanismResult, row.InitialFailureMechanismResult);
                Assert.AreEqual(initialFailureMechanismResultProbability, row.InitialFailureMechanismResultSectionProbability);
                Assert.AreEqual(result.FurtherAnalysisNeeded, row.FurtherAnalysisNeeded);
                Assert.AreEqual(result.RefinedSectionProbability, row.RefinedSectionProbability);

                TestHelper.AssertTypeConverter<AdoptableFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(AdoptableFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));
                TestHelper.AssertTypeConverter<AdoptableFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(AdoptableFailureMechanismSectionResultRow.RefinedSectionProbability));
                TestHelper.AssertTypeConverter<AdoptableFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(AdoptableFailureMechanismSectionResultRow.SectionProbability));

                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(6, columnStateDefinitions.Count);

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.FurtherAnalysisNeededIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.RefinedSectionProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SectionProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.AssemblyGroupIndex);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(InitialFailureMechanismResultType.Manual)]
        [TestCase(InitialFailureMechanismResultType.NoFailureProbability)]
        public void GivenRowWithInitialFailureMechanismResultAdopt_WhenValueChanged_ThenInitialProbabilitiesChanged(InitialFailureMechanismResultType newValue)
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            double sectionProbability = new Random(21).NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => sectionProbability, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Precondition
                Assert.AreEqual(sectionProbability, row.InitialFailureMechanismResultSectionProbability);

                // When
                row.InitialFailureMechanismResult = newValue;

                // Then
                Assert.AreEqual(result.ManualInitialFailureMechanismResultSectionProbability, row.InitialFailureMechanismResultSectionProbability);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithIsRelevantTrueAndInitialFailureMechanismResultAdopt_WhenErrorProviderReturnsError_ThenShowError()
        {
            // Given
            const string errorText = "error";
            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IInitialFailureMechanismResultErrorProvider>();
            errorProvider.Expect(ep => ep.GetProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(errorText);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Then
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(errorText, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false, InitialFailureMechanismResultType.Adopt)]
        [TestCase(true, InitialFailureMechanismResultType.Manual)]
        [TestCase(true, InitialFailureMechanismResultType.NoFailureProbability)]
        public void GivenRowWithIsRelevantAndInitialFailureMechanismResult_WhenErrorProviderReturnsError_ThenShowNoError(
            bool isRelevant, InitialFailureMechanismResultType initialFailureMechanismResultType)
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.StrictMock<IInitialFailureMechanismResultErrorProvider>();
            errorProvider.Stub(ep => ep.GetProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return("error message");
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResult = initialFailureMechanismResultType
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Then
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex].ErrorText);
            }

            mocks.VerifyAll();
        }

        #region Registration

        [Test]
        public void IsRelevant_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const bool newValue = false;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.IsRelevant = newValue,
                result => result.IsRelevant,
                newValue);
        }

        [Test]
        public void InitialFailureMechanismResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const InitialFailureMechanismResultType newValue = InitialFailureMechanismResultType.NoFailureProbability;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.InitialFailureMechanismResult = newValue,
                result => result.InitialFailureMechanismResult,
                newValue);
        }

        [Test]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetValidProbabilities))]
        public void InitialFailureMechanismResultSectionProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.InitialFailureMechanismResultSectionProbability = newValue,
                result => result.ManualInitialFailureMechanismResultSectionProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetInvalidProbabilities))]
        public void InitialFailureMechanismResultSectionProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.InitialFailureMechanismResultSectionProbability = value);
        }

        [Test]
        public void FurtherAnalysisNeeded_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const bool newValue = true;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.FurtherAnalysisNeeded = newValue,
                result => result.FurtherAnalysisNeeded,
                newValue);
        }

        [Test]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetValidProbabilities))]
        public void RefinedSectionProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.RefinedSectionProbability = newValue,
                result => result.RefinedSectionProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetInvalidProbabilities))]
        public void RefinedSectionProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.RefinedSectionProbability = value);
        }

        private static void Property_SetNewValue_NotifyObserversAndPropertyChanged<T>(
            Action<AdoptableFailureMechanismSectionResultRow> setPropertyAction,
            Func<AdoptableFailureMechanismSectionResult, T> assertPropertyFunc,
            T newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Call
                setPropertyAction(row);

                // Assert
                Assert.AreEqual(newValue, assertPropertyFunc(result));
            }

            mocks.VerifyAll();
        }

        private static void ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(
            Action<AdoptableFailureMechanismSectionResultRow> setPropertyAction)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Call
                void Call() => setPropertyAction(row);

                // Assert
                const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
            }

            mocks.VerifyAll();
        }

        #endregion

        #region Assembly

        [Test]
        public void Constructor_AssemblyRan_ReturnCategoryGroups()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            var random = new Random(39);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                // Call
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                FailureMechanismSectionAssemblyResult calculatorOutput = calculator.FailureMechanismSectionAssemblyResultOutput;
                FailureMechanismSectionAssemblyResult rowAssemblyResult = row.AssemblyResult;
                AssertFailureMechanismSectionAssemblyResult(calculatorOutput, rowAssemblyResult);

                Assert.AreEqual(rowAssemblyResult.SectionProbability, row.SectionProbability);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName(rowAssemblyResult.AssemblyGroup),
                                row.AssemblyGroup);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenAssemblyPropertiesSetToDefault()
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            var random = new Random(39);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Precondition
                FailureMechanismSectionAssemblyResult calculatorOutput = calculator.FailureMechanismSectionAssemblyResultOutput;
                AssertFailureMechanismSectionAssemblyResult(calculatorOutput, row.AssemblyResult);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.InitialFailureMechanismResult = InitialFailureMechanismResultType.Manual;

                // Then
                AssertFailureMechanismSectionAssemblyResult(new DefaultFailureMechanismSectionAssemblyResult(), row.AssemblyResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenShowError()
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Precondition
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.InitialFailureMechanismResult = InitialFailureMechanismResultType.Manual;

                // Then
                const string expectedErrorText = "Message";

                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndAssemblyDoesNotThrowException_ThenNoErrorShown()
        {
            // Given
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Precondition
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                const string expectedErrorText = "Message";

                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

                // When
                calculator.ThrowExceptionOnCalculate = false;
                row.InitialFailureMechanismResult = InitialFailureMechanismResultType.Manual;

                // Then
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);
            }

            mocks.VerifyAll();
        }

        private static void AssertFailureMechanismSectionAssemblyResult(FailureMechanismSectionAssemblyResult expected,
                                                                        FailureMechanismSectionAssemblyResult actual)
        {
            Assert.AreEqual(expected.N, actual.N);
            Assert.AreEqual(expected.SectionProbability, actual.SectionProbability);
            Assert.AreEqual(expected.ProfileProbability, actual.ProfileProbability);
            Assert.AreEqual(expected.AssemblyGroup, actual.AssemblyGroup);
        }

        #endregion

        #region Column States

        [Test]
        public void Constructor_Always_ExpectedColumnStates()
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex], true, true);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithIsRelevant_ExpectedColumnStates(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                IsRelevant = isRelevant,
                FurtherAnalysisNeeded = true,
                InitialFailureMechanismResult = InitialFailureMechanismResultType.Manual
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultIndex], isRelevant);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex], isRelevant);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.FurtherAnalysisNeededIndex], isRelevant);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex], isRelevant);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(InitialFailureMechanismResultType.Adopt, true, true)]
        [TestCase(InitialFailureMechanismResultType.Manual, true, false)]
        [TestCase(InitialFailureMechanismResultType.NoFailureProbability, false, true)]
        public void Constructor_WithInitialFailureMechanismResult_ExpectedColumnStates(InitialFailureMechanismResultType initialFailureMechanismResultType,
                                                                                       bool isEnabled, bool isReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            errorProvider.Stub(ep => ep.GetProbabilityValidationError(null))
                         .IgnoreArguments()
                         .Return(string.Empty);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResult = initialFailureMechanismResultType
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex], isEnabled, isReadOnly);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithFurtherAnalysisNeeded_ExpectedColumnStates(bool furtherAnalysisNeeded)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section)
            {
                FurtherAnalysisNeeded = furtherAnalysisNeeded
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex], furtherAnalysisNeeded);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(AssemblyGroupColorTestHelper), nameof(AssemblyGroupColorTestHelper.FailureMechanismSectionAssemblyGroupColorCases))]
        public void Constructor_WithAssemblyGroupsSet_ExpectedColumnStates(FailureMechanismSectionAssemblyGroup assemblyGroup,
                                                                           Color expectedBackgroundColor)
        {
            // Setup
            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResult(double.NaN, double.NaN, double.NaN, assemblyGroup);

                // Call
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(
                    columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex], expectedBackgroundColor);
            }

            mocks.VerifyAll();
        }

        #endregion
    }
}