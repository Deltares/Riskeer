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
            Assert.AreEqual((SimpleAssessmentValidityOnlyResultType) 0, calculator.SimpleAssessmentValidityOnlyInput);
            Assert.IsNull(calculator.SimpleAssessmentAssemblyOutput);

            Assert.AreEqual(0.0, calculator.DetailedAssessmentFailureMechanismSectionNInput);
            Assert.AreEqual(0.0, calculator.DetailedAssessmentFailureMechanismNInput);
            Assert.AreEqual(0.0, calculator.DetailedAssessmentProbabilityInput);
            Assert.AreEqual((DetailedAssessmentProbabilityOnlyResultType) 0, calculator.DetailedAssessmentProbabilityOnlyResultInput);
            Assert.IsNull(calculator.DetailedAssessmentAssemblyOutput);

            Assert.AreEqual((DetailedAssessmentResultType) 0, calculator.DetailedAssessmentResultForFactorizedSignalingNormInput);
            Assert.AreEqual((DetailedAssessmentResultType) 0, calculator.DetailedAssessmentResultForSignalingNormInput);
            Assert.AreEqual((DetailedAssessmentResultType) 0, calculator.DetailedAssessmentResultForMechanismSpecificLowerLimitNormInput);
            Assert.AreEqual((DetailedAssessmentResultType) 0, calculator.DetailedAssessmentResultForLowerLimitNormInput);
            Assert.AreEqual((DetailedAssessmentResultType) 0, calculator.DetailedAssessmentResultForFactorizedLowerLimitNormInput);
            Assert.IsNull(calculator.DetailedAssessmentAssemblyGroupOutput);

            Assert.AreEqual(0.0, calculator.TailorMadeAssessmentProbabilityInput);

            Assert.AreEqual((TailorMadeAssessmentProbabilityAndDetailedCalculationResultType) 0,
                            calculator.TailorMadeAssessmentProbabilityAndDetailedCalculationResultInput);
            Assert.AreEqual((TailorMadeAssessmentProbabilityCalculationResultType) 0,
                            calculator.TailorMadeAssessmentProbabilityCalculationResultInput);
            Assert.IsNull(calculator.TailorMadeAssessmentAssemblyOutput);

            Assert.AreEqual(0.0, calculator.TailorMadeAssessmentFailureMechanismContributionInput);
            Assert.AreEqual(0.0, calculator.TailorMadeAssessmentFailureMechanismNInput);
            Assert.AreEqual(0.0, calculator.TailorMadeAssessmentNormativeNormInput);

            Assert.IsNull(calculator.CombinedSimpleAssemblyInput);
            Assert.IsNull(calculator.CombinedDetailedAssemblyInput);
            Assert.IsNull(calculator.CombinedTailorMadeAssemblyInput);
            Assert.IsNull(calculator.CombinedAssemblyOutput);

            Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedSimpleAssemblyGroupInput);
            Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedDetailedAssemblyGroupInput);
            Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedTailorMadeAssemblyGroupInput);
            Assert.IsNull(calculator.CombinedAssemblyCategoryOutput);

            Assert.AreEqual(0.0, calculator.ManualAssemblyNInput);
            Assert.AreEqual(0.0, calculator.ManualAssemblyProbabilityInput);
            Assert.IsNull(calculator.ManualAssemblyAssemblyOutput);
            Assert.IsNull(calculator.ManualAssemblyCategoriesInput);

            Assert.IsNull(calculator.AssemblyCategoriesInput);
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput()
        {
            var random = new Random(39);
            return new AssemblyCategoriesInput(random.NextDouble(),
                                               random.NextDouble(),
                                               random.NextDouble(),
                                               random.NextDouble());
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
        public void AssembleSimpleAssessment_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.None);

            // Assert
            Assert.AreSame(calculator.SimpleAssessmentAssemblyOutput, assembly);
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
            FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(SimpleAssessmentValidityOnlyResultType.None);

            // Assert
            Assert.AreEqual(1, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIIv, assembly.Group);
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(SimpleAssessmentValidityOnlyResultType.None);

            // Assert
            Assert.AreSame(calculator.SimpleAssessmentAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            const SimpleAssessmentValidityOnlyResultType input = SimpleAssessmentValidityOnlyResultType.None;
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
            TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentValidityOnlyResultType) 0);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void AssembleDetailedAssessmentWithDetailedAssessmentResult_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleDetailedAssessment(random.NextEnumValue<DetailedAssessmentResultType>());

            // Assert
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, assembly);
        }

        [Test]
        public void AssembleDetailedAssessmentWithDetailedAssessmentResult_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                DetailedAssessmentAssemblyGroupOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleDetailedAssessment(random.NextEnumValue<DetailedAssessmentResultType>());

            // Assert
            Assert.AreEqual(calculator.DetailedAssessmentAssemblyGroupOutput, assembly);
        }

        [Test]
        public void AssembleDetailedAssessmentWithDetailedAssessmentResult_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentResultType>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleDetailedAssessment(detailedAssessmentResult);

            // Assert
            Assert.AreEqual(detailedAssessmentResult, calculator.DetailedAssessmentResultInput);
        }

        [Test]
        public void AssembleDetailedAssessmentWithDetailedAssessmentResult_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleDetailedAssessment(random.NextEnumValue<DetailedAssessmentResultType>());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleDetailedAssessment_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreEqual(1.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, assembly.Group);
        }

        [Test]
        public void AssembleDetailedAssessment_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.DetailedAssessmentAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleDetailedAssessment_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleDetailedAssessment(detailedAssessmentResult, probability, assemblyCategoriesInput);

            // Assert
            Assert.AreEqual(detailedAssessmentResult, calculator.DetailedAssessmentProbabilityOnlyResultInput);
            Assert.AreEqual(probability, calculator.DetailedAssessmentProbabilityInput);
            Assert.AreSame(assemblyCategoriesInput, calculator.AssemblyCategoriesInput);
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
                random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble());

            // Assert
            Assert.AreEqual(0.25, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IVv, assembly.Group);
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble());

            // Assert
            Assert.AreSame(calculator.DetailedAssessmentAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double normativeNorm = random.NextDouble();
            double n = random.NextDouble(1, 10);
            double failureMechanismContribution = random.NextDouble();
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleDetailedAssessment(detailedAssessmentResult,
                                                  probability,
                                                  normativeNorm,
                                                  n,
                                                  failureMechanismContribution);

            // Assert
            Assert.AreEqual(detailedAssessmentResult, calculator.DetailedAssessmentProbabilityOnlyResultInput);
            Assert.AreEqual(probability, calculator.DetailedAssessmentProbabilityInput);
            Assert.AreEqual(normativeNorm, calculator.DetailedAssessmentNormativeNormInput);
            Assert.AreEqual(n, calculator.DetailedAssessmentFailureMechanismNInput);
            Assert.AreEqual(failureMechanismContribution, calculator.DetailedAssessmentFailureMechanismContribution);
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleDetailedAssessment(
                random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(1, 10),
                random.NextDouble());

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
                random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                random.NextDouble(),
                random.NextDouble(1.0, 10.0),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreEqual(0.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, assembly.Group);
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                random.NextDouble(),
                random.NextDouble(1.0, 10.0),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.DetailedAssessmentAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble(1.0, 10.0);
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleDetailedAssessment(detailedAssessmentResult, probability, n, assemblyCategoriesInput);

            // Assert
            Assert.AreEqual(detailedAssessmentResult, calculator.DetailedAssessmentProbabilityOnlyResultInput);
            Assert.AreEqual(probability, calculator.DetailedAssessmentProbabilityInput);
            Assert.AreEqual(n, calculator.DetailedAssessmentFailureMechanismSectionNInput);
            Assert.AreSame(assemblyCategoriesInput, calculator.AssemblyCategoriesInput);
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
                random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                random.NextDouble(),
                random.NextDouble(1.0, 10.0),
                CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleDetailedAssessment(
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>());

            // Assert
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, assembly);
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                DetailedAssessmentAssemblyGroupOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleDetailedAssessment(
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>());

            // Assert
            Assert.AreEqual(calculator.DetailedAssessmentAssemblyGroupOutput, assembly);
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var detailedAssessmentResultForFactorizedSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForMechanismSpecificLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForFactorizedLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleDetailedAssessment(detailedAssessmentResultForFactorizedSignalingNorm,
                                                  detailedAssessmentResultForSignalingNorm,
                                                  detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                                                  detailedAssessmentResultForLowerLimitNorm,
                                                  detailedAssessmentResultForFactorizedLowerLimitNorm);

            // Assert
            Assert.AreEqual(detailedAssessmentResultForFactorizedSignalingNorm, calculator.DetailedAssessmentResultForFactorizedSignalingNormInput);
            Assert.AreEqual(detailedAssessmentResultForSignalingNorm, calculator.DetailedAssessmentResultForSignalingNormInput);
            Assert.AreEqual(detailedAssessmentResultForMechanismSpecificLowerLimitNorm, calculator.DetailedAssessmentResultForMechanismSpecificLowerLimitNormInput);
            Assert.AreEqual(detailedAssessmentResultForLowerLimitNorm, calculator.DetailedAssessmentResultForLowerLimitNormInput);
            Assert.AreEqual(detailedAssessmentResultForFactorizedLowerLimitNorm, calculator.DetailedAssessmentResultForFactorizedLowerLimitNormInput);
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
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
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>(),
                random.NextEnumValue<DetailedAssessmentResultType>());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        public void AssembleTailorMadeAssessmentWithTailorMadeAssessmentResult_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleTailorMadeAssessment(random.NextEnumValue<TailorMadeAssessmentResultType>());

            // Assert
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, assembly);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithTailorMadeAssessmentResult_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                TailorMadeAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleTailorMadeAssessment(random.NextEnumValue<TailorMadeAssessmentResultType>());

            // Assert
            Assert.AreEqual(calculator.TailorMadeAssemblyCategoryOutput, assembly);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithTailorMadeAssessmentResult_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult);

            // Assert
            Assert.AreEqual(tailorMadeAssessmentResult, calculator.TailorMadeAssessmentResultInput);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithTailorMadeAssessmentResult_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleTailorMadeAssessment(random.NextEnumValue<TailorMadeAssessmentResultType>());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble());

            // Assert
            Assert.AreEqual(1.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, assembly.Group);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble());

            // Assert
            Assert.AreSame(calculator.TailorMadeAssessmentAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>();
            double probability = random.NextDouble();
            double normativeNorm = random.NextDouble();
            double n = random.NextDouble();
            double failureMechanismContribution = random.NextDouble();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, normativeNorm, n, failureMechanismContribution);

            // Assert
            Assert.AreEqual(tailorMadeAssessmentResult, calculator.TailorMadeAssessmentProbabilityAndDetailedCalculationResultInput);
            Assert.AreEqual(probability, calculator.TailorMadeAssessmentProbabilityInput);
            Assert.AreEqual(normativeNorm, calculator.TailorMadeAssessmentNormativeNormInput);
            Assert.AreEqual(n, calculator.TailorMadeAssessmentFailureMechanismNInput);
            Assert.AreEqual(failureMechanismContribution, calculator.TailorMadeAssessmentFailureMechanismContributionInput);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreEqual(1.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, assembly.Group);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.TailorMadeAssessmentAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, assemblyCategoriesInput);

            // Assert
            Assert.AreEqual(tailorMadeAssessmentResult, calculator.TailorMadeAssessmentProbabilityCalculationResultInput);
            Assert.AreEqual(probability, calculator.TailorMadeAssessmentProbabilityInput);
            Assert.AreSame(assemblyCategoriesInput, calculator.AssemblyCategoriesInput);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResultWithLengthEffect_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                random.NextDouble(),
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreEqual(1.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, assembly.Group);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResultWithLengthEffect_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                random.NextDouble(),
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.TailorMadeAssessmentAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResultWithLengthEffect_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble();
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, n, assemblyCategoriesInput);

            // Assert
            Assert.AreEqual(tailorMadeAssessmentResult, calculator.TailorMadeAssessmentProbabilityCalculationResultInput);
            Assert.AreEqual(probability, calculator.TailorMadeAssessmentProbabilityInput);
            Assert.AreEqual(n, calculator.TailorMadeAssessmentFailureMechanismSectionNInput);
            Assert.AreSame(assemblyCategoriesInput, calculator.AssemblyCategoriesInput);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResultWithLengthEffect_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                random.NextDouble(),
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryGroupResult_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>());

            // Assert
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.Iv, assembly);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryGroupResult_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                TailorMadeAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleTailorMadeAssessment(
                random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>());

            // Assert
            Assert.AreEqual(calculator.TailorMadeAssemblyCategoryOutput, assembly);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryGroupResult_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult);

            // Assert
            Assert.AreEqual(tailorMadeAssessmentResult, calculator.TailorMadeAssessmentCategoryGroupResultInput);
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryGroupResult_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleTailorMadeAssessment(random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        #endregion

        #region Combined Assembly

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = new FailureMechanismSectionAssembly(
                random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly combinedAssembly = calculator.AssembleCombined(simpleAssembly);

            // Assert
            Assert.AreSame(simpleAssembly, combinedAssembly);
        }

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                CombinedAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly combinedAssembly = calculator.AssembleCombined(null);

            // Assert
            Assert.AreSame(calculator.CombinedAssemblyOutput, combinedAssembly);
        }

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = new FailureMechanismSectionAssembly(
                random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleCombined(simpleAssembly);

            // Assert
            Assert.AreSame(simpleAssembly, calculator.CombinedSimpleAssemblyInput);
        }

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculateCombinedAssembly = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleCombined(null);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleCombinedWithProbabilities_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputNotSet_ReturnOutput()
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
        public void AssembleCombinedWithProbabilities_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                CombinedAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly combinedAssembly = calculator.AssembleCombined(null, null, null);

            // Assert
            Assert.AreSame(calculator.CombinedAssemblyOutput, combinedAssembly);
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
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssemblyCategoryGroup combinedAssembly = calculator.AssembleCombined(
                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                tailorMadeAssembly);

            // Assert
            Assert.AreEqual(tailorMadeAssembly, combinedAssembly);
        }

        [Test]
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismSectionAssemblyCategoryGroup combinedAssembly = calculator.AssembleCombined(0, 0);

            // Assert
            Assert.AreEqual(calculator.CombinedAssemblyCategoryOutput, combinedAssembly);
        }

        [Test]
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var tailorMadeAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleCombined(simpleAssembly, tailorMadeAssembly);

            // Assert
            Assert.AreEqual(simpleAssembly, calculator.CombinedSimpleAssemblyGroupInput);
            Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedDetailedAssemblyGroupInput);
            Assert.AreEqual(tailorMadeAssembly, calculator.CombinedTailorMadeAssemblyGroupInput);
        }

        [Test]
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculateCombinedAssembly = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleCombined(0, 0);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssemblyCategoryGroup combinedAssembly = calculator.AssembleCombined(simpleAssembly);

            // Assert
            Assert.AreEqual(simpleAssembly, combinedAssembly);
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismSectionAssemblyCategoryGroup combinedAssembly = calculator.AssembleCombined(0);

            // Assert
            Assert.AreEqual(calculator.CombinedAssemblyCategoryOutput, combinedAssembly);
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleCombined(simpleAssembly);

            // Assert
            Assert.AreEqual(simpleAssembly, calculator.CombinedSimpleAssemblyGroupInput);
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_ThrowExceptionOnCalculateCombinedAssemblyTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculateCombinedAssembly = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleCombined(0);

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleCombined_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputNotSet_ReturnOutput()
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
        public void AssembleCombined_ThrowExceptionOnCalculateCombinedAssemblyFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
            };

            // Call
            FailureMechanismSectionAssemblyCategoryGroup combinedAssembly = calculator.AssembleCombined(0, 0, 0);

            // Assert
            Assert.AreEqual(calculator.CombinedAssemblyCategoryOutput, combinedAssembly);
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

        #region Manual Assessment

        [Test]
        public void AssembleManual_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleManual(
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreEqual(0.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, assembly.Group);
        }

        [Test]
        public void AssembleManual_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ManualAssemblyAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleManual(
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.ManualAssemblyAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleManual_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleManual(probability, assemblyCategoriesInput);

            // Assert
            Assert.AreEqual(probability, calculator.ManualAssemblyProbabilityInput);
            Assert.AreSame(assemblyCategoriesInput, calculator.ManualAssemblyCategoriesInput);
        }

        [Test]
        public void AssembleManual_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleManual(
                random.NextDouble(),
                CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        [Test]
        public void AssembleManualWithLengthEffect_ThrowExceptionOnCalculateFalseAndOutputNotSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleManual(
                random.NextDouble(),
                random.NextDouble(1.0, 10.0),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreEqual(1.0, assembly.Probability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIIv, assembly.Group);
        }

        [Test]
        public void AssembleManualWithLengthEffect_ThrowExceptionOnCalculateFalseAndOutputSet_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ManualAssemblyAssemblyOutput = new FailureMechanismSectionAssembly(
                    random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };

            // Call
            FailureMechanismSectionAssembly assembly = calculator.AssembleManual(
                random.NextDouble(),
                random.NextDouble(1.0, 10.0),
                CreateAssemblyCategoriesInput());

            // Assert
            Assert.AreSame(calculator.ManualAssemblyAssemblyOutput, assembly);
        }

        [Test]
        public void AssembleManualWithLengthEffect_ThrowExceptionOnCalculateFalse_SetsInput()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble(1.0, 10.0);
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            var calculator = new FailureMechanismSectionAssemblyCalculatorStub();

            // Call
            calculator.AssembleManual(probability, n, assemblyCategoriesInput);

            // Assert
            Assert.AreEqual(probability, calculator.ManualAssemblyProbabilityInput);
            Assert.AreEqual(n, calculator.ManualAssemblyNInput);
            Assert.AreSame(assemblyCategoriesInput, calculator.ManualAssemblyCategoriesInput);
        }

        [Test]
        public void AssembleManualWithLengthEffect_ThrowExceptionOnCalculateTrue_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var calculator = new FailureMechanismSectionAssemblyCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => calculator.AssembleManual(
                random.NextDouble(),
                random.NextDouble(1.0, 10.0),
                CreateAssemblyCategoriesInput());

            // Assert
            var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
        }

        #endregion
    }
}