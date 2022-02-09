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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.TestUtil;
using Riskeer.Piping.Data;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportablePipingFailureMechanismFactoryTest
    {
        private static readonly FailureMechanismSectionAssemblyOld expectedAssemblyResult = new FailureMechanismSectionAssemblyOld(0, FailureMechanismSectionAssemblyCategoryGroup.None);
        
        [Test]
        public void CreateExportableFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => ExportablePipingFailureMechanismFactory.CreateExportableFailureMechanism(
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
            TestDelegate call = () => ExportablePipingFailureMechanismFactory.CreateExportableFailureMechanism(
                new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismInAssemblyFalse_ReturnsDefaultExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new PipingFailureMechanism
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
                ExportablePipingFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

            // Assert
            ExportableFailureMechanismTestHelper.AssertDefaultFailureMechanismWithProbability(assessmentSection.ReferenceLine.Points,
                                                                                              ExportableFailureMechanismType.STPH,
                                                                                              ExportableFailureMechanismGroup.Group2,
                                                                                              ExportableAssemblyMethod.WBI1B1,
                                                                                              exportableFailureMechanism);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismInAssemblyTrue_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            var assessmentSection = new AssessmentSectionStub();

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                ExportablePipingFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(ExportableFailureMechanismType.STPH, exportableFailureMechanism.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group2, exportableFailureMechanism.Group);

            ExportableFailureMechanismAssemblyResultWithProbability exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.None, exportableFailureMechanismAssembly.AssemblyCategory);
            Assert.AreEqual(0, exportableFailureMechanismAssembly.Probability);
            Assert.AreEqual(ExportableAssemblyMethod.WBI1B1, exportableFailureMechanismAssembly.AssemblyMethod);

            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections = exportableFailureMechanism.SectionAssemblyResults
                                                                                                                          .Select(sar => sar.FailureMechanismSection);
            ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanismSections);
            AssertExportableFailureMechanismSectionResults(exportableFailureMechanismSections,
                                                           exportableFailureMechanism.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>());
        }

        private static void AssertExportableFailureMechanismSectionResults(IEnumerable<ExportableFailureMechanismSection> sections,
                                                                           IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability> results)
        {
            int expectedNrOfResults = sections.Count();
            Assert.AreEqual(expectedNrOfResults, results.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                ExportableFailureMechanismSection section = sections.ElementAt(i);
                ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability result = results.ElementAt(i);

                AssertExportableFailureMechanismSectionResult(section,
                                                              result);
            }
        }

        private static void AssertExportableFailureMechanismSectionResult(ExportableFailureMechanismSection expectedSection,
                                                                          ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability actualResult)
        {
            Assert.AreSame(expectedSection, actualResult.FailureMechanismSection);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedAssemblyResult,
                                                                                            ExportableAssemblyMethod.WBI0E1,
                                                                                            actualResult.SimpleAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedAssemblyResult,
                                                                                            ExportableAssemblyMethod.WBI0G5,
                                                                                            actualResult.DetailedAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedAssemblyResult,
                                                                                            ExportableAssemblyMethod.WBI0T5,
                                                                                            actualResult.TailorMadeAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedAssemblyResult,
                                                                                            ExportableAssemblyMethod.WBI0A1,
                                                                                            actualResult.CombinedAssembly);
        }
    }
}