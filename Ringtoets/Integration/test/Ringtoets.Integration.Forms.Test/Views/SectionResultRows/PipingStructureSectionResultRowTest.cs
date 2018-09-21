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
using System.Drawing;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRows
{
    [TestFixture]
    public class PipingStructureSectionResultRowTest

    {
        private static PipingStructureSectionResultRow.ConstructionProperties ConstructionProperties
        {
            get
            {
                return new PipingStructureSectionResultRow.ConstructionProperties
                {
                    SimpleAssessmentResultIndex = 1,
                    DetailedAssessmentResultIndex = 2,
                    TailorMadeAssessmentResultIndex = 3,
                    SimpleAssemblyCategoryGroupIndex = 4,
                    DetailedAssemblyCategoryGroupIndex = 5,
                    TailorMadeAssemblyCategoryGroupIndex = 6,
                    CombinedAssemblyCategoryGroupIndex = 7,
                    ManualAssemblyCategoryGroupIndex = 9
                };
            }
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new PipingStructureSectionResultRow(result, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionResultRow<PipingStructureFailureMechanismSectionResult>>(row);
                Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
                Assert.AreEqual(result.DetailedAssessmentResult, row.DetailedAssessmentResult);
                Assert.AreEqual(result.TailorMadeAssessmentResult, row.TailorMadeAssessmentResult);
                Assert.AreEqual(result.UseManualAssembly, row.UseManualAssembly);
                Assert.AreEqual(result.ManualAssemblyCategoryGroup, row.ManualAssemblyCategoryGroup);

                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(8, columnStateDefinitions.Count);

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SimpleAssessmentResultIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.TailorMadeAssessmentResultIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SimpleAssemblyCategoryGroupIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssemblyCategoryGroupIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.CombinedAssemblyCategoryGroupIndex);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.ManualAssemblyCategoryGroupIndex);
            }
        }

        [Test]
        public void Constructor_AssemblyRan_ReturnCategoryGroups()
        {
            // Setup
            var random = new Random(39);
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.DetailedAssessmentAssemblyGroupOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.TailorMadeAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

                // Call
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Assert
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.SimpleAssessmentAssemblyOutput.Group),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.DetailedAssessmentAssemblyGroupOutput.Value),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.TailorMadeAssemblyCategoryOutput.Value),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.CombinedAssemblyCategoryOutput.Value),
                                row.CombinedAssemblyCategoryGroup);
            }
        }

        [Test]
        public void Constructor_AssemblyThrowsException_ExpectedColumnStates()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                const string expectedErrorText = "Message";

                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex].ErrorText);
            }
        }

        [Test]
        public void UseManualAssembly_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);
            result.Attach(observer);

            bool newValue = !result.UseManualAssembly;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Precondition
                Assert.IsFalse(result.UseManualAssembly);

                // Call
                row.UseManualAssembly = newValue;

                // Assert
                Assert.AreEqual(newValue, result.UseManualAssembly);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ManualAssemblyCategoryGroup_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Call
                row.ManualAssemblyCategoryGroup = newValue;

                // Assert
                Assert.AreEqual(newValue, result.ManualAssemblyCategoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenAssemblyGroupSetToNone()
        {
            // Given
            var random = new Random(39);
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.DetailedAssessmentAssemblyGroupOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.TailorMadeAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Precondition
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.SimpleAssessmentAssemblyOutput.Group),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.DetailedAssessmentAssemblyGroupOutput.Value),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.TailorMadeAssemblyCategoryOutput.Value),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(calculator.CombinedAssemblyCategoryOutput.Value),
                                row.CombinedAssemblyCategoryGroup);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther;

                // Then
                string expectedAssemblyDisplayName = FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(FailureMechanismSectionAssemblyCategoryGroup.None);
                Assert.AreEqual(expectedAssemblyDisplayName, row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(expectedAssemblyDisplayName, row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(expectedAssemblyDisplayName, row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(expectedAssemblyDisplayName, row.CombinedAssemblyCategoryGroup);
            }
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndAssemblyDoesNotThrowException_ThenExpectedColumnStates()
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Precondition
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                const string expectedErrorText = "Message";

                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex].ErrorText);

                // When
                calculator.ThrowExceptionOnCalculate = false;
                row.SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther;

                // Then
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(string.Empty, columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex].ErrorText);
            }
        }

        #region Column States

        [Test]
        [TestCase(SimpleAssessmentResultType.None, true)]
        [TestCase(SimpleAssessmentResultType.AssessFurther, true)]
        [TestCase(SimpleAssessmentResultType.NotApplicable, false)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible, false)]
        public void Constructor_WithSimpleAssessmentResultSet_ExpectedColumnStates(SimpleAssessmentResultType simpleAssessmentResult,
                                                                                   bool cellsEnabled)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultIndex],
                                                                                     cellsEnabled);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex],
                                                                                     cellsEnabled);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithUseManualAssemblySet_ExpectedColumnStates(bool useManualAssembly)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section)
            {
                UseManualAssembly = useManualAssembly
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.SimpleAssessmentResultIndex],
                                                                                     !useManualAssembly);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultIndex],
                                                                                     !useManualAssembly);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex],
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

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.ManualAssemblyCategoryGroupIndex],
                                                                                     useManualAssembly);
            }
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.FailureMechanismSectionAssemblyCategoryGroupColorCases))]
        public void Constructor_WithAssemblyCategoryGroupsSet_ExpectedColumnStates(FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup,
                                                                                   Color expectedBackgroundColor)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                var assemblyOutput = new FailureMechanismSectionAssembly(0, assemblyCategoryGroup);
                calculator.SimpleAssessmentAssemblyOutput = assemblyOutput;
                calculator.DetailedAssessmentAssemblyGroupOutput = assemblyCategoryGroup;
                calculator.TailorMadeAssemblyCategoryOutput = assemblyCategoryGroup;
                calculator.CombinedAssemblyCategoryOutput = assemblyCategoryGroup;

                // Call
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex],
                                                                                              expectedBackgroundColor);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex],
                                                                                              expectedBackgroundColor);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex],
                                                                                              expectedBackgroundColor);
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex],
                                                                                              expectedBackgroundColor);
            }
        }

        #endregion

        #region Registration

        [Test]
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SimpleAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

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
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResult = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResult);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void TailorMadeAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<TailorMadeAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingStructureFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new PipingStructureSectionResultRow(result, ConstructionProperties);

                // Call
                row.TailorMadeAssessmentResult = newValue;

                // Assert
                Assert.AreEqual(newValue, result.TailorMadeAssessmentResult);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}