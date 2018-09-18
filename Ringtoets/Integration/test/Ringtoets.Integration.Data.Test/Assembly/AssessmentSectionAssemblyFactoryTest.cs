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
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class AssessmentSectionAssemblyFactoryTest
    {
        #region Assemble Failure Mechanisms With Probability

        [Test]
        public void AssembleFailureMechanismsWithProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(null, new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_WithAssessmentSection_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput = new FailureMechanismAssembly(
                    random.NextDouble(), random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection,
                                                                                          false);

                // Assert
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, assessmentSectionAssemblyCalculator.LowerLimitNormInput);
                Assert.AreEqual(failureMechanismContribution.SignalingNorm, assessmentSectionAssemblyCalculator.SignalingNormInput);

                AssertGroup1And2FailureMechanismInputs(assessmentSection,
                                                       failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput,
                                                       assessmentSectionAssemblyCalculator);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.AssembleFailureMechanismsAssemblyOutput = new FailureMechanismAssembly(
                    random.NextDouble(), random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

                // Call
                FailureMechanismAssembly output = AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection,
                                                                                                                            new Random(39).NextBoolean());

                // Assert
                Assert.AreSame(calculator.AssembleFailureMechanismsAssemblyOutput, output);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(CreateAssessmentSection(),
                                                                                                                    new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_FailureMechanismCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(CreateAssessmentSection(),
                                                                                                                    new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere toetssporen kan geen oordeel worden bepaald.", exception.Message);
            }
        }

        #region Manual Assembly Used

        #region Piping

        [Test]
        public void GivenAssessmentSectionWithPipingConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(probability, failureMechanismSectionAssemblyCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenAssessmentSectionWithPipingConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.IsNull(failureMechanismSectionAssemblyCalculator.ManualAssemblyAssemblyOutput);
            }
        }

        #endregion

        #region MacroStabilityInwards

        [Test]
        public void GivenAssessmentSectionWithMacroStabilityInwardsConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            MacroStabilityInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(probability, failureMechanismSectionAssemblyCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenAssessmentSectionWithMacroStabilityInwardsConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            MacroStabilityInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.IsNull(failureMechanismSectionAssemblyCalculator.ManualAssemblyAssemblyOutput);
            }
        }

        #endregion

        #region GrassCoverErosionInwards

        [Test]
        public void GivenAssessmentSectionWithGrassCoverErosionInwardsConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(probability, failureMechanismSectionAssemblyCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenAssessmentSectionWithGrassCoverErosionInwardsConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.IsNull(failureMechanismSectionAssemblyCalculator.ManualAssemblyAssemblyOutput);
            }
        }

        #endregion

        #region ClosingStructures

        [Test]
        public void GivenAssessmentSectionWithClosingStructuresConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(probability, failureMechanismSectionAssemblyCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenAssessmentSectionWithClosingStructuresConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.IsNull(failureMechanismSectionAssemblyCalculator.ManualAssemblyAssemblyOutput);
            }
        }

        #endregion

        #region HeightStructures

        [Test]
        public void GivenAssessmentSectionWithHeightStructuresConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(probability, failureMechanismSectionAssemblyCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenAssessmentSectionWithHeightStructuresConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.IsNull(failureMechanismSectionAssemblyCalculator.ManualAssemblyAssemblyOutput);
            }
        }

        #endregion

        #region StabilityPointStructures

        [Test]
        public void GivenAssessmentSectionWithStabilityPointStructuresConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(probability, failureMechanismSectionAssemblyCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void GivenAssessmentSectionWithStabilityPointStructuresConfigured_WhenAssemblingFailureMechanismsWithProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.IsNull(failureMechanismSectionAssemblyCalculator.ManualAssemblyAssemblyOutput);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Assemble Failure Mechanisms Without Probability

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(null,
                                                                                                                   new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_WithAssessmentSection_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Assert
                AssertGroup3And4FailureMechanismInputs(assessmentSection,
                                                       failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput.Value,
                                                       assessmentSectionAssemblyCalculator);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.AssembleFailureMechanismsAssemblyCategoryGroupOutput = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

                // Call
                FailureMechanismAssemblyCategoryGroup output = AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Assert
                Assert.AreEqual(calculator.AssembleFailureMechanismsAssemblyCategoryGroupOutput, output);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(CreateAssessmentSection(),
                                                                                                                       new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_FailureMechanismCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(CreateAssessmentSection(),
                                                                                                                       new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere toetssporen kan geen oordeel worden bepaald.", exception.Message);
            }
        }

        #region Manual Assembly Used

        #region DuneErosion

        [Test]
        public void GivenAssessmentSectionWithDuneErosionConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithDuneErosionConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region GrassCoverErosionOutwards

        [Test]
        public void GivenAssessmentSectionWithGrassCoverErosionOutwardsConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            GrassCoverErosionOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithGrassCoverErosionOutwardsConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            GrassCoverErosionOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region StabilityStoneCover

        [Test]
        public void GivenAssessmentSectionWithStabilityStoneCoverConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            StabilityStoneCoverFailureMechanism failureMechanism = assessmentSection.StabilityStoneCover;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            StabilityStoneCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithStabilityStoneCoverConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            StabilityStoneCoverFailureMechanism failureMechanism = assessmentSection.StabilityStoneCover;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            StabilityStoneCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region WaveImpactAsphaltCover

        [Test]
        public void GivenAssessmentSectionWithWaveImpactAsphaltCoverConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaveImpactAsphaltCover;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithWaveImpactAsphaltCoverConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaveImpactAsphaltCover;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region GrassCoverSlipOffInwards

        [Test]
        public void GivenAssessmentSectionWithGrassCoverSlipOffInwardsConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithGrassCoverSlipOffInwardsConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region GrassCoverSlipOffOutwards

        [Test]
        public void GivenAssessmentSectionWithGrassCoverSlipOffOutwardsConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithGrassCoverSlipOffOutwardsConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region MacroStabilityOutwards

        [Test]
        public void GivenAssessmentSectionWithMacroStabilityOutwardsConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            MacroStabilityOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithMacroStabilityOutwardsConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            MacroStabilityOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region Microstability

        [Test]
        public void GivenAssessmentSectionWithMicrostabilityConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithMicrostabilityConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region PipingStructure

        [Test]
        public void GivenAssessmentSectionWithPipingStructureConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithPipingStructureConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region StrengthStabilityLengthwiseConstruction

        [Test]
        public void GivenAssessmentSectionWithStrengthStabilityLengthwiseConstructionConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithStrengthStabilityLengthwiseConstructionConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region TechnicalInnovation

        [Test]
        public void GivenAssessmentSectionWithTechnicalInnovationConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            TechnicalInnovationFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithTechnicalInnovationConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            TechnicalInnovationFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #region WaterPressureAsphaltCover

        [Test]
        public void GivenAssessmentSectionWithWaterPressureAsphaltCoverConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            WaterPressureAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaterPressureAsphaltCover;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void GivenAssessmentSectionWithWaterPressureAsphaltCoverConfigured_WhenAssemblingFailureMechanismsWithoutProbabilityAndUseManualFalse_ThenNoInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            WaterPressureAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaterPressureAsphaltCover;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyCategoryGroup = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        private static AssessmentSection CreateIrrelevantAssessmentSection()
        {
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm => fm.IsRelevant = false);
            return assessmentSection;
        }

        #endregion

        #endregion

        #region Assemble Assessment Section

        [Test]
        public void AssembleAssessmentSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(null, new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleAssessmentSection_WithAssessmentSection_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyOutput = new FailureMechanismAssembly(
                    random.NextDouble(), random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
                assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyCategoryGroupOutput = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

                // Call
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection, false);

                // Assert
                AssertGroup1And2FailureMechanismInputs(assessmentSection,
                                                       failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput,
                                                       assessmentSectionAssemblyCalculator);

                AssertGroup3And4FailureMechanismInputs(assessmentSection,
                                                       failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput.Value,
                                                       assessmentSectionAssemblyCalculator);

                Assert.AreSame(assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyOutput,
                               assessmentSectionAssemblyCalculator.FailureMechanismsWithProbabilityInput);
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyCategoryGroupOutput,
                                assessmentSectionAssemblyCalculator.FailureMechanismsWithoutProbabilityInput);
            }
        }

        [Test]
        public void AssembleAssessmentSection_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.AssembleFailureMechanismsAssemblyOutput = new FailureMechanismAssembly(
                    random.NextDouble(), random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
                calculator.AssembleAssessmentSectionCategoryGroupOutput = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

                // Call
                AssessmentSectionAssemblyCategoryGroup output = AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection, false);

                // Assert
                Assert.AreEqual(calculator.AssembleAssessmentSectionCategoryGroupOutput, output);
            }
        }

        [Test]
        public void AssembleAssessmentSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(CreateAssessmentSection(),
                                                                                                     new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_FailureMechanismCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(CreateAssessmentSection(),
                                                                                                     new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere toetssporen kan geen oordeel worden bepaald.", exception.Message);
            }
        }

        #endregion

        #region Assemble Combined Per Failure Mechanism Section

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(null,
                                                                                                                  new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_WithAssessmentSection_SetsInputOnCalculator()
        {
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.CombinedFailureMechanismSectionAssemblyOutput = new CombinedFailureMechanismSectionAssembly[0];

                // Call
                AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection, false);

                // Assert
                IEnumerable<CombinedAssemblyFailureMechanismSection>[] actualInput = calculator.CombinedFailureMechanismSectionsInput.ToArray();
                IEnumerable<CombinedAssemblyFailureMechanismSection>[] expectedInput = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, assessmentSection.GetFailureMechanisms(), false).ToArray();
                Assert.AreEqual(expectedInput.Length, actualInput.Length);

                for (var i = 0; i < expectedInput.Length; i++)
                {
                    CombinedAssemblyFailureMechanismSection[] actualSections = actualInput[i].ToArray();
                    CombinedAssemblyFailureMechanismSection[] expectedSections = expectedInput[i].ToArray();
                    Assert.AreEqual(expectedSections.Length, actualSections.Length);

                    for (var j = 0; j < expectedSections.Length; j++)
                    {
                        Assert.AreEqual(expectedSections[j].SectionStart, actualSections[j].SectionStart);
                        Assert.AreEqual(expectedSections[j].SectionEnd, actualSections[j].SectionEnd);
                        Assert.AreEqual(expectedSections[j].CategoryGroup, actualSections[j].CategoryGroup);
                    }
                }
            }
        }

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_AssemblyRan_ReturnsOutput()
        {
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.CombinedFailureMechanismSectionAssemblyOutput = new[]
                {
                    CreateCombinedFailureMechanismSectionAssembly(assessmentSection, 20),
                    CreateCombinedFailureMechanismSectionAssembly(assessmentSection, 21)
                };

                // Call
                CombinedFailureMechanismSectionAssemblyResult[] output = AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection,
                                                                                                                                                     false)
                                                                                                         .ToArray();

                // Assert
                Dictionary<IFailureMechanism, int> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                        .Where(fm => fm.IsRelevant)
                                                                                        .Select((fm, i) => new
                                                                                        {
                                                                                            FailureMechanism = fm,
                                                                                            Index = i
                                                                                        })
                                                                                        .ToDictionary(x => x.FailureMechanism, x => x.Index);
                CombinedFailureMechanismSectionAssemblyResult[] expectedOutput = CombinedFailureMechanismSectionAssemblyResultFactory.Create(
                    calculator.CombinedFailureMechanismSectionAssemblyOutput, failureMechanisms, assessmentSection).ToArray();

                Assert.AreEqual(expectedOutput.Length, output.Length);
                for (var i = 0; i < expectedOutput.Length; i++)
                {
                    Assert.AreEqual(expectedOutput[i].SectionStart, output[i].SectionStart);
                    Assert.AreEqual(expectedOutput[i].SectionEnd, output[i].SectionEnd);
                    Assert.AreEqual(expectedOutput[i].TotalResult, output[i].TotalResult);
                    Assert.AreEqual(expectedOutput[i].Piping, output[i].Piping);
                    Assert.AreEqual(expectedOutput[i].GrassCoverErosionInwards, output[i].GrassCoverErosionInwards);
                    Assert.AreEqual(expectedOutput[i].MacroStabilityInwards, output[i].MacroStabilityInwards);
                    Assert.AreEqual(expectedOutput[i].MacroStabilityOutwards, output[i].MacroStabilityOutwards);
                    Assert.AreEqual(expectedOutput[i].Microstability, output[i].Microstability);
                    Assert.AreEqual(expectedOutput[i].StabilityStoneCover, output[i].StabilityStoneCover);
                    Assert.AreEqual(expectedOutput[i].WaveImpactAsphaltCover, output[i].WaveImpactAsphaltCover);
                    Assert.AreEqual(expectedOutput[i].WaterPressureAsphaltCover, output[i].WaterPressureAsphaltCover);
                    Assert.AreEqual(expectedOutput[i].GrassCoverErosionOutwards, output[i].GrassCoverErosionOutwards);
                    Assert.AreEqual(expectedOutput[i].GrassCoverSlipOffOutwards, output[i].GrassCoverSlipOffOutwards);
                    Assert.AreEqual(expectedOutput[i].GrassCoverSlipOffInwards, output[i].GrassCoverSlipOffInwards);
                    Assert.AreEqual(expectedOutput[i].HeightStructures, output[i].HeightStructures);
                    Assert.AreEqual(expectedOutput[i].ClosingStructures, output[i].ClosingStructures);
                    Assert.AreEqual(expectedOutput[i].PipingStructure, output[i].PipingStructure);
                    Assert.AreEqual(expectedOutput[i].StabilityPointStructures, output[i].StabilityPointStructures);
                    Assert.AreEqual(expectedOutput[i].StrengthStabilityLengthwiseConstruction, output[i].StrengthStabilityLengthwiseConstruction);
                    Assert.AreEqual(expectedOutput[i].DuneErosion, output[i].DuneErosion);
                    Assert.AreEqual(expectedOutput[i].TechnicalInnovation, output[i].TechnicalInnovation);
                }
            }
        }

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection,
                                                                                                                      new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_FailureMechanismSectionCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection,
                                                                                                                      new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere toetssporen kan geen oordeel worden bepaald.", exception.Message);
            }
        }

        #endregion

        #region Helpers

        private static CombinedFailureMechanismSectionAssembly CreateCombinedFailureMechanismSectionAssembly(AssessmentSection assessmentSection, int seed)
        {
            var random = new Random(seed);
            return new CombinedFailureMechanismSectionAssembly(new CombinedAssemblyFailureMechanismSection(random.NextDouble(),
                                                                                                           random.NextDouble(),
                                                                                                           random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                                                               assessmentSection.GetFailureMechanisms()
                                                                                .Where(fm => fm.IsRelevant)
                                                                                .Select(fm => random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()).ToArray());
        }

        private static void AssertGroup1And2FailureMechanismInputs(AssessmentSection assessmentSection,
                                                                   FailureMechanismAssembly expectedFailureMechanismAssembly,
                                                                   AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator)
        {
            IEnumerable<IFailureMechanism> expectedFailureMechanisms = GetExpectedGroup1And2FailureMechanisms(assessmentSection);
            IEnumerable<FailureMechanismAssembly> failureMechanismAssemblyInput = assessmentSectionAssemblyCalculator.FailureMechanismAssemblyInput;
            Assert.AreEqual(expectedFailureMechanisms.Count(), failureMechanismAssemblyInput.Count());
            foreach (FailureMechanismAssembly failureMechanismAssembly in failureMechanismAssemblyInput)
            {
                Assert.AreEqual(expectedFailureMechanismAssembly.Group, failureMechanismAssembly.Group);
                Assert.AreEqual(expectedFailureMechanismAssembly.Probability, failureMechanismAssembly.Probability);
            }
        }

        private static void AssertGroup3And4FailureMechanismInputs(AssessmentSection assessmentSection,
                                                                   FailureMechanismAssemblyCategoryGroup expectedAssemblyCategoryGroup,
                                                                   AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator)
        {
            IEnumerable<IFailureMechanism> expectedFailureMechanisms = GetExpectedGroup3And4FailureMechanisms(assessmentSection);
            IEnumerable<FailureMechanismAssemblyCategoryGroup> failureMechanismAssemblyInput =
                assessmentSectionAssemblyCalculator.FailureMechanismAssemblyCategoryGroupInput;
            Assert.AreEqual(expectedFailureMechanisms.Count(), failureMechanismAssemblyInput.Count());
            Assert.IsTrue(failureMechanismAssemblyInput.All(i => i == expectedAssemblyCategoryGroup));
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            var random = new Random(21);
            return new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
        }

        private static IEnumerable<IFailureMechanism> GetExpectedGroup1And2FailureMechanisms(AssessmentSection assessmentSection)
        {
            return new IFailureMechanism[]
            {
                assessmentSection.GrassCoverErosionInwards,
                assessmentSection.HeightStructures,
                assessmentSection.ClosingStructures,
                assessmentSection.StabilityPointStructures,
                assessmentSection.Piping,
                assessmentSection.MacroStabilityInwards
            };
        }

        private static IEnumerable<IFailureMechanism> GetExpectedGroup3And4FailureMechanisms(AssessmentSection assessmentSection)
        {
            return new IFailureMechanism[]
            {
                assessmentSection.StabilityStoneCover,
                assessmentSection.WaveImpactAsphaltCover,
                assessmentSection.GrassCoverErosionOutwards,
                assessmentSection.DuneErosion,
                assessmentSection.MacroStabilityOutwards,
                assessmentSection.Microstability,
                assessmentSection.WaterPressureAsphaltCover,
                assessmentSection.GrassCoverSlipOffOutwards,
                assessmentSection.GrassCoverSlipOffInwards,
                assessmentSection.PipingStructure,
                assessmentSection.StrengthStabilityLengthwiseConstruction,
                assessmentSection.TechnicalInnovation
            };
        }

        #endregion
    }
}