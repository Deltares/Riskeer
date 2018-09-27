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
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Calculators.Assembly
{
    [TestFixture]
    public class AssessmentSectionAssemblyCalculatorStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionAssemblyCalculator>(calculator);
            Assert.IsNull(calculator.FailureMechanismAssemblyInput);
            Assert.IsNull(calculator.FailureMechanismAssemblyCategoryGroupInput);
            Assert.IsNull(calculator.FailureMechanismsWithProbabilityInput);
            Assert.AreEqual(0.0, calculator.LowerLimitNormInput);
            Assert.AreEqual(0.0, calculator.SignalingNormInput);
            Assert.AreEqual(0.0, calculator.FailureProbabilityMarginFactorInput);
            Assert.AreEqual((FailureMechanismAssemblyCategoryGroup) 0,
                            calculator.FailureMechanismsWithoutProbabilityInput);
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismAssembly output = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                                   random.NextDouble(), random.NextDouble(),
                                                                                   random.NextDouble());

            // Assert
            Assert.AreEqual(0.75, output.Probability);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.IIIt, output.Group);
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                AssembleFailureMechanismsAssemblyOutput = new FailureMechanismAssembly(random.NextDouble(),
                                                                                       random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismAssembly output = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                                   random.NextDouble(), random.NextDouble(),
                                                                                   random.NextDouble());

            // Assert
            Assert.AreSame(calculator.AssembleFailureMechanismsAssemblyOutput, output);
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(21);
            double lowerLimitNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();
            double failureProbabilityMarginFactorInput = random.NextDouble();

            IEnumerable<FailureMechanismAssembly> failureMechanisms = Enumerable.Empty<FailureMechanismAssembly>();

            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleFailureMechanisms(failureMechanisms, signalingNorm, lowerLimitNorm, failureProbabilityMarginFactorInput);

            // Assert
            Assert.AreSame(failureMechanisms, calculator.FailureMechanismAssemblyInput);
            Assert.AreEqual(lowerLimitNorm, calculator.LowerLimitNormInput);
            Assert.AreEqual(signalingNorm, calculator.SignalingNormInput);
            Assert.AreEqual(failureProbabilityMarginFactorInput, calculator.FailureProbabilityMarginFactorInput);
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_ThrowExceptionOnCalculateTrue_ThrowsAssessmentSectionAssemblyException()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate call = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                           random.NextDouble(), random.NextDouble(),
                                                                           random.NextDouble());

            // Assert
            var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(call);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismAssemblyCategoryGroup output = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

            // Assert
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.IIIt, output);
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                AssembleFailureMechanismsAssemblyCategoryGroupOutput = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismAssemblyCategoryGroup output = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

            // Assert
            Assert.AreEqual(calculator.AssembleFailureMechanismsAssemblyCategoryGroupOutput, output);
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            IEnumerable<FailureMechanismAssemblyCategoryGroup> failureMechanisms = Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>();
            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleFailureMechanisms(failureMechanisms);

            // Assert
            Assert.AreSame(failureMechanisms, calculator.FailureMechanismAssemblyCategoryGroupInput);
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_ThrowExceptionOnCalculateTrue_ThrowsAssessmentSectionAssemblyException()
        {
            // Setup
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate call = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

            // Assert
            var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(call);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleAssessmentSection_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            AssessmentSectionAssemblyCategoryGroup output =
                calculator.AssembleAssessmentSection(random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>(),
                                                     new FailureMechanismAssembly(random.NextDouble(),
                                                                                  random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>()));

            // Assert
            Assert.AreEqual(AssessmentSectionAssemblyCategoryGroup.C, output);
        }

        [Test]
        public void AssembleAssessmentSection_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                AssembleAssessmentSectionCategoryGroupOutput = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>()
            };

            // Call
            AssessmentSectionAssemblyCategoryGroup output =
                calculator.AssembleAssessmentSection(random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>(),
                                                     new FailureMechanismAssembly(random.NextDouble(),
                                                                                  random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>()));

            // Assert
            Assert.AreEqual(calculator.AssembleAssessmentSectionCategoryGroupOutput, output);
        }

        [Test]
        public void AssembleAssessmentSection_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithoutProbability = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();
            var failureMechanismsWithProbability = new FailureMechanismAssembly(random.NextDouble(),
                                                                                random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability, failureMechanismsWithProbability);

            // Assert
            Assert.AreEqual(failureMechanismsWithoutProbability, calculator.FailureMechanismsWithoutProbabilityInput);
            Assert.AreSame(failureMechanismsWithProbability, calculator.FailureMechanismsWithProbabilityInput);
        }

        [Test]
        public void AssembleAssessmentSection_ThrowExceptionOnCalculateTrue_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate call = () =>
                calculator.AssembleAssessmentSection(random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>(),
                                                     new FailureMechanismAssembly(random.NextDouble(),
                                                                                  random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>()));

            // Assert
            var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(call);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            CombinedFailureMechanismSectionAssembly[] output =
                calculator.AssembleCombinedFailureMechanismSections(Enumerable.Empty<CombinedAssemblyFailureMechanismSection[]>(),
                                                                    new Random(21).NextDouble()).ToArray();

            // Assert
            var expectedOutput = new[]
            {
                new CombinedFailureMechanismSectionAssembly(
                    new CombinedAssemblyFailureMechanismSection(0, 1, FailureMechanismSectionAssemblyCategoryGroup.IIIv)
                    , new FailureMechanismSectionAssemblyCategoryGroup[0])
            };
            Assert.AreEqual(expectedOutput[0].Section.SectionStart, output[0].Section.SectionStart);
            Assert.AreEqual(expectedOutput[0].Section.SectionEnd, output[0].Section.SectionEnd);
            Assert.AreEqual(expectedOutput[0].Section.CategoryGroup, output[0].Section.CategoryGroup);
            CollectionAssert.AreEqual(expectedOutput[0].FailureMechanismResults, output[0].FailureMechanismResults);
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                CombinedFailureMechanismSectionAssemblyOutput = new[]
                {
                    new CombinedFailureMechanismSectionAssembly(
                        new CombinedAssemblyFailureMechanismSection(random.NextDouble(), random.NextDouble(),
                                                                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                        new[]
                        {
                            random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
                        })
                }
            };

            // Call
            IEnumerable<CombinedFailureMechanismSectionAssembly> output =
                calculator.AssembleCombinedFailureMechanismSections(Enumerable.Empty<CombinedAssemblyFailureMechanismSection[]>(),
                                                                    random.NextDouble());

            // Assert
            Assert.AreSame(calculator.CombinedFailureMechanismSectionAssemblyOutput, output);
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> failureMechanismSections = Enumerable.Empty<CombinedAssemblyFailureMechanismSection[]>();
            double assessmentSectionLength = new Random(21).NextDouble();
            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleCombinedFailureMechanismSections(failureMechanismSections, assessmentSectionLength);

            // Assert
            Assert.AreSame(failureMechanismSections, calculator.CombinedFailureMechanismSectionsInput);
            Assert.AreEqual(assessmentSectionLength, calculator.AssessmentSectionLength);
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_ThrowExceptionOnCalculateTrue_ThrowsAssessmentSectionAssemblyException()
        {
            // Setup
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate call = () => calculator.AssembleCombinedFailureMechanismSections(Enumerable.Empty<CombinedAssemblyFailureMechanismSection[]>(),
                                                                                          new Random(21).NextDouble());

            // Assert
            var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(call);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }
    }
}