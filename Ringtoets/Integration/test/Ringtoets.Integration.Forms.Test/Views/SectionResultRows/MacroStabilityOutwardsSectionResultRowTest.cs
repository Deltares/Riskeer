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
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRows
{
    [TestFixture]
    public class MacroStabilityOutwardsSectionResultRowTest
    {
        private static MacroStabilityOutwardsSectionResultRow.ConstructionProperties ConstructionProperties
        {
            get
            {
                return new MacroStabilityOutwardsSectionResultRow.ConstructionProperties
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
                    ManualAssemblyCategoryGroupIndex = 10
                };
            }
        }

        private static IEnumerable<TestCaseData> CategoryGroupColorCases
        {
            get
            {
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, Color.FromArgb(255, 255, 255));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.None, Color.FromArgb(255, 255, 255));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.Iv, Color.FromArgb(0, 255, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IIv, Color.FromArgb(118, 147, 60));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IIIv, Color.FromArgb(255, 255, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IVv, Color.FromArgb(204, 192, 218));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.Vv, Color.FromArgb(255, 153, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.VIv, Color.FromArgb(255, 0, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.VIIv, Color.FromArgb(255, 255, 255));
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
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new MacroStabilityOutwardsSectionResultRow(result, null, assessmentSection,
                                                                                 ConstructionProperties);

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
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), null,
                                                                                 ConstructionProperties);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new MacroStabilityOutwardsSectionResultRow(result, new MacroStabilityOutwardsFailureMechanism(), assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionResultRow<MacroStabilityOutwardsFailureMechanismSectionResult>>(row);

                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                Assert.AreEqual(10, columnStateDefinitions.Count);

                for (var i = 1; i < 11; i++)
                {
                    Assert.IsTrue(columnStateDefinitions.ContainsKey(i));
                    Assert.IsNotNull(columnStateDefinitions[i]);
                }

                AssertColumnStateIsEnabled(columnStateDefinitions[ConstructionProperties.SimpleAssessmentResultIndex]);
                AssertColumnStateIsEnabled(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultIndex]);
                AssertColumnStateIsEnabled(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex]);
                AssertColumnStateIsEnabled(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex]);
                AssertColumnStateIsDisabled(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex]);
                AssertColumnStateIsDisabled(columnStateDefinitions[ConstructionProperties.ManualAssemblyCategoryGroupIndex]);

                Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
                Assert.AreEqual(result.DetailedAssessmentResult, row.DetailedAssessmentResult);
                Assert.AreEqual(result.DetailedAssessmentProbability, row.DetailedAssessmentProbability);
                Assert.AreEqual(result.TailorMadeAssessmentResult, row.TailorMadeAssessmentResult);
                Assert.AreEqual(result.TailorMadeAssessmentProbability, row.TailorMadeAssessmentProbability);
                Assert.AreEqual(result.UseManualAssemblyCategoryGroup, row.UseManualAssemblyCategoryGroup);
                Assert.AreEqual(result.ManualAssemblyCategoryGroup, row.ManualAssemblyCategoryGroup);

                TestHelper.AssertTypeConverter<MacroStabilityOutwardsSectionResultRow,
                    NoProbabilityValueDoubleConverter>(
                    nameof(MacroStabilityOutwardsSectionResultRow.DetailedAssessmentProbability));
                TestHelper.AssertTypeConverter<MacroStabilityOutwardsSectionResultRow,
                    NoProbabilityValueDoubleConverter>(
                    nameof(MacroStabilityOutwardsSectionResultRow.TailorMadeAssessmentProbability));
                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_AssemblyRan_ReturnCategoryGroups()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

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
                calculator.CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

                // Call
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Assert
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(calculator.DetailedAssessmentAssemblyOutput.Group, row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(calculator.TailorMadeAssessmentAssemblyOutput.Group, row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(calculator.CombinedAssemblyCategoryOutput, row.CombinedAssemblyCategoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_AssemblyThrowsException_ExpectedColumnStates()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                const string expectedErrorText = "Message";

                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex].ErrorText);
                Assert.AreEqual(expectedErrorText, columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex].ErrorText);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void UseManualAssemblyCategoryGroup_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            bool newValue = !result.UseManualAssemblyCategoryGroup;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

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
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

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
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

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
                calculator.CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Precondition
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(calculator.DetailedAssessmentAssemblyOutput.Group, row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(calculator.TailorMadeAssessmentAssemblyOutput.Group, row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(calculator.CombinedAssemblyCategoryOutput, row.CombinedAssemblyCategoryGroup);

                // When
                calculator.ThrowExceptionOnCalculate = true;
                row.SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther;

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, row.SimpleAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, row.DetailedAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, row.TailorMadeAssemblyCategoryGroup);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, row.CombinedAssemblyCategoryGroup);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenRowWithAssemblyErrors_WhenUpdatingAndAssemblyDoesNotThrowException_ThenExpectedColumnStates()
        {
            // Given
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

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

                mocks.VerifyAll();
            }
        }

        private static void AssertColumnState(DataGridViewColumnStateDefinition columnStateDefinition, bool cellsEnabled)
        {
            if (cellsEnabled)
            {
                AssertColumnStateIsEnabled(columnStateDefinition);
            }
            else
            {
                AssertColumnStateIsDisabled(columnStateDefinition);
            }
        }

        private static void AssertColumnWithColorState(DataGridViewColumnStateDefinition columnStateDefinition,
                                                       Color expectedBackgroundColor)
        {
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), columnStateDefinition.Style.TextColor);
            Assert.AreEqual(expectedBackgroundColor, columnStateDefinition.Style.BackgroundColor);
        }

        private static void AssertColumnStateIsDisabled(DataGridViewColumnStateDefinition columnStateDefinition)
        {
            Assert.AreSame(CellStyle.Disabled, columnStateDefinition.Style);
            Assert.IsTrue(columnStateDefinition.ReadOnly);
            Assert.AreEqual(string.Empty, columnStateDefinition.ErrorText);
        }

        private static void AssertColumnStateIsEnabled(DataGridViewColumnStateDefinition columnStateDefinition)
        {
            Assert.AreSame(CellStyle.Enabled, columnStateDefinition.Style);
            Assert.IsFalse(columnStateDefinition.ReadOnly);
            Assert.AreEqual(string.Empty, columnStateDefinition.ErrorText);
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
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultIndex], cellsEnabled);
                AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex], cellsEnabled);
                AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex], cellsEnabled);
                AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex], cellsEnabled);

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
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section)
            {
                DetailedAssessmentResult = detailedAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex], cellEnabled);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.NotAssessed, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Insufficient, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.None, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Sufficient, false)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability, true)]
        public void Constructor_WithTailorMadeAssessmentResultSet_ExpectedColumnStates(
            TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult,
            bool cellEnabled)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section)
            {
                TailorMadeAssessmentResult = tailorMadeAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
                AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex], cellEnabled);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithUseManualAssemblyCategoryGroupSet_ExpectedColumnStates(bool useManualAssemblyCategoryGroup)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section)
            {
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability,
                UseManualAssemblyCategoryGroup = useManualAssemblyCategoryGroup
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                AssertColumnState(columnStateDefinitions[ConstructionProperties.SimpleAssessmentResultIndex], !useManualAssemblyCategoryGroup);
                AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentResultIndex], !useManualAssemblyCategoryGroup);
                AssertColumnState(columnStateDefinitions[ConstructionProperties.DetailedAssessmentProbabilityIndex], !useManualAssemblyCategoryGroup);
                AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentResultIndex], !useManualAssemblyCategoryGroup);
                AssertColumnState(columnStateDefinitions[ConstructionProperties.TailorMadeAssessmentProbabilityIndex], !useManualAssemblyCategoryGroup);

                if (useManualAssemblyCategoryGroup)
                {
                    AssertColumnStateIsDisabled(columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex]);
                    AssertColumnStateIsDisabled(columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex]);
                    AssertColumnStateIsDisabled(columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex]);
                    AssertColumnStateIsDisabled(columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex]);
                }

                AssertColumnState(columnStateDefinitions[ConstructionProperties.ManualAssemblyCategoryGroupIndex], useManualAssemblyCategoryGroup);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCaseSource(nameof(CategoryGroupColorCases))]
        public void Constructor_WithAssemblyCategoryGroupsSet_ExpectedColumnStates(FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup,
                                                                                   Color expectedBackgroundColor)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section)
            {
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                var assemblyOutput = new FailureMechanismSectionAssembly(0, assemblyCategoryGroup);
                calculator.SimpleAssessmentAssemblyOutput = assemblyOutput;
                calculator.DetailedAssessmentAssemblyOutput = assemblyOutput;
                calculator.TailorMadeAssessmentAssemblyOutput = assemblyOutput;
                calculator.CombinedAssemblyCategoryOutput = assemblyCategoryGroup;

                // Call
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Assert
                IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

                AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.SimpleAssemblyCategoryGroupIndex], expectedBackgroundColor);
                AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.DetailedAssemblyCategoryGroupIndex], expectedBackgroundColor);
                AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.TailorMadeAssemblyCategoryGroupIndex], expectedBackgroundColor);
                AssertColumnWithColorState(columnStateDefinitions[ConstructionProperties.CombinedAssemblyCategoryGroupIndex], expectedBackgroundColor);

                mocks.VerifyAll();
            }
        }

        #endregion

        #region Registration

        [Test]
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SimpleAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
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
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<DetailedAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Call
                row.DetailedAssessmentResult = newValue;

                // Assert
                Assert.AreEqual(newValue, result.DetailedAssessmentResult);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void DetailedAssessmentProbability_ValidValue_NotifyObserversAndPropertyChanged(double value)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Call
                row.DetailedAssessmentProbability = value;

                // Assert
                Assert.AreEqual(value, row.DetailedAssessmentProbability);
                mocks.VerifyAll();
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void DetailedAssessmentProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

                // Call
                TestDelegate test = () => row.DetailedAssessmentProbability = value;

                // Assert
                const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void TailorMadeAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

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
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

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
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityOutwardsSectionResultRow(result, failureMechanism, assessmentSection,
                                                                     ConstructionProperties);

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