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

using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assessments;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assessments;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Calculators.Assessments
{
    [TestFixture]
    public class FailureMechanismSectionAssessmentAssemblyCalculatorStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionAssessmentAssemblyCalculator>(calculator);
            Assert.IsNull(calculator.SimpleAssessmentAssemblyOutput);
        }

        [Test]
        public void AssembleSimpleAssessment_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssessment assessment = calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.None);

            // Assert
            Assert.AreEqual(0, assessment.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, assessment.Group);
        }

        [Test]
        public void AssembleSimpleAssessment_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub
            {
                SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssessment(0.4, FailureMechanismSectionAssemblyCategoryGroup.None)
            };

            // Call
            FailureMechanismSectionAssessment assessment = calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.None);

            // Assert
            Assert.AreSame(calculator.SimpleAssessmentAssemblyOutput, assessment);
        }

        [Test]
        public void AssembleSimpleAssessment_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            const SimpleAssessmentResultType input = SimpleAssessmentResultType.None;
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub();

            // Call
            calculator.AssembleSimpleAssessment(input);

            // Assert
            Assert.AreEqual(input, calculator.SimpleAssessmentInput);
        }

        [Test]
        public void AssembleSimpleAssessment_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssessmentAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentResultType) 0);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssessmentAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssessment assessment = calculator.AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType.None);

            // Assert
            Assert.AreEqual(1, assessment.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIIv, assessment.Group);
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub
            {
                SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssessment(0.4, FailureMechanismSectionAssemblyCategoryGroup.None)
            };

            // Call
            FailureMechanismSectionAssessment assessment = calculator.AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType.None);

            // Assert
            Assert.AreSame(calculator.SimpleAssessmentAssemblyOutput, assessment);
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            const SimpleAssessmentResultValidityOnlyType input = SimpleAssessmentResultValidityOnlyType.None;
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub();

            // Call
            calculator.AssembleSimpleAssessment(input);

            // Assert
            Assert.AreEqual(input, calculator.SimpleAssessmentValidityOnlyInput);
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssessmentAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentResultValidityOnlyType) 0);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssessmentAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }
    }
}