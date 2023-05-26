// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.ComponentModel;
using Assembly.Kernel.Model;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.Common.Primitives;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCalculatorInputCreatorTest
    {
        [Test]
        public void ConvertFailureMechanismSectionResultFurtherAnalysisType_InvalidFailureMechanismSectionResultFurtherAnalysisType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType = (FailureMechanismSectionResultFurtherAnalysisType) 99;

            // Call
            void Call() => FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionResultFurtherAnalysisType(furtherAnalysisType);

            // Assert
            var expectedMessage = $"The value of argument 'furtherAnalysisType' ({furtherAnalysisType}) is invalid for Enum type '{nameof(FailureMechanismSectionResultFurtherAnalysisType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, ERefinementStatus.NotNecessary)]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.Necessary, ERefinementStatus.Necessary)]
        [TestCase(FailureMechanismSectionResultFurtherAnalysisType.Executed, ERefinementStatus.Performed)]
        public void ConvertFailureMechanismSectionResultFurtherAnalysisType_ValidFailureMechanismSectionResultFurtherAnalysisType_ReturnsExpectedRefinementStatus(
            FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType, ERefinementStatus expectedRefinementStatus)
        {
            // Call
            ERefinementStatus refinementStatus = FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionResultFurtherAnalysisType(furtherAnalysisType);

            // Assert
            Assert.AreEqual(expectedRefinementStatus, refinementStatus);
        }
    }
}