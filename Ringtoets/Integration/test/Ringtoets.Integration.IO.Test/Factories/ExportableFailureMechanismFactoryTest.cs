// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableFailureMechanismFactoryTest
    {
        [Test]
        public void CreateDefaultExportableFailureMechanismWithProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            var random = new Random(21);
            var group = random.NextEnumValue<ExportableFailureMechanismGroup>();
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var failureMechanismAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            TestDelegate call = () => ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithProbability(null,
                                                                                                                               failureMechanismCode,
                                                                                                                               group,
                                                                                                                               failureMechanismAssemblyMethod);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateDefaultExportableFailureMechanismWithProbability_WithValidArguments_ReturnsExportableFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ReferenceLine referenceLine = ReferenceLineTestFactory.CreateReferenceLineWithGeometry();
            assessmentSection.ReferenceLine = referenceLine;

            var random = new Random(21);
            var group = random.NextEnumValue<ExportableFailureMechanismGroup>();
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var failureMechanismAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithProbability(assessmentSection,
                                                                                                         failureMechanismCode,
                                                                                                         group,
                                                                                                         failureMechanismAssemblyMethod);

            // Assert
            Assert.AreEqual(group, exportableFailureMechanism.Group);
            Assert.AreEqual(failureMechanismCode, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyResult = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(failureMechanismAssemblyMethod, failureMechanismAssemblyResult.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, failureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(0, failureMechanismAssemblyResult.Probability);

            var exportableFailureMechanismSectionAssembly =
                (ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult) exportableFailureMechanism.SectionAssemblyResults.Single();
            ExportableSectionAssemblyResultWithProbability combinedAssembly = exportableFailureMechanismSectionAssembly.CombinedAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI0A1, combinedAssembly.AssemblyMethod);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, combinedAssembly.AssemblyCategory);
            Assert.AreEqual(0, combinedAssembly.Probability);

            ExportableFailureMechanismSection failureMechanismSection = exportableFailureMechanismSectionAssembly.FailureMechanismSection;
            Assert.AreSame(referenceLine.Points, failureMechanismSection.Geometry);
            Assert.AreEqual(0, failureMechanismSection.StartDistance);
            Assert.AreEqual(referenceLine.Length, failureMechanismSection.EndDistance);
        }

        [Test]
        public void CreateDefaultExportableFailureMechanismWithoutProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            var random = new Random(21);
            var group = random.NextEnumValue<ExportableFailureMechanismGroup>();
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var failureMechanismAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            TestDelegate call = () => ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithoutProbability(null, 
                                                                                                                                  failureMechanismCode,
                                                                                                                                  group,
                                                                                                                                  failureMechanismAssemblyMethod);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateDefaultExportableFailureMechanismWithoutProbability_WithValidArguments_ReturnsExportableFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            ReferenceLine referenceLine = ReferenceLineTestFactory.CreateReferenceLineWithGeometry();
            assessmentSection.ReferenceLine = referenceLine;

            var random = new Random(21);
            var group = random.NextEnumValue<ExportableFailureMechanismGroup>();
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableFailureMechanism =
                ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithoutProbability(assessmentSection, 
                                                                                                            failureMechanismCode, 
                                                                                                            group, 
                                                                                                            assemblyMethod);

            // Assert
            Assert.AreEqual(group, exportableFailureMechanism.Group);
            Assert.AreEqual(failureMechanismCode, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResult failureMechanismAssemblyResult = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(assemblyMethod, failureMechanismAssemblyResult.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, failureMechanismAssemblyResult.AssemblyCategory);

            var exportableFailureMechanismSectionAssembly =
                (ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult) exportableFailureMechanism.SectionAssemblyResults.Single();
            ExportableSectionAssemblyResult combinedAssembly = exportableFailureMechanismSectionAssembly.CombinedAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI0A1, combinedAssembly.AssemblyMethod);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, combinedAssembly.AssemblyCategory);

            ExportableFailureMechanismSection failureMechanismSection = exportableFailureMechanismSectionAssembly.FailureMechanismSection;
            Assert.AreSame(referenceLine.Points, failureMechanismSection.Geometry);
            Assert.AreEqual(0, failureMechanismSection.StartDistance);
            Assert.AreEqual(referenceLine.Length, failureMechanismSection.EndDistance);
        }
    }
}