﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.TestUtil;
using Riskeer.StabilityPointStructures.Data;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableStabilityPointStructuresFailureMechanismFactoryTest
    {
        [Test]
        public void CreateExportableFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableFailureMechanism(
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
            TestDelegate call = () => ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableFailureMechanism(
                new StabilityPointStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismInAssemblyFalse_ReturnsDefaultExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                InAssembly = false
            };
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            var assessmentSection = new AssessmentSectionStub
            {
                ReferenceLine = ReferenceLineTestFactory.CreateReferenceLineWithGeometry()
            };

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

            // Assert
            ExportableFailureMechanismTestHelper.AssertDefaultFailureMechanismWithProbability(assessmentSection.ReferenceLine.Points,
                                                                                              ExportableFailureMechanismType.STKWp,
                                                                                              ExportableFailureMechanismGroup.Group1,
                                                                                              ExportableAssemblyMethod.WBI1B1,
                                                                                              exportableFailureMechanism);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismInAssemblyTrue_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorOldStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                    ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(ExportableFailureMechanismType.STKWp, exportableFailureMechanism.Code);
                Assert.AreEqual(ExportableFailureMechanismGroup.Group1, exportableFailureMechanism.Group);

                FailureMechanismAssembly calculatorOutput = failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput;
                ExportableFailureMechanismAssemblyResultWithProbability exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
                Assert.AreEqual(calculatorOutput.Group, exportableFailureMechanismAssembly.AssemblyCategory);
                Assert.AreEqual(calculatorOutput.Probability, exportableFailureMechanismAssembly.Probability);
                Assert.AreEqual(ExportableAssemblyMethod.WBI1B1, exportableFailureMechanismAssembly.AssemblyMethod);

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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            failureMechanism.SectionResultsOld.Single().UseManualAssembly = true;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, new AssessmentSectionStub());

                // Assert
                Assert.AreSame(failureMechanismSectionAssemblyCalculator.CombinedAssemblyOutput,
                               failureMechanismAssemblyCalculator.FailureMechanismSectionAssemblies.Single());
            }
        }

        private static void AssertExportableFailureMechanismSectionResults(FailureMechanismSectionAssemblyOld expectedSimpleAssembly,
                                                                           FailureMechanismSectionAssemblyOld expectedDetailedAssembly,
                                                                           FailureMechanismSectionAssemblyOld expectedTailorMadeAssembly,
                                                                           FailureMechanismSectionAssemblyOld expectedCombinedAssembly,
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

        private static void AssertExportableFailureMechanismSectionResult(FailureMechanismSectionAssemblyOld expectedSimpleAssembly,
                                                                          FailureMechanismSectionAssemblyOld expectedDetailedAssembly,
                                                                          FailureMechanismSectionAssemblyOld expectedTailorMadeAssembly,
                                                                          FailureMechanismSectionAssemblyOld expectedCombinedAssembly,
                                                                          ExportableFailureMechanismSection expectedSection,
                                                                          ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability actualResult)
        {
            Assert.AreSame(expectedSection, actualResult.FailureMechanismSection);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSimpleAssembly,
                                                                                            ExportableAssemblyMethod.WBI0E3,
                                                                                            actualResult.SimpleAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedDetailedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0G3,
                                                                                            actualResult.DetailedAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedTailorMadeAssembly,
                                                                                            ExportableAssemblyMethod.WBI0T3,
                                                                                            actualResult.TailorMadeAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedCombinedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0A1,
                                                                                            actualResult.CombinedAssembly);
        }
    }
}