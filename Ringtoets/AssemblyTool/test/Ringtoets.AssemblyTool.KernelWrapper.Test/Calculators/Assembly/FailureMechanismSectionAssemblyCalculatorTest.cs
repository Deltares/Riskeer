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
using System.Linq;
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Data;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using AssemblyTool.Kernel.Data.CalculationResults;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new FailureMechanismSectionAssemblyCalculator(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionAssemblyCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        private static void AssertCalculatorOutput(CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> original, FailureMechanismSectionAssembly actual)
        {
            Assert.AreEqual(GetGroup(original.Result.CategoryGroup), actual.Group);
            Assert.AreEqual(original.Result.EstimatedProbabilityOfFailure, actual.Probability);
        }

        private static FailureMechanismSectionAssemblyCategoryGroup GetGroup(FailureMechanismSectionCategoryGroup originalGroup)
        {
            switch (originalGroup)
            {
                case FailureMechanismSectionCategoryGroup.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case FailureMechanismSectionCategoryGroup.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case FailureMechanismSectionCategoryGroup.IIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case FailureMechanismSectionCategoryGroup.IVv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IVv;
                case FailureMechanismSectionCategoryGroup.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case FailureMechanismSectionCategoryGroup.VIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIv;
                case FailureMechanismSectionCategoryGroup.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                case FailureMechanismSectionCategoryGroup.None:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                default:
                    throw new NotSupportedException();
            }
        }

        #region Tailor Made Assessment

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_Always_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>(),
                    probability,
                    categories);

                // Assert
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIIv, assembly.Group);
                Assert.AreEqual(probability, assembly.Probability);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithInvalidAssessmentResultTypeEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                            random.NextDouble(),
                                                            random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment((TailorMadeAssessmentProbabilityCalculationResultType) 99,
                                                                                  random.NextDouble(),
                                                                                  categories);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithInvalidCategoryEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                            random.NextDouble(),
                                                            (FailureMechanismSectionAssemblyCategoryGroup) 99)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                                                  random.NextDouble(),
                                                                                  categories);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability, probability, categories);

                // Assert
                Assert.AreEqual(probability, kernel.TailorMadeCalculationInputFromProbabilityInput.Result.Probability);
                Assert.AreEqual(TailorMadeProbabilityCalculationResultGroup.Probability, kernel.TailorMadeCalculationInputFromProbabilityInput.Result.CalculationResultGroup);

                FailureMechanismSectionCategory actualCategory = kernel.TailorMadeCalculationInputFromProbabilityInput.Categories.Single();
                FailureMechanismSectionAssemblyCategory expectedCategory = categories.Single();
                Assert.AreEqual(expectedCategory.LowerBoundary, actualCategory.LowerBoundary);
                Assert.AreEqual(expectedCategory.UpperBoundary, actualCategory.UpperBoundary);
                Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualCategory.CategoryGroup);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                                                                   random.NextDouble(),
                                                                                                   categories);

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                                                  random.NextDouble(),
                                                                                  categories);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                                                  random.NextDouble(),
                                                                                  categories);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_WithInvalidAssessmentResultTypeEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                            random.NextDouble(),
                                                            random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment((TailorMadeAssessmentProbabilityCalculationResultType) 99,
                                                                                  probability,
                                                                                  categories);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_WithInvalidCategoryEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                            random.NextDouble(),
                                                            (FailureMechanismSectionAssemblyCategoryGroup) 99)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                                                  probability,
                                                                                  categories);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble(1.0, 10.0);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability, probability, categories, n);

                // Assert
                Assert.AreEqual(n, kernel.TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor.NValue);
                Assert.AreEqual(probability, kernel.TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor.Result.Probability);
                Assert.AreEqual(TailorMadeProbabilityCalculationResultGroup.Probability,
                                kernel.TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor.Result.CalculationResultGroup);

                FailureMechanismSectionCategory actualCategory = kernel.TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor.Categories.Single();
                FailureMechanismSectionAssemblyCategory expectedCategory = categories.Single();
                Assert.AreEqual(expectedCategory.LowerBoundary, actualCategory.LowerBoundary);
                Assert.AreEqual(expectedCategory.UpperBoundary, actualCategory.UpperBoundary);
                Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualCategory.CategoryGroup);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                                                                   random.NextDouble(),
                                                                                                   categories,
                                                                                                   random.NextDouble(1.0, 10.0));

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                                                  random.NextDouble(),
                                                                                  categories,
                                                                                  random.NextDouble(1.0, 10.0));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                                                  random.NextDouble(),
                                                                                  categories,
                                                                                  random.NextDouble());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Simple Assessment

        [Test]
        public void AssembleSimpleAssessment_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentResultType) 99);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            const SimpleAssessmentResultType assessmentResult = SimpleAssessmentResultType.AssessFurther;
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleSimpleAssessment(assessmentResult);

                // Assert
                Assert.AreEqual(SimpleCalculationResult.VB, kernel.SimpleAssessmentFailureMechanismsInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.AssessFurther);

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.AssessFurther);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.AssessFurther);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentResultValidityOnlyType) 99);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            const SimpleAssessmentResultValidityOnlyType assessmentResult = SimpleAssessmentResultValidityOnlyType.Applicable;
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleSimpleAssessment(assessmentResult);

                // Assert
                Assert.AreEqual(SimpleCalculationResultValidityOnly.WVT, kernel.SimpleAssessmentFailureMechanismsValidityOnlyInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType.NotApplicable);

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType.Applicable);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType.Applicable);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void AssembleDetailedAssessment_WithInvalidResultEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment((DetailedAssessmentResultType) 99,
                                                                                probability,
                                                                                Enumerable.Empty<FailureMechanismSectionAssemblyCategory>());

                // Assert
                string expectedMessage = $"The value of argument 'detailedAssessmentResult' (99) is invalid for Enum type '{nameof(DetailedAssessmentResultType)}'.";
                string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
                Assert.AreEqual("detailedAssessmentResult", parameterName);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_WithInvalidCategoryEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                            random.NextDouble(),
                                                            (FailureMechanismSectionAssemblyCategoryGroup) 99)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(DetailedAssessmentResultType.Probability, probability, categories);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_WithValidInputResultTypeNotAssessed_ReturnsExpectedCategory()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly detailedAssembly = calculator.AssembleDetailedAssessment(DetailedAssessmentResultType.NotAssessed, probability, categories);

                // Assert
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIIv, detailedAssembly.Group);
                Assert.AreEqual(0.0, detailedAssembly.Probability);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(DetailedAssessmentResultType.Probability, probability, categories);

                // Assert
                Assert.AreEqual(probability, kernel.DetailedAssessmentFailureMechanismFromProbabilityInput.Probability);

                FailureMechanismSectionCategory actualCategory = kernel.DetailedAssessmentFailureMechanismFromProbabilityInput.Categories.Single();
                FailureMechanismSectionAssemblyCategory expectedCategory = categories.Single();
                Assert.AreEqual(expectedCategory.LowerBoundary, actualCategory.LowerBoundary);
                Assert.AreEqual(expectedCategory.UpperBoundary, actualCategory.UpperBoundary);
                Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualCategory.CategoryGroup);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(DetailedAssessmentResultType.Probability,
                                                                                                 probability,
                                                                                                 categories);

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(DetailedAssessmentResultType.Probability,
                                                                                probability,
                                                                                categories);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(DetailedAssessmentResultType.Probability,
                                                                                probability,
                                                                                categories);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble(1.0, 10.0);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                            random.NextDouble(),
                                                            (FailureMechanismSectionAssemblyCategoryGroup) 99)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(probability, categories, n);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble(1.0, 10.0);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(probability, categories, n);

                // Assert
                Assert.AreEqual(probability, kernel.DetailedAssessmentFailureMechanismFromProbabilityWithLengthEffectInput.Probability);
                Assert.AreEqual(n, kernel.DetailedAssessmentFailureMechanismFromProbabilityWithLengthEffectInput.NValue);

                FailureMechanismSectionCategory actualCategory = kernel.DetailedAssessmentFailureMechanismFromProbabilityWithLengthEffectInput.Categories.Single();
                FailureMechanismSectionAssemblyCategory expectedCategory = categories.Single();
                Assert.AreEqual(expectedCategory.LowerBoundary, actualCategory.LowerBoundary);
                Assert.AreEqual(expectedCategory.UpperBoundary, actualCategory.UpperBoundary);
                Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualCategory.CategoryGroup);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble(1.0, 10.0);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(probability, categories, n);

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble(1.0, 10.0);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(probability, categories, n);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble(1.0, 10.0);
            var categories = new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(0.0, 0.5),
                                                            random.NextDouble(0.6, 1.0),
                                                            FailureMechanismSectionAssemblyCategoryGroup.IIv)
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(probability, categories, n);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Combined Assembly

        [Test]
        public void AssembleCombinedWithProbabilities_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(new FailureMechanismSectionAssembly(random.NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup) 99),
                                                                      new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                                                                      new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var detailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var tailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);

                // Assert
                AssertAssembly(simpleAssembly, kernel.CombinedSimpleAssessmentInput);
                AssertAssembly(detailedAssembly, kernel.CombinedDetailedAssessmentInput);
                AssertAssembly(tailorMadeAssembly, kernel.CombinedTailorMadeAssessmentInput);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleCombined_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<FailureMechanismSectionCategoryGroup>(
                    FailureMechanismSectionCategoryGroup.VIv);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined((FailureMechanismSectionAssemblyCategoryGroup) 99,
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleCombined_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var detailedAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var tailorMadeAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<FailureMechanismSectionCategoryGroup>(
                    FailureMechanismSectionCategoryGroup.VIv);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);

                // Assert
                Assert.AreEqual(simpleAssembly, GetGroup(kernel.CombinedSimpleAssessmentGroupInput.Value));
                Assert.AreEqual(detailedAssembly, GetGroup(kernel.CombinedDetailedAssessmentGroupInput.Value));
                Assert.AreEqual(tailorMadeAssembly, GetGroup(kernel.CombinedTailorMadeAssessmentGroupInput.Value));
            }
        }

        [Test]
        public void AssembleCombined_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<FailureMechanismSectionCategoryGroup>(
                    FailureMechanismSectionCategoryGroup.VIv);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyCategoryGroup group = calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                Assert.AreEqual(GetGroup(kernel.FailureMechanismSectionAssemblyCategoryGroup.Result), group);
            }
        }

        [Test]
        public void AssembleCombined_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<FailureMechanismSectionCategoryGroup>(
                    (FailureMechanismSectionCategoryGroup) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleCombined_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        private static void AssertAssembly(FailureMechanismSectionAssembly simpleAssembly, FailureMechanismSectionAssemblyCategoryResult kernelInput)
        {
            Assert.AreEqual(simpleAssembly.Probability, kernelInput.EstimatedProbabilityOfFailure);
            Assert.AreEqual(simpleAssembly.Group, GetGroup(kernelInput.CategoryGroup));
        }

        #endregion
    }
}