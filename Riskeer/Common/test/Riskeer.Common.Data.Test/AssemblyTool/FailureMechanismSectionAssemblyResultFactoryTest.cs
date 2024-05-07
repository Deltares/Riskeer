// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

        #region AdoptableFailureMechanismSectionResult

        [Test]
        public void AssembleSectionAdoptableSection_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(null, assessmentSection, calculateStrategy);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSectionAdoptableSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            mocks.ReplayAll();

            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, null, calculateStrategy);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSectionAdoptableSection_CalculateProbabilityStrategyNull_ThrowsArgumentNullException()
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
            Assert.AreEqual("calculateProbabilityStrategy", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(AdoptableInitialFailureMechanismResultType.Adopt, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.Manual, true)]
        [TestCase(AdoptableInitialFailureMechanismResultType.NoFailureProbability, false)]
        public void AssembleSectionAdoptableSection_WithInput_ReturnsExpectedOutput(
            AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType, bool expectedHasProbabilitySpecified)
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double manualSectionProbability = random.NextDouble();
            double calculatedSectionProbability = random.NextDouble();
            var furtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>();
            double refinedSectionProbability = random.NextDouble();

            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            calculateStrategy.Stub(c => c.CalculateSectionProbability()).Return(calculatedSectionProbability);
            mocks.ReplayAll();

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
                FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, calculateStrategy);

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
            
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSectionAdoptableSection_CalculatorRan_ReturnsExpectedOutput()
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResultWrapper result = FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, calculateStrategy);

                // Assert
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput, result);
            }
            
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSectionAdoptableSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculateStrategy = mocks.Stub<IFailureMechanismSectionResultCalculateProbabilityStrategy>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, calculateStrategy);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
            
            mocks.VerifyAll();
        }

        #endregion

        #region NonAdoptableFailureMechanismSectionResult

        [Test]
        public void AssembleSectionNonAdoptableSection_SectionResultNull_ThrowsArgumentNullException()
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
        public void AssembleSectionNonAdoptableSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResult = new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            void Call() => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.Manual, true)]
        [TestCase(NonAdoptableInitialFailureMechanismResultType.NoFailureProbability, false)]
        public void AssembleSectionNonAdoptable_WithInputAndUseLengthEffectFalse_SetsInputOnCalculator(
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
        public void AssembleSectionNonAdoptableSection_CalculatorRan_ReturnsExpectedOutput()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var sectionResult = new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResultWrapper result = FailureMechanismSectionAssemblyResultFactory.AssembleSection(
                    sectionResult, assessmentSection);

                // Assert
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput, result);
            }
        }

        [Test]
        public void AssembleSectionNonAdoptableSection_CalculatorThrowsException_ThrowsAssemblyException()
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