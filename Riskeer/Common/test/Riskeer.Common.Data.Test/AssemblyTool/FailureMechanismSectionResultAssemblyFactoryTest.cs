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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class FailureMechanismSectionResultAssemblyFactoryTest
    {
        private static bool IsInitialFailureMechanismResultTypeAdopt(AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType)
        {
            return initialFailureMechanismResultType == AdoptableInitialFailureMechanismResultType.Adopt;
        }

        #region AdoptableWithProfileProbabilityFailureMechanismSectionResult

        [Test]
        public void AssembleSectionAdoptableSectionWithProfileProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            mocks.ReplayAll();

            // Call
            void Call() => FailureMechanismSectionResultAssemblyFactory.AssembleSection(
                null, assessmentSection, calculateStrategy, random.NextBoolean(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSectionAdoptableSectionWithProfileProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            mocks.ReplayAll();

            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => FailureMechanismSectionResultAssemblyFactory.AssembleSection(
                sectionResult, null, calculateStrategy, random.NextBoolean(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSectionAdoptableSectionWithProfileProbability_CalculateProbabilityStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => FailureMechanismSectionResultAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection, null, random.NextBoolean(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculateProbabilityStrategy", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AdoptableInitialFailureMechanismResultType.Adopt, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.Manual, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.NoFailureProbability, false)]
        public void AssembleSectionAdoptableWithProfileProbability_WithInputAndUseLengthEffectFalse_SetsInputOnCalculator(
            AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType, bool expectedHasProbabilitySpecified)
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double manualSectionProbability = random.NextDouble();
            double calculateStrategySectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedSectionProbability = random.NextDouble();

            var mocks = new MockRepository();
            var calculateStrategy = mocks.StrictMock<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            bool isInitialFailureMechanismResultTypeAdopt = IsInitialFailureMechanismResultTypeAdopt(initialFailureMechanismResultType);
            if (isInitialFailureMechanismResultTypeAdopt)
            {
                calculateStrategy.Expect(cs => cs.CalculateSectionProbability())
                                 .Return(calculateStrategySectionProbability);
            }

            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultSectionProbability = manualSectionProbability,
                FurtherAnalysisType = furtherAnalysisType,
                RefinedSectionProbability = refinedSectionProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionResultAssemblyFactory.AssembleSection(
                    sectionResult, assessmentSection, calculateStrategy, false, random.NextDouble());

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalingNorm, calculatorInput.SignalingNorm);
                Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, calculatorInput.LowerLimitNorm);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(expectedHasProbabilitySpecified, calculatorInput.HasProbabilitySpecified);

                double expectedInitialSectionProbability = isInitialFailureMechanismResultTypeAdopt
                                                               ? calculateStrategySectionProbability
                                                               : manualSectionProbability;
                Assert.AreEqual(expectedInitialSectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(furtherAnalysisType, calculatorInput.FurtherAnalysisType);
                Assert.AreEqual(refinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        [TestCase(AdoptableInitialFailureMechanismResultType.Adopt, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.Manual, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.NoFailureProbability, false)]
        public void AssembleSectionAdoptableWithProfileProbability_WithInputAndUseLengthEffectTrue_SetsInputOnCalculator(
            AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType, bool expectedHasProbabilitySpecified)
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double manualProfileProbability = random.NextDouble();
            double manualSectionProbability = random.NextDouble();
            double calculateStrategySectionProbability = random.NextDouble();
            double calculateStrategyProfileSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedSectionProbability = random.NextDouble();
            double refinedProfileProbability = random.NextDouble();

            var mocks = new MockRepository();
            var calculateStrategy = mocks.StrictMock<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            bool isInitialFailureMechanismResultTypeAdopt = IsInitialFailureMechanismResultTypeAdopt(initialFailureMechanismResultType);
            if (isInitialFailureMechanismResultTypeAdopt)
            {
                calculateStrategy.Expect(cs => cs.CalculateProfileProbability())
                                 .Return(calculateStrategyProfileSectionProbability);
                calculateStrategy.Expect(cs => cs.CalculateSectionProbability())
                                 .Return(calculateStrategySectionProbability);
            }

            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultProfileProbability = manualProfileProbability,
                ManualInitialFailureMechanismResultSectionProbability = manualSectionProbability,
                FurtherAnalysisType = furtherAnalysisType,
                ProbabilityRefinementType = ProbabilityRefinementType.Both,
                RefinedProfileProbability = refinedProfileProbability,
                RefinedSectionProbability = refinedSectionProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionResultAssemblyFactory.AssembleSection(
                    sectionResult, assessmentSection, calculateStrategy, true, random.NextDouble());

                // Assert
                FailureMechanismSectionWithProfileProbabilityAssemblyInput calculatorInput = calculator.FailureMechanismSectionWithProfileProbabilityAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalingNorm, calculatorInput.SignalingNorm);
                Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, calculatorInput.LowerLimitNorm);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(expectedHasProbabilitySpecified, calculatorInput.HasProbabilitySpecified);

                double expectedInitialProfileProbability = isInitialFailureMechanismResultTypeAdopt
                                                               ? calculateStrategyProfileSectionProbability
                                                               : manualProfileProbability;
                Assert.AreEqual(expectedInitialProfileProbability, calculatorInput.InitialProfileProbability);
                double expectedInitialSectionProbability = isInitialFailureMechanismResultTypeAdopt
                                                               ? calculateStrategySectionProbability
                                                               : manualSectionProbability;
                Assert.AreEqual(expectedInitialSectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(furtherAnalysisType, calculatorInput.FurtherAnalysisType);
                Assert.AreEqual(refinedProfileProbability, calculatorInput.RefinedProfileProbability);
                Assert.AreEqual(refinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        [TestCase(ProbabilityRefinementType.Profile)]
        [TestCase(ProbabilityRefinementType.Section)]
        [TestCase(ProbabilityRefinementType.Both)]
        public void AssembleSectionAdoptableWithProfileProbability_WithInputAndUseLengthEffectTrueAndVariousProbabilityRefinementType_SetsInputOnCalculator(
            ProbabilityRefinementType probabilityRefinementType)
        {
            // Setup
            var random = new Random(21);
            double refinedSectionProbability = random.NextDouble();
            double refinedProfileProbability = random.NextDouble();
            double sectionN = random.NextDouble();

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                ProbabilityRefinementType = probabilityRefinementType,
                RefinedProfileProbability = refinedProfileProbability,
                RefinedSectionProbability = refinedSectionProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionResultAssemblyFactory.AssembleSection(
                    sectionResult, assessmentSection, calculateStrategy, true, sectionN);

                // Assert
                FailureMechanismSectionWithProfileProbabilityAssemblyInput calculatorInput = calculator.FailureMechanismSectionWithProfileProbabilityAssemblyInput;
                switch (probabilityRefinementType)
                {
                    case ProbabilityRefinementType.Profile:
                        Assert.AreEqual(refinedProfileProbability, calculatorInput.RefinedProfileProbability);
                        Assert.AreEqual(refinedProfileProbability * sectionN, calculatorInput.RefinedSectionProbability);
                        break;
                    case ProbabilityRefinementType.Section:
                        Assert.AreEqual(refinedSectionProbability / sectionN, calculatorInput.RefinedProfileProbability);
                        Assert.AreEqual(refinedSectionProbability, calculatorInput.RefinedSectionProbability);
                        break;
                    case ProbabilityRefinementType.Both:
                        Assert.AreEqual(refinedProfileProbability, calculatorInput.RefinedProfileProbability);
                        Assert.AreEqual(refinedSectionProbability, calculatorInput.RefinedSectionProbability);
                        break;
                }
            }
        }

        [Test]
        public void AssembleSectionAdoptableSectionWithProfileProbability_CalculatorRan_ReturnsExpectedOutput()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResult result = FailureMechanismSectionResultAssemblyFactory.AssembleSection(
                    sectionResult, assessmentSection, calculateStrategy, random.NextBoolean(), random.NextDouble());

                // Assert
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput, result);
            }
        }

        [Test]
        public void AssembleSectionAdoptableSectionWithProfileProbability_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => FailureMechanismSectionResultAssemblyFactory.AssembleSection(
                    sectionResult, assessmentSection, calculateStrategy, random.NextBoolean(), random.NextDouble());

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region AdoptableFailureMechanismSectionResult

        [Test]
        public void AssembleSectionAdoptableSectionWithoutProfileProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => FailureMechanismSectionResultAssemblyFactory.AssembleSection(null, assessmentSection, () => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSectionAdoptableSectionWithoutProfileProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => FailureMechanismSectionResultAssemblyFactory.AssembleSection(sectionResult, null, () => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleSectionAdoptableSectionWithoutProfileProbability_CalculateProbabilityFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => FailureMechanismSectionResultAssemblyFactory.AssembleSection(sectionResult, assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculateProbabilityFunc", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AdoptableInitialFailureMechanismResultType.Adopt, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.Manual, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.NoFailureProbability, false)]
        public void AssembleSectionAdoptableSectionWithoutProfileProbability_WithInput_ReturnsExpectedOutput(
            AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType, bool expectedHasProbabilitySpecified)
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double manualSectionProbability = random.NextDouble();
            double calculatedSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedSectionProbability = random.NextDouble();

            var assessmentSection = new AssessmentSectionStub();

            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultSectionProbability = manualSectionProbability,
                FurtherAnalysisType = furtherAnalysisType,
                RefinedSectionProbability = refinedSectionProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionResultAssemblyFactory.AssembleSection(sectionResult, assessmentSection, () => calculatedSectionProbability);

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalingNorm, calculatorInput.SignalingNorm);
                Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, calculatorInput.LowerLimitNorm);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(expectedHasProbabilitySpecified, calculatorInput.HasProbabilitySpecified);

                double expectedInitialSectionProbability = IsInitialFailureMechanismResultTypeAdopt(initialFailureMechanismResultType)
                                                               ? calculatedSectionProbability
                                                               : manualSectionProbability;
                Assert.AreEqual(expectedInitialSectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(furtherAnalysisType, calculatorInput.FurtherAnalysisType);
                Assert.AreEqual(refinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        public void AssembleSectionAdoptableSectionWithoutProfileProbability_CalculatorRan_ReturnsExpectedOutput()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResult result = FailureMechanismSectionResultAssemblyFactory.AssembleSection(sectionResult, assessmentSection, () => double.NaN);

                // Assert
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput, result);
            }
        }

        [Test]
        public void AssembleSectionAdoptableSectionWithoutProfileProbability_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => FailureMechanismSectionResultAssemblyFactory.AssembleSection(sectionResult, assessmentSection, () => double.NaN);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion
    }
}