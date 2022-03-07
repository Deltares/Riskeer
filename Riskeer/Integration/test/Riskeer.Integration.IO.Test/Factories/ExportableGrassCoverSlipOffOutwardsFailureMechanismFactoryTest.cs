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
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableGrassCoverSlipOffOutwardsFailureMechanismFactoryTest
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
            void Call() => ExportableGrassCoverSlipOffOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportableFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableGrassCoverSlipOffOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(new GrassCoverSlipOffOutwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableFailureMechanism_WithValidData_ReturnsExportableFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(2, 10));

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                ExportableFailureMechanism exportableFailureMechanism =
                    ExportableGrassCoverSlipOffOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(ExportableFailureMechanismType.GABU, exportableFailureMechanism.Code);

                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssembly = exportableFailureMechanism.FailureMechanismAssembly;
                Assert.AreEqual(failureMechanismAssemblyCalculator.AssemblyResult, exportableFailureMechanismAssembly.Probability);
                Assert.IsFalse(exportableFailureMechanismAssembly.IsManual);

                IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections = exportableFailureMechanism.SectionAssemblyResults.Select(sar => sar.FailureMechanismSection);
                ExportableFailureMechanismSectionTestHelper.AssertExportableFailureMechanismSections(failureMechanism.Sections, exportableFailureMechanismSections);
                AssertExportableFailureMechanismSectionResults(exportableFailureMechanismSections,
                                                               exportableFailureMechanism.SectionAssemblyResults.Cast<ExportableAggregatedFailureMechanismSectionAssemblyResult>());
            }
        }

        private static void AssertExportableFailureMechanismSectionResults(IEnumerable<ExportableFailureMechanismSection> sections,
                                                                           IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResult> results)
        {
            int expectedNrOfResults = sections.Count();
            Assert.AreEqual(expectedNrOfResults, results.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                ExportableFailureMechanismSection section = sections.ElementAt(i);
                ExportableAggregatedFailureMechanismSectionAssemblyResult result = results.ElementAt(i);

                AssertExportableFailureMechanismSectionResult(section,
                                                              result);
            }
        }

        private static void AssertExportableFailureMechanismSectionResult(ExportableFailureMechanismSection expectedSection,
                                                                          ExportableAggregatedFailureMechanismSectionAssemblyResult actualResult)
        {
            Assert.AreSame(expectedSection, actualResult.FailureMechanismSection);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSectionAssemblyCategoryGroup,
                                                                                            ExportableAssemblyMethod.WBI0E1,
                                                                                            actualResult.SimpleAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSectionAssemblyCategoryGroup,
                                                                                            ExportableAssemblyMethod.WBI0G1,
                                                                                            actualResult.DetailedAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSectionAssemblyCategoryGroup,
                                                                                            ExportableAssemblyMethod.WBI0T1,
                                                                                            actualResult.TailorMadeAssembly);

            ExportableSectionAssemblyResultTestHelper.AssertExportableSectionAssemblyResult(expectedSectionAssemblyCategoryGroup,
                                                                                            ExportableAssemblyMethod.WBI0A1,
                                                                                            actualResult.CombinedAssembly);
        }
    }
}