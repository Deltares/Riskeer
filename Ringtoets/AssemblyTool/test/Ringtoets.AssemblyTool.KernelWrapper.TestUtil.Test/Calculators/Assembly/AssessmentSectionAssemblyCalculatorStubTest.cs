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
            Assert.AreEqual((AssessmentSectionAssemblyCategoryGroup) 0,
                            calculator.FailureMechanismsWithoutProbabilityInput);
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            AssessmentSectionAssembly output = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                                    random.NextDouble(), random.NextDouble());

            // Assert
            Assert.AreEqual(0.75, output.Probability);
            Assert.AreEqual(AssessmentSectionAssemblyCategoryGroup.D, output.Group);
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                AssembleFailureMechanismsAssemblyOutput = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                        random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>())
            };

            // Call
            AssessmentSectionAssembly output = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                                    random.NextDouble(), random.NextDouble());

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

            IEnumerable<FailureMechanismAssembly> failureMechanisms = Enumerable.Empty<FailureMechanismAssembly>();

            var calculator = new AssessmentSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleFailureMechanisms(failureMechanisms, signalingNorm, lowerLimitNorm);

            // Assert
            Assert.AreSame(failureMechanisms, calculator.FailureMechanismAssemblyInput);
            Assert.AreEqual(lowerLimitNorm, calculator.LowerLimitNormInput);
            Assert.AreEqual(signalingNorm, calculator.SignalingNormInput);
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
                                                                           random.NextDouble(),
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
            AssessmentSectionAssemblyCategoryGroup output = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

            // Assert
            Assert.AreEqual(AssessmentSectionAssemblyCategoryGroup.D, output);
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculatorStub
            {
                AssembleFailureMechanismsAssemblyCategoryGroupOutput = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>()
            };

            // Call
            AssessmentSectionAssemblyCategoryGroup output = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

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
                calculator.AssembleAssessmentSection(random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>(),
                                                     new AssessmentSectionAssembly(random.NextDouble(),
                                                                                   random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>()));

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
                calculator.AssembleAssessmentSection(random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>(),
                                                     new AssessmentSectionAssembly(random.NextDouble(),
                                                                                   random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>()));

            // Assert
            Assert.AreEqual(calculator.AssembleAssessmentSectionCategoryGroupOutput, output);
        }

        [Test]
        public void AssembleAssessmentSection_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithoutProbability = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();
            var failureMechanismsWithProbability = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                 random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
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
                calculator.AssembleAssessmentSection(random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>(),
                                                     new AssessmentSectionAssembly(random.NextDouble(),
                                                                                   random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>()));

            // Assert
            var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(call);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }
    }
}