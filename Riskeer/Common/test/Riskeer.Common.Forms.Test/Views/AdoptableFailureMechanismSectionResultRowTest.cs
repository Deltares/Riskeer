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
using Riskeer.Common.Forms.Providers;
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
                InitialFailureMechanismResultTypeIndex = 2,
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
                Assert.IsInstanceOf<FailureMechanismSectionResultRow<AdoptableFailureMechanismSectionResult>>(row);
                Assert.AreEqual(result.IsRelevant, row.IsRelevant);
                Assert.AreEqual(result.InitialFailureMechanismResultType, row.InitialFailureMechanismResultType);
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

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultTypeIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.FurtherAnalysisNeededIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.RefinedSectionProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SectionProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.AssemblyGroupIndex);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AdoptableInitialFailureMechanismResultType.Manual)]
        [TestCase(AdoptableInitialFailureMechanismResultType.NoFailureProbability)]
        public void GivenRowWithInitialFailureMechanismResultTypeAdopt_WhenValueChanged_ThenInitialProbabilitiesChanged(AdoptableInitialFailureMechanismResultType newValue)
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
                row.InitialFailureMechanismResultType = newValue;

                // Then
                Assert.AreEqual(result.ManualInitialFailureMechanismResultSectionProbability, row.InitialFailureMechanismResultSectionProbability);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenRowWithIsRelevantTrueAndInitialFailureMechanismResultTypeAdopt_WhenErrorProviderReturnsError_ThenShowError()
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
        [TestCase(false, AdoptableInitialFailureMechanismResultType.Adopt)]
        [TestCase(true, AdoptableInitialFailureMechanismResultType.Manual)]
        [TestCase(true, AdoptableInitialFailureMechanismResultType.NoFailureProbability)]
        public void GivenRowWithIsRelevantAndInitialFailureMechanismResultType_WhenErrorProviderReturnsError_ThenShowNoError(
            bool isRelevant, AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType)
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
                InitialFailureMechanismResultType = initialFailureMechanismResultType
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
        public void InitialFailureMechanismResultType_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const AdoptableInitialFailureMechanismResultType newValue = AdoptableInitialFailureMechanismResultType.NoFailureProbability;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.InitialFailureMechanismResultType = newValue,
                result => result.InitialFailureMechanismResultType,
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
        public void Constructor_AssemblyRan_InputCorrectlySetOnCalculator()
        {
            // Setup
            const double initialSectionProbability = 0.2;

            var mocks = new MockRepository();
            var errorProvider = mocks.Stub<IInitialFailureMechanismResultErrorProvider>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => initialSectionProbability, errorProvider, assessmentSection, ConstructionProperties);

                // Assert
                FailureMechanismSectionWithProfileProbabilityAssemblyInput input = calculator.FailureMechanismSectionAssemblyInput;
                Assert.AreEqual(assessmentSection.FailureMechanismContribution.SignalingNorm, input.SignalingNorm);
                Assert.AreEqual(assessmentSection.FailureMechanismContribution.LowerLimitNorm, input.LowerLimitNorm);
                Assert.AreEqual(row.IsRelevant, input.IsRelevant);
                Assert.IsTrue(input.HasProbabilitySpecified);
                Assert.AreEqual(initialSectionProbability, input.InitialProfileProbability);
                Assert.AreEqual(initialSectionProbability, input.InitialSectionProbability);
                Assert.AreEqual(row.FurtherAnalysisNeeded, input.FurtherAnalysisNeeded);
                Assert.AreEqual(row.RefinedSectionProbability, input.RefinedProfileProbability);
                Assert.AreEqual(row.RefinedSectionProbability, input.RefinedSectionProbability);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssemblyRan_ReturnsAssemblyResult()
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
                Assert.AreSame(calculatorOutput, rowAssemblyResult);

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
                Assert.AreSame(calculatorOutput, row.AssemblyResult);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual;

                // Then
                var expectedAssemblyResult = new DefaultFailureMechanismSectionAssemblyResult();
                FailureMechanismSectionAssemblyResult actualAssemblyResult = row.AssemblyResult;
                Assert.AreEqual(expectedAssemblyResult.N, actualAssemblyResult.N);
                Assert.AreEqual(expectedAssemblyResult.SectionProbability, actualAssemblyResult.SectionProbability);
                Assert.AreEqual(expectedAssemblyResult.ProfileProbability, actualAssemblyResult.ProfileProbability);
                Assert.AreEqual(expectedAssemblyResult.AssemblyGroup, actualAssemblyResult.AssemblyGroup);
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
                row.InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual;

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
                row.InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual;

                // Then
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);
            }

            mocks.VerifyAll();
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
                InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new AdoptableFailureMechanismSectionResultRow(result, () => double.NaN, errorProvider,
                                                                        new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultTypeIndex], isRelevant);
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
        [TestCase(AdoptableInitialFailureMechanismResultType.Adopt, true, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.Manual, true, false)]
        [TestCase(AdoptableInitialFailureMechanismResultType.NoFailureProbability, false, true)]
        public void Constructor_WithInitialFailureMechanismResultType_ExpectedColumnStates(AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
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
                InitialFailureMechanismResultType = initialFailureMechanismResultType
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
        public void Constructor_WithAssemblyGroupSet_ExpectedColumnStates(FailureMechanismSectionAssemblyGroup assemblyGroup,
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