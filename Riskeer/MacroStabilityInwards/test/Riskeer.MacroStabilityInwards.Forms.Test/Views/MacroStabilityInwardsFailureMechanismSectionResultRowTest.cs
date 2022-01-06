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
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.Probability;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.Views;

namespace Riskeer.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionResultRowTest
    {
        private static MacroStabilityInwardsFailureMechanismSectionResultRow.ConstructionProperties ConstructionProperties =>
            new MacroStabilityInwardsFailureMechanismSectionResultRow.ConstructionProperties
            {
                InitialFailureMechanismResultIndex = 2,
                InitialFailureMechanismResultProfileProbabilityIndex = 3,
                InitialFailureMechanismResultSectionProbabilityIndex = 4,
                FurtherAnalysisNeededIndex = 5,
                ProbabilityRefinementTypeIndex = 6,
                RefinedProfileProbabilityIndex = 7,
                RefinedSectionProbabilityIndex = 8,
                ProfileProbabilityIndex = 9,
                SectionProbabilityIndex = 10,
                SectionNIndex = 11,
                AssemblyGroupIndex = 12
            };

        [Test]
        public void Constructor_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            void Call() => new MacroStabilityInwardsFailureMechanismSectionResultRow(result, null, new MacroStabilityInwardsFailureMechanism(),
                                                                                     new AssessmentSectionStub(), ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            void Call() => new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(), null,
                                                                                     new AssessmentSectionStub(), ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            void Call() => new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                     new MacroStabilityInwardsFailureMechanism(), null, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            void Call() => new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                     new MacroStabilityInwardsFailureMechanism(), new AssessmentSectionStub(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section)
            {
                ProbabilityRefinementType = ProbabilityRefinementType.Both
            };

            MacroStabilityInwardsCalculationScenario[] calculationScenarios =
            {
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(section)
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, calculationScenarios, failureMechanism,
                                                                                    new AssessmentSectionStub(), ConstructionProperties);

                // Assert
                double initialFailureMechanismResultProbability = result.GetInitialFailureMechanismResultProbability(
                    calculationScenarios, failureMechanism.GeneralInput.ModelFactor);

                Assert.IsInstanceOf<FailureMechanismSectionResultRow<MacroStabilityInwardsFailureMechanismSectionResult>>(row);
                Assert.AreEqual(result.IsRelevant, row.IsRelevant);
                Assert.AreEqual(result.InitialFailureMechanismResult, row.InitialFailureMechanismResult);
                Assert.AreEqual(initialFailureMechanismResultProbability, row.InitialFailureMechanismResultProfileProbability);
                Assert.AreEqual(initialFailureMechanismResultProbability * failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.GetN(section.Length),
                                row.InitialFailureMechanismResultSectionProbability);
                Assert.AreEqual(result.FurtherAnalysisNeeded, row.FurtherAnalysisNeeded);
                Assert.AreEqual(result.ProbabilityRefinementType, row.ProbabilityRefinementType);
                Assert.AreEqual(result.RefinedProfileProbability, row.RefinedProfileProbability);
                Assert.AreEqual(result.RefinedSectionProbability, row.RefinedSectionProbability);

                TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.InitialFailureMechanismResultProfileProbability));
                TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.InitialFailureMechanismResultSectionProbability));
                TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.RefinedProfileProbability));
                TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.RefinedSectionProbability));
                TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.ProfileProbability));
                TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.SectionProbability));
                TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoValueDoubleConverter>(
                    nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.SectionN));
            }
        }

        [Test]
        [TestCase(InitialFailureMechanismResultType.Manual)]
        [TestCase(InitialFailureMechanismResultType.NoFailureProbability)]
        public void GivenRowWithInitialFailureMechanismResultAdopt_WhenValueChanged_ThenInitialProbabilitiesChanged(InitialFailureMechanismResultType newValue)
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario[] calculationScenarios =
            {
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(section)
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            double profileProbability = result.GetInitialFailureMechanismResultProbability(
                calculationScenarios, failureMechanism.GeneralInput.ModelFactor);
            double sectionProbability = profileProbability * failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.GetN(section.Length);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, calculationScenarios, failureMechanism,
                                                                                    new AssessmentSectionStub(), ConstructionProperties);

                // Precondition
                Assert.AreEqual(profileProbability, row.InitialFailureMechanismResultProfileProbability);
                Assert.AreEqual(sectionProbability, row.InitialFailureMechanismResultSectionProbability);

                // When
                row.InitialFailureMechanismResult = newValue;

                // Then
                Assert.AreEqual(result.ManualInitialFailureMechanismResultProfileProbability, row.InitialFailureMechanismResultProfileProbability);
                Assert.AreEqual(result.ManualInitialFailureMechanismResultSectionProbability, row.InitialFailureMechanismResultSectionProbability);
            }
        }

        [Test]
        [TestCase(ProbabilityRefinementType.Profile, double.NaN, "<afgeleid>")]
        [TestCase(ProbabilityRefinementType.Section, "<afgeleid>", double.NaN)]
        public void GivenRowWithProbabilityRefinementType_WhenValueChanged_ThenInitialProbabilitiesChanged(ProbabilityRefinementType newValue, object newProfileValue, object newSectionValue)
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section)
            {
                ProbabilityRefinementType = ProbabilityRefinementType.Both
            };

            MacroStabilityInwardsCalculationScenario[] calculationScenarios =
            {
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(section)
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, calculationScenarios, failureMechanism,
                                                                                    new AssessmentSectionStub(), ConstructionProperties);

                // Precondition
                Assert.AreEqual(result.RefinedProfileProbability, row.RefinedProfileProbability);
                Assert.AreEqual(result.RefinedSectionProbability, row.RefinedSectionProbability);

                // When
                row.ProbabilityRefinementType = newValue;

                // Then
                Assert.AreEqual(newProfileValue, row.RefinedProfileProbability);
                Assert.AreEqual(newSectionValue, row.RefinedSectionProbability);
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
        public void InitialFailureMechanismResultProfileProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.InitialFailureMechanismResultProfileProbability = newValue,
                result => result.ManualInitialFailureMechanismResultProfileProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetInvalidProbabilities))]
        public void InitialFailureMechanismResultProfileProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.InitialFailureMechanismResultProfileProbability = value);
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
        public void ProbabilityRefinementType_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const ProbabilityRefinementType newValue = ProbabilityRefinementType.Both;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.ProbabilityRefinementType = newValue,
                result => result.ProbabilityRefinementType,
                newValue);
        }

        [Test]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetValidProbabilities))]
        public void RefinedProfileProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.RefinedProfileProbability = newValue,
                result => result.RefinedProfileProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(typeof(ProbabilityTestHelper), nameof(ProbabilityTestHelper.GetInvalidProbabilities))]
        public void RefinedProfileProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.RefinedProfileProbability = value);
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
            Action<MacroStabilityInwardsFailureMechanismSectionResultRow> setPropertyAction,
            Func<MacroStabilityInwardsFailureMechanismSectionResult, T> assertPropertyFunc,
            T newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                    new MacroStabilityInwardsFailureMechanism(), new AssessmentSectionStub(),
                                                                                    ConstructionProperties);

                // Call
                setPropertyAction(row);

                // Assert
                Assert.AreEqual(newValue, assertPropertyFunc(result));
            }

            mocks.VerifyAll();
        }

        private static void ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(
            Action<MacroStabilityInwardsFailureMechanismSectionResultRow> setPropertyAction)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                    new MacroStabilityInwardsFailureMechanism(), new AssessmentSectionStub(),
                                                                                    ConstructionProperties);

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
        public void Constructor_AssemblyRan_ReturnCategoryGroups()
        {
            // Setup
            var random = new Random(39);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.FailureMechanismSectionAssemblyResultOutput = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                // Call
                var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                    new MacroStabilityInwardsFailureMechanism(), new AssessmentSectionStub(),
                                                                                    ConstructionProperties);

                // Assert
                FailureMechanismSectionAssemblyResult calculatorOutput = calculator.FailureMechanismSectionAssemblyResultOutput;
                FailureMechanismSectionAssemblyResult rowAssemblyResult = row.AssemblyResult;
                AssertFailureMechanismSectionAssemblyResult(calculatorOutput, rowAssemblyResult);

                Assert.AreEqual(rowAssemblyResult.ProfileProbability, row.ProfileProbability);
                Assert.AreEqual(rowAssemblyResult.SectionProbability, row.SectionProbability);
                Assert.AreEqual(rowAssemblyResult.N, row.SectionN);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName(rowAssemblyResult.AssemblyGroup),
                                row.AssemblyGroup);
            }
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
    }
}