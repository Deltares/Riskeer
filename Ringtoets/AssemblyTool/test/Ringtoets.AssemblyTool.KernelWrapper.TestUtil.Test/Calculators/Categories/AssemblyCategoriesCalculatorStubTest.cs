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
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Calculators.Categories
{
    [TestFixture]
    public class AssemblyCategoriesCalculatorStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculator = new AssemblyCategoriesCalculatorStub();

            // Assert
            Assert.IsInstanceOf<IAssemblyCategoriesCalculator>(calculator);
            Assert.IsNull(calculator.AssessmentSectionCategoriesOutput);
            Assert.IsNull(calculator.FailureMechanismSectionCategoriesOutput);
            Assert.IsNull(calculator.AssemblyCategoriesInput);
            Assert.AreEqual(0.0, calculator.SignalingNorm);
            Assert.AreEqual(0.0, calculator.LowerLimitNorm);
            Assert.AreEqual(0.0, calculator.NormativeNorm);
            Assert.AreEqual(0.0, calculator.FailureMechanismN);
            Assert.AreEqual(0.0, calculator.FailureMechanismContribution);
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnsCategories()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> result = calculator.CalculateAssessmentSectionCategories(0, 0);

            // Assert
            Assert.AreSame(calculator.AssessmentSectionCategoriesOutput, result);
            Assert.AreEqual(3, result.Count());
            CollectionAssert.AreEqual(new[]
            {
                1,
                2.01,
                3.01
            }, result.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(new[]
            {
                2,
                3,
                4
            }, result.Select(r => r.UpperBoundary));
            CollectionAssert.AreEqual(new[]
            {
                AssessmentSectionAssemblyCategoryGroup.A,
                AssessmentSectionAssemblyCategoryGroup.B,
                AssessmentSectionAssemblyCategoryGroup.C
            }, result.Select(r => r.Group));
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnsCategories()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                AssessmentSectionCategoriesOutput = Enumerable.Empty<AssessmentSectionAssemblyCategory>()
            };

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> result = calculator.CalculateAssessmentSectionCategories(0, 0);

            // Assert
            Assert.AreSame(calculator.AssessmentSectionCategoriesOutput, result);
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

            // Assert
            Assert.AreEqual(signalingNorm, calculator.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, calculator.LowerLimitNorm);
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateTrue_ThrowsAssemblyCategoriesCalculatorException()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.CalculateAssessmentSectionCategories(0, 0);

            // Assert
            var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void CalculateFailureMechanismCategories_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnsCategories()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            IEnumerable<FailureMechanismAssemblyCategory> result = calculator.CalculateFailureMechanismCategories(CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.FailureMechanismCategoriesOutput, result);
            Assert.AreEqual(3, result.Count());
            CollectionAssert.AreEqual(new[]
            {
                1,
                2.01,
                3.01
            }, result.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(new[]
            {
                2,
                3,
                4
            }, result.Select(r => r.UpperBoundary));
            CollectionAssert.AreEqual(new[]
            {
                FailureMechanismAssemblyCategoryGroup.It,
                FailureMechanismAssemblyCategoryGroup.IIt,
                FailureMechanismAssemblyCategoryGroup.IIIt
            }, result.Select(r => r.Group));
        }

        [Test]
        public void CalculateFailureMechanismCategories_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnsCategories()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                FailureMechanismCategoriesOutput = Enumerable.Empty<FailureMechanismAssemblyCategory>()
            };

            // Call
            IEnumerable<FailureMechanismAssemblyCategory> result = calculator.CalculateFailureMechanismCategories(CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.FailureMechanismCategoriesOutput, result);
        }

        [Test]
        public void CalculateFailureMechanismCategories_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            calculator.CalculateFailureMechanismCategories(
                assemblyCategoriesInput);

            // Assert
            Assert.AreSame(assemblyCategoriesInput, calculator.AssemblyCategoriesInput);
        }

        [Test]
        public void CalculateFailureMechanismCategories_ThrowExceptionOnCalculateTrue_ThrowsAssemblyCategoriesCalculatorException()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.CalculateFailureMechanismCategories(CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnsCategories()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            IEnumerable<FailureMechanismSectionAssemblyCategory> result = calculator.CalculateFailureMechanismSectionCategories(CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.FailureMechanismSectionCategoriesOutput, result);
            Assert.AreEqual(3, result.Count());
            CollectionAssert.AreEqual(new[]
            {
                1,
                2.01,
                3.01
            }, result.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(new[]
            {
                2,
                3,
                4
            }, result.Select(r => r.UpperBoundary));
            CollectionAssert.AreEqual(new[]
            {
                FailureMechanismSectionAssemblyCategoryGroup.Iv,
                FailureMechanismSectionAssemblyCategoryGroup.IIv,
                FailureMechanismSectionAssemblyCategoryGroup.IIIv
            }, result.Select(r => r.Group));
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnsCategories()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                FailureMechanismSectionCategoriesOutput = Enumerable.Empty<FailureMechanismSectionAssemblyCategory>()
            };

            // Call
            IEnumerable<FailureMechanismSectionAssemblyCategory> result = calculator.CalculateFailureMechanismSectionCategories(CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.FailureMechanismSectionCategoriesOutput, result);
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            calculator.CalculateFailureMechanismSectionCategories(
                assemblyCategoriesInput);

            // Assert
            Assert.AreSame(assemblyCategoriesInput, calculator.AssemblyCategoriesInput);
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateTrue_ThrowsAssemblyCategoriesCalculatorException()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.CalculateFailureMechanismSectionCategories(CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void CalculateGeotechnicalFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnsCategories()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            IEnumerable<FailureMechanismSectionAssemblyCategory> result = calculator.CalculateGeotechnicalFailureMechanismSectionCategories(random.NextDouble(),
                                                                                                                                            random.NextDouble(),
                                                                                                                                            random.NextDouble());

            // Assert
            Assert.AreSame(calculator.GeotechnicalFailureMechanismSectionCategoriesOutput, result);
            Assert.AreEqual(3, result.Count());
            CollectionAssert.AreEqual(new[]
            {
                1,
                2.2,
                3.2
            }, result.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(new[]
            {
                2.1,
                3.1,
                4
            }, result.Select(r => r.UpperBoundary));
            CollectionAssert.AreEqual(new[]
            {
                FailureMechanismSectionAssemblyCategoryGroup.IIIv,
                FailureMechanismSectionAssemblyCategoryGroup.IVv,
                FailureMechanismSectionAssemblyCategoryGroup.Vv
            }, result.Select(r => r.Group));
        }

        [Test]
        public void CalculateGeotechnicalFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnsCategories()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                GeotechnicalFailureMechanismSectionCategoriesOutput = Enumerable.Empty<FailureMechanismSectionAssemblyCategory>()
            };

            // Call
            IEnumerable<FailureMechanismSectionAssemblyCategory> result = calculator.CalculateGeotechnicalFailureMechanismSectionCategories(random.NextDouble(),
                                                                                                                                            random.NextDouble(),
                                                                                                                                            random.NextDouble());

            // Assert
            Assert.AreSame(calculator.GeotechnicalFailureMechanismSectionCategoriesOutput, result);
        }

        [Test]
        public void CalculateGeotechnicalFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(21);
            double normativeNorm = random.NextDouble();
            double n = random.NextDouble();
            double failureMechanismContribution = random.NextDouble();

            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            calculator.CalculateGeotechnicalFailureMechanismSectionCategories(normativeNorm, n, failureMechanismContribution);

            // Assert
            Assert.AreEqual(normativeNorm, calculator.NormativeNorm);
            Assert.AreEqual(n, calculator.FailureMechanismN);
            Assert.AreEqual(failureMechanismContribution, calculator.FailureMechanismContribution);
        }

        [Test]
        public void CalculateGeotechnicalFailureMechanismSectionCategories_ThrowExceptionOnCalculateTrue_ThrowsAssemblyCategoriesCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.CalculateGeotechnicalFailureMechanismSectionCategories(random.NextDouble(),
                                                                                                        random.NextDouble(),
                                                                                                        random.NextDouble());

            // Assert
            var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput()
        {
            return new AssemblyCategoriesInput(0, 0, 0, 0);
        }
    }
}