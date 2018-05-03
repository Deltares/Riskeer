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
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.Data.Test
{
    [TestFixture]
    public class AssessmentSectionAssemblyFactoryTest
    {
        [Test]
        public void AssembleFailureMechanismsWithProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(null);

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
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput = new FailureMechanismAssembly(random.NextDouble(),
                                                                                                                 random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection);

                // Assert
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, assessmentSectionAssemblyCalculator.LowerLimitNormInput);
                Assert.AreEqual(failureMechanismContribution.SignalingNorm, assessmentSectionAssemblyCalculator.SignalingNormInput);

                AssertGroup1And2FailureMechanismInputs(assessmentSection.GetFailureMechanisms(),
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
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.AssessmentSectionAssemblyOutput = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                           random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());

                // Call
                AssessmentSectionAssembly output = AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection);

                // Assert
                Assert.AreSame(calculator.AssessmentSectionAssemblyOutput, output);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(CreateAssessmentSection());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssemblyFailureMechanismsWithoutProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(null);

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
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection);

                // Assert
                AssertGroup3And4FailureMechanismInputs(assessmentSection.GetFailureMechanisms(),
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
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.AssembleFailureMechanismsAssemblyOutput = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

                // Call
                AssessmentSectionAssemblyCategoryGroup output = AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection);

                // Assert
                Assert.AreEqual(calculator.AssembleFailureMechanismsAssemblyOutput, output);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(CreateAssessmentSection());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(null);

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
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;

                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                assessmentSectionAssemblyCalculator.AssessmentSectionAssemblyOutput = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                                                    random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
                assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyOutput = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

                // Call
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                AssertGroup1And2FailureMechanismInputs(assessmentSection.GetFailureMechanisms(),
                                                       failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput,
                                                       assessmentSectionAssemblyCalculator);

                AssertGroup3And4FailureMechanismInputs(assessmentSection.GetFailureMechanisms(),
                                                       failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput.Value,
                                                       assessmentSectionAssemblyCalculator);

                Assert.AreSame(assessmentSectionAssemblyCalculator.AssessmentSectionAssemblyOutput,
                               assessmentSectionAssemblyCalculator.FailureMechanismsWithProbabilityInput);
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyOutput,
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
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.AssessmentSectionAssemblyOutput = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                           random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
                calculator.AssembleAssessmentSectionOutput = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

                // Call
                AssessmentSectionAssemblyCategoryGroup output = AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                Assert.AreEqual(calculator.AssembleAssessmentSectionOutput, output);
            }
        }

        [Test]
        public void AssembleAssessmentSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(CreateAssessmentSection());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        private static void AssertGroup1And2FailureMechanismInputs(IEnumerable<IFailureMechanism> failureMechanisms,
                                                                   FailureMechanismAssembly expectedFailureMechanismAssembly,
                                                                   AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator)
        {
            IEnumerable<IFailureMechanism> expectedFailureMechanisms = failureMechanisms.Where(fm => fm.Group == 1 || fm.Group == 2);
            IEnumerable<FailureMechanismAssembly> failureMechanismAssemblyInput = assessmentSectionAssemblyCalculator.FailureMechanismAssemblyInput;
            Assert.AreEqual(expectedFailureMechanisms.Count(), failureMechanismAssemblyInput.Count());
            foreach (FailureMechanismAssembly failureMechanismAssembly in failureMechanismAssemblyInput)
            {
                Assert.AreEqual(expectedFailureMechanismAssembly.Group, failureMechanismAssembly.Group);
                Assert.AreEqual(expectedFailureMechanismAssembly.Probability, failureMechanismAssembly.Probability);
            }
        }

        private static void AssertGroup3And4FailureMechanismInputs(IEnumerable<IFailureMechanism> failureMechanisms,
                                                                   FailureMechanismAssemblyCategoryGroup expectedAssemblyCategoryGroup,
                                                                   AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator)
        {
            IEnumerable<IFailureMechanism> expectedFailureMechanisms = failureMechanisms.Where(fm => fm.Group == 3 || fm.Group == 4);
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
    }
}