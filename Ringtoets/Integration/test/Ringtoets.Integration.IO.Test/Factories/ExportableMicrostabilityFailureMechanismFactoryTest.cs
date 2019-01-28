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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;
using Ringtoets.Integration.IO.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableMicrostabilityFailureMechanismFactoryTest
    {
        [Test]
        public void CreateExportableFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => ExportableMicrostabilityFailureMechanismFactory.CreateExportableFailureMechanism(
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
            TestDelegate call = () => ExportableMicrostabilityFailureMechanismFactory.CreateExportableFailureMechanism(
                new MicrostabilityFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismNotRelevant_ReturnsDefaultExportableFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanism = new MicrostabilityFailureMechanism
            {
                IsRelevant = false
            };
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableFailureMechanism =
                ExportableMicrostabilityFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

            // Assert
            ExportableFailureMechanismTestHelper.AssertDefaultFailureMechanismWithoutProbability(assessmentSection.ReferenceLine.Points,
                                                                                                 ExportableFailureMechanismType.STMI,
                                                                                                 ExportableFailureMechanismGroup.Group4,
                                                                                                 ExportableAssemblyMethod.WBI1A1,
                                                                                                 exportableFailureMechanism);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismRelevant_ReturnsExportableFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanism = new MicrostabilityFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableFailureMechanism =
                    ExportableMicrostabilityFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(ExportableFailureMechanismType.STMI, exportableFailureMechanism.Code);
                Assert.AreEqual(ExportableFailureMechanismGroup.Group4, exportableFailureMechanism.Group);

                ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
                Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput, exportableFailureMechanismAssembly.AssemblyCategory);
                Assert.AreEqual(ExportableAssemblyMethod.WBI1A1, exportableFailureMechanismAssembly.AssemblyMethod);

                IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections = exportableFailureMechanism.SectionAssemblyResults
                                                                                                                              .Select(sar => sar.FailureMechanismSection);
                ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanismSections);
                AssertExportableFailureMechanismSectionResults(failureMechanismSectionAssemblyCalculator.SimpleAssessmentAssemblyOutput.Group,
                                                               failureMechanismSectionAssemblyCalculator.DetailedAssessmentAssemblyGroupOutput.Value,
                                                               failureMechanismSectionAssemblyCalculator.TailorMadeAssemblyCategoryOutput.Value,
                                                               failureMechanismSectionAssemblyCalculator.CombinedAssemblyCategoryOutput.Value,
                                                               exportableFailureMechanismSections,
                                                               exportableFailureMechanism.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResult>());
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismWithManualAssessment_ManualAssemblyIgnored()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MicrostabilityFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                ExportableMicrostabilityFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(failureMechanismSectionAssemblyCalculator.CombinedAssemblyCategoryOutput,
                                failureMechanismAssemblyCalculator.FailureMechanismSectionCategories.Single());
            }

            mocks.VerifyAll();
        }

        private static void AssertExportableFailureMechanismSectionResults(FailureMechanismSectionAssemblyCategoryGroup expectedSimpleAssembly,
                                                                           FailureMechanismSectionAssemblyCategoryGroup expectedDetailedAssembly,
                                                                           FailureMechanismSectionAssemblyCategoryGroup expectedTailorMadeAssembly,
                                                                           FailureMechanismSectionAssemblyCategoryGroup expectedCombinedAssembly,
                                                                           IEnumerable<ExportableFailureMechanismSection> sections,
                                                                           IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResult> results)
        {
            int expectedNrOfResults = sections.Count();
            Assert.AreEqual(expectedNrOfResults, results.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                ExportableFailureMechanismSection section = sections.ElementAt(i);
                ExportableAggregatedFailureMechanismSectionAssemblyResult result = results.ElementAt(i);

                AssertExportableFailureMechanismSectionResult(expectedSimpleAssembly,
                                                              expectedDetailedAssembly,
                                                              expectedTailorMadeAssembly,
                                                              expectedCombinedAssembly,
                                                              section,
                                                              result);
            }
        }

        private static void AssertExportableFailureMechanismSectionResult(FailureMechanismSectionAssemblyCategoryGroup expectedSimpleAssembly,
                                                                          FailureMechanismSectionAssemblyCategoryGroup expectedDetailedAssembly,
                                                                          FailureMechanismSectionAssemblyCategoryGroup expectedTailorMadeAssembly,
                                                                          FailureMechanismSectionAssemblyCategoryGroup expectedCombinedAssembly,
                                                                          ExportableFailureMechanismSection expectedSection,
                                                                          ExportableAggregatedFailureMechanismSectionAssemblyResult actualResult)
        {
            Assert.AreSame(expectedSection, actualResult.FailureMechanismSection);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSimpleAssembly,
                                                                                            ExportableAssemblyMethod.WBI0E1,
                                                                                            actualResult.SimpleAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedDetailedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0G1,
                                                                                            actualResult.DetailedAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedTailorMadeAssembly,
                                                                                            ExportableAssemblyMethod.WBI0T1,
                                                                                            actualResult.TailorMadeAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedCombinedAssembly,
                                                                                            ExportableAssemblyMethod.WBI0A1,
                                                                                            actualResult.CombinedAssembly);
        }
    }
}