// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.Assembly
{
    [TestFixture]
    public class CombinedFailureMechanismSectionAssemblyKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new CombinedFailureMechanismSectionAssemblyKernelStub();

            // Assert
            Assert.IsInstanceOf<ICommonFailureMechanismSectionAssembler>(kernel);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsNull(kernel.AssessmentSectionLengthInput);
            Assert.IsNull(kernel.FailureMechanismSectionListsInput);
            Assert.IsNull(kernel.AssemblyResult);
        }

        [Test]
        public void AssembleCommonFailureMechanismSections_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(21);
            bool partialAssembly = random.NextBoolean();
            double assessmentSectionLength = random.NextDouble();
            IEnumerable<FailureMechanismSectionList> failureMechanismSectionLists = Enumerable.Empty<FailureMechanismSectionList>();

            var kernel = new CombinedFailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.AssembleCommonFailureMechanismSections(failureMechanismSectionLists, assessmentSectionLength, partialAssembly);

            // Assert
            Assert.IsTrue(kernel.Calculated);
            Assert.AreSame(failureMechanismSectionLists, kernel.FailureMechanismSectionListsInput);
            Assert.AreEqual(assessmentSectionLength, kernel.AssessmentSectionLengthInput);
            Assert.AreEqual(partialAssembly, kernel.PartialAssembly);
        }

        [Test]
        public void AssembleCommonFailureMechanismSections_KernelDoesNotThrowException_ReturnAssessmentGrade()
        {
            // Setup
            var random = new Random(21);
            var kernel = new CombinedFailureMechanismSectionAssemblyKernelStub
            {
                AssemblyResult = new AssemblyResult(Enumerable.Empty<FailureMechanismSectionList>(), Enumerable.Empty<FmSectionWithDirectCategory>())
            };

            // Call
            AssemblyResult result = kernel.AssembleCommonFailureMechanismSections(Enumerable.Empty<FailureMechanismSectionList>(),
                                                                                  random.NextDouble(),
                                                                                  random.NextBoolean());

            // Assert
            Assert.AreEqual(kernel.AssemblyResult, result);
        }

        [Test]
        public void AssembleCommonFailureMechanismSections_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new CombinedFailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleCommonFailureMechanismSections(Enumerable.Empty<FailureMechanismSectionList>(),
                                                                                    random.NextDouble(),
                                                                                    random.NextBoolean());

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionLengthInput);
            Assert.IsNull(kernel.FailureMechanismSectionListsInput);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsNull(kernel.AssemblyResult);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void AssembleCommonFailureMechanismSections_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new CombinedFailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.AssembleCommonFailureMechanismSections(Enumerable.Empty<FailureMechanismSectionList>(),
                                                                                    random.NextDouble(),
                                                                                    random.NextBoolean());

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentSectionLengthInput);
            Assert.IsNull(kernel.FailureMechanismSectionListsInput);
            Assert.IsNull(kernel.PartialAssembly);
            Assert.IsNull(kernel.AssemblyResult);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void FindGreatestCommonDenominatorSectionsWbi3A1_Always_ThrowsNotImplementedException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new CombinedFailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.FindGreatestCommonDenominatorSectionsWbi3A1(Enumerable.Empty<FailureMechanismSectionList>(),
                                                                                         random.NextDouble());

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TranslateFailureMechanismResultsToCommonSectionsWbi3B1_Always_ThrowsNotImplementedException()
        {
            // Setup
            var kernel = new CombinedFailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateFailureMechanismResultsToCommonSectionsWbi3B1(CreateFailureMechanismSectionList(),
                                                                                                    CreateFailureMechanismSectionList());

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DeterminCombinedResultPerCommonSectionWbi3C1_Always_ThrowsNotImplementedException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new CombinedFailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DeterminCombinedResultPerCommonSectionWbi3C1(Enumerable.Empty<FailureMechanismSectionList>(),
                                                                                          random.NextBoolean());

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        private static FailureMechanismSectionList CreateFailureMechanismSectionList()
        {
            return new FailureMechanismSectionList(string.Empty, new[]
            {
                new FailureMechanismSection(0, 1)
            });
        }
    }
}