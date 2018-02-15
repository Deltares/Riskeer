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
using AssemblyTool.Kernel.Data.AssemblyCategories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Calculators.Assembly
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCalculatorStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionAssemblyCalculator>(calculator);
            Assert.AreEqual((SimpleAssessmentResultType) 0,
                            calculator.SimpleAssessmentInput);
            Assert.AreEqual((SimpleAssessmentResultValidityOnlyType) 0,
                            calculator.SimpleAssessmentValidityOnlyInput);
            Assert.IsNull(calculator.SimpleAssessmentAssemblyOutput);
            Assert.IsNull(calculator.DetailedAssessmentWithLengthEffectInput);
            Assert.IsNull(calculator.DetailedAssessmentInput);
            Assert.IsNull(calculator.DetailedAssessmentAssemblyOutput);
        }

        #region Simple Assessment

        [Test]
        public void AssembleSimpleAssessment_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.None);

            // Assert
            Assert.AreEqual(0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, assembly.Group);
        }

        [Test]
        public void AssembleSimpleAssessment_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            const SimpleAssessmentResultType input = SimpleAssessmentResultType.None;
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleSimpleAssessment(input);

            // Assert
            Assert.AreEqual(input, calculator.SimpleAssessmentInput);
        }

        [Test]
        public void AssembleSimpleAssessment_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentResultType) 0);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType.None);

            // Assert
            Assert.AreEqual(1, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIIv, assembly.Group);
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            const SimpleAssessmentResultValidityOnlyType input = SimpleAssessmentResultValidityOnlyType.None;
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleSimpleAssessment(input);

            // Assert
            Assert.AreEqual(input, calculator.SimpleAssessmentValidityOnlyInput);
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentResultValidityOnlyType) 0);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void AssembleDetailedAssessment_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                random.NextDouble(),
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(random.NextRoundedDouble(0.0, 0.5),
                                                                random.NextRoundedDouble(0.6, 1.0),
                                                                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                });

            // Assert
            Assert.AreEqual(1.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIIv, assembly.Group);
        }

        [Test]
        public void AssembleDetailedAssessment_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextRoundedDouble(0.0, 0.5);
            double upperBoundary = random.NextRoundedDouble(0.6, 1.0);

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleDetailedAssessment(
                probability,
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                upperBoundary,
                                                                FailureMechanismSectionAssemblyCategoryGroup.IIv)
                });

            // Assert
            Assert.AreEqual(probability, calculator.DetailedAssessmentInput.Probability);

            FailureMechanismSectionCategory actualSectionCategory = calculator.DetailedAssessmentInput.Categories.Single();
            Assert.AreEqual(lowerBoundary, actualSectionCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualSectionCategory.UpperBoundary);
            Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualSectionCategory.CategoryGroup);
        }

        [Test]
        public void AssembleDetailedAssessment_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleDetailedAssessment(
                random.NextDouble(),
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(random.NextRoundedDouble(0.0, 0.5),
                                                                random.NextRoundedDouble(0.6, 1.0),
                                                                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                });

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                random.NextDouble(),
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(random.NextRoundedDouble(0.0, 0.5),
                                                                random.NextRoundedDouble(0.6, 1.0),
                                                                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                },
                random.NextRoundedDouble(1.0, 10.0));

            // Assert
            Assert.AreEqual(0.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIIv, assembly.Group);
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextRoundedDouble(0.0, 0.5);
            double upperBoundary = random.NextRoundedDouble(0.6, 1.0);
            double n = random.NextRoundedDouble(1.0, 10.0);

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleDetailedAssessment(
                probability,
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                upperBoundary,
                                                                FailureMechanismSectionAssemblyCategoryGroup.IIv)
                },
                n);

            // Assert
            Assert.AreEqual(probability, calculator.DetailedAssessmentWithLengthEffectInput.Probability);
            Assert.AreEqual(n, calculator.DetailedAssessmentWithLengthEffectInput.NValue);

            FailureMechanismSectionCategory actualSectionCategory = calculator.DetailedAssessmentWithLengthEffectInput.Categories.Single();
            Assert.AreEqual(lowerBoundary, actualSectionCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualSectionCategory.UpperBoundary);
            Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualSectionCategory.CategoryGroup);
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleDetailedAssessment(
                random.NextDouble(),
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(random.NextRoundedDouble(0.0, 0.5),
                                                                random.NextRoundedDouble(0.6, 1.0),
                                                                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                },
                random.NextRoundedDouble(1.0, 10.0));

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        #endregion
    }
}