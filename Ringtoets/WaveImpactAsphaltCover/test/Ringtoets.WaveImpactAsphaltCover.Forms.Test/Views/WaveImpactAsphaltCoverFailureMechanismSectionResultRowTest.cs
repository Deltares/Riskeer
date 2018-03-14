﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.Views;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.Views
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismSectionResultRowTest
    {
        private static WaveImpactAsphaltCoverFailureMechanismSectionResultRow.ConstructionProperties ConstructionProperties
        {
            get
            {
                return new WaveImpactAsphaltCoverFailureMechanismSectionResultRow.ConstructionProperties
                {
                    SimpleAssessmentResultIndex = 1,
                    DetailedAssessmentResultForFactorizedSignalingNormIndex = 2,
                    DetailedAssessmentResultForSignalingNormIndex = 3,
                    DetailedAssessmentResultForMechanismSpecificLowerLimitNormIndex = 4,
                    DetailedAssessmentResultForLowerLimitNormIndex = 5,
                    DetailedAssessmentResultForFactorizedLowerLimitNormIndex = 6,
                    TailorMadeAssessmentResultIndex = 7,
                    SimpleAssemblyCategoryGroupIndex = 8,
                    DetailedAssemblyCategoryGroupIndex = 9,
                    TailorMadeAssemblyCategoryGroupIndex = 10,
                    CombinedAssemblyCategoryGroupIndex = 11,
                    ManualAssemblyCategoryGroupIndex = 13
                };
            }
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionResultRow<WaveImpactAsphaltCoverFailureMechanismSectionResult>>(row);
                Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
                Assert.AreEqual(result.DetailedAssessmentResultForFactorizedSignalingNorm, row.DetailedAssessmentResultForFactorizedSignalingNorm);
                Assert.AreEqual(result.DetailedAssessmentResultForSignalingNorm, row.DetailedAssessmentResultForSignalingNorm);
                Assert.AreEqual(result.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm, row.DetailedAssessmentResultForSignalingNorm);
                Assert.AreEqual(result.DetailedAssessmentResultForLowerLimitNorm, row.DetailedAssessmentResultForLowerLimitNorm);
                Assert.AreEqual(result.DetailedAssessmentResultForFactorizedLowerLimitNorm, row.DetailedAssessmentResultForFactorizedLowerLimitNorm);
                Assert.AreEqual(SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertTo(result.TailorMadeAssessmentResult),
                                row.TailorMadeAssessmentResult);
                Assert.AreEqual(result.UseManualAssemblyCategoryGroup, row.UseManualAssemblyCategoryGroup);
                Assert.AreEqual(SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertTo(result.ManualAssemblyCategoryGroup),
                                row.ManualAssemblyCategoryGroup);

                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(12, columnStateDefinitions.Count);

                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SimpleAssessmentResultIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForFactorizedSignalingNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForSignalingNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForMechanismSpecificLowerLimitNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForLowerLimitNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssessmentResultForFactorizedLowerLimitNormIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.TailorMadeAssessmentResultIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.SimpleAssemblyCategoryGroupIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.DetailedAssemblyCategoryGroupIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.CombinedAssemblyCategoryGroupIndex);
                FailureMechanismSectionResultRowTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.ManualAssemblyCategoryGroupIndex);
            }
        }

        [Test]
        public void Constructor_AssemblyRan_ReturnCategoryGroups()
        {
            // Setup
            var random = new Random(39);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.DetailedAssessmentAssemblyGroupOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.TailorMadeAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

                // Call
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);
                // Assert
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.SimpleAssessmentAssemblyOutput.Group),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.DetailedAssessmentAssemblyGroupOutput.Value),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.TailorMadeAssemblyCategoryOutput.Value),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.CombinedAssemblyCategoryOutput.Value),
                                row.CombinedAssemblyCategoryGroup);
            }
        }

        [Test]
        public void Constructor_AssemblyThrowsException_ExpectedColumnStates()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

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
        public void UseManualAssemblyCategoryGroup_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            bool newValue = !result.UseManualAssemblyCategoryGroup;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Precondition
                Assert.IsFalse(result.UseManualAssemblyCategoryGroup);

                // Call
                row.UseManualAssemblyCategoryGroup = newValue;

                // Assert
                Assert.AreEqual(newValue, result.UseManualAssemblyCategoryGroup);
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
            var newValue = random.NextEnumValue<SelectableFailureMechanismSectionAssemblyCategoryGroup>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Call
                row.ManualAssemblyCategoryGroup = newValue;

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedCategoryGroup = SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertFrom(newValue);
                Assert.AreEqual(expectedCategoryGroup, result.ManualAssemblyCategoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenRowWithoutAssemblyErrors_WhenUpdatingAndAssemblyThrowsException_ThenAssemblyGroupSetToNone()
        {
            // Given
            var random = new Random(39);
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
                calculator.DetailedAssessmentAssemblyGroupOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.TailorMadeAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
                calculator.CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Precondition
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.SimpleAssessmentAssemblyOutput.Group),
                                row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.DetailedAssessmentAssemblyGroupOutput.Value),
                                row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.TailorMadeAssemblyCategoryOutput.Value),
                                row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(calculator.CombinedAssemblyCategoryOutput.Value),
                                row.CombinedAssemblyCategoryGroup);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther;

                // Then
                string expectedAssemblyDisplayName = FailureMechanismSectionResultRowHelper.GetCategoryGroupDisplayname(FailureMechanismSectionAssemblyCategoryGroup.None);
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
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

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
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible, false)]
        [TestCase(SimpleAssessmentResultType.NotApplicable, false)]
        [TestCase(SimpleAssessmentResultType.AssessFurther, true)]
        public void Constructor_WithSimpleAssessmentResultSet_ExpectedColumnStates(SimpleAssessmentResultType simpleAssessmentResult,
                                                                                   bool cellsEnabled)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForFactorizedSignalingNormIndex],
                                                                             cellsEnabled);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForSignalingNormIndex],
                                                                             cellsEnabled);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForMechanismSpecificLowerLimitNormIndex],
                                                                             cellsEnabled);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForLowerLimitNormIndex],
                                                                             cellsEnabled);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForFactorizedLowerLimitNormIndex],
                                                                             cellsEnabled);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex],
                                                                             cellsEnabled);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithUseManualAssemblyCategoryGroupSet_ExpectedColumnStates(bool useManualAssemblyCategoryGroup)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section)
            {
                UseManualAssemblyCategoryGroup = useManualAssemblyCategoryGroup
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.SimpleAssessmentResultIndex],
                                                                             !useManualAssemblyCategoryGroup);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForFactorizedSignalingNormIndex],
                                                                             !useManualAssemblyCategoryGroup);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForSignalingNormIndex],
                                                                             !useManualAssemblyCategoryGroup);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForMechanismSpecificLowerLimitNormIndex],
                                                                             !useManualAssemblyCategoryGroup);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForLowerLimitNormIndex],
                                                                             !useManualAssemblyCategoryGroup);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultForFactorizedLowerLimitNormIndex],
                                                                             !useManualAssemblyCategoryGroup);
                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex],
                                                                             !useManualAssemblyCategoryGroup);

                if (useManualAssemblyCategoryGroup)
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

                FailureMechanismSectionResultRowTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.ManualAssemblyCategoryGroupIndex],
                                                                             useManualAssemblyCategoryGroup);
            }
        }

        [Test]
        [TestCaseSource(typeof(FailureMechanismSectionResultRowTestHelper), nameof(FailureMechanismSectionResultRowTestHelper.CategoryGroupColorCases))]
        public void Constructor_WithAssemblyCategoryGroupsSet_ExpectedColumnStates(FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup,
                                                                                   Color expectedBackgroundColor)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(0, assemblyCategoryGroup);
                calculator.DetailedAssessmentAssemblyGroupOutput = assemblyCategoryGroup;
                calculator.TailorMadeAssemblyCategoryOutput = assemblyCategoryGroup;
                calculator.CombinedAssemblyCategoryOutput = assemblyCategoryGroup;

                // Call
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                FailureMechanismSectionResultRowTestHelper.AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex],
                                                                                      expectedBackgroundColor);
                FailureMechanismSectionResultRowTestHelper.AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex],
                                                                                      expectedBackgroundColor);
                FailureMechanismSectionResultRowTestHelper.AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex],
                                                                                      expectedBackgroundColor);
                FailureMechanismSectionResultRowTestHelper.AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex],
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
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Call
                row.SimpleAssessmentResult = newValue;

                // Assert
                Assert.AreEqual(newValue, result.SimpleAssessmentResult);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForFactorizedSignalingNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForFactorizedSignalingNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForFactorizedSignalingNorm);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForSignalingNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForSignalingNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForSignalingNorm);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForMechanismSpecificLowerLimitNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForLowerLimitNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForLowerLimitNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForLowerLimitNorm);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void DetailedAssessmentResultForFactorizedLowerLimitNorm_SetNewvalue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Call
                row.DetailedAssessmentResultForFactorizedLowerLimitNorm = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResultForFactorizedLowerLimitNorm);
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
            var newValue = random.NextEnumValue<SelectableFailureMechanismSectionAssemblyCategoryGroup>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new WaveImpactAsphaltCoverFailureMechanismSectionResultRow(result, ConstructionProperties);

                // Call
                row.TailorMadeAssessmentResult = newValue;

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedCategoryGroup = SelectableFailureMechanismSectionAssemblyCategoryGroupConverter.ConvertFrom(newValue);
                Assert.AreEqual(expectedCategoryGroup, result.TailorMadeAssessmentResult);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}