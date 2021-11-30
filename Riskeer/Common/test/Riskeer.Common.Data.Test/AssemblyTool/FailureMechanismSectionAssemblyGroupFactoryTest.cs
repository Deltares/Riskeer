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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupFactoryTest
    {
        [Test]
        public void AssembleSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => FailureMechanismSectionAssemblyGroupFactory.AssembleSection(null,
                                                                                       random.NextBoolean(),
                                                                                       random.NextDouble(), random.NextDouble(),
                                                                                       random.NextBoolean(),
                                                                                       random.NextDouble(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleSection_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double profileProbability = random.NextDouble();
            double sectionProbability = random.NextDouble();
            bool furtherAnalysisNeeded = random.NextBoolean();
            double refinedProfileProbability = random.NextDouble();
            double refinedSectionProbability = random.NextDouble();

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyGroupFactory.AssembleSection(assessmentSection,
                                                                            isRelevant,
                                                                            profileProbability, sectionProbability,
                                                                            furtherAnalysisNeeded,
                                                                            refinedProfileProbability, refinedSectionProbability);

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalingNorm, calculatorInput.SignalingNorm);
                Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, calculatorInput.LowerLimitNorm);

                Assert.AreEqual(isRelevant, calculatorInput.IsRelevant);
                Assert.AreEqual(profileProbability, calculatorInput.InitialProfileProbability);
                Assert.AreEqual(sectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(furtherAnalysisNeeded, calculatorInput.FurtherAnalysisNeeded);
                Assert.AreEqual(refinedProfileProbability, calculatorInput.RefinedProfileProbability);
                Assert.AreEqual(refinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        public void AssembleSection_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(21);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResult output =
                    FailureMechanismSectionAssemblyGroupFactory.AssembleSection(new AssessmentSectionStub(),
                                                                                random.NextBoolean(),
                                                                                random.NextDouble(), random.NextDouble(),
                                                                                random.NextBoolean(),
                                                                                random.NextDouble(), random.NextDouble());

                // Assert
                FailureMechanismSectionAssemblyResult calculatorOutput = calculator.FailureMechanismSectionAssemblyResultOutput;
                Assert.AreEqual(calculatorOutput.N, output.N);
                Assert.AreEqual(calculatorOutput.AssemblyGroup, output.AssemblyGroup);
                Assert.AreEqual(calculatorOutput.ProfileProbability, output.ProfileProbability);
                Assert.AreEqual(calculatorOutput.SectionProbability, output.SectionProbability);
            }
        }

        [Test]
        public void AssembleSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => FailureMechanismSectionAssemblyGroupFactory.AssembleSection(new AssessmentSectionStub(),
                                                                                           random.NextBoolean(),
                                                                                           random.NextDouble(), random.NextDouble(),
                                                                                           random.NextBoolean(),
                                                                                           random.NextDouble(), random.NextDouble());

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }
    }
}