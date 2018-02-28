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
            Assert.AreEqual((SimpleAssessmentResultType) 0, calculator.SimpleAssessmentInput);
            Assert.AreEqual((SimpleAssessmentResultValidityOnlyType) 0, calculator.SimpleAssessmentValidityOnlyInput);
            Assert.IsNull(calculator.SimpleAssessmentAssemblyOutput);

            Assert.IsNull(calculator.DetailedAssessmentCategoriesInput);
            Assert.AreEqual(0.0, calculator.DetailedAssessmentNInput);
            Assert.AreEqual(0.0, calculator.DetailedAssessmentProbabilityInput);
            Assert.AreEqual((DetailedAssessmentResultType) 0, calculator.DetailedAssessmentResultInput);
            Assert.IsNull(calculator.DetailedAssessmentAssemblyOutput);

            Assert.IsNull(calculator.TailorMadeAssessmentCategoriesInput);
            Assert.AreEqual(0.0, calculator.TailorMadeAssessmentProbabilityInput);
            Assert.AreEqual((TailorMadeAssessmentResultType) 0, calculator.TailorMadeAssessmentResultInput);
            Assert.IsNull(calculator.TailorMadeAssessmentAssemblyOutput);

            Assert.IsNull(calculator.CombinedSimpleAssemblyInput);
            Assert.IsNull(calculator.CombinedDetailedAssemblyInput);
            Assert.IsNull(calculator.CombinedTailorMadeAssemblyInput);
            Assert.IsNull(calculator.CombinedAssemblyOutput);
        }

        #region Simple Assessment

        [Test]
        public void AssembleSimpleAssessment_ThrowExceptionOnCalculateFalse_ReturnOutput()
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
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateFalse_ReturnOutput()
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
        public void AssembleDetailedAssessment_ThrowExceptionOnCalculateFalse_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextDouble(),
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(random.NextRoundedDouble(0.0, 0.5),
                                                                random.NextRoundedDouble(0.6, 1.0),
                                                                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                });

            // Assert
            Assert.AreEqual(1.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, assembly.Group);
        }

        [Test]
        public void AssembleDetailedAssessment_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentResultType>();

            IEnumerable<FailureMechanismSectionAssemblyCategory> categoryInput = Enumerable.Empty<FailureMechanismSectionAssemblyCategory>();
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleDetailedAssessment(detailedAssessmentResult, probability, categoryInput);

            // Assert
            Assert.AreEqual(detailedAssessmentResult, calculator.DetailedAssessmentResultInput);
            Assert.AreEqual(probability, calculator.DetailedAssessmentProbabilityInput);
            Assert.AreSame(categoryInput, calculator.DetailedAssessmentCategoriesInput);
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
                random.NextEnumValue<DetailedAssessmentResultType>(),
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
        public void AssembleDetailedAssessmentWithLengthEffect_ThrowExceptionOnCalculateFalse_ReturnOutput()
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
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, assembly.Group);
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextRoundedDouble(1.0, 10.0);

            IEnumerable<FailureMechanismSectionAssemblyCategory> categoriesInput = Enumerable.Empty<FailureMechanismSectionAssemblyCategory>();
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleDetailedAssessment(probability, categoriesInput, n);

            // Assert
            Assert.AreEqual(probability, calculator.DetailedAssessmentProbabilityInput);
            Assert.AreEqual(n, calculator.DetailedAssessmentNInput);

            Assert.AreSame(categoriesInput, calculator.DetailedAssessmentCategoriesInput);
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

        #region Tailor Made Assessment

        [Test]
        public void AssembleTailorMadeAssessment_ThrowExceptionOnCalculateFalse_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentResultType>(),
                random.NextDouble(),
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(random.NextRoundedDouble(0.0, 0.5),
                                                                random.NextRoundedDouble(0.6, 1.0),
                                                                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                });

            // Assert
            Assert.AreEqual(1.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, assembly.Group);
        }

        [Test]
        public void AssembleTailorMadeAssessment_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();

            IEnumerable<FailureMechanismSectionAssemblyCategory> categoryInput = Enumerable.Empty<FailureMechanismSectionAssemblyCategory>();
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, categoryInput);

            // Assert
            Assert.AreEqual(tailorMadeAssessmentResult, calculator.TailorMadeAssessmentResultInput);
            Assert.AreEqual(probability, calculator.TailorMadeAssessmentProbabilityInput);
            Assert.AreSame(categoryInput, calculator.TailorMadeAssessmentCategoriesInput);
        }

        [Test]
        public void AssembleTailorMadeAssessment_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentResultType>(),
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

        #endregion

        #region Combined Assembly

        [Test]
        public void AssembleCombinedWithProbabilities_ThrowExceptionOnCalculateCombinedAssemblyFalse_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssembly = new FailureMechanismSectionAssembly(
                random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly combinedAssembly = calculator.AssembleCombined(null, null, tailorMadeAssembly);

            // Assert
            Assert.AreSame(tailorMadeAssembly, combinedAssembly);
        }

        [Test]
        public void AssembleCombinedWithProbabilities_ThrowExceptionOnCalculateCombinedAssemblyFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = new FailureMechanismSectionAssembly(
                random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var detailedAssembly = new FailureMechanismSectionAssembly(
                random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var tailorMadeAssembly = new FailureMechanismSectionAssembly(
                random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);

            // Assert
            Assert.AreSame(simpleAssembly, calculator.CombinedSimpleAssemblyInput);
            Assert.AreSame(detailedAssembly, calculator.CombinedDetailedAssemblyInput);
            Assert.AreSame(tailorMadeAssembly, calculator.CombinedTailorMadeAssemblyInput);
        }

        [Test]
        public void AssembleCombinedWithProbabilities_ThrowExceptionOnCalculateCombinedAssemblyTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculateCombinedAssembly = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleCombined(null, null, null);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleCombined_ThrowExceptionOnCalculateCombinedAssemblyFalse_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssemblyCategoryGroup combinedAssembly = calculator.AssembleCombined(0, 0, tailorMadeAssembly);

            // Assert
            Assert.AreEqual(tailorMadeAssembly, combinedAssembly);
        }

        [Test]
        public void AssembleCombined_ThrowExceptionOnCalculateCombinedAssemblyFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var detailedAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var tailorMadeAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);

            // Assert
            Assert.AreEqual(simpleAssembly, calculator.CombinedSimpleAssemblyGroupInput);
            Assert.AreEqual(detailedAssembly, calculator.CombinedDetailedAssemblyGroupInput);
            Assert.AreEqual(tailorMadeAssembly, calculator.CombinedTailorMadeAssemblyGroupInput);
        }

        [Test]
        public void AssembleCombined_ThrowExceptionOnCalculateCombinedAssemblyTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculateCombinedAssembly = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleCombined(0, 0, 0);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        #endregion
    }
}