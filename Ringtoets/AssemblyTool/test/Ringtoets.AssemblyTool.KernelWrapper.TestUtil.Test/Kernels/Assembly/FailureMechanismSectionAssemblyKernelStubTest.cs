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
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Assembly;
using AssemblyTool.Kernel.Assembly.CalculatorInput;
using AssemblyTool.Kernel.Data;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using AssemblyTool.Kernel.Data.CalculationResults;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.Assembly
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionAssemblyCalculator>(kernel);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.SimpleAssessmentFailureMechanismsInput);
            Assert.IsNull(kernel.SimpleAssessmentFailureMechanismsValidityOnlyInput);
        }

        [Test]
        public void SimpleAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<SimpleCalculationResult>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.SimpleAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreEqual(input, kernel.SimpleAssessmentFailureMechanismsInput);
        }

        [Test]
        public void SimpleAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateFalse_SetCalculatedTrue()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.SimpleAssessmentDirectFailureMechanisms((SimpleCalculationResult) 0);

            // Assert
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void SimpleAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyCategoryResult()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.IIIv, Probability.NaN))
            };

            // Call
            CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> result = kernel.SimpleAssessmentDirectFailureMechanisms((SimpleCalculationResult) 0);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyCategoryResult, result);
        }

        [Test]
        public void SimpleAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.SimpleAssessmentDirectFailureMechanisms((SimpleCalculationResult) 0);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.SimpleAssessmentFailureMechanismsInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyCategoryResult);
        }

        [Test]
        public void SimpleAssessmentDirectFailureMechanismsValidityOnly_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<SimpleCalculationResultValidityOnly>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.SimpleAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreEqual(input, kernel.SimpleAssessmentFailureMechanismsValidityOnlyInput);
        }

        [Test]
        public void SimpleAssessmentDirectFailureMechanismsValidityOnly_ThrowExceptionOnCalculateFalse_SetCalculatedTrue()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.SimpleAssessmentDirectFailureMechanisms((SimpleCalculationResultValidityOnly) 0);

            // Assert
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void SimpleAssessmentDirectFailureMechanismsValidityOnly_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyCategoryResult()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.IIIv, Probability.NaN))
            };

            // Call
            CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> result = kernel.SimpleAssessmentDirectFailureMechanisms((SimpleCalculationResultValidityOnly) 0);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyCategoryResult, result);
        }

        [Test]
        public void SimpleAssessmentDirectFailureMechanismsValidityOnly_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.SimpleAssessmentDirectFailureMechanisms((SimpleCalculationResultValidityOnly) 0);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.SimpleAssessmentFailureMechanismsInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyCategoryResult);
        }

        [Test]
        public void SimpleAssessmentIndirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.SimpleAssessmentIndirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentIndirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentIndirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithProbability_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms((DetailedCalculationInputFromProbability) null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithBoundaries_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms((DetailedCategoryBoundariesCalculationResult) null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithLengthEffect_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms((DetailedCalculationInputFromProbabilityWithLengthEffect) null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms((TailorMadeCalculationResult) 0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentIndirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentIndirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithProbability_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms((TailorMadeCalculationInputFromProbability) null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithCategories_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms((TailorMadeCategoryCalculationResult) 0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithLengthEffect_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms(null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResults_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.CombinedAssessmentFromFailureMechanismSectionResults(0, 0, 0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResultsWithCategories_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.CombinedAssessmentFromFailureMechanismSectionResults(null, null, null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }
    }
}