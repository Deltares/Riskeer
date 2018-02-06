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
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.AssemblyTool;

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
        }

        [Test]
        public void CalculateAssessmentSectionCategories_OutputNotSetAndThrowExceptionOnCalculateFalse_ReturnsCategories()
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
        public void CalculateAssessmentSectionCategories_OutputSetAndThrowExceptionOnCalculateFalse_ReturnsCategories()
        {
            // Setup
            var output = new AssessmentSectionAssemblyCategory[0];
            var calculator = new AssemblyCategoriesCalculatorStub
            {
                AssessmentSectionCategoriesOutput = output
            };

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> result = calculator.CalculateAssessmentSectionCategories(0 ,0);

            // Assert
            Assert.AreSame(output, result);
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double signalingNorm = random.NextDouble();
            double lowerBoundaryNorm = random.NextDouble();

            var calculator = new AssemblyCategoriesCalculatorStub();

            // Call
            calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerBoundaryNorm);

            // Assert
            Assert.AreEqual(signalingNorm, calculator.SignalingNorm);
            Assert.AreEqual(lowerBoundaryNorm, calculator.LowerBoundaryNorm);
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
    }
}