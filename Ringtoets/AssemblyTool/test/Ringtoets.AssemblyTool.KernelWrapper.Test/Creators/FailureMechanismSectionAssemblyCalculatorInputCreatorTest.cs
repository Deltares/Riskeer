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

using System.ComponentModel;
using AssemblyTool.Kernel.Data.CalculationResults;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCalculatorInputCreatorTest
    {
        [Test]
        public void CreateSimpleCalculationResult_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimpleCalculationResult((SimpleAssessmentResultType) 99);

            // Assert
            const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'SimpleAssessmentResultType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, SimpleCalculationResult.None)]
        [TestCase(SimpleAssessmentResultType.NotApplicable, SimpleCalculationResult.NVT)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible, SimpleCalculationResult.FV)]
        [TestCase(SimpleAssessmentResultType.AssessFurther, SimpleCalculationResult.VB)]
        public void CreateSimpleCalculationResult_ValidData_ReturnSimpleCalculationResult(SimpleAssessmentResultType originalResult,
                                                                                          SimpleCalculationResult expectedResult)
        {
            // Call
            SimpleCalculationResult result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimpleCalculationResult(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateSimplecalclCalculationResultValidityOnly_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimplecalclCalculationResultValidityOnly((SimpleAssessmentResultValidityOnlyType) 99);

            // Assert
            const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'SimpleAssessmentResultValidityOnlyType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(SimpleAssessmentResultValidityOnlyType.None, SimpleCalculationResultValidityOnly.None)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.NotApplicable, SimpleCalculationResultValidityOnly.NVT)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable, SimpleCalculationResultValidityOnly.WVT)]
        public void CreateSimplecalclCalculationResultValidityOnly_ValidData_ReturnSimpleCalculationResultValidityOnly(SimpleAssessmentResultValidityOnlyType originalResult,
                                                                                                                       SimpleCalculationResultValidityOnly expectedResult)
        {
            // Call
            SimpleCalculationResultValidityOnly result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimplecalclCalculationResultValidityOnly(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}