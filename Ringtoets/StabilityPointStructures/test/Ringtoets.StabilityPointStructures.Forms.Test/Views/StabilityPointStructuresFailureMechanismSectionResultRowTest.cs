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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Forms.Views;

namespace Ringtoets.StabilityPointStructures.Forms.Test.Views
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismSectionResultRowTest
    {
        private static StabilityPointStructuresFailureMechanismSectionResultRow.ConstructionProperties ConstructionProperties
        {
            get
            {
                return new StabilityPointStructuresFailureMechanismSectionResultRow.ConstructionProperties
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
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new StabilityPointStructuresFailureMechanismSectionResultRow(result, null, assessmentSection,
                                                                                                   ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new StabilityPointStructuresFailureMechanismSectionResultRow(
                result, new StabilityPointStructuresFailureMechanism(), null, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new StabilityPointStructuresFailureMechanismSectionResultRow(result,
                                                                                            new StabilityPointStructuresFailureMechanism(),
                                                                                            assessmentSection,
                                                                                            null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection,
                                                                                       ConstructionProperties);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionResultRow<StabilityPointStructuresFailureMechanismSectionResult>>(row);

                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(11, columnStateDefinitions.Count);

                for (var i = 1; i < 12; i++)
                {
                    Assert.IsTrue(columnStateDefinitions.ContainsKey(i));
                    Assert.IsNotNull(columnStateDefinitions[i]);
                }

                Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
                Assert.AreEqual(result.DetailedAssessmentResult, row.DetailedAssessmentResult);
                Assert.AreEqual(result.GetDetailedAssessmentProbability(failureMechanism, assessmentSection), row.DetailedAssessmentProbability);
                Assert.AreEqual(result.TailorMadeAssessmentResult, row.TailorMadeAssessmentResult);
                Assert.AreEqual(row.TailorMadeAssessmentProbability, result.TailorMadeAssessmentProbability);
                Assert.AreEqual(result.UseManualAssemblyProbability, row.UseManualAssemblyProbability);
                Assert.AreEqual(result.ManualAssemblyProbability, row.ManualAssemblyProbability);

                TestHelper.AssertTypeConverter<StabilityPointStructuresFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(StabilityPointStructuresFailureMechanismSectionResultRow.DetailedAssessmentProbability));
                TestHelper.AssertTypeConverter<StabilityPointStructuresFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(StabilityPointStructuresFailureMechanismSectionResultRow.TailorMadeAssessmentProbability));
                TestHelper.AssertTypeConverter<StabilityPointStructuresFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(StabilityPointStructuresFailureMechanismSectionResultRow.CombinedAssemblyProbability));
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
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

                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection,
                                                                                       ConstructionProperties);

                // Assert
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.SimpleAssessmentAssemblyOutput.Group),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.DetailedAssessmentAssemblyOutput.Group),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.TailorMadeAssessmentAssemblyOutput.Group),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.CombinedAssemblyOutput.Group),
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
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
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection,
                                                                                       ConstructionProperties);

                // Precondition
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.SimpleAssessmentAssemblyOutput.Group),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.DetailedAssessmentAssemblyOutput.Group),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.TailorMadeAssessmentAssemblyOutput.Group),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.CombinedAssemblyOutput.Group),
                                row.CombinedAssemblyCategoryGroup);
                Assert.AreEqual(calculator.CombinedAssemblyOutput.Probability, row.CombinedAssemblyProbability);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.SimpleAssessmentResult = SimpleAssessmentResultValidityOnlyType.Applicable;

                // Then
                string expectedAssemblyDisplayName = FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(FailureMechanismSectionAssemblyCategoryGroup.None);
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
            var random = new Random(39);
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory)AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;
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

                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection,
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
                row.SimpleAssessmentResult = SimpleAssessmentResultValidityOnlyType.Applicable;

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
        public void UseManualAssemblyProbability_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);
                bool originalValue = result.UseManualAssemblyProbability;
                bool newValue = !originalValue;

                // Call
                row.UseManualAssemblyProbability = newValue;

                // Assert
                Assert.AreEqual(newValue, result.UseManualAssemblyProbability);
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
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
        public void GetSectionResultCalculation_NoCalculationSetOnSectionResult_ReturnNull()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(result.Calculation);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection,
                                                                                       ConstructionProperties);

                // Call
                StructuresCalculation<StabilityPointStructuresInput> calculation = row.GetSectionResultCalculation();

                // Assert
                Assert.IsNull(calculation);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetSectionResultCalculation_WithCalculationSetOnSectionResult_ReturnCalculation()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var expectedCalculation = new StructuresCalculation<StabilityPointStructuresInput>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                Calculation = expectedCalculation
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection,
                                                                                       ConstructionProperties);

                // Call
                StructuresCalculation<StabilityPointStructuresInput> calculation = row.GetSectionResultCalculation();

                // Assert
                Assert.AreSame(expectedCalculation, calculation);
                mocks.VerifyAll();
            }
        }

        private static TestStabilityPointStructuresCalculation CreateCalculationWithOutput()
        {
            return new TestStabilityPointStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };
        }

        #region Column States

        [Test]
        [TestCase(SimpleAssessmentResultValidityOnlyType.None, true)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.NotApplicable, false)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable, true)]
        public void Constructor_WithSimpleAssessmentResultSet_ExpectedColumnStates(SimpleAssessmentResultValidityOnlyType simpleAssessmentResult,
                                                                                   bool cellsEnabled)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                Calculation = CreateCalculationWithOutput(),
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultIndex],
                                                                             cellsEnabled);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex],
                                                                             cellsEnabled,
                                                                             true);

                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex],
                                                                             cellsEnabled);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex],
                                                                             cellsEnabled);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(DetailedAssessmentResultType.NotAssessed, false)]
        [TestCase(DetailedAssessmentResultType.Probability, true)]
        public void Constructor_WithDetailedAssessmentResultSet_ExpectedColumnStates(DetailedAssessmentResultType detailedAssessmentResult,
                                                                                     bool cellEnabled)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                DetailedAssessmentResult = detailedAssessmentResult,
                Calculation = CreateCalculationWithOutput()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex],
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                TailorMadeAssessmentResult = tailorMadeAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex],
                                                                             cellEnabled);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithUseManualAssemblyProbability_ExpectedColumnStates(bool useManualAssemblyProbability)
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                Calculation = CreateCalculationWithOutput(),
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                UseManualAssemblyProbability = useManualAssemblyProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
                    result, failureMechanism, assessmentSection, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.SimpleAssessmentResultIndex],
                                                                             !useManualAssemblyProbability);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultIndex],
                                                                             !useManualAssemblyProbability);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex],
                                                                             !useManualAssemblyProbability, true);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex],
                                                                             !useManualAssemblyProbability);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex],
                                                                             !useManualAssemblyProbability);

                if (useManualAssemblyProbability)
                {
                    FailureMechanismSectionResultRowTestHelper.AssertColumnStateIsDisabled(
                        columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex]);
                    FailureMechanismSectionResultRowTestHelper.AssertColumnStateIsDisabled(
                        columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex]);
                    FailureMechanismSectionResultRowTestHelper.AssertColumnStateIsDisabled(
                        columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex]);
                    FailureMechanismSectionResultRowTestHelper.AssertColumnStateIsDisabled(
                        columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex]);
                }

                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.CombinedAssemblyProbabilityIndex],
                                                                             !useManualAssemblyProbability, true);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.ManualAssemblyProbabilityIndex],
                                                                             useManualAssemblyProbability);

                mocks.VerifyAll();
            }
        }

        #endregion

        #region Registration

        [Test]
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);
            result.Attach(observer);

            var newValue = new Random(21).NextEnumValue<SimpleAssessmentResultValidityOnlyType>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result,
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(sectionResult.Calculation);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRow(sectionResult, failureMechanism, assessmentSection,
                                                                                             ConstructionProperties);

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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            if (status == CalculationScenarioStatus.Failed)
            {
                calculation.Output = new TestStructuresOutput(double.NaN);
            }

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRow(sectionResult, failureMechanism, assessmentSection,
                                                                                             ConstructionProperties);

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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput(0.95)
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRow(sectionResult, failureMechanism, assessmentSection,
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
                    result, new StabilityPointStructuresFailureMechanism(), assessmentSection, ConstructionProperties);

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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new StabilityPointStructuresFailureMechanismSectionResultRow(
                    result, new StabilityPointStructuresFailureMechanism(), assessmentSection, ConstructionProperties);

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