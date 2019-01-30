// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.Assembly
{
    [TestFixture]
    public class AssessmentSectionAssemblyKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new AssessmentSectionAssemblyKernelStub();

            // Assert
            Assert.IsInstanceOf<IAssessmentGradeAssembler>(kernel);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsNull(kernel.FailureMechanismAssemblyResult);
            Assert.IsNull(kernel.FailureMechanismCategories);
            Assert.IsNull(kernel.FailureMechanismAssemblyResults);
            Assert.IsNull(kernel.AssemblyResultNoFailureProbability);
            Assert.IsNull(kernel.AssemblyResultWithFailureProbability);
            Assert.AreEqual((EAssessmentGrade) 0, kernel.AssessmentSectionAssemblyResult);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2A1_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(21);
            bool partialAssembly = random.NextBoolean();
            IEnumerable<FailureMechanismAssemblyResult> assemblyResults = Enumerable.Empty<FailureMechanismAssemblyResult>();

            var kernel = new AssessmentSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.AssembleAssessmentSectionWbi2A1(assemblyResults, partialAssembly);

            // Assert
            Assert.IsTrue(kernel.Calculated);
            Assert.AreEqual(partialAssembly, kernel.PartialAssembly);
            Assert.AreSame(assemblyResults, kernel.FailureMechanismAssemblyResults);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2A1_KernelDoesNotThrowException_ReturnAssessmentGrade()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssessmentSectionAssemblyKernelStub
            {
                FailureMechanismCategoryResult = random.NextEnumValue<EFailureMechanismCategory>()
            };

            // Call
            EFailureMechanismCategory result = kernel.AssembleAssessmentSectionWbi2A1(Enumerable.Empty<FailureMechanismAssemblyResult>(),
                                                                                      random.NextBoolean());

            // Assert
            Assert.AreEqual(kernel.FailureMechanismCategoryResult, result);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2A1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssessmentSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleAssessmentSectionWbi2A1(Enumerable.Empty<FailureMechanismAssemblyResult>(),
                                                                             random.NextBoolean());

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.FailureMechanismAssemblyResults);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2A1_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssessmentSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleAssessmentSectionWbi2A1(Enumerable.Empty<FailureMechanismAssemblyResult>(),
                                                                             random.NextBoolean());

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);

            Assert.IsNull(kernel.FailureMechanismAssemblyResults);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2B1_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(21);
            bool partialAssembly = random.NextBoolean();
            IEnumerable<FailureMechanismAssemblyResult> assemblyResults = Enumerable.Empty<FailureMechanismAssemblyResult>();
            CategoriesList<FailureMechanismCategory> categories = CategoriesListTestFactory.CreateFailureMechanismCategories();

            var kernel = new AssessmentSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.AssembleAssessmentSectionWbi2B1(assemblyResults, categories, partialAssembly);

            // Assert
            Assert.IsTrue(kernel.Calculated);
            Assert.AreEqual(partialAssembly, kernel.PartialAssembly);
            Assert.AreSame(assemblyResults, kernel.FailureMechanismAssemblyResults);
            Assert.AreSame(categories, kernel.FailureMechanismCategories);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2B1_KernelDoesNotThrowException_ReturnAssessmentGrade()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssessmentSectionAssemblyKernelStub
            {
                FailureMechanismAssemblyResult = CreateFailureMechanismAssemblyResult()
            };

            // Call
            FailureMechanismAssemblyResult result = kernel.AssembleAssessmentSectionWbi2B1(Enumerable.Empty<FailureMechanismAssemblyResult>(),
                                                                                           CategoriesListTestFactory.CreateFailureMechanismCategories(),
                                                                                           random.NextBoolean());

            // Assert
            Assert.AreEqual(kernel.FailureMechanismAssemblyResult, result);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2B1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssessmentSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleAssessmentSectionWbi2B1(Enumerable.Empty<FailureMechanismAssemblyResult>(),
                                                                             CategoriesListTestFactory.CreateFailureMechanismCategories(),
                                                                             random.NextBoolean());

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.FailureMechanismAssemblyResults);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2B1_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssessmentSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleAssessmentSectionWbi2B1(Enumerable.Empty<FailureMechanismAssemblyResult>(),
                                                                             CategoriesListTestFactory.CreateFailureMechanismCategories(),
                                                                             random.NextBoolean());

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.FailureMechanismAssemblyResults);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2C1_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(21);
            var assemblyResultNoFailureProbability = random.NextEnumValue<EFailureMechanismCategory>();
            FailureMechanismAssemblyResult assemblyResultWithFailureProbability = CreateFailureMechanismAssemblyResult();

            var kernel = new AssessmentSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.AssembleAssessmentSectionWbi2C1(assemblyResultNoFailureProbability,
                                                   assemblyResultWithFailureProbability);

            // Assert
            Assert.IsTrue(kernel.Calculated);
            Assert.AreEqual(assemblyResultNoFailureProbability, kernel.AssemblyResultNoFailureProbability);
            Assert.AreSame(assemblyResultWithFailureProbability, kernel.AssemblyResultWithFailureProbability);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2C1_KernelDoesNotThrowException_ReturnAssessmentGrade()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssessmentSectionAssemblyKernelStub
            {
                AssessmentSectionAssemblyResult = random.NextEnumValue<EAssessmentGrade>()
            };

            // Call
            EAssessmentGrade result = kernel.AssembleAssessmentSectionWbi2C1(random.NextEnumValue<EFailureMechanismCategory>(),
                                                                             CreateFailureMechanismAssemblyResult());

            // Assert
            Assert.AreEqual(kernel.AssessmentSectionAssemblyResult, result);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2C1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssessmentSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleAssessmentSectionWbi2C1(random.NextEnumValue<EFailureMechanismCategory>(),
                                                                             CreateFailureMechanismAssemblyResult());

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssemblyResultNoFailureProbability);
            Assert.IsNull(kernel.AssemblyResultWithFailureProbability);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void AssembleAssessmentSectionWbi2C1_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssessmentSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleAssessmentSectionWbi2C1(random.NextEnumValue<EFailureMechanismCategory>(),
                                                                             CreateFailureMechanismAssemblyResult());

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssemblyResultNoFailureProbability);
            Assert.IsNull(kernel.AssemblyResultWithFailureProbability);
            Assert.IsFalse(kernel.Calculated);
        }

        private static FailureMechanismAssemblyResult CreateFailureMechanismAssemblyResult()
        {
            var random = new Random(21);
            return new FailureMechanismAssemblyResult(random.NextEnumValue<EFailureMechanismCategory>(),
                                                      random.NextDouble());
        }
    }
}