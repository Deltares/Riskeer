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
    public class NonAdoptableFailureMechanismSectionResultRowTest
    {
        private static NonAdoptableFailureMechanismSectionResultRow.ConstructionProperties ConstructionProperties =>
            new NonAdoptableFailureMechanismSectionResultRow.ConstructionProperties
            {
                InitialFailureMechanismResultTypeIndex = 2,
                InitialFailureMechanismResultSectionProbabilityIndex = 3,
                FurtherAnalysisNeededIndex = 4,
                RefinedSectionProbabilityIndex = 5,
                SectionProbabilityIndex = 6,
                AssemblyGroupIndex = 7
            };

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new NonAdoptableFailureMechanismSectionResultRow(result, null, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionResultRow<NonAdoptableFailureMechanismSectionResult>>(row);
                Assert.AreEqual(result.IsRelevant, row.IsRelevant);
                Assert.AreEqual(result.InitialFailureMechanismResultType, row.InitialFailureMechanismResultType);
                Assert.AreEqual(result.ManualInitialFailureMechanismResultSectionProbability, row.InitialFailureMechanismResultSectionProbability);
                Assert.AreEqual(result.FurtherAnalysisNeeded, row.FurtherAnalysisNeeded);
                Assert.AreEqual(result.RefinedSectionProbability, row.RefinedSectionProbability);

                TestHelper.AssertTypeConverter<NonAdoptableFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(NonAdoptableFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));
                TestHelper.AssertTypeConverter<NonAdoptableFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(NonAdoptableFailureMechanismSectionResultRow.RefinedSectionProbability));
                TestHelper.AssertTypeConverter<NonAdoptableFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(NonAdoptableFailureMechanismSectionResultRow.SectionProbability));

                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(6, columnStateDefinitions.Count);

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultTypeIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.FurtherAnalysisNeededIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.RefinedSectionProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SectionProbabilityIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.AssemblyGroupIndex);
            }
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
            const NonAdoptableInitialFailureMechanismResultType newValue = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;
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
            Action<NonAdoptableFailureMechanismSectionResultRow> setPropertyAction,
            Func<NonAdoptableFailureMechanismSectionResult, T> assertPropertyFunc,
            T newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Call
                setPropertyAction(row);

                // Assert
                Assert.AreEqual(newValue, assertPropertyFunc(result));
            }

            mocks.VerifyAll();
        }

        private static void ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(
            Action<NonAdoptableFailureMechanismSectionResultRow> setPropertyAction)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Call
                void Call() => setPropertyAction(row);

                // Assert
                const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
            }
        }

        #endregion

        #region Assembly

        [Test]
        public void Constructor_AssemblyRan_InputCorrectlySetOnCalculator()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, assessmentSection, ConstructionProperties);

                // Assert
                FailureMechanismSectionWithProfileProbabilityAssemblyInput input = calculator.FailureMechanismSectionAssemblyInput;
                Assert.AreEqual(assessmentSection.FailureMechanismContribution.SignalingNorm, input.SignalingNorm);
                Assert.AreEqual(assessmentSection.FailureMechanismContribution.LowerLimitNorm, input.LowerLimitNorm);
                Assert.AreEqual(row.IsRelevant, input.IsRelevant);
                Assert.IsTrue(input.HasProbabilitySpecified);
                Assert.AreEqual(row.InitialFailureMechanismResultSectionProbability, input.InitialProfileProbability);
                Assert.AreEqual(row.InitialFailureMechanismResultSectionProbability, input.InitialSectionProbability);
                Assert.AreEqual(row.FurtherAnalysisNeeded, input.FurtherAnalysisNeeded);
                Assert.AreEqual(row.RefinedSectionProbability, input.RefinedProfileProbability);
                Assert.AreEqual(row.RefinedSectionProbability, input.RefinedSectionProbability);
            }
        }

        [Test]
        public void Constructor_AssemblyRan_ReturnsAssemblyResult()
        {
            // Setup
            var random = new Random(39);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                // Call
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                FailureMechanismSectionAssemblyResult calculatorOutput = calculator.FailureMechanismSectionAssemblyResultOutput;
                FailureMechanismSectionAssemblyResult rowAssemblyResult = row.AssemblyResult;
                Assert.AreSame(calculatorOutput, row.AssemblyResult);

                Assert.AreEqual(rowAssemblyResult.SectionProbability, row.SectionProbability);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName(rowAssemblyResult.AssemblyGroup),
                                row.AssemblyGroup);
            }
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenAssemblyPropertiesSetToDefault()
        {
            // Given
            var random = new Random(39);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Precondition
                FailureMechanismSectionAssemblyResult calculatorOutput = calculator.FailureMechanismSectionAssemblyResultOutput;
                Assert.AreSame(calculatorOutput, row.AssemblyResult);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;

                // Then
                var expectedAssemblyResult = new DefaultFailureMechanismSectionAssemblyResult();
                FailureMechanismSectionAssemblyResult actualAssemblyResult = row.AssemblyResult;
                Assert.AreEqual(expectedAssemblyResult.N, actualAssemblyResult.N);
                Assert.AreEqual(expectedAssemblyResult.SectionProbability, actualAssemblyResult.SectionProbability);
                Assert.AreEqual(expectedAssemblyResult.ProfileProbability, actualAssemblyResult.ProfileProbability);
                Assert.AreEqual(expectedAssemblyResult.AssemblyGroup, actualAssemblyResult.AssemblyGroup);
            }
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenShowError()
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Precondition
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;

                // Then
                const string expectedErrorText = "Message";

                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);
            }
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndAssemblyDoesNotThrowException_ThenNoErrorShown()
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Precondition
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                const string expectedErrorText = "Message";

                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);

                // When
                calculator.ThrowExceptionOnCalculate = false;
                row.InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;

                // Then
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex].ErrorText);
            }
        }

        #endregion

        #region Column States

        [Test]
        public void Constructor_Always_ExpectedColumnStates()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.SectionProbabilityIndex], true, true);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithIsRelevant_ExpectedColumnStates(bool isRelevant)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section)
            {
                IsRelevant = isRelevant,
                FurtherAnalysisNeeded = true,
                InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.Manual
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

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
        }

        [Test]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.Manual, true, false)]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.NoFailureProbability, false, true)]
        public void Constructor_WithInitialFailureMechanismResultType_ExpectedColumnStates(NonAdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
                                                                                           bool isEnabled, bool isReadOnly)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section)
            {
                InitialFailureMechanismResultType = initialFailureMechanismResultType
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.InitialFailureMechanismResultSectionProbabilityIndex], isEnabled, isReadOnly);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithFurtherAnalysisNeeded_ExpectedColumnStates(bool furtherAnalysisNeeded)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section)
            {
                FurtherAnalysisNeeded = furtherAnalysisNeeded
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(
                    columnStateDefinitions[ConstructionProperties.RefinedSectionProbabilityIndex], furtherAnalysisNeeded);
            }
        }

        [Test]
        [TestCaseSource(typeof(AssemblyGroupColorTestHelper), nameof(AssemblyGroupColorTestHelper.FailureMechanismSectionAssemblyGroupColorCases))]
        public void Constructor_WithAssemblyGroupSet_ExpectedColumnStates(FailureMechanismSectionAssemblyGroup assemblyGroup,
                                                                          Color expectedBackgroundColor)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResult(double.NaN, double.NaN, double.NaN, assemblyGroup);

                // Call
                var row = new NonAdoptableFailureMechanismSectionResultRow(result, new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(
                    columnStateDefinitions[ConstructionProperties.AssemblyGroupIndex], expectedBackgroundColor);
            }
        }

        #endregion
    }
}