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
using AssemblyTool.Kernel.Assembly.CalculatorInput;
using AssemblyTool.Kernel.Data.CalculationResults;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assessments;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.Assessments
{
    [TestFixture]
    public class FailureMechanismSectionAssessmentAssemblyKernelStubTest
    {
        [Test]
        public void SimpleAssessmentDirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.SimpleAssessmentDirectFailureMechanisms((SimpleCalculationResult) 0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void SimpleAssessmentDirectFailureMechanismsValidityOnly_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.SimpleAssessmentDirectFailureMechanisms((SimpleCalculationResultValidityOnly) 0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void SimpleAssessmentIndirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.SimpleAssessmentIndirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentIndirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentIndirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithProbability_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms((DetailedCalculationInputFromProbability) null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithBoundaries_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms((DetailedCategoryBoundariesCalculationResult) null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithLengthEffect_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms((DetailedCalculationInputFromProbabilityWithLengthEffect) null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms((TailorMadeCalculationResult) 0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentIndirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentIndirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithProbability_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms((TailorMadeCalculationInputFromProbability) null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithCategories_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms((TailorMadeCategoryCalculationResult) 0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithLengthEffect_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms(null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResults_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.CombinedAssessmentFromFailureMechanismSectionResults(0, 0, 0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResultsWithCategories_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssessmentAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.CombinedAssessmentFromFailureMechanismSectionResults(null, null, null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }
    }
}