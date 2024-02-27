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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class AssessmentSectionAssemblyFactoryTest
    {
        [Test]
        public void AssembleAssessmentSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        #region Assemble Assessment Section

        [Test]
        public void AssembleAssessmentSection_AssessmentSectionWithFailureMechanismsCorrelatedFalseAndContainingFailureMechanismsWithRandomInAssemblyState_SetsInputOnCalculator()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithRandomInAssemblyState();
            assessmentSection.AreFailureMechanismsCorrelated = false;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(contribution.SignalFloodingProbability, assessmentSectionAssemblyCalculator.SignalFloodingProbability);
                Assert.AreEqual(contribution.MaximumAllowableFloodingProbability, assessmentSectionAssemblyCalculator.MaximumAllowableFloodingProbabilityInput);

                int expectedNrOfProbabilities = assessmentSection.GetFailureMechanisms()
                                                                 .Concat(assessmentSection.SpecificFailureMechanisms)
                                                                 .Count(fp => fp.InAssembly);
                IEnumerable<double> calculatorInput = assessmentSectionAssemblyCalculator.FailureMechanismProbabilitiesInput;
                Assert.AreEqual(expectedNrOfProbabilities, calculatorInput.Count());
                foreach (double failureMechanismProbability in calculatorInput)
                {
                    Assert.AreEqual(failureMechanismAssemblyCalculator.AssemblyResultOutput.AssemblyResult, failureMechanismProbability);
                }

                Assert.IsNull(assessmentSectionAssemblyCalculator.CorrelatedFailureMechanismProbabilitiesInput);
                Assert.IsNull(assessmentSectionAssemblyCalculator.UncorrelatedFailureMechanismProbabilitiesInput);
            }
        }

        [Test]
        public void AssembleAssessmentSection_AssessmentSectionWithCorrelatedFailureMechanismsTrueAndAllCorrelatedFailureMechanismsInAssembly_SetsInputOnCalculator()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithRandomInAssemblyState();
            assessmentSection.GrassCoverErosionInwards.InAssembly = true;
            assessmentSection.HeightStructures.InAssembly = true;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(contribution.SignalFloodingProbability, assessmentSectionAssemblyCalculator.SignalFloodingProbability);
                Assert.AreEqual(contribution.MaximumAllowableFloodingProbability, assessmentSectionAssemblyCalculator.MaximumAllowableFloodingProbabilityInput);

                const int expectedNrOfCorrelatedProbabilities = 2;
                IEnumerable<double> correlatedFailureMechanismProbabilitiesInput = assessmentSectionAssemblyCalculator.CorrelatedFailureMechanismProbabilitiesInput;
                Assert.AreEqual(expectedNrOfCorrelatedProbabilities, correlatedFailureMechanismProbabilitiesInput.Count());
                foreach (double failureMechanismProbability in correlatedFailureMechanismProbabilitiesInput)
                {
                    Assert.AreEqual(failureMechanismAssemblyCalculator.AssemblyResultOutput.AssemblyResult, failureMechanismProbability);
                }

                int expectedNrOfUncorrelatedProbabilities = assessmentSection.GetFailureMechanisms()
                                                                             .Concat(assessmentSection.SpecificFailureMechanisms)
                                                                             .Count(fp => fp != assessmentSection.HeightStructures && fp != assessmentSection.GrassCoverErosionInwards && fp.InAssembly);
                IEnumerable<double> uncorrelatedFailureMechanismProbabilitiesInput = assessmentSectionAssemblyCalculator.UncorrelatedFailureMechanismProbabilitiesInput;
                Assert.AreEqual(expectedNrOfUncorrelatedProbabilities, uncorrelatedFailureMechanismProbabilitiesInput.Count());
                foreach (double failureMechanismProbability in uncorrelatedFailureMechanismProbabilitiesInput)
                {
                    Assert.AreEqual(failureMechanismAssemblyCalculator.AssemblyResultOutput.AssemblyResult, failureMechanismProbability);
                }

                Assert.IsNull(assessmentSectionAssemblyCalculator.FailureMechanismProbabilitiesInput);
            }
        }
        
        [Test]
        public void AssembleAssessmentSection_AssessmentSectionWithCorrelatedFailureMechanismsFalseAndAllCorrelatedFailureMechanismsInAssembly_SetsInputOnCalculator()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithRandomInAssemblyState();
            assessmentSection.AreFailureMechanismsCorrelated = false;
            assessmentSection.GrassCoverErosionInwards.InAssembly = true;
            assessmentSection.HeightStructures.InAssembly = true;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(contribution.SignalFloodingProbability, assessmentSectionAssemblyCalculator.SignalFloodingProbability);
                Assert.AreEqual(contribution.MaximumAllowableFloodingProbability, assessmentSectionAssemblyCalculator.MaximumAllowableFloodingProbabilityInput);

                int expectedNrOfProbabilities = assessmentSection.GetFailureMechanisms()
                                                                 .Concat(assessmentSection.SpecificFailureMechanisms)
                                                                 .Count(fp => fp.InAssembly);
                IEnumerable<double> calculatorInput = assessmentSectionAssemblyCalculator.FailureMechanismProbabilitiesInput;
                Assert.AreEqual(expectedNrOfProbabilities, calculatorInput.Count());
                foreach (double failureMechanismProbability in calculatorInput)
                {
                    Assert.AreEqual(failureMechanismAssemblyCalculator.AssemblyResultOutput.AssemblyResult, failureMechanismProbability);
                }

                Assert.IsNull(assessmentSectionAssemblyCalculator.CorrelatedFailureMechanismProbabilitiesInput);
                Assert.IsNull(assessmentSectionAssemblyCalculator.UncorrelatedFailureMechanismProbabilitiesInput);
            }
        }
        
        [Test]
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void AssembleAssessmentSection_AssessmentSectionWithCorrelatedFailureMechanismsTrueAndVariousCorrelatedFailureMechanismsNotInAssembly_SetsInputOnCalculator(
            bool grassCoverErosionInwardsInAssembly, bool heightStructuresInAssembly)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithRandomInAssemblyState();
            assessmentSection.GrassCoverErosionInwards.InAssembly = grassCoverErosionInwardsInAssembly;
            assessmentSection.HeightStructures.InAssembly = heightStructuresInAssembly;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(contribution.SignalFloodingProbability, assessmentSectionAssemblyCalculator.SignalFloodingProbability);
                Assert.AreEqual(contribution.MaximumAllowableFloodingProbability, assessmentSectionAssemblyCalculator.MaximumAllowableFloodingProbabilityInput);

                int expectedNrOfProbabilities = assessmentSection.GetFailureMechanisms()
                                                                 .Concat(assessmentSection.SpecificFailureMechanisms)
                                                                 .Count(fp => fp.InAssembly);
                IEnumerable<double> calculatorInput = assessmentSectionAssemblyCalculator.FailureMechanismProbabilitiesInput;
                Assert.AreEqual(expectedNrOfProbabilities, calculatorInput.Count());
                foreach (double failureMechanismProbability in calculatorInput)
                {
                    Assert.AreEqual(failureMechanismAssemblyCalculator.AssemblyResultOutput.AssemblyResult, failureMechanismProbability);
                }

                Assert.IsNull(assessmentSectionAssemblyCalculator.CorrelatedFailureMechanismProbabilitiesInput);
                Assert.IsNull(assessmentSectionAssemblyCalculator.UncorrelatedFailureMechanismProbabilitiesInput);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssembleAssessmentSection_AssemblyRan_ReturnsOutput(bool areFailureMechanismsCorrelated)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithInAssemblyTrue();
            assessmentSection.AreFailureMechanismsCorrelated = areFailureMechanismsCorrelated;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyResultWrapper result = AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                Assert.AreSame(assessmentSectionAssemblyCalculator.AssessmentSectionAssemblyResult, result);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssembleAssessmentSection_CalculatorThrowsException_ThrowsAssemblyException(bool areFailureMechanismsCorrelated)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithInAssemblyTrue();
            assessmentSection.AreFailureMechanismsCorrelated = areFailureMechanismsCorrelated;
            
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssembleAssessmentSection_FailureMechanismCalculatorThrowsException_ThrowsAssemblyException(bool areFailureMechanismsCorrelated)
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithInAssemblyTrue();
            assessmentSection.AreFailureMechanismsCorrelated = areFailureMechanismsCorrelated;
            
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere faalmechanismen kan geen assemblageresultaat worden bepaald.", exception.Message);
            }
        }

        #endregion

        #region Helpers

        private static AssessmentSection CreateAssessmentSectionContainingFailureMechanismsWithInAssemblyTrue()
        {
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            IEnumerable<SpecificFailureMechanism> failureMechanisms = Enumerable.Repeat(new SpecificFailureMechanism(), random.Next(1, 10))
                                                                                .ToArray();
            assessmentSection.SpecificFailureMechanisms.AddRange(failureMechanisms);
            AssessmentSectionTestHelper.GetAllFailureMechanisms(assessmentSection).ForEachElementDo(
                fm => fm.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P1);

            return assessmentSection;
        }

        private static AssessmentSection CreateAssessmentSectionContainingFailureMechanismsWithRandomInAssemblyState()
        {
            var random = new Random(21);

            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithInAssemblyTrue();

            IEnumerable<IFailureMechanism> allFailureMechanisms = AssessmentSectionTestHelper.GetAllFailureMechanisms(assessmentSection);
            allFailureMechanisms.ForEachElementDo(fp => fp.InAssembly = random.NextBoolean());
            allFailureMechanisms.ForEachElementDo(
                fm => fm.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P1);

            return assessmentSection;
        }

        #endregion
    }
}