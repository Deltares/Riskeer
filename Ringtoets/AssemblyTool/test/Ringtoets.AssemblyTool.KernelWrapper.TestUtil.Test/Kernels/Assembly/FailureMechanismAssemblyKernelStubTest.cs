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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;
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
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsNull(kernel.Categories);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.FailureMechanismAssemblyResult);
            Assert.IsNull(kernel.FmSectionAssemblyResultsWithProbabilityInput);
            Assert.IsNull(kernel.FmSectionAssemblyResultsInput);
            Assert.AreEqual((EFailureMechanismCategory) 0, kernel.FailureMechanismCategoryResult);
        }

        [Test]
        public void AssembleFailureMechanismWbi1A1_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            bool partialAssembly = random.NextBoolean();
            IEnumerable<FmSectionAssemblyDirectResult> sectionAssemblyResults = Enumerable.Empty<FmSectionAssemblyDirectResult>();
            var kernel = new FailureMechanismAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.AssembleFailureMechanismWbi1A1(sectionAssemblyResults, partialAssembly);

            // Assert
            Assert.AreSame(sectionAssemblyResults, kernel.FmSectionAssemblyResultsInput);
            Assert.AreEqual(partialAssembly, kernel.PartialAssembly);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void AssembleFailureMechanismWbi1A1_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismAssemblyKernelStub
            {
                FailureMechanismCategoryResult = random.NextEnumValue<EFailureMechanismCategory>()
            };

            // Call
            EFailureMechanismCategory result = kernel.AssembleFailureMechanismWbi1A1(Enumerable.Empty<FmSectionAssemblyDirectResult>(), random.NextBoolean());

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
            TestDelegate test = () => kernel.AssembleFailureMechanismWbi1A1(Enumerable.Empty<FmSectionAssemblyDirectResult>(), true);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.FmSectionAssemblyResultsInput);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
            Assert.AreEqual((EFailureMechanismCategory) 0, kernel.FailureMechanismCategoryResult);
        }

        [Test]
        public void AssembleFailureMechanismWbi1A1_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var kernel = new FailureMechanismAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleFailureMechanismWbi1A1(Enumerable.Empty<FmSectionAssemblyDirectResult>(), true);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.FmSectionAssemblyResultsInput);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
            Assert.AreEqual((EFailureMechanismCategory) 0, kernel.FailureMechanismCategoryResult);
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
        public void AssembleFailureMechanismWbi1B1_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            IEnumerable<FmSectionAssemblyDirectResultWithProbability> sectionAssemblyResults = Enumerable.Empty<FmSectionAssemblyDirectResultWithProbability>();
            CategoriesList<FailureMechanismCategory> categories = CategoriesListTestFactory.CreateFailureMechanismCategories();
            bool partialAssembly = random.NextBoolean();
            var kernel = new FailureMechanismAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.AssembleFailureMechanismWbi1B1(failureMechanism, sectionAssemblyResults, categories, partialAssembly);

            // Assert
            Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreSame(sectionAssemblyResults, kernel.FmSectionAssemblyResultsWithProbabilityInput);
            Assert.AreSame(categories, kernel.Categories);
            Assert.AreEqual(partialAssembly, kernel.PartialAssembly);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void AssembleFailureMechanismWbi1B1_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismAssemblyKernelStub
            {
                FailureMechanismCategoryResult = random.NextEnumValue<EFailureMechanismCategory>()
            };

            // Call
            FailureMechanismAssemblyResult result = kernel.AssembleFailureMechanismWbi1B1(CreateRandomFailureMechanism(random),
                                                                                          Enumerable.Empty<FmSectionAssemblyDirectResultWithProbability>(),
                                                                                          CategoriesListTestFactory.CreateFailureMechanismCategories(),
                                                                                          random.NextBoolean());

            // Assert
            Assert.AreEqual(kernel.FailureMechanismAssemblyResult, result);
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
            TestDelegate test = () => kernel.AssembleFailureMechanismWbi1B1(CreateRandomFailureMechanism(random),
                                                                            Enumerable.Empty<FmSectionAssemblyDirectResultWithProbability>(),
                                                                            CategoriesListTestFactory.CreateFailureMechanismCategories(),
                                                                            true);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.Categories);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.FmSectionAssemblyResultsInput);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
            Assert.AreEqual((EFailureMechanismCategory) 0, kernel.FailureMechanismCategoryResult);
        }

        [Test]
        public void AssembleFailureMechanismWbi1B1_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleFailureMechanismWbi1B1(CreateRandomFailureMechanism(random),
                                                                            Enumerable.Empty<FmSectionAssemblyDirectResultWithProbability>(),
                                                                            CategoriesListTestFactory.CreateFailureMechanismCategories(),
                                                                            true);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.Categories);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.FmSectionAssemblyResultsInput);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
            Assert.AreEqual((EFailureMechanismCategory) 0, kernel.FailureMechanismCategoryResult);
        }

        private static FailureMechanism CreateRandomFailureMechanism(Random random)
        {
            var failureMechanism = new FailureMechanism(random.NextDouble(1, 5), random.NextDouble());
            return failureMechanism;
        }
    }
}