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
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.TestUtil;

namespace Riskeer.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class CombinedAssemblyFailureMechanismSectionFactoryTest
    {
        [Test]
        public void CreateInput_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(null, Enumerable.Empty<IFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateInput_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());

            // Call
            void Call() => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void CreateInput_WithAllFailureMechanisms_ReturnsInputCollection()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, assessmentSection.GetFailureMechanisms().Concat(assessmentSection.SpecificFailureMechanisms));

                // Assert
                int expectedNrOfGeneralSectionResults = assessmentSection.GetFailureMechanisms().Count();
                int expectedNrOfSpecificSectionResults = assessmentSection.SpecificFailureMechanisms.Count;
                Assert.AreEqual(expectedNrOfGeneralSectionResults + expectedNrOfSpecificSectionResults, inputs.Count());

                FailureMechanismSectionAssemblyResult assemblyResult = calculator.FailureMechanismSectionAssemblyResultOutput.AssemblyResult; 
                AssertSectionsWithResult(assessmentSection.Piping.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(0));
                AssertSectionsWithResult(assessmentSection.GrassCoverErosionInwards.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(1));
                AssertSectionsWithResult(assessmentSection.MacroStabilityInwards.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(2));
                AssertSectionsWithResult(assessmentSection.Microstability.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(3));
                AssertSectionsWithResult(assessmentSection.StabilityStoneCover.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(4));
                AssertSectionsWithResult(assessmentSection.WaveImpactAsphaltCover.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(5));
                AssertSectionsWithResult(assessmentSection.WaterPressureAsphaltCover.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(6));
                AssertSectionsWithResult(assessmentSection.GrassCoverErosionOutwards.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(7));
                AssertSectionsWithResult(assessmentSection.GrassCoverSlipOffOutwards.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(8));
                AssertSectionsWithResult(assessmentSection.GrassCoverSlipOffInwards.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(9));
                AssertSectionsWithResult(assessmentSection.HeightStructures.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(10));
                AssertSectionsWithResult(assessmentSection.ClosingStructures.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(11));
                AssertSectionsWithResult(assessmentSection.PipingStructure.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(12));
                AssertSectionsWithResult(assessmentSection.StabilityPointStructures.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(13));
                AssertSectionsWithResult(assessmentSection.DuneErosion.SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup, inputs.ElementAt(14));
                for (var i = 0; i < expectedNrOfSpecificSectionResults; i++)
                {
                    AssertSectionsWithResult(assessmentSection.SpecificFailureMechanisms[i].SectionResults, assemblyResult.FailureMechanismSectionAssemblyGroup,
                                             inputs.ElementAt(expectedNrOfGeneralSectionResults + i));
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismTestCaseData))]
        public void CreateInput_WithOneFailureMechanism_ReturnsInputCollection(AssessmentSection assessmentSection, IFailureMechanism failureMechanismInAssembly)
        {
            // Setup
            assessmentSection.GetFailureMechanisms()
                             .ForEachElementDo(failureMechanism => failureMechanism.InAssembly = failureMechanism == failureMechanismInAssembly);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, new[]
                    {
                        failureMechanismInAssembly
                    });

                // Assert
                IObservableEnumerable<FailureMechanismSectionResult> failureMechanismSectionResults = ((IFailureMechanism<FailureMechanismSectionResult>) failureMechanismInAssembly).SectionResults;
                AssertSections(failureMechanismSectionResults, inputs.Single());
            }
        }

        [Test]
        public void CreateInput_WithAllFailureMechanismsAndCalculatorThrowsException_ReturnsInputCollection()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, assessmentSection.GetFailureMechanisms().Concat(assessmentSection.SpecificFailureMechanisms));

                // Assert
                int expectedNrOfGeneralSectionResults = assessmentSection.GetFailureMechanisms().Count();
                int expectedNrOfSpecificSectionResults = assessmentSection.SpecificFailureMechanisms.Count;
                Assert.AreEqual(expectedNrOfGeneralSectionResults + expectedNrOfSpecificSectionResults, inputs.Count());

                AssertSectionsWithResult(assessmentSection.Piping.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(0));
                AssertSectionsWithResult(assessmentSection.GrassCoverErosionInwards.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(1));
                AssertSectionsWithResult(assessmentSection.MacroStabilityInwards.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(2));
                AssertSectionsWithResult(assessmentSection.Microstability.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(3));
                AssertSectionsWithResult(assessmentSection.StabilityStoneCover.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(4));
                AssertSectionsWithResult(assessmentSection.WaveImpactAsphaltCover.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(5));
                AssertSectionsWithResult(assessmentSection.WaterPressureAsphaltCover.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(6));
                AssertSectionsWithResult(assessmentSection.GrassCoverErosionOutwards.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(7));
                AssertSectionsWithResult(assessmentSection.GrassCoverSlipOffOutwards.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(8));
                AssertSectionsWithResult(assessmentSection.GrassCoverSlipOffInwards.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(9));
                AssertSectionsWithResult(assessmentSection.HeightStructures.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(10));
                AssertSectionsWithResult(assessmentSection.ClosingStructures.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(11));
                AssertSectionsWithResult(assessmentSection.PipingStructure.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(12));
                AssertSectionsWithResult(assessmentSection.StabilityPointStructures.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(13));
                AssertSectionsWithResult(assessmentSection.DuneErosion.SectionResults, FailureMechanismSectionAssemblyGroup.NoResult, inputs.ElementAt(14));
                for (var i = 0; i < expectedNrOfSpecificSectionResults; i++)
                {
                    AssertSectionsWithResult(assessmentSection.SpecificFailureMechanisms[i].SectionResults, FailureMechanismSectionAssemblyGroup.NoResult,
                                             inputs.ElementAt(expectedNrOfGeneralSectionResults + i));
                }
            }
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismTestCaseData()
        {
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            yield return new TestCaseData(assessmentSection, assessmentSection.Piping);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverErosionInwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.MacroStabilityInwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.Microstability);
            yield return new TestCaseData(assessmentSection, assessmentSection.StabilityStoneCover);
            yield return new TestCaseData(assessmentSection, assessmentSection.WaveImpactAsphaltCover);
            yield return new TestCaseData(assessmentSection, assessmentSection.WaterPressureAsphaltCover);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverErosionOutwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverSlipOffOutwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverSlipOffInwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.HeightStructures);
            yield return new TestCaseData(assessmentSection, assessmentSection.ClosingStructures);
            yield return new TestCaseData(assessmentSection, assessmentSection.PipingStructure);
            yield return new TestCaseData(assessmentSection, assessmentSection.StabilityPointStructures);
            yield return new TestCaseData(assessmentSection, assessmentSection.DuneErosion);
            foreach (SpecificFailureMechanism specificFailureMechanism in assessmentSection.SpecificFailureMechanisms)
            {
                yield return new TestCaseData(assessmentSection, specificFailureMechanism);
            }
        }

        private static void AssertSectionsWithResult(IEnumerable<FailureMechanismSectionResult> originalSectionResults,
                                                     FailureMechanismSectionAssemblyGroup expectedAssemblyGroupInput,
                                                     IEnumerable<CombinedAssemblyFailureMechanismSection> inputSections)
        {
            AssertSections(originalSectionResults, inputSections);

            for (var i = 0; i < originalSectionResults.Count(); i++)
            {
                Assert.AreEqual(expectedAssemblyGroupInput, inputSections.ElementAt(i).FailureMechanismSectionAssemblyGroup);
            }
        }

        private static void AssertSections(IEnumerable<FailureMechanismSectionResult> originalSectionResults,
                                           IEnumerable<CombinedAssemblyFailureMechanismSection> inputSections)
        {
            Assert.AreEqual(originalSectionResults.Count(), inputSections.Count());

            double expectedSectionStart = 0;

            for (var i = 0; i < originalSectionResults.Count(); i++)
            {
                double expectedSectionEnd = expectedSectionStart + originalSectionResults.ElementAt(i).Section.Length;

                Assert.AreEqual(expectedSectionStart, inputSections.ElementAt(i).SectionStart);
                Assert.AreEqual(expectedSectionEnd, inputSections.ElementAt(i).SectionEnd);

                expectedSectionStart = expectedSectionEnd;
            }
        }
    }
}