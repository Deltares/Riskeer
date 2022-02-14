// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.Categories;
using Assembly.Kernel.Model.FailureMechanismSections;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class AssessmentSectionAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_WithFactory_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new AssessmentSectionAssemblyCalculator(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionAssemblyCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_WithInvalidInput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() =>
                    calculator.AssembleCombinedFailureMechanismSections(new[]
                    {
                        new[]
                        {
                            new CombinedAssemblyFailureMechanismSection(0, 1, (FailureMechanismSectionAssemblyGroup) 99)
                        }
                    }, random.Next());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var input = new[]
            {
                new[]
                {
                    new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>())
                }
            };
            double assessmentSectionLength = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.AssemblyResult = new AssemblyResult(new[]
                {
                    new FailureMechanismSectionList(string.Empty, new[]
                    {
                        new FailureMechanismSectionWithCategory(0, 1, random.NextEnumValue<EInterpretationCategory>())
                    })
                }, new[]
                {
                    new FailureMechanismSectionWithCategory(0, 1, random.NextEnumValue<EInterpretationCategory>())
                });

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombinedFailureMechanismSections(input, assessmentSectionLength);

                // Assert
                Assert.AreEqual(assessmentSectionLength, kernel.AssessmentSectionLength);
                CombinedFailureMechanismSectionsInputAssert.AssertCombinedFailureMechanismInput(input, kernel.FailureMechanismSectionLists);
                Assert.IsFalse(kernel.PartialAssembly);
                Assert.IsTrue(kernel.Calculated);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.AssemblyResult = new AssemblyResult(new[]
                {
                    new FailureMechanismSectionList(string.Empty, new[]
                    {
                        new FailureMechanismSectionWithCategory(0, 1, random.NextEnumValue<EInterpretationCategory>())
                    })
                }, new[]
                {
                    new FailureMechanismSectionWithCategory(0, 1, random.NextEnumValue<EInterpretationCategory>())
                });

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                CombinedFailureMechanismSectionAssembly[] output = calculator.AssembleCombinedFailureMechanismSections(new[]
                {
                    new[]
                    {
                        new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>())
                    }
                }, random.NextDouble()).ToArray();

                // Assert
                CombinedFailureMechanismSectionAssemblyAssert.AssertAssembly(kernel.AssemblyResult, output);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelWithInvalidOutput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.AssemblyResult = new AssemblyResult(new[]
                {
                    new FailureMechanismSectionList(string.Empty, new[]
                    {
                        new FailureMechanismSectionWithCategory(0, 1, (EInterpretationCategory) 99)
                    })
                }, new[]
                {
                    new FailureMechanismSectionWithCategory(0, 1, random.NextEnumValue<EInterpretationCategory>())
                });

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleCombinedFailureMechanismSections(new[]
                                         {
                                             new[]
                                             {
                                                 new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>())
                                             }
                                         }, random.NextDouble())
                                         .ToArray();

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() =>
                    calculator.AssembleCombinedFailureMechanismSections(new[]
                              {
                                  new[]
                                  {
                                      new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>())
                                  }
                              }, random.NextDouble())
                              .ToArray();

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() =>
                    calculator.AssembleCombinedFailureMechanismSections(new[]
                              {
                                  new[]
                                  {
                                      new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>())
                                  }
                              }, random.NextDouble())
                              .ToArray();

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.EmptyResultsList)
                }), exception.Message);
            }
        }
    }
}