﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;

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
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnsCategories()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            AssessmentSectionAssemblyCategory[] result = calculator.CalculateAssessmentSectionCategories(0, 0).ToArray();

            // Assert
            Assert.AreEqual(3, result.Length);
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
            var calculator = new AssemblyCategoriesCalculatorStub();
            calculator.AssessmentSectionCategoriesOutput = new[]
            {
                new AssessmentSectionAssemblyCategory(1, 2, AssessmentSectionAssemblyCategoryGroup.A),
                new AssessmentSectionAssemblyCategory(4.01, 5, AssessmentSectionAssemblyCategoryGroup.D)
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
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_ReturnsCategories()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            FailureMechanismSectionAssemblyCategory[] result = calculator.CalculateFailureMechanismSectionCategories(CreateAssemblyCategoriesInput()).ToArray();

            // Assert
            Assert.AreEqual(3, result.Length);
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
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var assemblyCategoriesInput = new AssemblyCategoriesInput(random.NextDouble(1, 5),
                                                                      random.NextDouble(), 
                                                                      random.NextDouble(0.0, 0.4), 
                                                                      random.NextDouble(0.5, 1.0));

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
        public void CalculateGeotechnicFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_ReturnsCategories()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            FailureMechanismSectionAssemblyCategory[] result = calculator.CalculateGeotechnicFailureMechanismSectionCategories(
                CreateAssemblyCategoriesInput()).ToArray();

            // Assert
            Assert.AreEqual(3, result.Length);
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
        public void CalculateGeotechnicFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var assemblyCategoriesInput = new AssemblyCategoriesInput(random.NextDouble(1, 5),
                                                                      random.NextDouble(), 
                                                                      random.NextDouble(0.0, 0.4), 
                                                                      random.NextDouble(0.5, 1.0));

            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            calculator.CalculateGeotechnicFailureMechanismSectionCategories(
                assemblyCategoriesInput);

            // Assert
            Assert.AreSame(assemblyCategoriesInput, calculator.AssemblyCategoriesInput);
        }

        [Test]
        public void CalculateGeotechnicFailureMechanismSectionCategories_ThrowExceptionOnCalculateTrue_ThrowsAssemblyCategoriesCalculatorException()
        {
            // Setup
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.CalculateGeotechnicFailureMechanismSectionCategories(CreateAssemblyCategoriesInput());

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