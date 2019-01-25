﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultRowTest
    {
        private static GrassCoverErosionInwardsFailureMechanismSectionResultRow.ConstructionProperties ConstructionProperties
        {
            get
            {
                return new GrassCoverErosionInwardsFailureMechanismSectionResultRow.ConstructionProperties
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
            }
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                result, null, assessmentSection, ConstructionProperties);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                result, new GrassCoverErosionInwardsFailureMechanism(), null, ConstructionProperties);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                result, new GrassCoverErosionInwardsFailureMechanism(), assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("constructionProperties", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = CreateCalculationWithOutput()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionResultRow<GrassCoverErosionInwardsFailureMechanismSectionResult>>(row);
                Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
                Assert.AreEqual(result.DetailedAssessmentResult, row.DetailedAssessmentResult);
                Assert.AreEqual(result.TailorMadeAssessmentResult, row.TailorMadeAssessmentResult);
                Assert.AreEqual(result.TailorMadeAssessmentProbability, row.TailorMadeAssessmentProbability);
                Assert.AreEqual(result.UseManualAssembly, row.UseManualAssembly);
                Assert.AreEqual(result.ManualAssemblyProbability, row.ManualAssemblyProbability);

                TestHelper.AssertTypeConverter<GrassCoverErosionInwardsFailureMechanismSectionResultRow,
                    NoProbabilityValueDoubleConverter>(
                    nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.DetailedAssessmentProbability));
                TestHelper.AssertTypeConverter<GrassCoverErosionInwardsFailureMechanismSectionResultRow,
                    NoProbabilityValueDoubleConverter>(
                    nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.TailorMadeAssessmentProbability));
                TestHelper.AssertTypeConverter<GrassCoverErosionInwardsFailureMechanismSectionResultRow,
                    NoProbabilityValueDoubleConverter>(
                    nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.ManualAssemblyProbability));

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
                mocks.VerifyAll();
            }
        }

        [Test]
        public void UseManualAssembly_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                TestDelegate test = () => row.ManualAssemblyProbability = value;

                // Assert
                const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenAssemblyGroupSetToNone()
        {
            // Given
            var random = new Random(39);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.CombinedAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection,
                                                                                       ConstructionProperties);

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
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(FailureMechanismSectionAssemblyCategoryGroup.None),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(FailureMechanismSectionAssemblyCategoryGroup.None),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(FailureMechanismSectionAssemblyCategoryGroup.None),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(FailureMechanismSectionAssemblyCategoryGroup.None),
                                row.CombinedAssemblyCategoryGroup);
                Assert.IsNaN(row.CombinedAssemblyProbability);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndAssemblyDoesNotThrowException_ExpectedColumnStates()
        {
            // Given
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection,
                                                                                       ConstructionProperties);

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

        private static GrassCoverErosionInwardsCalculation CreateCalculationWithOutput()
        {
            return new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0.56789),
                                                            new TestDikeHeightOutput(0),
                                                            new TestOvertoppingRateOutput(0))
            };
        }

        #region Column States

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None, true)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.NotApplicable, false)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable, true)]
        public void Constructor_WithSimpleAssessment_ExpectedColumnStates(SimpleAssessmentValidityOnlyResultType simpleAssessmentResult,
                                                                          bool cellsEnabled)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                Calculation = CreateCalculationWithOutput(),
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                DetailedAssessmentResult = detailedAssessmentResult,
                Calculation = CreateCalculationWithOutput()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                TailorMadeAssessmentResult = tailorMadeAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

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
        public void Constructor_WithUseManualAssemblySet_ExpectedColumnStates(bool useManualAssembly)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = CreateCalculationWithOutput(),
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                UseManualAssembly = useManualAssembly
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                var assemblyOutput = new FailureMechanismSectionAssembly(0, assemblyCategoryGroup);
                calculator.SimpleAssessmentAssemblyOutput = assemblyOutput;
                calculator.DetailedAssessmentAssemblyOutput = assemblyOutput;
                calculator.TailorMadeAssessmentAssemblyOutput = assemblyOutput;
                calculator.CombinedAssemblyCategoryOutput = assemblyCategoryGroup;

                // Call
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection,
                                                                                       ConstructionProperties);

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
        public void Constructor_SectionResultWithoutCalculation_DetailedAssessmentProbabilityHasErrorText(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.AreEqual("Er moet een maatgevende berekening voor dit vak worden geselecteerd.",
                                resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable)]
        public void Constructor_SectionResultAndCalculationNotCalculated_DetailedAssessmentProbabilityHasErrorText(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new GrassCoverErosionInwardsCalculation(),
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.AreEqual("De maatgevende berekening voor dit vak moet nog worden uitgevoerd.",
                                resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable)]
        public void Constructor_SectionResultAndFailedCalculation_DetailedAssessmentProbabilityHasErrorText(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(double.NaN),
                                                            new TestDikeHeightOutput(double.NaN),
                                                            new TestOvertoppingRateOutput(double.NaN))
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = calculation,
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.AreEqual("De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.",
                                resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_SectionResultAndSuccessfulCalculation_DetailedAssessmentProbabilityNoError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = CreateCalculationWithOutput()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.AreEqual(sectionResult.GetDetailedAssessmentProbability(failureMechanism, assessmentSection), resultRow.DetailedAssessmentProbability);
                Assert.IsEmpty(resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_SectionResultWithManualAssemblyAndNotCalculated_DetailedAssessmentProbabilityNoError()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new GrassCoverErosionInwardsCalculation(),
                UseManualAssembly = true
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call 
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new GrassCoverErosionInwardsCalculation(),
                DetailedAssessmentResult = DetailedAssessmentProbabilityOnlyResultType.NotAssessed
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.IsNaN(resultRow.DetailedAssessmentProbability);
                Assert.IsEmpty(resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(nameof(SimpleAssessmentResultIsSufficientVariousSectionResults))]
        public void Constructor_SectionResultAndAssessmentSimpleAssessmentSufficient_DetailedAssessmentProbabilityNoError(
            GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                Assert.AreEqual(sectionResult.GetDetailedAssessmentProbability(failureMechanism, assessmentSection), resultRow.DetailedAssessmentProbability);
                Assert.IsEmpty(resultRow.ColumnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex].ErrorText);
                mocks.VerifyAll();
            }
        }

        private static IEnumerable<TestCaseData> SimpleAssessmentResultIsSufficientVariousSectionResults()
        {
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            yield return new TestCaseData(new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.NotApplicable
            }).SetName("SectionWithoutCalculation");
            yield return new TestCaseData(new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.NotApplicable,
                Calculation = new GrassCoverErosionInwardsCalculation()
            }).SetName("SectionWithCalculationNoOutput");
            yield return new TestCaseData(new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.NotApplicable,
                Calculation = new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(double.NaN),
                                                                new TestDikeHeightOutput(double.NaN),
                                                                new TestOvertoppingRateOutput(double.NaN))
                }
            }).SetName("SectionWithInvalidCalculationOutput");
            yield return new TestCaseData(new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.NotApplicable,
                Calculation = CreateCalculationWithOutput()
            }).SetName("SectionWithValidCalculationOutput");
        }

        #endregion

        #region Registration

        [Test]
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result,
                                                                                       failureMechanism,
                                                                                       assessmentSection,
                                                                                       ConstructionProperties);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                row.DetailedAssessmentResult = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResult);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentProbability_NoCalculationSet_ReturnNaN()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(sectionResult.Calculation);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                double detailedAssessmentProbability = resultRow.DetailedAssessmentProbability;

                // Assert
                Assert.IsNaN(detailedAssessmentProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(CalculationScenarioStatus.Failed)]
        [TestCase(CalculationScenarioStatus.NotCalculated)]
        public void DetailedAssessmentProbability_CalculationNotDone_ReturnNaN(CalculationScenarioStatus status)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            if (status == CalculationScenarioStatus.Failed)
            {
                calculation.Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(double.NaN),
                                                                        new TestDikeHeightOutput(double.NaN),
                                                                        new TestOvertoppingRateOutput(double.NaN));
            }

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                double detailedAssessmentProbability = resultRow.DetailedAssessmentProbability;

                // Assert
                Assert.IsNaN(detailedAssessmentProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentProbability_CalculationSuccessful_ReturnDetailedAssessmentProbability()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0.95),
                                                            new TestDikeHeightOutput(0),
                                                            new TestOvertoppingRateOutput(0))
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    sectionResult, failureMechanism, assessmentSection, ConstructionProperties);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

                // Call
                TestDelegate test = () => row.TailorMadeAssessmentProbability = value;

                // Assert
                const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}