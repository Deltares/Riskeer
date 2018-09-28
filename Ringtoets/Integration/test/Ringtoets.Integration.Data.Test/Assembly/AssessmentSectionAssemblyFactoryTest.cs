// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.TestUtil;
using Ringtoets.Integration.TestUtil;
using Ringtoets.Piping.Data;
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
        public void AssembleFailureMechanismsWithProbability_WithAssessmentSectionWithoutManualSectionAssemblyResults_SetsInputOnCalculator()
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
                                                                                          random.NextBoolean());

                // Assert
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, assessmentSectionAssemblyCalculator.LowerLimitNormInput);
                Assert.AreEqual(failureMechanismContribution.SignalingNorm, assessmentSectionAssemblyCalculator.SignalingNormInput);
                Assert.AreEqual(assessmentSection.FailureProbabilityMarginFactor, assessmentSectionAssemblyCalculator.FailureProbabilityMarginFactorInput);

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
                                                                                                                            random.NextBoolean());

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

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithoutConfiguredFailureMechanismWithProbability))]
        public void AssembleFailureMechanismsWithProbability_FailureMechanismWithManualSectionAssemblyAndUseManualTrue_SetsManualAssemblyInputOnCalculator(AssessmentSection assessmentSection)
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, true);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreSame(failureMechanismSectionAssemblyCalculator.ManualAssemblyAssemblyOutput, failureMechanismAssemblyCalculator.FailureMechanismSectionAssemblies.Single());
            }
        }

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithoutConfiguredFailureMechanismWithProbability))]
        public void AssembleFailureMechanismsWithProbability_FailureMechanismWithManualSectionAssemblyAndUseManualFalse_SetsAssemblyInputOnCalculator(AssessmentSection assessmentSection)
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, false);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreSame(failureMechanismSectionAssemblyCalculator.CombinedAssemblyOutput, failureMechanismAssemblyCalculator.FailureMechanismSectionAssemblies.Single());
            }
        }

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
        public void AssembleFailureMechanismsWithoutProbability_WithAssessmentSectionWithoutManualSectionAssemblyResults_SetsInputOnCalculator()
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
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, random.NextBoolean());

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
                FailureMechanismAssemblyCategoryGroup output = AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection,
                                                                                                                                            random.NextBoolean());

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

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithConfiguredFailureMechanismsWithoutProbability))]
        public void AssembleFailureMechanismsWithoutProbability_FailureMechanismWithManualSectionAssemblyAndUseManualTrue_SetsManualAssemblyInputOnCalculator(AssessmentSection assessmentSection,
                                                                                                                                                              IFailureMechanism relevantFailureMechanism)
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                AssertFailureMechanismsWithoutProbabilityManualAssemblyCalculatorInput(relevantFailureMechanism, failureMechanismAssemblyCalculator);
            }
        }

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithoutConfiguredFailureMechanismWithoutProbability))]
        public void AssembleFailureMechanismsWithoutProbability_FailureMechanismWithManualSectionAssemblyAndUseManualFalse_SetsAssemblyInputOnCalculator(AssessmentSection assessmentSection)
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, false);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(failureMechanismSectionAssemblyCalculator.CombinedAssemblyCategoryOutput, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        private static void AssertFailureMechanismsWithoutProbabilityManualAssemblyCalculatorInput(IFailureMechanism failureMechanism,
                                                                                                   FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator)
        {
            var duneErosion = failureMechanism as DuneErosionFailureMechanism;
            if (duneErosion != null)
            {
                Assert.AreEqual(GetFailureMechanismSectionResult(duneErosion).ManualAssemblyCategoryGroup,
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var grassCoverErosionOutwards = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
            if (grassCoverErosionOutwards != null)
            {
                Assert.AreEqual(GetFailureMechanismSectionResult(grassCoverErosionOutwards).ManualAssemblyCategoryGroup,
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var stabilityStoneCover = failureMechanism as StabilityStoneCoverFailureMechanism;
            if (stabilityStoneCover != null)
            {
                Assert.AreEqual(GetFailureMechanismSectionResult(stabilityStoneCover).ManualAssemblyCategoryGroup,
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var waveImpactAsphaltCover = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
            if (waveImpactAsphaltCover != null)
            {
                Assert.AreEqual(GetFailureMechanismSectionResult(waveImpactAsphaltCover).ManualAssemblyCategoryGroup,
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var grassCoverSlipOffInwards = failureMechanism as GrassCoverSlipOffInwardsFailureMechanism;
            if (grassCoverSlipOffInwards != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(grassCoverSlipOffInwards).ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var grassCoverSlipOffOutwards = failureMechanism as GrassCoverSlipOffOutwardsFailureMechanism;
            if (grassCoverSlipOffOutwards != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(grassCoverSlipOffOutwards).ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var pipingStructure = failureMechanism as PipingStructureFailureMechanism;
            if (pipingStructure != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(pipingStructure).ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var strengthStabilityLengthwiseConstruction = failureMechanism as StrengthStabilityLengthwiseConstructionFailureMechanism;
            if (strengthStabilityLengthwiseConstruction != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(strengthStabilityLengthwiseConstruction).ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var technicalInnovation = failureMechanism as TechnicalInnovationFailureMechanism;
            if (technicalInnovation != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(technicalInnovation).ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var microStability = failureMechanism as MicrostabilityFailureMechanism;
            if (microStability != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(microStability).ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var macroStabilityOutwards = failureMechanism as MacroStabilityOutwardsFailureMechanism;
            if (macroStabilityOutwards != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(macroStabilityOutwards).ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            var waterPressureAsphaltCover = failureMechanism as WaterPressureAsphaltCoverFailureMechanism;
            if (waterPressureAsphaltCover != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(waterPressureAsphaltCover).ManualAssemblyCategoryGroup),
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
                return;
            }

            throw new NotSupportedException();
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
        public void AssembleAssessmentSection_WithAssessmentSectionWithoutManualSectionAssemblyResults_SetsInputOnCalculator()
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
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection, random.NextBoolean());

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
                AssessmentSectionAssemblyCategoryGroup output = AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection, random.NextBoolean());

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

        #region Manual Assembly Used

        #region With Probability

        [Test]
        public void GivenAssessmentSectionWithPipingConfigured_WhenAssemblingAssessmentSectionAndUseManualTrue_ThenInputSetOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(probability, failureMechanismSectionAssemblyCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void AssembleAssessmentSection_FailureMechanismWithProbabilityAndManualSectionAssemblyAndUseManualTrue_SetsManualAssemblyInputOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.IsNull(failureMechanismSectionAssemblyCalculator.ManualAssemblyAssemblyOutput);
            }
        }

        #endregion

        #region Without Probability

        [Test]
        public void AssembleAssessmentSection_FailureMechanismWithoutProbabilityAndWithManualSectionAssemblyAndUseManualTrue_SetsManualAssemblyInputOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(sectionResult.ManualAssemblyCategoryGroup, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        [Test]
        public void AssembleAssessmentSection_FailureMechanismWithoutProbabilityAndWithManualSectionAssemblyAndUseManualTrue_SetsAssemblyInputOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateIrrelevantAssessmentSection();
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(failureMechanismSectionAssemblyCalculator.CombinedAssemblyCategoryOutput, failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }
        }

        #endregion

        #endregion

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
        public void AssembleCombinedPerFailureMechanismSection_WithAssessmentSectionWithoutManualSectionAssemblyResults_SetsInputOnCalculator()
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
                AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection, random.NextBoolean());

                // Assert
                IEnumerable<CombinedAssemblyFailureMechanismSection>[] actualInput = calculator.CombinedFailureMechanismSectionsInput.ToArray();
                IEnumerable<CombinedAssemblyFailureMechanismSection>[] expectedInput = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, assessmentSection.GetFailureMechanisms(), random.NextBoolean()).ToArray();
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
                                                                                                                                                     random.NextBoolean())
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
                                                                                                                      random.NextBoolean());

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
                                                                                                                      random.NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere toetssporen kan geen oordeel worden bepaald.", exception.Message);
            }
        }

        #region Manual Assembly Used

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_FailureMechanismHasManualSectionAssemblyAndUseManualTrue_SetsManualAssemblyInputOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.ReferenceLine = new ReferenceLine();
            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(probability, failureMechanismSectionAssemblyCalculator.ManualAssemblyProbabilityInput);
            }
        }

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_FailureMechanismHasManualSectionAssemblyAndUseManualFalse_SetsAssemblyInputOnCalculator()
        {
            // Given
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.ReferenceLine = new ReferenceLine();
            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);

            PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            double probability = new Random(39).NextDouble();
            sectionResult.ManualAssemblyProbability = probability;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.IsNull(failureMechanismSectionAssemblyCalculator.ManualAssemblyAssemblyOutput);
            }
        }

        #endregion

        #endregion

        #region Helpers

        private static AssessmentSection CreateIrrelevantAssessmentSection()
        {
            AssessmentSection assessmentSection = CreateAssessmentSection();
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm => fm.IsRelevant = false);
            return assessmentSection;
        }

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

        private static T GetFailureMechanismSectionResult<T>(IHasSectionResults<T> failureMechanism) where T : FailureMechanismSectionResult
        {
            return failureMechanism.SectionResults.Single();
        }

        #endregion
    }
}