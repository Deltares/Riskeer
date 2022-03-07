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
using Riskeer.AssemblyTool.Data.Old;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableWaterPressureAsphaltCoverFailureMechanismFactoryTest
    {
        private const FailureMechanismSectionAssemblyCategoryGroup expectedSectionAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
        
        [Test]
        public void CreateExportableFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(new WaterPressureAsphaltCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismInAssemblyFalse_ReturnsDefaultExportableFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism
            {
                InAssembly = false
            };
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            // Call
            ExportableFailureMechanism exportableFailureMechanism =
                ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

            // Assert
            ExportableFailureMechanismTestHelper.AssertDefaultFailureMechanismWithoutProbability(assessmentSection.ReferenceLine.Points,
                                                                                                 ExportableFailureMechanismType.AWO,
                                                                                                 ExportableAssemblyMethod.WBI1A1,
                                                                                                 exportableFailureMechanism);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableFailureMechanism_WithFailureMechanismInAssemblyTrue_ReturnsExportableFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            // Call
            ExportableFailureMechanism exportableFailureMechanism =
                ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(ExportableFailureMechanismType.AWO, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.None, exportableFailureMechanismAssembly.AssemblyGroup);
            Assert.AreEqual(ExportableAssemblyMethod.WBI1A1, exportableFailureMechanismAssembly.AssemblyMethod);

            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections = exportableFailureMechanism.SectionAssemblyResults.Select(sar => sar.FailureMechanismSection);
            ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanismSections);
            AssertExportableFailureMechanismSectionResults(exportableFailureMechanismSections,
                                                           exportableFailureMechanism.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly>());

            mocks.VerifyAll();
        }

        private static void AssertExportableFailureMechanismSectionResults(IEnumerable<ExportableFailureMechanismSection> sections,
                                                                           IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly> results)
        {
            int expectedNrOfResults = sections.Count();
            Assert.AreEqual(expectedNrOfResults, results.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                ExportableFailureMechanismSection section = sections.ElementAt(i);
                ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly result = results.ElementAt(i);

                AssertExportableFailureMechanismSectionResult(section,
                                                              result);
            }
        }

        private static void AssertExportableFailureMechanismSectionResult(ExportableFailureMechanismSection expectedSection,
                                                                          ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly actualResult)
        {
            Assert.AreSame(expectedSection, actualResult.FailureMechanismSection);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSectionAssemblyCategoryGroup,
                                                                                            ExportableAssemblyMethod.WBI0E1,
                                                                                            actualResult.SimpleAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSectionAssemblyCategoryGroup,
                                                                                            ExportableAssemblyMethod.WBI0T1,
                                                                                            actualResult.TailorMadeAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSectionAssemblyCategoryGroup,
                                                                                            ExportableAssemblyMethod.WBI0A1,
                                                                                            actualResult.CombinedAssembly);
        }
    }
}