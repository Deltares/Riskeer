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
using Riskeer.Common.Data.FailurePath;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.TestUtil;
using Riskeer.Integration.TestUtil;

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
        public void AssembleAssessmentSection_WithAssessmentSectionContainingFailurePathsWithRandomInAssemblyState_SetsInputOnCalculator()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionContainingFailurePathsWithRandomInAssemblyState();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

                // Assert
                FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(contribution.SignalingNorm, assessmentSectionAssemblyCalculator.SignalingNormInput);
                Assert.AreEqual(contribution.LowerLimitNorm, assessmentSectionAssemblyCalculator.LowerLimitNormInput);

                int expectedNrOfProbabilities = assessmentSection.GetFailureMechanisms()
                                                                 .Cast<IFailurePath>()
                                                                 .Concat(assessmentSection.SpecificFailurePaths)
                                                                 .Count(fp => fp.InAssembly);
                IEnumerable<double> calculatorInput = assessmentSectionAssemblyCalculator.FailureMechanismProbabilitiesInput;
                Assert.AreEqual(expectedNrOfProbabilities, calculatorInput.Count());
                foreach (double failureMechanismProbability in calculatorInput)
                {
                    Assert.AreEqual(failureMechanismAssemblyCalculator.AssemblyResult, failureMechanismProbability);
                }
            }
        }

        [Test]
        public void AssembleAssessmentSection_AssemblyRan_ReturnsOutput()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;

                // Call
                AssessmentSectionAssemblyResult result = AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);

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
                void Call() => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(CreateAssessmentSection());

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
                void Call() => AssessmentSectionAssemblyFactory.AssembleAssessmentSection(CreateAssessmentSection());

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere toetssporen kan geen oordeel worden bepaald.", exception.Message);
            }
        }

        #endregion

        #region Assemble Combined Per Failure Mechanism Section

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssembleCombinedPerFailureMechanismSection_WithAssessmentSection_SetsInputOnCalculator(bool failureMechanismSectionAssemblyThrowsException)
        {
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                failureMechanismSectionAssemblyCalculator.ThrowExceptionOnCalculate = failureMechanismSectionAssemblyThrowsException;

                // Call
                AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection);

                // Assert
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                IEnumerable<CombinedAssemblyFailureMechanismSection>[] actualInput = assessmentSectionAssemblyCalculator.CombinedFailureMechanismSectionsInput.ToArray();
                IEnumerable<CombinedAssemblyFailureMechanismSection>[] expectedInput = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, assessmentSection.GetFailureMechanisms()).ToArray();
                Assert.AreEqual(expectedInput.Length, actualInput.Length);

                for (var i = 0; i < expectedInput.Length; i++)
                {
                    CombinedAssemblyFailureMechanismSection[] actualSections = actualInput[i].ToArray();
                    CombinedAssemblyFailureMechanismSection[] expectedSections = expectedInput[i].ToArray();
                    Assert.AreEqual(expectedSections.Length, actualSections.Length);

                    for (var j = 0; j < expectedSections.Length; j++)
                    {
                        Assert.AreEqual(expectedSections[j].SectionStart, actualSections[j].SectionStart);
                        Assert.AreEqual(expectedSections[j].SectionEnd, actualSections[j].SectionEnd);
                        Assert.AreEqual(expectedSections[j].FailureMechanismSectionAssemblyGroup, actualSections[j].FailureMechanismSectionAssemblyGroup);
                    }
                }
            }
        }

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_AssemblyRan_ReturnsOutput()
        {
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.CombinedFailureMechanismSectionAssemblyOutput = new[]
                {
                    CombinedFailureMechanismSectionAssemblyTestFactory.Create(assessmentSection, 20),
                    CombinedFailureMechanismSectionAssemblyTestFactory.Create(assessmentSection, 21)
                };

                // Call
                CombinedFailureMechanismSectionAssemblyResult[] output = AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(
                                                                                                             assessmentSection)
                                                                                                         .ToArray();

                // Assert
                Dictionary<IFailureMechanism, int> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                        .Where(fm => fm.InAssembly)
                                                                                        .Select((fm, i) => new
                                                                                        {
                                                                                            FailureMechanism = fm,
                                                                                            Index = i
                                                                                        })
                                                                                        .ToDictionary(x => x.FailureMechanism, x => x.Index);
                CombinedFailureMechanismSectionAssemblyResult[] expectedOutput = CombinedFailureMechanismSectionAssemblyResultFactory.Create(
                    calculator.CombinedFailureMechanismSectionAssemblyOutput, failureMechanisms, assessmentSection).ToArray();

                Assert.AreEqual(expectedOutput.Length, output.Length);
                for (var i = 0; i < expectedOutput.Length; i++)
                {
                    Assert.AreEqual(expectedOutput[i].SectionStart, output[i].SectionStart);
                    Assert.AreEqual(expectedOutput[i].SectionEnd, output[i].SectionEnd);
                    Assert.AreEqual(expectedOutput[i].TotalResult, output[i].TotalResult);
                    Assert.AreEqual(expectedOutput[i].Piping, output[i].Piping);
                    Assert.AreEqual(expectedOutput[i].GrassCoverErosionInwards, output[i].GrassCoverErosionInwards);
                    Assert.AreEqual(expectedOutput[i].MacroStabilityInwards, output[i].MacroStabilityInwards);
                    Assert.AreEqual(expectedOutput[i].Microstability, output[i].Microstability);
                    Assert.AreEqual(expectedOutput[i].StabilityStoneCover, output[i].StabilityStoneCover);
                    Assert.AreEqual(expectedOutput[i].WaveImpactAsphaltCover, output[i].WaveImpactAsphaltCover);
                    Assert.AreEqual(expectedOutput[i].WaterPressureAsphaltCover, output[i].WaterPressureAsphaltCover);
                    Assert.AreEqual(expectedOutput[i].GrassCoverErosionOutwards, output[i].GrassCoverErosionOutwards);
                    Assert.AreEqual(expectedOutput[i].GrassCoverSlipOffOutwards, output[i].GrassCoverSlipOffOutwards);
                    Assert.AreEqual(expectedOutput[i].GrassCoverSlipOffInwards, output[i].GrassCoverSlipOffInwards);
                    Assert.AreEqual(expectedOutput[i].HeightStructures, output[i].HeightStructures);
                    Assert.AreEqual(expectedOutput[i].ClosingStructures, output[i].ClosingStructures);
                    Assert.AreEqual(expectedOutput[i].PipingStructure, output[i].PipingStructure);
                    Assert.AreEqual(expectedOutput[i].StabilityPointStructures, output[i].StabilityPointStructures);
                    Assert.AreEqual(expectedOutput[i].DuneErosion, output[i].DuneErosion);
                }
            }
        }

        [Test]
        public void AssembleCombinedPerFailureMechanismSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssessmentSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Helpers

        private static AssessmentSection CreateAssessmentSection()
        {
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            IEnumerable<SpecificFailurePath> failurePaths = Enumerable.Repeat(new SpecificFailurePath(), random.Next(1, 10))
                                                                      .ToArray();
            assessmentSection.SpecificFailurePaths.AddRange(failurePaths);
            return assessmentSection;
        }

        private static AssessmentSection CreateAssessmentSectionContainingFailurePathsWithRandomInAssemblyState()
        {
            var random = new Random(21);

            AssessmentSection assessmentSection = CreateAssessmentSection();

            IEnumerable<IFailurePath> failurePaths = assessmentSection.GetFailureMechanisms();
            failurePaths = failurePaths.Concat(assessmentSection.SpecificFailurePaths);
            failurePaths.ForEachElementDo(fp => fp.InAssembly = random.NextBoolean());

            return assessmentSection;
        }

        #endregion
    }
}