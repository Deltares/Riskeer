﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupFactoryTest
    {
        [Test]
        public void AssembleSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                null, random.NextBoolean(), random.NextEnumValue<InitialFailureMechanismResultType>(), random.NextDouble(),
                random.NextDouble(), random.NextBoolean(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<ProbabilityRefinementType>(), () => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleSection_GetNFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                new AssessmentSectionStub(), random.NextBoolean(), random.NextEnumValue<InitialFailureMechanismResultType>(),
                random.NextDouble(), random.NextDouble(), random.NextBoolean(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<ProbabilityRefinementType>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getNFunc", exception.ParamName);
        }

        [Test]
        public void AssembleSection_InvalidInitialFailureMechanismResultType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);

            const InitialFailureMechanismResultType initialFailureMechanismResultType = (InitialFailureMechanismResultType) 99;

            // Call
            void Call() => FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                new AssessmentSectionStub(), random.NextBoolean(), initialFailureMechanismResultType,
                random.NextDouble(), random.NextDouble(), random.NextBoolean(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<ProbabilityRefinementType>(), () => double.NaN);

            // Assert
            var expectedMessage = $"The value of argument 'initialFailureMechanismResultType' ({initialFailureMechanismResultType}) is invalid for Enum type '{nameof(InitialFailureMechanismResultType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void AssembleSection_InvalidProbabilityRefinementType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);

            const ProbabilityRefinementType probabilityRefinementType = (ProbabilityRefinementType) 99;

            // Call
            void Call() => FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                new AssessmentSectionStub(), random.NextBoolean(), random.NextEnumValue<InitialFailureMechanismResultType>(),
                random.NextDouble(), random.NextDouble(), random.NextBoolean(), random.NextDouble(), random.NextDouble(),
                probabilityRefinementType, () => double.NaN);

            // Assert
            var expectedMessage = $"The value of argument 'probabilityRefinementType' ({probabilityRefinementType}) is invalid for Enum type '{nameof(ProbabilityRefinementType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(InitialFailureMechanismResultType.Adopt, true)]
        [TestCase(InitialFailureMechanismResultType.Manual, true)]
        [TestCase(InitialFailureMechanismResultType.NoFailureProbability, false)]
        public void AssembleSection_WithInput_SetsInputOnCalculator(InitialFailureMechanismResultType initialFailureMechanismResultType,
                                                                    bool expectedHasProbabilitySpecified)
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();
            bool furtherAnalysisNeeded = random.NextBoolean();
            double refinedProfileProbability = random.NextDouble();
            double refinedSectionProbability = random.NextDouble();

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                    assessmentSection, isRelevant, initialFailureMechanismResultType, profileProbability, sectionProbability,
                    furtherAnalysisNeeded, refinedProfileProbability, refinedSectionProbability,
                    ProbabilityRefinementType.Both, () => 1.0);

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalingNorm, calculatorInput.SignalingNorm);
                Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, calculatorInput.LowerLimitNorm);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(expectedHasProbabilitySpecified, calculatorInput.HasProbabilitySpecified);
                Assert.AreEqual(profileProbability, calculatorInput.InitialProfileProbability);
                Assert.AreEqual(sectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(furtherAnalysisNeeded, calculatorInput.FurtherAnalysisNeeded);
                Assert.AreEqual(refinedProfileProbability, calculatorInput.RefinedProfileProbability);
                Assert.AreEqual(refinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        [TestCase(ProbabilityRefinementType.Both, 1.5, 1.6)]
        [TestCase(ProbabilityRefinementType.Profile, 1.5, 3.0)]
        [TestCase(ProbabilityRefinementType.Section, 0.8, 1.6)]
        public void AssembleSection_WithRefinedProbabilities_SetsInputOnCalculator(
            ProbabilityRefinementType probabilityRefinementType, double expectedRefinedProfileProbability, double expectedRefinedSectionProbability)
        {
            // Setup
            var random = new Random(21);
            const double refinedProfileProbability = 1.5;
            const double refinedSectionProbability = 1.6;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                    new AssessmentSectionStub(), random.NextBoolean(), random.NextEnumValue<InitialFailureMechanismResultType>(),
                    random.NextDouble(), random.NextDouble(), random.NextBoolean(), refinedProfileProbability, refinedSectionProbability,
                    probabilityRefinementType, () => 2.0);

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;

                Assert.AreEqual(expectedRefinedProfileProbability, calculatorInput.RefinedProfileProbability);
                Assert.AreEqual(expectedRefinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        public void AssembleSection_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(21);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResult output =
                    FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                        new AssessmentSectionStub(), random.NextBoolean(), random.NextEnumValue<InitialFailureMechanismResultType>(),
                        random.NextDouble(), random.NextDouble(), random.NextBoolean(), random.NextDouble(), random.NextDouble(),
                        random.NextEnumValue<ProbabilityRefinementType>(), () => 1.0);

                // Assert
                FailureMechanismSectionAssemblyResult calculatorOutput = calculator.FailureMechanismSectionAssemblyResultOutput;
                Assert.AreEqual(calculatorOutput.N, output.N);
                Assert.AreEqual(calculatorOutput.AssemblyGroup, output.AssemblyGroup);
                Assert.AreEqual(calculatorOutput.ProfileProbability, output.ProfileProbability);
                Assert.AreEqual(calculatorOutput.SectionProbability, output.SectionProbability);
            }
        }

        [Test]
        public void AssembleSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => FailureMechanismSectionAssemblyGroupFactory.AssembleSection(
                    new AssessmentSectionStub(), random.NextBoolean(), random.NextEnumValue<InitialFailureMechanismResultType>(),
                    random.NextDouble(), random.NextDouble(), random.NextBoolean(), random.NextDouble(), random.NextDouble(),
                    random.NextEnumValue<ProbabilityRefinementType>(), () => double.NaN);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }
    }
}