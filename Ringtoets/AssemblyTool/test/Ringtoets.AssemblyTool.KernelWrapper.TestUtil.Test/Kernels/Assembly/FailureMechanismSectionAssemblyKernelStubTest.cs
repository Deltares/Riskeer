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
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.Assembly
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Assert
            Assert.IsInstanceOf<IAssessmentResultsTranslator>(kernel);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.AssessmentResultTypeE1Input);
            Assert.IsNull(kernel.AssessmentResultTypeE2Input);
            Assert.IsNull(kernel.AssessmentResultTypeG1Input);
            Assert.IsNull(kernel.AssessmentResultTypeG2Input);
            Assert.IsNull(kernel.AssessmentResultTypeT1Input);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNull(kernel.SectionCategoryInput);
            Assert.IsNull(kernel.SimpleAssessmentResultInput);
            Assert.IsNull(kernel.DetailedAssessmentResultInput);
            Assert.IsNull(kernel.TailorMadeAssessmentResultInput);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.FailureProbabilityInput);
            Assert.IsNull(kernel.LengthEffectFactorInput);
            Assert.IsNull(kernel.CategoryCompliancyResultsInput);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        private static FmSectionCategoryCompliancyResults CreateRandomCompliancyResults(Random random)
        {
            var compliancyResults = new FmSectionCategoryCompliancyResults();
            compliancyResults.Set(EFmSectionCategory.Iv, random.NextEnumValue<ECategoryCompliancy>());
            compliancyResults.Set(EFmSectionCategory.IIv, random.NextEnumValue<ECategoryCompliancy>());
            compliancyResults.Set(EFmSectionCategory.IIIv, random.NextEnumValue<ECategoryCompliancy>());
            compliancyResults.Set(EFmSectionCategory.IVv, random.NextEnumValue<ECategoryCompliancy>());
            compliancyResults.Set(EFmSectionCategory.Vv, random.NextEnumValue<ECategoryCompliancy>());
            return compliancyResults;
        }

        #region Simple Assessment

        [Test]
        public void TranslateAssessmentResultWbi0E1_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeE1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0E1(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeE1Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E1_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0E1(0);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E1(0);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeE1Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E2_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E2(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E3_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeE2>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0E3(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeE2Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E3_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0E3(0);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E3_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E3(0);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeE2Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E4_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E4(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void TranslateAssessmentResultWbi0G1_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeG1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0G1(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeG1Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G1_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeG1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0G1(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeG1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G1(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeG1Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G2_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G2(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G3_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0G3(section, failureMechanism, assessment, failureProbability);

            // Assert
            Assert.AreSame(section, kernel.AssessmentSectionInput);
            Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreEqual(assessment, kernel.AssessmentResultTypeG2Input);
            Assert.AreEqual(failureProbability, kernel.FailureProbabilityInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G3_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0G3(section, failureMechanism, assessment, failureProbability);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G3_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G3(section, failureMechanism, assessment, failureProbability);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeG2Input);
            Assert.IsNull(kernel.FailureProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G6_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            FmSectionCategoryCompliancyResults input = CreateRandomCompliancyResults(random);

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0G6(input);

            // Assert
            Assert.AreSame(input, kernel.CategoryCompliancyResultsInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G6_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            FmSectionCategoryCompliancyResults input = CreateRandomCompliancyResults(random);

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0G6(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G6_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            FmSectionCategoryCompliancyResults input = CreateRandomCompliancyResults(random);

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G6(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.CategoryCompliancyResultsInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G5_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0G5(section, failureMechanism, lengthEffect, assessment, failureProbability);

            // Assert
            Assert.AreSame(section, kernel.AssessmentSectionInput);
            Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreEqual(lengthEffect, kernel.LengthEffectFactorInput);
            Assert.AreEqual(assessment, kernel.AssessmentResultTypeG2Input);
            Assert.AreEqual(failureProbability, kernel.FailureProbabilityInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G5_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0G5(section, failureMechanism, lengthEffect, assessment, failureProbability);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G5_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G5(section, failureMechanism, lengthEffect, assessment, failureProbability);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.LengthEffectFactorInput);
            Assert.IsNull(kernel.AssessmentResultTypeG2Input);
            Assert.IsNull(kernel.FailureProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        public void TranslateAssessmentResultWbi0T1_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeT1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0T1(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeT1Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T1_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeT1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0T1(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeT1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T1(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeT1Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T2_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T2(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T3_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0T3(section, failureMechanism, assessment, failureProbability);

            // Assert
            Assert.AreSame(section, kernel.AssessmentSectionInput);
            Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreEqual(assessment, kernel.AssessmentResultTypeT3Input);
            Assert.AreEqual(failureProbability, kernel.FailureProbabilityInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T3_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0T3(section, failureMechanism, assessment, failureProbability);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T3_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T3(section, failureMechanism, assessment, failureProbability);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNull(kernel.FailureProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T4_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var assessment = random.NextEnumValue<EAssessmentResultTypeT3>();
            var category = random.NextEnumValue<EFmSectionCategory>();
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.TranslateAssessmentResultWbi0T4(assessment, category);

            // Assert
            Assert.AreEqual(assessment, kernel.AssessmentResultTypeT3Input);
            Assert.AreEqual(category, kernel.SectionCategoryInput);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T4_ThrowExceptionOnCalculateFalse_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var assessment = random.NextEnumValue<EAssessmentResultTypeT3>();
            var category = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T4(assessment, category);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNull(kernel.SectionCategoryInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T5_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0T5(section, failureMechanism, lengthEffect, assessment, failureProbability);

            // Assert
            Assert.AreSame(section, kernel.AssessmentSectionInput);
            Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreEqual(assessment, kernel.AssessmentResultTypeT3Input);
            Assert.AreEqual(failureProbability, kernel.FailureProbabilityInput);
            Assert.AreEqual(lengthEffect, kernel.LengthEffectFactorInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T5_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0T5(section, failureMechanism, lengthEffect, assessment, failureProbability);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T5_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4));
            var failureMechanism = new FailureMechanism(random.NextDouble(), random.NextDouble());
            var assessment = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T5(section, failureMechanism, lengthEffect, assessment, failureProbability);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNull(kernel.FailureProbabilityInput);
            Assert.IsNull(kernel.LengthEffectFactorInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        #endregion

        #region Combined Assessment

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResults_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var detailedAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var tailorMadeAssemblyResult = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.CombinedAssessmentFromFailureMechanismSectionResults(simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreEqual(simpleAssemblyResult, kernel.CombinedSimpleAssessmentGroupInput);
            Assert.AreEqual(detailedAssemblyResult, kernel.CombinedDetailedAssessmentGroupInput);
            Assert.AreEqual(tailorMadeAssemblyResult, kernel.CombinedTailorMadeAssessmentGroupInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResults_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var detailedAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var tailorMadeAssemblyResult = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(
                    random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.CombinedAssessmentFromFailureMechanismSectionResults(
                simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyCategoryGroup, result);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResults_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var detailedAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var tailorMadeAssemblyResult = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.CombinedAssessmentFromFailureMechanismSectionResults(simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.CombinedSimpleAssessmentGroupInput);
            Assert.IsNull(kernel.CombinedDetailedAssessmentGroupInput);
            Assert.IsNull(kernel.CombinedTailorMadeAssessmentGroupInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyCategoryGroup);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResultsWithProbabilities_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var detailedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var tailorMadeAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CombinedAssessmentFromFailureMechanismSectionResults(simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreSame(simpleAssemblyResult, kernel.CombinedSimpleAssessmentInput);
            Assert.AreSame(detailedAssemblyResult, kernel.CombinedDetailedAssessmentInput);
            Assert.AreSame(tailorMadeAssemblyResult, kernel.CombinedTailorMadeAssessmentInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResultsWithProbabilities_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var detailedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var tailorMadeAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(
                    new FailureMechanismSectionAssemblyResult(random.NextEnumValue<EFmSectionCategory>(), double.NaN))
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.CombinedAssessmentFromFailureMechanismSectionResults(
                simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResultsWithProbabilities_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var detailedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var tailorMadeAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.CombinedAssessmentFromFailureMechanismSectionResults(simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.CombinedSimpleAssessmentInput);
            Assert.IsNull(kernel.CombinedDetailedAssessmentInput);
            Assert.IsNull(kernel.CombinedTailorMadeAssessmentInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        #endregion
    }
}