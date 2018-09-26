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
using System.Linq;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.Exceptions;

namespace Ringtoets.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyToolCategoriesFactoryTest
    {
        [Test]
        public void CreateAssessmentSectionAssemblyCategories_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssemblyToolCategoriesFactory.CreateAssessmentSectionAssemblyCategories(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.AreEqual(signalingNorm, calculator.SignalingNorm);
                Assert.AreEqual(lowerLimitNorm, calculator.LowerLimitNorm);
            }
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssessmentSectionAssemblyCategory[] output = AssemblyToolCategoriesFactory.CreateAssessmentSectionAssemblyCategories(signalingNorm, lowerLimitNorm).ToArray();

                // Assert
                AssessmentSectionAssemblyCategory[] calculatorOutput = calculator.AssessmentSectionCategoriesOutput.ToArray();

                Assert.AreEqual(calculatorOutput.Length, output.Length);
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.LowerBoundary), output.Select(o => o.LowerBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.UpperBoundary), output.Select(o => o.UpperBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.Group), output.Select(o => o.Group));
            }
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate test = () => AssemblyToolCategoriesFactory.CreateAssessmentSectionAssemblyCategories(0, 0);

                // Assert
                var exception = Assert.Throws<AssemblyException>(test);
                Assert.IsInstanceOf<AssemblyCategoriesCalculatorException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategoriesWithN_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(signalingNorm,
                                                                                       lowerLimitNorm,
                                                                                       failureMechanismContribution,
                                                                                       n);

                // Assert
                AssemblyCategoriesInput assemblyCategoriesInput = calculator.AssemblyCategoriesInput;
                Assert.AreEqual(signalingNorm, assemblyCategoriesInput.SignalingNorm);
                Assert.AreEqual(lowerLimitNorm, assemblyCategoriesInput.LowerLimitNorm);
                Assert.AreEqual(failureMechanismContribution / 100, assemblyCategoriesInput.FailureMechanismContribution);
                Assert.AreEqual(n, assemblyCategoriesInput.N);
            }
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategoriesWithN_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                FailureMechanismAssemblyCategory[] output = AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(
                    signalingNorm,
                    lowerLimitNorm,
                    failureMechanismContribution,
                    n).ToArray();

                // Assert
                FailureMechanismAssemblyCategory[] calculatorOutput = calculator.FailureMechanismCategoriesOutput.ToArray();

                Assert.AreEqual(calculatorOutput.Length, output.Length);
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.LowerBoundary), output.Select(o => o.LowerBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.UpperBoundary), output.Select(o => o.UpperBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.Group), output.Select(o => o.Group));
            }
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategoriesWithN_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate test = () => AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(0, 0, 0, 0);

                // Assert
                var exception = Assert.Throws<AssemblyException>(test);
                Assert.IsInstanceOf<AssemblyCategoriesCalculatorException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategories_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();
            double failureProbabilityMarginFactor = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(signalingNorm,
                                                                                       lowerLimitNorm,
                                                                                       failureProbabilityMarginFactor);

                // Assert
                AssemblyCategoriesInput assemblyCategoriesInput = calculator.AssemblyCategoriesInput;
                Assert.AreEqual(signalingNorm, assemblyCategoriesInput.SignalingNorm);
                Assert.AreEqual(lowerLimitNorm, assemblyCategoriesInput.LowerLimitNorm);
                Assert.AreEqual(failureProbabilityMarginFactor, assemblyCategoriesInput.FailureMechanismContribution);
                Assert.AreEqual(1, assemblyCategoriesInput.N);
            }
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategories_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();
            double failureProbabilityMarginFactor = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                FailureMechanismAssemblyCategory[] output = AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(
                    signalingNorm,
                    lowerLimitNorm,
                    failureProbabilityMarginFactor).ToArray();

                // Assert
                FailureMechanismAssemblyCategory[] calculatorOutput = calculator.FailureMechanismCategoriesOutput.ToArray();

                Assert.AreEqual(calculatorOutput.Length, output.Length);
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.LowerBoundary), output.Select(o => o.LowerBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.UpperBoundary), output.Select(o => o.UpperBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.Group), output.Select(o => o.Group));
            }
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategories_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate test = () => AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(0, 0, 0);

                // Assert
                var exception = Assert.Throws<AssemblyException>(test);
                Assert.IsInstanceOf<AssemblyCategoriesCalculatorException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategories_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssemblyToolCategoriesFactory.CreateFailureMechanismSectionAssemblyCategories(signalingNorm,
                                                                                              lowerLimitNorm,
                                                                                              failureMechanismContribution,
                                                                                              n);

                // Assert
                AssemblyCategoriesInput assemblyCategoriesInput = calculator.AssemblyCategoriesInput;
                Assert.AreEqual(signalingNorm, assemblyCategoriesInput.SignalingNorm);
                Assert.AreEqual(lowerLimitNorm, assemblyCategoriesInput.LowerLimitNorm);
                Assert.AreEqual(failureMechanismContribution / 100, assemblyCategoriesInput.FailureMechanismContribution);
                Assert.AreEqual(n, assemblyCategoriesInput.N);
            }
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategories_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                FailureMechanismSectionAssemblyCategory[] output = AssemblyToolCategoriesFactory.CreateFailureMechanismSectionAssemblyCategories(
                    signalingNorm,
                    lowerLimitNorm,
                    failureMechanismContribution,
                    n).ToArray();

                // Assert
                FailureMechanismSectionAssemblyCategory[] calculatorOutput = calculator.FailureMechanismSectionCategoriesOutput.ToArray();

                Assert.AreEqual(calculatorOutput.Length, output.Length);
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.LowerBoundary), output.Select(o => o.LowerBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.UpperBoundary), output.Select(o => o.UpperBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.Group), output.Select(o => o.Group));
            }
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategories_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate test = () => AssemblyToolCategoriesFactory.CreateFailureMechanismSectionAssemblyCategories(0, 0, 0, 0);

                // Assert
                var exception = Assert.Throws<AssemblyException>(test);
                Assert.IsInstanceOf<AssemblyCategoriesCalculatorException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void CreateGeotechnicalFailureMechanismSectionAssemblyCategories_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            double normativeNorm = random.NextDouble();
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssemblyToolCategoriesFactory.CreateGeotechnicalFailureMechanismSectionAssemblyCategories(normativeNorm,
                                                                                                          failureMechanismContribution,
                                                                                                          n);

                // Assert
                Assert.AreEqual(normativeNorm, calculator.NormativeNorm);
                Assert.AreEqual(failureMechanismContribution / 100, calculator.FailureMechanismContribution);
                Assert.AreEqual(n, calculator.FailureMechanismN);
            }
        }

        [Test]
        public void CreateGeotechnicalFailureMechanismSectionAssemblyCategories_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(11);
            double normativeNorm = random.NextDouble();
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                FailureMechanismSectionAssemblyCategory[] output = AssemblyToolCategoriesFactory.CreateGeotechnicalFailureMechanismSectionAssemblyCategories(
                    normativeNorm,
                    failureMechanismContribution,
                    n).ToArray();

                // Assert
                FailureMechanismSectionAssemblyCategory[] calculatorOutput = calculator.GeotechnicalFailureMechanismSectionCategoriesOutput.ToArray();

                Assert.AreEqual(calculatorOutput.Length, output.Length);
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.LowerBoundary), output.Select(o => o.LowerBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.UpperBoundary), output.Select(o => o.UpperBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.Group), output.Select(o => o.Group));
            }
        }

        [Test]
        public void CreateGeotechnicalFailureMechanismSectionAssemblyCategories_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate test = () => AssemblyToolCategoriesFactory.CreateGeotechnicalFailureMechanismSectionAssemblyCategories(0, 0, 0);

                // Assert
                var exception = Assert.Throws<AssemblyException>(test);
                Assert.IsInstanceOf<AssemblyCategoriesCalculatorException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }
    }
}