// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableMacroStabilityInwardsFailureMechanismFactoryTest
    {
        [Test]
        public void CreateExportableFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableFailureMechanism(
                null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableFailureMechanism(
                new MacroStabilityInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismNotRelevant_ReturnsDefaultExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                IsRelevant = false
            };
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = ReferenceLineTestFactory.CreateReferenceLineWithGeometry()
            };

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

            // Assert
            ExportableFailureMechanismTestHelper.AssertDefaultFailureMechanismWithProbability(assessmentSection.ReferenceLine.Points,
                                                                                              ExportableFailureMechanismType.STBI,
                                                                                              ExportableFailureMechanismGroup.Group2,
                                                                                              ExportableAssemblyMethod.WBI1B1,
                                                                                              exportableFailureMechanism);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismRelevant_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                    ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(ExportableFailureMechanismType.STBI, exportableFailureMechanism.Code);
                Assert.AreEqual(ExportableFailureMechanismGroup.Group2, exportableFailureMechanism.Group);

                FailureMechanismAssembly calculatorOutput = failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput;
                ExportableFailureMechanismAssemblyResultWithProbability exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
                Assert.AreEqual(ExportableAssemblyMethod.WBI1B1, exportableFailureMechanismAssembly.AssemblyMethod);
                Assert.AreEqual(calculatorOutput.Group, exportableFailureMechanismAssembly.AssemblyCategory);
                Assert.AreEqual(calculatorOutput.Probability, exportableFailureMechanismAssembly.Probability);

                IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections = exportableFailureMechanism.SectionAssemblyResults
                                                                                                                              .Select(sar => sar.FailureMechanismSection);
                ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanismSections);
                AssertExportableFailureMechanismSectionResults(failureMechanismSectionAssemblyCalculator.SimpleAssessmentAssemblyOutput,
                                                               failureMechanismSectionAssemblyCalculator.DetailedAssessmentAssemblyOutput,
                                                               failureMechanismSectionAssemblyCalculator.TailorMadeAssessmentAssemblyOutput,
                                                               failureMechanismSectionAssemblyCalculator.CombinedAssemblyOutput,
                                                               exportableFailureMechanismSections,
                                                               exportableFailureMechanism.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>());
            }
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismWithManualAssessment_ManualAssemblyIgnored()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            failureMechanism.SectionResults.Single().UseManualAssembly = true;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, new AssessmentSectionStub());

                // Assert
                Assert.AreSame(failureMechanismSectionAssemblyCalculator.CombinedAssemblyOutput,
                               failureMechanismAssemblyCalculator.FailureMechanismSectionAssemblies.Single());
            }
        }

        private static void AssertExportableFailureMechanismSectionResults(FailureMechanismSectionAssembly expectedSimpleAssembly,
                                                                           FailureMechanismSectionAssembly expectedDetailedAssembly,
                                                                           FailureMechanismSectionAssembly expectedTailorMadeAssembly,
                                                                           FailureMechanismSectionAssembly expectedCombinedAssembly,
                                                                           IEnumerable<ExportableFailureMechanismSection> sections,
                                                                           IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability> results)
        {
            int expectedNrOfResults = sections.Count();
            Assert.AreEqual(expectedNrOfResults, results.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                ExportableFailureMechanismSection section = sections.ElementAt(i);
                ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability result = results.ElementAt(i);

                AssertExportableFailureMechanismSectionResult(expectedSimpleAssembly,
                                                              expectedDetailedAssembly,
                                                              expectedTailorMadeAssembly,
                                                              expectedCombinedAssembly,
                                                              section,
                                                              result);
            }
        }

        private static void AssertExportableFailureMechanismSectionResult(FailureMechanismSectionAssembly expectedSimpleAssembly,
                                                                          FailureMechanismSectionAssembly expectedDetailedAssembly,
                                                                          FailureMechanismSectionAssembly expectedTailorMadeAssembly,
                                                                          FailureMechanismSectionAssembly expectedCombinedAssembly,
                                                                          ExportableFailureMechanismSection expectedSection,
                                                                          ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability actualResult)
        {
            Assert.AreSame(expectedSection, actualResult.FailureMechanismSection);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSimpleAssembly,
                                                                                            ExportableAssemblyMethod.WBI0E1,
                                                                                            actualResult.SimpleAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedDetailedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0G5,
                                                                                            actualResult.DetailedAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedTailorMadeAssembly,
                                                                                            ExportableAssemblyMethod.WBI0T5,
                                                                                            actualResult.TailorMadeAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedCombinedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0A1,
                                                                                            actualResult.CombinedAssembly);
        }
    }
}