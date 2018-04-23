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
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.Assembly
{
    [TestFixture]
    public class FailureMechanismAssemblyKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new FailureMechanismAssemblyKernelStub();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismResultAssembler>(kernel);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.FailureMechanismAssemblyResult);
            Assert.AreEqual(EFailureMechanismCategory.It, kernel.FailureMechanismCategoryResult);
        }

        [Test]
        public void AssembleFailureMechanismWbi1A1_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            bool partialAssembly = random.NextBoolean();
            var sectionAssemblyResults = new List<FmSectionAssemblyDirectResult>();
            var kernel = new FailureMechanismAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.AssembleFailureMechanismWbi1A1(sectionAssemblyResults, partialAssembly);

            // Assert
            Assert.AreEqual(sectionAssemblyResults, kernel.FmSectionAssemblyResultsInput);
            Assert.AreEqual(partialAssembly, kernel.PartialAssembly);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void AssembleFailureMechanismWbi1A1_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismAssemblyKernelStub
            {
                FailureMechanismCategoryResult = random.NextEnumValue<EFailureMechanismCategory>()
            };

            // Call
            EFailureMechanismCategory result = kernel.AssembleFailureMechanismWbi1A1(new List<FmSectionAssemblyDirectResult>(), random.NextBoolean());

            // Assert
            Assert.AreEqual(kernel.FailureMechanismCategoryResult, result);
        }

        [Test]
        public void AssembleFailureMechanismWbi1A1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new FailureMechanismAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleFailureMechanismWbi1A1(new List<FmSectionAssemblyDirectResult>(), true);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.FmSectionAssemblyResultsInput);
            Assert.IsFalse(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
            Assert.AreEqual(EFailureMechanismCategory.It, kernel.FailureMechanismCategoryResult);
        }

        [Test]
        public void AssembleFailureMechanismWbi1A2_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.AssembleFailureMechanismWbi1A2(null, false);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void AssembleFailureMechanismWbi1B1_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var sectionAssemblyResults = new List<FmSectionAssemblyDirectResult>();
            bool partialAssembly = random.NextBoolean();
            var kernel = new FailureMechanismAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.AssembleFailureMechanismWbi1B1(assessmentSection, failureMechanism, sectionAssemblyResults, partialAssembly);

            // Assert
            Assert.AreEqual(assessmentSection, kernel.AssessmentSectionInput);
            Assert.AreEqual(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreEqual(sectionAssemblyResults, kernel.FmSectionAssemblyResultsInput);
            Assert.AreEqual(partialAssembly, kernel.PartialAssembly);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void AssembleFailureMechanismWbi1B1_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismAssemblyKernelStub
            {
                FailureMechanismCategoryResult = random.NextEnumValue<EFailureMechanismCategory>()
            };

            // Call
            FailureMechanismAssemblyResult result = kernel.AssembleFailureMechanismWbi1B1(CreateRandomAssessmentSection(random),
                                                                                          CreateRandomFailureMechanism(random),
                                                                                          new List<FmSectionAssemblyDirectResult>(),
                                                                                          random.NextBoolean());

            // Assert
            Assert.AreEqual(kernel.FailureMechanismCategoryResult, result);
        }

        [Test]
        public void AssembleFailureMechanismWbi1B1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleFailureMechanismWbi1B1(CreateRandomAssessmentSection(random),
                                                                            CreateRandomFailureMechanism(random),
                                                                            new List<FmSectionAssemblyDirectResult>(),
                                                                            true);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.FmSectionAssemblyResultsInput);
            Assert.IsFalse(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
            Assert.AreEqual(EFailureMechanismCategory.It, kernel.FailureMechanismCategoryResult);
        }

        private static FailureMechanism CreateRandomFailureMechanism(Random random)
        {
            var failureMechanism = new FailureMechanism(random.NextDouble(1, 5), random.NextDouble());
            return failureMechanism;
        }

        private static AssessmentSection CreateRandomAssessmentSection(Random random)
        {
            var assessmentSection = new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.5), random.NextDouble(0.5, 1.0));
            return assessmentSection;
        }
    }
}