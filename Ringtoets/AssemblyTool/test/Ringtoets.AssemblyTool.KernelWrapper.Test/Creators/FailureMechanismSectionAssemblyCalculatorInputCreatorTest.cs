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
using System.ComponentModel;
using AssemblyTool.Kernel.Data.CalculationResults;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.Common.Data.FailureMechanism;

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
        public void CreateSimpleCalculationResult_WithResultTypeNone_ThrowNotSupportedException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimpleCalculationResult(SimpleAssessmentResultType.None);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
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
    }
}