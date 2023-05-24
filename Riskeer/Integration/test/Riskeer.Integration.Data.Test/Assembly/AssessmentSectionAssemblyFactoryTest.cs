// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class AssessmentSectionAssemblyFactoryTest
    {
        #region Assemble Assessment Section

        [Test]
        public void AssembleAssessmentSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleAssessmentSection_AssessmentSectionContainingFailureMechanismsWithRandomInAssemblyState_SetsInputOnCalculator()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithRandomInAssemblyState();

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
            }
        }

        [Test]
        public void AssembleAssessmentSection_AssemblyRan_ReturnsOutput()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithInAssemblyTrue();

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
        public void AssembleAssessmentSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(CreateAssessmentSectionContainingFailureMechanismsWithInAssemblyTrue());

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_FailureMechanismCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(CreateAssessmentSectionContainingFailureMechanismsWithInAssemblyTrue());

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
            assessmentSection.GetFailureMechanisms()
                             .Concat(assessmentSection.SpecificFailureMechanisms)
                             .ForEachElementDo(
                                 fm => fm.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections);

            return assessmentSection;
        }

        private static AssessmentSection CreateAssessmentSectionContainingFailureMechanismsWithRandomInAssemblyState()
        {
            var random = new Random(21);

            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailureMechanismsWithInAssemblyTrue();

            IEnumerable<IFailureMechanism> getAllFailureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                      .Concat(assessmentSection.SpecificFailureMechanisms);
            getAllFailureMechanisms.ForEachElementDo(fp => fp.InAssembly = random.NextBoolean());
            getAllFailureMechanisms.ForEachElementDo(
                fm => fm.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections);

            return assessmentSection;
        }

        #endregion
    }
}