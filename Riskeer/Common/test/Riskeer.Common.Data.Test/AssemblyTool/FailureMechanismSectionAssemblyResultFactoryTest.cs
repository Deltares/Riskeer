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
    public class FailureMechanismSectionAssemblyResultFactoryTest
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
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(
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
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(
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
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(
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
            double manualInitialSectionProbability = random.NextDouble();
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
                ManualInitialFailureMechanismResultSectionProbability = manualInitialSectionProbability,
                FurtherAnalysisType = furtherAnalysisType,
                RefinedSectionProbability = refinedSectionProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResultFactory.AssembleSection(
                    sectionResult, assessmentSection, calculateStrategy, false, random.NextDouble());

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalFloodingProbability, calculatorInput.SignalFloodingProbability);
                Assert.AreEqual(failureMechanismContribution.MaximumAllowableFloodingProbability, calculatorInput.MaximumAllowableFloodingProbability);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(expectedHasProbabilitySpecified, calculatorInput.HasProbabilitySpecified);

                double expectedInitialSectionProbability = isInitialFailureMechanismResultTypeAdopt
                                                               ? calculateStrategySectionProbability
                                                               : manualInitialSectionProbability;
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
            double manualInitialProfileProbability = random.NextDouble();
            double manualInitialSectionProbability = random.NextDouble();
            double calculatedStrategySectionProbability = random.NextDouble();
            double calculatedStrategyProfileSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double manualRefinedSectionProbability = random.NextDouble();
            double manualRefinedProfileProbability = random.NextDouble();

            var mocks = new MockRepository();
            var calculateStrategy = mocks.StrictMock<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            bool isInitialFailureMechanismResultTypeAdopt = IsInitialFailureMechanismResultTypeAdopt(initialFailureMechanismResultType);
            if (isInitialFailureMechanismResultTypeAdopt)
            {
                calculateStrategy.Expect(cs => cs.CalculateProfileProbability())
                                 .Return(calculatedStrategyProfileSectionProbability);
                calculateStrategy.Expect(cs => cs.CalculateSectionProbability())
                                 .Return(calculatedStrategySectionProbability);
            }

            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultProfileProbability = manualInitialProfileProbability,
                ManualInitialFailureMechanismResultSectionProbability = manualInitialSectionProbability,
                FurtherAnalysisType = furtherAnalysisType,
                ProbabilityRefinementType = ProbabilityRefinementType.Both,
                RefinedProfileProbability = manualRefinedProfileProbability,
                RefinedSectionProbability = manualRefinedSectionProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResultFactory.AssembleSection(
                    sectionResult, assessmentSection, calculateStrategy, true, random.NextDouble());

                // Assert
                FailureMechanismSectionWithProfileProbabilityAssemblyInput calculatorInput = calculator.FailureMechanismSectionWithProfileProbabilityAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalFloodingProbability, calculatorInput.SignalFloodingProbability);
                Assert.AreEqual(failureMechanismContribution.MaximumAllowableFloodingProbability, calculatorInput.MaximumAllowableFloodingProbability);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(expectedHasProbabilitySpecified, calculatorInput.HasProbabilitySpecified);

                double expectedInitialProfileProbability = isInitialFailureMechanismResultTypeAdopt
                                                               ? calculatedStrategyProfileSectionProbability
                                                               : manualInitialProfileProbability;
                Assert.AreEqual(expectedInitialProfileProbability, calculatorInput.InitialProfileProbability);
                double expectedInitialSectionProbability = isInitialFailureMechanismResultTypeAdopt
                                                               ? calculatedStrategySectionProbability
                                                               : manualInitialSectionProbability;
                Assert.AreEqual(expectedInitialSectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(furtherAnalysisType, calculatorInput.FurtherAnalysisType);
                Assert.AreEqual(manualRefinedProfileProbability, calculatorInput.RefinedProfileProbability);
                Assert.AreEqual(manualRefinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        [TestCase(ProbabilityRefinementType.Profile, 1)]
        [TestCase(ProbabilityRefinementType.Profile, 10)]
        [TestCase(ProbabilityRefinementType.Section, 1)]
        [TestCase(ProbabilityRefinementType.Section, 10)]
        [TestCase(ProbabilityRefinementType.Both, 1)]
        [TestCase(ProbabilityRefinementType.Both, 10)]
        public void AssembleSectionAdoptableWithProfileProbability_WithInputAndUseLengthEffectTrueAndVariousProbabilityRefinementType_SetsInputOnCalculator(
            ProbabilityRefinementType probabilityRefinementType,
            double sectionN)
        {
            // Setup
            var random = new Random(21);
            double refinedSectionProbability = random.NextDouble();
            double refinedProfileProbability = random.NextDouble();

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
                FailureMechanismSectionAssemblyResultFactory.AssembleSection(
                    sectionResult, assessmentSection, calculateStrategy, true, sectionN);

                // Assert
                FailureMechanismSectionWithProfileProbabilityAssemblyInput calculatorInput = calculator.FailureMechanismSectionWithProfileProbabilityAssemblyInput;
                switch (probabilityRefinementType)
                {
                    case ProbabilityRefinementType.Profile:
                        Assert.AreEqual(refinedProfileProbability, calculatorInput.RefinedProfileProbability);
                        Assert.AreEqual(Math.Min(1.0, refinedProfileProbability * sectionN), calculatorInput.RefinedSectionProbability);
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
                FailureMechanismSectionAssemblyResult result = FailureMechanismSectionAssemblyResultFactory.AssembleSection(
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
                void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(
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
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(null, assessmentSection, () => double.NaN);

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
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, null, () => double.NaN);

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
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, null);

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
                FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, () => calculatedSectionProbability);

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalFloodingProbability, calculatorInput.SignalFloodingProbability);
                Assert.AreEqual(failureMechanismContribution.MaximumAllowableFloodingProbability, calculatorInput.MaximumAllowableFloodingProbability);

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
                FailureMechanismSectionAssemblyResult result = FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, () => double.NaN);

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
                void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, () => double.NaN);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region NonAdoptableWithProfileProbability

        [Test]
        public void AssembleSectionNonAdoptableSectionWithProfileProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(21);

            // Call
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(null, assessmentSection, random.NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSectionNonAdoptableSectionWithProfileProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, null, random.NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.Manual, true)]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.NoFailureProbability, false)]
        public void AssembleSectionNonAdoptableWithProfileProbability_WithInputAndUseLengthEffectFalse_SetsInputOnCalculator(
            NonAdoptableInitialFailureMechanismResultType initialFailureMechanismResultType, bool expectedHasProbabilitySpecified)
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double manualInitialSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedSectionProbability = random.NextDouble();

            var assessmentSection = new AssessmentSectionStub();

            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultSectionProbability = manualInitialSectionProbability,
                FurtherAnalysisType = furtherAnalysisType,
                RefinedSectionProbability = refinedSectionProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, false);

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalFloodingProbability, calculatorInput.SignalFloodingProbability);
                Assert.AreEqual(failureMechanismContribution.MaximumAllowableFloodingProbability, calculatorInput.MaximumAllowableFloodingProbability);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(expectedHasProbabilitySpecified, calculatorInput.HasProbabilitySpecified);

                Assert.AreEqual(manualInitialSectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(furtherAnalysisType, calculatorInput.FurtherAnalysisType);
                Assert.AreEqual(refinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.Manual, true)]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.NoFailureProbability, false)]
        public void AssembleSectionNonAdoptableWithProfileProbability_WithInputAndUseLengthEffectTrue_SetsInputOnCalculator(
            NonAdoptableInitialFailureMechanismResultType initialFailureMechanismResultType, bool expectedHasProbabilitySpecified)
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double manualInitialProfileProbability = random.NextDouble();
            double manualInitialSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double manualRefinedSectionProbability = random.NextDouble();
            double manualRefinedProfileProbability = random.NextDouble();

            var assessmentSection = new AssessmentSectionStub();

            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultProfileProbability = manualInitialProfileProbability,
                ManualInitialFailureMechanismResultSectionProbability = manualInitialSectionProbability,
                FurtherAnalysisType = furtherAnalysisType,
                RefinedProfileProbability = manualRefinedProfileProbability,
                RefinedSectionProbability = manualRefinedSectionProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, true);

                // Assert
                FailureMechanismSectionWithProfileProbabilityAssemblyInput calculatorInput = calculator.FailureMechanismSectionWithProfileProbabilityAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalFloodingProbability, calculatorInput.SignalFloodingProbability);
                Assert.AreEqual(failureMechanismContribution.MaximumAllowableFloodingProbability, calculatorInput.MaximumAllowableFloodingProbability);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(expectedHasProbabilitySpecified, calculatorInput.HasProbabilitySpecified);
                Assert.AreEqual(manualInitialProfileProbability, calculatorInput.InitialProfileProbability);
                Assert.AreEqual(manualInitialSectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(furtherAnalysisType, calculatorInput.FurtherAnalysisType);
                Assert.AreEqual(manualRefinedProfileProbability, calculatorInput.RefinedProfileProbability);
                Assert.AreEqual(manualRefinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        public void AssembleSectionNonAdoptableSectionWithProfileProbability_CalculatorRan_ReturnsExpectedOutput()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResult result = FailureMechanismSectionAssemblyResultFactory.AssembleSection(
                    sectionResult, assessmentSection, random.NextBoolean());

                // Assert
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput, result);
            }
        }

        [Test]
        public void AssembleSectionNonAdoptableSectionWithProfileProbability_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(
                    sectionResult, assessmentSection, random.NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region NonAdoptableWithoutProfileProbability

        [Test]
        public void AssembleSectionNonAdoptableSectionWithoutProfileProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSectionNonAdoptableSectionWithoutProfileProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
        
        [Test]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.Manual, true)]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.NoFailureProbability, false)]
        public void AssembleSectionNonAdoptableWithoutProfileProbability_WithInputAndUseLengthEffectFalse_SetsInputOnCalculator(
            NonAdoptableInitialFailureMechanismResultType initialFailureMechanismResultType, bool expectedHasProbabilitySpecified)
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double manualInitialSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedSectionProbability = random.NextDouble();

            var assessmentSection = new AssessmentSectionStub();

            var sectionResult = new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = isRelevant,
                InitialFailureMechanismResultType = initialFailureMechanismResultType,
                ManualInitialFailureMechanismResultSectionProbability = manualInitialSectionProbability,
                FurtherAnalysisType = furtherAnalysisType,
                RefinedSectionProbability = refinedSectionProbability
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalFloodingProbability, calculatorInput.SignalFloodingProbability);
                Assert.AreEqual(failureMechanismContribution.MaximumAllowableFloodingProbability, calculatorInput.MaximumAllowableFloodingProbability);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(expectedHasProbabilitySpecified, calculatorInput.HasProbabilitySpecified);

                Assert.AreEqual(manualInitialSectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(furtherAnalysisType, calculatorInput.FurtherAnalysisType);
                Assert.AreEqual(refinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        public void AssembleSectionNonAdoptableSectionWithoutProfileProbability_CalculatorRan_ReturnsExpectedOutput()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResult result = FailureMechanismSectionAssemblyResultFactory.AssembleSection(
                    sectionResult, assessmentSection);

                // Assert
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput, result);
            }
        }

        [Test]
        public void AssembleSectionNonAdoptableSectionWithoutProfileProbability_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(
                    sectionResult, assessmentSection);

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